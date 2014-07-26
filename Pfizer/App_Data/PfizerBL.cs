using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using Sify.Global;
using Sify.DBManager;

/// <summary>
/// Summary description for GlobeBL
/// </summary>
public class PfizerBL
{
    DataTable dt = new DataTable();
    #region rajKumar

    public DataSet GetDailyRptforAllActivity()
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[3];
        param[0] = new SqlParameter("@FromDate", "07/01/2012");
        param[1] = new SqlParameter("@ToDate", "07/01/2014");
        param[2] = new SqlParameter("@RBMFKID", 256);
        objDL.ReturnDataSet("GetDailyReportatGlanceAllActivityDMNew", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    public DataSet ChkSPecialityforTrDr(string DTerritoryFKID, string drpDTDoctor, string STerritoryFKID)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[3];
        param[0] = new SqlParameter("@DTerritoryFKID", DTerritoryFKID);
        param[1] = new SqlParameter("@drpDTDoctor ", drpDTDoctor);
        param[2] = new SqlParameter("@STerritoryFKID", STerritoryFKID);
        objDL.ReturnDataSet("ChkSPecialityforTrDr", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    public DataSet ChkSPecialityforCyDr(string DTerritoryFKID, string drpDTDoctor, string STerritoryFKID)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[3];
        param[0] = new SqlParameter("@DTerritoryFKID", DTerritoryFKID);
        param[1] = new SqlParameter("@drpDCDoctor", drpDTDoctor);
        param[2] = new SqlParameter("@STerritoryFKID", STerritoryFKID);
        objDL.ReturnDataSet("ChkSPecialityforCyDr", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    //public DataSet GetDailyReportatGlanceAllNew()
    //{
    //    DataSet ds = new DataSet();
    //    PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
    //    SqlParameter[] param = new SqlParameter[3];
    //    param[0] = new SqlParameter("@FromDate", "07/04/2012");
    //    param[1] = new SqlParameter("@ToDate", "07/04/2014");
    //    param[2] = new SqlParameter("@PSOFKID", 339);
    //    objDL.ReturnDataSet("GetDailyReportatGlanceAllNew", CommandType.StoredProcedure, ds, param);
    //    return ds;
    //}

    public DataSet AllPendingCustomer(string PSOFKID, string AreaFKID, string XMLCusCode)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[3];
        param[0] = new SqlParameter("@PSOFKID", PSOFKID);
        param[1] = new SqlParameter("@AreaFKID", AreaFKID);
        param[2] = new SqlParameter("@XMLCusCode", XMLCusCode);
        objDL.ReturnDataSet("AllPendingCustomer", CommandType.StoredProcedure, ds, param);
        return ds;
    }
    public DataSet AllSubmittedCustomer(string PSOFKID, string AreaFKID, string XMLCusCode)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("AreaFKID", AreaFKID);
        nodes.Add("XMLCusCode", XMLCusCode);
        SPManager spm = new SPManager(nodes);
        DataSet ds = new DataSet();
        ds = spm.ExecuteDSProcedure("AllSubmittedCustomer");  
        return ds;
    }
    public DataSet AllPSOApprovedCustomer(string PSOFKID, string AreaFKID, string XMLCusCode)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("AreaFKID", AreaFKID);
        nodes.Add("XMLCusCode", XMLCusCode);
        SPManager spm = new SPManager(nodes);
        DataSet ds = new DataSet();
        ds = spm.ExecuteDSProcedure("AllPSOApprovedCustomer");
        return ds;
    }

    public DataSet AllPSORejectedCustomer(string PSOFKID, string AreaFKID, string XMLCusCode)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("AreaFKID", AreaFKID);
        nodes.Add("XMLCusCode", XMLCusCode);
        SPManager spm = new SPManager(nodes);
        DataSet ds = new DataSet();
        ds = spm.ExecuteDSProcedure("AllPSORejectedCustomer");
        return ds;
    }
    

    public DataTable GetCurrentCycleForPSo()
    {
        DataTable dt = new DataTable();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        objDL.ReturnDataTable("GetCurrentCycleForPSo", CommandType.StoredProcedure, dt);
        return dt;
    }


    public DataSet GetDoctorChemistProfile(string DoctChemID, string PSOFKID,string TypeID, string CycleFKID)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[4];
        param[0] = new SqlParameter("@DoctChemID", DoctChemID);
        param[1] = new SqlParameter("@PSOFKID", PSOFKID);
        param[2] = new SqlParameter("@TypeID", TypeID);
        param[3] = new SqlParameter("@CycleFKID", CycleFKID);
        objDL.ReturnDataSet("GetDoctorChemistProfile", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    public string CyclePlanCoverageRpt(string UserId, string Month, string Year)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("UserId", UserId);
        nodes.Add("Month", Month);
        nodes.Add("Year", Year); 
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("CyclePlanCoverageRpt") + "</Root>";
    }



    public DataSet GetPSONameDRAtGforRG(string UserFKID, string NodeTypeFKID)
    {

        NodeCollection nodes = new NodeCollection();
        nodes.Add("UserFKID", UserFKID);
        nodes.Add("NodeTypeFKID", NodeTypeFKID); 
        SPManager spm = new SPManager(nodes);
        DataSet ds = spm.ExecuteDSProcedure("GetPSONameDRAtGforRG");
        return ds;
    }
    public string DRatGlanceforPSO(string RPSOFKID, string Rdate)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("RPSOFKID", RPSOFKID);
        nodes.Add("Rdate", Rdate); 
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("DRatGlanceforPSO") + "</Root>";
    }

    public string CallVolumesReport(string Year, string Month, string UserFKID)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("Year", Year);
        nodes.Add("Month", Month);
        nodes.Add("UserFKID", UserFKID); 
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("CallVolumesReport") + "</Root>";
    }
    public string GetInputStatusReportByPSO(string PSOFKID, string InpFKID, string Mon, string Yr)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("InpFKID", InpFKID);
        nodes.Add("Mon", Mon);
        nodes.Add("Yr", Yr);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("GetInputStatusReportByPSO") + "</Root>";
    }

    public string GetInputStatusReportRecvd(string PSOFKID, string Mon, string Yr ,string InpFKID)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PSOFKID", PSOFKID); 
        nodes.Add("Mon", Mon);
        nodes.Add("Yr", Yr);
        nodes.Add("InpFKID", InpFKID);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("GetInputStatusReportByPSO") + "</Root>";
    }

    public string GetInputStatusReportByDMPSOWise(string TerritoryFKID, string PSOFKID, string ProductFKID, string Mon, string Yr, string XmlData)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", TerritoryFKID);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("Mon", Mon);
        nodes.Add("Yr", Yr);
        nodes.Add("XmlData", XmlData);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("GetInputStatusReportByDMPSOWise") + "</Root>";
    }

    public string GetInputStatusReportRecvdDM(string PSOFKID, string Mon, string Yr, string InputFKID)
    {
        NodeCollection nodes = new NodeCollection();
       
        nodes.Add("PSOFKID", PSOFKID); 
        nodes.Add("Mon", Mon);
        nodes.Add("Yr", Yr);
        nodes.Add("InputFKID", InputFKID);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("GetInputStatusReportRecvd") + "</Root>";
    }

    public string GetProductSampleStatusReportDBmnt(string TerritoryFKID, string PSOFKID, string ProductFKID, string Mon, string Yr, string SampleFKID)
    {

        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", TerritoryFKID);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("MOn", Mon);
        nodes.Add("Yr", Yr);
        nodes.Add("SampleFKID", SampleFKID);
        SPManager spm = new SPManager(nodes);			
        return "<Root>" + spm.ExecuteXmlProcedure("GetProductSampleStatusReportDBmnt") + "</Root>";
    }

    public string GetDailyReportatGlanceAllNew(string FromDate, string ToDate, string PSOFKID)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("FromDate", FromDate);
        nodes.Add("ToDate", ToDate);
        nodes.Add("PSOFKID",PSOFKID);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("GetDailyReportatGlanceAllNew") + "</Root>";
    }

    public string GetDailyReportatGlanceRGNew(string FromDate, string ToDate, string PSOFKID)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("FromDate", FromDate);
        nodes.Add("ToDate", ToDate);
        nodes.Add("PSOFKID", PSOFKID);
        SPManager spm = new SPManager(nodes);	
        return "<Root>" + spm.ExecuteXmlProcedure("GetDailyReportatGlanceRGNew") + "</Root>";
    }

    
    public string GetNodeName(string psofkId) {
        NodeCollection nodeType = new NodeCollection();
        nodeType.Add("PKID", psofkId);
        SPManager spm = new SPManager(nodeType);
        DataTable dt = new DataTable();
        dt = spm.ExecuteDTProcedure("GetNodeNameByXML");
        return dt.Rows[0]["NodeName"].ToString();
    }


    public string GetDailyReportatGlanceAllActivityPSONew(string FromDate, string ToDate, string RBMFKID, string PSOFKID)
    {

        
        NodeCollection nodes = new NodeCollection();
        nodes.Add("FromDate", FromDate);
        nodes.Add("ToDate", ToDate);
        nodes.Add("RBMFKID", RBMFKID);
        string XMLResult;
        string nodeType = GetNodeName(PSOFKID);
        if (nodeType == "PSO")
        {
            nodes.Add("PSOFKID", PSOFKID);
            SPManager spm = new SPManager(nodes);
            XMLResult = "<Root>" + spm.ExecuteXmlProcedure("GetDailyReportatGlanceAllActivityPSONew") + "</Root>";
        }
        else
        {
            SPManager spm = new SPManager(nodes);
            XMLResult = "<Root>" + spm.ExecuteXmlProcedure("MVCGetDailyReportatGlanceAllActivityDMNew") + "</Root>";
        }
        return XMLResult;
    }
    public string GetDailyReportatGlancePSORGNew(string FromDate, string ToDate, string RBMFKID, string PSOFKID)
    {

        NodeCollection nodes = new NodeCollection();
        nodes.Add("FromDate", FromDate);
        nodes.Add("ToDate", ToDate);
        nodes.Add("RBMFKID", RBMFKID);
        string XMLResult;
        string nodeType = GetNodeName(PSOFKID);
        if (nodeType == "PSO")
        {
            nodes.Add("PSOFKID", PSOFKID);
            SPManager spm = new SPManager(nodes);
            XMLResult = "<Root>" + spm.ExecuteXmlProcedure("GetDailyReportatGlancePSORGNew") + "</Root>";
        }
        else
        {
            SPManager spm = new SPManager(nodes);
            XMLResult = "<Root>" + spm.ExecuteXmlProcedure("GetDailyReportatGlanceDMRGNew") + "</Root>";
        }
        return XMLResult;
    }


    #region Daily Report 
       
    public string DailyReportFromSettingMaster(string psofkId)
    {
        string XMLResult;
     
        NodeCollection nodeType = new NodeCollection();
        nodeType.Add("UserFKID", psofkId);
        SPManager spm = new SPManager(nodeType); 
        XMLResult = "<Root>" + spm.ExecuteXmlProcedure("DailyReportFromSettingMaster") + "</Root>";
        return XMLResult;
    }

    #endregion

    #endregion

    #region Mathan


    public DataTable GetEmployeeInfo()
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@EmpId", "");
            objDL.ReturnDataTable("usp_GetEmpInfo", CommandType.StoredProcedure, dt, param);

        }
        catch (Exception se)
        {
            se.ToString();
        }
        return dt;
    }

    #region Blayer For Customer Met Not Met

    public DataSet GetCustomerMetNotMetRpt(string PKID, string FDATE, string TODATE, string DOCTYPE, string BRIDGEKOL, string str)
    {

        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[6];
        param[0] = new SqlParameter("@PKID", Convert.ToDecimal(PKID));
        param[1] = new SqlParameter("@FromDate", FDATE);
        param[2] = new SqlParameter("@ToDate", TODATE);
        param[3] = new SqlParameter("@MCLFlag", DOCTYPE);
        param[4] = new SqlParameter("@BridgeKOL", BRIDGEKOL);
        param[5] = new SqlParameter("@XMLGrpCode", str);

        objDL.ReturnDataSet("GetDoctorMetbyXMLN", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    #endregion

    #region region for selected based on Team

    public DataSet GetRegionForTeamBased(string SelectedTeamID)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@ID", SelectedTeamID);

        objDL.ReturnDataSet("Get_Region_BasedOn_Team_ByXML", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    public DataSet GetDistrictForTeamBased(string SelectedTeamID)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@ID", SelectedTeamID);

        objDL.ReturnDataSet("Get_District_BasedOn_Region_ByXML", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    public DataSet GetTerritoryForTeamBased(string SelectedTeamID)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@ID", SelectedTeamID);

        objDL.ReturnDataSet("Get_Territory_BasedOn_District_ByXML", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    #endregion


    #region CFSA Master Grid Load

    public DataSet GetCFSAMasterGrid(string PKID)
    {

        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@PKID", PKID);

        objDL.ReturnDataSet("Search_CFSA_ByXML", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    #endregion

    #region Chemist Master Admin Grid Laod

    public DataSet GetChemistAdminMasterGrid(string PKID)
    {
        string Pattern = "All";
        string search = "";
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[3];

        param[0] = new SqlParameter("@SPattern", Pattern);
        param[1] = new SqlParameter("@SString", search);
        param[2] = new SqlParameter("@Territory", PKID);

        objDL.ReturnDataSet("GetChemistDetailsByXmlForSearch", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    #endregion

    #endregion

    #region Baskar

    public DataSet GetPSONameDRAtGforRG(decimal User, decimal Node)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[2];
        param[0] = new SqlParameter("@UserFKID", User);
        param[1] = new SqlParameter("@NodeTypeFKID", Node);

        objDL.ReturnDataSet("GetPSONameDRAtGforRG", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    public DataSet GetReportSubmissions(string XMLPsoList, DateTime FromDate, DateTime ToDate, string Status)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[4];
        param[0] = new SqlParameter("@XMLPsoList", XMLPsoList);
        param[1] = new SqlParameter("@FromDate", FromDate);
        param[2] = new SqlParameter("@ToDate", ToDate);
        param[3] = new SqlParameter("@Status", Status);

        objDL.ReturnDataSet("GetReportSubmissions", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    //Report Submission Xml 
    public string GetReportSubmissions1(string XMLPsoList, string FromDate, string ToDate, string Status)
    {
        //DataSet ds = new DataSet();
        //PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        //SqlParameter[] param = new SqlParameter[4];
        //param[0] = new SqlParameter("@XMLPsoList", XMLPsoList);
        //param[1] = new SqlParameter("@FromDate", FromDate);
        //param[2] = new SqlParameter("@ToDate", ToDate);
        //param[3] = new SqlParameter("@Status", Status);

        //objDL.ReturnDataSet("GetReportSubmissions", CommandType.StoredProcedure, ds, param);
        //return ds;


        NodeCollection nodes = new NodeCollection();
        nodes.Add("XMLPsoList", XMLPsoList);
        nodes.Add("FromDate", FromDate);
        nodes.Add("ToDate", ToDate);
        nodes.Add("Status", Status);

        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("GetReportSubmissions") + "</Root>";

    }

    public DataSet SummeryReportPSO(string userid)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@userid", userid);

        objDL.ReturnDataSet("SummeryReportPSO", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    public DataSet SummeryReportReportingUsersData(string userid)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@userid", userid);

        objDL.ReturnDataSet("SummeryReportReportingUsersData", CommandType.StoredProcedure, ds, param);
        return ds;
    }



    public DataSet RR_ProductDetails(string userid)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@UserFKID", userid);

        objDL.ReturnDataSet("RR_ProductDetails", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    public DataSet RR_GetSpecialization(string userid)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@TerritoryFKID", userid);

        objDL.ReturnDataSet("RR_GetSpecialization", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    public DataSet GetMonthDetails()
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        objDL.ReturnDataSet("GetMonthDetails", CommandType.StoredProcedure, ds);
        return ds;
    }

    public DataSet SearchCycleByYear(string year)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@CycleYear", year);
        objDL.ReturnDataSet("SearchCycleByYear", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    public DataSet RR_RemonderReport_PSO1(string strUserFkid, string strProductName, string strYear, string strCycle, string strMonth, string strSpeciality)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[6];
        param[0] = new SqlParameter("@UserFKID", strUserFkid);
        param[1] = new SqlParameter("@ProdFKID", strProductName);
        param[2] = new SqlParameter("@RYear", strYear);
        param[3] = new SqlParameter("@RCyFKID", strCycle);
        param[4] = new SqlParameter("@RMon", strMonth);
        param[5] = new SqlParameter("@SpeID", strSpeciality);
        objDL.ReturnDataSet("RR_RemonderReport_PSO1", CommandType.StoredProcedure, ds, param);
        return ds;
    }



    public DataSet RR_RemonderReport_Details(string strUserFkid, string strProductName, string strYear, string strCycle, string strMonth)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[5];
        param[0] = new SqlParameter("@UserFKID", strUserFkid);
        param[1] = new SqlParameter("@ProdFKID", strProductName);
        param[2] = new SqlParameter("@RYear", strYear);
        param[3] = new SqlParameter("@RCyFKID", strCycle);
        param[4] = new SqlParameter("@RMon", strMonth);
        objDL.ReturnDataSet("RR_RemonderReport_Details", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    //Leave Report.

    public DataSet SearchByLeaveReport1(string strLeaveTypeFKID, string strEmpNameFKID, string strNodeTypeFKID, string strStatus, string strFetchDate, string strFromdate, string strTodate, string strYear)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[8];
        param[0] = new SqlParameter("@LeaveTypeFKID", strLeaveTypeFKID);
        param[1] = new SqlParameter("@EmpNameFKID", strEmpNameFKID);
        param[2] = new SqlParameter("@NodeTypeFKID", strNodeTypeFKID);
        param[3] = new SqlParameter("@Status", strStatus);
        param[4] = new SqlParameter("@FetchDate", strFetchDate);
        param[5] = new SqlParameter("@Fromdate", strFromdate);
        param[6] = new SqlParameter("@Todate", strTodate);
        param[7] = new SqlParameter("@Year", strYear);

        objDL.ReturnDataSet("SearchByLeaveReport1", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    //New Leave Report.-- 14-07-2014

    public string SearchByLeaveReport(string strLeaveTypeFKID, string strEmpNameFKID, string strNodeTypeFKID, string strStatus, string strFetchDate, string strFromdate, string strTodate, string strYear)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("LeaveTypeFKID", strLeaveTypeFKID);
        nodes.Add("EmpNameFKID", strEmpNameFKID);
        nodes.Add("NodeTypeFKID", strNodeTypeFKID);
        nodes.Add("Status", strStatus);
        nodes.Add("FetchDate", strFetchDate);
        nodes.Add("Fromdate", strFromdate);
        nodes.Add("Todate", strTodate);
        nodes.Add("Year", strYear);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("SearchByLeaveReport") + "</Root>";
    }



    //Cycle Plan 

    public DataSet CyclePlanReportlatest(string Year, string CycleName, string PSOFKID)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[3];
        param[0] = new SqlParameter("@GetYear", Year);
        param[1] = new SqlParameter("@GetCycleName", CycleName);
        param[2] = new SqlParameter("@PSOFKID", PSOFKID);

        objDL.ReturnDataSet("CyclePlanReportlatest", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    //Cycle Plan -- new

    public string CyclePlanReportlatest1(string Year, string CycleName, string PSOFKID)
    {
        //DataSet ds = new DataSet();
        //PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        //SqlParameter[] param = new SqlParameter[3];
        //param[0] = new SqlParameter("@GetYear", Year);
        //param[1] = new SqlParameter("@GetCycleName", CycleName);
        //param[2] = new SqlParameter("@PSOFKID", PSOFKID);

        //objDL.ReturnDataSet("CyclePlanReportlatest", CommandType.StoredProcedure, ds, param);
        //return ds;

        NodeCollection nodes = new NodeCollection();
        nodes.Add("GetYear", Year);
        nodes.Add("GetCycleName", CycleName);
        nodes.Add("PSOFKID", PSOFKID);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("CyclePlanReportlatest") + "</Root>";
    }


    //Load Pso

    public DataSet GetPSONamebyRepID(string strUserFkid)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@UserFKID", strUserFkid);
        objDL.ReturnDataSet("GetPSONamebyRepID", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    //CyclePlanCoverageRptDM

    public DataSet CyclePlanCoverageRpt_DM(string strPsoId, string strUId, string strMonth, string strYear)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[4];
        param[0] = new SqlParameter("@PsoId", strPsoId);
        param[1] = new SqlParameter("@UId", strUId);
        param[2] = new SqlParameter("@Month", strMonth);
        param[3] = new SqlParameter("@Year", strYear);
        objDL.ReturnDataSet("CyclePlanCoverageRpt_DM", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    ////CyclePlanCoverageRptDM -- On Live
    public string CyclePlanCoverageRpt_DM1(string strPsoId, string strUId, string strMonth, string strYear)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PsoId", strPsoId);
        nodes.Add("UId", strUId);
        nodes.Add("Month", strMonth);
        nodes.Add("Year", strYear);
        SPManager spm = new SPManager(nodes);

        return "<Root>" + spm.ExecuteXmlProcedure("CyclePlanCoverageRpt_DM") + "</Root>";
    }

    //Load DM

    public DataSet GetDMNamebyRepID(string strUserFkid)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@UserFKID", strUserFkid);
        objDL.ReturnDataSet("GetDMNamebyRepID", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    public string CyclePlanCoverageRpt_RBM(string strDMId, string strUId, string strMonth, string strYear)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PsoId", strDMId);
        nodes.Add("UId", strUId);
        nodes.Add("Month", strMonth);
        nodes.Add("Year", strYear);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("CyclePlanCoverageRptfor_RBM") + "</Root>";

    }

    //Load RBM

    public DataSet GetRBMNamebyRepID(string strUserFkid)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@UserFKID", strUserFkid);
        objDL.ReturnDataSet("GetRBMNamebyRepID", CommandType.StoredProcedure, ds, param);
        return ds;
    }


    public string CyclePlanCoverageRBM_DMRpt(string StrMonth, string Stryear, string SampleFKID)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("Month", StrMonth);
        nodes.Add("Year", Stryear);
        nodes.Add("UId", SampleFKID);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("CyclePlanCoverageRBM_DMRpt") + "</Root>";

    }

    //Load Team

    public DataSet GetTeamsforUserByXML(string strUserFkid)
    {
        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[1];
        param[0] = new SqlParameter("@UserFKID", strUserFkid);
        objDL.ReturnDataSet("GetTeamsforUserByXML", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    //Load Nation
    public string CyclePlanCoverageRpt_Nation(string strDMId, string strUId, string strMonth, string strYear, string strTeamId)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PsoId", strDMId);
        nodes.Add("UId", strUId);
        nodes.Add("Month", strMonth);
        nodes.Add("Year", strYear);
        nodes.Add("TeamId", strTeamId);

        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("CyclePlanCoverageRpt_Nation") + "</Root>";

    }

    public string CyclePlanCoverageRBM_DMRpt_New(string StrMonth, string Stryear, string SampleFKID, string Team)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("Month", StrMonth);
        nodes.Add("Year", Stryear);
        nodes.Add("UId", SampleFKID);
        nodes.Add("TeamFKId", Team);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("CyclePlanCoverageNation_RBMRpt") + "</Root>";

    }

    public string CyclePlanCoverageNation_RBMRpt(string StrMonth, string Stryear, string SampleFKID, string Team)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("Month", StrMonth);
        nodes.Add("Year", Stryear);
        nodes.Add("UId", SampleFKID);
        //nodes.Add("TeamFKId", Team);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("CyclePlanCoverageRBM_DMRpt_New") + "</Root>";

    }

    public string MVCGetSpecializationandProductByXml(string Specialization, string Team)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("SpecialityFKID", Specialization);
        nodes.Add("TeamFKID", Team);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("MVCGetSpecializationandProductByXml") + "</Root>";
    }
    #endregion

    #region MuniPrathap


    public DataSet GETPotentialYieldForPSO(long? strPSOFKID, int? strMonth, int? strYear, string strProductsName, string strType)
    {

        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[5];
        param[0] = new SqlParameter("@PSOFKID", strPSOFKID);
        param[1] = new SqlParameter("@PlanMonth", strMonth);
        param[2] = new SqlParameter("@PlanYear", strYear);
        param[3] = new SqlParameter("@ProductFKID", strProductsName);
        param[4] = new SqlParameter("@Type", strType);
        objDL.ReturnDataSet("GETPotentialYieldForPSO", CommandType.StoredProcedure, ds, param);
        return ds;
    }
    public DataSet GETPotentialYieldForDM(int? strMonth, int? strYear, string strProductsName, string strType, string strPSOlist)
    {

        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[5];
        param[0] = new SqlParameter("@PlanMonth", strMonth);
        param[1] = new SqlParameter("@PlanYear", strYear);
        param[2] = new SqlParameter("@ProductFKID", strProductsName);
        param[3] = new SqlParameter("@Type", strType);
        param[4] = new SqlParameter("@XMLPsoList", strPSOlist);
        objDL.ReturnDataSet("GETPotentialYieldForDM", CommandType.StoredProcedure, ds, param);
        return ds;
    }
    public DataSet GETPotentialYieldForRBMFTM(int? strMonth, int? strYear, string strProductsName, string strType, string strPSOlist)
    {

        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[5];
        param[0] = new SqlParameter("@PlanMonth", strMonth);
        param[1] = new SqlParameter("@PlanYear", strYear);
        param[2] = new SqlParameter("@ProductFKID", strProductsName);
        param[3] = new SqlParameter("@Type", strType);
        param[4] = new SqlParameter("@XMLPsoList", strPSOlist);
        objDL.ReturnDataSet("GETPotentialYieldForRBMFTM", CommandType.StoredProcedure, ds, param);
        return ds;
    }
    public DataSet TrackSheetReportFROMJobsFoRDMRBM(int? strPSOFKID, int? strYear)
    {

        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[2];
        param[0] = new SqlParameter("@PSOFKID", strPSOFKID);
        param[1] = new SqlParameter("@Year", strYear);

        objDL.ReturnDataSet("TrackSheetReportFROMJobsFoRDMRBM", CommandType.StoredProcedure, ds, param);
        return ds;
    }
    public DataSet getPrescriptionByPSO(int? strPSOFKID, DateTime FromDate, DateTime ToDate, int? TerritoryFkid, int? ProductFkid, int? BVOStatus)
    {

        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[6];
        param[0] = new SqlParameter("@PsoFKID", strPSOFKID);
        param[1] = new SqlParameter("@FromDate", FromDate);
        param[2] = new SqlParameter("@ToDate", ToDate);
        param[3] = new SqlParameter("@TerritoryFKID", TerritoryFkid);
        param[4] = new SqlParameter("@ProductFKID", ProductFkid);
        param[5] = new SqlParameter("@BVOStatus", BVOStatus);

        objDL.ReturnDataSet("getPrescriptionByPSO", CommandType.StoredProcedure, ds, param);
        return ds;
    }
    public DataSet GetSampleStatusReportByDMPSOWise(int? Territory, int? PSOFKID, string ProductFKID, int? StrMonth, int? Stryear, string XmlData)
    {

        DataSet ds = new DataSet();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        SqlParameter[] param = new SqlParameter[6];
        param[0] = new SqlParameter("@TerritoryFKID", Territory);
        param[1] = new SqlParameter("@PSOFKID", PSOFKID);
        param[2] = new SqlParameter("@ProductFKID", ProductFKID);
        param[3] = new SqlParameter("@Mon", StrMonth);
        param[4] = new SqlParameter("@Yr", Stryear);
        param[5] = new SqlParameter("@XmlData", XmlData);

        objDL.ReturnDataSet("GetSampleStatusReportByDMPSOWise", CommandType.StoredProcedure, ds, param);
        return ds;
    }

    public string GetSampleStatusReportByDMPSOWiseDetails(string Territory, string PSOFKID, string ProductFKID, string StrMonth, string Stryear, string XmlData)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", Territory);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("Mon", StrMonth);
        nodes.Add("Yr", Stryear);
        nodes.Add("XmlData", XmlData);
        SPManager spm = new SPManager(nodes);
        return "<Root>" + spm.ExecuteXmlProcedure("GetSampleStatusReportByDMPSOWise") + "</Root>";

    }
    public string GetSampleStatusReportByDMPSOWiseDetails1(string Territory, string PSOFKID, string ProductFKID, string StrMonth, string Stryear, string SampleFKID, string Flag)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", Territory);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("Mon", StrMonth);
        nodes.Add("Yr", Stryear);
        nodes.Add("SampleFKID", SampleFKID);
        SPManager spm = new SPManager(nodes);
        if (Flag == "1")
            return "<Root>" + spm.ExecuteXmlProcedure("GetProductSampleStatusReportRecvd") + "</Root>";
        else
            return "<Root>" + spm.ExecuteXmlProcedure("GetProductSampleStatusReportRecvd") + "</Root>";
    }
    public string GetHistoryReportSummeryForDoctor(string PKID, string Month, string Year, string CType)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("PKID", PKID);
        nodes.Add("months", Month);
        nodes.Add("Year", Year);
        nodes.Add("CustomerType", CType);
        SPManager spm = new SPManager(nodes);

        return "<Root>" + spm.ExecuteXmlProcedure("GetHistoryRepSumerybyXML") + "</Root>";
    }
    public string SampleStatusReportForRBM(string TerritoryFKID, string PSOFKID, string ProductFKID, string Mon, string Yr, string XmlData, string XmlDMData)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", TerritoryFKID);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("Mon", Mon);
        nodes.Add("Yr", Yr);
        nodes.Add("XmlData", XmlData);
        nodes.Add("XmlDMData", XmlDMData);

        SPManager spm = new SPManager(nodes);

        return "<Root>" + spm.ExecuteXmlProcedure("GetSampleStatusReportByRBMPSOWise") + "</Root>";
    }
    public string SampleStatusReportForRBM1(string TerritoryFKID, string PSOFKID, string ProductFKID, string Mon, string Yr, string XmlData)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", TerritoryFKID);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("MOn", Mon);
        nodes.Add("Yr", Yr);
        nodes.Add("XmlData", XmlData);

        SPManager spm = new SPManager(nodes);

        return "<Root>" + spm.ExecuteXmlProcedure("GetSampleStatusReportByRBMPSOWise") + "</Root>";
    }
   
    #region SamplestatusForPSO

    public string GetSampleStatusReportBySSPSO(string TerritoryFKID, string PSOFKID, string ProductFKID, string Mon, string Yr)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", TerritoryFKID);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("MOn", Mon);
        nodes.Add("Yr", Yr);
        SPManager spm = new SPManager(nodes);

        return "<Root>" + spm.ExecuteXmlProcedure("GetSampleStatusReportByPSO") + "</Root>";
    }
    public string GetSampleStatusReportBySSPSORecieved(string TerritoryFKID, string PSOFKID, string ProductFKID, string Mon, string Yr, string SampleFKID)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", TerritoryFKID);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("MOn", Mon);
        nodes.Add("Yr", Yr);
        nodes.Add("SampleFKID", SampleFKID);

        SPManager spm = new SPManager(nodes);

        return "<Root>" + spm.ExecuteXmlProcedure("GetProductSampleStatusReportRecvd ") + "</Root>";
    }
    public string GetSampleStatusReportBySSPSODIS(string TerritoryFKID, string PSOFKID, string ProductFKID, string Mon, string Yr, string SampleFKID)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("TerritoryFKID", TerritoryFKID);
        nodes.Add("PSOFKID", PSOFKID);
        nodes.Add("ProductFKID", ProductFKID);
        nodes.Add("MOn", Mon);
        nodes.Add("Yr", Yr);
        nodes.Add("SampleFKID", SampleFKID);

        SPManager spm = new SPManager(nodes);

        return "<Root>" + spm.ExecuteXmlProcedure("GetProductSampleStatusReportDBmnt ") + "</Root>";
    }
    #endregion


    #region PSOWisePrescriptionTracker
    public string GetPSOWisePrescription(string FromDate, string ToDate, string XMLDocPsoXML, string ProductFKID)
    {
        NodeCollection nodes = new NodeCollection();
        nodes.Add("FromDate", FromDate);
        nodes.Add("ToDate", ToDate);
        nodes.Add("XMLDocPsoXML", XMLDocPsoXML);
        nodes.Add("ProductFKID", ProductFKID);

        SPManager spm = new SPManager(nodes);

        return "<Root>" + spm.ExecuteXmlProcedure("getPrescriptionForSM_New") + "</Root>";
    }
    #endregion
    
    
    #endregion

    #region Nagarajan

    public DataTable UnlockCyclePlanYear()
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[0];
            objDL.ReturnDataTable("GetCycleYearMonth", CommandType.StoredProcedure, dt, param);

        }
        catch (Exception se)
        {
            se.ToString();
        }
        return dt;
    }

    //Customer Deletion New Request
    public DataTable GetAllRequestCustormeDeletion(string User, string Brick, string Specility, string Status)
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@PSOFKID", Convert.ToInt32(User));
            param[1] = new SqlParameter("@BrickFKID", Convert.ToInt32(Brick));
            param[2] = new SqlParameter("@SpecFKID", Convert.ToInt32(Specility));
            param[3] = new SqlParameter("@Stauts", Convert.ToInt32(Status));
            objDL.ReturnDataTable("GetAllRequestByCusDeletion", CommandType.StoredProcedure, dt, param);



        }
        catch (Exception se)
        {
            se.ToString();
        }
        return dt;
    }
    //Customer Pending New Request
    public DataTable GetAllPendingCustormeDeletion(string User, string Brick, string Specility, string Status)
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@PSOFKID", Convert.ToInt32(User));
            param[1] = new SqlParameter("@BrickFKID", Convert.ToInt32(Brick));
            param[2] = new SqlParameter("@SpecFKID", Convert.ToInt32(Specility));
            param[3] = new SqlParameter("@Stauts", Status);
            objDL.ReturnDataTable("GetAllPendingByCusDeletion", CommandType.StoredProcedure, dt, param);



        }
        catch (Exception se)
        {
            se.ToString();
        }
        return dt;
    }

    //Customer Approved New Request
    public DataTable GetAllApprovedCustormeDeletion(string User, string Brick, string Specility, string Status)
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@PSOFKID", Convert.ToInt32(User));
            param[1] = new SqlParameter("@BrickFKID", Convert.ToInt32(Brick));
            param[2] = new SqlParameter("@SpecFKID", Convert.ToInt32(Specility));
            param[3] = new SqlParameter("@Stauts", Status);
            objDL.ReturnDataTable("GetAllApprovedByCusDeletion", CommandType.StoredProcedure, dt, param);



        }
        catch (Exception se)
        {
            se.ToString();
        }
        return dt;
    }

    //Customer Reject New Request
    public DataTable GetAllRejectCustormeDeletion(string User, string Brick, string Specility, string Status)
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@PSOFKID", Convert.ToInt32(User));
            param[1] = new SqlParameter("@BrickFKID", Convert.ToInt32(Brick));
            param[2] = new SqlParameter("@SpecFKID", Convert.ToInt32(Specility));
            param[3] = new SqlParameter("@Stauts", Status);
            objDL.ReturnDataTable("GetAllRejectByCusDeletion", CommandType.StoredProcedure, dt, param);



        }
        catch (Exception se)
        {
            se.ToString();
        }
        return dt;
    }


    //Chemist PrescriptionTracker Reports
    public DataTable GetChemistPresTrackRep(string Use, string TerriortyCode, string Product, string FrmDate, string ToDate)
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[5];
            param[0] = new SqlParameter("@PsoFKID", Convert.ToInt32(Use));
            param[1] = new SqlParameter("@FromDate", FrmDate);
            param[2] = new SqlParameter("@ToDate", ToDate);
            param[3] = new SqlParameter("@TerritoryFKID", Convert.ToInt32(TerriortyCode));
            param[4] = new SqlParameter("@ProductFKID", Convert.ToInt32(Product));
            objDL.ReturnDataTable("getPrescriptionChemistByPSO", CommandType.StoredProcedure, dt, param);
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
        return dt;
    }

    //Chemist PrescriptionTracker Reports for RBM
    public DataTable GetChemistPresTrackforRBMRep(string PSOName, string Product, string FrmDate, string ToDate)
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@FromDate", FrmDate);
            param[1] = new SqlParameter("@ToDate", ToDate);
            param[2] = new SqlParameter("@XMLDocPsoXML", PSOName);
            param[3] = new SqlParameter("@ProductFKID", Convert.ToInt32(Product));
            objDL.ReturnDataTable("getPrescriptionForSM_New", CommandType.StoredProcedure, dt, param);
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
        return dt;
    }
    public DataTable GetCurrentCycleForPSoNumber()
    {
        DataTable dt = new DataTable();
        PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
        objDL.ReturnDataTable("GetCurrentCycleForPSo", CommandType.StoredProcedure, dt);
        return dt;
    }


    public DataTable GetIncludeCustomer(string User, string Brick, string Territory, string xmlSpecification)
    {
        try
        {
            dt.Clear();
            PfizerDAL objDL = new PfizerDAL(HttpContext.Current.Session);
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@PSOFKID", Convert.ToDecimal(User));
            param[1] = new SqlParameter("@AreaFKID", Brick);
            param[2] = new SqlParameter("@TerritoryFKID", Convert.ToDecimal(Territory));
            param[3] = new SqlParameter("@XMLCusCode", xmlSpecification);
            objDL.ReturnDataTable("AllNewCustomerRequest", CommandType.StoredProcedure, dt, param);
        }
        catch (Exception ex)
        {
            ex.ToString();
        }
        return dt;
    }



    #endregion

}