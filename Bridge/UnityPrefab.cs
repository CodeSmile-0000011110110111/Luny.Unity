using Luny.Engine.Bridge;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Luny.Unity.Bridge
{
	public sealed class UnityPrefab : LunyPrefab
	{
		private Boolean _isPlaceholder;

		private static void SetInstanceName(GameObject instance) => instance.name = instance.name.Replace("(Clone)", "");

		public UnityPrefab(GameObject prefab, LunyAssetPath assetPath, Boolean isPlaceholder = false)
			: base(prefab, assetPath) => _isPlaceholder = isPlaceholder;

		private void ResetPlaceholderProperties(GameObject go)
		{
			go.hideFlags = HideFlags.None;
			go.transform.localRotation = Quaternion.Euler(45f, 45f, 45f);
			go.SetActive(true);
		}

		public override T Instantiate<T>(ILunyObject parent)
		{
			var unityPrefab = Cast<GameObject>();
			var instance = parent != null && parent.IsValid
				? Object.Instantiate(unityPrefab, parent.Transform.As<Transform>())
				: Object.Instantiate(unityPrefab);

			SetInstanceName(instance);
			if (_isPlaceholder)
				ResetPlaceholderProperties(instance);
			return instance as T;
		}

		public GameObject Instantiate(Transform parent, Vector3 position, Quaternion rotation)
		{
			var unityPrefab = Cast<GameObject>();
			var instance = parent != null
				? Object.Instantiate(unityPrefab, position, rotation, parent)
				: Object.Instantiate(unityPrefab, position, rotation);

			SetInstanceName(instance);
			if (_isPlaceholder)
				ResetPlaceholderProperties(instance);
			return instance;
		}

		public GameObject Instantiate() => Instantiate<GameObject>(null);
		public GameObject Instantiate(ILunyObject parent) => Instantiate<GameObject>(parent);
	}
}
