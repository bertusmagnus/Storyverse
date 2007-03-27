/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using NHibernate.Expression;
using StoryVerse.Core.Models;

namespace StoryVerse.Core.Criteria
{
    public interface IFindCriteria
    {
        IEntity ContextEntity { get; set; }
        string OrderBy { get; set; }
        bool OrderAscending { get; set; }
        void ApplyPresetAll();
        DetachedCriteria ToDetachedCriteria();
    }
}
