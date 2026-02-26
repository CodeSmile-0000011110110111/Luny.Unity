using Luny.Engine.Bridge;
using UnityEngine;

namespace Luny.Unity.Engine.Bridge
{
	public static class LunyVectorExt
	{
		public static Vector3 ToUnity(this LunyVector3 v) => new(v.X, v.Y, v.Z);
		public static LunyVector3 ToLuny(this Vector3 v) => new(v.x, v.y, v.z);

		public static Vector2 ToUnity(this LunyVector2 v) => new(v.X, v.Y);
		public static LunyVector2 ToLuny(this Vector2 v) => new(v.x, v.y);

		public static Quaternion ToUnity(this LunyQuaternion q) => new(q.X, q.Y, q.Z, q.W);
		public static LunyQuaternion ToLuny(this Quaternion q) => new(q.x, q.y, q.z, q.w);
	}
}
