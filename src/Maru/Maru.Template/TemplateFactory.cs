


using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace Mango.Templates {
	
	public static class TemplateFactory {
		
		private static Dictionary<string,IMangoTemplate> templates = new Dictionary<string, IMangoTemplate> ();
		
		public static IMangoTemplate Get (string name)
		{
			IMangoTemplate res = null;
			
			if (!TryGet (name, out res))
				return null;
			
			return res;
		}
		
		public static bool TryGet (string name, out IMangoTemplate template)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			
			return templates.TryGetValue (name, out template);
		}
		
		public static void Register (string name, IMangoTemplate template)
		{
			if (name == null)
				throw new ArgumentNullException ("name");
			if (template == null)
				throw new ArgumentNullException ("template");
			
			if (templates.ContainsKey (name))
				throw new InvalidOperationException (String.Format ("A template named {0} has already been registered.", name));
			
			templates.Add (name, template);
		}
		
		public static void Clear ()
		{
			templates.Clear ();
		}
	}
}

