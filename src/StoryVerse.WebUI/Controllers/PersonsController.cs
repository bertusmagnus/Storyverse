/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
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

        public override  void Save([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NullIfInvalidKey)] Person person)
        {
            if (person.Password != Form["passwordConfirm"])
            {
                Context.Response.StatusCode = 500;
                RenderText("Person NOT saved.  Passwords do not match");
                return;
            }
            Update(person);
        }

        protected override void SetupEntity(Person person)
        {
            PropertyBag["passwordConfirm"] = person.Password;
            person.InitUserPreferences();
            if (person.UserPreferences.Id == Guid.Empty)
            {
                person.UserPreferences.SaveAndFlush();
            }
        }

        protected override void SetupNewEntity(Person person)
        {
			ContextEntity = GetContextEntity(ContextEntity.Id);
			ContextEntity.AddEmployee(person);
        }

        [Layout("edit")]
        public void EditUserPrefs()
        {
            ContextEntity = ((Person) Context.CurrentUser).Company;
            RedirectToAction("edit", "id=" + CurrentUser.Id);
        }

        protected override bool GetUserCanEdit(Person person)
        {
            Person user = (Person) Context.CurrentUser;
            return user.IsAdmin || person.Id == user.Id;
        }

        protected override bool DeleteEditButtonVisible
        {
            get { return CurrentUser.IsAdmin; }
        }

        protected override bool ListEditButtonVisible
        {
            get { return CurrentUser.IsAdmin; }
        }

        protected override void RemoveFromContextEntity(Person person)
        {
            ContextEntity.RemoveEmployee(person);
        }
    }
}