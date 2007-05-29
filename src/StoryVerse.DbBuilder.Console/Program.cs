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

namespace StoryVerse.DbBuilder.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console console = new Console();
            console.RunProgram(args);
        }
    }
}
