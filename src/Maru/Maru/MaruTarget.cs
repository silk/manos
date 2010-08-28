
using System;

namespace Maru
{
	public class MaruTarget : IMaruTarget
	{
		private MaruAction action;
		
		public MaruTarget (MaruAction action)
		{
			Action = action;
		}
		
		public MaruAction Action {
			get { return action; }
			set {
				if (value == null)
					throw new ArgumentNullException ("action");
				action = value;
			}
		}
		
		public void Invoke (IMaruContext ctx)
		{
			Action (ctx);
		}
	}
}
