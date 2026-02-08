using Luny.Engine.Bridge;
using Luny.Engine.Bridge.Enums;
using Luny.Engine.Services;
using Luny.Unity.Engine.Bridge;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	public sealed class UnityObjectService : LunyObjectServiceBase, ILunyObjectService
	{
		public override ILunyObject CreateEmpty(String name) => UnityGameObject.ToLunyObject(new GameObject(name));

		public override ILunyObject CreatePrimitive(String name, LunyPrimitiveType type)
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
			return UnityGameObject.ToLunyObject(go);
		}

		public override ILunyObject CreateFromPrefab(ILunyPrefab prefab)
		{
			if (prefab is not UnityPrefab unityPrefab)
				throw new ArgumentException($"Prefab must be of type {nameof(UnityPrefab)}", nameof(prefab));

			var go = unityPrefab.Instantiate();
			return UnityGameObject.ToLunyObject(go);
		}
	}
}
