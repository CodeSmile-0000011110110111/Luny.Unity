using Luny.Engine.Bridge;
using System;
using UnityEngine.SceneManagement;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class UnityScene : LunyScene
	{
		public override String Name => Cast<Scene>().name;

		public UnityScene(Scene nativeScene)
			: base(nativeScene, new UnityPath(nativeScene.path)) {}
	}
}
