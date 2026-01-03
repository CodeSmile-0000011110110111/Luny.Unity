using Luny.Diagnostics;
using Luny.Exceptions;
using Luny.Unity.Diagnostics;
using System;
using UnityEngine;

namespace Luny.Unity
{
	/// <summary>
	/// Engine hook. Provides no public or internal API.
	/// </summary>
	[DefaultExecutionOrder(Int32.MinValue)] // Run before all other scripts
	[AddComponentMenu("GameObject/")] // Do not list in "Add Component" menu
	[DisallowMultipleComponent]
	internal sealed partial class LunyEngineUnityAdapter : MonoBehaviour, IEngineAdapter
	{
		// intentionally remains private - user code must use LunyEngine.Instance!
		private static LunyEngineUnityAdapter s_Instance;

		// hold on to LunyEngine reference
		private ILunyEngine _lunyEngine;

		// Note: in builds the SceneManager's root objects list is empty in 'BeforeSceneLoad' (unlike in editor)
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnBeforeSceneLoad() => Initialize();

		private static void Initialize()
		{
			// Logging comes first, we don't want to miss anything
			LunyLogger.Logger = new UnityLogger();
			LunyLogger.LogInfo("Initializing...", typeof(LunyEngineUnityAdapter));

			var go = new GameObject(nameof(LunyEngineUnityAdapter));
			EnsureSingleInstance(go); // safety check, in case of incorrect static field reset with "disabled domain reload"
			DontDestroyOnLoad(go);

			// Note: Awake and OnEnable run within AddComponent, within them s_Instance is and remains null!
			var unityAdapter = go.AddComponent<LunyEngineUnityAdapter>();
			EnsureSingleInstance(go); // double-safety check that nothing assigned s_Instance during Awake/OnEnable
			s_Instance = unityAdapter;

			LunyLogger.LogInfo("Initialization complete.", typeof(LunyEngineUnityAdapter));
		}

		private static void EnsureSingleInstance(GameObject current)
		{
			if (s_Instance != null)
			{
				LunyThrow.EngineAdapterSingletonDuplicationException(nameof(LunyEngineUnityAdapter),
					s_Instance.gameObject.name, s_Instance.GetInstanceID(), current.name, current.GetInstanceID());
			}
		}

		private void Awake()
		{
			// Note: s_Instance is and remains null during Awake - this is intentional!
			EnsureSingleInstance(gameObject);

			_lunyEngine = LunyEngine.CreateInstance(this);
		}

		private void Start() => _lunyEngine.OnStartup(); // => OnStartup()
		private void FixedUpdate() => _lunyEngine?.OnFixedStep(Time.fixedDeltaTime); // => OnFixedStep()
		private void Update() => _lunyEngine?.OnUpdate(Time.deltaTime); // => OnUpdate()
		private void LateUpdate() => _lunyEngine?.OnLateUpdate(Time.deltaTime); // => OnLateUpdate()

		private void OnApplicationQuit() // => OnShutdown()
		{
			LunyLogger.LogInfo($"{nameof(OnApplicationQuit)} running...", this);
			Shutdown();
			LunyLogger.LogInfo($"{nameof(OnApplicationQuit)} complete.", this);
		}

		private void OnDestroy()
		{
			LunyLogger.LogInfo($"{nameof(OnDestroy)} running...", this);

			// we should not get destroyed with an existing instance (indicates manual removal)
			if (s_Instance != null)
			{
				Shutdown(); // force shutdown 
				LunyThrow.EngineAdapterPrematurelyRemovedException(nameof(LunyEngineUnityAdapter));
			}

			LunyLogger.LogInfo($"{nameof(OnDestroy)} complete.", this);
			CollectGarbage();
		}

		private void CollectGarbage() => GC.Collect(0, GCCollectionMode.Forced, true);

		~LunyEngineUnityAdapter() => LunyLogger.LogInfo($"[{nameof(LunyEngineUnityAdapter)}] finalized {GetHashCode()}");

		private void Shutdown()
		{
			if (s_Instance == null)
				return;

			try
			{
				LunyLogger.LogInfo("Shutdown...", this);
				_lunyEngine.OnShutdown();
			}
			catch (Exception ex)
			{
				LunyLogger.LogException(ex);
			}
			finally
			{
				_lunyEngine = null;
				s_Instance = null;

				LunyLogger.LogInfo("Shutdown complete.", this);
				LunyLogger.Logger = null;
			}
		}
	}
}
