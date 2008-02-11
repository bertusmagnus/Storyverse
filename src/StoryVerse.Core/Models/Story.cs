/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using Castle.ActiveRecord.Queries;
using Castle.Components.Validator;
using StoryVerse.Core.Lookups;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class Story : BaseEntity<Story>
    {
        private int _number;
        private string _title;
        private string _body;
        private string _notes;
        private StoryPriority? _priority;
        private TechnicalRisk? _technicalRisk;
        private int? _estimateFiftyPercent;
        private int? _estimateNinetyPercent;
        private StoryStatus? _status;
        private Project _project;
        private Component _component;
        private Iteration _iteration;
        private IList<Task> _tasksList = new List<Task>();
        private IList<Test> _testsList = new List<Test>();

        [Property]
        public int Number
        {
            get { return _number; }
            internal set { _number = value; }
        }
        [Property, ValidateNonEmpty("Story Title is required.")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        [Property(SqlType = "ntext")]
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }
        [Property(SqlType = "ntext")]
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        [Property]
        public StoryPriority? Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }
        [Property]
        public TechnicalRisk? TechnicalRisk
        {
            get { return _technicalRisk; }
            set { _technicalRisk = value; }
        }
        [Property]
        public int? EstimateFiftyPercent
        {
            get { return _estimateFiftyPercent; }
            set { _estimateFiftyPercent = value; }
        }
        [Property]
        public int? EstimateNinetyPercent
        {
            get { return _estimateNinetyPercent; }
            set { _estimateNinetyPercent = value; }
        }
        [Property]
        public StoryStatus? Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [BelongsTo]
        public Iteration Iteration
        {
            get { return _iteration; }
            set { _iteration = value; }
        }

        [BelongsTo]
        public Project Project
        {
            get { return _project; }
            internal set { _project = value; }
        }

        [BelongsTo]
        public Component Component
        {
            get { return _component; }
            set { _component = value; }
        }

        [HasAndBelongsToMany(typeof(Task), Table = "TaskStory", RelationType = RelationType.Bag, ColumnRef = "TaskId", ColumnKey = "StoryId")]
        private IList<Task> TasksList
        {
            get { return _tasksList; }
            set { _tasksList = value; }
        }

        [HasMany(typeof(Test), RelationType = RelationType.Bag, Lazy = true, Cascade = ManyRelationCascadeEnum.All, Table = "Test", ColumnKey = "Story")]
        private IList<Test> TestsList
        {
            get { return _testsList; }
            set { _testsList = value; }
        }

        public IList<Task> Tasks
        {
            get { return new List<Task>(_tasksList).AsReadOnly(); }
        }

        public IList<Test> Tests
        {
            get { return new List<Test>(_testsList).AsReadOnly(); }
        }

        public Test[] TestsArray
        {
            get { return EntityUtility.CollectionToArray(_testsList); }
            set { _testsList = new List<Test>(value); }
        }

        public void AddTask(Task item)
        {
            if (!_tasksList.Contains(item))
            {
                _tasksList.Add(item);
            }
        }

        public void RemoveTask(Task item)
        {
            if (_tasksList.Contains(item))
            {
                _tasksList.Remove(item);
            }
        }

        public void AddTest(Test item)
        {
            if (!_testsList.Contains(item))
            {
                _testsList.Add(item);
            }
        }

        public void RemoveTest(Test item)
        {
            if (_testsList.Contains(item))
            {
                _testsList.Remove(item);
            }
        }

        public void ClearTests()
        {
            _testsList.Clear();
        }

        public int GetNextTestNumber()
        {
            int highestNumber = 0;
            foreach (Test test in _testsList)
            {
                if (test.Number > highestNumber)
                {
                    highestNumber = test.Number;
                }
            }
            return highestNumber + 1;
        }

        public string DisplayName
        {
            get { return string.Format("{0:000}:{1}", Number, Title); }
        }

        public void AssignNextNumber()
        {
            ScalarQuery q = new ScalarQuery(typeof(Story), 
                @"select max(s.Number) from Story s where s.Project = ?", Project);
            Number = (int)(ExecuteQuery(q) ?? 0) + 1;
        }

        public void AddTasks(string[] taskIds)
        {
            foreach (string idString in taskIds)
            {
                Guid id = new Guid(idString);
                foreach (Task task in Project.Tasks)
                {
                    if (id == task.Id && !_tasksList.Contains(task))
                    {
                        AddTask(task);
                    }
                }
            }
        }

        public void RemoveTasks(string[] taskIds)
        {
            foreach (string idString in taskIds)
            {
                Guid id = new Guid(idString);
                foreach (Task task in Project.Tasks)
                {
                    if (id == task.Id && _tasksList.Contains(task))
                    {
                        RemoveTask(task);
                    }
                }
            }
        }

        public override void Validate()
        {
            List<string> messages = new List<string>();

            messages.AddRange(EntityUtility.ValidateCollection(Tests));

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
                _number = Project.GetNextStoryNumber();
            }
            return saved;
        }

        protected override int GetRelativeValue(Story other)
        {
            switch (SortExpression)
            {
                case "Title":
                    return Title != null ? Title.CompareTo(other.Title) : -1;
                case "Project":
                    return Project != null ? Project.CompareTo(other.Project) : -1;
                case "Iteration":
                    return Iteration != null ? Iteration.CompareTo(other.Iteration) : -1;
                case "Component":
                    return Component != null ? Component.CompareTo(other.Component) : -1;
                case "TechnicalRisk":
                    return TechnicalRisk != null ? TechnicalRisk.Value.CompareTo(other.TechnicalRisk) : -1;
                case "Priority":
                    return Priority != null ? Priority.Value.CompareTo(other.Priority) : -1;
                case "Status":
                    return Status != null ? Status.Value.CompareTo(other.Status) : -1;
                case "EstimateFiftyPercent":
                    return EstimateFiftyPercent != null ? EstimateFiftyPercent.Value.CompareTo(other.EstimateFiftyPercent) : -1;
                case "EstimateNinetyPercent":
                    return EstimateNinetyPercent != null ? EstimateNinetyPercent.Value.CompareTo(other.EstimateNinetyPercent) : -1;
                default:
                    return Number.CompareTo(other.Number);
            }
        }
    }
}