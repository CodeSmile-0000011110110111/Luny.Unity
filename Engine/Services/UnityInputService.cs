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
					action.started += OnActionStarted;
					action.performed += OnActionPerformed;
					action.canceled += OnActionCanceled;
				}
			}

			// Tell the system to report activity from devices not yet "paired" to a user
			InputUser.listenForUnpairedDeviceActivity++;
			InputUser.onUnpairedDeviceUsed += OnInputFromUnpairedDevice;
		}

		protected override void OnServiceShutdown()
		{
			if (_inputAsset == null)
				return;

			foreach (var map in _inputAsset.actionMaps)
			{
				foreach (var action in map.actions)
				{
					action.started -= OnActionStarted;
					action.performed -= OnActionPerformed;
					action.canceled -= OnActionCanceled;
				}
			}

			SetControlSchemes("Keyboard&Mouse"); // reset back to default (w/o domain reload the last scheme sticks)
			_inputAsset.Disable();
		}

		private void OnInputFromUnpairedDevice(InputControl control, InputEventPtr eventPtr)
		{
			var device = control.device;
			if (device is Keyboard || device is Mouse)
				return; // Keyboard&Mouse should always remain active

			if (device is Gamepad)
				SetControlSchemes(nameof(Gamepad));
			else if (device is Joystick)
				SetControlSchemes(nameof(Joystick));
			else if (device is Pen)
				SetControlSchemes("Touch");

			InputUser.listenForUnpairedDeviceActivity--;
			InputUser.onUnpairedDeviceUsed -= OnInputFromUnpairedDevice;
		}

		public override void SetControlSchemes(params String[] schemeNames)
		{
			// This tells the Input System: "Only listen to bindings in this group"
			// 'schemeName' must match the name in your InputSystem_Actions window exactly
			_inputAsset.bindingMask = InputBinding.MaskByGroups(schemeNames);
			LunyLogger.LogInfo($"Using Input Control Schemes: {String.Join(", ", schemeNames)}", this);
		}

		private void ProcessInputEvent(InputAction.CallbackContext context)
		{
			var action = context.action;
			var inputEvent = GetOrCreateInputActionEvent(action.name);
			inputEvent.ActionMapName = action.actionMap.name;
			inputEvent.ActionName = action.name;
			inputEvent.Phase = (LunyInputActionPhase)context.phase;
			inputEvent.EventFrame = (int)LunyEngine.Instance.Time.FrameCount;
			HandleInputActionEvent(inputEvent);
		}

		private void OnActionStarted(InputAction.CallbackContext context) => ProcessInputEvent(context);

		private void OnActionPerformed(InputAction.CallbackContext context)
		{
			ProcessInputEvent(context);

			var layout = context.action.expectedControlType;
			var type = context.action.type;
			//LunyLogger.LogInfo($"Performed: {context.action.actionMap.name}.{context.action.name}", this);

			if (layout == "Vector2" || type == InputActionType.Value && String.IsNullOrEmpty(layout))
			{
				var vec = context.ReadValue<Vector2>();
				SetDirectionalInput(context.action.name, new LunyVector2(vec.x, vec.y));
			}
			else if (layout == "Axis")
				SetAxisInput(context.action.name, context.ReadValue<Single>());
			else if (type == InputActionType.Button || layout == "Button")
				SetButtonInput(context.action.name, true, context.ReadValue<Single>());
			else
				LunyLogger.LogInfo($"Unsupported control layout '{layout}' for action '{context.action.name}'", this);
		}

		private void OnActionCanceled(InputAction.CallbackContext context)
		{
			ProcessInputEvent(context);

			var layout = context.action.expectedControlType;
			var type = context.action.type;
			//LunyLogger.LogInfo($"Canceled: {ctx.action.name}, {ctx}", this);

			if (layout == "Vector2" || type == InputActionType.Value && String.IsNullOrEmpty(layout))
				SetDirectionalInput(context.action.name, LunyVector2.Zero);
			else if (layout == "Axis")
				SetAxisInput(context.action.name, 0f);
			else if (type == InputActionType.Button || layout == "Button")
				SetButtonInput(context.action.name, false, 0f);
		}
	}
}
