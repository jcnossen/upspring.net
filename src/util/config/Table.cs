using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace ce.util.config
{
	public class Table : Dictionary<string, object>
	{
		int uniqueKey;
		Dictionary<object, string> comments; // mapped to each object that comes after it, or null if its after the last object
		public Dictionary<object, string> Comments {
			get { 
				if (comments == null)
					comments = new Dictionary<object,string>();
				return comments; 
			}
			set {
				comments = value;
			}
		}
		
		public string Add(object o, string name)
		{
			if (name == null) {
				while (ContainsKey(uniqueKey.ToString()))
				    uniqueKey++;
			
				name = uniqueKey.ToString();
			}
			Add(name, o);
			return name;
		}

		public void Write(TextWriter tw, int ident)
		{
			foreach (var kv in this) {
				
			}
		}

		public object Select(string path)
		{
			return Select(path, false);
		}

		public object Select(string path, bool throwOnError)
		{
			string[] dirs = path.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries);
			if (!ContainsKey(dirs[0])) {
				if (throwOnError)
					throw new Exception("Config value not found: " + path);
				return null;
			}
			object item = this[dirs[0]];
			if (dirs.Length == 1)
				return item;
			return ((Table)item).Select(string.Join("/", dirs, 1, dirs.Length-1), throwOnError);
		}

		public Table SelectTable(string path, bool throwOnError)
		{
			return (Table)Select(path, throwOnError);
		}
		public Table SelectTable(string path)
		{
			return (Table)Select(path, true);
		}
		public string String(string path)
		{
			return Select(path).ToString();
		}
		public float Float(string path)
		{
			return Convert.ToSingle(Select(path));
		}
		public int Int(string path)
		{
			return Convert.ToInt32(Select(path));
		}
		public SizeF SizeF(string path)
		{
			Table tbl = SelectTable(path);
			return new SizeF(tbl.Float("0"), tbl.Float("1"));
		}
		public Size Size(string path)
		{
			Table tbl = SelectTable(path);
			return new Size(tbl.Int("0"), tbl.Int("1"));
		}
		public PointF PointF(string path)
		{
			return SizeF(path).ToPointF();
		}
		public Point Point(string path)
		{
			Table tbl = SelectTable(path);
			return new Point(tbl.Int("0"),tbl.Int("1"));
		}
	}
}
