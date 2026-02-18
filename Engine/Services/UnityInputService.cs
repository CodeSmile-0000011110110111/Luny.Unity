using Luny.Engine.Services;
using System;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of input service.
	/// In real Unity builds, subscribes to InputSystem action map callbacks.
	/// Mock version exposes Simulate* methods for testing.
	/// </summary>
	public sealed class UnityInputService : LunyInputServiceBase, ILunyInputService
	{
		/// <summary>
		/// Simulates axis input for testing. In real Unity, this would come from InputSystem callbacks.
		/// </summary>
		public void SimulateAxisInput(String actionName, LunyVector2 value) =>
			RaiseAxisInput(actionName, value);

		/// <summary>
		/// Simulates button press for testing. In real Unity, this would come from InputSystem callbacks.
		/// </summary>
		public void SimulateButtonInput(String actionName, Boolean pressed, Single analogValue = 1f) =>
			RaiseButtonInput(actionName, pressed, analogValue);
	}
}
