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

namespace StoryVerse.Tests.ModelTests
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
            try
            {
                scope = new SessionScope();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected void DisposeScope()
        {
            try
            {
                scope.Dispose();
            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        protected virtual void PrepareSchema()
        {
            // If you want to delete everything from the model.
            // Remember to do it in a descendent dependency order

            // Office.DeleteAll();
            // User.DeleteAll();

            // Another approach is to always recreate the schema 
            // (please use a separate test database if you want to do that)

            try
            {
                ActiveRecordStarter.CreateSchema();
            
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected virtual void DropSchema()
        {
            try
            {
                ActiveRecordStarter.DropSchema();
            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        protected virtual void InitFramework()
        {
            IConfigurationSource source = ActiveRecordSectionHandler.Instance;

            //ActiveRecordStarter.Initialize(source);
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


            // Remember to add the types, for example
            // ActiveRecordStarter.Initialize( source, typeof(Blog), typeof(Post) );
        }
    }
}