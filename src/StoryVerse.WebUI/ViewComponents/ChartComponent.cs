/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.IO;
using Castle.MonoRail.Framework;
using System.Collections.Generic;

namespace StoryVerse.WebUI.ViewComponents
{
    public class ChartComponent : ViewComponent
    {
        private static readonly string[] sections = new string[]
            {
                "title", "empty"
            };

        private static readonly string verticalBarTemplate =
            @"<div class='bar' style='height:{1}px; width:{0}px; 
                 display:table; margin-top:2px; margin-bottom:2px;'>
                 <p class='dataLabel' style='display:table-cell; 
                     vertical-align:middle; height:100%; font-size:{2}px;'>
                    {3}
                 </p>
              </div>";

        private static readonly string horizontalBarTemplate =
            @"<div class='bar' style='height:{0}px; width:{1}px;
                 margin-right:3px; margin-left:3px;'>
                 <p class='dataLabel' style='text-align:center; font-size:{2}px;'>
                    {3}
                 </p>
              </div>";

        private static readonly string verticalLabelTemplate =
            @"<td class='label' style='white-space:nowrap; font-size:{0}px;
                    border-right:solid; border-width:1px; 
                    padding:3px 3px 0px 3px;'>
                {1}
              </td>";

        private static readonly string horizontalLabelTemplate =
            @"<td class='label'style='white-space:normal; text-align:center;
                    font-size:{0}px; border-top:solid; border-width:1px; 
                    padding:3px 3px 0px 3px;'>
                {1}
            </td>";

        private decimal? barLengthPixelsPerDataUnitCached;

        private decimal? fontSizeChached;

        public override bool SupportsSection(string name)
        {
            foreach (string section in sections)
            {
                if (section.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        private ChartProperties Props
        {
            get { return ComponentParams["properties"] as ChartProperties; }
        }

        public override void Render()
        {
            if (Props == null)
            {
                throw new ViewComponentException("The chart component requires a parameter named 'properties' of type ChartProperties");
            }

            if (Props.Source == null)
            {
                throw new ViewComponentException("The chart component requires a parameter named 'source' which implements IDictionary");
            }

            RenderChartStart();

            if (Props.Source.Count > 0)
            {
                RenderBars();
            }
            else
            {
                RenderEmptySection();
            }

            RenderChartEnd();
        }

        private void RenderChartStart()
        {
            RenderText(string.Format("<div class={0} style='position:absolute'>", Props.CssClass));
            RenderTitleSection();
            RenderText("<div class='chartArea'>");
            RenderText("<table class='plotArea'>");
        }

        private void RenderTitleSection()
        {
            if (Context.HasSection("title"))
            {
                RenderSection("title");
            }
            else if (!string.IsNullOrEmpty(Props.Title))
            {
                RenderText(string.Format("<div class='title'>{0}</div>", Props.Title));
            }
        }

        private void RenderChartEnd()
        {
            RenderText(string.Format("</table></div></div>"));
        }

        private void RenderBars()
        {
            if (Props.Orientation == ChartOrientation.Vertical)
            {
                RenderVerticalBars();
            }
            else
            {
                RenderHorizontalBars();
            }
        }

        private void RenderVerticalBars()
        {
            StringWriter writer = new StringWriter();

            foreach (KeyValuePair<object, decimal> item in Props.Source)
            {
                string html = string.Format(
                     "<tr>" +
                        GetLabel(item.Key) +
                        @"<td class='value'>" +
                            GetBar(item.Value) +
                        @"</td>
                      </tr>", 
                      FontSize, item.Key);

                writer.Write(html);
            }

            RenderText(writer.ToString());
        }

        private string GetLabel(object labelValue)
        {
            string template = null;
            switch (Props.Orientation)
            {
                case ChartOrientation.Vertical:
                    template = verticalLabelTemplate;
                    break;
                case ChartOrientation.Horizontal:
                    template = horizontalLabelTemplate;
                    break;
            }
            return string.Format(template, FontSize,
                string.Format("{0:" + Props.LabelFormat + "}", labelValue));
        }

        private void RenderHorizontalBars()
        {
            StringWriter writer = new StringWriter();

            //write bars
            writer.Write("<tr>");
            foreach (KeyValuePair<object, decimal> item in Props.Source)
            {
                string html;
                html =
                    @"<td class='value' style='vertical-align:bottom;' align='center'>" +
                        GetBar(item.Value) +
                    @"</td>";
                writer.Write(html);
            }
            writer.Write("</tr>");

            //write labels
            writer.Write("<tr>");
            foreach (KeyValuePair<object, decimal> item in Props.Source)
            {
                writer.Write(GetLabel(item.Key));
            }
            writer.Write("</tr>");

            RenderText(writer.ToString());

        }

        private string GetBar(decimal itemValue)
        {
            decimal barLength = BarLengthPixelsPerDataUnit * itemValue;

            if (barLength.Equals(0)) return null;

            string template = null;
            switch (Props.Orientation)
            {
                case ChartOrientation.Vertical:
                    template = verticalBarTemplate;
                    break;
                case ChartOrientation.Horizontal:
                    template = horizontalBarTemplate;
                    break;
            }
            return string.Format(template, barLength, Props.BarWidthPixels, FontSize, itemValue);

        }

        private void RenderEmptySection()
        {
            if (Context.HasSection("empty"))
            {
                RenderSection("empty");
            }
            else
            {
                RenderText("<tr><td class='empty_message'>No data to display</td></tr>");
            }
        }

        private decimal BarLengthPixelsPerDataUnit
        {
            get
            {
                if (!barLengthPixelsPerDataUnitCached.HasValue)
                {
                    decimal maxValue = 0;
                    foreach (decimal value in Props.Source.Values)
                    {
                        if (value > maxValue)
                        {
                            maxValue = value;
                        }
                    }
                    if (maxValue == 0) return 0;
                    barLengthPixelsPerDataUnitCached =
                        Props.LongestBarPixels / Math.Round(maxValue, 0);
                }
                return barLengthPixelsPerDataUnitCached.Value;
            }
        }

        private string FontSize
        {
            get
            {
                switch (Props.Orientation)
                {
                    case ChartOrientation.Vertical:
                        if (!fontSizeChached.HasValue)
                        {
                            const decimal maxFontSize = 14m;
                            decimal relativeFontSize = Props.BarWidthPixels * .8m;
                            fontSizeChached = Props.BarWidthPixels * relativeFontSize > maxFontSize
                                       ? maxFontSize
                                       : relativeFontSize;
                        }
                        return fontSizeChached.Value.ToString();
                    case ChartOrientation.Horizontal:
                    default:
                        return "normal";
                }
            }
        }
    }
}