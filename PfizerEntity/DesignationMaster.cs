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
    using System.Collections.Generic;
    
    public partial class DesignationMaster
    {
        public decimal PKID { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public decimal NodeTypeFKID { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
    }
}
