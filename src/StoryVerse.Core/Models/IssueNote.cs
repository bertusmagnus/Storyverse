/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class IssueNote : BaseEntity<IssueNote>
    {
        private Issue _issue;
        private string _body;
        private Person _createdBy;
        private DateTime _dateCreated = DateTime.Now;

        [BelongsTo]
        public Issue Issue
        {
            get { return _issue; }
            set { _issue = value; }
        }

        [Property]
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        [BelongsTo]
        public Person CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        [Property]
        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { _dateCreated = value; }
        }

        protected override int GetRelativeValue(IssueNote other)
        {
            switch (SortExpression)
            {
                case "CreatedBy":
                    return (CreatedBy != null) ? CreatedBy.CompareTo(other.CreatedBy) : -1;
                default:
                    return DateCreated.CompareTo(other.DateCreated);
            }
        }
    }
}