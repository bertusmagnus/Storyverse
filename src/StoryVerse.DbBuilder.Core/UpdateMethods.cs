/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
 * 
 * Lunaverse DbBuilder is included with StoryVerse as a utility
 * to assist with deployment of StoryVerse.  Lunaverse DbBuilder 
 * is not an open source product and any usage except in conjunction
 * with StoryVerse is prohibited.
 * 
*/

using Lunaverse.DbBuilder.Core;
using Microsoft.SqlServer.Management.Smo;
using SmoTable = Microsoft.SqlServer.Management.Smo.Table;

namespace StoryVerse.DbBuilder.Core
{
    public partial class Methods : MethodsBase
    {
        [Update(0, "Tim Scott", "5/29/2007")]
        private void InitializeDatabaseUpdates()
        {
            CreateDatabaseUpdatesTable();
        }

        [Update(1, "Tim Scott", "5/29/2007")]
        private void AddUserPreferences()
        {
            Du.AddTable("UserPreferences", "Id", DataType.UniqueIdentifier, false)
                .AddColumn("RowsPerPage", DataType.Int);
        }

        [Update(2, "Tim Scott", "11/18/2007")]
        private void AddIssueAggregate()
        {
            Du.AddTable("Issue", "Id", DataType.UniqueIdentifier, false)
                .AddColumn("Number", DataType.Int, false)
                .AddColumn("Project", DataType.UniqueIdentifier, "Project", "Id")
                .AddColumn("Type", DataType.Int)
                .AddColumn("Status", DataType.Int)
                .AddColumn("Title", DataType.NVarChar(100))
                .AddColumn("Description", DataType.NVarCharMax)
                .AddColumn("Priority", DataType.Int)
                .AddColumn("Severity", DataType.Int)
                .AddColumn("Disposition", DataType.Int)
                .AddColumn("Component", DataType.UniqueIdentifier, "Component", "Id")
                .AddColumn("ReportedBy", DataType.UniqueIdentifier, "Person", "Id")
                .AddColumn("Owner", DataType.UniqueIdentifier, "Person", "Id")
                .AddColumn("LastUpdatedBy", DataType.UniqueIdentifier, "Person", "Id")
                .AddDatestampColumn("DateCreated")
                .AddDatestampColumn("DateLastUpdated");

            Du.AddTable("IssueNote", "Id", DataType.UniqueIdentifier, false)
                .AddColumn("Issue", DataType.UniqueIdentifier, "Issue", "Id")
                .AddColumn("Body", DataType.NVarCharMax)
                .AddColumn("CreatedBy", DataType.UniqueIdentifier, "Person", "Id")
                .AddDatestampColumn("DateCreated");

            Du.AddTable("IssueAttachment", "Id", DataType.UniqueIdentifier, false)
                .AddColumn("Issue", DataType.UniqueIdentifier, "Issue", "Id")
                .AddColumn("Title", DataType.VarChar(100))
                .AddColumn("Filename", DataType.VarChar(100))
                .AddColumn("Size", DataType.Int)
                .AddColumn("Body", DataType.VarBinaryMax)
                .AddColumn("CreatedBy", DataType.UniqueIdentifier, "Person", "Id")
                .AddDatestampColumn("DateCreated");

            Du.AddTable("IssueChange", "Id", DataType.UniqueIdentifier, false)
                .AddColumn("Issue", DataType.UniqueIdentifier, "Issue", "Id")
                .AddColumn("PropertyName", DataType.VarChar(50))
                .AddColumn("OldValue", DataType.VarCharMax)
                .AddColumn("NewValue", DataType.VarCharMax)
                .AddColumn("ChangedBy", DataType.UniqueIdentifier, "Person", "Id")
                .AddDatestampColumn("ChangeDate");

            Du.AddTable("AppSetting", "Id", DataType.UniqueIdentifier, false)
                .AddColumn("Name", DataType.VarChar(50))
                .AddColumn("Value", DataType.NVarCharMax);

            Du.GenerateRelations();
        }

        [Update(3, "Tim Scott", "1/12/2008")]
        private void MakeStoryProjectNullable()
        {
            SmoTable story = Du.Database.Tables["Story"];
            story.Columns["Project"].Nullable = true;
            story.Alter();
        }

        [Update(4, "Tim Scott", "1/26/2008")]
        private void AddEmailFieldsForUser()
        {
            Du.Table("Person").AddColumn("Email", DataType.VarChar(200));
            Du.Table("UserPreferences").AddColumnWithDefaultValue("NotifyOfIssueAssignment", DataType.Bit, false, "0");
        }

        [Update(5, "Tim Scott", "1/26/2008")]
        private void AddIssueLookupTables()
        {
            AddLookupTable("IssuePriority");
            AddLookupTable("IssueSeverity");
            AddLookupTable("IssueDisposition");

            AddRow("IssuePriority", "1, 'High', 1");
            AddRow("IssuePriority", "2, 'Medium', 2"); 
            AddRow("IssuePriority", "3, 'Low', 3");

            AddRow("IssueSeverity", "1, 'Blocker', 1");
            AddRow("IssueSeverity", "2, 'Critical', 2");
            AddRow("IssueSeverity", "3, 'Major', 3");
            AddRow("IssueSeverity", "4, 'Normal', 4");
            AddRow("IssueSeverity", "5, 'Minor', 5");
            AddRow("IssueSeverity", "6, 'Trivial', 6");

            AddRow("IssueDisposition", "1, 'Fixed', 1");
            AddRow("IssueDisposition", "2, 'Duplicate', 2");
            AddRow("IssueDisposition", "3, 'Invalid', 3");
            AddRow("IssueDisposition", "4, 'Cant Replicate', 4");
            AddRow("IssueDisposition", "5, 'Wont Fix', 5");

            Du.DefineRelation("Issue", "Priority", "IssuePriority", "Id", ForeignKeyAction.Cascade);
            Du.DefineRelation("Issue", "Severity", "IssueSeverity", "Id", ForeignKeyAction.Cascade);
            Du.DefineRelation("Issue", "Disposition", "IssueDisposition", "Id", ForeignKeyAction.Cascade);

            Du.GenerateRelations();
        }

        [Update(6, "Tim Scott", "11/18/2007")]
        private void ChangeNtextFieldsToNVarCharMax()
        {
            Du.Table("Story").Column("Body").ChangeDataType(DataType.NVarCharMax);
            Du.Table("Story").Column("Notes").ChangeDataType(DataType.NVarCharMax);
            Du.Table("Task").Column("Description").ChangeDataType(DataType.NVarCharMax);
            Du.Table("Task").Column("Notes").ChangeDataType(DataType.NVarCharMax);
        }
    }
}