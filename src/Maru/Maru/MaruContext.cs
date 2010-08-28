

using System;

using Maru.Server;

namespace Maru {

	public class MaruContext : IMaruContext {

		public MaruContext (IHttpTransaction transaction)
		{
			Transaction = transaction;
		}

		public IHttpTransaction Transaction {
			get;
			private set;
		}

		public IHttpRequest Request {
			get { return Transaction.Request; }
		}

		public IHttpResponse Response {
			get { return Transaction.Response; }
		}
	}
}
