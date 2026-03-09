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
		private InputActionAsset _inputAsset = InputSystem.actions;

		// Key: InputUser.id
		private Dictionary<UInt32, PlayerInputProfile> _inputProfiles = new();

		private InputDevice _lastUsedDevice;
		private PlayerInputProfile HostProfile => _inputProfiles[HostUser.id];
		private InputUser HostUser { get; set; }

		private static String[] GetPairedControlSchemes(InputUser user)
		{
			var schemes = new HashSet<String>();
			foreach (var pairedDevice in user.pairedDevices)
			{
				switch (pairedDevice)
				{
					case Gamepad:
						schemes.Add(nameof(Gamepad));
						break;
					case Joystick:
						schemes.Add(nameof(Joystick));
						break;
					case Keyboard:
					case Mouse:
						schemes.Add("Keyboard&Mouse");
						break;
				}
			}

			return schemes.ToArray();
		}

		private void CreateAndAddHostUser()
		{
			HostUser = InputUser.CreateUserWithoutPairedDevices();
			CreatePlayerProfile(HostUser, "Host");
		}

		private PlayerInputProfile CreatePlayerProfile(InputUser user, String userName)
		{
			var userInputAsset = Object.Instantiate(_inputAsset);
			user.AssociateActionsWithUser(userInputAsset);

			LunyLogger.LogInfo($"Using InputActionAsset: {userInputAsset.name}, enabled: {userInputAsset.enabled}", this);
			foreach (var map in userInputAsset.actionMaps)
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

			//var uiModules = GameObject.FindObjectsByType<InputSystemUIInputModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);

			var profile = new PlayerInputProfile { UserId = user.id, UserName = userName, ActionAsset = userInputAsset };
			_inputProfiles.Add(user.id, profile);

			var schemes = GetPairedControlSchemes(user);
			SetControlSchemes(profile, schemes);

			LunyLogger.LogInfo($"Created player profile: {profile}", this);
			return profile;
		}

		private PlayerInputProfile GetPlayerProfile(InputUser deviceUser) => _inputProfiles[deviceUser.id];

		private void PairKeyboardAndMouseToHostUser()
		{
			// Join Mouse+Keyboard to host right away
			if (Keyboard.current != null)
				InputUser.PerformPairingWithDevice(Keyboard.current, HostUser);
			if (Mouse.current != null)
				InputUser.PerformPairingWithDevice(Mouse.current, HostUser);
		}

		protected override void OnServiceStartup()
		{
			_inputAsset = InputSystem.actions;
			if (_inputAsset == null)
			{
				LunyLogger.LogError($"{nameof(UnityInputService)}: No project-wide InputActionAsset found.");
				return;
			}

			CreateAndAddHostUser();
			PairKeyboardAndMouseToHostUser();

			// Tell the system to report activity from devices not yet "paired" to a user
			InputUser.listenForUnpairedDeviceActivity++;
			InputUser.onUnpairedDeviceUsed += OnInputFromUnpairedDevice;
		}

		protected override void OnServiceShutdown()
		{
			if (_inputAsset == null)
				return;

			InputUser.listenForUnpairedDeviceActivity = 0;
			InputUser.onUnpairedDeviceUsed -= OnInputFromUnpairedDevice;

			foreach (var profile in _inputProfiles.Values)
			{
				profile.Pawns.Clear();

				var inputAsset = (InputActionAsset)profile.ActionAsset;
				if (inputAsset == null)
					continue;

				foreach (var map in inputAsset.actionMaps)
				{
					foreach (var action in map.actions)
					{
						action.started -= OnActionStarted;
						action.performed -= OnActionPerformed;
						action.canceled -= OnActionCanceled;
					}
				}

				Object.Destroy(inputAsset);
			}
			_inputProfiles.Clear();

			foreach (var inputUser in InputUser.all)
			{
				if (inputUser.valid)
					inputUser.UnpairDevicesAndRemoveUser();
			}

			_inputAsset.bindingMask = null; // reset back to default (w/o domain reload the last scheme sticks)
			_inputAsset.Disable();
			_inputAsset = null;
		}

		private void OnInputFromUnpairedDevice(InputControl control, InputEventPtr eventPtr)
		{
			// Handle Keyboard & Mouse in case they get plugged in after play started
			var device = control.device;
			if (device is Keyboard || device is Mouse)
				PairKeyboardAndMouseToHostUser();

			/*
			// This check is essential to prevent "accidental" processing merely by input drift or gyro.
			if (!control.IsPressed())
				return;
				*/
		}

		public void SetControlSchemes(PlayerInputProfile profile, params String[] schemeNames)
		{
			// This tells the Input System: "Only listen to bindings in this group"
			// 'schemeName' must match the name in your InputSystem_Actions window exactly
			var asset = (InputActionAsset)profile.ActionAsset;
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
			_inputAsset.bindingMask = InputBinding.MaskByGroups(schemeNames);
		}

		public override void AssignUserToLastDevice(String userName, ILunyObject lunyObject)
		{
			if (_lastUsedDevice == null)
				return;

			var deviceUser = InputUser.FindUserPairedToDevice(_lastUsedDevice);
			var profile = deviceUser.HasValue
				? CreatePlayerProfile(InputUser.PerformPairingWithDevice(_lastUsedDevice), userName)
				: GetPlayerProfile(deviceUser.Value);

			profile.Pawns.Add(lunyObject);
		}

		private void ProcessInputEvent(InputAction.CallbackContext context)
		{
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
	}
}
