using Luny.Engine.Bridge;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge
{
	internal sealed class UnityTransform : LunyTransform
	{
		private readonly Transform _nativeTransform;

		internal Transform NativeTransform => _nativeTransform;

		public override LunyVector3 Position
		{
			get => _nativeTransform.position.ToLuny();
			set => _nativeTransform.position = value.ToUnity();
		}

		public override LunyQuaternion Rotation
		{
			get => _nativeTransform.rotation.ToLuny();
			set => _nativeTransform.rotation = value.ToUnity();
		}

		public override LunyVector3 EulerAngles
		{
			get => _nativeTransform.eulerAngles.ToLuny();
			set => _nativeTransform.eulerAngles = value.ToUnity();
		}

		public override LunyVector3 LocalPosition
		{
			get => _nativeTransform.localPosition.ToLuny();
			set => _nativeTransform.localPosition = value.ToUnity();
		}

		public override LunyQuaternion LocalRotation
		{
			get => _nativeTransform.localRotation.ToLuny();
			set => _nativeTransform.localRotation = value.ToUnity();
		}

		public override LunyVector3 LocalEulerAngles
		{
			get => _nativeTransform.localEulerAngles.ToLuny();
			set => _nativeTransform.localEulerAngles = value.ToUnity();
		}

		public override LunyVector3 LocalScale
		{
			get => _nativeTransform.localScale.ToLuny();
			set => _nativeTransform.localScale = value.ToUnity();
		}

		public override LunyVector3 Forward => _nativeTransform.forward.ToLuny();
		public override LunyVector3 Back => (-_nativeTransform.forward).ToLuny();
		public override LunyVector3 Up => _nativeTransform.up.ToLuny();
		public override LunyVector3 Down => (-_nativeTransform.up).ToLuny();
		public override LunyVector3 Right => _nativeTransform.right.ToLuny();
		public override LunyVector3 Left => (-_nativeTransform.right).ToLuny();

		public override LunyTransform Parent
		{
			get => throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(Parent)}");
			set => throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(Parent)}");
		}

		public override LunyTransform Root => throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(Root)}");
		public override Int32 ChildCount => throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(ChildCount)}");
		public override IEnumerable<LunyTransform> Children =>
			throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(Children)}");

		internal UnityTransform(Transform nativeTransform) => _nativeTransform = nativeTransform;

		public override LunyTransform GetChild(Int32 index) =>
			throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(GetChild)}");

		public override void SetParent(LunyTransform parent, Boolean worldPositionStays = true) =>
			throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(SetParent)}");

		public override Boolean IsChildOf(LunyTransform parent) =>
			throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(IsChildOf)}");

		public override Int32 GetSiblingIndex() => throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(GetSiblingIndex)}");

		public override void SetSiblingIndex(Int32 index) =>
			throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(SetSiblingIndex)}");

		public override void SetAsFirstSibling() => throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(SetAsFirstSibling)}");
		public override void SetAsLastSibling() => throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(SetAsLastSibling)}");
		public override void DetachChildren() => throw new NotImplementedException($"{nameof(UnityTransform)}.{nameof(DetachChildren)}");

		public override LunyVector3 TransformPoint(LunyVector3 point) => _nativeTransform.TransformPoint(point.ToUnity()).ToLuny();

		public override LunyVector3 InverseTransformPoint(LunyVector3 point) =>
			_nativeTransform.InverseTransformPoint(point.ToUnity()).ToLuny();

		public override LunyVector3 TransformDirection(LunyVector3 direction) =>
			_nativeTransform.TransformDirection(direction.ToUnity()).ToLuny();

		public override LunyVector3 InverseTransformDirection(LunyVector3 direction) =>
			_nativeTransform.InverseTransformDirection(direction.ToUnity()).ToLuny();

		public override LunyVector3 TransformVector(LunyVector3 vector) => _nativeTransform.TransformVector(vector.ToUnity()).ToLuny();

		public override LunyVector3 InverseTransformVector(LunyVector3 vector) =>
			_nativeTransform.InverseTransformVector(vector.ToUnity()).ToLuny();

		public override void LookAt(LunyVector3 worldPosition) => _nativeTransform.LookAt(worldPosition.ToUnity());

		public override void LookAt(LunyVector3 worldPosition, LunyVector3 worldUp) =>
			_nativeTransform.LookAt(worldPosition.ToUnity(), worldUp.ToUnity());

		public override void LookAt(ILunyObject target) => _nativeTransform.LookAt(target.Transform.Position.ToUnity());

		public override void LookAt(ILunyObject target, LunyVector3 worldUp) =>
			_nativeTransform.LookAt(target.Transform.Position.ToUnity(), worldUp.ToUnity());

		public override void Rotate(LunyVector3 eulerAngles, LunySpace space = LunySpace.Self) =>
			_nativeTransform.Rotate(eulerAngles.ToUnity(), (Space)space);

		public override void Rotate(LunyVector3 axis, Single angle, LunySpace space = LunySpace.Self) =>
			_nativeTransform.Rotate(axis.ToUnity(), angle, (Space)space);

		public override void OrbitAround(LunyVector3 worldPoint, LunyVector3 axis, Single angle) =>
			_nativeTransform.RotateAround(worldPoint.ToUnity(), axis.ToUnity(), angle);

		public override void Translate(LunyVector2 translation, LunySpace space = LunySpace.Self) =>
			_nativeTransform.Translate(new Vector3(translation.X, 0, translation.Y), (Space)space);

		public override void Translate(LunyVector3 translation, LunySpace space = LunySpace.Self) =>
			_nativeTransform.Translate(translation.ToUnity(), (Space)space);
	}
}
