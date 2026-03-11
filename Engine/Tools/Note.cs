using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Luny.Unity.Engine
{
	public sealed class Note : MonoBehaviour
	{
#if UNITY_EDITOR
		//[HideInInspector] public String Title = nameof(Note);
		[HideInInspector] public String Text = "Type your note here...";
		public HelpBoxMessageType Kind = HelpBoxMessageType.None;
#else
		private void Awake() => Destroy(this); // Removes the component at runtime in the build
#endif
	}
}
