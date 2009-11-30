using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ce.util.config;


namespace ce.math
{
	/// <summary>
	/// Exposes Vector parsing functionality to the config Table class, using extension methods
	/// </summary>
	public static class MathTableParsing {

		public static Vector3 ParseVector3(this Table table)
		{
			float x = Convert.ToSingle(table["0"]);
			float y = Convert.ToSingle(table["1"]);
			float z = Convert.ToSingle(table["2"]);
			return new Vector3(x,y,z);
		}

		public static Vector3 Vector3(this Table table, string path)
		{
			return ((Table)table.Select(path)).ParseVector3();
		}


		public static Vector2 ParseVector2(this Table table)
		{
			float x = Convert.ToSingle(table["0"]);
			float y = Convert.ToSingle(table["1"]);
			return new Vector2(x,y);
		}

		public static Vector2 Vector2(this Table table, string path)
		{
			return ((Table)table.Select(path)).ParseVector2();
		}


		public static Vector4 ParseVector4(this Table table)
		{
			float x = Convert.ToSingle(table["0"]);
			float y = Convert.ToSingle(table["1"]);
			float z = Convert.ToSingle(table["2"]);
			float w = Convert.ToSingle(table["3"]);
			return new Vector4(x,y,z,w);
		}

		public static Vector4 Vector4(this Table table, string path)
		{
			return ((Table)table.Select(path)).ParseVector4();
		}

	}

}
