using System;
using Castle.MonoRail.Framework;
using Lunaverse.Tools.Common;

namespace StoryVerse.WebUI.ViewComponents
{
    public abstract class GroupedListComponent : ViewComponent
    {
        public static readonly string[] sections = new string[]
            {
                "item", "groupHeading", 
                "beginContainer", "endContainer", 
                "beginList", "endList", 
                "beginItem", "endItem"
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

        public void RenderComponent<T>(GroupedCollection<T> source)
        {
            if (source == null)
            {
            	return;
                //throw new ViewComponentException(string.Format(
                //    "GroupedListComponent requires a source of type GroupedCollection<{0}>", typeof(T).Name));
            }

            string cssClass = ComponentParams["cssClass"] as string;

            RenderSection("beginContainer", "<div class='{0}'>", cssClass ?? "groupedList");
            foreach (ItemGroup<T> group in source.Groups)
            {
                foreach (string key in group.Discriminator.Keys)
                {
                    PropertyBag[key.Replace(".", "_")] = group.Discriminator[key];
                }
                RenderSection("groupHeading");
                RenderSection("beginList", "<ul>");
                foreach (T item in group.Items)
                {
                    PropertyBag["item"] = item;
                    RenderSection("beginItem", "<li>");
                    RenderSection("item");
                    RenderSection("endItem", "</li>");
                }
                RenderSection("endList", "</ul>");
            }
            RenderSection("endContainer", "</div>");
        }

        private void RenderSection(string sectionName, string defaultValue, params string[] defaultValueArgs)
        {
            if (HasSection(sectionName))
            {
                RenderSection(sectionName);
            }
            else
            {
                RenderText(string.Format(defaultValue, defaultValueArgs));
            }
        }
    }
}