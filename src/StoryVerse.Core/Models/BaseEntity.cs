/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.ActiveRecord;
using StoryVerse.Core.Lookups;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public abstract class BaseEntity<T> : ActiveRecordValidationBase<T>, IEntity, IComparable<T> where T : class
    {
        private Guid _id;
        protected static string _sortExpression;
        protected static SortDirection _sortDirection = SortDirection.Ascending;

        [PrimaryKey(PrimaryKeyType.GuidComb, Access = PropertyAccess.NosetterCamelcaseUnderscore)]
        public Guid Id
        {
            get { return _id; }
        }

        public virtual void Validate()
        {
        }

        public bool IsTransient
        {
            get { return _id == Guid.Empty; }
        }

        public static string SortExpression
        {
            get { return _sortExpression; }
            set { _sortExpression = value; }
        }

        public static SortDirection SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; }
        }

        public int CompareTo(T other)
        {
            if (this == other) return 0;
            if (other == null) return 1;

            int relativeValue = GetRelativeValue(other);
            if (SortDirection == SortDirection.Descending)
            {
                return relativeValue * -1;
            }
            return relativeValue;
        }

        protected abstract int GetRelativeValue(T other);
    }
}