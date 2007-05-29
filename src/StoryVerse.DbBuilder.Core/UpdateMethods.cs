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
            Du.AddTable("UserPreferences", "Id", DataType.UniqueIdentifier, false);
            Du.AddColumn("RowsPerPage", DataType.Int);
        }
    }
}