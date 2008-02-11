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
    public class TaskCriteria : BaseCriteria<Task>
    {
        private Guid?[] iterationIds;
        private Guid?[] ownerIds;
        private TechnicalRisk[] techRisks;
        private TaskStatus[] statuses;
        private string term;
        private int? number;
        //private int? hoursRemainingFrom;
        //private int? hoursRemainingTo;

        public Guid?[] IterationIds
        {
            get { return iterationIds; }
            set { iterationIds = value; }
        }

        public Guid?[] OwnerIds
        {
            get { return ownerIds; }
            set { ownerIds = value; }
        }

        public TechnicalRisk[] TechRisks
        {
            get { return techRisks; }
            set { techRisks = value; }
        }

        public TaskStatus[] Statuses
        {
            get { return statuses; }
            set { statuses = value; }
        }

        public string Term
        {
            get { return term; }
            set { term = value; }
        }

        public string Number
        {
            get { return number.ToString(); }
            set { number = StringToNullableInt(value); }
        }

        //public string HoursRemainingFrom
        //{
        //    get { return hoursRemainingFrom.ToString(); }
        //    set { hoursRemainingFrom = StringToNullableInt(value); }
        //}

        //public string HoursRemainingTo
        //{
        //    get { return hoursRemainingTo.ToString(); }
        //    set { hoursRemainingTo = StringToNullableInt(value); }
        //}

        public void ClearAllCriteria()
        {
            iterationIds = null;
            ownerIds = null;
            techRisks = null;
            term = null;
            number = null;
            statuses = null;
            //hoursRemainingFrom = null;
            //hoursRemainingTo = null;
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
            if (statuses != null && statuses.Length > 0)
            {
                criteria.Add(Expression.In("Status", statuses));
            }
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
            if (ownerIds != null)
            {
                Disjunction orOwners = new Disjunction();
                foreach (Guid id in ownerIds)
                {
                    if (id == Guid.Empty)
                    {
                        orOwners.Add(Expression.IsNull("Owner"));
                    }
                    else
                    {
                        orOwners.Add(Expression.Eq("Owner.Id", id));
                    }
                }
                criteria.Add(orOwners);
            }
            
            if (number.HasValue) criteria.Add(Expression.Eq("Number", number));


            //ToDo: figure out hoursRemaining, etc.
            //if (hoursRemainingFrom.HasValue || hoursRemainingTo.HasValue)
            //{
            //    criteria.CreateAlias("EstimatesList", "e");
            //}
            //if (hoursRemainingFrom.HasValue)
            //{
            //    criteria.SetProjection(Projections.ProjectionList()
            //        .Add(Projections.Max("e.Date"))
            //        .Add(Projections.GroupProperty("e.HoursRemaining")));
            //    criteria.Add(Expression.Ge("e.HoursRemaining", hoursRemainingFrom));
            //}

            //if (hoursRemainingTo.HasValue)
            //{
            //    criteria.SetProjection(Projections.ProjectionList()
            //        .Add(Projections.Max("e.Date"))
            //        .Add(Projections.GroupProperty("e.HoursRemaining")));
            //    criteria.Add(Expression.Le("e.HoursRemaining", hoursRemainingFrom));
            //}

            if (!string.IsNullOrEmpty(term))
            {
                Disjunction orTerm = new Disjunction();
                orTerm.Add(Expression.Like("Title", term, MatchMode.Anywhere));
                orTerm.Add(Expression.Like("Description", term, MatchMode.Anywhere));
                orTerm.Add(Expression.Like("Notes", term, MatchMode.Anywhere));
                criteria.Add(orTerm);
            }
        }

        public override void ApplyPresetAll()
        {
            ClearAllCriteria();
        }

        public void ApplyPresetMy(Person currentUser)
        {
            ClearAllCriteria();
            OwnerIds = new Guid?[] { currentUser.Id };
        }

        public void ApplyPresetMyStarted(Person person)
        {
            ApplyPresetMy(person);
            statuses = new TaskStatus[] { TaskStatus.Started };
        }

        public void ApplyPresetMyNotStarted(Person person)
        {
            ApplyPresetMy(person);
            statuses = new TaskStatus[] { TaskStatus.NotStarted };
        }

        private static int? StringToNullableInt(string value)
        {
            int num;
            if (int.TryParse(value, out num))
            {
                return num;
            }
            else
            {
                return null;
            }
        }
    }
}
