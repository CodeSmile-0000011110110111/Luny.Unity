using Luny.Engine.Bridge;
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
