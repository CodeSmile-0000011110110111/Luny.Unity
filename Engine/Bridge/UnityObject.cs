using Luny.Engine.Bridge;
using Luny.Engine.Identity;
using Luny.Exceptions;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Luny.Unity.Engine.Bridge
{
	/// <summary>
	/// Unity-specific implementation wrapping UnityEngine.GameObject.
	/// </summary>
	public sealed class UnityObject : LunyObject
	{
		private readonly Int32 _nativeObjectID;
		private String _name;
		private Boolean _isDestroyed;
		private Boolean _isEnabled;

		/// <summary>
		/// Gets the wrapped Unity GameObject.
		/// </summary>
		public GameObject GameObject => Cast<GameObject>();
		public override LunyNativeObjectID NativeObjectID => _nativeObjectID;
		public override String Name
		{
			get
			{
				var go = Cast<GameObject>();
				return go != null ? go.name : _name;
			}
			set
			{
				var go = Cast<GameObject>();
				if (go != null)
					go.name = _name = value;
			}
		}
		public override Boolean IsValid => !_isDestroyed && Cast<GameObject>() != null;
		public override Boolean IsEnabled
		{
			get => IsValid && _isEnabled;
			set
			{
				if (_isEnabled == value || !IsValid)
					return;

				_isEnabled = value;
				var go = Cast<GameObject>();
				if (go != null && go.activeSelf != _isEnabled)
					go.SetActive(_isEnabled);

				InvokeOnEnableOrOnDisable(_isEnabled);
			}
		}

		public UnityObject(GameObject gameObject)
			: base(gameObject)
		{
			// stored for reference in case object reference unexpectedly becomes null or "missing"
			_nativeObjectID = gameObject.GetEntityId();
			_name = gameObject.name;

			// set initial state
			_isEnabled = gameObject.activeInHierarchy;
		}

		public override void Destroy()
		{
			// LunyLogger.LogInfo($"{nameof(UnityObject)}.{nameof(Destroy)}() => {this}", this);
			if (!IsValid)
				return;

			IsEnabled = false; // triggers OnDisable
			InvokeOnDestroy();
			_isDestroyed = true; // Mark as destroyed (native destruction happens later)
		}

		public override void DestroyNativeObject()
		{
			// LunyLogger.LogInfo($"{nameof(UnityObject)}.{nameof(DestroyNativeObject)}() => {this} => {_gameObject} ({_nativeID})", this);
			if (IsValid)
				throw new LunyLifecycleException($"{nameof(DestroyNativeObject)}() called without calling {nameof(Destroy)}() first: {this}");

			var go = Cast<GameObject>();
			Object.Destroy(go);
		}
	}
}
