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
    public class Test : BaseEntity<Test>
    {
        private int _number;
        private string _body;

        [Property]
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }
        [Property(SqlType = "nvarchar(1000)"), ValidateNonEmpty("Acceptance test body is required.")]
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        protected override int GetRelativeValue(Test other)
        {
            return Number.CompareTo(other.Number);
        }
    }
}