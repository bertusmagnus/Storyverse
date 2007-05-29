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
    public class UserPreferences : ActiveRecordValidationBase<UserPreferences>, IEntity
    {
        private Guid _id;
        private Person _person;
        private int _rowsPerPage;

        [PrimaryKey(PrimaryKeyType.Foreign, Access=PropertyAccess.NosetterCamelcaseUnderscore)]
        public Guid Id
        {
            get { return _id; }
        }

        [OneToOne]
        public Person Person
        {
            get { return _person; }
            set { _person = value; }
        }

        [Property]
        public int RowsPerPage
        {
            get { return _rowsPerPage; }
            set { _rowsPerPage = value; }
        }

        public void Validate()
        {
        }
    }
}