using Luny.Engine.Bridge.Physics;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge.Physics
{
	public sealed class UnityCollision : LunyCollision
	{
		private LunyCollider _collider;

		private Collision Collision => (Collision)NativeObject;
		private Boolean IsValid => _nativeObject != null;

		public override LunyCollider Collider => _collider ??= IsValid ? new UnityCollider(Collision.collider) : null;
		public override String Tag => IsValid ? Collision.gameObject.tag : null;
		public override String Name => IsValid ? Collision.gameObject.name : null;
		public override String LayerName => IsValid ? LayerMask.LayerToName(Collision.gameObject.layer) : null;
		public override Int32 Layer => IsValid ? Collision.gameObject.layer : 0;

		internal UnityCollision() {}

		public UnityCollision(Collision collision)
			: base(collision) {}

		internal void SetNativeObject(Collision nativeObject) => _nativeObject = nativeObject;

		public override Boolean HasComponent(Type type) => IsValid && Collision.gameObject.TryGetComponent(type, out var _);
	}
}
