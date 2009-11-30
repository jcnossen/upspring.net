using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ce.math;

namespace ce.engine.geom
{
	public struct Ray
	{
		public Vector3 start, dir;

		public static Ray XAxis = new Ray(new Vector3(), new Vector3(1, 0, 0));
		public static Ray YAxis = new Ray(new Vector3(), new Vector3(0, 1, 0));
		public static Ray ZAxis = new Ray(new Vector3(), new Vector3(0, 0, 1));

		public Ray (Vector3 rayStart, Vector3 rayDirection) {
			start = rayStart;
			dir = rayDirection;
		}

		public float DistanceToPoint (Vector3 pos)
		{
			return pos.DistanceToRay(start, dir);
		}


		public bool IntersectPoly(Polygon pl, out Vector3 hitPos)
		{
			if (IntersectPlane(pl.Plane, out hitPos)) {
				return pl.PointInside(hitPos);
			}
			hitPos = new Vector3();

			return false;
		}

		public bool IntersectPlane(Plane pl, out Vector3 hitPos)
		{
			float t;
			return pl.RayPlaneIntersection(start, dir, out hitPos, out t);
		}

		public Ray Transform(Matrix4x4 transform)
		{
			Vector3 end = transform.Transform(start + dir);
			Vector3 st = transform.Transform(start);
			return new Ray(st, st - end); 
		}

		public bool IntersectSphere(Vector3 mid, float radius, out Vector3 hitPos)
		{
			Vector3 normDir = dir;
			normDir.Normalize();

			// Assumes ray.d is normalized
			// Uses quadratic formula, A = 1 because ray.d is normalized.
			// For derivation, see:
			// http://www.siggraph.org/education/materials/HyperGraph/raytrace/rtinter1.htm
			Vector3 dst = start - mid;
			float B = dst | normDir; // B really equals 2.0f this value
			float C = (dst|dst)-radius*radius;
			float D = B*B - C;  // Discriminant.  D really equals 4.0f times this value
			if (D < 0.0f) {
				hitPos=new Vector3();
				return false;
			}
			float sqrtD = (float)Math.Sqrt(D); // sqrtD really equals 2.0f times this value
		//	float t0 = (-2.0f * B - 2.0f * sqrtD) * 0.5f;  // The factors cancel out
			float t0 = (-B - sqrtD); // Will always be the smaller, but may be negative
		//	float t1 = (-B + sqrtD); // The second intersection, if you care
			hitPos = start + normDir * t0;
			return true;
		}
	}
}
