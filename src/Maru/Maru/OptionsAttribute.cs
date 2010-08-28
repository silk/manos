
using System;
using System.Reflection;


namespace Maru {

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class OptionsAttribute : HttpMethodAttribute {

		public OptionsAttribute ()
		{
		}

		public OptionsAttribute (params string [] patterns) : base (patterns)
		{
			Methods = new string [] { "OPTIONS" };
		}
	}
}


