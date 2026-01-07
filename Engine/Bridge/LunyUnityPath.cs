using Luny.Engine.Bridge;
using System;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class LunyUnityPath : LunyPath
	{
		public static implicit operator LunyUnityPath(String enginePath) => new(enginePath);

		public LunyUnityPath(String nativePath)
			: base(nativePath) {}

		// Unity paths are fine, provided they are not absolute paths
		protected override String ToEngineAgnosticPath(String nativePath) => nativePath;
	}
}
