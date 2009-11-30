using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ce.math
{
	[System.ComponentModel.TypeConverter(typeof(ce.util.StructTypeConverter<Vector4>))]
	public struct Vector4
	{
		public float x,y,z,w;
		
		public Vector4(float X,float Y,float Z,float W)
		{
			x = X; y = Y; z = Z; w = W;
		}

		public Vector4(Vector3 v, float w)
		{
			x = v.x; y = v.y; z = v.z; this.w = w;
		}

		public static Vector4 operator*(Vector4 v, float f)
		{
			Vector4 r;
			r.x = v.x * f;
			r.y = v.y * f;
			r.z = v.z * f;
			r.w = v.w * f;
			return r;
		}

		public static Vector4 operator *(float f, Vector4 v)
		{
			Vector4 r;
			r.x = v.x * f;
			r.y = v.y * f;
			r.z = v.z * f;
			r.w = v.w * f;
			return r;
		}

		public static Vector4 operator /(Vector4 v, float f)
		{
			Vector4 r;
			r.x = v.x / f;
			r.y = v.y / f;
			r.z = v.z / f;
			r.w = v.w / f;
			return r;
		}

		public static Vector4 operator +(Vector4 a, Vector4 b)
		{
			Vector4 r;
			r.x = a.x + b.x;
			r.y = a.y + b.y;
			r.z = a.z + b.z;
			r.w = a.w + b.w;
			return r;
		}

		public static Vector4 operator -(Vector4 a, Vector4 b)
		{
			Vector4 r;
			r.x = a.x - b.x;
			r.y = a.y - b.y;
			r.z = a.z - b.z;
			r.w = a.w - b.w;
			return r;
		}

		public static Vector4 operator -(Vector4 a)
		{
			return new Vector4(-a.x, -a.y, -a.z, -a.w);
		}

		// Dot-product
		public static float operator*(Vector4 a,Vector4 b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}

		// Component-wise multiplication
		public void Multiply(Vector4 v)
		{
			x *= v.x;
			y *= v.y;
			z *= v.z;
			w *= v.w;
		}

		public float Length
		{
			get { return (float)System.Math.Sqrt((double)(x * x + y * y + z * z + w * w)); }
		}

		public float SqLength
		{
			get { return x * x + y * y + z * z + w * w; }
		}

		/// <summary>
		/// Normalizes the vector
		/// </summary>
		/// <returns>Original length</returns>
		public float Normalize()
		{
			float len = Length;
			if (len < 0.0001f)
				return 0.0f;

			float invLength = 1.0f / len;
			x *= invLength;
			y *= invLength;
			z *= invLength;
			z *= invLength;
			return len;
		}
	}
}
