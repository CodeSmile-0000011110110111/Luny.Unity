using System;
using UnityEngine;
using Object = System.Object;

namespace Luny.Unity.Bridge
{
	/// <summary>
	/// Unity-specific implementation of the Luny logger that forwards to UnityEngine.Debug.
	/// </summary>
	public sealed class UnityLogger : ILunyLogger
	{
		private const String Null = "<null>";
		public void LogInfo(Object obj) => Debug.Log(obj != null ? obj.ToString() : Null);
		public void LogWarning(Object obj) => Debug.LogWarning(obj != null ? obj.ToString() : Null);
		public void LogError(Object obj) => Debug.LogError(obj != null ? obj.ToString() : Null);
		public void LogInfo(String message) => Debug.Log(message);
		public void LogWarning(String message) => Debug.LogWarning(message);
		public void LogError(String message) => Debug.LogError(message);
		public void LogException(Exception exception) => Debug.LogException(exception);
	}
}
