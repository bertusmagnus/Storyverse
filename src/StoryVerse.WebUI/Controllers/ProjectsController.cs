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
using StoryVerse.WebUI.ViewComponents;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class ProjectsController : EntityControllerBase<Project, ProjectCriteria, IEntity>
    {
        public ProjectsController() : base(false) { }

        public override void Save([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NewInstanceIfInvalidKey)] Project project)
        {
            project.Company = NullifyEntityIfTransient(project.Company);
            Update(project);
        }

        [Layout("chart")]
        public void Burndown()
        {
            Project project = Project.Find(new Guid(Context.Params["id"]));

            BurndownChartProperties props = 
                new BurndownChartProperties(project.Burndown(project));
            props.Title = "Burndown For " + project.Name;
            PropertyBag["burndownProps"] = props;
        }

        protected override void SetCustomFilterPreset(string presetName)
        {
            switch (presetName)
            {
                case ("my"):
                    Criteria.ApplyPresetMy(CurrentUser);
                    break;
            }
        }

        protected override void SetupEntity(Project project)
        {
            PropertyBag["project"] = project;
            PropertyBag["burndown"] = project.Burndown(project);
        }

        protected override void SetupNewEntity(Project project)
        {
            project.Company = SetValueFromKey<Company>(Form["entity.Company.Id"]);
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

        protected override void PopulateEditSelects(Project project)
        {
            PopulateSelects(false);
        }

        private void PopulateSelects(bool addEmptyCompany)
        {
            IList<Company> companies = new List<Company>();
            Person currentUser = CurrentUser;
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

        public override void Delete([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Project project)
        {
            if (!CurrentUser.IsAdmin)
            {
                HandleEditError(new Exception("You do not permission to delete a project"), project, "NOT deleted");
                return;
            }
            base.Delete(project);
        }
    }
}