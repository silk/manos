
using System;
using System.Reflection;


namespace Maru {

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public abstract class IgnoreAttribute : Attribute {


	}

}
