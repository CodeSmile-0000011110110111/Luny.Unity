using Luny.Services;
using System;
using UnityEngine;

namespace Luny.Unity.Services
{
	/// <summary>
	/// Unity implementation of application control.
	/// </summary>
	public sealed partial class UnityApplicationService : ApplicationServiceBase, IApplicationService
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
