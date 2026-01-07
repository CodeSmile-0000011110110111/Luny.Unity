using System.Diagnostics;
using UnityEditor;

namespace Luny.Unity.Engine.Services
{
	public sealed partial class LunyUnityApplicationService
	{
		[Conditional("UNITY_EDITOR")]
		private void ExitPlayModeWhenInEditor()
		{
#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
				EditorApplication.isPlaying = false;
#endif
		}
	}
}
