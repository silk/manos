
using System;

namespace Maru.Testing
{


	public class MockMaruTarget : IMaruTarget
	{
		public MockMaruTarget ()
		{
		}
		
		public void Invoke (IMaruContext ctx)
		{
			throw new System.NotImplementedException();
		}
		
		
		public MaruAction Action {
			get {
				throw new System.NotImplementedException();
			}
			set {
				throw new System.NotImplementedException();
			}
		}
		
	}
}
