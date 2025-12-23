using Luny.Interfaces.Providers;
using System;
using UnityEngine;

namespace Luny.Unity.Providers
{
	/// <summary>
	/// Unity implementation of Debug provider.
	/// </summary>
	public sealed class UnityDebugServiceProvider : IDebugServiceProvider
	{
		public void LogInfo(String message) => Debug.Log(message);
		public void LogWarning(String message) => Debug.LogWarning(message);
		public void LogError(String message) => Debug.LogError(message);
		public void LogException(Exception exception) => Debug.LogException(exception);
	}
}
