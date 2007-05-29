/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Security.Principal;
using Castle.ActiveRecord;
using StoryVerse.Core.Lookups;

namespace StoryVerse.Core.Models
{
    [ActiveRecord()]
    public class Person : ActiveRecordValidationBase<Person>, IEntity, IPrincipal, IComparable<Person>
    {
        private Guid _id;
        private string _firstName;
        private string _lastName;
        private string _username;
        private string _password;
        private Company _company;
        private UserPreferences _userPreferences;
        private bool _isAdmin = false;
        private bool _canViewOnly = false;

        [PrimaryKey(PrimaryKeyType.GuidComb, Access=PropertyAccess.NosetterCamelcaseUnderscore)]
        public Guid Id
        {
            get { return _id; }
        }
        [Property, ValidateNotEmpty("First Name is required.")]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        [Property, ValidateNotEmpty("Last Name is required.")]
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        [Property, ValidateIsUnique("Another user exists with this username.")]
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }
        [Property]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        [Property]
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; }
        }
        [Property]
        public bool CanViewOnly
        {
            get { return _canViewOnly; }
            set { _canViewOnly = value; }
        }

        [OneToOne]
        public UserPreferences UserPreferences
        {
            get { return _userPreferences ?? new UserPreferences(); }
            internal set { _userPreferences = value; }
        }

        [BelongsTo(), ValidateNotEmpty("Company is required.")]
        public Company Company
        {
            get { return _company; }
            internal set { _company = value; }
        }

        public UserProjectScope ProjectScope
        {
            get { return Company.UserProjectScope; }
        }

        public void InitUserPreferences()
        {
            if (_userPreferences == null)
            {
                _userPreferences = new UserPreferences();
                _userPreferences.Person = this;
            }
        }

        public string AlphaName
        {
            get
            {
                return string.Format("{0}{1}{2}", _lastName,
                    (!string.IsNullOrEmpty(_lastName) && !string.IsNullOrEmpty(_lastName)) ? ", " : null,
                    _firstName);
            }
        }

        public string AlphaNameShort
        {
            get
            {
                return string.Format("{0}{1}{2}", _lastName,
                    (!string.IsNullOrEmpty(_lastName) && !string.IsNullOrEmpty(_lastName)) ? ", " : null, 
                    _firstName.Substring(0, 1));
            }
        }
        public string FullName
        {
            get
            {
                string val = _firstName + " " + _lastName;
                return val.Trim();
            }
        }

        public string Initials
        {
            get { return _firstName.Substring(0, 1) + _lastName.Substring(0, 1); }
        }

        public void Validate()
        {
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return new GenericIdentity(_username); }
        }

        public bool IsInRole(string role)
        {
            switch (role)
            {
                case ("Admin"):
                    return IsAdmin;
                case ("Editor"):
                    return !CanViewOnly;
                default:
                    return false;
            }
        }

        #endregion

        #region Sorting Members

        private static string _sortExpression = "AlphaName";
        public static string SortExpression
        {
            get { return _sortExpression; }
            set { _sortExpression = value; }
        }

        private static SortDirection _sortDirection = SortDirection.Ascending;
        public static SortDirection SortDirection
        {
            get { return _sortDirection; }
            set { _sortDirection = value; }
        }

        public int CompareTo(Person other)
        {
            if (this == other) return 0;
            if (other == null) return 1;
            if (this == null) return -1;

            int relativeValue;
            switch (SortExpression)
            {
                case "AlphaName":
                    relativeValue = (AlphaName != null) ? AlphaName.CompareTo(other.AlphaName) : -1;
                    break;
                case "Company":
                    relativeValue = (Company != null) ? Company.CompareTo(other.Company) : -1;
                    break;
                default:
                    relativeValue = 0;
                    break;
            }
            if (SortDirection == SortDirection.Descending)
            {
                relativeValue *= -1;
            }
            return relativeValue;
        }

        #endregion
    }
}