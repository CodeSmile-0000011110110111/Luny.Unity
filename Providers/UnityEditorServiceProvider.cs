using Luny.Interfaces.Providers;
using UnityEngine;

namespace Luny.Unity.Providers
{
	/// <summary>
	/// Unity implementation of Debug provider.
	/// </summary>
	public sealed class UnityEditorServiceProvider : IEditorServiceProvider
	{
		public void PausePlayer() => Debug.Break();
	}
}
