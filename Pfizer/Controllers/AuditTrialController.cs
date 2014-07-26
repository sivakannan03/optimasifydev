using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SifyGrid;
using SifyGrid.GridHelpers;
using PfizerEntity;
using System.Data.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;

namespace Pfizer.Controllers
{
    public class AuditTrialController : Controller
    {
        private pfizerEntities AuditData = new pfizerEntities();
        ExportPdf pdf = new ExportPdf();
        int Startyr = Convert.ToInt32(ConfigurationManager.AppSettings["StartYear"].ToString());
        string path = ConfigurationManager.AppSettings["PDFfilePath"].ToString();

        // GET: /AuditTrial/


        #region AuditTrial Report

        public ActionResult AuditTrial()
        {
            return View();
        }


        #region Bind Data To Grid

        public JsonResult GetAuditTrial(GridQueryModel gridQueryModel, string PKID, string FDATE, string TODATE)
        {
            try
            {
                int UPKID = Convert.ToInt32(PKID);
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string TODATES = tdate.ToString("MM/dd/yyyy");

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in AuditData.Searchaudittrial(UPKID,fdate,tdate) select q).ToList();
                //var query = (from a in OtherMasters.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.PageName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "PageName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.PageName != null && sq.PageName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.PageName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.PageName != null && sq.PageName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.PageName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.PageName != null && sq.PageName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.PageName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.PageName != null && sq.PageName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.PageName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "USERName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.USERName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "USERName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.USERName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "PageName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.PageName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "PageName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.PageName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "DateIn" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DateIn descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "DateIn" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.DateIn ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "TimeIn" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TimeIn descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "TimeIn" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.TimeIn ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "DateOut" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DateOut descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "DateOut" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.DateOut ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "TimeOut" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TimeOut descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "TimeOut" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.TimeOut ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "IpAddress" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.IpAddress descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "IpAddress" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.IpAddress ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        public ActionResult ExportAuditTrialToExcel(string PKID, string FDATE, string TODATE)
        {
            try
            {
                int UPKID = Convert.ToInt32(PKID);
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string TODATES = tdate.ToString("MM/dd/yyyy");

                var context = (from q in AuditData.Searchaudittrial(UPKID, fdate, tdate) select q).OrderBy(x => x.PageName).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.USERName == null ? "" : item.USERName,
                 item.PageName == null ? "" :item.PageName,
                 item.DateIn == null ? "" :item.DateIn,
                 item.TimeIn == null ? "" :item.TimeIn,
                 item.DateOut == null ? "" :item.DateOut,
                 item.TimeOut == null ? "" :item.TimeOut,
                 item.IpAddress == null ? "" :item.IpAddress,
             
                
               
            }));
                return new ExcelResult(HeadersAuditTrail, ColunmTypesAuditTrail, data, "AuditTrail.xlsx", "AuditTrail");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersAuditTrail = {
               "UserName","PageName", "DateIn", "TimeIn", "DateOut", "TimeOut", "IpAddress"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesAuditTrail = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportAuditTrialToPDF(string PKID, string FDATE, string TODATE)
        {
            try
            {
                int UPKID = Convert.ToInt32(PKID);
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string TODATES = tdate.ToString("MM/dd/yyyy");

                List<AuditTrail> userList = new List<AuditTrail>();

                var context = (from q in AuditData.Searchaudittrial(UPKID, fdate, tdate) select q).OrderBy(x => x.PageName).ToList();

                foreach (var item in context)
                {
                    AuditTrail user = new AuditTrail();
                    // user.ProductFkid = item.ProductFKID.ToString();
                    user.UserName = item.USERName == null ? "" : item.USERName;
                    user.PageName = item.PageName == null ? "" : item.PageName;
                    user.DateIn = item.DateIn == null ? "" : item.DateIn;
                    user.TimeIn = item.TimeIn == null ? "" : item.TimeIn;
                    user.DateOut = item.DateOut == null ? "" : item.DateOut;
                    user.TimeOut = item.TimeOut == null ? "" : item.TimeOut;
                    user.IpAddress = item.IpAddress == null ? "" : item.IpAddress;
                    userList.Add(user);
                }
                var customerList = userList;


                pdf.ExportPDF(customerList, new string[] { "UserName", "PageName", "DateIn", "TimeIn", "DateOut", "TimeOut", "IpAddress" }, path);
                return File(path, "application/pdf", "AuditTrail.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportAuditTrialToCsv(string PKID, string FDATE, string TODATE)
        {
            try
            {
                int UPKID = Convert.ToInt32(PKID);
                DateTime fdate = DateTime.ParseExact(FDATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string FDATES = fdate.ToString("MM/dd/yyyy");
                DateTime tdate = DateTime.ParseExact(TODATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                //string TODATES = tdate.ToString("MM/dd/yyyy");

                var model = (from q in AuditData.Searchaudittrial(UPKID,fdate,tdate) select q).OrderBy(x => x.PageName).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("User Name,Page Name,DateIn,TimeIn,DateOut,TimeOut,IpAddress");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6}", record == null ? "" : record.USERName, record == null ? "" : record.PageName, record == null ? "" : record.DateIn, record == null ? "" : record.TimeIn, record == null ? "" : record.DateOut, record == null ? "" : record.TimeOut, record == null ? "" : record.IpAddress

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "AuditTrail.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class AuditTrail
        {
            public string UserName { get; set; }
            public string PageName { get; set; }
            public string DateIn { get; set; }
            public string TimeIn { get; set; }
            public string DateOut { get; set; }
            public string TimeOut { get; set; }
            public string IpAddress { get; set; }

        }

        #endregion


        #endregion

    }
}
