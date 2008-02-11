/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using NHibernate.Expression;
using StoryVerse.Core.Models;

namespace StoryVerse.Core.Criteria
{
    public class IterationCriteria : BaseCriteria<Iteration>
    {
        protected override void BuildCriteria()
        {
            if (contextEntity != null)
            {
                criteria.Add(Expression.Eq("Project", contextEntity));
            }
        }
    }
}
