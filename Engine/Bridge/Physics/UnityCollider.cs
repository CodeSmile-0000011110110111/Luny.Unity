using Luny.Engine.Bridge;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge.Physics
{
	public sealed class UnityCollider : LunyCollider
	{
		private Collider Collider => (Collider)NativeCollider;

		public override String Tag => IsValid ? Collider.tag : null;
		public override String Name => IsValid ? Collider.name : null;
		public override String LayerName => IsValid ? LayerMask.LayerToName(Collider.gameObject.layer) : null;
		public override Int32 Layer => IsValid ? Collider.gameObject.layer : 0;

		public override Boolean HasComponent(Type type) => IsValid && Collider.TryGetComponent(type, out var _);
	}
}
