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

using Lunaverse.DbBuilder.Core;
using Microsoft.SqlServer.Management.Smo;

namespace StoryVerse.DbBuilder.Core
{
    public partial class Methods : MethodsBase
    {
        public Methods(string connectionString, IAcceptOutput outputReceiver)
            : base(connectionString, outputReceiver)
        {
        }

        public void ClearDatabase()
        {
            Du.ClearDatabase();
        }

        public void ApplyAllNewUpdates()
        {
            ApplyNewUpdates();
        }

        private void AddLookupTable(string name)
        {
            Du.AddTable(name, "Id", DataType.Int, false)
                .AddColumn("Name", DataType.VarChar(30), false)
                .AddColumn("Sort", DataType.Int);
        }
    }
}