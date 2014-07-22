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

namespace Pfizer.Controllers
{
    public class ManageGeographyController : Controller
    {

        ExportPdf pdf = new ExportPdf();

        private pfizerEntities dcManageGeography = new pfizerEntities();
        string path =  System.Configuration.ConfigurationManager.AppSettings["PDFfilePath"].ToString();
        // GET: /dcManageGeography/ 



        //City Master

        public ActionResult CityMaster()
        {
            return View();
        }

        public JsonResult GridDataCityMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.CityMasters
                             select new
                             {
                                 PKID = a.PKID,
                                 CityName = a.CityName,
                                 HQFLAG = a.HQFLAG == true ? "Active" : "Inactive",
                                 IsActive = a.IsActive == true ? "Active" : "Inactive"

                             }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "CityName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.CityName!=null && sq.CityName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.CityName != null && sq.CityName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.CityName != null && sq.CityName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.CityName != null && sq.CityName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.CityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        public JsonResult AddCityMaster(string id, string oper, string CityName, string HQFLAG, string IsActive)
        {
            try
            {
                  var rslt = (from a in dcManageGeography.CityMasters where a.CityName == CityName select a).ToList();
                    if (rslt.Count() > 0)
                        id = "City Name already Exists";
                    else
                    {

                        CityMaster gm = new CityMaster();
                        gm.CityName = CityName.TrimStart();
                        gm.CityCode = "1";

                        if (HQFLAG == "Active")
                            gm.HQFLAG = true;

                        if (HQFLAG == "Passive")
                            gm.HQFLAG = true;

                        if (IsActive == "Passive")
                            gm.IsActive = false;
                        if (IsActive == "Active")
                            gm.IsActive = true;

                        gm.CreatedDate = DateTime.Now;
                        gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                        dcManageGeography.CityMasters.Add(gm);
                        dcManageGeography.SaveChanges();
                        id = "City Master added Successfully.";
                      
            }
                    return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
      
        }

        public JsonResult EditCityMaster(string id, string oper, string CityName, string HQFLAG, string IsActive)
        {
            try
            {

                if (oper == "edit")
                {

                    int ePKID = Convert.ToInt32(id);
                    var qry = (from a in dcManageGeography.CityMasters where a.PKID == ePKID select a).Single();

                    qry.CityName = CityName.TrimStart();

                    if (HQFLAG == "Active")
                        qry.HQFLAG = true;

                    if (HQFLAG == "Passive")
                        qry.HQFLAG = false;

                    if (IsActive == "Active")
                        qry.IsActive = true;

                    if (IsActive == "Passive")
                        qry.IsActive = false;

                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    dcManageGeography.SaveChanges();


                    id = "City Master modified Successfully.";

                }
                
                 
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageGeography.CityMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    dcManageGeography.SaveChanges();
                    id = "City Master deleted Successfully.";
                }

                return Json(id);
            }
            catch(Exception msg) {
                return Json(msg.Message);
            }
        }

        //Brick Master 

        public ActionResult BrickMaster()
        {
            return View();
        }


        public JsonResult GridDataBrickMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.USP_GET_BRICKMASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "AreaName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.AreaName!=null && sq.AreaName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.AreaName != null && sq.AreaName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.AreaName != null && sq.AreaName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase)  select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.AreaName != null && sq.AreaName.ToUpper().Contains(searchString)  select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();

                    if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.CityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public JsonResult AddBrickMaster(string id, string oper, string AreaName, string Specialflag, string CityName, string IsActive)
        {
            try
            {
                 var rslt = (from a in dcManageGeography.AreaMasters where a.AreaName == AreaName select a).ToList();
                 if (rslt.Count() > 0)
                     id = "Brick Name already Exists";
                 else
                 {


                     AreaMaster gm = new AreaMaster();
                     gm.AreaName = AreaName.TrimStart();
                     gm.CityFKID = Convert.ToInt32(CityName);
                     gm.AreaCode = "1";
                     if (Specialflag == "Active")
                         gm.Specialflag = true;

                     if (Specialflag == "Passive")
                         gm.Specialflag = false;

                     if (IsActive == "Active")
                         gm.IsActive = true;

                     if (IsActive == "Passive")
                         gm.IsActive = false;


                     gm.CreatedDate = DateTime.Now;
                     gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                     dcManageGeography.AreaMasters.Add(gm);
                     dcManageGeography.SaveChanges();

                     id = "Brick Master added Successfully";
                  
                 }
                 return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }

        }

        public JsonResult EditBrickMaster(string id, string oper, string AreaName, string Specialflag, string CityName, string IsActive)
        {
            try
            {

                if (oper == "edit")
                {

                    int ePKID = Convert.ToInt32(id);
                    var qry = (from a in dcManageGeography.AreaMasters where a.PKID == ePKID select a).Single();

                    qry.AreaName = AreaName.TrimStart();
                    qry.CityFKID = Convert.ToInt32(CityName);
                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    if (Specialflag == "Active")
                        qry.Specialflag = true;

                    if (Specialflag == "Passive")
                        qry.Specialflag = false;

                    if (IsActive == "Active")
                        qry.IsActive = true;

                    if (IsActive == "Passive")
                        qry.IsActive = false;

                    dcManageGeography.SaveChanges();

                    id = "Brick Master modified Successfully";

                }
               
                
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageGeography.AreaMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    dcManageGeography.SaveChanges();

                    id = "Brick Master deleted Successfully";
                }

                return Json(id);
            }
            catch (Exception msg){
                return Json(msg.Message);
            }
        }

        public ActionResult LoadPVRCityMaster()
        {
            var query = (from e in dcManageGeography.CityMasters where e.IsActive == true select new { e.PKID, e.CityName }).ToDictionary(f => f.PKID, f => f.CityName.ToString());
            return PartialView("PVLoadCityMaster", query);
        }

        //Division Master
        public ActionResult DivisionMaster()
        {
            return View();
        }

        public JsonResult GridDataDivisionMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.DivisionMasters
                             select new
                             {
                                 PKID = a.PKID,
                                 DivCode = a.DivCode,
                                 DivName = a.DivName,
                                 IsActive = a.IsActive == true ? "Active" : "Inactive"

                             }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.DivName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "DivName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.DivName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DivName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.DivName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DivName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.DivName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DivName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.DivName.Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DivName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "DivName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DivName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DivName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DivName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "DivCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DivCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DivCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DivCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public JsonResult EditDivisionMaster(string id, string oper, string DivName, string HQFLAG, string IsActive)
        {
            try
            {
                if (oper == "edit")
                {
                    int ePKID = Convert.ToInt32(id);
                    var qry = (from a in dcManageGeography.DivisionMasters where a.PKID == ePKID select a).Single();
                    qry.DivName = DivName.TrimStart();
                    if (IsActive == "Active")
                        qry.IsActive = true;
                    if (IsActive == "Passive")
                        qry.IsActive = false;
                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageGeography.SaveChanges();
                    id = "Division Master modified Successfully.";
                }
                //if (oper == "add")
                //{

                //    var rslt = (from a in dcManageGeography.DivisionMasters where a.DivName == DivName select a).ToList();
                //    if (rslt.Count() > 0)
                //        id = "Division Name already Exists";
                //    else
                //    {

                //        DivisionMaster gm = new DivisionMaster();
                //        gm.DivName = DivName.TrimStart();
                //        gm.DivCode = "1";

                //        if (IsActive == "Passive")
                //            gm.IsActive = false;
                //        if (IsActive == "Active")
                //            gm.IsActive = true;

                //        gm.CreatedDate = DateTime.Now;
                //        gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                //        dcManageGeography.DivisionMasters.Add(gm);
                //        dcManageGeography.SaveChanges();

                //        id = "Division Master added Successfully.";
                //    }
                //}
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageGeography.DivisionMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    dcManageGeography.SaveChanges();
                    id = "Division Master deleted Successfully.";
                }

                return Json(id);
            }
            catch(Exception msg)
            {
                return Json(msg.Message);
            }
        }


        public JsonResult AddDivisionMaster(string id, string DivName, string IsActive)
        {
            try
            {
                var rslt = (from a in dcManageGeography.DivisionMasters where a.DivName == DivName select a).ToList();
                if (rslt.Count() > 0)
                    id = "Division Name already Exists";
                else
                {

                    DivisionMaster gm = new DivisionMaster();
                    gm.DivName = DivName.TrimStart();
                    gm.DivCode = "1";

                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    if (IsActive == "Active")
                        gm.IsActive = true;

                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageGeography.DivisionMasters.Add(gm);
                    dcManageGeography.SaveChanges();

                    id = "Division Master added Successfully.";
                }
                return Json(id);
            }
            catch(Exception msg) {
                return Json(msg.Message);
            }
           
        }
        //Territory Master

        public ActionResult TerritoryMaster()
        {
            return View();
        }

        public JsonResult GridDataTerritoryMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper(); 
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.USP_GET_TERRITORY_MASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.TerCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "TerCode"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.TerCode!=null && sq.TerCode.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TerCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.TerCode!=null && sq.TerCode.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TerCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.TerCode != null && sq.TerCode.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TerCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.TerCode != null && sq.TerCode.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TerCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();

                    if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "TerName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "DivName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DivName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DivName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DivName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Townclassname" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Townclassname descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Townclassname" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Townclassname ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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


        public ActionResult AddTerritoryMaster(string oper, string LocalityName, string TerCode, string TerName, string DivName, string Townclassname, string id, string IsActive)
        {
            try {
                bool Isactive = false;
                decimal CreatedBy;
                
                var rslt = (from a in dcManageGeography.TerritoryMasters where a.TerCode == TerCode select a).ToList();
                if (rslt.Count() > 0)
                    id = "Territory Code already Exists";
                else
                {

                    if (IsActive == "Active")
                        Isactive = true;

                    CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    dcManageGeography.USP_UPDATE_TERRITORYMASTER(oper, LocalityName.TrimStart(), " ", DivName, TerCode, TerName, Townclassname, Isactive.ToString(), CreatedBy.ToString(), CreatedBy.ToString());

                    id = "Territory Master added Successfully.";
                }
                return Json(id);
            }

            catch(Exception msg)
            {
                return Json(msg.Message);
            }
        }

        public ActionResult EditTerritoryMaster(string oper, string LocalityName, string TerCode, string TerName, string DivName, string Townclassname, string id, string IsActive)
        {

            bool Isactive = false;
            decimal CreatedBy;
            decimal ModifiedBy;
            try
            {

                if (oper == "edit")
                {

                    if (IsActive == "Active")
                        Isactive = true;

                    ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    dcManageGeography.USP_UPDATE_TERRITORYMASTER(oper, LocalityName.TrimStart(), id, DivName, TerCode, TerName, Townclassname, Isactive.ToString(), ModifiedBy.ToString(), ModifiedBy.ToString());

                    id = "Territory Master modified Successfully.";

                }
                
                if (oper == "del")
                {

                    ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageGeography.DeleteTerritoryMaster(Convert.ToDecimal(id), ModifiedBy);
                    id = "City Master deleted Successfully.";
                }

                return Json(id);
            }

            catch
            {
                return Json(id);
            }
           
        }


        public ActionResult LocalityNameLocalityMaster(string data)
        {
            if (data == "null")
            {
                var query = (from a in dcManageGeography.USP_GET_LOCALITY_MASTER() select new { a.LocalityFKID, a.LocalityName }).ToDictionary(f => Convert.ToInt32(f.LocalityFKID), f => f.LocalityName.ToString());
                return PartialView("PVGetTerritoryDivision", query);
            }
            else
            {
                var query = (from a in dcManageGeography.USP_GET_LOCALITYMASTER(Convert.ToInt32(data)) select new { a.NODENAME, a.NODETYPEFKID }).ToDictionary(f => Convert.ToInt32(f.NODETYPEFKID), f => f.NODENAME.ToString());

                if (query.Count == 0)
                {
                    query = (from a in dcManageGeography.USP_GET_LOCALITY_MASTER() select new { a.LocalityFKID, a.LocalityName }).ToDictionary(f => Convert.ToInt32(f.LocalityFKID), f => f.LocalityName.ToString());
                }

                return PartialView("NodeTypeMaster", query);
            }
        }

        public ActionResult LoadDivisionMaster()
        {
            var query = (from e in dcManageGeography.DivisionMasters where e.IsActive == true && e.DivName != "" select new { e.PKID, e.DivName }).ToDictionary(f => f.PKID, f => f.DivName.ToString());
            return PartialView("PVLoadCityMaster", query);
        }
        public ActionResult LoadTownClassMaster()
        {
            var query = (from e in dcManageGeography.TownClassMasters where e.IsActive == true select new { e.PKID, e.Townclassname }).ToDictionary(f => f.PKID, f => f.Townclassname.ToString());
            return PartialView("PVLoadCityMaster", query);
        }


        //Region Master

        public ActionResult RegionMaster()
        {
            return View();
        }

        public JsonResult GridDataRegionMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.RegionMasters
                             select new
                             {
                                 PKID = a.PKID,
                                 RegionCode = a.RegionCode,
                                 RegionName = a.RegionName,
                                 IsActive = a.IsActive == true ? "Active" : "Inactive"

                             }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "RegionCode") )
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.RegionCode!=null && sq.RegionCode.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.RegionCode != null && sq.RegionCode.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.RegionCode != null && sq.RegionCode.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.RegionCode != null && sq.RegionCode.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "RegionCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RegionCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "RegionCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.RegionCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "RegionName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RegionName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "RegionName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.RegionName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public JsonResult AddRegionMaster(string id, string oper, string RegionCode, string RegionName, string IsActive)
        {
            try {
                var rslt = (from a in dcManageGeography.RegionMasters where a.RegionName == RegionName select a).ToList();
                if (rslt.Count() > 0)
                    id = "Region Name already Exists";
                else
                {

                    RegionMaster gm = new RegionMaster();
                    gm.RegionCode = RegionCode.TrimStart();
                    gm.RegionName = RegionName.TrimStart();

                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    if (IsActive == "Active")
                        gm.IsActive = true;

                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageGeography.RegionMasters.Add(gm);
                    dcManageGeography.SaveChanges();
                    id = "Region Master added Successfully.";
                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }


        public JsonResult EditRegionMaster(string id, string oper, string RegionCode, string RegionName, string IsActive)
        {
            try
            {

                if (oper == "edit")
                {

                    int ePKID = Convert.ToInt32(id);
                    var qry = (from a in dcManageGeography.RegionMasters where a.PKID == ePKID select a).Single();
                    qry.RegionCode = RegionCode.TrimStart();
                    qry.RegionName = RegionName.TrimStart();

                    if (IsActive == "Active")
                        qry.IsActive = true;

                    if (IsActive == "Passive")
                        qry.IsActive = false;

                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    dcManageGeography.SaveChanges();

                    id = "Region Master modified Successfully.";

                }
                
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageGeography.RegionMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    dcManageGeography.SaveChanges();
                    id = "Region Master deleted Successfully.";
                }

                return Json(id);
            }
            catch (Exception msg){
                return Json(msg.Message);
            }
        }


        //DistrictMaster

        public ActionResult DistrictMaster()
        {
            return View();
        }


        public JsonResult GridDataDistrictMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.DistrictMasters
                             select new
                             {
                                 PKID = a.PKID,
                                 DistrictCode = a.DistrictCode,
                                 DistrictName = a.DistrictName,
                                 IsActive = a.IsActive == true ? "Active" : "In Active"

                             }).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "DistrictCode"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.DistrictCode!=null && sq.DistrictCode.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.DistrictCode != null && sq.DistrictCode.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.DistrictCode != null && sq.DistrictCode.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.DistrictCode != null && sq.DistrictCode.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "DistrictCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DistrictCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DistrictCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DistrictCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "DistrictName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DistrictName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DistrictName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DistrictName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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


        public JsonResult AddDistrictMaster(string id, string oper, string DistrictCode, string DistrictName, string IsActive)
        {
            try {
                var rslt = (from a in dcManageGeography.DistrictMasters where a.DistrictName == DistrictName select a).ToList();
                if (rslt.Count() > 0)
                    id = "District Name already Exists";
                else
                {

                    DistrictMaster gm = new DistrictMaster();
                    gm.DistrictCode = DistrictCode.TrimStart();
                    gm.DistrictName = DistrictName.TrimStart();

                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    if (IsActive == "Active")
                        gm.IsActive = true;

                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    dcManageGeography.DistrictMasters.Add(gm);
                    dcManageGeography.SaveChanges();
                    id = "District Master added Successfully.";
                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }

        }

        public JsonResult EditDistrictMaster(string id, string oper, string DistrictCode, string DistrictName, string IsActive)
        {
            try
            {
                if (oper == "edit")
                {

                    int ePKID = Convert.ToInt32(id);
                    var qry = (from a in dcManageGeography.DistrictMasters where a.PKID == ePKID select a).Single();
                    qry.DistrictCode = DistrictCode.TrimStart();
                    qry.DistrictName = DistrictName.TrimStart();

                    if (IsActive == "Active")
                        qry.IsActive = true;

                    if (IsActive == "Passive")
                        qry.IsActive = false;

                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    dcManageGeography.SaveChanges();

                    id = "District Master modified Successfully.";

                }
                

                   
                
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in dcManageGeography.DistrictMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    dcManageGeography.SaveChanges();
                    id = "District Master deleted Successfully.";
                }

                return Json(id);
            }
            catch(Exception msg){
                return Json(msg.Message);
            }
        }



        //City Master export excel,pdf,csv

        public ActionResult ExportCityMasterToExcel()
        {
            var context = (from a in dcManageGeography.CityMasters
                           select new
                           {
                                
                               CityName = a.CityName == null ? "" : a.CityName,
                               HQFLAG = a.HQFLAG == true ? "Active" : "In Active",
                               IsActive = a.IsActive == true ? "Active" : "In Active"

                           }).OrderBy(o=>o.CityName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
               
                item.CityName == null ? "" :item.CityName,                
                item.IsActive == null ? "" :item.IsActive
               
            }));
            return new ExcelResult(HeadersQuestions, ColunmTypesQuestions, data, "CityMaster.xlsx", "CityMaster");
        }
        private static readonly string[] HeadersQuestions = {
                 "City Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesQuestions = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String
            };
        public ActionResult ExportCityMasterToPDF()
        {
            List<CityMasterTypes> userList = new List<CityMasterTypes>();
            var context = (from a in dcManageGeography.CityMasters
                           select new
                           {
                               CityName =a.CityName == null ? "" : a.CityName,
                               IsActive = a.IsActive == true ? "Active" : "In Active"

                           }).OrderBy(o => o.CityName).ToList();

            foreach (var item in context)
            {
                CityMasterTypes user = new CityMasterTypes();
                user.CityName = item.CityName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
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
                           e.CityName,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "CityName", "Status" }, path);
            return File(path, "application/pdf", "CityMaster.pdf");
        }
        public ActionResult ExportCityMasterToCsv()
        {
            var model = (from a in dcManageGeography.CityMasters
                         select new
                         {
                             CityName = a.CityName== null ? "" : a.CityName,
                            
                             IsActive = a.IsActive == true ? "Active" : "In Active"

                         }).OrderBy(o => o.CityName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("City Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record.CityName, record.IsActive == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "CityMaster.txt");
        }
        public class CityMasterTypes
        {
            public string CityName { get; set; }
            public string Status { get; set; }
        }


        //DivisionMaster export excel,pdf,csv

        public ActionResult ExportDivisionMasterToExcel()
        {
            var context = (from a in dcManageGeography.DivisionMasters
                           select new
                           {
                               DivCode = a.DivCode == null ? "" : a.DivCode,
                               DivName = a.DivName == null ? "" : a.DivName,
                               IsActive = a.IsActive == true ? "Active" : "Inactive"

                           }).OrderBy(x => x.DivName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {                
                item.DivCode,                
                item.DivName,
                item.IsActive== null ? "" : item.IsActive,
               
            }));
            return new ExcelResult(HeadersDivisionmaster, ColunmTypesDivisionMaster, data, "DivisionMaster.xlsx", "Divisionmaster");
        }
        private static readonly string[] HeadersDivisionmaster = {
                 "Division Code","Division Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesDivisionMaster = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportDivisionMasterToPDF()
        {
            List<DivisionMasterTypes> userList = new List<DivisionMasterTypes>();
            var context = (from a in dcManageGeography.DivisionMasters
                           select new
                           {
                               
                               DivCode = a.DivCode == null ? "" : a.DivCode,
                               DivName = a.DivName == null ? "" : a.DivName,
                               IsActive = a.IsActive == true ? "Active" : "Inactive"
                           }).OrderBy(x => x.DivName).ToList();

            foreach (var item in context)
            {
                DivisionMasterTypes user = new DivisionMasterTypes();
                user.DivisionCode = item.DivCode;
                user.DivisionName = item.DivName;
                user.Status =  (item.IsActive == null) ? "" : item.IsActive;//item.IsActive;
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
                            e.DivisionCode,e.DivisionName,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "DivisionCode", "DivisionName", "Status" }, path);
            return File(path, "application/pdf", "DivisionMaster.pdf");
        }
        public ActionResult ExportDivisionMasterToCsv()
        {
            var model = (from a in dcManageGeography.DivisionMasters
                         select new
                         {

                             DivCode = a.DivCode == null ? "" : a.DivCode,
                             DivName = a.DivName == null ? "" : a.DivName,
                             IsActive = a.IsActive == true ? "Active" : "Inactive"

                         }).OrderBy(x => x.DivName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Division Code,Division Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.DivCode, record.DivName, record.IsActive== null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "DivisionMaster.txt");
        }
        public class DivisionMasterTypes
        {

            public string DivisionCode { get; set; }
            public string DivisionName { get; set; }
            public string Status { get; set; }
        }


        //BrickMaster export excel,pdf,csv

        public ActionResult ExportBrickMasterToExcel()
        {
            var context = (from a in dcManageGeography.USP_GET_BRICKMASTER() select a).OrderBy(o => o.CityName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {                
                item.AreaName==null?"":item.AreaName,                
                item.CityName==null?"":item.CityName,                
                item.IsActive==null?"":item.IsActive,                               
            }));
            return new ExcelResult(HeadersBrickMaster, ColunmTypesBrickMaster, data, "BrickMaster.xlsx", "BrickMaster");
        }
        private static readonly string[] HeadersBrickMaster = {
                 "Brick Name","City","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesBrickMaster = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,  
                DataForExcel.DataType.String
            };
        public ActionResult ExportBrickMasterToPDF()
        {
            List<BrickMasterTypes> userList = new List<BrickMasterTypes>();
            var context = (from a in dcManageGeography.USP_GET_BRICKMASTER() select a).OrderBy(x=>x.CityName).ToList();

            foreach (var item in context)
            {
                BrickMasterTypes user = new BrickMasterTypes();
                user.BrickName = (item.AreaName==null?"":item.AreaName);
                user.City = (item.CityName == null ? "" : item.CityName);  
                user.Status = (item.IsActive == null ? "" : item.IsActive); 
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
                           e.BrickName,e.City,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "BrickName", "City", "Status" }, path);
            return File(path, "application/pdf", "BrickMaster.pdf");
        }
        public ActionResult ExportBrickMasterToCsv()
        {
            var model = (from a in dcManageGeography.USP_GET_BRICKMASTER() select a).OrderBy(x => x.CityName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("BrickName,City,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.AreaName == null ? "" : record.AreaName, record.CityName == null ? "" : record.CityName, record.IsActive == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "BrickMaster.txt");
        }
        public class BrickMasterTypes
        {
            public string BrickName { get; set; }
            public string City { get; set; }
            public string Status { get; set; }
        }
         

        //TerritoryMaster export excel,pdf,csv

        public ActionResult ExportTerritoryMasterToExcel()
        {
            var context = (from a in dcManageGeography.USP_GET_TERRITORY_MASTER() select a).OrderBy(x => x.TerCode).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {                
                item.TerCode==null?"":item.TerCode,                
                item.TerName==null?"":item.TerName,
                item.DivName==null?"":item.DivName,
                item.Townclassname==null?"":item.Townclassname,
                item.IsActive==null?"":item.IsActive
               
            }));
            return new ExcelResult(HeadersTerritoryMaster, ColunmTypesTerritoryMaster, data, "TerritoryMaster.xlsx", "CityMaster");
        }
        private static readonly string[] HeadersTerritoryMaster = {
                "Territory Code","Territory Name","Division","Town Class","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesTerritoryMaster = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String 
            };

        public ActionResult ExportTerritoryMasterToPDF()
        {
            List<TerritoryMasterTypes> userList = new List<TerritoryMasterTypes>();
            var context = (from a in dcManageGeography.USP_GET_TERRITORY_MASTER() select a).OrderBy(x=>x.TerCode).ToList();

            foreach (var item in context)
            {
                TerritoryMasterTypes user = new TerritoryMasterTypes();
                user.TerritoryCode = (item.TerCode==null?"":item.TerCode);
                user.TerritoryName = (item.TerName==null?"":item.TerName);
                user.Division = (item.DivName==null?"":item.DivName);
                user.TownClass = (item.Townclassname==null?"":item.Townclassname);
                user.Status = (item.IsActive==null?"":item.IsActive);
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
                           e.TerritoryCode,e.TerritoryName,e.Division,e.TownClass,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "TerritoryCode", "TerritoryName", "Division", "TownClass", "Status" }, path);
            return File(path, "application/pdf", "TerritoryMaster.pdf");
        }
        public ActionResult ExportTerritoryMasterToCsv()
        {
            var model = (from a in dcManageGeography.USP_GET_TERRITORY_MASTER() select a).OrderBy(x => x.TerCode).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Territory Code,Territory Name,Division,Town Class,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4}", record.TerCode == null ? "" : record.TerCode, record.TerName == null ? "" : record.TerName, record.DivName == null ? "" : record.DivName, record.Townclassname == null ? "" : record.Townclassname, record.IsActive == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "TerritoryMaster.txt");
        }
        public class TerritoryMasterTypes
        {
            public string TerritoryCode { get; set; }
            public string TerritoryName { get; set; }
            public string Division { get; set; }
            public string TownClass { get; set; }
            public string Status { get; set; }
        } 

        //RegionMaster export excel,pdf,csv

        public ActionResult ExportRegionMasterToExcel()
        {
            var context = (from a in dcManageGeography.RegionMasters
                           select new
                           {
                               RegionCode = a.RegionCode == null ? "" : a.RegionCode,
                               RegionName = a.RegionName == null ? "" : a.RegionName,
                               IsActive = a.IsActive == true ? "Active" : "Inactive"

                           }).OrderBy(o=>o.RegionCode).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                
                item.RegionCode,                
                item.RegionName,
                item.IsActive == null ? "" : item.IsActive,
               
            }));
            return new ExcelResult(HeadersRegionMaster, ColunmTypesRegionMaster, data, "RegionMaster.xlsx", "CityMaster");
        }
        private static readonly string[] HeadersRegionMaster = {
                 "Region code","Region Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesRegionMaster = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String
            };
        public ActionResult ExportRegionMasterToPDF()
        {
            List<RegionMasterTypes> userList = new List<RegionMasterTypes>();
            var context = (from a in dcManageGeography.RegionMasters
                           select new
                           {
                                
                               RegionCode = (a.RegionCode==null)?"":a.RegionCode,
                               RegionName = a.RegionName==null?"":a.RegionName,
                               IsActive = a.IsActive == true ? "Active" : "Inactive"

                           }).OrderBy(o => o.RegionCode).ToList();

            foreach (var item in context)
            {
                RegionMasterTypes user = new RegionMasterTypes();
                user.RegionCode = item.RegionCode;
                user.RegionName = item.RegionName;
                user.Status = (item.IsActive == null ? "" : item.IsActive);
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
                           e.RegionCode,e.RegionName,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "RegionCode", "RegionName", "Status" }, path);
            return File(path, "application/pdf", "RegionMaster.pdf");
        }
        public ActionResult ExportRegionMasterToCsv()
        {
            var model = (from a in dcManageGeography.RegionMasters
                         select new
                         {
                             RegionCode = (a.RegionCode == null) ? "" : a.RegionCode,
                             RegionName = a.RegionName == null ? "" : a.RegionName,
                             IsActive = a.IsActive == true ? "Active" : "Inactive"

                         }).OrderBy(o => o.RegionCode).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Region Code,Region Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.RegionCode, record.RegionName, record.IsActive==null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "RegionMaster.txt");
        }
        public class RegionMasterTypes
        {
            public string RegionCode { get; set; }
            public string RegionName { get; set; }
            public string Status { get; set; }
        }


        //DistrictMaster export excel,pdf,csv

        public ActionResult ExportDistrictMasterToExcel()
        {
            var context = (from a in dcManageGeography.DistrictMasters
                           select new
                           {
                               
                               DistrictCode = a.DistrictCode==null?"" :a.DistrictCode,
                               DistrictName = a.DistrictName == null ? "" : a.DistrictName,
                               IsActive = a.IsActive == true ? "Active" : "In Active"

                           }).OrderBy(o=>o.DistrictCode).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                
                item.DistrictCode,                
                item.DistrictName,
                item.IsActive == null ? "" : item.IsActive,
               
            }));
            return new ExcelResult(HeadersDistrictMaster, ColunmTypesDistrictMaster, data, "DistrictMaster.xlsx", "CityMaster");
        }
        private static readonly string[] HeadersDistrictMaster = {
                 "District code","District Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesDistrictMaster = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String
            };
        public ActionResult ExportDistrictMasterToPDF()
        {
            List<DistrictMasterTypes> userList = new List<DistrictMasterTypes>();
            var context = (from a in dcManageGeography.DistrictMasters
                           select new
                           {
                              
                               DistrictCode = a.DistrictCode == null ? "" : a.DistrictCode,
                               DistrictName = a.DistrictName == null ? "" : a.DistrictName,
                               IsActive = a.IsActive == true ? "Active" : "In Active"

                           }).OrderBy(o => o.DistrictCode).ToList();

            foreach (var item in context)
            {
                DistrictMasterTypes user = new DistrictMasterTypes();
                user.DistrictCode = item.DistrictCode;
                user.DistrictName = item.DistrictName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
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
                           e.DistrictCode,e.DistrictName,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "DistrictCode", "DistrictName", "Status" }, path);
            return File(path, "application/pdf", "DistrictMaster.pdf");
        }
        public ActionResult ExportDistrictMasterToCsv()
        {
            var model = (from a in dcManageGeography.DistrictMasters
                         select new
                         {
                             
                             DistrictCode = a.DistrictCode == null ? "" : a.DistrictCode,
                             DistrictName = a.DistrictName == null ? "" : a.DistrictName,
                             IsActive = a.IsActive == true ? "Active" : "In Active"

                         }).OrderBy(o => o.DistrictCode).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("District Code,District Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.DistrictCode, record.DistrictName, record.IsActive == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "DistrictMaster.txt");
        }
        public class DistrictMasterTypes
        {
            public string DistrictCode { get; set; }
            public string DistrictName { get; set; }
            public string Status { get; set; }
        }


        //TerritoryHQLink 

        public ActionResult TerritoryHQLink()
        {
            return View();
        }


        public JsonResult GridDataTerritoryHQLink(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.USP_GET_TerritoryHQLink() select a).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "CityName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.CityName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.CityName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.CityName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.CityName.Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.CityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "CityName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.CityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "TerName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        public JsonResult AddTerritoryHQLink(string id, string oper,  string SelTerritory, string CityName)
        {

            try
            {
                string str = "";
                decimal StrcreatedBy;
                string StrHQName = "";
                StrcreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                StrHQName = CityName;

                str = "<root>";
                string StrSelectedSelTerritoryCode = SelTerritory;
                string[] SelectedTerritoryCodevalues = StrSelectedSelTerritoryCode.Split(',');
                for (int i = 0; i < SelectedTerritoryCodevalues.Length; i++)
                {
                    str += "<CompBrand";
                    str += " TerritoryFKID ='" + SelectedTerritoryCodevalues[i] + "'";
                    str += "/>";
                }
                str += "</root>";
                dcManageGeography.AddTerritoryHQLinkMaster(Convert.ToDecimal(StrHQName), str, Convert.ToInt32(StrcreatedBy));
                id = "Territory HQ Link Master Added Successfully.";


                return Json(id);
            }
            catch(Exception ex) {
                return Json(ex.Message);
            }
        }

        public JsonResult EditTerritoryHQLink(string id, string oper, string HQFKID, string TerritoryFKID, string SelTerritory, string CityName, string IsActive)
        {

            string str = "";
            decimal StrcreatedBy;
            decimal StrModifiedBy;
            string StrHQFKID = "";
            bool eIsactive = true;
            string StrTerritoryFKID = "";
            string StrHQName = "";

            if (oper == "edit")
            {
                StrcreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                StrModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                StrHQFKID = HQFKID;
                StrTerritoryFKID = TerritoryFKID;
                str = "<root>";
                string StrSelectedSelTerritoryCode = SelTerritory;
                string[] SelectedTerritoryCodevalues = StrSelectedSelTerritoryCode.Split(',');
                for (int i = 0; i < SelectedTerritoryCodevalues.Length; i++)
                {
                    str += "<CompBrand";
                    str += " TerritoryFKID ='" + SelectedTerritoryCodevalues[i] + "'";
                    str += "/>";
                }
                str += "</root>";
                dcManageGeography.EditTerritoryHQLinkMaster(Convert.ToDecimal(StrHQFKID), Convert.ToInt32(eIsactive), Convert.ToInt32(StrcreatedBy), str, Convert.ToInt32(StrModifiedBy));
                id = "Territory HQ Link Master modified Successfully.";
            }
            
            if (oper == "del")
            {

                if (Session["HQFKID"] != null)
                    StrHQFKID = Session["HQFKID"].ToString();
                if (Session["TerritoryFKID"] != null)
                    StrTerritoryFKID = Session["TerritoryFKID"].ToString();
                if (StrHQFKID != "" && StrTerritoryFKID != "")

                    dcManageGeography.DeleteTerritoryHQLinkMaster(Convert.ToDecimal(StrHQFKID), Convert.ToDecimal(StrTerritoryFKID));
                id = "Territory HQ Link Master deleted Successfully.";
            }

            return Json(id);
        }


        //TerritoryHQLink Delete Position Get TeamFKID,RegionFKID.

        public JsonResult GetHQFKID(string HQFKID, string TerritoryFKID)
        {

            Session["HQFKID"] = HQFKID;
            Session["TerritoryFKID"] = TerritoryFKID;
            return Json(new
            {
                areaItems = "",
                areaCount = "",
            }, JsonRequestBehavior.AllowGet);
        }


        //TerritoryHQLink Partial View

        public ActionResult NotSelectedTerritoryCode(string Type, string HQFKID, string TerritoryFKID)
        {
            ViewBag.Type = Type;
            dynamic query = "";
            if (HQFKID == "undefined")
            {
                //HQFKID = "30137";
                //TerritoryFKID = "1412";
                //query = (from a in dcManageGeography.GetTerritoryNameforHQEditByXml(HQFKID, TerritoryFKID) where a.TerritoryName != "" select new { a.PKID, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerritoryName.ToString());

                query = (from a in dcManageGeography.GetAllTeamRegionLinkByXml("0") where 1 == 2 select new { a.RegionFKID, a.RegionName }).ToDictionary(f => Convert.ToInt32(f.RegionFKID), f => f.RegionName.ToString());
                return PartialView("PVSelectedDDL", query);
            }
            else
            {

                ViewBag.Type = Type;
                query = (from a in dcManageGeography.GetTerritoryNameforHQEditByXml(HQFKID, TerritoryFKID) where a.TerritoryName != "" select new { a.PKID, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerritoryName.ToString());
                return PartialView("PVSelectedDDL", query);
            }


        }

        public ActionResult SelectedTerritoryCode(string Type, string HQFKID, string TerritoryFKID)
        {
            ViewBag.Type = Type;
            dynamic query = "";
            if (HQFKID == "undefined")
            {
                query = (from a in dcManageGeography.GetAllTeamRegionLinkByXml("0") where 1 == 2 select new { a.RegionFKID, a.RegionName }).ToDictionary(f => Convert.ToInt32(f.RegionFKID), f => f.RegionName.ToString());
                return PartialView("PVSelectedDDL", query);
            }
            else if (Type == "viewSelectedTerCode")
            {

                query = (from a in dcManageGeography.GetTerritoryNameforHQTerByXml(HQFKID, TerritoryFKID) select new { a.PKID, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerritoryName.ToString());
                return PartialView("PVSelectedDDL", query);
            }
            else
            {

                query = (from a in dcManageGeography.GetTerritoryNameforHQTerByXml(HQFKID, TerritoryFKID) select new { a.PKID, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerritoryName.ToString());
                return PartialView("PVSelectedDDL", query);
            }
        }

        public ActionResult LoadHQName(string Type, string data)
        {

            if (data == "undefined")
            {
                ViewBag.Type = Type;
                var query = (from a in dcManageGeography.GetHQNameByXML() where a.HQName != "" select new { a.HQFKID, a.HQName }).ToDictionary(f => Convert.ToInt32(f.HQFKID), f => f.HQName.ToString());
                return PartialView("PVSelectedDDL", query);
            }
            else
            {
                ViewBag.Type = Type;
                var query = (from a in dcManageGeography.GetHQNameForTerByXML(data.ToString()) select new { a.HQFKID, a.HQName }).ToDictionary(f => Convert.ToInt32(f.HQFKID), f => f.HQName.ToString());
                return PartialView("PVSelectedDDL", query);
            }

        }

        public ActionResult NotSelectedNewTerritoryCode(string Type, string HQNameFKID)
        {
            dynamic query = "";

            ViewBag.Type = Type;
            query = (from a in dcManageGeography.GetTerritoryNameforHQByXml(HQNameFKID) where a.TerritoryName != "" select new { a.PKID, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerritoryName.ToString());
            return PartialView("PVSelectedDDL", query);

        }

        //TerritoryHQLink export excel,pdf,csv

        public ActionResult ExportTerritoryHQLinkToExcel()
        {
            var context = (from a in dcManageGeography.USP_GET_TerritoryHQLink() select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
             
                item.CityName== null ? "" : item.CityName,                
                item.TerCode== null ? "" : item.TerCode,
                item.TerName== null ? "" : item.TerName,
                item.IsActive== null ? "" : item.IsActive
               
            }));
            return new ExcelResult(HeadersTerritoryHQLink, ColunmTypesTerritoryHQLink, data, "TerritoryHQLink.xlsx", "TerritoryHQLink");
        }
        private static readonly string[] HeadersTerritoryHQLink = {
                 "HQ Name","Territory Code","Territor Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesTerritoryHQLink = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                 DataForExcel.DataType.String, 
                DataForExcel.DataType.String
            };
        public ActionResult ExportTerritoryHQLinkToPDF()
        {
            List<TerritoryHQLinkTypes> userList = new List<TerritoryHQLinkTypes>();
            var context = (from a in dcManageGeography.USP_GET_TerritoryHQLink() select a).OrderBy(o=>o.CityName).ToList();

            foreach (var item in context)
            {
                TerritoryHQLinkTypes user = new TerritoryHQLinkTypes();
                user.CityName = item.CityName == null ? "" : item.CityName;
                user.TerCode = item.TerCode == null ? "" : item.TerCode;
                user.TerName = item.TerName == null ? "" : item.TerName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
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
                           e.CityName,e.TerCode,e.TerName,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "CityName", "TerCode", "TerName", "Status" }, path);
            return File(path, "application/pdf", "TerritoryHQLinkMaster.pdf");
        }
        public ActionResult ExportTerritoryHQLinkToCsv()
        {
            var model = (from a in dcManageGeography.USP_GET_TerritoryHQLink() select a).OrderBy(o => o.CityName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("HQ Name,Territory Code,Territory Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3}", record.CityName == null ? "" : record.CityName, record.TerCode == null ? "" : record.TerCode, record.TerName == null ? "" : record.TerName, record.IsActive == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "TerritoryHQLinkMaster.txt");
        }
        public class TerritoryHQLinkTypes
        {
            public string CityName { get; set; }
            public string TerCode { get; set; }
            public string TerName { get; set; }
            public string Status { get; set; }
        }


        //TeamRegionLink

        public ActionResult TeamRegionLink()
        {
            return View();
        }

        public JsonResult GridDataTeamRegionLink(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.USP_GET_TeamRegionLink() select a).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "TeamName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.TeamName!=null && sq.TeamName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.TeamName != null && sq.TeamName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.TeamName != null && sq.TeamName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.TeamName != null && sq.TeamName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();

                    if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TeamName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TeamName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "RegionCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RegionCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "RegionCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.RegionCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "RegionName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.RegionName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "RegionName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.RegionName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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
        public JsonResult AddTeamRegionLink(string id, string oper, string TeamFKID, string RegionFKID, string SelectedRegionName, string TeamName, string IsActive)
        {
            try {
                string str = "";
                string StrTeamName = "";
                StrTeamName = TeamName.TrimStart();
                str = "<root>";
                string StrSelectedRegionName = SelectedRegionName;
                string[] SelectedRegionNamevalues = StrSelectedRegionName.Split(',');
                for (int i = 0; i < SelectedRegionNamevalues.Length; i++)
                {
                    str += "<CompBrand";
                    str += " RegionFKID ='" + SelectedRegionNamevalues[i] + "'";
                    str += "/>";
                }
                str += "</root>";
                dcManageGeography.AddTeamRegionLinkMaster(Convert.ToDecimal(StrTeamName), str, Convert.ToInt32(Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString())));
                id = "Team Region Link Master Added Successfully.";
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }


        public JsonResult EditTeamRegionLink(string id, string oper, string TeamFKID, string RegionFKID, string SelectedRegionName, string TeamName, string IsActive)
        {
            string str = "";
            decimal StrcreatedBy;
            decimal StrModifiedBy;
            string StrTeamFKID = "";
            bool eIsactive = true;
            string StrRegionFKID = "";

            string StrTeamName = "";
            try
            {

                if (oper == "edit")
                {

                    StrcreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    StrModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    StrTeamFKID = TeamFKID;

                    str = "<root>";
                    string StrSelectedRegionName = SelectedRegionName;
                    string[] SelectedRegionNamevalues = StrSelectedRegionName.Split(',');
                    for (int i = 0; i < SelectedRegionNamevalues.Length; i++)
                    {

                        str += "<CompBrand";
                        str += " RegionFKID ='" + SelectedRegionNamevalues[i] + "'";
                        str += "/>";
                    }
                    str += "</root>";

                    //str = "<root><CompBrand RegionFKID ='2'/><CompBrand RegionFKID ='7'/></root>";

                    dcManageGeography.EditTeamRegionLinkMaster(Convert.ToDecimal(StrTeamFKID), Convert.ToInt32(eIsactive), Convert.ToInt32(StrcreatedBy), str, Convert.ToInt32(StrModifiedBy));

                    id = "Team Region Link Master modified Successfully.";

                }
                
                if (oper == "del")
                {
                    if (Session["TeamFkid"] != null)
                        StrTeamFKID = Session["TeamFkid"].ToString();
                    if (Session["RegionFkid"] != null)
                        StrRegionFKID = Session["RegionFkid"].ToString();
                    if (StrTeamFKID != "" && StrRegionFKID != "")
                        dcManageGeography.DeleteTeamRegionLinkMaster(Convert.ToDecimal(StrTeamFKID), Convert.ToDecimal(StrRegionFKID));
                    id = "Team Region Link Master deleted Successfully.";
                }

                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg.Message);
            }
        }


        //TeamRegionLink Delete Position Get TeamFKID,RegionFKID.

        public JsonResult GetTeamFKID(string TeamFkid, string RegionFkid)
        {

            Session["TeamFkid"] = TeamFkid;
            Session["RegionFkid"] = RegionFkid;
            return Json(new
            {
                areaItems = "",
                areaCount = "",
            }, JsonRequestBehavior.AllowGet);
        }

        //TeamRegionLink Partial View

        public ActionResult NotSelectedRegionName(string Type, string data)
        {
            ViewBag.Type = Type;
            var query = (from a in dcManageGeography.GetAllRegionNotAssignedTeamByXml() where a.RegionName != "" && a.RegionName!=null select new { a.RegionId, a.RegionName }).ToDictionary(f => Convert.ToInt32(f.RegionId), f => f.RegionName.ToString());
            return PartialView("PVSelectedDDL", query);
        }

        public ActionResult SelectedRegionName(string Type, string data)
        {
            ViewBag.Type = Type;
            dynamic query = "";
            if (data == "undefined")
            {
                query = (from a in dcManageGeography.GetAllTeamRegionLinkByXml("0") where 1 == 2 select new { a.RegionFKID, a.RegionName }).ToDictionary(f => Convert.ToInt32(f.RegionFKID), f => f.RegionName.ToString());
                return PartialView("PVSelectedDDL", query);
            }
            else
            {

                query = (from a in dcManageGeography.GetAllTeamRegionLinkByXml(data) select new { a.RegionFKID, a.RegionName }).ToDictionary(f => Convert.ToInt32(f.RegionFKID), f => f.RegionName.ToString());
                return PartialView("PVSelectedDDL", query);
            }
        }

        public ActionResult LoadTeamMasterName(string Type, string data)
        {

            if (data == "undefined")
            {
                ViewBag.Type = Type;
                var query = (from a in dcManageGeography.GetTeamMasterByXML() orderby a.OptimaTeamId select new { a.OptimaTeamId, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.OptimaTeamId), f => f.TeamName.ToString());
                return PartialView("PVSelectedDDL", query);
            }
            else
            {
                ViewBag.Type = Type;
                var query = (from a in dcManageGeography.GetTeamMasteByPKIDByXML(Convert.ToInt32(data)) select new { a.OptimaTeamId, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.OptimaTeamId), f => f.TeamName.ToString());
                return PartialView("PVSelectedDDL", query);
            }

        }

        //TeamRegionLink export excel,pdf,csv

        public ActionResult ExportTeamRegionLinkToExcel()
        {
            var context = (from a in dcManageGeography.USP_GET_TeamRegionLink() select a).OrderBy(o=>o.TeamName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
             
                item.TeamName==null?"":item.TeamName,                
                item.RegionCode==null?"":item.RegionCode,                
                item.RegionName==null?"":item.RegionName,                
                item.IsActive==null?"":item.IsActive               
               
            }));
            return new ExcelResult(HeadersTeamRegionLink, ColunmTypesTeamRegionLink, data, "TeamRegionLink.xlsx", "TeamRegionLink");
        }
        private static readonly string[] HeadersTeamRegionLink = {
                 "Team Name","Region Code"," Region Name", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesTeamRegionLink = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String
            };
        public ActionResult ExportTeamRegionLinkToPDF()
        {
            List<TeamRegionLinkType> userList = new List<TeamRegionLinkType>();
            var context = (from a in dcManageGeography.USP_GET_TeamRegionLink() select new { a.TeamName, a.RegionCode, a.RegionName,a.IsActive }).OrderBy(o => o.TeamName).ToList();

            foreach (var item in context)
            {
                TeamRegionLinkType user = new TeamRegionLinkType();
                user.TeamName =(item.TeamName == null) ? "" : item.TeamName;
                user.RegionCode =( item.RegionCode == null) ? "" : item.RegionCode;
                user.RegionName =( item.RegionName == null) ? "" : item.RegionName;
                user.Status = (item.IsActive == null) ? "" : item.IsActive;
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
                           e.TeamName,e.RegionCode,e.RegionName,e.Status 

                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "TeamName", "RegionCode", "RegionName", "Status" }, path);
            return File(path, "application/pdf", "TeamRegionLink.pdf");
        }
        public ActionResult ExportTeamRegionLinkToCsv()
        {
            var model = ((from a in dcManageGeography.USP_GET_TeamRegionLink() select a).OrderBy(o => o.TeamName).ToList());
            var sb = new StringBuilder();
            sb.AppendLine("Team Name,Region Code,Region Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3}", record.TeamName == null ? "" : record.TeamName, record.RegionCode == null ? "" : record.RegionCode, record.RegionName == null ? "" : record.RegionName, record.IsActive == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "TeamRegionLink.txt");
        }
        public class TeamRegionLinkType
        {
            public string TeamName { get; set; }
            public string RegionCode { get; set; }
            public string RegionName { get; set; }
            public string Status { get; set; }
        }

        #region District Territory Link
        public ActionResult DistrictTerritoryLink()
        {
            return View();
        }
        public JsonResult GetDistrictTerritoryLink(GridQueryModel gridQueryModel)
        {
            try
            {
                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in dcManageGeography.GetDistrictTerritoryLink() select q).ToList(); 
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "DistrictCode"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.DistrictCode!=null && sq.DistrictCode.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.DistrictCode != null && sq.DistrictCode.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.DistrictCode != null && sq.DistrictCode.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.DistrictCode != null && sq.DistrictCode.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DistrictCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();

                    if (gridQueryModel.sidx == "DistrictCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DistrictCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DistrictCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DistrictCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "DistrictName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DistrictName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "DistrictName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DistrictName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "TerName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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

        #region Partial View For District Territory Link

        public ActionResult GetTerritoryDistrictCode()
        {

            var Query = (from a in dcManageGeography.GetDistrictCodeByXml() where a.DistCode != "" select new { a.DistrictId, a.DistCode }).ToDictionary(f => Convert.ToInt32(f.DistrictId), f => f.DistCode.ToString());
            return PartialView("PVDistrictTerritoryLink", Query);

        }

        public ActionResult GetDistrictTerritoryCode(string DistrictFKID, string Type)
        {
            ViewBag.Type = Type;
            var Query = (from a in dcManageGeography.GetTerritoryNameExcludeforDistrictByXml(DistrictFKID) where a.TerritoryName != "" select new { a.PKID, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerritoryName.ToString());
            return PartialView("PVDistrictTerritoryLink", Query);

        }



        public ActionResult SelectedTerCode(string type, string data)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (data == "undefined")
            {
                query = (from a in dcManageGeography.GetTerritoryNameforDistrictByXml("0") where 1 == 2 select new { a.PKID, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerritoryName.ToString());
                return PartialView("PVDistrictTerritoryLink", query);
            }
            else
            {
                query = (from a in dcManageGeography.GetTerritoryNameforDistrictByXml(data) select new { a.PKID, a.TerritoryName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerritoryName.ToString());
                return PartialView("PVDistrictTerritoryLink", query);

            }
        }


        public JsonResult AddDistrictTerritoryLink(string id, string oper, string DistrictFKID, string DistrictCode, string SelTerCode, string IsActive)
        {
            string str = "";
            string msg = string.Empty;
            try
            { 
                str = "<row>";
                string StrSelectedSelTerCode = SelTerCode;
                string[] SelectedSelTerCodevalues = StrSelectedSelTerCode.Split(',');
                for (int i = 0; i < SelectedSelTerCodevalues.Length; i++)
                {
                    str += "<dp";
                    str += " TerritoryFKID ='" + SelectedSelTerCodevalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";


                int result = dcManageGeography.AddDistrictTerritorylinkMaster(Convert.ToDecimal(DistrictCode), str, true, Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));

                if (result >= 1)
                {
                    msg = "District Territory Links Added Successfully.";
                }

                return Json(msg);
            }
            catch (Exception ex) {
                return Json(ex.Message);
            }
        }

        #endregion

        public JsonResult EditDistrictTerritoryLink(string id, string oper, string DistrictFKID, string DistrictCode, string SelTerCode, string IsActive)
        {
            try
            {
                string str = "";
                string msg = string.Empty;


                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                if (oper == "edit")
                {


                    str = "<row>";
                    string StrSelectedSelTerCode = SelTerCode;
                    string[] SelectedSelTerCodevalues = StrSelectedSelTerCode.Split(',');
                    for (int i = 0; i < SelectedSelTerCodevalues.Length; i++)
                    {
                        str += "<dp";
                        str += " TerritoryFKID ='" + SelectedSelTerCodevalues[i] + "'";
                        str += "/>";
                    }
                    str += "</row>";


                    int result = dcManageGeography.AddDistrictTerritorylinkMaster(Convert.ToDecimal(DistrictCode), str, true, Convert.ToDecimal(ModifiedBy));

                    if (result >= 1)
                    {
                        msg = "District Territory Links  Updated Successfully.";
                    }


                }
               
                if (oper == "del")
                {

                    int result = dcManageGeography.DeleteDistrictTerritoryLinkMaster(Convert.ToDecimal(id));
                    if (result == 1)
                    {
                        msg = "District Territory Links Deleted Successfully.";
                    }

                }

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }

        //TerritoryBrick Excel,PDF and CSV
         
        public ActionResult ExportTerritoryBrickLinkToExcel()
        {
            try
            {
                var context = (from q in dcManageGeography.usp_GetTerritory() where q.TerCode != null && q.TerName != null select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[]{
                    item.TerCode.ToString(),
                    item.TerName.ToString(),
                    item.AreaName.ToString()

                }));
                return new ExcelResult(HeadersTerritoryBrickLink, ColunmTypesTerritoryBrickLink, data, "TerritoryBrickLink.xlsx", "TerritoryBrickLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }


        }
        private static readonly string[] HeadersTerritoryBrickLink = {
                "TerCode", "TerName", "AreaName"};
        private static readonly DataForExcel.DataType[] ColunmTypesTerritoryBrickLink = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
            };

        public ActionResult ExportTerritoryBrickLinkToPDF()
        {
            try
            {
                List<TerritoryBrickLink> userList = new List<TerritoryBrickLink>();
                var context = (from q in dcManageGeography.usp_GetTerritory()  select q).ToList();

                foreach (var item in context)
                {
                    TerritoryBrickLink user = new TerritoryBrickLink();
                    user.PKID = item.PKID.ToString();
                    user.TerCode = item.TerCode.ToString();
                    user.TerName = item.TerName.ToString();
                    user.AreaName = item.AreaName.ToString();
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
                            e.PKID.ToString(),e.TerCode,e.TerName,e.AreaName
                        }
                            })
                    ).ToArray()
                };
                pdf.ExportPDF(customerList, new string[] { "TerCode", "TerName", "AreaName" }, path);
                return File(path, "application/pdf", "TerritoryBrickLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }
        public class TerritoryBrickLink
        {
            public string PKID { get; set; }
            public string TerCode { get; set; }
            public string TerName { get; set; }
            public string AreaName { get; set; }
        }



        public ActionResult ExportTerritoryBrickLinkToCsv()
        {
            try
            {
                var model = (from q in dcManageGeography.usp_GetTerritory() where q.TerCode != null && q.TerName != null select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Ter Code, Ter Name, Area Name");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2}", record.TerCode, record.TerName, record.AreaName);
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "TerritoryBrickLink.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        //DistrictTerritory Excel,PDF and CSV

        public ActionResult ExportDistrictTerritoryLinkToExcel()
        {
            try
            {

                var context = (from q in dcManageGeography.GetDistrictTerritoryLink() select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.DistrictCode.ToString()==null?"":item.DistrictCode.ToString(),
                item.DistrictName.ToString()==null?"":item.DistrictName.ToString(),
                item.TerCode==null?"":item.TerCode.ToString(),
                item.TerName==null?"":item.TerName.ToString() 
               
            }));
                return new ExcelResult(HeadersDistrictTerritoryLink, ColunmTypesDistrictTerritoryLink, data, "DistrictTerritoryLink.xlsx", "DistrictTerritoryLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        private static readonly string[] HeadersDistrictTerritoryLink = {
                "DistrictCode", "DistrictName", "TerritoryCode", "TerritoryName"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesDistrictTerritoryLink = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
               

            };
        public ActionResult ExportDistrictTerritoryLinkToPDF()
        {
            try
            {
                List<DistTerritoryLink> userList = new List<DistTerritoryLink>();
                var context = (from q in dcManageGeography.GetDistrictTerritoryLink()   select q).ToList();

                foreach (var item in context)
                {
                    DistTerritoryLink user = new DistTerritoryLink();
                    user.PKID = (item.PKID.ToString()==null?"":item.PKID.ToString());
                    user.DistrictCode = item.DistrictCode.ToString() == null ? "" : item.DistrictCode.ToString();
                    user.DistrictName = item.DistrictName.ToString() == null ? "" : item.DistrictName;
                    user.TerritoryCode = item.TerCode == null ? "" : item.TerCode;
                    user.TerritoryName = item.TerName == null ? "" : item.TerName;
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
                           e.DistrictCode,e.DistrictName,e.TerritoryCode,e.TerritoryName
                        }
                            })
                    ).ToArray()
                };
                
                pdf.ExportPDF(customerList, new string[] { "DistrictCode", "DistrictName", "TerritoryCode", "TerritoryName" }, path);
                return File(path, "application/pdf", "DistrictTerritoryLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class DistTerritoryLink
        {
            public string PKID { get; set; }
            public string DistrictCode { get; set; }
            public string DistrictName { get; set; }
            public string TerritoryCode { get; set; }
            public string TerritoryName { get; set; }

        }


        public ActionResult ExportDistrictTerritoryLinkToCsv()
        {
            try
            {
                var model = (from q in dcManageGeography.GetDistrictTerritoryLink()  select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("District Code,District Name,Territory Code,Territory Name");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record.DistrictCode == null ? "" : record.DistrictCode, record.DistrictName == null ? "" : record.DistrictName, record.TerCode == null ? "" : record.TerCode, record.TerName == null ? "" : record.TerName
                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "DistrictTerritoryLink.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult GetProduct()
        {
            try
            {
                var query = (from e in dcManageGeography.GetProductforProductMatrixByXml() select new { e.PKID, e.ProductName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.ProductName.ToString());

                return PartialView("ProductMolecule", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult MoleculeName()
        {
            try
            {
                var query = (from e in dcManageGeography.GetMoleculeforProductByXML() select new { e.MoleculeID, e.MoleculeName }).ToDictionary(f => Convert.ToInt32(f.MoleculeID), f => f.MoleculeName.ToString());

                return PartialView("ProductMolecule", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }
        #endregion
        


        #region Territory Brick Link

        public ActionResult TerritoryBrickLinkMaster()
        {
            return View();
        }

        public JsonResult GridTerritoryBrickLinkMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in dcManageGeography.usp_GetTerritory() select a).ToList();

                var count = query.Count();
                var pageData = query.OrderBy(x => x.TerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "TerName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.TerName != null && sq.TerName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.TerName!=null && sq.TerName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.TerName != null && sq.TerName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.TerName != null && sq.TerName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TerName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "TerName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TerCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TerCode" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TerCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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


        public JsonResult AddTerritoryBrickLinkMaster(string id, string oper, string TerCode, string TerritoryFKID, string SelAreaName, string AreaFKID, string IsActive, string val)
        {
            string msg = "";
            string str = "";
            try {
               
                if (IsActive == "True")
                {
                    IsActive = "1";
                }
                else
                {
                    IsActive = "0";
                }

                str = "<root>";
                string StrSelectedSelAreaName = SelAreaName;
                string[] SelectedAreaNamevalues = StrSelectedSelAreaName.Split(',');
                for (int i = 0; i < SelectedAreaNamevalues.Length; i++)
                {
                    str += "<CompBrand";
                    str += " AreaFKID ='" + SelectedAreaNamevalues[i] + "'";
                    str += "/>";
                }
                str += "</root>";


                int result = dcManageGeography.AddTerritoryAreaLinkMaster(Convert.ToDecimal(TerritoryFKID), Convert.ToInt32(IsActive), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()), str);

                if (result >= 1)
                {
                    msg = "Territory Brick Links Added Successfully.";
                }
                return Json(msg);
            }
            catch (Exception ex)
            { return Json("Territory Brick Links already Exists"); }


        }

        #region Edit Territory Brick Link Master

        public JsonResult EditTerritoryBrickLinkMaster(string id, string oper, string TerCode, string TerritoryFKID, string SelAreaName, string AreaFKID, string IsActive, string val)
        {

            try
            {
                string str = "";
                string msg = string.Empty;
                if (IsActive == "Active")
                {
                    IsActive = "1";
                }
                else
                {
                    IsActive = "0";
                }


                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                if (oper == "edit")
                {


                    str = "<root>";
                    string StrSelectedSelAreaName = SelAreaName;
                    string[] SelectedAreaNamevalues = StrSelectedSelAreaName.Split(',');
                    for (int i = 0; i < SelectedAreaNamevalues.Length; i++)
                    {
                        str += "<CompBrand";
                        str += " AreaFKID ='" + SelectedAreaNamevalues[i] + "'";
                        str += "/>";
                    }
                    str += "</root>";


                    int result = dcManageGeography.EditTerritoryAreaLinkMaster(Convert.ToDecimal(TerritoryFKID), Convert.ToInt32(IsActive), Convert.ToInt32(ModifiedBy), str, Convert.ToInt32(ModifiedBy));

                    if (result >= 1)
                    {
                        msg = "Territory Brick Links  Updated Successfully.";
                    }


                }
                
                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }


        #region Delete Territory Brick Links Master

        public JsonResult DeleteTerritoryBrickLinkMaster(string TerritoryFKID, string AreaFKID)
        {
            var msg = "";
            try
            {
                dcManageGeography.DeleteTerritoryAreaLinkMaster(Convert.ToDecimal(TerritoryFKID), Convert.ToDecimal(AreaFKID));
                msg = "Territory Brick Links Deleted Successfully.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(msg);
        }

        #endregion

        #endregion

        #region Create Partial View For territory Bricks links Master

        public ActionResult Getselcharactorcityname(string AreaFKID, string type)
        {
            ViewBag.Type = type;
            if (AreaFKID == "undefined" || AreaFKID == null)
            {
                var Query = (from a in dcManageGeography.GetCityFCforTerritoryByXML() select new { a.CityName }).ToDictionary(f => f.CityName, f => f.CityName.ToString());
                return PartialView("PVTerBrickLinkChaCityName", Query);
            }
            else
            {
                var Query = (from a in dcManageGeography.GetCityNameForAreaXML(Convert.ToDecimal(AreaFKID)) select new { a.CityFKID, a.CityName }).ToDictionary(f => Convert.ToInt32(f.CityFKID), f => f.CityName.ToString());
                return PartialView("PVTerritoryBrickLinks", Query);
            }


        }

        public ActionResult Getselectedcityname(string AreaFKID, string type, string TerritoryFKID)
        {
            ViewBag.Type = type;
            if (AreaFKID == "undefined" || AreaFKID == null || TerritoryFKID == "undefined" || TerritoryFKID == null)
            {
                var Query = (from a in dcManageGeography.GetCityFCforTerritoryByXML() where 1 == 2 select new { a.CityName }).ToDictionary(f => f.CityName, f => f.CityName.ToString());
                return PartialView("PVTerBrickLinkChaCityName", Query);
            }
            else
            {
                var Query = (from a in dcManageGeography.GetCityNameforTerritoryByXML(Convert.ToDecimal(TerritoryFKID), Convert.ToDecimal(AreaFKID)) select new { a.CityFKID, a.CityName }).ToDictionary(f => Convert.ToInt32(f.CityFKID), f => f.CityName.ToString());
                return PartialView("PVTerritoryBrickLinks", Query);
            }


        }

        public ActionResult SelectedTerCodeBricks(string type, string TerritoryFKID)
        {
            ViewBag.Type = type;
            if (TerritoryFKID == "undefined" || TerritoryFKID == null)
            {
                var Query = (from a in dcManageGeography.GetCityFCforTerritoryByXML() where 1 == 2 select new { a.CityName }).ToDictionary(f => f.CityName, f => f.CityName.ToString());
                return PartialView("PVTerBrickLinkChaCityName", Query);
            }
            else
            {
                var Query = (from a in dcManageGeography.GetTerritoryforBrickXml(Convert.ToDecimal(TerritoryFKID)) select new { a.PKID, a.TerCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerCode.ToString());
                return PartialView("PVTerritoryBrickLinks", Query);
            }


        }

        public ActionResult SelectedBricksName(string AreaFKID, string type, string TerritoryFKID)
        {
            ViewBag.Type = type;
            if (AreaFKID == "undefined" || AreaFKID == null || TerritoryFKID == "undefined" || TerritoryFKID == null)
            {
                var Query = (from a in dcManageGeography.GetCityFCforTerritoryByXML() where 1 == 2 select new { a.CityName }).ToDictionary(f => f.CityName, f => f.CityName.ToString());
                return PartialView("PVTerBrickLinkChaCityName", Query);
            }
            else
            {
                var Query = (from a in dcManageGeography.GetBrickforTerritoryByXml(Convert.ToDecimal(TerritoryFKID), Convert.ToDecimal(AreaFKID)) select new { a.PKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
                return PartialView("PVTerritoryBrickLinks", Query);
            }


        }


        public ActionResult SelectedBricksNameForcity(string CityFKID, string type, string TerritoryFKID)
        {
            ViewBag.Type = type;
            if (CityFKID == "undefined" || CityFKID == null || TerritoryFKID == "undefined" || TerritoryFKID == null)
            {
                var Query = (from a in dcManageGeography.GetCityFCforTerritoryByXML() where 1 == 2 select new { a.CityName }).ToDictionary(f => f.CityName, f => f.CityName.ToString());
                return PartialView("PVTerBrickLinkChaCityName", Query);
            }
            else
            {
                var Query = (from a in dcManageGeography.GetBrickNameforCityByXml(CityFKID, TerritoryFKID) select new { a.PKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
                return PartialView("PVTerritoryBrickLinks", Query);
            }


        }

        public ActionResult GetselFCcityname(string Type, string FCcityname)
        {
            ViewBag.Type = Type;
            var Query = (from a in dcManageGeography.GetCityforTerritoryByXML(FCcityname) select new { a.CityFKID, a.CityName }).ToDictionary(f => Convert.ToInt32(f.CityFKID), f => f.CityName.ToString());
            return PartialView("PVTerritoryBrickLinks", Query);


        }

        public ActionResult SelectedUserName(string Type)
        {
            ViewBag.Type = Type;
            dynamic query = "";
            query = (from a in dcManageGeography.GetCityforTerritoryByXML("0") where 1 == 2 select new { a.CityFKID, a.CityName }).ToDictionary(f => Convert.ToInt32(f.CityFKID), f => f.CityName.ToString());
            return PartialView("PVTerritoryBrickLinks", query);
        }


        public ActionResult GetselcharactorTerricode(string type, string TerritoryFKID)
        {

            ViewBag.Type = type;
            if (TerritoryFKID == "undefined" || TerritoryFKID == null)
            {
                var Query = (from a in dcManageGeography.GetTCodeFCforTerritoryByXML() select new { a.PKID, a.TerCode }).ToDictionary(f => f.TerCode, f => f.TerCode.ToString());
                return PartialView("PVTerBrickLinkChaCityName", Query);
            }
            else
            {
                var Query = (from a in dcManageGeography.GetTerritoryforBrickXml(Convert.ToDecimal(TerritoryFKID)) select new { a.PKID, a.FCTerCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.FCTerCode.ToString());
                return PartialView("PVTerritoryBrickLinks", Query);
            }

        }



        public ActionResult GetselcityBricks(string CityFKID, string TerritoryFKID, string Type)
        {
            ViewBag.Type = Type;

            if (TerritoryFKID == "undefined")
            {
                TerritoryFKID = "";
            }

            var Query = (from a in dcManageGeography.GetBrickNameforCityByXml(CityFKID, TerritoryFKID) select new { a.PKID, a.AreaName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.AreaName.ToString());
            return PartialView("PVTerritoryBrickLinks", Query);
        }


        public ActionResult GetselFCTcode(string FCTcode, string Type)
        {



            ViewBag.Type = Type;
            var Query = (from a in dcManageGeography.GetTCodeforTerritoryByXML(FCTcode) select new { a.PKID, a.TerCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TerCode.ToString());
            return PartialView("PVTerritoryBrickLinks", Query);
        }




        #endregion

        #region Create Excel,CSV,PDF Files

        #region Convert Excel

        public ActionResult ExportTerritoryBricksLinkToExcel()
        {
            try
            {
                var context = (from a in dcManageGeography.usp_GetTerritory() select a).OrderBy(o=>o.TerName).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.TerCode==null?"":item.TerCode,
                item.TerName==null?"":item.TerName,
                item.AreaName==null?"":item.AreaName,
                item.IsActive==null?"":item.IsActive,
               
              
            }));
                return new ExcelResult(HeadersTerritoryBricksLink, ColunmTypesTerritoryBricksLink, data, "TerritoryBricksLink.xlsx", "TerritoryBricksLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersTerritoryBricksLink = {
                "TerritoryName, TerritoryCode, Brick"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesTerritoryBricksLink = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                 };

        #endregion

        #region Covert PDF

        public ActionResult ExportTerritoryBricksLinkToPDF()
        {
            try
            {
                List<TerritoryBricksLinkMaster> userList = new List<TerritoryBricksLinkMaster>();
                var context = (from a in dcManageGeography.usp_GetTerritory()  select a).OrderBy(x=>x.TerName).ToList();

                foreach (var item in context)
                {
                    TerritoryBricksLinkMaster user = new TerritoryBricksLinkMaster();
                    user.PKID = (item.PKID.ToString()==null?"":item.PKID.ToString());
                    user.TerritoryCode = (item.TerCode == null ? "" : item.TerCode);
                    user.TerritoryName = (item.TerName==null?"" : item.TerName);
                    user.Brick = (item.AreaName==null?"" : item.AreaName);
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
                          e.TerritoryName,e.TerritoryCode,e.Brick
                        }
                            })
                    ).ToArray()
                };
                pdf.ExportPDF(customerList, new string[] {"TerritoryCode", "TerritoryName", "Brick" }, path);
                return File(path, "application/pdf", "TerritoryBricksLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        #endregion

        #region PDF Class FIle

        public class TerritoryBricksLinkMaster
        {
            public string PKID { get; set; }
            public string TerritoryCode { get; set; }
            public string TerritoryName { get; set; }
            public string Brick { get; set; }
            //public string Status { get; set; }

        }
        #endregion

        #region Convert To CSV

        public ActionResult ExportTerritoryBricksLinkToCsv()
        {
            try
            {
                var model = (from a in dcManageGeography.usp_GetTerritory() select a).OrderBy(x=>x.TerName).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("TerritoryName,TerritoryCode,Brick,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record.TerCode == null ? "" : record.TerCode, record.TerName == null ? "" : record.TerName, record.AreaName == null ? "" : record.AreaName, record.IsActive == null ? "" : record.IsActive
                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "TerritoryBricksLink.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        #endregion

        #endregion

        #endregion

        #region Region District Link
        public ActionResult RegionDistrictLink()
        {
            return View();
        }

        public JsonResult GetRegionDistrictLink(GridQueryModel gridQueryModel)
        {
            try
            {


                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in dcManageGeography.GetRegionDistrictLink() select q).ToList();
                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "RegionCode") || (searchField == "RegionName") || (searchField == "DistrictCode") || (searchField == "DistrictName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.RegionCode.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.RegionName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.DistrictName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.DistrictCode.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.RegionCode.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.RegionName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.DistrictName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.DistrictCode.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.RegionCode.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.RegionName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.DistrictName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.DistrictCode.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.RegionCode.Contains(searchString) || sq.DistrictName.Contains(searchString) || sq.RegionName.Contains(searchString) || sq.DistrictName.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RegionCode).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "RegionName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DistrictName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "RegionName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DistrictName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        public JsonResult AddRegionDistrictLink(string id, string oper, string IsActive, string DistrictFKID, string DistrictName, string RegionCode, string SelDistrictCode)
        {
            string str = "";
            string msg = string.Empty;
            if (IsActive == "Active")
            {
                IsActive = "True";
            }
            else
            {
                IsActive = "True";
            }
            try
            { 
                str = "<row>";
                string StrSelectedSelDistrictCode = SelDistrictCode;
                string[] SelectedDistrictCodevalues = StrSelectedSelDistrictCode.Split(',');
                for (int i = 0; i < SelectedDistrictCodevalues.Length; i++)
                {
                    str += "<dp";
                    str += " DFKID ='" + SelectedDistrictCodevalues[i] + "'";
                    str += "/>";
                }
                str += "</row>";


                //  var Result = string.Empty;
                int result = dcManageGeography.AddRegionDistrictLinkMaster(Convert.ToDecimal(RegionCode), str, Convert.ToBoolean(IsActive),Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                if (result >= 1)
                {
                    msg = "Region District Link  Added Successfully.";
                }
                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        public JsonResult EditRegionDistrictLink(string id, string oper, string IsActive, string DistrictFKID, string DistrictName, string RegionCode, string SelDistrictCode)
        {
            try
            {
                string str = "";
                string msg = string.Empty;
               


                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
 

                if (oper == "edit")
                {

                    str = "<row>";
                    string StrSelectedSelDistrictCode = SelDistrictCode;
                    string[] SelectedDistrictCodevalues = StrSelectedSelDistrictCode.Split(',');
                    for (int i = 0; i < SelectedDistrictCodevalues.Length; i++)
                    {
                        str += "<dp";
                        str += " DFKID ='" + SelectedDistrictCodevalues[i] + "'";
                        str += "/>";
                    }
                    str += "</row>";


                    int result = dcManageGeography.AddRegionDistrictLinkMaster(Convert.ToDecimal(RegionCode), str, Convert.ToBoolean(IsActive), ModifiedBy);

                    if (result >= 1)
                    {
                        msg = "Region District Link  Updated Successfully.";
                    }

                }
                
                if (oper == "del")
                {
                    int result = dcManageGeography.DeleteRegionDistrictLinkMaster(Convert.ToDecimal(id));
                    if (result == 1)
                    {
                        msg = "Region District Link Deleted Successfully.";
                    }

                }

                return Json(msg);
                // return Json(new { success = false, showMessage = true, message = msg, title = "Error" });
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }



        public ActionResult ExportRegionDistrictLinkToExcel()
        {
            try
            {
                var context = (from q in dcManageGeography.GetRegionDistrictLink() select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.RegionCode.ToString(),
                item.RegionName.ToString(),
                item.DistrictCode.ToString(),
                item.DistrictName.ToString()
              
            }));
                return new ExcelResult(HeadersDistrictTerritoryLink, ColunmTypesDistrictTerritoryLink, data, "RegionDistrictLink.xlsx", "RegionDistrictLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersRegionDistrictLink = {
                "RegionCode","RegionName","DistrictCode", "DistrictName"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesRegionDistrictLink = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                 };
        public ActionResult ExportRegionDistrictLinkToPDF()
        {
            try
            {
                List<RegionDistrictLinkMaster> userList = new List<RegionDistrictLinkMaster>();
                var context = (from q in dcManageGeography.GetRegionDistrictLink() select q).ToList();

                foreach (var item in context)
                {
                    RegionDistrictLinkMaster user = new RegionDistrictLinkMaster();
                    user.PKID = item.PKID.ToString() == null ? "" : item.PKID.ToString();
                    user.RegionCode = item.RegionCode == null ? "" : item.RegionCode;
                    user.RegionName = item.RegionName == null ? "" : item.RegionName;
                    user.DistrictCode = item.DistrictCode.ToString() == null ? "" : item.DistrictCode.ToString();
                    user.DistrictName = item.DistrictName.ToString() == null ? "" : item.DistrictName.ToString();

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
                            e.PKID.ToString(),e.RegionCode,e.RegionName,e.DistrictCode,e.DistrictName
                        }
                            })
                    ).ToArray()
                };
                //var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Sample1.pdf");
                //pdf.ExportPDF(customerList, new string[] {"RegionCode", "RegionName", "DistrictCode", "DistrictName" }, path);
                //return File(path, "application/pdf", "RegionDistrictLink.pdf");
                pdf.ExportPDF(customerList, new string[] { "RegionCode", "RegionName", "DistrictCode", "DistrictName" }, path);
                return File(path, "application/pdf", "RegionDistrictLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class RegionDistrictLinkMaster
        {
            public string PKID { get; set; }
            public string RegionCode { get; set; }
            public string RegionName { get; set; }
            public string DistrictCode { get; set; }
            public string DistrictName { get; set; }

        }


        public ActionResult ExportRegionDistrictLinkToCsv()
        {
            try
            {
                var model = (from q in dcManageGeography.GetRegionDistrictLink() select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Region Code,Region Name,District Code,District Name");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3}", record.RegionCode, record.RegionName, record.DistrictCode, record.DistrictName
                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "RegionDistrictLink.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult GetRegionCodeByXml(string type)
        {
            ViewBag.Type = type;
            try
            {
                var query = (from e in dcManageGeography.GetRegionCodeByXml() where e.RegionCode != "" && e.RegionCode !=null select new { RegionId = (e.RegionId == null ? 0 : e.RegionId), RegionCode = (e.RegionCode == null ? "" : e.RegionCode) }).ToDictionary(f => Convert.ToInt32(f.RegionId), f => f.RegionCode.ToString());

                return PartialView("PVRegionDistrictLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult GetDistrictNameforTerittoryByXml(string type, string RegionFKID)
        {
            ViewBag.Type = type;
            try
            {
                var query = (from e in dcManageGeography.GetDistrictNameExcludeforTerittoryByXml(RegionFKID) where e.DistrictCode != "" select new { e.PKID, e.DistrictCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DistrictCode.ToString());
                return PartialView("PVRegionDistrictLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }


        public ActionResult SelectedDistrictName(string Type, string RegionFKID)
        {
            ViewBag.Type = Type;
            dynamic query = "";

            if (RegionFKID == null || RegionFKID == "undefined")
            {
                query = (from a in dcManageGeography.GetDistrictNameExcludeforTerittoryByXml("0") where 1 == 2 select new { a.PKID, a.DistrictCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DistrictCode.ToString());
                return PartialView("PVRegionDistrictLink", query);
            }
            else
            {
                query = (from a in dcManageGeography.GetDistrictNameforTerittoryByXml(RegionFKID) select new { a.PKID, a.DistrictCode }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.DistrictCode.ToString());
                return PartialView("PVRegionDistrictLink", query);
            }


        }
        #endregion

    }



}

