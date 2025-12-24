using Luny.Services;
using UnityEngine;

namespace Luny.Unity.Services
{
	/// <summary>
	/// Unity implementation of Debug.
	/// </summary>
	public sealed class UnityEditorService : IEditorService
	{
		public void PausePlayer() => Debug.Break();
	}
}
