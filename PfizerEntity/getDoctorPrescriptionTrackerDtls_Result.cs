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
    
    public partial class getDoctorPrescriptionTrackerDtls_Result
    {
        public string Element { get; set; }
        public string Date { get; set; }
        public decimal DoctorFKID { get; set; }
        public Nullable<decimal> ProductFKID { get; set; }
        public decimal PackSizeFKID { get; set; }
        public int Prescription { get; set; }
        public bool IsActive { get; set; }
        public string Hospital { get; set; }
        public string Stockist { get; set; }
        public string ProductName { get; set; }
        public string Productpack { get; set; }
    }
}