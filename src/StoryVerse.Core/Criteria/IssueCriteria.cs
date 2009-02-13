/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using NHibernate.Expression;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Models;
using System;

namespace StoryVerse.Core.Criteria
{
    public class IssueCriteria : BaseCriteria<Issue>
    {
        private int? number;
        private IssueType[] types;
        private string term;
        private bool termInTitleOnly;
        private IssueStatus[] statuses;
        private int?[] priorityIds;
        private int?[] severityIds;
        private int?[] dispositionIds;
        private Guid?[] componentIds;
        private Guid?[] ownerIds;

        public int? Number
        {
            get { return number; }
            set { number = value; }
        }

        public IssueType[] Types
        {
            get { return types; }
            set { types = value; }
        }

        public string Term
        {
            get { return term; }
            set { term = value; }
        }

        public bool TermInTitleOnly
        {
            get { return termInTitleOnly; }
            set { termInTitleOnly = value; }
        }

        public IssueStatus[] Statuses
        {
            get { return statuses; }
            set { statuses = value; }
        }

        public int?[] PriorityIds
        {
            get { return priorityIds; }
            set { priorityIds = value; }
        }

        public int?[] SeverityIds
        {
            get { return severityIds; }
            set { severityIds = value; }
        }

        public int?[] DispositionIds
        {
            get { return dispositionIds; }
            set { dispositionIds = value; }
        }

        public Guid?[] OwnerIds
        {
            get { return ownerIds; }
            set { ownerIds = value; }
        }

        public Guid?[] ComponentIds
        {
            get { return componentIds; }
            set { componentIds = value; }
        }

        protected override void BuildCriteria()
        {
            if (contextEntity != null)
            {
                criteria.Add(Expression.Eq("Project", contextEntity));
            }
            if (types != null && types.Length > 0)
            {
                criteria.Add(Expression.In("Type", types));
            }
            if (statuses != null && statuses.Length > 0)
            {
                criteria.Add(Expression.In("Status", statuses));
            }
            if (number.HasValue)
            {
                criteria.Add(Expression.Eq("Number", number));
            }

            AddIdsCriteria("Component", componentIds);

            AddIdsCriteria("Priority", priorityIds);

            AddIdsCriteria("Severity", severityIds);

            AddIdsCriteria("Disposition", dispositionIds);

            AddIdsCriteria("Owner", ownerIds);

            if (termInTitleOnly)
            {
                AddTermCriteria<Issue>(term, "Title");
            }
            else 
            {
                AddTermCriteria<Issue>(term, "Title", "Description", "NotesList.Body");
            }
        }

        public override void ApplyPresetAll()
        {
            number = null;
            types = null;
            term = null;
            statuses = null;
            priorityIds = null;
            severityIds = null;
            dispositionIds = null;
            componentIds = null;
            ownerIds = null;
        }

        public void ApplyPresetMy(Person user)
        {
            ApplyPresetAll();
            OwnerIds = new Guid?[] { user.Id };
        }

        public void ApplyPresetMyOpen(Person user)
        {
            ApplyPresetMy(user);
            statuses = Issue.OpenStatues;
        }

		public void ApplyPresetMyOpenDefects(Person user)
		{
			ApplyPresetMy(user);
			statuses = Issue.OpenStatues;
			types = new IssueType[] { IssueType.Defect };
		}

		public void ApplyPresetAllOpenDefects()
		{
			ApplyPresetAll();
			statuses = Issue.OpenStatues;
			types = new IssueType[] { IssueType.Defect };
		}
    }
}
