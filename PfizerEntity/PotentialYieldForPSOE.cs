using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PfizerEntity
{
    class PotentialYieldForPSOE
    {

    }
    public class RR_PotentialYieldForPSO
    {
        public string productName { get; set; }
        public string Potential { get; set; }
        public string Yield { get; set; }
        
    }
    public class RR_PotentialYieldForDM
    {
        public string ProductName { get; set; }
        public string Potential { get; set; }
        public string Yield { get; set; }

    }
    public class RR_PotentialYieldForPSODetails
    {
        public string DoctorName { get; set; }
        public string segment { get; set; }
        public string ProductName { get; set; }
        public string TerCode { get; set; }
        public string TerName { get; set; }
        public string DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string Potential { get; set; }
        public string Yield { get; set; }
        

    }
    public class RR_TrackSheetReportForDMRBM
    {
        public string PKID { get; set; }
        public string Parameters { get; set; }
        public string Sequenceno { get; set; }
        public string Dec { get; set; }
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string Mar { get; set; }
        public string Apr { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public string Jul { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string YTD { get; set; }
        public string FinancialYear { get; set; }
        public string DMRBMFKID { get; set; }


    }
    public class RR_PsoWisePrescriptionTracker
    {
        public string Date { get; set; }
        public string PSOName { get; set; }
        public string RegionCode { get; set; }
        public string TerritoryCode { get; set; }
        public string DoctorName { get; set; }
        public string Specilization { get; set; }
        public string ProductName { get; set; }
        public string ProductPackName { get; set; }
        public string Prescription { get; set; }
        public string Hospital { get; set; }
        public string Stokiest { get; set; }
        public string BVOStatus { get; set; }
       


    }
    public class RR_SampleStatusReportForDM
    {
        public string ProductName { get; set; }
        public string SamplePack { get; set; }
        public string OpeningBalance { get; set; }
        public string Received { get; set; }
        public string Disbursed { get; set; }
        public string ClosingBalance { get; set; }
      
    }

}
