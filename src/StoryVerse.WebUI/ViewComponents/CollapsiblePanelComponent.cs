using System;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using Castle.MonoRail.ViewComponents;

namespace StoryVerse.WebUI.ViewComponents
{
    public class CollapsiblePanelComponent : ViewComponentEx
    {
        private static readonly string[] sections = new string[] { "content", "heading", };

        public override bool SupportsSection(string name)
        {
            foreach (string section in sections)
            {
                if (section.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        private const string javascriptFunctionName = "showHideCollapsiblePanel";
        private const string wasJsRenderedKey = "collapsiblePanelComponentJsRendered";

        private string id;
        private string showButtonImagePath;
        private string hideButtonImagePath;
        private bool toggleOnClickHeader;
        private string cssClass;
        private string style;
        private bool collapsed;
        private string initialCommand;
        private string panelId;
        private string toggleId;
        private string javascriptCall;
        private string initialImage;
        private bool useImage;
        private string effect;
        private decimal effectDuration;

        public override void Initialize()
        {
            GetParameters();

            initialCommand = collapsed ? "Show" : "Hide";
            panelId = string.Format("{0}Panel", id);
            toggleId = string.Format("{0}Toggle", id);
            javascriptCall = string.Format("javascript:{0}(\"{1}\", \"{2}\")",
                javascriptFunctionName, panelId, toggleId);
            initialImage = collapsed ? showButtonImagePath : hideButtonImagePath;
            useImage =
                !string.IsNullOrEmpty(showButtonImagePath) ||
                !string.IsNullOrEmpty(hideButtonImagePath);

            base.Initialize();
        }

        private void GetParameters()
        {
            id = ComponentParams["id"] as string;
            if (id == null)
            {
                throw new ViewComponentException("CollapsiblePanelComponent: 'id' parameter is required");
            }
            showButtonImagePath = ComponentParams["showButtonImagePath"] as string;
            hideButtonImagePath = ComponentParams["hideButtonImagePath"] as string;
            cssClass = ComponentParams["cssClass"] as string;
            style = ComponentParams["style"] as string;
            collapsed = GetBoolParamValue("collapsed", false);
            effect = ComponentParams["effect"] as string ?? "blind";
            effectDuration = ComponentParams["effectDuration"] as decimal? ?? 0.4m;
            toggleOnClickHeader = GetBoolParamValue("toggleOnClickHeader", false);
        }

        public override void Render()
        {
            RenderComponent();
            RenderJavascript();
        }

        private void RenderComponent()
        {
            RenderTextFormat("<div id='{0}' class='{1}'{2}>", id,
                !string.IsNullOrEmpty(cssClass) 
                    ? cssClass 
                    : "collapsiblePanel",
                !string.IsNullOrEmpty(style) 
                    ? string.Format("style='{0}'", style) 
                    : null);

            RenderHeader();

            RenderContent();

            RenderText("</div>");
        }

        private void RenderJavascript()
        {
            if (Context.ContextVars[wasJsRenderedKey] as bool? != true)
            {
                ScriptaculousHelper helper = new ScriptaculousHelper();
                helper.SetController(RailsContext.CurrentController);
                RenderText(helper.InstallScripts());

                RenderText(AjaxHelper.ScriptBlock(ToggleJsFunction));
                Context.ContextVars[wasJsRenderedKey] = true;
            }
        }

        private void RenderHeader()
        {
            string toolTipAttribute = "title='Click to show/hide'";

            RenderTextFormat("<div class='header'{0}>", 
                toggleOnClickHeader 
                    ? string.Format(" onclick='{0}' style='cursor:pointer;' {1}",
                        javascriptCall, toolTipAttribute) 
                    : null);

            if (useImage)
            {
                RenderTextFormat("<img id='{0}' src='{1}' class='toggleImage' " + 
                                         "onclick='{2}' alt='{3}' {4}/>",
                    toggleId, initialImage, javascriptCall, initialCommand, toolTipAttribute);
            }

            if (Context.HasSection("heading"))
            {
                RenderSection("heading");
            }

            if (!useImage && !toggleOnClickHeader)
            {
                RenderTextFormat("<a id='{0}' href='{1}' class='toggleLink'>{2}</a>",
                    toggleId, javascriptCall, initialCommand);
            }

            RenderText("</div>");
        }

        private void RenderContent()
        {
            RenderTextFormat("<div id='{0}' class='content'{1}>",
                panelId, collapsed ? "style='display:none'" : null);

            if (Context.HasSection("content"))
            {
                RenderSection("content");
            }

            RenderText("</div>");
        }

        private string ToggleJsFunction
        {
            get
            {
                return string.Format(
@"
function {0}(controlName, togglerName)
{{
    new Effect.toggle(controlName, '{1}', {{duration:{2}}});
    var toggler = document.getElementById(togglerName);
    if (toggler.{3} == 'Hide')
    {{
        toggler.{3} = 'Show';
        if (toggler.tagName == 'IMG')
        {{
            toggler.src = '{4}';
        }}
    }}
    else if (toggler.{3} == 'Show')
    {{
        toggler.{3} = 'Hide';
        if (toggler.tagName == 'IMG')
        {{
            toggler.src = '{5}';
        }}
    }}
}}
",
                javascriptFunctionName, effect, effectDuration, 
                useImage ? "alt" : "innerHTML",
                showButtonImagePath, hideButtonImagePath);
            }
        }
    }
}