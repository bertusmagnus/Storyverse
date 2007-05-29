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

using Lunaverse.DbBuilder.Console;
using Lunaverse.DbBuilder.Core;
using StoryVerse.DbBuilder.Console.Properties;
using StoryVerse.DbBuilder.Core;

namespace StoryVerse.DbBuilder.Console
{
    public class Console : ConsoleBase
    {
        protected override void GetSettings()
        {
            serverName = Settings.Default.Server;
            databaseName = Settings.Default.Database;
            username = Settings.Default.Username;
            password = Settings.Default.Password;
        }

        public override MethodsBase Methods
        {
            get { return new Methods(serverName, databaseName, username, password, this); }
        }
    }
}
