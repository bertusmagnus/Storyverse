/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Models;
using NUnit.Framework;

namespace StoryVerse.IntegrationTests
{
    [TestFixture]
    public class IssueTests : BaseArTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void can_save_new_issue()
        {
            Company company = new Company();
            company.Name = "Test Company";
            company.CreateAndFlush();

            Issue issue = new Issue();
            issue.Title = "Test issue";
            issue.Type = IssueType.Defect;

            IssueNote note = new IssueNote();
            note.Body = "Test note";
            issue.AddNote(note);

            IssueChange change = new IssueChange();
            change.PropertyName = "Test change";
            issue.AddChange(change);

            IssueAttachment att = new IssueAttachment();
            att.Body = new byte[10];
            att.Title = "Test attachment";
            issue.AddAttachment(att);

            Project project = new Project();
            project.Company = company;
            project.AddIssue(issue);
            project.Name = "Test project";
            project.CreateAndFlush();

            using (SessionScope newScope = new SessionScope())
            {
                Issue actual = Issue.Find(issue.Id);
                Assert.AreEqual("Test issue", actual.Title);
                Assert.AreEqual("Test note", actual.Notes[0].Body);
                Assert.AreEqual("Test change", actual.Changes[0].PropertyName);
                Assert.AreEqual("Test attachment", actual.Attachments[0].Title);
            }

            project.DeleteAndFlush();
            company.DeleteAndFlush();

        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}