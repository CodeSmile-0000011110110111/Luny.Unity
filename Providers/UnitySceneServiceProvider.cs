using System;
using Luny.Providers;
using UnityEngine.SceneManagement;

namespace Luny.Unity.Providers
{
	/// <summary>
	/// Unity implementation of scene information provider.
	/// </summary>
	public sealed class UnitySceneServiceProvider : ISceneServiceProvider
	{
		public String CurrentSceneName => SceneManager.GetActiveScene().name;
	}
}
