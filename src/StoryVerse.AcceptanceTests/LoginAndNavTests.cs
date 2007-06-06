using System;
using System.Collections.Generic;
using System.Text;
using WatiN.Core;
using NUnit.Framework;
using WatiN.Core.DialogHandlers;
using WatiN.UnitTests;
using Attribute=WatiN.Core.Attribute;

namespace StoryVerse.AcceptanceTests
{
    [TestFixture]
    public class LoginAndNavTests
    {
        private TestHelper h;
        private IE ie;

        [TestFixtureSetUp]
        public void SetUp()
        {
            h = new TestHelper();
            ie = h.Ie;
            h.SetUp();
        }

        [Test]
        public void Can_login()
        {
            h.Login();
            Assert.IsTrue(ie.Form(Find.ByName("list")).Exists);
        }

        [Test]
        public void Can_nav_to_preferences()
        {
            h.Login();
            ie.Link(Find.ByText("Preferences")).Click();
            Assert.IsTrue(ie.ContainsText("Person Detail"));
        }

        [Test]
        public void Can_nav_to_stories()
        {
            h.Login();
            ie.Link(Find.ByText("Stories")).Click();
            Assert.IsTrue(ie.ContainsText("Stories"));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            h.TearDown();
        }

    }
}
