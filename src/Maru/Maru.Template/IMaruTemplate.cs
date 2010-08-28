

using System;

using Maru;
using Maru.Server;

namespace Maru.Templates {
	
	
	public interface IMaruTemplate
	{
		void Render (IMaruContext context, object the_arg);
		void RenderToResponse (IHttpResponse response, object the_arg);
	}
	
}

