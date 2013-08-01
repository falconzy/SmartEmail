using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace webzyEmailService.EmailModuleClass
{
	/// <summary>
	/// Smart Email Class (using for sending emails)
	/// </summary>
	[DataContract(Namespace = "http://email.webservice.web-zy.com")]
	public class SmartEmail
	{
		[DataMember]
		public string SenderName { get; set; }
		[DataMember]
		public string SenderAddress { get; set; }
		[DataMember]
		public string ReceiverName { get; set; }
		[DataMember]
		public string ReceiverAddress { get; set; }
		[DataMember]
		public string CCAddress { get; set; }
		[DataMember]
		public string Subject { get; set; }
		[DataMember]
		public string Content { get; set; }
		[DataMember]
		public string EmailFormat { get; set; }
		[DataMember]
		public string EmailSeverity { get; set; }
	}
}