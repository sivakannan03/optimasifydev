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
    
    public partial class Masters_Hospitalmaster_tb
    {
        public int HospitalId { get; set; }
        public string NameofHospital { get; set; }
        public string HotelAddress { get; set; }
        public string Street { get; set; }
        public int City { get; set; }
        public int Area { get; set; }
        public Nullable<decimal> ZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string MCLflag { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string ApprovalStatus { get; set; }
        public string ReasonForRequest { get; set; }
        public Nullable<decimal> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> TerFKID { get; set; }
    }
}
