using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace ce.engine.gl
{
	public enum TextureFormat
	{
		Alpha = Gl.GL_ALPHA,
		Alpha4 = Gl.GL_ALPHA4,
		Alpha8 = Gl.GL_ALPHA8,
		Alpha12 = Gl.GL_ALPHA12,
		Alpha16 = Gl.GL_ALPHA16,

		DepthComponent = Gl.GL_DEPTH_COMPONENT,
		DepthComponent16 = Gl.GL_DEPTH_COMPONENT16,
		DepthComponent24 = Gl.GL_DEPTH_COMPONENT24,
		DepthComponent32 = Gl.GL_DEPTH_COMPONENT32,

		Luminance = Gl.GL_LUMINANCE,
		Luminance4 = Gl.GL_LUMINANCE4,
		Luminance8 = Gl.GL_LUMINANCE8,
		Luminance12 = Gl.GL_LUMINANCE12,
		Luminance16 = Gl.GL_LUMINANCE16,

		LuminanceAlpha = Gl.GL_LUMINANCE_ALPHA,
		Intensity = Gl.GL_INTENSITY,

		RGB = Gl.GL_RGB,
		R3_G3_B2 = Gl.GL_R3_G3_B2,
		RGB4 = Gl.GL_RGB4, 
		RGB5 = Gl.GL_RGB5, 
		RGB8 = Gl.GL_RGB8, 
		RGB10 = Gl.GL_RGB10, 
		RGB12 = Gl.GL_RGB12, 
		RGB16 = Gl.GL_RGB16, 

		RGBA = Gl.GL_RGBA, 
		RGB5_A1 = Gl.GL_RGB5_A1,
		RGBA8 = Gl.GL_RGBA8,
		RGB10_A2 = Gl.GL_RGB10_A2,
		RGBA12 = Gl.GL_RGBA12,
		RGBA16 = Gl.GL_RGBA16,

		// Compressed formats
		CompressedAlpha = Gl.GL_COMPRESSED_ALPHA,
		CompressedIntensity = Gl.GL_COMPRESSED_INTENSITY,
		CompressedLuminance = Gl.GL_COMPRESSED_LUMINANCE,
		CompressedLuminanceAlpha = Gl.GL_COMPRESSED_LUMINANCE_ALPHA,
		CompressedRGB = Gl.GL_COMPRESSED_RGB,
		CompressedRGBA = Gl.GL_COMPRESSED_RGBA,

		CompressedRGBA_DXT1 = Gl.GL_COMPRESSED_RGBA_S3TC_DXT1_EXT,
		CompressedRGBA_DXT3 = Gl.GL_COMPRESSED_RGBA_S3TC_DXT3_EXT,
		CompressedRGBA_DXT5 = Gl.GL_COMPRESSED_RGBA_S3TC_DXT5_EXT,
		CompressedRGB_DXT1 = Gl.GL_COMPRESSED_RGB_S3TC_DXT1_EXT,

		// Float formats
		RGBA32F = Gl.GL_RGBA32F_ARB,
		Alpha32F = Gl.GL_ALPHA32F_ARB,
		Intensity32F = Gl.GL_INTENSITY32F_ARB,
		Luminance32F = Gl.GL_LUMINANCE32F_ARB,
		LuminanceAlpha32F = Gl.GL_LUMINANCE_ALPHA32F_ARB,
		RGBA16F = Gl.GL_RGBA16F_ARB,
		RGB16F = Gl.GL_RGB16F_ARB,
		Alpha16F = Gl.GL_ALPHA16F_ARB,
		Intensity16F = Gl.GL_INTENSITY16F_ARB,
		Luminance16F = Gl.GL_LUMINANCE16F_ARB,
		LuminanceAlpha16F = Gl.GL_LUMINANCE_ALPHA16F_ARB
	}


	public abstract class Texture : GfxDisposable
	{
		public abstract int Target { get; }

		public abstract int TextureID { get; }

		public abstract void Bind ();
		public abstract void Unbind ();

		public abstract bool Mipmapped { get; }
	}
}
