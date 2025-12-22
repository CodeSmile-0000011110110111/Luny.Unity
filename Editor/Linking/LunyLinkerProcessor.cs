using System;
using System.IO;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;

namespace LunyEditor
{
	public abstract class LunyLinkerProcessor : IUnityLinkerProcessor
	{
		public Int32 callbackOrder => Int32.MaxValue;

		public String GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
		{
			var path = $"{Application.dataPath}/../Library/{GetLinkFilename()}_{nameof(LunyLinkerProcessor)}_link.xml";

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
					Debug.LogException(e);
				}
			}

			return path;
		}

		public abstract PreserveDetails[] GetPreserveDetails();
		public abstract String GetLinkFilename();

		public struct PreserveDetails
		{
			public String Assembly;
			public String[] Types;
		}
	}
}
