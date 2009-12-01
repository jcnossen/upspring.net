using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using ce.math;

namespace UpspringSharp.geom
{
	/// <summary>
	/// Abstraction of storing rotation, 
	/// currently uses Euler angles, but former implementation have used quaternions
	/// </summary>
	public struct Rotator
	{
		public Vector3 euler;

		public Vector3 Euler {
			get { return euler; }
			set { euler = value; }
		}

		public Quaternion Quaternion
		{
			get { return Quaternion.FromMatrix( Matrix4x4.EulerYXZ(euler)); }
			set { euler = value.ToMatrix().CalcEulerYXZ(); }
		}

		public void AddEulerAbsolute(Vector3 rot)
		{
			euler += rot;
		}

		public void AddEulerRelative(Vector3 rot)
		{
			euler += rot;
		}

		public Matrix4x4 ToMatrix() 
		{
			return Matrix4x4.EulerYXZ(euler);
		}

		public void FromMatrix(Matrix4x4 r )
		{
			euler = r.CalcEulerYXZ();
		}	
	}

	public class MdlObject 
	{
		public Vector3 position;
		public Rotator rotation;
		public Vector3 scale;
		PolyMesh polyMesh;
		public string name;

		public MdlObject parent;
		public List<MdlObject> childs = new List<MdlObject>();
	
		public PolyMesh PolyMesh
		{
			get { return polyMesh; }
		}

		bool bTexturesLoaded;

		public void ApplyPolyMeshOperationR(Action<PolyMesh> f)
		{
			PolyMesh pm = PolyMesh;
			if (pm!=null) f(pm);
			foreach (MdlObject ch in childs)
				ch.ApplyPolyMeshOperationR(f);
		}

		public PolyMesh GetOrCreatePolyMesh()
		{
			if (polyMesh == null)
				polyMesh = new PolyMesh();
			return polyMesh;
		}

		public void InvalidateRenderData ()
		{
			if (polyMesh!=null)
				polyMesh.InvalidateRenderData();
		}

		public void RemoveChild(MdlObject o)
		{
			childs.Remove(o);
			o.parent = null;
		}

		public void AddChild(MdlObject o)
		{
			if (o.parent!=null)
				o.parent.RemoveChild(o);

			childs.Add(o);
			o.parent = this;
		}

		public bool IsEmpty ()
		{
			foreach (MdlObject o in childs) 
				if (!o.IsEmpty()) return false;

			PolyMesh pm = PolyMesh;
			return pm.poly.Length == 0;
		}

		public void Dump (int r)
		{
			for (int a=0;a<r;a++)
				Trace.Write("  ");
			Trace.WriteLine("MdlObject \'{0}\'", name);

			foreach (MdlObject ch in childs)
				ch.Dump(r+1);
		}

		public Matrix4x4 Transform
		{
			get {
				Matrix4x4 scaling = Matrix4x4.Scale(scale);
				Matrix4x4 rotationMatrix = rotation.ToMatrix();

				Matrix4x4 mat = scaling * rotationMatrix;
				mat.AddTranslation (position);
				return mat;
			}
		}

		Matrix4x4 FullTransform
		{
			get {
				Matrix4x4 m = Transform;

				if (parent != null) {
					Matrix4x4 parentTransform = parent.FullTransform;
					return parentTransform * m;
				}
				return m;
			}
		}

		/// <summary>
		/// Set position,rotation and scale from the transform matrix
		/// This does not support mirroring, so every component of scale has to be > 0
		/// </summary>
		/// <param name="transform"></param>
		public void SetPropertiesFromMatrix(Matrix4x4 transform)
		{
			position = transform.Translation;

			// extract scale and create a rotation matrix
			Vector3 cx = transform.Col(0);
			Vector3 cy = transform.Col(1);
			Vector3 cz = transform.Col(2);

			scale.x = cx.Length;
			scale.y = cy.Length;
			scale.z = cz.Length;

			Matrix4x4 rotationMatrix = Matrix4x4.Identity();
			rotationMatrix.SetCol(0, cx/scale.x);
			rotationMatrix.SetCol(1, cy/scale.y);
			rotationMatrix.SetCol(2, cz/scale.z);
			rotation.FromMatrix(rotationMatrix);
		}

		public void Load3DOTextures ()
		{
			if (!bTexturesLoaded) {
				polyMesh.Load3DOTextures();/*
				for (PolyIterator p(this); !p.End(); p.Next())
				{
					if (!p->texture && !p->texname.empty()) {
						p->texture = th->GetTexture (p->texname.c_str());
						if (p->texture) 
							p->texture->VideoInit();
						else
							p->texname.clear();
					}
				}*/
				bTexturesLoaded=true;
			}

			foreach (MdlObject o in childs)
				o.Load3DOTextures();
		}

		public void FlipPolygons()
		{
			ApplyPolyMeshOperationR(pm => pm.FlipPolygons());
		}

		// apply parent-space transform without modifying any transformation properties
		public void ApplyParentSpaceTransform(Matrix4x4 psTransform)
		{
			/*
			A = object transformation matrix to parent space
			T = given parent-space transform matrix
			p = original position
			p' = new position

			the new position, when transformed,
			needs to be equal to the transformed old position with scale/mirror applied to it:

			A*p' = SAp
			p' = (A^-1)SAp
			*/

			Matrix4x4 transform = Transform;
			Matrix4x4 inv = transform.Inverse();

			Matrix4x4 result = inv * psTransform;
			result *= transform;

			TransformVertices(result);

			// transform childs objects
			foreach(MdlObject o in childs) 
				o.ApplyParentSpaceTransform(result);
		}

		public void RemoveTransform (bool removeRotation, bool removeScaling, bool removePosition)
		{
			Matrix4x4 mat = Matrix4x4.Identity();
			if (removeScaling) {
				Matrix4x4 scaling = Matrix4x4.Scale(scale);
				scale = new Vector3(1,1,1);
				mat = scaling;
			}
			if (removeRotation) {
				Matrix4x4 rotationMatrix = rotation.ToMatrix();
				mat *= rotationMatrix;
				rotation = new Rotator();
			}
			
			if (removePosition) {
				mat.Translation = position;
				position = new Vector3();
			}
			ApplyTransform(mat);
		}

		public void TransformVertices (Matrix4x4 transform)
		{
			if (polyMesh != null)
				polyMesh.Transform(transform);
			InvalidateRenderData();
		}

		public void ApplyTransform (Matrix4x4 transform)
		{
			TransformVertices (transform);
		
			foreach(MdlObject o in childs) {
				Matrix4x4 subObjTr = o.Transform;
				subObjTr *= transform;
				o.SetPropertiesFromMatrix(subObjTr);
			}
		}

		public MdlObject Clone()
		{
			MdlObject cp = new MdlObject();

			if (polyMesh != null)
				cp.polyMesh = polyMesh.Clone();

			foreach (MdlObject o in childs)
				cp.AddChild(o.Clone());

			cp.position = position;
			cp.rotation = rotation;
			cp.scale = scale;
			cp.name = name;
			return cp;
		}

		public void ApproximateOffset()
		{
			Vector3 mid = polyMesh.CalcAveragePos();
			position += mid;

			TransformVertices(Matrix4x4.Translate(-mid));
		}

		public void UnlinkFromParent ()
		{
			if (parent!=null) {
				parent.RemoveChild(this);
			}
		}

		public void LinkToParent(MdlObject p)
		{
			if (parent!=null) 
				UnlinkFromParent();

			p.AddChild(this);
		}

		public List<MdlObject> GetChildObjects()
		{
			List<MdlObject> objects = new List<MdlObject>();

			foreach (MdlObject o in childs) {
				objects.AddRange(o.GetChildObjects());
				objects.Add(o);
			}

			return objects;
		}

		public void FullMerge ()
		{
			List<MdlObject> ch = childs;
			foreach (MdlObject o in ch) {
				o.FullMerge();
				MergeChild(o);
			}
		}

		public void MergeChild (MdlObject ch)
		{
			ch.RemoveTransform(true,true,true);
			PolyMesh pm = ch.PolyMesh;
			if (pm!=null)
				pm.MoveGeometry(GetOrCreatePolyMesh());

			// move the childs
			foreach (MdlObject o in ch.childs)
				o.parent = this;
			childs.AddRange(ch.childs);
			ch.childs.Clear();

			// delete the child
			RemoveChild(ch);
		}

		void MoveOrigin(Vector3 move)
		{
			// Calculate inverse
			Matrix4x4 tr = Transform;

			Matrix4x4 inv = tr.Inverse();
			if (inv != null) // Origin-move only works for objects with inverse transform (scale can't contain zero)
			{
				// move the object
				position += move;
				ApplyTransform(tr * Matrix4x4.Translate(-move) * inv);
			}
		}
	}
}
