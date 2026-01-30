using Luny.Engine.Bridge;
using System;

namespace Luny.Unity.Engine.Bridge
{
	public sealed class UnityPathConverter : ILunyPathConverter
	{
		private const String ResourcesPrefix = "Assets/Resources/";
		private const String LunyResourcesPrefix = "Assets/Resources/Luny/";
		private const String PersistentDataPrefix = "USER/"; // Mock for persistentDataPath

		public String ToLuny(String nativePath, LunyPathType type)
		{
			if (String.IsNullOrEmpty(nativePath))
				return nativePath;

			if (nativePath.StartsWith(LunyResourcesPrefix))
				return nativePath.Substring(LunyResourcesPrefix.Length);
			if (nativePath.StartsWith(ResourcesPrefix))
				return nativePath.Substring(ResourcesPrefix.Length);
			if (nativePath.StartsWith(PersistentDataPrefix))
				return nativePath.Substring(PersistentDataPrefix.Length);

			return nativePath;
		}

		public String ToNative(String agnosticPath, LunyPathType type)
		{
			if (String.IsNullOrEmpty(agnosticPath))
				return agnosticPath;

			return type switch
			{
				// Assets/Resources/Luny/ is the preferred location for Luny assets in Unity
				LunyPathType.Save => PersistentDataPrefix + agnosticPath, _ => "Luny/" + agnosticPath
			};
		}
	}
}
