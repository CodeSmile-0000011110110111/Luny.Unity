using Luny.Unity;
using Luny.Unity.Providers;
using System;

namespace LunyEditor
{
	internal sealed class PreventCodeStrippingLinkerProcessor : LunyLinkerProcessor
	{
		public override PreserveDetails[] GetPreserveDetails() => new[]
		{
			//new PreserveDetails { Assembly = nameof(Luny) },
			new PreserveDetails
			{
				Assembly = $"{nameof(Luny)}.{nameof(Luny.Unity)}",
				Types = new[]
				{
					// lifecycle is using [RuntimeInitializeOnLoadMethod]
					typeof(UnityLifecycleAdapter).FullName,

					// API services are discovered through reflection
					typeof(UnityApplicationServiceProvider).FullName,
					typeof(UnitySceneServiceProvider).FullName,
					typeof(UnityTimeServiceProvider).FullName,
				},
			},
		};

		public override String GetLinkFilename() => nameof(Luny);
	}
}
