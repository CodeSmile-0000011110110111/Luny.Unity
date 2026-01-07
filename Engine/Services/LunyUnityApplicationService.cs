using Luny.Engine.Services;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of application control.
	/// </summary>
	public sealed partial class LunyUnityApplicationService : LunyApplicationServiceBase, ILunyApplicationService
	{
		public Boolean IsEditor => Application.isEditor;

		public Boolean IsPlaying => Application.isPlaying;

		public void Quit(Int32 exitCode = 0)
		{
			Application.Quit(exitCode);
			ExitPlayModeWhenInEditor();
		}

		protected override void OnServiceInitialize(){}
		protected override void OnServiceStartup(){}

		protected override void OnServiceShutdown() {}
	}
}
