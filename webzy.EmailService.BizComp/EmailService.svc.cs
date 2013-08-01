using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using webzyEmailService.EmailModuleClass;
using webzy.EmailService.DataEntities;
using webzy.EmailService.BizComp.EmailService;
using webzy.EmailService.BizComp;

namespace webzyEmailService
{
	/// <summary>
	///  Email Service interface
	/// </summary>
	
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any, Namespace = "http://email.webservice.web-zy.com")]
    public class EmailService : IEmailService
    {
        BCEmaiService bcEmailService = new BCEmaiService();
        EmailReceivedService emailReceived = new EmailReceivedService();

        #region IEmailService
	
        public bool SendEmail(SmartEmail Email,string IdentityId)
        {
            
                bool EmailSent = false;
                int EmailServerId = -1;
                try
                {
                    if (bcEmailService.VerifyIdentityId(IdentityId))
                    {
                        EmailServerId = EmailSendingService.Current.GetEmailId(IdentityId);
                        EmailSent = EmailSendingService.Current.SendMail(Email, EmailServerId, IdentityId);
                        return EmailSent;
                    }
                    else
                    {
                        throw new Exception("Sorry,User Validation Fail");
                    }
                }
                catch (Exception ex)
                {
                    int ErrorId = bcEmailService.insertError(Email.ReceiverAddress, EmailServerId, "Sending", "SendEmail", ex.Message.ToString(), IdentityId);
                    throw new Exception("General Error: SendEmail" + ex.Message.ToString());
                }
            
            
        }
        public void ReceivedEmail(string IdentityId, string StartUID, string EndUID)
        {
            int EmailServerId = -1;
            try
            {
                
                if (bcEmailService.VerifyIdentityId(IdentityId))
                {
                    EmailServerId = EmailSendingService.Current.GetEmailId(IdentityId);
					emailReceived.ReceivedEmail(EmailServerId, IdentityId, StartUID, EndUID);
                }
                else
                {
                    throw new Exception("Sorry,User Validation Fail");
                }
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, EmailServerId, "Receiving", "ReceivedEmail", ex.Message.ToString(), IdentityId);
                throw new Exception("General Error: SendEmail" + ex.Message.ToString());
            }
        }
        public void ReadEmail(string IdentityId, string UID, bool Readed)
        {
            int EmailServerId = -1;
            try
            {
               if (bcEmailService.VerifyIdentityId(IdentityId))
                {
                    EmailServerId = EmailSendingService.Current.GetEmailId(IdentityId);
                    emailReceived.EmailReaded(EmailServerId, UID, Readed);
                }
                else
                {
                    throw new Exception("Sorry,User Validation Fail");
                }
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, EmailServerId, "Receiving", "ReadEmail", ex.Message.ToString(), IdentityId);
                throw new Exception("General Error: SendEmail" + ex.Message.ToString());
            }
           
        }
		public List<ReceivedMail> RetrievedEmail(string IdentityId, string StartUID, string EndUID, double size = 65536) 
        {
            try
            {          
                if (bcEmailService.VerifyIdentityId(IdentityId))
                {
                    int StartID = Convert.ToInt32(StartUID);
                    int EndID = Convert.ToInt32(EndUID);
					List<ReceivedMail> ReceivedEmails = emailReceived.RetrievedEmails(IdentityId, StartID, EndID, size);
                    return ReceivedEmails;
                }
                else
                {
                    throw new Exception("Sorry,User Validation Fail");
                }
            }
            catch (Exception ex)
            {
				int ErrorId = bcEmailService.insertError(null, 0, "Receiving", "RetrievedEmail", ex.Message.ToString(), IdentityId);
                throw new Exception("General Error: RetrievedEmail" + ex.Message.ToString());
            }
        }
        public string RetrievedAttchement(string IdentityId, int AttcheFileId)
        {
            try
            {
                if (bcEmailService.VerifyIdentityId(IdentityId))
                {
                    string FileLocation = emailReceived.RetrievedAttchement(IdentityId, AttcheFileId);
                    return FileLocation;
                }
                else
                {
                    throw new Exception("Sorry,User Validation Fail");
                }
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, 0, "Receiving", "RetrievedAttchement", ex.Message.ToString(), IdentityId);
                throw new Exception("General Error: RetrievedAttchement" + ex.Message.ToString());
            }
        }
        public string checkLastUID(string IdentityId)
        {
            try
            {
                int EmailServerId = -1;
                string LastUID;
                if (bcEmailService.VerifyIdentityId(IdentityId))
                {
                    EmailServerId = EmailSendingService.Current.GetEmailId(IdentityId);
                    LastUID = emailReceived.GetLastUID(IdentityId);
                    return LastUID;
                }
                else
                {
                    throw new Exception("Sorry,User Validation Fail");
                }
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, 0, "Receiving", "checkLastUID", ex.Message.ToString(), IdentityId);
                throw new Exception("General Error: SendEmail" + ex.Message.ToString());
            }
        }
        public string checkFirstUnReadUID(string IdentityId)
        {
            try
            {
                int EmailServerId = -1;
                string FirstUnReadUID;
                if (bcEmailService.VerifyIdentityId(IdentityId))
                {
                    EmailServerId = EmailSendingService.Current.GetEmailId(IdentityId);
                    FirstUnReadUID = emailReceived.GetFirstUnReadUID(IdentityId);
                    return FirstUnReadUID;
                }
                else
                {
                    throw new Exception("Sorry,User Validation Fail");
                }
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, 0, "Receiving", "checkFirstUnReadUID", ex.Message.ToString(), IdentityId);
                throw new Exception("General Error: SendEmail" + ex.Message.ToString());
            }
        }
        #endregion
    }
}
