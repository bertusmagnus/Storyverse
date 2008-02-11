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
    public class PersonCriteria : BaseCriteria<Person>
    {
        private Project project;
        private Person excludePerson;

        public Project Project
        {
            get { return project; }
            set { project = value; }
        }

        public Person ExcludePerson
        {
            get { return excludePerson; }
            set { excludePerson = value; }
        }

        protected override void BuildCriteria()
        {
            if (contextEntity != null && typeof(Company).IsAssignableFrom(contextEntity.GetType()))
            {
                criteria.Add(Expression.Eq("Company", contextEntity));
            }
            if (project != null)
            {
                criteria.CreateAlias("Company", "c");
                Disjunction orProject = new Disjunction();
                orProject.Add(Expression.In("c.Type", (IList)Company.GetDeveloperCompanyTypes()));
                orProject.Add(Expression.Eq("Company", project.Company));
                criteria.Add(orProject);
            }
            if (excludePerson != null)
            {
                criteria.Add(new NotExpression(new IdentifierEqExpression(excludePerson.Id)));
            }
        }

        public void ApplyPresetForProject(Project project)
        {
            this.project = project;
        }
    }
}
