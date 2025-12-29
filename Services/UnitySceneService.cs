using Luny.Proxies;
using Luny.Services;
using Luny.Unity.Proxies;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Luny.Unity.Services
{
	/// <summary>
	/// Unity implementation of scene information.
	/// </summary>
	public sealed class UnitySceneService : ISceneService
	{
		public String CurrentSceneName => SceneManager.GetActiveScene().name;

		public IReadOnlyList<ILunyObject> GetAllObjects()
		{
			var scene = SceneManager.GetActiveScene();
			var rootGameObjects = scene.GetRootGameObjects();
			var allObjects = new List<ILunyObject>();

			foreach (var rootObj in rootGameObjects)
			{
				// Add root object
				allObjects.Add(new UnityObject(rootObj));

				// Add all children recursively
				var transforms = rootObj.GetComponentsInChildren<Transform>(true);
				foreach (var transform in transforms)
				{
					if (transform.gameObject != rootObj) // Skip root (already added)
						allObjects.Add(new UnityObject(transform.gameObject));
				}
			}

			return allObjects;
		}

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
					return new UnityObject(rootObj);

				// Search children
				var transforms = rootObj.GetComponentsInChildren<Transform>(true);
				foreach (var transform in transforms)
				{
					if (transform.gameObject.name == name)
						return new UnityObject(transform.gameObject);
				}
			}

			return null;
		}
	}
}
