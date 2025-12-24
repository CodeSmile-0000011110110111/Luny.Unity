using Luny.Services;
using System;
using UnityEngine;

namespace Luny.Unity.Services
{
	/// <summary>
	/// Unity implementation of time service.
	/// Uses Unity's Time.frameCount and Time.realtimeSinceStartupAsDouble for cross-platform consistency.
	/// </summary>
	public sealed class UnityTimeService : ITimeService
	{
		public Int64 FrameCount => Time.frameCount;

		public Double ElapsedSeconds => Time.realtimeSinceStartupAsDouble;
	}
}
