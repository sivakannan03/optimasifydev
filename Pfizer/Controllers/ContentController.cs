using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using PfizerEntity;
using System.Data.Entity.Core.Objects;
using System.Web.Script.Serialization;
using System.Globalization;
using SifyGrid;
using SifyGrid.GridHelpers;
using System.Configuration;

namespace Pfizer.Controllers
{
    public class ContentController : Controller
    {
        ExportPdf pdf = new ExportPdf();
        pfizerEntities Content = new pfizerEntities();
        string path = ConfigurationSettings.AppSettings["PDFfilePath"].ToString();
        //
        // GET: /Content/
        #region Notification Master
        public ActionResult NotificationMaster()
        {
            return View();
        }
        public JsonResult GetNotificationMaster(GridQueryModel gridQueryModel)
        {
            try
            {


                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in Content.GetNotification() select q).ToList();
                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "Notification") || (searchField == "ExpiryDate"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.Notification!=null && sq.Notification.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Notification).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.Notification != null && sq.Notification.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Notification).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.Notification != null && sq.Notification.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Notification).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.Notification != null && sq.Notification.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Notification).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "Notification" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Notification descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Notification" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Notification ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                }

                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,

                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);


            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult NotificationMasterAdd(string Notification, string ExpiryDate, string Status)
        {
            try { 
                string msg = string.Empty;
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }

                var rslt = (from a in Content.Notifications where a.Notification1 == Notification select a).ToList();
                if (rslt.Count() > 0)
                  {
                      msg = "Notification Master already Exists";
                  }
                  else
                  {
                      Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                      //DateTime Edate = DateTime.Parse(ExpiryDate);
                      DateTime Edate = DateTime.ParseExact(ExpiryDate,"dd/MM/yyyy",CultureInfo.InvariantCulture);
                      int result = Content.AddNotification1(Notification,Edate,ModifiedBy);
                      if (result == 1) 
                      {
                          msg = "Notification Master Added Successfully.";
                      }
                                         
                  }
                return Json(msg);
              
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        public JsonResult EditNotificationMaster(string id, string oper, string PKID, string Notification, string ExpiryDate, string Status)
        {
            try
            {
                string msg = string.Empty;
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }
                if (oper == "edit")
                {
                               
                    Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    DateTime Edate = DateTime.ParseExact(ExpiryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    Decimal ePKID = Convert.ToDecimal(id);
                    int result = Content.EditNotification(ePKID, Notification, Edate, Convert.ToBoolean(Status), ModifiedBy);
                    if (result == 1)
                    {
                        msg = "Notification Master Updated Successfully.";
                    }

                }
               if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    int result = Content.DeleteNotification(quote);
                    if (result == 1)
                    {
                        msg = "Notification Master deleted Successfully.";
                    }

                }

                return Json(msg);
                // return Json(new { success = false, showMessage = true, message = msg, title = "Error" });
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportNotificationmMasterToExcel()
        {
            try
            {
                var context = (from q in Content.GetNotification() orderby q.Notification ascending select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                //item.PKID.ToString(),
                item.Notification==null?"":item.Notification,
                item.ExpiryDate==null?"":item.ExpiryDate,
                item.Status
               
            }));
                return new ExcelResult(HeadersNotificationMaster, ColunmTypesNotification, data, "NotificationMaster.xlsx", "NotificationMaster");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersNotificationMaster = {
                "Notification", "ExpiryDate", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesNotification = {
                //DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportNotificationMasterToPDF()
        {
            try
            {
                List<NotifiMaster> userList = new List<NotifiMaster>();
                var context = (from q in Content.GetNotification() orderby q.Notification ascending select q).ToList();

                foreach (var item in context)
                {
                    NotifiMaster user = new NotifiMaster();
                    user.PKID = item.PKID.ToString();
                    user.Notification = item.Notification==null?"":item.Notification;
                    user.ExpiryDate = item.ExpiryDate==null?"":item.ExpiryDate;
                    user.Status = item.Status;
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
                                id = e.PKID,
                                cell = new string[]{
                            e.PKID.ToString(),e.Notification,e.ExpiryDate,e.Status
                        }
                            })
                    ).ToArray()
                };
                
                pdf.ExportPDF(customerList, new string[] { "Notification", "ExpiryDate", "Status" }, path);
                return File(path, "application/pdf", "NotificationMaster.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class NotifiMaster
        {
            public string PKID { get; set; }
            public string Notification { get; set; }
            public string ExpiryDate { get; set; }
            public string Status { get; set; }
        }
        public ActionResult ExportNotificationMasterToCsv()
        {
            try
            {
                var model = (from q in Content.GetNotification() orderby q.Notification ascending select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Notification,ExpiryDate,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2}",record.Notification==null?"":record.Notification, record.ExpiryDate==null?"":record.ExpiryDate, record.Status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);
                // return File(csvBytes, "text/plain", "NotificationMaster.txt");
                return File(csvBytes, "text/csv", "NotificationMaster.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        #endregion
        #region Info Category
        public ActionResult InfoCategory()
        {
            return View();
        }
        public JsonResult GetInfoCategory(GridQueryModel gridQueryModel)
        {
            try
            {


                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in Content.GetInfoCategory() select q).ToList();
                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "ParentHeading") || (searchField == "ChildHeading"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.ParentHeading != null && sq.ParentHeading.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.ChildHeading.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ParentHeading).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.ParentHeading != null && sq.ParentHeading.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.ChildHeading.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ParentHeading).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.ParentHeading != null && sq.ParentHeading.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.ChildHeading.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ParentHeading).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.ParentHeading!=null && sq.ParentHeading.ToUpper().Contains(searchString) || sq.ChildHeading.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ParentHeading).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "ParentHeading" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.ParentHeading descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ParentHeading" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ParentHeading ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                }

                return Json(new
                {
                    page = gridQueryModel.page,
                    records = count,
                    rows = pageData,

                    total = Math.Ceiling((decimal)count / gridQueryModel.rows)
                }, JsonRequestBehavior.AllowGet);


            }
            catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult AddInfoCategory(string ParentHeading1, string ChildHeading1)
        {
            try {
                string msg = string.Empty;
                //  var Result = string.Empty;
                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                int result = Content.AddInfoCategory(ParentHeading1.TrimStart(), Convert.ToDecimal(ChildHeading1), "1", ModifiedBy);
                if (result == 1)
                    {
                        msg = "InfoCategory Added Successfully.";
                    }
               return Json(msg);
                }
            catch (Exception ex)
            {
                return Json("InfoCategory already Exists");
            }
        }

        public JsonResult EditInfoCategory(string id, string oper, string ParentId, string ParentHeading, string ChildHeading, string Status)
        {
            try
            {

                string msg = string.Empty;
                if (Status == "Active")
                {
                    Status = "1";
                }
                else
                {
                    Status = "0";
                }
                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                if (oper == "edit")
                {
                    Decimal ePKID = Convert.ToDecimal(id);
                    // var qry = (from q in ManageUser.LocationMasters select q).ToList();
                    int result = Content.EditInfoCategory(ePKID, ParentHeading.TrimStart(), Convert.ToDecimal(ChildHeading), "1", ModifiedBy);

                    if (result == 1)
                    {
                        msg = "Info Category Updated Successfully.";
                    }

                }
              
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    int result = Content.DeleteInfoCategory(quote, Status, ModifiedBy);
                    if (result == 1)
                    {
                        msg = "Info Category deleted Successfully.";
                    }

                }

                return Json(msg);
                // return Json(new { success = false, showMessage = true, message = msg, title = "Error" });
            }
            catch(Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportInfoCategoryToExcel()
        {
            try
            {
                var context = (from q in Content.GetInfoCategory()orderby q.ParentHeading ascending select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.Parentid.ToString()==null?"":item.Parentid.ToString(),
                item.ParentHeading==null?"":item.ParentHeading.ToString(),
                item.ChildHeading==null?"":item.ChildHeading.ToString(),
                item.Status
               
            }));
                return new ExcelResult(HeadersInfoCategory, ColunmTypesInfoCategory, data, "InfoCategory.xlsx", "InfoCategory");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersInfoCategory = {
                "ParentId", "ParentHeading", "ChildHeading", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesInfoCategory = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportInfoCategoryToPDF()
        {
            try
            {
                List<InfoBankCategory> userList = new List<InfoBankCategory>();
                var context = (from q in Content.GetInfoCategory() orderby q.ParentHeading ascending select q).ToList();

                foreach (var item in context)
                {
                    InfoBankCategory user = new InfoBankCategory();
                    user.ParentId = item.Parentid.ToString()==null?"":item.Parentid.ToString();
                    user.ParentHeading = item.ParentHeading == null ? "" : item.ParentHeading.ToString(); ;
                    user.ChildHeading = item.ChildHeading == null ? "" : item.ChildHeading.ToString(); ;
                    user.Status = item.Status;
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
                                id = e.ParentId,
                                cell = new string[]{
                            e.ParentId.ToString(),e.ParentHeading,e.ChildHeading,e.Status
                        }
                            })
                    ).ToArray()
                };
                //var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Sample2.pdf");
                //pdf.ExportPDF(customerList, new string[] { "ParentId", "ParentHeading", "ChildHeading", "Status" }, path);
                //return File(path, "application/pdf", "InfoCategory.pdf");


                pdf.ExportPDF(customerList, new string[] { "ParentId", "ParentHeading", "ChildHeading", "Status" }, path);
                return File(path, "application/pdf", "InfoCategory.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class InfoBankCategory
        {
            public string ParentId { get; set; }
            public string ParentHeading { get; set; }
            public string ChildHeading { get; set; }
            public string Status { get; set; }
        }
        public ActionResult ExportInfoCategoryToCsv()
        {
            try
            {
                var model = (from q in Content.GetInfoCategory() orderby q.ParentHeading ascending select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("ParentId,ParentHeading,ChildHeading,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record.Parentid, record.ParentHeading, record.ChildHeading, record.Status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "InfoCategory.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult SubCategory()
        {
            try
            {
                var query = (from e in Content.buildInfoBank() select new { e.PKID, e.Category }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.Category.ToString());

                return PartialView("SubCategory", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }
        
        #endregion

        /* Rajkumar */

        //Opinion Poll
        public ActionResult OpinionPoll()
        {
            return View();
        }

        public JsonResult OpinionGridData(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                
                var result = Content.OpinionPollViews.Select(x => new
                {

                    PKID = x.PKID,
                    x.Question,
                    x.Option1,
                    x.Option2,                    
                    IsActive = x.IsActive == true ? "Active" : "In Active",
                    ShowRating = "Show Rating"
                }).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "Question")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where sq.Question!=null && sq.Question.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where sq.Question!=null && sq.Question.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where sq.Question != null && sq.Question.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in result where sq.Question != null && sq.Question.ToUpper().ToString().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = result.Count();

                    if (gridQueryModel.sidx == "Question" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.Question ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Question" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.Question descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "Option1" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Option1" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "Option2" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Option2" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                }




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

        public JsonResult AddOpinionGrid(string Question, string IsActive, string SelDivision, string SelNodeType, string SelUserType, string Option1, string Option2, string Option3, string Option4, string Option5)
        {
            try 
            {
                string id = string.Empty;
                int result =Content.AddOpinionPoll(SelUserType, Question, Option1, Option2, Option3, Option4, Option5, Convert.ToDecimal("1"));
                
                if(result==2)
                {
                    id = "Opinion Poll Added Successfully.";
                }
                return Json(id);   
               
            }
           catch (Exception ex)
            {
                return Json ("Opinion Poll already Exists");
            }
        }


        public JsonResult EditOpinionGrid(string id, string oper, string PKID, string Question, string IsActive, string SelDivision, string SelNodeType, string SelUserType, string Option1, string Option2, string Option3, string Option4, string Option5)
        {

            //string xml = string.Empty;
            //string[] arrayUser = SelUserType.Split(',');
            //StringBuilder sb = new StringBuilder();
            //sb.Append("<root>");
            //for (int i = 0; i < arrayUser.Count(); i++)
            //{
            //    sb.Append("<TeamUserLink UserFKID='" + arrayUser[i]);
            //    sb.Append("'/>");

            //}
            //sb.Append("</root>");


            try
            {

                if (oper == "edit")
                {
                    Content.EditOpinionPoll(SelUserType, Question, Option1, Option2, Option3, Option4, Option5, Convert.ToDecimal("1"), Convert.ToDecimal(PKID));
                    id = "Opinion Poll Updated Successfully.";
                }

               if (oper == "del")
                {

                    Content.DeleteOpinionPoll(Convert.ToDecimal(id));
                    id = "Opinion Poll Deleted Successfully.";
                }


                return Json(id);
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }
           
        }

        public JsonResult JsGetOpinionPoll(decimal pkid) {
            string question = "",option1 = "",option2 = "",option3 = "",option4 = "",option5 = "";

            try
            {

                var rslt = (from a in Content.GetOpinionPollByXML(pkid) select new { a.PKID,a.Question,a.Option1,a.Option2,a.Option3,a.Option4,a.Option5 }).FirstOrDefault();

                 
                    return Json(new
                    {
                        question = rslt.Question,
                        option1=rslt.Option1,
                        option2=rslt.Option2,
                        option3=rslt.Option3,
                        option4=rslt.Option4,
                        option5=rslt.Option5

                    }, JsonRequestBehavior.AllowGet);
                
                 
            }


            catch
            {
                return Json(new
                {
                    question = "",
                    option1 = "",
                    option2 = "",
                    option3 = "",
                    option4 = "",
                    option5 = ""

                }, JsonRequestBehavior.AllowGet);

            }
        }


        public JsonResult JsGetUser(string division,string nodeType)
        {
            List<System.Web.Mvc.SelectListItem> retutnUser = new List<System.Web.Mvc.SelectListItem>();
            int count = 0;
            try
            {
                var item = (from a in Content.GetDivUserByXml(nodeType, division) select new { a.PKID, a.UserName }).ToList().OrderBy(O => O.UserName).Distinct();
                   
                    if (item.Count() > 0)
                    {
                        retutnUser = item.Select(x => new System.Web.Mvc.SelectListItem()
                        {
                            Text = x.UserName,
                            Value = x.PKID.ToString()
                        }).ToList();                        
                    }
                    return Json(new
                    {
                        retutnUser = retutnUser,
                        count = retutnUser.Count,
                    }, JsonRequestBehavior.AllowGet);

            }

            catch
            {

            }
            return Json(new
            {
                retutnUser = retutnUser,
                count = retutnUser.Count,
            }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult LoadDivision(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(rowId)) where a.Element == type && a.DivName != "         " select new { a.PKID, a.DivName }).OrderBy(x => x.DivName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            else
                query = (from a in Content.GetDivisionByXML() select new { a.DivFKID, a.DivName }).ToDictionary(f => Convert.ToInt32(f.DivFKID), f => f.DivName.ToString());

            return PartialView("PVLoadDivision", query);
        }

        public ActionResult LoadSelectedDivision(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(rowId)) where a.Element == type && a.DivName != "         " select new { a.PKID, a.DivName }).OrderBy(x => x.DivName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            else
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(0)) where a.DivName=="New" select new { a.PKID, a.DivName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());

            return PartialView("PVLoadDivision", query);
        }


        public ActionResult LoadNode(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(rowId)) where a.Element == type && a.DivName != "    " select new { a.PKID, a.DivName }).OrderBy(x=>x.DivName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            else
                query = (from a in Content.GetNodeTypeByXML()  select new { a.NodeTypeFKID, a.NodeName }).ToDictionary(f => Convert.ToInt32(f.NodeTypeFKID), f => f.NodeName.ToString());

            return PartialView("PVLoadNode", query);
        }

        public ActionResult LoadSelectedNode(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(rowId)) where a.Element == type && a.DivName != "     " select new { a.PKID, a.DivName }).OrderBy(x => x.DivName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            else
                query = (from a in Content.GetNodeTypeByXML() where a.NodeName=="New" select new { a.NodeTypeFKID, a.NodeName }).ToDictionary(f => Convert.ToInt32(f.NodeTypeFKID), f => f.NodeName.ToString());

            return PartialView("PVLoadNode", query);
        }

        public ActionResult LoadUser(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(rowId)) where a.Element == type && a.DivName != "         " select new { a.PKID, a.DivName }).OrderBy(x => x.DivName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            else
                query = (from a in Content.GetUserByXml() select new { a.PKID, a.LoginName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.LoginName.ToString());

            return PartialView("PVLoadDivision", query);
        }

        public ActionResult LoadSelectedUser(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(rowId)) where a.Element == type && a.DivName != "         " select new { a.PKID, a.DivName }).OrderBy(x => x.DivName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            else
                query = (from a in Content.GetUserByXml() where a.LoginName=="New" select new { a.PKID, a.LoginName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.LoginName.ToString());

            return PartialView("PVLoadDivision", query);
        }



        public JsonResult JsonGetRating(string pkid)
        {
            string option1 = "", option2 = "", option3 = "", option4 = "", option5 = "", count1 = "", count2 = "", count3 = "", count4 = "", count5 = "";

            var rslt = (from a in Content.ShowOpinionPollRating1(Convert.ToInt32(pkid)) select new {a.OPTION1,a.OPTION2,a.OPTION3,a.OPTION4,a.OPTION5}).First();


            return Json(new
            {
                option1 = (rslt.OPTION1 == null) ? "" : rslt.OPTION1,
                option2 = (rslt.OPTION2 == null) ? "" : rslt.OPTION2,
                option3 = (rslt.OPTION3 == null) ? "" : rslt.OPTION3,
                option4 = (rslt.OPTION4 == null) ? "" : rslt.OPTION4,
                option5 = (rslt.OPTION5 == null) ? "" : rslt.OPTION5,
                count1 = GetTotalOpinionCount("Option1", rslt.OPTION1, pkid),
                count2 = GetTotalOpinionCount("Option2", rslt.OPTION2, pkid),
                count3 = GetTotalOpinionCount("Option3", rslt.OPTION3, pkid),
                count4 = GetTotalOpinionCount("Option4", rslt.OPTION4, pkid),
                count5 = GetTotalOpinionCount("Option5", rslt.OPTION5, pkid)
                
            }, JsonRequestBehavior.AllowGet);
        }


        public string GetTotalOpinionCount(string columnName,string countLike,string pkId)
        {
          
            var rslt= (from b in Content.GetOpinionPollTotalCount1(columnName,countLike,Convert.ToDecimal(pkId)) select new {b.Value}).First(); 
            if(countLike!="")
            return  rslt.Value.ToString();
            else
                return "";
        }


        //end
        //Opinion Poll Master Excel,PDF and CSV file download

        public ActionResult ExportOpinionMasToExcel()
        {
            try
            {
                var context = (from q in Content.OpinionPollViews orderby q.Question ascending select q).ToList();
                if (context.Count == 0)
                return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.Question.ToString()==null ? "" : item.Question.ToString(),
                item.Option1==null ? "" : item.Option1.ToString(),
                item.Option2==null ? "" : item.Option2.ToString(),
                item.status
                }));
                return new ExcelResult(HeadersOpinion, ColunmTypesOpinion, data, "OpinionMaster.xlsx", "OpinionPoll");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersOpinion = {
                "Question", "Option1", "Option2", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesOpinion = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportOpinionMasToPDF()
        {
            try
            {
                List<OpinionPollCategory> userList = new List<OpinionPollCategory>();
                var context = (from q in Content.OpinionPollViews orderby q.Question ascending select q).ToList();
                foreach (var item in context)
                {
                    OpinionPollCategory user = new OpinionPollCategory();
                    user.Question = item.Question.ToString()==null ? "" : item.Question.ToString();
                    user.Option1 = item.Option1.ToString() == null ? "" : item.Option1.ToString(); ;
                    user.Option2 = item.Option2.ToString() == null ? "" : item.Option2.ToString(); ;
                    user.Status = item.status;
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
                            e.Question.ToString(),e.Option1,e.Option2,e.Status
                        }
                            })
                    ).ToArray()
                };
                pdf.ExportPDF(customerList, new string[] { "Question", "Option1", "Option2", "Status" }, path);
                return File(path, "application/pdf", "OpinionMaster.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class OpinionPollCategory
        {
            public string Question { get; set; }
            public string Option1 { get; set; }
            public string Option2 { get; set; }
            public string Status { get; set; }
        }
        public ActionResult ExportOpinionMasToCsv()
        {
            try
            {
                var model = (from q in Content.OpinionPollViews orderby q.Question ascending select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Question,Option1,Option2,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record.Question, record.Option1, record.Option2, record.status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "OpinionMaster.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }



        //Raj kumar

        #region InfoBankUpLoad
        public ActionResult InfoBankUpLoad()
        {
            return View();
        }



        public JsonResult GridDataInfoBankUpLoad(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in Content.USP_GET_InfoBankUpLoad() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.Category).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "Title"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.Title != null && sq.Title.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Title).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.Title != null && sq.Title.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Title).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.Title != null && sq.Title.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Title).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.Title!=null && sq.Title.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Title).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "Title" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Title descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Title" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Title ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Category" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Category descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Category" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Category ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "KeyWords" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.KeyWords descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "KeyWords" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.KeyWords ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                }

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


        public ActionResult GetCategory()
        {              
            dynamic query = "";
            query = (from a in Content.buildInfoBank() select new { a.PKID, a.Category }).OrderBy(x => x.Category).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.Category.ToString());
            return PartialView("LoadYear", query);
        }


        public ActionResult LoadBankDivision(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(rowId)) where a.Element == type && a.DivName != "         " select new { a.PKID, a.DivName }).OrderBy(x => x.DivName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            else
                query = (from a in Content.GetDivisonMasterByPkid() select new { a.PKID, a.DivName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());

            return PartialView("PVLoadBankDivision", query);
        }


        public ActionResult LoadBankNode(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in Content.GetOpinionPollSelectedValues1(Convert.ToDecimal(rowId)) where a.Element == type && a.DivName != "         " select new { a.PKID, a.DivName }).OrderBy(x => x.DivName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            else
                query = (from a in Content.GetNodeTypeByXML() select new { a.NodeTypeFKID, a.NodeName }).ToDictionary(f => Convert.ToInt32(f.NodeTypeFKID), f => f.NodeName.ToString());

            return PartialView("PVLoadBankDivision", query);
        }


        public JsonResult EditInfoBankUpLoad(string id, string oper, string SelectFile, string Division, string NodeType, string Category, string Title, string KeyWords, FormCollection frm, string fileName)
        {

              try
            {

                if (oper == "edit")
                {
                   // Content.EditOpinionPoll(SelUserType, Question, Option1, Option2, Option3, Option4, Option5, Convert.ToDecimal("1"), Convert.ToDecimal(PKID));
                    id = "Opinion Poll Edited Successfully.";
                }

                if (oper == "add")
                {

                    //Content.AddOpinionPoll(SelUserType, Question, Option1, Option2, Option3, Option4, Option5, Convert.ToDecimal("1"));
                  //  id = "Opinion Poll Added Successfully.";
                }

                if (oper == "del")
                {

                    Content.DeleteOpinionPoll(Convert.ToDecimal(id));
                    id = "Opinion Poll Deleted Successfully.";
                }


                return Json(id);
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }


        public ActionResult UploadImage(string id)
        {
            return View();
        }


        #region Public Methods - FileCopytoServerPath
        private void FileCopytoServerPath()
        {

            //string sGetFileName  = txtFileName1.PostedFile.FileName.Substring(txtFileName1.PostedFile.FileName.LastIndexOf("\\")+1);

            //if (txtFileName1.PostedFile.ContentLength > 0)
            //{
            //    string sGetFileName = Path.GetFileName(txtFileName1.Value);
            //    string sServerPath = Server.MapPath("../../DataBank\\");
            //    string sPath_FileName = sServerPath + sGetFileName;
            //    FileInfo fileMoveSour = new FileInfo(sPath_FileName);
            //    if (fileMoveSour.Exists)
            //    {
            //        fileMoveSour.Delete();

            //    }
            //    try
            //    {
            //        txtFileName1.PostedFile.SaveAs(sPath_FileName);
            //    }
            //    catch (Exception ex)
            //    {
            //        ER.errLabel.Text = ex.Message.ToString();
            //    }

            //}
        }

        #endregion


        #endregion

        #region FAQ

        //Raj Kumar 

        public ActionResult FAQ()
        {
            return View();
        }



        public JsonResult GridDataFAQ(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in Content.FAQS1View select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "GenName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.GenName!=null && sq.GenName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.GenName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.GenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Question" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Question descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Answer" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Answer ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Answer" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Answer descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Question" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Question ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                }

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

        public JsonResult EditFAQ(string id, string oper, string PKID, string GenName, string Question, string Answer, string IsActive)
        {
            decimal ePKID;
            decimal categoyFkid;
            bool eIsActive = false;
            decimal ModifiedBy;
            decimal CreatedBy;
            if (oper == "edit")
            {
                ePKID = Convert.ToInt32(PKID);
                categoyFkid = Convert.ToInt32(GenName);

                if (IsActive == "Active")
                    eIsActive = true;

                if (IsActive == "Passive")
                    eIsActive = false;

                ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                Content.EditFAQS(ePKID, categoyFkid, Question, Answer, eIsActive, ModifiedBy);

                id = "FAQ Master modified Successfully.";
            }
            if (oper == "add")
            {
                categoyFkid = Convert.ToInt32(GenName);

                if (IsActive == "Active")
                    eIsActive = true;

                if (IsActive == "Passive")
                    eIsActive = false;

                CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                Content.AddFAQS(categoyFkid, Question, Answer, eIsActive, CreatedBy);

                id = "FAQ Master Added Successfully.";

            }
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                Content.DeleteFAQS(quote);
                id = "FAQ Master deleted successfully";
            }

            return Json(id);
        }


        public ActionResult FAQCategory()
        {
            var query = (from e in Content.GetCategoryByID() select new { e.CategoryID, e.CategoryName }).ToDictionary(f => Convert.ToInt32(f.CategoryID), f => f.CategoryName.ToString());
            return PartialView("LoadYear", query);
        }
        #endregion

    }
}
