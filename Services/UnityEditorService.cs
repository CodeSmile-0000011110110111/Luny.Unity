using Luny.Services;
using System;
using UnityEngine;

namespace Luny.Unity.Services
{
	/// <summary>
	/// Unity implementation of Debug.
	/// </summary>
	public sealed class UnityEditorService : EditorServiceBase, IEditorService
	{
		public void PausePlayer() => Debug.Break();
	}
}
