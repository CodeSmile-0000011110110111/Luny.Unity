using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

namespace Luny.Unity
{
	public static class InputUserExtensions
	{
		public static InputUser? Find(this ReadOnlyArray<InputUser> users, uint id)
		{
			foreach (var user in users)
			{
				if (user.valid && user.id == id)
					return user;
			}
			return null;
		}

	}
}
