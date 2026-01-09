using Luny.Unity.Engine.Bridge;
using UnityEditor;
using UnityEngine;

namespace Luny.Unity.Engine
{
	internal sealed partial class LunyEngineUnityAdapter
	{
#if UNITY_EDITOR
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
				LunyLogger.LogInfo("Entering/exiting play mode ...");
				EnsureStaticFieldsAreNull();
			}
		}

		private static void EnsureStaticFieldsAreNull()
		{
			LunyEngine.ResetDisposedFlag_UnityEditorOnly();

			if (s_Instance != null)
			{
				Debug.LogWarning($"{nameof(LunyEngineUnityAdapter)} _instance not null when exiting/entering playmode. Resetting ...");
				s_Instance = null;
			}

			if (LunyLogger.Logger is UnityLogger)
			{
				Debug.LogWarning($"{nameof(LunyLogger)} still references a {nameof(UnityLogger)} instance when exiting/entering playmode.");
				LunyLogger.Logger = null;
			}
		}
#endif
	}
}
