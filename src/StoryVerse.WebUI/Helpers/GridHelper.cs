/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace StoryVerse.WebUI.Helpers
{
    public class GridHelper
    {

        public string Grid(GridInfo gridInfo)
        {
            string headerCellTemplate = "<td style='font-weight:bold'>{0}</td>";
            string cellTemplate = "<td>{0}</td>";
            string result;
            result = "<table><tr>";
            foreach (string cell in gridInfo.ColumnHeaders)
            {
                result += string.Format(headerCellTemplate, cell);
            }
            result += "</tr>";
            foreach (IList<string> row in gridInfo.Data)
            {
                result += "<tr>";
                foreach (string cell in row)
                {
                    result += string.Format(cellTemplate, cell);
                }
                result += "</tr>";
            }
            return result;
        }

        public struct GridInfo
        {
            private IList<IList<string>> content;

            public IList<string> ColumnHeaders
            {
                get { return content[0]; }
                set { content[0] = value;  }
            }
            public IList<IList<string>> Data
            {
                set
                {
                    if (content == null) content = new List<IList<string>>();
                    IList<string> columnHeadersTHold = ColumnHeaders;
                    content.Clear();
                    content.Add(columnHeadersTHold);
                    foreach (string[] item in value)
                    {
                        content.Add(item);
                    }
                }
                get
                {
                    IList<IList<string>> result = content;
                    result.RemoveAt(0);
                    return result;
                }
            }
        }
    }
}
