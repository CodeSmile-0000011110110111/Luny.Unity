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
		private readonly String _name;

		/// <summary>
		/// Gets the wrapped Unity GameObject.
		/// </summary>
		public GameObject GameObject => _gameObject;
		public override NativeID NativeID => _nativeID;
		public override String Name => _name;
		public override Boolean IsValid => _gameObject != null;
		public override Boolean Enabled
		{
			get => IsValid && _gameObject.activeSelf;
			set
			{
				if (IsValid)
					_gameObject.SetActive(value);
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

		public override Object GetNativeObject() => _gameObject;
	}
}
