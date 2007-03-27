using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Selenium;
using System.Diagnostics;
using Microsoft.VisualStudio.WebHost;
using StoryVerse.Core.Models;

namespace StoryVerse.AcceptanceTests
{
    [TestFixture]
    public class SeleniumExperiment_TestTestFixture
    {
        private ISelenium sel = null;
        private const string physicalPath = @"C:\StoryVerse\src\StoryVerse";
        private static readonly int webServerPort = new Random().Next(9000, 10000);
        private string baseUrl = "http://localhost:" + webServerPort;
        private const string waitTime = "3000";
        private Process selServer;
        private Server webServer;

        [TestFixtureSetUp]
        public void SetUp()
        {
            StartSeleniumServer();
            StartWebServer();
            sel = new DefaultSelenium("localhost", 4444, "*firefox", baseUrl);
            sel.Start();
        }

        [Test]
        public void LoginTest()
        {
            Login();
            Wait();
            Assert.IsTrue(sel.IsTextPresent("Projects"));
        }

        [Test]
        public void ModifyProjectTest()
        {
            Login();
            Wait();

            sel.Click("link=Web Request");
            Wait();

            sel.Type("entity.Name", "Web Request Rocks");
            sel.Click("actionButton Save");
            Wait();
            
            sel.Click("actionButton List");
            Wait();
            
            sel.Click("link=Web Request Rocks");
            Wait();
            
            sel.Type("entity.Name", "Web Request");
            sel.Click("actionButton Save");
        }

        private void Wait()
        {
            sel.WaitForPageToLoad(waitTime);
        }

        [Test]
        public void AddAndDeleteProjectTest()
        {
            string newProjectName = "TESTPROJECT";

            Login();
            Wait();

            sel.Click("link=New");
            Wait();

            sel.Type("entity.Name", newProjectName);
            sel.Select("entity.Company.Id", "getin");
            sel.Click("actionButton Create");
            Wait();
            
            sel.Click("link=" + newProjectName);
            Wait();
            
            sel.Click("actionButton Delete");
            Wait();
            sel.GetConfirmation();
            Assert.IsFalse(sel.IsElementPresent("link=" + newProjectName));
        }

        private void Login()
        {
            sel.Open(Url("login/index.rails"));
            sel.Type("username", "getin");
            sel.Type("password", "letmein");
            sel.Click("loginButton");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            sel.Stop();
            StopProc(selServer);
            StopWebServer();
        }

        private string Url(string path)
        {
            string result = string.Format(baseUrl + "/" + path);
            return result;
        }

        private void StartSeleniumServer()
        {
            selServer = new Process();
            selServer.StartInfo.FileName = "java";
            selServer.StartInfo.Arguments = "-jar selenium-server.jar -interactive  -debug";
            selServer.Start();
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

        //private void StartWebServerProc()
        //{
        //    webServerProc = new Process();
        //    webServerProc.StartInfo.FileName = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\WebDev.WebServer.EXE";
        //    webServerProc.StartInfo.Arguments = string.Format(@"/port:{0} /path:{1}", webServerPort, physicalPath);
        //    webServerProc.Start();
        //}

        private static void StopProc(Process proc)
        {
            if (proc != null) proc.Kill();
        }

    }
}
