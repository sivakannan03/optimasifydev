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
using Sify.Global;



namespace Pfizer.Controllers
{
    public class MastersController : Controller
    {
        #region Declarations :
        ExportPdf pdf = new ExportPdf();
        private pfizerEntities genData = new pfizerEntities();
        string path = ConfigurationSettings.AppSettings["PDFfilePath"].ToString();
        PfizerBL bLayer = new PfizerBL();
        string xslPath = ConfigurationManager.AppSettings["xslPath"].ToString();
        #endregion

        public ActionResult Index()
        {
            return View();
        }

        //Hospital Master 
        public ActionResult HospitalMaster()
        {
            return View();
        }

        public JsonResult GridDataHospitalMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                var query = (from a in genData.GetHospitalMaster_PSO(psoFkid) select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "NameofHospital"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.NameofHospital != null && sq.NameofHospital.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.NameofHospital != null && sq.NameofHospital.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.NameofHospital != null && sq.NameofHospital.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.NameofHospital != null && sq.NameofHospital.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "NameofHospital" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.NameofHospital ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "NameofHospital" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.NameofHospital descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "HotelAddress" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.HotelAddress ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "HotelAddress" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.HotelAddress descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "City" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.City ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "City" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.City descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Street" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Street ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Street" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Street descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "PhoneNumber" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.PhoneNumber ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "PhoneNumber" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.PhoneNumber descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "Email" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Email" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Status descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Status ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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
        public JsonResult AddHospitalMaster(string NameofHospital, string HotelAddress, string Street, string City, string Area, string ZipCode, string PhoneNumber, string Email)
        {

            try
            {
                string msg = string.Empty;
                decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                int resule = genData.sp_insert_hospitaldetails(NameofHospital, HotelAddress, Street, Convert.ToInt32(City), Convert.ToInt32(Area), Convert.ToDecimal(ZipCode), PhoneNumber, Email, psoFkid);
                if (resule >= 1)
                {

                    msg = "Hospital Master Added Successfully.";
                }
                return Json(msg);

            }
            catch (Exception ex)
            {
                return Json("Hospital Master already Exists");
            }
        }


        public JsonResult EditHospitalMaster(string id, string oper, string NameofHospital, string HotelAddress, string Street, string City, string Area, string ZipCode, string PhoneNumber, string Email, string Status)
        {
            try
            {
                string msg = string.Empty;

                if (oper == "edit")
                {
                    int ePKID = Convert.ToInt32(id);
                    var query = (from a in genData.Masters_Hospitalmaster_tb where a.HospitalId == ePKID select a).Single();
                    if (Status == "Active")
                    {
                        query.IsActive = true;
                    }
                    else
                    {
                        query.IsActive = false;
                    }
                    query.NameofHospital = NameofHospital.ToString().TrimStart();
                    query.HotelAddress = HotelAddress.ToString().TrimStart();
                    query.Street = Street.ToString().TrimStart();
                    query.City = Convert.ToInt32(City);
                    query.Area = Convert.ToInt32(Area);
                    query.ZipCode = Convert.ToDecimal(ZipCode);
                    query.PhoneNumber = PhoneNumber.ToString();
                    query.Email = Email.ToString();
                    genData.SaveChanges();
                    msg = "Hospital Master modified Successfully.";

                }

                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    genData.sp_delete_Hospitaldetails(quote);
                    msg = "Hospital Master deleted Successfully.";
                }
                return Json(msg);
            }

            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }


        }
        //Export Excel ,PDF and CSV file download
        public ActionResult ExportHospitalMasToExcel()
        {
            try
            {
                decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                var context = (from a in genData.GetHospitalMaster_PSO(psoFkid) orderby a.NameofHospital select a).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                  item.NameofHospital.ToString()==null?"":item.NameofHospital.ToString(),
                  item.HotelAddress.ToString()==null?"":item.HotelAddress.ToString(),
                  item.Street.ToString()==null?"":item.Street.ToString(),
                  item.City==null?"":item.City,
                  item.ZipCode.ToString()==null?"":item.ZipCode.ToString(),
                  item.PhoneNumber.ToString()==null?"":item.PhoneNumber.ToString(),
                  item.Email.ToString()==null?"":item.Email.ToString(),
                  item.Status,
               }));

                return new ExcelResult(HeadersHospitalMaster, ColunmTypesHospitalMaster, data, "HospitalMaster.xlsx", "Hospital Master");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        private static readonly string[] HeadersHospitalMaster = { "HospitalName", "Address", "Street", "City", "ZipCode", "PhoneNumber", "Email", "Status" };

        private static readonly DataForExcel.DataType[] ColunmTypesHospitalMaster = {

        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String ,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String

        };

        public ActionResult ExportHospitalMasToPDF()
        {
            try
            {
                List<HospitaMasterPDF> userList = new List<HospitaMasterPDF>();
                decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                var context = (from a in genData.GetHospitalMaster_PSO(psoFkid) orderby a.NameofHospital select a).ToList();
                foreach (var item in context)
                {
                    HospitaMasterPDF user = new HospitaMasterPDF();
                    user.HospitalName = item.NameofHospital.ToString() == null ? "" : item.NameofHospital.ToString();
                    user.Address = item.HotelAddress.ToString() == null ? "" : item.HotelAddress.ToString();
                    user.Street = item.Street.ToString() == null ? "" : item.Street.ToString();
                    user.City = item.City.ToString() == null ? "" : item.City.ToString();
                    user.ZipCode = item.ZipCode.ToString() == null ? "" : item.ZipCode.ToString();
                    user.PhoneNumber = item.PhoneNumber.ToString() == null ? "" : item.PhoneNumber.ToString();
                    user.Email = item.Email.ToString() == null ? "" : item.Email.ToString();
                    user.Status = item.Status == null ? "" : item.Status.ToString();
                    userList.Add(user);
                }
                var customerList = userList;
                pdf.ExportPDF(customerList, new string[] { "HospitalName", "Address", "Street", "City", "ZipCode", "PhoneNumber", "Email", "Status" }, path);
                return File(path, "application/pdf", "HospitalMaster.pdf");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public class HospitaMasterPDF
        {
            public string HospitalName { get; set; }
            public string Address { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string ZipCode { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
        }
        public ActionResult ExportHospitalMasToCsv()
        {

            decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            var model = (from a in genData.GetHospitalMaster_PSO(psoFkid) orderby a.NameofHospital select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("HospitalName,Address,Street,City,ZipCode,PhoneNumber,Email,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}",

                  record.NameofHospital.ToString() == null ? "" : record.NameofHospital.ToString(),
                  record.HotelAddress.ToString() == null ? "" : record.HotelAddress.ToString(),
                  record.Street.ToString() == null ? "" : record.Street.ToString(),
                  record.City == null ? "" : record.City,
                  record.ZipCode.ToString() == null ? "" : record.ZipCode.ToString(),
                  record.PhoneNumber.ToString() == null ? "" : record.PhoneNumber.ToString(),
                  record.Email.ToString() == null ? "" : record.Email.ToString(),
                  record.Status == null ? "" : record.Status.ToString()

                );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "HospitalMaster.txt");
        }



        public ActionResult AllCityTerritoryMaster(string Type, string data)
        {

            decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            dynamic query = "";
            ViewBag.Type = Type;
            query = (from e in genData.GetAllCityByTerritory(psoFkid) select new { e.PKID, e.CityName }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.CityName.ToString());
            return PartialView("PVMasters", query);
        }


        public ActionResult AreaNameLoad(string Type, string AreaCode)
        {
            dynamic query = "";
            ViewBag.Type = Type;
            int AreaCodeDetails = Convert.ToInt32(AreaCode);
            query = (from a in genData.GetAllBrickDetailsByXML(Convert.ToInt32(AreaCode)) select new { a.PKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
            return PartialView("PVHospitalMaster", query);

        }


        public ActionResult AreaCodeLoad(string type, string City)
        {
            dynamic query = "";
            ViewBag.Type = type;

            if (City == "undefined")
            {
                query = (from a in genData.GetAllBrickDetailsByXML(0) where 1 == 2 select new { a.PKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
                return PartialView("PVCityCodeMaster", query);
            }
            else
            {
                query = (from a in genData.GetAllBrickDetailsByXML(Convert.ToInt32(City)) select new { a.PKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
                return PartialView("PVCityCodeMaster", query);
            }

        }


        //Stockiest Master
        public ActionResult StockiestMaster()
        {
            return View();
        }

        public JsonResult GridDataStockiestMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                //Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                decimal psoFkid = 339;

                var query = (from a in genData.GetStockistMaster_PSO(psoFkid) select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "NameofStockist"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.NameofStockist != null && sq.NameofStockist.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.NameofStockist != null && sq.NameofStockist.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.NameofStockist != null && sq.NameofStockist.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.NameofStockist != null && sq.NameofStockist.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "NameofStockist" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.NameofStockist ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "NameofStockist" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.NameofStockist descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "StockistAddress" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.StockistAddress ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "StockistAddress" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.StockistAddress descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "City" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.City ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "City" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.City descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Street" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Street ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Street" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Street descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "PhoneNumber" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.PhoneNumber ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "PhoneNumber" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.PhoneNumber descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "Email" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Email" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Status descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Status ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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
        ////For City Drop Down
        //public ActionResult AllCityTerritoryMaster(string type, string data)
        //{

        //    //Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
        //    decimal psoFkid = 339;
        //    ViewBag.Type = type;
        //    var query = (from e in genData.GetAllCityByTerritory(psoFkid) select new { e.PKID, e.CityName }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.CityName.ToString());
        //    return PartialView("PVMasters", query);
        //}




        ////For Area DropDown
        //public ActionResult AreaNameLoad(string Type, string AreaCode)
        //{
        //    dynamic query = "";
        //    ViewBag.Type = Type;

        //    query = (from a in genData.GetAllBrickDetailsByXML(Convert.ToInt32(AreaCode)) select new { a.PKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
        //    return PartialView("PVHospitalMaster", query);

        //}

        ////For Area DropDown
        //public ActionResult AreaNameLoadforEdit(string Type, string AreaCode)
        //{
        //    dynamic query = "";
        //    ViewBag.Type = Type;

        //    var pkId = (from a in genData.usp_GetAreaForStock(AreaCode) select a).FirstOrDefault();

        //    query = (from a in genData.GetAllBrickDetailsByXML(Convert.ToInt32(pkId)) select new { a.PKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
        //    return PartialView("PVStokiestMaster", query);

        //}


        public JsonResult AddStokiestMaster(string NameofStockist, string StockistAddress, string Street, string City, string Area, string ZipCode, string PhoneNumber, string Email, string Status)
        {

            try
            {
                string msg = string.Empty;
                decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                int rslt = genData.sp_insert_Stockistdetails(NameofStockist, StockistAddress, Street, Convert.ToInt32(City), Convert.ToInt32(Area), Convert.ToDecimal(ZipCode), PhoneNumber, Email, psoFkid);

                if (rslt >= 1)
                {

                    msg = "Stokiest Master Added Successfully.";
                }
                return Json(msg);

            }
            catch (Exception ex)
            {
                return Json("Stokiest Master already Exists");
            }
        }

        public JsonResult EditStockiestMaster(string id, string oper, string NameofStockist, string StockistAddress, string Street, string City, string Area, string ZipCode, string PhoneNumber, string Email, string Status)
        {

            try
            {
                string msg = string.Empty;

                if (oper == "edit")
                {


                    if (Status == "Active")
                    {
                        Status = "1";
                    }
                    else
                    {
                        Status = "0";
                    }
                    genData.sp_update_Stockistdetails(Convert.ToInt32(id), NameofStockist, StockistAddress, Street, Convert.ToInt32(City), Convert.ToInt32(Area), Convert.ToDecimal(ZipCode), PhoneNumber, Email, Status);
                    genData.SaveChanges();
                    msg = "StokiestMaster modified Successfully.";

                }



                if (oper == "del")
                {

                    var quote = Convert.ToInt32(id);
                    genData.sp_delete_Stockistdetails(quote);
                    msg = "StockiestMaster Master deleted Successfully.";
                }

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json("StockiestMaster Master Modified Successfully.");
            }
        }


        //For Excel & PDF & CSV For Stokiest Master
        public ActionResult ExportStokiestMasterToExcel()
        {
            //decimal psoFkid = 339;
            decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            var context = (from a in genData.GetStockistMaster_PSO(psoFkid) orderby a.NameofStockist select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.NameofStockist.ToString()== null ? "" : item.NameofStockist.ToString(),                
                item.StockistAddress.ToString()==null?"":item.StockistAddress.ToString(),
                item.Street.ToString()==null?"":item.StockistAddress.ToString(),
                item.City==null?"":item.City,
                item.ZipCode.ToString()==null?"":item.ZipCode.ToString(),
               item.PhoneNumber.ToString()==null?"":item.PhoneNumber.ToString(),
               item.Email.ToString()==null?"":item.Email.ToString(),
               item.Status,

            }));
            return new ExcelResult(HeadersActivityMaster, ColunmTypesActivityMaster, data, "StokiestMaster.xlsx", "Stokiest Master");
        }
        private static readonly string[] HeadersActivityMaster = {
               "StokiestName","Address","Street","City","ZipCode","PhoneNumber","Email","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesActivityMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String 
                
            };

        public ActionResult ExportStokiestMasterToPDF()
        {
            List<StokiestMasterPDF> userList = new List<StokiestMasterPDF>();
            //decimal psoFkid = 339;
            decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            var context = (from a in genData.GetStockistMaster_PSO(psoFkid) orderby a.NameofStockist select a).ToList();

            foreach (var item in context)
            {
                StokiestMasterPDF user = new StokiestMasterPDF();


                user.NameofStockist = item.NameofStockist.ToString() == null ? "" : item.NameofStockist.ToString();
                user.StockistAddress = item.StockistAddress.ToString() == null ? "" : item.StockistAddress.ToString();
                user.Street = item.Street.ToString() == null ? "" : item.Street.ToString();
                user.City = item.City == null ? "" : item.City;
                user.ZipCode = item.ZipCode.ToString() == null ? "" : item.ZipCode.ToString();
                user.PhoneNumber = item.PhoneNumber.ToString() == null ? "" : item.PhoneNumber.ToString();
                user.Email = item.Email.ToString() == null ? "" : item.Email.ToString();
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
                            cell = new string[]{
                            e.NameofStockist,
                            e.StockistAddress,
                            e.Street,
                            e.City,
                            e.ZipCode,
                            e.PhoneNumber,
                            e.Email,
                            e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "NameofStockist", "StockistAddress", "Street", "City", "ZipCode", "PhoneNumber", "Email", "Status" }, path);
            return File(path, "application/pdf", "StokiestMaster.pdf");
        }
        public ActionResult ExportStokiestMasterPDFToCsv()
        {

            //decimal psoFkid = 339;
            decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            var model = (from a in genData.GetStockistMaster_PSO(psoFkid) orderby a.NameofStockist select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("NameofStockist,StockistAddress, Street, City, ZipCode,PhoneNumber,Email,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}",


                    record.NameofStockist == null ? "" : record.NameofStockist.ToString(),
                    record.StockistAddress == null ? "" : record.StockistAddress.ToString(),
                    record.Street == null ? "" : record.Street.ToString(),
                    record.City == null ? "" : record.City,
                    record.ZipCode == null ? "" : record.ZipCode.ToString(),
                    record.PhoneNumber == null ? "" : record.PhoneNumber.ToString(),
                    record.Email == null ? "" : record.Email.ToString(),
                    record.Status

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "StokiestMaster.txt");
        }
        public class StokiestMasterPDF
        {
            //StockistAddress
            //Street
            //City 
            //ZipCode
            //PhoneNumber
            //Email
            public string NameofStockist { get; set; }
            public string StockistAddress { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string ZipCode { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
        }

        // Unlock Cycle Plan

        //Usp_GetPSOsForCyclePlanUnlock
        public ActionResult CyclePlanDoctorUnlock()
        {
            return View();
        }

        public JsonResult CyclePlanDoctorGridData(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                //Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                decimal psoFkid = 339;

                var query = (from a in genData.GetHospitalMaster_PSO(psoFkid) select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "NameofHospital") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.NameofHospital.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.NameofHospital.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.NameofHospital.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.NameofHospital.Contains(searchString) || sq.Status.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.NameofHospital).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "NameofHospital" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.NameofHospital ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "NameofHospital" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.NameofHospital descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "HotelAddress" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.HotelAddress ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "HotelAddress" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.HotelAddress descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "City" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.City ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "City" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.City descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Street" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Street ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Street" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Street descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "PhoneNumber" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.PhoneNumber ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "PhoneNumber" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.PhoneNumber descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "Email" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Email" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Status descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Status ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        #region PCLtoMCLRequest

        public ActionResult PCLtoMCLRequest()
        {

            return View();
            //dynamic rslt = "";
            //rslt = (from a in genData.GetAreaDetailsPCL(Convert.ToInt32(Session["Territory_FKID"])) select new { a.PKID, a.AreaName }).OrderBy(x => x.AreaName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
            //return View(rslt);
        }


        public string ConvertXML(string CusTypeXML)
        {
            string xml = "<row>";
            string[] arr = CusTypeXML.Split(',');
            StringBuilder sw = new StringBuilder();

            for (var i = 0; i < arr.Length; i++)
            {

                sw.Append("<dp CusCode='" + arr[i] + "'/>");

            }
            sw.Append("</row>");

            return xml += sw.ToString();
        }

        public JsonResult PCLtoMCLGridData(GridQueryModel gridQueryModel, string CusTypeXML, string AreaFKID)
        {
            try
            {
                var searchString = gridQueryModel.searchString;


                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                string xmlNode = ConvertXML(CusTypeXML);// CusTypeXML 


                DataSet ds = bLayer.AllPendingCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
                List<UsefulLink> objList = new List<UsefulLink>();

                int id = 0;
                foreach (DataTable dt in ds.Tables)
                {

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {

                            UsefulLink useful = new UsefulLink();
                            useful.Type = dr["Type"].ToString();
                            useful.MASPKID = dr["MASPKID"].ToString();
                            useful.CustomerName = dr["CustomerName"].ToString();
                            useful.Specialization = dr["Specialization"].ToString();
                            useful.AreaName = dr["AreaName"].ToString();
                            useful.ID = id;
                            objList.Add(useful);
                            id++;
                        }
                    }
                }


                var count = objList.Count();
                var pageData = objList.OrderBy(x => x.Type).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        public JsonResult PCLPendingGridData(GridQueryModel gridQueryModel, string CusTypeXML, string AreaFKID)
        {
            try
            {
                var searchString = gridQueryModel.searchString;


                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                string xmlNode = ConvertXML(CusTypeXML);// CusTypeXML 


                DataSet ds = bLayer.AllSubmittedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
                List<UsefulLink> objList = new List<UsefulLink>();

                foreach (DataTable dt in ds.Tables)
                {

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {

                            UsefulLink useful = new UsefulLink();
                            useful.Type = dr["Type"].ToString();
                            useful.MASPKID = dr["MASPKID"].ToString();
                            useful.CustomerName = dr["CustomerName"].ToString();
                            useful.Specialization = dr["Specialization"].ToString();
                            useful.AreaName = dr["AreaName"].ToString();
                            useful.CreatedDate = dr["CreatedDate"].ToString();
                            useful.ReasonforRequest = dr["ReasonforRequest"].ToString();
                            objList.Add(useful);
                        }
                    }
                }


                var count = objList.Count();
                var pageData = objList.OrderBy(x => x.Type).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



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



        public JsonResult PCLApproveGridGridData(GridQueryModel gridQueryModel, string CusTypeXML, string AreaFKID)
        {
            try
            {
                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                string xmlNode = ConvertXML(CusTypeXML);


                DataSet ds = bLayer.AllPSOApprovedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
                List<UsefulLink> objList = new List<UsefulLink>();


                foreach (DataTable dt in ds.Tables)
                {

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {

                            UsefulLink useful = new UsefulLink();
                            useful.Type = dr["Type"].ToString() == null ? "" : dr["Type"].ToString();
                            useful.MASPKID = dr["CusFKID"].ToString() == null ? "" : dr["CusFKID"].ToString();
                            useful.CustomerName = dr["CustomerName"].ToString() == null ? "" : dr["CustomerName"].ToString();
                            useful.Specialization = dr["Specialization"].ToString() == null ? "" : dr["Specialization"].ToString();
                            useful.AreaName = dr["AreaName"].ToString() == null ? "" : dr["AreaName"].ToString();
                            useful.CreatedDate = dr["requestdate"].ToString() == null ? "" : dr["requestdate"].ToString();
                            useful.ReasonforRequest = dr["RequestReason"].ToString() == null ? "" : dr["RequestReason"].ToString();
                            useful.ApprovedBy = dr["ReportingName"].ToString() == null ? "" : dr["ReportingName"].ToString();
                            useful.ApprovedDate = dr["AppDate"].ToString() == null ? "" : dr["AppDate"].ToString();
                            objList.Add(useful);
                        }
                    }
                }


                var count = objList.Count();
                var pageData = objList.OrderBy(x => x.Type).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



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



        public JsonResult PCLRejectedGridData(GridQueryModel gridQueryModel, string CusTypeXML, string AreaFKID)
        {
            try
            {
                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                string xmlNode = ConvertXML(CusTypeXML);


                DataSet ds = bLayer.AllPSORejectedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
                List<UsefulLink> objList = new List<UsefulLink>();


                foreach (DataTable dt in ds.Tables)
                {

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {

                            UsefulLink useful = new UsefulLink();
                            useful.Type = dr["Type"].ToString();
                            useful.MASPKID = dr["CusFKID"].ToString();
                            useful.CustomerName = dr["CustomerName"].ToString();
                            useful.Specialization = dr["Specialization"].ToString();
                            useful.AreaName = dr["AreaName"].ToString();
                            useful.CreatedDate = dr["requestdate"].ToString();
                            useful.ReasonforRequest = dr["RequestReason"].ToString();
                            useful.ApprovedBy = dr["ReportingName"].ToString();
                            useful.ApprovedDate = dr["AppDate"].ToString();
                            useful.RejectedReason = dr["RejectedReason"].ToString();
                            objList.Add(useful);
                        }
                    }
                }


                var count = objList.Count();
                var pageData = objList.OrderBy(x => x.Type).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



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

        #region Fetch Doctor Details
        public JsonResult JsonGetDoctorDetails(GridQueryModel gridQueryModel, string MASPKID, string Type, string flag)
        {
            string FName = "", MName = "", LName = "", Address = "", Street = "", Specialization = "", AreaName = "", AreaFKID = "", Zipcode = "", TelClinic = "", TelRoles = "", Mobile = "", Email = "", CycleFKID = "", element = "", PhoneNo = "";
            DataTable dtCycle = bLayer.GetCurrentCycleForPSo();

            if (dtCycle.Rows.Count > 0)
                CycleFKID = dtCycle.Rows[0]["PKID"].ToString();
            DataSet ds = bLayer.GetDoctorChemistProfile(MASPKID, Session["USER_FKID"].ToString(), Type, CycleFKID);
           
            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {


                        FName = dr["FirstName"].ToString();
                        MName = dr["MiddleName"].ToString();
                        LName = dr["LastName"].ToString();
                        Address = dr["Address"].ToString();
                        Street = dr["Street"].ToString();

                        AreaName = dr["AreaName"].ToString();
                        AreaFKID = dr["AreaFKID"].ToString();


                        Mobile = dr["Mobile"].ToString();
                        Email = dr["Email"].ToString();

                        if (Type == "Doctor")
                        {
                            Specialization = dr["Specialization"].ToString();
                            TelClinic = dr["TelClinic"].ToString();
                            TelRoles = dr["TelRes"].ToString();
                            Zipcode = dr["ZipCode"].ToString();
                        }
                        else if (Type == "Chemist")
                        {

                            PhoneNo = dr["PhoneNo"].ToString();
                            Zipcode = dr["Zip"].ToString();
                        }

                        break;



                    }

                }
                break;

            }


            return Json(new
            {

                FName,
                MName,
                LName,
                Address,
                Street,
                Specialization,
                AreaName,
                AreaFKID,
                Zipcode,
                TelClinic,
                TelRoles,
                Mobile,
                Email,
                PhoneNo


            }, JsonRequestBehavior.AllowGet);



        }

        #endregion

        #region Fetch Doctor Call History
        public JsonResult JsonGetDoctorDetailsforGrid(GridQueryModel gridQueryModel, string MASPKID, string Type)
        {

            DataTable dtCycle = bLayer.GetCurrentCycleForPSo();
            string CycleFKID = "";
            if (dtCycle.Rows.Count > 0)
                CycleFKID = dtCycle.Rows[0]["PKID"].ToString();
            DataSet ds = bLayer.GetDoctorChemistProfile(MASPKID, Session["USER_FKID"].ToString(), Type, CycleFKID);
            List<UsefulLink> objList = new List<UsefulLink>();

            if (ds.Tables.Count > 1)
            {
                if (ds.Tables[1].Rows.Count > 0)
                {

                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        UsefulLink useful = new UsefulLink();
                        useful.LoginName = dr["Loginname"].ToString();
                        useful.Date = dr["Date"].ToString();
                        objList.Add(useful);

                    }
                }
            }
            else
            {
                UsefulLink useful = new UsefulLink();
                useful.LoginName = "";
                useful.Date = "";
                objList.Add(useful);

            }
            var count = objList.Count();
            var pageData = objList.OrderBy(x => x.Type).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
            return Json(new
            {
                page = gridQueryModel.page,
                records = count,
                rows = pageData,
                total = Math.Ceiling((decimal)count / gridQueryModel.rows)
            }, JsonRequestBehavior.AllowGet);



        }

        #endregion

        #region Submit
        public ActionResult SubmitPCLRequest(string gridData)
        {
            string msg = string.Empty;
            try
            {
                JavaScriptSerializer objJavaScriptSerializer = new JavaScriptSerializer();

                PCLTOMCLDoctor[] objDoctor = objJavaScriptSerializer.Deserialize<PCLTOMCLDoctor[]>(gridData);


                //var result = (from x in objDoctor where ids.Contains(x.MASPKID) select x).ToList();
                var result = (from x in objDoctor where x.chks == "Yes" select x).ToList();

                string xml = "<row>";
                StringBuilder sw = new StringBuilder();

                var doctorRlt = (from a in result where a.Type.ToUpper() == "DOCTOR" select a).ToList();
                var otherRlt = (from a in result where a.Type.ToUpper() != "DOCTOR" select a).ToList();
                sw.Append("<row>");
                foreach (var lst in doctorRlt)
                {

                    sw.Append("<dp CusFKID=\"" + lst.MASPKID + "\" ");
                    sw.Append(" CusType=\"" + lst.Type + "\" ");
                    sw.Append(" ReqReason=\"" + lst.Reason + "\"/>");

                }

                foreach (var lst in otherRlt)
                {

                    sw.Append("<dp CusFKID=\"" + lst.MASPKID + "\" ");
                    sw.Append(" ReqReason=\"" + lst.Reason + "\" ");
                    sw.Append(" CusType=\"" + lst.Type + "\"/>");

                }

                sw.Append("</row>");


                if (sw.ToString() != "<row></row>")
                {

                    genData.SubmitDoctor(sw.ToString(), Convert.ToDecimal(Session["USER_FKID"].ToString()));
                    genData.submitOtherCustomer(sw.ToString(), Convert.ToDecimal(Session["USER_FKID"].ToString()));

                    msg = "Request Submitted Successfully";

                }
                else
                {
                    msg = "notselect";
                }

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { msg = ex.Message }); }
        }
        #endregion

        public class PCLTOMCLDoctor
        {
            public string CustomerName { get; set; }
            public string Type { get; set; }
            public string AreaName { get; set; }
            public string Specialization { get; set; }
            public string Reason { get; set; }
            public int MASPKID { get; set; }
            public string chks { get; set; }
        }

        #region For Excel & PDF & CSV For PendingGrid
        public ActionResult ExportPCLPendingGridToExcel(string CusTypeXML, string AreaFKID, string status)
        {

            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllSubmittedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();

            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["MASPKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["CreatedDate"].ToString();
                        useful.ReasonforRequest = dr["ReasonforRequest"].ToString();
                        context.Add(useful);
                    }
                }
            }

            if (context.Count == 0)
                return new EmptyResult();

            var list = context.OrderBy(o => o.Type).ToList();
            var data = new List<string[]>(list.Count);
            data.AddRange(list.Select(item => new[] {
            item.CreatedDate.ToString()==null?"":item.CreatedDate.ToString() ,
            item.CustomerName.ToString()==null?"":item.CustomerName.ToString(),
            item.Type.ToString()== null ? "" : item.Type.ToString(),                
            item.AreaName.ToString()==null?"":item.AreaName.ToString(), 
            item.Specialization==null?"":item.Specialization,
            item.ReasonforRequest==null?"":item.ReasonforRequest 

            }));

            return new ExcelResult(HeadersPCLPendingGrid, ColunmTypesPCLPendingGrid, data, "PCLPendingRequest.xlsx", "PCL Pending Request");
        }
        private static readonly string[] HeadersPCLPendingGrid = {
               "Request Date","Customer Name","Customer Type","Brick","Speciality","Reason for Request"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesPCLPendingGrid = {  
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String  
                 
            };

        public ActionResult ExportPCLPendingRequestToPDF(string CusTypeXML, string AreaFKID, string status)
        {
            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllSubmittedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();

            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["MASPKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["CreatedDate"].ToString();
                        useful.ReasonforRequest = dr["ReasonforRequest"].ToString();
                        context.Add(useful);
                    }
                }
            }
            var customerList = (from a in context
                                select new
                                {
                                    RequestDate = a.CreatedDate,
                                    a.CustomerName,
                                    CustomerType = a.Type,
                                    Brick = a.AreaName,
                                    Speciality = a.Specialization,
                                    a.ReasonforRequest
                                }).OrderBy(o => o.CustomerType).ToList();
            var newRslt = customerList;

            pdf.ExportPDF(newRslt, new string[] { "RequestDate", "CustomerName", "CustomerType", "Brick", "Speciality", "ReasonforRequest" }, path);
            return File(path, "application/pdf", "PCLPendingRequest.pdf");
        }
        public ActionResult ExportPCLPendingRequestToCsv(string CusTypeXML, string AreaFKID, string status)
        {

            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllSubmittedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();

            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["MASPKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["CreatedDate"].ToString();
                        useful.ReasonforRequest = dr["ReasonforRequest"].ToString();
                        context.Add(useful);
                    }
                }
            }
            var customerList = (from a in context
                                select new
                                {
                                    RequestDate = a.CreatedDate,
                                    a.CustomerName,
                                    CustomerType = a.Type,
                                    Brick = a.AreaName,
                                    Speciality = a.Specialization,
                                    a.ReasonforRequest
                                }).OrderBy(o => o.CustomerType).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("RequestDate,CustomerName,CustomerType,Brick,Speciality,ReasonforRequest");
            foreach (var record in customerList)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5}",


                    record.RequestDate == null ? "" : record.RequestDate.ToString(),
                    record.CustomerName == null ? "" : record.CustomerName.ToString(),
                    record.CustomerType == null ? "" : record.CustomerType.ToString(),
                    record.Brick == null ? "" : record.Brick,
                    record.Speciality == null ? "" : record.Speciality.ToString(),
                    record.ReasonforRequest == null ? "" : record.ReasonforRequest.ToString()
                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "PCLPendingRequest.txt");
        }

        #endregion

        #region Excel & PDF & CSV For Approved Grid
        public ActionResult ExportPCLApprovedGridToExcel(string CusTypeXML, string AreaFKID, string status)
        {

            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllPSOApprovedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();


            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["CusFKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["requestdate"].ToString();
                        useful.ReasonforRequest = dr["RequestReason"].ToString();
                        useful.ApprovedBy = dr["ReportingName"].ToString();
                        useful.ApprovedDate = dr["AppDate"].ToString();
                        context.Add(useful);
                    }
                }
            }



            if (context.Count == 0)
                return new EmptyResult();

            var list = context.OrderBy(o => o.Type).ToList();
            var data = new List<string[]>(list.Count);
            data.AddRange(list.Select(item => new[] {
            item.CreatedDate.ToString()==null?"":item.CreatedDate.ToString() ,
            item.CustomerName.ToString()==null?"":item.CustomerName.ToString(),
            item.Type.ToString()== null ? "" : item.Type.ToString(),                
            item.AreaName.ToString()==null?"":item.AreaName.ToString(), 
            item.Specialization==null?"":item.Specialization,
            item.ReasonforRequest==null?"":item.ReasonforRequest ,
            item.ApprovedBy==null?"":item.ApprovedBy,
            item.ApprovedDate==null?"":item.ApprovedDate 

            }));

            return new ExcelResult(HeadersPCLApprovedGrid, ColunmTypesPCLApprovedGrid, data, "PCLApprovedRequest.xlsx", "PCL Approved Request");
        }
        private static readonly string[] HeadersPCLApprovedGrid = {
               "Request Date","Customer Name","Customer Type","Brick","Speciality","Reason for Request","Approved BY","Approved Date"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesPCLApprovedGrid = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                 DataForExcel.DataType.String,
                DataForExcel.DataType.String 
              
                
            };

        public ActionResult ExportPCLApprovedRequestToPDF(string CusTypeXML, string AreaFKID, string status)
        {
            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllPSOApprovedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();


            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["CusFKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["requestdate"].ToString();
                        useful.ReasonforRequest = dr["RequestReason"].ToString();
                        useful.ApprovedBy = dr["ReportingName"].ToString();
                        useful.ApprovedDate = dr["AppDate"].ToString();
                        context.Add(useful);
                    }
                }
            }


            var customerList = (from a in context
                                select new
                                {
                                    RequestDate = a.CreatedDate,
                                    a.CustomerName,
                                    CustomerType = a.Type,
                                    Brick = a.AreaName,
                                    Speciality = a.Specialization,
                                    a.ReasonforRequest,
                                    a.ApprovedBy,
                                    a.ApprovedDate
                                }).OrderBy(o => o.CustomerType).ToList();
            var newRslt = customerList;

            pdf.ExportPDF(newRslt, new string[] { "RequestDate", "CustomerName", "CustomerType", "Brick", "Speciality", "ReasonforRequest", "ApprovedBy", "ApprovedDate" }, path);
            return File(path, "application/pdf", "PCLApprovedRequest.pdf");
        }
        public ActionResult ExportPCLApprovedRequestToCsv(string CusTypeXML, string AreaFKID, string status)
        {

            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllPSOApprovedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();


            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["CusFKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["requestdate"].ToString();
                        useful.ReasonforRequest = dr["RequestReason"].ToString();
                        useful.ApprovedBy = dr["ReportingName"].ToString();
                        useful.ApprovedDate = dr["AppDate"].ToString();
                        context.Add(useful);
                    }
                }
            }

            var customerList = (from a in context
                                select new
                                {
                                    RequestDate = a.CreatedDate,
                                    a.CustomerName,
                                    CustomerType = a.Type,
                                    Brick = a.AreaName,
                                    Speciality = a.Specialization,
                                    a.ReasonforRequest,
                                    a.ApprovedBy,
                                    a.ApprovedDate
                                }).OrderBy(o => o.CustomerType).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("RequestDate,CustomerName,CustomerType,Brick,Speciality,ReasonforRequest,ApprovedBy,ApprovedDate");
            foreach (var record in customerList)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}",


                    record.RequestDate == null ? "" : record.RequestDate.ToString(),
                    record.CustomerName == null ? "" : record.CustomerName.ToString(),
                    record.CustomerType == null ? "" : record.CustomerType.ToString(),
                    record.Brick == null ? "" : record.Brick,
                    record.Speciality == null ? "" : record.Speciality.ToString(),
                    record.ReasonforRequest == null ? "" : record.ReasonforRequest.ToString(),
                     record.ApprovedBy == null ? "" : record.ApprovedBy.ToString(),
                    record.ApprovedDate == null ? "" : record.ApprovedDate.ToString()
                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "PCLApprovedRequest.txt");
        }

        #endregion

        #region Excel & PDF & CSV For Rejected Grid
        public ActionResult ExportPCLRejectedGridToExcel(string CusTypeXML, string AreaFKID, string status)
        {

            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllPSORejectedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();

            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["CusFKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["requestdate"].ToString();
                        useful.ReasonforRequest = dr["RequestReason"].ToString();
                        useful.ApprovedBy = dr["ReportingName"].ToString();
                        useful.ApprovedDate = dr["AppDate"].ToString();
                        useful.RejectedReason = dr["RejectedReason"].ToString();
                        context.Add(useful);
                    }
                }
            }



            if (context.Count == 0)
                return new EmptyResult();

            var list = context.OrderBy(o => o.Type).ToList();
            var data = new List<string[]>(list.Count);
            data.AddRange(list.Select(item => new[] {
            item.CreatedDate.ToString()==null?"":item.CreatedDate.ToString() ,
            item.CustomerName.ToString()==null?"":item.CustomerName.ToString(),
            item.Type.ToString()== null ? "" : item.Type.ToString(),                
            item.AreaName.ToString()==null?"":item.AreaName.ToString(), 
            item.Specialization==null?"":item.Specialization,
            item.ReasonforRequest==null?"":item.ReasonforRequest ,
            item.ApprovedBy==null?"":item.ApprovedBy,
            item.ApprovedDate==null?"":item.ApprovedDate ,
            item.RejectedReason==null?"":item.RejectedReason 

            }));

            return new ExcelResult(HeadersPCLRejectedGrid, ColunmTypesPCLRejectedGrid, data, "PCLRejectedRequest.xlsx", "PCL Rejected Request");
        }
        private static readonly string[] HeadersPCLRejectedGrid = {
               "Request Date","Customer Name","Customer Type","Brick","Speciality","Reason for Request","Approved BY","Approved Date","Rejected Reason"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesPCLRejectedGrid = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String ,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String  
                
            };

        public ActionResult ExportPCLRejectedRequestToPDF(string CusTypeXML, string AreaFKID, string status)
        {
            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllPSORejectedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();

            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["CusFKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["requestdate"].ToString();
                        useful.ReasonforRequest = dr["RequestReason"].ToString();
                        useful.ApprovedBy = dr["ReportingName"].ToString();
                        useful.ApprovedDate = dr["AppDate"].ToString();
                        useful.RejectedReason = dr["RejectedReason"].ToString();
                        context.Add(useful);
                    }
                }
            }



            var customerList = (from a in context
                                select new
                                {
                                    RequestDate = a.CreatedDate,
                                    a.CustomerName,
                                    CustomerType = a.Type,
                                    Brick = a.AreaName,
                                    Speciality = a.Specialization,
                                    a.ReasonforRequest,
                                    a.ApprovedBy,
                                    a.ApprovedDate,
                                    a.RejectedReason

                                }).OrderBy(o => o.CustomerType).ToList();
            var newRslt = customerList;

            pdf.ExportPDF(newRslt, new string[] { "RequestDate", "CustomerName", "CustomerType", "Brick", "Speciality", "ReasonforRequest", "ApprovedBy", "ApprovedDate", "RejectedReason" }, path);
            return File(path, "application/pdf", "PCLRejectedRequest.pdf");
        }
        public ActionResult ExportPCLRejectedRequestToCsv(string CusTypeXML, string AreaFKID, string status)
        {

            string xmlNode = ConvertXML(CusTypeXML);
            DataSet ds = bLayer.AllPSORejectedCustomer(Session["USER_FKID"].ToString(), AreaFKID, xmlNode);
            List<UsefulLink> context = new List<UsefulLink>();

            foreach (DataTable dt in ds.Tables)
            {

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {

                        UsefulLink useful = new UsefulLink();
                        useful.Type = dr["Type"].ToString();
                        useful.MASPKID = dr["CusFKID"].ToString();
                        useful.CustomerName = dr["CustomerName"].ToString();
                        useful.Specialization = dr["Specialization"].ToString();
                        useful.AreaName = dr["AreaName"].ToString();
                        useful.CreatedDate = dr["requestdate"].ToString();
                        useful.ReasonforRequest = dr["RequestReason"].ToString();
                        useful.ApprovedBy = dr["ReportingName"].ToString();
                        useful.ApprovedDate = dr["AppDate"].ToString();
                        useful.RejectedReason = dr["RejectedReason"].ToString();
                        context.Add(useful);
                    }
                }
            }

            var customerList = (from a in context
                                select new
                                {
                                    RequestDate = a.CreatedDate,
                                    a.CustomerName,
                                    CustomerType = a.Type,
                                    Brick = a.AreaName,
                                    Speciality = a.Specialization,
                                    a.ReasonforRequest,
                                    a.ApprovedBy,
                                    a.ApprovedDate,
                                    a.RejectedReason
                                }).OrderBy(o => o.CustomerType).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("RequestDate,CustomerName,CustomerType,Brick,Speciality,ReasonforRequest,ApprovedBy,ApprovedDate,RejectedReason");
            foreach (var record in customerList)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8}",


                    record.RequestDate == null ? "" : record.RequestDate.ToString(),
                    record.CustomerName == null ? "" : record.CustomerName.ToString(),
                    record.CustomerType == null ? "" : record.CustomerType.ToString(),
                    record.Brick == null ? "" : record.Brick,
                    record.Speciality == null ? "" : record.Speciality.ToString(),
                    record.ReasonforRequest == null ? "" : record.ReasonforRequest.ToString(),
                    record.ApprovedBy == null ? "" : record.ApprovedBy.ToString(),
                    record.ApprovedDate == null ? "" : record.ApprovedDate.ToString(),
                    record.RejectedReason == null ? "" : record.RejectedReason.ToString()
                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "PCLRejectedRequest.txt");
        }

        #endregion
        #endregion

        #region DoctorTransferCopy
        public ActionResult DoctorTransferCopy()
        {

            return View();

        }


        public JsonResult JsonLoadTerritory(decimal fkId)
        {
            List<SelectListItem> territoryItems = new List<SelectListItem>();
            territoryItems.Clear();


            var list = (from s in genData.GetTerritoryName(fkId) select new { s.TerPKID, s.TerCode }).ToList();
            if (list != null)
            {
                territoryItems = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.TerCode.ToString(),
                    Value = x.TerPKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                territoryItems = territoryItems,
                count = territoryItems.Count,
            }, JsonRequestBehavior.AllowGet);


        }


        public JsonResult JsonLoadDTerritory(string TerPKID, string DivFKID)
        {
            List<SelectListItem> territoryItems = new List<SelectListItem>();
            territoryItems.Clear();

            if (TerPKID != string.Empty)
            {
                var list = (from s in genData.GetTerritoryByDest(Convert.ToDecimal(TerPKID), Convert.ToDecimal(DivFKID)) select new { s.TerPKID, s.TerCode }).ToList();
                if (list != null)
                {
                    territoryItems = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.TerCode.ToString(),
                        Value = x.TerPKID.ToString()
                    }).ToList();
                }
            }
            else
            {
                var list = (from s in genData.GetTerritoryName(Convert.ToDecimal(DivFKID)) select new { s.TerPKID, s.TerCode }).ToList();
                if (list != null)
                {
                    territoryItems = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.TerCode.ToString(),
                        Value = x.TerPKID.ToString()
                    }).ToList();
                }
            }
            return Json(new
            {
                territoryItems = territoryItems,
                count = territoryItems.Count,
            }, JsonRequestBehavior.AllowGet);


        }



        public JsonResult JsonLoadAreaName(decimal fkId)
        {
            List<SelectListItem> areaItems = new List<SelectListItem>();
            areaItems.Clear();


            var list = (from s in genData.GetDrAreaDetails(fkId) select new { s.PKID, s.AreaName }).ToList();
            if (list != null)
            {
                areaItems = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.AreaName.ToString(),
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                areaItems = areaItems,
                count = areaItems.Count,
            }, JsonRequestBehavior.AllowGet);


        }


        public JsonResult JsonLoadDocotors(decimal TerPKID, decimal DivFKID, string AreaFKID)
        {

            List<SelectListItem> doctorItems = new List<SelectListItem>();
            doctorItems.Clear();
            var list = (from s in genData.GetDoctorNameBYPSO(TerPKID, AreaFKID, DivFKID) select new { s.DMPKID, s.UserName }).ToList();
            if (list != null)
            {
                doctorItems = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.UserName.ToString(),
                    Value = x.DMPKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                doctorItems = doctorItems,
                count = doctorItems.Count,
            }, JsonRequestBehavior.AllowGet);


        }

        public JsonResult JsonChkSPecialityforTrDr(string DTerritoryFKID, string drpDTDoctor, string STerritoryFKID)
        {
            drpDTDoctor = drpDTDoctor.Remove(drpDTDoctor.Length - 1, 1);
            drpDTDoctor = drpDTDoctor.Remove(0, 1);
            string TrDrlist = "";
            string NotTrDrlistStr = "";
            try
            {

                DataSet ds = bLayer.ChkSPecialityforTrDr(DTerritoryFKID, drpDTDoctor, STerritoryFKID);

                return Json(new
                {
                    TrDrlist = ds.Tables[0].Rows[0]["TrDrlistStr"].ToString(),
                    NotTrDrlistStr = ds.Tables[1].Rows[0]["NotTrDrlistStr"].ToString(),

                }, JsonRequestBehavior.AllowGet);
            }


            catch (Exception ex)
            {
                return Json(new
                {
                    TrDrlist = ex.Message,
                    NotTrDrlistStr = "",
                }, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult JsonChkSPecialityforCyDr(string DTerritoryFKID, string drpDTDoctor, string STerritoryFKID)
        {
            drpDTDoctor = drpDTDoctor.Remove(drpDTDoctor.Length - 1, 1);
            drpDTDoctor = drpDTDoctor.Remove(0, 1);
            string CyDrlistStr = "";
            string NotCyDrlistStr = "";
            try
            {

                DataSet ds = bLayer.ChkSPecialityforCyDr(DTerritoryFKID, drpDTDoctor, STerritoryFKID);

                return Json(new
                {
                    CyDrlistStr = ds.Tables[0].Rows[0]["CyDrlistStr"].ToString(),
                    NotCyDrlistStr = ds.Tables[1].Rows[0]["NotCyDrlistStr"].ToString(),

                }, JsonRequestBehavior.AllowGet);
            }


            catch (Exception ex)
            {
                return Json(new
                {
                    CyDrlistStr = ex.Message,
                    NotCyDrlistStr = "",
                }, JsonRequestBehavior.AllowGet);

            }

        }
        public JsonResult JsonGetSplDoctorName(string DocPKID)
        {
            string Doctor = "";
            DocPKID = DocPKID.Remove(0, 1);
            var rslt = (from a in genData.GetSplDoctorName1(DocPKID) select new { a.DoctorName }).FirstOrDefault();

            return Json(new
            {
                Doctor = (rslt.DoctorName == null) ? "" : rslt.DoctorName,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonSubmit(string TerritoryDFKID, string TerritorySFKID, string xml, string cond)
        {
            string msg = "";
            try
            {
                if (cond == "transfer")
                {
                    genData.addDoctorTranster(Convert.ToDecimal(TerritoryDFKID), Convert.ToDecimal(TerritorySFKID), Convert.ToDecimal(Session["USER_FKID"].ToString()), xml);

                }
                if (cond == "copy")
                {
                    genData.addDoctorCopy(Convert.ToDecimal(TerritoryDFKID), Convert.ToDecimal(TerritorySFKID), Convert.ToDecimal(Session["USER_FKID"].ToString()), xml);

                }

                return Json(new
                {
                    msg = "Doctor Transfer Copy Submitted Successfully",

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = ex.Message

                }, JsonRequestBehavior.AllowGet);

            }

        }
        #endregion

        #region ChemistTransferCopy
        public ActionResult ChemistTransferCopy()
        {

            return View();

        }

        public JsonResult JsonLoadChemist(decimal TerPKID, decimal DivFKID, string AreaFKID)
        {

            List<SelectListItem> chemistItems = new List<SelectListItem>();
            chemistItems.Clear();
            var list = (from s in genData.GetChemistNameByTerritory(TerPKID, AreaFKID, DivFKID) select new { s.DMPKID, s.UserName }).ToList();
            if (list != null)
            {
                chemistItems = list.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.UserName.ToString(),
                    Value = x.DMPKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                chemistItems = chemistItems,
                count = chemistItems.Count,
            }, JsonRequestBehavior.AllowGet);


        }


        public JsonResult JsonChemistSubmit(string TerritoryDFKID, string TerritorySFKID, string xml, string cond)
        {
            string msg = "";
            try
            {
                if (cond == "transfer")
                {
                    genData.AddChemistTransfer(Convert.ToDecimal(TerritoryDFKID), Convert.ToDecimal(TerritorySFKID), Convert.ToDecimal(Session["USER_FKID"].ToString()), xml);

                }
                if (cond == "copy")
                {
                    genData.AddChemistCopy(Convert.ToDecimal(TerritoryDFKID), Convert.ToDecimal(TerritorySFKID), Convert.ToDecimal(Session["USER_FKID"].ToString()), xml);

                }

                return Json(new
                {
                    msg = "Chemist Transfer Copy Submitted Successfully",

                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    msg = ex.Message

                }, JsonRequestBehavior.AllowGet);

            }

        }
        #endregion


        #region Doctor Master Admin

        #region Doctor Master Action Controller

        public ActionResult DoctorMaster()
        {

            return View();
        }

        #endregion

        public JsonResult GetDoctorMasterAdmin(GridQueryModel gridQueryModel, string PKID)
        {
            try
            {
                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                int userFKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                var query = (from a in genData.USP_GetAllDetailsFromDoctorMaster(userFKID, Convert.ToInt32(PKID)) select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "DoctorName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.DoctorName != null && sq.DoctorName.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.DoctorName != null && sq.DoctorName.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.DoctorName != null && sq.DoctorName.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.DoctorName != null && sq.DoctorName.ToUpper().ToString().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "DoctorCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DoctorCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DoctorCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DoctorCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DoctorName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DoctorName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "Brick" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Brick ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Brick" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Brick descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Speciality" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Speciality ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Speciality" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Speciality descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Status ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Status descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "TerriToryLinkStatus" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerriToryLinkStatus ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerriToryLinkStatus" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerriToryLinkStatus descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        #endregion


        #region CFSA Activate Master

        #region CFSA Activate Master Action Controller

        public ActionResult CFSA()
        {

            return View();
        }

        #endregion


        #region Bind Data To Grid

        public JsonResult GetCFSAMaster(GridQueryModel gridQueryModel, string PKID)
        {
            try
            {

                DataSet ds = bLayer.GetCFSAMasterGrid(PKID);

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                var query = ds.Tables[0].AsEnumerable().Select(dataRow => new GetCFSAMaster
                {

                    FirstName = dataRow[0].ToString(),
                    LastName = dataRow[1].ToString(),
                    UserFKID = dataRow[2].ToString(),
                    TerCode = dataRow[3].ToString(),
                    CFSAActive = dataRow[4].ToString(),
                }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.FirstName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "FirstName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.FirstName != null && sq.FirstName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FirstName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.FirstName != null && sq.FirstName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FirstName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.FirstName != null && sq.FirstName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FirstName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.FirstName != null && sq.FirstName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FirstName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "FirstName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.FirstName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "FirstName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.FirstName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "LastName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.LastName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "LastName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.LastName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.TerCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "CFSAActive" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CFSAActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "CFSAActive" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.CFSAActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        #region Load Datas To List Box

        #region Load Team Name For List Box

        public JsonResult JsonLoadTeamNameForCFSA()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var item = (from a in genData.Get_Team_ByXML() where a.TN != null select new { a.TID, a.TN }).ToList();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.TN,
                    Value = x.TID.ToString()
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Load Region Name For List Box

        public JsonResult JsonLoadRegionNameForCFSA()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var item = (from a in genData.Get_Region_ByXML() where a.RN != null select new { a.RID, a.RN }).ToList();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.RN,
                    Value = x.RID.ToString()
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Load District Name For List Box

        public JsonResult JsonLoadDistrictNameForCFSA()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var item = (from a in genData.Get_District_ByXML() where a.DN != null select new { a.DID, a.DN }).ToList();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.DN,
                    Value = x.DID.ToString()
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

        #region Load Territory Name For List Box

        public JsonResult JsonLoadTerritoryNameForCFSA()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            var item = (from a in genData.Get_Territory_ByXML() where a.TerName != null select new { a.TerID, a.TerName }).ToList();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.TerName,
                    Value = x.TerID.ToString()
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        #endregion


        #endregion

        #region List Box Team Clicked Load Rest of the list boxes

        public JsonResult JsonLoadSelectedRegionName(string SelectedTeamID)
        {

            DataSet ds = bLayer.GetRegionForTeamBased(SelectedTeamID);

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new RegionForTeamBased { RID = dataRow[0].ToString(), RN = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.RN,
                    Value = x.RID
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonLoadSelectedDistrictName(string SelectedTeamID)
        {

            DataSet ds = bLayer.GetDistrictForTeamBased(SelectedTeamID);

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new DistrictForTeamBased { DID = dataRow[0].ToString(), DN = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.DN,
                    Value = x.DID
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonLoadSelectedTerritoryName(string SelectedTeamID)
        {

            DataSet ds = bLayer.GetTerritoryForTeamBased(SelectedTeamID);

            var item = ds.Tables[0].AsEnumerable().Select(dataRow => new TerritoryForTeamBased { TerID = dataRow[0].ToString(), TerName = dataRow[1].ToString() }).ToList();

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.TerName,
                    Value = x.TerID
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }



        #endregion

        #region Active Conditions

        [HttpPost]
        public ActionResult CFSAActivateInActivateMaster(string gridData)
        {

            string msg = "";
            string status = "";
            var serialize = new JavaScriptSerializer();
            var deserialize = serialize.Deserialize<List<ClsSettingMasterTeamwise.CFSAMaster>>(gridData);

            foreach (var item in deserialize)
            {

                if ((item.chks.Equals("1")) && (item.CFSAActive.Equals("InActive")))
                {
                    status = "1";
                    genData.CFSAActivateORInActivateByXML(status, item.UserFKID);
                }
            }

            return Json(msg);

        }

        #endregion

        #region InActive Conditions

        [HttpPost]
        public ActionResult CFSAInActivateMaster(string gridData)
        {

            string msg = "";
            string status = "";
            var serialize = new JavaScriptSerializer();
            var deserialize = serialize.Deserialize<List<ClsSettingMasterTeamwise.CFSAMaster>>(gridData);

            foreach (var item in deserialize)
            {
                if ((item.chks.Equals("1")) && (item.CFSAActive.Equals("Active")))
                {
                    status = "0";
                    genData.CFSAActivateORInActivateByXML(status, item.UserFKID);
                }

            }

            return Json(msg);

        }

        #endregion

        #region Load PDF, Excel, CSV Files

        public ActionResult ExportCFSAMasterToExcel(string PKID)
        {
            try
            {
                DataSet ds = bLayer.GetCFSAMasterGrid(PKID);

                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new GetCFSAMaster
                {

                    FirstName = dataRow[0].ToString(),
                    LastName = dataRow[1].ToString(),
                    TerCode = dataRow[3].ToString(),
                    CFSAActive = dataRow[4].ToString(),
                }).ToList();

                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.FirstName == null ? "" :item.FirstName,
                 item.LastName == null ? "" :item.LastName,
                 item.TerCode == null ? "" :item.TerCode,
                 item.CFSAActive == null ? "" :item.CFSAActive,
             
                
               
            }));
                return new ExcelResult(HeadersCFSAMasters, ColunmTypesCFSAMasters, data, "CFSAMaster.xlsx", "CFSAMaster");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersCFSAMasters = {
               "FirstName","LastName","TerCode","CFSAActive"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesCFSAMasters = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportCFSAMasterPDF(string PKID)
        {
            try
            {
                DataSet ds = bLayer.GetCFSAMasterGrid(PKID);

                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new GetCFSAMaster
                {

                    FirstName = dataRow[0].ToString(),
                    LastName = dataRow[1].ToString(),
                    TerCode = dataRow[3].ToString(),
                    CFSAActive = dataRow[4].ToString(),
                }).ToList();

                List<CFSAMasters> userList = new List<CFSAMasters>();

                foreach (var item in context)
                {
                    CFSAMasters user = new CFSAMasters();
                    // user.ProductFkid = item.ProductFKID.ToString();
                    user.FirstName = item.FirstName == null ? "" : item.FirstName;
                    user.LastName = item.LastName == null ? "" : item.LastName;
                    user.TerCode = item.TerCode == null ? "" : item.TerCode;
                    user.CFSAActive = item.CFSAActive == null ? "" : item.CFSAActive;
                    userList.Add(user);
                }
                var customerList = userList;


                pdf.ExportPDF(customerList, new string[] { "FirstName", "LastName", "TerCode", "CFSAActive" }, path);
                return File(path, "application/pdf", "CFSAMaster.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportCFSAMasterCsv(string PKID)
        {
            try
            {
                DataSet ds = bLayer.GetCFSAMasterGrid(PKID);

                var model = ds.Tables[0].AsEnumerable().Select(dataRow => new GetCFSAMaster
                {

                    FirstName = dataRow[0].ToString(),
                    LastName = dataRow[1].ToString(),
                    TerCode = dataRow[3].ToString(),
                    CFSAActive = dataRow[4].ToString(),
                }).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("FirstName,LastName,TerCode,CFSAActive");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record == null ? "" : record.FirstName, record == null ? "" : record.LastName, record == null ? "" : record.TerCode, record == null ? "" : record.CFSAActive

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "CFSAMaster.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class CFSAMasters
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string TerCode { get; set; }
            public string CFSAActive { get; set; }

        }

        #endregion


        #endregion


        #region Chemist Master Admin

        #region Chemist Master Action Controller

        public ActionResult ChemistMaster()
        {

            return View();
        }

        #endregion

        #region Grid Load

        public JsonResult GetChemistMasterAdmin(GridQueryModel gridQueryModel, string PKID)
        {
            try
            {

                DataSet ds = bLayer.GetChemistAdminMasterGrid(PKID);

                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = ds.Tables[0].AsEnumerable().Select(dataRow => new GetChemistMasterAdminData
                {

                    PKID = dataRow[0].ToString(),
                    ChemistName = dataRow[1].ToString(),
                    FirstName = dataRow[2].ToString(),
                    MiddleName = dataRow[3].ToString(),
                    LastName = dataRow[4].ToString(),
                    Address = dataRow[5].ToString(),
                    Street = dataRow[6].ToString(),
                    Zip = dataRow[7].ToString(),
                    PhoneNo = dataRow[8].ToString(),
                    Mobile = dataRow[9].ToString(),
                    Fax = dataRow[10].ToString(),
                    Email = dataRow[11].ToString(),
                    PrescriptionPerMonth = dataRow[12].ToString(),
                    AvgCustomers = dataRow[13].ToString(),
                    NoCustomersMed = dataRow[14].ToString(),
                    OTCSales = dataRow[15].ToString(),
                    AreaFKID = dataRow[16].ToString(),
                    IsActive = dataRow[17].ToString(),
                    AreaName = dataRow[18].ToString(),
                    CLMStatus = dataRow[20].ToString(),
                }).ToList();


                var count = query.Count();
                var pageData = query.OrderBy(x => x.ChemistName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "ChemistName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.ChemistName != null && sq.ChemistName.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ChemistName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.ChemistName != null && sq.ChemistName.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ChemistName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.ChemistName != null && sq.ChemistName.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ChemistName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.ChemistName != null && sq.ChemistName.ToUpper().ToString().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ChemistName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "ChemistName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.ChemistName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ChemistName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ChemistName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "CLMStatus" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CLMStatus ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "CLMStatus" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.CLMStatus descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        #endregion

        #region Add Chemist Name

        public JsonResult AddChemistAdminMaster(string id, string oper, string ChemistName, string FirstName, string MiddleName, string LastName, string Address, string Street, string AreaName, string Zip, string PhoneNo, string Mobile, string Fax, string Email, string PrescriptionPerMonth, string IsActive, string AvgCustomers, string NoCustomersMed, string OTCSales)
        {
            try
            {
                Decimal CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string isd = "";
                string std = "";

                string msg = string.Empty;

                var rslt = (from a in genData.ChemistMasters where a.ChemistName == ChemistName select a).ToList();
                if (rslt.Count() > 0)
                    msg = "Chemist Name already Exists";
                else
                {


                    ChemistName = ChemistName.TrimStart();
                    FirstName = FirstName.TrimStart();
                    LastName = LastName.TrimStart();
                    MiddleName = MiddleName.TrimStart();
                    Address = Address.TrimStart();
                    Street = Street.TrimStart();
                    Zip = Zip.TrimStart();
                    PhoneNo = PhoneNo.TrimStart();
                    Mobile = Mobile.TrimStart();
                    Fax = Fax.TrimStart();
                    Email = Email.TrimStart();
                    PrescriptionPerMonth = PrescriptionPerMonth.TrimStart();
                    AvgCustomers = AvgCustomers.TrimStart();
                    NoCustomersMed = NoCustomersMed.TrimStart();
                    OTCSales = OTCSales.TrimStart();

                    int Result = genData.AddChemistMaster(ChemistName, FirstName, MiddleName, LastName, Address, Street, Convert.ToDecimal(AreaName), Zip, isd, std, PhoneNo, Mobile, Fax, Email, Convert.ToInt32(PrescriptionPerMonth), true, CreatedBy, false, Convert.ToInt32(AvgCustomers), Convert.ToInt32(NoCustomersMed), Convert.ToInt32(OTCSales));

                    msg = "Chemist Name added Successfully.";

                }

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }

        #endregion

        #region Edit For Chemist Master

        public JsonResult EditChemistMasterAdmin(string PKID, string oper, string ChemistName, string FirstName, string MiddleName, string LastName, string Address, string Street, string AreaName, string Zip, string PhoneNo, string Mobile, string Fax, string Email, string PrescriptionPerMonth, string IsActive, string AvgCustomers, string NoCustomersMed, string OTCSales)
        {
            try
            {
                Decimal CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                string isd = "";
                string std = "";
                string msg = string.Empty;

                ChemistName = ChemistName.TrimStart();
                FirstName = FirstName.TrimStart();
                LastName = LastName.TrimStart();
                MiddleName = MiddleName.TrimStart();
                Address = Address.TrimStart();
                Street = Street.TrimStart();
                Zip = Zip.TrimStart();


                PhoneNo = PhoneNo.TrimStart();
                Mobile = Mobile.TrimStart();
                Fax = Fax.TrimStart();
                Email = Email.TrimStart();
                PrescriptionPerMonth = PrescriptionPerMonth.TrimStart();
                AvgCustomers = AvgCustomers.TrimStart();
                NoCustomersMed = NoCustomersMed.TrimStart();
                OTCSales = OTCSales.TrimStart();

                int Result = genData.EditChemistMaster(Convert.ToDecimal(PKID), ChemistName, FirstName, MiddleName, LastName, Address, Street, Convert.ToDecimal(AreaName), Zip, isd, std, PhoneNo, Mobile, Fax, Email, Convert.ToInt32(PrescriptionPerMonth), true, CreatedBy, Convert.ToInt32(AvgCustomers), Convert.ToInt32(NoCustomersMed), Convert.ToInt32(OTCSales));


                msg = "Chemist Name Updated Successfully.";


                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }

        #endregion

        #region Delete Chemist Name

        public JsonResult DeleteChemistAdminMaster(string PKID)
        {
            try
            {
                string msg = string.Empty;

                int Result = genData.DeleteChemistMaster1(Convert.ToDecimal(PKID));
                msg = "Chemist Name Deleted Successfully.";

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }

        #endregion

        #region Create Partial View and Load DropDown

        public ActionResult Getselectcharactorcityname(string AreaFKID, string type)
        {
            ViewBag.Type = type;
            if (AreaFKID == "undefined" || AreaFKID == null)
            {
                var Query = (from a in genData.GetCityFCforTerritoryByXML() select new { a.CityName }).ToDictionary(f => f.CityName, f => f.CityName.ToString());
                return PartialView("PVForLoadFCCityNameForChemist", Query);
            }
            else
            {
                var Query = (from a in genData.GetCityNameForAreaXML(Convert.ToDecimal(AreaFKID)) select new { a.CityFKID, a.CityName }).ToDictionary(f => Convert.ToInt32(f.CityFKID), f => f.CityName.ToString());
                return PartialView("PVChemistMasterAdmin", Query);

            }



        }

        public ActionResult GetselFCcitynameForChemist(string Type, string FCcityname)
        {
            ViewBag.Type = Type;
            var Query = (from a in genData.GetCityforTerritoryByXML(FCcityname) select new { a.CityFKID, a.CityName }).ToDictionary(f => Convert.ToInt32(f.CityFKID), f => f.CityName.ToString());
            return PartialView("PVChemistMasterAdmin", Query);


        }

        public ActionResult GetselCityForFirstChaCityName(string Type, string FCCName)
        {
            ViewBag.Type = Type;
            if (FCCName == "undefined" || FCCName == null)
            {
                dynamic query = "";
                query = (from a in genData.GetCityforTerritoryByXML("0") where 1 == 2 select new { a.CityFKID, a.CityName }).ToDictionary(f => f.CityName, f => f.CityName.ToString());
                return PartialView("PVForLoadFCCityNameForChemist", query);
            }
            else
            {
                var Query = (from a in genData.GetCityforTerritoryByXML(FCCName) select new { a.CityFKID, a.CityName }).ToDictionary(f => Convert.ToInt32(f.CityFKID), f => f.CityName.ToString());
                return PartialView("PVChemistMasterAdmin", Query);
            }

        }

        public ActionResult GetselAreaForChemist(string Type, string CityFKID)
        {
            ViewBag.Type = Type;
            decimal ss = 0;
            if (CityFKID == "undefined" || CityFKID == null)
            {
                dynamic query = "";
                query = (from a in genData.GetAreaByCityFKIDXML(ss) where 1 == 2 select new { a.AreaFKID, a.AreaName }).ToDictionary(f => f.AreaName, f => f.AreaName.ToString());
                return PartialView("PVForLoadFCCityNameForChemist", query);
            }
            else
            {
                var Query = (from a in genData.GetAreaByCityFKIDXML(Convert.ToDecimal(CityFKID)) select new { a.AreaFKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.AreaFKID), f => f.AreaName.ToString());
                return PartialView("PVChemistMasterAdmin", Query);
            }

        }


        public ActionResult GetLoadAreaForChemist(string Type, string CityFKID, string CityFKIDS)
        {
            ViewBag.Type = Type;

            var Query = (from a in genData.GetAreaByCityFKIDXML(Convert.ToDecimal(CityFKID)) select new { a.AreaFKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.AreaFKID), f => f.AreaName.ToString());
            return PartialView("PVChemistMasterAdmin", Query);


        }

        public JsonResult JsonGetEditFirstNameCity(string AreaFKID)
        {

            string genId = "";
            string cityFkid = "";

            var Query = (from a in genData.GetCityNameForAreaXML(Convert.ToDecimal(AreaFKID)) select new { a.CityFKID, a.CityName }).First();

            return Json(new
            {
                genId = Query.CityName,
                cityFkid = Query.CityFKID,
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonLoadCityNameForEdit(string CityName)
        {


            long? USER_FKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in genData.GetCityforTerritoryByXML(CityName) select new { a.CityFKID, a.CityName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.CityName,
                    Value = x.CityFKID.ToString()
                }).ToList();
            }


            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult JsonLoadAreaNameForEdit(string CityFkid)
        {


            long? USER_FKID = Convert.ToInt64(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in genData.GetAreaByCityFKIDXML(Convert.ToDecimal(CityFkid)) select new { a.AreaFKID, a.AreaName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.AreaName,
                    Value = x.AreaFKID.ToString()
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

        public ActionResult ExportChemistMasterAdminToExcel(string PKID)
        {
            try
            {
                DataSet ds = bLayer.GetChemistAdminMasterGrid(PKID);

                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new GetChemistMasterAdminData
                {

                    ChemistName = dataRow[1].ToString(),
                    AreaName = dataRow[18].ToString(),
                    IsActive = dataRow[17].ToString(),
                    CLMStatus = dataRow[20].ToString(),
                }).ToList();

                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.ChemistName == null ? "" :item.ChemistName,
                 item.AreaName == null ? "" :item.AreaName,
                 item.IsActive == null ? "" :item.IsActive,
                 item.CLMStatus == null ? "" :item.CLMStatus,
             
                
               
            }));
                return new ExcelResult(HeadersChemistMasterAdmins, ColunmTypesChemistMasterAdmins, data, "ChemistMasterAdmin.xlsx", "ChemistMasterAdmin");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersChemistMasterAdmins = {
               "ChemistName","Area","Status","TerritorylinkStatus"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesChemistMasterAdmins = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportChemistMasterAdminToPDF(string PKID)
        {
            try
            {
                DataSet ds = bLayer.GetChemistAdminMasterGrid(PKID);

                var context = ds.Tables[0].AsEnumerable().Select(dataRow => new GetChemistMasterAdminData
                {

                    ChemistName = dataRow[1].ToString(),
                    AreaName = dataRow[18].ToString(),
                    IsActive = dataRow[17].ToString(),
                    CLMStatus = dataRow[20].ToString(),
                }).ToList();

                List<ChemistMastersAdmin> userList = new List<ChemistMastersAdmin>();

                foreach (var item in context)
                {
                    ChemistMastersAdmin user = new ChemistMastersAdmin();
                    // user.ProductFkid = item.ProductFKID.ToString();
                    user.ChemistName = item.ChemistName == null ? "" : item.ChemistName;
                    user.Area = item.AreaName == null ? "" : item.AreaName;
                    user.Status = item.IsActive == null ? "" : item.IsActive;
                    user.TerritoryLinkStatus = item.CLMStatus == null ? "" : item.CLMStatus;
                    userList.Add(user);
                }
                var customerList = userList;


                pdf.ExportPDF(customerList, new string[] { "ChemistName", "Area", "Status", "TerritoryLinkStatus" }, path);
                return File(path, "application/pdf", "ChemistMasterAdmin.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportChemistMasterAdminToCsv(string PKID)
        {
            try
            {
                DataSet ds = bLayer.GetChemistAdminMasterGrid(PKID);

                var model = ds.Tables[0].AsEnumerable().Select(dataRow => new GetChemistMasterAdminData
                {

                    ChemistName = dataRow[1].ToString(),
                    AreaName = dataRow[18].ToString(),
                    IsActive = dataRow[17].ToString(),
                    CLMStatus = dataRow[20].ToString(),
                }).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("ChemistName,Area,Status,TerritorylinkStatus");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record == null ? "" : record.ChemistName, record == null ? "" : record.AreaName, record == null ? "" : record.IsActive, record == null ? "" : record.CLMStatus

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "ChemistMasterAdmin.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class ChemistMastersAdmin
        {
            public string ChemistName { get; set; }
            public string Area { get; set; }
            public string Status { get; set; }
            public string TerritoryLinkStatus { get; set; }

        }

        #endregion

        #endregion


        #region Nagarajan

        #region UnlockCyclePlanRetailer
        ////UnlockCyclePlanRetailer

        public ActionResult UnlockCyclePlanRetailer()
        {
            return View();
        }

        ////Unlock CyclePlanRetailer Data Load

        public ActionResult GEtUnlockPlanRetailer(GridQueryModel gridQueryModel, int Cycle, int Division, int Team)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;

                var searchField = gridQueryModel.searchField;
                var query = (from a in genData.Usp_GetPSOsForCyclePlanRetailerUnlock(Cycle, Division, Team) select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.PsoName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                count = query.Count();
                if (gridQueryModel.sidx == "PsoName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.PsoName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PsoName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.PsoName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "TERRITORYCODE" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.TERRITORYCODE ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "TERRITORYCODE" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.TERRITORYCODE descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ReptUserName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.ReptUserName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ReptUserName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.ReptUserName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CPCreatedDate" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.CPCreatedDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CPCreatedDate" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.CPCreatedDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        ////Unlock Cycle plan Insert Query
        [HttpPost]
        public ActionResult UnlockCyclePlanRetailer(string gridData, string SelectDivision)
        {
            try
            {
                decimal UserFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string msg = "";
                var j = 1;
                var serialize = new JavaScriptSerializer();
                var deserialize = serialize.Deserialize<List<UnlockCyclePlanRetailer>>(gridData);
                var str = "<row>";
                foreach (var item in deserialize)
                {
                    if (item.chks.Equals("Yes"))
                    {
                        str += "<dp ";
                        str += " PkID='" + j + "'";
                        str += " PSOFKID='" + item.PSOFKID + "'";
                        str += "/>";
                    }
                    j = j + 1;
                }
                str += "</row>";
                genData.Usp_UpdtForUnLockingCyclePlanRetailer(UserFKID, Convert.ToDecimal(SelectDivision), str);

                msg = "Cycle Plan Retailer";
                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }

        #endregion

        #region Unlock Cycle Plan

        ////Unlock Cycle Plan
        public ActionResult UnlockCyclePlan()
        {
            return View();
        }
        ////Unlock Cycle Plan Year Cycle
        public JsonResult JsonGetYear()
        {
            DataTable dtUnlockPlan = new DataTable();
            dtUnlockPlan = bLayer.UnlockCyclePlanYear();
            var Unlock = dtUnlockPlan.AsEnumerable().Select(dataRow => new GetCycleYear
            {
                Element = dataRow[0].ToString(),
                PKID = dataRow[1].ToString(),
                cycleName = dataRow[2].ToString(),
                Year = dataRow[3].ToString(),
                CyclesMonth = dataRow[4].ToString(),
                StartMonth = Convert.ToDecimal(dataRow[5]),

            }).ToList();
            var items = (from a in Unlock select new { a.PKID, a.cycleName, a.Year, a.CyclesMonth });
            return Json(items);

        }

        ////Unlock Data Load
        public JsonResult GetUnlockCyclePlan(GridQueryModel gridQueryModel, int Cycle, int Division, int Team)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from a in genData.Usp_GetPSOsForCyclePlanUnlock(Cycle, Division, Team) select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.PsoName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                count = query.Count();
                if (gridQueryModel.sidx == "PsoName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.PsoName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "PsoName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.PsoName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "TERRITORYCODE" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.TERRITORYCODE ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "TERRITORYCODE" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.TERRITORYCODE descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ReptUserName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.ReptUserName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ReptUserName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.ReptUserName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CPCreatedDate" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.CPCreatedDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CPCreatedDate" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.CPCreatedDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        ////Unlock CyclePlan Division
        public ActionResult GetDivisionLoad()
        {
            List<SelectListItem> UseDivision = new List<SelectListItem>();
            UseDivision.Clear();
            var item = (from a in genData.GetSampleDivisionByXML() select new { a.DIVPKID, a.DivName }).ToList();
            if (item.Count() > 0)
            {
                UseDivision = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.DivName,
                    Value = x.DIVPKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                DivisionName = UseDivision,
                DivisionCount = UseDivision.Count,
            }, JsonRequestBehavior.AllowGet);


        }
        ////Unlock CyclePlan Team
        public ActionResult JsonLoadSelectTeam(string SelectDivision)
        {
            List<SelectListItem> UseTeam = new List<SelectListItem>();
            UseTeam.Clear();
            var item = (from a in genData.GetTeamNameByXml(Convert.ToDecimal(SelectDivision)) select new { a.TeamPKID, a.TeamName }).ToList();
            if (item.Count() > 0)
            {
                UseTeam = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.TeamName,
                    Value = x.TeamPKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                TeamNamke = UseTeam,
                TeamCount = UseTeam.Count,
            }, JsonRequestBehavior.AllowGet);

        }
        ////Unlock Cycle plan Insert Query
        [HttpPost]
        public ActionResult UnlockCycleInserQuery(string gridData, string SelectDivision)
        {
            try
            {
                decimal UserFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string msg = "";
                var j = 1;
                var serialize = new JavaScriptSerializer();
                var deserialize = serialize.Deserialize<List<UnlockCyclePlan>>(gridData);
                var str = "<row>";
                foreach (var item in deserialize)
                {
                    if (item.chks.Equals("Yes"))
                    {
                        str += "<dp ";
                        str += " PkID='" + j + "'";
                        str += " PSOFKID='" + item.PSOFKID + "'";
                        str += " UserFkid='" + UserFKID + "'";
                        str += " CycleFKID='" + SelectDivision + "'";
                        str += "/>";
                    }
                    j = j + 1;
                }
                str += "</row>";
                genData.Usp_UpdtForUnLockingCyclePlan(str);
                msg = "CyclePlan Successfully Unlocked!";
                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }


        #endregion

        #region Customer Deletion

        public ActionResult CustomerDeletion()
        {
            return View();
        }
        ////New Request
        public JsonResult GridNewRequest(GridQueryModel gridQueryModel, string Brick, string Specialization, string Status)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                DataTable dsCustomerDeletion = new DataTable();
                string psoFkid = Convert.ToString(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string Bricks = Brick.ToString() == "" ? "0" : Brick.ToString();
                string Specializations = Specialization.ToString() == "" ? "0" : Specialization.ToString();
                string Statuss = Status.ToString() == "" ? "0" : Status.ToString();
                dsCustomerDeletion = bLayer.GetAllRequestCustormeDeletion(psoFkid, Bricks, Specializations, Statuss);
                var NewPerson = dsCustomerDeletion.AsEnumerable().Select(dataRow => new CustomerDeletion
                {
                    ELement = dataRow[0].ToString(),
                    ordID = Convert.ToDecimal(dataRow[1]),
                    PKID = Convert.ToDecimal(dataRow[2]),
                    CustomerName = dataRow[3].ToString(),
                    DoctorFKID = dataRow[4].ToString(),
                    Type = dataRow[5].ToString(),
                    BrickName = dataRow[6].ToString(),
                    Specialization = dataRow[7].ToString(),
                    MCLFlag = dataRow[8].ToString(),

                }).ToList();
                var query = (from a in NewPerson select new { a.PKID, a.DoctorFKID, a.CustomerName, a.Type, a.BrickName, a.Specialization, a.MCLFlag }).ToList();
                var Records = query.Count();
                var pageData = query.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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
        ////Pending Request
        public JsonResult GridPendingRequest(GridQueryModel gridQueryModel, string Brick, string Specialization, string Status)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                //Variable Declaration
                DataTable dsCustomerDeletion = new DataTable();
                //User Loginid
                string psoFkid = Convert.ToString(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                //string psoFkid = "339";
                string Bricks = Brick.ToString() == "" ? "0" : Brick.ToString();
                string Specializations = Specialization.ToString() == "" ? "0" : Specialization.ToString();
                string Statuss = Status.ToString() == "" ? "0" : Status.ToString();
                dsCustomerDeletion = bLayer.GetAllPendingCustormeDeletion(psoFkid, Bricks, Specializations, Statuss);
                var NewPerson = dsCustomerDeletion.AsEnumerable().Select(dataRow => new CustomerDeletion
                {
                    ELement = dataRow[0].ToString(),
                    ordID = Convert.ToDecimal(dataRow[1]),
                    PKID = Convert.ToDecimal(dataRow[2]),
                    CustomerName = dataRow[3].ToString(),
                    Type = dataRow[4].ToString(),
                    BrickName = dataRow[5].ToString(),
                    Specialization = dataRow[6].ToString(),
                    MCLFlag = dataRow[7].ToString(),
                    Reason = dataRow[8].ToString(),
                    RequestDate = dataRow[9].ToString()

                }).ToList();
                var query = (from a in NewPerson select new { a.PKID, a.RequestDate, a.CustomerName, a.Type, a.BrickName, a.Specialization, a.MCLFlag, a.Reason }).ToList();
                var Records = query.Count();
                var pageData = query.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        ////Approval Request 

        public JsonResult GridApprovalRequest(GridQueryModel gridQueryModel, string Brick, string Specialization, string Status)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                //Variable Declaration
                DataTable dsCustomerDeletion = new DataTable();
                //User Loginid
                string psoFkid = Convert.ToString(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string Bricks = Brick.ToString() == "" ? "0" : Brick.ToString();
                string Specializations = Specialization.ToString() == "" ? "0" : Specialization.ToString();
                string Statuss = Status.ToString() == "" ? "0" : Status.ToString();
                dsCustomerDeletion = bLayer.GetAllApprovedCustormeDeletion(psoFkid, Bricks, Specializations, Statuss);
                var NewPerson = dsCustomerDeletion.AsEnumerable().Select(dataRow => new CustomerDeletion
                {
                    ordID = Convert.ToDecimal(dataRow[0]),
                    PKID = Convert.ToDecimal(dataRow[1]),
                    CustomerName = dataRow[2].ToString(),
                    Type = dataRow[3].ToString(),
                    BrickName = dataRow[4].ToString(),
                    Specialization = dataRow[5].ToString(),
                    MCLFlag = dataRow[6].ToString(),
                    RequestDate = dataRow[7].ToString(),
                    ApprovedBy = dataRow[8].ToString(),
                    ApprovedDate = dataRow[9].ToString()

                }).ToList();
                var query = (from a in NewPerson select new { a.PKID, a.RequestDate, a.CustomerName, a.Type, a.BrickName, a.Specialization, a.MCLFlag, a.ApprovedDate, a.ApprovedBy }).ToList();
                var Records = query.Count();
                var pageData = query.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        ////Reject Request

        public JsonResult GridRejectRequest(GridQueryModel gridQueryModel, string Brick, string Specialization, string Status)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                //Variable Declaration
                DataTable dsCustomerDeletion = new DataTable();
                //User Loginid
                string psoFkid = Convert.ToString(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                //string psoFkid = "339";
                string Bricks = Brick.ToString() == "" ? "0" : Brick.ToString();
                string Specializations = Specialization.ToString() == "" ? "0" : Specialization.ToString();
                string Statuss = Status.ToString() == "" ? "0" : Status.ToString();
                dsCustomerDeletion = bLayer.GetAllRejectCustormeDeletion(psoFkid, Bricks, Specializations, Statuss);
                var NewPerson = dsCustomerDeletion.AsEnumerable().Select(dataRow => new CustomerDeletion
                {
                    ordID = Convert.ToDecimal(dataRow[0]),
                    PKID = Convert.ToDecimal(dataRow[1]),
                    CustomerName = dataRow[2].ToString(),
                    Type = dataRow[3].ToString(),
                    BrickName = dataRow[4].ToString(),
                    Specialization = dataRow[5].ToString(),
                    MCLFlag = dataRow[6].ToString(),
                    RejectReason = dataRow[7].ToString(),
                    RequestDate = dataRow[8].ToString(),
                    RejectBy = dataRow[9].ToString(),
                    RejectDate = dataRow[10].ToString()

                }).ToList();
                var query = (from a in NewPerson select new { a.PKID, a.RequestDate, a.CustomerName, a.Type, a.Specialization, a.BrickName, a.MCLFlag, a.RejectDate, a.RejectBy, a.RejectReason }).ToList();
                var Records = query.Count();
                var pageData = query.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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


        //View Doctor and Chemist
        #region Fetch Doctor and Chemist Details
        public JsonResult JsonGetDoctorList(GridQueryModel gridQueryModel, string DocID, string Type, string flag)
        {
            string FName = "", MName = "", LName = "", Address = "", Street = "", Specialization = "", AreaName = "", AreaFKID = "", Zipcode = "", TelClinic = "", TelRoles = "", Mobile = "", Email = "", CycleFKID = "", element = "", PhoneNo = "";

            DataTable dtCycleNo = bLayer.GetCurrentCycleForPSoNumber();

            if (dtCycleNo.Rows.Count > 0)

                CycleFKID = dtCycleNo.Rows[0]["PKID"].ToString();

            DataSet ds = bLayer.GetDoctorChemistProfile(DocID, Session["USER_FKID"].ToString(), Type, CycleFKID);

            List<UsefulLink> objList = new List<UsefulLink>();

            foreach (DataTable dt in ds.Tables)
            {
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        FName = dr["FirstName"].ToString();
                        MName = dr["MiddleName"].ToString();
                        LName = dr["LastName"].ToString();
                        Address = dr["Address"].ToString();
                        Street = dr["Street"].ToString();

                        AreaName = dr["AreaName"].ToString();
                        AreaFKID = dr["AreaFKID"].ToString();


                        Mobile = dr["Mobile"].ToString();
                        Email = dr["Email"].ToString();

                        if (Type == "Doctor")
                        {
                            Specialization = dr["Specialization"].ToString();
                            TelClinic = dr["TelClinic"].ToString();
                            TelRoles = dr["TelRes"].ToString();
                            Zipcode = dr["ZipCode"].ToString();
                        }
                        else if (Type == "Chemist")
                        {

                            PhoneNo = dr["PhoneNo"].ToString();
                            Zipcode = dr["Zip"].ToString();
                        }

                        break;



                    }

                }
                break;

            }
            return Json(new
            {

                FName,
                MName,
                LName,
                Address,
                Street,
                Specialization,
                AreaName,
                AreaFKID,
                Zipcode,
                TelClinic,
                TelRoles,
                Mobile,
                Email,
                PhoneNo
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion
        #region Fetch Doctor and Chemist Call History
        public JsonResult GetDoctorDetailsforGrid(GridQueryModel gridQueryModel, string DocID, string Type)
        {

            DataTable dtCycle = bLayer.GetCurrentCycleForPSoNumber();
            string CycleFKID = "";
            if (dtCycle.Rows.Count > 0)
                CycleFKID = dtCycle.Rows[0]["PKID"].ToString();
            DataSet ds = bLayer.GetDoctorChemistProfile(DocID, Session["USER_FKID"].ToString(), Type, CycleFKID);
            List<UsefulLink> objList = new List<UsefulLink>();

            if (ds.Tables.Count > 1)
            {
                if (ds.Tables[1].Rows.Count > 0)
                {

                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        UsefulLink useful = new UsefulLink();
                        useful.LoginName = dr["Loginname"].ToString();
                        useful.Date = dr["Date"].ToString();
                        objList.Add(useful);

                    }
                }
            }
            else
            {
                UsefulLink useful = new UsefulLink();
                useful.LoginName = "";
                useful.Date = "";
                objList.Add(useful);

            }
            var count = objList.Count();
            var pageData = objList.OrderBy(x => x.Type).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
            return Json(new
            {
                page = gridQueryModel.page,
                records = count,
                rows = pageData,
                total = Math.Ceiling((decimal)count / gridQueryModel.rows)
            }, JsonRequestBehavior.AllowGet);



        }
        #endregion



        //// Customer Insert Query
        [HttpPost]
        public ActionResult CustomerDeletionNewRequest(string gridData)
        {
            try
            {
                decimal PSOFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string msg = "";
                var serialize = new JavaScriptSerializer();
                var deserialize = serialize.Deserialize<List<ClsApproval>>(gridData);
                var str = "<root>";
                foreach (var item in deserialize)
                {
                    if (item.chks.Equals("Yes"))
                    {
                        str += "<CustomerDetails>";
                        str += "<PKID>" + item.PKID + "</PKID>";
                        str += "<PSOFKID>" + PSOFKID + "</PSOFKID>";
                        str += "<Type>" + item.Type + "</Type>";
                        str += "<Reson>" + item.Reason + "</Reson>";
                        str += "</CustomerDetails>";
                    }
                }
                str += "</root>";


                if (str != "<root></root>")
                {
                    genData.EditCustomDeletions(str);
                    msg = "New Request Successfully.";
                }
                else
                {
                    msg = "notselect";

                }
                return Json(new
                {
                    msg
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }


        #endregion


        #region LeaveApproval

        public ActionResult LeaveApproval()
        {
            return View();
        }

        public JsonResult GridDataLeaveApproval(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                Int32 Psoid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                var query = (from a in genData.GetLeaveApprovalNew(Convert.ToDecimal(Psoid)) select a).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.UserName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                count = query.Count();
                if (gridQueryModel.sidx == "UserName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.UserName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "UserName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.UserName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.CityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.CityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CreatedDate" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.CreatedDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CreatedDate" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.CreatedDate descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.GenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.GenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "FromDate" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.FromDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "FromDate" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.FromDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ToDate" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.ToDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "ToDate" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.ToDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "LeaveBalance" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.LeaveBalance ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "LeaveBalance" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.LeaveBalance ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "OnProcess" && gridQueryModel.sord == "asc")
                    pageData = (from cust in query orderby cust.OnProcess ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "OnProcess" && gridQueryModel.sord == "desc")
                    pageData = (from cust in query orderby cust.OnProcess ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        public JsonResult GetLeaveReasonGrid(string Reason)
        {
            dynamic query = (from a in genData.GetLeaveMasterReason(Convert.ToDecimal(Reason)) select a).ToList();
            return Json(query);

        }
      
        [HttpPost]
        public ActionResult LeaveApprovalDetails(string gridData)
        {
            try
            {
                decimal? PSOFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                string msg = "";
                var serialize = new JavaScriptSerializer();
               
                //Approval 
                var LeaveApproval1 = serialize.Deserialize<List<LeaveApproval>>(gridData);

                var strApproval = "<row>";
                foreach (var item in LeaveApproval1)
                {
                    if ((item.Approved.Equals("A")) && (!item.Rejected.Equals("R")))
                    {
                        strApproval += "<dp ";
                        strApproval += "PKID='" + item.PKID + "'";
                        strApproval += "Status='" + item.Approved + "'";
                        strApproval += "ReasonForApproveReject='" + item.Comments + "'";
                        strApproval += "ModifiedBy='" + PSOFKID + "'";
                        strApproval += "/>";
                    }
                }
                strApproval += "</row>";

                if (strApproval != "<row></row>")
                {
                    msg = "Leave Approval Submitted Successfully.";
                   // goto retry;
                }
                else
                {
                    msg = "notselect";
                    goto retry;
                }
                //Rejected
                var LeaveRejected1 = serialize.Deserialize<List<LeaveRejected>>(gridData);

                var strRejected = "<row>";
                foreach (var item in LeaveRejected1)
                {
                    if ((!item.Approved.Equals("A")) && (item.Rejected.Equals("R")))
                    {
                        strRejected += "<dp ";
                        strRejected += "PKID='" + item.PKID + "'";
                        strRejected += "Status='" + item.Rejected + "'";
                        strRejected += "ReasonForApproveReject='" + item.Comments + "'";
                        strRejected += "ModifiedBy='" + PSOFKID + "'";
                        strRejected += "/>";
                    }
                }
                strRejected += "</row>";

                if (strRejected != "<row></row>")
                {
                    msg = "Leave Rejected Submitted Successfully.";
                   // goto retry;
                }
                else
                {
                    msg = "notselect";
                    goto retry;
                }

                //Approval and Rejected
                var deserialize = serialize.Deserialize<List<LeaveApprovalRejected>>(gridData);
                foreach (var item in deserialize)
                {
                    if ((item.Approved.Equals("A")) && (item.Rejected.Equals("R")))
                    {
                        msg = "NoRejectApproval";
                        goto retry;
                    }
                }
                retry:
                return Json(new
                {
                   
                    msg
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }



        #endregion
        #endregion


        #region Baskar Master

        public ActionResult SpecialtyMappingMaster()
        {
            return View();
        }

        public JsonResult JsonLoadSpecialization()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in genData.MVCGetSpecializationNameByXml() select new { a.PKID, a.Specialization }).ToList().Distinct();

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


        public JsonResult JsonLoadSelectedTeamName(string Specialization)
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();

            var item = (from a in genData.MVCGetTeamsBySpecialityXML(Convert.ToDecimal(Specialization)) select new { a.PKID, a.TeamName }).ToList().Distinct();

            if (item.Count() > 0)
            {
                roleItems = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.TeamName,
                    Value = x.PKID.ToString()
                }).ToList();
            }

            return Json(new
            {
                countyItems = roleItems,
                countyCount = roleItems.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult JsonPostSpecializationDetails(string Specialization, string Team)
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            roleItems.Clear();
            string Result = "";

            XmlTransform trans = new XmlTransform();

            //var Retrslt = (from a in genData.MVCGetSpecializationandProductByXml(Convert.ToInt64(Specialization), Convert.ToInt64(Team)) select a).ToList();
            Result = bLayer.MVCGetSpecializationandProductByXml(Specialization, Team);
            trans.XslFile = xslPath + "SpecialtyProductLinkMasterNew.xsl";
            trans.XmlString = Result;
            var rslt = trans.Transform();
            rslt = rslt.Remove(0, 39);


            //var item = (from a in genData.MVCGetSpecializationandProductByXml(Convert.ToInt64(Specialization),Convert.ToInt64(Team)) select a).ToList().Distinct();


            return Json(new
            {
                rslt

            }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult JsPostSpecialtyMaster(string ProductName, string Specialization, string Team)
        {
            string alertMsg = string.Empty;
            var result = "";
            try
            {
                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                StringBuilder sb = new StringBuilder();
                sb.Append("<Root>");
                string[] arrayName = ProductName.Remove(ProductName.Length - 1, 1).Split(',');
                string[] rslt;
                foreach (var item in arrayName)
                {
                    rslt = item.Split('|');
                    sb.Append("<Doc1><Pro>" + rslt[0] + "</Pro>" + "<DR>" + rslt[1] + "</DR></Doc1>");
                }
                sb.Append("</Root>");

                int Result = genData.MVCAddSpecializationProductlinkMasterNew(Convert.ToDecimal(Specialization), Convert.ToDecimal(Team), sb.ToString(), ModifiedBy);

                if (Result >= 1)
                {
                    alertMsg = "Specialty Mapping Master added Successfully.";
                }

                return Json(new
                {
                    alertMsg

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
    }
}
