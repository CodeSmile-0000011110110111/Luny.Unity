using Luny.Services;
using System;
using UnityEngine;

namespace Luny.Unity.Services
{
	/// <summary>
	/// Unity implementation of Debug.
	/// </summary>
	public sealed class UnityDebugService : IDebugService
	{
		public void LogInfo(String message) => Debug.Log(message);
		public void LogWarning(String message) => Debug.LogWarning(message);
		public void LogError(String message) => Debug.LogError(message);
		public void LogException(Exception exception) => Debug.LogException(exception);
	}
}
