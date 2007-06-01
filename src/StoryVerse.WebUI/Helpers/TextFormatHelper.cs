/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.MonoRail.Framework.Helpers;

namespace StoryVerse.Helpers
{
    public class TextFormatHelper : AbstractHelper
    {
        public string AddLineBreaks(string value)
        {
            return LineBreaksToHtml(value);
        }
    }
}
