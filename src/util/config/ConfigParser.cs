using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ce.util.config
{
	public abstract class ConfigParser
	{
		string text;
		int pos;

		protected bool End() { return pos == text.Length; }
		protected char Read() {
			if (pos == text.Length)
				return '\0';
			if (text[pos] == '\n')
				line++;
			return text[pos++]; 
		}
		protected string Read(int c) {
			string x="";
			for(int i=0;i<c;i++)
				x+=Read();
			return x;
		}
		protected char Peek() { 
			if (pos == text.Length)
				return '\0';
			return text[pos]; 
		}
		protected char Peek(int ahead) { 
			if (pos+ahead>=text.Length)
				return '\0';
			return text[pos+ahead];
		}
		protected void Expect(char p)
		{
			char ch = Read();
			if (ch!=p)
				throw new InternalParseException(String.Format("Expecting '{0}' instead of '{1}'", p,ch));
		}

		protected int line = 1;

		protected ConfigParser(string src)
		{
			text = src;
		}

		protected class InternalParseException : Exception
		{
			public InternalParseException(string msg) : base (msg) {}
		}
		
		public class ParseException : Exception
		{
			public ParseException(int line, Exception inner) 
				: base (inner.Message, inner) 
			{
				this.line = line;
			}
			int line;
			public int Line { get { return line; } }
		}


	}
}
