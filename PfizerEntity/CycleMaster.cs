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
    
    public partial class CycleMaster
    {
        public decimal PKID { get; set; }
        public decimal Year { get; set; }
        public decimal CycleNo { get; set; }
        public string CycleName { get; set; }
        public string CycleFromMonth { get; set; }
        public decimal CycleFromYear { get; set; }
        public string CycleToMonth { get; set; }
        public decimal CycleToYear { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public decimal CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
    }
}
