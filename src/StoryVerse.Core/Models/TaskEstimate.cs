/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using StoryVerse.Attributes;
using Castle.ActiveRecord;
using StoryVerse.Core.Lookups;

namespace StoryVerse.Core.Models
{
    [ActiveRecord()]
    public class TaskEstimate : ActiveRecordValidationBase<TaskEstimate>, IEntity, IComparable<TaskEstimate>
    {
        private Guid _id;
        private int _hoursRemaining;
        private DateTime _date;
        private Person _createdBy;
        private Task _task;

        [PrimaryKey(PrimaryKeyType.GuidComb, Access = PropertyAccess.NosetterCamelcaseUnderscore)]
        public Guid Id
        {
            get { return _id; }
        }
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
        [BelongsTo()]
        public Person CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        //public string GetBar(int ceiling, int maxHours)
        //{
        //    if (maxHours == 0) return string.Empty;
        //    decimal factor = (decimal)maxHours/(decimal)ceiling;
        //    return new string('|', (int)Math.Round(HoursRemaining/factor, 0));
        //}

        public void Validate()
        {
        }

        #region Sorting Members
        public int CompareTo(TaskEstimate other)
        {
            int relativeValue;
            switch (Task.EstimatesSortExpression)
            {
                case "Id":
                    relativeValue = Id.CompareTo(other.Id);
                    break;
                case "HoursRemaining":
                    relativeValue = (HoursRemaining != null) ? HoursRemaining.CompareTo(other.HoursRemaining) : -1;
                    break;
                case "Date":
                    relativeValue = (Date != null) ? Date.CompareTo(other.Date) : -1;
                    break;
                case "CreatedBy":
                    relativeValue = (CreatedBy != null) ? CreatedBy.LastName.CompareTo(other.CreatedBy.LastName) : -1;
                    break;
                default:
                    relativeValue = 0;
                    break;
            }
            if (Task.EstimatesSortDirection  == SortDirection.Descending)
            {
                relativeValue *= -1;
            }
            return relativeValue;
        } 
        #endregion
    }
}