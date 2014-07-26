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
using System.Xml;


namespace Pfizer.Controllers
{
    public class TransactionsController : Controller
    {
        ExportPdf pdf = new ExportPdf();
        private pfizerEntities dcTransaction = new pfizerEntities();
        PfizerBL bLayer = new PfizerBL();
        string path = ConfigurationSettings.AppSettings["PDFfilePath"].ToString();

        /// <summary>
        /// GET: /Transactions/
        /// </summary>
        /// <returns></returns>
        #region Transactions IncludeCustomer
        

        public ActionResult IncludeCustomer()
        {
          
            return View();
        }

       // //Area Name 

        public JsonResult JsonGetBrick()
        {
            List<SelectListItem> UseName = new List<SelectListItem>();
             UseName.Clear();
            var item = (from a in dcTransaction.GetAreaDetails(Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString())) select new { a.PKID,a.AreaName }).ToList();
            if (item.Count() > 0)
            {
                UseName = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.AreaName.ToString(),
                    Value = x.PKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                countyItems = UseName,
                countyCount = UseName.Count,

            }, JsonRequestBehavior.AllowGet);

        }

        ////Specification 
        public string ReturnsXmlTeam(string Specialization)
        {

            string[] arrayUser = Specialization.Split(',');
            string xml = "<row>";
            for (int i = 0; i < arrayUser.Count(); i++)
            {
                xml += "<dp";
                xml += " CusCode='"+arrayUser[i]+"'";
               xml += "/>";
            }   
            xml += "</row>";
            return xml;
        }
       // //Grid Control Load

        public JsonResult GridIncludeCustomer(GridQueryModel gridQueryModel, string Brick, string Specialization)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                string xmlSpecification="";
                Int32 PSOFKID = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                Int32 Territory = Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());
                xmlSpecification = ReturnsXmlTeam(Specialization);
                DataTable dt = new DataTable();
                dt = bLayer.GetIncludeCustomer(Session["USER_FKID"].ToString(), Brick, Session["Territory_FKID"].ToString(), xmlSpecification);

                var result = dt.AsEnumerable().Select(dataRow => new GetIncludeCustomer
                {
                  Type=dataRow[0].ToString(),
                  pkid=dataRow[1].ToString(),
                  CustomerName=dataRow[2].ToString(),
                  Specification=dataRow[3].ToString(),
                  Kol=dataRow[4].ToString(),
                  AMPKID=dataRow[5].ToString(),
                  AreaName=dataRow[6].ToString(),
                  SMPKID=dataRow[7].ToString(),
                  }).ToList();
                var count = result.Count();
                var pageData = result.OrderBy(x => x.CustomerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                              
                
               

                if (gridQueryModel.sidx == "CustomerName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.CustomerName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "CustomerName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.CustomerName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.Specification ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.Specification descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                    pageData = (from cust in result orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                else if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                    pageData = (from cust in result orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                 
               
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
        ////Pastial view Load Specialization 
        public ActionResult PVLoadSpecialization()
        {

            decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            dynamic query = "";
            query = (from a in dcTransaction.usp_getSpecialization(psoFkid) select new { a.SpecializationID, a.SpecializationName }).ToDictionary(f => Convert.ToDecimal(f.SpecializationID), f => f.SpecializationName.ToString());

            return PartialView("PVSelectSpeci", query);
        }

        public JsonResult GetDoctorDetailsforinccust(GridQueryModel gridQueryModel, string MASPKID, string Type, string flag)
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

        #region Fetch Doctor Call History
        public JsonResult GetDoctorDetailsforIncCustGrid(GridQueryModel gridQueryModel, string MASPKID, string Type)
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


        [HttpPost]
        public ActionResult IncludeCustomerDetails(string gridData ,string CustomerType)
        {
           
            try
            {
                string msg = "";
                decimal PSOFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                Int32 Territory = Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());
                var serialize = new JavaScriptSerializer();
               var deserialize = serialize.Deserialize<List<IncludeCustomer>>(gridData);
                var str = "<row>";
                foreach (var item in deserialize)
                {
                    if ((item.chks.Equals("Yes") && item.Type.Equals("Doctor")))
                    {

                        str += "<dp ";
                        str += " CusFKID='" + item.pkid + "'";
                        str += " CusType='" + item.Type + "'";
                        str += " AreaFKID='" + item.AMPKID + "'";
                        str += " Speciality='" + SpecializationCode(item.PractSpeciality) + "'";
                        str += " KOL='" + item.Kol + "'";
                        str += "/>";
                    }
                    if ((item.chks.Equals("Yes") && !item.Type.Equals("Doctor")))
                    {
                        str += "<dp ";
                        str += " CusFKID='" + item.pkid + "'";
                        str += " CusType='" + item.Type + "'";
                        str += " AreaFKID='" + item.AMPKID + "'";
                        str += "/>";
                    }
                }
                str += "</row>";
                if (CustomerType.Equals("Doctor"))
                {
                    if (str != "<row></row>")
                    {
                        dcTransaction.IncludeDoctor(str, PSOFKID, Territory);
                        msg = "Customer Added Successfully.";
                    }
                    else
                    {
                        msg = "notselect";

                    }
                }

                if (!CustomerType.Equals("Doctor"))
                {
                    if (str != "<row></row>")
                    {
                        dcTransaction.IncludeOtherCustomer(str, PSOFKID, Territory);
                        msg = "Customer Added Successfully.";
                    }
                    else
                    {
                        msg = "notselect";

                    }
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

       private string SpecializationCode(string p)
       {
           var query = (from a in dcTransaction.USP_GetSpecializationCode(p) select new { aSpecializationID = a.SpecializationID.ToString() }).FirstOrDefault();
           return query.aSpecializationID;
       }
       #endregion

       #region rajKumar

       #region Daily Report

       public ActionResult DailyReport()
       {

           return View();

       }

       public JsonResult JsonGetSettingMaster(string nodeId, string nodeValue)
       {

           XmlDocument doc = new XmlDocument();
           doc.LoadXml(bLayer.DailyReportFromSettingMaster(Session["USER_FKID"].ToString()));

           
           XmlNodeList xnList = doc.SelectNodes("/Root/row");

           string sa = xnList.Item(1).ToString();

           
           XmlNodeList xnList1 = doc.SelectNodes("/Root/lockEntry");




           XmlElement root = doc.DocumentElement;
           //String genre = root.GetAttribute("DRDaysBeforeAllowed");
           //genre = root.GetAttribute("DREntryAllowedTime");
           //genre = root.GetAttribute("DRUnLockEntry");
           

           //// Check to see if the element has a genre attribute. 
           //if (root.HasAttribute("DRDaysBeforeAllowed"))
           //{
           //      genre = root.GetAttribute("DRDaysBeforeAllowed"); 
           //}
           //if (root.HasAttribute("DRDaysBeforeAllowed"))
           //{
           //      genre = root.GetAttribute("DRDaysBeforeAllowed");
           //}

         //  DataSet ds = bLayer.DailyReportFromSettingMaster(Session["USER_FKID"].ToString());
         
           return Json(new
           {
               teamItems = "",
               count = 0,

           }, JsonRequestBehavior.AllowGet);

       }

       #endregion

       #endregion

    }
}
