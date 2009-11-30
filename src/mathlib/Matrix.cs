using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

// #define USE_NUNIT
#if USE_NUNIT
using NUnit.Framework;
#endif

namespace ce.math
{
	public class Matrix
	{
		int nr, nc;
		public float[] v;

		public int Rows
		{
			get { return nr; }
		}

		public int Cols
		{
			get { return nc; }
		}

		Matrix()
		{}

		public float this[int r, int c]
		{
			get { return v[r * nc + c]; }
			set { v[r * nc + c] = value; }
		}

		public Matrix(int rows, int cols)
		{
			v = new float[rows * cols];
			nr = rows;
			nc = cols;
		}

		public Matrix(int rows, float[] data)
		{
			nr = rows;
			v = data;
			nc = data.Length / nr;
		}


		public static Matrix Identity(int n)
		{
			Matrix m = new Matrix(n, n);
			for (int i = n - 1; i >= 0; i--)
				m.v[i] = 1.0f;
			return m;
		}

		public static unsafe Matrix operator*(Matrix A, Matrix B)
		{
			Debug.Assert(A.nc == B.nr);
			Matrix r = new Matrix(A.nr, B.nc);

			fixed (float* av = A.v)
			fixed (float* bv = B.v)
			fixed (float* rv = r.v)
			{
				float* dst = rv;
				for (int i = 0; i < A.nr; i++)
					for (int j = 0; j < B.nc; j++) {
						float acc = 0.0f;

						for (int u = 0; u < A.nc; u++)
							acc += av[i * A.nc + u] * bv[u * B.nc + j];

						*(dst++) = acc;
					}
			}
			return r;
		}

		public static Matrix operator+(Matrix A, Matrix B)
		{
			Debug.Assert(A.nc == B.nc);
			Debug.Assert(A.nr == B.nr);

			float[] d=new float[A.v.Length];
			for (int i = 0; i < A.v.Length; i++)
				d[i] = A.v[i] + B.v[i];
			return new Matrix(A.nr, d);
		}

		public static Matrix operator-(Matrix A, Matrix B)
		{
			Debug.Assert(A.nc == B.nc);
			Debug.Assert(A.nr == B.nr);

			float[] d = new float[A.v.Length];
			for (int i = 0; i < A.v.Length; i++)
				d[i] = A.v[i] - B.v[i];
			return new Matrix(A.nr, d);
		}

		public static Matrix operator*(Matrix A, float x)
		{
			Matrix r = new Matrix(A.nr, A.nc);
			for (int i = 0; i < r.v.Length; i++)
				r.v[i] = A.v[i] * x;
			return r;
		}

		public unsafe void ConvertToREF()
		{
			// pc=pivot column
			int cr = 0;

			fixed (float* u = v) {
				for (int pc = 0; pc < nc; pc++) {
			//		global::System.Diagnostics.Trace.WriteLine("pc = " + pc.ToString());
		//			Trace();

					// find row with highest value in pc
					int r, bestRow = 0;
					float bestVal = 0.0f;

					for (r = cr; r < nr; r++)
					{
						float val = u[r * nc + pc];
						if (Math.Abs(val) > Math.Abs(bestVal))
						{
							bestVal = val;
							bestRow = r;
						}
					}

					if (bestVal == 0.0f)
						continue;

					// swap rows
					if (bestRow != cr)
						SwapRows(bestRow, cr);

					// scale row to make pivot equal to 1
					float f = 1.0f / u[cr * nc + pc];
					for (int k = pc + 1; k < nc; k++)
						u[cr * nc + k] *= f;
					u[cr * nc + pc] = 1.0f;

					for (r=cr+1;r<nr;r++) {
						f = u[r * nc + pc];
						if (f != 0.0f) {
							for (int col = pc; col < nc; col++)
								u[r * nc + col] -= u[cr * nc + col] * f;
						}
						u[r * nc + pc] = 0.0f;
					}

					cr++;
				}
			}
		}

		public unsafe void Backsubstitute ()
		{
			fixed (float *u = v)
			{
				for (int c = nr - 1; c > 0; c--)
				{
					for (int i = 0; i < c; i++) {
						float f = u[i * nc + c];
						for (int j = c + 1; j < nc; j++)
							u[i * nc + j] -= u[c * nc + j] * f;
						u[i * nc + c] = 0.0f;
					}
				}
			}
		}

		/// <summary>
		/// Solve Ax = b using gauss-jordan elimination
		/// </summary>
		/// <param name="b"></param>
		/// <returns>x</returns>
		public float[] Solve(float[] b)
		{
			Debug.Assert(nc == b.Length);

			Matrix aug = new Matrix(nr, nc + 1);
			aug.LoadSubmatrix(this);

			for (int i = 0; i < nr; i++)
				aug[i, nc] = b[i];

			aug.ConvertToREF();
			aug.Backsubstitute();

			float[] x = new float[b.Length];
			for (int i = 0; i < nr; i++)
				x[i] = aug[i, nc];

			return x;
		}

		public void LoadSubmatrix(Matrix subm)
		{
			for (int i = 0; i < subm.nr; i++)
				for (int j = 0; j < subm.nc; j++)
					v[i * nc + j] = subm[i, j];
		}

		public void SwapRows(int r1, int r2)
		{
			for (int i = 0; i < nc; i++)
			{
				float tmp = this[r1, i];
				this[r1, i] = this[r2, i];
				this[r2, i] = tmp;
			}
		}

		public Matrix4x4 ToMatrix4x4()
		{
			Matrix4x4 r = new Matrix4x4();

			Debug.Assert(nc == 4);
			Debug.Assert(nr == 4);

			for (int i = 0; i < 16; i++)
				r[i] = v[i];

			return r;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int i=0;i<nr;i++) 
				for (int j=0;j<nc;j++) {
					sb.Append(this[i, j].ToString());
					sb.Append(j < nc - 1 ? "; " : "\n");
				}
			return sb.ToString();
		}

		public void Trace()
		{
			global::System.Diagnostics.Trace.Write(ToString());
		}
	}

#if USE_NUNIT

	[TestFixture]
	public class TestMatrix : Assert
	{
		[Test]
		public void ConvertToREF()
		{
			Matrix m = new Matrix(4, new float[]
			{
          1,  -1 ,  2,   -1,   -8,
          2,  -2 ,  3,   -3,  -20,
          1,   1 ,  1,    0,   -2,
          1,  -1 ,  4,    3,    4,
			});

			m.ConvertToREF();

			Matrix cmp = new Matrix(4, new float[]
			{
				1, -1, 1.5f, -1.5f, -10,
				0, 1, -0.25f, 0.75f,  4,
				0, 0,  1,     1.8f,   5.6f,
				0, 0,  0,     1,     2
			});

			for (int i = 0; i < 4 * 5; i++)
				IsTrue(Math.Abs(m.v[i] - cmp.v[i]) < Util.Epsilon);
		}

		[Test]
		public void Solve()
		{
			Matrix m = new Matrix(4, new float[]
			{
          1,  -1 ,  2,   -1,  
          2,  -2 ,  3,   -3,  
          1,   1 ,  1,    0,
          1,  -1 ,  4,    3,   
			});

			float[] b = new float[] { -8, -20, -2, 4 };

			float[] x = m.Solve(b);
			float[] wanted = new float[] { -7, 3, 2, 2 };
			for (int i = 0; i < 4; i++)
				IsTrue(Math.Abs(x[i] - wanted[i]) < Util.Epsilon);
		}
	}
#endif
}
