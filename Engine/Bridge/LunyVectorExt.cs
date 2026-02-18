namespace Luny.Unity
{
	public static class LunyVectorExt
	{
		public static UnityEngine.Vector3 ToUnity(this LunyVector3 v) => new(v.X, v.Y, v.Z);
		public static LunyVector3 ToLuny(this UnityEngine.Vector3 v) => new(v.x, v.y, v.z);

		public static UnityEngine.Vector2 ToUnity(this LunyVector2 v) => new(v.X, v.Y);
		public static LunyVector2 ToLuny(this UnityEngine.Vector2 v) => new(v.x, v.y);

		public static UnityEngine.Quaternion ToUnity(this LunyQuaternion q) => new(q.X, q.Y, q.Z, q.W);
		public static LunyQuaternion ToLuny(this UnityEngine.Quaternion q) => new(q.x, q.y, q.z, q.w);
	}
}
