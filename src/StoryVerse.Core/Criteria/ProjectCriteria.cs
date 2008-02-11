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
    public class ProjectCriteria : BaseCriteria<Project>
    {
        private Guid? companyId;
        private string name;

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

        protected override void BuildCriteria()
        {
            if (companyId != null && companyId != Guid.Empty)
            {
                criteria.Add(Expression.Eq("Company.Id", companyId));
            }
            if (!string.IsNullOrEmpty(name))
            {
                criteria.Add(Expression.Like("Name", name, MatchMode.Anywhere));
            }
        }

        public override void ApplyPresetAll()
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
