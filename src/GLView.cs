using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Tao.Platform.Windows;
using Tao.OpenGl;
using Tao.DevIl;

using System.Drawing;
using ce.math;
using ce.engine.geom;
using ce.engine.gl;

namespace UpspringSharp
{
	using GLFont = ce.engine.gl.Font;

	public partial class GLView : UserControl
	{
		GLFont font;

		Color backgroundColor;
		Camera.Mode cameraMode;

		public Camera.Mode CameraMode {
			get { return cameraMode;  }
		}

		public enum ViewMode
		{
			// view modes
			WIRE,
			SOLID,
			TEX
		}

		public GLView()
		{
			InitializeComponent();
		}

		private void GLView_Load(object sender, EventArgs e)
		{
			glControl.InitializeContexts();

			if (DesignMode)
				return;

			MakeCurrent();
			Ilut.ilutInit();

			font = GLFont.LoadFont("Times", 10.0f, System.Drawing.FontStyle.Regular);
		}

		public new void MakeCurrent() {
			glControl.MakeCurrent();
		}

		static string[] AxisLabel = new string[] { "x", "y", "z" };
		static Vector3[] AxisVector = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) };

		private void RenderAllAxis()
		{
			for (int a = 0; a < 3; a++) {
				Ray ray = new Ray(new Vector3(), AxisVector[a]);
				Vector3[] hits = camera.Frustum.RayIntersect(ray);

				if (hits.Length >= 2) {
					GLUtil.Color(AxisVector[a]);
					GLUtil.Line(hits[0], hits[1]);

					// Print the label on the far hit pos
					Vector3 wPos = (from h in hits orderby (h - camera.Position).SqLength select h).Last();
					Vector2 pos = camera.ProjectPoint(wPos);
					PrintClampedCentered(pos.x, camera.Height - pos.y, AxisLabel[a], 1.2f);
				}
			}

			//	Vector2 p = camera.ProjectPoint(new Vector3());
			//	PrintClampedCentered(p.x,camera.Height - p.y, "o", 0.8f);

			//	PrintClampedCentered(lastMousePos.X, camera.Height - lastMousePos.Y, "mp", 0.8f);
		}



		private void PrintClampedCentered(float x, float y, string txt, float scale)
		{
			float w = GLFont.StdFont.GetTextWidth(txt) * scale * 0.5f;
			if (x < w) x = w;
			if (x > glControl.Width - w) x = glControl.Width - w;

			float h = GLFont.StdFont.GetTextHeight() * scale * 0.5f;
			if (y < h) y = h;
			if (y > glControl.Height - h) y = glControl.Height - h;

			GLFont.StdFont.Print(x - w, y, txt, scale);
		}
		

		void InitGL ()
		{
			Gl.glViewport(0,0,w(), h());
			Gl.glShadeModel(Gl.GL_SMOOTH);
			Gl.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
			Gl.glClearDepth(1.0f);
			Gl.glDepthFunc(Gl.GL_LEQUAL);
			Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
			Gl.glEnable(Gl.GL_DEPTH_TEST);
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glDisable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glEnable (Gl.GL_POINT_SMOOTH);
			Gl.glFrontFace (Gl.GL_CW);
		}

		void SetRasterMatrix ()
		{
			Gl.glMatrixMode (Gl.GL_PROJECTION);
			Gl.glLoadIdentity ();
			Gl.glScalef(2.0f / (float)w(), -2.0f / (float)h(), 1.0f);
			Gl.glTranslatef (-((float)w() / 2.0f), -((float)h() / 2.0f), 0.0f);
			Gl.glMatrixMode (Gl.GL_MODELVIEW);
		}

		void Render() 
		{
			Gl.glClearColor(backgroundColor.R/255.0f, backgroundColor.G/255.0f, backgroundColor.B/255.0f, 0.0f);
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glLoadIdentity();
			SetupProjectionMatrix();

			Gl.DrawScene();
			Gl.SetRasterMatrix();
			Gl.Draw2D();

		}
	}
}
