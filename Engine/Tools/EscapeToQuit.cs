using UnityEngine;
using UnityEngine.InputSystem;

public sealed class EscapeToQuit : MonoBehaviour
{
	private void Update()
	{
		if (Keyboard.current.escapeKey.wasPressedThisFrame)
			Application.Quit();
	}
}
