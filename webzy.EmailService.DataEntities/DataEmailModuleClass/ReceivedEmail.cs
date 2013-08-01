using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace webzyEmailService.EmailModuleClass
{
    /// <summary>
    /// ReceivedEmail Class (Using for received Emails)
    /// </summary>
	[DataContract(Namespace = "http://email.webservice.web-zy.com")]
	[Serializable]
    public class ReceivedMail : SmartEmail
    {
        [DataMember]
        public string UID { get; set; }
        [DataMember]
        public DateTime EmailSentTime { get; set; }
        [DataMember]
        public string AttachedFileId { get; set; }
    }
}
