using Luny.Engine.Bridge;
using System;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class UnityPath : LunyPath
	{
		public static implicit operator UnityPath(String enginePath) => new(enginePath);

		public UnityPath(String nativePath)
			: base(nativePath) {}

		// Unity paths are fine, provided they are not absolute paths
		protected override String ToEngineAgnosticPath(String nativePath) => nativePath;
	}
}
