/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using Castle.ActiveRecord;
using Castle.Components.Validator;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class ProductionRelease : BaseEntity<ProductionRelease>
    {
        private string _name;

        [Property, ValidateNonEmpty("Project Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected override int GetRelativeValue(ProductionRelease other)
        {
            return Name.CompareTo(other.Name); 
        }
    }
}