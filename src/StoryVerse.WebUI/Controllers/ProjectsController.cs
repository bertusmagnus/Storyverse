/* 
 * Copyright © Lunaverse Software 2007  
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

        [Layout("edit")]
        public override void Edit([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NewInstanceIfInvalidKey)] Project project)
        {
            project.Company = NullifyIfTransient(project.Company);
            DoEditAction(project);
        }

        protected override void SetCustomPreset(string presetName)
        {
            switch (presetName)
            {
                case ("my"):
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

        protected override void PopulateListSelects()
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

        protected override void Delete(Project project)
        {
            if (!((Person)Context.CurrentUser).IsAdmin)
            {
                HandleEditError(new Exception("You do not permission to delete a project"), project, "NOT deleted");
                return;
            }
            base.Delete(project);
        }

        //public void AddComponent([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Project project)
        //{
        //    if (((Person) Context.CurrentUser).CanViewOnly)
        //    {
        //        PropertyBag["componentUpdateResult"] = "You do not have permission to add a component";
        //    }
        //    else
        //    {
        //        Component component = new Component();
        //        try
        //        {
        //            component.Name = Form["newComponentName"];
        //            project.AddComponent(component);
        //            project.Validate();
        //            project.UpdateAndFlush();
        //            PropertyBag["componentUpdateResult"] = "Component added: " + component.Name;
        //        }
        //        catch (Exception ex)
        //        {
        //            project.RemoveComponent(component);
        //            PropertyBag["componentUpdateResult"] = "Failed to add component:<br/>" + GetErrorMessage(ex);
        //        }
        //    }
        //    SetupViewComponents(project);
        //}

        //public void DeleteComponent()
        //{
        //    Project project = Project.Find(new Guid(Context.Params["projectId"]));
        //    if (((Person)Context.CurrentUser).CanViewOnly)
        //    {
        //        PropertyBag["componentUpdateResult"] = "You do not have permission to delete a component";
        //    }
        //    try
        //    {
        //        project.RemoveComponent(new Guid(Context.Params["componentId"]));
        //        project.Validate();
        //        project.UpdateAndFlush();
        //        PropertyBag["componentUpdateResult"] = "Component deleted";
        //    }
        //    catch (Exception ex)
        //    {
        //        if (project != null) project.Refresh();
        //        PropertyBag["componentUpdateResult"] = "Failed to delete component:<br/>" + GetErrorMessage(ex);
        //    }
        //    SetupViewComponents(project);
        //}

        //private void SetupViewComponents(Project project)
        //{
        //    PropertyBag["entity"] = project;
        //    PropertyBag["newComponentName"] = string.Empty;
        //    SetViewContext();
        //    RenderView("_components", true);
        //}
    }
}