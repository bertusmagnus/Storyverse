/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace StoryVerse.WebUI
{
    public interface IWebObjectCache
    {
        void Add<T>(string id, T entity);
        T Retrieve<T>(string id);
        void Remove(string id);
        void Clear();
    }
}
