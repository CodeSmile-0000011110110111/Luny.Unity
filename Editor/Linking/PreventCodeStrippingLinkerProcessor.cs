using Luny.Engine;
using Luny.Engine.Services;
using Luny.Unity.Engine;
using System;

namespace Luny.UnityEditor.Linking
{
	internal sealed class PreventCodeStrippingLinkerProcessor : LunyLinkerProcessor
	{
		public override PreserveDetails[] GetPreserveDetails()
		{
			// Engine services are discovered through reflection
			var details = PreserveAllDerivedClasses<ILunyEngineService>();

			details.Add(new PreserveDetails
			{
				Assembly = $"{nameof(Luny)}.{nameof(Unity)}",
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
