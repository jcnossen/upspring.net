using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using Tao.OpenGl;
using Tao.Platform.Windows;
using Tao.DevIl;

using ce.engine.gl;
using ce.math;
using ce.engine.rendering;

using ce.engine.geom;

namespace GeomEditor
{
	using GLFont = ce.engine.gl.Font;
	using PrimitiveType = ce.engine.geom.PrimitiveType;

	public partial class RenderCtl : UserControl, ISceneEditorRenderHost
	{
		static string[] AxisLabel = new string[] { "x", "y", "z" };
		static Vector3[] AxisVector = new Vector3[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1) };

		Point lastMousePos, clickMousePos;
		Camera camera;
		int fps, lastFps;
		float timeCounter;
		float camDistance = 10.0f; // distance from center
		Vector3 camOffset;
		bool cullFace = false;

		public enum TransformToolMode
		{
			Move, Rotate, Scale
		}
		TransformToolMode activeMode = TransformToolMode.Move;
		public event EventHandler TransformModeChanged;
		public TransformToolMode TransformMode
		{
			get { return activeMode; }
			set { activeMode=value; 
				if (TransformModeChanged!=null)
					TransformModeChanged(this, null);
			}
		}

		public PrimitiveType SelectionType = PrimitiveType.Object;

		// If nonzero: currently moving an anchor, so don't switch to camera moving now
		MoveAnchorArrow movingAnchor;

		Vector3 renderPoint;
		Vector3 testEdgeSt = new Vector3(2, 0, -4), testEdgeEnd = new Vector3(20, 0, 1);
		float dbgDist;

		bool renderAxis = true;
		bool renderGrid = true;
		
		HashSet<IEditorPrimitive> selection = new HashSet<IEditorPrimitive >();
		IEditorPrimitive Highlighted;
		MoveAnchorArrow[] moveAnchors;

		/// <summary>
		/// Returns all primitives that have to be drawn
		/// </summary>
		public IEnumerable<IEditorPrimitive> GetRenderPrimitives()
		{
			IEnumerable<IEditorPrimitive> prims = Program.AllPrimitives;

			if (moveAnchors != null)
				return prims.Concat(moveAnchors);

			return prims;
		}

		class Transformer : ITransformer
		{
			public Vector3 translation;
			public Vector3 scale;
			public Vector3 euler;

			public void Transform(ref Vector3 pos, ref Vector3 scale, ref Vector3 euler)
			{
				pos += translation;
				scale.Multiply(this.scale);
				euler += this.euler;
			}

			public Matrix4x4 TransformMatrix
			{
				get {
					Matrix4x4 transform = Matrix4x4.Scale(scale) * Matrix4x4.EulerYXZ(euler);
					transform.AddTranslation(translation);
					return transform;
				}
			}
		}


		class MoveAnchorArrow : Edge, IEditorPrimitive
		{
			public int axis;
			public float radius;
			public MoveAnchorArrow(Vector3 v1, Vector3 v2) : base(v1,v2) {}

			public Vector3 Color(bool highlight)
			{
				float c = 0.6f;
				if (highlight)
					c += 0.4f;
				return AxisVector[axis] * c;
			}

			public void Draw (bool highlight)
			{
				Vector3 color = Color(highlight);
				float hr = radius * 2;
				float hlen = V1.DistanceTo(V2) * 0.1f;

				Gl.glColor3f(color.x, color.y, color.z);
				GLUtil.Render3DArrow(V1, V2, radius, hr, hlen, 6);
			}

			public void MovePrimitives(IEnumerable<IEditorPrimitive> primitives, Vector2 move2d, Camera camera, TransformToolMode mode)
			{
				Vector2 v1proj = camera.ProjectPoint(V1);
				Vector2 v2proj = camera.ProjectPoint(V1 + AxisVector[axis]);

				Vector2 dir = (v2proj - v1proj);

				float l = dir.Normalize();
				float amount = (dir.Dot(move2d)) / l;
				
				Vector3 translation = new Vector3();
				Vector3 scale = new Vector3(1,1,1);
				Vector3 euler = new Vector3();

				if (mode == TransformToolMode.Move)
					translation = amount * AxisVector[axis];
				else if (mode == TransformToolMode.Rotate)
					euler = AxisVector[axis] * (amount * 0.1f);
				else if (mode == TransformToolMode.Scale)
					scale += AxisVector[axis] * amount * 0.1f;

				Transformer transformer = new Transformer()
					{ euler =euler, translation = translation, scale = scale };

				// Get vertices
				foreach (ISceneEditor se in Program.SceneManager.SceneEditors)
					se.TransformSelection (primitives, transformer);
			}

			// Favor move anchors over regular edges
			public new float GetSelectFactor(Vector3 rayStart, Vector3 rayDir, Vector3 pt)
			{
				return base.GetSelectFactor(rayStart, rayDir, pt) * 0.1f-0.1f;
			}

			public void UpdateSize(Vector3 camPos)
			{
				float dist = (V1 - camPos).Length;
				this[2].position = V1 + AxisVector[axis] * dist * 0.05f;
				radius = dist * 0.005f;
			}
		}


		bool IsHighlighted (IEditorPrimitive o) { return Highlighted==o; }
		

		#region initialization
		public RenderCtl()
		{
			InitializeComponent();

			glControl.InitializeContexts();
		}		
		
		private void RenderCtl_Load(object sender, EventArgs e)
		{
			if (DesignMode)
				return;
	
			Ilut.ilutInit();
			Ilut.ilutRenderer(Ilut.ILUT_OPENGL);

			camera = new Camera();
			camera.FOV = 30.0f;
			camera.Yaw = -30.0f * (float)(Math.PI / 180.0);
			camera.Pitch = 25.0f * (float)(Math.PI / 180.0);

			UpdateViewport();

			glControl.MouseWheel += glControl_MouseWheel;
		}

		#endregion

		#region Rendering

		void UpdateViewport()
		{
			if (camera == null)
				return;

			Gl.glViewport(0,0,glControl.Width, glControl.Height);
			camera.Width = glControl.Width;
			camera.Height = glControl.Height;
		}


		private void RenderCtl_Resize(object sender, EventArgs e)
		{
			glControl.MakeCurrent();
			UpdateViewport();
		}

		private void glControl_Paint(object sender, PaintEventArgs e)
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
			if (!DesignMode) {
				Render();
				PrintInfo();
			}
		}

		private void Render()
		{
			if (DesignMode)
				return;

			camera.SetProjectionMatrix();
			camera.Position = -camera.Front * camDistance + camOffset;

			Gl.glEnable(Gl.GL_DEPTH_TEST);

			if (cullFace)
				Gl.glEnable(Gl.GL_CULL_FACE);
			else
				Gl.glDisable(Gl.GL_CULL_FACE);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadTransposeMatrixf(camera.Transform.m);

			foreach (ISceneEditor se in Program.SceneManager.SceneEditors)
				se.Render(this);

			if (moveAnchors != null) {
				foreach (MoveAnchorArrow ma in moveAnchors) {
					ma.UpdateSize(camera.Position);
					ma.Draw(Highlighted == ma || ma == movingAnchor);
				}
			}

//			GLUtil.RenderCenterPoint(renderPoint, 0.1f, new Vector3(1, 1, 1));
		//	GLUtil.RenderCenterPoint(new Vector3(), 1.0f);

			if (renderAxis)
				RenderAllAxis();

			if (renderGrid)
				RenderGrid();
		}

		private void RenderGrid()
		{
			
		}

		private void RenderAllAxis()
		{
			for (int a=0;a<3;a++) {
				Ray ray = new Ray(new Vector3(), AxisVector[a]);
				Vector3[] hits = camera.Frustum.RayIntersect(ray);

				if(hits.Length >= 2) {
					GLUtil.Color(AxisVector[a]);
					GLUtil.Line(hits[0], hits[1]);

					// Print the label on the far hit pos
					Vector3 wPos = (from h in hits orderby (h-camera.Position).SqLength select h).Last();
					Vector2 pos = camera.ProjectPoint(wPos);
					PrintClampedCentered(pos.x, camera.Height-pos.y, AxisLabel[a], 1.2f);
				}
			}

		//	Vector2 p = camera.ProjectPoint(new Vector3());
		//	PrintClampedCentered(p.x,camera.Height - p.y, "o", 0.8f);

		//	PrintClampedCentered(lastMousePos.X, camera.Height - lastMousePos.Y, "mp", 0.8f);
		}

		private void PrintClampedCentered(float x,float y,string txt,float scale)
		{
			float w = GLFont.StdFont.GetTextWidth(txt) * scale * 0.5f;
			if (x < w) x = w;
			if (x > glControl.Width - w) x = glControl.Width - w;

			float h = GLFont.StdFont.GetTextHeight() * scale * 0.5f;
			if (y < h) y = h;
			if (y > glControl.Height - h) y = glControl.Height - h;

			GLFont.StdFont.Print(x - w, y, txt, scale);
		}


		private void PrintInfo()
		{
			Gl.glColor3f(1, 1, 1);

			string str1 = String.Format("Position: {0:0.0}, {1:0.0}, {2:0.0}", camera.Position.x, camera.Position.y, camera.Position.z);
			string str2 = String.Format("Angles: Yaw={0:0.0}, Pitch={1:0.0}, dbgdist:{2:0.00}", camera.Yaw * (180.0/Math.PI), camera.Pitch * (180.0/Math.PI),dbgDist);
			GLFont.StdFont.Print(0.0f, 20.0f, str1);
			GLFont.StdFont.Print(0.0f, 0.0f, str2);

			string[] debugInfo = ce.engine.DebugInfo.GetDebugInfo();
			for(int i=0;i<debugInfo.Length;i++)
				GLFont.StdFont.Print(0.0f, 40.0f + i*20, debugInfo[i]);
		}

		#endregion

		MoveAnchorArrow[] GetMoveAnchors(Vector3 pos)
		{
			float s=0.1f, e=0.6f;

			MoveAnchorArrow[] r = new MoveAnchorArrow[3];
			for (int a = 0; a < 3; a++)
				r[a] = new MoveAnchorArrow(pos + s * AxisVector[a], pos + e * AxisVector[a]) { axis = a };
			return r;
		}

		void SelectItem (Vector3 start, Vector3 dir, bool multiple)
		{
			IEditorPrimitive[] prims;
			IEnumerable<IEditorPrimitive> primitiveSet = Program.AllPrimitives;

			if (multiple)
				prims = Array.ConvertAll(RayPrimitivesIntersect(new Ray(start, dir), camera.Position, primitiveSet), i => i.primitive);
			else
				prims = new IEditorPrimitive[] { GetPrimitiveOnRay(new Ray(start, dir), camera.Position, primitiveSet).primitive };

			foreach (IEditorPrimitive o in prims)
			{
				if (o != null) {
					if (selection.Contains(o))
						selection.Remove(o);
					else {
						if ((ModifierKeys & Keys.Shift) == 0)
							selection.Clear();
						selection.Add(o);
					}
				} else {
					if ((ModifierKeys & Keys.Shift) == 0) 
						selection.Clear();
				}
			}

			UpdateMoveAnchors();
		}

		/// <summary>
		/// Tests all primitives against the given ray for intersections. 
		/// All intersections that come within range are returned
		/// </summary>
		public static Intersection[] RayPrimitivesIntersect(Ray ray, Vector3 camPos, IEnumerable<IEditorPrimitive> primitives)
		{
			List<Intersection> intersections = new List<Intersection>();

			foreach (var i in primitives) {
				Vector3 hitPos, hitNormal;
				if (i.RayIntersect(ray, camPos, 0.008f, out hitPos, out hitNormal)) {
					Intersection inters = new Intersection();

					inters.primitive = i;
					inters.point = hitPos;

					intersections.Add(inters);
				}
			}
			return intersections.ToArray();
		}

		public static Intersection GetPrimitiveOnRay(Ray ray, Vector3 relViewPos, IEnumerable<IEditorPrimitive> primitives)
		{
			var items = RayPrimitivesIntersect(ray, relViewPos, primitives);
				
			Intersection[] isect = new Intersection[(int)PrimitiveType.MaxTypes];

			foreach (var i in items) {
				//float dist = i.point.DistanceTo(camera.Position);
				float dist = ray.DistanceToPoint(i.point);

				float f = i.primitive.GetSelectFactor(ray.start, ray.dir, i.point);

				int sp = (int)i.primitive.PrimitiveType;
				if (isect [sp].primitive == null || f < i.primitive.GetSelectFactor (ray.start, ray.dir, isect[sp].point))
					isect[sp] = i;
			}

			return isect.FirstOrDefault(i => i.primitive != null);
		}


		private void UpdateMoveAnchors()
		{
			if (selection.Count > 0)
			{
				Vector3 sum = new Vector3();
				foreach (var prim in selection)
					sum += prim.CalcMidPos();
				moveAnchors = GetMoveAnchors(sum / (float)selection.Count);
			}
			else
				moveAnchors = null;
		}

		#region Camera/mouse handling

		private void glControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (clickMousePos.X == e.Location.X && clickMousePos.Y==e.Location.Y) {
				if (e.Button == MouseButtons.Left ) {

					Vector3 dir = camera.ScreenPosToRay(e.Location.X, e.Location.Y);
					SelectItem(camera.Position, dir, (ModifierKeys & Keys.Control) != 0);

					glControl.Refresh();
				}
				if (e.Button == MouseButtons.Right)
					ShowContextMenu(e.Location);
			}
			movingAnchor = null;
		}


		private void openGLControl_MouseDown(object sender, MouseEventArgs e)
		{
			lastMousePos = e.Location;
			clickMousePos = e.Location;

			if (Highlighted is MoveAnchorArrow && e.Button == MouseButtons.Left)
				movingAnchor = (MoveAnchorArrow)Highlighted;
		}

		void glControl_MouseWheel(object sender, MouseEventArgs e)
		{
			camDistance *= (float)Math.Pow(1.05, e.Delta/120.0);
			Refresh();
		}

		private void openGLControl_MouseMove(object sender, MouseEventArgs e)
		{
			Vector3 rayDir = camera.ScreenPosToRay(e.X, e.Y);

			var isect = GetPrimitiveOnRay(new Ray (camera.Position, rayDir), camera.Position, GetRenderPrimitives());
			Highlighted = isect.primitive;
			if (Highlighted != null) {
				renderPoint = isect.point;
				dbgDist = renderPoint.DistanceToRay(camera.Position, rayDir);
			}

			//selection = new HashSet<IEditorPrimitive>(Array.ConvertAll(Scene.RayPrimitivesIntersect(camera.Position, rayDir, camera.Position, GetScene().Primitives).ToArray(), i=>{ renderPoint=i.point; return  i.obj;}));
			glControl.Refresh();
			
			if (e.Button == MouseButtons.None)
				return;

			Point move = (Point)((Size)e.Location - (Size)lastMousePos);

			if (movingAnchor != null) {
				movingAnchor.MovePrimitives(selection.Concat(moveAnchors), new Vector2( move.X, move.Y), camera, activeMode);
			} else {
				const float angSpeed = 0.005f;

				if ( (e.Button & MouseButtons.Left) != 0) {
					camera.Yaw -= move.X * angSpeed;
					camera.Pitch += move.Y * angSpeed;
				}
				else if ( (e.Button & MouseButtons.Middle) != 0) {
					float speed = 0.001f * camDistance;
					camOffset += camera.Right * (-move.X * speed );
					camOffset += camera.Up * (move.Y * speed );
				}
				else if ( (e.Button & MouseButtons.Right) != 0) {
					camDistance *= (float) Math.Pow(1.003, move.Y);
				}

			}
		

			lastMousePos = e.Location;
		}

		public void MoveTick(float dTime)
		{
			timeCounter += dTime;
			if (timeCounter > 0.25f) {
				lastFps = fps*4;
				fps = 0;
				timeCounter -= 0.25f;
			}

			Refresh();
		}

		#endregion

		private void ShowContextMenu (Point clickLocation)
		{
			List<MenuItem> items = new List<MenuItem>();

			foreach (ISceneEditor se in Program.SceneManager.SceneEditors) {
				MenuItem[] editorItems = se.GetContextMenuItems(selection,this);
				if (editorItems!=null)
					items.AddRange(editorItems);
			}

			MenuItem selectModeMenu = new MenuItem("Selection mode");
			for (int i=0;i<(int)PrimitiveType.MaxTypes ;i++) {
				PrimitiveType sm = (PrimitiveType)i;
				selectModeMenu.MenuItems.Add(new MenuItem(sm.ToString(), delegate {
					SelectionType = sm;
				}) { Checked = sm == SelectionType});
			}
			items.Add(selectModeMenu);

			MenuItem toolMenu = new MenuItem("Tool");
			AddToolMenuItem(TransformToolMode.Move, toolMenu);
			AddToolMenuItem(TransformToolMode.Rotate, toolMenu);
			AddToolMenuItem(TransformToolMode.Scale, toolMenu);
			items.Add(toolMenu);

			items.Add(new MenuItem("Rendering", new MenuItem[] {
				new MenuItem("Backface culling", delegate {					
					cullFace = !cullFace;
					glControl.Refresh();
				} ) {Checked=cullFace}
			}));

			new ContextMenu(items.ToArray()).Show(glControl, clickLocation);
		}

		private void AddToolMenuItem(TransformToolMode transformMode, MenuItem toolMenu)
		{
			toolMenu.MenuItems.Add(new MenuItem(transformMode.ToString(), delegate 	{
				TransformMode = transformMode;
			}) { Checked = activeMode == transformMode });
		}

		#region ISceneEditorHost Members

		bool ISceneEditorRenderHost.IsHighlighted(IEditorPrimitive p)
		{
			return p == Highlighted;
		}

		public bool IsSelected(IEditorPrimitive p)
		{
			return selection.Contains(p);
		}

		public Camera Camera
		{
			get { return camera; }
		}

		public void RefreshView( ) { glControl.Refresh(); }

		#endregion
	}
}
