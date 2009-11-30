using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ce.math
{
	[System.ComponentModel.TypeConverter(typeof(ce.util.StructTypeConverter<Int2>))]
	public struct Int2 : IEquatable<Int2>
	{	
		[XmlAttribute] public int x;
		[XmlAttribute] public int y;
		
		[XmlIgnore] public int X { get { return x; } set { x=value; }}
		[XmlIgnore] public int Y { get { return y; } set { y=value; }}

		public Int2(int X,int Y)
		{ x = X; y = Y; }

		public static implicit operator Vector2(Int2 i)
		{
			return new Vector2(i.x, i.y);
		}

		public static implicit operator Int2(Vector2 i)
		{
			return new Int2((int)i.x,(int) i.y);
		}

		public static bool operator ==(Int2 a, Int2 b)
		{
			return a.x == b.x && a.y == b.y;
		}
		public static bool operator !=(Int2 a, Int2 b)
		{
			return a.x != b.x || a.y != b.y;
		}

		#region Operators

		public static Int2 operator*(Int2 v, int f)
		{
			Int2 r;
			r.x = v.x * f;
			r.y = v.y * f;
			return r;
		}

		public static Int2 operator *(int f, Int2 v)
		{
			Int2 r;
			r.x = v.x * f;
			r.y = v.y * f;
			return r;
		}

		public static Int2 operator /(Int2 v, int f)
		{
			Int2 r;
			r.x = v.x / f;
			r.y = v.y / f;
			return r;
		}

		public static Int2 operator +(Int2 a, Int2 b)
		{
			Int2 r;
			r.x = a.x + b.x;
			r.y = a.y + b.y;
			return r;
		}

		public static Int2 operator -(Int2 a, Int2 b)
		{
			Int2 r;
			r.x = a.x - b.x;
			r.y = a.y - b.y;
			return r;
		}

		public static Int2 operator -(Int2 a)
		{
			return new Int2(-a.x, -a.y);
		}

		// Dot-product
		public static float operator*(Int2 a,Int2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		// Component-wise multiplication
		public void Multiply(Int2 v)
		{
			x *= v.x;
			y *= v.y;
		}

		#endregion

		public override string ToString()
		{
			return String.Format("X={0}, Y={1}", x, y);
		}

		public override bool Equals(object obj)
		{
			Int2 o = (Int2)obj;
			return o.x == x && o.y == y;
		}

		#region IEquatable<Int2> Members

		bool IEquatable<Int2>.Equals(Int2 other)
		{
			return other.x == x && other.y == y;
		}

		#endregion
	}
}
