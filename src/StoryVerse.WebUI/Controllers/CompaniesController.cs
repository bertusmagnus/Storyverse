/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Criteria;
using StoryVerse.Core.Models;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("admin"), Rescue("generalerror")]
    public class CompaniesController : EntityControllerBase<Company, CompanyCriteria, IEntity>
    {
        public CompaniesController() : base(false) { }

        public override string SortExpression
        {
            get { return Company.SortExpression; }
            set { Company.SortExpression = value; }
        }

        public override SortDirection SortDirection
        {
            get { return Company.SortDirection; }
            set { Company.SortDirection = value; }
        }

        protected override void PopulateEditSelects()
        {
            PropertyBag["companyTypes"] = Enum.GetValues(typeof(CompanyType));
        }

        protected override void SetupNewEntity(Company company)
        {
            //company.Type = SetEntityValue<Company>(Form["entity.Company.Id"]);
        }
    }
}