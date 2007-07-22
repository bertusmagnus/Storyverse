/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System.Collections.Generic;
using Castle.MonoRail.ViewComponents;

namespace StoryVerse.WebUI.ViewComponents
{
    public class BurndownChartProperties: ChartProperties
    {
        public BurndownChartProperties(IDictionary<object, decimal> data) : base(data)
        {
            DataFormat = "#,##0";
            CssClass = "burndown";
            GridUnit = 20;
            BarSpacingPixels = 1;
            PlotHeightPixels = 300;
            LabelFormat = "M/dd/yy (ddd)";
            BarWidthPixels = 10;
            LabelInterval = 7;
            XUnitLabel = "Day";
            YUnitLabel = "Rem-<br/>aining";
        }
    }
}