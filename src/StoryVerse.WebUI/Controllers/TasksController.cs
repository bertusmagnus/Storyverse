/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.ActiveRecord;
using Castle.MonoRail.ActiveRecordSupport;
using StoryVerse.Core.Models;
using Castle.MonoRail.Framework;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Criteria;
using System.Collections.Generic;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class TasksController : EntityControllerBase<Task, TaskCriteria, Project>
    {
        public TasksController() : base(true) { }

        public override string SortExpression
        {
            get { return Task.SortExpression; }
            set { Task.SortExpression = value; }
        }

        public override SortDirection SortDirection
        {
            get { return Task.SortDirection; }
            set { Task.SortDirection = value; }
        }

        protected override void SetupEntity(Task task)
        {
            PropertyBag["newRemainingHours"] = string.Empty;
        }

        protected override void SetCustomFilterPreset(string presetName)
        {
            switch (presetName)
            {
                case ("my"):
                    Criteria.ApplyPresetMy((Person)Context.CurrentUser);
                    break;
                case ("mystarted"):
                    Criteria.ApplyPresetMyStarted((Person)Context.CurrentUser);
                    break;
                case ("mynotstarted"):
                    Criteria.ApplyPresetMyNotStarted((Person)Context.CurrentUser);
                    break;
            }
        }

        protected override void AddListSummary()
        {
            PropertyBag["totalInitialEstimate"] = GetListSum("InitialEstimateHours");
            PropertyBag["totalLatestEstimate"] = GetListSum("LatestEstimateHours");
        }

        protected override void PopulateEditSelects()
        {
            PopulateSelects();
        }

        protected override void PopulateFilterSelects()
        {
            PopulateSelects();
        }

        private void PopulateSelects()
        {
            ContextEntity.Refresh();
            PropertyBag["iterations"] = ContextEntity.Iterations;
            PersonCriteria personCriteria = new PersonCriteria();
            personCriteria.ApplyPresetForProject(ContextEntity);
            PropertyBag["owners"] = Person.FindAll(personCriteria.ToDetachedCriteria());
            PropertyBag["techrisks"] = Enum.GetValues(typeof(TechnicalRisk));
            PropertyBag["statuses"] = Enum.GetValues(typeof(TaskStatus));
        }

        public void GoToStory()
        {
            RedirectToAction("../stories/edit", "id=" +  Form["storyToGoToId"]);
        }

        public void UpdateEstimate([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Task task)
        {
            int hours;
            if (!int.TryParse(Form["newRemainingHours"], out hours))
            {
                RedirectToEdit(task.Id, "Estimate NOT updated (new hours not specified)");
            }
            else
            {
                TaskEstimate estimate = new TaskEstimate();
                estimate.HoursRemaining = hours;
                estimate.Date = DateTime.Now;
                estimate.CreatedBy = (Person) Context.CurrentUser;
                try
                {
                    task.AddEstimate(estimate);
                    task.Validate();
                    task.UpdateAndFlush();
                    RedirectToEdit(task.Id, "Estimate updated");
                }
                catch (Exception ex)                
                {
                    task.RemoveEstimate(estimate);
                    HandleEditError(ex, task, "Estimate NOT updated");
                }
            }
        }

        public void AddStory([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Task task)
        {
            if (Form["storiesToAdd"] != null)
            {
                string[] storyIds = Form["storiesToAdd"].Split(',');
                string resultNoun = storyIds.Length == 1 ? "story" : "stories";
                string resultMessage = storyIds.Length == 0
                                           ? "No stories selected to add"
                                           : storyIds.Length + " " + resultNoun + " added";
                string failureMessage = string.Format("{0} NOT added", resultNoun);

                if (((Person)Context.CurrentUser).CanViewOnly)
                {
                    HandleEditError(new Exception("You do not have permission to add a story to a task"), task, failureMessage);
                    return;
                }

                try
                {
                    task.AddStories(storyIds);
                    task.Validate();
                    task.UpdateAndFlush();
                    RedirectToEdit(task.Id, resultMessage);
                }
                catch (Exception ex)
                {
                    task.RemoveStories(storyIds);
                    HandleEditError(ex, task, failureMessage);
                }
            }
        }

        public void RemoveStory([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Task task)
        {
            if (Form["storiesToRemove"] != null)
            {
                string[] storyIds = Form["storiesToRemove"].Split(',');
                string resultNoun = storyIds.Length == 1 ? "story" : "stories";
                string resultMessage = storyIds.Length == 0
                                           ? "No stories selected to remove"
                                           : storyIds.Length + " " + resultNoun + " removed";
                string failureMessage = string.Format("{0} NOT removed", resultNoun);

                if (((Person)Context.CurrentUser).CanViewOnly)
                {
                    HandleEditError(new Exception("You do not have permission to remove a story from a task"), task, failureMessage);
                    return;
                }

                try
                {
                    task.RemoveStories(storyIds);
                    task.Validate();
                    task.UpdateAndFlush();
                    RedirectToEdit(task.Id, resultMessage);
                }
                catch (Exception ex)
                {
                    task.AddStories(storyIds);
                    HandleEditError(ex, task, failureMessage);
                }
            }
        }

        public void DeleteEstimate()
        {
            if (!((Person)Context.CurrentUser).IsAdmin)
            {
                SetError(new Exception("You do not have permission to delete an estimate"));
            }
            else
            {
                TaskEstimate estimate = TaskEstimate.Find(new Guid(Context.Params["estimateId"]));
                estimate.DeleteAndFlush();
            }
            CancelView();
        }

        protected override void SetupNewEntity(Task task)
        {
            task.Iteration = SetEntityValue<Iteration>(Form["entity.Iteration.Id"]);
            task.Owner = SetEntityValue<Person>(Form["entity.Owner.Id"]);
            ContextEntity.Refresh();
            ContextEntity.AddTask(task);
            task.Number = ContextEntity.GetNextTaskNumber();
        }
    }
}