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


namespace Pfizer.Controllers
{
    public class AdminSettingController : Controller
    {
        private pfizerEntities genData = new pfizerEntities();
        ExportPdf pdf = new ExportPdf();
        int Startyr = Convert.ToInt32(ConfigurationManager.AppSettings["StartYear"].ToString());        
        string path = ConfigurationManager.AppSettings["PDFfilePath"].ToString();


        public JsonResult GridDataCycleSettingMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GET_CYCLE_SETTING_MASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.Month).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "Year") )
                    {
                        if (searchOper == "bw")//begins with
                        {
                            //var q = (from sq in query where sq.CityName!=null && sq.CityName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)select sq).ToList();
                            var q = (from sq in query where sq.Year.ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.Year.ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.Year.ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Status.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.Year.ToString().Contains(searchString) || sq.Status.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "Year" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Year ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Year" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Year descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "NoOfCycles" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.NoOfCycles ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "NoOfCycles" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.NoOfCycles descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Month" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Month ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Month" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Month descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Status ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Status" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Status descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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
        public ActionResult AddCycleSettingMaster(string id, string oper, string NoOfCycles, string Month, string Year, string CycleStartYear, string Status)
        {
            try
            {
                
                    int aYear = Convert.ToInt32(Year);
                    var rslt = (from a in genData.CycleDefinitionMasters where a.Year == aYear select a).ToList();
                    if (rslt.Count() > 0)
                        id = "CycleSettingMaster already Exists";
                    else
                    {

                        CycleDefinitionMaster gm = new CycleDefinitionMaster();
                        gm.Year = Convert.ToInt32(Year);
                        gm.NoOfCycles = Convert.ToInt32(NoOfCycles);
                        gm.CycleStartYear = Convert.ToInt32(CycleStartYear);
                        gm.CycleStartMonth = Convert.ToInt32(Month);
                        // if (Status == "Active")
                        gm.IsActive = true;
                        gm.CreatedDate = DateTime.Now;
                        gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                        genData.CycleDefinitionMasters.Add(gm);
                        genData.SaveChanges();

                        id = "CycleSettingMaster added successfully";
                    }
                
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(new { error = msg.Message });
            }
        }
        



        public JsonResult EditCycleSettingMaster(string id, string oper, string NoOfCycles, string Month, string Year, string CycleStartYear, string Status)
        {
            try
            {
                if (oper == "edit")
                {
                    int ePKID = Convert.ToInt32(id);
                    var qry = (from a in genData.CycleDefinitionMasters where a.PKID == ePKID select a).Single();

                    qry.NoOfCycles = Convert.ToInt32(NoOfCycles);
                    qry.Year = Convert.ToInt32(Year);
                    qry.CycleStartYear = Convert.ToInt32(CycleStartYear);
                    qry.CycleStartMonth = Convert.ToInt32(Month);
                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    if (Status == "Active")
                        qry.IsActive = true;

                    if (Status == "Passive")
                        qry.IsActive = false;

                    genData.SaveChanges();

                    id = "CycleSettingMaster modified Successfully.";
                }
                //if (oper == "add")
                //{
                //    int aYear = Convert.ToInt32(Year);
                //    var rslt = (from a in genData.CycleDefinitionMasters where a.Year == aYear select a).ToList();
                //    if (rslt.Count() > 0)
                //        id = "CycleSettingMaster already Exists";
                //    else
                //    {

                //        CycleDefinitionMaster gm = new CycleDefinitionMaster();
                //        gm.Year = Convert.ToInt32(Year);
                //        gm.NoOfCycles = Convert.ToInt32(NoOfCycles);
                //        gm.CycleStartYear = Convert.ToInt32(CycleStartYear);
                //        gm.CycleStartMonth = Convert.ToInt32(Month);
                //        // if (Status == "Active")
                //        gm.IsActive = true;
                //        gm.CreatedDate = DateTime.Now;
                //        gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                //        genData.CycleDefinitionMasters.Add(gm);
                //        genData.SaveChanges();

                //        id = "CycleSettingMaster added successfully";
                //    }
                //}
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in genData.CycleDefinitionMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    genData.SaveChanges();

                    id = "CycleDefinitionMaster deleted successfully";
                }

                return Json(id);
            }

            catch (Exception msg)
            {
                return Json("Cycle setting Year Already Exits");

            }
        }
        //CycleDefinitionMaster Grid
        public JsonResult GridDataCycleDefinitionMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GET_CYCLE_DEFINITION_MASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "CycleName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            //var q = (from sq in query where sq.CityName!=null && sq.CityName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)select sq).ToList();
                            var q = (from sq in query where sq.CycleName!=null &&sq.CycleName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.CycleName != null && sq.CycleName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.CycleName != null && sq.CycleName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.CycleName != null && sq.CycleName.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Year).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "CycleName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CycleName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "CycleName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.CycleName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Year" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Year ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Year" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Year descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "CycleNo" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.CycleNo ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "CycleNo" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.CycleNo descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "ToMonth" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.ToMonth ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ToMonth" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ToMonth descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "FromMonth" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.FromMonth ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "FromMonth" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.FromMonth descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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
        public ActionResult AddCycleDefinitionMaster(string id, string oper, string Year, string CycleName, string CycleNo, string FromMonth, string ToMonth, string CycleFromYear, string CycleToYear, string IsActive)
        {
            try
            {
               if (oper == "add")
            {

                int aYear = Convert.ToInt32(Year);
                var rslt = (from a in genData.CycleMasters where a.Year == aYear select a).ToList();
                if (rslt.Count() > 0)
                    id = "CycleDefinitionMaster already Exists";
                else
                {
                    CycleMaster cm = new CycleMaster();
                    cm.Year = Convert.ToInt32(Year);
                    cm.CycleName = CycleName.TrimStart();
                    cm.CycleNo = Convert.ToInt32(CycleNo);
                    cm.CycleFromMonth = FromMonth;
                    cm.CycleFromYear = Convert.ToInt32(CycleFromYear);
                    cm.CycleToMonth = ToMonth;
                    cm.CycleToYear = Convert.ToInt32(CycleToYear);
                    // if (Status == "Active")
                    cm.IsActive = true;
                    cm.CreatedDate = DateTime.Now;
                    cm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    genData.CycleMasters.Add(cm);
                    genData.SaveChanges();

                    id = "CycleDefinitionMaster added successfully";
                }
            
                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg);
            }
        }




        public JsonResult EditCycleDefinitionMaster(string id, string oper, string Year, string CycleName, string CycleNo, string FromMonth, string ToMonth, string CycleFromYear, string CycleToYear, string IsActive)
        {
            try { 

            if (oper == "edit")
            {
                int ePKID = Convert.ToInt32(id);
                var qry = (from a in genData.CycleMasters where a.PKID == ePKID select a).Single();

                qry.Year = Convert.ToInt32(Year);
                qry.CycleName = CycleName.TrimStart();
                qry.CycleNo = Convert.ToInt32(CycleNo);
                qry.CycleFromMonth = FromMonth;
                qry.CycleFromYear = Convert.ToInt32(CycleFromYear);
                qry.CycleToMonth = ToMonth;
                qry.CycleToYear = Convert.ToInt32(CycleToYear);
                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                if (IsActive == "Active")
                    qry.IsActive = true;

                if (IsActive == "Passive")
                    qry.IsActive = false;

                genData.SaveChanges();

                id = "CycleDefinitionMaster modified Successfully.";
            }
            //if (oper == "add")
            //{

            //    int aYear = Convert.ToInt32(Year);
            //    var rslt = (from a in genData.CycleMasters where a.Year == aYear select a).ToList();
            //    if (rslt.Count() > 0)
            //        id = "CycleDefinitionMaster already Exists";
            //    else
            //    {
            //        CycleMaster cm = new CycleMaster();
            //        cm.Year = Convert.ToInt32(Year);
            //        cm.CycleName = CycleName.TrimStart();
            //        cm.CycleNo = Convert.ToInt32(CycleNo);
            //        cm.CycleFromMonth = FromMonth;
            //        cm.CycleFromYear = Convert.ToInt32(CycleFromYear);
            //        cm.CycleToMonth = ToMonth;
            //        cm.CycleToYear = Convert.ToInt32(CycleToYear);
            //        // if (Status == "Active")
            //        cm.IsActive = true;
            //        cm.CreatedDate = DateTime.Now;
            //        cm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            //        genData.CycleMasters.Add(cm);
            //        genData.SaveChanges();

            //        id = "CycleDefinitionMaster added successfully";
            //    }
            //}
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in genData.CycleMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                genData.SaveChanges();

                id = "CycleDefinitionMaster deleted successfully";
            }

            return Json(id);
        }
        
        
        catch(Exception msg)
       {
           return Json(new { error = msg.Message });
          }
        }
        //Activity master
        public JsonResult GridDataActivityMaster(GridQueryModel gridQueryModel)
        {

            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GET_ACTIVITY_MASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.ActivityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "ActivityName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            
                            var q = (from sq in query where sq.ActivityName != null && sq.ActivityName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ActivityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.ActivityName != null && sq.ActivityName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ActivityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.ActivityName != null && sq.ActivityName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ActivityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.ActivityName != null && sq.ActivityName.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ActivityName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "ActivityName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.ActivityName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ActivityName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ActivityName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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
        public ActionResult AddActivityMaster(string oper, string NodeType, string Team, string ActivityName, string Defaultflag, string Specialflag, string Fieldwork, string id, string IsActive)
        {
            bool defaultFlag = false;
            bool specialFlag = false;
            bool fieldWork = false;
            bool Isactive = false;
            string ModifiedDate = "";
            decimal ModifiedBy;
            try
            {
                if (oper == "add")
                {
                    ActivityName = ActivityName.TrimStart();
                    
                        var rsult = (from a in genData.ActivityMasters where a.ActivityName == ActivityName select a).ToList();
                        if (rsult.Count() > 0)
                        {
                            id = " Activity Master already Exists";
                        }
                        else
                        {
                            if (Defaultflag == "Active")
                                defaultFlag = true;

                            if (Specialflag == "Active")
                                specialFlag = true;

                            if (Fieldwork == "Active")
                                fieldWork = true;

                            if (IsActive == "Active")
                                Isactive = true;

                            if (IsActive == "Passive")
                                Isactive = false;

                            ModifiedDate = DateTime.Now.ToShortDateString();

                            ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                            genData.USP_UPDATE_NODETYPEMASTER(oper, NodeType, Team, ActivityName, defaultFlag, specialFlag, fieldWork, id, Isactive.ToString(), ModifiedBy.ToString(), ModifiedDate, ModifiedBy.ToString(), ModifiedDate);

                            id = "Activity master added successfully";
                        }
                    

                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(new {error=msg.Message });
            }
        }
        
        public ActionResult EditActivityMaster(string oper, string NodeType, string Team, string ActivityName, string Defaultflag, string Specialflag, string Fieldwork, string id, string IsActive)
        {
            
            bool defaultFlag = false;
            bool specialFlag = false;
            bool fieldWork = false;
            bool Isactive = false;
            string ModifiedDate = "";
            decimal ModifiedBy;
            try
            {

                if (oper == "edit")
                {
                    if (ActivityName != "")
                    {

                        if (Defaultflag == "Active")
                            defaultFlag = true;

                        if (Specialflag == "Active")
                            specialFlag = true;

                        if (Fieldwork == "Active")
                            fieldWork = true;

                        if (IsActive == "Active")
                            Isactive = true;

                        if (IsActive == "Passive")
                            Isactive = false;

                        ModifiedDate = DateTime.Now.ToShortDateString();

                        ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                        ActivityName = ActivityName.TrimStart();
                        genData.USP_UPDATE_NODETYPEMASTER(oper, NodeType, Team, ActivityName, defaultFlag, specialFlag, fieldWork, id, Isactive.ToString(), ModifiedBy.ToString(), ModifiedDate, ModifiedBy.ToString(), ModifiedDate);

                        id = "Activity master modified Successfully.";
                    }
                    else
                    {
                        id = "Please Enter The Valid Activity Name";
                    }

                }
                
                if (oper == "del")
                {
                    genData.USP_DELETE_ACTIVITYMASTER(Convert.ToDecimal(id));

                    id = "Activity master deleted successfully";
                }

                return Json(id);
            }

            catch(Exception msg)
            {
                return Json(new {error=msg.Message });
            }
            //return Json(id);
        }


        //  Export --> Excel ,CSV,PDF 

        public ActionResult ExportActivityMasterToExcel()
        {
            var context = (from a in genData.USP_GET_ACTIVITY_MASTER() orderby a.ActivityName select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.ActivityName.ToString()== null ? "" : item.ActivityName.ToString(),                
                item.IsActive 
               
            }));
            return new ExcelResult(HeadersActivityMaster, ColunmTypesActivityMaster, data, "ActivityMaster.xlsx", "Activity Master");
        }
        private static readonly string[] HeadersActivityMaster = {
               "Activity Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesActivityMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String 
                
            };


        public ActionResult ExportActivityMasterToPDF()
        {
            List<ActivityMasterPDF> userList = new List<ActivityMasterPDF>();
            var context = (from a in genData.USP_GET_ACTIVITY_MASTER() orderby a.ActivityName select a).ToList();

            foreach (var item in context)
            {
                ActivityMasterPDF user = new ActivityMasterPDF();

                user.ActivityName = item.ActivityName == null ? "" : item.ActivityName.ToString();                
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
                            e.ActivityName, 
                            e.Status
                        }
                        })
                ).ToArray()
            };
            
            pdf.ExportPDF(customerList, new string[] { "ActivityName","Status" }, path);
            return File(path, "application/pdf", "ActivityMaster.pdf");
        }
        public ActionResult ExportInputMasterPDFToCsv()
        {
            var model = (from a in genData.USP_GET_ACTIVITY_MASTER() orderby a.ActivityName select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ActivityName,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record.ActivityName == null ? "" : record.ActivityName.ToString(), record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "ActivityMaster.txt");
        }
        public class ActivityMasterPDF
        {
            public string ActivityName { get; set; } 
            public string Status { get; set; }
        }

        //end


        //Unlock User Details.
        public JsonResult GridDataLockUserDetails(GridQueryModel gridQueryModel)
        {

            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GETLOGDEAILS() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.EmployeeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "EmployeeName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.EmployeeName != null && sq.EmployeeName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.EmployeeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.EmployeeName != null && sq.EmployeeName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.EmployeeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.EmployeeName != null && sq.EmployeeName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.EmployeeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.EmployeeName != null && sq.EmployeeName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.EmployeeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "EmployeeName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.EmployeeName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "EmployeeName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.EmployeeName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "LockedDate" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.LockedDate ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "LockedDate" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.LockedDate descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        //SubmitFormUnlockUser
        public ActionResult SubmitFormUnlockUser(string pkid)
        {
            genData.USP_DELETE_UNLOCKUSERDETAILS(pkid);
            //return Json(pkid);
            return RedirectToAction("LockUserDetails", "AdminSetting");
        }


        //CycleSettingMaster
        public ActionResult CycleSettingMaster()
        {
            return View();
        }
        //CycleDefinitionMaster
        public ActionResult CycleDefinitionMaster()
        {
            return View();
        }

        //ActivityMaster
        public ActionResult ActivityMaster()
        {
            return View();
        }

        //Unlock User Details.
        public ActionResult LockUserDetails()
        {
            return View();
        }

        //LoadYear

        public ActionResult LoadYear()
        {
            // var query = (from e in genData.CycleDefinitionMasters select new {  e.Year }).ToDictionary(f => Convert.ToInt32(f.Year), f => f.Year.ToString());
            // return PartialView("LoadYear", query);

            Dictionary<int, string> rslt = new Dictionary<int, string>();

            int yr;

            int currentYr = DateTime.Now.Year;

            for (yr = Startyr; yr <= currentYr + 2; yr++)
            {
                rslt.Add(yr, yr.ToString());
            }
            return PartialView("LoadYear", rslt);
        }

        public ActionResult LoadCycleStartYear()
        {
            //  var query = (from e in genData.CycleDefinitionMasters select new { e.CycleStartYear }).ToDictionary(f => Convert.ToInt32(f.CycleStartYear), f => f.CycleStartYear.ToString());
            Dictionary<int, string> rslt = new Dictionary<int, string>();

            int yr;
            int currentYr = DateTime.Now.Year;

            for (yr = Startyr; yr <= currentYr + 2; yr++)
            {
                rslt.Add(yr, yr.ToString());
            }
            return PartialView("LoadYear", rslt);
        }

        public ActionResult LoadCycleStartMonth()
        {
            var query = (from e in genData.USP_GET_MONTH() select new { e.PKID, e.GenName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.GenName.ToString());
            return PartialView("LoadYear", query);
        }
        //Activity master Partial View on NodeTypeMaster & TeamMaster .
        public ActionResult NodeActivityLinkMaster(string data)
        {
            if (data == "null")
            {
                var query = (from a in genData.USP_GET_NODETYPE_MASTER() select new { a.NODENAME, a.NODETYPEFKID }).ToDictionary(f => Convert.ToInt32(f.NODETYPEFKID), f => f.NODENAME.ToString());
                return PartialView("LoadYear", query);
            }
            else
            {
                var query = (from a in genData.USP_GET_NODETYPEMASTER(Convert.ToInt32(data)) select new { a.NODENAME, a.NODETYPEFKID }).ToDictionary(f => Convert.ToInt32(f.NODETYPEFKID), f => f.NODENAME.ToString());

                if (query.Count == 0)
                {
                    query = (from a in genData.USP_GET_NODETYPE_MASTER() select new { a.NODENAME, a.NODETYPEFKID }).ToDictionary(f => Convert.ToInt32(f.NODETYPEFKID), f => f.NODENAME.ToString());
                }

                return PartialView("NodeTypeMaster", query);
            }
        }

        public ActionResult TeamActivitylinkMaster(string data)
        {
            if (data == "null")
            {
                var query = (from e in genData.USP_GET_TEAM_MASTER() select new { e.TEAMFKID, e.TEAMNAME }).ToDictionary(f => Convert.ToInt32(f.TEAMFKID), f => f.TEAMNAME.ToString());
                return PartialView("LoadYear", query);
            }
            else
            {
                var query = (from e in genData.USP_GET_TEAMMASTER(Convert.ToInt32(data)) select new { e.TEAMFKID, e.TEAMNAME }).ToDictionary(f => Convert.ToInt32(f.TEAMFKID), f => f.TEAMNAME.ToString());
                return PartialView("NodeTypeMaster", query);
            }
        }


        //Setting Team Wise Master ...

        public ActionResult SettingMasterTeamWise()
        {
            return View();
        }

        public JsonResult GridDataSettingMasterTeamWise(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.GetSettingMasterTeamWise() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "TeamName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.TeamName != null && sq.TeamName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
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
                        pageData = (from cust in query orderby cust.TeamName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TeamName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        public ActionResult SettingMaster()
        {

            return View();

        }

        public ActionResult GridSettingMaster(GridQueryModel gridQueryModel)
        {
            var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
            var searchOper = gridQueryModel.searchOper;
            var searchField = gridQueryModel.searchField;


            var query = (from a in genData.uspGetSettingMaster() select new { a.Code, a.FromValue, a.ImpactScreenName, a.IsActive, a.PKID, a.SettingDescription, a.SettingType, a.ToValue }).ToList();
            var count = query.Count();
            var pageData = query.OrderBy(x => x.PKID).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

            if (gridQueryModel._search == true && searchString != "")
            {
                if ((searchField == "ImpactScreenName"))
                {
                    if (searchOper == "bw")//begins with
                    {
                        var q = (from sq in query where sq.ImpactScreenName != null && sq.ImpactScreenName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                        count = q.Count();
                        pageData = q.OrderBy(x => x.ImpactScreenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    }
                    else if (searchOper == "eq") //equal
                    {
                        var q = (from sq in query where sq.ImpactScreenName != null && sq.ImpactScreenName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                        count = q.Count();
                        pageData = q.OrderBy(x => x.ImpactScreenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    }
                    else if (searchOper == "ew") // ends with
                    {
                        var q = (from sq in query where sq.ImpactScreenName != null && sq.ImpactScreenName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                        count = q.Count();
                        pageData = q.OrderBy(x => x.ImpactScreenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    }
                    else if (searchOper == "cn")//contains
                    {
                        var q = (from sq in query where sq.ImpactScreenName != null && sq.ImpactScreenName.ToUpper().Contains(searchString) select sq).ToList();
                        count = q.Count();
                        pageData = q.OrderBy(x => x.ImpactScreenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "ImpactScreenName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.ImpactScreenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ImpactScreenName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ImpactScreenName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Code" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Code ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Code" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Code descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "SettingDescription" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.SettingDescription ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "SettingDescription" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.SettingDescription descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "SettingType" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.SettingType ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "SettingType" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.SettingType descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    if (gridQueryModel.sidx == "FromValue" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.FromValue ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "FromValue" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.FromValue descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "ToValue" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.ToValue ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ToValue" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ToValue descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.IsActive ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "IsActive" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.IsActive descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                }
            }
            return Json(new
            {
                page = gridQueryModel.page,
                records = count,
                rows = pageData,
                total = Math.Ceiling((decimal)count / gridQueryModel.rows)
            }, JsonRequestBehavior.AllowGet);


        }


        public ActionResult LoadSettingType()
        {

            Dictionary<int, string> objDist = new Dictionary<int, string>();

            objDist.Add(1, "Text");
            objDist.Add(2, "Number");
            objDist.Add(3, "Flag");
            objDist.Add(4, "Date");
            objDist.Add(5, "Time");
            objDist.Add(6, "Mail");
            objDist.Add(7, "Days");

            return 
                View("PV_SettingType", objDist);
        }


        public ActionResult AddSettingMaster(FormCollection frm, string id)
        {
            try
            {
                var code = frm["Code"].ToString();
                var reslt = (from a in genData.SettingMasters where a.Code == code select a).ToList();
                if (reslt.Count > 0)
                {
                    id = "Setting Master already Exists";
                }
                else
                {
                    string fromvalue;
                    string Tovalue;
                    fromvalue = "";
                    Tovalue = "";
                    int USERFKID = int.Parse(Session["USER_FKID"].ToString());
                    if (frm["oper"].ToString() != "del")
                    {
                        if (frm["SettingType"].ToString() == "1" || frm["SettingType"].ToString() == "2" || frm["SettingType"].ToString() == "5" || frm["SettingType"].ToString() == "6" || frm["SettingType"].ToString() == "7")
                        {
                            fromvalue = frm["FromValue"].ToString();
                            Tovalue = frm["ToValue"].ToString();
                        }
                        if (frm["SettingType"].ToString() == "3")
                        {
                            fromvalue = frm["Flag"].ToString();
                            Tovalue = "";
                        }
                        if (frm["SettingType"].ToString() == "4")
                        {
                            if (frm["Fromdate"].ToString() == "")
                            {
                                fromvalue = "01/01/1900";
                            }
                            else
                            {
                                fromvalue = frm["Fromdate"].ToString();
                            }
                            if (frm["Todate"].ToString() == "")
                            {
                                Tovalue = "01/01/1900";
                            }
                            else
                            {
                                Tovalue = frm["Todate"].ToString();
                            }
                        }
                    }
                        genData.AddSettingMaster(frm["ImpactScreenName"].TrimStart().ToString(), frm["SettingDescription"].TrimStart().ToString(), frm["SettingType"].TrimStart().ToString(), fromvalue, Tovalue, true, USERFKID, frm["Code"].ToString());
                        id = "Setting Master Added Successfully";
                    }
                    return Json(id);
                }
            
            catch (Exception msg)
            {
                return Json(new { error = msg.Message });
            }
        }
        

        public ActionResult EditSettingMaster(FormCollection frm, string id)
        {
            try
            {
                string fromvalue;
                string Tovalue;
                fromvalue = "";
                Tovalue = "";
                int USERFKID = int.Parse(Session["USER_FKID"].ToString());
                if (frm["oper"].ToString() != "del")
                {
                    if (frm["SettingType"].ToString() == "1" || frm["SettingType"].ToString() == "2" || frm["SettingType"].ToString() == "5" || frm["SettingType"].ToString() == "6" || frm["SettingType"].ToString() == "7")
                    {
                        fromvalue = frm["FromValue"].ToString();
                        Tovalue = frm["ToValue"].ToString();
                    }
                    if (frm["SettingType"].ToString() == "3")
                    {
                        fromvalue = frm["Flag"].ToString();
                        Tovalue = "";
                    }
                    if (frm["SettingType"].ToString() == "4")
                    {
                        if (frm["Fromdate"].ToString() == "")
                        {
                            fromvalue = "01/01/1900";
                        }
                        else
                        {
                            fromvalue = frm["Fromdate"].ToString();
                        }
                        if (frm["Todate"].ToString() == "")
                        {
                            Tovalue = "01/01/1900";
                        }
                        else
                        {
                            Tovalue = frm["Todate"].ToString();
                        }
                    }

                    if (frm["oper"].ToString() == "edit")
                    {


                        bool Status = false;

                        if (frm["IsActive"].ToString() == "on" || frm["IsActive"].ToString() == "Active")
                        {
                            Status = true;
                        }
                        if (frm["IsActive"].ToString() == "off")
                        {
                            Status = false;
                        }

                        genData.EditSettingMaster(Convert.ToDecimal(frm["PKID"].ToString()), frm["ImpactScreenName"].TrimStart().ToString(), frm["SettingDescription"].TrimStart().ToString(), frm["SettingType"].TrimStart().ToString(), fromvalue.TrimStart(), Tovalue.TrimStart(), Status, USERFKID, frm["Code"].ToString());

                        id="Setting Master Modified Successfully";
                    }

                //    if (frm["oper"].ToString() == "add")
                //    {
                //    //    if(){
                //    //     int aYear = Convert.ToInt32(Year);
                //    //var rslt = (from a in genData.AddSettingMasters where a.Year == aYear select a).ToList();
                //    //if (rslt.Count() > 0)
                //    //    id = "CycleSettingMaster already Exists";
                        
                //    //    }
                //    //    else{
                //        //var rslt=(from a in genData.AddSettingMaster where a.Code==frm["Code"].ToString()).ToList();
                     
                        
                //    }
                }
                if (frm["oper"].ToString() == "del")
                {
                    genData.DeleteSettingMaster(Convert.ToDecimal(id));
                    id="Setting Master deleted Succecssfully ";
                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(new { error = msg.Message });
            }
        }
        
   

        

        //settingmaster Export --> Excel ,CSV,PDF


        public ActionResult ExportSettingMasToExcel()
        {
            var context = (from a in genData.uspGetSettingMaster()
                           select new { a.ImpactScreenName, a.SettingDescription, a.SettingType, a.ToValue, a.FromValue, a.Code, IsActive = a.IsActive == "1" ? "Active" : "In Active" }).ToList();



           // var context =  genData.uspGetSettingMaster() .Select(a => new{ 
           //a.Code, a.FromValue, a.ImpactScreenName,  a.PKID, a.SettingDescription, a.SettingType, a.ToValue,
           //  IsActive = a.IsActive == "1" ? "Active" : "In Active"
           // }).ToList();
            
            //select new { a.Code, a.FromValue, a.ImpactScreenName, a.IsActive, a.PKID, a.SettingDescription, a.SettingType, a.ToValue }).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
               
                item.ImpactScreenName.ToString()== null ? "" :item.ImpactScreenName.ToString(),                
                item.Code.ToString()== null ? "" :item.Code.ToString(), 
                item.SettingDescription== null ? "" :item.SettingDescription, 
                 item.SettingType== null ? "" :item.SettingType, 
                 item.FromValue== null ? "" :item.FromValue.ToString(), 
                 item.ToValue== null ? "" :item.ToValue.ToString(),
                item.IsActive
               
            }));

            return new ExcelResult(HeadersSettingMaster, ColunmTypesSettingMaster, data, "SettingMaster.xlsx", "SettingMaster");
        }
        private static readonly string[] HeadersSettingMaster = {
                "ImpactScreenName", "Code","SettingDescription","SettingType","FromValue","ToValue","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesSettingMaster = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                 DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };

        public class SettingMasterCls
        {

            public string ImpactScreenName { get; set; }
            public string Code { get; set; }
            public string SettingDescription { get; set; }
            public string SettingType { get; set; }

            public string FromValue { get; set; }
            public string ToValue { get; set; }
            public string Status { get; set; }
            
        }



        public ActionResult ExportSettingMasToPDF()
        {
            List<SettingMasterCls> userList = new List<SettingMasterCls>();
            //var context = genData.uspGetSettingMaster().Select(a => new
            //{
            //    a.Code,
            //    a.FromValue,
            //    a.ImpactScreenName,
            //    a.PKID,
            //    a.SettingDescription,
            //    a.SettingType,
            //    a.ToValue,
            //    IsActive = a.IsActive == "1" ? "Active" : "In Active"
            //}).ToList();
            var context = (from a in genData.uspGetSettingMaster() select new { a.ImpactScreenName,a.IsActive,a.SettingDescription,a.SettingType,a.ToValue,a.FromValue,a.Code}).ToList();
            foreach (var item in context)
            {
                SettingMasterCls user = new SettingMasterCls();
                user.ImpactScreenName = item.ImpactScreenName.ToString() == null ? "" : item.ImpactScreenName.ToString();
                user.Code = item.Code.ToString() == null ? "" : item.Code.ToString();
                user.SettingDescription = item.SettingDescription.ToString() == null ? "" : item.SettingDescription.ToString();
                user.SettingType = item.SettingType == null ? "" : item.SettingType;
                user.FromValue = item.FromValue.ToString() == null ? "" : item.FromValue.ToString();

                user.ToValue = item.ToValue.ToString() == null ? "" : item.ToValue.ToString();
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
                    customerList.Select(a =>
                        new
                        {

                            cell = new string[]{
                          a.Code, a.FromValue, a.ImpactScreenName, a.Status,  a.SettingDescription, a.SettingType, a.ToValue
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "ImpactScreenName", "Code", "SettingDescription", "SettingType", "FromValue", "ToValue", "Status" }, path);
            return File(path, "application/pdf", "SettingMaster.pdf");
        }
        public ActionResult ExportSettingMasToCsv()
        {
            //var model = genData.uspGetSettingMaster().Select(a => new
            //{
            //    a.Code,
            //    a.FromValue,
            //    a.ImpactScreenName,
            //    a.PKID,
            //    a.SettingDescription,
            //    a.SettingType,
            //    a.ToValue,
            //    IsActive = a.IsActive == "1" ? "Active" : "In Active"
            //}).ToList();
            var model = (from a in genData.uspGetSettingMaster() select new { a.ImpactScreenName, a.IsActive, a.SettingDescription, a.SettingType, a.ToValue, a.FromValue, a.Code }).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ImpactScreenName, Code, SettingDescription, SettingType, FromValue, ToValue, Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6}",
                    record.ImpactScreenName == null ? "" : record.ImpactScreenName.ToString(),
                    record.Code == null ? "" : record.Code.ToString(),
                    record.SettingDescription == null ? "" : record.SettingDescription.ToString(),
                    record.SettingType == null ? "" : record.SettingType,
                    record.FromValue == null ? "" : record.FromValue.ToString(),
                    record.ToValue == null ? "" : record.ToValue.ToString(), 
                    record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "SettingMaster.txt");
        }
        


        //end

        public ActionResult UserQuestionLink()
        {

            return View();
        }

        public JsonResult gridUserQuestionLink(GridQueryModel gridQueryModel)
        {

            var query = (from q in genData.GetUserDefault() select q).ToList();
            var pageData = query.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
             
            return Json(new
            {
                page = gridQueryModel.page,
                records = query.Count(),
                rows = pageData,
                total = Math.Ceiling((decimal)query.Count() / gridQueryModel.rows)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateGridDataSettingTeamwise(string gridData)
        {

            decimal PSOFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            var serialize = new JavaScriptSerializer();
            var deserialize = serialize.Deserialize<List<ClsSettingMasterTeamwise>>(gridData);

            var str = "<Root>";
            foreach (var item in deserialize)
            {

                str += "<SMTDtls ";
                str += "TeamFKID='" + item.TeamFKID + "' ";
                str += "CPDaysBeforeAllowed='" + item.CPDaysBeforeAllowed + "' ";

                str += "PTcutOffDays='" + item.PTcutOffDay + "' ";

                str += "MinMonthlyFrequency='" + item.MinMonthlyFrequency + "' ";
                str += "MaxMothlyFrequency='" + item.MaxMothlyFrequency + "' ";
                str += "CPUnlockEntry='" + item.CPUnlockEntry + "' ";

                str += "TPcutOffDays='" + item.TPcutOffDays + "' ";
                str += "TPNoOfMonths='" + item.TPNoOfMonths + "' ";

                str += "TPlock='" + item.TPlock + "' ";
                str += "DREntryAllowedTime='" + item.DREntryAllowedTime + "' ";
                str += "DREntryAllowedDays='" + item.DREntryAllowedDays + "' ";

                str += "DRUnLockEntry='" + item.DRUnLockEntry + "' ";
                str += "DPOLockPLStage='" + item.DPOLockPLStage + "' ";
                str += "DPPLOCKall='" + item.DPPLOCKall + "' ";




                str += "CPRRVMinMonthlyFrequency='" + item.CPRRVMinMonthlyFrequency + "' ";
                str += "CPRRVMaxMothlyFrequency='" + item.CPRRVMaxMothlyFrequency + "' ";
                str += "CPRRBMinMonthlyFrequency='" + item.CPRRBMinMonthlyFrequency + "' ";
                str += "CPRRBMaxMothlyFrequency='" + item.CPRRBMaxMothlyFrequency + "' ";
                str += "Product_Preloaded='" + item.Product_Preloaded + "' ";



                str += "DM_DREntryAllowedTime ='" + item.DM_DREntryAllowedTime + "' ";
                str += "DM_DREntryAllowedDays  ='" + item.DM_DREntryAllowedDays + "' ";

                str += "DM_DRUnLockEntry  ='" + item.DM_DRUnLockEntry + "' ";



                str += "RBM_DREntryAllowedTime ='" + item.RBM_DREntryAllowedTime + "' ";
                str += "RBM_DREntryAllowedDays  ='" + item.RBM_DREntryAllowedDays + "' ";
                str += "RBM_DRUnLockEntry  ='" + item.RBM_DRUnLockEntry + "' ";


                str += "SS_MonthEnd ='" + item.SS_MonthEnd + "' ";
                str += "SS_MonthBegin  ='" + item.SS_MonthBegin + "' ";


                str += "/>";

            }
            
            str += "</Root>";

            decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            genData.AddSettingMasterTeamWiseNew(PSOFKID, str, ModifiedBy);

            return Json("");

        }
       
        // Newly Added
        public ActionResult RelocateMenu()
        {

            return View();
        }

        /* Baskar Starts */


        //CycleSettingMaster Export --> Excel ,CSV,PDF


        public ActionResult ExportCycleSettingMasterToExcel()
        {
            var context = (from a in genData.USP_GET_CYCLE_SETTING_MASTER() orderby a.Year  select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
               // item.PKID.ToString(),
                item.Year.ToString()== null ? "" :item.Year.ToString(),                
                item.NoOfCycles.ToString()== null ? "" : item.NoOfCycles.ToString(), 
                item.Month== null ? "" : item.Month, 
                item.Status
               
            }));

            return new ExcelResult(HeadersCycleSettingMaster, ColunmTypesCycleSettingMaster, data, "CycleSettingMaster.xlsx", "CycleSettingMaster");
        }
        private static readonly string[] HeadersCycleSettingMaster = {
               "Year","NoofCycles","Cycle Start Month","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesCycleSettingMaster = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };


        public ActionResult ExportCycleSettingMasterToPDF()
        {
            List<CycleSettingMasterType> userList = new List<CycleSettingMasterType>();
            var context = (from a in genData.USP_GET_CYCLE_SETTING_MASTER() orderby a.Year select a).ToList();

            foreach (var item in context)
            {
                CycleSettingMasterType user = new CycleSettingMasterType();
               // user.PKID = item.PKID.ToString();
                //user.EmployeeCode = item.EmpCode == null ? "" : item.EmpCode;

                user.Year = item.Year.ToString() == null ? "" :item.Year.ToString();
                user.NoofCycle = item.NoOfCycles.ToString() == null ? "" : item.NoOfCycles.ToString();
                user.CycleStartMonths = item.Month == null ? "" : item.Month;
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
                            //id = e.PKID,
                            cell = new string[]{
                            //e.PKID.ToString(),e.Year.ToString(),e.NoofCycle.ToString(),e.CycleStartMonths,e.Status
                        }
                        })
                ).ToArray()
            };
            
            pdf.ExportPDF(customerList, new string[] { "Year", "NoofCycle", "CycleStartMonths", "Status" }, path);
            return File(path, "application/pdf", "CycleSettingMaster.pdf");
        }
        public ActionResult ExportCycleSettingMasterToCsv()
        {
            var model = (from a in genData.USP_GET_CYCLE_SETTING_MASTER() orderby a.Year select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine(" Year, NoOfCycles, CycleStartMonth, Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3}",record.Year, record.NoOfCycles, record.Month, record.Status

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "CycleSettingMaster.txt");
        }
        public class CycleSettingMasterType
        {
            public string PKID { get; set; }
            public string Year { get; set; }
            public string NoofCycle { get; set; }
            public string CycleStartMonths { get; set; }
            public string Status { get; set; }
        }




        //CycleDefinitionMaster Export --> Excel ,CSV,PDF

        public ActionResult ExportCycleDefinitionMasterToExcel()
        {
            var context = (from a in genData.USP_GET_CYCLE_DEFINITION_MASTER() orderby a.Year  select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
               // item.PKID.ToString(),
                item.Year.ToString()== null ? "" :item.Year.ToString(),                
                item.CycleName== null ? "" :item.CycleName.ToString(),                
                item.CycleNo.ToString()== null ? "" :item.CycleNo.ToString(),                
                item.FromMonth== null ? "" :item.FromMonth,                
                item.ToMonth== null ? "" :item.ToMonth,                
                item.IsActive
               
            }));
            return new ExcelResult(HeadersCycleDefinitionMasterType, ColunmTypesCycleDefinitionMasterType, data, "CycleDefinitionMaster.xlsx", "GeneralMaster");
        }
        private static readonly string[] HeadersCycleDefinitionMasterType = {
               "Year", "Cycle Name","Cycle No","Cycle Start Month","Cycle End Month","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesCycleDefinitionMasterType = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportCycleDefinitionMasterToPDF()
        {
            try
            {
                List<CycleDefinitionMasterType> userList = new List<CycleDefinitionMasterType>();
                var context = (from a in genData.USP_GET_CYCLE_DEFINITION_MASTER() orderby a.Year select a).ToList();

                foreach (var item in context)
                {
                    CycleDefinitionMasterType user = new CycleDefinitionMasterType();
                    //user.PKID = item.PKID.ToString();
                    user.Year = item.Year.ToString() == null ? "" : item.Year.ToString();
                    user.CycleName = item.CycleName == null ? "" : item.CycleName.ToString();
                    user.CycleNo = item.CycleNo.ToString() == null ? "" : item.CycleNo.ToString();
                    user.CycleFromMonth = item.FromMonth== null ? "" :item.FromMonth;
                    user.CycleToMonth = item.ToMonth ==null ? "" :item.ToMonth;
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
                                //id = e.PKID,
                                cell = new string[]{
                           e.Year.ToString(),e.CycleName,e.CycleNo,e.CycleFromMonth,e.CycleToMonth,e.Status
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] {"Year", "CycleName", "CycleNo", "CycleFromMonth", "CycleToMonth", "Status" }, path);
                return File(path, "application/pdf", "CycleDefinitionMaster.pdf");
            }
            catch(Exception ex) { 
            
            }
           return File(path, "application/pdf", "CycleDefinitionMaster.pdf");
        }
        public ActionResult ExportCycleDefinitionMasterToCsv()
        {
            var model = (from a in genData.USP_GET_CYCLE_DEFINITION_MASTER() orderby a.Year select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Year,CycleName,CycleNo,CycleFromMonth,CycleToMonth,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5}",
                    record.Year.ToString(),
                    record.CycleName == null ? "" : record.CycleName,
                    record.CycleNo.ToString() == null ? "" : record.CycleNo.ToString(),
                    record.FromMonth == null ? "" : record.FromMonth,
                    record.ToMonth == null ? "" : record.ToMonth, 
                    record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "CycleDefinitionMaster.txt");
        }
        public class CycleDefinitionMasterType
        {
            public string PKID { get; set; }
            public string Year { get; set; }
            public string CycleName { get; set; }
            public string CycleNo { get; set; }
            public string CycleFromMonth { get; set; }
            public string CycleToMonth { get; set; }
            public string Status { get; set; }
        }





        //UnlockUser export excel,pdf,csv

        public ActionResult ExportUnlockUserToExcel()
        {
            var context = (from a in genData.USP_GETLOGDEAILS() orderby a.EmployeeName select a).ToList();
            //if (context.Count == 0)
            //    return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {                
                item.EmployeeName,                
                item.LockedDate
               
            }));
            return new ExcelResult(HeadersUnlockUser, ColunmTypesUnlockUser, data, "UnlockUser.xlsx", "UnlockUser");
        }
        private static readonly string[] HeadersUnlockUser = {
                 "Employee Name","Locked Date"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesUnlockUser = {
                DataForExcel.DataType.String,                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String
            };
        public ActionResult ExportUnlockUserToPDF()
        {
            List<UnlockUserTypes> userList = new List<UnlockUserTypes>();
            var context = (from a in genData.USP_GETLOGDEAILS() orderby a.EmployeeName select a).ToList();

            foreach (var item in context)
            {
                UnlockUserTypes user = new UnlockUserTypes();
                user.EmployeeName = item.EmployeeName;
                user.LockedDate = item.LockedDate;
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
                           e.EmployeeName,e.LockedDate
                        }
                        })
                ).ToArray()
            };
            
            pdf.ExportPDF(customerList, new string[] { "EmployeeName", "LockedDate" }, path);
            return File(path, "application/pdf", "UnlockUser.pdf");
        }
        public ActionResult ExportUnlockUserToCsv()
        {
            var model = (from a in genData.USP_GETLOGDEAILS() orderby a.EmployeeName select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("EmployeeName,LockedDate");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record.EmployeeName, record.LockedDate

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "DistrictMaster.txt");
        }
        public class UnlockUserTypes
        {
            public string EmployeeName { get; set; }
            public string LockedDate { get; set; }

        }

        /* Ends */


        /* Reconcile starts */



        public ActionResult SampleInputReconcileSettings()
        {


            return View();
        }

        public JsonResult ReconcileGridData(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var result = genData.usp_ReconcileSettings().Select(x => new
                {

                    PKID = x.PKID,
                    DateofBegining = x.DateofBegining,
                    TeamName=x.TeamName,
                    x.TeamFKID
                }).ToList();
                var count = result.Count();

                var pageData = result.OrderBy(x => x.DateofBegining).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "TeamName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where sq.TeamName != null && sq.TeamName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.DateofBegining).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where sq.TeamName != null && sq.TeamName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where sq.TeamName != null && sq.TeamName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in result where sq.TeamName != null && sq.TeamName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = result.Count();

                    if (gridQueryModel.sidx == "DateofBegining" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.DateofBegining ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "DateofBegining" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.DateofBegining descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.TeamName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.TeamName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public ActionResult LoadTeam(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            
                if (rowId != "null")
                    query = (from a in genData.GetTeamNameForReconcileByPKID(Convert.ToDecimal(rowId)) where a.Element == type select new { a.Element, a.PKID, a.TeamName }).OrderBy(x => x.TeamName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName.ToString());
                else
                    query = (from a in genData.GetTeamsByXML() select new { a.PKID, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName.ToString());

            return PartialView("PVLoadReconcileTeam", query);
        }

        public ActionResult LoadSelectedTeam(string rowId, string type)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (rowId != "null")
                query = (from a in genData.GetTeamNameForReconcileByPKID(Convert.ToDecimal(rowId)) where a.Element == type select new { a.Element, a.PKID, a.TeamName }).OrderBy(x => x.TeamName).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName.ToString());
            else
                query = (from a in genData.GetTeamsByXML() where 1==2 select new { a.PKID, a.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName.ToString());

            return PartialView("PVLoadReconcileTeam", query);
        }



        public string returnXml(string teamName)
        {

            string[] arrayUser = teamName.Split(',');

            string xml = "<root>";

            for (int i = 0; i < arrayUser.Count(); i++)
            {

                xml += "<Teams";
                xml += " TeamFKID='" + arrayUser[i] + "'";
                xml += "/>";


            }
            xml += "</root>";
            return xml;


        }
        public JsonResult AddReconcileGrid(string id, string oper, string PKID, string DateofBegining, string SelTeamName)
        {
            string xml = "";
            string[] date;
            string newDate=string.Empty;
               
            try
            {
                
                    
                    if (DateofBegining != "" || DateofBegining != null)
                    {
                        date = DateofBegining.Split('/');
                        newDate = date[1] + "/" + date[0] + "/" + date[2];
                    }
                    xml = returnXml(SelTeamName);



                  //  var result = (from a in genData.SampleInputReconcileSettings where a.DateofBegining == Convert.ToDateTime(DateofBegining) && a.TeamFKID == int.Parse(SelTeamName) select a).ToList();
                  //  if (result.Count() > 0)
                  //     id = "Reconcile Date is already updated for one of the selected Team in the Selected Month";
                  //else { 
                   var rslt= genData.AddSampleInputReconcileSettings(newDate, xml, Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                    id = "Reconcile Date Added Successfully.";
                   //}
                   
                } 
            catch(Exception msg)
            {
                return Json("Reconcile Date is already updated for one of the selected Team in the Selected Month");

            }
            return Json(id);
        }
        public JsonResult EditReconcileGrid(string id, string oper, string PKID, string DateofBegining, string SelTeamName)
        {

            string xml = "";
            string[] date;
            string newDate=string.Empty;
               
            try
            {
                 
                if (oper == "edit")
                {
                    long? pkId = Convert.ToInt32(PKID);
                    if (DateofBegining != "" || DateofBegining != null)
                    {
                        date = DateofBegining.Split('/');
                        newDate = date[1] + "/" + date[0] + "/" + date[2];
                    }
                    xml = returnXml(SelTeamName);
                    genData.EditSampleInputReconcileSettings(newDate, xml, Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()), pkId);

                    id = "Reconcile Date Edited Successfully.";
                }
                
                if (oper == "del")
                { 
                    genData.DeleteSampleInputReconcileSettings(Convert.ToDecimal(id), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                    id = "Reconcile Date Deleted Successfully.";
                }
            }
            catch(Exception msg) {
                return Json("Reconcile Date Edited Successfully.");
            }
            return Json(id);
        }

        //  Export --> Excel ,CSV,PDF 

        public ActionResult ExportReconcileToExcel()
        {
            //var context = genData.usp_ReconcileSettings().Select(x => new
            //{
            //    PKID = x.PKID,
            //    DateofBegining = x.DateofBegining.ToShortDateString(),
            //    TeamName = x.TeamName,
            //    x.TeamFKID
            //}).ToList();
            var context = (from a in genData.usp_ReconcileSettings()
                         select new {a.DateofBegining,a.TeamName }).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.DateofBegining.ToString() == null ? "" : item.DateofBegining.ToString(),
                item.TeamName  == null ? "" : item.TeamName.ToString()
               
            }));
            return new ExcelResult(HeadersReconcile, ColunmTypesReconcile, data, "Reconcile.xlsx", "Sample Input Reconcile Settings");
        }
            private static readonly string[] HeadersReconcile = {
                   "DateofBegining","TeamName"
                };
            private static readonly DataForExcel.DataType[] ColunmTypesReconcile = {  
                    DataForExcel.DataType.String,
                    DataForExcel.DataType.String  
                };


            public ActionResult ExportReconcileToPDF()
        {
            try
            {
                List<ReconcilePDF> reconcileList = new List<ReconcilePDF>();
                //var context = genData.usp_ReconcileSettings().Select(x => new
                //{ 
                //    DateofBegining = x.DateofBegining.ToShortDateString(),
                //    TeamName = x.TeamName 
                   
                //}).ToList();
                var context = (from a in genData.usp_ReconcileSettings()
                               
                               select new { DateofBegining=a.DateofBegining,TeamName= a.TeamName }).ToList();

                foreach (var item in context)
                {
                    ReconcilePDF recon = new ReconcilePDF();

                    recon.DateofBegining = item.DateofBegining == null ? "" : item.DateofBegining.ToString();
                    recon.TeamName = item.TeamName == null ? "" : item.TeamName.ToString();
                    reconcileList.Add(recon);
                }
                var customerList = reconcileList;

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
                            e.DateofBegining,e.TeamName
                        }
                            })
                    ).ToArray()
                };



                pdf.ExportPDF(customerList, new string[] { "DateofBegining", "TeamName" }, path);
                return File(path, "application/pdf", "Reconcile.pdf");
            }
            catch
            {
                return View();
            }
        }
            public ActionResult ExportReconcileToCsv()
        {
            //var model = genData.usp_ReconcileSettings().Select(x => new
            //{

            //    PKID = x.PKID,
            //    DateofBegining = x.DateofBegining.ToShortDateString(),
            //    TeamName = x.TeamName,
            //    x.TeamFKID
            //}).ToList();
             var model = (from a in genData.usp_ReconcileSettings() orderby a.DateofBegining ascending
                         select new {DateofBegining=a.DateofBegining,TeamName=a.TeamName }).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("DateofBegining,TeamName");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record.DateofBegining == null ? "" : record.DateofBegining.ToString(), record.TeamName == null ? "" : record.TeamName.ToString()

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "Reconcile.txt");
        }
        public class ReconcilePDF
        {

            public string DateofBegining { get; set; }
            public string TeamName { get; set; }
        }

        //    end 

        /* ends */


        #region Mathan

        #region Default User QuestionLink Master


        #region Action Controller Class

        public ActionResult DefaultUserQuestionLinkMaster()
        {

            return View();
        }

        #endregion

        #region Load Grid Data

        public JsonResult GetUserQuestionLinkMaster(GridQueryModel gridQueryModel)
        {
            try
            {

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in genData.GetUserDefault() select q).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "Question"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query
                                     where sq.Question != null && sq.Question.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)
                                     select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.Question != null && sq.Question.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.Question != null && sq.Question.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.Question != null && sq.Question.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "Question" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Question descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Question" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Question ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        #region Upadate the Default User QuestionLink Master

        public ActionResult UpdateTeamReminderLinkMaster(string gridData)
        {
            try
            {
                JavaScriptSerializer objJavaScriptSerializer = new JavaScriptSerializer();

                UserQuestionLinkMaster[] objStatus = objJavaScriptSerializer.Deserialize<UserQuestionLinkMaster[]>(gridData);
                string msg = string.Empty;

                string s = string.Empty;
                StringBuilder sb = new StringBuilder();
                string Flag = string.Empty;
                sb.Append("<Root>");
                foreach (UserQuestionLinkMaster val in objStatus)
                {
                    sb.Append("<CampaignDtls ");
                    sb.Append("QuestionFKID= '" + val.PKID + "' ");
                    sb.Append("Answer= '" + val.Answer + "' ");
                    sb.Append("/> ");

                }
                sb.Append("</Root>");

                string strxml = sb.ToString();
                decimal userfkid = Convert.ToDecimal(Session["USER_FKID"]);
                decimal createtdby = Convert.ToDecimal(Session["USER_FKID"]);
                int save = genData.AddUserMaster3(userfkid, createtdby, strxml);

                msg = "Question Link Master Updated Successfully.";

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        public class UserQuestionLinkMaster
        {
            public string PKID { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
        }


        #endregion

        #endregion

        #endregion

     
        #region Nagarajan

        public JsonResult GetRoleBasedmenuItem(string RoleId)
        {
            dynamic query = (from a in genData.getRoleBasedmenu(RoleId) select new { a.RoleID, a.MenuFKID }).ToList();
            return Json(query);

        }

        //Relocation Menu
        public ActionResult Reallocatemenu()
        {

            return View();
        }


        public string MenuRightid(string Right)
        {

            string[] arrayUser = Right.Split(',');

            string xml = "<root>";

            for (int i = 0; i < arrayUser.Count(); i++)
            {

                xml += "<row";
                xml += " MenuID='" + arrayUser[i] + "'";
                xml += "/>";


            }
            xml += "</root>";
            return xml;


        }

        public ActionResult UseMenuRight(string RoleId, string Right)
        {
            string xml = "";
            string msg = "";
            xml = MenuRightid(Right);
            genData.insertRoleBasedMenu(RoleId, xml);
            msg = "Reallocation Menu Rights Added Successfully.";

            return Json(new
            {
                msg
            }, JsonRequestBehavior.AllowGet);


        }

        //Menu Heading

        public JsonResult JsonLoadRole()
        {
            string str = string.Empty;
            string str1 = string.Empty;
            var RoleDate = (from a in genData.GetMenuForRole(Convert.ToString(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString())) select new { a.MenuID, a.MenuHeading, a.MenuSequence, a.Element }).ToList();

            foreach (var item in RoleDate)
            {
                str += "<h3>" + item.MenuHeading + "</h3><div><p>";
                str += "<table border='0' align='left'>";
                if (item.Element == "MenuHead")
                {
                    //str += " <tr><td width='1%'><input type='checkbox'id='" + item.MenuID + "'></td>";
                    //  str += "<tr><td align='left' width='75%' style='font-family:Times New Roman;font-weight:bold;'>" + item.MenuHeading + "</td></tr></p>";
                    str1 = ChileMenu(item.MenuID);

                }
                str += str1 + "</table></div>";

            }

            return Json(new
            {
                str
            }, JsonRequestBehavior.AllowGet);

        }
        //Child Heading
        public string ChileMenu(decimal ParentID)
        {
            string str = string.Empty;
            var MenuChild = (from a in genData.GetMenuForChile(Convert.ToInt32(ParentID)) select new { a.MasterID, a.MenuHeading, a.ParentMenuID, a.MenuSequence, a.Element }).ToList();
            foreach (var child in MenuChild)
            {
                str += "<table id='ChildCheckboxid'>";
                if (child.Element == "MenuMaster")
                {
                    str += "<tr><td><input type='checkbox' class='checkbox' id='" + child.MasterID + "'/></td>";
                    str += " <td class='text_style_bd'>" + child.MenuHeading + "</td></tr>";
                }
                str += "</table>";
            }
            return str;

        }


        //Get Roles
        public JsonResult GetRoles()
        {
            List<SelectListItem> UseRoles = new List<SelectListItem>();
            UseRoles.Clear();
            var item = (from a in genData.GetRoles() select new { a.RoleCode, a.RoleName, a.PKID }).ToList();
            if (item.Count() > 0)
            {
                UseRoles = item.Select(x => new System.Web.Mvc.SelectListItem()
                {
                    Text = x.RoleName,
                    Value = x.PKID.ToString()
                }).ToList();
            }
            return Json(new
            {
                RoleItem = UseRoles,
                RoleCount = UseRoles.Count,
            }, JsonRequestBehavior.AllowGet);

        }
        #endregion
      
      

    }
}
