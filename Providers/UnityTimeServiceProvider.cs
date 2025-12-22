using System;
using UnityEngine;

namespace Luny.Unity.Providers
{
	/// <summary>
	/// Unity implementation of time service provider.
	/// Uses Unity's Time.frameCount and Time.realtimeSinceStartupAsDouble for cross-platform consistency.
	/// </summary>
	public sealed class UnityTimeServiceProvider : ITimeServiceProvider
	{
		public Int64 FrameCount => Time.frameCount;

		public Double ElapsedSeconds => Time.realtimeSinceStartupAsDouble;
	}
}
