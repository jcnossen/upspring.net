using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ce.util
{
	public static class Util
	{
		public static string ReadAsciiZString(BinaryReader r)
		{
			List<byte> data = new List<byte>();
			while (true) {
				byte b = r.ReadByte();
				if (b == 0)
					return Encoding.ASCII.GetString(data.ToArray());
				data.Add(b);
			}
		}

		public static bool IsPowerOfTwo(int p)
		{
			int i = 1;
			while (i < p)
				i *= 2;
			return i == p;
		}
	}
}
