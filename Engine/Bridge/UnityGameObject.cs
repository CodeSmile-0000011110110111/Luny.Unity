using Luny.Engine.Bridge;
using System;
using System.Runtime.CompilerServices;
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
		private Renderer Renderer
		{
			get
			{
				if (_renderer != null)
					return _renderer;

				var go = GO;
				if (go == null)
					return null;

				return go.TryGetComponent(out _renderer) ? _renderer : null;
			}
		}
		private GameObject GO
		{
			get
			{
				if (!IsNativeObjectValid())
					return null;

				return (GameObject)NativeObject;
			}
		}

		public static ILunyObject ToLunyObject(GameObject gameObject)
		{
			var instanceId = gameObject.GetEntityId();
			if (TryGetCached(instanceId, out var lunyObject))
				return lunyObject;

			return new UnityGameObject(gameObject, instanceId);
		}

		internal static ILunyObject FindNativeObject(String name)
		{
			var foundObject = GameObject.Find(name);
			if (foundObject == null)
				return null;

			return ToLunyObject(foundObject);
		}

		private static Boolean IsNativeObjectVisible(GameObject gameObject)
		{
			if (gameObject == null)
				return false;

			return gameObject.TryGetComponent<Renderer>(out var renderer) && renderer.enabled;
		}

		private UnityGameObject(GameObject gameObject, Int64 instanceId)
			: base(gameObject, instanceId, gameObject.activeSelf, IsNativeObjectVisible(gameObject)) => Name = gameObject.name;

		protected override void DestroyNativeObject() => Object.Destroy(GO); // Destroy handles null parameters

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override Boolean IsNativeObjectValid()
		{
			var go = NativeObject as GameObject; // must use NativeObject here to avoid stackoverflow
			return go != null;
		}

		protected override String GetNativeObjectName()
		{
			var go = GO;
			if (go == null)
				return "<null>";

			return go.name;
		}

		protected override void SetNativeObjectName(String name)
		{
			var go = GO;
			if (go == null)
				return;

			go.name = name;
		}

		protected override Boolean GetNativeObjectEnabledInHierarchy()
		{
			var go = GO;
			if (go == null)
				return false;

			return go.activeInHierarchy;
		}

		protected override Boolean GetNativeObjectEnabled()
		{
			var go = GO;
			if (go == null)
				return false;

			return go.activeSelf;
		}

		protected override void SetNativeObjectEnabled()
		{
			var go = GO;
			if (go == null)
				return;

			go.SetActive(true);
		}

		protected override void SetNativeObjectDisabled()
		{
			var go = GO;
			if (go == null)
				return;

			go.SetActive(false);
		}

		protected override void SetNativeObjectVisible()
		{
			var renderer = Renderer;
			if (renderer == null)
				return;

			renderer.enabled = true;
		}

		protected override void SetNativeObjectInvisible()
		{
			var renderer = Renderer;
			if (renderer == null)
				return;

			renderer.enabled = false;
		}
	}
}
