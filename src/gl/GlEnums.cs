using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Tao.OpenGl;

namespace ce.engine.gl
{
	public enum PrimitiveType
	{
		Points = Gl.GL_POINTS,
		Lines = Gl.GL_LINES,
		LineLoop = Gl.GL_LINE_LOOP,
		LineStrip = Gl.GL_LINE_STRIP,
		Triangles = Gl.GL_TRIANGLES,
		TriangleFan = Gl.GL_TRIANGLE_FAN,
		TriangleStrip = Gl.GL_TRIANGLE_STRIP,
		Quads = Gl.GL_QUADS,
		QuadStrip = Gl.GL_QUAD_STRIP,
		Polygon = Gl.GL_POLYGON
	}
}
