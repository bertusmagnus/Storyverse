/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System.Security.Principal;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using StoryVerse.Core.Lookups;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class Person : BaseEntity<Person>, IPrincipal
    {
        private string _firstName;
        private string _lastName;
        private string _username;
        private string _password;
        private string _email;
        private Company _company;
        private UserPreferences _userPreferences = new UserPreferences();
        private bool _isAdmin;
        private bool _canViewOnly;

        [Property, ValidateNonEmpty("First Name is required.")]
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        [Property, ValidateNonEmpty("Last Name is required.")]
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
        public string Email
        {
            get { return _email; }
            set { _email = value; }
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

        [BelongsTo, ValidateNonEmpty("Company is required.")]
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

         protected override int GetRelativeValue(Person other)
        {
            switch (SortExpression)
            {
                case "Company":
                    return (Company != null) ? Company.CompareTo(other.Company) : -1;
                default:
                    return (AlphaName != null) ? AlphaName.CompareTo(other.AlphaName) : -1;
            }
        }
    }
}