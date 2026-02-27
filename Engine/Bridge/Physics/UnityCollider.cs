using Luny.Engine.Bridge.Physics;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge.Physics
{
	public sealed class UnityCollider : LunyCollider
	{
		private Collider Collider => (Collider)_nativeObject;
		private Boolean IsValid => _nativeObject != null;

		public override String Tag => IsValid ? Collider.tag : null;
		public override String Name => IsValid ? Collider.name : null;
		public override String LayerName => IsValid ? LayerMask.LayerToName(Collider.gameObject.layer) : null;
		public override Int32 Layer => IsValid ? Collider.gameObject.layer : 0;

		internal UnityCollider() {}

		public UnityCollider(Collider collider)
			: base(collider) {}

		internal void SetNativeObject(Collider nativeObject) => _nativeObject = nativeObject;

		public override Boolean HasComponent(Type type) => IsValid && Collider.TryGetComponent(type, out var _);
	}
}
