/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.MonoRail.ActiveRecordSupport;
using Lunaverse.Tools.Common;
using StoryVerse.Core.Models;
using Castle.MonoRail.Framework;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Criteria;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class TasksController : EntityControllerBase<Task, TaskCriteria, Project>
    {
        public TasksController() : base(true) { }

        protected override void SetupEntity(Task task)
        {
            PropertyBag["newRemainingHours"] = string.Empty;
        }

        protected override void SetCustomFilterPreset(string presetName)
        {
            switch (presetName)
            {
                case ("my"):
                    Criteria.ApplyPresetMy(CurrentUser);
                    break;
                case ("mystarted"):
                    Criteria.ApplyPresetMyStarted(CurrentUser);
                    break;
                case ("mynotstarted"):
                    Criteria.ApplyPresetMyNotStarted(CurrentUser);
                    break;
            }
        }

        protected override void AddListSummary()
        {
            PropertyBag["totalInitialEstimate"] = GetListSum("InitialEstimateHours");
            PropertyBag["totalLatestEstimate"] = GetListSum("LatestEstimateHours");
        }

        protected override void PopulateEditSelects(Task task)
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
                    Log.Error(ex);
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

                if (CurrentUser.CanViewOnly)
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
                    Log.Error(ex);
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

                if (CurrentUser.CanViewOnly)
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
                    Log.Error(ex);
                    task.AddStories(storyIds);
                    HandleEditError(ex, task, failureMessage);
                }
            }
        }

        public void DeleteEstimate()
        {
            if (!CurrentUser.IsAdmin)
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
            task.Iteration = SetValueFromKey<Iteration>(Form["entity.Iteration.Id"]);
            task.Owner = SetValueFromKey<Person>(Form["entity.Owner.Id"]);
            ContextEntity.AddTask(task);
        }

        protected override void RemoveFromContextEntity(Task task)
        {
            ContextEntity.RemoveTask(task);
        }
    }
}