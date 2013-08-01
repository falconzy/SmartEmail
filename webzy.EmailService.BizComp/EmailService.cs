using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace EmailRESTfulService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class EmailService : IEmailService
    {
        BCEmaiService bcEmailService = new BCEmaiService();
        #region IEmailService
        public int CreateEmail(string Sender, string SenderAddress, string Receiver, string ReceiverAddress,string CCAddress,bool Stored,string IdentityId)
        {
            EmailAddress newEmailAddress = new EmailAddress();
            try
            {
                if (bcEmailService.VerifyIdentityId(IdentityId))
                { 
                    newEmailAddress.SenderName = Sender;
                    newEmailAddress.SenderAddress = SenderAddress;
                    newEmailAddress.ReceiverName = Receiver;
                    newEmailAddress.ReceiverAddress = ReceiverAddress;
                    newEmailAddress.CCAddress = CCAddress;
                    newEmailAddress.Stored = Stored;
                    newEmailAddress.ModifiedBy = bcEmailService.GetUserNameBy(IdentityId);
                    newEmailAddress.ModifiedOn = DateTime.Now;

                    using (CCEmailModuleEntities ctx = new CCEmailModuleEntities())
                    {
                        ctx.EmailAddresses.Add(newEmailAddress);
                        ctx.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("Sorry,User Validation Fail");
                }
            
                return newEmailAddress.Id;
            }
            catch (Exception ex)
            {
               int ErrorId = bcEmailService.insertError(ex.Message.ToString());
               return ErrorId;
            }

        }

        public int AddEmailContent(int AddressId, string IdentityId, string Subject, string Content, string EmailFormat, string EmailSeverity, bool Stored)
        {
            EmailTemplate emailTemplate = new EmailTemplate();
            try
            {
                if (bcEmailService.VerifyIdentityId(IdentityId))
                {
                    emailTemplate.EmailAddressFk = AddressId;
                    emailTemplate.Subject = Subject;
                    emailTemplate.EmailContent = Content;
                    emailTemplate.EmailFormat = EmailFormat;
                    emailTemplate.EmailSeverity = EmailSeverity;
                    emailTemplate.Stored = Stored;
                    emailTemplate.ModifiedBy = bcEmailService.GetUserNameBy(IdentityId);
                    emailTemplate.ModifiedOn = DateTime.Now;

                    using (CCEmailModuleEntities ctx = new CCEmailModuleEntities())
                    {
                        ctx.EmailTemplates.Add(emailTemplate);
                        ctx.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception("Sorry,User Validation Fail");
                }
                return emailTemplate.Id;
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(ex.Message.ToString());
                return ErrorId;
            }
        }

        public int SendEmail(int EmailId , int EmailServerId)
        {
            int EmailSent = -1;
            try
            {
                EmailSent = EmailSendingService.Current.SendMail(EmailId, EmailServerId); //1 success ;else error Id;
                return EmailSent;
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(ex.Message.ToString());
                return ErrorId;
            }
        }

        public string GetErrorMsg(int ErrorId)
        {
            string ErrorMsg;
            try
            {
                using (CCEmailModuleEntities ctx = new CCEmailModuleEntities())
                {
                    ErrorMsg = (from c in ctx.ErrorMsgLogs where c.Id == ErrorId select c.ErrorMessage).ToString();
                }

                return ErrorMsg;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    
        #endregion 
    }
}