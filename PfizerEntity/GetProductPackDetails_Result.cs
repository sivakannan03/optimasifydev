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
    
    public partial class GetProductPackDetails_Result
    {
        public Nullable<long> ROWID { get; set; }
        public decimal PKID { get; set; }
        public decimal ProductFKID { get; set; }
        public string Productpack { get; set; }
        public Nullable<decimal> FormFKID { get; set; }
        public string FormName { get; set; }
        public string IBISCode { get; set; }
        public bool SampleFlag { get; set; }
        public Nullable<bool> NonSampleFlag { get; set; }
    }
}
