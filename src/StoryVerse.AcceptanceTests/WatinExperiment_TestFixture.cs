using System;
using System.Collections.Generic;
using System.Text;
using WatiN.Core;
using NUnit.Framework;
using WatiN.Core.DialogHandlers;
using WatiN.UnitTests;

namespace StoryVerse.AcceptanceTests
{
    [TestFixture]
    public class WatinTestTestFixture
    {
        private TestHelper helper = new TestHelper();
        private IE ie;

        [TestFixtureSetUp]
        public void SetUp()
        {
            helper.StartWebServer();
            ie = helper.Ie;
        }

        [Test]
        public void LoginTest()
        {
            helper.Login();
            Assert.IsTrue(ie.ContainsText("Project"));
        }

        [Test]
        public void AddAndDeleteProjectTest()
        {
            string newProjectName = "TESTPROJECT";

            helper.Login();
            ie.Link(Find.ByText("New")).Click();

            ie.TextField(Find.ByName("entity.Name")).TypeText(newProjectName);
            ie.SelectList(Find.ByName("entity.Company.Id")).Select("getin");
            ie.Button(Find.ByValue("Create")).Click();
            ie.Button(Find.ByValue("List")).Click();

            ie.Link(Find.ByText(newProjectName)).Click();

            ie.Button(Find.ByValue("Delete")).Click();

            AlertDialogHandler alertDialogHandler = new AlertDialogHandler();
            using (new UseDialogOnce(ie.DialogWatcher, alertDialogHandler))
            {
                alertDialogHandler.WaitUntilExists();
                alertDialogHandler.OKButton.Click();
                ie.WaitForComplete();
            }

            Assert.AreEqual(null, ie.Button(Find.ByValue("Create")));
            Assert.AreEqual(null, ie.Element(Find.ByValue(newProjectName)));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            helper.StopWebServer();
        }

    }
}
