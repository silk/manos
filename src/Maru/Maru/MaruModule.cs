

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using Maru.Server;
using Maru.Routing;
using Maru.Templates;

namespace Maru {

	public class MaruModule {

		private RouteHandler routes = new RouteHandler ();

		public MaruModule ()
		{
			StartInternal ();
		}

		public RouteHandler Routes {
			get { return routes; }
		}

		private RouteHandler AddRouteHandler (MaruModule module, string [] patterns, string [] methods)
		{
			if (module == null)
				throw new ArgumentNullException ("module");
			if (patterns == null)
				throw new ArgumentNullException ("patterns");
			if (methods == null)
				throw new ArgumentNullException ("methods");
			
			module.Routes.Patterns = patterns;
			module.Routes.Methods = methods;
			Routes.Children.Add (module.Routes);
			return module.Routes;
		}

		private RouteHandler AddRouteHandler (MaruAction action, string [] patterns, string [] methods)
		{
			// TODO: Need to decide if this is a good or bad idea
			// RemoveImplicitHandlers (action);

			if (action == null)
				throw new ArgumentNullException ("action");
			if (patterns == null)
				throw new ArgumentNullException ("patterns");
			if (methods == null)
				throw new ArgumentNullException ("methods");
			
			RouteHandler res = new RouteHandler (patterns, methods, new MaruTarget (action));
			Routes.Children.Add (res);
			return res;
		}

		private RouteHandler AddImplicitRouteHandler (MaruModule module, string [] patterns, string [] methods)
		{
			module.Routes.IsImplicit = true;
			module.Routes.Patterns = patterns;
			module.Routes.Methods = methods;
			Routes.Children.Add (module.Routes);
			return module.Routes;
		}

		private RouteHandler AddImplicitRouteHandler (MaruAction action, string [] patterns, string [] methods)
		{
			RouteHandler res = new RouteHandler (patterns, methods, new MaruTarget (action)) {
				IsImplicit = true,
			};

			Routes.Children.Add (res);
			return res;
		}

		public RouteHandler Route (string pattern, MaruModule module)
		{
			return AddRouteHandler (module, new string [] { pattern }, HttpMethods.RouteMethods);
		}

		public RouteHandler Route (string pattern, MaruAction action)
		{
			return AddRouteHandler (action, new string [] { pattern }, HttpMethods.RouteMethods);
		}

		public RouteHandler Route (MaruAction action, params string [] patterns)
		{
			return AddRouteHandler (action, patterns, HttpMethods.RouteMethods);
		}

		public RouteHandler Route (MaruModule module, params string [] patterns)
		{
			return AddRouteHandler (module, patterns, HttpMethods.RouteMethods);
		}

		public RouteHandler Get (string pattern, MaruModule module)
		{
			return AddRouteHandler (module, new string [] { pattern }, HttpMethods.GetMethods);
		}

		public RouteHandler Get (string pattern, MaruAction action)
		{
			return AddRouteHandler (action, new string [] { pattern }, HttpMethods.GetMethods);
		}

		public RouteHandler Get (MaruAction action, params string [] patterns)
		{
			return AddRouteHandler (action, patterns, HttpMethods.GetMethods);
		}

		public RouteHandler Get (MaruModule module, params string [] patterns)
		{
			return AddRouteHandler (module, patterns, HttpMethods.GetMethods);
		}
		
		//
		
		public RouteHandler Put (string pattern, MaruModule module)
		{
			return AddRouteHandler (module, new string [] { pattern }, HttpMethods.PutMethods);
		}

		public RouteHandler Put (string pattern, MaruAction action)
		{
			return AddRouteHandler (action, new string [] { pattern }, HttpMethods.PutMethods);
		}

		public RouteHandler Put (MaruAction action, params string [] patterns)
		{
			return AddRouteHandler (action, patterns, HttpMethods.PutMethods);
		}

		public RouteHandler Put (MaruModule module, params string [] patterns)
		{
			return AddRouteHandler (module, patterns, HttpMethods.PutMethods);
		}
		
		public RouteHandler Post (string pattern, MaruModule module)
		{
			return AddRouteHandler (module, new string [] { pattern }, HttpMethods.PostMethods);
		}

		public RouteHandler Post (string pattern, MaruAction action)
		{
			return AddRouteHandler (action, new string [] { pattern }, HttpMethods.PostMethods);
		}

		public RouteHandler Post (MaruAction action, params string [] patterns)
		{
			return AddRouteHandler (action, patterns, HttpMethods.PostMethods);
		}

		public RouteHandler Post (MaruModule module, params string [] patterns)
		{
			return AddRouteHandler (module, patterns, HttpMethods.PostMethods);
		}
		
		//
		
		public RouteHandler Delete (string pattern, MaruModule module)
		{
			return AddRouteHandler (module, new string [] { pattern }, HttpMethods.DeleteMethods);
		}

		public RouteHandler Delete (string pattern, MaruAction action)
		{
			return AddRouteHandler (action, new string [] { pattern }, HttpMethods.DeleteMethods);
		}

		public RouteHandler Delete (MaruAction action, params string [] patterns)
		{
			return AddRouteHandler (action, patterns, HttpMethods.DeleteMethods);
		}

		public RouteHandler Delete (MaruModule module, params string [] patterns)
		{
			return AddRouteHandler (module, patterns, HttpMethods.DeleteMethods);
		}
		
		//
		
		public RouteHandler Head (string pattern, MaruModule module)
		{
			return AddRouteHandler (module, new string [] { pattern }, HttpMethods.HeadMethods);
		}

		public RouteHandler Head (string pattern, MaruAction action)
		{
			return AddRouteHandler (action, new string [] { pattern }, HttpMethods.HeadMethods);
		}

		public RouteHandler Head (MaruAction action, params string [] patterns)
		{
			return AddRouteHandler (action, patterns, HttpMethods.HeadMethods);
		}

		public RouteHandler Head (MaruModule module, params string [] patterns)
		{
			return AddRouteHandler (module, patterns, HttpMethods.HeadMethods);
		}
		
		//
		
		public RouteHandler Options (string pattern, MaruModule module)
		{
			return AddRouteHandler (module, new string [] { pattern }, HttpMethods.OptionsMethods);
		}

		public RouteHandler Options (string pattern, MaruAction action)
		{
			return AddRouteHandler (action, new string [] { pattern }, HttpMethods.OptionsMethods);
		}

		public RouteHandler Options (MaruAction action, params string [] patterns)
		{
			return AddRouteHandler (action, patterns, HttpMethods.OptionsMethods);
		}

		public RouteHandler Options (MaruModule module, params string [] patterns)
		{
			return AddRouteHandler (module, patterns, HttpMethods.OptionsMethods);
		}
		
		//
		
		public RouteHandler Trace (string pattern, MaruModule module)
		{
			return AddRouteHandler (module, new string [] { pattern }, HttpMethods.TraceMethods);
		}

		public RouteHandler Trace (string pattern, MaruAction action)
		{
			return AddRouteHandler (action, new string [] { pattern }, HttpMethods.TraceMethods);
		}

		public RouteHandler Trace (MaruAction action, params string [] patterns)
		{
			return AddRouteHandler (action, patterns, HttpMethods.TraceMethods);
		}

		public RouteHandler Trace (MaruModule module, params string [] patterns)
		{
			return AddRouteHandler (module, patterns, HttpMethods.TraceMethods);
		}
		
		public void HandleTransaction (IHttpTransaction con)
		{
			if (con == null)
				throw new ArgumentNullException ("con");

			var handler = Routes.Find (con.Request);
			if (handler == null) {
				con.Response.StatusCode = 404;
				con.Response.Finish ();
				return;
			}

			try {
				handler.Invoke (new MaruContext (con));
			} catch (Exception e) {
				con.Response.StatusCode = 500;
				//
				// TODO: Maybe the cleanest thing to do is
				// have a HandleError, HandleException thing
				// on HttpTransaction, along with an UnhandledException
				// method/event on MaruModule.
				//
			}

			con.Response.Finish ();
		}

		public static void RenderTemplate (MaruContext context, string template, object data)
		{
			MemoryStream stream = new MemoryStream ();

			using (StreamWriter writer = new StreamWriter (stream)) {
				Maru.Templates.Engine.RenderToStream (template, writer, data);
				writer.Flush ();
			}

			context.Response.Write (stream.GetBuffer ());
		}

		protected virtual void OnStart ()
		{
		}
		
		protected void StartInternal ()
		{
			OnStart ();
			
			AddImplicitRoutes ();
		}

		private void AddImplicitRoutes ()
		{
			MethodInfo [] methods = this.GetType ().GetMethods ();

			foreach (MethodInfo meth in methods) {
				if (!IsActionSignature (meth))
					continue;
				if (IsIgnoredAction (meth))
					continue;
				AddHandlersForAction (Routes, meth);
			}
		}

		private bool IsActionSignature (MethodInfo meth)
		{
			ParameterInfo [] p = meth.GetParameters ();

			if (p.Length != 1)
				return false;

			if (p [0].ParameterType != typeof (IMaruContext))
				return false;

			return true;
		}

		private bool IsIgnoredAction (MethodInfo info)
		{
			var atts = info.GetCustomAttributes (typeof (IgnoreAttribute), false);

			return atts.Length > 0;
		}

		private void AddHandlersForAction (RouteHandler routes, MethodInfo info)
		{
			HttpMethodAttribute [] atts = (HttpMethodAttribute []) info.GetCustomAttributes (typeof (HttpMethodAttribute), false);

			if (atts.Length == 0) {
				AddDefaultHandlerForAction (routes, info);
				return;
			}

			foreach (HttpMethodAttribute att in atts) {
				AddHandlerForAction (routes, att, info);
			}

		}

		private void AddDefaultHandlerForAction (RouteHandler routes, MethodInfo info)
		{
			MaruAction action = (MaruAction) Delegate.CreateDelegate (typeof (MaruAction), info);
			AddImplicitRouteHandler (action, new string [] { info.Name }, HttpMethods.RouteMethods);
		}

		private void AddHandlerForAction (RouteHandler routes, HttpMethodAttribute att, MethodInfo info)
		{
			MaruAction action = (MaruAction) Delegate.CreateDelegate (typeof (MaruAction), info);
			AddImplicitRouteHandler (action, att.Patterns, att.Methods);
		}
		
	}
}

