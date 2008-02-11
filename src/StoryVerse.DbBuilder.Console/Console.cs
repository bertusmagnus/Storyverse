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

using System.Configuration;
using Lunaverse.DbBuilder.Console;
using Lunaverse.DbBuilder.Core;
using StoryVerse.DbBuilder.Core;

namespace StoryVerse.DbBuilder.Console
{
    public class Console : ConsoleBase
    {
        protected override void GetSettings()
        {
            ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["Admin"];
            connectionString = cs.ConnectionString;
        }

        public override MethodsBase Methods
        {
            get { return methods = methods ?? new Methods(connectionString, this); }
        }
    }
}
