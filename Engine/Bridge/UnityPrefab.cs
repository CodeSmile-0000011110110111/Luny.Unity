using Luny.Engine.Bridge;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class UnityPrefab : LunyPrefab
	{
		public UnityPrefab(GameObject prefab, LunyAssetPath assetPath)
			: base(prefab, assetPath) {}

		public override T Instantiate<T>(ILunyObject parent)
		{
			var unityPrefab = Cast<GameObject>();

			GameObject instance = default;
			instance = parent != null && parent.IsValid
				? Object.Instantiate(unityPrefab, (Transform)parent.Transform.NativeObject)
				: Object.Instantiate(unityPrefab);

			instance.name = instance.name.Replace("(Clone)", "");
			return instance as T;
		}

		public GameObject Instantiate() => Instantiate<GameObject>(null);
		public GameObject Instantiate(ILunyObject parent) => Instantiate<GameObject>(parent);
	}
}
