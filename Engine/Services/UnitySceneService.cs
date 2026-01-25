using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Engine.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of scene information.
	/// </summary>
	public sealed class UnitySceneService : LunySceneServiceBase, ILunySceneService
	{
		public void ReloadScene() => SceneManager.LoadScene(CurrentScene?.Name, LoadSceneMode.Single);

		public IReadOnlyList<ILunyObject> GetObjects(IReadOnlyCollection<String> objectNames)
		{
			if (objectNames == null || objectNames.Count == 0)
				return Array.Empty<ILunyObject>();

			var scene = SceneManager.GetActiveScene();
			var rootGameObjects = scene.GetRootGameObjects();
			var foundObjects = new List<ILunyObject>();

			foreach (var rootObj in rootGameObjects)
			{
				if (objectNames.Contains(rootObj.name))
					foundObjects.Add(UnityGameObject.ToLunyObject(rootObj));

				// check all children recursively => getting their Transform is shorthand
				var transforms = rootObj.GetComponentsInChildren<Transform>(true);
				foreach (var transform in transforms)
				{
					var go = transform.gameObject;
					if (go == rootObj) // Skip root (already added)
						continue;

					if (objectNames.Contains(transform.name))
						foundObjects.Add(UnityGameObject.ToLunyObject(go));
				}
			}

			return foundObjects.AsReadOnly();
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
					return UnityGameObject.ToLunyObject(rootObj);

				// Search children
				var transforms = rootObj.GetComponentsInChildren<Transform>(true);
				foreach (var transform in transforms)
				{
					if (transform.gameObject.name == name)
						return UnityGameObject.ToLunyObject(transform.gameObject);
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
			CurrentScene = new UnityScene(SceneManager.GetActiveScene());
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
				CurrentScene = new UnityScene(scene);
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
