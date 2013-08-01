using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Security.Cryptography;
using System.IO;
using EmailSendTest.EmailService;

namespace EmailSendTest
{
   
    public class SendEmail
    {
        # region Constants
        private byte[] key = { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34 };
        private byte[] iv = { 22, 21, 20, 19, 18, 17, 16, 15 };
        # endregion

        # region Variables
        private Component component = new Component();
        private TripleDESCryptoServiceProvider cryptdes = new TripleDESCryptoServiceProvider();
        private UTF8Encoding utf8 = new UTF8Encoding();
        # endregion
        IEmailService EmailService = new EmailServiceClient();
		SmartEmail Mail = new SmartEmail();
        
        public void EmailSending(string Indentity)
        {
            try
            {
				Mail.ReceiverAddress = "yan.zhao@nxg-c.com";
				Mail.ReceiverName = "yan.zhao@nxg-c.com";
				Mail.SenderName = "Helpdesk";
				Mail.SenderAddress = "falconzy@gmail.com";
                Mail.Content = "test email.";
				Mail.EmailFormat = "HTML";
				Mail.EmailSeverity = "Important";
                Mail.Subject = "test Mail";
				EmailService.SendEmail(Mail,Indentity);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
                //catch here.
            }

        }
        public void receivedEmail()
        {
            //EmailService.RetrievedEmail("tjFW8NzScxwswzvMa5dnCu3K4lC6OJRF","151","154");
            //EmailService.ReceivedEmail("tjFW8NzScxwswzvMa5dnCu3K4lC6OJRF", "464", "*");
			EmailService.ReceivedEmail("HtagiwMSIZIVemupB0c0Rg==", "5313", "*");
			//EmailService.ReceivedEmail("HtagiwMSIZIVemupB0c0Rg==", "4379", "*");
			//EmailService.ReceivedEmail(IdentityId, lastUID, "*")
        }

        public string EncrypToText(string textToEncrypt)
        {
            try
            {
                return Convert.ToBase64String(this.EncryptToBinary(textToEncrypt));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public byte[] EncryptToBinary(string textToEncrypt)
        {
            try
            {
                byte[] input = this.utf8.GetBytes(textToEncrypt);
                return this.Transform(input, this.cryptdes.CreateEncryptor(this.key, this.iv));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private byte[] Transform(byte[] input, ICryptoTransform CryptoTransform)
        {
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptStream = new CryptoStream(memStream, CryptoTransform, CryptoStreamMode.Write);
            cryptStream.Write(input, 0, input.Length);
            cryptStream.FlushFinalBlock();
            memStream.Position = 0;
            byte[] result = memStream.ToArray();
            memStream.Close();
            cryptStream.Close();
            return result;
        }
    }
}
