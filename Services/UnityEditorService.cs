using Luny.Engine.Services;
using UnityEngine;

namespace Luny.Unity.Services
{
	/// <summary>
	/// Unity implementation of Debug.
	/// </summary>
	public sealed class UnityEditorService : LunyEditorServiceBase, ILunyEditorService
	{
		public override void PausePlayer() => Debug.Break();
		protected override void OnServiceInitialize() {}

		protected override void OnServiceStartup() {}

		protected override void OnServiceShutdown() {}
	}
}
