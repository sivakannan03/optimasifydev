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
    
    public partial class ChemistMaster
    {
        public decimal PKID { get; set; }
        public string ChemistName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public Nullable<decimal> AreaFKID { get; set; }
        public Nullable<int> Zip { get; set; }
        public string PhoneNo { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public Nullable<int> PrescriptionPerMonth { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<decimal> CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<decimal> ModifiedBy { get; set; }
        public Nullable<decimal> ChemistCode { get; set; }
        public string ReasonForPSODeletion { get; set; }
        public Nullable<bool> IsDeletion { get; set; }
        public string PhoneNoISDCode { get; set; }
        public string PhoneNoSTDCode { get; set; }
        public Nullable<int> AvgCustomers { get; set; }
        public Nullable<int> NoCustomersMed { get; set; }
        public Nullable<int> OTCSales { get; set; }
    }
}