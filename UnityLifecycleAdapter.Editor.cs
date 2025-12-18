using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Luny.Unity
{
	internal sealed partial class UnityLifecycleAdapter
	{
#if UNITY_EDITOR
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			Debug.Log($"state: {state}, playing: {EditorApplication.isPlaying}, {Application.isPlaying}, willChange: {EditorApplication.isPlayingOrWillChangePlaymode}");
			if (state == PlayModeStateChange.EnteredEditMode)
				CatchAllSingletonInstanceReset();
		}

		// safety reset for "disabled domain reload"
		private static void CatchAllSingletonInstanceReset()
		{
			if (_instance != null)
			{
				Debug.LogError($"{nameof(UnityLifecycleAdapter)} _instance not null when exiting playmode - investigate!");
				_instance = null;
			}
			LunyLogger.SetLogger(null);
			Debug.Log($"{nameof(UnityLifecycleAdapter)} exiting playmode complete");
		}
#endif
	}
}
