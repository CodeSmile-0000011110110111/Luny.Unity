using System;

namespace Luny.Unity
{
    using UnityEngine;

    /// <summary>
    /// Unity-specific implementation of the Luny logger that forwards to UnityEngine.Debug.
    /// </summary>
    internal sealed class UnityLogger : ILunyLogger
    {
        public void Info(String message) => Debug.Log(message);
        public void Warn(String message) => Debug.LogWarning(message);
        public void Error(String message) => Debug.LogError(message);
        public void Exception(Exception exception) => Debug.LogException(exception);
    }
}
