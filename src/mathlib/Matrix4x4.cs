using System;
using System.Collections.Generic;
using System.Text;

namespace ce.math
{
	public class Matrix4x4
	{
		public Matrix4x4(float[] d) { m = d; }
		public Matrix4x4() {}

		public float[] m = new float[16];

		public float this[int index]
		{
			get { return m[index]; }
			set { m[index] = value; }
		}

		public float this[int row, int col]
		{
			get { return m[row*4+col]; }
			set { m[row*4+col] = value; }
		}

		public Matrix4x4 Clone()
		{
			Matrix4x4 r = new Matrix4x4();
			for(int x=0;x<16;x++)
				r.m[x]=m[x];
			return r;
		}

		public Vector3 Front
		{
			get { return new Vector3(m[2], m[6], m[10]); }
			set { m[2]=value.x; m[6]=value.y; m[10]=value.z; }
		}
		public Vector3 Right
		{
			get { return new Vector3(m[0], m[4], m[8]); }
			set { m[0]=value.x;m[4]=value.y;m[8]=value.z; }
		}
		public Vector3 Up
		{
			get { return new Vector3(m[1], m[5], m[9]); }
			set { m[1]=value.x; m[5]=value.y; m[9]=value.z; }
		}
		public Vector3 Translation
		{
			get { return new Vector3(m[3], m[7], m[11]); }
			set { m[3] = value.x; m[7] = value.y; m[11] = value.z; }
		}

		public Vector3 Row (int r)
		{
			return new Vector3(v(r,0),v(r,1),v(r,2));
		}

		public Vector3 Col (int c)
		{
			return new Vector3(v(0,c),v(1,c),v(2,c));
		}

		public void SetRow(int r, Vector3 rv)
		{
			v(r,0, rv.x);
			v(r,1, rv.y);
			v(r,2, rv.z);
		}

		public void SetCol(int c, Vector3 cv)
		{
			v(0,c, cv.x);
			v(1,c, cv.y);
			v(2,c, cv.z);
		}

		// Allegro style accessors
		public float v(int row,int col)
		{
			return m[row*4+col];
		}

		public void v(int row, int col, float value)
		{
			m[row * 4 + col] = value;
		}

		public float t(int y) { return m[3+4*y]; }

		#region internal utils
		private static float msin (float x)
		{
			return (float)System.Math.Sin ((double)x);
		}

		private static float mcos (float x)
		{
			return (float)System.Math.Cos((double)x);
		}
		private static float atan2(float y,float x)
		{
			return (float)System.Math.Atan2((double)y,(double)x);
		}
		#endregion
		/*
		matrix layout:

		x       y       z       w
		x   0       1       2       3
		y   4       5       6       7
		z   8       9       10      11
		w   12      13      14      15
		*/

		public void Clear ()
		{
			for (int a=0;a<16;a++)
				m[a] = 0.0f;
		}

		public static Matrix4x4 Identity ()
		{
			Matrix4x4 m = new Matrix4x4();
			m[0] = 1.0f;
			m[5] = 1.0f;
			m[10] = 1.0f;
			m[15] = 1.0f;
			return m;
		}
		#region X,Y,Z Rotate
		public static Matrix4x4 ZRotate (float t)
		{
			Matrix4x4 m = new Matrix4x4();
			float st=msin(t), ct=mcos(t);
			/*
			| cos t -sin t  0 |
			Mr = | sin t  cos t  0 |
			| 0      0      0 |
			*/
			m[10] = m[15] = 1.0f;
			m[0] = ct;
			m[1] = -st;
			m[4] = st;
			m[5] = ct;
			return m;
		}

		public static Matrix4x4 YRotate (float t)
		{
			Matrix4x4 m = new Matrix4x4();
			float st=msin(t), ct=mcos(t);
			/*
			| cos t  0   -sin t |
			Mr = | 0      1     0    |
			| sin t  0    cos t |
			*/
			m[0] = ct;
			m[2] = -st;
			m[5] = 1.0f;
			m[8] = st;
			m[10] = ct;
			m[15] = 1.0f;
			return m;
		}

		public static Matrix4x4 XRotate (float t)
		{
			Matrix4x4 m = new Matrix4x4();
			float st = msin(t), ct = mcos(t);
			/*
			| 1     0      0   |
			Mr = | 0   cos t -sin t |
			| 0   sin t  cos t |
			*/
			m[0] = 1.0f;
			m[5] = ct;
			m[6] = -st;
			m[9] = st;
			m[10] = ct;
			m[15] = 1.0f;
			return m;
		}
		#endregion

		#region Euler angle conversions

		// Calculate a rotation matrix from euler angles. Rotation is applied in ZXY order
		public static Matrix4x4 EulerZXY(Vector3 rot)
		{
			Matrix4x4 m = new Matrix4x4();
			float cy = mcos(rot.y), sy = msin(rot.y);
			float cx = mcos(rot.x), sx = msin(rot.x);
			float cz = mcos(rot.z), sz = msin(rot.z);

			/*	[cz cy+sz sx sy,    -sz cx,     -cz sy+sz sx cy]
			[sz cy-cz sx sy,     cz cx,     -sz sy-cz sx cy]
			[cx sy,                sx,               cx cy]*/
			m[0] = cy*cz+sz*sx*sy;
			m[1] = -sz*cx;
			m[2] = -sy*cz+sz*sx*cy;
			m[4] = cy*sz-cz*sx*sy;
			m[5] = cx*cz;
			m[6] = -sy*sz-sx*cz*cy;
			m[8] = sy*cx;
			m[9] = sx;
			m[10] = cx*cy;
			m[15] = 1.0f;
			return m;
		}

		// Calculate a rotation matrix from euler angles. Rotation is applied in YXZ order
		public static Matrix4x4 EulerYXZ(Vector3 rot)
		{
			Matrix4x4 m = new Matrix4x4();
			float cy = mcos(rot.y), sy = msin(rot.y);
			float cx = mcos(rot.x), sx = msin(rot.x);
			float cz = mcos(rot.z), sz = msin(rot.z);

			/*
			[cz cy-sz sx sy,   -sz cy-cz sx sy,    -cx sy]
			[sz cx,            cz cx,                 -sx]
			[cz sy+sz sx cy,   -sz sy+cz sx cy,    cx cy]]
			*/
			
			m[0] = cz*cy-sz*sx*sy;
			m[1] = -sz*cy-cz*sx*sy;
			m[2] = -cx*sy;
			m[4] = sz*cx;
			m[5] = cz*cx;
			m[6] = -sx;
			m[8] = cz*sy+sz*sx*cy;
			m[9] = -sz*sy+cz*sx*cy;
			m[10] = cx*cy;
			m[15]=1.0f;
			return m;
		}

		// Assuming m is a pure rotation matrix
		public Vector3 CalcEulerZXY()
		{
			Vector3 r;

			// If fabs(sx) = 1, then cx is zero and the Y and Z angle (heading and bank) can't be calculated
			if (m[9] > 0.998f) {
			/*
			| cy*cz+sy*sz   0   -(sy*cz-cy*sz) |     | cos(Y-Z)   0  -cos(Y-Z) |
			|-(sy*cz-cy*sz) 0   -(cy*cz+sy*sz) |  =  | -sin(Y-Z)  0  cos(Y-Z)  |
			|   0           1          0       |     |    0       1     0      |

			sin(Y+Z) = sy*cz + cy*sz
			cos(Y+Z) = cy*cz - sy*sz
			sin(Y-Z) = sy*cz - cy*sz
			cos(Y-Z) = cy*cz + sy*sz

			*/
			r.y = atan2 (-m[4],m[0]);  // it can be represented with both Y rotation and Z rotation. Y is used here
			r.z = 0.0f;
			r.x = (float)System.Math.PI*0.5f;
			} else if (m[9] < -0.998f) {
			/*
			| cy*cz-sy*sz      0      -sy*cz-sz*cy  |     | cos(Y+Z)  ...
			| sy*cz+cy*sz      0      -sy*sz+cz*cy  |  =  | sin(Y+Z) ..
			|        0         -1            0      |     | 
			*/
			r.y = atan2(m[4], m[0]);
			r.z = 0.0f;
			r.x = -(float)System.Math.PI * 0.5f;
			} else {
			r.x = (float)System.Math.Asin ((double)m[9]);
			r.y = atan2(m[8], m[10]);
			r.z = atan2(-m[1], m[5]);
		}

		return r;
		}

		// Assuming m is a pure rotation matrix
		public Vector3 CalcEulerYXZ()
		{
			Vector3 r;
			/*
			[cz cy-sz sx sy,   -sz cy-cz sx sy,    -cx sy]  0 1 2
			[sz cx,            cz cx,                 -sx]  4 5 6
			[cz sy+sz sx cy,   -sz sy+cz sx cy,    cx cy]]  8 9 10
			*/

			if (m[6] > 0.998f) {
				// sx = -1, cx=0
				/*
				[cy*cz+sy*sz,   -cy*sz+sy*cz,   0]  0 1 2
				[0,              0,             1]  4 5 6
				[sy*cz-cy*sz,   -(cy*cz+sy*sz), 0]  8 9 10
				=
				[cos(y-z),   sin(y-z),   0]  0 1 2
				[0,              0,      1]  4 5 6
				[sin(y-z),   -cos(y-z),  0]  8 9 10
				*/

				/*
				sin(Y+Z) = sy*cz + cy*sz
				cos(Y+Z) = cy*cz - sy*sz
				sin(Y-Z) = sy*cz - cy*sz
				cos(Y-Z) = cy*cz + sy*sz
				*/
				r.y = atan2(m[1], m[0]);  // it can be represented with both Y rotation and Z rotation. Y is used here
				r.z = 0.0f;
				r.x = -(float)System.Math.PI * 0.5f;
				} else if (m[9] < -0.998f) {
				// sx = 1, cx = 0
				/*
				[cz cy-sz sy,   -sz cy-cz sy,    0]  0 1 2
				[0,               0,            -1]  4 5 6
				[cz sy+sz cy,   -sz sy+cz cy,    0]  8 9 10
				=
				[cos(y+z),      -sin(y+z),  0]  0 1 2
				[0,               0,            -1]  4 5 6
				[sin(y+z),      cos(y+z),    0]  8 9 10
				*/
				r.y = atan2(m[8], m[0]);
				r.z = 0.0f;
				r.x = (float)System.Math.PI * 0.5f;
			} else {
				r.y = atan2(-m[2],m[10]);
				r.z = atan2(m[4], m[5]);
				r.x = (float)System.Math.Asin ((double)-m[6]);
			}

			return r;
		}
		#endregion

		#region Multiply
		public Matrix4x4 Multiply (Matrix4x4 t)
		{
			Matrix4x4 dst = new Matrix4x4();
			Matrix4x4 a = t, b = this;

			for (int row=0;row<4;row++)
				for (int col=0;col<4;col++)
				{
					dst.v(row, col, a.v(row, 0) * b.v(0,col) + a.v(row,1) * b.v(1,col) + 
							a.v(row,2) * b.v(2,col) + a.v(row,3) * b.v(3,col));
				}
			return dst;
		}

		public static Matrix4x4 operator*(Matrix4x4 a,Matrix4x4 b)
		{
			return a.Multiply(b);
		}

		public void Multiply(float a)
		{
			for (int i=0;i<16;i++)
				m[i] *= a;
		}

		public static Matrix4x4 operator*(Matrix4x4 a, float f)
		{
			Matrix4x4 r = new Matrix4x4();
			for(int x=0;x<16;x++)
				r[x] = a[x]*f;
			return r;
		}
		#endregion

		#region Matrix inversion
		//-----------------------------------------------------------------------
		private float Minor(int r0, int r1, int r2, int c0, int c1, int c2)
		{
			return m[r0*4+c0] * (m[r1*4+c1] * m[r2*4+c2] - m[r2*4+c1] * m[r1*4+c2]) -
				m[r0*4+c1] * (m[r1*4+c0] * m[r2*4+c2] - m[r2*4+c0] * m[r1*4+c2]) +
				m[r0*4+c2] * (m[r1*4+c0] * m[r2*4+c1] - m[r2*4+c0] * m[r1*4+c1]);
		}

		public Matrix4x4 Adjoint()
		{
			Matrix4x4 r = new Matrix4x4();

			r[0] = Minor(1, 2, 3, 1, 2, 3);
			r[1] = -Minor(0, 2, 3, 1, 2, 3);
			r[2] = Minor(0, 1, 3, 1, 2, 3);
			r[3] = -Minor(0, 1, 2, 1, 2, 3);

			r[4] = -Minor(1, 2, 3, 0, 2, 3);
			r[5] = Minor(0, 2, 3, 0, 2, 3);
			r[6] = -Minor(0, 1, 3, 0, 2, 3);
			r[7] = Minor(0, 1, 2, 0, 2, 3);

			r[8] = Minor(1, 2, 3, 0, 1, 3);
			r[9] = -Minor(0, 2, 3, 0, 1, 3);
			r[10] = Minor(0, 1, 3, 0, 1, 3);
			r[11] = -Minor(0, 1, 2, 0, 1, 3);

			r[12] = -Minor(1, 2, 3, 0, 1, 2);
			r[13] = Minor(0, 2, 3, 0, 1, 2);
			r[14] = -Minor(0, 1, 3, 0, 1, 2);
			r[15] = Minor(0, 1, 2, 0, 1, 2);

			return r;
		}

		public float Determinant() 
		{
			return m[0*4+0] * Minor(1, 2, 3, 1, 2, 3) -
						m[0*4+1] * Minor(1, 2, 3, 0, 2, 3) +
						m[0*4+2] * Minor(1, 2, 3, 0, 1, 3) -
						m[0*4+3] * Minor(1, 2, 3, 0, 1, 2);
		}

		public Matrix4x4 Inverse()
		{
			Matrix4x4 o = new Matrix4x4();
			float det = Determinant();
			if(System.Math.Abs(det) < Util.Epsilon)
				return null;

			o = Adjoint();
			o *= 1.0f / det;
			return o;
		}
		#endregion

		public static Matrix4x4 Scale(float f)
		{
			Matrix4x4 m = new Matrix4x4();
			m[0] = f;
			m[5] = f;
			m[10] = f;
			m[15] = 1.0f;
			return m;
		}

		public static Matrix4x4 Scale (Vector3 s)
		{
			Matrix4x4 m = new Matrix4x4();
			m[0] = s.x;
			m[5] = s.y;
			m[10] = s.z;
			m[15] = 1.0f;
			return m;
		}

		public static Matrix4x4 Scale (float sx, float sy, float sz)
		{
			Matrix4x4 m = new Matrix4x4();
			m[0] = sx;
			m[5] = sy;
			m[10] = sz;
			m[15] = 1.0f;
			return m;
		}

		public static Matrix4x4 Translate (Vector3 t)
		{
			Matrix4x4 m = Identity();
			m[3] = t.x;
			m[7] = t.y;
			m[11] = t.z;
			return m;
		}

		public static Matrix4x4 Translate (float tx,float ty,float tz)
		{
			Matrix4x4 m = Identity();
			m[3] = tx;
			m[7] = ty;
			m[11] = tz;
			return m;
		}

		public Matrix4x4 Transpose ()
		{
			Matrix4x4 r = new Matrix4x4();
			int x,y;
			for(y=0;y<4;y++)
				for(x=0;x<4;x++)
					r.m[x*4+y] = m[y*4+x];
			return r;
		}

		public Vector3 Transform (Vector3 v)
		{
			// w component is 1
			Vector3 o;
			o.x = m[0] * v.x + m[1] * v.y + m[2] * v.z + m[3];
			o.y = m[4] * v.x + m[5] * v.y + m[6] * v.z + m[7];
			o.z = m[8] * v.x + m[9] * v.y + m[10] * v.z + m[11];
			return o;
		}

		/// <summary>
		/// Transform a vector using only the 3x3 rotation part of the matrix
		/// </summary>
		public Vector3 TransformNormal (Vector3 v)
		{
			// w component is 0
			Vector3 o;
			o.x = m[0] * v.x + m[1] * v.y + m[2] * v.z;
			o.y = m[4] * v.x + m[5] * v.y + m[6] * v.z;
			o.z = m[8] * v.x + m[9] * v.y + m[10] * v.z;
			return o;
		}

		public Vector4 Transform (Vector4 v)
		{
			Vector4 o;
			o.x = m[0] * v.x + m[1] * v.y + m[2] * v.z + m[3] * v.w;
			o.y = m[4] * v.x + m[5] * v.y + m[6] * v.z + m[7] * v.w;
			o.z = m[8] * v.x + m[9] * v.y + m[10] * v.z + m[11] * v.w;
			o.w = m[12] * v.x + m[13] * v.y + m[14] * v.z + m[15] * v.w;
			return o;
		}

		public static Matrix4x4 VectorRotation (Vector3 axis, float angle)
		{
			Matrix4x4 m = new Matrix4x4();
			float c = mcos(angle);
			float s = msin(angle);
			float cc = 1 - c;

			axis.Normalize ();

			m[0,0] = (cc * axis.x * axis.x) + c;
			m[0,1] = (cc * axis.x * axis.y) + (axis.z * s);
			m[0,2] = (cc * axis.x * axis.z) - (axis.y * s);

			m[1,0] = (cc * axis.x * axis.y) - (axis.z * s);
			m[1,1] = (cc * axis.y * axis.y) + c;
			m[1,2] = (cc * axis.z * axis.y) + (axis.x * s);

			m[2,0] = (cc * axis.x * axis.z) + (axis.y * s);
			m[2,1] = (cc * axis.y * axis.z) - (axis.x * s);
			m[2,2] = (cc * axis.z * axis.z) + c;
			m[3,3] = 1.0f;
			return m;
		}


		/// <summary>
		/// Produces a perspective matrix similar to gluPerspective (This is called a left handed coordinate system IIRC)
		/// </summary>
		/// <param name="fovY">Field of view in radians</param>
		/// <param name="aspect"></param>
		/// <param name="zNear"></param>
		/// <param name="zFar"></param>
		/// <returns></returns>
		public static Matrix4x4 Perspective(float fovY, float aspect, float zNear, float zFar)
		{
			float f = 1.0f / (float)Math.Tan(fovY / 2.0);
			Matrix4x4 m = new Matrix4x4();

			m[0, 0] = f / aspect;
			m[1, 1] = f;
			m[2, 2] = (zFar + zNear) / (zNear - zFar);
			m[3, 2] = -1.0f;
			m[2, 3] = (2.0f * zFar * zNear) / (zNear - zFar);
			return m;
		}

		/// <summary>
		/// Produces a right handed perspective matrix
		/// </summary>
		/// <param name="fovY">Field of view in radians</param>
		/// <param name="aspect">W/H</param>
		/// <param name="zNear"></param>
		/// <param name="zFar"></param>
		/// <returns></returns>
		public static Matrix4x4 PerspectiveRH(float fovY, float aspect, float zNear, float zFar)
		{
			Matrix4x4 m = Matrix4x4.Perspective(fovY, aspect, zNear, zFar);
			for(int i=0;i<4;i++) // glScalef(1,1,-1)
				m[i, 2] = -m[i, 2];
			return m;
		}

		public Vector3[] Get3x3CoordSys()
		{
			return new Vector3[] {
				new Vector3(m[0],m[1],m[2]),
				new Vector3(m[4],m[4],m[5]),
				new Vector3(m[8],m[9],m[10])
			};
		}

		public void SetCameraPos(Vector3 pos)
		{
			m[3] = -pos.Dot(Row(0));
			m[7] = -pos.Dot(Row(1));
			m[11] = -pos.Dot(Row(2));
		}

		public float[] Get3x3Matrix()
		{
			float[] r = new float[9];
			for (int y = 0; y < 3; y++)
				for (int x = 0; x < 3; x++)
					r[y * 3 + x] = m[y * 4 + x];
			return r;
		}

		public Matrix ToMatrix()
		{
			float[] d = new float[16];
			for (int i = 0; i < 16; i++)
				d[i] = m[i];
			return new Matrix(4, d);
		}

		public void AddTranslation(Vector3 v)
		{
			m[3] += v.x;
			m[7] += v.y;
			m[11] += v.z;
		}

		public Vector3 GetPos()
		{
			return new Vector3(m[3], m[7], m[11]);
		}
	}
}
