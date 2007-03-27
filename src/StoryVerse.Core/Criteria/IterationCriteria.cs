/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using NHibernate.Expression;
using StoryVerse.Core.Models;

namespace StoryVerse.Core.Criteria
{
    public class IterationCriteria : IFindCriteria
    {
        private IEntity project;
        private string orderBy;
        private bool orderAscending = true;

        public IEntity ContextEntity
        {
            get { return project; }
            set { project = value; }
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
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Iteration));
            if (project != null)
                criteria.Add(Expression.Eq("Project", project));
            if (!string.IsNullOrEmpty(orderBy))
                CriteriaUtility.AddOrder(this, criteria);
            return criteria;
        }

        public void ApplyPresetAll()
        {
        }
    }
}
