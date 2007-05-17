/* 
 * Copyright � Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using Castle.MonoRail.ActiveRecordSupport;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Criteria;
using StoryVerse.Core.Models;
using Castle.MonoRail.Framework;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class ProjectsController : EntityControllerBase<Project, ProjectCriteria, IEntity>
    {
        public ProjectsController() : base(false) { }

        public override string SortExpression
        {
            get { return Project.SortExpression; }
            set { Project.SortExpression = value; }
        }

        public override SortDirection SortDirection
        {
            get { return Project.SortDirection; }
            set { Project.SortDirection = value; }
        }

        public override void Save([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NewInstanceIfInvalidKey)] Project project)
        {
            project.Company = NullifyIfTransient(project.Company);
            Update(project);
        }

        protected override void SetCustomFilterPreset(string presetName)
        {
            //Important: the default is "my".  This prevents users seeing by default
            //projects for companies that they are not supposed to
            switch (presetName)
            {
                case ("my"):
                default:
                    Criteria.ApplyPresetMy((Person)Context.CurrentUser);
                    break;
            }
        }

        protected override void SetupEntity(Project project)
        {
            PropertyBag["project"] = project;
            int maxHours;
            PropertyBag["burndown"] = project.Burndown(project, out maxHours);
            PropertyBag["maxHours"] = maxHours;
        }

        protected override void SetupNewEntity(Project project)
        {
            project.Company = SetEntityValue<Company>(Form["entity.Company.Id"]);
        }

        protected override void SetupUpdateEntity(Project project)
        {
            //this is needed because monorail will only bind a child collection if finds at least one item in the form
            if (Form["componentCount"] == "0")
            {
                project.ClearComponents();
            }
            foreach (Story item in project.Stories)
            {
                if (!project.Components.Contains(item.Component)) item.Component = null;
            }
        }

        protected override void PopulateFilterSelects()
        {
            PopulateSelects(true);
        }

        protected override void PopulateEditSelects()
        {
            PopulateSelects(false);
        }

        private void PopulateSelects(bool addEmptyCompany)
        {
            IList<Company> companies = new List<Company>();
            Person currentUser = (Person)Context.CurrentUser;
            if (currentUser.ProjectScope == UserProjectScope.All)
            {
                if (addEmptyCompany) companies.Add(new Company());
                foreach (Company company in Company.FindAll())
                {
                    companies.Add(company);
                }
            }
            else
            {
                companies.Add(currentUser.Company);
            }
            PropertyBag["companies"] = companies;
        }

        public override void Delete(Project project)
        {
            if (!((Person)Context.CurrentUser).IsAdmin)
            {
                HandleEditError(new Exception("You do not permission to delete a project"), project, "NOT deleted");
                return;
            }
            base.Delete(project);
        }
    }
}