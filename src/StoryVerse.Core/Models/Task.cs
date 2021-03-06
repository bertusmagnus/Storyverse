/* 
 * Copyright � Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using Castle.Components.Validator;
using StoryVerse.Core.Lookups;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class Task : BaseEntity<Task>, IEntity
    {
        private int _number;
        private string _title;
        private string _description;
        private TechnicalRisk? _technicalRisk;
        private TaskStatus _status;
        private string _notes;
        private Person _owner;
        private Project _project;
        private Iteration _iteration;
        private IList<Story> _storiesList;
        private IList<TaskEstimate> _estimatesList;

        [Property]
        public int Number
        {
            get { return _number; }
            internal set { _number = value; }
        }
        [Property, ValidateNonEmpty("Task title is required.")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        [Property(SqlType = "nvarchar(MAX)")]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        [Property(SqlType = "nvarchar(MAX)")]
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        [Property]
        public TechnicalRisk? TechnicalRisk
        {
            get { return _technicalRisk; }
            set { _technicalRisk = value; }
        }
        [Property]
        public TaskStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [BelongsTo]
        public Person Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        [BelongsTo]
        public Project Project
        {
            get { return _project; }
            internal set { _project = value; }
        }

        [BelongsTo]
        public Iteration Iteration
        {
            get { return _iteration; }
            set { _iteration = value; }
        }

        [HasMany(typeof(TaskEstimate), RelationType = RelationType.Bag, Lazy = false, Cascade = ManyRelationCascadeEnum.All, Table = "TaskEstimate", ColumnKey = "Task")]
        private IList<TaskEstimate> EstimatesList
        {
            get
            {
                if (_estimatesList == null) _estimatesList = new List<TaskEstimate>();
                return _estimatesList;
            }
            set { _estimatesList = value; }
        }

        [HasAndBelongsToMany(typeof(Story), Table = "TaskStory", 
           RelationType = RelationType.Bag,
           ColumnRef = "StoryId", ColumnKey = "TaskId")]
        private IList<Story> StoriesList
        {
            get
            {
                if (_storiesList == null) _storiesList = new List<Story>();
                return _storiesList;
            }
            set { _storiesList = value; }
        }

        public IList<TaskEstimate> Estimates
        {
            get
            {
                List<TaskEstimate> result = new List<TaskEstimate>();
                foreach (TaskEstimate item in EstimatesList)
                {
                    result.Add(item);
                }
                result.Sort();
                return result.AsReadOnly();
            }
        }

        public void AddEstimate(TaskEstimate item)
        {
            //if an item exists for the same day one being added, remove it
            foreach (TaskEstimate est in EstimatesList)
            {
                if (item.Date.Date == est.Date.Date)
                {
                    EstimatesList.Remove(est);
                    break;
                }
            }
            if (!EstimatesList.Contains(item))
            {
                EstimatesList.Add(item);
            }
        }

        public void RemoveEstimate(TaskEstimate item)
        {
            if (EstimatesList.Contains(item))
            {
                EstimatesList.Remove(item);
            }
        }

        public TaskEstimate GetInitialEstimate()
        {
            if (EstimatesList.Count == 0) return null;

            TaskEstimate.SortExpression = "Date";
            TaskEstimate.SortDirection = SortDirection.Ascending;
            return Estimates[0];
        }

        public TaskEstimate GetLatestEstimate()
        {
            if (EstimatesList.Count == 0) return null;

            TaskEstimate.SortExpression = "Date";
            TaskEstimate.SortDirection = SortDirection.Descending;
            return Estimates[0];
        }

        public int? InitialEstimateHours
        {
            get
            {
                TaskEstimate est = GetInitialEstimate();
                if (est == null) return null;
                return est.HoursRemaining;
            }
        }

        public int? LatestEstimateHours
        {
            get
            {
                TaskEstimate est = GetLatestEstimate();
                if (est == null) return null;
                return est.HoursRemaining;
            }
        }

        public DateTime? LatestEstimateDate
        {
            get
            {
                TaskEstimate est = GetLatestEstimate();
                if (est == null) return null;
                return est.Date;
            }
        }

        public int GetHoursRemainingAsOf(DateTime date)
        {
            int result = 0;
            DateTime latestDate = DateTime.MinValue;
            foreach (TaskEstimate item in Estimates)
            {
                if (item.Date.Date > latestDate.Date && item.Date.Date <= date.Date)
                {
                    latestDate = item.Date;
                    result = item.HoursRemaining;
                } 
            }
            return result;
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

        public void AddStory(Story item)
        {
            if (!StoriesList.Contains(item))
            {
                StoriesList.Add(item);
            }
        }

        public void RemoveStory(Story item)
        {
            if (StoriesList.Contains(item))
            {
                StoriesList.Remove(item);
            }
        }

        public int StoriesCount
        {
            get { return StoriesList.Count; }
        }

        public string DisplayName
        {
            get { return string.Format("{0:000}:{1}", Number, Title); }
        }

        public void AddStories(string[] storyIds)
        {
            foreach (string idString in storyIds)
            {
                Guid id = new Guid(idString);
                foreach (Story story in Project.Stories)
                {
                    if (id == story.Id && !StoriesList.Contains(story))
                    {
                        StoriesList.Add(story);
                    }
                }
            }
        }

        public void RemoveStories(string[] storyIds)
        {
            foreach (string idString in storyIds)
            {
                Guid id = new Guid(idString);
                foreach (Story story in Project.Stories)
                {
                    if (id == story.Id && StoriesList.Contains(story))
                    {
                        StoriesList.Remove(story);
                    }
                }
            }
        }

        public override void Validate()
        {
            List<string> messages = new List<string>();

            if (Status == TaskStatus.NotStarted && EstimatesList.Count > 1)
            {
                messages.Add("A task that is not started cannot have more than one estimate");
            }
            if (Status == TaskStatus.Done && !(LatestEstimateHours == 0 || LatestEstimateHours == null))
            {
                messages.Add("A task cannot be marked Done if remaining hours is greater than zero");
            }

            messages.AddRange(EntityUtility.ValidateCollection(Estimates));

            if (messages.Count > 0)
            {
                throw EntityUtility.BuildValidationException(messages);
            }
        }

        protected override object IsUnsaved()
        {
            object saved = base.IsUnsaved();
            if (saved == null)
            {
                _number = Project.GetNextTaskNumber();
            }
            return saved;
        }

        protected override int GetRelativeValue(Task other)
        {
            switch (SortExpression)
            {
                case "Title":
                    return Title != null ? Title.CompareTo(other.Title) : -1;
                case "TechnicalRisk":
                    return TechnicalRisk != null ? TechnicalRisk.Value.CompareTo(other.TechnicalRisk) : -1;
                case "Status":
                    return Status.CompareTo(other.Status);
                case "Owner":
                    return Owner != null ? Owner.CompareTo(other.Owner) : -1;
                case "Project":
                    return Project != null ? Project.CompareTo(other.Project) : -1;
                case "Iteration":
                    return Iteration != null ? Iteration.CompareTo(other.Iteration) : -1;
                case "InitialEstimateHours":
                    return (InitialEstimateHours != null) ? InitialEstimateHours.Value.CompareTo(other.InitialEstimateHours) : -1;
                case "LatestEstimateHours":
                    return (LatestEstimateHours != null) ? LatestEstimateHours.Value.CompareTo(other.LatestEstimateHours) : -1;
                case "StoriesCount":
                    return StoriesCount.CompareTo(other.StoriesCount);
                case "LatestEstimateDate":
                    return (LatestEstimateDate != null) ? LatestEstimateDate.Value.CompareTo(other.LatestEstimateDate) : -1;
                default:
                    return Number.CompareTo(other.Number);
            }
        }
    }
}