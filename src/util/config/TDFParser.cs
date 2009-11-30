using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace ce.util.config
{	
	/// <summary>
	/// Parses Total Annihilation TDF config files
	/// </summary>
	public class TDFParser : ConfigParser
	{
		private TDFParser(string src) : base(src)
		{	}
		
		public static Table ParseTable(TextReader reader)
		{
			TDFParser parser = new TDFParser(reader.ReadToEnd());
			try {
				return parser.Parse();
			} catch (InternalParseException e) {
				throw new ParseException(parser.line, e);
			}
		}

		Table Parse()
		{
			Table table = new Table();

			while (true) {
				SkipWhitespace();
				if (End())
					break;

				char ch = Peek();
				if (ch == '[')
				{
					Read(); 
					string name = ReadIdent();
					Expect(']');
					SkipWhitespace();
					Expect('{');
					SkipWhitespace();
					table[name] = Parse();
					Expect('}');
				}
				else if (ch == '}')
					break;
				else {
					string name = ReadIdent();
					Expect('=');
					string val = "";
					while (!End() && Peek()!=';') 
						val += Read();
					Read();
					table[name] = val;
				}
			}
			return table;
		}

		string ReadIdent()
		{
			string ident = "";
			while (!End()) {
				char ch = Peek();
				if (ch != '-' && !Char.IsDigit(ch) && !Char.IsLetter(ch) && ch != '_')
					break;
				ident += Read();
			}
			return ident;
		}

		void SkipWhitespace()
		{
			while (!End() && Char.IsWhiteSpace(Peek())) {
				// Skip whitespace
				while ( !End() ) {
					char c = Peek();
					if (!Char.IsWhiteSpace(c))
							break;
					Read();
				}
				// Skip comments
				if (Peek(0) == '/' && Peek(1) == '/') {
					// read to end
					while (!End() && Read()!='\n');
				}
				if (Peek(0) == '/' && Peek(1) == '*') {
					while (!End()) {
						if (Peek(0) == '*' && Peek(1) == '/') {
							Read(2);
							break;
						}
						Read();
					}
				}
			}
		}	
	}
}
