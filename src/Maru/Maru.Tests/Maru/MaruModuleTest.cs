
using System;
using NUnit.Framework;

using Maru;
using Maru.Testing;
using Maru.Server.Testing;

namespace Maru.Tests
{


	[TestFixture()]
	public class MaruModuleTest
	{

		private void FakeAction (IMaruContext ctx)
		{
		}
		
		private class FakeModule : MockMaruModule {

			public static void FakeAction (IMaruContext ctx)
			{
			}
		}
		
		[Test()]
		public void TestAddRouteNull ()
		{
			MaruModule m = new MaruModule ();
			
			Assert.Throws<ArgumentNullException> (() => m.Route (new MaruModule (), null, null));
			Assert.Throws<ArgumentNullException> (() => m.Route (new MaruAction (FakeAction), null, null));
			Assert.Throws<ArgumentNullException> (() => m.Route (null, new MaruAction (FakeAction)));
			Assert.Throws<ArgumentNullException> (() => m.Route (null, new MaruModule ()));                          
		}
		
		[Test]
		public void TestRouteToTarget ()
		{
			string [] methods = new string [] {
				"GET", "PUT", "POST", "HEAD", "DELETE", "TRACE", "OPTIONS",
			};
			
			for (int i = 0; i < methods.Length; i++) {
				var m = new MockMaruModule ();
				var req = new MockHttpRequest (methods [i], "Foobar");
			
				m.Route ("Foobar", new MaruAction (FakeAction));
			
				Assert.AreEqual (new MaruAction (FakeAction), m.Routes.Find (req).Action);
			}
		}
		
		[Test]
		public void TestRouteNull ()
		{
			var m = new MockMaruModule ();
			
			MaruModule mm = null;
			Assert.Throws<ArgumentNullException> (() => m.Route ("foo", mm), "a1");
			Assert.Throws<ArgumentNullException> (() => m.Route (mm, "foo", "bar", "baz"), "a2");
			
			MaruAction ma = null;
			Assert.Throws<ArgumentNullException> (() => m.Route ("foo", ma), "a3");
			Assert.Throws<ArgumentNullException> (() => m.Route (ma, "foo", "bar", "baz"), "a4");
			
			mm = new MockMaruModule ();
			Assert.Throws<ArgumentNullException> (() => m.Route (null, mm), "a4");
			Assert.Throws<ArgumentNullException> (() => m.Route (mm, "foo", "bar", "baz", null), "a5");
			
			ma = FakeAction;
			Assert.Throws<ArgumentNullException> (() => m.Route (null, ma), "a6");
			Assert.Throws<ArgumentNullException> (() => m.Route (ma, "foo", "bar", "baz", null), "a7");
		}
		
		[Test]
		public void TestRouteToModule ()
		{
			string [] methods = new string [] {
				"GET", "PUT", "POST", "HEAD", "DELETE", "TRACE", "OPTIONS",
			};
			
			for (int i = 0; i < methods.Length; i++) {
				var m = new MockMaruModule ();
				var req = new MockHttpRequest (methods [i], "FakeModule/FakeAction");
			
				m.Route ("FakeModule/", new FakeModule ());
			
				//
				// I guess technically this is testing the auto name registering too
				//
				Assert.AreEqual (new MaruAction (FakeModule.FakeAction), m.Routes.Find (req).Action);	
			}
		}
		
		[Test]
		public void TestGetToTarget ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("GET", "Foobar");
			
			m.Get ("Foobar", new MaruAction (FakeAction));
			Assert.AreEqual (new MaruAction (FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("POST", "Foobar");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestGetToModule ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("GET", "FakeModule/FakeAction");
			
			m.Get ("FakeModule/", new FakeModule ());
			
			//
			// I guess technically this is testing the auto name registering too
			//
			Assert.AreEqual (new MaruAction (FakeModule.FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("POST", "FakeModule/FakeAction");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestPutToTarget ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("PUT", "Foobar");
			
			m.Put ("Foobar", new MaruAction (FakeAction));
			Assert.AreEqual (new MaruAction (FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("POST", "Foobar");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestPutToModule ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("PUT", "FakeModule/FakeAction");
			
			m.Put ("FakeModule/", new FakeModule ());
			
			//
			// I guess technically this is testing the auto name registering too
			//
			Assert.AreEqual (new MaruAction (FakeModule.FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("POST", "FakeModule/FakeAction");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestPostToTarget ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("POST", "Foobar");
			
			m.Post ("Foobar", new MaruAction (FakeAction));
			Assert.AreEqual (new MaruAction (FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "Foobar");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestPostToModule ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("POST", "FakeModule/FakeAction");
			
			m.Post ("FakeModule/", new FakeModule ());
			
			//
			// I guess technically this is testing the auto name registering too
			//
			Assert.AreEqual (new MaruAction (FakeModule.FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "FakeModule/FakeAction");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestDeleteToTarget ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("DELETE", "Foobar");
			
			m.Delete ("Foobar", new MaruAction (FakeAction));
			Assert.AreEqual (new MaruAction (FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "Foobar");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestDeleteToModule ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("DELETE", "FakeModule/FakeAction");
			
			m.Delete ("FakeModule/", new FakeModule ());
			
			//
			// I guess technically this is testing the auto name registering too
			//
			Assert.AreEqual (new MaruAction (FakeModule.FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "FakeModule/FakeAction");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestHeadToTarget ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("HEAD", "Foobar");
			
			m.Head ("Foobar", new MaruAction (FakeAction));
			Assert.AreEqual (new MaruAction (FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "Foobar");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestHeadToModule ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("HEAD", "FakeModule/FakeAction");
			
			m.Head ("FakeModule/", new FakeModule ());
			
			//
			// I guess technically this is testing the auto name registering too
			//
			Assert.AreEqual (new MaruAction (FakeModule.FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "FakeModule/FakeAction");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestOptionsToTarget ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("OPTIONS", "Foobar");
			
			m.Options ("Foobar", new MaruAction (FakeAction));
			Assert.AreEqual (new MaruAction (FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "Foobar");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestOptionsToModule ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("OPTIONS", "FakeModule/FakeAction");
			
			m.Options ("FakeModule/", new FakeModule ());
			
			//
			// I guess technically this is testing the auto name registering too
			//
			Assert.AreEqual (new MaruAction (FakeModule.FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "FakeModule/FakeAction");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestTraceToTarget ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("TRACE", "Foobar");
			
			m.Trace ("Foobar", new MaruAction (FakeAction));
			Assert.AreEqual (new MaruAction (FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "Foobar");
			Assert.IsNull (m.Routes.Find (req));
		}
		
		[Test]
		public void TestTraceToModule ()
		{
			var m = new MockMaruModule ();
			var req = new MockHttpRequest ("TRACE", "FakeModule/FakeAction");
			
			m.Trace ("FakeModule/", new FakeModule ());
			
			//
			// I guess technically this is testing the auto name registering too
			//
			Assert.AreEqual (new MaruAction (FakeModule.FakeAction), m.Routes.Find (req).Action);
			
			req = new MockHttpRequest ("GET", "FakeModule/FakeAction");
			Assert.IsNull (m.Routes.Find (req));
		}
	}
}
