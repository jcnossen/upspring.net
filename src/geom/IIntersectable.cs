using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ce.math;

namespace ce.engine.geom
{
	public interface IIntersectable
	{
		void RayIntersect(Ray ray, ICollector<Intersection> intersections);
	}

	public interface ICollector<T>
	{
		void Add(T item);
		void AddRange(T[] item);
	}

	public class ListCollector<T> : List<T>, ICollector<T>
	{
		void ICollector<T>.Add(T item) { Add(item); }
		void ICollector<T>.AddRange(T[] items) { AddRange(items); }
	}

	public struct Intersection
	{
		public Vector3 position, normal;
		public object item;
	}

	public static class IntersectionHelper
	{
		public static Intersection GetClosest(this IEnumerable<Intersection> intersections, Vector3 pos)
		{
			Intersection best = new Intersection();
			float bestDistSq = 0.0f;
			bool first = true;

			foreach (Intersection s in intersections) {
				float distSq = (s.position - pos).SqLength;
				if (first || distSq < bestDistSq) {
					first = false;
					bestDistSq = distSq;
					best = s;
				}
			}
			return best;
		}
	}


	public enum PrimitiveType
	{
		Vertex,
		Edge,
		Surface,
		Object,
		MaxTypes
	}

	public interface IEditorPrimitive
	{
		Vector3 CalcMidPos();
		bool InFrustum(Frustum frustum);
		PrimitiveType PrimitiveType {get;}
		float GetSelectFactor(Vector3 rayStart, Vector3 rayDir, Vector3 pt);
		object SelectableObject { get; } // the object that gets selected when this primitve is clicked
		bool RayIntersect(Ray ray, Vector3 viewPos, float border, out Vector3 pos, out Vector3 normal);
	}

	public interface IGeomPrimitive : IEditorPrimitive
	{
		Vertex[] Vertices { get; }
	}

}
