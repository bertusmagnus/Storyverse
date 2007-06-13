using System;
using System.Collections;
using System.IO;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace StoryVerse.WebUI.ViewComponents
{
    public class CheckboxListComponent : ViewComponent
    {
        private bool isHorizontal = false;

        private static readonly string[] sections = new string[]
            {
                "label", "itemStart", "itemEnd"
            };

        public override bool SupportsSection(string name)
        {
            foreach (string section in sections)
            {
                if (section.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Render()
        {
            string target = ComponentParams["target"] as string;
            if (target == null)
            {
                throw new ViewComponentException("The checkbox list component requires a view component parameter named 'target' that is a string");
            }

            IEnumerable source = ComponentParams["source"] as IEnumerable;
            if (source == null)
            {
                throw new ViewComponentException("The checkbox list component requires a parameter named 'source' that implements 'IEnumerable'");
            }

            string orientation = ComponentParams["orientation"] as string;
            isHorizontal = orientation != null &&
                (orientation.ToLower() == "horizontal" || 
                orientation.ToLower() == "h" ||
                orientation.ToLower() == "horiz");

            FormHelper helper = (FormHelper)Context.ContextVars["FormHelper"];

            FormHelper.CheckboxList list = helper.CreateCheckboxList(
                target, source, GetHtmlAttibutes());

            foreach (object item in list)
            {
                PropertyBag["item"] = item;
                RenderItemStart();
                RenderText(list.Item());
                RenderLabel(item);
                RenderItemEnd();
            }
        }

        private void RenderLabel(object item)
        {
            if (Context.HasSection("label"))
            {
                RenderSection("label");
            }
            else
            {
                RenderText(item.ToString());
            }
        }

        private void RenderItemEnd()
        {
            if (Context.HasSection("itemEnd"))
            {
                RenderSection("itemEnd");
            }
            else
            {
                if (isHorizontal)
                {
                    RenderText("</span>");
                }
                else
                {
                    RenderText("</div>");
                }
            }
        }

        private void RenderItemStart()
        {
            if (Context.HasSection("itemStart"))
            {
                RenderSection("itemStart");
            }
            else
            {
                if (isHorizontal)
                {
                    RenderText("<span class='horizontalCheckboxListItem'>");
                }
                else
                {
                    RenderText("<div class='verticalCheckboxListItem'>");
                }
            }
        }

        private IDictionary GetHtmlAttibutes()
        {
            IDictionary result = new Hashtable();
            foreach (DictionaryEntry entry in ComponentParams)
            {
                if (entry.Key.ToString() != "source" && 
                    entry.Key.ToString() != "target" &&
                    entry.Key.ToString() != "orientation")
                {
                    result.Add(entry.Key, entry.Value);
                }
            }
            return result;
        }
    }
}