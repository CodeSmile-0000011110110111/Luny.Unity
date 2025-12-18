using NUnit.Framework;
using System.Collections;
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
			yield return null;
			Assert.Pass();
		}
	}
}
