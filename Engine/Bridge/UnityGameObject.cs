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
		private Renderer Renderer =>
			_renderer != null ? _renderer : IsNativeObjectValid() && GO.TryGetComponent(out _renderer) ? _renderer : null;
		private GameObject GO => Cast<GameObject>();

		public static ILunyObject ToLunyObject(GameObject gameObject)
		{
			var instanceId = gameObject.GetEntityId();
			if (TryGetCached(instanceId, out var lunyObject))
				return lunyObject;

			return new UnityGameObject(gameObject, instanceId);
		}

		internal static ILunyObject FindNativeObject(String name)
		{
			var nativeGo = GameObject.Find(name);
			return nativeGo != null ? ToLunyObject(nativeGo) : null;
		}

		private static Boolean IsNativeObjectVisible(GameObject gameObject) =>
			gameObject != null && gameObject.TryGetComponent<Renderer>(out var renderer) && renderer.enabled;

		private UnityGameObject(GameObject gameObject, Int64 instanceId)
			: base(gameObject, instanceId, gameObject.activeSelf, IsNativeObjectVisible(gameObject)) => Name = gameObject.name;

		protected override void DestroyNativeObject() => Object.Destroy(GO); // Destroy handles null parameters
		protected override Boolean IsNativeObjectValid() => GO != null;
		protected override String GetNativeObjectName() => IsNativeObjectValid() ? GO.name : "<null>";

		protected override void SetNativeObjectName(String name)
		{
			if (IsNativeObjectValid())
				GO.name = name;
		}

		protected override Boolean GetNativeObjectEnabledInHierarchy() => IsNativeObjectValid() && GO.activeInHierarchy;
		protected override Boolean GetNativeObjectEnabled() => IsNativeObjectValid() && GO.activeSelf;

		protected override void SetNativeObjectEnabled()
		{
			if (IsNativeObjectValid())
				GO.SetActive(true);
		}

		protected override void SetNativeObjectDisabled()
		{
			if (IsNativeObjectValid())
				GO?.SetActive(false);
		}

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
