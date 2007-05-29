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

namespace StoryVerse.DbBuilder.Core
{
    public partial class Methods : MethodsBase
    {
        public Methods(string ServerName, string DatabaseName, string Username, string Password,
                                IAcceptOutput OutputReceiver)
            : base(ServerName, DatabaseName, Username, Password, OutputReceiver)
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
    }
}