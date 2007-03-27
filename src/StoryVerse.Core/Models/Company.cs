/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using Castle.ActiveRecord;
using StoryVerse.Core.Lookups;
using System.Collections;

namespace StoryVerse.Core.Models
{
    [ActiveRecord()]
    public class Company : ActiveRecordValidationBase<Company>, IEntity, IComparable<Company>
    {
        private Guid _id;
        private string _name;
        private CompanyType _type;
        private IList<Person> _employeesList;

        [PrimaryKey(PrimaryKeyType.GuidComb, Access=PropertyAccess.NosetterCamelcaseUnderscore)]
        public Guid Id
        {
            get { return _id; }
        }

        [Property, ValidateNotEmpty("Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [Property, ValidateNotEmpty("Type is required.")]
        public CompanyType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        [HasMany(typeof(Person), RelationType = RelationType.Bag, Lazy = true, Cascade = ManyRelationCascadeEnum.All)]
        private IList<Person> EmployeesList
        {
            get
            {
                if (_employeesList == null) _employeesList = new List<Person>();
                return _employeesList;
            }
            set { _employeesList = value; }
        }

        public IList<Person> Employees
        {
            get
            {
                List<Person> result = new List<Person>();
                foreach (Person item in EmployeesList)
                {
                    result.Add(item);
                }
                return result.AsReadOnly();
            }
        }

        public void AddEmployee(Person item)
        {
            if (!EmployeesList.Contains(item))
            {
                EmployeesList.Add(item);
                item.Company = this;
            }
        }

        public void RemoveEmployee(Person item)
        {
            if (EmployeesList.Contains(item))
            {
                EmployeesList.Remove(item);
                item.Company = null;
            }
        }

        public void AddChild<T>(T item) 
            where T : IEntity
        {
            EntityUtility.AddChild(this, item, false);
        }

        public void RemoveChild<T>(T item)
            where T : IEntity
        {
            EntityUtility.RemoveChild(this, item, false);
        }

        public UserProjectScope UserProjectScope
        {
            get
            {
                if (GetDeveloperCompanyTypes().Contains(Type))
                {
                    return UserProjectScope.All;
                }
                else
                {
                    return UserProjectScope.MyCompany;
                }
            }
        }

        public static IList<CompanyType> GetDeveloperCompanyTypes()
        {
            IList<CompanyType> result = new List<CompanyType>();
            result.Add(CompanyType.Us);
            result.Add(CompanyType.Contractor);
            return result;
        }

        public void Validate()
        {
            List<string> messages = EntityUtility.ValidateCollection(Employees);
            if (messages.Count > 0)
            {
                throw EntityUtility.BuildValidationException(messages);
            }
        }

        #region Sorting Members

        private static string _sortExpression = "Name";
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

        public int CompareTo(Company other)
        {
            if (this == other) return 0;
            if (other == null) return 1;
            if (this == null) return -1;

            int relativeValue;
            switch (SortExpression)
            {
                case "Name":
                    relativeValue = Name.CompareTo(other.Name);
                    break;
                case "Type":
                    relativeValue = Type.CompareTo(other.Type);
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