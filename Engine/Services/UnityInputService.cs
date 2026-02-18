using Luny.Engine.Services;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of input service.
	/// Subscribes to InputSystem action map callbacks and raises Luny input events.
	/// </summary>
	public sealed class UnityInputService : LunyInputServiceBase
	{
		protected override void OnServiceStartup()
		{
			var asset = InputSystem.actions;
			if (asset == null)
			{
				LunyLogger.LogWarning($"{nameof(UnityInputService)}: No project-wide InputActionAsset found.");
				return;
			}

			LunyLogger.LogInfo($"Using InputActionAsset: {asset.name}, enabled: {asset.enabled}", this);
			foreach (var map in asset.actionMaps)
			{
				if (map.name == "Player")
					map.Enable();
				else
					map.Disable();

				LunyLogger.LogInfo($"== Processing InputActionMap: {map.name}, enabled: {map.enabled}", this);
				foreach (var action in map.actions)
				{
					LunyLogger.LogInfo($"---- Registering InputAction: {action.name}, enabled: {action.enabled}", this);
					action.performed += OnActionPerformed;
					action.canceled += OnActionCanceled;
				}
			}
		}

		protected override void OnServiceShutdown()
		{
			var asset = InputSystem.actions;
			if (asset == null)
				return;

			foreach (var map in asset.actionMaps)
			{
				foreach (var action in map.actions)
				{
					action.performed -= OnActionPerformed;
					action.canceled -= OnActionCanceled;
				}
			}
			asset.Disable();
		}

		private void OnActionPerformed(InputAction.CallbackContext ctx)
		{
			var layout = ctx.action.expectedControlType;
			var type = ctx.action.type;
			//LunyLogger.LogInfo($"Performed: {ctx.action.name}, {ctx}", this);

			if (layout == "Vector2" || String.IsNullOrEmpty(layout) && type == InputActionType.Value)
			{
				var vec = ctx.ReadValue<Vector2>();
				RaiseDirectionalInput(ctx.action.name, new LunyVector2(vec.x, vec.y));
			}
			else if (layout == "Button" || layout == "Axis" || type == InputActionType.Button)
				RaiseButtonInput(ctx.action.name, true, ctx.ReadValue<Single>());
			else
				LunyLogger.LogInfo($"{nameof(UnityInputService)}: Unsupported control layout '{layout}' for action '{ctx.action.name}'");
		}

		private void OnActionCanceled(InputAction.CallbackContext ctx)
		{
			var layout = ctx.action.expectedControlType;
			var type = ctx.action.type;
			//LunyLogger.LogInfo($"Canceled: {ctx.action.name}, {ctx}", this);

			if (layout == "Vector2" || String.IsNullOrEmpty(layout) && type == InputActionType.Value)
				RaiseDirectionalInput(ctx.action.name, LunyVector2.Zero);
			else if (layout == "Button" || layout == "Axis" || type == InputActionType.Button)
				RaiseButtonInput(ctx.action.name, false, 0f);
			// Unsupported types already logged in OnActionPerformed
		}

		/// <summary>
		/// Simulates axis input for testing. In real Unity, this comes from InputSystem callbacks.
		/// </summary>
		internal void SimulateAxisInput(String actionName, LunyVector2 value) => RaiseDirectionalInput(actionName, value);

		/// <summary>
		/// Simulates button press for testing. In real Unity, this comes from InputSystem callbacks.
		/// </summary>
		internal void SimulateButtonInput(String actionName, Boolean pressed, Single analogValue = 1f) =>
			RaiseButtonInput(actionName, pressed, analogValue);
	}
}
