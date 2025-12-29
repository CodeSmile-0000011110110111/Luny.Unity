using Luny.Unity;
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
					typeof(UnityLifecycleAdapter).FullName,
				},
			});

			return details.ToArray();
		}

		public override String GetAssemblyName() => $"{nameof(Luny)}.Unity";
	}
}
