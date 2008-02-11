/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Queries;
using System.Collections;
using NHibernate.Expression;

namespace StoryVerse.Core.Lookups
{
    [ActiveRecord]
    public abstract class BaseLookup<T> : ActiveRecordValidationBase<T>, ILookup, IComparable where T : class
    {
        private int _id;
        private string _name;
        private int _sort;

        [PrimaryKey(PrimaryKeyType.Assigned, Access = PropertyAccess.NosetterCamelcaseUnderscore)]
        public int Id
        {
            get { return _id; }
        }

        [Property]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [Property]
        public int Sort
        {
            get { return _sort; }
            set { _sort = value; }
        }

        public static IList<T> GetList()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof (T));
            criteria.AddOrder(new Order("Sort", true));
            IList list = FindAll(typeof (T), criteria);
            List<T> result = new List<T>();
            foreach (T item in list)
            {
                result.Add(item);
            }
            return result;
        }

        public static int GetNextId()
        {
            IActiveRecordQuery<T> query = new ScalarQuery<T>(typeof (int?), QueryLanguage.Hql,
                string.Format("select max(Id) from {0}", typeof (T).Name));
            return ExecuteQuery(query) as int? ?? 0;
        }

        public int CompareTo(object other)
        {
            if (this == other) return 0;
            if (other == null) return 1;
            return Sort.CompareTo(((BaseLookup<T>)other).Sort);
        }
    }
}