using Luny.Engine.Services;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of Debug.
	/// </summary>
	public sealed class LunyUnityEditorService : LunyEditorServiceBase, ILunyEditorService
	{
		public void PausePlayer() => Debug.Break();
		protected override void OnServiceInitialize() {}

		protected override void OnServiceStartup() {}

		protected override void OnServiceShutdown() {}
	}
}
