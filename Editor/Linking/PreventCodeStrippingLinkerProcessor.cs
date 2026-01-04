using Luny.Engine;
using Luny.Unity;
using Luny.Unity.Engine;
using System;

namespace Luny.UnityEditor.Linking
{
	internal sealed class PreventCodeStrippingLinkerProcessor : LunyLinkerProcessor
	{
		public override PreserveDetails[] GetPreserveDetails()
		{
			// Engine services are discovered through reflection
			var details = PreserveAllDerivedClasses<IEngineService>();

			details.Add(new PreserveDetails
			{
				Assembly = $"{nameof(Luny)}.{nameof(Luny.Unity)}",
				Types = new[]
				{
					// lifecycle is using [RuntimeInitializeOnLoadMethod]
					typeof(LunyEngineUnityAdapter).FullName,
				},
			});

			return details.ToArray();
		}

		public override String GetAssemblyName() => $"{nameof(Luny)}.Unity";
	}
}
