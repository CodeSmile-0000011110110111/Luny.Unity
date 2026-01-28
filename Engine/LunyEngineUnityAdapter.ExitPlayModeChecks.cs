#if UNITY_EDITOR
using Luny.Unity.Engine.Bridge;
using UnityEditor;
using UnityEngine;
using System;

namespace Luny.Unity.Engine
{
	internal sealed partial class LunyEngineUnityAdapter
	{
		// precautionary verification that static fields have been set to null
		// this ensures proper "disabled domain reload" behaviour
		[InitializeOnLoadMethod]
		private static void Init()
		{
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

			// clear once after domain reload
			EnsureStaticFieldsAreNull();
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.ExitingEditMode)
			{
				LunyLogger.LogInfo($"{state} ...", nameof(LunyEngineUnityAdapter));
				EnsureStaticFieldsAreNull();
			}
		}

		private static void EnsureStaticFieldsAreNull()
		{
			LunyEngine.ForceReset_UnityEditorAndUnitTestsOnly();

			if (s_Instance != null)
			{
				LogWarning($"{nameof(LunyEngineUnityAdapter)} _instance not null when exiting/entering playmode. Resetting ...");
				s_Instance = null;
			}

			if (LunyLogger.Logger is UnityLogger)
			{
				LogWarning($"{nameof(LunyLogger)} still references a {nameof(UnityLogger)} instance when exiting/entering playmode.");
				LunyLogger.Logger = null;
			}
		}

		private static void LogWarning(String msg)
		{
			if (LunyLogger.Logger is UnityLogger)
				LunyLogger.LogWarning(msg, nameof(LunyEngineUnityAdapter));
			else
				Debug.LogWarning(msg);
		}
	}
}
#endif
