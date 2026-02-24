using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Engine.Bridge;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	public sealed class UnityObjectService : LunyObjectServiceBase, ILunyObjectService
	{
		private static void ApplyProperties(GameObject go, ILunyObject parent, in LunyVector3 position, in LunyQuaternion rotation,
			in LunyVector3 scale)
		{
			var transform = go.transform;
			transform.localPosition = position.ToUnity();
			transform.localRotation = rotation.ToUnity();
			transform.localScale = scale.ToUnity();

			if (parent is not null && parent.IsValid)
				transform.SetParent(parent.Cast<GameObject>().transform);
		}

		public override ILunyObject CreatePrimitive(String name, LunyPrimitiveType type, ILunyObject parent, LunyVector3 position,
			LunyQuaternion rotation, LunyVector3 scale)
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

		public override ILunyObject CreateFromPrefab(ILunyPrefab prefab, ILunyObject parent, LunyVector3 position, LunyQuaternion rotation,
			LunyVector3 scale)
		{
			if (prefab is not UnityPrefab unityPrefab)
				throw new ArgumentException($"Prefab must be of type {nameof(UnityPrefab)}", nameof(prefab));

			var go = unityPrefab.Instantiate(parent);
			ApplyProperties(go, parent, position, rotation, scale);
			return UnityGameObject.ToLunyObject(go);
		}

		public override ILunyObject CreateEmpty(String name, ILunyObject parent, LunyVector3 position, LunyQuaternion rotation,
			LunyVector3 scale)
		{
			var go = new GameObject(name);
			ApplyProperties(go, parent, position, rotation, scale);
			return UnityGameObject.ToLunyObject(go);
		}
	}
}
