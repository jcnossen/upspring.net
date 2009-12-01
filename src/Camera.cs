using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ce.math;

namespace UpspringSharp
{

	public class Camera
	{
		public enum CtlMode {
			Rotating, FPS
		} 
		
		public enum Mode {
			XZ, XY, YZ, PERSP
		}

		CtlMode ctlmode;

		// FPS state
		float yaw, pitch;
		Vector3 pos, strafe;
		float zoom;

		const float SpeedMod = 0.05f;
		const float AngleMod = 1.0f;

		Camera()
		{
			Reset();
			ctlmode = CtlMode.Rotating;
		}

		void Reset ()
		{
			yaw = pitch = 0.0f;
			zoom = 1.0f;
			pos = new Vector3();
		}

		void MouseEvent(GLView view, int msg, Int2 move)
		{
			if (view.CameraMode == Mode.PERSP) {
				switch (ctlmode) {
					case CtlMode.Rotating: MouseEventRotatingMode (view,msg,move); break;
					case CtlMode.FPS: MouseEventFPSMode (view,msg,move); break;
				}
			} else
				MouseEvent2DView (view,msg,move);
		}

		void MouseEvent2DView (GLView view, int msg, Int2  move)
		{
/*			uint s = fltk::event_state();
			if((msg == fltk::DRAG || msg == fltk::MOVE) && ((s & fltk::BUTTON1) || (s & fltk::BUTTON2)))
			{
				pos += Vector3 (-move.x / zoom, move.y / zoom , 0) * 0.05f;
				view->redraw();
			}
			else if((msg == fltk::DRAG || msg == fltk::MOVE) && (s & fltk::BUTTON3))
			{
				zoom *= (1-move.x * 0.02-move.y*0.02);
				view->redraw();
			}*/
		}

		void MouseEventFPSMode (GLView view, int msg, Int2 move)
		{
/*			uint s = fltk::event_state();
				if((msg == fltk::DRAG || msg == fltk::MOVE) && (s & fltk::BUTTON1))
			{
				Vector3 f;
				yaw += (M_PI * move.x * AngleMod / 360);
				Math::ComputeOrientation (yaw, 0.0f, 0.0f,0,0, &f);
				pos -= f * move.y * SpeedMod * 2;
				view->redraw();
			}
			else if((msg == fltk::DRAG || msg == fltk::MOVE) && (s & fltk::BUTTON3))
			{
				yaw += (M_PI * (move.x * AngleMod) / 180); 
				pitch -= (M_PI * (move.y * AngleMod) / 360);
				view->redraw();
			}
			else if((msg == fltk::DRAG || msg == fltk::MOVE) && ((s & fltk::BUTTON2) || ((s & fltk::BUTTON1) && (s & fltk::BUTTON3))))
			{
				Vector3 right;
				Math::ComputeOrientation (yaw, 0.0f, 0.0f,&right,0,0);
				pos.y -= move.y * 0.5f* SpeedMod;
				pos += right * (move.x * 0.5f * SpeedMod);
				view->redraw();
			}*/
		}

		void MouseEventRotatingMode (GLView view, int msg, Int2 move)
		{/*
			uint s = fltk::event_state(); // event_button() only works for PUSH/RELEASE
			if (msg==fltk::DRAG && (s&fltk::BUTTON1))
			{
				yaw += M_PI * move.x * AngleMod / 360; // AngleMod deg per x
				pitch -= M_PI * move.y * AngleMod / 360;
				view->redraw();
			}
			else if(msg==fltk::DRAG && (s&fltk::BUTTON3))
			{
				zoom *= 1-move.x * 0.02-move.y*0.02;
				view->redraw();
			}
			else if(msg==fltk::DRAG && (s&fltk::BUTTON2))
			{
				// x & y are used to displace the camera sideways
				Vector3 r,u;
				Math::ComputeOrientation (yaw, pitch, 0, &r, &u, 0);

				float Xmove = -move.x * 0.5f * SpeedMod * zoom;
				float Ymove = move.y * 0.5f * SpeedMod * zoom;
				strafe += r * Xmove + u * Ymove;
				view->redraw();
			}*/
		}

		Matrix4x4 GetMatrix()
		{
/*			if (ctlmode == CtlMode.FPS)  {
				Vector3 r,u,f;
				Math::ComputeOrientation (yaw, pitch, 0, &r, &u, &f);
				vm.camera (&pos, &r, &u, &f);
			}
			else if(ctlmode == Rotating) {
						Matrix tmp;
				vm.translation(-strafe);//initial translation of the model, pos is the center of rotation
				tmp.yrotate(yaw);
				vm *= tmp;
				tmp.xrotate(pitch);
				vm *= tmp;
				vm.addtranslation(Vector3(0.0f, 0.0f, 10*zoom));
			}*/

			return new Matrix4x4();
		}

		Vector3 GetOrigin ()
		{
			if (ctlmode == CtlMode.FPS) 
				return pos;
			else {
				Matrix4x4 mat = GetMatrix ();
				Matrix4x4 inv = mat.Inverse();
				if (inv != null)
				{
					Vector3 origin =  inv.Transform(new Vector3()); 
					return origin * 0.15f; //seems to work ok for 2d lighting
				}
				return new Vector3();
			}
		}
	}


}
