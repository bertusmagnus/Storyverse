/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.ActiveRecord;
using StoryVerse.Core.Validators;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class TaskEstimate : BaseEntity<TaskEstimate>
    {
        private int _hoursRemaining;
        private DateTime _date;
        private Person _createdBy;

        [Property, ValidateNumberIsPositive("Hours Remaining cannot be negative")]
        public int HoursRemaining
        {
            get { return _hoursRemaining; }
            set { _hoursRemaining = value; }
        }
        [Property]
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        [BelongsTo]
        public Person CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        protected override int GetRelativeValue(TaskEstimate other)
        {
            switch (SortExpression)
            {
                case "HoursRemaining":
                    return HoursRemaining.CompareTo(other.HoursRemaining);
                case "Date":
                    return Date.CompareTo(other.Date);
                case "CreatedBy":
                    return (CreatedBy != null) ? CreatedBy.LastName.CompareTo(other.CreatedBy.LastName) : -1;
                default:
                    return Id.CompareTo(other.Id);
            }
        } 
    }
}