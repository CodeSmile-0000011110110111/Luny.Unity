using System;
using UnityEngine;

namespace Luny.Unity
{
	// TODO: consider creating a companion behaviour that runs last (for structural changes)
	[DefaultExecutionOrder(Int32.MinValue)]
	internal sealed class UnityLifecycleAdapter : MonoBehaviour
	{
		private static UnityLifecycleAdapter _instance;

		private IEngineLifecycleDispatcher _dispatcher;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void AutoInitialize()
		{
			LunyLog.SetLogger(new UnityLogger());

			var go = new GameObject(nameof(UnityLifecycleAdapter));
			EnsureSingleInstance(go); // safety, in case of incorrect static field reset with "disabled domain reload"
			_instance = go.AddComponent<UnityLifecycleAdapter>();
			DontDestroyOnLoad(go);
		}

		private static void EnsureSingleInstance(GameObject currentObject)
		{
			if (_instance != null)
			{
				Throw.LifecycleAdapterSingletonDuplicationException(
					nameof(UnityLifecycleAdapter),
					_instance.gameObject.name,
					_instance.GetInstanceID(),
					currentObject.name,
					currentObject.GetInstanceID());
			}
		}

		private void Awake()
		{
			EnsureSingleInstance(gameObject);

			_dispatcher = EngineLifecycleDispatcher.Instance;
		}

		private void Update() => _dispatcher.OnUpdate(Time.deltaTime);

		private void LateUpdate() => _dispatcher.OnLateUpdate(Time.deltaTime);

		private void FixedUpdate() => _dispatcher.OnFixedStep(Time.fixedDeltaTime);

		private void OnDestroy()
		{
			// we should not get destroyed with an existing instance (indicates manual removal)
			if (_instance != null)
			{
				Shutdown(); // clear _instance anyway to avoid exiting with singleton reference with "disabled domain reload"
				Throw.LifecycleAdapterPrematurelyRemovedException(nameof(UnityLifecycleAdapter));
			}
		}

		private void OnApplicationQuit() => Shutdown();

		private void Shutdown()
		{
			if (_instance == null)
				return;

			try
			{
				LunyLog.Info("[UnityLifecycleAdapter] Shutting down...");
				_dispatcher?.OnShutdown();
			}
			catch (Exception ex)
			{
				LunyLog.Exception(ex);
			}
			finally
			{
				LunyLog.Info("[UnityLifecycleAdapter] Shutdown complete.");
				LunyLog.SetLogger(null);
				_dispatcher = null;
				_instance = null;
			}
		}
	}
}
