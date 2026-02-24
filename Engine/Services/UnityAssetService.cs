using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Engine.Bridge;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	public sealed class UnityAssetService : LunyAssetServiceBase
	{
		protected override void OnServiceInitialize() => LunyPath.Converter = new UnityPathConverter();

		protected override T LoadAsset<T>(LunyAssetPath path)
		{
			UnityEngine.Object asset = null;
			var nativePath = path.NativePath;
			LunyLogger.LogInfo($"Try load asset path: {nativePath}", this);
			if (nativePath.StartsWith("Assets/"))
			{
				asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(nativePath);

				if (asset != null)
					LunyLogger.LogWarning($"Using Asset path '{nativePath}' only works in Editor, not in builds!", this);
			}
			else
				asset = Resources.Load(nativePath);

			if (asset == null)
				return null;

			if (typeof(T) == typeof(ILunyPrefab) && asset is GameObject go)
				return new UnityPrefab(go, path) as T;

			LunyLogger.LogError($"Loading assets of type '{typeof(T).Name}' is not supported.", this);

			// Add more types as needed
			return null;
		}

		protected override void UnloadAsset(ILunyAsset asset)
		{
			// Resources.UnloadAsset is only for specific types, for GameObjects we typically don't unload individual prefabs like this.
			// In real engine, we'd use Resources.UnloadUnusedAssets().
		}

		protected override IReadOnlyDictionary<Type, String[]> GetExtensionMapping() => new Dictionary<Type, String[]>
		{
			{
				// Resources.Load doesn't use extensions
				typeof(ILunyPrefab), new[] { ".prefab", "" }
			},
		};

		protected override T GetPlaceholder<T>(LunyAssetPath path)
		{
			if (typeof(T) == typeof(ILunyPrefab))
			{
				// TODO: load an actual prefab asset because this creates duplicates in the scene ("prefab" + instance)
				var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				//go.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
				go.name = $"Not found: {path}";
				go.AddComponent<BoxCollider>();
				go.AddComponent<Rigidbody>();
				return new UnityPrefab(go, path) as T;
			}
			return null;
		}
	}
}
