/*
 * Created by: 
 * Created: Thursday, January 11, 2007
 */

using StoryVerse.Core.Models;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Framework.Config;
using NUnit.Framework;
using System;

namespace StoryVerse.Core.UnitTests.Model
{
    public abstract class AbstractModelTestCase
    {
        protected SessionScope scope;

        [TestFixtureSetUp]
        public virtual void FixtureInit()
        {
             InitFramework();
        }

        [SetUp]
        public virtual void Init()
        {
            PrepareSchema();
            CreateScope();
        }

        [TearDown]
        public virtual void Terminate()
        {
            DisposeScope();
            DropSchema();
        }

        [TestFixtureTearDown]
        public virtual void TerminateAll()
        {
        }

        protected void FlushAndRecreateScope()
        {
            DisposeScope();
            CreateScope();
        }

        protected void CreateScope()
        {
            scope = new SessionScope();
        }

        protected void DisposeScope()
        {
            scope.Dispose();
        }

        protected virtual void PrepareSchema()
        {
            // If you want to delete everything from the model.
            // Remember to do it in a descendent dependency order

            // Office.DeleteAll();
            // User.DeleteAll();

            // Another approach is to always recreate the schema 
            // (please use a separate test database if you want to do that)

            ActiveRecordStarter.CreateSchema();
        }

        protected virtual void DropSchema()
        {
            ActiveRecordStarter.DropSchema();
        }

        protected virtual void InitFramework()
        {
            IConfigurationSource source = ActiveRecordSectionHandler.Instance;
            ActiveRecordStarter.Initialize(source,
                   new Type[]
                                   {
                                       typeof(Project), 
                                       typeof(Story),
                                       typeof(Iteration),
                                       typeof(Task),
                                       typeof(Person),
                                       typeof(ProductionRelease),
                                       typeof(Component),
                                   });
        }
    }
}