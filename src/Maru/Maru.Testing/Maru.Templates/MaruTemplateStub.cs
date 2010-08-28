using System;

using Maru;
using Maru.Server;


namespace Maru.Templates.Testing
{
	public class MaruTemplateStub : IMaruTemplate
	{
		public MaruTemplateStub ()
		{
		}
	
		public object RenderedArgument {
			get;
			private set;
		}
		
		public void Render (IMaruContext context, object the_arg)
		{
			RenderToResponse (context.Response, the_arg);
		}

		public void RenderToResponse (IHttpResponse response, object the_arg)
		{
			RenderedArgument = the_arg;
		}
	}
}

