using Luny.Engine.Bridge;
using System;
using UnityEngine;

namespace Luny.Unity.Bridge
{
	internal sealed class UnityRigidbody : LunyRigidbody
	{
		private readonly Rigidbody _rigidbody;

		internal UnityRigidbody(ILunyObject owner, Rigidbody rigidbody)
			: base(owner) => _rigidbody = rigidbody;

		public override void SetKinematic(Boolean enabled) => _rigidbody.isKinematic = enabled;

		public override void SetGravityEnabled(Boolean enabled) => _rigidbody.useGravity = enabled;

		public override void MovePosition(LunyVector3 delta, LunyTransformSpace space)
		{
			if (space == LunyTransformSpace.Local)
			{
				var worldDelta = _rigidbody.transform.TransformDirection(delta.ToUnity());
				_rigidbody.MovePosition(_rigidbody.position + worldDelta);
			}
			else
				_rigidbody.MovePosition(_rigidbody.position + delta.ToUnity());
		}

		public override void MoveRotation(LunyVector3 eulerDelta, LunyTransformSpace space)
		{
			if (space == LunyTransformSpace.Local)
				_rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(eulerDelta.ToUnity()));
			else
				_rigidbody.MoveRotation(Quaternion.Euler(eulerDelta.ToUnity()) * _rigidbody.rotation);
		}

		public override void AddForce(LunyVector3 force, LunyForceMode forceMode, LunyTransformSpace space)
		{
			var unityForceMode = ToUnityForceMode(forceMode);
			if (space == LunyTransformSpace.Local)
				_rigidbody.AddRelativeForce(force.ToUnity(), unityForceMode);
			else
				_rigidbody.AddForce(force.ToUnity(), unityForceMode);
		}

		public override void AddForceAtPosition(LunyVector3 force, LunyVector3 worldPosition, LunyForceMode forceMode) =>
			_rigidbody.AddForceAtPosition(force.ToUnity(), worldPosition.ToUnity(), ToUnityForceMode(forceMode));

		public override void AddTorque(LunyVector3 torque, LunyForceMode forceMode, LunyTransformSpace space)
		{
			var unityForceMode = ToUnityForceMode(forceMode);
			if (space == LunyTransformSpace.Local)
				_rigidbody.AddRelativeTorque(torque.ToUnity(), unityForceMode);
			else
				_rigidbody.AddTorque(torque.ToUnity(), unityForceMode);
		}

		private static ForceMode ToUnityForceMode(LunyForceMode forceMode) => forceMode switch
		{
			LunyForceMode.Force => ForceMode.Force,
			LunyForceMode.Acceleration => ForceMode.Acceleration,
			LunyForceMode.Impulse => ForceMode.Impulse,
			LunyForceMode.VelocityChange => ForceMode.VelocityChange,
			_ => ForceMode.Force,
		};
	}
}
