using Luny.Engine.Bridge;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class UnityPrefab : LunyPrefab
	{
		public UnityPrefab(GameObject prefab, LunyAssetPath assetPath)
			: base(prefab, assetPath) {}

		public override T Instantiate<T>()
		{
			var unityPrefab = Cast<GameObject>();
			var instance = Object.Instantiate(unityPrefab);
			instance.name = instance.name.Replace("(Clone)", "");
			return instance as T;
		}

		public GameObject Instantiate() => Instantiate<GameObject>();
	}
}
