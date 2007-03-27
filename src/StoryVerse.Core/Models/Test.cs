/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Castle.ActiveRecord;

namespace StoryVerse.Core.Models
{
    [ActiveRecord()]
    public class Test : ActiveRecordValidationBase<Test>, IEntity
    {
        private Guid _id;
        private int _number;
        private string _body;
        private Story _story;

        [PrimaryKey(PrimaryKeyType.GuidComb, Access = PropertyAccess.NosetterCamelcaseUnderscore)]
        public Guid Id
        {
            get { return _id; }
        }
        [Property]
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }
        [Property(SqlType = "nvarchar(1000)"), ValidateNotEmpty("Acceptance test body is required.")]
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public void Validate()
        {
        }
    }
}