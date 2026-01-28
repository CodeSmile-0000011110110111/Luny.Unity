using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Luny.Unity.Engine.Services
{
	public sealed partial class UnityApplicationService
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
