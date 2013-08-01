using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webzy.EmailService.DataEntities;
using System.Collections;
using AE.Net.Mail;
using System.IO;
using System.Configuration;
using webzyEmailService.EmailModuleClass;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.Entity.Validation;


namespace webzy.EmailService.BizComp.EmailService
{
    public class EmailReceivedService
    {
        BCAccountDetails bcAccountDetails = new BCAccountDetails();
        BCEmaiService bcEmailService = new BCEmaiService();
        List<ReceivedEmail> ReceivedEmails = new List<ReceivedEmail>();
        string uploadFileDirectory = ConfigurationManager.AppSettings["AttchedFileFolder"].ToString();
        string MailBox = ConfigurationManager.AppSettings["MailBox"].ToString();
        public void ReceivedEmail(int EmailServerId, string IdentityId,string StartUID,string EndUID,int MaxEmail = 100 )
        {

            int SUID = 0;
            bool SSL = false;
			try
			{
				Setting receivedEmailSetting = bcAccountDetails.GetReceiveMailConfigureBy(EmailServerId);
				if (receivedEmailSetting.IMAPsecurity.HasValue)
				{
					SSL = receivedEmailSetting.IMAPsecurity.Value;
				}
				if (receivedEmailSetting != null)
				{

					if (StartUID == "*")
					{
						using (ImapClient ic = new ImapClient(receivedEmailSetting.IMAP, receivedEmailSetting.EmailAddress, receivedEmailSetting.Password, ImapClient.AuthMethods.Login, Convert.ToInt32(receivedEmailSetting.IMAPPort), SSL))
						{
							ic.SelectMailbox(MailBox);
							Lazy<MailMessage>[] messages = ic.SearchMessages(SearchCondition.New(), true);
							foreach (Lazy<MailMessage> message in messages)
							{
								MailMessage m = ic.SearchMessages(SearchCondition.UID(message.Value.Uid)).SingleOrDefault().Value;
								SUID = Convert.ToInt32(m.Uid);
								int attachements = m.Attachments.Count;
								if (SUID != 0)
								{
									StartUID = SUID.ToString();
									AddEmail(EmailServerId, IdentityId, m);
								}
								else
								{
									return;
								}
								ic.Disconnect();
								ic.Dispose();
							}
						}
					}
					else
					{
						using (ImapClient ic = new ImapClient(receivedEmailSetting.IMAP, receivedEmailSetting.EmailAddress, receivedEmailSetting.Password, ImapClient.AuthMethods.Login, Convert.ToInt32(receivedEmailSetting.IMAPPort), SSL))
						{

							ic.SelectMailbox(MailBox);
							MailMessage[] messages = ic.GetMessages(StartUID.ToString(), "*",true,true);
							foreach (MailMessage message in messages)
							{
							
								if (StartUID.ToString().Trim() != message.Uid)
								{
									try
									{
										MailMessage Newmessage = ic.GetMessage(message.Uid);
										AddEmail(EmailServerId, IdentityId, Newmessage);
									}
									catch(Exception ex)
									{
										int ErrorId = bcEmailService.insertError(message.From.ToString() , EmailServerId, "Receiving", "EmailReceived", ex.Message.ToString(), IdentityId);
									}
								}
							}
							ic.Disconnect();
							ic.Dispose();
						}
					}
				}
			}
			catch (Exception ex)
			{
				int ErrorId = bcEmailService.insertError(null, EmailServerId, "Receiving", "EmailReceived", ex.Message.ToString(), IdentityId);
			}
            
        }

        private void AddEmail(int EmailServerId, string IdentityId, MailMessage message)
        {
            try
			{
				string BodyMsg;
                ReceivedEmail receivedEmail = new ReceivedEmail();
                receivedEmail.UID = message.Uid.ToString().Trim();
				receivedEmail.Subject = message.Subject;
				receivedEmail.FromAddress = (message.From.Address.Length > 0) ? message.From.Address.ToString() : string.Empty;
                receivedEmail.ToAddress = GetToAddress(message.To);
                receivedEmail.AttachedFile = false;
                receivedEmail.EmailFormat = message.ContentType.ToString();
				if (message.Body.Length > 8000)
				{
					BodyMsg = message.Body.Remove(8001);
				}
				else
				{
					BodyMsg = message.Body.ToString();
				}
                if (message.ContentType.Contains("text/html"))
                {
                    receivedEmail.EmailContent = StripHTML(message.Body.ToString(), IdentityId);
                    SaveHTMLFormatToService(message.Body.ToString(), message.Uid, EmailServerId, IdentityId);
                }
				else if (message.ContentType.Contains("text/plain"))
				{
					receivedEmail.EmailContent = message.Body.ToString();
				}
				else
				{
					receivedEmail.EmailContent = StripHTML(message.Body.ToString(), IdentityId);
				}
                receivedEmail.EmailSeverity = message.Importance.ToString().Trim();
                receivedEmail.EmailAccountFk = EmailServerId;
                receivedEmail.ModifiedBy = bcAccountDetails.GetUserNameBy(IdentityId);
                receivedEmail.ModifiedOn = DateTime.Now;
                if (message.Cc.Count != 0)
                {
                    List<System.Net.Mail.MailAddress> MailAddress = message.Cc.ToList();
                    foreach (System.Net.Mail.MailAddress address in MailAddress)
                    {
                        receivedEmail.CCAddress += address.Address.ToString();
                    }
                }
                if (message.Attachments.Count != 0)
                {
                    receivedEmail.AttachedFile = true;
                    
                }
                receivedEmail.Readed = false;
                receivedEmail.EmailSentTime = message.Date;
                if (checkUniqueEmail(receivedEmail, IdentityId))
                {
                    int EmailId = InsertEmail(receivedEmail, EmailServerId, IdentityId);
					if (EmailId != -1)
					{
						if (receivedEmail.AttachedFile)
						{
							foreach (Attachment attachment in message.Attachments)
							{
								
								string FileDirectory,attacheFileName;
								attacheFileName = (attachment.Filename.Length > 0) ? attachment.Filename : "" + message.Subject + ".eml";
								FileDirectory = CreateFileDirectoryLink(attacheFileName, uploadFileDirectory, IdentityId);
								attachment.Save(FileDirectory);
								insertAttachementFile(EmailId, attachment.Filename, FileDirectory, message.Uid.ToString().Trim(), receivedEmail.ModifiedBy, IdentityId);
							}
						}
					}
                }

            }
            catch (Exception ex)
            {
				int ErrorId = bcEmailService.insertError(null, EmailServerId, "Receiving", "AddEmail", ex.Message.ToString(), IdentityId);
            }
        }

        private string GetToAddress(ICollection<System.Net.Mail.MailAddress> ToAddressList)
        {
            string ToAddress = string.Empty;
            foreach (System.Net.Mail.MailAddress Adress in ToAddressList)
            {
                ToAddress += "" + Adress.Address.ToString() + ";";
            }
            return ToAddress;
        }

        private string CreateFileDirectoryLink(string FileName, string uploadFileDirectory, string IdentityId)
        {
            try
            {
				int i = 0;             
                bool FileExist;
				string fileName = "" + uploadFileDirectory + "" + FileName + "";
                FileInfo attachedmentFileinfo = new System.IO.FileInfo(fileName);
                FileExist = attachedmentFileinfo.Exists;
                if (FileExist)
                {
					string extension = attachedmentFileinfo.Extension.ToString().Trim();
					string NameOfFile = attachedmentFileinfo.Name.ToString().Trim().Replace(extension,string.Empty);
					
                    do
                    {
						FileName = string.Format("{0}({1}){2}", NameOfFile, i, extension);
						fileName = "" + uploadFileDirectory + "" + FileName + "";
						FileInfo NewattachedmentFileinfo = new System.IO.FileInfo(fileName);
                        FileExist = NewattachedmentFileinfo.Exists;
						i++;

                    } while (FileExist);

					return fileName;
                }
                else
                {
                    return fileName;
                }
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, 0, "Receiving", "CreateFileDirectoryLink", ex.Message.ToString(), IdentityId);
                return null;
            }

        }

        private void SaveHTMLFormatToService(string HTMLContent, string UID, int EmailServerId, string IdentityId)
        {
            try
            {
                string EmailFolder = ConfigurationManager.AppSettings["EmailFolder"].ToString();
                string FolderName = bcAccountDetails.GetMailConfigureBy(EmailServerId).Name;
                if (!Directory.Exists("" + EmailFolder + "\\" + FolderName + ""))
                {
                    Directory.CreateDirectory("" + EmailFolder + "\\" + FolderName + "");
                }
                string EmailFile = "" + EmailFolder + "\\" + FolderName + "\\" + UID + ".html";
                StreamWriter file = new StreamWriter(EmailFile);
                file.WriteLine(HTMLContent);
                file.Close();
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, EmailServerId, "Receiving", "SaveHTMLFormatToService", ex.Message.ToString(), IdentityId);
            }
        }

        private int insertAttachementFile(int EmailId,string FileName, string fileLocation, string UID, string ModifiedBy, string IdentityId)
        {
            try
            {
                int attchedmentFileId;
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
					AttachmentFile attachedfile = new AttachmentFile();
                    attachedfile.EmailReceivedFk = EmailId;
                    attachedfile.FileLocation = fileLocation;
                    attachedfile.FileName = FileName;
                    attachedfile.UID = UID;
                    attachedfile.ModifiedBy = ModifiedBy;
                    attachedfile.ModifiedOn = DateTime.Now;
					ctx.AttachmentFiles.Add(attachedfile);
                    ctx.SaveChanges();
                    attchedmentFileId = attachedfile.Id;
                }
                return attchedmentFileId;
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, 0, "Receiving", "insertAttachementFile", ex.Message.ToString(), IdentityId);
                return -1;
            }
        }

        private int InsertEmail(ReceivedEmail ReceivedEmail, int EmailServerId, string IdentityId)
        {
            try
            {
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
                        ctx.ReceivedEmails.Add(ReceivedEmail);
						ctx.SaveChanges();
                        return ReceivedEmail.Id;                   
                }
            }
			catch (DbEntityValidationException dbEx)
			{
				foreach (var validationErrors in dbEx.EntityValidationErrors)
				{
					foreach (var validationError in validationErrors.ValidationErrors)
					{
						bcEmailService.insertError(null, EmailServerId, "Receiving", "InsertReceivedEmail", "" + validationError.PropertyName + ";" + validationError.ErrorMessage + "", IdentityId);
					}
				}
				return -1;
			}
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, EmailServerId, "Receiving", "InsertReceivedEmail", ex.Message.ToString(), IdentityId);
                return -1;
            }
        }

        private bool checkUniqueEmail(ReceivedEmail ReceivedEmail, string IdentityId)
        {
            try
            {
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {

                    int uniqueEmail = (from c in ctx.EmailReceivedViews where c.FromAddress == ReceivedEmail.FromAddress && c.UID == ReceivedEmail.UID select c.Id).ToList().Count;
                    if (uniqueEmail == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, 0, "Receiving", "checkUniqueEmail", ex.Message.ToString(), IdentityId);
                return false;
            }
        }

        private string StripHTML(string source, string IdentityId)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating spaces because browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result,
                //         @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //         string.Empty,
                //         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything that's enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
     
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testing
                //System.Text.RegularExpressions.Regex.Replace(result,
                //       this.txtRegex.Text,string.Empty,
                //       System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4.
                // Prepare first to remove any whitespaces in between
                // the escaped characters and remove redundant tabs in between line breaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multiple tabs following a line break with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for line breaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // That's it.
                return result;
            }
            catch (Exception ex)
            {
                int ErrorId = bcEmailService.insertError(null, 0, "Receiving", "StripHTML", ex.Message.ToString(), IdentityId);
                return null;
            }
        }

        internal void EmailReaded(int EmailServerId, string UID, bool Readed)
        {
            using (webzyEmailEntities ctx = new webzyEmailEntities())
            {
                ReceivedEmail Email = (from c in ctx.ReceivedEmails where c.UID == UID && c.EmailAccountFk == EmailServerId select c).FirstOrDefault();
                Email.Readed = Readed;
                ctx.SaveChanges();
            }
        }

        internal string GetLastUID(string IdentityId)
        {
            using (webzyEmailEntities ctx = new webzyEmailEntities())
            {
                var UID = from c in ctx.EmailReceivedViews where c.IdentityId == IdentityId orderby c.Id descending select c.UID;
                if (UID.Count() > 0)
                {
                    string LastUID = UID.FirstOrDefault();
                    return LastUID;
                }
                else
                {
                    return "*";
                }
            }
        }

        internal string GetFirstUnReadUID(string IdentityId)
        {
            using (webzyEmailEntities ctx = new webzyEmailEntities())
            {
                string UID = (from c in ctx.EmailReceivedViews where c.IdentityId == IdentityId && c.Readed == false orderby c.Id ascending select c.UID).FirstOrDefault();
                return UID;
            }
        }

		internal List<ReceivedMail> RetrievedEmails(string IdentityId, int StartUID, int EndUID, double size)
        {
			List<ReceivedMail> ReceivedEmails = new List<ReceivedMail>();
			double EmailSize = 0;
            for (int i = StartUID; i <= EndUID; i++)
            {
				ReceivedMail Email = new ReceivedMail();
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
                    string SUID = i.ToString().Trim();
                    var EmailView = (from c in ctx.EmailReceivedViews where c.IdentityId == IdentityId && c.UID == SUID select c).FirstOrDefault();
					if (EmailView != null)
					{
						Email.UID = EmailView.UID;
						Email.EmailSentTime = EmailView.EmailSentTime;
						Email.ReceiverAddress = EmailView.ToAddress;
						Email.SenderAddress = EmailView.FromAddress;
						Email.CCAddress = EmailView.CCAddress;
						Email.Subject = EmailView.Subject;
						Email.Content = EmailView.EmailContent;
						Email.EmailSeverity = EmailView.EmailSeverity;
						Email.AttachedFileId = (EmailView.AttachedFile) ? RetrievedAttchementFileId(EmailView.Id) : string.Empty;
						EmailSize += GetSizeOf(Email);
						if (EmailSize < size)
						{
							ReceivedEmails.Add(Email);
						}
						else
						{
							return ReceivedEmails;
						}
					}
						
					}
                }
			 return ReceivedEmails;
            }

		private double GetSizeOf(ReceivedMail Email)
		{
			double size = 0;
			using (Stream s = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(s, Email);
				size = s.Length;
			}
			return size;
		}
                      
        private string RetrievedAttchementFileId(int EmailId)
        {
            string AttchementFileId = string.Empty;
            using (webzyEmailEntities ctx = new webzyEmailEntities())
            {
                var AttachedFiles = from c in ctx.AttachmentFiles where c.EmailReceivedFk == EmailId select c;
				foreach (AttachmentFile Attchement in AttachedFiles)
                {
                    AttchementFileId += "<" + Attchement.Id.ToString().Trim() + ">";

                }
                return AttchementFileId;
            }
        }

        internal string RetrievedAttchement(string IdentityId, int AttcheFileId)
        {
            using (webzyEmailEntities ctx = new webzyEmailEntities())
            {
				var File = (from c in ctx.AttachmentFiles where c.Id == AttcheFileId select c).SingleOrDefault();
                string FileInfo = "<" + File.FileName.ToString().Trim() + "><" + File.FileLocation.ToString().Trim() + ">";
                return FileInfo;
            }
        }
    }
}