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

            if (Props.Source.Count == 0)
            {
                RenderEmptySection();
            }
            else
            {
                RenderBars();
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

        private void RenderChartEnd()
        {
            RenderText(string.Format("</table></div></div>"));
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

            decimal barPxPerUnit = GetBarLengthPixelsPerDataUnit(Props.Source);
            const decimal labelMaxFontSize = 14m;
            decimal labelFontSize = Props.BarWidthPixels * .8m > labelMaxFontSize
                ? labelMaxFontSize
                : Props.BarWidthPixels * .8m;

            foreach (KeyValuePair<object, decimal> item in Props.Source)
            {
                decimal barLength = barPxPerUnit * item.Value;

                string html = null;
                html += 
                    @"<tr>
                        <td class='label' style='white-space:nowrap; font-size:{3}px;
                                    border-right:solid; border-width:1px; padding:3px 3px 0px 3px;'>{0:" + 
                                    Props.LabelFormat + @"}
                        </td>
                        <td class='value'>" +
                        (barLength.Equals(0) ? null :
                        @"<div class='bar' style='height:{1}px; width:{2}px; display:table;
                                    margin-top:2px; margin-bottom:2px;'>
                            <p class='dataLabel' style='display:table-cell; vertical-align:middle;
                                    height:100%; font-size:{3}px;'>{4}</p>
                          </div>") +
                      @"</td>
                      </tr>";

                html = string.Format(html, item.Key, Props.BarWidthPixels, barLength, labelFontSize, item.Value);
                writer.Write(html);
            }

            RenderText(writer.ToString());
        }

        private void RenderHorizontalBars()
        {
            StringWriter writer = new StringWriter();

            decimal barPxPerUnit = GetBarLengthPixelsPerDataUnit(Props.Source);

            //write bars
            writer.Write("<tr>");
            foreach (KeyValuePair<object, decimal> item in Props.Source)
            {
                decimal barLength = barPxPerUnit * item.Value;

                string html;
                html = string.Format(
                    @"<td classbarLengthvalue' style='vertical-align:bottom;' align='center'>" +
                        (barLength > 0 ? 
                        @"<div class='bar' style='height:{0}px; width:{1}px;
                                margin-right:3px; margin-left:3px;'>
                          <p class='dataLabel' style='text-align:center;'>{2}</p>
                        </div>" : string.Empty) +
                      @"</td>",
                    barPxPerUnit * item.Value, 
                    Props.BarWidthPixels,
                    item.Value);
                writer.Write(html);
            }
            writer.Write("</tr>");

            //write labels
            writer.Write("<tr>");
            foreach (KeyValuePair<object, decimal> item in Props.Source)
            {
                string html = string.Format(
                    @"<td class='label'style='white-space:normal; text-align:center;
                            border-top:solid; border-width:1px; padding:3px 3px 0px 3px;'>{0:" + 
                            Props.LabelFormat + @"}
                    </td>",
                    item.Key);
                writer.Write(html);
            }
            writer.Write("</tr>");

            RenderText(writer.ToString());

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

        private decimal GetBarLengthPixelsPerDataUnit(IDictionary<object, decimal> source)
        {
            decimal maxValue = 0;
            foreach (decimal value in source.Values)
            {
                if (value > maxValue)
                {
                    maxValue = value;
                }
            }
            if (maxValue == 0) return 0;
            return Props.LongestBarPixels / Math.Round(maxValue, 0);
        }
    }
}