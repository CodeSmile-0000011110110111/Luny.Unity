using Luny.Engine.Bridge;
using System;
using UnityEngine.SceneManagement;

namespace Luny.Unity.Bridge
{
	public sealed class UnityScene : LunyScene
	{
		public override String Name => Cast<Scene>().name;

		public UnityScene(Scene nativeScene)
			: base(nativeScene, LunyPath.FromNative(nativeScene.path)) {}
	}
}
