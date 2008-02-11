/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.Components.Validator;
using StoryVerse.Core.Lookups;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class Iteration : BaseEntity<Iteration>, IComparable<Iteration>
    {
        private string _name;
        private Project _project;
        //private IList _storiesList;
        //private IList _tasksList;

        [Property, ValidateNonEmpty("Iteration Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [BelongsTo]
        public Project Project
        {
            get { return _project; }
            set { _project = value; }
        }

        public int GetStoriesCount()
        {
            int result = 0;
            foreach (Story story in Project.Stories)
            {
                if (story.Iteration == this)
                {
                    result ++;
                }
            }
            return result;
        }

        public int GetTasksCount()
        {
            int result = 0;
            foreach (Task task in Project.Tasks)
            {
                if (task.Iteration == this)
                {
                    result++;
                }
            }
            return result;
        }

        //[HasMany(typeof(Story), RelationType = RelationType.Bag, Lazy = true)]
        //public IList StoriesList
        //{
        //    get { return _storiesList; }
        //    set { _storiesList = value; }
        //}

        //public IList<Story> Stories
        //{
        //    get
        //    {
        //        List<Story> result = new List<Story>();
        //        foreach (Story item in _storiesList)
        //        {
        //            result.Add(item);
        //        }
        //        return result.AsReadOnly();
        //    }
        //}

        //[HasMany(typeof(Task), RelationType = RelationType.Bag, Lazy = true)]
        //public IList TasksList
        //{
        //    get { return _tasksList; }
        //    set { _tasksList = value; }
        //}

        //public IList<Task> Tasks
        //{
        //    get
        //    {
        //        List<Task> result = new List<Task>();
        //        foreach (Task item in _tasksList)
        //        {
        //            result.Add(item);
        //        }
        //        return result.AsReadOnly();
        //    }
        //}

        //public void RemoveStory(Story item)
        //{
        //    if (StoriesList.Contains(item))
        //    {
        //        StoriesList.Remove(item);
        //        item.Iteration = this;
        //    }
        //}

        //public void AddStory(Story item)
        //{
        //    if (!StoriesList.Contains(item))
        //    {
        //        StoriesList.Add(item);
        //        item.Iteration = null;
        //    }
        //}

        //public void AddTask(Task item)
        //{
        //    if (!TasksList.Contains(item))
        //    {
        //        TasksList.Add(item);
        //        item.Iteration = this;
        //    }
        //}

        //public void RemoveTask(Task item)
        //{
        //    if (TasksList.Contains(item))
        //    {
        //        TasksList.Remove(item);
        //        item.Iteration = null;
        //    }
        //}

        //public IList<Story> GetStoriesNotAssignedToTask(Task task)
        //{
        //    IList<Story> result = new List<Story>();
        //    foreach (Story story in Stories)
        //    {
        //        if (!task.Stories.Contains(story))
        //        {
        //            result.Add(story);
        //        }
        //    }
        //    return result;
        //}

        //public IList<Task> GetTasksNotAssignedToStory(Story story)
        //{
        //    IList<Task> result = new List<Task>();
        //    foreach (Task task in Tasks)
        //    {
        //        if (!story.Tasks.Contains(task))
        //        {
        //            result.Add(task);
        //        }
        //    }
        //    return result;
        //}

        protected override int GetRelativeValue(Iteration other)
        {
            switch (SortExpression)
            {
                case "Project":
                    return (Project != null) ? Project.CompareTo(other.Project) : -1;
                default: //default sort by Name
                    return (Name != null) ? Name.CompareTo(other.Name) : -1;
            }
        }
    }
}