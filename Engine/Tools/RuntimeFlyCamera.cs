using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class RuntimeFlyCamera : MonoBehaviour
{
	[Header("Movement Settings")]
	public Single moveSpeed = 10f;
	public Single fastMoveFactor = 3f;
	public Single panSensitivity = 0.05f;
	[Range(0.01f, 1.0f)] public Single movementSmoothing = 0.1f;

	[Header("Look Settings")]
	public Single lookSensitivity = 0.1f;
	[Range(0.01f, 1.0f)] public Single rotationSmoothing = 0.05f;
	public Single maxLookAngle = 90f;

	private Vector3 currentVelocity;
	private Vector2 targetRotation;
	private Vector2 currentRotation;
	private Vector2 rotationVelocity;

	private void Start()
	{
		// Normalize rotation to prevent the "initial snap"
		var euler = transform.eulerAngles;
		targetRotation.x = euler.x > 180 ? euler.x - 360 : euler.x;
		targetRotation.y = euler.y;
		currentRotation = targetRotation;
	}

	private void Update()
	{
		HandleRotation();
		HandleActionInput(); // Panning and Keyboard
	}

	private void HandleRotation()
	{
		// Right Click = Rotate
		var mouse = Mouse.current;
		if (mouse != null && mouse.rightButton.isPressed)
		{
			Cursor.lockState = CursorLockMode.Locked;

			var mouseDelta = mouse.delta.ReadValue() * lookSensitivity;
			targetRotation.y += mouseDelta.x;
			targetRotation.x -= mouseDelta.y;
			targetRotation.x = Mathf.Clamp(targetRotation.x, -maxLookAngle, maxLookAngle);
		}
		else
			Cursor.lockState = CursorLockMode.None;

		currentRotation.x = Mathf.SmoothDampAngle(currentRotation.x, targetRotation.x, ref rotationVelocity.x, rotationSmoothing);
		currentRotation.y = Mathf.SmoothDampAngle(currentRotation.y, targetRotation.y, ref rotationVelocity.y, rotationSmoothing);
		transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
	}

	private void HandleActionInput()
	{
		var targetMoveDir = Vector3.zero;

		// --- 1. Panning (Middle Mouse) ---
		var mouse = Mouse.current;
		if (mouse != null && mouse.middleButton.isPressed)
		{
			var mouseDelta = mouse.delta.ReadValue();
			// Panning moves the camera opposite to mouse direction
			targetMoveDir -= transform.right * (mouseDelta.x * panSensitivity);
			targetMoveDir -= transform.up * (mouseDelta.y * panSensitivity);
		}

		// --- 2. Keyboard Movement (WASDQE) ---
		var kb = Keyboard.current;
		if (kb != null)
		{
			if (kb.wKey.isPressed)
				targetMoveDir += transform.forward;
			if (kb.sKey.isPressed)
				targetMoveDir -= transform.forward;
			if (kb.aKey.isPressed)
				targetMoveDir -= transform.right;
			if (kb.dKey.isPressed)
				targetMoveDir += transform.right;
			if (kb.qKey.isPressed)
				targetMoveDir -= transform.up;
			if (kb.eKey.isPressed)
				targetMoveDir += transform.up;

			// Apply Speed and Sprint
			var speed = kb.leftShiftKey.isPressed ? moveSpeed * fastMoveFactor : moveSpeed;

			// --- 3. Apply Smoothing ---
			// We use SmoothDamp on the position itself toward the desired offset
			var targetPos = transform.position + targetMoveDir * speed;
			transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, movementSmoothing);
		}
	}
}
