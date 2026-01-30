using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;

namespace Luny.UnityEditor.Linking
{
	public abstract class LunyLinkerProcessor : IUnityLinkerProcessor
	{
		public Int32 callbackOrder => Int32.MaxValue;

		protected static List<PreserveDetails> PreserveAllDerivedClasses<T>()
		{
			var details = new List<PreserveDetails>();

			var preserveScripts = new Dictionary<String, List<String>>();
			foreach (var scriptType in TypeDiscovery.FindAll<T>())
			{
				var assemblyName = scriptType.Assembly.GetName().Name;
				if (preserveScripts.ContainsKey(assemblyName) == false)
					preserveScripts[assemblyName] = new List<String>();

				preserveScripts[assemblyName].Add(scriptType.FullName);
			}

			foreach (var pair in preserveScripts)
				details.Add(new PreserveDetails { Assembly = pair.Key, Types = pair.Value.ToArray() });

			return details;
		}

		public String GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
		{
			var path = $"{Application.dataPath}/../Library/{GetAssemblyName()}_{nameof(LunyLinkerProcessor)}_link.xml";

			var details = GetPreserveDetails();
			if (details?.Length > 0)
			{
				var sb = new StringBuilder("<linker>\n");
				foreach (var detail in details)
				{
					if (String.IsNullOrEmpty(detail.Assembly))
						continue;

					var hasTypesToPreserve = detail.Types?.Length > 0;
					if (hasTypesToPreserve)
					{
						sb.AppendLine($"\t<assembly fullname=\"{detail.Assembly}\">");
						foreach (var type in detail.Types)
							sb.AppendLine($"\t\t<type fullname=\"{type}\" />");
						sb.AppendLine("\t</assembly>");
					}
					else
						sb.AppendLine($"\t<assembly fullname=\"{detail.Assembly}\" />");
				}
				sb.AppendLine("</linker>");

				try
				{
					File.WriteAllText(path, sb.ToString());
				}
				catch (Exception e)
				{
					Debug.LogError($"{nameof(LunyLinkerProcessor)} failed to write linker config: {path}\n{e}");
					throw;
				}
			}

			return path;
		}

		public abstract PreserveDetails[] GetPreserveDetails();
		public abstract String GetAssemblyName();

		public struct PreserveDetails
		{
			public String Assembly;
			public String[] Types;
		}
	}
}
