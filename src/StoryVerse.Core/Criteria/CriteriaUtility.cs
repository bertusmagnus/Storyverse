/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using NHibernate.Expression;
using StoryVerse.Core.Models;

namespace StoryVerse.Core.Criteria
{
    public class CriteriaUtility
    {
        private const string orderByAlias = "orderByAlias";

        public static void AddOrder(IFindCriteria criteria, DetachedCriteria detCriteria)
        {
            string orderByProp = criteria.OrderBy;
            if (orderByProp.Contains("."))
            {
                string baseName = orderByProp.Remove(orderByProp.LastIndexOf('.'));
                orderByProp = orderByProp.Replace(baseName, orderByAlias);
                detCriteria.CreateAlias(baseName, orderByAlias);
            }
            detCriteria.AddOrder(new Order(orderByProp, criteria.OrderAscending));
        }
    }
}
