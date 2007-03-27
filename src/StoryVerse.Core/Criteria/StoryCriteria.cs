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
    public class StoryCriteria : IFindCriteria
    {
        private IEntity project;
        private Guid?[] iterationIds;
        private StoryPriority[] priorities;
        private StoryStatus[] statuses;
        private TechnicalRisk[] techRisks;
        private Guid?[] componentIds;
        private string term;
        private int? number;
        private string orderBy;
        private bool orderAscending = true;

        public IEntity ContextEntity
        {
            get { return project; }
            set { project = value; }
        }

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

        public DetachedCriteria ToDetachedCriteria()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Story));
            if (project != null)
                criteria.Add(Expression.Eq("Project", project));
            if (techRisks != null && techRisks.Length > 0)
                criteria.Add(Expression.In("TechnicalRisk", techRisks));
            if (priorities != null && priorities.Length > 0)
                criteria.Add(Expression.In("Priority", priorities));
            if (statuses != null && statuses.Length > 0)
                criteria.Add(Expression.In("Status", statuses));
            if (iterationIds != null)
            {
                Disjunction orIterations = new Disjunction();
                foreach (Guid? id in iterationIds)
                {
                    if (id == null)
                    {
                        orIterations.Add(Expression.IsNull("Iteration"));
                    }
                    else
                    {
                        orIterations.Add(Expression.Eq("Iteration.Id", id));
                    }
                }
                criteria.Add(orIterations);
            }
            if (componentIds != null)
            {
                Disjunction orComponents = new Disjunction();
                foreach (Guid? id in componentIds)
                {
                    if (id == null)
                    {
                        orComponents.Add(Expression.IsNull("Component"));
                    }
                    else
                    {
                        orComponents.Add(Expression.Eq("Component.Id", id));
                    }
                }
                criteria.Add(orComponents);
            }
            if (number.HasValue)
                criteria.Add(Expression.Eq("Number", number));
            if (!string.IsNullOrEmpty(term))
            {
                Disjunction orTerm = new Disjunction();
                orTerm.Add(Expression.Like("Title", term, MatchMode.Anywhere));
                orTerm.Add(Expression.Like("Body", term, MatchMode.Anywhere));
                orTerm.Add(Expression.Like("Notes", term, MatchMode.Anywhere));
                criteria.Add(orTerm);
            }
            if (!string.IsNullOrEmpty(orderBy))
                CriteriaUtility.AddOrder(this, criteria);
            return criteria;
        }

        public void ApplyPresetAll()
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
