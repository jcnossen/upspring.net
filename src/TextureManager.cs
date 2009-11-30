using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ce.util.vfs;
using ce.engine.gl;
using Tao.OpenGl;

namespace ce.engine
{
	/// <summary>
	/// TextureManager, allows textures to be shared between objects
	/// </summary>
	public class TextureManager
	{
		Dictionary<string, Texture> textures = new Dictionary<string, Texture>();

		public FileSystem FileSystem;

		public TextureManager(FileSystem fileSystem)
		{
			this.FileSystem = fileSystem;
		}

		public Texture2D LoadTexture2D(string name)
		{
			Texture tex;
			if (textures.TryGetValue(name, out tex))
				return tex as Texture2D;

			try {
				Image img = new Image(FileSystem.ReadAllBytes(name));
				Texture2D tex2d = new Texture2D(img);

				textures[name] = tex2d;
				return tex2d;
			}
			catch (FileNotFoundException e) {
				throw new ContentException("Can't load texture " + name, e);
			}
		}

		public void SetLinearFiltering(bool bilinear)
		{
			// Set filtering for all future textures
			Texture2D.useLinearFiltering = bilinear;

			// Update current textures
			foreach (Texture tex in textures.Values){
				tex.Bind();
				Gl.glTexParameteri(tex.Target, Gl.GL_TEXTURE_MAG_FILTER, bilinear ? Gl.GL_LINEAR : Gl.GL_NEAREST);

				if (tex.Mipmapped)
					Gl.glTexParameteri(tex.Target, Gl.GL_TEXTURE_MIN_FILTER, bilinear ? Gl.GL_LINEAR_MIPMAP_LINEAR : Gl.GL_NEAREST_MIPMAP_NEAREST);
				else
					Gl.glTexParameteri(tex.Target, Gl.GL_TEXTURE_MIN_FILTER, bilinear ? Gl.GL_LINEAR : Gl.GL_NEAREST);
				tex.Unbind();
			}
		}
	}
}
