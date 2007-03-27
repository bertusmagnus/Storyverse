/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Web;
using System.Web.Security;
using Castle.MonoRail.Framework;
using StoryVerse.Core.Models;

namespace StoryVerse.WebUI
{
    public class AuthenticationFilter : IFilter
    {
        public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
        {
            Person user = (Person)context.Session["user"];
            context.CurrentUser = user;
            if (context.CurrentUser == null)
            {
                string cookieValue = controller.Request.ReadCookie("uid");
                if (cookieValue != null)
                {
                    user = Person.TryFind(new Guid(cookieValue));
                }
                if (user == null)
                {
                    controller.Redirect("../login/index.rails");
                    return false;
                }
                context.CurrentUser = user;
            }

            if (context.CurrentUser == null ||
                !context.CurrentUser.Identity.IsAuthenticated)
            {
                controller.Redirect("../login/index.rails");
                return false;
            }
            return true;
        }

        private static Guid? GetUserId(HttpCookie cookie)
        {
            try
            {
                FormsAuthenticationTicket ticket =
                    FormsAuthentication.Decrypt(cookie.Value);
                return new Guid(ticket.UserData);
            }
            catch (ArgumentException ex)
            {
                return null;
            }

        }
    }
}