/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    [ActiveRecord()]
    public class ProductionRelease : ActiveRecordValidationBase<ProductionRelease>, IEntity
    {
        private Guid _id;
        private string _name;

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

        public void Validate()
        {
        }
    }
}