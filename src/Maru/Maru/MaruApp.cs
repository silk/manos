


using System;
using System.IO;
using System.Reflection;

using Maru.Server;
using Maru.Routing;
using Maru.Templates;

namespace Maru {

	public class MaruApp : MaruModule {

		public MaruApp ()
		{
		}
		
		protected override void OnStart ()
		{
			LoadTemplates ();
		}
		
		private void LoadTemplates ()
		{
			TemplateFactory.Clear ();
			
			if (!File.Exists ("Templates.dll"))
				return;
				
			/*
			Assembly templates = Assembly.LoadFile ("Templates.dll");
			
			foreach (Type t in templates) {
				// if (typeof (IMaruTemplate).IsAssignableFrom (t))
					// TemplateFactory.Register (
			}
			*/
		}
	}
}

