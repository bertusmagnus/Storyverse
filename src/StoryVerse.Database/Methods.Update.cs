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

using Lunaverse.DbVerse.Core;
using Microsoft.SqlServer.Management.Smo;
using SmoTable = Microsoft.SqlServer.Management.Smo.Table;

namespace StoryVerse.Database
{
    public partial class Methods
    {
        [Update(1, "Tim Scott", "5/29/2007")]
        private void AddUserPreferences()
        {
            Db.AddTable("UserPreferences")
                .AddColumn("RowsPerPage", DataType.Int);
        }

        [Update(2, "Tim Scott", "11/18/2007")]
        private void AddIssueAggregate()
        {
            Db.AddTable("Issue", "Id")
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

            Db.AddTable("IssueNote")
                .AddColumn("Issue", DataType.UniqueIdentifier, "Issue", "Id")
                .AddColumn("Body", DataType.NVarCharMax)
                .AddColumn("CreatedBy", DataType.UniqueIdentifier, "Person", "Id")
                .AddDatestampColumn("DateCreated");

            Db.AddTable("IssueAttachment")
                .AddColumn("Issue", DataType.UniqueIdentifier, "Issue", "Id")
                .AddColumn("Title", DataType.VarChar(100))
                .AddColumn("Filename", DataType.VarChar(100))
                .AddColumn("Size", DataType.Int)
                .AddColumn("Body", DataType.VarBinaryMax)
                .AddColumn("CreatedBy", DataType.UniqueIdentifier, "Person", "Id")
                .AddDatestampColumn("DateCreated");

            Db.AddTable("IssueChange")
                .AddColumn("Issue", DataType.UniqueIdentifier, "Issue", "Id")
                .AddColumn("PropertyName", DataType.VarChar(50))
                .AddColumn("OldValue", DataType.VarCharMax)
                .AddColumn("NewValue", DataType.VarCharMax)
                .AddColumn("ChangedBy", DataType.UniqueIdentifier, "Person", "Id")
                .AddDatestampColumn("ChangeDate");

            Db.AddTable("AppSetting")
                .AddColumn("Name", DataType.VarChar(50))
                .AddColumn("Value", DataType.NVarCharMax);

            Db.GenerateRelations();
        }

        [Update(3, "Tim Scott", "1/12/2008")]
        private void MakeStoryProjectNullable()
        {
            SmoTable story = Db.Table("Story").SmoTable;
            story.Columns["Project"].Nullable = true;
            story.Alter();
        }

        [Update(4, "Tim Scott", "1/26/2008")]
        private void AddEmailFieldsForUser()
        {
            Db.Table("Person").AddColumn("Email", DataType.VarChar(200));
            Db.Table("UserPreferences").AddColumnWithDefaultValue("NotifyOfIssueAssignment", DataType.Bit, false, "0");
        }

        [Update(5, "Tim Scott", "1/26/2008")]
        private void AddIssueLookupTables()
        {
            AddLookupTable("IssuePriority")
                .InsertRow("1, 'High', 1")
                .InsertRow("2, 'Medium', 2")
                .InsertRow("3, 'Low', 3");

            AddLookupTable("IssueSeverity")
                .InsertRow("1, 'Blocker', 1")
                .InsertRow("2, 'Critical', 2")
                .InsertRow("3, 'Major', 3")
                .InsertRow("4, 'Normal', 4")
                .InsertRow("5, 'Minor', 5")
                .InsertRow("6, 'Trivial', 6");

            AddLookupTable("IssueDisposition")
                .InsertRow("1, 'Fixed', 1")
                .InsertRow("2, 'Duplicate', 2")
                .InsertRow("3, 'Invalid', 3")
                .InsertRow("4, 'Cant Replicate', 4")
                .InsertRow("5, 'Wont Fix', 5");

            Db.DefineRelation("Issue", "Priority", "IssuePriority", "Id", ForeignKeyAction.Cascade);
            Db.DefineRelation("Issue", "Severity", "IssueSeverity", "Id", ForeignKeyAction.Cascade);
            Db.DefineRelation("Issue", "Disposition", "IssueDisposition", "Id", ForeignKeyAction.Cascade);

            Db.GenerateRelations();
        }

        [Update(6, "Tim Scott", "11/18/2007")]
        private void ChangeNtextFieldsToNVarCharMax()
        {
            Db.Table("Story").Column("Body").ChangeDataType(DataType.NVarCharMax);
            Db.Table("Story").Column("Notes").ChangeDataType(DataType.NVarCharMax);
            Db.Table("Task").Column("Description").ChangeDataType(DataType.NVarCharMax);
            Db.Table("Task").Column("Notes").ChangeDataType(DataType.NVarCharMax);
        }
    }
}