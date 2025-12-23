using Luny.Diagnostics;
using System;
using UnityEngine;

namespace Luny.Unity.Diagnostics
{
	/// <summary>
	/// Unity-specific implementation of the Luny logger that forwards to UnityEngine.Debug.
	/// </summary>
	public sealed class UnityLogger : ILunyLogger
	{
		public void LogInfo(String message) => Debug.Log(message);
		public void LogWarning(String message) => Debug.LogWarning(message);
		public void LogError(String message) => Debug.LogError(message);
		public void LogException(Exception exception) => Debug.LogException(exception);
	}
}
