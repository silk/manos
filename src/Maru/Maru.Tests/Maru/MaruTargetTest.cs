
using System;
using NUnit.Framework;

namespace Maru.Tests
{


	[TestFixture()]
	public class MaruTargetTest
	{

		public static void FakeAction (IMaruContext ctx)
		{
		}
		
		[Test()]
		public void TextNullCtor ()
		{
			Assert.Throws<ArgumentNullException> (() => new MaruTarget (null));
		}
		
		[Test]
		public void TestSetAction ()
		{
			var t = new MaruTarget (FakeAction);
			
			Assert.NotNull (t.Action, "not null");
			Assert.AreEqual (new MaruAction (FakeAction), t.Action, "equals");
		}
		
		[Test]
		public void TestSetActionNull ()
		{
			var t = new MaruTarget (FakeAction);
			
			Assert.Throws<ArgumentNullException> (() => t.Action = null);
		}
	}
}
