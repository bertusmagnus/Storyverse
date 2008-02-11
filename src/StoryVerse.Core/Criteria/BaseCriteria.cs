/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using NHibernate.Expression;
using StoryVerse.Core.Models;
using System.Reflection;

namespace StoryVerse.Core.Criteria
{
    public abstract class BaseCriteria<T> : IFindCriteria
    {
        private const string orderByAlias = "orderByAlias";
        protected IEntity contextEntity;
        protected string orderBy;
        protected bool orderAscending = true;
        protected DetachedCriteria criteria;

        public IEntity ContextEntity
        {
            get { return contextEntity; }
            set { contextEntity = value; }
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

        public DetachedCriteria ToDetachedCriteria()
        {
            criteria = DetachedCriteria.For(typeof(T));
            BuildCriteria();
            AddOrder();
            return criteria;
        }

        protected abstract void BuildCriteria();

        public void AddOrder()
        {
            if (!string.IsNullOrEmpty(OrderBy))
            {
                if (OrderBy.Contains("."))
                {
                    string baseName = OrderBy.Remove(OrderBy.LastIndexOf('.'));
                    OrderBy = OrderBy.Replace(baseName, orderByAlias);
                    criteria.CreateAlias(baseName, orderByAlias);
                }
                criteria.AddOrder(new Order(OrderBy, OrderAscending));
            }
        }

        public virtual void ApplyPresetAll()
        {
        }

        protected void AddIdsCriteria(string propertyName, Guid?[] ids)
        {
            if (ids != null)
            {
                Disjunction or = new Disjunction();
                foreach (Guid? id in ids)
                {
                    if (id == null)
                    {
                        or.Add(Expression.IsNull(propertyName));
                    }
                    else
                    {
                        or.Add(Expression.Eq(
                            string.Format("{0}.Id", propertyName), id));
                    }
                }
                criteria.Add(or);
            }
        }

        protected void AddIdsCriteria(string propertyName, int?[] ids)
        {
            if (ids != null)
            {
                Disjunction or = new Disjunction();
                foreach (int? id in ids)
                {
                    if (id == null)
                    {
                        or.Add(Expression.IsNull(propertyName));
                    }
                    else
                    {
                        or.Add(Expression.Eq(
                            string.Format("{0}.Id", propertyName), id));
                    }
                }
                criteria.Add(or);
            }
        }

        protected void AddTermCriteria<T>(string term, params string[] propertyNames)
        {
            if (!string.IsNullOrEmpty(term) && propertyNames.Length > 0)
            {
                Disjunction orTerm = new Disjunction();
                foreach (string name in propertyNames)
                {
                    if (name.Contains("."))
                    {
                        string subPropName = name.Remove(name.LastIndexOf('.'));
                        Type subPropType = typeof(T).GetProperty(subPropName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).PropertyType;
                        Type subItemType = subPropType.GetGenericArguments()[0];
                        string propertyName = name.Substring(name.LastIndexOf('.') + 1);
                        DetachedCriteria sub = DetachedCriteria.For(subItemType);
                        sub.SetProjection(Projections.Id())
                            .Add(Expression.InsensitiveLike(propertyName, term, 
                                MatchMode.Anywhere));
                        orTerm.Add(Subqueries.Exists(sub));
                    }
                    else
                    {
                        orTerm.Add(Expression.InsensitiveLike(name, term, MatchMode.Anywhere));
                    }
                }
                criteria.Add(orTerm);
            }
        }
    }
}
