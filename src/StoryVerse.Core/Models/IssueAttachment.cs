/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class IssueAttachment : BaseEntity<IssueAttachment>
    {
        private Issue _issue;
        private string _title;
        private string _filename;
        private int _size;
        private byte[] _body;
        private Person _createdBy;
        private DateTime _dateCreated = DateTime.Now;

        [BelongsTo]
        public Issue Issue
        {
            get { return _issue; }
            set { _issue = value; }
        }

        [Property]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [Property]
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        [Property]
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        [Property]
        public byte[] Body
        {
            get { return _body; }
            set { _body = value; }
        }

        [BelongsTo]
        public Person CreatedBy
        {
            get { return _createdBy; }
            set { _createdBy = value; }
        }

        [Property]
        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set { _dateCreated = value; }
        }

        public decimal SizeKb
        {
            get
            {
                decimal kb = _size/1000;
                return Math.Round(kb, 0, MidpointRounding.AwayFromZero);
            }
        }

        protected override int GetRelativeValue(IssueAttachment other)
        {
            switch (SortExpression)
            {
                case "CreatedBy":
                    return  (CreatedBy != null) ? CreatedBy.CompareTo(other.CreatedBy) : -1;
                case "DateCreated":
                    return DateCreated.CompareTo(other.DateCreated);
                default: //default sort by Title
                    return Title.CompareTo(other.Title);
            }
        }
    }
}