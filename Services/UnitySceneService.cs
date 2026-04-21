using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Luny.Unity.Services
{
	/// <summary>
	/// Unity implementation of scene information.
	/// </summary>
	public sealed class UnitySceneService : LunySceneServiceBase, ILunySceneService
	{
		public override void ReloadScene() => SceneManager.LoadScene(CurrentScene?.Name);

		public override IReadOnlyList<ILunyGameObject> GetObjects(IReadOnlyCollection<String> objectNames)
		{
			if (objectNames == null || objectNames.Count == 0)
				return Array.Empty<ILunyGameObject>();

			var scene = SceneManager.GetActiveScene();
			var rootGameObjects = scene.GetRootGameObjects();
			var foundObjects = new List<ILunyGameObject>();

			foreach (var rootObj in rootGameObjects)
			{
				if (objectNames.Contains(rootObj.name))
				{
					var lunyObject = UnityGameObject.ToLunyObject(rootObj);
					foundObjects.Add(lunyObject);
				}

				// check all children (including inactive) recursively => getting their Transform is shorthand
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

		public override ILunyGameObject FindObjectByName(String name)
		{
			if (String.IsNullOrEmpty(name))
				return null;

			return UnityGameObject.FindNativeObject(name);
		}

		public override ILunyGameObject FindChildByName(ILunyGameObject parent, String name)
		{
			if (parent == null || String.IsNullOrEmpty(name))
				return null;

			return UnityGameObject.FindNativeChildObject(parent, name);
		}

		protected override void OnServiceInitialize()
		{
			SceneManager.sceneLoaded += OnNativeSceneLoaded;
			SceneManager.sceneUnloaded += OnNativeSceneUnloaded;
		}

		protected override void OnServiceStartup()
		{
			var activeScene = SceneManager.GetActiveScene();
			if (String.IsNullOrEmpty(activeScene.path))
				throw new LunyServiceException("Cannot launch Luny in an 'Untitled' (unsaved) scene. Save the scene, then try again!");

			CurrentScene = new UnityScene(activeScene);
			LunyLogger.LogInfo($"{nameof(OnServiceStartup)}: CurrentScene={CurrentScene}", this);

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
			LunyLogger.LogInfo($"OnNativeSceneLoaded: {scene.path} in frame {Time.frameCount}", this);

			if (loadMode == LoadSceneMode.Single)
				CurrentScene = new UnityScene(scene);
			else
				LunyLogger.LogWarning($"additive scene load not yet supported: {scene.path}");

			LunyLogger.LogInfo("-----------------------------------------------------------------------------------------------------", this);
			LunyLogger.LogInfo($"{nameof(OnNativeSceneLoaded)}: {CurrentScene} => {ToString()}", this);
			LunyLogger.LogInfo("-----------------------------------------------------------------------------------------------------", this);
			InvokeOnSceneLoaded(CurrentScene);
		}

		private void OnNativeSceneUnloaded(Scene scene)
		{
			LunyLogger.LogInfo("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", this);
			LunyLogger.LogInfo($"{nameof(OnNativeSceneUnloaded)}: {CurrentScene} => {ToString()}", this);
			LunyLogger.LogInfo("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", this);
			if (CurrentScene?.Name == scene.name)
			{
				InvokeOnSceneUnloaded(CurrentScene);
				CurrentScene = null;
			}
		}
	}
}
