using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SifyGrid;
using SifyGrid.GridHelpers;
using PfizerEntity;
using System.Data.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Data;
using System.Web.Script.Serialization;
using Sify.Global;
using Sify.DBManager;
using System.Xml.Linq;
using System.Globalization;




namespace Pfizer.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Reports/
        #region rajKumar
        #region Declarations
        private pfizerEntities dcReports = new pfizerEntities();
        PfizerBL bLayer = new PfizerBL();
        ExportPdf pdf = new ExportPdf();
        string path = ConfigurationManager.AppSettings["PDFfilePath"].ToString();
        string xslPath = ConfigurationManager.AppSettings["xslPath"].ToString();


        #endregion

        #region Customer Rpt
        public ActionResult NewCustomerReportPSO()
        {
            ViewBag.Today = DateTime.Today.ToShortDateString();
            return View();
        }
        public JsonResult NewCustomeGrid(GridQueryModel gridQueryModel, int PSO)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var result = dcReports.usp_SELECT_CustList_COUNT_ForDashboard(PSO).Select(x => new
                {
                    x.Custtype,
                    x.MCL,
                    x.PCL
                }).OrderBy(x => x.Custtype).ToList();



                var count = result.Count();
                var pageData = result.OrderBy(x => x.PCL).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                count = result.Count();

                if (gridQueryModel.sidx == "Custtype" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.Custtype ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Custtype" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.Custtype descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "MCL" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.MCL ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "MCL" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.MCL descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PCL" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.PCL ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PCL" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.PCL descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);




                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }
        #endregion

        #region Chemist Rpt
        public ActionResult CustomerRpt(string custType, string flag, string input)
        {

            //var rsltPSO=(from a in dcReports.usp_GetPSOName(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())) 
            //              select new {a.PSOName,a.Element}
            //              ).FirstOrDefault();
            //var rsltTerritory=(from a in dcReports.usp_GetCustTerritory(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString()))
            //              select new {a.TerCode,a.TerName}
            //              ).FirstOrDefault();

            //ViewBag.PSO=rsltPSO.PSOName;
            //ViewBag.Territory=rsltTerritory.TerCode+" ( "+rsltTerritory.TerName+" )";
            //ViewBag.custType = custType;
            //ViewBag.flag = flag;
            //ViewBag.input = input;

            return PartialView("Partila1");
        }

        public JsonResult CustomerRptGridData(GridQueryModel gridQueryModel, string custType, string flag, string input)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                var result = dcReports.usp_GetChemistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
                {
                    x.ChemistName,
                    x.AreaName,
                    x.email
                }).OrderBy(x => x.AreaName).ToList();



                var count = result.Count();
                var pageData = result.OrderBy(x => x.AreaName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                count = result.Count();

                if (gridQueryModel.sidx == "ChemistName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.ChemistName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ChemistName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.ChemistName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);




                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }

        #region Fetch Pso Details
        public JsonResult JsonGetHeader(string custType, string flag, string input)
        {
            string PSO = "", territory = "";

            var rsltPSO = (from a in dcReports.usp_GetPSOName(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString()))
                           select new { a.PSOName, a.Element }
                     ).FirstOrDefault();

            //var rsltPSO = (from a in dcReports.usp_GetPSOName(Convert.ToDecimal(input), custType, flag, 339, 339, 15)
            //               select new { a.PSOName, a.Element }
            //    ).FirstOrDefault();

            var rsltTerritory = (from a in dcReports.usp_GetCustTerritory(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString()))
                                 select new { a.TerCode, a.TerName }
                          ).FirstOrDefault();

            //var rsltTerritory = (from a in dcReports.usp_GetCustTerritory(Convert.ToDecimal(input), custType, flag, 339, 339, 15)
            //                     select new { a.TerCode, a.TerName }
            //              ).FirstOrDefault();



            return Json(new
            {
                PSO = (rsltPSO.PSOName == null) ? "" : rsltPSO.PSOName,
                territory = (rsltTerritory.TerCode == null) ? "" : rsltTerritory.TerCode + " (" + rsltTerritory.TerName + ")",
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion
        public ActionResult ExportCustomerRptExcel(string custType, string flag, string input)
        {
            var context = dcReports.usp_GetChemistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
            {
                x.ChemistName,
                x.AreaName,
                x.email
            }).OrderBy(x => x.AreaName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
               
                item.ChemistName == null ? "" :item.ChemistName,                
                item.AreaName == null ? "" :item.AreaName,
                 item.email == null ? "" :item.email
               
            }));
            return new ExcelResult(HeadersCustomerRpt, ColunmTypesCustomerRpt, data, "CustomerReport.xlsx", "Customer Report");
        }
        private static readonly string[] HeadersCustomerRpt = {
                 "Chemist Name","Area","Email"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesCustomerRpt = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,
                 DataForExcel.DataType.String
            };
        public ActionResult ExportCustomerRptToPDF(string custType, string flag, string input)
        {
            List<CustomerRptCls> userList = new List<CustomerRptCls>();
            var context = dcReports.usp_GetChemistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
            {
                x.ChemistName,
                x.AreaName,
                x.email
            }).OrderBy(x => x.AreaName).ToList();

            foreach (var item in context)
            {
                CustomerRptCls user = new CustomerRptCls();

                user.ChemistName = item.ChemistName == null ? "" : item.ChemistName;
                user.AreaName = item.AreaName == null ? "" : item.AreaName;
                user.email = item.email == null ? "" : item.email;

                userList.Add(user);
            }
            var customerList = userList;

            var result = new
            {
                total = 1,
                page = 1,
                records = customerList.Count(),
                rows = (
                    customerList.Select(e =>
                        new
                        {

                            cell = new string[]{
                           e.ChemistName,e.AreaName,e.email
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "ChemistName", "AreaName", "email" }, path);
            return File(path, "application/pdf", "CustomerReport.pdf");
        }

        public class CustomerRptCls
        {
            public string ChemistName { get; set; }
            public string AreaName { get; set; }
            public string email { get; set; }
        }

        #endregion Chemist

        #region Doctor Rpt

        public JsonResult DocotorRptGridData(GridQueryModel gridQueryModel, string custType, string flag, string input)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                var result = dcReports.usp_GetDoctorRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
                {
                    DoctorCode = (x.DoctorCode == null) ? 0 : x.DoctorCode,
                    DoctorName = (x.DoctorName == null) ? "" : x.DoctorName,
                    Specialization = (x.Specialization == null) ? "" : x.Specialization,
                    AreaName = (x.AreaName == null) ? "" : x.AreaName,
                    KOLFlag = (x.KOLFlag == null) ? "" : x.KOLFlag,
                    RegistrationNo = (x.RegistrationNo == null) ? "" : x.RegistrationNo,
                    email = (x.email == null) ? "" : x.email,
                }).OrderBy(x => x.AreaName).ToList();



                var count = result.Count();
                var pageData = result.OrderBy(x => x.AreaName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                count = result.Count();

                if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorCode" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.DoctorCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorCode" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.DoctorCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.DoctorName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.DoctorName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.Specialization ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.Specialization descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "KOLFlag" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.KOLFlag ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "KOLFlag" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.KOLFlag descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "RegistrationNo" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.RegistrationNo ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "RegistrationNo" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.RegistrationNo descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }


        public ActionResult ExportDocotorRptExcel(string custType, string flag, string input)
        {


            var context = dcReports.usp_GetDoctorRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
            {
                DoctorCode = (x.DoctorCode == null) ? 0 : x.DoctorCode,
                DoctorName = (x.DoctorName == null) ? "" : x.DoctorName,
                Specialization = (x.Specialization == null) ? "" : x.Specialization,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                KOLFlag = (x.KOLFlag == null) ? "" : x.KOLFlag,
                RegistrationNo = (x.RegistrationNo == null) ? "" : x.RegistrationNo,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.AreaName).ToList();

            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
              
                item.DoctorName,
                item.DoctorCode.ToString()=="0"?"":item.DoctorCode.ToString(),
                item.Specialization,
                item.AreaName,
                item.KOLFlag,
                item.RegistrationNo,
                item.email
               
               
            }));
            return new ExcelResult(HeadersDocotorRpt, ColunmTypesDocotorRpt, data, "DoctorReport.xlsx", "Doctor Report");
        }
        private static readonly string[] HeadersDocotorRpt = {
                 "Doctor","Doctor Code","Specialization","Area","KOL","Registration No.","Email"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesDocotorRpt = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportDocotorRptToPDF(string custType, string flag, string input)
        {
            List<DocotorRptCls> userList = new List<DocotorRptCls>();
            var context = dcReports.usp_GetDoctorRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
            {
                DoctorCode = (x.DoctorCode == null) ? 0 : x.DoctorCode,
                DoctorName = (x.DoctorName == null) ? "" : x.DoctorName,
                Specialization = (x.Specialization == null) ? "" : x.Specialization,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                KOLFlag = (x.KOLFlag == null) ? "" : x.KOLFlag,
                RegistrationNo = (x.RegistrationNo == null) ? "" : x.RegistrationNo,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.AreaName).ToList();

            foreach (var item in context)
            {
                DocotorRptCls user = new DocotorRptCls();

                user.DoctorCode = item.DoctorCode.ToString();
                user.DoctorName = item.DoctorName == null ? "" : item.DoctorName;
                user.Specialization = item.Specialization == null ? "" : item.Specialization;
                user.AreaName = item.AreaName == null ? "" : item.AreaName;
                user.KOLFlag = item.KOLFlag == null ? "" : item.KOLFlag;
                user.RegistrationNo = item.RegistrationNo == null ? "" : item.RegistrationNo;
                user.email = item.email == null ? "" : item.email;

                userList.Add(user);
            }
            var customerList = userList;

            var result = new
            {
                total = 1,
                page = 1,
                records = customerList.Count(),
                rows = (
                    customerList.Select(e =>
                        new
                        {

                            cell = new string[]{
                         e.DoctorName,e.DoctorCode,e.Specialization,e.AreaName,e.KOLFlag,e.RegistrationNo,e.email
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "DoctorName", "DoctorCode", "Specialization", "AreaName", "KOLFlag", "RegistrationNo", "email" }, path);
            return File(path, "application/pdf", "DoctorReport.pdf");
        }

        public class DocotorRptCls
        {
            public string DoctorCode { get; set; }
            public string DoctorName { get; set; }
            public string Specialization { get; set; }
            public string AreaName { get; set; }
            public string KOLFlag { get; set; }
            public string RegistrationNo { get; set; }
            public string email { get; set; }
        }

        #endregion Doctor Rpt

        #region Hospital Rpt

        public JsonResult HospitalRptGridData(GridQueryModel gridQueryModel, string custType, string flag, string input)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                var result = dcReports.usp_GetHospitalRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
                {
                    NameofHospital = (x.NameofHospital == null) ? "" : x.NameofHospital,
                    CityName = (x.CityName == null) ? "" : x.CityName,
                    AreaName = (x.AreaName == null) ? "" : x.AreaName,
                    email = (x.email == null) ? "" : x.email,
                }).OrderBy(x => x.NameofHospital).ToList();



                var count = result.Count();
                var pageData = result.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                count = result.Count();

                if (gridQueryModel.sidx == "NameofHospital" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.NameofHospital ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "NameofHospital" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.NameofHospital descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.CityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.CityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }


        public ActionResult ExportHospitalRptExcel(string custType, string flag, string input)
        {


            var context = dcReports.usp_GetHospitalRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
            {
                NameofHospital = (x.NameofHospital == null) ? "" : x.NameofHospital,
                CityName = (x.CityName == null) ? "" : x.CityName,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.NameofHospital).ToList();


            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
              
                item.NameofHospital,
                item.CityName, 
                item.AreaName, 
                item.email
               
               
            }));
            return new ExcelResult(HeadersHospitalRpt, ColunmTypesHospitalRpt, data, "HospitalReport.xlsx", "Hospital Report");
        }
        private static readonly string[] HeadersHospitalRpt = {
                 "Hospital Name","City","Area","Email"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesHospitalRpt = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
                DataForExcel.DataType.String 
            };
        public ActionResult ExportHospitalRptToPDF(string custType, string flag, string input)
        {
            List<HospitalRptCls> userList = new List<HospitalRptCls>();
            var context = dcReports.usp_GetHospitalRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
            {
                NameofHospital = (x.NameofHospital == null) ? "" : x.NameofHospital,
                CityName = (x.CityName == null) ? "" : x.CityName,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.NameofHospital).ToList();

            foreach (var item in context)
            {
                HospitalRptCls user = new HospitalRptCls();

                user.HospitalName = item.NameofHospital.ToString();
                user.City = item.CityName;
                user.AreaName = item.AreaName;
                user.email = item.email;
                userList.Add(user);
            }
            var customerList = userList;

            var result = new
            {
                total = 1,
                page = 1,
                records = customerList.Count(),
                rows = (
                    customerList.Select(e =>
                        new
                        {

                            cell = new string[]{
                         e.HospitalName,e.City,e.AreaName,e.email
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "HospitalName", "City", "AreaName", "AreaName", "email" }, path);
            return File(path, "application/pdf", "HospitalReport.pdf");
        }

        public class HospitalRptCls
        {
            public string HospitalName { get; set; }
            public string City { get; set; }
            public string AreaName { get; set; }
            public string email { get; set; }
        }


        #endregion

        #region Stockist Rpt

        public JsonResult StockistRptGridData(GridQueryModel gridQueryModel, string custType, string flag, string input)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;



                var result = dcReports.usp_GetStockistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
                {
                    NameofStockist = (x.NameofStockist == null) ? "" : x.NameofStockist,
                    CityName = (x.CityName == null) ? "" : x.CityName,
                    AreaName = (x.AreaName == null) ? "" : x.AreaName,
                    email = (x.email == null) ? "" : x.email,
                }).OrderBy(x => x.NameofStockist).ToList();



                var count = result.Count();
                var pageData = result.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                count = result.Count();

                if (gridQueryModel.sidx == "NameofStockist" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.NameofStockist ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "NameofStockist" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.NameofStockist descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.CityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.CityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }


        public ActionResult ExportStockistRptExcel(string custType, string flag, string input)
        {


            var context = dcReports.usp_GetStockistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
            {
                NameofStockist = (x.NameofStockist == null) ? "" : x.NameofStockist,
                CityName = (x.CityName == null) ? "" : x.CityName,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.NameofStockist).ToList();


            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
              
                item.NameofStockist,
                item.CityName, 
                item.AreaName, 
                item.email
               
               
            }));
            return new ExcelResult(HeadersStockistRpt, ColunmTypesStockistRpt, data, "StockiestReport.xlsx", "Stockiest Report");
        }
        private static readonly string[] HeadersStockistRpt = {
                 "Stockist Name","City","Area","Email"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesStockistRpt = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
                DataForExcel.DataType.String 
            };
        public ActionResult ExportStockistRptToPDF(string custType, string flag, string input)
        {
            List<StockistRptCls> userList = new List<StockistRptCls>();
            var context = dcReports.usp_GetStockistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"].ToString()), Convert.ToInt32(Session["NodeType_FKID"].ToString())).Select(x => new
            {
                NameofStockist = (x.NameofStockist == null) ? "" : x.NameofStockist,
                CityName = (x.CityName == null) ? "" : x.CityName,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.NameofStockist).ToList();

            foreach (var item in context)
            {
                StockistRptCls user = new StockistRptCls();

                user.StockistName = item.NameofStockist.ToString();
                user.City = item.CityName;
                user.AreaName = item.AreaName;
                user.email = item.email;
                userList.Add(user);
            }
            var customerList = userList;

            var result = new
            {
                total = 1,
                page = 1,
                records = customerList.Count(),
                rows = (
                    customerList.Select(e =>
                        new
                        {

                            cell = new string[]{
                         e.StockistName,e.City,e.AreaName,e.email
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "StockistName", "City", "AreaName", "AreaName", "email" }, path);
            return File(path, "application/pdf", "StockiestReport.pdf");
        }

        public class StockistRptCls
        {
            public string StockistName { get; set; }
            public string City { get; set; }
            public string AreaName { get; set; }
            public string email { get; set; }
        }

        #endregion

        #region DailyReportAtGlanceforManager
        public ActionResult DailyReportAtGlanceforManagerNew()
        {

            return View();

        }


        public JsonResult JsonLoadMgrUser(string NodeTypeFKID)
        {

            DataSet ds = bLayer.GetPSONameDRAtGforRG(Session["USER_FKID"].ToString(), NodeTypeFKID);

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new UsefulLink.DailyRptPSO { UserFKID = dataRow.Field<string>("UserFKID"), EmpName = dataRow.Field<string>("EmpName") }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.UserFKID
                }).ToList();
            }

            return Json(new
            {
                userItems = roleItems,
                count = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult JsonAllRptMGRforDM(string frmDate, string toDate, string nodeName, string user)
        {
            try
            {
                string[] from = frmDate.Split('/');
                string[] to = toDate.Split('/');

                string xml = bLayer.GetDailyReportatGlanceAllActivityPSONew(from[1] + "/" + from[0] + "/" + from[2], to[1] + "/" + to[0] + "/" + to[2], user, user);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "DailyReportAtAGlanceForRBM.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }



        public JsonResult JsonAllRptMGRforPSO(string frmDate, string toDate, string user)
        {
            try
            {
                string[] from = frmDate.Split('/');
                string[] to = toDate.Split('/');

                string xml = bLayer.GetDailyReportatGlanceAllNew(from[1] + "/" + from[0] + "/" + from[2], to[1] + "/" + to[0] + "/" + to[2], user);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "DailyReportAtAGlanceforPSO.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public JsonResult JsonRegRptMGRforDM(string frmDate, string toDate, string nodeName, string user)
        {
            try
            {
                string[] from = frmDate.Split('/');
                string[] to = toDate.Split('/');

                string xml = bLayer.GetDailyReportatGlancePSORGNew(from[1] + "/" + from[0] + "/" + from[2], to[1] + "/" + to[0] + "/" + to[2], user, user);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "DailyReportAtAGlanceForRBM.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public JsonResult JsonRegRptMGRforPSO(string frmDate, string toDate, string user)
        {
            try
            {
                string[] from = frmDate.Split('/');
                string[] to = toDate.Split('/');

                string xml = bLayer.GetDailyReportatGlanceRGNew(from[1] + "/" + from[0] + "/" + from[2], to[1] + "/" + to[0] + "/" + to[2], user);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "DailyReportAtAGlanceforPSO.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }


        #endregion

        #region DailyReportAtGlanceNew
        public ActionResult DailyReportAtGlanceNew()
        {


            return View();
        }

        public JsonResult JsonDailyRptPSO(string frmDate, string toDate, string psoId)
        {
            try
            {
                string[] from = frmDate.Split('/');
                string[] to = toDate.Split('/');

                string xml = bLayer.GetDailyReportatGlanceAllNew(from[1] + "/" + from[0] + "/" + from[2], to[1] + "/" + to[0] + "/" + to[2], psoId);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "DailyReportAtAGlanceforPSO.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }



        public JsonResult JsonDailyRptRegPSO(string frmDate, string toDate, string psoId)
        {
            try
            {
                string[] from = frmDate.Split('/');
                string[] to = toDate.Split('/');

                string xml = bLayer.GetDailyReportatGlanceRGNew(from[1] + "/" + from[0] + "/" + from[2], to[1] + "/" + to[0] + "/" + to[2], psoId);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "DailyReportAtAGlanceforPSO.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }


        #endregion

        #region CycleplanCoverageRptPSO

        public ActionResult CycleplanCoverageRptPSO()
        {

            return View();
        }

        public JsonResult JsonPrintPSORpt(string month, string year)
        {
            string xml = bLayer.CyclePlanCoverageRpt(Session["USER_FKID"].ToString(), month, year);
            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "CycleplanCoverageRptPsoWise.xsl";
            trans.XmlString = xml;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            Session["tblExcel"] = rslt;
            rslt = "<table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }

        #region ExcelDownload
        [HttpGet]
        public ActionResult ExcelDownload(string fileName)
        {
            //string xml = bLayer.CyclePlanCoverageRpt(Session["USER_FKID"].ToString(), month, year);
            //XmlTransform trans = new XmlTransform();
            //trans.XslFile = xslPath + "CycleplanCoverageRptPsoWise.xsl";
            //trans.XmlString = xml;
            //var rslt = trans.Transform();
            //rslt = rslt.Remove(0, 39);
            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            System.Web.HttpContext.Current.Response.Charset = " ";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;fileName=" + fileName + ".xls");

            System.Web.HttpContext.Current.Response.Write(Session["tblExcel"].ToString());

            Response.End();
            return View();
        }
        #endregion


        #endregion


        #region DailyRptatGlanceRegularVisit

        public ActionResult DailyRptatGlanceRegularVisit()
        {


            return View();
        }

        public JsonResult JsonLoadUser(string nodeId, string nodeValue)
        {

            DataSet ds = new DataSet();
            ds = bLayer.GetPSONameDRAtGforRG(Session["USER_FKID"].ToString(), nodeId);

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new UsefulLink.DailyRptPSO { UserFKID = dataRow.Field<string>("UserFKID"), EmpName = dataRow.Field<string>("EmpName") }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.UserFKID
                }).ToList();
            }

            return Json(new
            {
                userItems = roleItems,
                count = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult JsonPSODailyRpt(string reportDate, string type)
        {
            string[] date = reportDate.Split('/');

            string xml = bLayer.DRatGlanceforPSO(Session["USER_FKID"].ToString(), date[2] + "-" + date[1] + "-" + date[0]);
            XmlTransform trans = new XmlTransform();
            if (type == "1")
                trans.XslFile = xslPath + "DailyReportatDlanceRGDisp.xsl";
            if (type == "2")
                trans.XslFile = xslPath + "DailyReportatDlanceRGDispRBM.xsl";
            trans.XmlString = xml;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            Session["tblExcel"] = rslt;
            rslt = "<table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }


        #endregion

        #region CallVolumesReport
        public ActionResult CallVolumesReport()
        {

            return View();
        }



        public JsonResult JsonLoadTeamName(string nodeId, string nodeValue)
        {

            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();

            var list = (from s in dcReports.TeamsForCallVolumeReport() select new { s.PKID, s.TeamName }).ToList();
            if (list != null)
            {
                returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.TeamName.ToString(),
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                teamItems = returnValue,
                count = returnValue.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonLoadPSOUser(long? teamfkid)
        {
            List<SelectListItem> returnValue = new List<SelectListItem>();
            returnValue.Clear();

            var list = (from s in dcReports.TeamPSOList(teamfkid) select new { s.PKID, s.Name }).ToList();
            if (list != null)
            {
                returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.Name.ToString(),
                    Value = x.PKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                userItems = returnValue,
                count = returnValue.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonVolumeRpt(string year, string month, string users)
        {
            try
            {
                string[] splitUsers = users.Split(',');

                var str = string.Empty;
                str = "<root>";
                foreach (var item in splitUsers)
                {
                    str += "<User";
                    str += " UserFKID='" + item + "'";
                    str += "/>";
                }
                str += "</root>";

                string xml = bLayer.CallVolumesReport(year, month, str);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "CallVolumesReport.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }



        #endregion

        #region InputStatusReportforPSO

        public ActionResult InputStatusReportforPSO()
        {

            return View();

        }

        public JsonResult JsonLoadProducts()
        {

            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();

            var list = (from s in dcReports.GetInputNameforReport(Convert.ToDecimal(Session["USER_FKID"].ToString())) select new { s.PKID, s.InputName }).ToList();
            if (list != null)
            {
                returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.InputName.ToString(),
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                productItems = returnValue,
                count = returnValue.Count,

            }, JsonRequestBehavior.AllowGet);

        }



        public JsonResult JsonPSOStatusRpt(string year, string month, string products)
        {
            try
            {


                string xml = bLayer.GetInputStatusReportByPSO(Session["USER_FKID"].ToString(), products, month, year);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "InputStatusReportforPSO.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public JsonResult JsonInputPSODetailRpt(string year, string month, string input)
        {
            try
            {


                string xml = bLayer.GetInputStatusReportRecvd(Session["USER_FKID"].ToString(), month, year, input);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "InputStatusRptPSORec.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }
        public JsonResult JsonInputPSODisbursedRpt(string year, string month, string Products, string InputId)
        {
            try
            {


                string xml = bLayer.GetProductSampleStatusReportDBmnt(Session["Territory_FKID"].ToString(), Session["USER_FKID"].ToString(), Products, month, year, InputId);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "SampleStatusRptPSODisbursed.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        #endregion

        #region InputStatusReportforDM

        public ActionResult InputStatusReportforDM()
        {
            ViewBag.Date = DateTime.Now.ToShortDateString();
            return View();
        }

        public JsonResult JsonDMStatusRpt(string year, string month, string products, string psos)
        {
            try
            {

                string xmlData = string.Empty;

                if (psos != "null" && psos != "")
                {
                    string[] arr = psos.Split(',');

                    foreach (var item in arr)
                    {
                        xmlData += "<dp PSOFKID=\"" + item + "\"/>";

                    }
                }
                xmlData = "<row>" + xmlData + "</row>";

                string xml = bLayer.GetInputStatusReportByDMPSOWise(Session["Territory_FKID"].ToString(), Session["USER_FKID"].ToString(), products, month, year, xmlData);
                XmlTransform trans = new XmlTransform();
                if (xmlData == "<row></row>")
                    trans.XslFile = xslPath + "InputStatusRptDM.xsl";
                else
                    trans.XslFile = xslPath + "InputStatusRptDMPsoWise.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public JsonResult JsonLoadDMProducts()
        {

            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            var list = (from s in dcReports.GetInputNameforDMReport() select new { s.PKID, s.InputName }).ToList();
            if (list != null)
            {
                returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.InputName.ToString(),
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                productItems = returnValue,
                count = returnValue.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonLoadPSOName()
        {

            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();

            var list = (from s in dcReports.GetPsoNameBySampleReport(Convert.ToDecimal(Session["USER_FKID"].ToString())) select new { s.PKID, s.EmpName }).ToList();
            if (list != null)
            {
                returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName.ToString(),
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                psoItems = returnValue,
                count = returnValue.Count,

            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult JsonInputDMDetailRpt(string year, string month, string InputId)
        {
            try
            {


                string xml = bLayer.GetInputStatusReportRecvdDM(Session["USER_FKID"].ToString(), month, year, InputId);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "InputStatusRptPSORec.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public JsonResult JsonInputDMDisbursedRpt(string year, string month, string Products, string InputId)
        {
            try
            {


                string xml = bLayer.GetProductSampleStatusReportDBmnt(Session["Territory_FKID"].ToString(), Session["USER_FKID"].ToString(), Products, month, year, InputId);
                XmlTransform trans = new XmlTransform();
                trans.XslFile = xslPath + "SampleStatusRptPSODisbursed.xsl";
                trans.XmlString = xml;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table style='vertical-align:middle' align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table  align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb'   border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td  align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        #endregion

        #endregion

        #region Mathan

        #region Usage Report

        public ActionResult UsageReport()
        {
            return View();
        }

        #region Bind Data To Grid

        public JsonResult GetUsageReport(GridQueryModel gridQueryModel, string PKID, string FDATE, string TODATE)
        {
            try
            {
                decimal UPKID = Convert.ToDecimal(PKID);
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string TODATES = tdate.ToString("MM/dd/yyyy");

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in dcReports.GetUsageRepbyXML(UPKID, FDATES, TODATES) select q).ToList();
                //var query = (from a in OtherMasters.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.UserName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "UserName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.UserName != null && sq.UserName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.UserName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.UserName != null && sq.UserName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.UserName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.UserName != null && sq.UserName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.UserName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.UserName != null && sq.UserName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.UserName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "UserName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.UserName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "UserName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.UserName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "DesignationName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DesignationName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "DesignationName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.DesignationName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "LoginDate" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.LoginDate descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "LoginDate" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.LoginDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "LogOutDate" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.LogOutDate descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "LogOutDate" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.LogOutDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                }

                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,

                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        #endregion

        #region Load PDF, Excel, CSV Files

        public ActionResult ExportUsageReportToExcel(string PKID, string FDATE, string TODATE)
        {
            try
            {
                decimal UPKID = Convert.ToDecimal(PKID);
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string TODATES = tdate.ToString("MM/dd/yyyy");

                var context = (from q in dcReports.GetUsageRepbyXML(UPKID, FDATES, TODATES) select q).OrderBy(x => x.UserName).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.UserName == null ? "" :item.UserName,
                 item.DesignationName == null ? "" :item.DesignationName,
                 item.LoginDate == null ? "" :item.LoginDate,
                 item.LogOutDate == null ? "" :item.LogOutDate,
             
                
               
            }));
                return new ExcelResult(Headersusagereport, ColunmTypesusagereport, data, "Usagereport.xlsx", "Usagereport");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] Headersusagereport = {
               "UserName","DesignationName","LoginDate","LogOutDate"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesusagereport = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportUsageReportToPDF(string PKID, string FDATE, string TODATE)
        {
            try
            {
                decimal UPKID = Convert.ToDecimal(PKID);
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string TODATES = tdate.ToString("MM/dd/yyyy");

                List<UsageReports> userList = new List<UsageReports>();

                var context = (from q in dcReports.GetUsageRepbyXML(UPKID, FDATES, TODATES) select q).OrderBy(x => x.UserName).ToList();

                foreach (var item in context)
                {
                    UsageReports user = new UsageReports();
                    // user.ProductFkid = item.ProductFKID.ToString();
                    user.UserNames = item.UserName == null ? "" : item.UserName;
                    user.DesignationNames = item.DesignationName == null ? "" : item.DesignationName;
                    user.LoginDates = item.LoginDate == null ? "" : item.LoginDate;
                    user.LogOutDates = item.LogOutDate == null ? "" : item.LogOutDate;
                    userList.Add(user);
                }
                var customerList = userList;


                pdf.ExportPDF(customerList, new string[] { "UserNames", "DesignationNames", "LoginDates", "LogOutDates" }, path);
                return File(path, "application/pdf", "UsageReport.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportUsageReportToCsv(string PKID, string FDATE, string TODATE)
        {
            try
            {
                decimal UPKID = Convert.ToDecimal(PKID);
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string TODATES = tdate.ToString("MM/dd/yyyy");

                var model = (from q in dcReports.GetUsageRepbyXML(UPKID, FDATES, TODATES) select q).OrderBy(x => x.UserName).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("User Name,Designation Name,LoginDate,LogOutDate");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record == null ? "" : record.UserName, record == null ? "" : record.DesignationName, record == null ? "" : record.LoginDate, record == null ? "" : record.LogOutDate

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "UsageReport.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class UsageReports
        {
            public string UserNames { get; set; }
            public string DesignationNames { get; set; }
            public string LoginDates { get; set; }
            public string LogOutDates { get; set; }

        }

        #endregion

        #endregion


        #region Customer Met Or Not Met

        public ActionResult CustomerMetNotMet()
        {

            return View();
        }

        #region Customer Doctor

        #region Bind Data To Grid

        public JsonResult GetCustomerMetNotMet(GridQueryModel gridQueryModel, string PKID, string FDATE, string TODATE, string DOCTYPE, string CUSTYPE, string BRIDGEKOL)
        {
            string str = "";


            try
            {
                str = "<row>";
                string StrSelectedCUSTYPE = CUSTYPE;
                string[] SelectedCUSTYPEvalues = StrSelectedCUSTYPE.Split(',');
                for (int i = 0; i < SelectedCUSTYPEvalues.Length; i++)
                {
                    str += "<dp";
                    str += " GenCode ='" + SelectedCUSTYPEvalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";

                DataSet ds = bLayer.GetCustomerMetNotMetRpt(PKID, FDATE, TODATE, DOCTYPE, BRIDGEKOL, str);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                var query = ds.Tables[1].AsEnumerable().Select(dataRow => new ReportSubmission
                {
                    CustomerName = dataRow[2].ToString(),
                    Specialization = dataRow[3].ToString(),
                    AreaName = dataRow[4].ToString(),
                    PlannedVisits = dataRow[6].ToString(),
                    VisitCount = dataRow[5].ToString(),
                    BalanceVisits = dataRow[7].ToString()
                }).ToList();


                //var dd = ds.Tables[1].Rows.Count.ToString();
                //var query1 = (from q in ds.Tables[1].ToString() select q).ToList();
                //var query = (from q in dcReports.USP_GetDoctorMetbyXMLN(Convert.ToDecimal(PKID), FDATE, TODATE, DOCTYPE, BRIDGEKOL, CUSTYPE) select q).ToList();
                //var query = (from a in OtherMasters.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "CustomerName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.CustomerName != null && sq.CustomerName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.CustomerName != null && sq.CustomerName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.CustomerName != null && sq.CustomerName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.CustomerName != null && sq.CustomerName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "CustomerName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CustomerName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "CustomerName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.CustomerName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Specialization descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.Specialization ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "PlannedVisits" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.PlannedVisits descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "PlannedVisits" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.PlannedVisits ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    else if (gridQueryModel.sidx == "VisitCount" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.VisitCount descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "VisitCount" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.VisitCount ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "BalanceVisits" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.BalanceVisits descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "BalanceVisits" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.BalanceVisits ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                }

                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,

                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        #endregion

        #region Load PDF, Excel, CSV Files For Doctor

        public ActionResult ExportCustomerMetNotMetToExcel(string PKID, string FDATE, string TODATE, string DOCTYPE, string CUSTYPE, string BRIDGEKOL)
        {
            string str = "";

            try
            {
                str = "<row>";
                string StrSelectedCUSTYPE = CUSTYPE;
                string[] SelectedCUSTYPEvalues = StrSelectedCUSTYPE.Split(',');
                for (int i = 0; i < SelectedCUSTYPEvalues.Length; i++)
                {
                    str += "<dp";
                    str += " GenCode ='" + SelectedCUSTYPEvalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";

                DataSet ds = bLayer.GetCustomerMetNotMetRpt(PKID, FDATE, TODATE, DOCTYPE, BRIDGEKOL, str);
                var context = ds.Tables[1].AsEnumerable().Select(dataRow => new ReportSubmission
                {
                    CustomerName = dataRow[2].ToString(),
                    Specialization = dataRow[3].ToString(),
                    AreaName = dataRow[4].ToString(),
                    PlannedVisits = dataRow[6].ToString(),
                    VisitCount = dataRow[5].ToString(),
                    BalanceVisits = dataRow[7].ToString()
                }).ToList();

                //var context = (from q in dcReports.USP_GetDoctorMetbyXMLN(Convert.ToDecimal(PKID), FDATE, TODATE, DOCTYPE, BRIDGEKOL, CUSTYPE) select q).OrderBy(x => x.CustomerName).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.CustomerName== null ? "" :item.CustomerName,
                 item.AreaName == null ? "" :item.AreaName,
                 item.Specialization == null ? "" :item.Specialization,
                 item.PlannedVisits == null ? "" :item.PlannedVisits,
                 item.VisitCount == null ? "" :item.VisitCount,
                 item.BalanceVisits == null ? "" :item.BalanceVisits,
             
                
               
            }));
                return new ExcelResult(HeadersCustometMetNotMet, ColunmTypesCustometMetNotMet, data, "CustometMetNotMet.xlsx", "CustometMetNotMet");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersCustometMetNotMet = {
               "DoctorName","Brick","Specialization", "PlannedVisits", "Visitecount", "BalanceVisite"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesCustometMetNotMet = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportCustomerMetNotMetToPDF(string PKID, string FDATE, string TODATE, string DOCTYPE, string CUSTYPE, string BRIDGEKOL)
        {
            string str = "";

            try
            {
                str = "<row>";
                string StrSelectedCUSTYPE = CUSTYPE;
                string[] SelectedCUSTYPEvalues = StrSelectedCUSTYPE.Split(',');
                for (int i = 0; i < SelectedCUSTYPEvalues.Length; i++)
                {
                    str += "<dp";
                    str += " GenCode ='" + SelectedCUSTYPEvalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";

                DataSet ds = bLayer.GetCustomerMetNotMetRpt(PKID, FDATE, TODATE, DOCTYPE, BRIDGEKOL, str);

                List<CustomerMetNotMets> userList = new List<CustomerMetNotMets>();

                //var context = (from q in dcReports.USP_GetDoctorMetbyXMLN(Convert.ToDecimal(PKID), FDATE, TODATE, DOCTYPE, BRIDGEKOL, CUSTYPE) select q).OrderBy(x => x.CustomerName).ToList();

                var context = ds.Tables[1].AsEnumerable().Select(dataRow => new ReportSubmission
                {
                    CustomerName = dataRow[2].ToString(),
                    Specialization = dataRow[3].ToString(),
                    AreaName = dataRow[4].ToString(),
                    PlannedVisits = dataRow[6].ToString(),
                    VisitCount = dataRow[5].ToString(),
                    BalanceVisits = dataRow[7].ToString()
                }).ToList();

                foreach (var item in context)
                {
                    CustomerMetNotMets user = new CustomerMetNotMets();
                    // user.ProductFkid = item.ProductFKID.ToString();
                    user.DoctorName = item.CustomerName == null ? "" : item.CustomerName;
                    user.Brick = item.AreaName == null ? "" : item.AreaName;
                    user.Specialization = item.Specialization == null ? "" : item.Specialization;
                    user.PlannedVisits = item.PlannedVisits == null ? "" : item.PlannedVisits;
                    user.Visitecount = item.VisitCount == null ? "" : item.VisitCount;
                    user.BalanceVisite = item.BalanceVisits == null ? "" : item.BalanceVisits;

                    userList.Add(user);
                }
                var customerList = userList;


                pdf.ExportPDF(customerList, new string[] { "DoctorName", "Brick", "Specialization", "PlannedVisits", "Visitecount", "BalanceVisite" }, path);
                return File(path, "application/pdf", "CustomerMetNotMet.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportCustomerMetNotMetToCsv(string PKID, string FDATE, string TODATE, string DOCTYPE, string CUSTYPE, string BRIDGEKOL)
        {
            string str = "";

            try
            {
                str = "<row>";
                string StrSelectedCUSTYPE = CUSTYPE;
                string[] SelectedCUSTYPEvalues = StrSelectedCUSTYPE.Split(',');
                for (int i = 0; i < SelectedCUSTYPEvalues.Length; i++)
                {
                    str += "<dp";
                    str += " GenCode ='" + SelectedCUSTYPEvalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";

                DataSet ds = bLayer.GetCustomerMetNotMetRpt(PKID, FDATE, TODATE, DOCTYPE, BRIDGEKOL, str);
                // var model = (from q in dcReports.USP_GetDoctorMetbyXMLN(Convert.ToDecimal(PKID), FDATE, TODATE, DOCTYPE, BRIDGEKOL, CUSTYPE) select q).OrderBy(x => x.CustomerName).ToList();
                var model = ds.Tables[1].AsEnumerable().Select(dataRow => new ReportSubmission
                {
                    CustomerName = dataRow[2].ToString(),
                    Specialization = dataRow[3].ToString(),
                    AreaName = dataRow[4].ToString(),
                    PlannedVisits = dataRow[6].ToString(),
                    VisitCount = dataRow[5].ToString(),
                    BalanceVisits = dataRow[7].ToString()
                }).ToList();

                var sb = new StringBuilder();
                sb.AppendLine("Doctor Name,Brick,Specialization,PlannedVisits,Visitecount,BalanceVisite");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5}", record == null ? "" : record.CustomerName, record == null ? "" : record.AreaName, record == null ? "" : record.Specialization, record == null ? "" : record.PlannedVisits, record == null ? "" : record.VisitCount, record == null ? "" : record.BalanceVisits
                    );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "CustomerMetNotMet.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class CustomerMetNotMets
        {
            public string DoctorName { get; set; }
            public string Brick { get; set; }
            public string Specialization { get; set; }
            public string PlannedVisits { get; set; }
            public string Visitecount { get; set; }
            public string BalanceVisite { get; set; }

        }

        #endregion

        #endregion

        #region Customer Chemist

        #region Bind Data To Chemist Grid

        public JsonResult GetCustomerChemistMetNotMet(GridQueryModel gridQueryModel, string PKID, string FDATE, string TODATE, string DOCTYPE, string CUSTYPE, string BRIDGEKOL)
        {
            string str = "";


            try
            {
                str = "<row>";
                string StrSelectedCUSTYPE = CUSTYPE;
                string[] SelectedCUSTYPEvalues = StrSelectedCUSTYPE.Split(',');
                for (int i = 0; i < SelectedCUSTYPEvalues.Length; i++)
                {
                    str += "<dp";
                    str += " GenCode ='" + SelectedCUSTYPEvalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";

                DataSet ds = bLayer.GetCustomerMetNotMetRpt(PKID, FDATE, TODATE, DOCTYPE, BRIDGEKOL, str);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                var query = ds.Tables[1].AsEnumerable().Select(dataRow => new ReportSubmissionChemist
                {
                    CustomerName = dataRow[2].ToString(),
                    AreaName = dataRow[3].ToString(),
                    PlannedVisits = dataRow[4].ToString(),
                    VisitCount = dataRow[5].ToString(),
                    BalanceVisits = dataRow[6].ToString(),
                    RBPlannedVisits = dataRow[7].ToString(),
                    RBVisitCount = dataRow[8].ToString(),
                    RBBalanceVisits = dataRow[9].ToString()
                }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "CustomerName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.CustomerName != null && sq.CustomerName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.CustomerName != null && sq.CustomerName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.CustomerName != null && sq.CustomerName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.CustomerName != null && sq.CustomerName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "CustomerName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CustomerName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "CustomerName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.CustomerName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    else if (gridQueryModel.sidx == "PlannedVisits" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.PlannedVisits descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "PlannedVisits" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.PlannedVisits ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    else if (gridQueryModel.sidx == "VisitCount" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.VisitCount descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "VisitCount" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.VisitCount ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "BalanceVisits" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.BalanceVisits descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "BalanceVisits" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.BalanceVisits ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    else if (gridQueryModel.sidx == "RBPlannedVisits" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RBPlannedVisits descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "RBPlannedVisits" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.RBPlannedVisits ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    else if (gridQueryModel.sidx == "RBVisitCount" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RBVisitCount descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "RBVisitCount" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.RBVisitCount ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "RBBalanceVisits" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RBBalanceVisits descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "RBBalanceVisits" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.RBBalanceVisits ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                }

                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,

                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        #endregion

        #region Load PDF, Excel, CSV Files For Chemist

        public ActionResult ExportCustomerMetNotMetChemistToExcel(string PKID, string FDATE, string TODATE, string DOCTYPE, string CUSTYPE, string BRIDGEKOL)
        {
            string str = "";

            try
            {
                str = "<row>";
                string StrSelectedCUSTYPE = CUSTYPE;
                string[] SelectedCUSTYPEvalues = StrSelectedCUSTYPE.Split(',');
                for (int i = 0; i < SelectedCUSTYPEvalues.Length; i++)
                {
                    str += "<dp";
                    str += " GenCode ='" + SelectedCUSTYPEvalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";

                DataSet ds = bLayer.GetCustomerMetNotMetRpt(PKID, FDATE, TODATE, DOCTYPE, BRIDGEKOL, str);
                var context = ds.Tables[1].AsEnumerable().Select(dataRow => new ReportSubmissionChemist
                {
                    CustomerName = dataRow[2].ToString(),
                    AreaName = dataRow[3].ToString(),
                    PlannedVisits = dataRow[5].ToString(),
                    VisitCount = dataRow[4].ToString(),
                    BalanceVisits = dataRow[6].ToString(),
                    RBVisitCount = dataRow[7].ToString(),
                    RBPlannedVisits = dataRow[8].ToString(),
                    RBBalanceVisits = dataRow[9].ToString()
                }).ToList();

                //var context = (from q in dcReports.USP_GetDoctorMetbyXMLN(Convert.ToDecimal(PKID), FDATE, TODATE, DOCTYPE, BRIDGEKOL, CUSTYPE) select q).OrderBy(x => x.CustomerName).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.CustomerName== null ? "" :item.CustomerName,
                 item.AreaName == null ? "" :item.AreaName,
                 item.PlannedVisits == null ? "" :item.PlannedVisits,
                 item.VisitCount == null ? "" :item.VisitCount,
                 item.BalanceVisits == null ? "" :item.BalanceVisits,
                 item.RBVisitCount == null ? "" :item.RBVisitCount,
                 item.RBPlannedVisits == null ? "" :item.RBPlannedVisits,
                 item.RBBalanceVisits == null ? "" :item.RBBalanceVisits,
             
                
               
            }));
                return new ExcelResult(HeadersCustometMetNotMetChemist, ColunmTypesCustometMetNotMetChemist, data, "CustometMetNotMetChemist.xlsx", "CustometMetNotMetChemist");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersCustometMetNotMetChemist = {
               "DoctorName","Brick", "PlannedVisits", "Visitecount", "BalanceVisite", "RBVisitecount","RBPlannedVisits",  "RBBalanceVisite"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesCustometMetNotMetChemist = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportCustomerMetNotMetChemistToPDF(string PKID, string FDATE, string TODATE, string DOCTYPE, string CUSTYPE, string BRIDGEKOL)
        {
            string str = "";

            try
            {
                str = "<row>";
                string StrSelectedCUSTYPE = CUSTYPE;
                string[] SelectedCUSTYPEvalues = StrSelectedCUSTYPE.Split(',');
                for (int i = 0; i < SelectedCUSTYPEvalues.Length; i++)
                {
                    str += "<dp";
                    str += " GenCode ='" + SelectedCUSTYPEvalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";

                DataSet ds = bLayer.GetCustomerMetNotMetRpt(PKID, FDATE, TODATE, DOCTYPE, BRIDGEKOL, str);

                List<CustomerMetNotMetsChemist> userList = new List<CustomerMetNotMetsChemist>();

                //var context = (from q in dcReports.USP_GetDoctorMetbyXMLN(Convert.ToDecimal(PKID), FDATE, TODATE, DOCTYPE, BRIDGEKOL, CUSTYPE) select q).OrderBy(x => x.CustomerName).ToList();

                var context = ds.Tables[1].AsEnumerable().Select(dataRow => new ReportSubmissionChemist
                {
                    CustomerName = dataRow[2].ToString(),
                    AreaName = dataRow[3].ToString(),
                    PlannedVisits = dataRow[5].ToString(),
                    VisitCount = dataRow[4].ToString(),
                    BalanceVisits = dataRow[6].ToString(),
                    RBVisitCount = dataRow[7].ToString(),
                    RBPlannedVisits = dataRow[8].ToString(),
                    RBBalanceVisits = dataRow[9].ToString()
                }).ToList();

                foreach (var item in context)
                {
                    CustomerMetNotMetsChemist user = new CustomerMetNotMetsChemist();
                    // user.ProductFkid = item.ProductFKID.ToString();
                    user.DoctorName = item.CustomerName == null ? "" : item.CustomerName;
                    user.Brick = item.AreaName == null ? "" : item.AreaName;
                    user.PlannedVisits = item.PlannedVisits == null ? "" : item.PlannedVisits;
                    user.Visitecount = item.VisitCount == null ? "" : item.VisitCount;
                    user.BalanceVisite = item.BalanceVisits == null ? "" : item.BalanceVisits;
                    user.RBVisitecount = item.RBVisitCount == null ? "" : item.RBVisitCount;
                    user.RBPlannedVisits = item.RBPlannedVisits == null ? "" : item.RBPlannedVisits;
                    user.RBBalanceVisite = item.RBBalanceVisits == null ? "" : item.RBBalanceVisits;

                    userList.Add(user);
                }
                var customerList = userList;


                pdf.ExportPDF(customerList, new string[] { "DoctorName", "Brick", "PlannedVisits", "Visitecount", "BalanceVisite", "RBVisitecount", "RBPlannedVisits", "RBBalanceVisite" }, path);
                return File(path, "application/pdf", "CustomerMetNotMetChemist.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportCustomerMetNotMetChemistToCsv(string PKID, string FDATE, string TODATE, string DOCTYPE, string CUSTYPE, string BRIDGEKOL)
        {
            string str = "";

            try
            {
                str = "<row>";
                string StrSelectedCUSTYPE = CUSTYPE;
                string[] SelectedCUSTYPEvalues = StrSelectedCUSTYPE.Split(',');
                for (int i = 0; i < SelectedCUSTYPEvalues.Length; i++)
                {
                    str += "<dp";
                    str += " GenCode ='" + SelectedCUSTYPEvalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";

                DataSet ds = bLayer.GetCustomerMetNotMetRpt(PKID, FDATE, TODATE, DOCTYPE, BRIDGEKOL, str);
                // var model = (from q in dcReports.USP_GetDoctorMetbyXMLN(Convert.ToDecimal(PKID), FDATE, TODATE, DOCTYPE, BRIDGEKOL, CUSTYPE) select q).OrderBy(x => x.CustomerName).ToList();
                var model = ds.Tables[1].AsEnumerable().Select(dataRow => new ReportSubmissionChemist
                {
                    CustomerName = dataRow[2].ToString(),
                    AreaName = dataRow[3].ToString(),
                    PlannedVisits = dataRow[5].ToString(),
                    VisitCount = dataRow[4].ToString(),
                    BalanceVisits = dataRow[6].ToString(),
                    RBVisitCount = dataRow[7].ToString(),
                    RBPlannedVisits = dataRow[8].ToString(),
                    RBBalanceVisits = dataRow[9].ToString()
                }).ToList();

                var sb = new StringBuilder();
                sb.AppendLine("Doctor Name,Brick,PlannedVisits,Visitecount,BalanceVisite,RBVisitecount,RBPlannedVisits,RBBalanceVisite");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", record == null ? "" : record.CustomerName, record == null ? "" : record.AreaName, record == null ? "" : record.PlannedVisits, record == null ? "" : record.VisitCount, record == null ? "" : record.BalanceVisits, record == null ? "" : record.RBVisitCount, record == null ? "" : record.RBPlannedVisits, record == null ? "" : record.RBBalanceVisits
                    );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "CustomerMetNotMetChemist.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class CustomerMetNotMetsChemist
        {
            public string DoctorName { get; set; }
            public string Brick { get; set; }
            public string PlannedVisits { get; set; }
            public string Visitecount { get; set; }
            public string BalanceVisite { get; set; }
            public string RBPlannedVisits { get; set; }
            public string RBVisitecount { get; set; }
            public string RBBalanceVisite { get; set; }

        }

        #endregion

        #endregion


        #endregion


        #region CFSAPSO Report

        public ActionResult CFSAPSOReport()
        {
            return View();
        }

        #region Bind Data To Grid

        public JsonResult GetCFSAPSOReport(GridQueryModel gridQueryModel, string PKID, string FDATE, string TODATE)
        {
            try
            {
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string TODATES = tdate.ToString("MM/dd/yyyy");

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in dcReports.USP_sp_CFSAReportforPSO(FDATES, TODATES, PKID) select q).ToList();
                //var query = (from a in OtherMasters.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.PSOName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "DoctorName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.DoctorName != null && sq.DoctorName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.DoctorName != null && sq.DoctorName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.DoctorName != null && sq.DoctorName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.DoctorName != null && sq.DoctorName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "PSOName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.PSOName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "PSOName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.PSOName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "Date" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Date descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Date" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.Date ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DoctorName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.DoctorName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "DrResponse" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DrResponse descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "DrResponse" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.DrResponse ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "PSOResponse" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.PSOResponse descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "PSOResponse" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.PSOResponse ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "Strategic" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Strategic descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Strategic" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.Strategic ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "Tailored" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Tailored descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Tailored" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.Tailored ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "Evaluation" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Evaluation descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Evaluation" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.Evaluation ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                }

                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,

                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        #endregion




        #region Load PSO Name For List Box

        public JsonResult JsonLoadPsoNamesForCFSA()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            decimal PKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            var item = (from a in dcReports.GetPsoNameByCFSAReport(PKID) select new { a.PKID, a.EmpName }).ToList();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        #endregion


        #region Load PDF, Excel, CSV Files

        public ActionResult ExportCFSAPSOReportToExcel(string PKID, string FDATE, string TODATE)
        {
            try
            {
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string TODATES = tdate.ToString("MM/dd/yyyy");

                var context = (from q in dcReports.USP_sp_CFSAReportforPSO(FDATES, TODATES, PKID) select q).OrderBy(x => x.PSOName).ToList();
                //if (context.Count == 0)
                //    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.PSOName == null ? "" :item.PSOName,
                 item.Date == null ? "" :item.Date,
                 item.DoctorName == null ? "" :item.DoctorName,
                 item.DrResponse == null ? "" :item.DrResponse,
                 item.PSOResponse == null ? "" :item.PSOResponse,
                 item.Strategic == null ? "" :item.Strategic,
                 item.Tailored == null ? "" :item.Tailored,
                 item.Evaluation == null ? "" :item.Evaluation,
             
                
               
            }));
                return new ExcelResult(HeadersCFSAPSOReport, ColunmTypesCFSAPSOReport, data, "CFSAPSOReport.xlsx", "CFSAPSOReport");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersCFSAPSOReport = {
               "PSO Name","Date","DoctorName","Dr Response","PSO Response","Strategic","Tailored","Evaluation"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesCFSAPSOReport = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportCFSAPSOReportToPDF(string PKID, string FDATE, string TODATE)
        {
            try
            {
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string TODATES = tdate.ToString("MM/dd/yyyy");

                List<CFSAPSOReports> userList = new List<CFSAPSOReports>();

                var context = (from q in dcReports.USP_sp_CFSAReportforPSO(FDATES, TODATES, PKID) select q).OrderBy(x => x.PSOName).ToList();

                foreach (var item in context)
                {
                    CFSAPSOReports user = new CFSAPSOReports();
                    // user.ProductFkid = item.ProductFKID.ToString();
                    user.PSONames = item.PSOName == null ? "" : item.PSOName;
                    user.Date = item.Date == null ? "" : item.Date;
                    user.DoctorName = item.DoctorName == null ? "" : item.DoctorName;
                    user.DrResponse = item.DrResponse == null ? "" : item.DrResponse;
                    user.PSOResponse = item.PSOResponse == null ? "" : item.PSOResponse;
                    user.Strategic = item.Strategic == null ? "" : item.Strategic;
                    user.Tailored = item.Tailored == null ? "" : item.Tailored;
                    user.Evaluation = item.Evaluation == null ? "" : item.Evaluation;
                    userList.Add(user);
                }
                var customerList = userList;


                pdf.ExportPDF(customerList, new string[] { "PSONames", "Date", "DoctorName", "DrResponse", "PSOResponse", "Strategic", "Tailored", "Evaluation" }, path);
                return File(path, "application/pdf", "CFSAPSOReport.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportCFSAPSOReportToCsv(string PKID, string FDATE, string TODATE)
        {
            try
            {
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                string TODATES = tdate.ToString("MM/dd/yyyy");

                var model = (from q in dcReports.USP_sp_CFSAReportforPSO(FDATES, TODATES, PKID) select q).OrderBy(x => x.PSOName).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("PSONames, Date, DoctorName, Dr Response, PSO Response, Strategic, Tailored, Evaluation");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}", record == null ? "" : record.PSOName, record == null ? "" : record.Date, record == null ? "" : record.DoctorName, record == null ? "" : record.DrResponse, record == null ? "" : record.PSOResponse, record == null ? "" : record.Strategic, record == null ? "" : record.Tailored, record == null ? "" : record.Evaluation

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "CFSAPSOReport.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class CFSAPSOReports
        {
            public string PSONames { get; set; }
            public string Date { get; set; }
            public string DoctorName { get; set; }
            public string DrResponse { get; set; }
            public string PSOResponse { get; set; }
            public string Strategic { get; set; }
            public string Tailored { get; set; }
            public string Evaluation { get; set; }

        }

        #endregion


        #endregion

        #endregion

        #region Baskar
        #region LeaveReport

        public ActionResult LeaveReport()
        {
            return View();
        }

        public JsonResult JsonLoadNodeType()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetNodeTypeByLeaveReport() select new { a.PKID, a.NodeName }).ToList().OrderBy(O => O.PKID).Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.NodeName,
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult JsonLoadLeaveType()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetLeaveTypeByLeaveReport() select new { a.PKID, a.GenName }).ToList().OrderBy(O => O.PKID).Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.GenName,
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonLoadSelectedName(string SelectedNodeType)
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var Rstitem = (from a in dcReports.GetNameBySelectedNodeType(SelectedNodeType, 1) select new { a.PKID, a.EmployeeName }).ToList().OrderBy(o => o.PKID).Distinct();


            if (Rstitem.Count() > 0)
            {
                roleItems = Rstitem.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmployeeName,
                    Value = x.PKID.ToString()

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonLoadName(string nodeType)
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetNodeTypeByLeaveReport() select new { a.PKID, a.NodeName }).ToList();

            string PkId = "";

            foreach (var id in item)
            {
                PkId += id.PKID + ",";
            }

            var Rstitem = (from a in dcReports.GetNameBySelectedNodeType(PkId.Remove(PkId.Length - 1, 1), 1) select new { a.PKID, a.EmployeeName }).ToList().OrderBy(o => o.PKID).Distinct();


            if (Rstitem.Count() > 0)
            {
                roleItems = Rstitem.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmployeeName,
                    Value = x.PKID.ToString()

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult JsPostLeaveReport(string LeaveTypeFKID, string EmpNameFKID, string NodeTypeFKID, string Status, string FetchDate, string Fromdate, string Todate, string Year)
        {
            string alertMsg = "";
            var result = "";
            var rslt = "";

            try
            {
                Session.Remove("tblExcel");
                //LeaveTypeFKID = "0,72,75,138,74,73";
                //EmpNameFKID = "0,1208,947,2402,2387,2340,2263,1450,1976,1440,760,679,978,1845,305,824,386,2282,1869,579,2168,495,2464,667,373,1448,353,2178,998,366,979,2285,997,303,2391,327,388,483,149,1830,7483,339,385,1096,600,1078,257,2375,331,927,1082,378,1674,1527,1429,1515,337,2327,833,267,332,2347,408,1603,999,2145,229,1177,2434,278,2055,2218,1890,2051,2076,638,228,1925,1936,2162,2041,783,1446,1466,1316,1051,1428,1415,1544,1981,2065,1828,2366,315,2023,401,2110,1327,243,2062,2361,480,541,1470,1263,2420,2309,2169,348,1084,964,607,1858,1675,524,2182,2372,1249,415,1105,808,990,921,2164,2397,1944,2080,2356,2250,1909,1631,1807,2067,1200,262,725,1210,1483,2413,479,447,1857,460,2047,1418,1842,156,528,718,224,553,2436,2137,1322,2246,509,605,634,477,2310,1471,2209,1453,1034,1336,1351,2095,796,2151,301,1484,1003,1183,1837,1472,1739,708,2244,276,1245,594,463,1255,1348,891,2460,809,1313,464,1270,1256,2157,349,1877,453,649,887,1905,1383,2007,2271,1229,333,1016,1068,1217,580,2052,699,2357,1219,2134,858,901,1768,1274,903,247,834,223,2075,1609,1206,1734,1849,300,1197,2127,2415,443,275,7484,1863,2428,1023,1220,2458,2221,1246,328,2346,761,2177,2462,2141,1347,1587,1244,2054,1113,2313,2082,2348,2438,908,1377,2330,1546,768,321,797,2279,2156,728,648,1404,1468,221,2020,716,367,440,1072,2316,2077,2284,1380,1713,1333,1598,2053,623,1822,7497,7499,1870,1475,2414,1810,1883,1283,2371,2287,1686,187,1485,2455,1059,2461,2112,2332,503,567,885,2395,989,1873,1549,810,1275,948,1835,418,1276,794,1260,2217,2429,918,400,1301,733,1918,859,2223,1368,2335,1405,1474,2171,1400,2117,343,640,811,1798,1586,2260,1356,1657,450,825,529,1212,2089,350,2154,2126,635,597,281,2231,1856,1029,2268,2451,1885,2378,1473,2035,376,1342,936,1422,812,2453,724,683,168,1386,334,1997,1181,1974,1923,1811,1640,1280,525,692,1408,2121,2237,2229,389,866,604,1061,410,2010,1218,878,448,2235,455,466,1285,1287,380,1286,546,1272,1296,304,1278,1254,240,2400,2379,1915,2230,530,813,2394,1099,2014,643,826,7503,254,1735,1032,1120,1934,2149,2275,2153,1542,357,515,1785,2390,2064,1632,2234,2326,2333,538,572,342,1841,1262,2120,397,1259,1462,220,1311,1834,1685,1833,471,1007,1012,2142,752,249,2380,2363,1251,542,1990,1645,218,2365,2056,439,1476,1986,1704,319,755,982,1900,603,2457,843,446,1983,414,2396,1035,500,313,965,302,1596,311,2274,2452,625,633,2270,1433,617,330,873,854,1359,2454,1895,1897,253,1250,2311,2376,1477,1392,786,2113,381,2068,1772,2341,1412,1398,1538,1975,494,1783,1320,2331,1230,2236,1385,911,2322,754,1028,1852,517,2219,1840,2317,215,1463,1456,1445,1331,544,1616,2123,2008,1292,307,1294,1273,1289,896,2226,2281,2063,232,923,1797,283,2405,1000,1364,814,578,2210,564,462,2433,653,1355,912,2269,2401,1732,345,184,2251,1352,996,2416,1627,1896,2255,1743,905,698,290,1754,2336,2344,581,1898,312,1247,2083,2165,1911,2345,1601,2339,2320,1534,1892,158,1166,2245,1523,1399,2081,419,191,2057,431,484,1258,364,1133,390,852,696,1252,2386,941,1874,950,2362,2431,2459,1599,1551,1431,2155,352,369,1821,2399,336,1459,2016,894,289,1479,1888,157,2001,1690,2466,1241,1585,1996,763,2359,770,1228,551,1753,895,499,1444,251,279,329,1664,2211,374,2276,1943,1930,1396,1388,1441,1548,2446,2213,1988,2465,1939,1369,1436,1291,2013,1853,2243,2216,2321,2277,2388,2422,189,2448,430,2258,601,2012,2172,2028,628,1696,1567,1223,1235,2033,1995,1238,1727,2403,2280,295,2050,2042,874,1375,1478,398,438,940,485,1269,575,1268,782,197,405,1299,624,815,1317,325,1876,1532,1901,1921,268,1636,2325,2241,2343,844,993,1706,1539,1344,504,1461,1932,2337,881,1271,1755,646,1132,1403,1796,2392,2261,1325,2175,2318,520,2060,442,1321,795,1353,1879,1021,476,758,318,316,1481,2161,456,173,784,1323,317,1191,1165,1362,1836,1851,2456,286,1081,1187,771,828,1098,944,2404,508,1257,486,198,2254,516,2329,2442,561,361,274,1135,772,956,435,879,1315,970,1447,2215,862,256,1843,2239,445,1482,2342,1382,280,595,615,722,1848,620,1906,953,1904,1207,2314,1332,1759,889,2005,540,913,680,1878,335,162,2427,1919,1872,1465,861,178,1506,2338,1267,365,1824,847,609,1306,1312,404,867,533,202,1070,2463,1560,429,273,1390,323,829,1850,309,1115,1397,1394,2334,2179,2383,2181,629,1800,1279,2381,1242,1910,1831,2441,2058,2224,1693,1209,2088,1782,288,934,2437,1069,505,1341,406,2135,167,775,1528,1401,802,2040,514,1555,2430,1552,1233,432,1008,2303,457,449,1328,252,974,1469,1700,1407,1226,1156,402,759,1452,277,650,1232,1363,1349,421,651,2389,1443,250,568,2385,1213,255,272,1167,577,1011,7494,2087,636,1427,1019,201,1409,1374,347,2143,2435,919,2133,1107,2312,2148,2367,482,774,1847,1324,2183,417,1416,2323,764,2170,757,1410,977,2286,709,566,1343,734,412,1899,576,1817,2180,618,2249,416,1372,1510,287,1346,2166,773,2228,647,2278,588,2071,2398,589,1761,2160,461,1426,238,1178,203,1604,1889,2032,2247,7493,712,1095,789,1518,1205,1731,548,394,2240,407,1801,196,208,676,1022,1236,2443,310,2426,2163,1625,848,194,1026,1231,2315,1578,1175,470,1424,1729,346,2450,1216,488,1395,1358,282,1093,2225,1335,433,1745,2432,1222,469,2319,2264,2393,1644,2267,2084,2024,1075,780,294,1516,1457,1169,1224,1747,1040,1360,1330,967,2139,573,598,297,537,865,706,637,1419,1683,1764,2445,1454,383,161,1340,1787,790,1656,403,876,1411,1467,1861,1815,779,1227,420,396,985,1762,838,2328,613,214,925,705,1038,840,261,2030,166,929,1185,729,606,2118,452,697,7492,7496,2072,1240,592,377,2003,614,916,849,1756,1295,1663,1293,1297,428,621,1451,340,1253,1818,689,2048,1497,2238,2070,382,444,522,2412,877,1788,7489,2262,557,822,1816,2302,2374,1371,1338,2444,1055,1414,1437,478,1413,776,239,200,230,2093,2440,1982,2384,652,1455,2147,627,497,2031,2419,1702,1697,285,322,492,501,1926,2272,391,715,1122,1511,2421,1421,983,723,1006,1376,1639,583,682,1757,2115,212,1384,932,526,1928,1799";
                //NodeTypeFKID = "0,9,14,13,10,16,15,12,17";
                //FetchDate = "1";
                //Fromdate = "2013-01-01";
                //Todate = "2013-12-31";
                //Status = "0";
                //Year = "2013";
                string Retrslt = "";
                XmlTransform trans = new XmlTransform();
                XmlTransform Pending = new XmlTransform();
                XmlTransform Rejected = new XmlTransform();
                Retrslt = bLayer.SearchByLeaveReport(LeaveTypeFKID, EmpNameFKID, NodeTypeFKID, Status, FetchDate, Fromdate, Todate, Year);

                if (Status == "0" || Status == "0,A,R,S")
                {
                    trans.XslFile = xslPath + "LeaveReportApproved.xsl";
                    trans.XmlString = Retrslt;

                    Pending.XslFile = xslPath + "LeaveReportPending.xsl";
                    Pending.XmlString = Retrslt;

                    Rejected.XslFile = xslPath + "LeaveReportRejected.xsl";
                    Rejected.XmlString = Retrslt;
                    rslt = trans.Transform() + "<br>" + Rejected.Transform().Remove(0, 39) + "<br>" + Pending.Transform().Remove(0, 39);
                }
                else
                {
                    var sStatussplit = Status.Split(',');
                    for (var j = 0; j < sStatussplit.Count(); j++)
                    {
                        if (sStatussplit[j] == "A")
                        {
                            trans.XslFile = xslPath + "LeaveReportApproved.xsl";
                            trans.XmlString = Retrslt;
                        }
                        if (sStatussplit[j] == "R")
                        {
                            Rejected.XslFile = xslPath + "LeaveReportRejected.xsl";
                            Rejected.XmlString = Retrslt;
                        }

                        if (sStatussplit[j] == "S")
                        {
                            Pending.XslFile = xslPath + "LeaveReportPending.xsl";
                            Pending.XmlString = Retrslt;
                        }
                    }

                }

                if (Status == "A")
                {
                    rslt = trans.Transform();
                }
                else if (Status == "R")
                {
                    rslt = Rejected.Transform();
                }
                else if (Status == "S")
                {
                    rslt = Pending.Transform();
                }
                else if (Status == "A,R")
                {
                    rslt = trans.Transform() + "<br>" + Rejected.Transform();
                }
                else if (Status == "A,S")
                {
                    rslt = trans.Transform() + "<br>" + Pending.Transform();
                }
                else if (Status == "R,S")
                {
                    rslt = Rejected.Transform() + "<br>" + Pending.Transform();
                }
                else if (Status == "A,R,S")
                {
                    rslt = trans.Transform() + "<br>" + Rejected.Transform() + "<br>" + Pending.Transform();
                }

                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;

                rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";


                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);


            }

            catch (Exception ex)
            {
                alertMsg = ex.Message;

            }
            return Json(new
            {
                rslt = result,
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        //cycle Plan

        #region cycle Plan

        public ActionResult CyclePlan()
        {

            //decimal ss= Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            return View();
        }

        public JsonResult JsonLoadSelectedCycleName(string Year)
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var Rstitem = (from a in dcReports.SearchCycleByYear(Year) select new { a.PKID, a.CycleName }).ToList().OrderBy(o => o.PKID).Distinct();


            if (Rstitem.Count() > 0)
            {
                roleItems = Rstitem.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.CycleName,
                    Value = x.PKID.ToString()

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public class PostCyclePlan
        {
            public int year { get; set; }
            public int month { get; set; }
            public int frequency { get; set; }
            public int doctorFKID { get; set; }
        }

        public class PostProductName
        {
            public int doctorFKID { get; set; }
            public string ProductName { get; set; }
            public string GenName { get; set; }
            public string Drivername { get; set; }
            public string PLStage { get; set; }
            public string ClassName { get; set; }
            public string QualifiedDrCapacity { get; set; }
            public string QualifiedDrShare { get; set; }
            public string CycleObjective { get; set; }

        }



        public JsonResult JsPostCyclePlan(string PSOName, string Year, string Cycle)
        {
            XmlTransform trans = new XmlTransform();
            string Retrslt = "";
            try
            {
                Retrslt = bLayer.CyclePlanReportlatest1(Year, Cycle, PSOName);
                trans.XslFile = xslPath + "CyclePlanDetails.xsl";
                trans.XmlString = Retrslt;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);
                Session["tblExcel"] = rslt;
                rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";

                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                Retrslt = ex.Message;

            }
            return Json(new
            {
                Retrslt

            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //Report Submission
        #region Report Submission
        public ActionResult ReportSubmission()
        {

            try
            {

            }
            catch (Exception ex)
            {

            }
            return View();
        }


        public JsonResult JsonLoadRSNodeName()
        {
            decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetNodeByPSOFKID(USER_FKID) select new { a.NodeFKID, a.NodeName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.NodeName,
                    Value = x.NodeFKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult JsonLoadRSName(string NodeName)
        {

            decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.GetPSONameDRAtGforRG(USER_FKID, Convert.ToDecimal(NodeName));

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new ReportSubmission { UserFKID = dataRow.Field<string>("UserFKID"), EmpName = dataRow.Field<string>("EmpName") }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.UserFKID
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }


        public JsonResult JsPostRptSubmissionSubmit(string SelectNodeType, string Status, string Name, string Frmdate, string Todate)
        {
            string alertMsg = "";

            var Result = "";
            string Retrslt = "";
            XmlTransform trans = new XmlTransform();
            try
            {

                string SelName = Name.Remove(Name.Length - 1, 1);

                string[] arrayName = SelName.Split(',');
                StringBuilder sb = new StringBuilder();
                sb.Append("<row>");
                for (int i = 0; i < arrayName.Count(); i++)
                {
                    sb.Append("<dp PSOFKID='" + arrayName[i]);
                    sb.Append("'/>");

                }
                sb.Append("</row>");


                Retrslt = bLayer.GetReportSubmissions1(sb.ToString(), Frmdate, Todate, Status);
                trans.XslFile = xslPath + "PsoWiseReportSubmission.xsl";
                trans.XmlString = Retrslt;
                var rslt = trans.Transform();
                rslt = rslt.Remove(0, 39);

                Session["tblExcel"] = rslt;

                rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/>" + rslt + "<br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";


                return Json(new
                {
                    rslt

                }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)
            {
                alertMsg = ex.Message;

            }


            return Json(new
            {
                rslt = Result,

            }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        //Summery Report Pso
        #region Summery Report Pso
        public ActionResult SummeryReportPSO()
        {
            return View();
        }

        public JsonResult GridDataSummeryReportPSO(GridQueryModel gridQueryModel)
        {
            decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.SummeryReportPSO(USER_FKID.ToString());

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
            {
                NodeName = dataRow[1].ToString(),
                Name = dataRow[0].ToString(),
                CityName = dataRow[2].ToString(),
                Specialization = dataRow[4].ToString(),
                APlus = dataRow[5].ToString(),
                A = dataRow[6].ToString(),
                B = dataRow[7].ToString(),
                C = dataRow[8].ToString(),
                total = dataRow[9].ToString(),

            }
                ).ToList();

            var query = (from a in item select new { a.NodeName, a.Name, a.CityName, a.Specialization, a.APlus, a.A, a.B, a.C, a.total }).ToList();
            var pageData = query.OrderBy(x => x.NodeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
            var count = query.Count();
            return Json(new
            {
                page = gridQueryModel.page,
                records = count,
                rows = pageData,
                total = Math.Ceiling((decimal)count / gridQueryModel.rows)
            }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult ExportSummeryReportPSOToExcel()
        {

            decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.SummeryReportPSO(USER_FKID.ToString());

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
            {
                NodeName = dataRow[1].ToString(),
                CityName = dataRow[2].ToString(),
                Name = dataRow[0].ToString(),
                Specialization = dataRow[4].ToString(),
                APlus = dataRow[5].ToString(),
                A = dataRow[6].ToString(),
                B = dataRow[7].ToString(),
                C = dataRow[8].ToString(),
                total = dataRow[9].ToString(),

            }
               ).ToList();

            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.NodeName== null ? "" : item.NodeName.ToString(),
                item.CityName== null ? "" : item.CityName.ToString(),
                item.Name== null ? "" : item.Name.ToString(),                
                item.Specialization== null ? "" : item.Specialization.ToString(),
                item.APlus== null ? "" : item.APlus.ToString(),
                item.A== null ? "" : item.A.ToString(),
                item.B== null ? "0" : item.B.ToString(),
                item.C== null ? "0"  : item.C.ToString(),
                item.total== null ? "" : item.total.ToString()
                
               
            }));
            return new ExcelResult(HeadersSummeryRptPSO, ColunmTypesSummeryRptPSO, data, "SummeryReportforPSO.xlsx", "SummeryReportforPSO");
        }

        private static readonly string[] HeadersSummeryRptPSO = {               
               "Designation","HQ", "Name", "SPLSegment", "APlus", "A", "B", "C", "Total"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesSummeryRptPSO = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };

        public ActionResult ExportSummeryReportPSOToPDF()
        {

            List<SummeryRptPSO> userList = new List<SummeryRptPSO>();
            // var context = (from a in dcReports.SampleAckRpt_Admin(fromDate, toDate, Convert.ToInt32(strPending), Convert.ToInt32(strAll)) select a).ToList();

            decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.SummeryReportPSO(USER_FKID.ToString());

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
            {
                NodeName = dataRow[1].ToString(),
                CityName = dataRow[2].ToString(),
                Name = dataRow[0].ToString(),
                Specialization = dataRow[4].ToString(),
                APlus = dataRow[5].ToString(),
                A = dataRow[6].ToString(),
                B = dataRow[7].ToString(),
                C = dataRow[8].ToString(),
                total = dataRow[9].ToString(),

            }
               ).ToList();

            foreach (var item in context)
            {
                SummeryRptPSO user = new SummeryRptPSO();
                user.Designation = item.NodeName == null ? "" : item.NodeName;
                user.HQ = item.CityName == null ? "" : item.CityName;
                user.Name = item.Name == null ? "" : item.Name;
                user.SPLSegment = item.Specialization == null ? "" : item.Specialization;
                user.APlus = item.APlus == null ? "" : item.APlus;
                user.A = item.A == null ? "" : item.A;
                user.B = item.B == null ? "" : item.B;
                user.C = item.C == null ? "" : item.C;
                user.Total = item.total == null ? "" : item.total;

                userList.Add(user);
            }
            var customerList = userList;



            pdf.ExportPDF(customerList, new string[] { "Designation", "HQ", "Name", "SPLSegment", "APlus", "A", "B", "C", "Total" }, path);
            return File(path, "application/pdf", "SummeryReportforPSO.pdf");
        }


        public class SummeryRptPSO
        {
            public string Designation { get; set; }
            public string HQ { get; set; }
            public string Name { get; set; }
            public string SPLSegment { get; set; }
            public string APlus { get; set; }
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string Total { get; set; }
        }

        #endregion


        #region Summery Report DM

        public ActionResult SummeryReportDM()
        {
            return View();
        }

        public JsonResult JsonLoadSREmpName()
        {
            long? USER_FKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.SummeryReportReportingUsers(USER_FKID) select new { a.pkid, a.EmpName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.pkid.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GridDataSummeryReportDM(GridQueryModel gridQueryModel, string Userid)
        {
            string alertMsg = "";
            try
            {
                DataSet ds = new DataSet();
                ds = bLayer.SummeryReportReportingUsersData(Userid.ToString());

                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
                {
                    NodeName = dataRow[1].ToString(),
                    CityName = dataRow[2].ToString(),
                    Name = dataRow[0].ToString(),
                    Specialization = dataRow[4].ToString(),
                    APlus = dataRow[5].ToString(),
                    A = dataRow[6].ToString(),
                    B = dataRow[7].ToString(),
                    C = dataRow[8].ToString(),
                    total = dataRow[9].ToString(),

                }
                    ).ToList();

                var query = (from a in item select new { a.NodeName, a.CityName, a.Name, a.Specialization, a.APlus, a.A, a.B, a.C, a.total }).ToList();
                var pageData = query.OrderBy(x => x.NodeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                var count = query.Count();
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                alertMsg = ex.Message;
            }
            return Json(new
            {
                rslt = alertMsg,
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExportSummeryReportDMToExcel(string Userid)
        {

            //decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.SummeryReportReportingUsersData(Userid.ToString());

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
            {
                NodeName = dataRow[1].ToString(),
                CityName = dataRow[2].ToString(),
                Name = dataRow[0].ToString(),
                Specialization = dataRow[4].ToString(),
                APlus = dataRow[5].ToString(),
                A = dataRow[6].ToString(),
                B = dataRow[7].ToString(),
                C = dataRow[8].ToString(),
                total = dataRow[9].ToString(),

            }
               ).ToList();

            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.NodeName== null ? "" : item.NodeName.ToString(),
                item.CityName== null ? "" : item.CityName.ToString(),
                item.Name== null ? "" : item.Name.ToString(),               
                item.Specialization== null ? "" : item.Specialization.ToString(),
                item.APlus== null ? "" : item.APlus.ToString(),
                item.A== null ? "" : item.A.ToString(),
                item.B== null ? "0" : item.B.ToString(),
                item.C== null ? "0"  : item.C.ToString(),
                item.total== null ? "" : item.total.ToString()
                
               
            }));
            return new ExcelResult(HeadersSummeryRptDM, ColunmTypesSummeryRptDM, data, "SummeryReportforDM.xlsx", "SummeryReportforDM");
        }

        private static readonly string[] HeadersSummeryRptDM = {               
               "Designation","HQ", "Name", "SPLSegment", "APlus", "A", "B", "C", "Total"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesSummeryRptDM = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };

        public ActionResult ExportSummeryReportDMToPDF(string Userid)
        {

            List<SummeryRptDM> userList = new List<SummeryRptDM>();

            // var context = (from a in dcReports.SampleAckRpt_Admin(fromDate, toDate, Convert.ToInt32(strPending), Convert.ToInt32(strAll)) select a).ToList();
            //decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            DataSet ds = new DataSet();
            ds = bLayer.SummeryReportReportingUsersData(Userid.ToString());

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
            {
                NodeName = dataRow[1].ToString(),
                CityName = dataRow[2].ToString(),
                Name = dataRow[0].ToString(),
                Specialization = dataRow[4].ToString(),
                APlus = dataRow[5].ToString(),
                A = dataRow[6].ToString(),
                B = dataRow[7].ToString(),
                C = dataRow[8].ToString(),
                total = dataRow[9].ToString(),

            }
               ).ToList();

            foreach (var item in context)
            {
                SummeryRptDM user = new SummeryRptDM();
                user.Designation = item.NodeName == null ? "" : item.NodeName;
                user.HQ = item.CityName == null ? "" : item.CityName;
                user.Name = item.Name == null ? "" : item.Name;
                user.SPLSegment = item.Specialization == null ? "" : item.Specialization;
                user.APlus = item.APlus == null ? "" : item.APlus;
                user.A = item.A == null ? "" : item.A;
                user.B = item.B == null ? "" : item.B;
                user.C = item.C == null ? "" : item.C;
                user.Total = item.total == null ? "" : item.total;

                userList.Add(user);
            }
            var customerList = userList;



            pdf.ExportPDF(customerList, new string[] { "Designation", "HQ", "Name", "SPLSegment", "APlus", "A", "B", "C", "Total" }, path);
            return File(path, "application/pdf", "SummeryReportforDM.pdf");
        }


        public class SummeryRptDM
        {
            public string Designation { get; set; }
            public string HQ { get; set; }
            public string Name { get; set; }
            public string SPLSegment { get; set; }
            public string APlus { get; set; }
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string Total { get; set; }
        }



        #endregion

        #region Summery Report RBM

        public ActionResult SummeryReportRBM()
        {
            return View();
        }

        public JsonResult JsonLoadRBMEmpName()
        {
            long? USER_FKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.SummeryReportReportingUsers(USER_FKID) select new { a.pkid, a.EmpName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.pkid.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GridDataSummeryReportRBM(GridQueryModel gridQueryModel, string Userid)
        {
            string alertMsg = "";
            try
            {
                DataSet ds = new DataSet();
                ds = bLayer.SummeryReportReportingUsersData(Userid.ToString());

                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
                {
                    NodeName = dataRow[1].ToString(),
                    CityName = dataRow[2].ToString(),
                    Name = dataRow[0].ToString(),
                    Specialization = dataRow[4].ToString(),
                    APlus = dataRow[5].ToString(),
                    A = dataRow[6].ToString(),
                    B = dataRow[7].ToString(),
                    C = dataRow[8].ToString(),
                    total = dataRow[9].ToString(),

                }
                    ).ToList();

                var query = (from a in item select new { a.NodeName, a.CityName, a.Name, a.Specialization, a.APlus, a.A, a.B, a.C, a.total }).ToList();
                var pageData = query.OrderBy(x => x.NodeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                var count = query.Count();
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                alertMsg = ex.Message;
            }
            return Json(new
            {
                rslt = alertMsg,
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ExportSummeryReportRBMToExcel(string Userid)
        {

            //decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.SummeryReportReportingUsersData(Userid.ToString());

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
            {
                NodeName = dataRow[1].ToString(),
                CityName = dataRow[2].ToString(),
                Name = dataRow[0].ToString(),
                Specialization = dataRow[4].ToString(),
                APlus = dataRow[5].ToString(),
                A = dataRow[6].ToString(),
                B = dataRow[7].ToString(),
                C = dataRow[8].ToString(),
                total = dataRow[9].ToString(),

            }
               ).ToList();

            //if (context.Count == 0)
            //    return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.NodeName== null ? "" : item.NodeName.ToString(),
                item.CityName== null ? "" : item.CityName.ToString(),
                item.Name== null ? "" : item.Name.ToString(),               
                item.Specialization== null ? "" : item.Specialization.ToString(),
                item.APlus== null ? "" : item.APlus.ToString(),
                item.A== null ? "" : item.A.ToString(),
                item.B== null ? "0" : item.B.ToString(),
                item.C== null ? "0"  : item.C.ToString(),
                item.total== null ? "" : item.total.ToString()
                
               
            }));
            return new ExcelResult(HeadersSummeryRptRBM, ColunmTypesSummeryRptRBM, data, "SummeryReportforRBM.xlsx", "SummeryReportforRBM");
        }

        private static readonly string[] HeadersSummeryRptRBM = {               
               "Designation","HQ", "Name", "SPLSegment", "APlus", "A", "B", "C", "Total"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesSummeryRptRBM = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };

        public ActionResult ExportSummeryReportRBMToPDF(string Userid)
        {

            List<SummeryRptRBM> userList = new List<SummeryRptRBM>();

            // var context = (from a in dcReports.SampleAckRpt_Admin(fromDate, toDate, Convert.ToInt32(strPending), Convert.ToInt32(strAll)) select a).ToList();
            //decimal USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            DataSet ds = new DataSet();
            ds = bLayer.SummeryReportReportingUsersData(Userid.ToString());

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new SummeryReportPSO
            {
                NodeName = dataRow[1].ToString(),
                CityName = dataRow[2].ToString(),
                Name = dataRow[0].ToString(),
                Specialization = dataRow[4].ToString(),
                APlus = dataRow[5].ToString(),
                A = dataRow[6].ToString(),
                B = dataRow[7].ToString(),
                C = dataRow[8].ToString(),
                total = dataRow[9].ToString(),

            }
               ).ToList();

            foreach (var item in context)
            {
                SummeryRptRBM user = new SummeryRptRBM();
                user.Designation = item.NodeName == null ? "" : item.NodeName;
                user.HQ = item.CityName == null ? "" : item.CityName;
                user.Name = item.Name == null ? "" : item.Name;
                user.SPLSegment = item.Specialization == null ? "" : item.Specialization;
                user.APlus = item.APlus == null ? "" : item.APlus;
                user.A = item.A == null ? "" : item.A;
                user.B = item.B == null ? "" : item.B;
                user.C = item.C == null ? "" : item.C;
                user.Total = item.total == null ? "" : item.total;

                userList.Add(user);
            }
            var customerList = userList;



            pdf.ExportPDF(customerList, new string[] { "Designation", "HQ", "Name", "SPLSegment", "APlus", "A", "B", "C", "Total" }, path);
            return File(path, "application/pdf", "SummeryReportforRBM.pdf");
        }


        public class SummeryRptRBM
        {
            public string Designation { get; set; }
            public string HQ { get; set; }
            public string Name { get; set; }
            public string SPLSegment { get; set; }
            public string APlus { get; set; }
            public string A { get; set; }
            public string B { get; set; }
            public string C { get; set; }
            public string Total { get; set; }
        }



        #endregion


        //Sample Acknowledgement Report - for Admin

        #region Sample Acknowledgement Report - for Admin

        public ActionResult SampleAcknowledgementReportforAdmin()
        {
            return View();
        }

        public JsonResult JsPostSampleAcknowledgementReportforAdmin(GridQueryModel gridQueryModel, string strFrmdate, string strTodate, string strPending, string strAll)
        {
            string alertMsg = "";
            try
            {
                DateTime? fromDate = Convert.ToDateTime(strFrmdate);
                DateTime? toDate = Convert.ToDateTime(strTodate);
                var Rstitem = (from a in dcReports.SampleAckRpt_Admin(fromDate, toDate, Convert.ToInt32(strPending), Convert.ToInt32(strAll)) select a).ToList();
                var count = Rstitem.Count();
                // var pageData = rslt.OrderBy(x => x.userName).OrderBy(x => x.reportingTo).OrderBy(x=>x.NodeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                var pageData = Rstitem.OrderBy(x => x.productname).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                //alertMsg = "User Master Updated Successfully.";
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                alertMsg = ex.Message;
            }
            return Json(new
            {
                rslt = alertMsg,
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportSampleAckToExcel(string strFrmdate, string strTodate, string strPending, string strAll)
        {

            DateTime? fromDate = Convert.ToDateTime(strFrmdate);
            DateTime? toDate = Convert.ToDateTime(strTodate);

            var context = (from a in dcReports.SampleAckRpt_Admin(fromDate, toDate, Convert.ToInt32(strPending), Convert.ToInt32(strAll)) select new { a.SBDate, a.EmpName, a.TerName, a.productname, a.Productpack, a.AllocatedQty, a.AcknowledgeQty, a.ReasonForShortFall }).ToList();
            //if (context.Count == 0)
            //    return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.SBDate== null ? "" : item.SBDate.ToString(),
                item.EmpName== null ? "" : item.EmpName.ToString(),
                item.TerName== null ? "" : item.TerName.ToString(),
                item.productname== null ? "" : item.productname.ToString(),
                item.Productpack== null ? "" : item.Productpack.ToString(),
                item.AllocatedQty== null ? "0" : item.AllocatedQty.ToString(),
                item.AcknowledgeQty== null ? "0"  : item.AcknowledgeQty.ToString(),
                item.ReasonForShortFall== null ? "" : item.ReasonForShortFall.ToString()
                
               
            }));
            return new ExcelResult(HeadersSampleAck, ColunmTypesSampleAck, data, "SampleAckReportforAdmin.xlsx", "SampleAckReportforAdmin");
        }

        private static readonly string[] HeadersSampleAck = {               
               "Date","PSO Name","Territory","Product","Sample pack","Allocated Qty","Acknowledged Qty","Reason for Shortfall"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesSampleAck = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };

        public ActionResult ExportSampleAckToPDF(string strFrmdate, string strTodate, string strPending, string strAll)
        {
            DateTime? fromDate = Convert.ToDateTime(strFrmdate);
            DateTime? toDate = Convert.ToDateTime(strTodate);

            List<SampleAckAdmin> userList = new List<SampleAckAdmin>();
            var context = (from a in dcReports.SampleAckRpt_Admin(fromDate, toDate, Convert.ToInt32(strPending), Convert.ToInt32(strAll)) select a).ToList();

            foreach (var item in context)
            {
                SampleAckAdmin user = new SampleAckAdmin();
                user.Date = item.SBDate == null ? "" : item.SBDate;
                user.PSOName = item.EmpName == null ? "" : item.EmpName;
                user.Territory = item.TerName == null ? "" : item.TerName;
                user.Product = item.productname == null ? "" : item.productname;
                user.Samplepack = item.Productpack == null ? "" : item.Productpack;
                user.AllocatedQty = item.AllocatedQty;
                user.AcknowledgeQty = Convert.ToDecimal(item.AcknowledgeQty);
                user.ReasonForShortFall = item.ReasonForShortFall == null ? "" : item.ReasonForShortFall;

                userList.Add(user);
            }
            var customerList = userList;



            pdf.ExportPDF(customerList, new string[] { "Date", "PSOName", "Territory", "Product", "Samplepack", "AllocatedQty", "AcknowledgeQty", "ReasonForShortFall" }, path);
            return File(path, "application/pdf", "SampleAckReportforAdmin.pdf");
        }


        public class SampleAckAdmin
        {


            public string Date { get; set; }
            public string PSOName { get; set; }
            public string Territory { get; set; }
            public string Product { get; set; }
            public string Samplepack { get; set; }
            public decimal AllocatedQty { get; set; }
            public decimal AcknowledgeQty { get; set; }
            public string ReasonForShortFall { get; set; }

        }

        #endregion


        //Reminder Plan Vs Actual Report

        #region Reminder Plan Vs Actual Report

        public ActionResult ReminderReportPSO()
        {
            return View();
        }

        public JsonResult JsonLoadPSOProductName()
        {

            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.RR_ProductDetails(USER_FKID.ToString());

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new ReminderReportPSO { PKID = dataRow[0].ToString(), ProductName = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.ProductName,
                    Value = x.PKID

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult JsonLoadSpeciality()
        {
            decimal Territory_FKID = Convert.ToDecimal(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.RR_GetSpecialization(Territory_FKID.ToString());

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_GetSpecialization { PKID = dataRow[0].ToString(), Specialization = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.Specialization,
                    Value = x.PKID
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult JsonLoadMonth()
        {

            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.GetMonthDetails();

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_GetMonthDetails { MonthNo = dataRow[0].ToString(), MonthName = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.MonthName,
                    Value = x.MonthNo

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult JsonLoadSelectedCycle(string SelectedYear)
        {
            DataSet ds = new DataSet();
            ds = bLayer.SearchCycleByYear(SelectedYear);

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_GetCycleDetails { PKID = dataRow[0].ToString(), CycleName = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.CycleName,
                    Value = x.PKID

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        //Grid

        public JsonResult JsPostRRReportPSO(GridQueryModel gridQueryModel, string strUserFkid, string strProductName, string strYear, string strCycle, string strMonth, string strSpeciality)
        {
            string alertMsg = "";
            try
            {
                DataSet ds = new DataSet();

                //strUserFkid = "339";
                //strProductName = "37";
                //strYear = "2013";
                //strCycle = "24";
                //strMonth = "0";
                //strSpeciality = "11,12,45,43,13,14,15,37,16,17,35,19,20,21,44,22,34,23,24,25,41,26,28,18,29,27,39,30,40,31,32,46,49,48,38,33";



                ds = bLayer.RR_RemonderReport_PSO1(strUserFkid, strProductName, strYear, strCycle, strMonth, strSpeciality);

                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_GetGridLoad
                {
                    DrName = dataRow[0].ToString(),
                    Specialization = dataRow[1].ToString(),
                    P1 = dataRow[2].ToString(),
                    A1 = dataRow[3].ToString(),
                    P2 = dataRow[4].ToString(),
                    A2 = dataRow[5].ToString(),
                    P3 = dataRow[6].ToString(),
                    A3 = dataRow[7].ToString(),
                    P4 = dataRow[8].ToString(),
                    A4 = dataRow[9].ToString(),
                    R1 = dataRow[10].ToString(),
                    RA1 = dataRow[11].ToString(),
                    R2 = dataRow[12].ToString(),
                    RA2 = dataRow[13].ToString(),

                }
                    ).ToList();

                var query = (from a in item select new { a.DrName, a.Specialization, a.P1, a.A1, a.P2, a.A2, a.P3, a.A3, a.P4, a.A4, a.R1, a.RA1, a.R2, a.RA2 }).ToList();
                var pageData = query.OrderBy(x => x.DrName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                var count = query.Count();
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                alertMsg = ex.Message;
            }
            return Json(new
            {
                rslt = alertMsg,
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult JsonLoadHeader(GridQueryModel gridQueryModel, string strUserFkid, string strProductName, string strYear, string strCycle, string strMonth)
        {

            DataSet ds = new DataSet();
            ds = bLayer.RR_RemonderReport_Details(strUserFkid, strProductName, strYear, strCycle, strMonth);
            //RR_GetHeader
            string uName = "", pName = "", sDate = "", eDate = "";

            //var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_GetHeader
            //{
            //    UName = dataRow[0].ToString(),
            //    PName = dataRow[1].ToString(),
            //    SDate = dataRow[2].ToString(),
            //    EDate = dataRow[3].ToString(),

            //}).ToList();


            return Json(new
            {
                uName = ds.Tables[0].Rows[0]["UName"].ToString(),
                pName = ds.Tables[0].Rows[0]["PName"].ToString(),
                sDate = ds.Tables[0].Rows[0]["SDate"].ToString(),
                eDate = ds.Tables[0].Rows[0]["EDate"].ToString()

            }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult ExportSRRReportPSOToExcel(string strUserFkid, string strProductName, string strYear, string strCycle, string strMonth, string strSpeciality)
        {


            DataSet ds = new DataSet();

            //strUserFkid = "339";
            //strProductName = "37";
            //strYear = "2013";
            //strCycle = "24";
            //strMonth = "0";
            //strSpeciality = "11,12,45,43,13,14,15,37,16,17,35,19,20,21,44,22,34,23,24,25,41,26,28,18,29,27,39,30,40,31,32,46,49,48,38,33";



            ds = bLayer.RR_RemonderReport_PSO1(strUserFkid, strProductName, strYear, strCycle, strMonth, strSpeciality);

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_GetGridLoad
            {
                DrName = dataRow[0].ToString(),
                Specialization = dataRow[1].ToString(),
                P1 = dataRow[2].ToString(),
                A1 = dataRow[3].ToString(),
                P2 = dataRow[4].ToString(),
                A2 = dataRow[5].ToString(),
                P3 = dataRow[6].ToString(),
                A3 = dataRow[7].ToString(),
                P4 = dataRow[8].ToString(),
                A4 = dataRow[9].ToString(),
                R1 = dataRow[10].ToString(),
                RA1 = dataRow[11].ToString(),
                R2 = dataRow[12].ToString(),
                RA2 = dataRow[13].ToString()

            }
                   ).ToList();

            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.DrName== null ? "" : item.DrName.ToString(),
                item.Specialization== null ? "" : item.Specialization.ToString(),
                item.P1== null ? "" : item.P1.ToString(),                
                item.A1== null ? "" : item.A1.ToString(),
                item.P2== null ? "" : item.P2.ToString(),
                item.A2== null ? "" : item.A2.ToString(),
                item.P3== null ? "" : item.P3.ToString(),
                item.A3== null ? ""  : item.A3.ToString(),
                item.P4== null ? "" : item.P4.ToString(),
                item.A4== null ? ""  : item.A4.ToString(),                
                item.R1== null ? "" : item.R1.ToString(),
                item.RA1== null ? ""  : item.RA1.ToString(),
                item.R2== null ? "" : item.R2.ToString(),
                item.RA2== null ? ""  : item.RA2.ToString()
               
            }));

            return new ExcelResult(HeadersRRReportPSO, ColunmTypesRRReportPSO, data, "ReminderReportPSO.xlsx", "ReminderReportPSO");
        }

        private static readonly string[] HeadersRRReportPSO = {               
               "Doctor Name", "Speciality", "Plan", "Actual", "Plan", "Actual", "Plan", "Actual", "Plan", "Actual", "Plan", "Actual", "Plan", "Actual"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesRRReportPSO = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };

        public ActionResult ExportRRReportPSOToPDF(string strUserFkid, string strProductName, string strYear, string strCycle, string strMonth, string strSpeciality)
        {

            List<RRReportPSO> userList = new List<RRReportPSO>();

            //strUserFkid = "339";
            //strProductName = "37";
            //strYear = "2013";
            //strCycle = "24";
            //strMonth = "0";
            //strSpeciality = "11,12,45,43,13,14,15,37,16,17,35,19,20,21,44,22,34,23,24,25,41,26,28,18,29,27,39,30,40,31,32,46,49,48,38,33";

            DataSet ds = new DataSet();
            ds = bLayer.RR_RemonderReport_PSO1(strUserFkid, strProductName, strYear, strCycle, strMonth, strSpeciality);

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_GetGridLoad
            {
                DrName = dataRow[0].ToString(),
                Specialization = dataRow[1].ToString(),
                P1 = dataRow[2].ToString(),
                A1 = dataRow[3].ToString(),
                P2 = dataRow[4].ToString(),
                A2 = dataRow[5].ToString(),
                P3 = dataRow[6].ToString(),
                A3 = dataRow[7].ToString(),
                P4 = dataRow[8].ToString(),
                A4 = dataRow[9].ToString(),
                R1 = dataRow[10].ToString(),
                RA1 = dataRow[11].ToString(),
                R2 = dataRow[12].ToString(),
                RA2 = dataRow[13].ToString(),

            }
                   ).ToList();


            foreach (var item in context)
            {
                RRReportPSO user = new RRReportPSO();
                user.DoctorName = item.DrName == null ? "" : item.DrName;

                user.Speciality = item.Specialization == null ? "" : item.Specialization;
                user.D1Plan = item.P1 == null ? "" : item.P1;

                user.D1Actual = item.A1 == null ? "" : item.A1;
                user.D2Plan = item.P2 == null ? "" : item.P2;
                user.D2Actual = item.A2 == null ? "" : item.A2;
                user.D3Plan = item.P3 == null ? "" : item.P3;
                user.D3Actual = item.A3 == null ? "" : item.A3;
                user.D4Plan = item.P4 == null ? "" : item.P4;
                user.D4Actual = item.A4 == null ? "" : item.A4;
                user.R1Plan = item.R1 == null ? "" : item.R1;
                user.R1Actual = item.RA1 == null ? "" : item.RA1;
                user.R2Plan = item.P1 == null ? "" : item.R2;
                user.R2Actual = item.P1 == null ? "" : item.RA2;

                userList.Add(user);
            }
            var customerList = userList;



            pdf.ExportPDF(customerList, new string[] { "DoctorName", "Speciality", "D1Plan", "D1Actual", "D2Plan", "D2Actual", "D3Plan", "D3Actual", "D4Plan", "D4Actual", "R1Plan", "R1Actual", "R2Plan", "R2Actual" }, path);
            return File(path, "application/pdf", "ReminderReportPSO.pdf");

        }


        public class RRReportPSO
        {
            public string DoctorName { get; set; }
            public string Speciality { get; set; }
            public string D1Plan { get; set; }
            public string D1Actual { get; set; }
            public string D2Plan { get; set; }
            public string D2Actual { get; set; }
            public string D3Plan { get; set; }
            public string D3Actual { get; set; }
            public string D4Plan { get; set; }
            public string D4Actual { get; set; }
            public string R1Plan { get; set; }
            public string R1Actual { get; set; }
            public string R2Plan { get; set; }
            public string R2Actual { get; set; }
        }





        #endregion

        //CyclePlanCoverageRptDM

        #region CyclePlanCoverageRptDM

        public ActionResult CycleplanCoverageRptDM()
        {
            return View();
        }

        public JsonResult JsonLoadPSO()
        {
            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.GetPSONamebyRepID(USER_FKID.ToString());

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new ReminderReportPSO { PKID = dataRow[0].ToString(), ProductName = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.ProductName,
                    Value = x.PKID

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult JsonPostCPRptDM(string PSOId, string Year, string Month)
        {
            Session.Remove("tblExcel");
            string pso = "";
            string Retrslt = "";
            decimal Uid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            XmlTransform trans = new XmlTransform();

            string HeaddingMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(Month));

            pso = ProductsXmlDM1(PSOId);

            if (pso == "<row></row>")
            {
                Retrslt = bLayer.CyclePlanCoverageRpt_DM1(pso, Uid.ToString(), Month, Year);
                //DataSet ds = bLayer.CyclePlanCoverageRpt_DM(pso, Uid.ToString(), Month, Year);

                trans.XslFile = xslPath + "CycleplanCoverageRpt_PSO.xsl";
                trans.XmlString = Retrslt;
            }
            else
            {
                Retrslt = bLayer.CyclePlanCoverageRpt_DM1(pso, Uid.ToString(), Month, Year);
                //DataSet ds = bLayer.CyclePlanCoverageRpt_DM(pso, Uid.ToString(), Month, Year);

                trans.XslFile = xslPath + "CycleplanCoverageRptDM_PSO.xsl";
                trans.XmlString = Retrslt;
            }

            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);

            Session["tblExcel"] = rslt;

            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table><tr><td align='right' class='tb-r1' style='color:#2F72C3;font-weight:bold;'>  Cycle Plan Coverage Report of " + Session["Emp_Name"].ToString() + " (DM) for " + HeaddingMonth + " " + Year + "</td></tr></table>" + rslt + "<br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";

            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);

        }

        public string ProductsXmlDM1(string lstPSOName)
        {

            //string SelName = lstPSOName.Remove(lstPSOName.Length - 1, 1);
            StringBuilder sb = new StringBuilder();

            string SelName = lstPSOName;

            if (SelName == "null" || SelName == "0")
            {
                sb.Append("<row></row>");

            }
            else
            {
                string[] arrayName = SelName.Split(',');

                sb.Append("<row>");
                for (int i = 0; i < arrayName.Count(); i++)
                {

                    sb.Append("<dp PSOFKID='" + arrayName[i]);
                    sb.Append("'/>");


                }
                sb.Append("</row>");

            }
            return sb.ToString();

        }
        #endregion

        //CyclePlanCoverageRptRBM
        #region CyclePlanCoverageRptRBM
        public ActionResult CycleplanCoverageRptRBM()
        {
            return View();
        }

        public JsonResult JsonLoadDM()
        {
            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.GetDMNamebyRepID(USER_FKID.ToString());

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new ReminderReportPSO { PKID = dataRow[0].ToString(), ProductName = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.ProductName,
                    Value = x.PKID

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult JsonPostCPRptRBM(string DMId, string Year, string Month)
        {
            Session.Remove("tblExcel");
            string DM = "";
            string Retrslt = "";
            decimal Uid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            XmlTransform trans = new XmlTransform();
            string HeaddingMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(Month));
            DM = ProductsXmlDM1(DMId);

            if (DM == "<row></row>")
            {
                Retrslt = bLayer.CyclePlanCoverageRpt_RBM(DM, Uid.ToString(), Month, Year);
                //DataSet ds = bLayer.CyclePlanCoverageRpt_DM(pso, Uid.ToString(), Month, Year);

                trans.XslFile = xslPath + "CycleplanCoverageRpt_PSO.xsl";
                trans.XmlString = Retrslt;
            }
            else
            {
                Retrslt = bLayer.CyclePlanCoverageRpt_RBM(DM, Uid.ToString(), Month, Year);
                //DataSet ds = bLayer.CyclePlanCoverageRpt_DM(pso, Uid.ToString(), Month, Year);

                trans.XslFile = xslPath + "CycleplanCoverageRptRBM_PSO.xsl";
                trans.XmlString = Retrslt;
            }




            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);

            Session["tblExcel"] = rslt;

            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table><tr><td align='right' class='tb-r1' style='color:#2F72C3;font-weight:bold;'>  Cycle Plan Coverage Report of " + Session["Emp_Name"].ToString() + " (RBM) for " + HeaddingMonth + " " + Year + "</td></tr></table>" + rslt + "<br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";

            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult JsonCCRPSO(string SampleFKID, string Month, string Year, string EmpName)
        {
            Session.Remove("tblExcel");
            string HeaddingMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(Month));
            string Retrslt = "";

            DataSet ds = new DataSet();
            Retrslt = bLayer.CyclePlanCoverageRBM_DMRpt(Month, Year, SampleFKID);
            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "CycleplanCoverageRptDM_PSO.xsl";
            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            Session["tblExcel"] = rslt;
            //rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal1'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table><tr><td align='right' class='tb-r1' style='color:#2F72C3;font-weight:bold;'>  Cycle Plan Coverage Report of " + EmpName.ToString() + " (DM) for " + HeaddingMonth + " " + Year + "</td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";


            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }

        #endregion

        #region CyclePlanCoverageRptNation

        public ActionResult CyclePlanCoverageRptNation()
        {
            return View();
        }

        public JsonResult JsonLoadRBM()
        {
            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            // decimal? USER_FKID = 149;
            DataSet ds = new DataSet();
            ds = bLayer.GetRBMNamebyRepID(USER_FKID.ToString());

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new ReminderReportPSO { PKID = dataRow[0].ToString(), ProductName = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.ProductName,
                    Value = x.PKID

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult JsonLoadTeam()
        {
            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            DataSet ds = new DataSet();
            ds = bLayer.GetTeamsforUserByXML(USER_FKID.ToString());

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new ReminderReportPSO { PKID = dataRow[0].ToString(), ProductName = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.ProductName,
                    Value = x.PKID

                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult JsonPostCPRptNation(string RBMId, string Year, string Month, string TeamId)
        {

            Session.Remove("tblExcel");

            string RBM = "";
            string Team = "";
            string Retrslt = "";

            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            //decimal? USER_FKID = 149;
            // Session["Emp_Name"] = "Test";

            XmlTransform trans = new XmlTransform();
            string HeaddingMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(Month));
            RBM = ProductsXmlDM1(RBMId);
            Team = TeamXmlRBM(TeamId);
            if (RBM == "<row></row>")
            {
                Retrslt = bLayer.CyclePlanCoverageRpt_Nation(RBM, USER_FKID.ToString(), Month, Year, Team);
                trans.XslFile = xslPath + "CycleplanCoverageRpt_PSO.xsl";
                trans.XmlString = Retrslt;
            }
            else
            {
                Retrslt = bLayer.CyclePlanCoverageRpt_Nation(RBM, USER_FKID.ToString(), Month, Year, Team);
                trans.XslFile = xslPath + "CycleplanCoverageRptNation_PSO.xsl";
                trans.XmlString = Retrslt;
            }


            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);

            Session["tblExcel"] = rslt;

            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table><tr><td align='right' class='tb-r1' style='color:#2F72C3;font-weight:bold;'>  Cycle Plan Coverage Report of " + Session["Emp_Name"].ToString() + " (MM) for " + HeaddingMonth + " " + Year + "</td></tr></table>" + rslt + "<br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tableContainer);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";


            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);

        }


        public string TeamXmlRBM(string lstTeamName)
        {
            StringBuilder sb = new StringBuilder();
            string SelName = lstTeamName;

            if (SelName == "null" || SelName == "0")
            {
                sb.Append("<row></row>");

            }
            else
            {
                string[] arrayName = SelName.Split(',');

                sb.Append("<row>");
                for (int i = 0; i < arrayName.Count(); i++)
                {

                    sb.Append("<dp TId='" + arrayName[i]);
                    sb.Append("'/>");


                }
                sb.Append("</row>");

            }
            return sb.ToString();

        }


        public JsonResult JsonCCRNationPSO(string SampleFKID, string Month, string Year, string EmpName, string TeamId)
        {
            Session.Remove("tblExcel");

            //I need this session value based popup pso's Report  CCRNation  /* JsonResult Name  ---->  JsonCCRNPSO  */
            Session["GetName"] = EmpName;
            Session["GetId"] = SampleFKID;

            string Team = "";
            string HeaddingMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(Month));
            string Retrslt = "";
            Team = TeamXmlRBM(TeamId);
            DataSet ds = new DataSet();
            Retrslt = bLayer.CyclePlanCoverageRBM_DMRpt_New(Month, Year, SampleFKID, Team);
            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "CycleplanCoverageRptRBM_PSO.xsl";
            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            Session["tblExcel"] = rslt;
            //rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal1'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table><tr><td align='right' class='tb-r1' style='color:#2F72C3;font-weight:bold;'>  Cycle Plan Coverage Report of " + EmpName.ToString() + " for " + HeaddingMonth + " " + Year + "</td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";


            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }



        public JsonResult JsonCCRNPSO(string SampleFKID, string Month, string Year, string EmpName, string TeamId)
        {
            Session.Remove("tblExcel");
            string Team = "";

            //I need this session value based popup pso's Report  CCRNation
            SampleFKID = Session["GetId"].ToString();
            EmpName = Session["GetName"].ToString();


            string HeaddingMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Convert.ToInt32(Month));
            string Retrslt = "";
            Team = TeamXmlRBM(TeamId);
            DataSet ds = new DataSet();
            Retrslt = bLayer.CyclePlanCoverageNation_RBMRpt(Month, Year, SampleFKID, Team);
            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "CycleplanCoverageRptDM_PSO.xsl";
            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            Session["tblExcel"] = rslt;
            //rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal1'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='85%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table><tr><td align='right' class='tb-r1' style='color:#2F72C3;font-weight:bold;'>  Cycle Plan Coverage Report of " + EmpName.ToString() + " for " + HeaddingMonth + " " + Year + "</td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";




            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }

        #endregion

        #endregion

        #region MuniPrathap

        //PrescriptionTracker  Starts 
        public ActionResult PrescriptionTracker()
        {
            return View();
        }
        //Loading ListBoxFor PSO
        public JsonResult JsonLoadListName()
        {

            int USER_FKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetPsoNameByReport(USER_FKID) select new { a.PKID, a.EmpName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }
        //For Loading Product Name
        public JsonResult JsonLoadProductName()
        {
            decimal Psoid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetPrescriptionProductNamebyPSO(Psoid) select new { a.PKID, a.ProductName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.ProductName,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public string ProductPackXml(string lstPSOName)
        {

            string SelName = lstPSOName.Remove(lstPSOName.Length - 1, 1);

            string[] arrayName = SelName.Split(',');
            StringBuilder sb = new StringBuilder();
            sb.Append("<Root>");
            for (int i = 0; i < arrayName.Count(); i++)
            {
                sb.Append("<PSO PSOFKID='" + arrayName[i]);
                sb.Append("'/>");

            }
            sb.Append("</Root>");
            return sb.ToString();
        }


        public JsonResult GridPrescriptionTracker(GridQueryModel gridQueryModel, string BVOStatus, string ProductName, string Frmdate, string Todate)
        {
            try
            {
                DataSet ds = new DataSet();

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                int? Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                int? Territory_FKID = Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());

                ds = bLayer.getPrescriptionByPSO(Psoid, DateTime.Parse(Frmdate), DateTime.Parse(Todate), Territory_FKID, Convert.ToInt32(ProductName), Convert.ToInt32(BVOStatus));
                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PsoWisePrescriptionTracker
                {
                    Date = dataRow[0].ToString(),
                    PSOName = dataRow[1].ToString(),
                    RegionCode = dataRow[2].ToString(),
                    TerritoryCode = dataRow[3].ToString(),
                    DoctorName = dataRow[4].ToString(),
                    Specilization = dataRow[5].ToString(),
                    ProductName = dataRow[6].ToString(),
                    ProductPackName = dataRow[7].ToString(),
                    Prescription = dataRow[8].ToString(),
                    Hospital = dataRow[9].ToString(),
                    Stokiest = dataRow[10].ToString(),
                    BVOStatus = dataRow[11].ToString(),

                }
                ).ToList();

                var query = (from a in item select new { a.Date, a.PSOName, a.RegionCode, a.TerritoryCode, a.DoctorName, a.Specilization, a.ProductName, a.ProductPackName, a.Prescription, a.Hospital, a.Stokiest, a.BVOStatus }).ToList();


                var count = query.Count();
                var pageData = query.OrderBy(x => x.Date).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }
        public ActionResult ExportPrescriptionTrackerToExcel(string BVOStatus, string ProductName, string Frmdate, string Todate)
        {

            //DateTime? fromDate = Convert.ToDateTime(txtFrmdate);
            //DateTime? toDate = Convert.ToDateTime(txtTodate);
            DataSet ds = new DataSet();
            int? Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            int? Territory_FKID = Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());
            ds = bLayer.getPrescriptionByPSO(Psoid, DateTime.Parse(Frmdate), DateTime.Parse(Todate), Territory_FKID, Convert.ToInt32(ProductName), Convert.ToInt32(BVOStatus));
            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PsoWisePrescriptionTracker
            {
                Date = dataRow[0].ToString(),
                PSOName = dataRow[1].ToString(),
                RegionCode = dataRow[2].ToString(),
                TerritoryCode = dataRow[3].ToString(),
                DoctorName = dataRow[4].ToString(),
                Specilization = dataRow[5].ToString(),
                ProductName = dataRow[6].ToString(),
                ProductPackName = dataRow[7].ToString(),
                Prescription = dataRow[8].ToString(),
                Hospital = dataRow[9].ToString(),
                Stokiest = dataRow[10].ToString(),
                BVOStatus = dataRow[11].ToString(),

            }
                   ).ToList();

            //if (context.Count == 0)
            // return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[]{
                 
                  item.Date.ToString()==null?"":item.Date.ToString(),
                  item.PSOName.ToString()==null?"":item.PSOName.ToString(),
                  item.RegionCode.ToString()==null?"":item.RegionCode.ToString(),
                  item.TerritoryCode.ToString()==null?"":item.RegionCode.ToString(),
                  item.DoctorName.ToString()==null?"":item.RegionCode.ToString(),
                  item.Specilization.ToString()==null?"":item.RegionCode.ToString(),
                  item.ProductName.ToString()==null?"":item.RegionCode.ToString(),
                  item.ProductPackName.ToString()==null?"":item.RegionCode.ToString(),
                  item.Prescription.ToString()==null?"":item.RegionCode.ToString(),
                  item.Hospital.ToString()==null?"":item.RegionCode.ToString(),
                  item.Stokiest.ToString()==null?"":item.RegionCode.ToString(),
                  item.BVOStatus.ToString()==null?"":item.RegionCode.ToString()

                 
                  
               }));

            return new ExcelResult(HeadersPSOWisePrescriptionTracker, ColunmTypesForPSOPrescription, data, "PSOWisePrescriptionTracker.xlsx", "PSOWisePrescriptionTracker");


        }

        private static readonly string[] HeadersPSOWisePrescriptionTracker = { "Date", "PSOName", "RegionCode", "TerritoryCode", "DoctorName", "Specilization", "ProductName", "ProductPackName", "Prescription", "Hospital", "Stokiest", "BVOStatus" };

        private static readonly DataForExcel.DataType[] ColunmTypesForPSOPrescription = {

        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
         DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String
         };
        public ActionResult ExportPrescriptionTrackerToPDF(string BVOStatus, string ProductName, string Frmdate, string Todate)
        {

            //DateTime? fromDate = Convert.ToDateTime(txtFrmdate);
            //DateTime? toDate = Convert.ToDateTime(txtTodate);
            List<RR_PsoWisePrescriptionTracker> userList = new List<RR_PsoWisePrescriptionTracker>();
            //var context = (from a in dcReports.getPrescriptionForSM_New(fromDate, toDate, xml, drpSelectProductName) select a).ToList();
            DataSet ds = new DataSet();
            int? Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            int? Territory_FKID = Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());
            ds = bLayer.getPrescriptionByPSO(Psoid, DateTime.Parse(Frmdate), DateTime.Parse(Todate), Territory_FKID, Convert.ToInt32(ProductName), Convert.ToInt32(BVOStatus));
            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PsoWisePrescriptionTracker
            {
                Date = dataRow[0].ToString(),
                PSOName = dataRow[1].ToString(),
                RegionCode = dataRow[2].ToString(),
                TerritoryCode = dataRow[3].ToString(),
                DoctorName = dataRow[4].ToString(),
                Specilization = dataRow[5].ToString(),
                ProductName = dataRow[6].ToString(),
                ProductPackName = dataRow[7].ToString(),
                Prescription = dataRow[8].ToString(),
                Hospital = dataRow[9].ToString(),
                Stokiest = dataRow[10].ToString(),
                BVOStatus = dataRow[11].ToString(),

            }
                   ).ToList();
            foreach (var item in context)
            {
                RR_PsoWisePrescriptionTracker user = new RR_PsoWisePrescriptionTracker();

                user.Date = item.Date == null ? "" : item.Date.ToString();
                user.PSOName = item.PSOName == null ? "" : item.PSOName.ToString();
                user.RegionCode = item.RegionCode == null ? "" : item.RegionCode.ToString();
                user.TerritoryCode = item.TerritoryCode == null ? "" : item.TerritoryCode.ToString();
                user.DoctorName = item.DoctorName == null ? "" : item.DoctorName.ToString();
                user.Specilization = item.Specilization == null ? "" : item.Specilization.ToString();
                user.ProductName = item.ProductName == null ? "" : item.ProductName.ToString();
                user.ProductPackName = item.ProductPackName == null ? "" : item.ProductPackName.ToString();
                user.Prescription = item.Prescription == null ? "" : item.Prescription.ToString();
                user.Hospital = item.Hospital == null ? "" : item.Hospital.ToString();
                user.Stokiest = item.Stokiest == null ? "" : item.Stokiest.ToString();
                user.BVOStatus = item.BVOStatus == null ? "" : item.BVOStatus.ToString();

                userList.Add(user);
            }
            var customerList = userList;
            pdf.ExportPDF(customerList, new string[] { "Date", "PSOName", "RegionCode", "TerritoryCode", "DoctorName", "Specilization", "ProductName", "ProductPackName", "Prescription", "Hospital", "Stokiest", "BVOStatus" }, path);
            return File(path, "application/pdf", "PSOWisePrescriptionTracker.pdf");
        }

        public class PSOPrescriptionTrackerPDF
        {
            public string DoctorName { get; set; }
            public string Specialization { get; set; }
            public string AreaName { get; set; }
        }



        //TrackSheetReportForFTM  Starts 
        public ActionResult TrackSheetReportForFTM()
        {
            return View();
        }
        public JsonResult JsonLoadSelectFTMName()
        {
            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetNamebyIDForFTMTrackSheet(USER_FKID) select new { a.PKID, a.EmpName }).ToList();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult JsonLoadSelectFTMYear()
        {

            Dictionary<int, string> rslt = new Dictionary<int, string>();

            int yr;
            int Startyr = Convert.ToInt32(ConfigurationManager.AppSettings["StartYear"].ToString());
            int currentYr = DateTime.Now.Year;

            for (yr = Startyr; yr <= currentYr + 2; yr++)
            {
                rslt.Add(yr, yr.ToString());
            }
            return PartialView("LoadYear", rslt);

        }
        public JsonResult GridTrackSheetReportForFTM(GridQueryModel gridQueryModel, string FTM, string YEAR, string Flag)
        {
            try
            {

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                if (Flag == "2")
                {
                    var query = (from a in dcReports.TrackSheetReportFromJobsForFTM(Convert.ToInt32(FTM), Convert.ToInt32(YEAR)) select a).ToList();
                    var count = query.Count();
                    var pageData = query.OrderBy(x => x.PKID).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    return Json(new
                    {
                        page = gridQueryModel.page,
                        records = count,
                        rows = pageData,
                        total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var query = (from a in dcReports.TrackSheetReportFromJobs(Convert.ToInt32(FTM), Convert.ToInt32(YEAR)) select a).ToList();
                    var count = query.Count();
                    var pageData = query.OrderBy(x => x.PKID).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    return Json(new
                    {
                        page = gridQueryModel.page,
                        records = count,
                        rows = pageData,
                        total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                return Json(gridQueryModel);
            }


        }

        public ActionResult ExportTrackSheetReportForFTMToExcel(string YEAR, string Flag, string FTM)
        {
            try
            {
                if (Flag == "2")
                {
                    var context = (from a in dcReports.TrackSheetReportFromJobsForFTM(Convert.ToInt32(FTM), Convert.ToInt32(YEAR)) select a).ToList();

                    //if (context.Count == 0)
                    //    return new EmptyResult();
                    var data = new List<string[]>(context.Count);
                    data.AddRange(context.Select(item => new[]{
                  item.Parameters.ToString()==null?"":item.Parameters.ToString(),
                  item.Dec.ToString()==null?"":item.Dec.ToString(),
                  item.Jan.ToString()==null?"":item.Jan.ToString(),
                  item.Feb.ToString()==null?"":item.Feb.ToString(),
                  item.Mar.ToString()==null?"":item.Mar.ToString(),
                  item.Apr.ToString()==null?"":item.Apr.ToString(),
                  item.May.ToString()==null?"":item.May.ToString(),
                  item.Jun.ToString()==null?"":item.Jun.ToString(),
                  item.Jul.ToString()==null?"":item.Jul.ToString(),
                  item.Aug.ToString()==null?"":item.Aug.ToString(),
                  item.Sep.ToString()==null?"":item.Sep.ToString(),
                  item.Oct.ToString()==null?"":item.Oct.ToString(),
                  item.Nov.ToString()==null?"":item.Nov.ToString(),
                  item.YTD.ToString()==null?"":item.YTD.ToString()
                  
               }));

                    return new ExcelResult(HeadersTrackSheetForFTM, ColunmTypesHospitalMaster, data, "TrackSheetReportForFTM.xlsx", "TrackSheetReportForFTM");
                }
                else
                {

                    var context = (from a in dcReports.TrackSheetReportFromJobs(Convert.ToInt32(FTM), Convert.ToInt32(YEAR)) select a).ToList();

                    //if (context.Count == 0)
                    //    return new EmptyResult();
                    var data = new List<string[]>(context.Count);
                    data.AddRange(context.Select(item => new[]{
                  item.Parameters.ToString()==null?"":item.Parameters.ToString(),
                  item.Dec.ToString()==null?"":item.Dec.ToString(),
                  item.Jan.ToString()==null?"":item.Jan.ToString(),
                  item.Feb.ToString()==null?"":item.Feb.ToString(),
                  item.Mar.ToString()==null?"":item.Mar.ToString(),
                  item.Apr.ToString()==null?"":item.Apr.ToString(),
                  item.May.ToString()==null?"":item.May.ToString(),
                  item.Jun.ToString()==null?"":item.Jun.ToString(),
                  item.Jul.ToString()==null?"":item.Jul.ToString(),
                  item.Aug.ToString()==null?"":item.Aug.ToString(),
                  item.Sep.ToString()==null?"":item.Sep.ToString(),
                  item.Oct.ToString()==null?"":item.Oct.ToString(),
                  item.Nov.ToString()==null?"":item.Nov.ToString(),
                  item.YTD.ToString()==null?"":item.YTD.ToString()
                  
               }));
                    return new ExcelResult(HeadersTrackSheetForFTM, ColunmTypesHospitalMaster, data, "TrackSheetReport.xlsx", "TrackSheetReport");

                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private static readonly string[] HeadersTrackSheetForFTM = { "Parameters", "Last DEC", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "YTD" };

        private static readonly DataForExcel.DataType[] ColunmTypesHospitalMaster = {

        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,

        };

        public ActionResult ExportTrackSheetReportForFTMToPDF(string YEAR, string Flag, string FTM)
        {
            try
            {
                List<TrackSheetForFTMPDF> userList = new List<TrackSheetForFTMPDF>();
                if (Flag == "2")
                {
                    var context = (from a in dcReports.TrackSheetReportFromJobsForFTM(Convert.ToInt32(FTM), Convert.ToInt32(YEAR)) select a).ToList();

                    foreach (var item in context)
                    {
                        TrackSheetForFTMPDF user = new TrackSheetForFTMPDF();
                        user.Parameters = item.Parameters.ToString() == null ? "" : item.Parameters.ToString();
                        user.Dec = item.Dec.ToString() == null ? "" : item.Dec.ToString();
                        user.Jan = item.Jan.ToString() == null ? "" : item.Jan.ToString();
                        user.Feb = item.Feb.ToString() == null ? "" : item.Feb.ToString();
                        user.Mar = item.Mar.ToString() == null ? "" : item.Mar.ToString();
                        user.Apr = item.May.ToString() == null ? "" : item.May.ToString();
                        user.May = item.May.ToString() == null ? "" : item.May.ToString();
                        user.Jun = item.Jun.ToString() == null ? "" : item.Jun.ToString();
                        user.Jul = item.Jul.ToString() == null ? "" : item.Jul.ToString();
                        user.Aug = item.Aug.ToString() == null ? "" : item.Aug.ToString();
                        user.Sep = item.Sep.ToString() == null ? "" : item.Sep.ToString();
                        user.Oct = item.Oct.ToString() == null ? "" : item.Oct.ToString();
                        user.Nov = item.Nov.ToString() == null ? "" : item.Nov.ToString();
                        user.YTD = item.YTD.ToString() == null ? "" : item.YTD.ToString();



                        userList.Add(user);
                    }
                    var customerList = userList;
                    pdf.ExportPDF(customerList, new string[] { "Parameters", "Dec", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "YTD" }, path);
                    return File(path, "application/pdf", "TrackSheetForFTM.pdf");
                }
                else
                {

                    var context = (from a in dcReports.TrackSheetReportFromJobs(Convert.ToInt32(FTM), Convert.ToInt32(YEAR)) select a).ToList();

                    foreach (var item in context)
                    {
                        TrackSheetForFTMPDF user = new TrackSheetForFTMPDF();
                        user.Parameters = item.Parameters.ToString() == null ? "" : item.Parameters.ToString();
                        user.Dec = item.Dec.ToString() == null ? "" : item.Dec.ToString();
                        user.Jan = item.Jan.ToString() == null ? "" : item.Jan.ToString();
                        user.Feb = item.Feb.ToString() == null ? "" : item.Feb.ToString();
                        user.Mar = item.Mar.ToString() == null ? "" : item.Mar.ToString();
                        user.Apr = item.May.ToString() == null ? "" : item.May.ToString();
                        user.May = item.May.ToString() == null ? "" : item.May.ToString();
                        user.Jun = item.Jun.ToString() == null ? "" : item.Jun.ToString();
                        user.Jul = item.Jul.ToString() == null ? "" : item.Jul.ToString();
                        user.Aug = item.Aug.ToString() == null ? "" : item.Aug.ToString();
                        user.Sep = item.Sep.ToString() == null ? "" : item.Sep.ToString();
                        user.Oct = item.Oct.ToString() == null ? "" : item.Oct.ToString();
                        user.Nov = item.Nov.ToString() == null ? "" : item.Nov.ToString();
                        user.YTD = item.YTD.ToString() == null ? "" : item.YTD.ToString();



                        userList.Add(user);
                    }
                    var customerList = userList;
                    pdf.ExportPDF(customerList, new string[] { "Parameters", "Dec", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "YTD" }, path);
                    return File(path, "application/pdf", "TrackSheet.pdf");
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public class TrackSheetForFTMPDF
        {

            public string Parameters { get; set; }
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
        }

        //History Report Summery
        public ActionResult HistoryReportSummery()
        {
            return View();
        }

        public JsonResult JsonLoadSelectPSOName()
        {
            //decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetPsoNameForAdmin() select new { a.PKID, a.EmpName }).ToList();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult LoadSelectPSOYear()
        {

            Dictionary<int, string> rslt = new Dictionary<int, string>();

            int yr;
            int Startyr = Convert.ToInt32(ConfigurationManager.AppSettings["StartYear"].ToString());
            int currentYr = DateTime.Now.Year;

            for (yr = Startyr; yr <= currentYr + 2; yr++)
            {
                rslt.Add(yr, yr.ToString());
            }
            return PartialView("LoadYear", rslt);

        }
        public ActionResult LoadSelectPSOMonth()
        {

            // var query = (from e in dcReports.USP_GET_MONTH() select new { e.PKID, e.GenName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.GenName.ToString());
            var query = (from e in dcReports.GetMonthDetails() select new { e.MonthNo, e.MonthName }).ToDictionary(f => Convert.ToInt32(f.MonthNo), f => f.MonthName.ToString());
            return PartialView("LoadYear", query);


        }
        //POtential Yield For PSO
        public ActionResult PotentialyieldForPSO()
        {
            return View();

        }
        public string ProductsXmlPSO(string lstPSOName)
        {

            string SelName = lstPSOName.Remove(lstPSOName.Length - 1, 1);

            string[] arrayName = SelName.Split(',');
            StringBuilder sb = new StringBuilder();

            sb.Append("<root>");
            for (int i = 0; i < arrayName.Count(); i++)
            {
                sb.Append("<Prod productFKID='" + arrayName[i]);
                sb.Append("'/>");


            }
            sb.Append("</root>");
            return sb.ToString();
        }
        public JsonResult JsonLoadlstProducts()
        {

            int USER_FKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetProductNameforSampleReport(USER_FKID) select new { a.ProductFKID, a.ProductName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.ProductName,
                    Value = x.ProductFKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GridPotentialYieldForPSOSSummery(GridQueryModel gridQueryModel, string Products, string Year, string Month, string Type)
        {
            try
            {
                string xml = "";

                xml = ProductsXmlPSO(Products);
                DataSet ds = new DataSet();
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                long? psoFKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                ds = bLayer.GETPotentialYieldForPSO(psoFKID, Convert.ToInt32(Month), Convert.ToInt32(Year), xml, Type);

                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSO
                {
                    productName = dataRow[0].ToString(),
                    Potential = dataRow[1].ToString(),
                    Yield = dataRow[2].ToString(),


                }
                ).ToList();

                var query = (from a in item select new { a.productName, a.Potential, a.Yield }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.productName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }

        public JsonResult GridPotentialYieldForPSODDetails(GridQueryModel gridQueryModel, string Products, string Year, string Month, string Type)
        {
            try
            {
                string xml = "";

                xml = ProductsXmlPSO(Products);

                DataSet ds = new DataSet();
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                long? psoFKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                ds = bLayer.GETPotentialYieldForPSO(psoFKID, Convert.ToInt32(Month), Convert.ToInt32(Year), xml, Type);
                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString(),


                }
                ).ToList();

                var query = (from a in item select a).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }
        /*Coding For Excel Downloading */
        public ActionResult ExportPotentialYieldForPSOSToExcel(string Name, string Year, string Month, string Type)
        {

            string xml = "";

            xml = ProductsXmlPSO(Name);

            DataSet ds = new DataSet();

            long? psoFKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            ds = bLayer.GETPotentialYieldForPSO(psoFKID, Convert.ToInt32(Month), Convert.ToInt32(Year), xml, Type);

            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSO
            {
                productName = dataRow[0].ToString(),
                Potential = dataRow[1].ToString(),
                Yield = dataRow[2].ToString(),

            }
              ).ToList();

            //if (context.Count == 0)
            // return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[]{
                 
                  item.productName.ToString()==null?"":item.productName.ToString(),
                  item.Potential.ToString()==null?"":item.Potential.ToString(),
                  item.Yield.ToString()==null?"":item.Yield.ToString()
                 
                  
               }));

            return new ExcelResult(HeadersTrackSheetForPSOSummery, ColunmTypesForPSOSummery, data, "PotentialYieldForPsoSummery.xlsx", "PotentialYieldForPsoSummery");


        }

        private static readonly string[] HeadersTrackSheetForPSOSummery = { "productName", "Potential", "Yield" };

        private static readonly DataForExcel.DataType[] ColunmTypesForPSOSummery = {

        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String
       

        };
        public ActionResult ExportPotentialYieldForPSOSToPDF(string Name, string Year, string Month, string Type)
        {

            List<RRPotentialYieldForPSOS> userList = new List<RRPotentialYieldForPSOS>();

            string xml = "";

            xml = ProductsXmlPSO(Name);
            DataSet ds = new DataSet();
            long? psoFKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            ds = bLayer.GETPotentialYieldForPSO(psoFKID, Convert.ToInt32(Month), Convert.ToInt32(Year), xml, Type);
            var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSO
            {
                productName = dataRow[0].ToString(),
                Potential = dataRow[1].ToString(),
                Yield = dataRow[2].ToString(),



            }
                           ).ToList();


            foreach (var item in context)
            {
                RRPotentialYieldForPSOS user = new RRPotentialYieldForPSOS();

                user.productName = item.productName == null ? "" : item.productName;
                user.Potential = item.Potential == null ? "" : item.Potential;
                user.Yield = item.Yield == null ? "" : item.Yield;

                userList.Add(user);
            }
            var customerList = userList;



            pdf.ExportPDF(customerList, new string[] { "productName", "Potential", "Yield" }, path);
            return File(path, "application/pdf", "PotentialYieldForPsoSummery.pdf");

        }


        public class RRPotentialYieldForPSOS
        {
            public string productName { get; set; }
            public string Potential { get; set; }
            public string Yield { get; set; }
        }
        public ActionResult ExportPotentialYieldForPSODToExcel(string Name, string Year, string Month, string Type, string PSOName, string Flag)
        {

            //string xml = "";

            //xml = ProductsXmlPSO(Name);
            string xmlPSO = "";
            string xmlProducts = "";
            xmlProducts = ProductsXmlPSO(Name);

            DataSet ds = new DataSet();
            if (Flag == "1")
            {
                long? psoFKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                ds = bLayer.GETPotentialYieldForPSO(psoFKID, Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type);
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString()
                }
              ).ToList();

                //if (context.Count == 0)
                // return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                 
                  item.DoctorName.ToString()==null?"":item.DoctorName.ToString(),
                  item.segment.ToString()==null?"":item.segment.ToString(),
                  item.ProductName.ToString()==null?"":item.ProductName.ToString(),
                  item.TerCode.ToString()==null?"":item.TerCode.ToString(),
                  item.TerName.ToString()==null?"":item.TerName.ToString(),
                  item.DistrictCode.ToString()==null?"":item.DistrictCode.ToString(),
                  item.DistrictName.ToString()==null?"":item.DistrictName.ToString(),
                  item.RegionCode.ToString()==null?"":item.RegionCode.ToString(),
                  item.RegionName.ToString()==null?"":item.RegionName.ToString(),
                  item.Potential.ToString()==null?"":item.Potential.ToString(),
                   item.Yield.ToString()==null?"":item.Yield.ToString()
                 
                  
               }));
                return new ExcelResult(HeadersTrackSheetForPSODetails, ColunmTypesForPSODetails, data, "PotentialYieldForPsoDetails.xlsx", "PotentialYieldForPsoDetails");

            }
            else if (Flag == "2")
            {
                xmlPSO = ProductsXmlDM(PSOName);
                ds = bLayer.GETPotentialYieldForDM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString()
                }
              ).ToList();

                //if (context.Count == 0)
                // return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                 
                  item.DoctorName.ToString()==null?"":item.DoctorName.ToString(),
                  item.segment.ToString()==null?"":item.segment.ToString(),
                  item.ProductName.ToString()==null?"":item.ProductName.ToString(),
                  item.TerCode.ToString()==null?"":item.TerCode.ToString(),
                  item.TerName.ToString()==null?"":item.TerName.ToString(),
                  item.DistrictCode.ToString()==null?"":item.DistrictCode.ToString(),
                  item.DistrictName.ToString()==null?"":item.DistrictName.ToString(),
                  item.RegionCode.ToString()==null?"":item.RegionCode.ToString(),
                  item.RegionName.ToString()==null?"":item.RegionName.ToString(),
                  item.Potential.ToString()==null?"":item.Potential.ToString(),
                   item.Yield.ToString()==null?"":item.Yield.ToString()
                 
                  
               }));
                return new ExcelResult(HeadersTrackSheetForPSODetails, ColunmTypesForPSODetails, data, "PotentialYieldForDMDetails.xlsx", "PotentialYieldForDMDetails");

            }
            else
            {

                xmlPSO = ProductsXmlDM(PSOName);
                //ds = bLayer.GETPotentialYieldForDM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                ds = bLayer.GETPotentialYieldForRBMFTM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);

                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString()
                }
              ).ToList();

                //if (context.Count == 0)
                // return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                 
                  item.DoctorName.ToString()==null?"":item.DoctorName.ToString(),
                  item.segment.ToString()==null?"":item.segment.ToString(),
                  item.ProductName.ToString()==null?"":item.ProductName.ToString(),
                  item.TerCode.ToString()==null?"":item.TerCode.ToString(),
                  item.TerName.ToString()==null?"":item.TerName.ToString(),
                  item.DistrictCode.ToString()==null?"":item.DistrictCode.ToString(),
                  item.DistrictName.ToString()==null?"":item.DistrictName.ToString(),
                  item.RegionCode.ToString()==null?"":item.RegionCode.ToString(),
                  item.RegionName.ToString()==null?"":item.RegionName.ToString(),
                  item.Potential.ToString()==null?"":item.Potential.ToString(),
                   item.Yield.ToString()==null?"":item.Yield.ToString()
                 
                  
               }));
                return new ExcelResult(HeadersTrackSheetForPSODetails, ColunmTypesForPSODetails, data, "PotentialYieldForRBMDetails.xlsx", "PotentialYieldForRBMDetails");


            }




        }

        private static readonly string[] HeadersTrackSheetForPSODetails = { "DoctorName", "segment", "ProductName", "TerCode", "TerName", "DistrictCode", "DistrictName", "RegionCode", "RegionName", "Potential", "Yield" };

        private static readonly DataForExcel.DataType[] ColunmTypesForPSODetails = {

        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String
        

        };
        public ActionResult ExportPotentialYieldForPSODToPDF(string Name, string Year, string Month, string Type, string PSOName, string Flag)
        {

            List<RRPotentialYieldForPSOD> userList = new List<RRPotentialYieldForPSOD>();


            string xmlPSO = "";
            string xmlProducts = "";
            xmlProducts = ProductsXmlPSO(Name);
            DataSet ds = new DataSet();
            if (Flag == "1")
            {
                long? psoFKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                ds = bLayer.GETPotentialYieldForPSO(psoFKID, Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type);
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString()



                }
                                           ).ToList();


                foreach (var item in context)
                {
                    RRPotentialYieldForPSOD user = new RRPotentialYieldForPSOD();


                    user.DoctorName = item.DoctorName == null ? "" : item.DoctorName.ToString();
                    user.segment = item.segment == null ? "" : item.segment.ToString();
                    user.ProductName = item.ProductName == null ? "" : item.ProductName.ToString();
                    user.TerCode = item.TerCode == null ? "" : item.TerCode.ToString();
                    user.TerName = item.TerName == null ? "" : item.TerName.ToString();
                    user.DistrictCode = item.DistrictCode == null ? "" : item.DistrictCode.ToString();
                    user.DistrictName = item.ProductName == null ? "" : item.ProductName.ToString();
                    user.RegionCode = item.RegionCode == null ? "" : item.RegionCode.ToString();
                    user.RegionName = item.RegionName == null ? "" : item.RegionName.ToString();
                    user.Potential = item.Potential == null ? "" : item.Potential.ToString();
                    user.Yield = item.Yield == null ? "" : item.Yield.ToString();


                    userList.Add(user);
                }
                var customerList = userList;



                pdf.ExportPDF(customerList, new string[] { "DoctorName", "segment", "ProductName", "TerCode", "TerName", "DistrictCode", "DistrictName", "RegionCode", "RegionName", "Potential", "Yield" }, path);
                return File(path, "application/pdf", "PotentialYieldForPsoDetails.pdf");

            }
            else if (Flag == "2")
            {
                xmlPSO = ProductsXmlDM(PSOName);
                ds = bLayer.GETPotentialYieldForDM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString()



                }
                            ).ToList();


                foreach (var item in context)
                {
                    RRPotentialYieldForPSOD user = new RRPotentialYieldForPSOD();


                    user.DoctorName = item.DoctorName == null ? "" : item.DoctorName.ToString();
                    user.segment = item.segment == null ? "" : item.segment.ToString();
                    user.ProductName = item.ProductName == null ? "" : item.ProductName.ToString();
                    user.TerCode = item.TerCode == null ? "" : item.TerCode.ToString();
                    user.TerName = item.TerName == null ? "" : item.TerName.ToString();
                    user.DistrictCode = item.DistrictCode == null ? "" : item.DistrictCode.ToString();
                    user.DistrictName = item.ProductName == null ? "" : item.ProductName.ToString();
                    user.RegionCode = item.RegionCode == null ? "" : item.RegionCode.ToString();
                    user.RegionName = item.RegionName == null ? "" : item.RegionName.ToString();
                    user.Potential = item.Potential == null ? "" : item.Potential.ToString();
                    user.Yield = item.Yield == null ? "" : item.Yield.ToString();


                    userList.Add(user);
                }
                var customerList = userList;



                pdf.ExportPDF(customerList, new string[] { "DoctorName", "segment", "ProductName", "TerCode", "TerName", "DistrictCode", "DistrictName", "RegionCode", "RegionName", "Potential", "Yield" }, path);
                return File(path, "application/pdf", "PotentialYieldForDMDetails.pdf");

            }
            else
            {

                xmlPSO = ProductsXmlDM(PSOName);
                ds = bLayer.GETPotentialYieldForRBMFTM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString()



                }
                            ).ToList();


                foreach (var item in context)
                {
                    RRPotentialYieldForPSOD user = new RRPotentialYieldForPSOD();


                    user.DoctorName = item.DoctorName == null ? "" : item.DoctorName.ToString();
                    user.segment = item.segment == null ? "" : item.segment.ToString();
                    user.ProductName = item.ProductName == null ? "" : item.ProductName.ToString();
                    user.TerCode = item.TerCode == null ? "" : item.TerCode.ToString();
                    user.TerName = item.TerName == null ? "" : item.TerName.ToString();
                    user.DistrictCode = item.DistrictCode == null ? "" : item.DistrictCode.ToString();
                    user.DistrictName = item.ProductName == null ? "" : item.ProductName.ToString();
                    user.RegionCode = item.RegionCode == null ? "" : item.RegionCode.ToString();
                    user.RegionName = item.RegionName == null ? "" : item.RegionName.ToString();
                    user.Potential = item.Potential == null ? "" : item.Potential.ToString();
                    user.Yield = item.Yield == null ? "" : item.Yield.ToString();


                    userList.Add(user);
                }
                var customerList = userList;



                pdf.ExportPDF(customerList, new string[] { "DoctorName", "segment", "ProductName", "TerCode", "TerName", "DistrictCode", "DistrictName", "RegionCode", "RegionName", "Potential", "Yield" }, path);
                return File(path, "application/pdf", "PotentialYieldForRBMDetails.pdf");


            }

        }


        public class RRPotentialYieldForPSOD
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
        /*PotentialYield For DM */
        public ActionResult PotentialYieldForDM()
        {

            return View();
        }

        public JsonResult JsonLoadlstPSO()
        {

            int USER_FKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetPsoNameByReport(USER_FKID) select new { a.PKID, a.EmpName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }
        public string ProductsXmlDM(string lstPSOName)
        {

            string SelName = lstPSOName.Remove(lstPSOName.Length - 1, 1);

            string[] arrayName = SelName.Split(',');
            StringBuilder sb = new StringBuilder();

            sb.Append("<row>");
            for (int i = 0; i < arrayName.Count(); i++)
            {

                sb.Append("<dp PSOFKID=\"" + arrayName[i]);
                sb.Append("\"/>");


            }
            sb.Append("</row>");
            return sb.ToString();
        }
        public JsonResult GridPotentialYieldForDMS(GridQueryModel gridQueryModel, string Products, string Year, string Month, string Type, string PSOName)
        {
            try
            {
                string xmlPSO = "";
                string xmlProducts = "";
                xmlProducts = ProductsXmlPSO(Products);
                xmlPSO = ProductsXmlDM(PSOName);
                DataSet ds = new DataSet();
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                long? psoFKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                ds = bLayer.GETPotentialYieldForDM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForDM
                {
                    ProductName = dataRow[0].ToString(),
                    Potential = dataRow[1].ToString(),
                    Yield = dataRow[2].ToString(),


                }
                ).ToList();

                var query = (from a in item select new { a.ProductName, a.Potential, a.Yield }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }
        public ActionResult ExportPotentialYieldForDMSToExcel(string Products, string PSOName, string Year, string Month, string Type, string Flag)
        {

            string xmlPSO = "";
            string xmlProducts = "";
            xmlProducts = ProductsXmlPSO(Products);
            xmlPSO = ProductsXmlDM(PSOName);

            DataSet ds = new DataSet();
            if (Flag == "1")
            {
                ds = bLayer.GETPotentialYieldForDM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForDM
                {
                    ProductName = dataRow[0].ToString(),
                    Potential = dataRow[1].ToString(),
                    Yield = dataRow[2].ToString(),

                }
                 ).ToList();

                //if (context.Count == 0)
                // return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                 
                  item.ProductName.ToString()==null?"":item.ProductName.ToString(),
                  item.Potential.ToString()==null?"":item.Potential.ToString(),
                  item.Yield.ToString()==null?"":item.Yield.ToString()
                 
                  
               }));

                return new ExcelResult(HeadersTrackSheetForDMSummery, ColunmTypesForDMSummery, data, "PotentialYieldForDMSummery.xlsx", "PotentialYieldForDMSummery");

            }
            else
            {
                ds = bLayer.GETPotentialYieldForRBMFTM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForDM
                {
                    ProductName = dataRow[0].ToString(),
                    Potential = dataRow[1].ToString(),
                    Yield = dataRow[2].ToString(),

                }
              ).ToList();

                //if (context.Count == 0)
                // return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                 
                  item.ProductName.ToString()==null?"":item.ProductName.ToString(),
                  item.Potential.ToString()==null?"":item.Potential.ToString(),
                  item.Yield.ToString()==null?"":item.Yield.ToString()
                 
                  
               }));

                return new ExcelResult(HeadersTrackSheetForDMSummery, ColunmTypesForDMSummery, data, "PotentialYieldForRBMSummery.xlsx", "PotentialYieldForRBMSummery");

            }


        }

        private static readonly string[] HeadersTrackSheetForDMSummery = { "ProductName", "Potential", "Yield" };

        private static readonly DataForExcel.DataType[] ColunmTypesForDMSummery = {

        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String
       

        };
        public ActionResult ExportPotentialYieldForDMSToPDF(string Products, string PSOName, string Year, string Month, string Type, string Flag)
        {

            List<RRPotentialYieldForDMS> userList = new List<RRPotentialYieldForDMS>();

            string xmlPSO = "";
            string xmlProducts = "";
            xmlProducts = ProductsXmlPSO(Products);
            xmlPSO = ProductsXmlDM(PSOName);
            DataSet ds = new DataSet();
            if (Flag == "1")
            {
                ds = bLayer.GETPotentialYieldForDM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForDM
                {
                    ProductName = dataRow[0].ToString(),
                    Potential = dataRow[1].ToString(),
                    Yield = dataRow[2].ToString(),



                }).ToList();

                foreach (var item in context)
                {
                    RRPotentialYieldForDMS user = new RRPotentialYieldForDMS();

                    user.ProductName = item.ProductName == null ? "" : item.ProductName;
                    user.Potential = item.Potential == null ? "" : item.Potential;
                    user.Yield = item.Yield == null ? "" : item.Yield;

                    userList.Add(user);
                }
                var customerList = userList;



                pdf.ExportPDF(customerList, new string[] { "ProductName", "Potential", "Yield" }, path);
                return File(path, "application/pdf", "PotentialYieldForDMSummery.pdf");
            }
            else
            {

                ds = bLayer.GETPotentialYieldForRBMFTM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);

                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForDM
                {
                    ProductName = dataRow[0].ToString(),
                    Potential = dataRow[1].ToString(),
                    Yield = dataRow[2].ToString(),



                }).ToList();

                foreach (var item in context)
                {
                    RRPotentialYieldForDMS user = new RRPotentialYieldForDMS();

                    user.ProductName = item.ProductName == null ? "" : item.ProductName;
                    user.Potential = item.Potential == null ? "" : item.Potential;
                    user.Yield = item.Yield == null ? "" : item.Yield;

                    userList.Add(user);
                }
                var customerList = userList;



                pdf.ExportPDF(customerList, new string[] { "ProductName", "Potential", "Yield" }, path);
                return File(path, "application/pdf", "PotentialYieldForRBMSummery.pdf");


            }
        }


        public class RRPotentialYieldForDMS
        {
            public string ProductName { get; set; }
            public string Potential { get; set; }
            public string Yield { get; set; }
        }
        public JsonResult GridPotentialYieldForDMDDetails(GridQueryModel gridQueryModel, string Products, string Year, string Month, string Type, string PSOName)
        {
            try
            {
                string xmlPSO = "";
                string xmlProducts = "";
                xmlProducts = ProductsXmlPSO(Products);
                xmlPSO = ProductsXmlDM(PSOName);

                DataSet ds = new DataSet();
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                ds = bLayer.GETPotentialYieldForDM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);

                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString(),


                }
                ).ToList();

                var query = (from a in item select a).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }

        }
        /*PotentialYield For RBM */
        public ActionResult PotentialYieldForRBM()
        {
            return View();
        }
        public JsonResult JsonLoadlstDM()
        {

            long? USER_FKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.SummeryReportReportingUsers(USER_FKID) select new { a.pkid, a.EmpName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.pkid.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult JsonLoadPSONameByDM(string DMName)
        {


            string[] arrayName = DMName.Split(',');
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            long? DMname = Convert.ToInt64(arrayName[0]);
            var item = (from a in dcReports.SummeryReportReportingUsers(DMname) select new { a.pkid, a.EmpName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.pkid.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult GridPotentialYieldForRBMS(GridQueryModel gridQueryModel, string Products, string Year, string Month, string Type, string DMName)
        {
            try
            {
                string xmlPSO = "";
                string xmlProducts = "";
                xmlProducts = ProductsXmlPSO(Products);
                xmlPSO = ProductsXmlDM(DMName);
                DataSet ds = new DataSet();
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                ds = bLayer.GETPotentialYieldForRBMFTM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);
                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForDM
                {
                    ProductName = dataRow[0].ToString(),
                    Potential = dataRow[1].ToString(),
                    Yield = dataRow[2].ToString(),


                }
                ).ToList();

                var query = (from a in item select new { a.ProductName, a.Potential, a.Yield }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }

        public JsonResult GridPotentialYieldForRBMDDetails(GridQueryModel gridQueryModel, string Products, string Year, string Month, string Type, string PSOName)
        {
            try
            {
                string xmlPSO = "";
                string xmlProducts = "";
                xmlProducts = ProductsXmlPSO(Products);
                xmlPSO = ProductsXmlDM(PSOName);

                DataSet ds = new DataSet();
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                ds = bLayer.GETPotentialYieldForRBMFTM(Convert.ToInt32(Month), Convert.ToInt32(Year), xmlProducts, Type, xmlPSO);

                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_PotentialYieldForPSODetails
                {
                    DoctorName = dataRow[0].ToString(),
                    segment = dataRow[1].ToString(),
                    ProductName = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    TerName = dataRow[4].ToString(),
                    DistrictCode = dataRow[5].ToString(),
                    DistrictName = dataRow[6].ToString(),
                    RegionCode = dataRow[7].ToString(),
                    RegionName = dataRow[8].ToString(),
                    Potential = dataRow[9].ToString(),
                    Yield = dataRow[10].ToString(),


                }
                ).ToList();

                var query = (from a in item select a).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }

        }
        /*Potential Yield For SM*/
        public ActionResult PotentialYieldForSM()
        {
            return View();
        }
        public JsonResult JsonLoadDrpSpec()
        {

            decimal? Territory_FKID = Convert.ToDecimal(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());


            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.RR_GetSpecialization(Territory_FKID) select new { a.PKID, a.Specialization }).ToList().Distinct();
            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.Specialization,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }


        /*Tracksheet Report For DM*/
        public ActionResult TracksheetReportForDM()
        {

            return View();
        }
        public JsonResult JsonLoadDrpNodeType()
        {

            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var item = (from a in dcReports.GetNodeByFKID(USER_FKID) select new { a.NodeFKID, a.NodeName }).ToList().Distinct();
            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.NodeName,
                    Value = x.NodeFKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JsonLoadDrpUser(string NodeType)
        {

            decimal? USER_FKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var item = (from a in dcReports.GetNamebyRepIDForTrackSheet(USER_FKID, Convert.ToDecimal(NodeType)) select new { a.UserFKID, a.EmpName }).ToList().Distinct();
            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.UserFKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GridTrackSheetReportForDMRBM(GridQueryModel gridQueryModel, string YEAR)
        {
            try
            {
                DataSet ds = new DataSet();
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                int? USER_FKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                ds = bLayer.TrackSheetReportFROMJobsFoRDMRBM(USER_FKID, Convert.ToInt32(YEAR));

                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_TrackSheetReportForDMRBM
                {
                    PKID = dataRow[0].ToString(),
                    Parameters = dataRow[1].ToString(),
                    Sequenceno = dataRow[2].ToString(),
                    Dec = dataRow[3].ToString(),
                    Jan = dataRow[4].ToString(),
                    Feb = dataRow[5].ToString(),
                    Mar = dataRow[6].ToString(),
                    Apr = dataRow[7].ToString(),
                    May = dataRow[8].ToString(),
                    Jun = dataRow[9].ToString(),
                    Jul = dataRow[10].ToString(),
                    Aug = dataRow[11].ToString(),
                    Sep = dataRow[12].ToString(),
                    Oct = dataRow[13].ToString(),
                    Nov = dataRow[14].ToString(),
                    YTD = dataRow[15].ToString(),
                    FinancialYear = dataRow[16].ToString(),
                    DMRBMFKID = dataRow[17].ToString(),


                }
                ).ToList();
                var query = (from a in item select a).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.PKID).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }
        public ActionResult ExportTrackSheetReportForDMRBMToExcel(string YEAR)
        {
            try
            {
                DataSet ds = new DataSet();

                //if (context.Count == 0)
                //    return new EmptyResult();
                int? USER_FKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                ds = bLayer.TrackSheetReportFROMJobsFoRDMRBM(USER_FKID, Convert.ToInt32(YEAR));
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_TrackSheetReportForDMRBM
                {
                    PKID = dataRow[0].ToString(),
                    Parameters = dataRow[1].ToString(),
                    Sequenceno = dataRow[2].ToString(),
                    Dec = dataRow[3].ToString(),
                    Jan = dataRow[4].ToString(),
                    Feb = dataRow[5].ToString(),
                    Mar = dataRow[6].ToString(),
                    Apr = dataRow[7].ToString(),
                    May = dataRow[8].ToString(),
                    Jun = dataRow[9].ToString(),
                    Jul = dataRow[10].ToString(),
                    Aug = dataRow[11].ToString(),
                    Sep = dataRow[12].ToString(),
                    Oct = dataRow[13].ToString(),
                    Nov = dataRow[14].ToString(),
                    YTD = dataRow[15].ToString(),
                    FinancialYear = dataRow[16].ToString(),
                    DMRBMFKID = dataRow[17].ToString(),


                }
               ).ToList();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                   // item.PKID.ToString() == null ? "" : item.PKID.ToString(),
                  item.Parameters.ToString()==null?"":item.Parameters.ToString(),
                  item.Sequenceno.ToString()==null?"":item.Sequenceno.ToString(),
                  item.Dec.ToString()==null?"":item.Dec.ToString(),
                  item.Jan.ToString()==null?"":item.Jan.ToString(),
                  item.Feb.ToString()==null?"":item.Feb.ToString(),
                  item.Mar.ToString()==null?"":item.Mar.ToString(),
                  item.Apr.ToString()==null?"":item.Apr.ToString(),
                  item.May.ToString()==null?"":item.May.ToString(),
                  item.Jun.ToString()==null?"":item.Jun.ToString(),
                  item.Jul.ToString()==null?"":item.Jul.ToString(),
                  item.Aug.ToString()==null?"":item.Aug.ToString(),
                  item.Sep.ToString()==null?"":item.Sep.ToString(),
                  item.Oct.ToString()==null?"":item.Oct.ToString(),
                  item.Nov.ToString()==null?"":item.Nov.ToString(),
                  item.YTD.ToString()==null?"":item.YTD.ToString(),
                  item.FinancialYear.ToString()==null?"":item.FinancialYear.ToString(),
                 // item.DMRBMFKID.ToString()==null?"":item.DMRBMFKID.ToString()
                  
               }));

                return new ExcelResult(HeadersTrackSheetForDMRBM, ColunmTypesTrackSheetForDMRBM, data, "TrackSheetReportForDMRBM.xlsx", "TrackSheetReportForDMRBM");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private static readonly string[] HeadersTrackSheetForDMRBM = { "Parameters", "Sequenceno", "DEC", "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "YTD", "FinancialYear" };

        private static readonly DataForExcel.DataType[] ColunmTypesTrackSheetForDMRBM = {

        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        
        };
        public ActionResult ExportTrackSheetReportForDMRBMToPDF(string YEAR)
        {
            try
            {
                DataSet ds = new DataSet();
                List<TrackSheetForDMRBMPDF> userList = new List<TrackSheetForDMRBMPDF>();
                //var context = (from a in dcReports.TrackSheetReportFromJobsForFTM(Convert.ToInt32(FTM), Convert.ToInt32(YEAR)) select a).ToList();
                int? USER_FKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                ds = bLayer.TrackSheetReportFROMJobsFoRDMRBM(USER_FKID, Convert.ToInt32(YEAR));
                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_TrackSheetReportForDMRBM
                {
                    PKID = dataRow[0].ToString(),
                    Parameters = dataRow[1].ToString(),
                    Sequenceno = dataRow[2].ToString(),
                    Dec = dataRow[3].ToString(),
                    Jan = dataRow[4].ToString(),
                    Feb = dataRow[5].ToString(),
                    Mar = dataRow[6].ToString(),
                    Apr = dataRow[7].ToString(),
                    May = dataRow[8].ToString(),
                    Jun = dataRow[9].ToString(),
                    Jul = dataRow[10].ToString(),
                    Aug = dataRow[11].ToString(),
                    Sep = dataRow[12].ToString(),
                    Oct = dataRow[13].ToString(),
                    Nov = dataRow[14].ToString(),
                    YTD = dataRow[15].ToString(),
                    FinancialYear = dataRow[16].ToString(),
                    DMRBMFKID = dataRow[17].ToString(),


                }
               ).ToList();
                foreach (var item in context)
                {
                    TrackSheetForDMRBMPDF user = new TrackSheetForDMRBMPDF();
                    //user.PKID = item.PKID.ToString() == null ? "" : item.PKID.ToString();
                    user.Parameters = item.Parameters.ToString() == null ? "" : item.Parameters.ToString();
                    user.Sequenceno = item.Sequenceno.ToString() == null ? "" : item.Sequenceno.ToString();
                    user.Dec = item.Dec.ToString() == null ? "" : item.Dec.ToString();
                    user.Jan = item.Jan.ToString() == null ? "" : item.Jan.ToString();
                    user.Feb = item.Feb.ToString() == null ? "" : item.Feb.ToString();
                    user.Mar = item.Mar.ToString() == null ? "" : item.Mar.ToString();
                    user.Apr = item.May.ToString() == null ? "" : item.May.ToString();
                    user.May = item.May.ToString() == null ? "" : item.May.ToString();
                    user.Jun = item.Jun.ToString() == null ? "" : item.Jun.ToString();
                    user.Jul = item.Jul.ToString() == null ? "" : item.Jul.ToString();
                    user.Aug = item.Aug.ToString() == null ? "" : item.Aug.ToString();
                    user.Sep = item.Sep.ToString() == null ? "" : item.Sep.ToString();
                    user.Oct = item.Oct.ToString() == null ? "" : item.Oct.ToString();
                    user.Nov = item.Nov.ToString() == null ? "" : item.Nov.ToString();
                    user.YTD = item.YTD.ToString() == null ? "" : item.YTD.ToString();
                    user.FinancialYear = item.FinancialYear.ToString() == null ? "" : item.FinancialYear.ToString();
                    //user.DMRBMFKID = item.DMRBMFKID.ToString() == null ? "" : item.DMRBMFKID.ToString();



                    userList.Add(user);
                }
                var customerList = userList;
                pdf.ExportPDF(customerList, new string[] { "Parameters", "Sequenceno", "Dec", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "YTD", "FinancialYear" }, path);
                return File(path, "application/pdf", "TrackSheetForDMRBM.pdf");

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public class TrackSheetForDMRBMPDF
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
        /*Tracksheet */
        public ActionResult Tracksheet()
        {
            return View();
        }
        public JsonResult JsonLoadDrpPSOName()
        {

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var item = (from a in dcReports.GetPsoNameForAdmin() select new { a.PKID, a.EmpName }).ToList().Distinct();
            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }
        /*Sample Status Report For DM */
        public ActionResult SampleStatusReportForDM()
        {

            return View();
        }

        public JsonResult GridSampleStatusReportForDM(GridQueryModel gridQueryModel, string Products, string Year, string Month)
        {
            try
            {
                DataSet ds = new DataSet();
                string xmlData = "";

                //xml = ProductsXmlPSO(Products);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                int? PsoFkid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                int? Territory_FKID = Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());

                ds = bLayer.GetSampleStatusReportByDMPSOWise(PsoFkid, Territory_FKID, Products, Convert.ToInt32(Month), Convert.ToInt32(Year), xmlData);

                var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_SampleStatusReportForDM
                {
                    ProductName = dataRow[0].ToString(),
                    SamplePack = dataRow[1].ToString(),
                    OpeningBalance = dataRow[2].ToString(),
                    Received = dataRow[3].ToString(),
                    Disbursed = dataRow[4].ToString(),
                    ClosingBalance = dataRow[5].ToString(),


                }
                ).ToList();

                var query = (from a in item select new { a.ProductName, a.SamplePack, a.OpeningBalance, a.Received, a.Disbursed, a.ClosingBalance }).ToList();


                var count = query.Count();
                var pageData = query.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);



            }
            catch
            {
                return Json(gridQueryModel);
            }


        }
        public ActionResult ExportSampleStatusReportForDMSToExcel(string Products, string Year, string Month, string Type)
        {
            try
            {
                DataSet ds = new DataSet();
                var xmlData = "";
                //if (context.Count == 0)
                //    return new EmptyResult();
                int? PsoFkid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                int? Territory_FKID = Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());

                ds = bLayer.GetSampleStatusReportByDMPSOWise(Territory_FKID, PsoFkid, Products, Convert.ToInt32(Month), Convert.ToInt32(Year), xmlData);

                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_SampleStatusReportForDM
                {
                    ProductName = dataRow[0].ToString(),
                    SamplePack = dataRow[1].ToString(),
                    OpeningBalance = dataRow[2].ToString(),
                    Received = dataRow[3].ToString(),
                    Disbursed = dataRow[4].ToString(),
                    ClosingBalance = dataRow[5].ToString(),


                }
                ).ToList();

                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                   
                  item.ProductName.ToString()==null?"":item.ProductName.ToString(),
                  item.SamplePack.ToString()==null?"":item.SamplePack.ToString(),
                  item.OpeningBalance.ToString()==null?"":item.OpeningBalance.ToString(),
                  item.Received.ToString()==null?"":item.Received.ToString(),
                  item.Disbursed.ToString()==null?"":item.Disbursed.ToString(),
                  item.ClosingBalance.ToString()==null?"":item.ClosingBalance.ToString(),
                 
                  
               }));

                return new ExcelResult(HeadersSampleStatusReportForDMS, ColunmTypesSampleStatusReportForDMS, data, "SampleStatusReportForDMS.xlsx", "SampleStatusReportForDMS");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private static readonly string[] HeadersSampleStatusReportForDMS = { "ProductName", "SamplePack", "OpeningBalance", "Received", "Disbursed", "ClosingBalance" };

        private static readonly DataForExcel.DataType[] ColunmTypesSampleStatusReportForDMS = {

        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
       
        
        };
        public ActionResult ExportSampleStatusReportForDMSToPDF(string Products, string Year, string Month, string Type)
        {
            try
            {

                List<SampleStatusReportForDMSPDF> userList = new List<SampleStatusReportForDMSPDF>();
                DataSet ds = new DataSet();
                var xmlData = "";

                int? PsoFkid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                int? Territory_FKID = Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());

                ds = bLayer.GetSampleStatusReportByDMPSOWise(Territory_FKID, PsoFkid, Products, Convert.ToInt32(Month), Convert.ToInt32(Year), xmlData);

                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new RR_SampleStatusReportForDM
                {
                    ProductName = dataRow[0].ToString(),
                    SamplePack = dataRow[1].ToString(),
                    OpeningBalance = dataRow[2].ToString(),
                    Received = dataRow[3].ToString(),
                    Disbursed = dataRow[4].ToString(),
                    ClosingBalance = dataRow[5].ToString(),


                }
                ).ToList();
                foreach (var item in context)
                {
                    SampleStatusReportForDMSPDF user = new SampleStatusReportForDMSPDF();

                    user.ProductName = item.ProductName.ToString() == null ? "" : item.ProductName.ToString();
                    user.SamplePack = item.SamplePack.ToString() == null ? "" : item.SamplePack.ToString();
                    user.OpeningBalance = item.OpeningBalance.ToString() == null ? "" : item.OpeningBalance.ToString();
                    user.Received = item.Received.ToString() == null ? "" : item.Received.ToString();
                    user.Disbursed = item.Disbursed.ToString() == null ? "" : item.Disbursed.ToString();
                    user.ClosingBalance = item.ClosingBalance.ToString() == null ? "" : item.ClosingBalance.ToString();
                    userList.Add(user);
                }
                var customerList = userList;
                pdf.ExportPDF(customerList, new string[] { "ProductName", "SamplePack", "OpeningBalance", "Received", "Disbursed", "ClosingBalance" }, path);
                return File(path, "application/pdf", "SampleStatusReportForDMSPDF.pdf");

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public class SampleStatusReportForDMSPDF
        {
            public string ProductName { get; set; }
            public string SamplePack { get; set; }
            public string OpeningBalance { get; set; }
            public string Received { get; set; }
            public string Disbursed { get; set; }
            public string ClosingBalance { get; set; }

        }
        public JsonResult JsontestForSSDM(string Products, string Year, string Month, string PSOName)
        {
            string xmlData = "";
            xmlData = ProductsXmlDM(PSOName);
            string Retrslt = "";
            string PsoFkid = Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString();
            string Territory_FKID = Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString();
            DataSet ds = new DataSet();
            Retrslt = bLayer.GetSampleStatusReportByDMPSOWiseDetails(Territory_FKID, PsoFkid, Products, Month, Year, xmlData);

            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "SampleStatusRptDMPsoWise.xsl";

            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult JsontestForHPSO(string SampleFKID, string PsoId, string TerriTory, string Month, string Year, string Products)
        {
            string Retrslt = "";
            string Flag = "1";
            DataSet ds = new DataSet();
            Retrslt = bLayer.GetSampleStatusReportByDMPSOWiseDetails1(TerriTory, PsoId, Products, Month, Year, SampleFKID, Flag);
            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "SampleStatusRptPSORec.xsl";
            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal1'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;

            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult JsontestForHPSODis(string SampleFKID, string PsoId, string TerriTory, string Month, string Year, string Products)
        {

            string Retrslt = "";
            DataSet ds = new DataSet();
            string Flag = "2";
            Retrslt = bLayer.GetSampleStatusReportByDMPSOWiseDetails1(TerriTory, PsoId, Products, Month, Year, SampleFKID, Flag);

            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "SampleStatusRptPSODisbursed.xsl";

            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal2);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal2'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal2);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }

        //History Report Summery
        public JsonResult JsontestForHistoryRS(string PSO, string YEAR, string Month, string CType)
        {
            string Retrslt = "";
            string PKID = Session["USER_PKID"] == null ? "0" : Session["USER_PKID"].ToString();
            Retrslt = bLayer.GetHistoryReportSummeryForDoctor(PKID, Month, YEAR, CType);
            XmlTransform trans = new XmlTransform();
            if (CType == "2")
            {

                trans.XslFile = xslPath + "HistorySummeryReoport.xsl";
            }
            else
            {


                trans.XslFile = xslPath + "ChemistHistorySummeryReoport.xsl";
            }
            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal' ><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }
        /*Sample Status Report For RBM*/
        public ActionResult SampleStatusReportForRBM()
        {
            return View();
        }
        public JsonResult JsonLoadlstDMS()
        {

            int USER_FKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetDMNameByReport(USER_FKID) select new { a.PKID, a.EmpName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JsonLoadlstPSOByDM(string DMName)
        {

            string DMFKID = DMName.Remove(DMName.Length - 1, 1);
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in dcReports.GetPsoNameByReportmul(DMFKID) select new { a.PKID, a.EmpName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);
        }
        public string XmlDM(string lstPSOName)
        {

            string SelName = lstPSOName.Remove(lstPSOName.Length - 1, 1);

            string[] arrayName = SelName.Split(',');
            StringBuilder sb = new StringBuilder();

            sb.Append("<row>");
            for (int i = 0; i < arrayName.Count(); i++)
            {

                sb.Append("<dp DMFKID='" + arrayName[i]);
                sb.Append("'/>");


            }
            sb.Append("</row>");
            return sb.ToString();
        }
        public JsonResult JsontestForSRBMS(string Products, string Year, string Month, string DMName, string PSOName)
        {
            string xmlData = "";
            string xmlDMData = "";
            string ProductFKID = Products.Remove(Products.Length - 1, 1);
            string Retrslt = "";
            string PsoFkid = Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString();
            string Territory_FKID = Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString();
            DataSet ds = new DataSet();
            //Retrslt = bLayer.SampleStatusReportForRBM(Territory_FKID, PsoFkid, ProductFKID, Month, Year, xmlData, xmlDMData);
            XmlTransform trans = new XmlTransform();
            if (DMName == null && PSOName == null)
            {
                Retrslt = bLayer.SampleStatusReportForRBM(Territory_FKID, PsoFkid, ProductFKID, Month, Year, xmlData, xmlDMData);
                trans.XslFile = xslPath + "SampleStatusRptRBM.xsl";
            }
            else if (DMName != null && PSOName == null)
            {

                xmlDMData = XmlDM(DMName);
                Retrslt = bLayer.SampleStatusReportForRBM(Territory_FKID, PsoFkid, ProductFKID, Month, Year, xmlData, xmlDMData);
                trans.XslFile = xslPath + "SampleStatusRptRBM_DMwise.xsl";
            }
            else
            {
                //xmlDMData = XmlDM(DMName);
                xmlDMData = "";
                xmlData = ProductsXmlDM(PSOName);
                Retrslt = bLayer.SampleStatusReportForRBM1(Territory_FKID, PsoFkid, ProductFKID, Month, Year, xmlData);
                trans.XslFile = xslPath + "SampleStatusRptRBM_PsoWise.xsl";

            }
            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }
        /* SamplestatusReportForPSO */
        #region SamplestatusReportForPSO
        public ActionResult SamplestatusReportForPSO()
        {
            return View();
        }
        public JsonResult JsontestForSSPSO(string Products, string Year, string Month)
        {

            string Retrslt = "";
            string PsoFkid = Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString();
            string Territory_FKID = Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString();
            DataSet ds = new DataSet();
            Retrslt = bLayer.GetSampleStatusReportBySSPSO(Territory_FKID, PsoFkid, Products, Month, Year);
            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "SampleStatusReportforPSO.xsl";

            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult JsontestForREPSO(string SampleFKID, string Month, string Year, string Products)
        {
            string Retrslt = "";

            DataSet ds = new DataSet();
            string PSOFKID = Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString();
            string TerriotoryFKID = Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString();
            Retrslt = bLayer.GetSampleStatusReportBySSPSORecieved(TerriotoryFKID, PSOFKID, Products, Month, Year, SampleFKID);
            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "SampleStatusRptPSORec.xsl";
            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal1'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal1);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;

            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult JsontestForDISPSO(string SampleFKID, string Month, string Year, string Products)
        {

            string Retrslt = "";
            DataSet ds = new DataSet();
            string PSOFKID = Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString();
            string TerriotoryFKID = Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString();
            Retrslt = bLayer.GetSampleStatusReportBySSPSODIS(TerriotoryFKID, PSOFKID, Products, Month, Year, SampleFKID);

            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "SampleStatusRptPSODisbursed.xsl";

            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal2);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal2'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal2);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }

        #endregion
        /*PSOWisePrescriptionTracker*/
        #region PSOWisePrescriptionTracker
        public ActionResult PSOWisePrescriptionTracker()
        {
            return View();
        }

        public JsonResult JsontestForPSOPRescription(string Products, string FrmDate, string ToDate, string PSOName)
        {

            string Retrslt = "";
            string PsoFkid = Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString();
            string Territory_FKID = Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString();
            string PSONameFKID = PSOName.Remove(PSOName.Length - 1, 1);
            DataSet ds = new DataSet();
            Retrslt = bLayer.GetPSOWisePrescription(FrmDate, ToDate, PSONameFKID, Products);
            XmlTransform trans = new XmlTransform();
            trans.XslFile = xslPath + "PrescriptionTrackerReport.xsl";

            trans.XmlString = Retrslt;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);
            rslt = "<table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='75%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='right' class='tb-r1' ><a onclick='DownloadExcel();' >Download Excel</a></td></tr></table><br/><table id='tblFinal'><tr><td>" + rslt + "</td></tr></table><br/><table align=center'width=100%' cellspacing=''1' cellpadding='0'class='inner_tb' border='0'><tr><td align='right' class='tb-r1' width='50%' ><a onclick='Print(tblFinal);'>Print</a></td><td align='left' class='tb-r1' ><a onclick='DownloadExcel();'>&nbsp;Download Excel</a></td></tr></table>";
            Session["tblExcel"] = rslt;
            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);


        }
        #endregion


        #endregion

        #region Nagarajan

        ////Chemist Prescription Tracker  for RBM
        #region Chemist Prescription Tracker For RBM

        public ActionResult ChemistPrescriptionTrackerForRBM()
        {
            return View();
        }

        ////PSO Name Dynamic Load

        public JsonResult JsonGetPSOName()
        {
            List<SelectListItem> UsePSOName = new List<SelectListItem>();
            UsePSOName.Clear();
            var item = (from a in dcReports.GetPsoNameForAdmin() select new { a.PKID, a.EmpName }).ToList();
            if (item.Count() > 0)
            {
                UsePSOName = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }
            return Json(new
            {

                PSOIterm = UsePSOName,
                PSOCount = UsePSOName.Count,
            }, JsonRequestBehavior.AllowGet);

        }
        ////Grid View Control Load
        public JsonResult ChemistPrescriptionTrackDataCount(GridQueryModel gridQueryModel, string PSOName, string Product, string FrmDate, string ToDate)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                DataTable DtChemistPresTrack = new DataTable();

                DtChemistPresTrack = bLayer.GetChemistPresTrackforRBMRep(PSOName, Product, FrmDate, ToDate);
                var ChemistPresCriptionRBM = DtChemistPresTrack.AsEnumerable().Select(dataRow => new ChemistPrescriptionTracker
                {
                    date = dataRow[0].ToString(),
                    PsoName = dataRow[1].ToString(),
                    RegionCode = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    DoctorName = dataRow[4].ToString(),
                    Specialization = dataRow[5].ToString(),
                    ProductName = dataRow[5].ToString(),
                    Productpack = dataRow[6].ToString(),
                    Prescription = dataRow[7].ToString(),
                    Hospital = dataRow[8].ToString(),
                    Stockist = dataRow[9].ToString()
                }).ToList();
                var query = (from a in ChemistPresCriptionRBM select new { a.date, a.PsoName, a.RegionCode, a.TerCode, a.DoctorName, a.Specialization, a.ProductName, a.Productpack, a.Prescription, a.Hospital, a.Stockist }).ToList();
                var Record = query.Count();
                var pageData = query.OrderBy(x => x.PsoName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                Record = query.Count();

                if (gridQueryModel.sidx == "date" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.date ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "date" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.date descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PsoName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.PsoName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PsoName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.PsoName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "RegionCode" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.RegionCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "RegionCode" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.RegionCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.DoctorName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.DoctorName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.Specialization descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.Specialization descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.ProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.ProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Productpack" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.Productpack descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Productpack" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.Productpack descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Prescription" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.Prescription descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Prescription" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.Prescription descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Hospital" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.Hospital descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Hospital" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.Hospital descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Stockist" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.Stockist descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Stockist" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.Stockist descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = Record,
                    rows = pageData,
                    total = Math.Ceiling((decimal)Record / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);

            }
            catch
            {
                return Json(gridQueryModel);
            }

        }
        ////Excel Download 

        public ActionResult ExportChemistPrescriptionTrackRepExcel(string PSOName, string Product, string FrmDate, string ToDate)
        {

            DataTable DtChemistPresTrack = new DataTable();

            DtChemistPresTrack = bLayer.GetChemistPresTrackforRBMRep(PSOName, Product, FrmDate, ToDate);
            var ChemistPresCriptionRBM = DtChemistPresTrack.AsEnumerable().Select(dataRow => new ChemistPrescriptionTracker
            {
                date = dataRow[0].ToString(),
                PsoName = dataRow[1].ToString(),
                RegionCode = dataRow[2].ToString(),
                TerCode = dataRow[3].ToString(),
                DoctorName = dataRow[4].ToString(),
                Specialization = dataRow[5].ToString(),
                ProductName = dataRow[5].ToString(),
                Productpack = dataRow[6].ToString(),
                Prescription = dataRow[7].ToString(),
                Hospital = dataRow[8].ToString(),
                Stockist = dataRow[9].ToString()
            }).ToList();
            var query = (from a in ChemistPresCriptionRBM select new { a.date, a.PsoName, a.RegionCode, a.TerCode, a.DoctorName, a.Specialization, a.ProductName, a.Productpack, a.Prescription, a.Hospital, a.Stockist }).ToList();
            //if (query.Count == 0)
            //    return new EmptyResult();
            var data = new List<string[]>(query.Count);

            data.AddRange(query.Select(item => new[] 
           {
             item.date,
             item.PsoName,
             item.RegionCode,
             item.TerCode,
             item.DoctorName, 
             item.ProductName,
             item.Productpack,
             item.Prescription,
             item.Hospital,
             item.Stockist,
             
           }));

            return new ExcelResult(HeadersChemistPresTrackRBM, ColunmTypesChemistPresTrackRBM, data, "ChemistPrescriptionTrack.xlsx", "Chemist Prescription Tracker Reports");

        }

        private static readonly string[] HeadersChemistPresTrackRBM = {
                "Date", "Pso Name", "Region Code", "Territory Code", "Doctor Name","Specialization", "Product Name","ProductPack Name","Prescription","Hospital","Stockist"
              };
        private static readonly DataForExcel.DataType[] ColunmTypesChemistPresTrackRBM = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
               
            };

        public class ChemistPrescriptionTackPDF
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

        /////PDF Download 

        public ActionResult ExportChemistPrescriptionTrackRepToPDF(string PSOName, string Product, string FrmDate, string ToDate)
        {

            List<ChemistPrescriptionTackPDF> UseList = new List<ChemistPrescriptionTackPDF>();

            DataTable DtChemistPresTrack = new DataTable();

            DtChemistPresTrack = bLayer.GetChemistPresTrackforRBMRep(PSOName, Product, FrmDate, ToDate);
            var ChemistPresCriptionRBM = DtChemistPresTrack.AsEnumerable().Select(dataRow => new ChemistPrescriptionTracker
            {
                date = dataRow[0].ToString(),
                PsoName = dataRow[1].ToString(),
                RegionCode = dataRow[2].ToString(),
                TerCode = dataRow[3].ToString(),
                DoctorName = dataRow[4].ToString(),
                Specialization = dataRow[5].ToString(),
                ProductName = dataRow[5].ToString(),
                Productpack = dataRow[6].ToString(),
                Prescription = dataRow[7].ToString(),
                Hospital = dataRow[8].ToString(),
                Stockist = dataRow[9].ToString()
            }).ToList();

            var query = (from a in ChemistPresCriptionRBM select new { a.date, a.PsoName, a.RegionCode, a.TerCode, a.DoctorName, a.Specialization, a.ProductName, a.Productpack, a.Prescription, a.Hospital, a.Stockist }).ToList();

            foreach (var item in query)
            {
                ChemistPrescriptionTackPDF use = new ChemistPrescriptionTackPDF();
                use.date = item.date == null ? "" : item.date;
                use.PsoName = item.PsoName == null ? "" : item.PsoName;
                use.RegionCode = item.RegionCode == null ? "" : item.RegionCode;
                use.TerCode = item.TerCode == null ? "" : item.TerCode;
                use.DoctorName = item.DoctorName == null ? "" : item.DoctorName;
                use.ProductName = item.ProductName == null ? "" : item.ProductName;
                use.Productpack = item.Productpack == null ? "" : item.Productpack;
                use.Prescription = item.Prescription == null ? "" : item.Prescription;
                use.Hospital = item.Hospital == null ? "" : item.Hospital;
                use.Stockist = item.Stockist == null ? "" : item.Stockist;
                UseList.Add(use);
            }

            var CustomerList = UseList;

            pdf.ExportPDF(CustomerList, new string[] { "Date", "Pso Name", "Region Code", "Territory Code", "Doctor Name", "Specialization", "Product Name", "ProductPack Name", "Prescription", "Hospital", "Stockist" }, path);
            return File(path, "application/pdf", "ChemistPrescriptionTrackerRBM.pdf");

        }

        //End Page
        #endregion

        //Common Function 
        public JsonResult GetProductData()
        {
            List<SelectListItem> UseProduct = new List<SelectListItem>();
            UseProduct.Clear();
            var item = (from a in dcReports.GetBVOProductByXml() select new { a.PKID, a.ProductName }).ToList();
            if (item.Count() > 0)
            {
                UseProduct = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.ProductName,
                    Value = x.PKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                ProductItems = UseProduct,
                ProductCount = UseProduct.Count,
            }, JsonRequestBehavior.AllowGet);

        }

        //ChemistPrescriptionTrackerReport
        #region Chemist Prescription Tracker For RBM

        //Start Page

        public ActionResult ChemistPrescriptionTracker()
        {
            return View();
        }

        //Grid View Control

        public JsonResult PrescriptionTrackDataCount(GridQueryModel gridQueryModel, string Product, string FrmDate, string ToDate)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                string PSOFKID = Convert.ToString(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string TerriotoryFKID = Convert.ToString(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());

                DataTable DtPreTrack = new DataTable();

                DtPreTrack = bLayer.GetChemistPresTrackRep(PSOFKID, TerriotoryFKID, Product, FrmDate, ToDate);
                var PresCriptionTrack = DtPreTrack.AsEnumerable().Select(dataRow => new PrescriptionTack
                {
                    date = dataRow[0].ToString(),
                    PsoName = dataRow[1].ToString(),
                    RegionCode = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    ChemistName = dataRow[4].ToString(),
                    ProductName = dataRow[5].ToString(),
                    Productpack = dataRow[6].ToString(),
                    Prescription = dataRow[7].ToString()
                }).ToList();
                var query = (from a in PresCriptionTrack select new { a.date, a.PsoName, a.RegionCode, a.TerCode, a.ChemistName, a.ProductName, a.Productpack, a.Prescription }).ToList();
                var Records = query.Count();
                var pageData = query.OrderBy(x => x.PsoName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                Records = query.Count();

                if (gridQueryModel.sidx == "date" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.date ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "date" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.date descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PsoName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.PsoName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PsoName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.PsoName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "RegionCode" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.RegionCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "RegionCode" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.RegionCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ChemistName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.ChemistName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ChemistName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.ChemistName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.ProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.ProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Productpack" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.Productpack descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Productpack" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.Productpack descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Prescription" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.Prescription descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Prescription" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.Prescription descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                return Json(new
                {
                    page = gridQueryModel.page,
                    records = Records,
                    rows = pageData,
                    total = Math.Ceiling((decimal)Records / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }

        }

        //Excel Download 

        public ActionResult ExportPrescriptionTrackRepExcel(string Product, string FrmDate, string ToDate)
        {
            string PSOFKID = Convert.ToString(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            string TerriotoryFKID = Convert.ToString(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());

            DataTable DtPreTrack = new DataTable();

            DtPreTrack = bLayer.GetChemistPresTrackRep(PSOFKID, TerriotoryFKID, Product, FrmDate, ToDate);
            var PresCriptionTrack = DtPreTrack.AsEnumerable().Select(dataRow => new PrescriptionTack
            {
                date = dataRow[0].ToString(),
                PsoName = dataRow[1].ToString(),
                RegionCode = dataRow[2].ToString(),
                TerCode = dataRow[3].ToString(),
                ChemistName = dataRow[4].ToString(),
                ProductName = dataRow[5].ToString(),
                Productpack = dataRow[6].ToString(),
                Prescription = dataRow[7].ToString()
            }).ToList();
            var query = (from a in PresCriptionTrack select new { a.date, a.PsoName, a.RegionCode, a.TerCode, a.ChemistName, a.ProductName, a.Productpack, a.Prescription }).OrderBy(x => x.PsoName).ToList();

            //if (query.Count == 0)

            //    return new EmptyResult();

            var data = new List<string[]>(query.Count);

            data.AddRange(query.Select(item => new[] 
           {
             item.date,
             item.PsoName,
             item.RegionCode,
             item.TerCode,
             item.ChemistName,
             item.ProductName,
             item.Productpack,
             item.Prescription
           }));

            return new ExcelResult(HeadersChemistPresTrack, ColunmTypesChemistPresTrack, data, "ChemistPrescriptionTrack.xlsx", "Chemist Prescription Tracker Reports");

        }

        private static readonly string[] HeadersChemistPresTrack = {
                "Date", "Pso Name", "Region Code", "Territory Code", "Chemist Name", "Product Name","ProductPack Name","Prescription"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesChemistPresTrack = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
                DataForExcel.DataType.String 
            };

        //PDF Download 

        public class PrescriptionTackPDF
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

        public ActionResult ExportPrescriptionTrackRepToPDF(string Product, string FrmDate, string ToDate)
        {

            List<PrescriptionTackPDF> UseList = new List<PrescriptionTackPDF>();
            string PSOFKID = Convert.ToString(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            string TerriotoryFKID = Convert.ToString(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());

            DataTable DtPreTrack = new DataTable();

            DtPreTrack = bLayer.GetChemistPresTrackRep(PSOFKID, TerriotoryFKID, Product, FrmDate, ToDate);
            var PresCriptionTrack = DtPreTrack.AsEnumerable().Select(dataRow => new PrescriptionTack
            {
                date = dataRow[0].ToString(),
                PsoName = dataRow[1].ToString(),
                RegionCode = dataRow[2].ToString(),
                TerCode = dataRow[3].ToString(),
                ChemistName = dataRow[4].ToString(),
                ProductName = dataRow[5].ToString(),
                Productpack = dataRow[6].ToString(),
                Prescription = dataRow[7].ToString()
            }).ToList();
            var query = (from a in PresCriptionTrack select new { a.date, a.PsoName, a.RegionCode, a.TerCode, a.ChemistName, a.ProductName, a.Productpack, a.Prescription }).OrderBy(x => x.PsoName).ToList();

            foreach (var item in query)
            {
                PrescriptionTackPDF use = new PrescriptionTackPDF();
                use.date = item.date == null ? "" : item.date;
                use.PsoName = item.PsoName == null ? "" : item.PsoName;
                use.RegionCode = item.RegionCode == null ? "" : item.RegionCode;
                use.TerCode = item.TerCode == null ? "" : item.TerCode;
                use.ChemistName = item.ChemistName == null ? "" : item.ChemistName;
                use.ProductName = item.ProductName == null ? "" : item.ProductName;
                use.Productpack = item.Productpack == null ? "" : item.Productpack;
                use.Prescription = item.Prescription == null ? "" : item.Prescription;
                UseList.Add(use);
            }

            var CustomerList = UseList;

            pdf.ExportPDF(CustomerList, new string[] { "Date", "Pso Name", "Region Code", "Territory Code", "Chemist Name", "Product Name", "ProductPack Name", "Prescription" }, path);
            return File(path, "application/pdf", "ChemistPrescriptionTrack.pdf");


        }
        //End Page

        #endregion


        #region Customer DM/PM PSO

        public ActionResult NewCustomerALLPSOReportByDM()
        {
            ViewBag.Today = DateTime.Today.ToShortDateString();
            return View();
        }

        public JsonResult JsonGetUserName()
        {
            List<SelectListItem> UseName = new List<SelectListItem>();
            UseName.Clear();
            var item = (from a in dcReports.GetPsoNameByUserId(Convert.ToInt32(@Session["USER_FKID"].ToString())) select new { a.PKID, a.EmpName }).ToList();
            if (item.Count() > 0)
            {
                UseName = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.EmpName,
                    Value = x.PKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                countyItems = UseName,
                countyCount = UseName.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult NewCustomeGridDM(GridQueryModel gridQueryModel, int UserId)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

                var result = dcReports.usp_SELECT_CustPSO_COUNT_BYDM(UserId, Psoid, Roleid).Select(x => new { x.Custtype, x.MCL, x.PCL }).OrderBy(x => x.Custtype).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.PCL).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                count = result.Count();

                if (gridQueryModel.sidx == "Custtype" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.Custtype ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Custtype" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.Custtype descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "MCL" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.MCL ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "MCL" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.MCL descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PCL" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.PCL ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PCL" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.PCL descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);




                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }
        #endregion

        #region Fetch Pso Details
        public JsonResult JsonGetDMHeader(string custType, string flag, string input, string UserIDs)
        {
            string PSO = string.Empty;
            string territory = string.Empty;

            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            var rsltPSO = (from a in dcReports.usp_Get_UserName(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid) select new { a.PSOName, a.Element }).FirstOrDefault();

            var rsltTerritory = (from a in dcReports.usp_Get_CustomerTerritory(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid)
                                 select new { a.TerCode, a.TerName }).FirstOrDefault();
            return Json(new
            {
                PSO = (rsltPSO.PSOName == null) ? "" : rsltPSO.PSOName,
                territory = (rsltTerritory.TerCode == null) ? "" : rsltTerritory.TerCode + " (" + rsltTerritory.TerName + ")",
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region New Customer DM/RBM

        public JsonResult DMCustomerRptGridData(GridQueryModel gridQueryModel, string custType, string flag, string input, string UserIDs)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

                var result = dcReports.usp_DMGetChemistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
                {
                    x.ChemistName,
                    x.AreaName,
                    x.email
                }).OrderBy(x => x.AreaName).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.AreaName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                count = result.Count();

                if (gridQueryModel.sidx == "ChemistName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.ChemistName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ChemistName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.ChemistName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }

        #endregion

        #region Chemist Rpt
        public ActionResult ExportCustomerRptExcelRBM(string custType, string flag, string input, string UserIDs)
        {

            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            var context = dcReports.usp_DMGetChemistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
            {
                x.ChemistName,
                x.AreaName,
                x.email
            }).OrderBy(x => x.AreaName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
               
                item.ChemistName == null ? "" :item.ChemistName,                
                item.AreaName == null ? "" :item.AreaName,
                 item.email == null ? "" :item.email
               
            }));
            return new ExcelResult(HeadersCustomerRptRBM, ColunmTypesCustomerRptRBM, data, "CustomerReport.xlsx", "Customer Report");
        }
        private static readonly string[] HeadersCustomerRptRBM = {
                 "Chemist Name","Area","Email"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesCustomerRptRBM = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,
                 DataForExcel.DataType.String
            };
        public ActionResult ExportCustomerRptToPDFRBM(string custType, string flag, string input, string UserIDs)
        {
            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            List<CustomerRptClsRBM> userList = new List<CustomerRptClsRBM>();

            var context = dcReports.usp_DMGetChemistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
            {
                x.ChemistName,
                x.AreaName,
                x.email
            }).OrderBy(x => x.AreaName).ToList();

            foreach (var item in context)
            {
                CustomerRptClsRBM user = new CustomerRptClsRBM();

                user.ChemistName = item.ChemistName == null ? "" : item.ChemistName;
                user.AreaName = item.AreaName == null ? "" : item.AreaName;
                user.email = item.email == null ? "" : item.email;

                userList.Add(user);
            }
            var customerList = userList;

            var result = new
            {
                total = 1,
                page = 1,
                records = customerList.Count(),
                rows = (
                    customerList.Select(e =>
                        new
                        {

                            cell = new string[]{
                           e.ChemistName,e.AreaName,e.email
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "ChemistName", "AreaName", "email" }, path);
            return File(path, "application/pdf", "CustomerReport.pdf");
        }

        public class CustomerRptClsRBM
        {
            public string ChemistName { get; set; }
            public string AreaName { get; set; }
            public string email { get; set; }
        }
        #endregion

        #region Doctor Rpt

        public JsonResult DMDocotorRptGridData(GridQueryModel gridQueryModel, string custType, string flag, string input, string UserIDs)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

                var result = dcReports.usp_DMGetDoctorRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
                {
                    DoctorCode = (x.DoctorCode == null) ? 0 : x.DoctorCode,
                    DoctorName = (x.DoctorName == null) ? "" : x.DoctorName,
                    Specialization = (x.Specialization == null) ? "" : x.Specialization,
                    AreaName = (x.AreaName == null) ? "" : x.AreaName,
                    KOLFlag = (x.KOLFlag == null) ? "" : x.KOLFlag,
                    RegistrationNo = (x.RegistrationNo == null) ? "" : x.RegistrationNo,
                    email = (x.email == null) ? "" : x.email,
                }).OrderBy(x => x.AreaName).ToList();



                var count = result.Count();
                var pageData = result.OrderBy(x => x.AreaName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                count = result.Count();

                if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorCode" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.DoctorCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorCode" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.DoctorCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.DoctorName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.DoctorName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.Specialization ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.Specialization descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "KOLFlag" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.KOLFlag ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "KOLFlag" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.KOLFlag descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "RegistrationNo" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.RegistrationNo ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "RegistrationNo" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.RegistrationNo descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }


        public ActionResult ExportDocotorRptExcelRBM(string custType, string flag, string input, string UserIDs)
        {
            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            var context = dcReports.usp_DMGetDoctorRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
            {
                DoctorCode = (x.DoctorCode == null) ? 0 : x.DoctorCode,
                DoctorName = (x.DoctorName == null) ? "" : x.DoctorName,
                Specialization = (x.Specialization == null) ? "" : x.Specialization,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                KOLFlag = (x.KOLFlag == null) ? "" : x.KOLFlag,
                RegistrationNo = (x.RegistrationNo == null) ? "" : x.RegistrationNo,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.AreaName).ToList();

            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
              
                item.DoctorName,
                item.DoctorCode.ToString()=="0"?"":item.DoctorCode.ToString(),
                item.Specialization,
                item.AreaName,
                item.KOLFlag,
                item.RegistrationNo,
                item.email
               
               
            }));
            return new ExcelResult(HeadersDocotorRptRBM, ColunmTypesDocotorRptRBM, data, "DoctorReport.xlsx", "Doctor Report");
        }
        private static readonly string[] HeadersDocotorRptRBM = {
                 "Doctor","Doctor Code","Specialization","Area","KOL","Registration No.","Email"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesDocotorRptRBM = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportDocotorRptToPDFRBM(string custType, string flag, string input, string UserIDs)
        {

            List<DocotorRptClsRBM> userList = new List<DocotorRptClsRBM>();

            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            var context = dcReports.usp_DMGetDoctorRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
            {
                DoctorCode = (x.DoctorCode == null) ? 0 : x.DoctorCode,
                DoctorName = (x.DoctorName == null) ? "" : x.DoctorName,
                Specialization = (x.Specialization == null) ? "" : x.Specialization,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                KOLFlag = (x.KOLFlag == null) ? "" : x.KOLFlag,
                RegistrationNo = (x.RegistrationNo == null) ? "" : x.RegistrationNo,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.AreaName).ToList();

            foreach (var item in context)
            {
                DocotorRptClsRBM user = new DocotorRptClsRBM();

                user.DoctorCode = item.DoctorCode.ToString();
                user.DoctorName = item.DoctorName == null ? "" : item.DoctorName;
                user.Specialization = item.Specialization == null ? "" : item.Specialization;
                user.AreaName = item.AreaName == null ? "" : item.AreaName;
                user.KOLFlag = item.KOLFlag == null ? "" : item.KOLFlag;
                user.RegistrationNo = item.RegistrationNo == null ? "" : item.RegistrationNo;
                user.email = item.email == null ? "" : item.email;

                userList.Add(user);
            }
            var customerList = userList;

            var result = new
            {
                total = 1,
                page = 1,
                records = customerList.Count(),
                rows = (
                    customerList.Select(e =>
                        new
                        {

                            cell = new string[]{
                         e.DoctorName,e.DoctorCode,e.Specialization,e.AreaName,e.KOLFlag,e.RegistrationNo,e.email
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "DoctorName", "DoctorCode", "Specialization", "AreaName", "KOLFlag", "RegistrationNo", "email" }, path);
            return File(path, "application/pdf", "DoctorReport.pdf");
        }

        public class DocotorRptClsRBM
        {
            public string DoctorCode { get; set; }
            public string DoctorName { get; set; }
            public string Specialization { get; set; }
            public string AreaName { get; set; }
            public string KOLFlag { get; set; }
            public string RegistrationNo { get; set; }
            public string email { get; set; }
        }

        #endregion Doctor Rpt

        #region Hospital Rpt

        public JsonResult DMHospitalRptGridData(GridQueryModel gridQueryModel, string custType, string flag, string input, string UserIDs)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());
                var result = dcReports.usp_DMGetHospitalRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
                {
                    NameofHospital = (x.NameofHospital == null) ? "" : x.NameofHospital,
                    CityName = (x.CityName == null) ? "" : x.CityName,
                    AreaName = (x.AreaName == null) ? "" : x.AreaName,
                    email = (x.email == null) ? "" : x.email,
                }).OrderBy(x => x.NameofHospital).ToList();



                var count = result.Count();
                var pageData = result.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                count = result.Count();

                if (gridQueryModel.sidx == "NameofHospital" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.NameofHospital ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "NameofHospital" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.NameofHospital descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.CityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.CityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }


        public ActionResult ExportHospitalRptExcelRBM(string custType, string flag, string input, string UserIDs)
        {
            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            var context = dcReports.usp_DMGetHospitalRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
            {
                NameofHospital = (x.NameofHospital == null) ? "" : x.NameofHospital,
                CityName = (x.CityName == null) ? "" : x.CityName,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.NameofHospital).ToList();


            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
              
                item.NameofHospital,
                item.CityName, 
                item.AreaName, 
                item.email
               
               
            }));
            return new ExcelResult(HeadersHospitalRptRBM, ColunmTypesHospitalRptRBM, data, "HospitalReport.xlsx", "Hospital Report");
        }
        private static readonly string[] HeadersHospitalRptRBM = {
                 "Hospital Name","City","Area","Email"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesHospitalRptRBM = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
                DataForExcel.DataType.String 
            };
        public ActionResult ExportHospitalRptToPDFRBM(string custType, string flag, string input, string UserIDs)
        {
            List<HospitalRptClsRBM> userList = new List<HospitalRptClsRBM>();

            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            var context = dcReports.usp_DMGetHospitalRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
            {
                NameofHospital = (x.NameofHospital == null) ? "" : x.NameofHospital,
                CityName = (x.CityName == null) ? "" : x.CityName,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.NameofHospital).ToList();

            foreach (var item in context)
            {
                HospitalRptClsRBM user = new HospitalRptClsRBM();

                user.HospitalName = item.NameofHospital.ToString();
                user.City = item.CityName;
                user.AreaName = item.AreaName;
                user.email = item.email;
                userList.Add(user);
            }
            var customerList = userList;

            var result = new
            {
                total = 1,
                page = 1,
                records = customerList.Count(),
                rows = (
                    customerList.Select(e =>
                        new
                        {

                            cell = new string[]{
                         e.HospitalName,e.City,e.AreaName,e.email
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "HospitalName", "City", "AreaName", "AreaName", "email" }, path);
            return File(path, "application/pdf", "HospitalReport.pdf");
        }

        public class HospitalRptClsRBM
        {
            public string HospitalName { get; set; }
            public string City { get; set; }
            public string AreaName { get; set; }
            public string email { get; set; }
        }


        #endregion

        #region Stockist Rpt

        public JsonResult DMStockistRptGridData(GridQueryModel gridQueryModel, string custType, string flag, string input, string UserIDs)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

                var result = dcReports.usp_DMGetStockistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
                {
                    NameofStockist = (x.NameofStockist == null) ? "" : x.NameofStockist,
                    CityName = (x.CityName == null) ? "" : x.CityName,
                    AreaName = (x.AreaName == null) ? "" : x.AreaName,
                    email = (x.email == null) ? "" : x.email,
                }).OrderBy(x => x.NameofStockist).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                count = result.Count();

                if (gridQueryModel.sidx == "NameofStockist" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.NameofStockist ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "NameofStockist" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.NameofStockist descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.CityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.CityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "email" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,
                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(gridQueryModel);
            }
        }


        public ActionResult ExportStockistRptExcelRBM(string custType, string flag, string input, string UserIDs)
        {

            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            var context = dcReports.usp_DMGetStockistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
            {
                NameofStockist = (x.NameofStockist == null) ? "" : x.NameofStockist,
                CityName = (x.CityName == null) ? "" : x.CityName,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.NameofStockist).ToList();


            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
              
                item.NameofStockist,
                item.CityName, 
                item.AreaName, 
                item.email
               
               
            }));
            return new ExcelResult(HeadersStockistRptRBM, ColunmTypesStockistRptRBM, data, "StockiestReport.xlsx", "Stockiest Report");
        }
        private static readonly string[] HeadersStockistRptRBM = {
                 "Stockist Name","City","Area","Email"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesStockistRptRBM = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String,
                DataForExcel.DataType.String 
            };
        public ActionResult ExportStockistRptToPDFRBM(string custType, string flag, string input, string UserIDs)
        {
            List<StockistRptClsRBM> userList = new List<StockistRptClsRBM>();

            Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            Int32 Roleid = Convert.ToInt32(Session["NodeType_FKID"] == null ? "0" : Session["NodeType_FKID"].ToString());

            var context = dcReports.usp_DMGetStockistRpt(Convert.ToDecimal(input), custType, flag, Convert.ToDecimal(Psoid), Convert.ToInt32(UserIDs), Roleid).Select(x => new
            {
                NameofStockist = (x.NameofStockist == null) ? "" : x.NameofStockist,
                CityName = (x.CityName == null) ? "" : x.CityName,
                AreaName = (x.AreaName == null) ? "" : x.AreaName,
                email = (x.email == null) ? "" : x.email,
            }).OrderBy(x => x.NameofStockist).ToList();

            foreach (var item in context)
            {
                StockistRptClsRBM user = new StockistRptClsRBM();

                user.StockistName = item.NameofStockist.ToString();
                user.City = item.CityName;
                user.AreaName = item.AreaName;
                user.email = item.email;
                userList.Add(user);
            }
            var customerList = userList;

            var result = new
            {
                total = 1,
                page = 1,
                records = customerList.Count(),
                rows = (
                    customerList.Select(e =>
                        new
                        {

                            cell = new string[]{
                         e.StockistName,e.City,e.AreaName,e.email
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "StockistName", "City", "AreaName", "AreaName", "email" }, path);
            return File(path, "application/pdf", "StockiestReport.pdf");
        }

        public class StockistRptClsRBM
        {
            public string StockistName { get; set; }
            public string City { get; set; }
            public string AreaName { get; set; }
            public string email { get; set; }
        }

        #endregion

        #endregion

    }

}
