
using System;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace ce.util.config
{	
	/// <summary>
	/// Parses a lua file that contains only nested tables without code
	/// </summary>
	public class LuaTableParser : ConfigParser
	{
		class Token
		{
			public Token(TokenType type, object val) {
				this.value = val;
				this.type = type;				
			}
			public object value;
			public TokenType type;
		}
		
		enum TokenType {
			EOF,
			OpenParen, CloseParen,
			Equals,
			Float,
			Int,
			String,
			Ident,
			Comma,
			Comment
		}

		private LuaTableParser(string src) : base(src)
		{}
		
		public static Table ParseTable(TextReader reader)
		{
			LuaTableParser parser = new LuaTableParser(reader.ReadToEnd());
			try {
				return parser.Parse();
			} catch (InternalParseException e) {
				throw new ParseException(parser.line, e);
			}
		}
		
		Table Parse()
		{
			Table table = new Table();
			string name = null;
			Dictionary<object, string> comments = new Dictionary<object,string>();

			while (true) {
				Token token = ReadToken();

				if (token.type == TokenType.Comment) {
					comments[new object()] = (string)token.value;
					continue;
				}

				if (token.type == TokenType.CloseParen || 
				    token.type == TokenType.Ident || 
				    token.type == TokenType.Comma)
				{
					if (name != null)
						throw new InternalParseException(String.Format("Expecting value after '{0} ='", name));
				}
				
				if (token.type == TokenType.CloseParen || token.type == TokenType.EOF)
					break;
				
				if (token.type == TokenType.Ident) {
					name = (string)token.value;
					if (ReadToken().type != TokenType.Equals)
						throw new InternalParseException("Expecting '=' after '" + name + "'");
					continue;
				} 
				if (token.type == TokenType.Equals)
					throw new InternalParseException("Unexpected = token");
				
				object value = null;
				switch (token.type) {
					case TokenType.OpenParen:
						value = Parse();
						break;
					case TokenType.Int:
					case TokenType.Float:
					case TokenType.String:
						value = token.value;
						break;
				}
				if (value != null) {
					name = table.Add(value, name);
					if (comments.Count>0) {
						string comment = "";
						foreach (var kv in comments)
							 comment += kv.Value + "\n";
						table.Comments[table[name]] = comment;
						comments.Clear();
					}
					name = null;
				}
			}
			return table;
		}
		
		Token ReadToken()
		{
			char c;
			while (true) {
				while ( (c = Peek()) != '\0' ) {			
					if (!Char.IsWhiteSpace(c))
					    break;
					Read();
				}
				if (End())
					return new Token(TokenType.EOF, null);
				c = Read();

				if (c == '"') {
					StringBuilder sb = new StringBuilder();
					while (!End()) {
						c = Read();
						if (c == '"') break;
						else sb.Append(c);
					}
					if (c != '"')
						throw new InternalParseException("LuaTableParser: Unexpected end of string");
					Read(); // skip "
					return new Token(TokenType.String, sb.ToString());
				} else if (c == '{')
					return new Token(TokenType.OpenParen, null);
				else if (c == '}')
					return new Token(TokenType.CloseParen, null);
				else if (c == ',')
					return new Token(TokenType.Comma, null);
				else if (c == '=') 
					return new Token(TokenType.Equals, null);
				else if (c == '-' && Peek() == '-') {
					Read();
					string comment = "";
					while (!End()) {
						c = Read();
						if (c=='\n')
							break;
						comment+=c;
					}
					return new Token(TokenType.Comment, comment);
				}
				else if (Char.IsDigit(c) || c == '-' || c == '.') {
					string str = "";
					str += c;
					while (!End()) {
						c = Peek();
						if (!Char.IsDigit(c) && c!='.')
							break;
						str += c;
						Read();
					}
					if (str.Contains("."))
						return new Token(TokenType.Float, Convert.ToSingle(str));
					else
						return new Token(TokenType.Int, Convert.ToInt32(str));
				} else if (Char.IsLetter(c) || c == '_') {
					string ident = "";
					ident += c;
					while (!End()) {
						c = Peek();
						if (!Char.IsLetterOrDigit(c) && c != '_')
							break;
						ident += c;
						Read();
					}
					return new Token(TokenType.Ident, ident);
				}
				else
					throw new InternalParseException("Unknown character: " + c);
			}
		}
	}
}
