using Luny.Engine.Bridge;
using Luny.Engine.Services;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of input service.
	/// Subscribes to InputSystem action map callbacks and raises Luny input events.
	/// </summary>
	public sealed class UnityInputService : LunyInputServiceBase
	{
		private InputActionAsset _inputAsset = InputSystem.actions;

		protected override void OnServiceStartup()
		{
			_inputAsset = InputSystem.actions;
			if (_inputAsset == null)
			{
				LunyLogger.LogWarning($"{nameof(UnityInputService)}: No project-wide InputActionAsset found.");
				return;
			}

			LunyLogger.LogInfo($"Using InputActionAsset: {_inputAsset.name}, enabled: {_inputAsset.enabled}", this);
			foreach (var map in _inputAsset.actionMaps)
			{
				if (map.name == "Player")
					map.Enable();
				else
					map.Disable();

				foreach (var action in map.actions)
				{
					action.performed += OnActionPerformed;
					action.canceled += OnActionCanceled;
				}
			}

			// Tell the system to report activity from devices not yet "paired" to a user
			InputUser.listenForUnpairedDeviceActivity++;
			InputUser.onUnpairedDeviceUsed += OnInputFromUnpairedDevice;
		}

		private void OnInputFromUnpairedDevice(InputControl control, InputEventPtr eventPtr)
		{
			var device = control.device;
			if (device is Keyboard || device is Mouse)
				return; // Keyboard&Mouse is active by default, we don't want to lock it in

			if (device is Gamepad)
				SetControlScheme(nameof(Gamepad));
			else if (device is Joystick)
				SetControlScheme(nameof(Joystick));
			else if (device is Pen)
				SetControlScheme("Touch");

			InputUser.listenForUnpairedDeviceActivity--;
			InputUser.onUnpairedDeviceUsed -= OnInputFromUnpairedDevice;
		}

		public override void SetControlScheme(String schemeName)
		{
			// This tells the Input System: "Only listen to bindings in this group"
			// 'schemeName' must match the name in your InputSystem_Actions window exactly
			_inputAsset.bindingMask = InputBinding.MaskByGroup(schemeName);
			LunyLogger.LogInfo($"Using Input Control Scheme: {schemeName}", this);
		}

		protected override void OnServiceShutdown()
		{
			if (_inputAsset == null)
				return;

			foreach (var map in _inputAsset.actionMaps)
			{
				foreach (var action in map.actions)
				{
					action.performed -= OnActionPerformed;
					action.canceled -= OnActionCanceled;
				}
			}

			_inputAsset.Disable();
		}

		private void OnActionPerformed(InputAction.CallbackContext ctx)
		{
			var layout = ctx.action.expectedControlType;
			var type = ctx.action.type;
			//LunyLogger.LogInfo($"Performed: {ctx.action.name}, {ctx}", this);

			if (layout == "Vector2" || type == InputActionType.Value && String.IsNullOrEmpty(layout))
			{
				var vec = ctx.ReadValue<Vector2>();
				SetDirectionalInput(ctx.action.name, new LunyVector2(vec.x, vec.y));
			}
			else if (layout == "Axis")
				SetAxisInput(ctx.action.name, ctx.ReadValue<Single>());
			else if (type == InputActionType.Button || layout == "Button")
				SetButtonInput(ctx.action.name, true, ctx.ReadValue<Single>());
			else
				LunyLogger.LogInfo($"Unsupported control layout '{layout}' for action '{ctx.action.name}'", this);
		}

		private void OnActionCanceled(InputAction.CallbackContext ctx)
		{
			var layout = ctx.action.expectedControlType;
			var type = ctx.action.type;
			//LunyLogger.LogInfo($"Canceled: {ctx.action.name}, {ctx}", this);

			if (layout == "Vector2" || type == InputActionType.Value && String.IsNullOrEmpty(layout))
				SetDirectionalInput(ctx.action.name, LunyVector2.Zero);
			else if (layout == "Axis")
				SetAxisInput(ctx.action.name, 0f);
			else if (type == InputActionType.Button || layout == "Button")
				SetButtonInput(ctx.action.name, false, 0f);
		}
	}
}
