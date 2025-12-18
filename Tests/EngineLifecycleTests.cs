using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Luny.Unity.Tests
{
	public sealed class EngineLifecycleTests
	{
		public static Boolean DidCreateEngineLifecycleEventOrderMockInstance { get; set; }

		[UnityTest]
		public IEnumerator EngineLifecycleExpectedEventOrder()
		{
			// Mock lifecycle auto-instantiates and is already running (several frames actually) at this point

			Assert.That(DidCreateEngineLifecycleEventOrderMockInstance, Is.True, $"{nameof(EngineLifecycleExpectedEventOrderMock)} was not instantiated");

			yield return new WaitForEndOfFrame();

			yield return null;
			Assert.That(DidCreateEngineLifecycleEventOrderMockInstance, Is.False, $"{nameof(EngineLifecycleExpectedEventOrderMock)} was not shut down");
		}
	}

	public sealed class EngineLifecycleExpectedEventOrderMock : IEngineLifecycleObserver
	{
		private Boolean _didRunStartup;
		private Int32 _fixedStepRunCount;
		private Int32 _updateRunCount;
		private Int32 _lateUpdateRunCount;
		private Boolean _didRunShutdown;

		public EngineLifecycleExpectedEventOrderMock()
		{
			LunyLogger.LogInfo($"{nameof(EngineLifecycleExpectedEventOrderMock)} ctor", this);
			Assert.That(EngineLifecycleTests.DidCreateEngineLifecycleEventOrderMockInstance, Is.False,
				$"{nameof(EngineLifecycleExpectedEventOrderMock)} instantiated multiple times");

			EngineLifecycleTests.DidCreateEngineLifecycleEventOrderMockInstance = true;
		}

		public void OnStartup()
		{
			LunyLogger.LogInfo(nameof(OnStartup), this);
			Assert.That(_didRunStartup, Is.False, $"{nameof(OnStartup)} called more than once");
			Assert.That(_fixedStepRunCount, Is.Zero, $"{nameof(OnFixedStep)} already ran before {nameof(OnStartup)}");
			Assert.That(_updateRunCount, Is.Zero, $"{nameof(OnUpdate)} already ran before {nameof(OnStartup)}");
			Assert.That(_lateUpdateRunCount, Is.Zero, $"{nameof(OnLateUpdate)} already ran before {nameof(OnStartup)}");
			Assert.That(_didRunShutdown, Is.False, $"{nameof(OnShutdown)} already ran before {nameof(OnStartup)}");
			_didRunStartup = true;
		}

		public void OnFixedStep(Double fixedDeltaTime)
		{
			LunyLogger.LogInfo(nameof(OnFixedStep), this);

			_fixedStepRunCount++;
		}

		public void OnUpdate(Double deltaTime)
		{
			LunyLogger.LogInfo(nameof(OnUpdate), this);
			Assert.That(_fixedStepRunCount, Is.GreaterThan(0), $"{nameof(OnUpdate)} ran before {nameof(OnFixedStep)}");

			_updateRunCount++;
		}

		public void OnLateUpdate(Double deltaTime)
		{
			LunyLogger.LogInfo(nameof(OnLateUpdate), this);
			Assert.That(_updateRunCount, Is.GreaterThanOrEqualTo(1), $"{nameof(OnLateUpdate)} ran before {nameof(OnUpdate)}");

			_lateUpdateRunCount++;
			Assert.That(_lateUpdateRunCount, Is.EqualTo(_updateRunCount),
				$"{nameof(OnLateUpdate)} and {nameof(OnUpdate)} did not run same number of times");
		}

		public void OnShutdown()
		{
			LunyLogger.LogInfo(nameof(OnShutdown), this);

			Assert.That(_didRunStartup, Is.True, $"{nameof(OnStartup)} did not run");
			Assert.That(_fixedStepRunCount, Is.GreaterThan(0), $"{nameof(OnFixedStep)} did not run");
			Assert.That(_fixedStepRunCount, Is.LessThanOrEqualTo(_updateRunCount),
				$"{nameof(OnFixedStep)} ran more often than {nameof(OnUpdate)}");
			Assert.That(_updateRunCount, Is.GreaterThan(0), $"{nameof(OnUpdate)} did not run");
			Assert.That(_lateUpdateRunCount, Is.GreaterThan(0), $"{nameof(OnLateUpdate)} did not run");
			Assert.That(_lateUpdateRunCount, Is.EqualTo(_updateRunCount),
				$"{nameof(OnLateUpdate)} and {nameof(OnUpdate)} did not run same number of times");
			Assert.That(_didRunShutdown, Is.False, $"{nameof(OnShutdown)} called more than once");

			_didRunShutdown = true;
			EngineLifecycleTests.DidCreateEngineLifecycleEventOrderMockInstance = false;
		}
	}
}
