using Luny.Interfaces.Providers;
using System;
using UnityEngine;

namespace Luny.Unity.Providers
{
	/// <summary>
	/// Unity implementation of application control provider.
	/// </summary>
	public sealed partial class UnityApplicationServiceProvider : IApplicationServiceProvider
	{
		public Boolean IsEditor => Application.isEditor;

		public Boolean IsPlaying => Application.isPlaying;

		public void Quit(Int32 exitCode = 0)
		{
			Application.Quit(exitCode);
			ExitPlayModeWhenInEditor();
		}
	}
}
