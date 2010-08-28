
using System;
using System.Reflection;


namespace Maru {

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class PostAttribute : HttpMethodAttribute {

		public PostAttribute ()
		{
		}

		public PostAttribute (params string [] patterns) : base (patterns)
		{
			Methods = new string [] { "POST" };
		}
	}
}


