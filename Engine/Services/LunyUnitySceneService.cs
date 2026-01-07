using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Engine.Bridge;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of scene information.
	/// </summary>
	public sealed class LunyUnitySceneService : LunySceneServiceBase, ILunySceneService
	{
		public String ActiveSceneName => SceneManager.GetActiveScene().name;

		// FIXME: temporary solution, converts every object to LunyObject without even consulting the LunyObjectRegistry
		public void ReloadScene() => SceneManager.LoadScene(ActiveSceneName, LoadSceneMode.Single);

		public IReadOnlyList<ILunyObject> GetAllObjects()
		{
			var scene = SceneManager.GetActiveScene();
			var rootGameObjects = scene.GetRootGameObjects();
			var allObjects = new List<ILunyObject>();

			foreach (var rootObj in rootGameObjects)
			{
				// FIXME: we probably shouldn't register all scene objects, just the ones "being used"

				// Add root object
				allObjects.Add(new LunyUnityGameObject(rootObj));

				// Add all children recursively
				var transforms = rootObj.GetComponentsInChildren<Transform>(true);
				foreach (var transform in transforms)
				{
					if (transform.gameObject != rootObj) // Skip root (already added)
						allObjects.Add(new LunyUnityGameObject(transform.gameObject));
				}
			}

			return allObjects;
		}

		// FIXME: temporary solutions, does not consult with LunyObjectRegistry for existing objects
		public ILunyObject FindObjectByName(String name)
		{
			if (String.IsNullOrEmpty(name))
				return null;

			var scene = SceneManager.GetActiveScene();
			if (!scene.isLoaded)
				return null;

			// Search all objects in scene hierarchy
			var rootGameObjects = scene.GetRootGameObjects();
			foreach (var rootObj in rootGameObjects)
			{
				if (rootObj.name == name)
					return new LunyUnityGameObject(rootObj);

				// Search children
				var transforms = rootObj.GetComponentsInChildren<Transform>(true);
				foreach (var transform in transforms)
				{
					if (transform.gameObject.name == name)
						return new LunyUnityGameObject(transform.gameObject);
				}
			}

			return null;
		}

		protected override void OnServiceInitialize() {}

		protected override void OnServiceStartup()
		{
			SceneManager.sceneLoaded += OnNativeSceneLoaded;
			SceneManager.sceneUnloaded += OnNativeSceneUnloaded;
		}

		protected override void OnServiceShutdown()
		{
			SceneManager.sceneLoaded -= OnNativeSceneLoaded;
			SceneManager.sceneUnloaded -= OnNativeSceneUnloaded;
		}

		private void OnNativeSceneLoaded(Scene scene, LoadSceneMode loadMode) =>
			LunyLogger.LogInfo($"{nameof(OnNativeSceneLoaded)}: {scene}", this);

		private void OnNativeSceneUnloaded(Scene scene) => LunyLogger.LogInfo($"{nameof(OnNativeSceneUnloaded)}: {scene}", this);
	}
}
