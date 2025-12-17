using System;

namespace Luny.Unity
{
	using UnityEngine;

	// TODO: consider creating a companion behaviour that runs last (for structural changes)
	[DefaultExecutionOrder(int.MinValue)]
	internal sealed class UnityLifecycleAdapter : MonoBehaviour
	{
		private static UnityLifecycleAdapter _instance;

		private IEngineLifecycleDispatcher _dispatcher;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void AutoInitialize()
		{
			// Force creation before first scene loads
			var go = new GameObject(nameof(UnityLifecycleAdapter));
			_instance = go.AddComponent<UnityLifecycleAdapter>();
			DontDestroyOnLoad(go);
		}

		private void Awake()
		{
			if (_instance != null)
			{
				Throw.LifecycleAdapterSingletonDuplicationException(
					nameof(UnityLifecycleAdapter),
					_instance.gameObject.name,
					_instance.GetInstanceID(),
					gameObject.name,
					GetInstanceID());
			}

			_dispatcher = EngineLifecycleDispatcher.Instance;
		}

		private void Update() => _dispatcher.OnUpdate(Time.deltaTime);

		private void LateUpdate()
		{
			_dispatcher.OnLateUpdate(Time.deltaTime);
		}

		private void FixedUpdate() => _dispatcher.OnFixedStep(Time.fixedDeltaTime);

		private void OnDestroy()
		{
			// we should not get destroyed with an existing instance (indicates manual removal)
			if (_instance != null)
			{
				Shutdown();
				Throw.LifecycleAdapterPrematurelyRemovedException(nameof(UnityLifecycleAdapter));
			}
		}

		private void OnApplicationQuit()
		{
			Shutdown();
		}

		private void Shutdown()
		{
			if (_instance == null)
				return;

			try
			{
				Log.Info("[Luny] Shutting down...");
				_dispatcher?.OnShutdown();
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}
			finally
			{
				_dispatcher = null;
				_instance = null;
				Log.Info("[Luny] Shutdown complete.");
			}
		}
	}
}
