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
    public class IssueChange : BaseEntity<IssueChange>
    {
        private Issue _issue;
        private string _propertName;
        private string _oldValue;
        private string _newValue;
        private Person _changedBy;
        private DateTime _changeDate = DateTime.Now;

        [BelongsTo]
        public Issue Issue
        {
            get { return _issue; }
            set { _issue = value; }
        }

        [Property]
        public string PropertyName
        {
            get { return _propertName; }
            set { _propertName = value; }
        }

        [Property]
        public string OldValue
        {
            get { return _oldValue; }
            set { _oldValue = value; }
        }

        [Property]
        public string NewValue
        {
            get { return _newValue; }
            set { _newValue = value; }
        }

        [BelongsTo]
        public Person ChangedBy
        {
            get { return _changedBy; }
            set { _changedBy = value; }
        }

        [Property]
        public DateTime ChangeDate
        {
            get { return _changeDate; }
            set { _changeDate = value; }
        }

        protected override int GetRelativeValue(IssueChange other)
        {
            switch (SortExpression)
            {
                case "ChangedBy":
                    return (ChangedBy != null) ? ChangedBy.CompareTo(other.ChangedBy) : -1;
                default:
                    return ChangeDate.CompareTo(other.ChangeDate);
            }
        }
    }
}
