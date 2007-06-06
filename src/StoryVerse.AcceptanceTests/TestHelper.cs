using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.WebHost;
using NUnit.Framework;
using WatiN.Core;
using System.Reflection;
using Attribute = WatiN.Core.Attribute;

namespace StoryVerse.AcceptanceTests
{
    public class TestHelper
    {
        private static readonly int _webServerPort = new Random().Next(9000, 10000);
        private static string _baseUrl = "http://localhost:" + _webServerPort;
        private Server _webServer;
        private IE _ie;

        public IE Ie
        {
            get
            {
                if (_ie == null)
                {
                    _ie = new IE(_baseUrl);
                }
                return _ie;
            }
        }

        public void SetUp()
        {
            StartWebServer();
        }

        public void TearDown()
        {
            StopWebServer();
            _ie.ForceClose();
        }

        public void Login()
        {
            Ie.GoTo(Url("login/index.rails"));
            Ie.TextField(Find.ById("username")).TypeText("getin");
            Ie.TextField(Find.ById("password")).TypeText("letmein");
            Ie.Button(Find.ByName("loginButton")).Click();
        }

        private string Url(string path)
        {
            string result = string.Format(_baseUrl + "/" + path);
            return result;
        }

        public void StartWebServer()
        {
            DirectoryInfo trunk = new DirectoryInfo(
                Assembly.GetExecutingAssembly().Location)
                .Parent.Parent.Parent.Parent;
            string physicalPath = Path.Combine(trunk.FullName, "StoryVerse.WebUI");
            _webServer = new Server(_webServerPort, @"/", physicalPath);
            _webServer.Start();
        }

        public void StopWebServer()
        {
            if (_webServer != null) _webServer.Stop();
        }
    }
}
