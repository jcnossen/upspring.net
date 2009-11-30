/*
 * Quaternion class.
 * Originally based on Allegro's C quaterion routines (alleg.sf.net)
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ce.math
{	
	[System.ComponentModel.TypeConverter(typeof(ce.util.StructTypeConverter<Quaternion>))]
	public struct Quaternion
	{
		public enum SlerpType
		{
			/// <summary>
			/// use shortest path
			/// </summary>
			qshort, 
			/// <summary>
			/// rotation will be greater than 180 degrees
			/// </summary>
			qlong, 
			/// <summary>
			/// rotate clockwise when viewed from above
			/// </summary>
			qcw, 
			/// <summary>
			/// rotate counterclockwise when viewed from above
			/// </summary>
			qccw,
			/// <summary>
			/// the quaternions are interpolated exactly as given
			/// </summary>
			quser
		};

		public float x, y, z, w;

		static float sinf (float r) { return (float)Math.Sin(r); }
		static float cosf (float r) { return (float)Math.Cos(r); }

		public Quaternion(float X,float Y,float Z, float W)
		{ 
			x=X;y=Y;z=Z;w=W;
		}

		public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

		public Quaternion Conjugate()
		{
			return new Quaternion(-x, -y, -z, w);
		}

		public float Normal() { return ((w*w) + (x*x) + (y*y) + (z*z)); }

		/// <summary>
		/// Multiplies two quaternions, storing the result in out. The resulting
		/// quaternion will have the same effect as the combination of p and q,
		/// ie. when applied to a point, (point * out) = ((point * p) * q). Any
		/// number of rotations can be concatenated in this way. Note that quaternion
		/// multiplication is not commutative, ie. quat_mul(p, q) != quat_mul(q, p).
		/// </summary>
		public Quaternion Multiply (Quaternion q)
		{
			Quaternion ret;

			/* qp = ww' - v.v' + vxv' + wv' + w'v */

			/* w" = ww' - xx' - yy' - zz' */
			ret.w = (w * q.w) - (x * q.x) -	(y * q.y) -	(z * q.z);

			/* x" = wx' + xw' + yz' - zy' */
			ret.x = (w * q.x) + (x * q.w) +  (y * q.z) -(z * q.y);

		/* y" = wy' + yw' + zx' - xz' */
			ret.y = (w * q.y) + (y * q.w) + (z * q.x) - (x * q.z);

			/* z" = wz' + zw' + xy' - yx' */
			ret.z = w * q.z + z * q.w + x * q.y - y * q.x;

			return ret;
		}

		public static Quaternion operator*(Quaternion a, Quaternion b) {
			return a.Multiply(b);
		}

		/// <summary>
		///  Construct X axis rotation quaternion
		/// </summary>
		public static Quaternion XRotate (float r)
		{
			Quaternion q;
			q.w = cosf(r / 2);
			q.x = sinf(r / 2);
			q.y = 0;
			q.z = 0;
			return q;
		}

		/// <summary>
		///  Construct Y axis rotation quaternion
		/// </summary>
		public static Quaternion YRotate (float r)
		{
			Quaternion q;
			q.w = cosf(r / 2.0f);
			q.x = 0;
			q.y = sinf(r / 2.0f);
			q.z = 0;
			return q;
		}

		/// <summary>
		/// Construct Z axis rotation quaternion
		/// </summary>
		public static Quaternion ZRotate (float r)
		{
			Quaternion q;
			q.w = cosf(r / 2);
			q.x = 0;
			q.y = 0;
			q.z = sinf(r / 2);
			return q;
		}

		public static Quaternion Rotation (Vector3 v)
		{
			return Rotation(v.x,v.y,v.z);
		}


		/// <summary>
		///  Constructs a quaternion which will rotate points around all three axis by
		///  the specified amounts
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="z"></param>
		public static Quaternion Rotation (float x, float y, float z)
		{
			float sx, sy, sz;
			float cx, cy, cz;
			float cycz, sysz;

			sx = sinf(x / 2);
			sy = sinf(y / 2);
			sz = sinf(z / 2);
			cx = cosf(x / 2);
			cy = cosf(y / 2);
			cz = cosf(z / 2);

			sysz = sy * sz;
			cycz = cy * cz;

			Quaternion q;
			q.w = (cx * cycz) + (sx * sysz);
			q.x = (sx * cycz) - (cx * sysz);
			q.y = (cx * sy * cz) + (sx * cy * sz);
			q.z = (cx * cy * sz) - (sx * sy * cz);
			return q;
		}

		/// <summary>
		/// Constructs a quaternion which will rotate points around the specified x,y,z vector by the specified angle 
		/// </summary>
		public static Quaternion VectorRotation(Vector3 axis, float r)
		{
			float s;
			axis.Normalize();

			Quaternion q;
			q.w = cosf(r / 2);
			s = sinf(r / 2);
			q.x = s * axis.x;
			q.y = s * axis.y;
			q.z = s * axis.z;
			return q;
		}

			/* This is the layout for converting the values in a quaternion to a
			* matrix.
			*
			*  | ww + xx - yy - zz       2xy + 2wz             2xz - 2wy     |
			*  |     2xy - 2wz       ww - xx + yy - zz         2yz - 2wx     |
			*  |     2xz + 2wy           2yz - 2wx         ww + xx - yy - zz |
			*/

		public Matrix3x3 ToMatrix3x3()
		{
			Matrix3x3 m = new Matrix3x3();
			float ww = w * w, xx = x * x, yy = y * y, zz = z * z;
			float wx = w * x * 2;
			float wy = w * y * 2;
			float wz = w * z * 2;
			float xy = x * y * 2;
			float xz = x * z * 2;
			float yz = y * z * 2;

			m._11 =  ww + xx - yy - zz;
			m._21 = xy - wz;
			m._31 = xz + wy;

			m._12 = xy + wz;
			m._22 = ww - xx + yy - zz;
			m._32 = yz - wx;

			m._13 = xz - wy;
			m._23 = yz + wx;
			m._33 = ww - xx - yy + zz;
			return m;
		}

		public Matrix4x4 ToMatrix()
		{
			Matrix4x4 m = new Matrix4x4();
			float ww = w * w, xx = x * x, yy = y * y, zz = z * z;
			float wx = w * x * 2;
			float wy = w * y * 2;
			float wz = w * z * 2;
			float xy = x * y * 2;
			float xz = x * z * 2;
			float yz = y * z * 2;

			m[0,0] = ww + xx - yy - zz;
			m[1,0] = xy - wz;
			m[2,0] = xz + wy;

			m[0,1] = xy + wz;
			m[1,1] = ww - xx + yy - zz;
			m[2,1] = yz - wx;

			m[0,2] = xz - wy;
			m[1,2] = yz + wx;
			m[2,2] = ww - xx - yy + zz;

			m[3,3] = 1.0f;
			return m;
		}

		/// <summary>
		/// Constructs a quaternion from a rotation matrix. Translation is discarded
		/// during the conversion. The matrix needs to be orthonormalized
		/// </summary>
		public static Quaternion FromMatrix (Matrix4x4 m)
		{
			Quaternion q;
			float diag, s;
			int i,j,k;

			int[] next = new int[] { 1, 2, 0 };
			diag = m[0,0] + m[1,1] + m[2,2];

			if (diag > 0.0f) 
			{
				s    = (float)(Math.Sqrt(diag + 1.0));
				q.w = s / 2.0f;
				s    = 0.5f / s;
				q.x = (m[1,2] - m[2,1]) * s;
				q.y = (m[2,0] - m[0,2]) * s;
				q.z = (m[0,1] - m[1,0]) * s;
			}
			else 
			{
				i = 0;

				if(m[1,1] > m[0,0]) i = 1;
				if(m[2,2] > m[i,i]) i = 2;

				j = next[i];
				k = next[j];

				s = m[i,i] - (m[j,j] + m[k,k]);

				/* 
				* NOTE: Passing non-orthonormalized matrices can result in odd things
				*       happening, like the calculation of s below trying to find the
				*       square-root of a negative number, which is imaginary.  Some
				*       implementations of sqrt will crash, while others return this
				*       as not-a-number (NaN). NaN could be very subtle because it will
				*       not throw an exception on Intel processors.
				*/
				Debug.Assert(s > 0.0);

				s = (float)(Math.Sqrt(s) + 1.0);

				float[] o = new float[4];
				o[i] = s / 2.0f;
				s      = 0.5f / s;
				o[j] = (m[i,j] + m[j,i]) * s;
				o[k] = (m[i,k] + m[k,i]) * s;
				o[3] = (m[j,k] - m[k,j]) * s;

				q.x = o[0];
				q.y = o[1];
				q.z = o[2];
				q.w = o[3];
			}
			return q;
		}

		/// <summary>
		/// A quaternion inverse is the conjugate divided by the normal.
		/// </summary>
		/// <returns></returns>
		public Quaternion Inverse ()
		{
			/* q^-1 = q^* / N(q) */
			Quaternion con = Conjugate();
			float norm = Normal();

			/* NOTE: If the normal is 0 then a divide-by-zero exception will occur, we
			*       let this happen because the inverse of a zero quaternion is
			*       undefined
			*/
			Debug.Assert(norm != 0.0f);

			return new Quaternion(x/norm, y/norm, z/norm, w/norm);
		}

		/// <summary>
		/// Multiplies the point (x, y, z) by the quaternion q, storing the result in
		/// (*xout, *yout, *zout). This is quite a bit slower than apply_matrix_f.
		/// So, only use it if you only need to translate a handful of points.
		/// Otherwise it is much more efficient to call quat_to_matrix then use
		/// apply_matrix_f.
		/// </summary>
		public Vector3 Apply(Vector3 pos)
		{
			Vector3 o;
			Quaternion v;
			Quaternion i;
			Quaternion t;

			/* v' = q * v * q^-1 */
			v.w = 0.0f;
			v.x = x;
			v.y = y;
			v.z = z;

			/* NOTE: Rotating about a zero quaternion is undefined. This can be shown
			*       by the fact that the inverse of a zero quaternion is undefined
			*       and therefore causes an exception below.
			*/
			Debug.Assert(!((x == 0) && (y == 0) && (z == 0)));

			i = Inverse();
			t = i * v;
			v = t * this;

			o.x = v.x;
			o.y = v.y;
			o.z = v.z;
			return o;
		}

		/// <summary>
		/// Constructs a quaternion that represents a rotation between 'from' and
		/// 'to'. The argument 't' can be anything between 0 and 1 and represents
		/// where between from and to the result will be.  0 returns 'from', 1
		/// returns 'to', and 0.5 will return a rotation exactly in between. The
		/// result is copied to 'out'.
		/// 
		/// The variable 'how' can be any one of the following:
		/// 
		/// qshort - This equivalent quat_interpolate, the rotation will be less than 180 degrees
		/// qlong  - rotation will be greater than 180 degrees
		/// qcw    - rotation will be clockwise when viewed from above
		/// qccw   - rotation will be counterclockwise when viewed from above
		/// quser  - the quaternions are interpolated exactly as given
		/// </summary>
		public static Quaternion Slerp(Quaternion from, Quaternion to, float t, SlerpType how)
		{
			Quaternion q;
			Quaternion to2;
			double angle;
			double cos_angle;
			double scale_from;
			double scale_to;
			double sin_angle;

			cos_angle = (from.x * to.x) + (from.y * to.y) + (from.z * to.z) + (from.w * to.w);

			if (((how == SlerpType.qshort) && (cos_angle < 0.0)) ||
				((how == SlerpType.qlong) && (cos_angle > 0.0)) ||
				((how == SlerpType.qcw) && (from.w > to.w)) ||
				((how == SlerpType.qccw) && (from.w < to.w)))
			{
				cos_angle = -cos_angle;
				to2.w = -to.w;
				to2.x = -to.x;
				to2.y = -to.y;
				to2.z = -to.z;
			}
			else
				to2 = to;

			if ((1.0 - Math.Abs(cos_angle)) > Util.Epsilon) 
			{
				/* spherical linear interpolation (SLERP) */
				angle = Math.Acos(cos_angle);
				sin_angle  = Math.Sin(angle);
				scale_from = Math.Sin((1.0 - t) * angle) / sin_angle;
				scale_to   = Math.Sin(t         * angle) / sin_angle;
			}
			else {
				/* to prevent divide-by-zero, resort to linear interpolation */
				scale_from = 1.0 - t;
				scale_to   = t;
			}

			q.w = (float)((scale_from * from.w) + (scale_to * to2.w));
			q.x = (float)((scale_from * from.x) + (scale_to * to2.x));
			q.y = (float)((scale_from * from.y) + (scale_to * to2.y));
			q.z = (float)((scale_from * from.z) + (scale_to * to2.z));
			return q;
		}

		public void Normalize ()
		{
			float dot = (x * x) + (y * y) + (z * z) + (w * w);
			Debug.Assert(dot != 0.0f);

			float scale = 1.0f / (float)Math.Sqrt(dot);
			x *= scale;
			y *= scale;
			z *= scale;
			w *= scale;
		}


	}
}
