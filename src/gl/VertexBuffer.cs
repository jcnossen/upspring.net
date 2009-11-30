using System;
using System.Collections.Generic;
using System.Text;
using Tao.OpenGl;
using System.Diagnostics;

namespace ce.engine.gl
{
	public class VertexBuffer : GfxDisposable, IDisposable
	{
		protected int numElem;
		protected int elemSize;
		protected int id;
		protected BufferUsage usage;
		protected int type;

		public enum BufferUsage
		{
			StreamDraw = Gl.GL_STREAM_DRAW,
			StreamRead = Gl.GL_STREAM_READ,
			StreamCopy = Gl.GL_STREAM_COPY,
			StaticDraw = Gl.GL_STATIC_DRAW,
			StaticRead = Gl.GL_STATIC_READ,
			StaticCopy = Gl.GL_STATIC_COPY,
			DynamicDraw = Gl.GL_DYNAMIC_DRAW,
			DynamicRead = Gl.GL_DYNAMIC_READ,
			DynamicCopy = Gl.GL_DYNAMIC_COPY
		}

		public enum AccessType
		{
			WriteOnly = Gl.GL_WRITE_ONLY,
			ReadOnly = Gl.GL_READ_ONLY,
			ReadWrite = Gl.GL_READ_WRITE
		}

		#region properties

		/// <summary>
		/// Total size in bytes: equal to NumElements * ElemSize
		/// </summary>
		public int ByteSize
		{
			get { return numElem * elemSize; }
		}

		/// <summary>
		/// Number of elements (vertices or indices)
		/// </summary>
		public int NumElements
		{
			get { return numElem; }
		}

		/// <summary>
		/// Element size in bytes
		/// </summary>
		public int ElemSize 
		{
			get { return elemSize; }
		}

		public BufferUsage Usage
		{
			get { return usage; }
		}

		/// <summary>
		/// Returns the buffer type: GL_ELEMENT_ARRAY_BUFFER or GL_ARRAY_BUFFER
		/// </summary>
		public int GlType
		{
			get { return type; }
		}
		#endregion

		public VertexBuffer(int _numElements, int _elementSize, BufferUsage _usage)
		{
			type = Gl.GL_ARRAY_BUFFER;

			numElem = _numElements;
			elemSize = _elementSize;
			usage = _usage;

			int[] ids = new int[1];
			Gl.glGenBuffers (1, ids);
			id = ids[0];
		}

		~VertexBuffer()
		{
			Dispose();
		}

		public override void GfxDispose()
		{
			if (id!=0) {
				Gl.glDeleteBuffers(1, new int[1] { id } );
				id = 0;
			}
		}

		public void DiscardData()
		{
			Gl.glBindBuffer(type, id);
			Gl.glBufferData(type, (IntPtr)ByteSize, IntPtr.Zero, (int)usage);
		}

		public IntPtr LockData(AccessType accessType)
		{
			Debug.Assert (id != 0);

			Gl.glBindBuffer(type, id);
/*			unsafe {
				int byteSize = (int)ByteSize;
				Gl.glBufferData(type, new IntPtr((void*)&byteSize), IntPtr.Zero, (int)usage);
			}*/
			// Somehow (IntPtr)ByteSize works, maybe a Tao bug?
			Gl.glBufferData(type, (IntPtr)ByteSize, IntPtr.Zero, (int)usage);
			IntPtr ptr = Gl.glMapBuffer(type, (int)accessType);
			GLUtil.CheckError();

			return ptr;
		}

		public void UnlockData()
		{
			Debug.Assert (id != 0);
			Gl.glUnmapBuffer(type);
			Gl.glBindBuffer(type, 0);
			GLUtil.CheckError();
		}

		public void FillBuffer(IntPtr data)
		{
			Debug.Assert (id != 0);
			Gl.glBindBuffer(type,id);
			Gl.glBufferData(type, (IntPtr)ByteSize, data, (int)usage);
			Gl.glBindBuffer(type,0);

			GLUtil.CheckError();
		}

		public IntPtr Bind()
		{
			Debug.Assert (id != 0);
			Gl.glBindBufferARB(type, id);
			// returns a pointer, so system-memory arrays can be supported as fallback
			return IntPtr.Zero; 
		}

		public void Unbind()
		{
			Gl.glBindBufferARB(type, 0);
		}

		public void DrawAll(PrimitiveType type)
		{
			Gl.glDrawArrays((int)type, 0, (int) numElem);
		}
	}
	
	public class IndexBuffer : VertexBuffer
	{
		public IndexBuffer(int _numElements, int _elementSize, BufferUsage _usage) : base (_numElements, _elementSize, _usage)
		{
			type = Gl.GL_ELEMENT_ARRAY_BUFFER;
		}

		int IndexType 
		{
			get { return ElemSize == 2 ? Gl.GL_UNSIGNED_SHORT : Gl.GL_UNSIGNED_INT; }
		}

		public void DrawAllElements ()
		{
			DrawAllElements(PrimitiveType.Triangles);
		}

		public void DrawAllElements(PrimitiveType type)
		{
			Gl.glDrawElements((int)type, (int)NumElements, IndexType, Bind());
			Unbind();
		}

		public void DrawRange(int first, int count)
		{
			if (first < 0 || first + count > NumElements)
				throw new ArgumentOutOfRangeException();

			unsafe {
				byte* ptr = (byte*)Bind().ToPointer();
				ptr += first * ElemSize;
				Gl.glDrawElements(Gl.GL_TRIANGLES, count, IndexType, (IntPtr)ptr);
				Unbind();
			}
		}
	}
}
