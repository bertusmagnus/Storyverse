using System;
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace StoryVerse.WebUI.ViewComponents
{
    public class CheckboxListComponent : ViewComponent
    {
        private bool isHorizontal = false;

        private static readonly string[] sections = new string[]
            {
                "label", "containerStart", "containerEnd", "itemStart", "itemEnd"
            };

        private string CssPrefix
        {
            get { return isHorizontal ? "horizontal" : "vertical"; }
        }

        private string CssClassList
        {
            get
            {
                string cssClass = ComponentParams["cssClass"] as string;
                if (!string.IsNullOrEmpty(cssClass)) return cssClass;
                return string.Format("{0}CheckboxListItem", CssPrefix);
            }
        }

        private string CssClassItem
        {
            get
            {
                string cssClass = ComponentParams["cssClassItem"] as string;
                if (!string.IsNullOrEmpty(cssClass)) return cssClass;
                return string.Format("{0}CheckboxList", CssPrefix);
            }
        }

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

            RenderStart();

            int index = 0;
            foreach (object item in list)
            {
                PropertyBag["item"] = item;
                RenderItemStart();
                RenderText(list.Item());
                string checkboxId = string.Format("{0}_{1}_", target.Replace('.', '_'), index);
                RenderLabel(item, checkboxId);
                RenderItemEnd();
                index++;
            }

            RenderEnd();
        }

        private void RenderStart()
        {
            if (Context.HasSection("containerEnd"))
            {
                RenderSection("containerEnd");
            }
            else
            {
                RenderText(string.Format(
                    "<div class='{0}' style='overflow:auto; white-space:nowrap;'>", 
                    CssClassList));
            }
        }

        private void RenderEnd()
        {
            if (Context.HasSection("containerStart"))
            {
                RenderSection("containerStart");
            }
            else
            {
                RenderText("</div>");
            }
        }

        private void RenderLabel(object item, string forId)
        {
            if (Context.HasSection("label"))
            {
                RenderSection("label");
            }
            else
            {
                RenderText(string.Format("<label for='{0}'>{1}</label>", forId, item));
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
                RenderText(string.Format("<{0} class='{1}'>",
                    isHorizontal ? "span" : "div", CssClassItem));
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