using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ce.math;
using Tao.OpenGl;

namespace ce.engine.geom
{
	/// <summary>
	/// The frustum contains planes generated from the camera properties, 
	/// and can be used to test whether bounding volumes or points are visible
	/// </summary>
	public class Frustum
	{
		public enum VisType { Inside, Outside, Partial };

		public Plane[] planes;
		public Vector3 origin, front, right, up;
		public Vector3[] pos;

		public void InversePlanes ()
		{
			foreach(Plane pl in planes)
				pl.Invert();
		}

		// find the box vertices to compare against the plane
		static void GetBoxPlaneCompareVerts (Vector3 min, Vector3 max, Vector3 plane, 
									 out Vector3 close, out Vector3 far)
		{
			if(plane.x > 0) { close.x = min.x; far.x = max.x; }
			else { close.x = max.x; far.x = min.x; }
			if(plane.y > 0) { close.y = min.y; far.y = max.y; }
			else { close.y = max.y; far.y = min.y; }
			if(plane.z > 0) { close.z = min.z; far.z = max.z; }
			else { close.z = max.z; far.z = min.z; }
		}

		public void CalcCameraPlanes (Vector3 cbase, Vector3 cright, Vector3 cup, Vector3 cfront, float fov, float aspect, float near, float far)
		{
			float tanHalfFov = (float)Math.Tan(fov / 2.0f);
			List<Plane> planes = new List<Plane>();

			// Near plane
			planes.Add(new Plane(cfront, cfront | (cbase + near * cfront)));
			if (!float.IsInfinity(far))
				planes.Add(new Plane(-cfront, (-cfront) | (cbase + far * cfront)));

			float m = 200.0f;
			Vector3 mid = cbase + cfront * m;
			up = cup;
			right = cright;
			up *= tanHalfFov * m;
			right *= tanHalfFov * m * aspect;
			front = cfront;

			pos = new Vector3[4];
			pos [0] = mid + right + up; // rightup
			pos [1] = mid + right - up; // rightdown
			pos [2] = mid - right - up; // leftdown
			pos [3] = mid - right + up; // leftup

			origin = cbase;

			planes.Add(Plane.FromTriangle(origin, pos[2], pos[3])); // left
			planes.Add(Plane.FromTriangle(origin, pos[3], pos[0])); // up
			planes.Add(Plane.FromTriangle(origin, pos[0], pos[1])); // right
			planes.Add(Plane.FromTriangle(origin, pos[1], pos[2]));// down

			this.planes = planes.ToArray();

			right.Normalize();
			up.Normalize();
			front.Normalize();
		}

		public void Draw ()
		{
			Gl.glDisable(Gl.GL_CULL_FACE);

			Gl.glColor3f(1,1,1);
			Gl.glBegin (Gl.GL_LINES);
			for (int a=0;a<4;a++) {
				glVertex(origin);
				glVertex(pos[a]);
			}
			Gl.glEnd();
			Gl.glBegin (Gl.GL_LINE_LOOP);
			for (int a = 0; a < 4; a++)
				glVertex(pos[a]);
			Gl.glEnd();
		}

		void glVertex(Vector3 v)
		{
			Gl.glVertex3f(v.x, v.y, v.z);
		}

		public void DrawPlane(Plane pl)
		{
			Vector3 b = pl.Normal * pl.d;
			Vector3 o1, o2;
			pl.Normal.OrthoSpace(out o1, out o2);
			o1 *= 20;
			o2 *= 20;
			Gl.glBegin(Gl.GL_QUADS);
			glVertex(b - o1 + o2);
			glVertex(b + o1 + o2);
			glVertex(b + o1 - o2);
			glVertex(b - o1 - o2);
			Gl.glEnd();
		}

		public VisType IsBoxVisible (Vector3 min, Vector3 max)
		{
			bool full = true;
			Vector3 c,f;
			float dc, df;

			foreach (Plane pl in planes)
			{
				GetBoxPlaneCompareVerts (min, max, pl.Normal, out c, out f);
				
				dc = pl.Distance(c);
				df = pl.Distance(f);
				if(dc < 0.0f || df < 0.0f) full=false;
				if(dc < 0.0f && df < 0.0f) return VisType.Outside;
			}

			return full ? VisType.Inside : VisType.Partial;
		}

		public bool IsPointVisible (Vector3 pt)
		{
			foreach (Plane pl in planes) {
				float d = pl.Distance(pt);

				if (d < -Polygon.Epsilon) return false;
			}
			return true;
		}

		public VisType IsSphereVisible (Vector3 center, float radius)
		{
			bool partial = false;
			foreach (Plane pl in planes) {
				float d = pl.Distance(center);
				if (d < radius) {
					if (d <= -radius)
						return VisType.Outside;
					partial = true;
				}
			}
			return partial ? VisType.Partial : VisType.Inside;
		}

		public Polygon ClipPolygon (Polygon poly)
		{
			foreach (Plane plane in planes) {
				poly = poly.Clip(plane);
				if (poly == null)
					break;
			}
			return poly;
		}

		public Vector3[] RayIntersect(Ray ray)
		{
			List<Vector3> hits = new List<Vector3>();
			foreach (Plane pl in planes) {
				Vector3 hp;
				if (ray.IntersectPlane(pl, out hp)) {
					// See if the hitpoint is actually within the frustum
					if (IsPointVisible(hp))
						hits.Add(hp);
				}
			}
			return hits.ToArray();
		}
	}
}
