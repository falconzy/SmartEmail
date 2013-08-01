using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webzy.EmailService.DataEntities;

namespace webzy.EmailService.BizComp
{
    public class BCAccountDetails
    {
        public string GetUserNameBy(string IdentityId)
        {
            string UserName;
            try
            {
                using (webzyEmailEntities ctx = new webzyEmailEntities())
                {
                    UserName = (from c in ctx.ServiceAccounts where c.IdentityId == IdentityId select c.UserName).SingleOrDefault().ToString();
                }

                return UserName;
            }
            catch (Exception ex)
            {
                throw new Exception("General Error:VerifyIdentityId" + ex.Message.ToString());
            }
        }

        public Setting GetMailConfigureBy(int EmailServerId)
        {
            using (webzyEmailEntities ctx = new webzyEmailEntities())
            {
                Setting MailConfiguration = new Setting();
                try
                {
                     MailConfiguration = (from c in ctx.Settings where c.Id == EmailServerId select c).SingleOrDefault();
                }
                catch (NullReferenceException)
                {
                     MailConfiguration = (from c in ctx.Settings where c.DefaultServer  == true select c).SingleOrDefault();
                }
                return MailConfiguration;
            }
            
        }

        public Setting GetReceiveMailConfigureBy(int EmailServerId)
        {
            using (webzyEmailEntities ctx = new webzyEmailEntities())
            {
                Setting MailConfiguration = (from c in ctx.Settings where c.Id == EmailServerId select c).SingleOrDefault();
                ctx.SaveChanges();
                return MailConfiguration;
            }
           
        }
    }       
}