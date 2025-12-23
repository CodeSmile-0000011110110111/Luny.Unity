using System.Diagnostics;
using UnityEditor;

namespace Luny.Unity.Providers
{
	public sealed partial class UnityApplicationServiceProvider
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
