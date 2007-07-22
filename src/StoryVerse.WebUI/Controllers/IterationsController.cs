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
using StoryVerse.WebUI.ViewComponents;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class IterationsController : EntityControllerBase<Iteration, IterationCriteria, Project>
    {
        public IterationsController() : base(false) { }

        public override string SortExpression
        {
            get { return Iteration.SortExpression; }
            set { Iteration.SortExpression = value; }
        }

        public override SortDirection SortDirection
        {
            get { return Iteration.SortDirection; }
            set { Iteration.SortDirection = value; }
        }

        [Layout("chart")]
        public void Burndown()
        {
            Iteration iteration = Iteration.Find(new Guid(Context.Params["id"]));
            RefreshContextEntity();

            BurndownChartProperties props = 
                new BurndownChartProperties(ContextEntity.Burndown(iteration));
            props.Title = "Burndown For " + iteration.Name;
            PropertyBag["burndownProps"] = props;
        }


        protected override void SetupNewEntity(Iteration iteration)
        {
            ContextEntity.AddIteration(iteration);
        }

        protected override void SetupEntity(Iteration iteration)
        {
        }
    }
}