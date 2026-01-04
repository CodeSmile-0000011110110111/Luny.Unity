using Luny.Unity.Engine.Services;
using UnityEditor;
using UnityEngine;

namespace Luny.Unity.Engine
{
	sealed partial class LunyEngineUnityAdapter
	{
#if UNITY_EDITOR
		// precautionary verification that static fields have been set to null
		// this ensures proper "disabled domain reload" behaviour
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.EnteredEditMode)
			{
				EnsureStaticFieldsAreNull();
				LunyEngine.ResetDisposedFlag_UnityEditorOnly();
			}
		}

		private static void EnsureStaticFieldsAreNull()
		{
			if (s_Instance != null)
			{
				Debug.LogError($"{nameof(LunyEngineUnityAdapter)} _instance not null when exiting playmode!");
				s_Instance = null;
			}

			if (LunyLogger.Logger is UnityLogger)
			{
				Debug.LogError($"{nameof(LunyLogger)} still references a {nameof(UnityLogger)} instance when exiting playmode!");
				LunyLogger.Logger = null;
			}
		}
#endif
	}
}
