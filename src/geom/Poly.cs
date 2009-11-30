using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;

using ce.math;

namespace ce.engine.geom
{
	public enum PlaneSide
	{
		Front,
		Back,
		On,
		Both
	}

	/// <summary>
	/// A clippable polygon
	/// </summary>
	public class Polygon : IIntersectable
	{
		public Vector3[] verts;
		Plane plane;
		bool validPlane;
		public const float Epsilon = 0.001f;

		public Plane Plane
		{
			get
			{
				if (!validPlane)
					CalcPlane();
				return plane;
			}
			set {
				validPlane = true;
				plane = value;
			}
		}

		protected Polygon()
		{}

		public Polygon(Vector3[] v, Plane plane)
		{
			verts = v;
			this.plane = plane;
			validPlane = true;
		}
		public Polygon(int n)
		{
			verts = new Vector3[n];
		}
		public Polygon(Vector3[] vrt)
		{
			verts = vrt;
		}
		public Vector3 this[int i]
		{
			get { return verts[i]; }
			set { verts[i] = value; validPlane = false;  }
		}
		public int Length { get { return verts.Length; } }
		public int NumVerts { get { return verts.Length; } }

		public void CalcPlane ()
		{
			plane = Plane.FromTriangle(verts[0], verts[1], verts[2]);
			validPlane = true;
		}

		public PlaneSide CheckAgainstPlane (Plane pln)
		{
			bool front = true;
			bool back = true;
			bool on = true;

			foreach (Vector3 v in verts) {
				float dis = pln.Distance(v);
				if(dis < -Epsilon) front = on = false;
				if(dis > Epsilon) back = on = false;
			}

			if(on) return PlaneSide.On;
			if(front) return PlaneSide.Front;
			if(back) return PlaneSide.Back;
			return PlaneSide.Both;
		}


		public Vector3[] ClipVerticesToPlane (Plane pl)
		{
			List<Vector3> dst = new List<Vector3>(verts.Length);

			int v1 = verts.Length - 1;
			for(int v2=0;v2<verts.Length;v1=v2++)
			{
				float d1 = pl.Distance(verts[v1]);
				float d2 = pl.Distance(verts[v2]);

				if(d1 >= 0 && d2 >= 0)  // Both on front, add end point(v2)
					dst.Add(verts[v2]);

				// Crossing plane
				if((d1 < 0 && d2 >= 0) || (d1 >= 0 && d2 < 0)) 
				{
					float part = d1 / (d1 - d2);
					dst.Add(verts[v1] + (verts[v2] - verts[v1]) * part);
					if(d1 < 0) 
						dst.Add(verts[v2]);
				} 
			}

			return dst.ToArray();
		}

		public void SplitVertices (Plane pl, out Vector3[] frontVrt, out Vector3[] backVrt)
		{
			List<Vector3> fv = new List<Vector3>();
			List<Vector3> bv = new List<Vector3>();

			frontVrt = backVrt = null;

			int v1 = verts.Length - 1;
			for(int v2=0;v2<verts.Length;v1=v2++)
			{
				float d1 = pl.Distance(verts[v1]);
				float d2 = pl.Distance(verts[v2]);

				if(d1 >= 0 && d2 >= 0)  // Both on front, add end point(v2)
					fv.Add(verts[v2]);

				else if(d1 < 0 && d2 < 0) // Both on back, add end point (v2)
					bv.Add(verts[v2]);

				else if((d1 < 0 && d2 >= 0) || (d1 >= 0 && d2 < 0)) 
				{
					float part = d1 / (d1 - d2);
					Vector3 v = verts[v1] + (verts[v2] - verts[v1]) * part;

					fv.Add(v);
					bv.Add(v);

					if (d1 < 0)
						fv.Add(verts[v2]);
					else
						bv.Add(verts[v2]);
				}
			}

			if (fv.Count > 0)
				frontVrt = fv.ToArray();
			if (bv.Count > 0)
				backVrt = bv.ToArray();
		}



		public bool PointInside(Vector3 hitPos)
		{
			int v1 = verts.Length - 1;
			Vector3 normal = Plane.Normal;
			for(int v2=0;v2<verts.Length;v2++) {
				Vector3 edge = verts[v2] - verts[v1];
				Vector3 edgeNormal = normal % edge;

				if ((edgeNormal.Dot(hitPos)) > (edgeNormal | verts[v1]))
					return false;
				
				v1 = v2;
			}
			return true;
		}

		public Vector3[] FlipVertices()
		{
			Vector3[] v = new Vector3[verts.Length];

			for(int x=0;x<verts.Length;x++) {
				v[x] = verts[verts.Length - x - 1];
			}
			return v;
		}

		public void Flip ()
		{
			verts = FlipVertices();
			validPlane = false;
		}

		/// <summary>
		/// Clip a polygon against a plane. The front side of the polygon is returned
		/// </summary>
		public Polygon Clip(Plane pl)
		{
			Vector3[] rv = ClipVerticesToPlane(pl);
			if (rv.Length > 0)
				return new Polygon(rv, Plane);
			return null;
		}
		
		public void Transform(Matrix4x4 m)
		{
			for (int i = 0; i < verts.Length; i++)
				verts[i] = m.Transform(verts[i]);

			validPlane = false;
		}

		#region IPrimitive Members

		public Vector3 CalcMidPos()
		{
			Vector3 p = new Vector3();
			for (int i = 0; i < verts.Length; i++)
				p += verts[i];
			return p / (float)verts.Length;
		}

		public bool InFrustum(Frustum frustum)
		{
			return frustum.ClipPolygon(this) == null;
		}

		public PrimitiveType PrimitiveType
		{
			get { return PrimitiveType.Surface; }
		}

		public float GetSelectFactor(Vector3 rayStart, Vector3 rayDir, Vector3 pt)
		{
			return 0.0f; // mostly used for edges/vertices that aren't really clickable because they have no surface
		}
		public object SelectableObject
		{
			get { return this; }
		}

		#endregion

		#region IIntersectable Members

		public void RayIntersect(Ray ray, ICollector<Intersection> results)
		{
			Vector3 pos;

			if (ray.IntersectPoly(this, out pos)) {
				results.Add(new Intersection() {
					item = this,
					normal = Plane.Normal,
					position = pos
				});
			}
		}

		#endregion
	}
}
