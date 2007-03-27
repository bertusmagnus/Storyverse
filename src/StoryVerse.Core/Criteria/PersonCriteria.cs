/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System.Collections;
using NHibernate.Expression;
using StoryVerse.Core.Models;

namespace StoryVerse.Core.Criteria
{
    public class PersonCriteria : IFindCriteria
    {
        private IEntity company;
        private Project project;
        private string orderBy;
        private bool orderAscending = true;

        public IEntity ContextEntity
        {
            get { return company; }
            set { company = value; }
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
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Person));
            if (company != null)
                criteria.Add(Expression.Eq("Company", company));
            if (project != null)
            {
                criteria.CreateAlias("Company", "c");
                Disjunction orProject = new Disjunction();
                orProject.Add(Expression.In("c.Type", (IList)Company.GetDeveloperCompanyTypes()));
                orProject.Add(Expression.Eq("Company", project.Company));
                criteria.Add(orProject);
            }
            if (!string.IsNullOrEmpty(orderBy))
                CriteriaUtility.AddOrder(this, criteria);
            return criteria;
        }

        public void ApplyPresetAll()
        {
        }

        public void ApplyPresetForProject(Project project)
        {
            this.project = project;
        }
    }
}
