using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Luny.Unity.Tests
{
	public sealed class EngineLifecycleTests
	{
		[UnityTest]
		public IEnumerator EngineLifecycleExpectedEventOrder()
		{
			// Mock lifecycle auto-instantiates and is already running at this point
			// If we get here without exceptions from the mock, the test passes
			yield return new WaitForEndOfFrame();

			// TODO: get observer and test its fields
			Assert.Fail();
		}
	}
}
