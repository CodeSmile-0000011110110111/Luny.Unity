using Luny.Proxies;
using Luny.Unity.Proxies;
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
			if (state == PlayModeStateChange.EnteredEditMode)
				CheckStaticFieldReferencesAreNull();
		}

		private static void CheckStaticFieldReferencesAreNull()
		{
			// precautionary static field checks for exiting playmode with "disabled domain reload"

			if (_instance != null)
			{
				Debug.LogError($"{nameof(UnityLifecycleAdapter)} _instance not null when exiting playmode - investigate!");
				_instance = null;
			}

			if (LunyLogger.Logger is UnityLogger)
			{
				Debug.LogError($"{nameof(LunyLogger)}.Logger still assigned when exiting playmode - investigate!");
				LunyLogger.Logger = null;
			}
		}
#endif
	}
}
