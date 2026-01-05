using Luny.Engine.Bridge;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Luny.Unity.Engine.Bridge
{
	/// <summary>
	/// Unity-specific implementation wrapping UnityEngine.GameObject.
	/// </summary>
	internal sealed class LunyUnityGameObject : LunyObject
	{
		private GameObject GameObject => Cast<GameObject>();

		private static Boolean IsNativeObjectVisible(GameObject gameObject) =>
			gameObject.TryGetComponent<Renderer>(out var renderer) && renderer.enabled;

		public LunyUnityGameObject(GameObject gameObject)
			: base(gameObject, gameObject.GetEntityId(), gameObject.activeSelf, IsNativeObjectVisible(gameObject)) {}

		protected override void DestroyNativeObject() => Object.Destroy(GameObject);
		protected override Boolean IsNativeObjectValid() => GameObject != null;
		protected override String GetNativeObjectName() => GameObject.name;
		protected override void SetNativeObjectName(String name) => GameObject.name = name;
		protected override Boolean GetNativeObjectEnabledInHierarchy() => GameObject.activeInHierarchy;
		protected override Boolean GetNativeObjectEnabled() => GameObject.activeSelf;
		protected override void SetNativeObjectEnabled() => GameObject.SetActive(true);
		protected override void SetNativeObjectDisabled() => GameObject.SetActive(false);

		protected override void SetNativeObjectVisible()
		{
			if (GameObject.TryGetComponent<Renderer>(out var renderer))
				renderer.enabled = true;
		}

		protected override void SetNativeObjectInvisible()
		{
			if (GameObject.TryGetComponent<Renderer>(out var renderer))
				renderer.enabled = false;
		}
	}
}
