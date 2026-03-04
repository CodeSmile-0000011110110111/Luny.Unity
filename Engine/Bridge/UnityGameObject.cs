using Luny.Engine.Bridge;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
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

		/// <summary>
		/// Wraps a GameObject instance as engine-agnostic LunyObject.
		/// </summary>
		/// <remarks></remarks>
		/// <param name="gameObject">A valid GameObject instance. The parameter is System.Object on purpose: Unity Console message double-click will not open the topmost line of the callstack otherwise for some reason.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static ILunyObject ToLunyObject(System.Object gameObject)
		{
			if (gameObject is not GameObject go)
				throw new ArgumentException($"[{nameof(UnityGameObject)}] {nameof(ToLunyObject)}: {gameObject} must be a GameObject instance");

			var instanceId = go.GetEntityId();
			if (TryGetCached(instanceId, out var lunyObject))
				return lunyObject;

			return new UnityGameObject(go, instanceId);
		}

		internal static ILunyObject FindNativeObject(String name)
		{
			var foundObject = GameObject.Find(name);
			if (foundObject == null)
				foundObject = FindInactive(name);

			if (foundObject == null)
				return null;

			return ToLunyObject(foundObject);
		}

		private static GameObject FindInactive(String name)
		{
			foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
			{
				var found = FindRecursive(root.transform, name);
				if (found != null)
					return found.gameObject;
			}

			return null;
		}

		private static Transform FindRecursive(Transform parent, String name)
		{
			if (parent.name == name)
				return parent;

			foreach (Transform child in parent)
			{
				var found = FindRecursive(child, name);
				if (found != null)
					return found;
			}

			return null;
		}

		private static Boolean IsNativeObjectVisible(GameObject gameObject)
		{
			if (gameObject == null)
				return false;

			return gameObject.TryGetComponent<Renderer>(out var renderer) && renderer.enabled;
		}

		// Odd: method signature must not use UnityEngine.Object arguments or else double-clicking Console messages
		// with callstacks pointing to within will not open the topmost line in the callstack!
		private UnityGameObject(System.Object gameObject, Int64 instanceId)
			: base(gameObject, instanceId, ((GameObject)gameObject).activeSelf, IsNativeObjectVisible((GameObject)gameObject)) =>
			Name = ((GameObject)gameObject).name;

		protected override LunyTransform GetNativeTransform()
		{
			var go = GO;
			if (go == null)
				return null;

			return new UnityTransform(go.transform);
		}

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
