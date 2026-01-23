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
		protected override ILunyObject OnCreateEmpty(String name) => 
			new UnityGameObject(new GameObject(name));

		protected override ILunyObject OnCreatePrimitive(PrimitiveType type, String name)
		{
			var go = GameObject.CreatePrimitive(type switch
			{
				PrimitiveType.Cube => UnityEngine.PrimitiveType.Cube,
				PrimitiveType.Sphere => UnityEngine.PrimitiveType.Sphere,
				PrimitiveType.Capsule => UnityEngine.PrimitiveType.Capsule,
				PrimitiveType.Cylinder => UnityEngine.PrimitiveType.Cylinder,
				PrimitiveType.Plane => UnityEngine.PrimitiveType.Plane,
				PrimitiveType.Quad => UnityEngine.PrimitiveType.Quad,
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			});
			go.name = name;
			return new UnityGameObject(go);
		}
	}
}
