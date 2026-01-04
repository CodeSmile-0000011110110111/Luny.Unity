using Luny.Engine;
using Luny.Unity.Engine.Services;
using System;
using UnityEngine;

namespace Luny.Unity.Engine
{
	/// <summary>
	/// Engine hook. Provides no public or internal API.
	/// </summary>
	[DefaultExecutionOrder(Int32.MinValue)] // Run before all other scripts
	[AddComponentMenu("GameObject/")] // Do not list in "Add Component" menu
	[DisallowMultipleComponent]
	internal sealed partial class LunyEngineUnityAdapter : MonoBehaviour, ILunyEngineAdapter
	{
		// intentionally remains private - user code must use LunyEngine.Instance!
		private static ILunyEngineAdapter s_Instance;

		// hold on to LunyEngine reference (not a MonoBehaviour type)
		private ILunyEngine _lunyEngine;

		private Boolean _applicationIsQuitting;

		// Note: in builds the SceneManager's root objects list is empty in 'BeforeSceneLoad' (unlike in editor)
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void OnBeforeSceneLoad() => Initialize();

		private static void Initialize()
		{
			// Logging comes first, we don't want to miss anything
			LunyLogger.Logger = new UnityLogger();
			LunyLogger.LogInfo("Initializing...", typeof(LunyEngineUnityAdapter));

			var go = new GameObject(nameof(LunyEngineUnityAdapter));
			DontDestroyOnLoad(go);

			// Note: Awake and OnEnable run within AddComponent, within them s_Instance is and remains null!
			var unityAdapter = go.AddComponent<LunyEngineUnityAdapter>();
			s_Instance = ILunyEngineAdapter.ValidateAdapterSingletonInstance(s_Instance, unityAdapter);

			LunyLogger.LogInfo("Initialization complete.", typeof(LunyEngineUnityAdapter));
		}

		private void Awake() =>
			// Note: s_Instance is and remains null during Awake - this is intentional!
			_lunyEngine = LunyEngine.CreateInstance(this);

		private void Start()
		{
			ILunyEngineAdapter.AssertNotNull(s_Instance);
			ILunyEngineAdapter.AssertLunyEngineNotNull(_lunyEngine);

			_lunyEngine.OnStartup();
			// => OnStartup()
		}

		private void FixedUpdate() => _lunyEngine?.OnFixedStep(Time.fixedDeltaTime); // => OnFixedStep()
		private void Update() => _lunyEngine?.OnUpdate(Time.deltaTime); // => OnUpdate()
		private void LateUpdate() => _lunyEngine?.OnLateUpdate(Time.deltaTime); // => OnLateUpdate()

		private void OnApplicationQuit() // => OnShutdown()
		{
			LunyLogger.LogInfo($"{nameof(OnApplicationQuit)} running...", this);
			_applicationIsQuitting = true;
			Shutdown();
			LunyLogger.LogInfo($"{nameof(OnApplicationQuit)} complete.", this);
		}

		private void OnDestroy()
		{
			LunyLogger.LogInfo($"{nameof(OnDestroy)} running...", this);

			// we should not get destroyed with an existing instance (indicates manual removal)
			if (!_applicationIsQuitting)
			{
				ILunyEngineAdapter.AssertNotPrematurelyRemoved(s_Instance, _lunyEngine);
				Shutdown();
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
				ILunyEngineAdapter.ShutdownLunyEngine(s_Instance, _lunyEngine);
			}
			catch (Exception ex)
			{
				LunyLogger.LogException(ex);
			}
			finally
			{
				ILunyEngineAdapter.ShutdownComplete(s_Instance);

				_lunyEngine = null;
				s_Instance = null;
			}
		}
	}
}
