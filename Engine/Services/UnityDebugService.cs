using Luny.Engine.Services;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of Debug.
	/// </summary>
	public sealed class UnityDebugService : LunyDebugServiceBase, ILunyDebugService
	{
		public void LogInfo(String message) => Debug.Log(message);
		public void LogWarning(String message) => Debug.LogWarning(message);
		public void LogError(String message) => Debug.LogError(message);
		public void LogException(Exception exception) => Debug.LogException(exception);
		protected override void OnServiceInitialize() {}
		protected override void OnServiceStartup() {}
		protected override void OnServiceShutdown() {}
	}
}
