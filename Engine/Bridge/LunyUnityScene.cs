using Luny.Engine.Bridge;
using System;
using UnityEngine.SceneManagement;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class LunyUnityScene : LunyScene
	{
		public LunyUnityScene(Scene nativeScene)
			: base(nativeScene, new LunyUnityPath(nativeScene.path)) {}

		public override String Name => Cast<Scene>().name;
	}
}
