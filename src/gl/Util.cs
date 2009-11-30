using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Tao.OpenGl;
using ce.math;

namespace ce.engine.gl
{
	public static class GLUtil
	{
		public static void PushMatrix(Matrix4x4 m)
		{
			Gl.glPushMatrix();
			Gl.glMultTransposeMatrixf(m.m);
		}

		public static void PopMatrix()
		{
			Gl.glPopMatrix();
		}

		public static void Vertex3(Vector3 v)
		{
			Gl.glVertex3f(v.x, v.y, v.z);
		}

		public static void Line(Vector3 v1, Vector3 v2)
		{
			Gl.glBegin(Gl.GL_LINES);

			Vertex3(v1);
			Vertex3(v2);

			Gl.glEnd();
		}

		static public void PushOrthoMatrix()
		{
			Gl.glPushAttrib(Gl.GL_TRANSFORM_BIT);
			int[] viewport = new int[4];
			Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			Gl.glOrtho(viewport[0], viewport[2], viewport[1], viewport[3], 0, 1);
			Gl.glPopAttrib();
		}

		static public void PopProjectionMatrix()
		{
			Gl.glPushAttrib(Gl.GL_TRANSFORM_BIT);
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPopMatrix();
			Gl.glPopAttrib();
		}

		static public void Scale(Vector3 s)
		{
			Gl.glScalef(s.x, s.y, s.z);
		}

		static public void Translate(Vector3 s)
		{
			Gl.glTranslatef(s.x, s.y, s.z);
		}

		static public void CheckError()
		{
			int err = Gl.glGetError();
			if (err != Gl.GL_NO_ERROR) {
				string msg = Glu.gluErrorString(err);
				throw new Exception("GL Error: " + msg);
			}
		}

		static public void RenderBox(Vector3 a, Vector3 b)
		{
			Gl.glBegin(Gl.GL_QUADS);

			// Bottom quad
			Gl.glVertex3f(a.x, a.y, a.z);
			Gl.glVertex3f(a.x, a.y, b.z);
			Gl.glVertex3f(b.x, a.y, b.z);
			Gl.glVertex3f(b.x, a.y, a.z);

			// Right
			Gl.glVertex3f(a.x, a.y, a.z);
		
			Gl.glEnd();
		}

		static public void RenderCenterPoint(Vector3 a, float s)
		{
			RenderCenterPoint(a, s, new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) });
		}

		static public void RenderCenterPoint(Vector3 a, float s, Vector3 color)
		{
			RenderCenterPoint(a, s, new Vector3[] { color, color, color });
		}

		static public void RenderCenterPoint(Vector3 a, float s, Vector3[] colors)
		{
			Gl.glBegin(Gl.GL_LINES);
			if (colors != null)
				Gl.glColor3f(colors[0].x, colors[0].y, colors[0].z);
			Gl.glVertex3f(a.x - s, a.y, a.z);
			Gl.glVertex3f(a.x + s, a.y, a.z);

			if (colors != null)
				Gl.glColor3f(colors[1].x, colors[1].y, colors[1].z);
			Gl.glVertex3f(a.x, a.y - s, a.z);
			Gl.glVertex3f(a.x, a.y + s, a.z);

			if (colors != null)
				Gl.glColor3f(colors[2].x, colors[2].y, colors[2].z);
			Gl.glVertex3f(a.x, a.y, a.z - s);
			Gl.glVertex3f(a.x, a.y, a.z + s);
			Gl.glEnd();	
		}

		static public void RenderCilinder (Vector3 start, Vector3 end, float radius, int steps)
		{
			Vector3 up, right;
			(end - start).OrthoSpace(out up, out right);

			float step = 2*(float)Math.PI / steps;

			Gl.glBegin(Gl.GL_QUADS);

			float t = 0.0f;
			for (int i = 0; i < steps; i++) {
				float t2 = t + step;

				float c1 = radius * (float)Math.Cos(t);
				float s1 = radius * (float)Math.Sin(t);

				float c2 = radius * (float)Math.Cos(t2);
				float s2 = radius * (float)Math.Sin(t2);

				Vertex3(start + up * s1 + right * c1);
				Vertex3(start + up * s2 + right * c2);
				Vertex3(end + up * s2 + right * c2);
				Vertex3(end + up * s1 + right * c1);

				t = t2;
			}

			Gl.glEnd();
		}
		
		static public void RenderCone (Vector3 start, Vector3 end, float radius, int steps)
		{
			Vector3 up, right;
			(end - start).OrthoSpace(out up, out right);

			float step = 2*(float)Math.PI / steps;

			Gl.glBegin(Gl.GL_TRIANGLES);

			float t = 0.0f;
			for (int i = 0; i < steps; i++) {
				float t2 = t + step;

				float c1 = radius * (float)Math.Cos(t);
				float s1 = radius * (float)Math.Sin(t);

				float c2 = radius * (float)Math.Cos(t2);
				float s2 = radius * (float)Math.Sin(t2);

				Vertex3(start + up * s1 + right * c1);
				Vertex3(start + up * s2 + right * c2);
				Vertex3(end);

				t = t2;
			}

			Gl.glEnd();
		}

		static public void Render3DArrow(Vector3 start, Vector3 end, float radius, float headRadius, float headLength, int steps)
		{
			Vector3 headStart = start+(end-start)*(1.0f-headLength);
			RenderCilinder(start, headStart, radius, steps);
			RenderCone(headStart, end, headRadius, steps);
		}


		public static void Color(Vector3 c)
		{
			Gl.glColor3f(c.x, c.y, c.z);
		}
		public static void Color(Vector4 c)
		{
			Gl.glColor4f(c.x, c.y, c.z, c.w);
		}
		public static void Color(Color c)
		{
			Gl.glColor4f(c.R/255.0f, c.G/255.0f, c.B/255f, c.A/255.0f);
		}

		public static void RenderPlane(Plane plane, Vector2 texScale)
		{
			Vector3 U, V;
			float s = 10000.0f;
			Vector2 texPos = texScale * s;

			Vector3 mid = plane.Normal * plane.d;
			plane.Normal.OrthoSpace(out U, out V);
			U *= s;
			V *= s;
			
			Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(-texPos.x, -texPos.y);
				GLUtil.Vertex3(mid - U - V);

				Gl.glTexCoord2f(texPos.x, -texPos.y);
				GLUtil.Vertex3(mid + U - V);

				Gl.glTexCoord2f(texPos.x, texPos.y);
				GLUtil.Vertex3(mid + U + V);

				Gl.glTexCoord2f(-texPos.x, texPos.y);
				GLUtil.Vertex3(mid - U + V);
			Gl.glEnd();
		}

		public static void RenderLineLoop(Vector3[] v)
		{
			Gl.glBegin(Gl.GL_LINE_LOOP);

			foreach (Vector3 vrt in v)
				Vertex3(vrt);

			Gl.glEnd();
		}

		public static void Vertex2(Vector2 v)
		{
			Gl.glVertex2f(v.x, v.y);
		}
	}
}
