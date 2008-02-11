using Castle.MonoRail.Framework;
using Lunaverse.Tools.Common;
using StoryVerse.Core.Models;

namespace StoryVerse.WebUI.ViewComponents
{
    public class IssueNoteGroupedListComponent : GroupedListComponent
    {
        public override void Render()
        {
            GroupedCollection<IssueNote> source = ComponentParams["source"] as GroupedCollection<IssueNote>;
            RenderComponent(source);
        }
    }
}