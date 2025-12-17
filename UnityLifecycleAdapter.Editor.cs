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
			EditorApplication.playModeStateChanged -= OnExitPlayMode;
			EditorApplication.playModeStateChanged += OnExitPlayMode;
		}

		private static void OnExitPlayMode(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingPlayMode)
				CatchAllSingletonInstanceReset();
		}

		// safety reset for "disabled domain reload"
		private static void CatchAllSingletonInstanceReset() => _instance = null;
#endif
	}
}
