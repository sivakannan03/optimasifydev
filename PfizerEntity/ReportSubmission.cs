using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PfizerEntity
{
    #region Mathan
    public class ReportSubmission
    {
        public string CustomerName { get; set; }
        public string Specialization { get; set; }
        public string AreaName { get; set; }
        public string VisitCount { get; set; }
        public string PlannedVisits { get; set; }
        public string BalanceVisits { get; set; }
        public string UserFKID { get; set; }
        public string EmpName { get; set; }

    }

    public class ReportSubmissionChemist
    {
        public string CustomerName { get; set; }
        public string AreaName { get; set; }
        public string VisitCount { get; set; }
        public string PlannedVisits { get; set; }
        public string BalanceVisits { get; set; }
        public string RBVisitCount { get; set; }
        public string RBPlannedVisits { get; set; }
        public string RBBalanceVisits { get; set; }

    }

    public class RegionForTeamBased
    {
        public string RID { get; set; }
        public string RN { get; set; }

    }

    public class DistrictForTeamBased
    {
        public string DID { get; set; }
        public string DN { get; set; }

    }

    public class TerritoryForTeamBased
    {
        public string TerID { get; set; }
        public string TerName { get; set; }

    }

    public class GetCFSAMaster
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserFKID { get; set; }
        public string TerCode { get; set; }
        public string CFSAActive { get; set; }
       
    }

    public class GetChemistMasterAdminData
    {
        public string PKID { get; set; }
        public string ChemistName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string PhoneNo { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string PrescriptionPerMonth { get; set; }
        public string AvgCustomers { get; set; }
        public string NoCustomersMed { get; set; }
        public string OTCSales { get; set; }
        public string AreaFKID { get; set; }
        public string AreaName { get; set; }
        public string IsActive { get; set; }
        public string CLMStatus { get; set; }
    }
#endregion

#region Baskar
    public class ReminderReportPSO
    {
        public string PKID { get; set; }
        public string ProductName { get; set; }
    }

    public class RR_GetSpecialization
    {
        public string PKID { get; set; }
        public string Specialization { get; set; }
    }

    public class RR_GetMonthDetails
    {
        public string MonthNo { get; set; }
        public string MonthName { get; set; }
    }


    public class RR_GetCycleDetails
    {
        public string PKID { get; set; }
        public string CycleName { get; set; }
    }


    public class RR_GetGridLoad
    {
        public string DrName { get; set; }
        public string Specialization { get; set; }
        public string P1 { get; set; }
        public string A1 { get; set; }
        public string P2 { get; set; }
        public string A2 { get; set; }
        public string P3 { get; set; }
        public string A3 { get; set; }
        public string P4 { get; set; }
        public string A4 { get; set; }
        public string R1 { get; set; }
        public string RA1 { get; set; }
        public string R2 { get; set; }
        public string RA2 { get; set; }
    }

    public class RR_GetHeader
    {
        public string UName { get; set; }
        public string PName { get; set; }
        public string SDate { get; set; }
        public string EDate { get; set; }
    }
#endregion

}
