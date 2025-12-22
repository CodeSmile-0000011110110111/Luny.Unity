using Luny.Providers;
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Luny.Unity.Providers
{
	/// <summary>
	/// Unity implementation of application control provider.
	/// </summary>
	public sealed class UnityApplicationServiceProvider : IApplicationServiceProvider
	{
		public Boolean IsEditor => Application.isEditor;

		public Boolean IsPlaying => Application.isPlaying;

		public void Quit(Int32 exitCode = 0)
		{
			Application.Quit(exitCode);

#if UNITY_EDITOR
			if (IsEditor)
				EditorApplication.isPlaying = false;
#endif
		}
	}
}
