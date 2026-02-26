using Luny.Engine.Bridge.Physics;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge.Physics
{
	public sealed class UnityCollision : LunyCollision
	{
		private Collision Collision => (Collision)NativeObject;

		public override String Tag => Collision.gameObject.tag;
		public override String Name => Collision.gameObject.name;
		public override String LayerName => LayerMask.LayerToName(Layer);
		public override Int32 Layer => Collision.gameObject.layer;
		public override Boolean HasComponent(Type type) => Collision.gameObject.TryGetComponent(type, out var _);
	}
}
