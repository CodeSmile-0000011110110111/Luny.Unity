using System;
using UnityEditor;
using UnityEngine;

namespace Luny.UnityEditor
{
	internal static class CollectGarbageAfterPlaymode
	{
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeOnLoad() => EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;

		private static void OnPlaymodeStateChanged(PlayModeStateChange mode)
		{
			if (mode == PlayModeStateChange.EnteredEditMode)
				EditorApplication.delayCall += () => GC.Collect();
		}
	}
}
