using Luny.Engine.Bridge;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Luny.Unity.Engine.Bridge
{
	/// <summary>
	/// Unity-specific implementation wrapping UnityEngine.GameObject.
	/// </summary>
	internal sealed class UnityGameObject : LunyObject
	{
		private Renderer _renderer;
		private Renderer Renderer => _renderer != null ? _renderer : GO.TryGetComponent(out _renderer) ? _renderer : null;
		private GameObject GO => Cast<GameObject>();

		private static Boolean IsNativeObjectVisible(GameObject gameObject) =>
			gameObject.TryGetComponent<Renderer>(out var renderer) && renderer.enabled;

		public UnityGameObject(GameObject gameObject)
			: base(gameObject, gameObject.GetEntityId(), gameObject.activeSelf, IsNativeObjectVisible(gameObject)) {}

		protected override void DestroyNativeObject() => Object.Destroy(GO);
		protected override Boolean IsNativeObjectValid() => GO != null;
		protected override String GetNativeObjectName() => GO.name;
		protected override void SetNativeObjectName(String name) => GO.name = name;
		protected override Boolean GetNativeObjectEnabledInHierarchy() => GO.activeInHierarchy;
		protected override Boolean GetNativeObjectEnabled() => GO.activeSelf;
		protected override void SetNativeObjectEnabled() => GO.SetActive(true);
		protected override void SetNativeObjectDisabled() => GO.SetActive(false);

		protected override void SetNativeObjectVisible()
		{
			if (Renderer != null)
				Renderer.enabled = true;
		}

		protected override void SetNativeObjectInvisible()
		{
			if (Renderer != null)
				Renderer.enabled = false;
		}
	}
}
