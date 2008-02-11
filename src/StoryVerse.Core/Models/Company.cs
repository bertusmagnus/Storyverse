/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System.Collections.Generic;
using Castle.ActiveRecord;
using Castle.Components.Validator;
using StoryVerse.Core.Lookups;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class Company : BaseEntity<Company>
    {
        private string _name;
        private CompanyType _type;
        private IList<Person> _employeesList = new List<Person>();

        [Property, ValidateNonEmpty("Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [Property, ValidateNonEmpty("Type is required.")]
        public CompanyType Type
        {
            get { return _type; }
            set { _type = value; }
        }
        [HasMany(typeof(Person), RelationType = RelationType.Bag, Lazy = true, Cascade = ManyRelationCascadeEnum.All)]
        private IList<Person> EmployeesList
        {
            get { return _employeesList; }
            set { _employeesList = value; }
        }

        public IList<Person> Employees
        {
            get { return new List<Person>(_employeesList).AsReadOnly(); }
        }

        public void AddEmployee(Person item)
        {
            if (!_employeesList.Contains(item))
            {
                _employeesList.Add(item);
                item.Company = this;
            }
        }

        public void RemoveEmployee(Person item)
        {
            if (_employeesList.Contains(item))
            {
                _employeesList.Remove(item);
                item.Company = null;
            }
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

        public override void Validate()
        {
            List<string> messages = EntityUtility.ValidateCollection(Employees);
            if (messages.Count > 0)
            {
                throw EntityUtility.BuildValidationException(messages);
            }
        }

        protected override int GetRelativeValue(Company other)
        {
            switch (SortExpression)
            {
                case "Type":
                    return Type.CompareTo(other.Type);
                default: //default sort by Name
                    return Name.CompareTo(other.Name);
            }
        }
    }
}