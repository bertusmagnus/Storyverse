/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using System.Collections;
using NHibernate.Mapping;
using StoryVerse.Core.Lookups;

namespace StoryVerse.Core.Models
{
    [ActiveRecord()]
    public class Project : ActiveRecordValidationBase<Project>, IEntity, IComparable<Project>
    {
        private Guid _id;
        private string _name;
        private Company _company;
        private IList<Story> _storiesList;
        private IList<Iteration> _iterationsList;
        private IList<Task> _tasksList;
        private IList<ProductionRelease> _productionReleasesList;
        private IList<Component> _componentsList;

        [PrimaryKey(PrimaryKeyType.GuidComb, Access=PropertyAccess.NosetterCamelcaseUnderscore)]
        public Guid Id
        {
            get { return _id; }
        }
        [Property, ValidateNotEmpty("Project Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [BelongsTo(), ValidateNotEmpty("Company is required.")]
        public Company Company
        {
            get { return _company; }
            set { _company = value; }
        }

        [HasMany(typeof(Story), RelationType = RelationType.Bag, Lazy = true, Cascade = ManyRelationCascadeEnum.All)]
        private IList<Story> StoriesList
        {
            get
            {
                if (_storiesList == null) _storiesList = new List<Story>();
                return _storiesList;
            }
            set { _storiesList = value; }
        }

        [HasMany(typeof(Iteration), RelationType = RelationType.Bag, Lazy = true, Cascade = ManyRelationCascadeEnum.All)]
        private IList<Iteration> IterationsList
        {
            get
            {
                if (_iterationsList == null) _iterationsList = new List<Iteration>();
                return _iterationsList;
            }
            set { _iterationsList = value; }
        }

        [HasMany(typeof(Task), RelationType = RelationType.Bag, Lazy = true, Cascade = ManyRelationCascadeEnum.All)]
        private IList<Task> TasksList
        {
            get
            {
                if (_tasksList == null) _tasksList = new List<Task>();
                return _tasksList;
            }
            set { _tasksList = value; }
        }

        [HasMany(typeof(ProductionRelease), RelationType = RelationType.Bag, Lazy = true, Cascade = ManyRelationCascadeEnum.All, Table="ProductionRelease", ColumnKey="Project")]
        private IList<ProductionRelease> ProductionReleasesList
        {
            get
            {
                if (_productionReleasesList == null) _productionReleasesList = new List<ProductionRelease>();
                return _productionReleasesList;
            }
            set { _productionReleasesList = value; }
        }

        [HasMany(typeof(Component), RelationType = RelationType.Bag, Lazy = false, Cascade = ManyRelationCascadeEnum.All, Table="Component", ColumnKey="Project")]
        private IList<Component> ComponentsList
        {
            get
            {
                if (_componentsList == null) _componentsList = new List<Component>();
                return _componentsList;
            }
            set { _componentsList = value; }
        }

        public IList<Story> Stories
        {
            get
            {
                List<Story> result = new List<Story>();
                foreach (Story item in StoriesList)
                {
                    result.Add(item);
                }
                return result.AsReadOnly();
            }
        }

        public IList<Iteration> Iterations
        {
            get
            {
                List<Iteration> result = new List<Iteration>();
                foreach (Iteration item in IterationsList)
                {
                    result.Add(item);
                }
                return result.AsReadOnly();
            }
        }

        public IList<Task> Tasks
        {
            get
            {
                List<Task> result = new List<Task>();
                foreach (Task item in TasksList)
                {
                    result.Add(item);
                }
                return result.AsReadOnly();
            }
        }

        public IList<ProductionRelease> ProductionReleases
        {
            get
            {
                List<ProductionRelease> result = new List<ProductionRelease>();
                foreach (ProductionRelease item in ProductionReleasesList)
                {
                    result.Add(item);
                }
                return result.AsReadOnly();
            }
        }

        public IList<Component> Components
        {
            get
            {
                List<Component> result = new List<Component>();
                foreach (Component item in ComponentsList)
                {
                    result.Add(item);
                }
                return result.AsReadOnly();
            }
        }

        public Component[] ComponentsArray
        {
            get
            {
                Component[] items = new Component[ComponentsList.Count];
                ComponentsList.CopyTo(items, 0);
                return items;
            }
            set { ComponentsList = new List<Component>(value); }
        }

        public void AddIteration(Iteration item)
        {
            if (!IterationsList.Contains(item))
            {
                IterationsList.Add(item);
                item.Project = this;
            }
        }

        public void RemoveIteration(Iteration item)
        {
            if (IterationsList.Contains(item))
            {
                IterationsList.Remove(item);
                item.Project = null;
            }
        }

        public void AddStory(Story item)
        {
            if (!StoriesList.Contains(item))
            {
                StoriesList.Add(item);
                item.Project = this;
            }
        }

        public void RemoveStory(Story item)
        {
            if (StoriesList.Contains(item))
            {
                StoriesList.Remove(item);
                item.Project = null;
            }
        }

        public void AddTask(Task item)
        {
            if (!TasksList.Contains(item))
            {
                TasksList.Add(item);
            }
        }

        public void RemoveTask(Task item)
        {
            if (TasksList.Contains(item))
            {
                TasksList.Remove(item);
            }
        }

        public void AddProductionRelease(ProductionRelease item)
        {
            if (!ProductionReleasesList.Contains(item))
            {
                ProductionReleasesList.Add(item);
            }
        }

        public void RemoveProductionRelease(ProductionRelease item)
        {
            if (ProductionReleasesList.Contains(item))
            {
                ProductionReleasesList.Remove(item);
            }
        }

        public void AddComponent(Component item)
        {
            if (!ComponentsList.Contains(item))
            {
                ComponentsList.Add(item);
            }
        }

        public void RemoveComponent(Component item)
        {
            if (ComponentsList.Contains(item))
            {
                ComponentsList.Remove(item);
            }
        }

        public void RemoveComponent(Guid componentId)
        {
            foreach (Component item in ComponentsList)
            {
                if (item.Id == componentId)
                {
                    RemoveComponent(item);
                    break;
                }
            }
        }

        public void ClearComponents()
        {
            ComponentsList.Clear();
        }

        public int GetNextStoryNumber()
        {
            int highestNumber = 0;
            foreach (Story story in _storiesList)
            {
                if (story.Number > highestNumber)
                {
                    highestNumber = story.Number;
                }
            }
            return highestNumber + 1;
        }

        public int GetNextTaskNumber()
        {
            int highestNumber = 0;
            foreach (Task task in _tasksList)
            {
                if (task.Number > highestNumber)
                {
                    highestNumber = task.Number;
                }
            }
            return highestNumber + 1;
        }

        public IList<Story> GetStoriesNotAssignedToTask(Task task)
        {
            IList<Story> result = new List<Story>();
            foreach (Story story in Stories)
            {
                if (!task.Stories.Contains(story))
                {
                    result.Add(story);
                }
            }
            return result;
        }

        public IList<Task> GetTasksNotAssignedToStory(Story story)
        {
            IList<Task> result = new List<Task>();
            foreach (Task task in Tasks)
            {
                if (!story.Tasks.Contains(task))
                {
                    result.Add(task);
                }
            }
            return result;
        }

        public IList<TaskEstimate> Burndown(IEntity entity, out int maxHours)
        {
            bool scopeIsProject = entity.GetType().IsAssignableFrom(typeof(Project));

            IList<TaskEstimate> result = new List<TaskEstimate>();

            DateTime startDate = DateTime.MaxValue;
            DateTime endDate = DateTime.MinValue;
            foreach (Task task in Tasks)
            {
                if (scopeIsProject || task.Iteration == entity)
                {
                    TaskEstimate firstEst = task.GetInitialEstimate();
                    TaskEstimate lastEst = task.GetLatestEstimate();
                    if (firstEst != null && firstEst.Date < startDate)
                    {
                        startDate = firstEst.Date.Date;
                    }
                    if (lastEst != null && lastEst.Date > endDate)
                    {
                        endDate = lastEst.Date.Date;
                    }
                }
            }

            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                for (DateTime date = startDate; date.Date <= endDate.Date; date = date.AddDays(1))
                {
                    TaskEstimate est = new TaskEstimate();
                    est.Date = date;
                    foreach (Task task in Tasks)
                    {
                        if (scopeIsProject || task.Iteration == entity)
                        {
                            est.HoursRemaining += task.GetHoursRemainingAsOf(date);
                        }
                    }
                    result.Add(est);
                }
            }
            maxHours = 0;
            foreach (TaskEstimate item in result)
            {
                if (item.HoursRemaining > maxHours)
                {
                    maxHours = item.HoursRemaining;
                }
            }
            return result;
        }

        //public void AddStoriesToTask(Task task, string[] storyIds)
        //{
        //    if (!Tasks.Contains(task))
        //    {
        //        throw new ValidationException("The stories were not added to the task", new string[] { "Cannot associate task and story in different projects" + Name });
        //    }

        //    foreach (string idString in storyIds)
        //    {
        //        Guid id = new Guid(idString);
        //        foreach (Story story in Stories)
        //        {
        //            if (id == story.Id && !task.Stories.Contains(story))
        //            {
        //                task.AddStory(story);
        //            }
        //        }
        //    }
        //}

        //public void RemoveStoriesFromTask(Task task, string[] storyIds)
        //{
        //    foreach (string idString in storyIds)
        //    {
        //        Guid id = new Guid(idString);
        //        foreach (Story story in Stories)
        //        {
        //            if (id == story.Id && task.Stories.Contains(story))
        //            {
        //                task.Stories.Remove(story);
        //            }
        //        }
        //    }
        //}

        public void Validate()
        {
            List<string> messages = new List<string>();

            messages.AddRange(EntityUtility.ValidateCollection(Stories));
            messages.AddRange(EntityUtility.ValidateCollection(Tasks));
            messages.AddRange(EntityUtility.ValidateCollection(Iterations));
            messages.AddRange(EntityUtility.ValidateCollection(ProductionReleases));
            messages.AddRange(EntityUtility.ValidateCollection(Components));

            if (messages.Count > 0)
            {
                throw EntityUtility.BuildValidationException(messages);
            }
        }



        #region Sorting Members

        private static string _sortExpression = "Name";
        public static string SortExpression
        {
            get { return _sortExpression; }
            set { _sortExpression = value; }
        }

        private static SortDirection _sortDirection = SortDirection.Ascending;
        public static SortDirection SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; }
        }

        public int CompareTo(Project other)
        {
            if (this == other) return 0;
            if (other == null) return 1;
            if (this == null) return -1;

            int relativeValue;
            switch (SortExpression)
            {
                case "Name":
                    relativeValue = (Name != null) ? Name.CompareTo(other.Name) : -1;
                    break;
                case "Company":
                    relativeValue = (Company != null) ? Company.CompareTo(other.Company) : -1;
                    break;
                default:
                    relativeValue = 0;
                    break;
            }
            if (SortDirection == SortDirection.Descending)
            {
                relativeValue *= -1;
            }
            return relativeValue;
        }

        #endregion
    }
}