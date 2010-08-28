
using System;

using Maru.Server;

namespace Maru
{
	public interface IMaruContext
	{
		IHttpTransaction Transaction {
			get;
		}

		IHttpRequest Request {
			get;
		}

		IHttpResponse Response {
			get;
		}
	}
}
