using Luny.Engine.Bridge;
using System;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class UnityPath : LunyPath
	{
		public UnityPath(String path, Boolean isNative)
			: base(path, isNative) {}

		public static implicit operator UnityPath(String nativePath) => new(nativePath, true);
	}
}
