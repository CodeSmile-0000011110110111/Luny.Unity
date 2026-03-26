using System;
using UnityEditor;
using UnityEngine;

namespace Luny.UnityEditor
{
	internal static class CollectGarbageOnTogglePlayMode
	{
		[RuntimeInitializeOnLoadMethod]
		private static void InitializeOnLoad()
		{
			EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
			EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
		}

		private static void OnPlaymodeStateChanged(PlayModeStateChange mode)
		{
			if (mode == PlayModeStateChange.EnteredPlayMode || mode == PlayModeStateChange.EnteredEditMode)
				CollectAndReport();
		}

		private static void CollectAndReport()
		{
			var memoryBefore = GC.GetTotalMemory(false);

			GC.Collect();
			GC.WaitForPendingFinalizers();

			var memoryAfter = GC.GetTotalMemory(true);
			var reclaimed = memoryBefore - memoryAfter;
			LunyLogger.LogInfo($"{reclaimed / 1024.0 / 1024.0:F1} MB garbage collected", nameof(CollectGarbageOnTogglePlayMode));
		}
	}
}
