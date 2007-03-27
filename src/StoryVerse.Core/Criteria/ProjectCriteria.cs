/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using NHibernate.Expression;
using StoryVerse.Core.Models;
using StoryVerse.Core.Lookups;

namespace StoryVerse.Core.Criteria
{
    public class ProjectCriteria : IFindCriteria
    {
        private Guid? companyId;
        private string name;
        private string orderBy;
        private bool orderAscending = true;

        public IEntity ContextEntity
        {
            get { return null; }
            set { }
        }

        public Guid? CompanyId
        {
            get { return companyId; }
            set { companyId = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
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
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Project));
            if (companyId != null && companyId != Guid.Empty)
                criteria.Add(Expression.Eq("Company.Id", companyId));
            if (!string.IsNullOrEmpty(name))
                criteria.Add(Expression.Like("Name", name, MatchMode.Anywhere));
            if (!string.IsNullOrEmpty(orderBy))
                CriteriaUtility.AddOrder(this, criteria);
            return criteria;
        }

        public void ApplyPresetAll()
        {
            companyId = null;
            name = null;
        }

        public void ApplyPresetMy(Person currentUser)
        {
            ApplyPresetAll();
            switch (currentUser.ProjectScope)
            {
                case (UserProjectScope.All):
                    break;
                case (UserProjectScope.MyCompany):
                    CompanyId = currentUser.Company.Id;
                    break;
            }
        }
    }
}
