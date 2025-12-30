using Luny.Proxies;
using System;
using UnityEngine;
using Object = System.Object;

namespace Luny.Unity.Proxies
{
	/// <summary>
	/// Unity-specific implementation wrapping UnityEngine.GameObject.
	/// </summary>
	public sealed class UnityObject : LunyObject
	{
		private readonly GameObject _gameObject;
		private readonly Int32 _nativeID;
		private String _name;
		private Boolean _isValid = true;

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
		public override Boolean IsValid => _isValid && _gameObject != null;
		public override Boolean Enabled
		{
			get => IsValid && _gameObject.activeSelf;
			set
			{
				if (!IsValid || _gameObject.activeSelf == value)
					return;

				_gameObject.SetActive(value);
				if (value) OnEnable?.Invoke();
				else OnDisable?.Invoke();
			}
		}

		public UnityObject(GameObject gameObject)
		{
			if (gameObject == null)
				throw new ArgumentNullException(nameof(gameObject), $"{nameof(UnityObject)} GameObject reference must not be null.");

			_gameObject = gameObject;

			// stored for reference in case object reference unexpectedly becomes null or "missing"
			_nativeID = gameObject.GetEntityId();
			_name = gameObject.name;
		}

		public override void Destroy()
		{
			if (!IsValid)
				return;

			Enabled = false; // Disable first (triggers OnDisable via OnEnabledChanged hook)
			OnDestroy?.Invoke(); // Then trigger OnDestroy events
			_isValid = false; // Mark as destroyed (native destruction happens later)
		}

		public override void DestroyNativeObject()
		{
			if (_isValid && _gameObject != null)
				throw new InvalidOperationException(
					$"{nameof(UnityObject)} must not call {nameof(DestroyNativeObject)} directly (object still valid): {this}");

			UnityEngine.Object.Destroy(_gameObject);
		}

		public override Object GetNativeObject() => _gameObject;
	}
}
