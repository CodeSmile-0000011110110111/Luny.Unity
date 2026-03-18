using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Bridge;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Luny.Unity.Services
{
	public sealed class UnityAssetService : LunyAssetServiceBase
	{
		protected override void OnServiceInitialize() => LunyPath.Converter = new UnityPathConverter();

		protected override T LoadAsset<T>(LunyAssetPath path)
		{
			//LunyLogger.LogInfo($"Try load asset path: {path.NativePath}", this);

			Object asset;
			var nativePath = path.NativePath;
			if (nativePath.StartsWith("Assets/") || nativePath.StartsWith("Packages/"))
			{
				asset = AssetDatabase.LoadAssetAtPath<Object>(nativePath);

				if (asset != null)
					LunyLogger.LogWarning($"Using Asset path '{nativePath}' currently only works in Editor, not in builds!", this);
			}
			else
				asset = Resources.Load(nativePath);

			if (asset == null)
				return null;

			if (typeof(T) == typeof(ILunyPrefab) && asset is GameObject go)
				return new UnityPrefab(go, path) as T;

			LunyLogger.LogError($"Loading assets of type '{typeof(T).Name}' is not yet supported.", this);

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
				go.SetActive(false);
				go.AddComponent<BoxCollider>();
				go.AddComponent<Rigidbody>();
				go.hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;
				go.name = $"Placeholder('{path}')";
				return new UnityPrefab(go, path, true) as T;
			}
			return null;
		}
	}
}
