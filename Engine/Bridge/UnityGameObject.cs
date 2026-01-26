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

		public static ILunyObject ToLunyObject(GameObject gameObject)
		{
			var instanceId = (Int64)gameObject.GetEntityId();
			if (TryGetCached(instanceId, out var lunyObject))
				return lunyObject;

			return new UnityGameObject(gameObject, instanceId);
		}

		internal static ILunyObject FindNativeObject(string name)
		{
			var nativeGo = GameObject.Find(name);
			return nativeGo != null ? ToLunyObject(nativeGo) : null;
		}

		private static Boolean IsNativeObjectVisible(GameObject gameObject) =>
			gameObject.TryGetComponent<Renderer>(out var renderer) && renderer.enabled;

		private UnityGameObject(GameObject gameObject, Int64 instanceId)
			: base(gameObject, instanceId, gameObject.activeSelf, IsNativeObjectVisible(gameObject))
		{
			Name = gameObject.name;
		}

		protected override void DestroyNativeObject() => Object.Destroy(GO);
		protected override Boolean IsNativeObjectValid() => GO != null;
		protected override String GetNativeObjectName() => GO != null ? GO.name : "Destroyed";
		protected override void SetNativeObjectName(String name)
		{
			if (GO != null) GO.name = name;
		}
		protected override Boolean GetNativeObjectEnabledInHierarchy() => GO != null && GO.activeInHierarchy;
		protected override Boolean GetNativeObjectEnabled() => GO != null && GO.activeSelf;
		protected override void SetNativeObjectEnabled() => GO?.SetActive(true);
		protected override void SetNativeObjectDisabled() => GO?.SetActive(false);

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
