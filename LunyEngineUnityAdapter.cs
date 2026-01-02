using Luny.Diagnostics;
using Luny.Exceptions;
using Luny.Unity.Diagnostics;
using System;
using UnityEngine;

namespace Luny.Unity
{
	// TODO: consider creating a companion behaviour that runs last (for structural changes)

	[DefaultExecutionOrder(Int32.MinValue)]
	internal sealed partial class LunyEngineUnityAdapter : MonoBehaviour
	{
		private static LunyEngineUnityAdapter _instance;

		private ILunyEngine _lunyEngine;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnBeforeSceneLoad() => Initialize();

		// OnStartup deliberately deferred to AfterSceneLoad
		// Problem: in builds during BeforeSceneLoad the SceneManager's root objects list is empty (unlike in editor)
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void OnAfterSceneLoad() => _instance._lunyEngine.OnStartup();

		private static void Initialize()
		{
			// Logging comes first, we don't want to miss anything
			LunyLogger.Logger = new UnityLogger();
			LunyLogger.LogInfo("Initializing...", typeof(LunyEngineUnityAdapter));

			var go = new GameObject(nameof(LunyEngineUnityAdapter));
			EnsureSingleInstance(go); // safety check, in case of incorrect static field reset with "disabled domain reload"
			DontDestroyOnLoad(go);

			// CAUTION: Awake and OnEnable run within AddComponent, before _instance is assigned!
			_instance = go.AddComponent<LunyEngineUnityAdapter>();

			// instantiates LunyEngine by "getting" it
			_instance._lunyEngine = LunyEngine.Instance;

			LunyLogger.LogInfo("Initialization complete.", typeof(LunyEngineUnityAdapter));
		}

		private static void EnsureSingleInstance(GameObject current)
		{
			if (_instance != null)
			{
				LunyThrow.LifecycleAdapterSingletonDuplicationException(nameof(LunyEngineUnityAdapter),
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
			LunyLogger.LogInfo($"{nameof(OnDestroy)} running...", this);

			// we should not get destroyed with an existing instance (indicates manual removal)
			if (_instance != null)
			{
				Shutdown(); // clear _instance anyway to avoid exiting with singleton reference with "disabled domain reload"
				LunyThrow.LifecycleAdapterPrematurelyRemovedException(nameof(LunyEngineUnityAdapter));
			}

			CollectGarbage();
			LunyLogger.LogInfo($"{nameof(OnDestroy)} complete.", this);
		}

		private void OnApplicationQuit()
		{
			LunyLogger.LogInfo($"{nameof(OnApplicationQuit)} running...", this);
			Shutdown();
			LunyLogger.LogInfo($"{nameof(OnApplicationQuit)} complete.", this);
		}

		private void CollectGarbage() => GC.Collect(0, GCCollectionMode.Forced, true);

		~LunyEngineUnityAdapter() => Debug.Log($"[{nameof(LunyEngineUnityAdapter)}] finalized {GetHashCode()}");

		private void Shutdown()
		{
			if (_instance == null)
				return;

			try
			{
				LunyLogger.LogInfo("Shutdown...", this);
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
