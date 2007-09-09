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

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class StoriesController : EntityControllerBase<Story, StoryCriteria, Project>
    {
        public StoriesController() : base(true) { }

        public override string SortExpression
        {
            get { return Story.SortExpression; }
            set { Story.SortExpression = value; }
        }

        public override SortDirection SortDirection
        {
            get { return Story.SortDirection; }
            set { Story.SortDirection = value; }
        }

        [Layout("edit")]
        public override void Save([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NewInstanceIfInvalidKey)] Story story)
        {
            story.Component = NullifyIfTransient(story.Component);
            story.Iteration = NullifyIfTransient(story.Iteration);
            foreach (Test test in story.Tests)
            {
                if (test.Number == 0) test.Number = story.GetNextTestNumber();
            }
            Update(story);
        }

        protected override void SetupEntity(Story story)
        {
            PropertyBag["newTestBody"] = string.Empty;
        }

        protected override void SetupUpdateEntity(Story project)
        {
            //this is needed because monorail will only bind a child collection if finds at least one item in the form
            if (Form["testCount"] == "0")
            {
                project.ClearTests();
            }
        }

        [Layout("report")]
        public void Report()
        {
            try
            {
                PropertyBag[_contextEntityName] = ContextEntity;
                PropertyBag["currentDate"] = DateTime.Now.ToString("D");
                PropertyBag[EntityListName] = Story.FindAll(Criteria.ToDetachedCriteria());
            }
            catch (Exception ex)
            {
                HandleListError(ex);
            }
        }

        [Layout("report")]
        public void Printable(Guid id)
        {
            try
            {
                PropertyBag["entity"] = Story.Find(id) ;
            }
            catch (Exception ex)
            {
                SetError(ex);
            }
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
            PropertyBag["iterations"] = ContextEntity.Iterations;
            PropertyBag["priorities"] = Enum.GetValues(typeof(StoryPriority));
            PropertyBag["statuses"] = Enum.GetValues(typeof(StoryStatus));
            PropertyBag["techrisks"] = Enum.GetValues(typeof(TechnicalRisk));
            PropertyBag["components"] = ContextEntity.Components;
        }

        public void GoToTask()
        {
            RedirectToAction("../tasks/edit", "id=" + Form["taskToGoToId"]);
        }

        protected override void AddListSummary()
        {
            PropertyBag["totalEstimateFiftyPercent"] = GetListSum("EstimateFiftyPercent");
            PropertyBag["totalEstimateNinetyPercent"] = GetListSum("EstimateNinetyPercent");
        }

        public void AddTask([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Story story)
        {
            if (Form["tasksToAdd"] != null)
            {
                string[] taskIds = Form["tasksToAdd"].Split(',');
                string resultNoun = taskIds.Length == 1 ? "task" : "tasks";
                string resultMessage = taskIds.Length == 0
                                           ? "No tasks selected to add"
                                           : taskIds.Length + " " + resultNoun + " added";
                string failureMessage = string.Format("{0} NOT added", resultNoun);

                if (((Person)Context.CurrentUser).CanViewOnly)
                {
                    HandleEditError(new Exception("You do not have permission to add a task to a story"), story, failureMessage);
                    return;
                }

                try
                {
                    story.AddTasks(taskIds);
                    story.Validate();
                    story.UpdateAndFlush();
                    RedirectToEdit(story.Id, resultMessage);
                }
                catch (ValidationException ex)
                {
                    story.RemoveTasks(taskIds);
                    HandleEditError(ex, story, failureMessage);
                }
            }
        }

        public void RemoveTask([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Story story)
        {
            if (Form["tasksToRemove"] != null)
            {
                string[] taskIds = Form["tasksToRemove"].Split(',');
                string resultNoun = taskIds.Length == 1 ? "task" : "tasks";
                string resultMessage = taskIds.Length == 0
                                           ? "No tasks selected to remove"
                                           : taskIds.Length + " " + resultNoun + " removed";
                string failureMessage = string.Format("{0} NOT removed", resultNoun);

                if (((Person)Context.CurrentUser).CanViewOnly)
                {
                    HandleEditError(new Exception("You do not have permission to remove a task from a story"), story, failureMessage);
                    return;
                }

                try
                {
                    story.RemoveTasks(taskIds);
                    story.Validate();
                    story.UpdateAndFlush();
                    RedirectToEdit(story.Id, resultMessage);
                }
                catch (Exception ex)
                {
                    story.AddTasks(taskIds);
                    HandleEditError(ex, story, failureMessage);
                }
            }
        }

        public void NewTask([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Story story)
        {
            if (((Person)Context.CurrentUser).CanViewOnly)
            {
                HandleEditError(new Exception("You do not have permission to add a new task"), story, "Task NOT created");
                return;
            }

            Task task = new Task();
            try
            {
                task.Iteration = story.Iteration;
                task.Title = "[new task]";
                task.AddStory(story);
                RefreshContextEntity();
                ContextEntity.AddTask(task);
                task.Number = ContextEntity.GetNextTaskNumber();
                ContextEntity.Validate();
                ContextEntity.UpdateAndFlush();
                RedirectToAction("../tasks/edit", "id=" + task.Id);
            }
            catch (Exception ex)
            {
                ContextEntity.RemoveTask(task);
                HandleEditError(ex, story, "Task NOT created");
            }
        }

        public void AddTest([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] Story story)
        {
            if (((Person)Context.CurrentUser).CanViewOnly)
            {
                HandleEditError(new Exception("You do not have permission to add a new test"), story, "Test NOT added");
                return;
            }

            Test test = new Test();
            try
            {
                test.Body = Form["newTestBody"];
                test.Number = story.GetNextTestNumber();
                story.AddTest(test);
                story.Validate();
                story.UpdateAndFlush();
                RedirectToEdit(story.Id, "Test added");
            }
            catch (Exception ex)
            {
                story.RemoveTest(test);
                HandleEditError(ex, story, "Test NOT added");
            }
        }

        public void DeleteTest()
        {
            if (((Person)Context.CurrentUser).CanViewOnly)
            {
                SetError(new Exception("You do not have permission to delete a test"));
            }
            else
            {
                Test test = Test.Find(new Guid(Context.Params["testId"]));
                test.DeleteAndFlush();
            }
            RenderView("_tests", true);
        }

        protected override void SetupNewEntity(Story story)
        {
            story.Iteration = SetEntityValue<Iteration>(Form["entity.Iteration.Id"]);
            story.Component = SetEntityValue<Component>(Form["entity.Component.Id"]);
            ContextEntity.AddStory(story);
            story.Number = ContextEntity.GetNextStoryNumber();
        }
    }
}