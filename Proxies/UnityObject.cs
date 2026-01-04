using Luny.Diagnostics;
using Luny.Exceptions;
using Luny.Proxies;
using System;
using UnityEngine;
using SystemObject = System.Object;

namespace Luny.Unity.Proxies
{
	/// <summary>
	/// Unity-specific implementation wrapping UnityEngine.GameObject.
	/// </summary>
	public sealed class UnityObject : LunyObject
	{
		private readonly Int32 _nativeID;
		private String _name;
		private Boolean _isDestroyed;
		private Boolean _isEnabled;

		/// <summary>
		/// Gets the wrapped Unity GameObject.
		/// </summary>
		public GameObject GameObject => As<GameObject>();
		public override NativeID NativeID => _nativeID;
		public override String Name
		{
			get
			{
				var go = As<GameObject>();
				return go != null ? go.name : _name;
			}
			set
			{
				var go = As<GameObject>();
				if (go != null)
					go.name = _name = value;
			}
		}
		public override Boolean IsValid => !_isDestroyed && As<GameObject>() != null;
		public override Boolean IsEnabled
		{
			get => IsValid && _isEnabled;
			set
			{
				if (_isEnabled == value || !IsValid)
					return;

				_isEnabled = value;
				var go = As<GameObject>();
				if (go != null && go.activeSelf != _isEnabled)
					go.SetActive(_isEnabled);

				if (_isEnabled)
					InvokeOnEnable();
				else
					InvokeOnDisable();
			}
		}

		public UnityObject(GameObject gameObject) : base(gameObject)
		{
			// stored for reference in case object reference unexpectedly becomes null or "missing"
			_nativeID = gameObject.GetEntityId();
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

			var go = As<GameObject>();
			UnityEngine.Object.Destroy(go);
		}
	}
}
