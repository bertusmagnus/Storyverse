/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using Castle.Components.Validator;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class Component : BaseEntity<Component>
    {
        private string _name;

        [Property, ValidateNonEmpty("Component Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override void Validate()
        {
        }

        protected override int GetRelativeValue(Component other)
        {
            switch (SortExpression)
            {
                default: //default sort by Name
                    return (Name != null) ? Name.CompareTo(other.Name) : -1;
            }
        }
    }
}