using System;
using System.Collections;
using System.IO;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace StoryVerse.WebUI.ViewComponents
{
    public class CheckboxListComponent : ViewComponent
    {
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

            FormHelper.CheckboxList list = new FormHelper().CreateCheckboxList(target, source);   

            foreach (object item in list)
            {
                RenderView(string.Format("{0}{1}<br/>)", list.Item(), item ));
            }
        }
    }
}