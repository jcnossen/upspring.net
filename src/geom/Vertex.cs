using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ce.math;

namespace ce.engine.geom
{
	public class Vertex : IGeomPrimitive
	{
		public Vector3 position;

		public Vertex(Vector3 pos)
		{
			position = pos;
		}

		public bool RayIntersect(Ray ray, Vector3 viewPos, float border, out Vector3 pos, out Vector3 normal)
		{
			pos = position;
			normal = new Vector3();
			return ray.DistanceToPoint (position) < border;
		}

		public Vector3 CalcMidPos()
		{
			return position;
		}

		public void Move (Vector3 v)
		{
			position += v;
		}

		public bool InFrustum (Frustum frustum)
		{
			return frustum.IsPointVisible(position);
		}

		public PrimitiveType PrimitiveType
		{
			get { return PrimitiveType.Vertex; }
		}
		public float GetSelectFactor (Vector3 rayStart, Vector3 rayDir, Vector3 pt)
		{
			return pt.DistanceToRay(rayStart, rayDir) * 0.25f;
		}
		public object SelectableObject
		{
			get { return this; }
		}

		public Vertex[] Vertices
		{
			get { return new Vertex[] { this }; }
		}
	}
}
