/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using NHibernate.Expression;
using StoryVerse.Core.Models;

namespace StoryVerse.Core.Criteria
{
    public class CompanyCriteria : IFindCriteria
    {
        private string orderBy;
        private bool orderAscending = true;

        public IEntity ContextEntity
        {
            get { return null; }
            set { }
        }

        public string OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }

        public bool OrderAscending
        {
            get { return orderAscending; }
            set { orderAscending = value; }
        }

        public DetachedCriteria ToDetachedCriteria()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Company));
            if (!string.IsNullOrEmpty(orderBy))
                CriteriaUtility.AddOrder(this, criteria);
            return criteria;
        }

        public void ApplyPresetAll()
        {
        }
    }
}
