using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using webzy.EmailService.DataEntities;
using webzyEmailService.EmailModuleClass;
using webzy.EmailService.BizComp;
using NxGen.EmailService.BizComp.Credentials;


namespace webzy.EmailService.BizComp.EmailService
{
    public class BCEmaiService
    {
        BCAccountDetails bcAccountDetails = new BCAccountDetails();
		EncryptionHelper encryptionHelper = new EncryptionHelper();
        public bool VerifyIdentityId(string IdentityId)
        {
            bool UserVerified = false;
             try
            {
				string ServiceName = encryptionHelper.DecryptToText(IdentityId);
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
					var Identity = from c in ctx.ServiceAccounts where c.UserName == ServiceName select c;
                    if (Identity.Count() != 0)
                    {
                        UserVerified = true; 
                    }
                }
                return UserVerified;
            }
            catch (Exception ex)
            {
                int ErrorId = insertError(null, 0, "VerifyId", "VerifyIdentityId", ex.Message.ToString(), IdentityId);
                return false;
            }

        }
        public int insertError(string EmailAddress,int EmailSettingId,string Action,string Process, string ErrorMessage, string IdentityId)
        {
			ServiceLog NewError = new ServiceLog();
            try
            {
				NewError.EmailAddress = EmailAddress;
                NewError.EmailSettingId = EmailSettingId;
                NewError.Action = Action;
                NewError.ProcessName = Process;
                NewError.ErrorMessage = ErrorMessage;
                NewError.ModifiedBy = bcAccountDetails.GetUserNameBy(IdentityId);
                NewError.ModifiedOn = DateTime.Now;
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
                    ctx.ServiceLogs.Add(NewError);
                    ctx.SaveChanges();
                }
                return NewError.Id;
            }
            catch (Exception)
            {
                return -1;
                //Do Nothing to avoid deadlock;
            }
        }
        public int AddToEmailLog(SmartEmail Email, string IdentityId)
        {
            try
            {
                EmailLog NewMailLog = new EmailLog();
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
					NewMailLog.SenderName = (Email.SenderName != null) ? Email.SenderName.ToString() : string.Empty;
					NewMailLog.SenderAddress = (Email.SenderAddress != null) ? Email.SenderAddress.ToString() : string.Empty;
					NewMailLog.ReceiverName = (Email.ReceiverName != null) ? Email.ReceiverName.ToString() : string.Empty;
					NewMailLog.ReceiverAddress = Email.ReceiverAddress.ToString();
					NewMailLog.CCAddress = (Email.CCAddress != null) ? Email.CCAddress.ToString() : string.Empty;
                    NewMailLog.Subject = (Email.Subject != null) ? Email.Subject.ToString() : string.Empty;
                    NewMailLog.EmailContent = (Email.Content!=null)?Email.Content.ToString():string.Empty;
                    NewMailLog.EmailFormat = (Email.EmailFormat!=null)?Email.EmailFormat.ToString():string.Empty;
                    NewMailLog.EmailSeverity = (Email.EmailSeverity != null) ? Email.EmailSeverity.ToString() : string.Empty;
					NewMailLog.EmailSent = false;
                    NewMailLog.ModifiedBy = bcAccountDetails.GetUserNameBy(IdentityId);
                    NewMailLog.ModifiedOn = DateTime.Now;
                    ctx.EmailLogs.Add(NewMailLog);
                    ctx.SaveChanges();
					return NewMailLog.Id;
                }
            }
            catch (Exception ex)
            {
                int ErrorId = insertError(Email.ReceiverAddress, GetServerId(IdentityId), "AddToEmailLog", "AddToEmailLog", ex.InnerException.Message.ToString(), IdentityId);
				throw new Exception("General Error:VerifyIdentityId" + ex.Message.ToString());
            }
        }

        public int GetServerId(string IdentityId)
        {
            int ServerId;
            try
            {
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
                    var Identity = from c in ctx.ServiceAccounts where c.IdentityId == IdentityId select c;
                    if (Identity.Count() == 1)
                    {
                        ServerId = Identity.SingleOrDefault().EmailSettingFk;
                    }
                    else
                    {
                        throw new Exception("Multiply IdentityId Found");
                    }
                }
                return ServerId;
            }
            catch (Exception ex)
            {
                int ErrorId = insertError(null, 0, "GetServerId", "GetServerId", ex.Message.ToString(), IdentityId);
                throw new Exception("General Error:VerifyIdentityId" + ex.Message.ToString());
            }
        }

		internal void UpdateEmailLog(int EmailLogId, string IdentityId)
		{
			try
			{
				EmailLog NewMailLog = new EmailLog();
				using (webzyEmailEntities ctx = new webzyEmailEntities())
				{
					NewMailLog = (from c in ctx.EmailLogs where c.Id == EmailLogId select c).SingleOrDefault();
					NewMailLog.EmailSent = true;
					ctx.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				int ErrorId = insertError("EmailLogId : " + EmailLogId + "", 0, "UpdateEmailLog", "UpdateEmailLog", ex.Message.ToString(), IdentityId);
				throw new Exception("General Error:VerifyIdentityId" + ex.Message.ToString());
			}

		}
	}
}
