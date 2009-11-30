/*
 * OpenGL font renderer
 * 
 * Characters are loaded by rendering them to a bitmap, using System.Drawing.Font
 * They are then copied to a GL texture and rendered as a quad
 * 
 * Copyright Jelmer Cnossen
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Tao.OpenGl;
using System.Diagnostics;

namespace ce.engine.gl
{
	public class Font : GfxDisposable
	{
		System.Drawing.Font font;
		int height;
		string name;

		class Glyph
		{
			public int displayList;
			public int texture;
			public Size size;
		};
		SortedDictionary<char, Glyph> charToGlyph = new SortedDictionary<char, Glyph>();

		public System.Drawing.Font GetSystemFont() { return font; }

		static Dictionary<string, Font> fonts = new Dictionary<string, Font>();

		static string FontID(string name, float size, FontStyle style)
		{
			return string.Format("{0}-{1}-{2}", name, size, style);
		}

		public static Font LoadFont (string name, float size, FontStyle style)
		{
			Font font;

			string id = FontID(name, size, style);

			if (fonts.TryGetValue(id, out font))
				return font;

			return new Font(name, size, style);
		}
		
		Font(string name, float size, FontStyle style)
		{
			font = new System.Drawing.Font(name, size, style);
			int avgWidth = 0;
			int numChars = 0;

			char[] chars = new char[180];
			for(int i=0;i<chars.Length;i++)
				chars[i] = (char)(i+20);

			this.name = name;
			fonts[FontID(name, size, style)] = this;

			foreach (char ch in chars)
			{
				Glyph glyph = new Glyph();
				charToGlyph[ch] = glyph;

				string chstr = new string(ch, 1);

				if (Char.IsControl(ch))
					continue;

				Bitmap tmp = new Bitmap(100, 100, PixelFormat.Format32bppArgb);
				Graphics gfx = Graphics.FromImage(tmp);

				RectangleF rect = MeasureDisplayString(gfx, chstr, font);
				glyph.size = new Size((int)rect.Width, (int)rect.Height);

				if (rect.Width < 1.0f || rect.Height < 1.0f)
					continue;

				gfx.Clear(Color.Black);
				gfx.DrawString(chstr, font, Brushes.White, -rect.Left, -rect.Top);

				BitmapData bm = tmp.LockBits(new Rectangle(0, 0, glyph.size.Width, glyph.size.Height),
								ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

				// Copy bitmap data to buffer
				byte[] buf = new byte[bm.Width * bm.Height * 4];
				for (int y = 0; y < bm.Height; y++)
				{
					long src = bm.Scan0.ToInt64();
					src += (long)(y * bm.Stride);

					System.Runtime.InteropServices.Marshal.Copy((IntPtr)src, buf, 4 * y * bm.Width, bm.Width * 4);
				}
				tmp.UnlockBits(bm);

				// Make power-of-two image
				Size texSize = new Size(NextPowerOfTwo(glyph.size.Width), NextPowerOfTwo(glyph.size.Height));
				byte[] texbuf = new byte[2 * (texSize.Width * texSize.Height)];

				for (int y = 0; y < bm.Height; y++)
					for (int x = 0; x < bm.Width; x++)
					{
						texbuf[(y * texSize.Width + x) * 2 + 0] = 255;// buf[(y * bm.Width + x) * 4 + 0];
						texbuf[(y * texSize.Width + x) * 2 + 1] = buf[(y * bm.Width + x) * 4 + 0];
					}
				int[] texid = new int[1];
				Gl.glGenTextures(1, texid);
				glyph.texture = texid[0];
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, glyph.texture);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
				Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);

				//Create the texture
				Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_LUMINANCE_ALPHA, texSize.Width, texSize.Height,
					0, Gl.GL_LUMINANCE_ALPHA, Gl.GL_UNSIGNED_BYTE, texbuf);

				// Create drawlist
				glyph.displayList = Gl.glGenLists(1);
				Gl.glNewList(glyph.displayList, Gl.GL_COMPILE);

				Gl.glBindTexture(Gl.GL_TEXTURE_2D, glyph.texture);
				float tcx = (float)bm.Width / (float)texSize.Width;
				float tcy = (float)bm.Height / (float)texSize.Height;

				Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0, tcy); Gl.glVertex2i(0, 0);
				Gl.glTexCoord2f(tcx, tcy); Gl.glVertex2i(bm.Width, 0);
				Gl.glTexCoord2f(tcx, 0); Gl.glVertex2i(bm.Width, bm.Height);
				Gl.glTexCoord2f(0, 0); Gl.glVertex2i(0, bm.Height);
				Gl.glEnd();
				// for next char
				Gl.glTranslatef(glyph.size.Width, 0.0f, 0.0f);

				Gl.glEndList();

				avgWidth += glyph.size.Width;
				if (height < glyph.size.Height)
					height = glyph.size.Height;
				numChars++;
			}

			// Insert a space char
			Glyph cg = new Glyph();
			cg.displayList = Gl.glGenLists(1);
			cg.size.Width = (int)(0.7f * (float)avgWidth / (float)numChars);
			Gl.glNewList(cg.displayList, Gl.GL_COMPILE);
			Gl.glTranslatef(cg.size.Width, 0.0f, 0.0f);
			Gl.glEndList();
			charToGlyph[' '] = cg;

			Gl.glDisable(Gl.GL_TEXTURE_2D);
		}

		public int GetTextWidth(string s)
		{
			int w = 0;
			foreach (char ch in s)
			{
				Glyph glyph = charToGlyph[ch];
				w += glyph.size.Width;
			}
			return w;
		}

		public int GetTextHeight()
		{
			return height;
		}

		public int GetCharTexture(char c)
		{
			return charToGlyph[c].texture;
		}

		internal int NextPowerOfTwo(int a)
		{
			int rval = 1;
			while (rval < a) rval <<= 1;
			return rval;
		}

		public void OutputChars(string text)
		{
			Gl.glPushAttrib(Gl.GL_LIST_BIT | Gl.GL_CURRENT_BIT | Gl.GL_ENABLE_BIT);
			Gl.glDisable(Gl.GL_LIGHTING);
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			Gl.glDisable(Gl.GL_DEPTH_TEST);
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

			for (int i = 0; i < text.Length; i++)
			{
				Glyph cg = charToGlyph[text[i]];
				Gl.glCallList(cg.displayList);
			}
			Gl.glPopAttrib();
		}

		public void Print(float x,float y, string what)
		{
			Print(x, y, what, 1.0f);
		}

		public void Print(float x, float y, string what, float scale)
		{
			//Prepare openGL for rendering the font characters
			GLUtil.PushOrthoMatrix();

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
	//		float[] modelview_matrix = new float[16];
//			Gl.glGetFloatv(Gl.GL_MODELVIEW_MATRIX, modelview_matrix);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			Gl.glTranslatef(x, y, 0);
			Gl.glScalef(scale, scale, 1.0f);
//			Gl.glMultMatrixf(modelview_matrix);

			//Render
			OutputChars(what);

			//Restore openGL state
			Gl.glPopMatrix();
			GLUtil.PopProjectionMatrix();
		}

		public void PrintCentered(float x, float y, string what, float scale)
		{
			float w = GetTextWidth(what) * scale;
			float h = GetTextHeight() * scale;

			Print(x - w / 2.0f, y - h / 2.0f, what, scale);
		}

		static RectangleF MeasureDisplayString(Graphics graphics, string text, System.Drawing.Font font)
		{
			StringFormat format = new StringFormat();
			RectangleF rect = new RectangleF(0, 0, 1000, 1000);
			CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
			Region[] regions = new Region[1];

			format.SetMeasurableCharacterRanges(ranges);

			regions = graphics.MeasureCharacterRanges(text, font, rect, format);
			return regions[0].GetBounds(graphics);
		}

		public override void GfxDispose()
		{
			foreach (Glyph g in charToGlyph.Values)
			{
				Gl.glDeleteLists(g.displayList, 1);
				if (g.texture != 0)
				{
					int[] texid = new int[] { g.texture };
					Gl.glDeleteTextures(1, texid);
				}
			}
			charToGlyph.Clear();

			if (this == stdFont)
				stdFont = null;

			if (fonts.ContainsKey (name))
				fonts.Remove (name);
		}

		static Font stdFont;
		public static Font StdFont
		{
			get { 
				if (stdFont == null) {
					char[] chars = new char[180];
					for (int a = 0; a < 180; a++)
						chars[a] = (char)(a + 20);
					stdFont = LoadFont("Courier", 14, FontStyle.Regular);
				}
				return stdFont;
			}
		}

	}
}
