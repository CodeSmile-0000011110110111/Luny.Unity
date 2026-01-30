using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Engine.Bridge;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	public sealed class UnityAssetService : LunyAssetServiceBase
	{
		protected override void OnServiceInitialize() => LunyPath.Converter = new UnityPathConverter();

		protected override T LoadNative<T>(LunyAssetPath path)
		{
			var nativePath = path.NativePath;
			var asset = Resources.Load(nativePath);
			if (asset == null)
				return null;

			if (typeof(T) == typeof(ILunyPrefab) && asset is GameObject go)
				return new UnityPrefab(go, path) as T;

			// Add more types as needed
			return null;
		}

		protected override void UnloadNative(ILunyAsset asset)
		{
			// Resources.UnloadAsset is only for specific types, for GameObjects we typically don't unload individual prefabs like this.
			// In real engine, we'd use Resources.UnloadUnusedAssets().
		}

		protected override IReadOnlyDictionary<Type, String[]> GetExtensionMapping() => new Dictionary<Type, String[]>
		{
			// Resources.Load doesn't use extensions
			{ typeof(ILunyPrefab), new[] { "" } },
		};

		protected override T GetPlaceholder<T>(LunyAssetPath path)
		{
			if (typeof(T) == typeof(ILunyPrefab))
			{
				var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				go.name = $"Missing: {path}";
				return new UnityPrefab(go, path) as T;
			}
			return null;
		}
	}
}
