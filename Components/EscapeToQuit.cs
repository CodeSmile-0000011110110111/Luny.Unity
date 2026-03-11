using UnityEngine;
using UnityEngine.InputSystem;

namespace Luny.Unity.Components
{
	internal sealed class EscapeToQuit : MonoBehaviour
	{
		private void Update()
		{
			if (Keyboard.current.escapeKey.wasPressedThisFrame)
				Application.Quit();
		}
	}
}
