//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PfizerEntity
{
    using System;
    
    public partial class AuthenticateUser3_Result
    {
        public string Result { get; set; }
        public decimal USER_FKID { get; set; }
        public decimal EMP_FKID { get; set; }
        public string EmpName { get; set; }
        public decimal ROLE_FKID { get; set; }
        public Nullable<decimal> Division_FKID { get; set; }
        public Nullable<decimal> Territory_FKID { get; set; }
        public Nullable<decimal> NodeType_FKID { get; set; }
        public string LastLogin { get; set; }
        public string LoginName { get; set; }
        public Nullable<System.DateTime> LastAddresUpdateDate { get; set; }
    }
}
