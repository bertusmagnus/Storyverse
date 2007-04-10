/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using Castle.ActiveRecord;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Models;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework;
using StoryVerse.Helpers;
using StoryVerse.Core.Criteria;

namespace StoryVerse.WebUI.Controllers
{
    [Helper(typeof(TextFormatHelper))]
    [Filter(ExecuteEnum.BeforeAction, typeof(AuthenticationFilter))]
    public abstract class EntityControllerBase<TEntity, TCriteria, TContextEntity> : ARSmartDispatcherController
        where TEntity : IEntity
        where TContextEntity : IEntity
        where TCriteria : IFindCriteria
    {
        private bool hasContext;
        protected string entityProperName;
        protected string entityName;
        protected string contextEntityName;
        protected string criteriaName;
        protected bool isEditFormLong;

        public EntityControllerBase(bool isEditFormLong)
        {
            entityProperName = typeof(TEntity).Name;
            entityName = entityProperName.ToLower();
            contextEntityName = typeof(TContextEntity).Name.ToLower();
            criteriaName = typeof(TCriteria).Name.ToLower();
            this.isEditFormLong = isEditFormLong;
            hasContext = typeof(TContextEntity).IsClass;
        }

        public string EntityListName
        {
            get { return MakePlural(entityName); }
        }

        public string ContextEntityIdName
        {
            get { return contextEntityName + "Id"; }
        }

        public abstract string SortExpression { get; set;}
        public abstract SortDirection SortDirection { get; set; }

        public void Index()
        {
            SetViewContext();
        }

        [Layout("list")]
        public virtual void List()
        {
            Flash["error"] = null;

            switch (Context.Params["preset"])
            {
                case ("all"):
                    Criteria.ApplyPresetAll();
                    break;
                default:
                    SetCustomPreset(Context.Params["preset"]);
                    break;
            }
            DoList(null);
        }

        [Layout("list")]
        public void List([DataBind("criteria")] TCriteria criteria)
        {
            Flash["error"] = null;

            switch (Form["actionButton"])
            {
                case ("Update"):
                    criteria.OrderAscending = Criteria.OrderAscending;
                    Criteria = criteria;
                    DoList(null);
                    break;
                default:
                    DoList(Form["sortExpression"]);
                    break;
            }
        }

        protected void DoList(string sortExpression)
        {
            SetViewContext();
            try
            {
                if (hasContext)
                {
                    if (Context.Params[ContextEntityIdName] != null)
                    {
                        ContextEntity = GetContextEntity(new 
                            Guid(Context.Params[ContextEntityIdName]));
                    }
                    else
                    {
                        ContextEntity.Refresh();
                    }
                    Criteria.ContextEntity = ContextEntity;
                    PropertyBag[contextEntityName] = ContextEntity;
                }
                PropertyBag["criteria"] = Criteria;
                PopulateListSelects();
                if (sortExpression == null)
                {
                    EntitiesList.Clear();
                    EntitiesList.InsertRange(0, ActiveRecordBase<TEntity>.FindAll(Criteria.ToDetachedCriteria()));
                }
                else
                {
                    SortList(sortExpression);
                }
                PropertyBag[EntityListName] = EntitiesList;
                AddSummary();
            }
            catch (Exception ex)
            {
                HandleListError(ex);
            }
        }

        private void SortList(string expression)
        {
            //if the expression has changed set to ascending, otherwise flip direction
            if (SortExpression != expression || 
                SortDirection != SortDirection.Ascending)
            {
                SortDirection = SortDirection.Ascending;
            }
            else
            {
                SortDirection = SortDirection.Descending;
            }
            SortExpression = expression;
            EntitiesList.Sort();
        }

        protected virtual void SetupSortList(string expression)
        {
        }

        protected virtual void SetCustomPreset(string presetName)
        {
        }

        protected virtual void AddSummary()
        {
        }

        [Layout("new")]
        public virtual void New()
        {
            Flash["error"] = null;
            DoNew();
        }

        [Layout("new")]
        public void New([DataBind("entity")] TEntity entity)
        {
            Flash["error"] = null;
            switch (Form["actionButton"])
            {
                case ("Create"):
                    Create(entity);
                    break;
                case ("Cancel"): 
                    RedirectToList();
                    break;
            }
        }

        protected void DoNew()
        {
            DoNew(default(TEntity), null);
        }

        protected void DoNew(TEntity entity, string actionResult)
        {
            SetViewContext();
            if (hasContext)
            {
                ContextEntity.Refresh();
                PropertyBag[contextEntityName] = ContextEntity;
            }
            PropertyBag["entity"] = entity;
            PropertyBag["actionResult"] = actionResult;
            PropertyBag["entityIsNew"] = true;
            PopulateEditSelects();
        }

        protected void Create(TEntity entity)
        {
            string successMessage = entityProperName + " created";
            string failureMessage = entityProperName + " NOT created";

            if (((Person)Context.CurrentUser).CanViewOnly)
            {
                HandleEditError(new Exception("You do not have create permission"), entity, failureMessage);
                return;
            }

            try
            {
                if (hasContext)
                {
                    ContextEntity.Refresh();
                }
                SetupNewEntity(entity);
                entity.Validate();
                if (hasContext)
                {
                    ContextEntity.UpdateAndFlush();
                }
                else
                {
                    entity.CreateAndFlush();
                }
                RedirectToEdit(entity.Id, successMessage);
            }
            catch (Exception ex)
            {
                HandleNewError(ex, entity, failureMessage);
            }
        }

        protected virtual void SetupNewEntity(TEntity entity)
        {
        }

        [Layout("edit")]
        public virtual void Edit(Guid id)
        {
            Flash["error"] = null;
            try
            {
                DoEdit(ActiveRecordBase<TEntity>.Find(id), null);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        [Layout("edit")]
        public virtual void Edit([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NullIfInvalidKey)] TEntity entity)
        {
            DoEditAction(entity);
        }

        protected void DoEditAction(TEntity entity)
        {
            Flash["error"] = null;
            switch (Form["actionButton"])
            {
                case ("Save"):
                    Update(entity);
                    break;
                case ("Delete"):
                    Delete(entity);
                    break;
                case ("List"):
                    RedirectToList();
                    break;
                default:
                    DoCustomEditAction(entity);
                    break;
            }
        }

        protected void DoEdit(TEntity entity)
        {
            DoEdit(entity, null);
        }

        protected void DoEdit(TEntity entity, string actionResult)
        {
            try
            {
                SetupEntity(entity);
                SetViewContext();
                PropertyBag["actionResult"] = actionResult;
                PropertyBag["entity"] = entity;
                PropertyBag["previousId"] = GetPreviousId(entity);
                PropertyBag["nextId"] = GetNextId(entity);
                PropertyBag["entityIsNew"] = false;
                if (hasContext)
                {
                    //ToDo: this is a HACK.  It should not be needed, not should Refresh.  It 
                    //prevents a lazy load exception in the case of a redirect after a create.
                    ContextEntity = GetContextEntity(ContextEntity.Id);
                    
                    PropertyBag[contextEntityName] = ContextEntity;
                }
                PopulateEditSelects();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        protected virtual void SetupEntity(TEntity entity)
        {
        }

        protected virtual void PopulateListSelects()
        {
        }

        protected virtual void PopulateEditSelects()
        {
        }

        protected virtual void DoCustomEditAction(TEntity entity)
        {
        }

        protected TEntity GetEntity(Guid id)
        {
            foreach (TEntity entity in EntitiesList)
            {
                if (entity.Id == id)
                {
                    return entity;
                }
            }
            return default(TEntity);
        }

        private Guid? GetNextId(TEntity entity)
        {
            if (EntitiesList == null) return null;

            Guid? result = null;
            foreach (TEntity item in EntitiesList)
            {
                if (item.Id == entity.Id)
                {
                    int index = EntitiesList.IndexOf(item);
                    if (index < EntitiesList.Count - 1) result = EntitiesList[index + 1].Id;
                }
            }
            return result;
        }

        private Guid? GetPreviousId(TEntity entity)
        {
            if (EntitiesList == null) return null;

            Guid? result = null;
            foreach (TEntity item in EntitiesList)
            {
                if (item.Id == entity.Id)
                {
                    int index = EntitiesList.IndexOf(item);
                    if (index > 0) result = EntitiesList[index - 1].Id;
                }
            }
            return result;
        }

        protected void Update(TEntity entity)
        {
            string successMessage = entityProperName + " saved";
            string failureMessage = entityProperName + " NOT saved";

            if (((Person)Context.CurrentUser).CanViewOnly)
            {
                HandleEditError(new Exception("You do not have update permission"), entity, failureMessage);
                return;
            }

            try
            {
                SetupUpdateEntity(entity);
                entity.Validate();
                entity.UpdateAndFlush();
                DoEdit(entity, successMessage);
            }
            catch (Exception ex)
            {
                HandleEditError(ex, entity, failureMessage);
            }
        }

        protected virtual void SetupUpdateEntity(TEntity entity)
        {
        }

        protected virtual void Delete(TEntity entity)
        {
            string failureMessage = entityProperName + " NOT deleted";

            if (((Person)Context.CurrentUser).CanViewOnly)
            {
                HandleEditError(new Exception("You do not have delete permission"), entity, failureMessage);
                return;
            }

            try
            {
                entity.DeleteAndFlush();
                RedirectToList();
            }
            catch (Exception ex)
            {
                HandleEditError(ex, entity, failureMessage);
            }
        }

        protected static T SetEntityValue<T>(string id)
        {
            Guid idGuid;
            try
            {
                idGuid = new Guid(id);
            }
            catch
            {
                return default(T);
            }
            return ActiveRecordBase<T>.Find(idGuid);
        }

        protected void HandleEditError(Exception ex, TEntity entity, string actionResult)
        {
            ShowError(ex);
            using (new SessionScope(FlushAction.Never))
            {
                DoEdit(entity, actionResult);
            }
        }

        protected void HandleNewError(Exception ex, TEntity entity, string actionResult)
        {
            ShowError(ex);
            using (new SessionScope(FlushAction.Never))
            {
                DoNew(entity, actionResult);
            }
        }

        protected void HandleListError(Exception ex)
        {
            ShowError(ex);
            using (new SessionScope(FlushAction.Never))
            {
                DoList(null);
            }
        }

        protected void ShowError(Exception ex)
        {
            string message = GetErrorMessage(ex);
            Flash["error"] = message.Trim();
        }

        protected static string GetErrorMessage(Exception ex)
        {
            string message = string.Empty;
            if (ex.GetBaseException().GetType() == typeof(ValidationException))
            {
                foreach (string item in ((ValidationException)ex.GetBaseException()).ValidationErrorMessages)
                {
                    message += item + "<br>";
                }
            }
            else
            {
                message = ex.ToString();
            }
            return message;
        }

        private const string contextCookieName = "contextId";

        private TContextEntity GetContextEntity(Guid id)
        {
            TContextEntity result = ActiveRecordBase<TContextEntity>.Find(id);
            HttpCookie cookie = new HttpCookie(contextCookieName, id.ToString());
            cookie.Expires = DateTime.MaxValue;
            HttpContext.Response.SetCookie(cookie);
            return result;
        }

        protected TContextEntity ContextEntity
        {
            get
            {
                TContextEntity result = WebObjectCache.GetInstance().Retrieve<TContextEntity>(contextEntityName);
                if (result == null)
                {
                    string cookieValue = Request.ReadCookie(contextCookieName);
                    result = GetContextEntity(new Guid(cookieValue));
                    ContextEntity = result;
                }
                return result;
            }
            set { WebObjectCache.GetInstance().Add(contextEntityName, value); }
        }

        protected List<TEntity> EntitiesList
        {
            get
            {
                List<TEntity> result = WebObjectCache.GetInstance().Retrieve<List<TEntity>>(EntityListName);
                if (result == null)
                {
                    result = new List<TEntity>();
                    EntitiesList = result;
                }
                return result;
            }
            set { WebObjectCache.GetInstance().Add(EntityListName, value); }
        }

        protected TCriteria Criteria
        {
            get
            {
                TCriteria result = WebObjectCache.GetInstance().Retrieve<TCriteria>(criteriaName);
                if (result == null)
                {
                    result = (TCriteria)Activator.CreateInstance(typeof(TCriteria));
                    Criteria = result;
                }
                return result;
            }
            set { WebObjectCache.GetInstance().Add(criteriaName, value); }
        }

        protected void RedirectToList()
        {
            RedirectToAction("list");
        }

        protected void RedirectToEdit(Guid entityId, string actionResult)
        {
            RedirectToAction("edit", new string[] {"id=" + entityId, "actionResult=" + actionResult});
        }

        protected void SetViewContext()
        {
            PropertyBag["userIsAdmin"] = ((Person)Context.CurrentUser).IsAdmin;
            PropertyBag["userCanEdit"] = !((Person)Context.CurrentUser).CanViewOnly;
            PropertyBag["contextEntityName"] = contextEntityName;
            string entityNameProper = char.ToUpper(entityName[0]) + entityName.Substring(1);
            PropertyBag["entityName"] = entityNameProper;
            PropertyBag["entityNamePlural"] = MakePlural(entityNameProper);
            PropertyBag["isEditFormLong"] = isEditFormLong;
        }

        protected void AddListSum(string propertyName)
        {
            PropertyBag["total" + propertyName] = GetListSum(propertyName);
        }

        protected void AddListAverage(string propertyName)
        {

            PropertyBag["average" + propertyName] = 
                (GetListSum(propertyName) / EntitiesList.Count);
        }

        protected double GetListSum(string propertyName)
        {
            double sum = 0;
            foreach (TEntity item in EntitiesList)
            {
                sum += Convert.ToDouble(
                    typeof (TEntity).GetProperty(propertyName)
                        .GetValue(item, null));
            }
            return sum;
        }

        private static string MakePlural(string name)
        {

            Regex plural1 = new Regex("(?<keep>[^aeiou])y$");
            Regex plural2 = new Regex("(?<keep>[aeiou]y)$");
            Regex plural3 = new Regex("(?<keep>[sxzh])$");
            Regex plural4 = new Regex("(?<keep>[^sxzhy])$");

            if (plural1.IsMatch(name))
                return plural1.Replace(name, "${keep}ies");
            else if (plural2.IsMatch(name))
                return plural2.Replace(name, "${keep}s");
            else if (plural3.IsMatch(name))
                return plural3.Replace(name, "${keep}es");
            else if (plural4.IsMatch(name))
                return plural4.Replace(name, "${keep}s");

            return name;
        }

        public class SelectItem
        {
            public object Id;
            public string Text;
            public SelectItem(object id, string text)
            {
                Id = id;
                Text = text;
            }
        }

        protected static T NullifyIfTransient<T>(T entity) where T : IEntity
        {
            if (entity.Id == Guid.Empty)
            {
                return default(T);
            }
            else
            {
                return entity;
            }
        }
    }
}