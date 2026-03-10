using Luny.Engine.Bridge;
using Luny.Engine.Services;
using Luny.Unity.Engine.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using Object = UnityEngine.Object;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of input service.
	/// Subscribes to InputSystem action map callbacks and raises Luny input events.
	/// </summary>
	public sealed class UnityInputService : LunyInputServiceBase
	{
		private InputActionAsset _globalInputActions;

		// Key: InputUser.id
		private Dictionary<UInt32, PlayerInputProfile> _inputProfiles = new();

		private InputDevice _lastUsedDevice;
		private UInt32 _hostUserId;
		private PlayerInputProfile HostProfile => _inputProfiles[_hostUserId];

		private static InputUser? UnpairUserFromDevice(InputDevice device)
		{
			if (device == null)
				return null;

			var pairedUser = InputUser.FindUserPairedToDevice(device);
			if (pairedUser.HasValue)
			{
				LunyLogger.LogInfo($"Unpairing device {device.name} from current user {pairedUser.Value.id} ...");
				pairedUser.Value.UnpairDevice(device);
			}
			return pairedUser;
		}

		private InputUser? GetHostUser() => InputUser.all.Find(_hostUserId);

		private Boolean TryGetPlayerProfile(InputUser deviceUser, out PlayerInputProfile profile) =>
			_inputProfiles.TryGetValue(deviceUser.id, out profile);

		private PlayerInputProfile GetPlayerProfile(String userName)
		{
			foreach (var profile in _inputProfiles.Values)
			{
				if (profile.UserName == userName)
					return profile;
			}
			return null;
		}

		private PlayerInputProfile GetPlayerProfile(UInt32 userId)
		{
			foreach (var profile in _inputProfiles.Values)
			{
				if (profile.UserId == userId)
					return profile;
			}
			return null;
		}

		private void CreateHostUserAndPairAllDevices()
		{
			var hostUser = InputUser.CreateUserWithoutPairedDevices();
			_hostUserId = hostUser.id;
			var hostProfile = CreatePlayerProfile(hostUser, "{InputHost}");

			foreach (var device in InputSystem.devices)
				InputUser.PerformPairingWithDevice(device, hostUser);
		}

		private PlayerInputProfile CreatePlayerProfile(InputUser user, String userName)
		{
			var userInputAsset = Object.Instantiate(_globalInputActions);

			// order matters
			user.AssociateActionsWithUser(userInputAsset);
			user.ActivateControlScheme(null);

			foreach (var map in userInputAsset.actionMaps)
			{
				if (map.name == "Player")
					map.Enable();

				if (map.enabled)
					LunyLogger.LogInfo($"Input Action Map enabled: {map.name}");

				foreach (var action in map.actions)
				{
					action.started += OnActionStarted;
					action.performed += OnActionPerformed;
					action.canceled += OnActionCanceled;
				}
			}

			//var uiModules = GameObject.FindObjectsByType<InputSystemUIInputModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
			var profile = new PlayerInputProfile { UserId = user.id, UserName = userName, Actions = userInputAsset, UiInput = null };
			_inputProfiles.Add(user.id, profile);

			LunyLogger.LogInfo($"User {user} is using actions: {userInputAsset.name}", this);
			return profile;
		}

		private void DestroyPlayerProfile(PlayerInputProfile profile)
		{
			if (profile == null || profile.UserId == InputUser.InvalidId)
				return;

			var user = InputUser.all.Find(profile.UserId);
			if (user.HasValue && user.Value.valid)
			{
				var hostUser = GetHostUser();
				foreach (var device in user.Value.pairedDevices)
				{
					user.Value.UnpairDevice(device);

					// re-pair device with host
					if (hostUser.HasValue)
						InputUser.PerformPairingWithDevice(device, hostUser.Value);
				}

				user.Value.UnpairDevicesAndRemoveUser();
			}

			var inputAsset = profile.GetActions();
			if (inputAsset != null)
			{
				foreach (var map in inputAsset.actionMaps)
				{
					map.Disable();

					foreach (var action in map.actions)
					{
						action.started -= OnActionStarted;
						action.performed -= OnActionPerformed;
						action.canceled -= OnActionCanceled;
					}
				}

				Object.Destroy(inputAsset);
			}

			_inputProfiles.Remove(profile.UserId);
			profile.UserId = InputUser.InvalidId;
			profile.Pawns.Clear();
			profile.Actions = null;
			profile.UiInput = null;
		}

		public override void UnassignUser(String userName)
		{
			var profile = GetPlayerProfile(userName);
			if (profile != null)
				DestroyPlayerProfile(profile);
		}

		public override void AssignUserToLastDevice(String userName, ILunyObject lunyObject)
		{
			if (_lastUsedDevice == null)
			{
				LunyLogger.LogWarning("Can't assign user to last input device: no input received this frame", this);
				return;
			}

			AssignUserToDevice(userName, _lastUsedDevice, lunyObject);
		}

		private void AssignUserToDevice(String userName, InputDevice device, ILunyObject lunyObject)
		{
			var pairedUser = InputUser.FindUserPairedToDevice(device);
			if (pairedUser.HasValue && pairedUser.Value != GetHostUser())
			{
				LunyLogger.LogWarning($"{device.name} already paired with user {pairedUser.Value.id}: " +
				                      $"{GetPlayerProfile(pairedUser.Value.id)?.UserName}");
				return;
			}

			pairedUser = UnpairUserFromDevice(device);
			pairedUser = InputUser.PerformPairingWithDevice(device);

			// pair Mouse/Keyboard together
			var otherDevice = device is Keyboard ? Mouse.current?.device : device is Mouse ? Keyboard.current?.device : null;
			if (otherDevice != null)
			{
				UnpairUserFromDevice(otherDevice);
				InputUser.PerformPairingWithDevice(otherDevice, pairedUser.Value);
			}
			LunyLogger.LogInfo($"Paired device {device.name} to user {pairedUser.Value.id}.");

			if (!TryGetPlayerProfile(pairedUser.Value, out var profile))
				profile = CreatePlayerProfile(pairedUser.Value, userName);

			profile.Pawns.Add(lunyObject);
			pairedUser.Value.ActivateControlScheme(null);
		}

		public void SetControlSchemes(PlayerInputProfile profile, params String[] schemeNames)
		{
			// This tells the Input System: "Only listen to bindings in this group"
			// 'schemeName' must match the name in your InputSystem_Actions window exactly
			var asset = (InputActionAsset)profile.Actions;
			if (asset != null)
			{
				LunyLogger.LogInfo($"User Input Control Schemes: {String.Join(", ", schemeNames)}", this);
				asset.bindingMask = InputBinding.MaskByGroups(schemeNames);
			}
		}

		public override void SetControlSchemes(params String[] schemeNames)
		{
			LunyLogger.LogInfo($"Global Input Control Schemes: {String.Join(", ", schemeNames)}", this);

			// This tells the Input System: "Only listen to bindings in this group"
			// 'schemeName' must match the name in your InputSystem_Actions window exactly
			_globalInputActions.bindingMask = InputBinding.MaskByGroups(schemeNames);
		}

		private void ProcessInputEvent(InputAction.CallbackContext context)
		{
			//LunyLogger.LogInfo($"{context.control.device.name} => {context.action.name} ({context.action.phase})", nameof(ProcessInputEvent));
			_lastUsedDevice = context.control.device;

			var action = context.action;
			var inputEvent = GetOrCreateInputActionEvent(action.name);
			inputEvent.ActionMapName = action.actionMap.name;
			inputEvent.ActionName = action.name;
			inputEvent.UserName = GetUserName(_lastUsedDevice);
			inputEvent.Phase = (LunyInputActionPhase)context.phase;
			HandleInputActionEvent(inputEvent);
		}

		private String GetUserName(InputDevice device)
		{
			var user = InputUser.FindUserPairedToDevice(device);
			return user.HasValue && _inputProfiles.TryGetValue(user.Value.id, out var profile) ? profile.UserName : null;
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

		private void PairDeviceWithHost(InputDevice device)
		{
			var hostUser = GetHostUser();
			if (hostUser.HasValue)
				InputUser.PerformPairingWithDevice(device, hostUser.Value);
		}

		private void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			LunyLogger.LogInfo($"{change} device: {device.name}", this);

			switch (change)
			{
				case InputDeviceChange.Added:
					// Automatically give new devices to the Host
					PairDeviceWithHost(device);
					break;
				case InputDeviceChange.Removed:
					var user = InputUser.FindUserPairedToDevice(device);
					if (user.HasValue)
						DestroyPlayerProfile(GetPlayerProfile(user.Value.id));
					break;
				case InputDeviceChange.Disconnected:
					break;
				case InputDeviceChange.Reconnected:
					break;
				case InputDeviceChange.Enabled:
					break;
				case InputDeviceChange.Disabled:
					break;
				// probably needn't be handled
				case InputDeviceChange.UsageChanged:
					break;
				case InputDeviceChange.ConfigurationChanged:
					break;
				case InputDeviceChange.SoftReset:
					break;
				case InputDeviceChange.HardReset:
					break;
				// deprecated:
				//case InputDeviceChange.Destroyed: break;
				default:
					LunyLogger.LogWarning($"Unhandled {nameof(InputDeviceChange)}: {change}");
					break;
			}
		}

		private void OnInputFromUnpairedDevice(InputControl control, InputEventPtr eventPtr)
		{
			LunyLogger.LogInfo($"Unpaired device: {control.device.name}");
			PairDeviceWithHost(control.device);
		}

		protected override void OnServiceInitialize()
		{
			_globalInputActions = InputSystem.actions;
			if (_globalInputActions == null)
				LunyLogger.LogError("Project-wide Actions not assigned in: Project Settings / Input System Package", this);

			_globalInputActions?.Disable();
		}

		protected override void OnServiceStartup()
		{
			CreateHostUserAndPairAllDevices();

			// Tell the system to report activity from devices not yet "paired" to a user
			InputUser.listenForUnpairedDeviceActivity++;
			InputUser.onUnpairedDeviceUsed += OnInputFromUnpairedDevice;
			InputSystem.onDeviceChange += OnDeviceChange;
		}

		protected override void OnServiceShutdown()
		{
			if (_globalInputActions == null)
				return;

			InputUser.listenForUnpairedDeviceActivity--;
			InputUser.onUnpairedDeviceUsed -= OnInputFromUnpairedDevice;
			InputSystem.onDeviceChange -= OnDeviceChange;

			foreach (var profile in _inputProfiles.Values.ToArray())
				DestroyPlayerProfile(profile);
			_inputProfiles.Clear(); // just in case, it should be empty by now

			// destroying profiles should have unpaired devices and removed users, nevertheless better err on the safe side
			foreach (var inputUser in InputUser.all)
			{
				if (inputUser.valid)
					inputUser.UnpairDevicesAndRemoveUser();
			}

			_globalInputActions.bindingMask = null; // reset back to default (w/o domain reload the last scheme sticks)
			_globalInputActions.Disable();
			_globalInputActions = null;
		}
	}

	internal static class PlayerInputProfileExt
	{
		public static InputActionAsset GetActions(this PlayerInputProfile profile) => (InputActionAsset)profile.Actions;
	}
}
