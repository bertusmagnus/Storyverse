/* Copyright © Lunaverse Software 2007  */

using System;
using System.Web.Security;
using Castle.ActiveRecord.Framework;
using Castle.MonoRail.Framework;
using NHibernate.Expression;
using StoryVerse.Core.Models;
using System.Web;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class LoginController : SmartDispatcherController
    {
        public void Index()
        {
        }

        public void LogIn(string username, string password, bool rememberMe, string returnUrl)
        {
            Person user = GetAuthenticUser(username, password);
            if (user != null)
            {
                CancelView();
                Session["user"] = user;
                if (rememberMe)
                {
                    HttpCookie cookie = new HttpCookie("uid", user.Id.ToString());
                    cookie.Expires = DateTime.Now.AddDays(5);
                    HttpContext.Response.SetCookie(cookie);
                }
                if (returnUrl != null)
                {
                    Redirect(returnUrl);
                }
                else
                {
                    RedirectToAction("../projects/list", "companyId=" + user.Company.Id);
                }
            }
            else
            {
                Flash["error"] = "Login failed";
                RedirectToAction("index", returnUrl != null ? "returnUrl=" + returnUrl : string.Empty);
            }
        }

        public void LogOut()
        {
            Session.Clear();
            //HttpContext.Response.Cookies["uid"].Expires = DateTime.Now.AddMinutes(-1);
            RedirectToAction("index");
        }

        private static Person GetAuthenticUser(string username, string password)
        {
            Person user = null;
            if (username != null && username.Trim().Length > 0 &&
                password != null && password.Trim().Length > 0)
            {
                DetachedCriteria crieria = DetachedCriteria.For(typeof(Person));
                crieria.Add(Expression.Eq("Username", username));
                try
                {
                    user = Person.FindOne(crieria);
                    if (user == null || password.Trim() != user.Password.Trim())
                    {
                        return null;
                    }
                }
                catch (ActiveRecordException ex)
                {
                    return null;
                }
            }
            return user;
        }

        //private void AddAuthenticationTicket(Person user)
        //{
            //DateTime expiration = DateTime.Now.AddDays(5);
            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
            //    1, user.Username, DateTime.Now, expiration, 
            //    true, user.Id.ToString());
            //string cookieValue = FormsAuthentication.Encrypt(ticket);
            //Context.Response.CreateCookie(FormsAuthentication.FormsCookieName,
            //    cookieValue, expiration);
        //} 
    }
}
