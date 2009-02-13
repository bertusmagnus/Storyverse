using Castle.MonoRail.Framework;
using Lunaverse.Tools.Common;
using StoryVerse.Core.Models;

namespace StoryVerse.WebUI.ViewComponents
{
    public class IssueChangeGroupedListComponent : GroupedListComponent
    {
        public override void Render()
        {
            GroupedCollection<IssueChange> source = ComponentParams["source"] as GroupedCollection<IssueChange>;
            if (source == null)
            {
                ViewComponentException ex = new ViewComponentException("IssueNoteGroupedListComponent requires a source of type GroupedCollection<IssueChange>");
                Log.Error(ex);
                //throw ex;
            }
            RenderComponent(source);
        }
    }
}