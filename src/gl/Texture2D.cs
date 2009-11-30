using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;
using Tao.DevIl;

namespace ce.engine.gl
{
	public class Texture2D : Texture, IDisposable
	{
		int id;
		int target;

		int width, height;
		bool hasMipmaps;

		public int Width { get { return width; } }
		public int Height { get { return height; } }

		public static bool useLinearFiltering = true;

		public Texture2D()
		{}

		public override bool Mipmapped
		{
			get { return hasMipmaps; }
		}

		public Texture2D(Image img) : this (img, Gl.GL_TEXTURE_2D)
		{}

		public Texture2D(Image img, int target)
		{
			this.target = target;
			SetImage(img);
		}

		public override int TextureID
		{
			get { return id; }
		}

		public void SetImage(Image image)
		{
			width = image.Width;
			height = image.Height;

			Il.ilBindImage(image.ImageID);
			int cloned = Il.ilCloneCurImage();
			Il.ilBindImage(cloned);
			Ilu.iluFlipImage();
			id = Ilut.ilutGLBindMipmaps();
			Il.ilDeleteImage(cloned);
			hasMipmaps = true;
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, useLinearFiltering ? Gl.GL_LINEAR : Gl.GL_NEAREST);
			Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, useLinearFiltering ? Gl.GL_LINEAR_MIPMAP_LINEAR : Gl.GL_NEAREST_MIPMAP_NEAREST);

			if (id == 0)
				throw new Exception("Failed to generate GL texture");
		}

		public override void Bind()
		{
			Gl.glEnable(target);
			Gl.glBindTexture(target, id);
		}

		public override void Unbind()
		{
			Gl.glDisable(target);
			Gl.glBindTexture(target, 0);
		}

		public override int Target
		{
			get { return target; }
		}

		~Texture2D()
		{
			Dispose();
		}

		public override void GfxDispose()
		{
			if (id != 0)
			{
				Gl.glDeleteTextures(1, new int[] { id });
				id = 0;
			}
		}
	}
}
