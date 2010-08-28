using System;

using Maru;
using Maru.Server;


namespace Maru.Templates
{
	public abstract class MaruTemplate
	{
		public void Render (IMaruContext context, object the_arg)
		{
			RenderToResponse (context.Response, the_arg);
		}
		
		public abstract void RenderToResponse (IHttpResponse response, object the_arg);
	}
}

