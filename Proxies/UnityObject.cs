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
		private readonly GameObject _gameObject;

		public override String Name => _gameObject != null ? _gameObject.name : "<null>";
		public override Boolean IsValid => _gameObject != null;

		/// <summary>
		/// Gets the wrapped Unity GameObject.
		/// </summary>
		public GameObject GameObject => _gameObject;

		public UnityObject(GameObject gameObject) => _gameObject = gameObject;

		public override SystemObject GetNativeObject() => _gameObject;
	}
}
