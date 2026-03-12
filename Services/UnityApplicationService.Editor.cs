using System.Diagnostics;
using UnityEditor;

namespace Luny.Unity.Services
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
