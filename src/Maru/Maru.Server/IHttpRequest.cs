

using System;
using System.Collections.Specialized;

namespace Maru.Server {

	public interface IHttpRequest {
		
		string Method {
			get;	
		}
		
		string LocalPath {
			get;
		}

		NameValueCollection UriData {
			get;	
		}
	}
}

