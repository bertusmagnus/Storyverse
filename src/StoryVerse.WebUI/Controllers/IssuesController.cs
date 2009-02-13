/* 
 * Copyright © Lunaverse Software 2007  
 * Distributed without waranty under the terms of the GPL
 * See the included file "Licence.txt" for terms of the license
*/

using System;
using System.Configuration;
using System.IO;
using System.Web;
using Castle.MonoRail.ActiveRecordSupport;
using Lunaverse.Tools.Common;
using StoryVerse.Core.Lookups;
using StoryVerse.Core.Criteria;
using StoryVerse.Core.Models;
using Castle.MonoRail.Framework;
using Castle.Components.Common.EmailSender;

namespace StoryVerse.WebUI.Controllers
{
    [Layout("default"), Rescue("generalerror")]
    public class IssuesController : EntityControllerBase<Issue, IssueCriteria, Project>
    {
        public IssuesController() : base(true) { }

        protected override bool DeleteEditButtonVisible
        {
            get { return CurrentUser.IsAdmin; }
        }

        protected override void PopulateEditSelects(Issue issue)
        {
            PopulateCommonSelects(issue.Owner);
        }

        [Layout("edit")]
        public override void Save([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NullIfInvalidKey)] Issue issue)
        {
            if (PerformAction(issue))
            {
                AddNewNote(issue);
                issue.LastUpdatedBy = CurrentUser;
                Update(issue);
            }
        }

        protected override void SetupEditView(Issue issue, bool isNew)
        {
            base.SetupEditView(issue, isNew);

            PropertyBag["newNote"] = string.Empty;
            PropertyBag["ccEmail"] = string.Empty;

            if (issue.IsTransient)
            {
                PropertyBag["actionChoice"] = "assign";
            }
            else
            {
                PropertyBag["actionChoice"] = "none";
            }

            GroupedCollection<IssueNote> groupedNotes = new GroupedCollection<IssueNote>(issue.Notes);
            groupedNotes.AddGroupDescription("CreatedBy.FullName");
            groupedNotes.AddGroupDescription("DateCreated.Date");
            PropertyBag["groupedNotes"] = groupedNotes;

            GroupedCollection<IssueChange> groupedChanges = new GroupedCollection<IssueChange>(issue.Changes);
            groupedChanges.AddGroupDescription("ChangedBy.FullName");
            groupedChanges.AddGroupDescription("ChangeDate");
            PropertyBag["groupedChanges"] = groupedChanges;

        }

        private void AddNewNote(Issue issue)
        {
            string newNote = Form["newNote"];
            if (!string.IsNullOrEmpty(newNote))
            {
                IssueNote note = new IssueNote();
                note.Body = newNote;
                note.CreatedBy = CurrentUser;
                note.DateCreated = DateTime.Now;
                issue.AddNote(note);
                issue.DateLastUpdated = DateTime.Now;
                issue.LastUpdatedBy = CurrentUser;
            }
        }

        private bool PerformAction(Issue issue)
        {
            string action = Form["actionChoice"];
            string cc = Form["ccEmail"];
            switch (action)
            {
                case "assign":
                    if (string.IsNullOrEmpty(Form["newOwner"]))
                    {
                        SetActionResult("NOT saved.  To assign you must select a new owner");
                        RenderEditContainer(issue);
                        return false;
                    }
                    issue.Owner = Person.TryFind(new Guid(Form["newOwner"]));
                    issue.Status = IssueStatus.Assigned;
					NotifyIssueAssignment(issue, cc, HttpContext.Request.UrlReferrer.AbsoluteUri);
                    return true;
                case "reopen":
                    issue.Status = IssueStatus.Pending;
                    return true;
                case "resolve":
                    if (string.IsNullOrEmpty(Form["newDisposition"]))
                    {
                        SetActionResult("NOT saved.  To mark resolved you must select a disposition");
                        RenderEditContainer(issue);
                        return false;
                    }
                    issue.Disposition = IssueDisposition.TryFind(
                        Convert.ToInt32(Form["newDisposition"]));
                    issue.Status = IssueStatus.Pending;
                    if (Convert.ToBoolean(Form["assignToReporter"].Split(',')[0]))
                    {
                        issue.Owner = issue.ReportedBy;
						NotifyIssueAssignment(issue, cc, HttpContext.Request.UrlReferrer.AbsoluteUri);
                    }
                    return true;
                case "close":
                    issue.Status = IssueStatus.Closed;
                    return true;
                default:
                    return true;
            }
        }

        public override void Create([DataBind("entity")] Issue issue)
        {
            CreateEntity(issue);
        	string editUrl = string.Format("{0}?id={1}",
				HttpContext.Request.Url.AbsoluteUri.Replace("create", "edit"), issue.Id);
			NotifyIssueAssignment(issue, Form["ccEmail"], editUrl);
        }

        private void NotifyIssueAssignment(Issue issue, string cc, string editUrl)
        {
            bool ownerHasEmail = !string.IsNullOrEmpty(issue.Owner.Email);

            if ((!issue.Owner.UserPreferences.NotifyOfIssueAssignment || !ownerHasEmail) && 
                string.IsNullOrEmpty(cc))
            {
                return;
            }

            string messageBase = string.Format("Notification for assignment of issue {0}", issue.Number.ToString("0000"));
            string notSentMessage = string.Format("{0} NOT sent", messageBase);

            if (ownerHasEmail && !Emailer.IsValidEmailList(issue.Owner.Email))
            {
                Log.Warn(notSentMessage + string.Format(".  '{0}' is not a valid email address list",
                    issue.Owner.Email));
                return;
            }

            string sentMessage = string.Format("{0} sent to: {1}", messageBase, issue.Owner.Email);

            PropertyBag["salutation"] = string.Format("{0} {1}", issue.Owner.FirstName, issue.Owner.LastName);
            PropertyBag["entity"] = issue;

			GroupedCollection<IssueNote> groupedNotes = new GroupedCollection<IssueNote>(issue.Notes);
			groupedNotes.AddGroupDescription("CreatedBy.FullName");
			groupedNotes.AddGroupDescription("DateCreated.Date");
			PropertyBag["groupedNotes"] = groupedNotes;

        	PropertyBag["issueUrl"] = editUrl;

            Message email = RenderMailMessage("issue_assigned");
			email.From = ConfigurationManager.AppSettings["emailFromAddress"] ?? "no_reply@storyverse.com";
            email.To = issue.Owner.Email;
            email.Cc = cc;
            email.Subject = string.Format("Issue assigned - {0:0000}:{1}", issue.Number, issue.Title);

            if (!string.IsNullOrEmpty(cc))
            {
                if (Emailer.IsValidEmailList(cc))
                {
                    if (!ownerHasEmail)
                    {
                        email.To = cc;
                    }
                    sentMessage += string.Format("{0}{1}{2}", email.To, ownerHasEmail ? "; " : null, cc);
                }
                else
                {
                    if (ownerHasEmail)
                    {
                        Log.Warn("{0} to CC.  '{1}' is not a valid email address list", notSentMessage, cc);
                    }
                    else
                    {
                        Log.Warn("{0}. Owner email is blank, and CC '{1}' is not a valid email address list", notSentMessage, cc);
                        return;
                    }
                }
            }

            try
            {
                DeliverEmail(email);
                Log.Info(sentMessage);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("{0}. From:{1} To:{2}", notSentMessage, email.From, email.To), ex);
            }
        }

        [Layout("edit")]
        public void AttachFile([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NullIfInvalidKey)] Issue issue, HttpPostedFile attachment, string attachmentTitle)
        {
            IssueAttachment att = new IssueAttachment();
            int length = Convert.ToInt32(attachment.InputStream.Length);

            int maxKbOfAttachment = Convert.ToInt32(
                AppSetting.GetSettingValue(AppSettingName.MaxIssueAttachmentSizeKb));
            if (length > maxKbOfAttachment * 1000)
            {
                RedirectToEdit(issue.Id,
                    string.Format("Attachment exceeds max size of {0}Kb", maxKbOfAttachment));
                return;
            }

            byte[] buffer = new byte[length];
            attachment.InputStream.Read(buffer, 0, length);
            att.Body = buffer;
            att.DateCreated = DateTime.Now;
            att.Filename = Path.GetFileName(attachment.FileName);
            att.Size = length;
            att.CreatedBy = CurrentUser;
            if (string.IsNullOrEmpty(attachmentTitle))
            {
                att.Title = Path.GetFileName(attachment.FileName);
            }
            else
            {
                att.Title = attachmentTitle;
            }
            issue.AddAttachment(att);
            att.CreateAndFlush();

            RedirectToEdit(issue.Id, "Attachment added");
        }

        [Layout("edit")]
        public void OpenAttachment([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NullIfInvalidKey)] Issue issue, Guid selectedAttachmentId)
        {
            WriteAttachment(issue, selectedAttachmentId);
        }

        [Layout("list")]
        public void OpenAttachment(Guid selectedIssueId, Guid selectedAttachmentId)
        {
            Issue issue = Issue.Find(selectedIssueId);
            WriteAttachment(issue, selectedAttachmentId);
        }

        private void WriteAttachment(Issue issue, Guid selectedAttachmentId)
        {
            IssueAttachment attachment = issue.GetAttachment(selectedAttachmentId);
            if (attachment != null)
            {
                WriteResponse(attachment.Body, true, null, attachment.Filename.Replace(" ", "_"));
            }
        }

        [Layout("edit")]
        public void DeleteAttachment([ARDataBind("entity", AutoLoad = AutoLoadBehavior.NullIfInvalidKey)] Issue issue, Guid selectedAttachmentId)
        {
            IssueAttachment attachment = issue.GetAttachment(selectedAttachmentId);
            issue.RemoveAttachment(attachment);
            issue.UpdateAndFlush();
            RedirectToEdit(issue.Id, "Attachment deleted");
        }

        protected void SetupDownload(string filename, string contentType)
        {
            CancelLayout();
            CancelView();
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
        }

        protected override void SetupNewEntity(Issue issue)
        {
            issue.Component = SetValueFromKey<Component>(Form["entity.Component.Id"]);
            issue.Priority = SetValueFromKey<IssuePriority>(Form["entity.Priority.Id"]);
            issue.Severity = SetValueFromKey<IssueSeverity>(Form["entity.Severity.Id"]);
            issue.Disposition = SetValueFromKey<IssueDisposition>(Form["entity.Disposition.Id"]);
            issue.ReportedBy = CurrentUser;
            issue.DateCreated = DateTime.Now;
            issue.Owner = SetValueFromKey<Person>(Form["newOwner"]);
			if (issue.Owner != null)
			{
				issue.Status = IssueStatus.Assigned;
			}
        	ContextEntity.AddIssue(issue);
        }

        protected override void RemoveFromContextEntity(Issue issue)
        {
            ContextEntity.RemoveIssue(issue);
        }

        protected override void PopulateFilterSelects()
        {
            PopulateCommonSelects(null);
        }

        private void PopulateCommonSelects(Person personToExlcude)
        {
            PropertyBag["types"] = Enum.GetValues(typeof(IssueType));
            PropertyBag["statuses"] = Enum.GetValues(typeof(IssueStatus));
            PropertyBag["priorities"] = IssuePriority.GetList();
            PropertyBag["dispositions"] = IssueDisposition.GetList();
            PropertyBag["severities"] = IssueSeverity.GetList();
            PropertyBag["components"] = ContextEntity.Components;

            PersonCriteria criteria = new PersonCriteria();
            criteria.ExcludePerson = personToExlcude;
            criteria.Project = ContextEntity;
            PropertyBag["persons"] = Person.FindAll(criteria.ToDetachedCriteria());
        }

        protected override void SetCustomFilterPreset(string presetName)
        {
            switch (presetName)
            {
                case ("my"):
                    Criteria.ApplyPresetMy(CurrentUser);
                    break;
                case ("myopen"):
                    Criteria.ApplyPresetMyOpen(CurrentUser);
                    break;
				case ("myopendefects"):
					Criteria.ApplyPresetMyOpenDefects(CurrentUser);
					break;
				case ("allopendefects"):
					Criteria.ApplyPresetAllOpenDefects();
					break;
            }
        }
    }
}