using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ce.math
{
	/// <summary>
	/// 3x3 matrices, mainly implemented to store 3D orientations
	/// </summary>
	public class Matrix3x3
	{
		public float _11, _12, _13;
		public float _21, _22, _23;
		public float _31, _32, _33;

		public void OrthoNormalize()
		{

		}

		public static Matrix3x3 Identity
		{
			get {
				Matrix3x3 m = new Matrix3x3();
				m._11 = m._22 = m._33 = 1.0f;
				return m;
			}
		}

		public Vector3 ColX 
		{
			get { return new Vector3(_11, _21, _31); }
			set { _11 = value.x; _21 = value.y; _31 = value.z; }
		}
		public Vector3 ColY 
		{
			get { return new Vector3(_12, _22, _32); }
			set { _12 = value.x; _22 = value.y; _32 = value.z; }
		}
		public Vector3 ColZ 
		{
			get { return new Vector3(_13, _23, _33); }
			set { _13 = value.x; _23 = value.y; _33 = value.z; }
		}

		public Vector3 Apply(Vector3 v)
		{
			return new Vector3(
				v.x * _11 + v.y * _12 + v.z * _13,
				v.x * _21 + v.y * _22 + v.z * _23,
				v.x * _31 + v.y * _32 + v.z * _33);
		}

		public Matrix4x4 ToMatrix4x4()
		{
			Matrix4x4 m = new Matrix4x4();
			m[0, 0] = _11; m[0, 1] = _12; m[0, 2] = _13; 
			m[1, 0] = _21; m[1, 1] = _22; m[1, 2] = _23;
			m[2, 0] = _31; m[2, 1] = _32; m[2, 2] = _33;
			m[3, 3] = 1.0f;
			return m;
		}

		public Matrix3x3(Vector3 X, Vector3 Y, Vector3 Z)
		{
			ColX = X;
			ColY = Y;
			ColZ = Z;
		}

		public Matrix3x3()
		{	}

		public Matrix3x3(Matrix4x4 org)
		{
			_11 = org[0, 0]; _12 = org[0, 1]; _13 = org[0, 2];
			_21 = org[1, 0]; _22 = org[1, 1]; _23 = org[1, 2];
			_31 = org[2, 0]; _32 = org[2, 1]; _33 = org[2, 2];
		}

		public static Matrix3x3 FromAngularSpeed(Vector3 v)
		{
			Vector3 nv = v;
			float amount = nv.Normalize();

			return new Matrix3x3(Matrix4x4.VectorRotation(nv, amount));
		}
	}
}
