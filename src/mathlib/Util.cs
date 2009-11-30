/*
 * Date: 3/23/2008
 * Time: 2:12 PM
 * By Jelmer Cnossen
 */

using System;

namespace ce.math
{
	/// <summary>
	/// Description of Util.
	/// </summary>
	public static class Util
	{
		public const float Epsilon=0.001f;

		public static Vector3[] ComputeOrientation (float yaw, float pitch, float roll)
		{
			// get x rotation matrix:
			Matrix4x4 result = Matrix4x4.XRotate(pitch);
		
			// get y rotation matrix:
			Matrix4x4 cur = Matrix4x4.YRotate(yaw);
			result = cur.Multiply (result);
		
			// get z rotation matrix
			result = Matrix4x4.ZRotate(roll).Multiply(result);
		 
			return new Vector3[] {
				result.Row(0), result.Row(1), result.Row(2)
			};
		}
	}
}
