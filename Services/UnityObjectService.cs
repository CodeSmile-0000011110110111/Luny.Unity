using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Bridge;
using System;
using UnityEngine;

namespace Luny.Unity.Services
{
	public sealed class UnityObjectService : LunyObjectServiceBase, ILunyObjectService
	{
		private static void ApplyProperties(GameObject go, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale)
		{
			var transform = go.transform;
			if (parent is not null && parent.IsValid)
				transform.SetParent(parent.Cast<GameObject>().transform);

			var hasRigidbody = go.TryGetComponent<Rigidbody>(out var rigidbody);
			var hasRigidbody2D = go.TryGetComponent<Rigidbody2D>(out var rigidbody2d);
			if (position.HasValue)
			{
				transform.localPosition = position.Value.ToUnity();

				// sync rigidbody in case of: a) interpolation enabled and b) instantiated outside of FixedUpdate
				if (hasRigidbody)
					rigidbody.position = transform.position;
				else if (hasRigidbody2D)
				{
					var pos = transform.position;
					rigidbody2d.position = new Vector2(pos.x, pos.z);
				}
			}
			if (rotation.HasValue)
			{
				transform.localRotation = rotation.Value.ToUnity();

				// sync rigidbody in case of: a) interpolation enabled and b) instantiated outside of FixedUpdate
				if (hasRigidbody)
					rigidbody.rotation = transform.rotation;
				else if (hasRigidbody2D)
					rigidbody2d.rotation = transform.rotation.eulerAngles.z;
			}
			if (scale.HasValue)
				transform.localScale = scale.Value.ToUnity();

			// TODO: remove this - resetting hideflags because our current placeholder is created on the fly and hidden in the scene hierarchy
			if (go.hideFlags == (HideFlags.HideAndDontSave | HideFlags.HideInInspector))
			{
				go.SetActive(true);
				go.hideFlags = HideFlags.None;
			}

			// LunyLogger.LogInfo($"Created {go.name} ({transform.GetEntityId()}) at:{transform.localPosition}, " +
			//                    $"rot:{transform.localRotation.eulerAngles}, scale:{transform.localScale}", go);
		}

		public override ILunyGameObject CreatePrimitive(String name, LunyPrimitiveType type, ILunyGameObject parent, LunyVector3? position,
			LunyQuaternion? rotation, LunyVector3? scale)
		{
			var go = GameObject.CreatePrimitive(type switch
			{
				LunyPrimitiveType.Cube => PrimitiveType.Cube,
				LunyPrimitiveType.Sphere => PrimitiveType.Sphere,
				LunyPrimitiveType.Capsule => PrimitiveType.Capsule,
				LunyPrimitiveType.Cylinder => PrimitiveType.Cylinder,
				LunyPrimitiveType.Plane => PrimitiveType.Plane,
				LunyPrimitiveType.Quad => PrimitiveType.Quad,
				var _ => throw new ArgumentOutOfRangeException(nameof(type), type.ToString()),
			});
			go.name = name;
			ApplyProperties(go, parent, position, rotation, scale);
			return UnityGameObject.ToLunyObject(go);
		}

		public override ILunyGameObject CreateFromPrefab(ILunyPrefab prefab, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale)
		{
			if (prefab is not UnityPrefab unityPrefab)
				throw new ArgumentException($"Prefab must be of type {nameof(UnityPrefab)}", nameof(prefab));

			var pos = position?.ToUnity() ?? Vector3.zero;
			var rot = rotation?.ToUnity() ?? Quaternion.identity;
			var go = unityPrefab.Instantiate(parent?.Transform?.Cast<Transform>(), pos, rot);
			if (scale.HasValue)
				go.transform.localScale = scale.Value.ToUnity();
			return UnityGameObject.ToLunyObject(go);
		}

		public override ILunyGameObject Clone(ILunyGameObject original, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale)
		{
			var go = original?.Clone(parent?.Transform);
			ApplyProperties(go.As<GameObject>(), parent, position, rotation, scale);
			return go;
		}

		public override ILunyGameObject CreateEmpty(String name, ILunyGameObject parent, LunyVector3? position, LunyQuaternion? rotation,
			LunyVector3? scale)
		{
			var go = new GameObject(name);
			ApplyProperties(go, parent, position, rotation, scale);
			return UnityGameObject.ToLunyObject(go);
		}
	}
}
