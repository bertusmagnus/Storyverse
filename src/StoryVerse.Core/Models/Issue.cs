/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using StoryVerse.Core.Lookups;
using System.Reflection;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class Issue : BaseEntity<Issue>
    {
        private int _number;
        private Project _project;
        private IssueType _type;
        private IssueStatus _status = IssueStatus.New;
        private string _title;
        private string _description;
        private IssuePriority _priority;
        private IssueSeverity _severity;
        private IssueDisposition _disposition;
        private Component _component;
        private Person _reportedBy;
        private Person _owner;
        private Person _lastUpdatedBy;
        private DateTime _dateCreated = DateTime.Now;
        private DateTime _dateLastUpdated = DateTime.Now;
        private IList<IssueNote> _notesList = new List<IssueNote>();
        private IList<IssueChange> _changesList = new List<IssueChange>();
        private IList<IssueAttachment> _attachmentsList = new List<IssueAttachment>();

        [Property]
        public int Number
        {
            get { return _number; }
            internal set { _number = value; }
        }
        [BelongsTo]
        public Project Project
        {
            get { return _project; }
            internal set { _project = value; }
        }

        [Property, ValidateNonEmpty("Type is required.")]
        public IssueType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [Property]
        public IssueStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [Property, ValidateNonEmpty("Title is required.")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [Property(SqlType = "nvarchar(MAX)"), ValidateNonEmpty("Description is required.")]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [BelongsTo(Cascade = CascadeEnum.None, NotFoundBehaviour = NotFoundBehaviour.Ignore)]
        public IssuePriority Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        [BelongsTo(Cascade = CascadeEnum.None, NotFoundBehaviour = NotFoundBehaviour.Ignore)]
        public IssueSeverity Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        [BelongsTo(Cascade = CascadeEnum.None, NotFoundBehaviour = NotFoundBehaviour.Ignore)]
        public IssueDisposition Disposition
        {
            get { return _disposition; }
            set { _disposition = value; }
        }

        [BelongsTo]
        public Component Component
        {
            get { return _component; }
            set { _component = value; }
        }

        [BelongsTo]
        public Person ReportedBy
        {
            get { return _reportedBy; }
            set { _reportedBy = value; }
        }

        [BelongsTo, ValidateNonEmpty("Owner is required.  You must assign the issue.")]
        public Person Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        [BelongsTo]
        public Person LastUpdatedBy
        {
            get { return _lastUpdatedBy; }
            set { _lastUpdatedBy = value; }
        }

        [Property]
        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { _dateCreated = value; }
        }

        [Property]
        public DateTime DateLastUpdated
        {
            get { return _dateLastUpdated; }
            set { _dateLastUpdated = value; }
        }

        #region Notes collection

        [HasMany(typeof(IssueNote), RelationType = RelationType.Bag, Lazy = true, 
            Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Table="IssueNote")]
        private IList<IssueNote> NotesList
        {
            get { return _notesList; }
            set { _notesList = value; }
        }

        public IList<IssueNote> Notes
        {
            get { return new List<IssueNote>(_notesList).AsReadOnly(); }
        }

        public void AddNote(IssueNote item)
        {
            if (!_notesList.Contains(item))
            {
                _notesList.Add(item);
                item.Issue = this;
            }
        }

        public void RemoveNote(IssueNote item)
        {
            if (_notesList.Contains(item))
            {
                _notesList.Remove(item);
                item.Issue = null;
            }
        } 

        #endregion

        #region Changes collection

        [HasMany(typeof(IssueChange), RelationType = RelationType.Bag, Lazy = true, 
            Cascade = ManyRelationCascadeEnum.AllDeleteOrphan, Table="IssueChange")]
        private IList<IssueChange> ChangesList
        {
            get { return _changesList; }
            set { _changesList = value; }
        }

        public IList<IssueChange> Changes
        {
            get { return new List<IssueChange>(_changesList).AsReadOnly(); }
        }

        public void AddChange(IssueChange item)
        {
            if (!_changesList.Contains(item))
            {
                _changesList.Add(item);
                item.Issue = this;
            }
        }

        public void RemoveChange(IssueChange item)
        {
            if (_changesList.Contains(item))
            {
                _changesList.Remove(item);
                item.Issue = null;
            }
        } 

        #endregion
        
        #region Attachments collection

        [HasMany(typeof(IssueAttachment), RelationType = RelationType.Bag, Lazy = true,
            Cascade = ManyRelationCascadeEnum.Delete, Table = "IssueAttachment")]
        private IList<IssueAttachment> AttachmentsList
        {
            get { return _attachmentsList; }
            set { _attachmentsList = value; }
        }

        public IList<IssueAttachment> Attachments
        {
            get { return new List<IssueAttachment>(_attachmentsList).AsReadOnly(); }
        }

        public static IssueStatus[] OpenStatues
        {
            get 
            { 
                return new IssueStatus[]
                {
                    IssueStatus.New, 
                    IssueStatus.Assigned, 
                    IssueStatus.Pending
                }; 
            }
        }

        public void AddAttachment(IssueAttachment item)
        {
            if (!_attachmentsList.Contains(item))
            {
                _attachmentsList.Add(item);
                item.Issue = this;
            }
        }

        public void RemoveAttachment(IssueAttachment item)
        {
            if (_attachmentsList.Contains(item))
            {
                _attachmentsList.Remove(item);
                item.Issue = null;
            }
        }

        #endregion
        
        protected override int GetRelativeValue(Issue other)
        {
            switch (SortExpression)
            {
                case "Type":
                    return Type.CompareTo(other.Type);
                case "Status":
                    return Status.CompareTo(other.Status);
                case "Number":
                    return Number.CompareTo(other.Number);
                case "Owner":
                    return Owner == null ? -1 : Owner.CompareTo(other.Owner);
                case "Priority":
                    return Priority == null ? -1 : Priority.CompareTo(other.Priority);
                case "Disposition":
                    return Disposition == null ? -1 : Disposition.CompareTo(other.Disposition);
                case "Severity":
                    return Severity == null ? -1 : Severity.CompareTo(other.Severity);
                case "Component":
                    return Component == null ? -1 : Component.CompareTo(other.Component);
                default: //default sort by Title
                    return Title.CompareTo(other.Title);
            }
        }

        protected override object IsUnsaved()
        {
            object saved = base.IsUnsaved();
            if (saved == null)
            {
                _number = Project.GetNextIssueNumber();
            }
            return saved;
        }

        protected override bool OnFlushDirty(object id, IDictionary previousState, IDictionary currentState, NHibernate.Type.IType[] types)
        {
            LogChange<Person>("Owner", "FullName", previousState, currentState);
            LogChange<IssueStatus>("Status",previousState, currentState);
            LogChange<string>("Title", previousState, currentState);
            LogChange<string>("Description", previousState, currentState);
            LogChange<IssuePriority>("Priority", "Name", previousState, currentState);
            LogChange<IssueSeverity>("Severity", "Name", previousState, currentState);
            LogChange<IssueDisposition>("Disposition", "Name", previousState, currentState);
            LogChange<Component>("Component", "Name", previousState, currentState);
            return base.OnFlushDirty(id, previousState, currentState, types);
        }

        private void LogChange<T>(string propertyName, IDictionary previousState, IDictionary currentState)
        {
            LogChange<T>(propertyName, null, currentState, previousState);
        }

        private void LogChange<T>(string propertyName, string subPropertyName, IDictionary previousState, IDictionary currentState)
        {
            if (IsTransient)
            {
                return;
            }

            T previous = (T)previousState[propertyName];
            T current = (T)currentState[propertyName];

            if (previous == null && previous == null) return;
            if (previous != null && previous.Equals(current)) return;
            if (current != null && current.Equals(previous)) return;

            if (subPropertyName == null)
            {
                AddAndSaveChange(propertyName, previous, current);
            }
            else
            {
                PropertyInfo subProperty = typeof(T).GetProperty(subPropertyName);
                AddAndSaveChange(propertyName,
                                 subProperty.GetValue(current, null),
                                 subProperty.GetValue(previous, null));   
            }
        }

        private void AddAndSaveChange(string propertyName, object oldValue, object newValue)
        {
            IssueChange change = new IssueChange();
            change.ChangeDate = DateTime.Now;
            change.PropertyName = propertyName;
            change.ChangedBy = LastUpdatedBy;
            change.NewValue = oldValue == null ? "blank" : oldValue.ToString();
            change.OldValue = newValue == null ? "blank" : newValue.ToString();
            AddChange(change);
            using (new SessionScope())
            {
                IssueChange.Save(change);
            }
        }

        public IssueAttachment GetAttachment(Guid attachmentId)
        {
            foreach (IssueAttachment att in _attachmentsList)
            {
                if (att.Id == attachmentId)
                {
                    return att;
                }
            }
            return null;
        }
    }
}