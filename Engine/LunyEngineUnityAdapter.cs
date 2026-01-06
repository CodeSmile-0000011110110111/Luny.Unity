using Luny.Engine.Bridge;
using Luny.Engine.Services;
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
	internal sealed partial class LunyEngineUnityAdapter : MonoBehaviour, ILunyEngineNativeAdapter
	{
		// intentionally remains private - user code must use LunyEngine.Instance!
		private static ILunyEngineNativeAdapter s_Instance;

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
			LunyTraceLogger.LogInfoInitializing(typeof(LunyEngineUnityAdapter));

			var go = new GameObject(nameof(LunyEngineUnityAdapter));
			DontDestroyOnLoad(go);

			// Note: Awake and OnEnable run within AddComponent, within them s_Instance is and remains null!
			var unityAdapter = go.AddComponent<LunyEngineUnityAdapter>();
			s_Instance = ILunyEngineNativeAdapter.ValidateAdapterSingletonInstance(s_Instance, unityAdapter);

			LunyTraceLogger.LogInfoInitializationComplete(typeof(LunyEngineUnityAdapter));
		}

		// Note: s_Instance is and remains null during Awake - this is intentional!
		private void Awake() => _lunyEngine = LunyEngine.CreateInstance(this);

		private void Start()
		{
			ILunyEngineNativeAdapter.ThrowIfAdapterNull(s_Instance);
			ILunyEngineNativeAdapter.ThrowIfLunyEngineNull(_lunyEngine);

			_lunyEngine.OnEngineStartup();
			// => OnStartup()
		}

		private void FixedUpdate() => _lunyEngine?.OnEngineFixedStep(Time.fixedDeltaTime); // => OnFixedStep()
		private void Update() => _lunyEngine?.OnEngineUpdate(Time.deltaTime); // => OnUpdate()
		private void LateUpdate() => _lunyEngine?.OnEngineLateUpdate(Time.deltaTime); // => OnLateUpdate()

		private void OnApplicationQuit() // => OnShutdown()
		{
			_applicationIsQuitting = true;
			Shutdown();
		}

		private void OnDestroy()
		{
			LunyTraceLogger.LogInfoDestroying(this);

			// we should not get destroyed with an existing instance (indicates manual removal)
			if (!_applicationIsQuitting)
			{
				ILunyEngineNativeAdapter.ThrowIfPrematurelyRemoved(s_Instance, _lunyEngine);
				Shutdown();
			}

			LunyTraceLogger.LogInfoDestroyed(this);
			ILunyEngineNativeAdapter.EndLogging();
		}

		~LunyEngineUnityAdapter() => LunyTraceLogger.LogInfoFinalized(this);

		private void Shutdown()
		{
			if (s_Instance == null)
				return;

			try
			{
				ILunyEngineNativeAdapter.Shutdown(s_Instance, _lunyEngine);
			}
			catch (Exception ex)
			{
				LunyLogger.LogException(ex);
			}
			finally
			{
				ILunyEngineNativeAdapter.ShutdownComplete(s_Instance);

				_lunyEngine = null;
				s_Instance = null;
			}
		}
	}
}
