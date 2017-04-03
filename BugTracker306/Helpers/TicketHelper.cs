// 2017 Revisions to AccountViewModels.cs
// 3/22 JMz EmailForUser - TBD null means get current user
//  3/21 JMz    ticket historyies.  coded overloaded RecordChange methods
//  3/21 JMz    working on eMail Notify().  cloned from AccountController.cs/public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
//  3/10 JMz    cloned from UserHelper.cs
//  3/08 JMz   per assignment by Antonio
using BugTracker306.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace BugTracker306.Helpers
{
    public class HistoryException : Exception
    {
        public int iErrorCode;
        public string Message;
        public  HistoryException(int errCode, string sMessage) 
        {
            Message = sMessage;
            iErrorCode = errCode;
        }
    }

    public class TicketHelper
    {
        // two working vars.   Don't want to put them on the stack like crazy.
        private int tktId; // The ticket we are working on. 
        private string uCurrentUserEMail;
        // instance all methods use.
        private ApplicationDbContext db = new ApplicationDbContext();

//ToDo: thoroughly test TicketHelper - TickhetHeler.cs
        public string UnassignedDeveloperId()
        {   // should it be null?  This method needs more documention on the why's of this
            // design decision. I think I need something to put in a drop down list box.
            ApplicationUser initDeveloper = db.Users.First(Sn322 => Sn322.DisplayName == "(unassigned)");
            //if (null == initStatus) throw  
            return initDeveloper.Id;
        }

        public ApplicationUser UnassignedDeveloperObj()
        {
            ApplicationUser initDeveloper = db.Users.First(Sn322 => Sn322.DisplayName == "(unassigned)");
            //if (null == initStatus) throw  
            return initDeveloper;
        }

        public int InitialSprint()
        {
            // once sprint functionality is installed there will be real logic here.
            // cross your fingers or pay me money. 
            return 1;
        }

        public int InitialStatus ()
        {
           TicketStatus initStatus = db.TicketStatuses
                .FirstOrDefault(Sn314 => Sn314.StsName == "new");
            //if (null == initStatus) throw  
            return initStatus.Id;
        }

        public int FinishedStatus()
        {
            TicketStatus cloStatus = db.TicketStatuses
                 .FirstOrDefault(Sn322 => Sn322.StsName == "finished");
            //if (null == initStatus) throw  
            return cloStatus.Id;
        }

        public string EmailForUser(string sUserGUID = null)
        {
            if (null == sUserGUID)
            {
                string TryMe321 = "HttpContext.Current. for user.";
            }
            string JasonAsNOTrackingRedSquiggle = "I can't figure this out.";
            var uCurrentUser = db.Users  // .AsNoTracking()
                    .FirstOrDefault(uu => uu.Id == sUserGUID);
            return uCurrentUser.Email;
        }

        private int RecordPropertyChange( string sPropertyName
                , string sNewValue, string sOldValue)
        {
            if (null == sNewValue || sOldValue == sNewValue) return 0; 
            TicketHistory logIt = new TicketHistory();
            db.TicketHistories.Attach(logIt);
            logIt.TicketId = tktId;
            logIt.Property = sPropertyName;
            logIt.NewValue = sNewValue ?? "";
            logIt.OldValue = sOldValue ?? "";
            logIt.UserId = uCurrentUserEMail;
            //  uCurrentUser.Email; //  UserGUID;
            logIt.ChangeDt = DateTimeOffset.Now;
            db.TicketHistories.Add(logIt);
            db.SaveChanges();
            return 1;
        }

        private int RecordPropertyChange( string sPropertyName
                    , ApplicationUser uNewValue, ApplicationUser uOldValue)
        {
            string sOldValue;
            string sNewValue = sOldValue = "(unassigned)";

            if (null != uNewValue) sNewValue = uNewValue.Email;
            if (null != uOldValue) sOldValue = uOldValue.Email;
            return RecordPropertyChange( sPropertyName
                    , sNewValue, sOldValue);
        }

        private int RecordPropertyChange(string sPropertyName
                    , int iNewValue, int iOldValue)
        {
            return RecordPropertyChange(sPropertyName
                    , iNewValue.ToString()
                    , iOldValue.ToString());
        }

        private int RecordPropertyChange( string sPropertyName
                    , TicketPriority iNewValue, TicketPriority iOldValue)
        {
            var WhatDoesToStringMeanHere = iNewValue.ToString();
            return RecordPropertyChange( sPropertyName
                    , iNewValue.Name
                    , iOldValue.Name);
        }

        private int RecordPropertyChange( string sPropertyName
                    , Project pNewValue, Project pOldValue)
        {
            string sold = pOldValue.Id + " " + pOldValue.Name; 
            string snew = pNewValue.Id + " " + pNewValue.Name;
            return RecordPropertyChange( sPropertyName
                    , pNewValue.Id + " " + pNewValue.Name 
                    , pOldValue.Id + " " + pOldValue.Name);
        }

        private int RecordPropertyChange( string sPropertyName
                    , TicketSeverity iNewValue, TicketSeverity iOldValue)
        {
            return RecordPropertyChange( sPropertyName
                    , iNewValue.ToString()
                    , iOldValue.ToString());
        }

        private int RecordPropertyChange( string sPropertyName
                    , Sprint iNewValue, Sprint iOldValue)
        {
            return RecordPropertyChange( sPropertyName
                    , iNewValue.ToString()
                    , iOldValue.ToString());
        }

        private int RecordPropertyChange( string sPropertyName
            , TicketStatus iNewValue, TicketStatus iOldValue)
        {
            return RecordPropertyChange( sPropertyName
                    , iNewValue.StsName.ToString()
                    , iOldValue.StsName.ToString());
        }

        private int RecordPropertyChange( string sPropertyName
                    , TicketType iNewValue, TicketType iOldValue)
        {
            return RecordPropertyChange( sPropertyName
                    , iNewValue.ToString()
                    , iOldValue.ToString());
        }

        public int RecordHistory(string sUserGUID, Ticket oldBeforeChangesTicket,
            Ticket newAfterSaveChangesTicket)
        {
            #region = JimmyJohns
            Ticket newTkt = newAfterSaveChangesTicket; // That long name is for users of the method.  Short name is for those of us here in the weeds.
            Ticket oldTkt = oldBeforeChangesTicket;
            tktId = newTkt.Id;
            // get a copy of data from database
            // we use AsNoTracking to make a disconnected dataset.  that is, changes
            // to the entity or the database will not affect eachother.
            // .Find(tktId); got me red underlines
            // Ticket - wrong type.  Using var would be easier.      
            //var oldTkt = db.Tickets.AsNoTracking()
            //                    .FirstOrDefault(xx => xx.Id == tktId);
            // might also be able to use First() or Single() instead of Where().
            // Does AsNoTracking() work with find() ?  If so we would have a ticket with properties
            // what about a null?  Is that a change or not?
            // example 2.  old description is there in db.  In the newTkt it comes as null.
            // in some cases that null to the DB means ignore this change.

            int iCCnt = 0;  // change ecount. may use for Notification logic.
            // working vars.   Don't want to put them on the stack like crazy.
            uCurrentUserEMail = EmailForUser(sUserGUID);

            // string uCurrentUser = "can is get this deep down inthis helper, or do I need to pass it down.";
            iCCnt += RecordPropertyChange("Title", newTkt.Title, oldTkt.Title);
            iCCnt += RecordPropertyChange("Description", newTkt.Description, oldTkt.Description);
            iCCnt += RecordPropertyChange("EstHours", newTkt.EstHours, oldTkt.EstHours);
            iCCnt += RecordPropertyChange("EstRemaining", newTkt.EstRemaining, oldTkt.EstRemaining);
            iCCnt += RecordPropertyChange("ElapsedHours", newTkt.ElapsedHours, oldTkt.ElapsedHours);
            iCCnt += RecordPropertyChange("Sprint",               newTkt.sprint, oldTkt.sprint);
            iCCnt += RecordPropertyChange("iTktsThatNeedThisDone", newTkt.iTktsThatNeedThisDone, oldTkt.iTktsThatNeedThisDone);
            iCCnt += RecordPropertyChange("iTktsHoldingUpThisTkt", newTkt.iTktsHoldingUpThisTkt, oldTkt.iTktsHoldingUpThisTkt);
            iCCnt += RecordPropertyChange("sTktsThatNeedThisDone", newTkt.sTktsThatNeedThisDone, oldTkt.sTktsThatNeedThisDone);
            iCCnt += RecordPropertyChange("sTktsHoldingUpThisTkt", newTkt.sTktsHoldingUpThisTkt, oldTkt.sTktsHoldingUpThisTkt);
            iCCnt += RecordPropertyChange("Project", newTkt.Proj, oldTkt.Proj);
            iCCnt += RecordPropertyChange("Priority", newTkt.Priority, oldTkt.Priority);
            iCCnt += RecordPropertyChange("Severity", newTkt.Severity, oldTkt.Severity);
            iCCnt += RecordPropertyChange("Status", newTkt.Status, oldTkt.Status);
            iCCnt += RecordPropertyChange("Type", newTkt.Type, oldTkt.Type);
            iCCnt += RecordPropertyChange("Submitter", newTkt.Submitter, oldTkt.Submitter);
            iCCnt += RecordPropertyChange("Developer", newTkt.Developer, oldTkt.Developer);
            //RecordPropertyChange("Attachments", newTkt.Attachments, oldTkt.Attachments);
            //RecordPropertyChange("Comments", newTkt.Comments, oldTkt.Comments);
            #endregion
            return iCCnt; // number of changes
        }

        // lecture 3/21 and 3/22/2017
        private void JasonHistory(string sUserGUID, Ticket oldBeforeChangesTicket,
            Ticket newAfterSaveChangesTicket)
        {
            // he called it private void addTicketHistory(theTicketId, ticket oldTicket)
            // get a disconnected instance of the old ticket - do this in the caller.
            // var oldTicket = db.Tickets.AsNoTaracking(().First(tt->tt.Id == ticket.Id);
            Ticket newTicket = newAfterSaveChangesTicket; // That long name is for users of the method.  Short name is for those of us here in the weeds.
            Ticket oldTicket = oldBeforeChangesTicket;
            var userId = HttpContext.Current.User.Identity.GetUserId();

            // loop over the Ticket Properties and compare their values..
            foreach (var prop in typeof(Ticket).GetProperties())
            {
                if (prop.GetValue(oldTicket) == null) prop.SetValue(oldTicket, "", null);
                if (prop.GetValue(newTicket) == null) prop.SetValue(newTicket, "", null);
                // very likely need && logic for NULL in case something doesn't 
                // get bound because it is just staying the same.
                // this questioon can be answered at debug time.
                {
                    var ticketHistory = new TicketHistory {
                        TicketId = newTicket.Id,
                        Property = prop.Name.ToString(),
                        OldValue = prop.GetValue(oldTicket).ToString(),
                        NewValue = prop.GetValue(oldTicket).ToString(),
                        ChangeDt = DateTimeOffset.Now,
                        UserId = userId
                    };
                    db.TicketHistories.Add(ticketHistory);
                } 
            }
       }

    public async void Notify44444(string UserGUID, Ticket TktWithChange, string sTypeOfChange)
            // LOGIC for NOTIFICATIONS TABLE is here.
            // returns 0 for no errors.
        // 1 means email not sent to avoid 'spamming'
        // 2 means other email error.
        {
            string TryingToGetEMailToWork = "issues with Async method - fixed elsewhere";
            string emSubject;
            TicketNotification newNotif = new TicketNotification();
            ApplicationUser uDeveloper;
            uDeveloper = TktWithChange.Developer;  // db.Users.Find(TktWithChange.DeveloperId);
                                                   // newNotif.Id = ?;
            newNotif.NotifAddress = db.Users.Find(UserGUID).Email;
            newNotif.SentDt = DateTimeOffset.Now;
            newNotif.ticket = TktWithChange; // should I just use the Id.

            string sTicketURL = "https://Google.com";
            string fromLine = "Bug Tracker <JamesSGCF@gmail.cm>";
            string emBody = "Dear " + uDeveloper.DisplayName + ",<br />" 
                + "There has been a change on ticket #" + TktWithChange.Id + ".<br />"
                + "To wit: " + sTypeOfChange + "<br />"
                + "To view this ticket <a href='" + sTicketURL + "'>click here</a>."
                + "To view all of your notifications<a href='" + sTicketURL + "'>click here</a>."
                + " <br />The Bug Tracker Team";

            var email = new MailMessage(fromLine, uDeveloper.Email) // REQUIRES USING systems.net.mail or such
            {
                Subject = "BT306/ Ticket Change",
                // Body =  string.Format(body, "Bug Tracker 306", "JamesSGCF@gmail.com", modBody),
                Body = emBody,
                IsBodyHtml = true
            };

            var svc = new PersonalEmail();
            string asyncIsGivnigRedSquriggles = "";
       //     await svc.SendAsync(email);
            if ("done" == svc.status)
            {
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
                // from controller.home/actionresult. . return RedirectToAction("Sent");
            }
            //314 end from Controller/Home/ActionResult Contact

            try
            {
                if (null == uDeveloper) throw new HistoryException(1234, "uDev is null");
                string WhereIsUserManagerDefined = "Look In Account Controller";
                // await UserManager.SendEmailAsync(uDeveloper.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                string eMailstatus99 = "User " + uDeveloper.Email + " should check his/her eMail.";
            }
            catch (Exception ex)
            {
                string debugeMailExceptionMessage = ex.Message;
                // there is also a finally block.
            }

        }
    } // class
} // namespace
