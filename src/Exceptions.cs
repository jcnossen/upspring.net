using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ce.engine
{
	public class ContentException : ApplicationException
	{
		public ContentException(string msg) : base(msg) { }
		public ContentException(string msg, Exception inner) : base(msg, inner) { }
	}

	public class ScriptException : ContentException
	{
		public ScriptException(string msg) : base(msg) { }
	}
}
