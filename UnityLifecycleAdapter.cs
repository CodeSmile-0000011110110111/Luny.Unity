using Luny.Diagnostics;
using Luny.Exceptions;
using Luny.Unity.Diagnostics;
using System;
using UnityEngine;

namespace Luny.Unity
{
	// TODO: consider creating a companion behaviour that runs last (for structural changes)

	[DefaultExecutionOrder(Int32.MinValue)]
	internal sealed partial class UnityLifecycleAdapter : MonoBehaviour
	{
		private static UnityLifecycleAdapter _instance;

		private ILunyEngine _lunyEngine;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnBeforeSceneLoad() => Initialize();

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnAfterSceneLoad() =>
			// deferred until after scene load to ensure all objects have been loaded
			_instance._lunyEngine.OnStartup();

		private static void Initialize()
		{
			// Logging comes first, we don't want to miss anything
			LunyLogger.Logger = new UnityLogger();
			LunyLogger.LogInfo("Initializing...", typeof(UnityLifecycleAdapter));

			var go = new GameObject(nameof(UnityLifecycleAdapter));
			EnsureSingleInstance(go); // safety check, in case of incorrect static field reset with "disabled domain reload"
			DontDestroyOnLoad(go);

			// CAUTION: Awake and OnEnable run within AddComponent, before _instance is assigned!
			_instance = go.AddComponent<UnityLifecycleAdapter>();

			// instantiates LunyEngine by "getting" it
			_instance._lunyEngine = LunyEngine.Instance;
		}

		private static void EnsureSingleInstance(GameObject current)
		{
			if (_instance != null)
			{
				LunyThrow.LifecycleAdapterSingletonDuplicationException(nameof(UnityLifecycleAdapter),
					_instance.gameObject.name, _instance.GetInstanceID(), current.name, current.GetInstanceID());
			}
		}

		// Note: _instance is null during Awake - this is intentional!
		private void Awake() => EnsureSingleInstance(gameObject);
		private void FixedUpdate() => _lunyEngine?.OnFixedStep(Time.fixedDeltaTime);
		private void Update() => _lunyEngine?.OnUpdate(Time.deltaTime);
		private void LateUpdate() => _lunyEngine?.OnLateUpdate(Time.deltaTime);

		private void OnDestroy()
		{
			// we should not get destroyed with an existing instance (indicates manual removal)
			if (_instance != null)
			{
				Shutdown(); // clear _instance anyway to avoid exiting with singleton reference with "disabled domain reload"
				LunyThrow.LifecycleAdapterPrematurelyRemovedException(nameof(UnityLifecycleAdapter));
			}
		}

		private void OnApplicationQuit() => Shutdown();

		private void Shutdown()
		{
			if (_instance == null)
				return;

			try
			{
				LunyLogger.LogInfo("Shutting down...", this);
				_lunyEngine?.OnShutdown();
			}
			catch (Exception ex)
			{
				LunyLogger.LogException(ex);
			}
			finally
			{
				LunyLogger.LogInfo("Shutdown complete.", this);
				LunyLogger.Logger = null;
				_lunyEngine = null;
				_instance = null;
			}
		}
	}
}
