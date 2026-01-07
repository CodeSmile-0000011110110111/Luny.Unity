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
		public void ReloadScene() => SceneManager.LoadScene(CurrentScene?.Name, LoadSceneMode.Single);

		public IReadOnlyList<ILunyObject> GetAllObjects()
		{
			// FIXME: temporary solution, converts every object to LunyObject without even consulting the LunyObjectRegistry
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

		public ILunyObject FindObjectByName(String name)
		{
			// FIXME: temporary solutions, does not consult with LunyObjectRegistry for existing objects
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

		protected override void OnServiceInitialize()
		{
			SceneManager.sceneLoaded += OnNativeSceneLoaded;
			SceneManager.sceneUnloaded += OnNativeSceneUnloaded;
		}

		protected override void OnServiceStartup()
		{
			CurrentScene = new LunyUnityScene(SceneManager.GetActiveScene());
			LunyLogger.LogInfo($"{nameof(OnServiceInitialize)}: CurrentScene={CurrentScene}", this);

			InvokeOnSceneLoaded(CurrentScene);
		}

		protected override void OnServiceShutdown()
		{
			SceneManager.sceneLoaded -= OnNativeSceneLoaded;
			SceneManager.sceneUnloaded -= OnNativeSceneUnloaded;
			CurrentScene = null;
		}

		private void OnNativeSceneLoaded(Scene scene, LoadSceneMode loadMode)
		{
			if (loadMode == LoadSceneMode.Single)
				CurrentScene = new LunyUnityScene(scene);
			else
				throw new NotImplementedException("additive scene load not yet supported");

			LunyLogger.LogInfo($"{nameof(OnNativeSceneLoaded)}: {CurrentScene} => {ToString()}", this);
			InvokeOnSceneLoaded(CurrentScene);
		}

		private void OnNativeSceneUnloaded(Scene scene)
		{
			LunyLogger.LogInfo($"{nameof(OnNativeSceneUnloaded)}: {CurrentScene} => {ToString()}", this);
			if (CurrentScene?.Name == scene.name)
			{
				InvokeOnSceneUnloaded(CurrentScene);
				CurrentScene = null;
			}
		}
	}
}
