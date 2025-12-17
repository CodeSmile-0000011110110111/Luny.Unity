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

		private static void EnsureSingleInstance(GameObject current)
		{
			if (_instance != null)
			{
				Throw.LifecycleAdapterSingletonDuplicationException(nameof(UnityLifecycleAdapter), _instance.gameObject.name,
					_instance.GetInstanceID(), current.name, current.GetInstanceID());
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnBeforeSceneLoad() => Initialize();

		private static void Initialize()
		{
			// Logging comes first, we don't want to miss anything
			LunyLogger.SetLogger(new UnityLogger());

			var go = new GameObject(nameof(UnityLifecycleAdapter));
			EnsureSingleInstance(go); // safety check, in case of incorrect static field reset with "disabled domain reload"
			DontDestroyOnLoad(go);

			// Note: Awake runs before _instance is assigned
			_instance = go.AddComponent<UnityLifecycleAdapter>();
			_instance._dispatcher = EngineLifecycleDispatcher.Instance;
		}

		// Note: _instance is null during Awake - this is intentional!
		private void Awake() => EnsureSingleInstance(gameObject);

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
				LunyLogger.LogInfo("[UnityLifecycleAdapter] Shutting down...");
				_dispatcher?.OnShutdown();
			}
			catch (Exception ex)
			{
				LunyLogger.LogException(ex);
			}
			finally
			{
				LunyLogger.LogInfo("[UnityLifecycleAdapter] Shutdown complete.");
				LunyLogger.SetLogger(null);
				_dispatcher = null;
				_instance = null;
			}
		}
	}
}
