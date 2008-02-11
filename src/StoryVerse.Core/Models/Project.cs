/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class Project : BaseEntity<Project>
    {
        private string _name;
        private Company _company;
        private IList<Story> _storiesList = new List<Story>();
        private IList<Iteration> _iterationsList = new List<Iteration>();
        private IList<Task> _tasksList = new List<Task>();
        private IList<ProductionRelease> _productionReleasesList = new List<ProductionRelease>();
        private IList<Component> _componentsList = new List<Component>();
        private IList<Issue> _issuesList = new List<Issue>();

        [Property, ValidateNonEmpty("Project Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [BelongsTo, ValidateNonEmpty("Company is required.")]
        public Company Company
        {
            get { return _company; }
            set { _company = value; }
        }

        [HasMany(typeof(Story), RelationType = RelationType.Bag, Lazy = true, 
            Cascade = ManyRelationCascadeEnum.All)]
        private IList<Story> StoriesList
        {
            get { return _storiesList; }
            set { _storiesList = value; }
        }

        [HasMany(typeof(Iteration), RelationType = RelationType.Bag, Lazy = true, 
            Cascade = ManyRelationCascadeEnum.All)]
        private IList<Iteration> IterationsList
        {
            get { return _iterationsList; }
            set { _iterationsList = value; }
        }

        [HasMany(typeof(Task), RelationType = RelationType.Bag, Lazy = true, 
            Cascade = ManyRelationCascadeEnum.All)]
        private IList<Task> TasksList
        {
            get { return _tasksList; }
            set { _tasksList = value; }
        }

        [HasMany(typeof(Issue), RelationType = RelationType.Bag, Lazy = false, 
            Cascade = ManyRelationCascadeEnum.AllDeleteOrphan)]
        private IList<Issue> IssuesList
        {
            get { return _issuesList; }
            set { _issuesList = value; }
        }

        [HasMany(typeof(ProductionRelease), RelationType = RelationType.Bag, Lazy = true, 
            Cascade = ManyRelationCascadeEnum.All, Table="ProductionRelease", ColumnKey="Project")]
        private IList<ProductionRelease> ProductionReleasesList
        {
            get { return _productionReleasesList; }
            set { _productionReleasesList = value; }
        }

        [HasMany(typeof(Component), RelationType = RelationType.Bag, Lazy = false, 
            Cascade = ManyRelationCascadeEnum.All, Table="Component", ColumnKey="Project")]
        private IList<Component> ComponentsList
        {
            get { return _componentsList; }
            set { _componentsList = value; }
        }

        public IList<Story> Stories
        {
            get { return new List<Story>(_storiesList).AsReadOnly(); }
        }

        public IList<Iteration> Iterations
        {
            get { return new List<Iteration>(_iterationsList).AsReadOnly();}
        }

        public IList<Task> Tasks
        {
            get { return new List<Task>(_tasksList).AsReadOnly(); }
        }

        public IList<Issue> Issues
        {
            get { return new List<Issue>(_issuesList).AsReadOnly(); }
        }

        public IList<ProductionRelease> ProductionReleases
        {
            get { return new List<ProductionRelease>(_productionReleasesList).AsReadOnly(); }
        }

        public IList<Component> Components
        {
            get { return new List<Component>(_componentsList).AsReadOnly(); }
        }

        public Component[] ComponentsArray
        {
            get { return EntityUtility.CollectionToArray(_componentsList); }
            set { _componentsList = new List<Component>(value); }
        }

        public void AddIteration(Iteration item)
        {
            if (!_iterationsList.Contains(item))
            {
                _iterationsList.Add(item);
                item.Project = this;
            }
        }

        public void RemoveIteration(Iteration item)
        {
            if (_iterationsList.Contains(item))
            {
                _iterationsList.Remove(item);
                item.Project = null;
            }
        }

        public void AddStory(Story item)
        {
            if (!_storiesList.Contains(item))
            {
                _storiesList.Add(item);
                item.Project = this;
            }
        }

        public void RemoveStory(Story item)
        {
            if (_storiesList.Contains(item))
            {
                _storiesList.Remove(item);
                item.Project = null;
            }
        }

        public void AddTask(Task item)
        {
            if (!_tasksList.Contains(item))
            {
                _tasksList.Add(item);
                item.Project = this;
            }
        }

        public void RemoveTask(Task item)
        {
            if (_tasksList.Contains(item))
            {
                _tasksList.Remove(item);
            }
        }

        public void AddIssue(Issue item)
        {
            if (!_issuesList.Contains(item))
            {
                _issuesList.Add(item);
                item.Project = this;
            }
        }

        public void RemoveIssue(Issue item)
        {
            if (_issuesList.Contains(item))
            {
                _issuesList.Remove(item);
                item.Project = null;
            }
        }

        public void AddProductionRelease(ProductionRelease item)
        {
            if (!_productionReleasesList.Contains(item))
            {
                _productionReleasesList.Add(item);
            }
        }

        public void RemoveProductionRelease(ProductionRelease item)
        {
            if (_productionReleasesList.Contains(item))
            {
                _productionReleasesList.Remove(item);
            }
        }

        public void AddComponent(Component item)
        {
            if (!_componentsList.Contains(item))
            {
                _componentsList.Add(item);
            }
        }

        public void RemoveComponent(Component item)
        {
            if (_componentsList.Contains(item))
            {
                _componentsList.Remove(item);
            }
        }

        public void RemoveComponent(Guid componentId)
        {
            foreach (Component item in _componentsList)
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
            _componentsList.Clear();
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

        public int GetNextIssueNumber()
        {
            int highestNumber = 0;
            foreach (Issue issue in _issuesList)
            {
                if (issue.Number > highestNumber)
                {
                    highestNumber = issue.Number;
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

        public IDictionary<object, decimal> Burndown(IEntity entity)
        {
            bool scopeIsProject = entity.GetType().IsAssignableFrom(typeof(Project));

            IDictionary<object, decimal> result = new Dictionary<object, decimal>();

            DateTime startDate = DateTime.MaxValue;
            DateTime endDate = DateTime.MinValue;
            foreach (Task task in Tasks)
            {
                if (scopeIsProject || task.Iteration != null && 
                    task.Iteration.Id == entity.Id)
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
                    int hoursRemaining = 0;
                    foreach (Task task in Tasks)
                    {
                        if (scopeIsProject || task.Iteration != null &&
                            task.Iteration.Id == entity.Id)
                        {
                            hoursRemaining += task.GetHoursRemainingAsOf(date);
                        }
                    }
                    result.Add(date, hoursRemaining);
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

        public override void Validate()
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

        protected override int GetRelativeValue(Project other)
        {
            switch (SortExpression)
            {
                case "Company":
                    return (Company != null) ? Company.CompareTo(other.Company) : -1;
                default:
                    return (Name != null) ? Name.CompareTo(other.Name) : -1;
            }
        }
    }
}