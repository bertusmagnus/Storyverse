/*
 * Created by: 
 * Created: Monday, January 01, 2007
 */

using System;
using StoryPlanner.Core.Lookups;
using Castle.ActiveRecord;

namespace StoryPlanner.Models
{
    [ActiveRecord()]
    public class Component : ActiveRecordValidationBase<Component>
    {
        private Guid _id;
        private string _name;
        private Project _project;

        [PrimaryKey(PrimaryKeyType.GuidComb, Access = PropertyAccess.NosetterCamelcaseUnderscore)]
        public Guid Id
        {
            get { return _id; }
        }
        [Property, ValidateNotEmpty("Component Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [BelongsTo()]
        public Project Project
        {
            get { return _project; }
            set { _project = value; }
        }
    }
}