/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace StoryVerse.WebUI
{
    public class WebObjectCache : IWebObjectCache
    {

        private WebObjectCache() {}

        private static IWebObjectCache cache;
        public static IWebObjectCache GetInstance()
        {
            if (cache == null)
            {
                cache = new WebObjectCache();
            }
            return cache;
        }

        public void Add<T>(string id, T item)
        {
            EnsureSessionExists();
            HttpContext.Current.Session[id] = item;
        }

        public T Retrieve<T>(string id)
        {
            EnsureSessionExists();
            T item = (T) HttpContext.Current.Session[id];
            return item;
        }

        public void Remove(string id)
        {
            EnsureSessionExists(); 
            HttpContext.Current.Session.Remove(id);
        }

        public void Clear()
        {
            EnsureSessionExists();
            HttpContext.Current.Session.Clear();
        }

        private void EnsureSessionExists()
        {
            if (HttpContext.Current.Session == null)
            {
                FormsAuthentication.RedirectToLoginPage();
                HttpContext.Current.Response.End();
            }
        }
    }
}
