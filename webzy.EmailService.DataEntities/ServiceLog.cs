//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace webzy.EmailService.DataEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class ServiceLog
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<int> EmailSettingId { get; set; }
        public string ErrorMessage { get; set; }
        public string Action { get; set; }
        public string ProcessName { get; set; }
        public string Remarks { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}