using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.WebHost;
using WatiN.Core;
using NUnit.Framework;
using WatiN.Core.DialogHandlers;
using WatiN.UnitTests;

namespace StoryVerse.AcceptanceTests
{
    public class WatinExperiment_TestFixture
    {
        [TestFixture]
        public class SeleniumTestTestFixture
        {
            private const string physicalPath = @"C:\LunaverseRepositories\StoryVerse\src\StoryVerse.WebUI";
            private static readonly int webServerPort = new Random().Next(9000, 10000);
            private static string baseUrl = "http://localhost:" + webServerPort;
            private Server webServer;
            private IE ie;

            [TestFixtureSetUp]
            public void SetUp()
            {
                StartWebServer();
                ie = new IE(baseUrl);
            }

            [Test]
            public void LoginTest()
            {
                Login();
                Assert.IsTrue(ie.ContainsText("Project"));
            }

            [Test]
            public void AddAndDeleteProjectTest()
            {
                string newProjectName = "TESTPROJECT";

                Login();
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

            private void Login()
            {
                ie.GoTo(Url("login/index.rails"));
                ie.TextField(Find.ById("username")).TypeText("getin");
                ie.TextField(Find.ById("password")).TypeText("letmein");
                ie.Button(Find.ByName("loginButton")).Click();
            }

            [TestFixtureTearDown]
            public void TearDown()
            {
                StopWebServer();
                //StopProc(webServerProc);
            }

            private static string Url(string path)
            {
                string result = string.Format(baseUrl + "/" + path);
                return result;
            }

            private void StartWebServer()
            {
                webServer = new Server(webServerPort, @"/", physicalPath);
                webServer.Start();
            }

            private void StopWebServer()
            {
                if (webServer != null) webServer.Stop();
            }
        }
    }
}
