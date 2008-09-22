/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using Castle.ActiveRecord;
using Castle.Components.Validator;
using NHibernate.Expression;

namespace StoryVerse.Core.Models
{
    [ActiveRecord]
    public class AppSetting : BaseEntity<AppSetting>
    {
        private string _name;
        private string _value;

        [Property, ValidateNonEmpty("Name is required.")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [Property(SqlType = "nvarchar(MAX)")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public static string GetSettingValue(AppSettingName name)
        {
            AppSetting setting = GetSetting(name);
            if (setting == null)
            {
                //throw new Exception(string.Format("Setting named '{0}' does not exist.", name));
                return null;
            }
            return setting.Value;
        }

        private static AppSetting GetSetting(AppSettingName name)
        {
            return FindOne(new EqExpression("Name", name.ToString()));
        }

        public static void SaveSetting(AppSettingName name, object value)
        {
            AppSetting setting = GetSetting(name);
            if (setting == null)
            {
                setting = new AppSetting();
                setting.Name = name.ToString();
                setting.Value = value.ToString();
                CreateAndFlush(setting);
            }
            else
            {
                setting.Value = value.ToString();
            }
        }

        protected override int GetRelativeValue(AppSetting other)
        {
            switch (SortExpression)
            {
                default: //default sort by Name
                    return Name.CompareTo(other.Name);
            }
        }
    }

    public enum AppSettingName
    {
        MaxIssueAttachmentSizeKb,
        SmtpServer
    }
}