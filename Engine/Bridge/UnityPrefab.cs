using Luny.Engine.Bridge;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class UnityPrefab : LunyPrefab
	{
		private static void SetInstanceName(GameObject instance) => instance.name = instance.name.Replace("(Clone)", "");

		public UnityPrefab(GameObject prefab, LunyAssetPath assetPath)
			: base(prefab, assetPath) {}

		public override T Instantiate<T>(ILunyObject parent)
		{
			var unityPrefab = Cast<GameObject>();
			var instance = parent != null && parent.IsValid
				? Object.Instantiate(unityPrefab, parent.Transform.As<Transform>())
				: Object.Instantiate(unityPrefab);

			SetInstanceName(instance);
			return instance as T;
		}

		public GameObject Instantiate(Transform parent, Vector3 position, Quaternion rotation)
		{
			var unityPrefab = Cast<GameObject>();
			var instance = parent != null
				? Object.Instantiate(unityPrefab, position, rotation, parent)
				: Object.Instantiate(unityPrefab, position, rotation);

			SetInstanceName(instance);
			return instance;
		}

		public GameObject Instantiate() => Instantiate<GameObject>(null);
		public GameObject Instantiate(ILunyObject parent) => Instantiate<GameObject>(parent);
	}
}
