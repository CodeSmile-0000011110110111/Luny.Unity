using System;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity-specific implementation of the Luny logger that forwards to UnityEngine.Debug.
	/// </summary>
	public sealed class LunyUnityLogger : ILunyLogger
	{
		public void LogInfo(String message) => Debug.Log(message);
		public void LogWarning(String message) => Debug.LogWarning(message);
		public void LogError(String message) => Debug.LogError(message);
		public void LogException(Exception exception) => Debug.LogException(exception);
	}
}
