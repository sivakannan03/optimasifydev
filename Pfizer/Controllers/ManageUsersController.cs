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
using Sify.Encryption;
using System.Globalization;

namespace Pfizer.Controllers
{
    public class ManageUsersController : Controller
    {
        //
        // GET: /ManageUsers/
        private pfizerEntities dcManageUsers = new pfizerEntities();
        ExportPdf pdf = new ExportPdf();
        string path = ConfigurationManager.AppSettings["PDFfilePath"].ToString();
        Sify.Encryption.Base64 bs = new Base64();
        #region Location Master

        public ActionResult LocationMaster()
        {

            return View();
        }
        public JsonResult GetLocMaster(GridQueryModel gridQueryModel)
        {
            try
            {

                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in dcManageUsers.GetLocationMaster() select q).ToList();

                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "LocationDesc") || (searchField == "Status"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.LocationDesc.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.LocationDesc).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.LocationDesc.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.LocationDesc).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.LocationDesc.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.LocationDesc).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.LocationDesc.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.LocationDesc).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "LocationDesc" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.LocationDesc descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "LocationDesc" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.LocationDesc ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        public JsonResult AddLocationMaster(string id, string oper, string PKID, string LocationDesc, string Status)
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

                var rslt = (from a in dcManageUsers.LocationMasters where a.LocationDesc == LocationDesc select a).ToList();
                if (rslt.Count() > 0)
                    msg = "Location Details already Exists";
                else
                {

                    LocationMaster locMas = new LocationMaster();
                    locMas.LocationDesc = LocationDesc;
                    locMas.IsActive = Convert.ToBoolean(Status);
                    locMas.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    locMas.CreatedDate = DateTime.Now;
                    locMas.TerritoryFKID = 1;
                    dcManageUsers.LocationMasters.Add(locMas);
                    var Result = dcManageUsers.SaveChanges();
                    if (Result == 1)
                    {
                        msg = "Location Details added Successfully.";
                    }
                }

                return Json(msg);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }


        public JsonResult EditLocationMaster(string id, string oper, string PKID, string LocationDesc, string Status)
        {
            try
            {
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }
                string msg = string.Empty;

                if (oper == "edit")
                {

                    //var rslt = (from a in dcManageUsers.LocationMasters where a.LocationDesc == LocationDesc  select a).ToList();
                    //if (rslt.Count() > 0)
                    //    msg = "Location Details already Exists";
                    //else
                    //{

                    int ePKID = Convert.ToInt32(id);

                    // var qry = (from q in ManageUser.LocationMasters select q).ToList();
                    var qry = (from a in dcManageUsers.LocationMasters where a.PKID == ePKID select a).Single();

                    qry.LocationDesc = LocationDesc.TrimStart();
                    //  qry.IsActive = Convert.ToBoolean(Status);
                    qry.IsActive = Convert.ToBoolean(Status);
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    qry.ModifiedDate = DateTime.Now;
                    if (qry.IsActive == false)
                        qry.IsActive = Convert.ToBoolean(Status);
                    var result = dcManageUsers.SaveChanges();
                    if (result == 1)
                    {
                        msg = "Location Details Updated Successfully";
                    }
                    //}
                }

                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageUsers.LocationMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    var Result = dcManageUsers.SaveChanges();
                    if (Result == 1)
                    {
                        msg = "Location Details Deleted Successfully.";
                    }


                }

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        public ActionResult ExportLocMasterToExcel(string gridData)
        {
            try
            {
                //JavaScriptSerializer objJavaScriptSerializer = new JavaScriptSerializer();

                //List<LocMaster> objStatus = objJavaScriptSerializer.Deserialize<List<LocMaster>>(gridData);

                var context = (from q in dcManageUsers.GetLocationMaster() select q).ToList();

                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                //item.PKID.ToString(),
                item.LocationDesc==null?"":item.LocationDesc,
                item.Status
               
            }));
                return new ExcelResult(HeadersLocationQuestions, ColunmTypesLocationQuestions, data, "LocationMaster.xlsx", "LocationMaster");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersLocationQuestions = {
                 "Location Description", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesLocationQuestions = {
                //DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportLocMasterToPDF()
        {
            try
            {
                List<LocMaster> userList = new List<LocMaster>();
                var context = (from q in dcManageUsers.GetLocationMaster() select q).ToList();

                foreach (var item in context)
                {
                    LocMaster user = new LocMaster();
                    user.PKID = item.PKID.ToString();
                    user.LocationDesc = item.LocationDesc == null ? "" : item.LocationDesc;
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
                            e.PKID.ToString(),e.LocationDesc,e.Status
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] { "LocationDesc", "Status" }, path);
                return File(path, "application/pdf", "LocMaster.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportLocMasterCsv()
        {
            try
            {
                var model = (from q in dcManageUsers.GetLocationMaster() select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Location Description,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{1},{2}", record.PKID, record.LocationDesc, record.Status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "LocationMaster.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class LocMaster
        {
            public string PKID { get; set; }
            public string LocationDesc { get; set; }
            public string Status { get; set; }
        }
        #endregion
       

        #region MuniPrathap SalesHierachy
        public ActionResult SalesHierarchy()
        {

            return View();
        }

        public string returnXml(string SelTeamName)
        {

            string[] arrayUser = SelTeamName.Split(',');

            string xml = "<root>";

            for (int i = 0; i < arrayUser.Count(); i++)
            {

                xml += "<CompBrand";
                xml += " TeamFkid='" + arrayUser[i] + "'";
                xml += "/>";


            }
            xml += "</root>";

            return xml;


        }


        public JsonResult AddSalesHierarchy(string id, string oper, string IsLower, string NodeType, string Division, string User, string reportingTo, string NodeFunction, string NodeDescription, string Location, string LocationDescription, string ForumCreator,
                                              string HQ, string SelTeams, string RegionCode, string DistrictCode, string Territory, string IsActive)
        {
            string msg = string.Empty;
            string xml = "";
            if (IsActive == "true")
            {
                IsActive = "1";
            }
            else
            {
                IsActive = "0";
            }
            try
            {

                var Islower = Convert.ToInt32(IsLower);
                var Nodetype = Convert.ToInt32(NodeType);
                var dPkid = Convert.ToDecimal(Division);
                var user = Convert.ToDecimal(User);
                var territory = Convert.ToDecimal(Territory);
                var reporting = Convert.ToDecimal(reportingTo);

                var Nodefunction = Convert.ToInt32(NodeFunction);

                var location = Convert.ToDecimal(Location);


                var forum = Convert.ToInt32(ForumCreator);

                xml = returnXml(SelTeams);

                var createdBy = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                var Hq = Convert.ToDecimal(HQ);

                var district = Convert.ToDecimal(DistrictCode);

                var region = Convert.ToDecimal(RegionCode);


                dcManageUsers.AddSalesMaster2(Islower, Nodetype, dPkid, user, territory, reporting, Nodefunction, NodeDescription, location, LocationDescription, forum, xml, createdBy, Hq, district, region);


                msg = "SalesHierachy added Successfully.";


                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json("SalesHierachy added Successfully.");

            }


        }
        public JsonResult EditSalesHierarchy(string id, string oper, string IsLower, string NodeType, string Division, string User, string reportingTo, string NodeFunction, string NodeDescription, string Location, string LocationDescription, string ForumCreator,
                                               string HQ, string SelTeams, string RegionCode, string DistrictCode, string Territory, string IsActive)
        {

            string msg = string.Empty;
            try
            {
                string xml = "";

                if (oper == "edit")
                {


                    if (IsActive == "true")
                    {
                        IsActive = "1";
                    }
                    else
                    {
                        IsActive = "0";
                    }
                    var pKID = Convert.ToDecimal(id);
                    var Islower = Convert.ToInt32(IsLower);
                    var Nodetype = Convert.ToInt32(NodeType);
                    var dPkid = Convert.ToDecimal(Division);
                    var user = Convert.ToDecimal(User);
                    var territory = Convert.ToDecimal(Territory);
                    var reporting = Convert.ToDecimal(reportingTo);

                    var Nodefunction = Convert.ToInt32(NodeFunction);

                    var location = Convert.ToDecimal(Location);


                    var forum = Convert.ToInt32(ForumCreator);

                    xml = returnXml(SelTeams);

                    var createdBy = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    var modifiedBy = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    var Hq = Convert.ToDecimal(HQ);

                    var district = Convert.ToDecimal(DistrictCode);

                    var region = Convert.ToDecimal(RegionCode);


                    dcManageUsers.EditSalesMaster2(pKID, Islower, Nodetype, dPkid, user, territory, reporting, Nodefunction, LocationDescription, forum, xml, location, NodeDescription, createdBy, modifiedBy, Convert.ToInt32(IsActive), Hq, district, region);

                    dcManageUsers.SaveChanges();
                    msg = "SalesHierachy modified Successfully.";

                }



                if (oper == "del")
                {

                    var PKID = Convert.ToDecimal(id);
                    var userid = Convert.ToDecimal(User);
                    dcManageUsers.DeleteSalesHierarchy_New(PKID, userid);

                    msg = "SalesHierachy Master deleted Successfully.";
                }

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json("SalesHierachy Master Modified Successfully.");
            }
        }


        public JsonResult GridSalesHierarchy(GridQueryModel gridQueryModel)
        {
            try
            {

                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                var query = (from a in dcManageUsers.usp_GetSalesHierarchy()
                             select new
                             {
                                 a.IsActive,
                                 userName = a.FirstName + " " + a.uMiddleName + " " + a.uLastName,
                                 reportingTo = a.LMN + " " + a.MiddleName + " " + a.LastName,
                                 a.NodeName,
                                 a.PKID
                             }).ToList();
                var count = query.Count();
                
                var pageData = query.OrderBy(x => x.PKID).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "userName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.userName != null && sq.userName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.userName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.userName != null && sq.userName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.userName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.userName != null && sq.userName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.userName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in pageData where sq.userName != null && sq.userName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.userName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                    else if (searchField == "NodeName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.NodeName != null && sq.NodeName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NodeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.NodeName != null && sq.NodeName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NodeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.NodeName != null && sq.NodeName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NodeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.NodeName != null && sq.NodeName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NodeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }
                    else if (searchField == "reportingTo")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.reportingTo != null && sq.reportingTo.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.reportingTo).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.reportingTo != null && sq.reportingTo.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.reportingTo).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.reportingTo != null && sq.reportingTo.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.reportingTo).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in pageData where sq.reportingTo != null && sq.reportingTo.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.reportingTo).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }


                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "userName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.userName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "userName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.userName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "NodeName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.NodeName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "NodeName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.NodeName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "reportingTo" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.reportingTo ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "reportingTo" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.reportingTo descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



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
        //For Excel & PDF & CSV For SalesHierachy Master
        public ActionResult ExportSalesHierarchyMasterToExcel()
        {


            var context = (from a in dcManageUsers.usp_GetSalesHierarchy()
                           orderby a.PKID
                           select new
                           {
                               PKID = a.PKID.ToString(),
                               userName = a.FirstName + " " + a.uMiddleName + " " + a.uLastName,
                               a.NodeName,
                               reportingTo = a.LMN + " " + a.MiddleName + " " + a.LastName,
                               IsActive = a.IsActive == null ? "" : a.IsActive.ToString()
                           }).ToList();

            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[]{            
             // item.PKID,
              item.userName,
              item.NodeName ,
              item.reportingTo,
              item.IsActive 
                   
                          
            }));


            return new ExcelResult(HeadersActivityMaster, ColunmTypesActivityMaster, data, "SalesHierachy.xlsx", "SalesHierachy");
        }
        private static readonly string[] HeadersActivityMaster = {
               "userName","NodeName","reportingTo","IsActive"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesActivityMaster = { 
                
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                                              
            };

        public ActionResult ExportSalesHierarchyMasterToPDF()
        {
            List<SalesHierachyMasterPDF> userList = new List<SalesHierachyMasterPDF>();


            var context = (from a in dcManageUsers.usp_GetSalesHierarchy()
                           orderby a.PKID
                           select new
                           {
                               a.IsActive,
                               userName = a.FirstName + " " + a.uMiddleName + " " + a.uLastName,
                               reportingTo = a.LMN + " " + a.MiddleName + " " + a.LastName,
                               a.NodeName,
                               a.PKID
                           }).ToList();

            foreach (var item in context)
            {
                SalesHierachyMasterPDF user = new SalesHierachyMasterPDF();

               // user.PKID = item.PKID.ToString() == null ? "" : item.PKID.ToString();
                user.userName = item.userName == null ? "" : item.userName;
                user.NodeName = item.NodeName == null ? "" : item.NodeName;
                user.reportingTo = item.reportingTo == null ? "" : item.reportingTo;
                user.IsActive = item.IsActive.ToString();




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
                            //e.PKID,
                            e.userName,
                            e.NodeName,
                            e.reportingTo,
                            e.IsActive,
                           
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] {"userName", "NodeName", "reportingTo", "IsActive", }, path);
            return File(path, "application/pdf", "SalesHierachy.pdf");
        }
        public ActionResult ExportSalesHierarchyMasterPDFToCsv()
        {


            var model = (from a in dcManageUsers.usp_GetSalesHierarchy()
                         orderby a.PKID
                         select new
                         {
                             a.IsActive,
                             userName = a.FirstName + " " + a.uMiddleName + " " + a.uLastName,
                             reportingTo = a.LMN + " " + a.MiddleName + " " + a.LastName,
                             a.NodeName,
                             a.PKID
                         }).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("userName, NodeName, reportingTo, IsActive");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3}",


                    //record.PKID == null ? "" : record.PKID.ToString(),
                    record.userName == null ? "" : record.userName,
                    record.NodeName == null ? "" : record.NodeName.ToString(),
                    record.reportingTo == null ? "" : record.reportingTo,
                    record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "SalesHierachyMaster.txt");
        }
        public class SalesHierachyMasterPDF
        {

            //public string PKID { get; set; }
            public string userName { get; set; }
            public string NodeName { get; set; }
            public string reportingTo { get; set; }
            public string IsActive { get; set; }

        }
        public ActionResult LoadIsLower(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = "";


            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.Islower }).FirstOrDefault();
                ViewBag.Selected = rslt.Islower.ToString();
            }

            return PartialView("PVLoadNodeType");
        }
        public ActionResult LoadNodeFunc(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = "";


            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.NodeFunctionFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.NodeFunctionFKID.ToString();
            }

            return PartialView("PVLoadNodeType");
        }

        public ActionResult LoadForum(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = "";


            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.ForumCreator }).FirstOrDefault();
                ViewBag.Selected = rslt.ForumCreator.ToString();
            }

            return PartialView("PVLoadNodeType");
        }

        public JsonResult JsonLoadNodeDesc(string id)
        {
            string NodeDesc = "";

            var query = dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(id)).Select(x => new { x.NodeDesc }).FirstOrDefault();
            return Json(new
            {
                NodeDesc = query.NodeDesc == null ? string.Empty : query.NodeDesc,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonLoadLocationDesc(string id)
        {
            string LocDesc = "";

            var query = dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(id)).Select(x => new { x.LocationDesc }).FirstOrDefault();
            return Json(new
            {
                LocDesc = query.LocationDesc == null ? string.Empty : query.LocationDesc,

            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult LoadNodeType(string rowId, string type)
        {
            dynamic query = "";
            dynamic edit = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;

            query = (from a in dcManageUsers.GetNodeTypeByXML() select new { a.NodeTypeFKID, a.NodeName }).ToDictionary(f => Convert.ToInt32(f.NodeTypeFKID), f => f.NodeName.ToString());

            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.NodeTypeFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.NodeTypeFKID;
            }

            return PartialView("PVLoadNodeType", query);
        }

        public ActionResult LoadSalesDivision(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;
            query = (from a in dcManageUsers.GetDivisonMasterByPkid() select new { a.PKID, a.DivName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DivName.ToString());
            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.DivFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.DivFKID;
            }
            return PartialView("PVLoadNodeType", query);
        }

        public ActionResult LoadUserType(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;
            query = (from a in dcManageUsers.GetReportingByXml() select new { a.PKID, a.LoginName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.LoginName.ToString());
            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.UserFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.UserFKID;
            }
            return PartialView("PVLoadNodeType", query);
        }


        public ActionResult LoadLocation(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;

            query = (from a in dcManageUsers.GetLocatioNameByXml() select new { a.PKID, a.LocationName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.LocationName.ToString());


            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.LocationFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.LocationFKID;
            }
            return PartialView("PVLoadNodeType", query);
        }


        public ActionResult LoadHQType(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;
            query = (from a in dcManageUsers.GetHQNameByXML() select new { a.HQFKID, a.HQName }).ToDictionary(f => Convert.ToInt32(f.HQFKID), f => f.HQName.ToString());
            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.HQFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.HQFKID;
            }

            return PartialView("PVLoadNodeType", query);
        }


        public ActionResult LoadTeam(string type, string DivFKID)
        {

            dynamic query = "";
            ViewBag.Type = type;
            query = (from a in dcManageUsers.GetTeamMasterByXML_New(Convert.ToDecimal(DivFKID)) select new { a.PKID, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName.ToString());

            return PartialView("PVLoadReportValue", query);
        }

        public ActionResult LoadTeamType(string rowId, string type, string fkId)
        {
            dynamic query = "";
            ViewBag.Type = type;
            if (rowId == "null")
                query = (from a in dcManageUsers.GetTeamMasterByXML_New(Convert.ToDecimal(1)) select new { a.PKID, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName.ToString());
            else
                query = (from a in dcManageUsers.GetTeamMasterByXML_New(Convert.ToDecimal(1)) select new { a.PKID, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName.ToString());

            return PartialView("PVLoadReportValue", query);
        }
        public ActionResult LoadSelectedTeam(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            ViewBag.Selected = "";
            if (rowId != "null")
            {
                query = (from a in dcManageUsers.GetSalesTeamByXml(Convert.ToDecimal(rowId)) select new { a.TeamFKID, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.TeamFKID), f => f.TeamName.ToString());
                return PartialView("PVLoadReportValue", query);
            }
            else
            {
                //query = (from a in dcManageUsers.GetTeamMasterByXML_New(Convert.ToDecimal(1)) select new { a.PKID, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName.ToString());
                ViewBag.Selected = rowId;

                return PartialView("PVLoadReportValue");
            }
        }
        public ActionResult LoadRegionType(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;

            query = (from a in dcManageUsers.getRegionNameNew() where a.RegionCode != "" && a.RegionCode != null select new { a.PKID, a.RegionCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.RegionCode.ToString());
            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.RegionFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.RegionFKID;
            }

            return PartialView("PVLoadReportValue", query);
        }



        public ActionResult LoadDistrictCodeType(string rowId, string pkid, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;
            query = (from a in dcManageUsers.getDistrictNameWSNew() where a.DistrictCode != "" && a.DistrictCode != null select new { a.PKID, a.DistrictCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DistrictCode.ToString());
            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.DistrictFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.DistrictFKID;
            }

            return PartialView("PVLoadReportValue", query);
        }
        public ActionResult LoadTerritoryType(string rowId, string pkid, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;
            int District = 0;
            if (rowId != "null" && rowId != "")
            {
                var rslt = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.DistrictFKID }).FirstOrDefault();
                District = Convert.ToInt32(rslt.DistrictFKID);
                query = (from a in dcManageUsers.GetAllTerritoryCodeXmlByDistrictFKID(District) where a.TerritoryName != "" && a.TerritoryName != null select new { a.TerritoryId, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.TerritoryId), f => f.TerritoryName.ToString());

                var rslt1 = (from a in dcManageUsers.GetSalesHierrachy(Convert.ToDecimal(rowId)) select new { a.TerritoryFKID }).FirstOrDefault();
                ViewBag.Selected = rslt1.TerritoryFKID;
            }

            return PartialView("PVLoadNodeType", query);
        }
        public ActionResult LoadDistrictByRegion(string pkid, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;

            query = (from a in dcManageUsers.GetAllDistrictMasterXMLByRegionFKID(Convert.ToInt32(pkid)) where a.DistrictCode != null select new { a.PKID, a.DistrictCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DistrictCode.ToString());

            return PartialView("PVLoadReportValue", query);
        }


        public ActionResult LoadTerritoryByDistrict(string pkid, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;

            //if (rowId == "null")
            query = (from a in dcManageUsers.GetAllTerritoryCodeXmlByDistrictFKID(Convert.ToInt32(pkid)) where a.TerritoryName != "" && a.TerritoryName != null select new { a.TerritoryId, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.TerritoryId), f => f.TerritoryName.ToString());

            return PartialView("PVLoadReportValue", query);
        }

        #endregion
        /* Holiday Group Master starts */

        public ActionResult HolidayGroup()
        {

            return View();
        }

        public JsonResult HolidayGridData(GridQueryModel gridQueryModel)
        {
            try
            {

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var result = dcManageUsers.HolidayGroupMasters.Select(x => new
                {
                    PKID = x.PKID,
                    HolidayGroupName = x.HolidayGroupName,
                    IsActive = x.IsActive == true ? "Active" : "In Active",
                }).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "HolidayGroupName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where sq.HolidayGroupName.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where sq.HolidayGroupName.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where sq.HolidayGroupName.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in result where sq.HolidayGroupName.ToUpper().ToString().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = result.Count();

                    if (gridQueryModel.sidx == "HolidayGroupName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.HolidayGroupName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "HolidayGroupName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.HolidayGroupName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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


        public JsonResult AddHolidayGroupMaster(string id, string oper, string PKID, string HolidayGroupName, string IsActive)
        {
            try
            {


                var rslt = (from a in dcManageUsers.HolidayGroupMasters where a.HolidayGroupName == HolidayGroupName select a).ToList();
                if (rslt.Count() > 0)
                    id = "Holiday Group Master already Exists";
                else
                {
                    HolidayGroupMaster holygroup = new PfizerEntity.HolidayGroupMaster();
                    holygroup.HolidayGroupName = HolidayGroupName.TrimStart();
                    if (IsActive == "on")
                        holygroup.IsActive = true;
                    else if (IsActive == "off")
                        holygroup.IsActive = false;
                    holygroup.CreatedDate = DateTime.Now;
                    holygroup.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageUsers.HolidayGroupMasters.Add(holygroup);
                    dcManageUsers.SaveChanges();
                    id = "Holiday Group Master added successfully";



                }

                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg);
            }
        }
        public JsonResult EditHolidayGroupMaster(string id, string oper, string PKID, string HolidayGroupName, string IsActive)
        {
            try
            {

                if (oper == "edit")
                {
                    int ePKID = Convert.ToInt32(PKID);

                    var rslt = (from a in dcManageUsers.HolidayGroupMasters where a.PKID == ePKID select a).Single();

                    rslt.HolidayGroupName = HolidayGroupName.TrimStart();
                    rslt.ModifiedDate = DateTime.Now;
                    rslt.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    if (IsActive == "on")
                        rslt.IsActive = true;
                    else if (IsActive == "off")
                        rslt.IsActive = false;


                    dcManageUsers.SaveChanges();
                    id = "Holiday Group Master modified Successfully.";

                }
                //if (oper == "add")
                //{

                //    var rslt = (from a in dcManageUsers.HolidayGroupMasters where a.HolidayGroupName==HolidayGroupName select a).ToList();
                //    if (rslt.Count() > 0)
                //        id = "Holiday Group Master already Exists";
                //    else
                //    {
                //        HolidayGroupMaster holygroup = new PfizerEntity.HolidayGroupMaster();
                //        holygroup.HolidayGroupName = HolidayGroupName.TrimStart();
                //        if (IsActive == "on")
                //            holygroup.IsActive = true;
                //        else if (IsActive == "off")
                //            holygroup.IsActive = false;
                //        holygroup.CreatedDate = DateTime.Now;
                //        holygroup.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                //        dcManageUsers.HolidayGroupMasters.Add(holygroup); 
                //        dcManageUsers.SaveChanges();
                //        id = "Holiday Group Master added successfully";

                //    }

                //}
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageUsers.HolidayGroupMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    delete_quote.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageUsers.SaveChanges();
                    id = "Holiday Group Master deleted successfully";
                }
                return Json(id);
            }
            catch
            {
                return Json(id);
            }


        }


        //  Export --> Excel ,CSV,PDF


        public ActionResult ExportHolidayGroupMasterToExcel()
        {
            //var context =  dcManageUsers.HolidayGroupMasters.Select(x => new
            //    {  
            //        HolidayGroupName = x.HolidayGroupName,
            //        IsActive = x.IsActive == true ? "Active" : "In Active",
            //    }).ToList();
            var context = (from a in dcManageUsers.HolidayGroupMasters orderby a.HolidayGroupName select new { a.HolidayGroupName, IsActive = a.IsActive == true ? "Active" : "In Active" }).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.HolidayGroupName.ToString()== null ? "" : item.HolidayGroupName.ToString(),
                item.IsActive 
               
            }));
            return new ExcelResult(HeadersHolidayGroupMaster, ColunmTypesHolidayGroupMaster, data, "HolidayGroup.xlsx", "Holiday Group");
        }
        private static readonly string[] HeadersHolidayGroupMaster = {
               "Holiday Group Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesHolidayGroupMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportHolidayGroupMasterToPDF()
        {
            List<HolidayMasterPDF> userList = new List<HolidayMasterPDF>();
            //var context = dcManageUsers.HolidayGroupMasters.Select(x => new
            //{
            //    HolidayGroupName = x.HolidayGroupName,
            //    IsActive = x.IsActive == true ? "Active" : "In Active",
            //}).ToList();
            var context = (from a in dcManageUsers.HolidayGroupMasters orderby a.HolidayGroupName select new { a.HolidayGroupName, IsActive = a.IsActive == true ? "Active" : "In Active" }).ToList();
            foreach (var item in context)
            {
                HolidayMasterPDF user = new HolidayMasterPDF();

                user.HolidayGroupName = item.HolidayGroupName == null ? "" : item.HolidayGroupName;
                user.Status = item.IsActive;
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
                            e.HolidayGroupName,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "HolidayGroupName", "Status" }, path);
            return File(path, "application/pdf", "HolidayGroup.pdf");
        }
        public ActionResult ExportHolidayGroupMasterToCsv()
        {
            //var model = dcManageUsers.HolidayGroupMasters.Select(x => new
            //{
            //    HolidayGroupName = x.HolidayGroupName,
            //    IsActive = x.IsActive == true ? "Active" : "In Active",
            //}).ToList();
            var model = (from a in dcManageUsers.HolidayGroupMasters orderby a.HolidayGroupName select new { a.HolidayGroupName, IsActive = a.IsActive == true ? "Active" : "In Active" }).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Holiday Group Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record.HolidayGroupName, record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "HolidayGroup.txt");
        }
        public class HolidayMasterPDF
        {

            public string HolidayGroupName { get; set; }
            public string Status { get; set; }
        }

        //    end

        /* Holiday Group Master ends */

        #region CreateUser

        public ActionResult CreateUser()
        {

            return View();

        }

        public JsonResult JsonLoadUserRoles(string pkid, string type)
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            roleItems = PfizerEntity.UsefulLink.GetRoles(pkid, type);
            string loginUser, pwd = "";
            var rslt = (from a in dcManageUsers.GetCreateUserByPKID(Convert.ToDecimal(pkid)) select new { a.LoginUser, a.Password }).FirstOrDefault();

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,
                loginUser = rslt.LoginUser,
                pwd = bs.Decrypt(rslt.Password)
            }, JsonRequestBehavior.AllowGet);

        }



        public JsonResult JsonGetQuestion(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table with='100%' id='mytable'>");
            string tbl;
            decimal userFKID = Convert.ToDecimal(id);
            var rslt = (from a in dcManageUsers.GetUserQuestionMaster(userFKID) select new { a.PKID, a.Question, a.Answer }).ToList();
            int i = 0;
            foreach (var items in rslt)
            {
                sb.Append("<tr><td style='display:none'><input type='text' value=" + items.PKID + " id=hdn" + i + "></td><td>" + items.Question + "</td><td><input type='text' id=txt" + i + " value=" + items.Answer + "></td></tr>");
                i++;
            }

            sb.Append("</table>");

            return Json(new
            {
                tbl = sb.ToString()

            }, JsonRequestBehavior.AllowGet);

        }




        public JsonResult JsonSetDefault(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table with='100%'>");
            string tbl = string.Empty;
            decimal userFKID = Convert.ToDecimal(id);
            var rslt = (from a in dcManageUsers.DefaultAnswer() select new { a.PKID, a.Question, a.Answer }).ToList();
            int i = 0;
            foreach (var items in rslt)
            {
                sb.Append("<tr><td style='display:none'><input type='text' value=" + items.PKID + " id=hdn" + i + "></td><td>" + items.Question + "</td><td><input type='text' id=txt" + i + " value=" + items.Answer + "></td></tr>");
                i++;
            }

            sb.Append("</table>");

            return Json(new
            {
                tbl = sb.ToString()

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonGetDefaultQuestion()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<table with='100%'>");
            string tbl = string.Empty;

            var rslt = (from a in dcManageUsers.DefaultAnswer() select new { a.PKID, a.Question, a.Answer }).ToList();
            int i = 0;
            foreach (var items in rslt)
            {
                sb.Append("<tr><td style='display:none'><input type='text' value=" + items.PKID + " id=hdn" + i + "></td><td>" + items.Question + "</td><td><input type='text' id=txt" + i + " value=" + items.Answer + "></td></tr>");
                i++;
            }

            sb.Append("</table>");

            return Json(new
            {
                tbl = sb.ToString()

            }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult CreateUser(FormCollection form)
        {

            return View();
        }

        public JsonResult JsPostUser(string XMLstr, string Employee, string Login, string Pwd, string ConfirmPwd, string role)
        {

            string[] arr = XMLstr.Split('|');
            role = role.Remove(role.Length - 1, 1);
            int i = 1, j = 0;
            string str = "";
            string alertMsg = "";
            string rslt = "";


            try
            {

                if (arr.Count() > 1)
                {

                    str = "<ROOT>";
                    foreach (var item in arr)
                    {

                        if (j < arr.Count() - 1)
                        {
                            if (i == 1)
                            {


                                str += "<CampaignDtls ";
                                str += "QuestionFKID='" + item + "' ";

                                i++;
                            }
                            else
                            {

                                if (item == "")
                                    str += "Answer=''/> ";
                                else
                                {
                                    str += "Answer='" + item + "' ";
                                    str += "/>";
                                }
                                i = 1;
                            }
                            j++;
                        }
                    }
                    str += "</ROOT>";
                }

                dcManageUsers.AddUserMaster(Convert.ToDecimal(Employee), role, Login, bs.Encrypt(Pwd), "1", Convert.ToDecimal(Session["USER_FKID"].ToString()), str);
                alertMsg = "User Master Updated Successfully.";

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



        #endregion


        /* Holiday Territory Link*/

        public ActionResult HolidayGroupTerritory()
        {
            return View();
        }


        public JsonResult TerritoryGridData(GridQueryModel gridQueryModel)
        {
            try
            {

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var result = dcManageUsers.uspGetHolidayMaster().Select(x => new
                {
                    x.HolidaygroupFKID,
                    x.HolidayGroupName,
                    forward = ">>",
                    backward = "<<"
                }).OrderBy(O => O.HolidayGroupName).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.HolidaygroupFKID).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "HolidayGroupName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where sq.HolidayGroupName.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where sq.HolidayGroupName.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where sq.HolidayGroupName.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in result where sq.HolidayGroupName.ToUpper().ToString().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = result.Count();

                    if (gridQueryModel.sidx == "HolidayGroupName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.HolidayGroupName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "HolidayGroupName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.HolidayGroupName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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


        public ActionResult LoadTerritory(string type, string HolidaygroupFKID, string rowId)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in dcManageUsers.GetHolidayGroupTerritoryLinkmasterByXml(Convert.ToDecimal(HolidaygroupFKID)) where a.Element == type select new { a.TerritoryFKID, a.TerCode }).ToDictionary(f => Convert.ToInt32(f.TerritoryFKID), f => f.TerCode.ToString());
            else
                query = (from a in dcManageUsers.GetHolidayGroupTerritoryLinkmasterByXml(Convert.ToDecimal(5)) select new { a.TerritoryFKID, a.TerCode }).ToDictionary(f => Convert.ToInt32(f.TerritoryFKID), f => f.TerCode.ToString());

            return PartialView("PVLoadHolidayTeritory", query);

        }

        public ActionResult LoadSubTerritory(string rowId, string HolidaygroupFKID, string type)
        {
            ViewBag.Type = type;

            dynamic query = "";

            if (rowId != "null")
                query = (from a in dcManageUsers.GetHolidayGroupTerritoryLinkmasterByXml(Convert.ToDecimal(HolidaygroupFKID)) where a.Element == type select new { a.TerritoryFKID, a.TerCode }).ToDictionary(f => Convert.ToInt32(f.TerritoryFKID), f => f.TerCode.ToString());
            else
                query = (from a in dcManageUsers.GetHolidayGroupTerritoryLinkmasterByXml(Convert.ToDecimal(0)) where 1 == 2 select new { a.TerritoryFKID, a.TerCode }).ToDictionary(f => Convert.ToInt32(f.TerritoryFKID), f => f.TerCode.ToString());


            return PartialView("PVLoadHolidayTeritory", query);
        }


        public ActionResult LoadGroup(string rowId, string type)
        {
            dynamic query = "";
            ViewBag.Type = type;

            query = (from a in dcManageUsers.GetHolidayGroupNameByXml() select new { a.PKID, a.HolidayGroupName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.HolidayGroupName.ToString());

            return PartialView("PVLoadHolidayTeritory", query);
        }


        public JsonResult AddHolidayTerritory(string id, string oper, string HolidaygroupFKID, string HolidayGroupName, string IsActive, string Territory, string SelTerritory, string selectedTerritory, FormCollection col, string xyz)
        {
            string xml = string.Empty;
            string[] arrayTerritory = SelTerritory.Split(',');
            StringBuilder sb = new StringBuilder();
            sb.Append("<root>");
            for (int i = 0; i < arrayTerritory.Count(); i++)
            {
                sb.Append("<CompBrand TerritoryFKID='" + arrayTerritory[i]);
                sb.Append("'/>");

            }
            sb.Append("</root>");

            try
            {
                dcManageUsers.AddHolidayGroupTerritoryLinkMaster(Convert.ToDecimal(HolidayGroupName), sb.ToString(), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                id = "Holiday Group Link Master Added Successfully.";
                return Json(id);
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public JsonResult EditHolidayTerritory(string id, string oper, string HolidaygroupFKID, string HolidayGroupName, string IsActive, string Territory, string SelTerritory, string selectedTerritory, FormCollection col, string xyz)
        {

            string xml = string.Empty;
            string[] arrayTerritory = SelTerritory.Split(',');
            StringBuilder sb = new StringBuilder();
            sb.Append("<root>");
            for (int i = 0; i < arrayTerritory.Count(); i++)
            {
                sb.Append("<CompBrand TerritoryFKID='" + arrayTerritory[i]);
                sb.Append("'/>");

            }
            sb.Append("</root>");

            try
            {

                if (oper == "edit")
                {
                    dcManageUsers.EditHolidayGroupTerritoryLinkMaster(Convert.ToDecimal(HolidayGroupName), sb.ToString(), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                    id = "Holiday Group Link Master Edited Successfully.";

                }

                return Json(id);
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }


        }



        //  Export --> Excel ,CSV,PDF 

        public ActionResult ExportHolidayGroupMasToExcel()
        {
            var context = dcManageUsers.uspGetHolidayMaster().Select(x => new
            {

                x.HolidayGroupName,

            }).OrderBy(O => O.HolidayGroupName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.HolidayGroupName.ToString()==null?"":item.HolidayGroupName.ToString() 
               
            }));
            return new ExcelResult(HeadersHolidayGroupMas, ColunmTypesHolidayGroupMas, data, "HolidayGroupTerritory.xlsx", "Holiday Group Master");
        }
        private static readonly string[] HeadersHolidayGroupMas = {
               "HolidayGroupName"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesHolidayGroupMas = { 
               
                DataForExcel.DataType.String
               
                
            };


        public ActionResult ExportHolidayGroupMasToPDF()
        {
            List<HolidayGroupMasPDF> userList = new List<HolidayGroupMasPDF>();
            var context = dcManageUsers.uspGetHolidayMaster().Select(x => new
            {

                x.HolidayGroupName,

            }).OrderBy(O => O.HolidayGroupName).ToList();

            foreach (var item in context)
            {
                HolidayGroupMasPDF user = new HolidayGroupMasPDF();

                user.HolidayGroupName = item.HolidayGroupName == null ? "" : item.HolidayGroupName;
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
                            e.HolidayGroupName
                          
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "HolidayGroupName" }, path);
            return File(path, "application/pdf", "HolidayGroupTerritory.pdf");
        }
        public ActionResult ExportHolidayGroupMasToCsv()
        {
            var model = dcManageUsers.uspGetHolidayMaster().Select(x => new
            {

                x.HolidayGroupName,

            }).OrderBy(O => O.HolidayGroupName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("HolidayGroupMaster");
            foreach (var record in model)
            {
                sb.AppendFormat("{0}", record.HolidayGroupName

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "HolidayGroupTerritory.txt");
        }
        public class HolidayGroupMasPDF
        {
            public string HolidayGroupName { get; set; }

        }

        //    end




        /* ends */


        /*  Baskar Starts */

        //RoleTypeMaster...
        public ActionResult RoleTypeMaster(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "RoleTypeMaster";
            ViewData["TableName"] = "RoleTypeMaster";
            return View();
        }

        public JsonResult GridData(GridQueryModel gridQueryModel, string fkId)
        {
            try
            {
                decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();

                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageUsers.usp_Get_CampTypeMaster() where a.GenFKID == QrygeneralFKID select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "GenName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.GenName != "" && sq.GenName.ToUpper().StartsWith(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase) || sq.TableName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.GenName != "" && sq.GenName.ToUpper().Equals(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase) || sq.TableName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.GenName != "" && sq.GenName.ToUpper().EndsWith(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase) || sq.TableName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.GenName != "" && sq.GenName.ToUpper().Contains(searchString.ToUpper()) select sq).ToList();
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
                return Json(fkId);
            }
        }

        public JsonResult AddRoleTypeMaster(string id, string oper, string PKID, string Gencode, string GenName, string IsActive, string fkId)
        {
            try
            {
                decimal QrygeneralFKID = Convert.ToDecimal(Session["generalFKID"]);
                var rslt = (from a in dcManageUsers.GeneralMasters where a.GenName == GenName && a.GenFKID == QrygeneralFKID select a).ToList();
                if (rslt.Count() > 0)
                    id = "RoleTypeMaster already Exists";
                else
                {
                    GeneralMaster gm = new GeneralMaster();
                    gm.GenFKID = Convert.ToDecimal(QrygeneralFKID);//24;
                    gm.GenCode = Gencode.TrimStart();
                    gm.GenName = GenName.TrimStart();
                    gm.IsActive = true;
                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageUsers.GeneralMasters.Add(gm);
                    dcManageUsers.SaveChanges();

                    id = "RoleTypeMaster added successfully.";
                }

                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }


        public JsonResult Edit(string id, string oper, string PKID, string Gencode, string GenName, string IsActive, string fkId)
        {
            if (oper == "edit")
            {
                int ePKID = Convert.ToInt32(PKID);
                var qry = (from a in dcManageUsers.GeneralMasters where a.PKID == ePKID select a).Single();

                qry.GenCode = Gencode.TrimStart();
                qry.GenName = GenName.TrimStart();
                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                if (IsActive == "on" || IsActive == "Active")
                    qry.IsActive = true;
                else if (IsActive == "off" || IsActive == "Inactive")
                    qry.IsActive = false;

                dcManageUsers.SaveChanges();

                id = "RoleTypeMaster modified Successfully.";

            }
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in dcManageUsers.GeneralMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                dcManageUsers.SaveChanges();

                id = "RoleTypeMaster deleted successfully.";

            }

            return Json(id);
        }

        //Role Master ...

        public ActionResult RoleMaster()
        {
            return View();
        }

        public JsonResult GridDataRoleMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageUsers.USP_GET_ROLEMASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.RoleName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "RoleName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.RoleName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RoleName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.RoleName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RoleName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.RoleName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RoleName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.RoleName.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RoleName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "RoleName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RoleName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "RoleName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.RoleName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "RoleType" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RoleType descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "RoleType" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.RoleType ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public JsonResult AddRoleMaster(string id, string oper, string GenCode, string GMPKID, string RoleName, string IsActive, string RoleType)
        {
            try
            {
                var rslt = (from a in dcManageUsers.RoleMasters where a.RoleName == RoleName select a).ToList();
                if (rslt.Count() > 0)
                    id = "Role Master already Exists";
                else
                {
                    RoleMaster gm = new RoleMaster();
                    gm.RoleName = RoleName.TrimStart();
                    gm.RoleTypeFKID = Convert.ToInt32(RoleType);
                    if (IsActive == "Active")
                        gm.IsActive = true;
                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageUsers.RoleMasters.Add(gm);
                    dcManageUsers.SaveChanges();

                    id = "Role Master added successfully.";
                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public JsonResult EditRoleMaster(string id, string oper, string GenCode, string GMPKID, string RoleName, string IsActive, string RoleType)
        {
            try
            {
                if (oper == "edit")
                {

                    //var rslt = (from a in dcManageUsers.RoleMasters where a.RoleName == RoleName select a).ToList();
                    //if (rslt.Count() > 0)
                    //    id = "Role Master already Exists";
                    //else
                    //{

                    int ePKID = Convert.ToInt32(id);
                    var qry = (from a in dcManageUsers.RoleMasters where a.PKID == ePKID select a).Single();

                    qry.RoleName = RoleName.TrimStart();
                    qry.RoleTypeFKID = Convert.ToInt32(RoleType);
                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    if (IsActive == "Active")
                        qry.IsActive = true;

                    if (IsActive == "Passive")
                        qry.IsActive = false;

                    dcManageUsers.SaveChanges();

                    id = "Role Master modified Successfully.";
                    // }
                }

                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageUsers.RoleMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    dcManageUsers.SaveChanges();

                    id = "Role Master deleted successfully.";
                }

                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public ActionResult LoadPVRoleMaster()
        {
            var query = (from e in dcManageUsers.GeneralMasters where e.GenFKID == 1 && e.IsActive == true && e.GenName != "" select new { e.PKID, e.GenName }).ToDictionary(f => f.PKID, f => f.GenName.ToString());
            return PartialView("PVLoadRoleMaster", query);
        }

        //Node Type Master ..

        public ActionResult Nodetypemaster()
        {
            return View();
        }

        public JsonResult GridDataNodetypemaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageUsers.USP_GET_MU_NODETYPEMASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.NODENAME).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "NODENAME") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.NODENAME.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NODENAME).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.NODENAME.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NODENAME).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.NODENAME.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NODENAME).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.NODENAME.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NODENAME).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "NODENAME" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.NODENAME descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "NODENAME" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.NODENAME ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        public JsonResult AddNodetypemaster(string id, string oper, string NODENAME, string DAILYREPORTFLAG, string IsActive)
        {
            try
            {
                var rslt = (from a in dcManageUsers.NodeTypeMasters where a.NodeName == NODENAME select a).ToList();
                if (rslt.Count() > 0)
                    id = "Node Type Master already Exists";
                else
                {

                    NodeTypeMaster gm = new NodeTypeMaster();
                    gm.NodeName = NODENAME.TrimStart();
                    gm.NodeCode = "1";

                    if (DAILYREPORTFLAG == "Active")
                        gm.DailyReportFlag = true;

                    if (DAILYREPORTFLAG == "Passive")
                        gm.DailyReportFlag = true;

                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    if (IsActive == "Active")
                        gm.IsActive = true;

                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageUsers.NodeTypeMasters.Add(gm);
                    dcManageUsers.SaveChanges();

                    id = "Node Type Master added successfully.";
                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }

        }

        public JsonResult EditNodetypemaster(string id, string oper, string NODENAME, string DAILYREPORTFLAG, string IsActive)
        {

            if (oper == "edit")
            {

                int ePKID = Convert.ToInt32(id);
                var qry = (from a in dcManageUsers.NodeTypeMasters where a.PKID == ePKID select a).Single();

                qry.NodeName = NODENAME.TrimStart();

                if (DAILYREPORTFLAG == "Active")
                    qry.DailyReportFlag = true;

                if (DAILYREPORTFLAG == "Passive")
                    qry.DailyReportFlag = false;

                if (IsActive == "Active")
                    qry.IsActive = true;

                if (IsActive == "Passive")
                    qry.IsActive = false;

                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                dcManageUsers.SaveChanges();

                id = "Node Type Master modified Successfully.";

            }
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in dcManageUsers.NodeTypeMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                dcManageUsers.SaveChanges();

                id = "Node Type Master deleted successfully.";
            }

            return Json(id);
        }


        //Tema master 

        public ActionResult Teammaster()
        {
            return View();
        }


        public JsonResult GridDataTeammaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageUsers.USP_GET_MU_TEAMMASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "TeamName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.TeamName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.TeamName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.TeamName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.TeamName.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TeamName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TeamName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "DivName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DivName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DivName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DivName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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


        public JsonResult AddTeammaster(string id, string oper, string DivName, string TeamName, string IsActive)
        {
            try
            {
                var rslt = (from a in dcManageUsers.TeamMasters where a.TeamName == TeamName select a).ToList();
                if (rslt.Count() > 0)
                    id = "Tema master already Exists";
                else
                {

                    TeamMaster gm = new TeamMaster();
                    gm.DivFKID = Convert.ToDecimal(DivName);
                    gm.TeamName = TeamName.TrimStart();
                    gm.TeamCode = "1";

                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    if (IsActive == "Active")
                        gm.IsActive = true;

                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageUsers.TeamMasters.Add(gm);
                    dcManageUsers.SaveChanges();

                    id = "Tema master added Successfully.";
                }

                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public JsonResult EditTeammaster(string id, string oper, string DivName, string TeamName, string IsActive)
        {

            if (oper == "edit")
            {

                int ePKID = Convert.ToInt32(id);
                var qry = (from a in dcManageUsers.TeamMasters where a.PKID == ePKID select a).Single();

                qry.TeamName = TeamName.TrimStart();

                qry.DivFKID = Convert.ToDecimal(DivName);

                if (IsActive == "Active")
                    qry.IsActive = true;

                if (IsActive == "Passive")
                    qry.IsActive = false;

                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                dcManageUsers.SaveChanges();

                id = "Tema master modified Successfully.";

            }
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in dcManageUsers.TeamMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                dcManageUsers.SaveChanges();
                id = "Tema master deleted successfully.";
            }

            return Json(id);
        }

        public ActionResult LoadPVDivisionMaster()
        {

            var query = (from a in dcManageUsers.DivisionMasters where a.IsActive == true && a.DivName != "" select new { a.PKID, a.DivName }).ToDictionary(f => f.PKID, f => f.DivName.ToString());
            return PartialView("PVLoadRoleMaster", query);
        }

        //Holiday Master

        public ActionResult HolidayMaster()
        {
            return View();
        }

        public JsonResult GridDataHolidayMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageUsers.USP_GET_MU_HOLIDAYMASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.HolidayDescription).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "HolidayDescription") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.HolidayDescription.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayDescription).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.HolidayDescription.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayDescription).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.HolidayDescription.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayDescription).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.HolidayDescription.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayDescription).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "HolidayDescription" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.HolidayDescription ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "HolidayDescription" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.HolidayDescription descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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


        public JsonResult AddHolidayMaster(string id, string oper, string HolidayDescription, string DisplayName, string HolidayDate, string IsActive)
        {
            bool Isactive = false;
            decimal CreatedBy;
            var date = "";
            try
            {
                var rslt = (from a in dcManageUsers.HolidayMasters where a.HolidayDescription == HolidayDescription select a).ToList();
                if (rslt.Count() > 0)
                    id = "Holiday Master already Exists";
                else
                {

                    if (IsActive == "Passive")
                        Isactive = false;
                    if (IsActive == "Active")
                        Isactive = true;
                    CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    dcManageUsers.AddHolidayMaster(HolidayDescription, DisplayName.TrimStart(), Convert.ToDateTime(HolidayDate), Isactive, CreatedBy);

                    id = "Holiday Master added Successfully.";
                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }

        }


        public JsonResult EditHolidayMaster(string id, string oper, string HolidayDescription, string DisplayName, string HolidayDate, string IsActive)
        {
            bool Isactive = false;
            var date = "";
            if (oper != "del")
            {
                date = DateConversion(HolidayDate);
            }

            decimal CreatedBy;
            if (oper == "edit")
            {

                int ePKID = Convert.ToInt32(id);
                var qry = (from a in dcManageUsers.HolidayMasters where a.PKID == ePKID select a).Single();

                qry.DisplayName = DisplayName.TrimStart();
                qry.HolidayDescription = HolidayDescription.TrimStart();
                qry.HolidayDate = Convert.ToDateTime(date);

                if (IsActive == "Active")
                    qry.IsActive = true;

                if (IsActive == "Passive")
                    qry.IsActive = false;

                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                dcManageUsers.SaveChanges();

                id = "Holiday Master modified Successfully.";

            }
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in dcManageUsers.HolidayMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                dcManageUsers.SaveChanges();

                id = "Holiday Master deleted Successfully.";
            }

            return Json(id);
        }

        public string DateConversion(string ip)
        {
            string[] val = ip.Split('/');


            ip = val[2] + "-" + val[1] + "-" + val[0];
            return ip;


        }

        /* Ends */

        /* Baskar starts */


        //Role type Master excel,pdf,csv

        public ActionResult ExportRoleTypeMasterToExcel()
        {
            var context = (from a in dcManageUsers.usp_Get_CampTypeMaster() where a.GenFKID == Convert.ToInt32(Session["generalFKID"].ToString()) select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                //item.PKID.ToString(),
                item.TableName==null?"":item.TableName,
                item.GenName==null?"":item.GenName,
                item.IsActive               
            }));
            return new ExcelResult(HeadersQuestions, ColunmTypesQuestions, data, Session["TableType"].ToString() + ".xlsx", "Roletype");
        }
        private static readonly string[] HeadersQuestions = {
                 "Table type", "Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesQuestions = {
               //DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportRoleTypeMasterToPDF()
        {
            List<GeneralMasterTypes> userList = new List<GeneralMasterTypes>();
            var context = (from a in dcManageUsers.usp_Get_CampTypeMaster() where a.GenFKID == Convert.ToDecimal(Session["generalFKID"].ToString()) select a).ToList();

            foreach (var item in context)
            {
                GeneralMasterTypes user = new GeneralMasterTypes();
                user.PKID = item.PKID.ToString();
                user.TableType = item.TableName == null ? "" : item.TableName;
                user.Name = item.GenName == null ? "" : item.GenName;
                user.Status = item.IsActive;
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
                            e.PKID.ToString(),e.TableType,e.Name,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "TableType", "Name", "Status" }, path);
            return File(path, "application/pdf", Session["TableType"].ToString() + ".pdf");
        }
        public ActionResult ExportRoleTypeMasterToCsv()
        {
            var model = (from a in dcManageUsers.usp_Get_CampTypeMaster() where a.GenFKID == Convert.ToDecimal(Session["generalFKID"].ToString()) select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Table Type,Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.TableName, record.GenName, record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", Session["TableType"].ToString() + ".txt");
        }
        public class GeneralMasterTypes
        {
            public string PKID { get; set; }
            public string TableType { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
        }



        //Role Master excel,pdf,csv

        public ActionResult ExportRoleMasterToExcel()
        {
            var context = (from a in dcManageUsers.USP_GET_ROLEMASTER() select a).OrderBy(o => o.RoleName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                //item.PKID.ToString(),
                item.RoleName== null ? "" : item.RoleName,
                item.RoleType== null ? "" : item.RoleType,
                item.IsActive
               
            }));
            return new ExcelResult(HeadersRoleMaster, ColunmTypesRoleMaster, data, "RoleMaster.xlsx", "RoleMaster");
        }
        private static readonly string[] HeadersRoleMaster = {
                 "Role Name", "Role Type","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesRoleMaster = {
                //DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportRoleMasterToPDF()
        {
            List<RoleMasterTypes> userList = new List<RoleMasterTypes>();
            var context = (from a in dcManageUsers.USP_GET_ROLEMASTER() select a).OrderBy(o => o.RoleName).ToList();

            foreach (var item in context)
            {
                RoleMasterTypes user = new RoleMasterTypes();
                user.PKID = item.PKID.ToString();
                user.RoleName = item.RoleName == null ? "" : item.RoleName;
                user.RoleType = item.RoleType == null ? "" : item.RoleType;
                user.Status = item.IsActive;
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
                            e.PKID.ToString(),e.RoleName,e.RoleType,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "RoleName", "RoleType", "Status" }, path);
            return File(path, "application/pdf", "RoleMaster.pdf");
        }
        public ActionResult ExportRoleMasterToCsv()
        {
            var model = (from a in dcManageUsers.USP_GET_ROLEMASTER() select a).OrderBy(o => o.RoleName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Role Name,Role Type,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{1},{2},{3}", record.PKID, record.RoleName, record.RoleType, record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "RoleMaster.txt");
        }
        public class RoleMasterTypes
        {
            public string PKID { get; set; }
            public string RoleName { get; set; }
            public string RoleType { get; set; }
            public string Status { get; set; }
        }


        //Node Type Master excel,pdf,csv

        public ActionResult ExportNodeTypeMasterToExcel()
        {
            var context = (from a in dcManageUsers.USP_GET_MU_NODETYPEMASTER() select a).OrderBy(o => o.NODENAME).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                //item.PKID.ToString(),
                item.NODENAME==null?"":item.NODENAME,                
                item.IsActive
               
            }));
            return new ExcelResult(HeadersNodetypeMaster, ColunmTypesNodetypeMaster, data, "NodeTypeMaster.xlsx", "camptype");
        }
        private static readonly string[] HeadersNodetypeMaster = {
                 "Node Name" ,"Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesNodetypeMaster = {
                //DataForExcel.DataType.String,
                DataForExcel.DataType.String,                
                DataForExcel.DataType.String
            };
        public ActionResult ExportNodeTypeMasterToPDF()
        {
            List<NodeTypeMasterTypes> userList = new List<NodeTypeMasterTypes>();
            var context = (from a in dcManageUsers.USP_GET_MU_NODETYPEMASTER() select a).OrderBy(o => o.NODENAME).ToList();

            foreach (var item in context)
            {
                NodeTypeMasterTypes user = new NodeTypeMasterTypes();
                user.PKID = item.PKID.ToString();
                user.NodeName = item.NODENAME == null ? "" : item.NODENAME;
                user.Status = item.IsActive;
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
                            e.PKID.ToString(),e.NodeName,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "NodeName", "Status" }, path);
            return File(path, "application/pdf", "NodeTypeMaster.pdf");
        }
        public ActionResult ExportNodeTypeMasterToCsv()
        {
            var model = (from a in dcManageUsers.USP_GET_MU_NODETYPEMASTER() select a).OrderBy(o => o.NODENAME).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("NodeName,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{1},{2}", record.PKID, record.NODENAME, record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "NodeTypeMaster.txt");
        }
        public class NodeTypeMasterTypes
        {
            public string PKID { get; set; }
            public string NodeName { get; set; }
            public string Status { get; set; }
        }


        //Team Master excel,pdf,csv

        public ActionResult ExportTeamMasterToExcel()
        {
            var context = (from a in dcManageUsers.USP_GET_MU_TEAMMASTER() select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
               // item.PKID.ToString(),
                item.DivName== null ? "" : item.DivName,                
                item.TeamName== null ? "" : item.TeamName,
                item.IsActive
               
            }));
            return new ExcelResult(HeadersTeamMaster, ColunmTypesTeamMaster, data, "TeamMaster.xlsx", "TeamMaster");
        }
        private static readonly string[] HeadersTeamMaster = {
                "Division" ,"Team Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesTeamMaster = {
                //DataForExcel.DataType.String,
                DataForExcel.DataType.String,   
                DataForExcel.DataType.String,   
                DataForExcel.DataType.String
            };
        public ActionResult ExportTeamMasterToPDF()
        {
            List<TeamMasterTypes> userList = new List<TeamMasterTypes>();
            var context = (from a in dcManageUsers.USP_GET_MU_TEAMMASTER() select a).ToList();

            foreach (var item in context)
            {
                TeamMasterTypes user = new TeamMasterTypes();
                user.PKID = item.PKID.ToString();
                user.DivName = item.DivName == null ? "" : item.DivName;
                user.TeamName = item.TeamName == null ? "" : item.TeamName;
                user.Status = item.IsActive;
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
                            e.PKID.ToString(),e.DivName,e.TeamName,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "DivName", "TeamName", "Status" }, path);
            return File(path, "application/pdf", "TeamMaster.pdf");
        }
        public ActionResult ExportTeamMasterToCsv()
        {
            var model = (from a in dcManageUsers.USP_GET_MU_TEAMMASTER() select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Division,TeamName,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{1},{2},{3}", record.PKID, record.DivName, record.TeamName, record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "TeamMaster.txt");
        }
        public class TeamMasterTypes
        {
            public string PKID { get; set; }
            public string DivName { get; set; }
            public string TeamName { get; set; }
            public string Status { get; set; }
        }


        //Holiday Master excel,pdf,csv

        public ActionResult ExportHolidayMasterToExcel()
        {
            var context = (from a in dcManageUsers.USP_GET_MU_HOLIDAYMASTER() select a).OrderBy(o => o.HolidayDescription).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
              //  item.PKID.ToString(),
                item.HolidayDescription==null?"":item.HolidayDescription,                
                item.HolidayDate.ToString()==null?"":item.HolidayDate.ToString(),
                item.IsActive
               
            }));
            return new ExcelResult(HeadersHolidayMaster, ColunmTypesHolidayMaster, data, "HolidayMaster.xlsx", "HolidayMaster");
        }
        private static readonly string[] HeadersHolidayMaster = {
                "Holiday Description" ,"Holiday Date","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesHolidayMaster = {
               // DataForExcel.DataType.String,
                DataForExcel.DataType.String,   
                DataForExcel.DataType.String,   
                DataForExcel.DataType.String
            };
        public ActionResult ExportHolidayMasterToPDF()
        {
            List<HolidayMasterTypes> userList = new List<HolidayMasterTypes>();
            var context = (from a in dcManageUsers.USP_GET_MU_HOLIDAYMASTER() select a).OrderBy(o => o.HolidayDescription).ToList();

            foreach (var item in context)
            {
                HolidayMasterTypes user = new HolidayMasterTypes();
                user.PKID = item.PKID.ToString();
                user.HolidayDescription = item.HolidayDescription == null ? "" : item.HolidayDescription;
                user.HolidayDate = item.HolidayDate.ToString() == null ? "" : item.HolidayDate.ToString();
                user.Status = item.IsActive;
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
                            e.PKID.ToString(),e.HolidayDescription,e.HolidayDate,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "HolidayDescription", "HolidayDate", "Status" }, path);
            return File(path, "application/pdf", "HolidayMaster.pdf");
        }
        public ActionResult ExportHolidayMasterToCsv()
        {
            var model = (from a in dcManageUsers.USP_GET_MU_HOLIDAYMASTER() select a).OrderBy(o => o.HolidayDescription).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Holiday Description,Holiday Date,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.HolidayDescription, record.HolidayDate, record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "HolidayMaster.txt");
        }

        public class HolidayMasterTypes
        {
            public string PKID { get; set; }
            public string HolidayDescription { get; set; }
            public string HolidayDate { get; set; }
            public string Status { get; set; }
        }



        /* ends */


        #region Designation Master
        public ActionResult DesignationMaster()
        {
            return View();
        }
        public JsonResult GetDesigMaster(GridQueryModel gridQueryModel)
        {
            try
            {


                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in dcManageUsers.GetDesignationMasterDetail() select q).ToList();
                //var query = (from a in dcManageUsers.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "DesignationName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.DesignationName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DesignationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.DesignationName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DesignationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.DesignationName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DesignationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.DesignationName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DesignationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "DesignationName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DesignationName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DesignationName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DesignationName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        public JsonResult AddDesigMaster(string id, string oper, string PKID, string DesignationName, string NodeName, string Status)
        {
            try
            {


                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }

                string msg = string.Empty;

                int Nodecode = Convert.ToInt32(NodeName);
                var rslt = (from a in dcManageUsers.DesignationMasters where a.DesignationName == DesignationName && a.NodeTypeFKID == Nodecode select a).ToList();
                if (rslt.Count() > 0)
                    msg = "Designation already Exists";
                else
                {
                    DesignationMaster DesMas = new DesignationMaster();
                    //locMas.PKID = Convert.ToInt32(PKID);
                    DesMas.DesignationName = DesignationName.TrimStart();
                    //   locMas.IsActive =  Convert.ToBoolean(Status);
                    DesMas.IsActive = Convert.ToBoolean(Status);
                    DesMas.NodeTypeFKID = Convert.ToDecimal(NodeName);
                    //if (locMas.IsActive == false)
                    //    locMas.IsActive = Convert.ToBoolean(Status);
                    DesMas.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    DesMas.CreatedDate = DateTime.Now;
                    DesMas.DesignationCode = "1";
                    dcManageUsers.DesignationMasters.Add(DesMas);
                    var Result = dcManageUsers.SaveChanges();
                    // var result = dcManageUsers.DesignationMaster_Ins(DesignationName, Convert.ToInt32(NodeTypeFKID), Status, Session["USER_FKID"].ToString(), DateTime.Now);
                    //  var Result = string.Empty;
                    if (Result == 1)
                    {
                        msg = "Designation Details added Successfully.";
                    }
                }

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public JsonResult EditDesigMaster(string id, string oper, string PKID, string DesignationName, string NodeName, string Status)
        {
            try
            {
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }

                string msg = string.Empty;
                if (oper == "edit")
                {
                    Decimal ePKID = Convert.ToDecimal(id);
                    string cb = Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString();

                    var qry = (from a in dcManageUsers.DesignationMasters where a.PKID == ePKID select a).Single();

                    qry.DesignationName = DesignationName.TrimStart();
                    qry.NodeTypeFKID = Convert.ToDecimal(NodeName);
                    qry.IsActive = Convert.ToBoolean(Status);
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    qry.ModifiedDate = DateTime.Now;
                    //if (qry.IsActive == false)
                    //    qry.IsActive = Convert.ToBoolean(Status);
                    // dcManageUsers.DesignationMasters.Add(qry);
                    var result = dcManageUsers.SaveChanges();
                    if (result == 1)
                    {
                        msg = "Designation Details Updated Successfully.";
                    }

                }

                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageUsers.DesignationMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    var Result = dcManageUsers.SaveChanges();
                    if (Result == 1)
                    {
                        msg = "Designation Details deleted Successfully.";
                    }


                }

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult Nodetype()
        {
            try
            {
                var query = (from e in dcManageUsers.GetNodeTypeByXML() select new { e.NodeTypeFKID, e.NodeName }).ToDictionary(f => Convert.ToInt32(f.NodeTypeFKID), f => f.NodeName.ToString());

                return PartialView("Nodetype", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportDesignationMasterToExcel()
        {
            try
            {
                var context = (from q in dcManageUsers.GetDesignationMasterDetail() select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                //item.PKID.ToString(),
                item.DesignationName==null?"":item.DesignationName,
                item.NodeName==null?"":item.NodeName,
                item.Status
               
            }));
                return new ExcelResult(HeadersDesignation, ColunmTypesDesignation, data, "DesignationMaster.xlsx", "DesignationMaster");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersDesignation = {
                "Designation Name","Node Type", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesDesignation = {
               // DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportDesignationMasterToPDF()
        {
            try
            {
                List<DesignMas> userList = new List<DesignMas>();
                var context = (from q in dcManageUsers.GetDesignationMasterDetail() select q).ToList();

                foreach (var item in context)
                {
                    DesignMas user = new DesignMas();
                    user.PKID = item.PKID.ToString();
                    user.DesignationName = item.DesignationName == null ? "" : item.DesignationName;
                    user.NodeType = item.NodeName == null ? "" : item.NodeName;
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
                            e.PKID.ToString(),e.DesignationName,e.NodeType,e.Status
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] { "DesignationName", "NodeType", "Status" }, path);
                return File(path, "application/pdf", "Designation.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportDesignationMasterCsv()
        {
            try
            {
                var model = (from q in dcManageUsers.GetDesignationMasterDetail() select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Designation Name,Node Type,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2}", record.DesignationName, record.NodeName, record.Status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "DesignationMaster.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class DesignMas
        {
            public string PKID { get; set; }
            public string DesignationName { get; set; }
            public string NodeType { get; set; }
            public string Status { get; set; }
        }
        #endregion


        //#region Holiday Group link
        //public ActionResult HolidayGrouplink()
        //{

        //    return View();
        //}
        //public JsonResult GetHolidayGrouplink(GridQueryModel gridQueryModel)
        //{
        //    try
        //    {


        //        //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
        //        var searchString = gridQueryModel.searchString;
        //        var searchOper = gridQueryModel.searchOper;
        //        var searchField = gridQueryModel.searchField;
        //        var query = (from q in dcManageUsers.GetHolidaygroup() select q).ToList();
        //        //var query = (from a in dcManageUsers.LocationMasters select a).ToList();
        //        var count = query.Count();
        //        var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


        //        if (gridQueryModel._search == true && searchString != "")
        //        {
        //            if ((searchField == "HolidayGroupName"))
        //            {
        //                if (searchOper == "bw")//begins with
        //                {
        //                    var q = (from sq in query where sq.HolidayGroupName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
        //                    count = q.Count();
        //                    pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //                }
        //                else if (searchOper == "eq") //equal
        //                {
        //                    var q = (from sq in query where sq.HolidayGroupName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
        //                    count = q.Count();
        //                    pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //                }
        //                else if (searchOper == "ew") // ends with
        //                {
        //                    var q = (from sq in query where sq.HolidayGroupName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
        //                    count = q.Count();
        //                    pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //                }
        //                else if (searchOper == "cn")//contains
        //                {
        //                    var q = (from sq in query where sq.HolidayGroupName.Contains(searchString) select sq).ToList();
        //                    count = q.Count();
        //                    pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //                }

        //            }

        //        }
        //        else
        //        {
        //            count = query.Count();
        //            if (gridQueryModel.sidx == "HolidayGroupName" && gridQueryModel.sord == "asc")
        //                pageData = (from cust in query orderby cust.HolidayGroupName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //            else
        //                if (gridQueryModel.sidx == "HolidayGroupName" && gridQueryModel.sord == "desc")
        //                    pageData = (from cust in query orderby cust.HolidayGroupName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //        }

        //        return Json(new
        //        {
        //            page = gridQueryModel.page,
        //            records = count,
        //            rows = pageData,

        //            total = Math.Ceiling((decimal)count / gridQueryModel.rows)
        //        }, JsonRequestBehavior.AllowGet);


        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { error = ex.Message });
        //    }

        //}
        //public JsonResult EditHolidayGrouplink(string id, string oper, string HolidaygroupFKID, string HolidayGroupName, string HolidayGrpName, string HolidayDescription)
        //{
        //    try
        //    {
        //        string msg = string.Empty;
        //        decimal HFKID = Convert.ToDecimal(HolidaygroupFKID);
        //        int createdby = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
        //        if (oper == "edit")
        //        {


        //            // var qry = (from q in dcManageUsers.LocationMasters select q).ToList();
        //            var query = dcManageUsers.EditHolidayGroupLinkMaster(HFKID, HolidayGroupName, createdby);
        //            var result = dcManageUsers.SaveChanges();
        //            if (result == 1)
        //            {
        //                msg = "Holiday Group Link Master Edited Successfully.";
        //            }

        //        }
        //        if (oper == "add")
        //        {

        //            var query = dcManageUsers.AddHolidayGroupLinkMaster(HFKID, HolidayGroupName, createdby);
        //            //  var Result = dcManageUsers.SaveChanges();
        //            //  var Result = string.Empty;
        //            if (query == 1)
        //            {
        //                msg = "Holiday Group Link Master Added Successfully.";
        //            }

        //        }

        //        return Json(msg);
        //    }
        //    catch (Exception ex)
        //    { return Json(new { error = ex.Message }); }
        //}
        //public ActionResult GetHolidayGroupName()
        //{
        //    try
        //    {

        //        var query = (from e in dcManageUsers.GetHolidayGroupNameByXml() select new { e.PKID, HolidayGrpName = e.HolidayGroupName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.HolidayGrpName.ToString());

        //        return PartialView("PVHolidayGroup", query);
        //    }
        //    catch (Exception ex)
        //    { return Json(new { error = ex.Message }); }

        //}
        //public ActionResult GetHolidayDescription()
        //{
        //    try
        //    {
        //        var query = (from e in dcManageUsers.GetHolidayDescriptionByXml() select new { e.HolidayFKID, e.HolidayDescription }).ToDictionary(f => Convert.ToInt32(f.HolidayFKID), f => f.HolidayDescription.ToString());

        //        return PartialView("PVHolidayGroup", query);
        //    }
        //    catch (Exception ex)
        //    { return Json(new { error = ex.Message }); }

        //}
        //public ActionResult ExportHolidayGrouplinkToExcel()
        //{
        //    try
        //    {
        //        var context = (from q in dcManageUsers.GetHolidaygroup() select q).ToList();
        //        if (context.Count == 0)
        //            return new EmptyResult();
        //        var data = new List<string[]>(context.Count);
        //        data.AddRange(context.Select(item => new[] {
        //        item.HolidaygroupFKID.ToString(),
        //        item.HolidayGroupName


        //    }));
        //        return new ExcelResult(HeadersHolidayGrp, ColunmTypesHolidayGrp, data, "HolidayGroupLink.xlsx", "HolidayGroupLink");
        //    }
        //    catch (Exception ex)
        //    { return Json(new { error = ex.Message }); }
        //}
        //private static readonly string[] HeadersHolidayGrp = {
        //        "HolidaygroupFKID", "HolidayGroupName"
        //    };
        //private static readonly DataForExcel.DataType[] ColunmTypesHolidayGrp = {
        //        DataForExcel.DataType.String,
        //        DataForExcel.DataType.String

        //    };
        //public ActionResult ExportHolidayGrouplinkToPDF()
        //{
        //    try
        //    {
        //        List<HolidayGrp> userList = new List<HolidayGrp>();
        //        var context = (from q in dcManageUsers.GetHolidaygroup() select q).ToList();

        //        foreach (var item in context)
        //        {
        //            HolidayGrp user = new HolidayGrp();
        //            user.HolidaygroupFKID = item.HolidaygroupFKID.ToString();
        //            user.HolidayGroupName = item.HolidayGroupName;
        //            userList.Add(user);
        //        }
        //        var customerList = userList;

        //        var result = new
        //        {
        //            total = 1,
        //            page = 1,
        //            records = customerList.Count(),
        //            rows = (
        //                customerList.Select(e =>
        //                    new
        //                    {
        //                        id = e.HolidaygroupFKID,
        //                        cell = new string[]{
        //                    e.HolidaygroupFKID.ToString(),e.HolidayGroupName
        //                }
        //                    })
        //            ).ToArray()
        //        };
        //        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Sample3.pdf");
        //        pdf.ExportPDF(customerList, new string[] { "HolidaygroupFKID", "HolidayGroupName" }, path);
        //        return File(path, "application/pdf", "HolidayGroupLink.pdf");
        //    }
        //    catch (Exception ex)
        //    { return Json(new { error = ex.Message }); }
        //}

        //public class HolidayGrp
        //{
        //    public string HolidaygroupFKID { get; set; }
        //    public string HolidayGroupName { get; set; }
        //}

        //#endregion


        #region User Team Link
        public ActionResult UserTeamLink()
        {

            return View();
        }
        public JsonResult GetUserTeamLink(GridQueryModel gridQueryModel)
        {
            try
            {
                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in dcManageUsers.getUserTeamLink() select q).ToList();
                //var query = (from a in dcManageUsers.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "TeamName") || searchField == "NodeName" || searchField == "UserName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.TeamName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.NodeName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.UserName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.TeamName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.TeamName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.NodeName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.UserName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.TeamName.ToUpper().Contains(searchString) || sq.NodeName.Contains(searchString) || sq.UserName.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.UserName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.UserName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        public JsonResult AddUserTeamLink(string Status, string TeamName, string SelUserName, string NodeName)
        {
            string str = "", msg = "";
            if (Status == "Active")
            {
                Status = "True";
            }
            else
            {
                Status = "False";
            }
            try
            {
                str = "<root>";
                string StrSelectedSelUserName = SelUserName.TrimStart();
                string[] SelectedUserNamevalues = StrSelectedSelUserName.Split(',');
                for (int i = 0; i < SelectedUserNamevalues.Length; i++)
                {
                    str += "<TeamUserLink";
                    str += " UserFKID ='" + SelectedUserNamevalues[i] + "'";
                    str += "/>";
                }
                str += "</root>";

                int query = dcManageUsers.USerTeamLinkAddLatest(Convert.ToDecimal(TeamName), Convert.ToDecimal(NodeName), str, Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()), Convert.ToBoolean(Status));

                if (query == 1)
                {
                    msg = "User Team Link Added Successfully.";
                }
                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult EditUserTeamLinks(string id, string oper, string Status, string TeamName, string SelUserName, string NodeName)
        {
            try
            {
                string msg = string.Empty;
                if (oper == "del")
                {
                    int result = dcManageUsers.USerTeamLinkDelete(Convert.ToDecimal(id));

                    if (result == 1)
                    {
                        msg = "User Team Link deleted Successfully.";
                    }
                }

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }); 
            }
        }

        public JsonResult EditUserTeamLink(string id, string oper, string Status, string TeamName, string SelUserName, string NodeName)
        {
            string str = "";
            try
            {

                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }
                string msg = string.Empty;

                int createdby = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                if (oper == "edit")
                {

                    str = "<root>";
                    string StrSelectedSelUserName = SelUserName.TrimStart();
                    string[] SelectedUserNamevalues = StrSelectedSelUserName.Split(',');
                    for (int i = 0; i < SelectedUserNamevalues.Length; i++)
                    {
                        str += "<TeamUserLink";
                        str += " UserFKID ='" + SelectedUserNamevalues[i] + "'";
                        str += "/>";
                    }
                    str += "</root>";

                    int result = dcManageUsers.USerTeamLinkAddNew(Convert.ToDecimal(TeamName), Convert.ToDecimal(NodeName), str, createdby, Convert.ToBoolean(Status));

                    if (result >= 1)
                    {
                        msg = "User Team Link Edited Successfully.";
                    }

                }

                if (oper == "del")
                {
                    int result = dcManageUsers.USerTeamLinkDelete(Convert.ToDecimal(id));

                    if (result == 1)
                    {
                        msg = "User Team Link deleted Successfully.";
                    }
                }

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        public ActionResult GetTeam(string Type)
        {
            ViewBag.Type = Type;
            try
            {
                var query = (from e in dcManageUsers.GetTeamsByXML() select new { e.PKID, e.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName);
                return PartialView("PVUserTeamLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }
        // public ActionResult GetNodeType(string Type, string rowId, string TeamFKID, string NodeTypeFKID)

        public ActionResult GetNodeType(string Type)
        {
            ViewBag.Type = Type;
            dynamic query;
            try
            {
                //if (rowId == "undefined")
                //{
                query = (from e in dcManageUsers.GetNodeTypeByXML() select new { e.NodeTypeFKID, e.NodeName }).ToDictionary(f => Convert.ToInt32(f.NodeTypeFKID), f => f.NodeName);
                return PartialView("PVUserTeamLink", query);
                //}
                //else
                //{
                //    query = (from a in dcManageUsers.GetLinkUserTeamMasterforDrpuserfillByXml(Convert.ToDecimal(TeamFKID), Convert.ToDecimal(NodeTypeFKID)) where a.Element == Type select new { a.UserFKID, a.EmpName }).ToDictionary(f => Convert.ToInt32(f.UserFKID), f => f.EmpName.ToString());
                //    return PartialView("PVUserTeamLink", query);
                //}
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }

        public ActionResult GetUserName(string Type, string PKID, string TeamFKID, string NodeTypeFKID)
        {
            ViewBag.Type = Type;
            dynamic query;
            NodeTypeFKID = (NodeTypeFKID == "undefined" || NodeTypeFKID == null || NodeTypeFKID == "") ? "0" : NodeTypeFKID;
            try
            {
                if (PKID == "undefined" || PKID == null)
                {
                    query = (from e in dcManageUsers.GetEmpNameByNodeTypeFKIDXml(Convert.ToInt32(NodeTypeFKID)) select new { e.EmpFKID, e.EmpName }).ToDictionary(f => Convert.ToInt32(f.EmpFKID), f => f.EmpName);
                    return PartialView("PVUserTeam", query);
                }
                else
                {
                    query = (from a in dcManageUsers.GetLinkUserTeamMasterforDrpuserfillByXml(Convert.ToDecimal(TeamFKID), Convert.ToDecimal(NodeTypeFKID)) where a.Element == Type select new { a.UserFKID, a.EmpName }).ToDictionary(f => Convert.ToInt32(f.UserFKID), f => f.EmpName.ToString());
                    return PartialView("PVUserTeam", query);
                }
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }



        public ActionResult GetUser(string NodeTypeFKID, string Type, string TeamFKID)
        {
            ViewBag.Type = Type;
            dynamic query;
            try
            {
                query = (from e in dcManageUsers.GetEmpNameByNodeTypeFKIDXml(Convert.ToInt32(NodeTypeFKID)) select new { e.EmpFKID, e.EmpName }).ToDictionary(f => Convert.ToInt32(f.EmpFKID), f => f.EmpName.ToString());
                return PartialView("PVUserTeamLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        public ActionResult EditGetUser(string NodeTypeFKID, string Type, string TeamFKID)
        {
            ViewBag.Type = Type;
            var test = (from a in dcManageUsers.GetLinkUserTeamMasterforDrpuserfillByXml(Convert.ToDecimal(TeamFKID), Convert.ToDecimal(NodeTypeFKID)) where a.Element == Type select new { a.UserFKID, a.EmpName }).ToDictionary(f => Convert.ToInt32(f.UserFKID), f => f.EmpName.ToString());
            return PartialView("PVUserTeamLink", test);
        }
        public ActionResult EditSelGetUser(string NodeTypeFKID, string Type, string TeamFKID)
        {
            ViewBag.Type = Type;
            var test = (from a in dcManageUsers.GetLinkUserTeamMasterforDrpuserfillByXml(Convert.ToDecimal(TeamFKID), Convert.ToDecimal(NodeTypeFKID)) where a.Element == Type select new { a.UserFKID, a.EmpName }).ToDictionary(f => Convert.ToInt32(f.UserFKID), f => f.EmpName.ToString());
            return PartialView("PVUserTeamLink", test);
        }


        public ActionResult ExportUserTeamLinkToExcel()
        {
            try
            {
                var context = (from q in dcManageUsers.getUserTeamLink() select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.TeamName== null ? "" : item.TeamName,
                item.NodeName== null ? "" : item.NodeName,
                item.UserName== null ? "" : item.UserName,
                item.Status
                
               
            }));
                return new ExcelResult(HeadersUserTeamLink, ColunmTypesUserTeamLink, data, "UserTeamLink.xlsx", "UserTeamLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        public ActionResult SelectedUserName(string Type, string id)
        {
            ViewBag.Type = Type;
            dynamic query = "";//GetLinkUserTeamMasterforDrpuserfillByXml
            query = (from a in dcManageUsers.GetAllTeamRegionLinkByXml("0") where 1 == 2 select new { a.RegionFKID, a.RegionName }).ToDictionary(f => Convert.ToInt32(f.RegionFKID), f => f.RegionName.ToString());
            return PartialView("PVUserTeamLink", query);
        }


        private static readonly string[] HeadersUserTeamLink = {
                "TeamName", "NodeName","UserName","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesUserTeamLink = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
               
            };
        public ActionResult ExportUserTeamLinkToPDF()
        {
            try
            {
                List<clsUserTeamLink> userList = new List<clsUserTeamLink>();
                var context = (from q in dcManageUsers.getUserTeamLink() select q).ToList();

                foreach (var item in context)
                {
                    clsUserTeamLink user = new clsUserTeamLink();
                    user.TeamFkid = item.TeamFKID.ToString() == null ? "" : item.TeamFKID.ToString();
                    user.TeamName = item.TeamName == null ? "" : item.TeamName;
                    user.NodeName = item.NodeName == null ? "" : item.NodeName;
                    user.UserName = item.UserName == null ? "" : item.UserName;
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
                                id = e.TeamFkid,
                                cell = new string[]{
                            e.TeamName,e.NodeName,e.UserName,e.Status
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] { "TeamName", "NodeName", "UserName", "Status" }, path);
                return File(path, "application/pdf", "UserTeamLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportUserTeamLinkCsv()
        {
            try
            {
                var model = (from q in dcManageUsers.getUserTeamLink() select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("TeamName,NodeName,UserName,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record.TeamName, record.NodeName, record.UserName, record.Status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "UserTeamLink.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class clsUserTeamLink
        {
            public string TeamFkid { get; set; }
            public string TeamName { get; set; }
            public string NodeName { get; set; }
            public string UserName { get; set; }
            public string Status { get; set; }
        }

        #endregion
        #region Employee Master

        public ActionResult EmployeeMaster()
        {

            return View();
        }



        public JsonResult GetEmployeeMaster(GridQueryModel gridQueryModel)
        {
            try
            {

                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from q in dcManageUsers.GetEmployeMaster() select q).ToList();
                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "EmpCode") || (searchField == "EmpName") || (searchField == "DesignationName") || (searchField == "CityName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query
                                     where (sq.EmpCode != null && sq.EmpCode.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)) || (sq.EmpName != null && sq.EmpName.ToUpper().StartsWith(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase)) || (sq.DesignationName != null && sq.DesignationName.ToUpper().StartsWith(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase)) || (sq.CityName != null && sq.CityName.ToUpper().StartsWith(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase))
                                     select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.EmpCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {

                            var q = (from sq in query where (sq.EmpCode != null && sq.EmpCode.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase)) || (sq.EmpName != null && sq.EmpName.ToUpper().Equals(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase)) || (sq.DesignationName != null && sq.DesignationName.ToUpper().Equals(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase)) || (sq.CityName != null && sq.CityName.ToUpper().Equals(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase)) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.EmpCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where (sq.EmpCode != null && sq.EmpCode.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase)) || (sq.EmpName != null && sq.EmpName.ToUpper().EndsWith(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase)) || (sq.DesignationName != null && sq.DesignationName.ToUpper().EndsWith(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase)) || (sq.CityName != null && sq.CityName.ToUpper().EndsWith(searchString.ToUpper(), StringComparison.CurrentCultureIgnoreCase)) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.EmpCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where (sq.EmpCode != null && sq.EmpCode.ToUpper().Contains(searchString)) || (sq.EmpName != null && sq.EmpName.ToUpper().Contains(searchString)) || (sq.DesignationName != null && sq.DesignationName.ToUpper().Contains(searchString)) || (sq.CityName != null && sq.CityName.ToUpper().Contains(searchString)) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.EmpCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "EmpName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.EmpName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "EmpName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.EmpName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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


        public JsonResult AddEmployeeMaster(string id, string oper, string PKID, string TitleFKID, string FirstName, string MiddleName, string LastName, string ShortName, string Gender, string DOB, string DOJ, string Address, string Address1, string Address2, string Street, string Area, string Zipcode, string ISDCode, string STDCode, string PhoneNo, string Mobile, string Email, string HQ, string MaritalStatusFKID, string OldEmpCode, string EmpCode, string EmpName, string DesignationFKID, string DesignationName, string CityName, string Status)
        {
            try
            {
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }
                TitleFKID = "1";
                Address = Address1 + "," + Address2;
                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string msg = string.Empty;

                var rslt = (from a in dcManageUsers.EmployeeMasters where a.EmpCode == EmpCode select a).ToList();
                if (rslt.Count() > 0)
                    msg = "Employee Master already Exists";
                else
                {
                    DateTime dob = DateTime.Parse(DOB);
                    DateTime hdate = DateTime.Parse(DOJ);

                    EmpCode = EmpCode.TrimStart();
                    FirstName = FirstName.TrimStart();
                    LastName = LastName.TrimStart();
                    ShortName = ShortName.TrimStart();
                    Address = Address.TrimStart();
                    OldEmpCode = OldEmpCode.TrimStart();
                    Zipcode = Zipcode.TrimStart();

                    int Result = dcManageUsers.AddEmployeeMaster(EmpCode, Convert.ToDecimal(TitleFKID), FirstName, MiddleName, LastName, ShortName, Gender, dob, hdate, Address, Street, CityName, Zipcode, PhoneNo, Mobile, Email, HQ, Convert.ToDecimal(MaritalStatusFKID), Convert.ToDecimal(DesignationName), OldEmpCode, Convert.ToInt32(ISDCode), Convert.ToInt32(STDCode), Convert.ToBoolean(Status), ModifiedBy);

                    if (Result == 1)
                    {
                        msg = "Employee Master added Successfully.";
                    }
                }

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }

        }



        public JsonResult EditEmployeeMaster(string id, string oper, string PKID, string TitleFKID, string FirstName, string MiddleName, string LastName, string ShortName, string Gender, string DOB, string DOJ, string Address, string Address1, string Address2, string Street, string Area, string Zipcode, string ISDCode, string STDCode, string PhoneNo, string Mobile, string Email, string HQ, string MaritalStatusFKID, string OldEmpCode, string EmpCode, string EmpName, string DesignationFKID, string DesignationName, string CityName, string Status)
        {
            try
            {
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }
                TitleFKID = "1";
                Address = Address1 + "," + Address2;
                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                string msg = string.Empty;
                if (oper == "edit")
                {
                    
                    DateTime dob = DateTime.Parse(DOB);
                    DateTime hdate = DateTime.Parse(DOJ);
                    Decimal ePKID = Convert.ToDecimal(id);

                    EmpCode = EmpCode.TrimStart();
                    FirstName = FirstName.TrimStart();
                    LastName = LastName.TrimStart();
                    ShortName = ShortName.TrimStart();
                    Address = Address.TrimStart();
                    OldEmpCode = OldEmpCode.TrimStart();
                    Zipcode = Zipcode.TrimStart();


                    int result = dcManageUsers.EditEmployeeMaster(ePKID, EmpCode, Convert.ToDecimal(TitleFKID), FirstName, MiddleName, LastName, ShortName, Gender, dob, hdate, Address, Street, CityName, Zipcode, PhoneNo, Mobile, Email, HQ, Convert.ToDecimal(MaritalStatusFKID), Convert.ToDecimal(DesignationName), OldEmpCode, Convert.ToInt32(ISDCode), Convert.ToInt32(STDCode), Convert.ToBoolean(Status), ModifiedBy);

                    if (result == 1)
                    {
                        msg = "Employee Master Updated Successfully";
                    }

                }
                //if (oper == "add")
                //{

                //    var rslt = (from a in dcManageUsers.EmployeeMasters where a.EmpCode == EmpCode select a).ToList();
                //    if (rslt.Count() > 0)
                //        msg = "Employee Master already Exists";
                //    else
                //    {
                //        DateTime dob = DateTime.Parse(DOB);
                //        DateTime hdate = DateTime.Parse(DOJ);

                //        EmpCode = EmpCode.TrimStart();
                //        FirstName = FirstName.TrimStart();
                //        LastName = LastName.TrimStart();
                //        ShortName = ShortName.TrimStart();
                //        Address = Address.TrimStart();
                //        OldEmpCode = OldEmpCode.TrimStart();
                //        Zipcode = Zipcode.TrimStart();

                //        int Result = dcManageUsers.AddEmployeeMaster(EmpCode, Convert.ToDecimal(TitleFKID), FirstName, MiddleName, LastName, ShortName, Gender, dob, hdate, Address, Street, CityName, Zipcode, PhoneNo, Mobile, Email, HQ, Convert.ToDecimal(MaritalStatusFKID), Convert.ToDecimal(DesignationName), OldEmpCode, Convert.ToInt32(ISDCode), Convert.ToInt32(STDCode), Convert.ToBoolean(Status), ModifiedBy);

                //        if (Result == 1)
                //        {
                //            msg = "Employee Master added Successfully.";
                //        }
                //    }

                //}
                if (oper == "del")
                {

                    int Result = dcManageUsers.DeleteEmployeeMaster(Convert.ToDecimal(id), ModifiedBy);

                    msg = "Employee Master deleted Successfully.";


                }

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }
        public ActionResult ExportEmployeeMasterToExcel()
        {
            try
            {

                var context = (from q in dcManageUsers.GetEmployeMaster() select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.EmpCode== null ? "" : item.EmpCode,
                item.EmpName == null ? "" : item.EmpName,
                item.DesignationName== null ? "" : item.DesignationName,
                item.CityName== null ? "" : item.CityName,
                item.Status
               
            }));
                return new ExcelResult(HeadersEmployeeMaster, ColunmTypesEmployeeMaster, data, "EmployeeMaster.xlsx", "EmployeeMaster");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersEmployeeMaster = {
                 "EmployeeCode", "EmployeeName", "Designation", "City", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesEmployeeMaster = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportEmployeeMasterToPDF()
        {
            try
            {
                List<EmployeeMas> userList = new List<EmployeeMas>();

                var context = (from q in dcManageUsers.GetEmployeMaster() select q).ToList();

                foreach (var item in context)
                {
                    EmployeeMas user = new EmployeeMas();
                    user.EmployeeCode = item.EmpCode == null ? "" : item.EmpCode;
                    user.EmployeeName = item.EmpName == null ? "" : item.EmpName;
                    user.Designation = item.DesignationName == null ? "" : item.DesignationName;
                    user.City = item.CityName == null ? "" : item.CityName;
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
                                id = e.EmployeeCode,
                                cell = new string[]{
                            e.EmployeeCode.ToString(),e.EmployeeName,e.Designation,e.City,e.Status
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] { "EmployeeCode", "EmployeeName", "Designation", "City", "Status" }, path);
                return File(path, "application/pdf", "EmployeeMaster.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportEmployeeMasterToCsv()
        {
            try
            {

                var model = (from q in dcManageUsers.GetEmployeMaster() select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Employee Code,Employee Name,Designation,City,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4}", record.EmpCode, record.EmpName, record.DesignationName, record.CityName, record.Status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "EmployeeMaster.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }


        public ActionResult GetCityByXML()
        {
            try
            {

                var query = (from e in dcManageUsers.GetCityByXML() select new { e.CityID, e.CityName }).ToDictionary(f => Convert.ToInt32(f.CityID), f => f.CityName);

                return PartialView("PVEmpMaster", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }
        public ActionResult GetDesignationByXML()
        {
            try
            {

                var query = (from e in dcManageUsers.GetDesignationByXML() select new { e.DesignationFKID, e.DesignationName }).ToDictionary(f => Convert.ToInt32(f.DesignationFKID), f => f.DesignationName);

                return PartialView("PVEmpMaster", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }
        public class EmployeeMas
        {
            public string EmployeeCode { get; set; }
            public string EmployeeName { get; set; }
            public string Designation { get; set; }
            public string City { get; set; }
            public string Status { get; set; }
        }

        #endregion



        #region Holiday Group link

        #region  Holiday Group link Action Control

        public ActionResult HolidayGrouplink()
        {

            return View();
        }

        #endregion

        #region Bind The data to grid

        public JsonResult GetHolidayGrouplink(GridQueryModel gridQueryModel)
        {
            try
            {


                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in dcManageUsers.GetHolidaygroup() select q).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "HolidayGroupName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.HolidayGroupName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.HolidayGroupName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.HolidayGroupName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.HolidayGroupName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.HolidayGroupName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "HolidayGroupName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.HolidayGroupName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "HolidayGroupName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.HolidayGroupName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        #region HolyDay Group Link Add, Edit

        public JsonResult AddHolidayGroupLink(string id, string oper, string HolidaygroupFKID, string SelHolidayDescription, string HolidayGroupName)
        {
            string xml = string.Empty;
            string[] arrayTerritory = SelHolidayDescription.Split(',');
            StringBuilder sb = new StringBuilder();
            sb.Append("<root>");
            for (int i = 0; i < arrayTerritory.Count(); i++)
            {
                sb.Append("<CompBrand HolidayFKID='" + arrayTerritory[i]);
                sb.Append("'/>");

            }
            sb.Append("</root>");

            try
            {
                dcManageUsers.AddHolidayGroupLinkMaster(Convert.ToDecimal(HolidayGroupName), sb.ToString(), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                id = "Holiday Group Link Master Added Successfully.";
                return Json(id);
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        public JsonResult EditHolidayGroupLink(string id, string oper, string HolidaygroupFKID, string SelHolidayDescription, string HolidayGroupName)
        {
            string xml = string.Empty;
            string[] arrayTerritory = SelHolidayDescription.Split(',');
            StringBuilder sb = new StringBuilder();
            sb.Append("<root>");
            for (int i = 0; i < arrayTerritory.Count(); i++)
            {
                sb.Append("<CompBrand HolidayFKID='" + arrayTerritory[i]);
                sb.Append("'/>");

            }
            sb.Append("</root>");

            try
            {

                if (oper == "edit")
                {
                    dcManageUsers.EditHolidayGroupLinkMaster(Convert.ToDecimal(HolidaygroupFKID), sb.ToString(), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                    id = "Holiday Group Link Master Edited Successfully.";
                }


                return Json(id);
            }

            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        #endregion

        #region Create PartialView

        public ActionResult LoadHolidayGroup(string Type, string rowId)
        {
            try
            {
                ViewBag.Type = Type;
                var query = (from e in dcManageUsers.GetHolidayGroupNameByXml() select new { e.PKID, HolidayGrpName = e.HolidayGroupName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.HolidayGrpName.ToString());

                return PartialView("PVHolidayGroup", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }

        public ActionResult LoadHolidayDescription(string Type, string rowId)
        {
            try
            {
                ViewBag.Type = Type;
                var query = (from e in dcManageUsers.GetHolidayDescriptionByXml() select new { e.HolidayFKID, e.HolidayDescription }).ToDictionary(f => Convert.ToInt32(f.HolidayFKID), f => f.HolidayDescription.ToString());

                return PartialView("PVHolidayGroup", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }

        public ActionResult LoadselHolidayDescription(string type, string HolidaygroupFKID)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (HolidaygroupFKID == "undefined")
            {
                query = (from a in dcManageUsers.GetMoleculeforProductByXML() where 1 == 2 select new { a.MoleculeID, a.MoleculeName }).ToDictionary(f => Convert.ToInt32(f.MoleculeID), f => f.MoleculeName.ToString());
            }
            else
            {
                query = (from a in dcManageUsers.GetHolidayGroupLinkmasterByXml(Convert.ToDecimal(HolidaygroupFKID)) select new { a.HolidayFKID, a.HolidayDescription }).ToDictionary(f => Convert.ToInt32(f.HolidayFKID), f => f.HolidayDescription.ToString());
            }


            return PartialView("PVHolidayGroup", query);
        }

        #endregion

        #region Covertion For Excel, PDF, CSV

        #region Convert To Excel

        public ActionResult ExportHolidayGrouplinkToExcel()
        {
            try
            {
                var context = (from q in dcManageUsers.GetHolidaygroup() select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                //item.HolidaygroupFKID.ToString(),
                item.HolidayGroupName== null ? "" : item.HolidayGroupName
                
               
            }));
                return new ExcelResult(HeadersHolidayGrp, ColunmTypesHolidayGrp, data, "HolidayGroupLink.xlsx", "HolidayGroupLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        private static readonly string[] HeadersHolidayGrp = {
                 "HolidayGroupName"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesHolidayGrp = {
               // DataForExcel.DataType.String,
                DataForExcel.DataType.String
               
            };
        #endregion

        #region Convert To PDF

        public ActionResult ExportHolidayGrouplinkToPDF()
        {
            try
            {
                List<HolidayGrp> userList = new List<HolidayGrp>();
                var context = (from q in dcManageUsers.GetHolidaygroup() select q).ToList();

                foreach (var item in context)
                {
                    HolidayGrp user = new HolidayGrp();
                    user.HolidaygroupFKID = item.HolidaygroupFKID.ToString();
                    user.HolidayGroupName = item.HolidayGroupName == null ? "" : item.HolidayGroupName;
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
                                id = e.HolidaygroupFKID,
                                cell = new string[]{
                            e.HolidaygroupFKID.ToString(),e.HolidayGroupName
                        }
                            })
                    ).ToArray()
                };
                pdf.ExportPDF(customerList, new string[] { "HolidayGroupName" }, path);
                return File(path, "application/pdf", "HolidayGroupLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        #endregion

        #region Convert To CSV

        public ActionResult ExportHolidayGroupLinkToCsv()
        {
            var model = dcManageUsers.GetHolidaygroup().Select(x => new
            {
                x.HolidaygroupFKID,
                x.HolidayGroupName,
            }).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("HolidayGroupName");
            foreach (var record in model)
            {
                sb.AppendFormat("{0}", record.HolidayGroupName

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "HolidayGroupLink.txt");
        }

        #endregion

        #region PDF Class File

        public class HolidayGrp
        {
            public string HolidaygroupFKID { get; set; }
            public string HolidayGroupName { get; set; }
        }

        #endregion

        #endregion

        #endregion


    }
}
