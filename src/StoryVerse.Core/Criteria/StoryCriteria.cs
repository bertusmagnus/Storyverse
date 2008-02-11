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
    public class StoryCriteria : BaseCriteria<Story>
    {
        private Guid?[] iterationIds;
        private StoryPriority[] priorities;
        private StoryStatus[] statuses;
        private TechnicalRisk[] techRisks;
        private Guid?[] componentIds;
        private string term;
        private int? number;

        public Guid?[] IterationIds
        {
            get { return iterationIds; }
            set { iterationIds = value; }
        }

        public Guid?[] ComponentIds
        {
            get { return componentIds; }
            set { componentIds = value; }
        }

        public StoryPriority[] Priorities
        {
            get { return priorities; }
            set { priorities = value; }
        }

        public StoryStatus[] Statuses
        {
            get { return statuses; }
            set { statuses = value; }
        }

        public TechnicalRisk[] TechRisks
        {
            get { return techRisks; }
            set { techRisks = value; }
        }

        public string Term
        {
            get { return term; }
            set { term = value; }
        }

        public string Number
        {
            get { return number.ToString(); }
            set
            {
                int num;
                if (int.TryParse(value, out num))
                {
                    number = num;
                }
                else
                {
                    number = null;
                }
            }
        }

        protected override void BuildCriteria()
        {
            if (contextEntity != null)
            {
                criteria.Add(Expression.Eq("Project", contextEntity));
            }
            if (techRisks != null && techRisks.Length > 0)
            {
                criteria.Add(Expression.In("TechnicalRisk", techRisks));
            }
            if (priorities != null && priorities.Length > 0)
            {
                criteria.Add(Expression.In("Priority", priorities));
            }
            if (statuses != null && statuses.Length > 0)
            {
                criteria.Add(Expression.In("Status", statuses));
            }
            
            AddIdsCriteria("Iteration", iterationIds);
            
            AddIdsCriteria("Component", componentIds);

            if (number.HasValue)
            {
                criteria.Add(Expression.Eq("Number", number));
            }

            AddTermCriteria<Story>(term, "Title", "Body", "Notes");
        }

        public override void ApplyPresetAll()
        {
            iterationIds = null;
            componentIds = null;
            priorities = null;
            statuses = null;
            techRisks = null;
            term = null;
            number = null;
        }
    }
}
