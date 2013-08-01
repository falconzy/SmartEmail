using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Collections;
using webzyEmailService.EmailModuleClass;

namespace webzyEmailService
{
	/// <summary>
	/// Email Service 2013
	/// </summary>
	[ServiceContract(Namespace = "http://email.webservice.web-zy.com")]
	public interface IEmailService
	{
		[OperationContract]
		bool SendEmail(SmartEmail Email,string IdentityId);

		/// <summary>
		/// Received emails by UID
		/// </summary>
		/// <param name="IdentityId">The identity id.</param>
		/// <param name="StartUID">The start UID.</param>
		/// <param name="EndUID">The end UID.</param>
		[OperationContract]
		void ReceivedEmail(string IdentityId, string StartUID, string EndUID);

		/// <summary>
		/// To Mark Email as Reads or Unread.
		/// </summary>
		/// <param name="IdentityId">The identity id.</param>
		/// <param name="UID">The UID.</param>
		/// <param name="Readed">if set to <c>true</c> [readed].</param>
		[OperationContract]
		void ReadEmail(string IdentityId, string UID ,bool Readed);

		/// <summary>
		/// Retrieved emails by UID
		/// </summary>
		/// <param name="IdentityId">The identity id.</param>
		/// <param name="StartUID">The start UID.</param>
		/// <param name="EndUID">The end UID.</param>
		/// <returns>ReceivedEmail Class</returns>
		[OperationContract]
		List<ReceivedMail> RetrievedEmail(string IdentityId, string StartUID, string EndUID, double size = 65536);

		/// <summary>
		/// Retrieveds the attchement (retrun attchement file name and file location).
		/// </summary>
		/// <param name="IdentityId">The identity id.</param>
		/// <param name="AttcheFileId">The attche file id.</param>
		/// <returns>FileName and File location(Format:<FileName><FileLocation>)</returns>
		[OperationContract]
		string RetrievedAttchement(string IdentityId, int AttcheFileId);

		/// <summary>
		/// Checks the first unread UID.
		/// </summary>
		/// <param name="IdentityId">The identity id.</param>
		/// <returns></returns>
		[OperationContract]
		string checkFirstUnReadUID(string IdentityId);

		/// <summary>
		/// Checks the last UID in Email Service.
		/// </summary>
		/// <param name="IdentityId">The identity id.</param>
		/// <returns></returns>
		[OperationContract]
		string checkLastUID(string IdentityId);      
	}
}
