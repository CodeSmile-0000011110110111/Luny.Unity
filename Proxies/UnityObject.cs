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
		private readonly String _name;
		private readonly NativeID _nativeID;

		/// <summary>
		/// Gets the wrapped Unity GameObject.
		/// </summary>
		public GameObject GameObject => _gameObject;
		public override NativeID NativeID => _nativeID;
		public override String Name => _gameObject != null ? _gameObject.name : $"<null> ({_name})";
		public override Boolean IsValid => _gameObject != null;
		public override Boolean Enabled
		{
			get => _gameObject != null && _gameObject.activeSelf;
			set => _gameObject.SetActive(value);
		}

		public UnityObject(GameObject gameObject)
		{
			if (gameObject == null)
				throw new ArgumentNullException(nameof(gameObject), $"{nameof(UnityObject)} GameObject reference must not be null.");

			_gameObject = gameObject;

			// stored for reference in case object reference unexpectedly becomes null or "missing"
			_name = gameObject.name;
			_nativeID = new NativeID(gameObject.GetEntityId());
		}

		public override Object GetNativeObject() => _gameObject;
	}
}
