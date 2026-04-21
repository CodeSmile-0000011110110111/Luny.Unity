using Luny.Engine.Bridge;
using System;
using UnityEngine;

namespace Luny.Unity.Bridge
{
	public static class LunyInterpolationExtensions
	{
		public static LunyRigidbodyInterpolation ToLuny(this RigidbodyInterpolation ip) => (LunyRigidbodyInterpolation)ip;
		public static RigidbodyInterpolation FromLuny(this LunyRigidbodyInterpolation ip) => (RigidbodyInterpolation)ip;
	}

	public sealed class UnityRigidbody : LunyRigidbody
	{
		private readonly Rigidbody _rigidbody;

		public override Boolean IsKinematic { get => _rigidbody.isKinematic; set => _rigidbody.isKinematic = value; }

		public override Boolean UseGravity { get => _rigidbody.useGravity; set => _rigidbody.useGravity = value; }

		public override LunyRigidbodyInterpolation Interpolation
		{
			get => _rigidbody.interpolation.ToLuny();
			set => _rigidbody.interpolation = value.FromLuny();
		}

		private static ForceMode ToUnityForceMode(LunyForceMode forceMode) => forceMode switch
		{
			LunyForceMode.Force => ForceMode.Force,
			LunyForceMode.Acceleration => ForceMode.Acceleration,
			LunyForceMode.Impulse => ForceMode.Impulse,
			LunyForceMode.VelocityChange => ForceMode.VelocityChange,
			var _ => throw new ArgumentOutOfRangeException(nameof(forceMode), $"unhandled ForceMode: {forceMode}"),
		};

		internal UnityRigidbody(ILunyGameObject owner, Rigidbody rigidbody)
			: base(owner) => _rigidbody = rigidbody;

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
	}
}
