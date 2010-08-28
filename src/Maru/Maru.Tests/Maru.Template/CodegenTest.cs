
using System;
using NUnit.Framework;

namespace Maru.Templates.Tests
{
	[TestFixture()]
	public class CodegenTest
	{

		[Test()]
		public void TestCase ()
		{
		}
		
		[Test]
		public void TestFullTypeNameForPath ()
		{
			string app_name = "FooBar";
			string path;
			string name;
			
			path = "Tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.TestsHtml", name, "a1");
			
			path = "Maru.Tests.Tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Maru.Tests.TestsHtml", name, "a2");
			
			path = "mango.tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Maru.TestsHtml", name, "a3");
			
			path = "Maru/Tests.html";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Maru.TestsHtml", name, "a4");
			
			path = "Maru.Tests/Tests.HTML";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Maru.Tests.TestsHtml", name, "a5");
			
			path = "mango/tests.hTMl";
			name = Page.FullTypeNameForPath (app_name, path);
			Assert.AreEqual ("FooBar.Templates.Maru.TestsHtml", name, "a6");
		}
		
		[Test]
		public void TestTypeNameForPathWithDoubleDots ()
		{
			string app_name = "FooBar";
			string path;
			
			path = "Maru/tests..html";
			Assert.Throws<ArgumentException> (() => Page.FullTypeNameForPath (app_name, path));
		}
	}
}
