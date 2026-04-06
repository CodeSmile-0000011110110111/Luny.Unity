using Luny.Engine.Services;
using System;

namespace Luny.UnityEditor
{
	internal sealed class PreventCodeStrippingLinkerProcessor : LunyLinkerProcessor
	{
		public override PreserveDetails[] GetPreserveDetails()
		{
			// Engine services are discovered through reflection
			var details = PreserveAllDerivedClasses<ILunyEngineService>();
			var observers = PreserveAllDerivedClasses<ILunyEngineObserver>();
			details.AddRange(observers);
			var adapters = PreserveAllDerivedClasses<ILunyEngineNativeAdapter>();
			details.AddRange(adapters);
			return details.ToArray();
		}

		public override String GetAssemblyName() => "Luny.Unity";
	}
}
