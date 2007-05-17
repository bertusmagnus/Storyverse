/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using System.Web;
using Castle.ActiveRecord;
using StoryVerse.Common.Utilities;
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
        #region private/protected fields

        private bool _hasContext;
        protected string _entityProperName;
        protected string _entityName;
        protected string _contextEntityName;
        protected string _criteriaName;
        protected bool _isEditFormLong;
        private const string contextCookieName = "contextId";
        
        #endregion

        #region constructors

        public EntityControllerBase(bool isEditFormLong)
        {
            _entityProperName = typeof(TEntity).Name;
            _entityName = _entityProperName.ToLower();
            _contextEntityName = typeof(TContextEntity).Name.ToLower();
            _criteriaName = typeof(TCriteria).Name.ToLower();
            this._isEditFormLong = isEditFormLong;
            _hasContext = typeof(TContextEntity).IsClass;
        } 

        #endregion

        #region public properties

        public string EntityListName
        {
            get { return TextUtil.MakePlural(_entityName); }
        }

        public string ContextEntityIdName
        {
            get { return _contextEntityName + "Id"; }
        } 

        #endregion

        #region list action

        [Layout("list")]
        public virtual void List()
        {
            string preset = Context.Params["preset"];
            switch (preset)
            {
                case null:
                case "":
                    break;
                case "all":
                    Criteria.ApplyPresetAll();
                    break;
                default:
                    SetCustomFilterPreset(preset);
                    break;
            }
            DoList();
        }

        [Layout("list")]
        public void Search([DataBind("criteria")] TCriteria criteria)
        {
            criteria.OrderAscending = Criteria.OrderAscending;
            Criteria = criteria;
            RedirectToList();
        }

        protected void DoList()
        {
            SetViewContext();
            try
            {
                if (_hasContext)
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
                    PropertyBag[_contextEntityName] = ContextEntity;
                }
                PropertyBag["criteria"] = Criteria;
                PopulateFilterSelects();
                if (string.IsNullOrEmpty(Form["sortExpression"]))
                {
                    EntitiesList.Clear();
                    EntitiesList.InsertRange(0, ActiveRecordBase<TEntity>.FindAll(Criteria.ToDetachedCriteria()));
                }
                else
                {
                    SortList(Form["sortExpression"]);
                }
                PropertyBag[EntityListName] = EntitiesList;
                AddListSummary();
            }
            catch (Exception ex)
            {
                HandleListError(ex);
            }
        }

        protected TCriteria Criteria
        {
            get
            {
                TCriteria result = WebObjectCache.GetInstance().Retrieve<TCriteria>(_criteriaName);
                if (result == null)
                {
                    result = (TCriteria)Activator.CreateInstance(typeof(TCriteria));
                    Criteria = result;
                }
                return result;
            }
            set { WebObjectCache.GetInstance().Add(_criteriaName, value); }
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
                    typeof(TEntity).GetProperty(propertyName)
                        .GetValue(item, null));
            }
            return sum;
        }

        protected void HandleListError(Exception ex)
        {
            ShowError(ex);
            using (new SessionScope(FlushAction.Never))
            {
                DoList();
            }
        }

        protected void RedirectToList()
        {
            RedirectToAction("list");
        }

        public abstract string SortExpression { get; set;}
        public abstract SortDirection SortDirection { get; set; }

        protected virtual void PopulateFilterSelects()
        {
        }

        protected virtual void SetupSortList(string expression)
        {
        }

        protected virtual void SetCustomFilterPreset(string presetName)
        {
        }

        protected virtual void AddListSummary()
        {
        }
        
        #endregion list action

        #region new action

		[Layout("new")]
        public virtual void New(string actionResult)
        {
            SetViewContext();
            if (_hasContext)
            {
                ContextEntity.Refresh();
                PropertyBag[_contextEntityName] = ContextEntity;
            }
            PropertyBag["entity"] = default(TEntity);
            PropertyBag["actionResult"] = actionResult;
            PropertyBag["entityIsNew"] = true;
            PopulateEditSelects();
        }

        public void Create([DataBind("entity")] TEntity entity)
        {
            string successMessage = string.Format("{0} created", _entityProperName);
            string failureMessage = string.Format("{0} NOT saved", _entityProperName);

            if (((Person)Context.CurrentUser).CanViewOnly)
            {
                HandleNewError(new Exception("You do not have create permission"), entity, failureMessage);
                return;
            }

            try
            {
                if (_hasContext)
                {
                    ContextEntity.Refresh();
                }
                SetupNewEntity(entity);
                entity.Validate();
                if (_hasContext)
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

        protected void HandleNewError(Exception ex, TEntity entity, string actionResult)
        {
            ShowError(ex);
            RedirectToNew(actionResult);
        }

        protected void RedirectToNew(string actionResult)
        {
            RedirectToAction("new", new string[] { "actionResult=" + actionResult });
        }

        #endregion new action

        #region edit action

        [Layout("edit")]
        public virtual void Edit(Guid id)
        {
            Flash["error"] = null;
            try
            {
                TEntity entity = ActiveRecordBase<TEntity>.Find(id);
                SetupEntity(entity);
                SetViewContext();
                PropertyBag["actionResult"] = Form["actionResult"];
                PropertyBag["entity"] = entity;
                PropertyBag["previousId"] = GetPreviousId(entity);
                PropertyBag["nextId"] = GetNextId(entity);
                PropertyBag["entityIsNew"] = false;
                if (_hasContext)
                {
                    //ToDo: this is a HACK.  It should not be needed, not should Refresh.  It 
                    //prevents a lazy load exception in the case of a redirect after a create.
                    ContextEntity = GetContextEntity(ContextEntity.Id);

                    PropertyBag[_contextEntityName] = ContextEntity;
                }
                PopulateEditSelects();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public virtual void Save([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NullIfInvalidKey)] TEntity entity)
        {
            Update(entity);
        }

        protected void Update(TEntity entity)
        {
            string successMessage = string.Format("{0} saved", _entityProperName);
            string failureMessage = string.Format("{0} NOT saved", _entityProperName);

            if (((Person)Context.CurrentUser).CanViewOnly)
            {
                RenderText(string.Format("{0}. {1}", failureMessage, "You do not have update permission"));
                CancelView();
                return;
            }

            try
            {
                SetupUpdateEntity(entity);
                entity.Validate();
                entity.UpdateAndFlush();
                RenderText(successMessage);
            }
            catch (Exception ex)
            {
                Context.Response.StatusCode = 500;
                RenderText(string.Format("{0}. {1}", failureMessage, GetErrorMessage(ex).Trim()));
            }
            finally
            {
                CancelView();
            }
        }

        protected virtual void SetupEntity(TEntity entity)
        {
        }

        protected virtual void PopulateEditSelects()
        {
        }

        protected virtual void DoCustomEditAction(TEntity entity)
        {
        }

        protected virtual void SetupUpdateEntity(TEntity entity)
        {
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
            RedirectToEdit(entity.Id, actionResult);
        }

        protected void RedirectToEdit(Guid entityId, string actionResult)
        {
            RedirectToAction("edit", new string[] { "id=" + entityId, "actionResult=" + actionResult });
        }

        #endregion edit action

        #region delete action

        public virtual void Delete([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] TEntity entity)
        {
            string failureMessage = _entityProperName + " NOT deleted";

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

        #endregion delete action

        #region context/utilitiy/navigation

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

        protected TContextEntity ContextEntity
        {
            get
            {
                TContextEntity result = WebObjectCache.GetInstance().Retrieve<TContextEntity>(_contextEntityName);
                if (result == null)
                {
                    string cookieValue = Request.ReadCookie(contextCookieName);
                    result = GetContextEntity(new Guid(cookieValue));
                    ContextEntity = result;
                }
                return result;
            }
            set { WebObjectCache.GetInstance().Add(_contextEntityName, value); }
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

        protected void SetViewContext()
        {
            PropertyBag["userIsAdmin"] = ((Person)Context.CurrentUser).IsAdmin;
            PropertyBag["userCanEdit"] = !((Person)Context.CurrentUser).CanViewOnly;
            PropertyBag["contextEntityName"] = _contextEntityName;
            string entityNameProper = char.ToUpper(_entityName[0]) + _entityName.Substring(1);
            PropertyBag["entityName"] = entityNameProper;
            PropertyBag["entityNamePlural"] = TextUtil.MakePlural(entityNameProper);
            PropertyBag["isEditFormLong"] = _isEditFormLong;
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

        protected void RefreshContextEntity()
        {
            ContextEntity = GetContextEntity(ContextEntity.Id);
        }

        private TContextEntity GetContextEntity(Guid id)
        {
            TContextEntity result = ActiveRecordBase<TContextEntity>.Find(id);
            HttpCookie cookie = new HttpCookie(contextCookieName, id.ToString());
            cookie.Expires = DateTime.MaxValue;
            HttpContext.Response.SetCookie(cookie);
            return result;
        }
    } 
    
    #endregion context/utilitiy/navigation
}
