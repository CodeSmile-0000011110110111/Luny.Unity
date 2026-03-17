using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = System.Object;

namespace Luny.UnityEditor
{
	/// <summary>
	/// Listens to the creation of a `.request-refresh` file in the project's root.
	/// If that file exists, it will trigger an AssetDatabase.Refresh() and thus import assets
	/// and possibly recompile. During that time, a `.refresh-in-progress` file will exist in the project's root,
	/// which external programs can watch for.
	/// </summary>
	internal static class ExternalRefreshListener
	{
		private const String RefreshFile = ".request-refresh";
		private const String BusyFile = ".refresh-in-progress";

		private static FileSystemWatcher _watcher;

		private static void DoRefresh()
		{
			Debug.Log($"[{nameof(ExternalRefreshListener)}] AssetDatabase.Refresh() requested by external program.");

			TryCreateBusyFile();
			TryDeleteRefreshFile();
			AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
			CompilationPipeline.RequestScriptCompilation();
		}

		private static void OnCompilationFinished(Object obj)
		{
			Debug.Log($"[{nameof(ExternalRefreshListener)}] Compilation finished.");
			TryDeleteBusyFile();
		}

		/*
		private static void OnCompilationNotRequired(String name)
		{
			Debug.Log($"[{nameof(ExternalRefreshListener)}] Compilation not required.");
			TryDeleteBusyFile();
		}
		*/

		private static void TryDeleteRefreshFile()
		{
			try
			{
				if (File.Exists(RefreshFile))
					File.Delete(RefreshFile); // Clear the trigger
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private static void TryCreateBusyFile()
		{
			try
			{
				File.Create(BusyFile).Close();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		private static void TryDeleteBusyFile()
		{
			try
			{
				if (File.Exists(BusyFile))
					File.Delete(BusyFile);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		[InitializeOnLoadMethod]
		private static void InitExternalRefreshListener()
		{
			Debug.Log($"[{nameof(ExternalRefreshListener)}] Registering file watcher.");

			_watcher = new FileSystemWatcher(Directory.GetCurrentDirectory(), RefreshFile);
			_watcher.Created += (s, e) =>
			{
				Debug.Log($"[{nameof(ExternalRefreshListener)}] Refresh request received ...");
				EditorApplication.delayCall += DoRefresh;
				EditorApplication.QueuePlayerLoopUpdate();
			};
			_watcher.EnableRaisingEvents = true;

			// Register for compilation finish
			CompilationPipeline.compilationFinished += OnCompilationFinished;

			// Handle cases where no recompilation is needed
			// CompilationPipeline.assemblyCompilationNotRequired += OnCompilationNotRequired;
		}
	}
}
