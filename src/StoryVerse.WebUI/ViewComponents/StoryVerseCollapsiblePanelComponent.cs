using Castle.MonoRail.ViewComponents;

namespace StoryVerse.WebUI.ViewComponents
{
    public class StoryVerseCollapsiblePanelComponent : CollapsiblePanelComponent
    {
        public override void Initialize()
        {
            Context.ComponentParameters.Add("expandImagePath", "../Images/expand.jpg");
            Context.ComponentParameters.Add("collapseImagePath", "../Images/collapse.jpg");
            //Context.ComponentParameters.Add("effect", "appear");

            base.Initialize();
        }
    }
}