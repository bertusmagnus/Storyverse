/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using NHibernate.Expression;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Models;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework;
using StoryVerse.Core.Criteria;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("admin"), Rescue("generalerror")]
    public class PersonsController : EntityControllerBase<Person, PersonCriteria, Company>
    {
        public PersonsController() : base(false) { }

        public override string SortExpression
        {
            get { return Person.SortExpression; }
            set { Person.SortExpression = value; }
        }

        public override SortDirection SortDirection
        {
            get { return Person.SortDirection; }
            set { Person.SortDirection = value; }
        }

        protected override void SetupNewEntity(Person person)
        {
            ContextEntity.AddEmployee(person);
        }
    }
}