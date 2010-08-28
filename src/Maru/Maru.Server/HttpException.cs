
using System;

namespace Maru.Server
{
	public class HttpException : Exception
	{
		public HttpException (string message) : base (message)
		{
		}
	}
}
