using Luny.Engine.Bridge.Physics;
using System;
using UnityEngine;
using Object = System.Object;

namespace Luny.Unity.Engine.Bridge.Physics
{
	public sealed class UnityCollision : LunyCollision
	{
		public Collision Collision => (Collision)NativeCollision;

		public override LunyCollider Collider { get; } = new UnityCollider();
		public override String Tag => IsValid ? Collision.gameObject.tag : null;
		public override String Name => IsValid ? Collision.gameObject.name : null;
		public override String LayerName => IsValid ? LayerMask.LayerToName(Collision.gameObject.layer) : null;
		public override Int32 Layer => IsValid ? Collision.gameObject.layer : 0;

		public override void SetNativeCollision(Object nativeCollision)
		{
			base.SetNativeCollision(nativeCollision);
			Collider.SetNativeCollider(IsValid ? Collision.collider : null);
		}

		public override Boolean HasComponent(Type type) => IsValid && Collision.gameObject.TryGetComponent(type, out var _);
	}
}
