
using System;

namespace Maru
{
	public interface IMaruTarget
	{
		MaruAction Action {
			get;
			set;
		}
		
		void Invoke (IMaruContext ctx);
	}
}
