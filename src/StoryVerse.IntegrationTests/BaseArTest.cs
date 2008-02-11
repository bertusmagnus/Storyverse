/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using NUnit.Framework;

namespace StoryVerse.IntegrationTests
{
    public abstract class BaseArTest
    {
        protected SessionScope scope;

        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
            ActiveRecordStarter.Initialize(Assembly.Load("StoryVerse.Core"),
                ActiveRecordSectionHandler.Instance);
            scope = new SessionScope();
        }

        [TestFixtureTearDown]
        public virtual void FixtureTeardown()
        {
            scope.Dispose();
        }
    }
}