/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.ActiveRecord;
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

        protected override void SetCustomPreset(string presetName)
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

        protected override void AddSummary()
        {
            PropertyBag["totalInitialEstimate"] = GetListSum("InitialEstimateHours");
            PropertyBag["totalLatestEstimate"] = GetListSum("LatestEstimateHours");
        }

        protected override void PopulateEditSelects()
        {
            PopulateSelects();
        }

        protected override void PopulateListSelects()
        {
            PopulateSelects();
        }

        private void PopulateSelects()
        {
            ContextEntity.Refresh(); //= Project.Find(ContextEntity.Id);
            PropertyBag["iterations"] = ContextEntity.Iterations;
            PersonCriteria personCriteria = new PersonCriteria();
            personCriteria.ApplyPresetForProject(ContextEntity);
            PropertyBag["owners"] = Person.FindAll(personCriteria.ToDetachedCriteria());
            PropertyBag["techrisks"] = Enum.GetValues(typeof(TechnicalRisk));
            PropertyBag["statuses"] = Enum.GetValues(typeof(TaskStatus));
        }

        protected override void DoCustomEditAction(Task task)
        {
            switch (Form["actionButton"])
            {
                case ("<<"):
                    RemoveStory(task);
                    break;
                case (">>"):
                    AddStory(task);
                    break;
                case ("Update Hours"):
                    UpdateEstimate(task);
                    break;
                default:
                    string targetStoryId = Form["storyToGoToId"];
                    if (!string.IsNullOrEmpty(targetStoryId))
                    {
                        RedirectToAction("../stories/edit", "id=" + targetStoryId);
                    }
                    else
                    {
                        HandleEditError(new Exception("Action not recognized"), task, null);
                    }
                    break;
            }
        }

        private void UpdateEstimate(Task task)
        {
            int hours;
            if (int.TryParse(Form["newRemainingHours"], out hours))
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
                    DoEdit(task, "Estimate updated");
                }
                catch (Exception ex)                
                {
                    task.RemoveEstimate(estimate);
                    HandleEditError(ex, task, "Estimate NOT updated");
                }
            }
            else
            {
                DoEdit(task, "Estimate NOT updated (new hours not specified)");
            }
        }

        public void AddStory(Task task)
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
                    DoEdit(task, resultMessage);
                }
                catch (Exception ex)
                {
                    task.RemoveStories(storyIds);
                    HandleEditError(ex, task, failureMessage);
                }
            }
        }

        public void RemoveStory(Task task)
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
                    DoEdit(task, resultMessage);
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
                ShowError(new Exception("You do not have permission to delete an estimate"));
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