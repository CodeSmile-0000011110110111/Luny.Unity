using Luny.Engine.Services;
using System;
using UnityEngine;

namespace Luny.Unity.Engine.Services
{
	/// <summary>
	/// Unity implementation of time service.
	/// Uses Unity's Time.frameCount and Time.realtimeSinceStartupAsDouble for cross-platform consistency.
	/// </summary>
	public sealed class LunyUnityTimeService : LunyTimeServiceBase, ILunyTimeService
	{
		public Int64 EngineFrameCount => Time.frameCount;
		public Double ElapsedSeconds => Time.realtimeSinceStartupAsDouble;
		protected override void OnServiceInitialize() {}
		protected override void OnServiceStartup() {}
		protected override void OnServiceShutdown() {}
	}
}
