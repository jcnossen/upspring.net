using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ce.engine.geom;

using ce.math;
using ce.engine.gl;

namespace UpspringSharp.geom
{
	public class Poly
	{
		public int[] verts;
		public string texname;
		public Vector3 color = new Vector3(1, 1, 1);
		public int taColor = -1; // TA indexed color
		public Texture texture;

		public int Length
		{
			get { return verts.Length; }
		}

		public int this[int index]
		{
			get { return verts[index]; }
			set { verts[index] = value; }
		}

		public Poly Clone()
		{
			Poly pl = new Poly();
			pl.verts = (int[])verts.Clone();
			pl.texname = texname;
			pl.color = color;
			pl.taColor = taColor;
			pl.texture = texture;
			return pl;
		}

		public void Flip()
		{
			var nv = new int[verts.Length];
			for (int a = 0; a < verts.Length; a++)
				nv[verts.Length - a - 1] = verts[(a + 2) % verts.Length];
			verts = nv;
		}

		public Plane CalcPlane(Vertex[] vrt)
		{
			return Plane.FromTriangle(vrt[verts[0]].pos, vrt[verts[1]].pos, vrt[verts[2]].pos);
		}

		public void RotateVerts()
		{
			var n = new int[verts.Length];
			for (int a = 0; a < verts.Length; a++)
				n[a] = verts[(a + 1) % n.Length];
			verts = n;
		}

	}

	public class PolyMesh
	{
		public Vertex[] verts = new Vertex[0];
		public Poly[] poly = new Poly[0];

		PolyMesh Clone()
		{
			PolyMesh cp = new PolyMesh();

			cp.verts = (Vertex[])verts.Clone();

			cp.poly = new Poly[poly.Length];
			for (int i = 0; i < poly.Length; i++) {
				cp.poly[i] = poly[i].Clone();
			}

			return cp;
		}

		void Transform(Matrix4x4 transform)
		{
			Matrix4x4 normalTransform = transform.Inverse().Transpose();

			// transform and add the child vertices to the parent vertices list
			for (int a = 0; a < verts.Length; a++) {
				verts[a].pos = transform.Transform(verts[a].pos);
				verts[a].normal = transform.Transform(verts[a].normal);
			}
		}


		bool IsEqualVertexTC(Vertex a, Vertex b)
		{
			return (a.pos - b.pos).SqLength < 0.001f &&
				a.tc.x == b.tc.x && a.tc.y == b.tc.y;
		}

		bool IsEqualVertexTCNormal(Vertex a, Vertex b)
		{
			return (a.pos - b.pos).SqLength < 0.001f &&
				a.tc.x == b.tc.x && a.tc.y == b.tc.y &&
				(a.normal - b.normal).SqLength < 0.001f;
		}

		public delegate bool IsEqualVertexDelegate(Vertex a, Vertex b);

		public void OptimizeVertices(IsEqualVertexDelegate cb)
		{
			int[] old2new = new int[verts.Length];
			int[] usage = new int[verts.Length];
			List<Vertex> nv = new List<Vertex>();

			foreach (Poly pl in poly) {
				for (int b = 0; b < pl.Length; b++)
					usage[pl.verts[b]]++;
			}

			for (int a = 0; a < verts.Length; a++) {
				bool matched = false;

				if (usage[a] == 0)
					continue;

				for (int b = 0; b < nv.Count; b++) {
					if (cb(verts[a], nv[b])) {
						matched = true;
						old2new[a] = b;
						break;
					}
				}

				if (!matched) {
					old2new[a] = nv.Count;
					nv.Add(verts[a]);
				}
			}

			verts = nv.ToArray();

			// map the poly vertex-indices to the new set of vertices
			foreach (Poly pl in poly) {
				for (int b = 0; b < pl.Length; b++)
					pl.verts[b] = old2new[pl.verts[b]];
			}
		}

		void Optimize(IsEqualVertexDelegate cb)
		{
			OptimizeVertices(cb);

			// remove double linked vertices
			List<Poly> npl = new List<Poly>();
			foreach (Poly pl in poly) {
				bool finished;
				do {
					finished = true;
					for (int i = 0, j = (int)pl.Length - 1; i < pl.Length; j = i++)
						if (pl[i] == pl[j]) {
							var l = new List<int>(pl.verts);
							l.RemoveAt(i);
							pl.verts = l.ToArray();
							finished = false;
							break;
						}
				} while (!finished);

				if (pl.Length >= 3)
					npl.Add(pl);
			}
			poly = npl.ToArray();
		}


		void CalculateRadius(Matrix4x4 tr, Vector3 mid, ref float radius)
		{
			for (int v = 0; v < verts.Length; v++) {
				float r = (tr.Transform(verts[v].pos) - mid).Length;
				if (radius < r) radius = r;
			}
		}

		int[] MakeTris()
		{
			List<int> tris = new List<int>();

			foreach (Poly pl in poly) {
				for (int b = 2; b < pl.Length; b++) {
					tris.Add(pl.verts[0]);
					tris.Add(pl.verts[b - 1]);
					tris.Add(pl.verts[b]);
				}
			}
			return tris.ToArray();
		}


		void GenerateUniqueVectors(Vertex[] verts, out Vector3[] vertPos, out int[] old2new)
		{
			List<Vector3> vp = new List<Vector3>();
			old2new = new int[verts.Length];

			for (int a = 0; a < verts.Length; a++) {
				bool matched = false;

				for (int b = 0; b < vp.Count; b++)
					if ((vp[b] - verts[a].pos).SqLength < 0.001f) {
						old2new[a] = b;
						matched = true;
						break;
					}
				if (!matched) {
					old2new[a] = vp.Count;
					vp.Add(verts[a].pos);
				}
			}
			vertPos = vp.ToArray();
		}


		struct FaceVert
		{
			public List<int> adjacentFaces = new List<int>();
		};

		void CalculateNormals2(float maxSmoothAngle)
		{
			float ang_c = (float)Math.Cos(Math.PI * maxSmoothAngle / 180.0f);
			Vector3[] vertPos;
			int[] old2new;
			GenerateUniqueVectors(verts, out vertPos, out old2new);

			List<List<int>> new2old = new List<List<int>>(vertPos.Length);
			for (int a = 0; a < new2old.Count; a++)
				new2old.Add(new List<int>());
			for (int a = 0; a < old2new.Length; a++)
				new2old[old2new[a]].Add(a);

			// Calculate planes
			List<Plane> polyPlanes = new List<Plane>(poly.Length);
			for (int a = 0; a < poly.Length; a++)
				polyPlanes.Add(poly[a].CalcPlane(verts));

			// Determine which faces are using which unique vertex
			FaceVert[] faceVerts = new FaceVert[old2new.Length]; // one per unique vertex

			for (int a = 0; a < poly.Length; a++) {
				Poly pl = poly[a];
				for (int v = 0; v < pl.Length; v++)
					faceVerts[old2new[pl[v]]].adjacentFaces.Add(a);
			}

			// Calculate normals
			int cnorm = 0;
			for (int a = 0; a < poly.Length; a++)
				cnorm += poly[a].Length;
			Vector3[] normals = new Vector3[cnorm];

			cnorm = 0;
			for (int a = 0; a < poly.Length; a++) {
				Poly pl = poly[a];
				for (int v = 0; v < pl.Length; v++) {
					FaceVert fv = faceVerts[old2new[pl[v]]];
					List<Vector3> vnormals = new List<Vector3>();
					vnormals.Add(polyPlanes[a].Normal);
					for (int adj = 0; adj < fv.adjacentFaces.Count; adj++) {
						// Same poly?
						if (fv.adjacentFaces[adj] == a)
							continue;

						Plane adjPlane = polyPlanes[fv.adjacentFaces[adj]];

						// Spring 3DO style smoothing
						float dot = adjPlane.Normal.Dot(polyPlanes[a].Normal);
						//	logger.Print("Dot: %f\n",dot);
						if (dot < ang_c)
							continue;

						// see if the normal is unique for this vertex
						bool isUnique = true;
						for (int norm = 0; norm < vnormals.Count; norm++)
							if ((vnormals[norm] - adjPlane.Normal).SqLength < 0.001f) {
								isUnique = false;
								break;
							}

						if (isUnique)
							vnormals.Add(adjPlane.Normal);
					}
					Vector3 normal = new Vector3();
					for (int n = 0; n < vnormals.Count; n++)
						normal += vnormals[n];

					normal.Normalize();
					normals[cnorm++] = normal;
				}
			}

			// Create a new set of vertices with the calculated normals
			List<Vertex> newVertices = new List<Vertex>(poly.Length * 4);  // approximate
			cnorm = 0;
			for (int a = 0; a < poly.Length; a++) {
				Poly pl = poly[a];
				for (int v = 0; v < pl.Length; v++) {
					FaceVert fv = faceVerts[old2new[pl[v]]];
					Vertex nv = verts[pl[v]];
					nv.normal = normals[cnorm++];
					newVertices.Add(nv);
					pl[v] = newVertices.Count - 1;
				}
			}

			// Optimize
			verts = newVertices.ToArray();
			Optimize(IsEqualVertexTCNormal);
		}

		// In short, the reason for the complexity of this function is:
		//  - creates a list of vertices where every vertex has a unique position (UV ignored)
		//  - doesn't allow the same poly normal to be added to the same vertex twice
		void CalculateNormals()
		{
			Vector3[] vertPos;
			int[] old2new;
			GenerateUniqueVectors(verts, out vertPos, out old2new);

			List<List<int>> new2old = new List<List<int>>(vertPos.Length);
			for (int a = 0; a < new2old.Count; a++)
				new2old.Add(new List<int>());
			for (int a = 0; a < old2new.Length; a++)
				new2old[old2new[a]].Add(a);

			List<Vector3>[] normals = new List<Vector3>[vertPos.Length];

			for (int a = 0; a < poly.Length; a++) {
				Poly pl = poly[a];
				Plane plane = Plane.FromTriangle(
					vertPos[old2new[pl[0]]],
					vertPos[old2new[pl[1]]],
					vertPos[old2new[pl[2]]]);

				Vector3 plnorm = plane.Normal;
				for (int b = 0; b < pl.Length; b++) {
					List<Vector3> norms = normals[old2new[pl[b]]];
					int c;
					for (c = 0; c < norms.Count; c++)
						if ((norms[c] - plnorm).SqLength < 0.001f) break;

					if (c == norms.Count)
						norms.Add(plnorm);
				}
			}

			for (int a = 0; a < normals.Length; a++) {
				Vector3 sum = new Vector3();
				List<Vector3> vn = normals[a];
				for (int b = 0; b < vn.Count; b++)
					sum += vn[b];

				sum.Normalize();

				List<int> vlist = new2old[a];
				for (int b = 0; b < vlist.Count; b++)
					verts[vlist[b]].normal = sum;
			}
		}

		void FlipPolygons()
		{
			foreach (Poly pl in poly)
				pl.Flip();
		}


		void MoveGeometry(PolyMesh dst)
		{
			// offset the vertex indices and move polygons
			for (int a = 0; a < poly.Length; a++) {
				Poly pl = poly[a];

				for (int b = 0; b < pl.Length; b++)
					pl[b] += dst.verts.Length;
			}
			List<Poly> dstPoly = new List<Poly>(dst.poly);
			dstPoly.AddRange(poly);
			dst.poly = dstPoly.ToArray();
			poly = null;

			// insert the child vertices
			List<Vertex> dstVerts = new List<Vertex>(dst.verts);
			dstVerts.AddRange(verts);
			dst.verts = dstVerts.ToArray();
			verts = null;

			//			InvalidateRenderData();
		}
	}
}

