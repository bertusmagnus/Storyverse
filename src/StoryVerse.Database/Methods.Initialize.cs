/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
 * 
 * Lunaverse DbBuilder is included with StoryVerse as a utility
 * to assist with deployment of StoryVerse.  Lunaverse DbBuilder 
 * is not an open source product and any usage except in conjunction
 * with StoryVerse is prohibited.
*/

using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using Microsoft.SqlServer.Management.Smo;
using StoryVerse.Core.Models;
using SmoTable = Microsoft.SqlServer.Management.Smo.Table;

namespace StoryVerse.Database
{
    public partial class Methods
    {
        private void CreateSchema()
        {
            Db.AddTable("Company", "Id")
                .AddColumn("Name", DataType.NVarChar(100))
                .AddColumn("Type", DataType.Int);

            Db.AddTable("Component")
				.AddColumn("Name", DataType.NVarChar(100))
				.AddColumn("Project", DataType.UniqueIdentifier, "Project", "Id"); ;

			Db.AddTable("Iteration")
				.AddColumn("Name", DataType.NVarChar(100))
				.AddColumn("Project", DataType.UniqueIdentifier, "Project", "Id");

            Db.AddTable("Person")
				.AddColumn("FirstName", DataType.NVarChar(100))
                .AddColumn("LastName", DataType.VarChar(100))
				.AddColumn("Username", DataType.VarChar(200))
				.AddColumn("Password", DataType.VarChar(200))
				.AddColumnWithDefaultValue("IsAdmin", DataType.Bit, false, "((0))")
				.AddColumnWithDefaultValue("CanViewOnly", DataType.Bit, false, "((0))")
				.AddColumn("Company", DataType.UniqueIdentifier, "Company", "Id");

			Db.AddTable("ProductionRelease")
				.AddColumn("Name", DataType.NVarChar(100))
				.AddColumn("Project", DataType.UniqueIdentifier, "Project", "Id");

            Db.AddTable("Project")
                .AddColumn("Name", DataType.NVarChar(100))
				.AddColumn("Company", DataType.UniqueIdentifier, "Company", "Id");

            Db.AddTable("Story")
                .AddColumn("Number", DataType.Int)
                .AddColumn("Title", DataType.NVarChar(500))
				.AddColumn("Body", DataType.NText)
				.AddColumn("Notes", DataType.NText)
				.AddColumn("Priority", DataType.Int)
				.AddColumn("TechnicalRisk", DataType.Int)
				.AddColumn("EstimateFiftyPercent", DataType.Int)
				.AddColumn("EstimateNinetyPercent", DataType.Int)
				.AddColumn("Status", DataType.Int)
				.AddColumn("Project", DataType.UniqueIdentifier, "Project", "Id")
				.AddColumn("Component", DataType.UniqueIdentifier, "Component", "Id")
				.AddColumn("Iteration", DataType.UniqueIdentifier, "Iteration", "Id");

        	Db.AddTable("Task")
        		.AddColumn("Number", DataType.Int)
        		.AddColumn("Title", DataType.NVarChar(500))
        		.AddColumn("Description", DataType.NText)
        		.AddColumn("TechnicalRisk", DataType.Int)
        		.AddColumn("Status", DataType.Int)
        		.AddColumn("Notes", DataType.NText)
        		.AddColumn("Owner", DataType.UniqueIdentifier, "Person", "Id")
        		.AddColumn("Project", DataType.UniqueIdentifier, "Project", "Id")
        		.AddColumn("Iteration", DataType.UniqueIdentifier, "Iteration", "Id");

        	Db.AddTable("TaskStory")
        		.AddColumn("TaskId", DataType.UniqueIdentifier, "Task", "Id")
        		.AddColumn("StoryId", DataType.UniqueIdentifier, "Story", "Id");

        	Db.AddTable("TaskEstimate")
        		.AddColumn("HoursRemaining", DataType.Int)
        		.AddColumn("Date", DataType.DateTime)
        		.AddColumn("CreatedBy", DataType.UniqueIdentifier, "Person", "Id")
        		.AddColumn("Task", DataType.UniqueIdentifier, "Task", "Id");

        	Db.AddTable("Test")
        		.AddColumn("Number", DataType.Int)
        		.AddColumn("Body", DataType.NText)
				.AddColumn("Story", DataType.UniqueIdentifier, "Story", "Id");
		}

		public void AddInitialCompanyAndUser()
		{
			Assembly assembly = Assembly.Load("StoryVerse.Core");
			ActiveRecordStarter.Initialize(assembly,
				ActiveRecordSectionHandler.Instance);
			Company c = new Company();
			c.Name = "getin";
			Person p = new Person();
			p.FirstName = "Get";
			p.LastName = "In";
			p.Username = "getin";
			p.Password = "letmein";
			p.IsAdmin = true;
			c.AddEmployee(p);
			c.CreateAndFlush();
		}
    }
}