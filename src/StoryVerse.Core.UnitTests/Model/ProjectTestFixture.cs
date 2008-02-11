/*
 * Created by: 
 * Created: Thursday, January 11, 2007
 */

using StoryVerse.Core.Models;
using NUnit.Framework;

namespace StoryVerse.Core.UnitTests.Model
{
    [TestFixture]
    public class ProjectTestFixture : AbstractModelTestCase
    {
        [Test]
        public void Creation()
        {
            Project project = new Project();
            project.Name = "The Project";
            project.Create();

            //FlushAndRecreateScope(); // Persist the changes as we're using scopes

            Project[] projects = Project.FindAll();
            Assert.AreEqual(1, projects.Length);
            Assert.AreEqual("The Project", projects[0].Name);
        }
    }
}