using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ce.math
{
	/// <summary>
	/// Axis-Aligned Bounding Box
	/// </summary>
	[System.ComponentModel.TypeConverter(typeof(ce.util.StructTypeConverter<AABBox>))]
	public struct AABBox
	{
		public Vector3 min, max;

		public AABBox(Vector3 s, Vector3 e)
		{
			min = s;
			max = e;
		}

		public void Extend(Vector3 pos)
		{
			min.BBMin(pos);
			max.BBMax(pos);
		}

		public Vector3 Size
		{
			get { return max-min; }
		}
	}
}
