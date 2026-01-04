using Luny.Engine.Services;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of Debug.
	/// </summary>
	public sealed class UnityEditorService : EditorServiceBase, IEditorService
	{
		public void PausePlayer() => Debug.Break();
	}
}
