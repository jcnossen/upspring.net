using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Tao.DevIl;

namespace ce.engine
{
	public class Image : IDisposable
	{
		int id;

		Image(int id)
		{
			this.id = id;
		}

		~Image()
		{
			Dispose();
		}

		public Image(int w,int h, int channels, int format) {
			id = Il.ilGenImage();
			Il.ilBindImage(id);
			Il.ilTexImage(w, h, 1, 4, format, Il.IL_UNSIGNED_BYTE, IntPtr.Zero);
		}

		public Image(byte[] data)
		{
			id = Il.ilGenImage();
			Il.ilBindImage(id);
			if (!Il.ilLoadL(Il.IL_TYPE_UNKNOWN, data, data.Length))
			{
				int err = Il.ilGetError();
				string errMsg = Ilu.iluErrorString(err);
				Il.ilDeleteImage(id);
				throw new InvalidDataException("Error loading image: " + errMsg);
			}
		}

		public Image(string fn)
		{
			if (!File.Exists(fn))
				throw new FileNotFoundException(fn + " not found", fn);

			id = Il.ilGenImage();
			Il.ilBindImage(id);
			if (!Il.ilLoadImage(fn))
			{
				int err = Il.ilGetError();
				string errMsg = Ilu.iluErrorString(err);
				Il.ilDeleteImage(id);
				throw new Exception("Error loading image '" + fn + "'. Error: " + errMsg);
			}
		}

		#region IL Function wrappers
		public void Flip()
		{
			Il.ilBindImage(id);
			Ilu.iluFlipImage();
		}

		public void Mirror()
		{
			Il.ilBindImage(id);
			Ilu.iluMirror();
		}

		public Image Clone()
		{
			Il.ilBindImage(id);
			return new Image(Il.ilCloneCurImage());
		}

		public byte[] GetBytes()
		{
			Il.ilBindImage(id);

			int len = Il.ilGetInteger(Il.IL_IMAGE_SIZE_OF_DATA);
			byte[] data = new byte[len];

			IntPtr src = Il.ilGetData();
			Marshal.Copy(src, data, 0, len);

			return data;
		}

		public int Format
		{
			get {
				return Il.ilGetInteger(Il.IL_IMAGE_FORMAT);
			}
		}

		public int Type
		{
			get {
				return Il.ilGetInteger(Il.IL_IMAGE_TYPE);
			}
		}

		public int Width
		{
			get
			{
				Il.ilBindImage(id);
				return Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
			}
		}

		public int Height
		{
			get
			{
				Il.ilBindImage(id);
				return Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
			}
		}

		public int BitsPerPixel
		{
			get
			{
				Il.ilBindImage(id);
				return Il.ilGetInteger(Il.IL_IMAGE_BITS_PER_PIXEL);
			}
		}

		public int BytesPerPixel
		{
			get
			{
				Il.ilBindImage(id);
				return Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);
			}
		}

		public int DataLength
		{
			get 
			{
				Il.ilBindImage(id);
				return Il.ilGetInteger(Il.IL_IMAGE_SIZE_OF_DATA);
			}
		}

		public int Channels
		{
			get
			{
				Il.ilBindImage(id);
				return Il.ilGetInteger(Il.IL_IMAGE_CHANNELS);
			}
		}

		public bool BlurGaussian(int iter)
		{
			Il.ilBindImage(id);
			return Ilu.iluBlurGaussian(iter);
		}
		public bool BlurAverage(int iter)
		{
			Il.ilBindImage(id);
			return Ilu.iluBlurAvg(iter);
		}

		public bool Contrast(float contrast)
		{
			Il.ilBindImage(id);
			return Ilu.iluContrast(contrast);
		}

		public bool Scale(int width, int height)
		{
			Il.ilBindImage(id);
			return Ilu.iluScale(width,height,1);
		}

		public bool Rotate(float ang)
		{
			Il.ilBindImage(id);
			return Ilu.iluRotate(ang);
		}
		public bool ScaleColors(float r, float g, float b)
		{
			Il.ilBindImage(id);
			return Ilu.iluScaleColours(r, g, b);
		}
		public bool Negative()
		{
			Il.ilBindImage(id);
			return Ilu.iluNegative();
		}
		#endregion

		public int ImageID
		{
			get { return id; }
			set
			{
				if (id != 0) Il.ilDeleteImage(id);
				id = value;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (id != 0)
			{
				Il.ilDeleteImage(id);
				id = 0;
			}
		}

		#endregion

		public Bitmap ToBitmap()
		{
			int h = Height, w = Width;

			Il.ilBindImage(id);
			int type = Il.ilGetInteger(Il.IL_IMAGE_TYPE);
			int srcfmt = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);

			// TODO: Take large pixel formats (>32 bit) into account
			int srcimg = id;
			PixelFormat format = PixelFormat.Undefined;
			switch (srcfmt) {
				case Il.IL_RGB:
					srcimg = Il.ilCloneCurImage();
					Il.ilBindImage(srcimg);
					Il.ilConvertImage(Il.IL_BGR, Il.IL_UNSIGNED_BYTE);
					format = PixelFormat.Format24bppRgb; 
					break;
				case Il.IL_BGR:
					format = PixelFormat.Format24bppRgb;
					break;
				case Il.IL_LUMINANCE:
					srcimg = Il.ilCloneCurImage();
					Il.ilBindImage(srcimg);
					Il.ilConvertImage(Il.IL_RGB, Il.IL_UNSIGNED_BYTE);
					format = PixelFormat.Format24bppRgb;
					break;
				case Il.IL_COLOUR_INDEX:
					srcimg = Il.ilCloneCurImage();
					Il.ilBindImage(srcimg);
					Il.ilConvertImage(Il.IL_RGB, Il.IL_UNSIGNED_BYTE);
					format = PixelFormat.Format24bppRgb;
					break;
				case Il.IL_RGBA:
					srcimg = Il.ilCloneCurImage();
					Il.ilBindImage(srcimg);
					Il.ilConvertImage(Il.IL_BGRA, Il.IL_UNSIGNED_BYTE);
					format = PixelFormat.Format32bppArgb;
					break;
				case Il.IL_BGRA:
					throw new NotImplementedException();
				default:
					return null;
			}

			Bitmap bmp = new Bitmap(w, h, format);

			BitmapData d = bmp.LockBits(new Rectangle(new Point(), bmp.Size), ImageLockMode.WriteOnly, format);
			Il.ilBindImage(srcimg);

			unsafe {
				byte* dst = (byte*)d.Scan0.ToPointer();
				byte* src = (byte*)Il.ilGetData().ToPointer();
				int bpp = Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);
				
				for(int y=0;y<h;y++) {
					for(int x=0;x<w * bpp;x++) {
						dst[x] = src[x];
					}
					dst += d.Stride;
					src += bpp * w;
				}
			}
			bmp.UnlockBits(d);

			if (srcimg != id) 
				Il.ilDeleteImage(srcimg);

			return bmp;
		}
	}
}
