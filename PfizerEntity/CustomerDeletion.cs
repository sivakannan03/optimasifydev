using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PfizerEntity
{
    public class CustomerDeletion
    {
        public string ELement { get; set; }
        public Decimal ordID { get; set; }
        public Decimal PKID { get; set; }
        public string DoctorFKID { get; set; }
        public string CustomerName { get; set; }
        public string Type { get; set; }
        public string BrickName { get; set; }
        public string Specialization { get; set; }
        public string MCLFlag { get; set; }
        public string RequestDate { get; set; }
        public string ApprovedDate { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectDate { get; set; }
        public string RejectBy { get; set; }
        public string RejectReason { get; set; }
        public string Reason { get; set; }
    }

    public class ClsApproval
    {
        public string PKID { get; set; }
        public string chks { get; set; }
        public string DoctorFKID { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
    }

    public class IncludeCustomer
    {
        public string chks { get; set; }
        public Decimal pkid { get; set; }
        public string Type { get; set; }
        public Decimal AMPKID { get; set; }
        public string PractSpeciality { get; set; }
        public Int32 Kol { get; set; }

    }
    public class GetIncludeCustomer
    {
        public string pkid { get; set; }
        public string Type { get; set; }
        public string CustomerName { get; set;}
        public string Specification { get; set; }
        public string Kol { get; set; }
        public string AMPKID { get; set; }
        public string AreaName { get; set; }
        public string SMPKID { get; set; }
        
    }

    public class GetCycleYear
    {
        public string Element { get; set; }
        public string PKID { get; set; }
        public string cycleName { get; set; }
        public string Year { get; set; }
        public string CyclesMonth { get; set; }
        public Decimal StartMonth { get; set; }

    }

    public class UnlockCyclePlan
    {
        public string chks { get; set; }
        public Decimal PSOFKID { get; set; }
    }
    public class UnlockCyclePlanRetailer
    {
        public string chks { get; set; }
        public Decimal PSOFKID { get; set; }
    }

    public class PrescriptionTack
    {
        public string date { get; set; }
        public string PsoName { get; set; }
        public string RegionCode { get; set; }
        public string TerCode { get; set; }
        public string ChemistName { get; set; }
        public string ProductName { get; set; }
        public string Productpack { get; set; }
        public string Prescription { get; set; }
    }
    public class ChemistPrescriptionTracker
    {
        public string date { get; set; }
        public string PsoName { get; set; }
        public string RegionCode { get; set; }
        public string TerCode { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public string ProductName { get; set; }
        public string Productpack { get; set; }
        public string Prescription { get; set; }
        public string Hospital { get; set; }
        public string Stockist { get; set; }
    }
    public class LeaveApproval
    {
        public Decimal PKID { get; set; }
        public string Approved { get; set; }
        public string Rejected { get; set; }
        public string Comments { get; set; }
        public string ModifiedBy { get; set; }

    }
    public class LeaveRejected
    {
        public Decimal PKID { get; set; }
        public string Approved { get; set; }
        public string Rejected { get; set; }
        public string Comments { get; set; }
        public string ModifiedBy { get; set; }
    }
    public class LeaveApprovalRejected
    {
        public string Rejected { get; set; }
        public string Approved { get; set; }


    }
}
 