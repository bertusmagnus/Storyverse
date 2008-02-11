/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Criteria;
using StoryVerse.Core.Models;
using Castle.MonoRail.Framework;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("admin"), Rescue("generalerror")]
    public class CompaniesController : EntityControllerBase<Company, CompanyCriteria, IEntity>
    {
        public CompaniesController() : base(false) { }

        protected override void PopulateEditSelects(Company company)
        {
            PropertyBag["companyTypes"] = Enum.GetValues(typeof(CompanyType));
        }
    }
}