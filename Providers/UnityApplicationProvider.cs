using System;
using Luny.Providers;
using UnityEngine;

namespace Luny.Unity.Providers
{
	/// <summary>
	/// Unity implementation of application control provider.
	/// </summary>
	public sealed class UnityApplicationProvider : IApplicationProvider
	{
		public void Quit(Int32 exitCode = 0)
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit(exitCode);
#endif
		}

		public Boolean IsEditor => Application.isEditor;

		public Boolean IsPlaying => Application.isPlaying;
	}
}
