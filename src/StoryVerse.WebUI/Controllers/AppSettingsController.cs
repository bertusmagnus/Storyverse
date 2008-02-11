/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using Lunaverse.Tools.Common;
using StoryVerse.Core.Models;
using Castle.MonoRail.Framework;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class AppSettingsController : SmartDispatcherController
    {
        public void Index(Company company)
        {
            PropertyBag["maxIssueAttachmentSize"] = AppSetting.GetSettingValue(AppSettingName.MaxIssueAttachmentSizeKb);
        }

        public void SaveMaxIssueAttachmentSize(string maxIssueAttachmentSize)
        {
            SaveSetting(AppSettingName.MaxIssueAttachmentSizeKb, maxIssueAttachmentSize);
        }

        private void SaveSetting(AppSettingName settingName, string maxIssueAttachmentSize)
        {
            try
            {
                AppSetting.SaveSetting(settingName, maxIssueAttachmentSize);
                RenderText("Saved");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                RenderText("NOT saved<br><br>Error Details:<br>{0}", ex.ToString());
            }
            CancelLayout();
        }
    }
}