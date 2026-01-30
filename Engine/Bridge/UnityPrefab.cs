using Luny.Engine.Bridge;
using Luny.Engine.Bridge.Identity;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class UnityPrefab : ILunyPrefab
	{
		private readonly GameObject _prefab;

		public LunyAssetID AssetID { get; internal set; }
		public System.Object NativeAsset => _prefab;
		public LunyAssetPath AssetPath { get; }

		public UnityPrefab(GameObject prefab, LunyAssetPath assetPath)
		{
			_prefab = prefab ?? throw new ArgumentNullException(nameof(prefab));
			AssetPath = assetPath ?? throw new ArgumentNullException(nameof(assetPath));
		}

		public T Cast<T>() where T : class => _prefab as T;

		public GameObject Instantiate()
		{
			var instance = UnityEngine.Object.Instantiate(_prefab);
			instance.name = instance.name.Replace("(Clone)", "");
			return instance;
		}
	}
}
