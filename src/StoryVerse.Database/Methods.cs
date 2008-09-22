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

using System;
using Lunaverse.DbVerse.Core;
using Microsoft.SqlServer.Management.Smo;
using SmoTable = Microsoft.SqlServer.Management.Smo.Table;
using Table = Lunaverse.DbVerse.Core.DbObjects.Table;

namespace StoryVerse.Database
{
    public partial class Methods : MethodsBase
    {
		public void InitializeDatabase()
		{
			if (Server.HasDatabase(DatabaseName))
			{
				Report.Error("Cannot intiailize database.  A database '{0}' already exists", DatabaseName);
				return;
			}
			CreateDatabase();
		}

		public void ReInitializeDatabase()
		{
			if (Server.HasDatabase(DatabaseName))
			{
				Db.Drop();
			}
			CreateDatabase();
		}

    	private void CreateDatabase()
    	{
    		Server.CreateDatabase(DatabaseName);
    		CreateSchema();
    		ApplyAllUpdates();
    		AddInitialCompanyAndUser();
    	}

    	public void ApplyAllNewUpdates()
        {
            ApplyNewUpdates();
        }

        private Table AddLookupTable(string name)
        {
            return Db.AddTable(name, DataType.Int)
                .AddColumn("Name", DataType.VarChar(30), false)
                .AddColumn("Sort", DataType.Int);
        }
    }
}