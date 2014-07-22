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
    public class MastersController : Controller
    {
        ExportPdf pdf = new ExportPdf();
        private pfizerEntities genData = new pfizerEntities();
        string path = ConfigurationSettings.AppSettings["PDFfilePath"].ToString();
        //
        // GET: /Masters/

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


            dynamic rslt = "";
            rslt = (from a in genData.GetAreaDetailsPCL(Convert.ToInt32(Session["Territory_FKID"])) select new { a.PKID, a.AreaName }).OrderBy(x => x.AreaName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
            return View(rslt);
        }



        //public JsonResult GridDataPCLtoMCLRequest(GridQueryModel gridQueryModel)
        //{
        //    //try
        //    //{
        //    //    var searchString = gridQueryModel.searchString;
        //    //    var searchOper = gridQueryModel.searchOper;
        //    //    var searchField = gridQueryModel.searchField;

        //    //    //Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
        //    //    decimal psoFkid = 339;

        //    //    var query = (from a in genData.AllPendingCustomer(Convert.ToDecimal(Session["USER_FKID"].ToString()),) select a).ToList();
        //    //    var count = query.Count();
        //    //    var pageData = query.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

        //    //    if (gridQueryModel._search == true && searchString != "")
        //    //    {
        //    //        if ((searchField == "NameofHospital") || (searchField == "IsActive"))
        //    //        {
        //    //            if (searchOper == "bw")//begins with
        //    //            {
        //    //                var q = (from sq in query where sq.NameofStockist.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
        //    //                count = q.Count();
        //    //                pageData = q.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //            }
        //    //            else if (searchOper == "eq") //equal
        //    //            {
        //    //                var q = (from sq in query where sq.NameofStockist.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
        //    //                count = q.Count();
        //    //                pageData = q.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //            }
        //    //            else if (searchOper == "ew") // ends with
        //    //            {
        //    //                var q = (from sq in query where sq.NameofStockist.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.EndsWith(searchString) select sq).ToList();
        //    //                count = q.Count();
        //    //                pageData = q.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //            }
        //    //            else if (searchOper == "cn")//contains
        //    //            {
        //    //                var q = (from sq in query where sq.NameofStockist.Contains(searchString) || sq.Status.Contains(searchString) select sq).ToList();
        //    //                count = q.Count();
        //    //                pageData = q.OrderBy(x => x.NameofStockist).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //            }
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        count = query.Count();
        //    //        if (gridQueryModel.sidx == "NameofStockist" && gridQueryModel.sord == "asc")
        //    //            pageData = (from cust in query orderby cust.NameofStockist ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //        else
        //    //            if (gridQueryModel.sidx == "NameofStockist" && gridQueryModel.sord == "desc")
        //    //                pageData = (from cust in query orderby cust.NameofStockist descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

        //    //        if (gridQueryModel.sidx == "StockistAddress" && gridQueryModel.sord == "asc")
        //    //            pageData = (from cust in query orderby cust.StockistAddress ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //        else
        //    //            if (gridQueryModel.sidx == "StockistAddress" && gridQueryModel.sord == "desc")
        //    //                pageData = (from cust in query orderby cust.StockistAddress descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


        //    //        if (gridQueryModel.sidx == "City" && gridQueryModel.sord == "asc")
        //    //            pageData = (from cust in query orderby cust.City ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //        else
        //    //            if (gridQueryModel.sidx == "City" && gridQueryModel.sord == "desc")
        //    //                pageData = (from cust in query orderby cust.City descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

        //    //        if (gridQueryModel.sidx == "Street" && gridQueryModel.sord == "asc")
        //    //            pageData = (from cust in query orderby cust.Street ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //        else
        //    //            if (gridQueryModel.sidx == "Street" && gridQueryModel.sord == "desc")
        //    //                pageData = (from cust in query orderby cust.Street descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


        //    //        if (gridQueryModel.sidx == "PhoneNumber" && gridQueryModel.sord == "asc")
        //    //            pageData = (from cust in query orderby cust.PhoneNumber ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //        else
        //    //            if (gridQueryModel.sidx == "PhoneNumber" && gridQueryModel.sord == "desc")
        //    //                pageData = (from cust in query orderby cust.PhoneNumber descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


        //    //        if (gridQueryModel.sidx == "Email" && gridQueryModel.sord == "asc")
        //    //            pageData = (from cust in query orderby cust.Email ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //        else
        //    //            if (gridQueryModel.sidx == "Email" && gridQueryModel.sord == "desc")
        //    //                pageData = (from cust in query orderby cust.Email descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


        //    //        if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "asc")
        //    //            pageData = (from cust in query orderby cust.Status descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //        else
        //    //            if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "desc")
        //    //                pageData = (from cust in query orderby cust.Status ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
        //    //    }

        //    //    return Json(new
        //    //    {
        //    //        page = gridQueryModel.page,
        //    //        records = count,
        //    //        rows = pageData,
        //    //        total = Math.Ceiling((decimal)count / gridQueryModel.rows)
        //    //    }, JsonRequestBehavior.AllowGet);


        //    //}
        //    //catch
        //    //{
        //    //    return Json(gridQueryModel);
        //    //}
        //}



        #endregion

    }
}
