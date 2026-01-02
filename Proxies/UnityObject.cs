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
		private GameObject _gameObject;
		private readonly Int32 _nativeID;
		private String _name;
		private Boolean _isDestroyed;
		private Boolean _isEnabled;

		/// <summary>
		/// Gets the wrapped Unity GameObject.
		/// </summary>
		public GameObject GameObject => _gameObject;
		public override NativeID NativeID => _nativeID;
		public override String Name
		{
			get => IsValid ? _gameObject.name : _name;
			set
			{
				if (IsValid)
					_gameObject.name = _name = value;
			}
		}
		public override Boolean IsValid => !_isDestroyed && _gameObject != null;
		public override Boolean IsEnabled
		{
			get => IsValid && _isEnabled;
			set
			{
				if (_isEnabled == value || !IsValid)
					return;

				_isEnabled = value;
				if (_gameObject.activeSelf != _isEnabled)
					_gameObject.SetActive(_isEnabled);

				if (_isEnabled)
					OnEnable?.Invoke();
				else
					OnDisable?.Invoke();
			}
		}

		public UnityObject(GameObject gameObject)
		{
			if (gameObject == null)
				throw new ArgumentNullException(nameof(gameObject), $"{nameof(UnityObject)} {nameof(GameObject)} reference must not be null.");

			_gameObject = gameObject;

			// stored for reference in case object reference unexpectedly becomes null or "missing"
			_nativeID = gameObject.GetEntityId();
			_name = gameObject.name;

			// set initial state
			_isEnabled = gameObject.activeInHierarchy;
		}

		public override void Destroy()
		{
			LunyLogger.LogInfo($"{nameof(UnityObject)}.{nameof(Destroy)}() => {ToString()}", this);
			if (!IsValid)
				return;

			IsEnabled = false; // triggers OnDisable
			OnDestroy?.Invoke();
			_isDestroyed = true; // Mark as destroyed (native destruction happens later)
		}

		public override void DestroyNativeObject()
		{
			LunyLogger.LogInfo($"{nameof(UnityObject)}.{nameof(DestroyNativeObject)}() => {_name} ({_nativeID})", this);
			if (IsValid)
				throw new LunyLifecycleException($"{nameof(DestroyNativeObject)}() called without calling {nameof(Destroy)}() first: {this}");

			UnityEngine.Object.Destroy(_gameObject);
			_gameObject = null;
		}

		public override SystemObject GetNativeObject() => _gameObject;
	}
}
