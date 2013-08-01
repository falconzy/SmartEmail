using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Collections;
using System.Text.RegularExpressions;
using webzyEmailService.EmailModuleClass;
using webzy.EmailService.DataEntities;
using webzy.EmailService.BizComp;

namespace webzy.EmailService.BizComp.EmailService
{
    public class EmailSendingService
    {
        private static volatile EmailSendingService current;
        private static Object syncRoot = new Object();   
        BCEmaiService bcEmailService = new BCEmaiService();
        Setting EmailConfiguraton = new Setting();
        BCAccountDetails bcAccountDetails = new BCAccountDetails();
       
        #region Constructors

        private EmailSendingService()
        {

        }

        public static EmailSendingService Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                        {
                            current = new EmailSendingService();
                        }
                    }
                }
                return current;
            }
        }
        #endregion
        internal bool SendMail(SmartEmail Email, int EmailServerId,string IdentityId)
        {
            bool MailSent = false;
			int EmailLogId = 0;
            try
            {
				EmailLogId = bcEmailService.AddToEmailLog(Email, IdentityId);
                MailMessage mail = new MailMessage();
                ArrayList SendMailList = new ArrayList();
                ArrayList CCMailList = new ArrayList();
                EmailConfiguraton = bcAccountDetails.GetMailConfigureBy(EmailServerId);
                SendMailList = GetMailList(Email.ReceiverAddress);
                CCMailList = GetMailList(Email.CCAddress);
				mail.From = new MailAddress(Email.SenderAddress.ToString(), Email.SenderName.ToString());
                //Mail.TO
                for (int i = 0; i < SendMailList.Count; i++)
                {
                    if (SendMailList[i] != null)
                    {
                        if (!(string.IsNullOrEmpty(SendMailList[i].ToString())))
                        {
                            mail.To.Add(SendMailList[i].ToString());
                        }
                    }
                }
                //Mail.CC
                for (int i = 0; i < CCMailList.Count; i++)
                {
                    if (CCMailList[i] != null)
                    {
                        if (!(string.IsNullOrEmpty(CCMailList[i].ToString())))
                        {
                            mail.CC.Add(CCMailList[i].ToString());
                        }
                    }
                }

                mail.Subject = Email.Subject.ToString();

                if (Email.EmailFormat == "HTML")
                {
                    mail.IsBodyHtml = true;
                }
                else
                {
                    mail.IsBodyHtml = false;
                }
                mail.Body = Email.Content.ToString();
                if (Email.EmailSeverity == "Important")
                {
                    mail.Priority = MailPriority.High;
                }
                SmtpClient smtp = new SmtpClient(EmailConfiguraton.SMTP,Convert.ToInt32(EmailConfiguraton.SMTPPort));             
                if ((!(string.IsNullOrEmpty(EmailConfiguraton.UserId))) && (!(string.IsNullOrEmpty(EmailConfiguraton.Password))))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(EmailConfiguraton.UserId, EmailConfiguraton.Password);
                    if (EmailConfiguraton.SMTPsecurity.HasValue && EmailConfiguraton.SMTPsecurity.Value)
                    {
                        smtp.EnableSsl = true;
                    }
                    
                }
                smtp.Send(mail);
				bcEmailService.UpdateEmailLog(EmailLogId,IdentityId);
                MailSent= true;
                return MailSent;
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(Email.ReceiverAddress, EmailServerId, "Sending", "SendMail", ex.Message.ToString(), IdentityId);
                return false;
            }
           
        }
        private ArrayList GetMailList(string ReceiverAddress)
        {
            ArrayList MailList = new ArrayList();
            if (ReceiverAddress != null && ReceiverAddress.ToString().Count() != 0)
            {
               
                string[] EmailAddress = ReceiverAddress.Split(';');
                string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
          + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
          + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
          + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
          + @"[a-zA-Z]{2,}))$";
                Regex CheckEmail = new Regex(patternStrict);
                foreach (string SEmailAddress in EmailAddress)
                {
                    if (CheckEmail.IsMatch(SEmailAddress))
                    {
                        MailList.Add(SEmailAddress);
                    }
                }
            }
            return MailList;
        }
        internal int GetEmailId(string IdentityId)
        {
            try
            {
                int EmailServerId;
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
                    var ServerId = (from c in ctx.ServiceAccounts where c.IdentityId == IdentityId select c.EmailSettingFk);
                    if (ServerId.Count() != 1)
                    {
                        EmailServerId = (from c in ctx.Settings where c.DefaultServer == true select c.Id).SingleOrDefault();
                    }
                    else
                    {
                        EmailServerId = ServerId.SingleOrDefault();
                    }
                    return EmailServerId;
                }
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, 0, "Sending", "GetEmailId", ex.Message.ToString(), IdentityId);
                return -1;
            }
        }
    }
}