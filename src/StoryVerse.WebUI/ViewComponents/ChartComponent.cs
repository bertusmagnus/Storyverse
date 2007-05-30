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

        public override void Render()
        {
            IDictionary<object, decimal> source = ComponentParams["source"] as IDictionary<object, decimal>;
            if (source == null)
            {
                throw new ViewComponentException("The chart component requires a parameter named 'source' which implements IDictionary");
            }

            RenderChartStart();
            
            if (source.Count == 0)
            {
                RenderEmptySection();
            }
            else
            {
                RenderRows(source);
            }
            
            RenderChartEnd();
        }

        private void RenderChartEnd()
        {
            RenderText(string.Format("</table></div></div>"));
        }

        private void RenderChartStart()
        {
            string cssClass = ComponentParams["cssClass"] as string;
            RenderText(string.Format("<div class={0}>", 
                string.IsNullOrEmpty(cssClass) ? "chart" : cssClass));
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
        }

        private void RenderRows(IDictionary<object, decimal> source)
        {
            StringWriter writer = new StringWriter();

            string labelFormat = ComponentParams["labelFormat"] as string;

            foreach (KeyValuePair<object, decimal> item in source)
            {
                string html;
                html = string.Format(
                    @"<tr>
                        <td class='label'>{0" + (string.IsNullOrEmpty(labelFormat) ? null : ":" + labelFormat) + "}" +
                      @"</td>
                        <td class='value'>
                          <div class='bar' style='width:{1}px;'>
                            <span class='dataLabel'>{2}</span>
                          </div>
                        </td>
                      </tr>",
                    item.Key, GetPixelsPersUnit(source) * item.Value, item.Value);
                writer.Write(html);
            }

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

        private decimal GetPixelsPersUnit(IDictionary<object, decimal> source)
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

            int width = 600;
            if (ComponentParams["width"] != null)
            {
                int.TryParse(ComponentParams["width"].ToString(), out width);
            }

            return width/Math.Round(maxValue, 0);
        }


    }
}