/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Internal;
using Castle.Components.Validator;
using Castle.MonoRail.Framework.Helpers;
using Lunaverse.Tools.Common;
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
        where TEntity : BaseEntity<TEntity>
        where TContextEntity : IEntity
        where TCriteria : IFindCriteria
    {
        #region private/protected fields

        private readonly bool _hasContext;
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
            _isEditFormLong = isEditFormLong;
            _hasContext = typeof(TContextEntity).IsClass;
        } 

        #endregion

        #region public properties

        public string EntityListName
        {
            get { return Inflector.Pluralize(_entityName); }
        }

        public string ContextEntityIdName
        {
            get { return _contextEntityName + "Id"; }
        } 

        #endregion

        protected Person CurrentUser
        {
            get { return (Person)Context.CurrentUser; }
        }

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
            ShowError();
            SetViewContext();
            PropertyBag["newLinkVisible"] = !CurrentUser.CanViewOnly;
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
                PopulateEntitiesList();

                AddListSummary();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                HandleListError(ex);
            }
        }

        private void PopulateEntitiesList()
        {
            bool sortRequested = !string.IsNullOrEmpty(Form["sortExpression"]);
            int rowsPerPage = CurrentUser.UserPreferences.RowsPerPage;
            bool paginated = rowsPerPage != 0;
            bool isMultiPage = paginated && rowsPerPage < EntitiesList.Count;

            if (!sortRequested || isMultiPage)
            {
                EntitiesList.Clear();
                try
                {
                    TEntity[] items = ActiveRecordBase<TEntity>.FindAll(Criteria.ToDetachedCriteria());
                    EntitiesList.InsertRange(0, items);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    HandleListError(ex);
                }
            }

            if (sortRequested)
            {
                SortList(Form["sortExpression"]);
            }

            if (paginated)
            {
                int pageNumber;
                int.TryParse(Context.Params["page"], out pageNumber);
                PropertyBag[EntityListName] = PaginationHelper.CreatePagination(
                    (ICollection<TEntity>)EntitiesList, rowsPerPage, pageNumber);
            }
            else
            {
                PropertyBag[EntityListName] = EntitiesList;
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
                object value = typeof (TEntity).GetProperty(propertyName).GetValue(item, null);
                if (value != null)
                {
                    sum += Convert.ToDouble(value);
                }
            }
            return sum;
        }

        protected void HandleListError(Exception ex)
        {
            //Criteria = default(TCriteria); //reset criteria to prevent error from repeating
            SetError(ex);
            //RedirectToList();
        }

        protected void RedirectToList()
        {
            RedirectToAction("list");
        }

        public string SortExpression
        {
            get { return BaseEntity<TEntity>.SortExpression; }
            set { BaseEntity<TEntity>.SortExpression = value; }
        }

        public SortDirection SortDirection
        {
            get { return BaseEntity<TEntity>.SortDirection; }
            set { BaseEntity<TEntity>.SortDirection = value; }
        }

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
        public virtual void New()
        {
			TEntity entity = Entity ??
				(TEntity)Activator.CreateInstance(typeof(TEntity));
		    SetViewContext();
            if (_hasContext)
            {
                ContextEntity.Refresh();
                PropertyBag[_contextEntityName] = ContextEntity;
            }
            SetupEditView(entity, true);
		    SetActionResult(ActionResult);
		    ActionResult = null;
            PopulateEditSelects(entity);
        }

        protected virtual void SetupEditView(TEntity entity, bool isNew)
        {
            SetViewContext();
            PropertyBag["userCanEdit"] = GetUserCanEdit(entity);
            PropertyBag["entity"] = entity;
            PropertyBag["entityIsNew"] = isNew;
        }

        public virtual void Create([DataBind("entity")] TEntity entity)
        {
            CreateEntity(entity);
        }

        protected void CreateEntity(TEntity entity)
        {
            string successMessage = string.Format("{0} created", _entityProperName);
            string failureMessage = string.Format("{0} NOT saved", _entityProperName);

            if (CurrentUser.CanViewOnly)
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
                entity.SaveAndFlush();
                RedirectToEdit(entity.Id, successMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                HandleNewError(ex, entity, failureMessage);
            }
        }

        protected virtual void SetupNewEntity(TEntity entity)
        {
        }

        protected void HandleNewError(Exception ex, TEntity entity, string actionResult)
        {
            SetError(ex);
			RedirectToNew(entity, actionResult);
        }

        protected void RedirectToNew(TEntity entity, string actionResult)
        {
			Entity = entity;
            ActionResult = actionResult;
            RedirectToAction("new");
        }

        #endregion new action

        #region edit action

        [Layout("edit")]
        public virtual void Edit(Guid id)
        {
            try
            {
                TEntity entity = ActiveRecordBase<TEntity>.Find(id);
                Edit(entity);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                SetError(ex);
            }
        }

        private void Edit(TEntity entity)
        {
            ShowError();
            try
            {
                SetupEntity(entity);
                SetupEditView(entity, false);
                PropertyBag["previousId"] = GetPreviousId(entity);
                PropertyBag["nextId"] = GetNextId(entity);
                PropertyBag["entityIsNew"] = false;
                if (_hasContext)
                {
                    //TODO: this is a HACK.  It should not be needed, nor should Refresh.  It 
                    //TODO: prevents a lazy load exception in the case of a redirect after a create.
                    ContextEntity = GetContextEntity(ContextEntity.Id);

                    PropertyBag[_contextEntityName] = ContextEntity;
                }
                PopulateEditSelects(entity);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                SetError(ex);
            }
        }

        public virtual void Save([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NullIfInvalidKey)] TEntity entity)
        {
            Update(entity);
        }

        protected void Update(TEntity entity)
        {
            string failureMessageTemplate = string.Format("{0} NOT saved{{1}}", _entityProperName);

            if (!GetUserCanEdit(entity))
            {
                SetActionResult(failureMessageTemplate, "You do not have update permission");
                RenderEditContainer(entity);
                return;
            }

            try
            {
                SetupUpdateEntity(entity);
                entity.Validate();
                entity.UpdateAndFlush();
                SetActionResult("{0} saved", _entityProperName);
            }
            catch (ValidationException ex)
            {
                Context.Response.StatusCode = 500;
                SetActionResult(failureMessageTemplate, _entityProperName, 
                    ". " + GetErrorMessage(ex));
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Context.Response.StatusCode = 500;
                SetActionResult(failureMessageTemplate, _entityProperName, null);
                SetError(ex);
            }
            finally
            {
                RenderEditContainer(entity);
            }

        }

        protected void RenderEditContainer(TEntity entity)
        {
            using (new SessionScope())
            {
                Edit(entity);
            }
            LayoutName = "fragments/edit_container";
            RenderView("edit");
        }

        protected void SetActionResult(string message, params object[] args)
        {
            PropertyBag["actionResult"] = string.Format(message ?? string.Empty, args);
        }

        protected virtual void SetupEntity(TEntity entity)
        {
        }

        protected virtual void PopulateEditSelects(TEntity entity)
        {
        }

        protected virtual void DoCustomEditAction(TEntity entity)
        {
        }

        protected virtual void SetupUpdateEntity(TEntity entity)
        {
        }

        protected static T SetValueFromKey<T>(string id)
        {
            if (typeof(IEntity).IsAssignableFrom(typeof(T)))
            {
                try
                {
                    Guid key = new Guid(id);
                    return ActiveRecordBase<T>.Find(key);
                }
                catch {}
            }
            else if (typeof(ILookup).IsAssignableFrom(typeof(T)))
            {
                int key;
                if (int.TryParse(id, out key))
                {
                    return ActiveRecordBase<T>.Find(key);
                }
            }
            return default(T);
        }

        protected void HandleEditError(Exception ex, TEntity entity, string actionResult)
        {
            SetError(ex);
            RedirectToEdit(entity.Id, actionResult);
        }

        protected void RedirectToEdit(Guid entityId, string actionResult)
        {
            ActionResult = actionResult;
            RedirectToAction("edit", new string[] { "id=" + entityId });
        }

        #endregion edit action

        #region delete action

        public virtual void Delete([ARDataBind("entity", AutoLoad = AutoLoadBehavior.Always)] TEntity entity)
        {
            string failureMessage = _entityProperName + " NOT deleted";

            if (CurrentUser.CanViewOnly)
            {
                HandleEditError(new Exception("You do not have delete permission"), entity, failureMessage);
                return;
            }

            try
            {
                if (_hasContext)
                {
                    ContextEntity = GetContextEntity(ContextEntity.Id);
                    RemoveFromContextEntity(entity);
                    ContextEntity.UpdateAndFlush();
                }
                else
                {
                    entity.DeleteAndFlush();   
                }
                RedirectToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                HandleEditError(ex, entity, failureMessage);
            }
        }

        protected virtual void RemoveFromContextEntity(TEntity entity)
        {
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

        protected void SetError(Exception ex)
        {
            Error = GetErrorMessage(ex);
        }

        protected void ShowError()
        {
            PropertyBag["error"] = Error;
            Error = null;
        }

        protected object Error
        {
            get { return Flash["error"]; }
            set { Flash["error"] = value; }
        }

		protected TEntity Entity
		{
			get { return Flash["entity"] as TEntity; }
			set { Flash["entity"] = value; }
		}

        private string ActionResult
        {
            get 
            { 
                if (Flash["actionResult"] == null)
                {
                    return null;
                }
                return Flash["actionResult"].ToString(); 
            }
            set { Flash["actionResult"] = value; }
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
            message = message.Replace("\r\n", "<br/>");
            return message.Trim();
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
            PropertyBag["userIsAdmin"] = CurrentUser.IsAdmin;
            PropertyBag["deleteEditButtonVisible"] = DeleteEditButtonVisible;
            PropertyBag["listEditButtonVisible"] = ListEditButtonVisible;
            PropertyBag["contextEntityName"] = _contextEntityName;
            PropertyBag["entityName"] = _entityProperName;
            PropertyBag["entityNamePlural"] = Inflector.Pluralize(_entityProperName);
            PropertyBag["isEditFormLong"] = _isEditFormLong;
        }

        protected virtual bool GetUserCanEdit(TEntity entity)
        {
            return !CurrentUser.CanViewOnly;
        }

        protected virtual bool DeleteEditButtonVisible
        {
            get { return true; }
        }

        protected virtual bool ListEditButtonVisible
        {
            get { return true; }
        }

        protected static T NullifyEntityIfTransient<T>(T entity) where T : IEntity
        {
            if (entity != null && entity.Id == Guid.Empty)
            {
                return default(T);
            }
            return entity;
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

        protected void WriteResponse(object content, bool asAttachment, string contentType, string filename, params object[] filenameArgs)
        {
            if (content != null)
            {
                filename = string.Format(filename, filenameArgs);

                bool isBinary = content.GetType().IsAssignableFrom(typeof(byte[]));

                Response.Clear();
                CancelLayout();
                CancelView();

                if (string.IsNullOrEmpty(contentType))
                {
                    Response.ContentType = GetContentType(filename, isBinary);
                }
                else
                {
                    Response.ContentType = contentType;
                }
                string header = string.Format("{0}Filename={1}",
                    asAttachment
                        ? "attachment; "
                        : string.Empty,
                    filename);
                Response.AppendHeader("Content-Disposition", header);
                try
                {
                    if (isBinary)
                    {
                        Response.AppendHeader("Content-Length", ((byte[])content).Length.ToString());
                        Response.BinaryWrite((byte[])content);
                    }
                    else
                    {
                        Response.AppendHeader("Content-Length",
                            Encoding.Default.GetByteCount(content.ToString()).ToString());
                        Response.Write(content);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    Response.Clear();
                }
            }
        }

        private string GetContentType(string filename, bool isBinary)
        {
            string ext = Path.GetExtension(filename);
            switch (ext.ToLower())
            {
                case "htm":
                case "html":
                    return "text/html";
                case "txt":
                    return "text/plain";
                case "doc":
                case "rtf":
                case "docx":
                    return "application/msword";
                case "xls":
                    return "application/msexcel";
                case "pdf":
                    return "application/pdf";
                default:
                    if (isBinary)
                    {
                        return "application/octet-stream";
                    }
                    else
                    {
                        return "text/plain";
                    }
            }
        }

        #endregion context/utilitiy/navigation
    } 
}
