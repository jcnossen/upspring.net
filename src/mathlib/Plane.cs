using System;
using System.Collections.Generic;
using System.Text;

namespace ce.math
{
	[System.ComponentModel.TypeConverter(typeof(ce.util.StructTypeConverter<Plane>))]
	public struct Plane
	{
		public float a, b, c, d;

		public Plane(float x, float y, float z, float dist)
		{
			a = x; b = y; c = z; d = dist;
		}

		public Plane(Vector3 norm, float dist)
		{
			a = norm.x;
			b = norm.y;
			c = norm.z;
			d = dist;
		}

		public float[] ToArray()
		{
			return new float[] { a, b, c, d };
		}

		public float Distance(Vector3 pos)
		{
			return a * pos.x + b * pos.y + c * pos.z - d;
		}
		public double DistanceD(Vector3 pos)
		{
			return (double)a * (double)pos.x + (double)b * (double)pos.y + (double)c * (double)pos.z - (double)d;
		}
		public void CalcDist(Vector3 pos)
		{
			d = pos.x * a + pos.y * b + pos.z * c;
		}

		public Vector3 Normal
		{
			get 
			{ 
				return new Vector3(a, b, c);
			}
			set
			{
				a = value.x;
				b = value.y;
				c = value.z;
			}
		}
		public static Plane FromTriangle(Vector3 v1, Vector3 v2,Vector3 v3)
		{
			Vector3 r1 = v2 - v1;
			Vector3 r2 = v3 - v1;
			Vector3 norm = r2 % r1;
			norm.Normalize();
			return new Plane(norm, norm | v2);
		}
		public static Plane FromTriangle(Vector3[] pts)
		{
			return FromTriangle(pts[0], pts[1], pts[2]);
		}

		public Plane Invert()
		{
			return new Plane(-a, -b, -c, -d);
		}

		public bool RayPlaneIntersection(Vector3 start, Vector3 dir, out Vector3 pos, out float t)
		{
			double a = DistanceD(start);
			double b = DistanceD(start + dir);

			double diff = a-b;
			if (Math.Abs(diff) < 0.001) {
				pos = new Vector3();
				t = 0.0f;
				return false;
			}

			t = (float)(a / diff);
			pos = start + t * dir;
			return true;
		}
	}
}
