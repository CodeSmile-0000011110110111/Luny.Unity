using Luny.Engine;
using Luny.Unity.Engine.Bridge;
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
		private ILunyEngineLifecycle _lunyEngine;

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

			// Note: Awake and OnEnable run within AddComponent
			go.AddComponent<LunyEngineUnityAdapter>();
			LunyTraceLogger.LogInfoInitialized(typeof(LunyEngineUnityAdapter));
		}

		private void Awake() => _lunyEngine = ILunyEngineNativeAdapter.CreateEngine(ref s_Instance, this);

		private void Start()
		{
			ILunyEngineNativeAdapter.ThrowIfAdapterNull(s_Instance);
			ILunyEngineNativeAdapter.ThrowIfLunyEngineNull(_lunyEngine);
			ILunyEngineNativeAdapter.Startup(s_Instance, _lunyEngine); // => OnStartup()
		}

		private void FixedUpdate() => ILunyEngineNativeAdapter.FixedStep(Time.fixedDeltaTime, s_Instance, _lunyEngine); // => OnFixedStep()
		private void Update() => ILunyEngineNativeAdapter.Update(Time.deltaTime, s_Instance, _lunyEngine); // => OnUpdate()
		private void LateUpdate() => ILunyEngineNativeAdapter.LateUpdate(Time.deltaTime, s_Instance, _lunyEngine); // => OnLateUpdate()

		private void OnApplicationQuit()
		{
			ILunyEngineNativeAdapter.IsApplicationQuitting = true;
			Shutdown();
		}

		private void OnDestroy()
		{
			LunyTraceLogger.LogInfoDestroying(this);

			// we should not get destroyed with an existing instance (indicates manual removal)
			ILunyEngineNativeAdapter.ThrowIfPrematurelyRemoved(s_Instance, _lunyEngine);

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
				ILunyEngineNativeAdapter.Shutdown(s_Instance, _lunyEngine); // => OnShutdown()
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
