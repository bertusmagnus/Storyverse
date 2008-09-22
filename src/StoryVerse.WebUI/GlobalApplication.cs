/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System.Reflection;
using System.Web.Configuration;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using Lunaverse.Tools.Common;

namespace StoryVerse.WebUI
{
	using System;
	using System.Web;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	public class GlobalApplication : HttpApplication, IContainerAccessor
	{
		private static IWindsorContainer container;

		public GlobalApplication()
		{
            BeginRequest += OnBeginRequest;
            EndRequest += OnEndRequest;
		}

		#region IContainerAccessor

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion

		public void Application_OnStart()
		{
			container = new WindsorContainer(new XmlInterpreter());

			try
			{
				Assembly assembly = Assembly.Load("StoryVerse.Core");
				if (assembly == null)
				{
					Exception ex = new Exception("Failed to get core assembly");
					Log.Error(ex);
					throw ex;
				}
				ActiveRecordStarter.Initialize(assembly, 
					ActiveRecordSectionHandler.Instance);
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				throw;
			}
        }

		public void Application_OnEnd() 
		{
			container.Dispose();
		}

        public void OnBeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Items.Add("nh.sessionscope", new SessionScope());            
        }

        public void OnEndRequest(object sender, EventArgs e)
        {
            try
            {
                SessionScope scope = HttpContext.Current.Items["nh.sessionscope"] as SessionScope;
                if (scope != null)
                {
                    //without 'true' all objects in session are flushed (i.e. persisted) automatically
                    scope.Dispose(true);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                HttpContext.Current.Trace.Warn("Error", "EndRequest: " + ex.Message, ex);
            }
        }
	}
}
