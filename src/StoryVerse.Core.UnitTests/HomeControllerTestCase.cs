using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StoryVerse.Tests
{
	using System;

	using Castle.MonoRail.TestSupport;
	
	using NUnit.Framework;
    using StoryVerse.Core.Lookups;

    [TestFixture]
    public class HomeControllerTestCase : AbstractMRTestCase
    {
        [Test]
        public void IndexAction()
        {
            DoGet("home/index.rails");

            // Use the assert family of methods available in the base class
            // for example:
            // AssertReplyContains("something");
            // AssertReplyStartsWith("something");
        }
    }
}
