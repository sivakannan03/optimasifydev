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
    public class GeneralMasterController : Controller
    {
        ExportPdf pdf = new ExportPdf();        
        private pfizerEntities genData = new pfizerEntities();
        string path = ConfigurationSettings.AppSettings["PDFfilePath"].ToString();



        public JsonResult GridData(GridQueryModel gridQueryModel, string fkId)
        {
            try
            {
                

                decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper(); 

                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.usp_Get_CampTypeMaster() where a.GenFKID == QrygeneralFKID select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "TableName") || (searchField == "GenName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.TableName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.TableName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.TableName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }                     


                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().Contains(searchString.ToUpper()) || sq.TableName.ToUpper().Contains(searchString.ToUpper()) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }

                }
                else
                {
                    count = query.Count();

                    if (gridQueryModel.sidx == "TableName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TableName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "TableName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TableName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.GenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.GenName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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
                return Json(fkId);
            }
        }

        public JsonResult Edit(string id, string oper, string PKID, string Gencode, string GenName, string IsActive, string fkId, string TableName)
        {
            try { 

            if (oper == "edit")
            {
                int ePKID = Convert.ToInt32(PKID);
                var qry = (from a in genData.GeneralMasters where a.PKID == ePKID select a).Single();

                qry.GenCode = Gencode.TrimStart();
                qry.GenName = GenName.TrimStart();
                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());               

                if (IsActive == "on" || IsActive == "Active")
                    qry.IsActive = true;
                else if (IsActive == "off" || IsActive == "Inactive")
                    qry.IsActive = false;

                genData.SaveChanges();

                id = TableName + " modified Successfully.";
            }
            if (oper == "add")
            {

                 decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                 var rslt = (from a in genData.GeneralMasters where a.GenName==GenName && a.GenFKID == QrygeneralFKID select a).ToList();
                 if (rslt.Count() > 0)
                     id =GenName+ " Name already Exists";
                 else
                 {
                     GeneralMaster gm = new GeneralMaster();
                     gm.GenFKID = Convert.ToDecimal(fkId);//24;
                     gm.GenCode = Gencode.TrimStart();
                     gm.GenName = GenName.TrimStart();
                     gm.IsActive = true;
                     gm.CreatedDate = DateTime.Now;
                     gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                     genData.GeneralMasters.Add(gm);
                     genData.SaveChanges();

                     id = TableName + " added successfully.";
                 }
            }
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in genData.GeneralMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                genData.SaveChanges();
                id = delete_quote.GenName + " deleted successfully.";
            }

            return Json(id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        #region UseFull Link

        public JsonResult UsefullinkGridData(GridQueryModel gridQueryModel, string fkId)
        {
            try
            {


                decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper(); 

                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.usp_Get_CampTypeMaster() where a.GenFKID == QrygeneralFKID select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "TableName") || (searchField == "GenName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.TableName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.TableName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.TableName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }


                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().Contains(searchString.ToUpper()) || sq.TableName.ToUpper().Contains(searchString.ToUpper()) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.GenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.GenName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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
                return Json(fkId);
            }
        }

        public JsonResult EditUsefullink(string id, string oper, string PKID, string Gencode, string GenName, string IsActive, string fkId, string TableName)
        {
            
                if (oper == "edit")
                {
                    int ePKID = Convert.ToInt32(PKID);
                    var qry = (from a in genData.GeneralMasters where a.PKID == ePKID select a).Single();

                    qry.GenCode = Gencode.TrimStart();
                    qry.GenName = GenName.TrimStart();
                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    if (IsActive == "on" || IsActive == "Active")
                        qry.IsActive = true;
                    else if (IsActive == "off" || IsActive == "Inactive")
                        qry.IsActive = false;

                    genData.SaveChanges();

                    id = TableName + " modified Successfully.";
                }
                if (oper == "add")
                {

                    decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                    var rslt = (from a in genData.GeneralMasters where a.GenName == GenName && a.GenFKID == QrygeneralFKID select a).ToList();
                    if (rslt.Count() > 0)
                        id = GenName + " Name already Exists";
                    else
                    {
                        GeneralMaster gm = new GeneralMaster();
                        gm.GenFKID = Convert.ToDecimal(fkId);//24;
                        gm.GenCode = Gencode.TrimStart();
                        gm.GenName = GenName.TrimStart();
                        gm.IsActive = true;
                        gm.CreatedDate = DateTime.Now;
                        gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                        genData.GeneralMasters.Add(gm);
                        genData.SaveChanges();

                        id = TableName + " added successfully.";
                    }
                }
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in genData.GeneralMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    genData.SaveChanges();
                    id = delete_quote.GenName + " deleted successfully.";
                }

                return Json(id);
            }
       

        #endregion


        //StrategicPlanningMaster

        public JsonResult GridDataStrategicPlanningMaster(GridQueryModel gridQueryModel)
        {
            try
            {


                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper(); 
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                //var query = (from a in genData.usp_Get_CampTypeMaster() select a).ToList();
                //var count = query.Count();
                //var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                var query = (from a in genData.USP_GET_STRATEGICPLANNINGMASTER() orderby a.FormName select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "FormName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "FormName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.FormName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "FormName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.FormName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public JsonResult AddStrategicPlanningMaster(string FormName, string IsActive, string id)
        {           
            try
            {

                var rslt = (from a in genData.StrategicPlanningMasters where a.FormName == FormName select a).ToList();
                if (rslt.Count() > 0)
                    id = "StrategicPlanning Master already Exists";
                else
                {

                    StrategicPlanningMaster gm = new StrategicPlanningMaster();
                    gm.FormName = FormName.TrimStart();
                    if (IsActive == "Active")
                        gm.IsActive = true;
                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    genData.StrategicPlanningMasters.Add(gm);
                    genData.SaveChanges();

                    id = "StrategicPlanning Master added successfully";
                }
                return Json(id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult EditStrategicPlanningMaster(string id, string oper, string PKID, string FormName, string IsActive)
        {
            try { 
            if (oper == "edit")
            {
                int ePKID = Convert.ToInt32(PKID);
                var qry = (from a in genData.StrategicPlanningMasters where a.PKID == ePKID select a).Single();

                qry.FormName = FormName.TrimStart();
                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                //if (qry.IsActive == false)
                // qry.IsActive = Convert.ToBoolean(IsActive);
                if (IsActive == "Active")
                    qry.IsActive = true;

                if (IsActive == "Passive")
                    qry.IsActive = false;

                genData.SaveChanges();

                id = "StrategicPlanning Master modified Successfully.";
            }
            
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in genData.StrategicPlanningMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                genData.SaveChanges();

                id = "StrategicPlanning Master deleted successfully";
            }

            return Json(id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        // TailoredExecutionMaster      

        public JsonResult GridDataTailoredExecutionMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper(); 
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GET_TAILOREDEXECUTIONMASTER() orderby a.FormName select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "FormName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "FormName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.FormName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "FormName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.FormName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public JsonResult AddTailoredExecutionMaster(string FormName, string IsActive, string id)
        {
            try
            {

                var rslt = (from a in genData.TailoredExecutionMasters where a.FormName == FormName select a).ToList();
                if (rslt.Count() > 0)
                    id = "TailoredExecutionMaster already Exists";
                else
                {

                    TailoredExecutionMaster gm = new TailoredExecutionMaster();
                    gm.FormName = FormName.TrimStart();
                    if (IsActive == "Active")
                        gm.IsActive = true;
                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    genData.TailoredExecutionMasters.Add(gm);
                    genData.SaveChanges();

                    id = "TailoredExecutionMaster added successfully";
                }
                return Json(id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult EditTailoredExecutionMaster(string id, string oper, string PKID, string FormName, string IsActive)
        {
            try
            {
            if (oper == "edit")
            {
                int ePKID = Convert.ToInt32(PKID);
                var qry = (from a in genData.TailoredExecutionMasters where a.PKID == ePKID select a).Single();

                qry.FormName = FormName.TrimStart();
                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                //if (qry.IsActive == false)
                // qry.IsActive = Convert.ToBoolean(IsActive);
                if (IsActive == "Active")
                    qry.IsActive = true;

                if (IsActive == "Passive")
                    qry.IsActive = false;

                genData.SaveChanges();

                id = "TailoredExecution Master modified Successfully.";
            }
            
             if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in genData.TailoredExecutionMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                genData.SaveChanges();

                id = "TailoredExecution Master deleted successfully";
            }

            return Json(id);
             }
            catch (Exception ex)
            {
                id = ex.Message;
                return Json(id);
            }
        }

        //EvaluationMaster

        public JsonResult GridDataEvaluationMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper(); 
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GET_EVALUATIONMASTER() orderby a.FormName select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "FormName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.FormName != null && sq.FormName.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "FormName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.FormName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "FormName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.FormName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public JsonResult AddEvaluationMaster(string FormName, string IsActive, string id)
        {
            try
            {

                var rslt = (from a in genData.EvaluationMasters where a.FormName == FormName select a).ToList();
                if (rslt.Count() > 0)
                    id = "Evaluation Master already Exists";
                else
                {

                    EvaluationMaster gm = new EvaluationMaster();
                    gm.FormName = FormName.TrimStart();
                    if (IsActive == "Active")
                        gm.IsActive = true;
                    if (IsActive == "Passive")
                        gm.IsActive = false;
                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    genData.EvaluationMasters.Add(gm);
                    genData.SaveChanges();

                    id = "Evaluation Master added successfully";
                }
                return Json(id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult EditEvaluationMaster(string id, string oper, string PKID, string FormName, string IsActive)
        {
            try
            { 
            if (oper == "edit")
            {
                int ePKID = Convert.ToInt32(PKID);
                var qry = (from a in genData.EvaluationMasters where a.PKID == ePKID select a).Single();

                qry.FormName = FormName.TrimStart();
                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                if (IsActive == "Active")
                    qry.IsActive = true;

                if (IsActive == "Passive")
                    qry.IsActive = false;

                genData.SaveChanges();

                id = "Evaluation Master modified Successfully.";
            }
            
                if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in genData.EvaluationMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                genData.SaveChanges();
                id = "Evaluation Master deleted successfully";
            }

            return Json(id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        //QualificationMaster

        public JsonResult GridDataQualificationMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString== null ? "" : gridQueryModel.searchString.ToUpper(); 
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GET_QUALIFICATIONMASTER() orderby a.QualificationName select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.QualificationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "QualificationName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.QualificationName != null && sq.QualificationName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.QualificationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.QualificationName != null && sq.QualificationName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.QualificationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.QualificationName != null && sq.QualificationName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.QualificationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.QualificationName != null && sq.QualificationName.ToUpper().Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.QualificationName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "QualificationName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.QualificationName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "QualificationName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.QualificationName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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


        public JsonResult AddQualificationMaster(string QualificationName, string IsActive, string id)
        {
            //string id = "";
            try
            {
                
                var rslt = (from a in genData.QualificationMasters where a.QualificationName == QualificationName select a).ToList();
                if (rslt.Count() > 0)
                    id = "Qualification Master already Exists";
                else
                {

                    QualificationMaster gm = new QualificationMaster();
                    gm.QualificationName = QualificationName.TrimStart();
                    if (IsActive == "Active")
                        gm.IsActive = true;
                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    genData.QualificationMasters.Add(gm);
                    genData.SaveChanges();
                    id = "Qualification Master added successfully";
                }
                return Json(id);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult EditQualificationMaster(string id, string oper, string PKID, string QualificationName, string IsActive)
        {
            try
            {
            if (oper == "edit")
            {
                int ePKID = Convert.ToInt32(PKID);
                var qry = (from a in genData.QualificationMasters where a.PKID == ePKID select a).Single();

                qry.QualificationName = QualificationName.TrimStart();
                qry.ModifiedDate = DateTime.Now;
                qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                if (IsActive == "Active")
                    qry.IsActive = true;

                if (IsActive == "Passive")
                    qry.IsActive = false;

                genData.SaveChanges();

                id = "Qualification Master modified Successfully.";
            }
           
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in genData.QualificationMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                genData.SaveChanges();
                id = "Qualification Master deleted successfully";
            }

            return Json(id);
             }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult JsonValidate(string val, string FKID)
        {
            string rslt = string.Empty;

            var query =  genData.GetGencodeCount(val.ToString(), Convert.ToInt32(FKID));
            //var query =(from a in genData.GetGencodeCount((val.ToString(), Convert.ToInt32(FKID)) select  ).Single();             
            var count = query.Count();
            
            return Json(new
            {
                rslt = count

            }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult CamType(string generalFKID)
        {            
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "CampTypeMaster";
            ViewData["TableName"] = "CampTypeMaster";
            return View();

        }
        public ActionResult ObjectiveMaster(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "ObjectiveMaster";
            ViewData["TableName"] = "ObjectiveMaster";
            return View();
        }
        public ActionResult ConfTypeMaster(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "ConfTypeMaster";
            ViewData["TableName"] = "ConfTypeMaster";
            return View();
        }

        public ActionResult LeaveTypeMaster(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "LeaveTypeMaster";
            ViewData["TableName"] = "LeaveTypeMaster";
            return View();
        }

        public ActionResult UsefulLinks(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "UsefulLinks";
            ViewData["TableName"] = "UsefulLinks";
            return View();
        }
        public ActionResult CampObjective(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "CampObjective";
            ViewData["TableName"] = "CampObjective";
            return View();
        }
        public ActionResult CoreMeetingObjective(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "CoreMeetingObjective";
            ViewData["TableName"] = "CoreMeetingObjective";
            return View();
        }

        public ActionResult GroupMeetingObjective(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "GroupMeetingObjective";
            ViewData["TableName"] = "GroupMeetingObjective";
            return View();
        }
        public ActionResult StallObjective(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "StallObjective";
            ViewData["TableName"] = "StallObjective";
            return View();
        }

        public ActionResult SeminarObjective(string generalFKID)
        {
            ViewBag.FkId = generalFKID;
            Session["generalFKID"] = generalFKID;
            Session["TableType"] = "SeminarObjective";
            ViewData["TableName"] = "SeminarObjective";
            return View();
        }

        public ActionResult StrategicPlanningMaster()
        {
            Session["GeneralMaster"] = "StrategicPlanningMaster";
            return View();
        }


        public ActionResult TailoredExecutionMaster()
        {
            Session["GeneralMaster"] = "TailoredExecutionMaster";
            return View();
        }


        public ActionResult EvaluationMaster()
        {
            Session["GeneralMaster"] = "EvaluationMaster";
            return View();
        }

        public ActionResult QualificationMaster()
        {
            Session["GeneralMaster"] = "QualificationMaster";
            return View();
        }



        //export excel,pdf,csv
        public ActionResult ExportFormMasterToExcel()
        {
            var context = (from a in genData.usp_Get_CampTypeMaster() where a.GenFKID == Convert.ToDecimal(Session["generalFKID"].ToString()) select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.TableName == null ? "" :item.TableName,
                item.GenName == null ? "" :item.GenName,
                item.IsActive == null ? "" :item.IsActive
               
            }));
            return new ExcelResult(HeadersQuestions, ColunmTypesQuestions, data, Session["TableType"].ToString() + ".xlsx", "camptype");
        }
        private static readonly string[] HeadersQuestions = {
                "Table type", "Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesQuestions = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportFormMasterToPDF()
        {
            List<GeneralMasterTypes> userList = new List<GeneralMasterTypes>();
            var context = (from a in genData.usp_Get_CampTypeMaster() where a.GenFKID == Convert.ToDecimal(Session["generalFKID"].ToString()) select a).ToList();

            foreach (var item in context)
            {
                GeneralMasterTypes user = new GeneralMasterTypes();
                user.TableType = item.TableName == null ? "" : item.TableName;
                user.Name =   item.GenName == null ? "" : item.GenName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
                userList.Add(user);
            }
            var customerList = userList;
            pdf.ExportPDF(customerList, new string[] { "TableType", "Name", "Status" }, path);
            return File(path, "application/pdf", Session["TableType"].ToString() + ".pdf");
        }
        public ActionResult ExportFormMasterToCsv()
        {
            var model = (from a in genData.usp_Get_CampTypeMaster() where a.GenFKID == Convert.ToDecimal(Session["generalFKID"].ToString()) select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Table Type,Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record == null ? "" : record.TableName, record == null ? "" : record.GenName,record== null ? "" : record.IsActive

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


        //StrategicPlanningMaster Export --> Excel ,CSV,PDF


        public ActionResult ExportStrategicPlanningMasterToExcel()
        {
           var context = (from a in genData.USP_GET_STRATEGICPLANNINGMASTER() select a).OrderBy(x => x.FormName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                
                item.FormName == null ? "" :item.FormName,
                item.IsActive == null ? "" :item.IsActive
                
               
            }));
            return new ExcelResult(HeadersStrategicPlanningMaster, ColunmTypesStrategicPlanningMaster, data, Session["GeneralMaster"].ToString() + ".xlsx", "GeneralMaster");
        }
        private static readonly string[] HeadersStrategicPlanningMaster = {
                "Strategic Planning","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesStrategicPlanningMaster = {
             
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportStrategicPlanningMasterToPDF()
        {
            List<StrategicPlanningMasterType> userList = new List<StrategicPlanningMasterType>();
            var context = (from a in genData.USP_GET_STRATEGICPLANNINGMASTER() orderby a.FormName select a).ToList();

            foreach (var item in context)
            {
                StrategicPlanningMasterType user = new StrategicPlanningMasterType();
                user.StrategicPlanning = item.FormName == null ? "" : item.FormName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
                userList.Add(user);
            }
            var customerList = userList;



            pdf.ExportPDF(customerList, new string[] { "StrategicPlanning", "Status" }, path);
          
            return File(path, "application/pdf", "StrategicPlanning.pdf");
        }
        public ActionResult ExportStrategicPlanningMasterToCsv()
        {
            var model = (from a in genData.USP_GET_STRATEGICPLANNINGMASTER() orderby a.FormName select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Strategic Planning,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record == null ? "" : record.FormName, record== null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", Session["GeneralMaster"].ToString() + ".txt");
        }
        public class StrategicPlanningMasterType
        {
            public string PKID { get; set; }
            public string StrategicPlanning { get; set; }
            public string Status { get; set; }
        }

        //TailoredExecutionMaster Export --> Excel ,CSV,PDF

        public ActionResult ExportTailoredExecutionMasterToExcel()
        {
            var context = (from a in genData.USP_GET_TAILOREDEXECUTIONMASTER() orderby a.FormName select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.FormName == null ? "" :item.FormName,
                item.IsActive == null ? "" :item.IsActive
               
            }));
            return new ExcelResult(HeadersTailoredExecutionMasterType, ColunmTypesTailoredExecutionMasterType, data, Session["GeneralMaster"].ToString() + ".xlsx", "GeneralMaster");
        }
        private static readonly string[] HeadersTailoredExecutionMasterType = {
               "Tailored Execution","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesTailoredExecutionMasterType = {
                 DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportTailoredExecutionMasterToPDF()
        {
            List<TailoredExecutionMasterType> userList = new List<TailoredExecutionMasterType>();
            var context = (from a in genData.USP_GET_TAILOREDEXECUTIONMASTER() orderby a.FormName select a).ToList();

            foreach (var item in context)
            {
                TailoredExecutionMasterType user = new TailoredExecutionMasterType();
                user.TailoredExecution = item.FormName == null ? "" : item.FormName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
                userList.Add(user);
            }
            var customerList = userList;
                     

            pdf.ExportPDF(customerList, new string[] { "TailoredExecution", "Status" }, path);
            return File(path, "application/pdf", Session["GeneralMaster"].ToString() + ".pdf");
        }
        public ActionResult ExportTailoredExecutionMasterToCsv()
        {
            var model = (from a in genData.USP_GET_TAILOREDEXECUTIONMASTER() orderby a.FormName select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Tailored Execution,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record == null ? "" : record.FormName, record == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", Session["GeneralMaster"].ToString() + ".txt");
        }
        public class TailoredExecutionMasterType
        {
            public string PKID { get; set; }
            public string TailoredExecution { get; set; }
            public string Status { get; set; }
        }

        //EvaluationMaster Export --> Excel ,CSV,PDF

        public ActionResult ExportEvaluationMasterToExcel()
        {
            var context = (from a in genData.USP_GET_EVALUATIONMASTER() orderby a.FormName select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                 item.FormName == null ? "" :item.FormName,
                item.IsActive == null ? "" :item.IsActive
               
            }));
            return new ExcelResult(HeadersEvaluationMasterType, ColunmTypesEvaluationMasterType, data, Session["GeneralMaster"].ToString() + ".xlsx", "GeneralMaster");
        }
        private static readonly string[] HeadersEvaluationMasterType = {
                "Evaluation","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesEvaluationMasterType = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportEvaluationMasterToPDF()
        {
            List<EvaluationMasterType> userList = new List<EvaluationMasterType>();
            var context = (from a in genData.USP_GET_EVALUATIONMASTER() orderby a.FormName select a).ToList();

            foreach (var item in context)
            {
                EvaluationMasterType user = new EvaluationMasterType();
                user.Evaluation = item.FormName == null ? "" : item.FormName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
                userList.Add(user);
            }
            var customerList = userList;
                       

            pdf.ExportPDF(customerList, new string[] { "Evaluation", "Status" }, path);
            return File(path, "application/pdf", Session["GeneralMaster"].ToString() + ".pdf");
        }
        public ActionResult ExportEvaluationMasterToCsv()
        {
            var model = (from a in genData.USP_GET_EVALUATIONMASTER() orderby a.FormName select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Evaluation,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record == null ? "" : record.FormName, record == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", Session["GeneralMaster"].ToString() + ".txt");
        }
        public class EvaluationMasterType
        {
            public string PKID { get; set; }
            public string Evaluation { get; set; }
            public string Status { get; set; }
        }


        //QualificationMaster Export --> Excel ,CSV,PDF

        public ActionResult ExportQualificationMasterToExcel()
        {
            var context = (from a in genData.USP_GET_QUALIFICATIONMASTER() orderby a.QualificationName select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.QualificationName == null ? "" :item.QualificationName,
                item.IsActive == null ? "" :item.IsActive
               
            }));
            return new ExcelResult(HeadersQualificationMasterType, ColunmTypesQualificationMasterType, data, Session["GeneralMaster"].ToString() + ".xlsx", "GeneralMaster");
        }
        private static readonly string[] HeadersQualificationMasterType = {
                "Qualification Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesQualificationMasterType = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportQualificationMasterToPDF()
        {
            List<QualificationMasterType> userList = new List<QualificationMasterType>();
            var context = (from a in genData.USP_GET_QUALIFICATIONMASTER() orderby a.QualificationName select a).ToList();

            foreach (var item in context)
            {
                QualificationMasterType user = new QualificationMasterType();
                user.QualificationName = item.QualificationName == null ? "" : item.QualificationName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
                userList.Add(user);
            }
            var customerList = userList;
                     
            pdf.ExportPDF(customerList, new string[] { "QualificationName", "Status" }, path);
            return File(path, "application/pdf", Session["GeneralMaster"].ToString() + ".pdf");




        }
        public ActionResult ExportQualificationMasterToCsv()
        {
            var model = (from a in genData.USP_GET_QUALIFICATIONMASTER() orderby a.QualificationName select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Qualification Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record == null ? "" : record.QualificationName, record == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", Session["GeneralMaster"].ToString() + ".txt");
        }
        public class QualificationMasterType
        {
            public string PKID { get; set; }
            public string QualificationName { get; set; }
            public string Status { get; set; }
        }









        //Raj Kumar 

        public ActionResult FAQ()
        {
            return View();
        }



        public JsonResult GridDataFAQ(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString;
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.FAQS1View select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "GenName") || (searchField == "IsActive"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.GenName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.GenName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.GenName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.IsActive.EndsWith(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.GenName.Contains(searchString) || sq.IsActive.Contains(searchString) select sq).ToList();
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


        public JsonResult AddFAQ(string id, string oper, string PKID, string GenName, string Question, string Answer, string IsActive)
        {
            try
            {

                decimal categoyFkid;
                bool eIsActive = false;
                categoyFkid = Convert.ToInt32(GenName.TrimStart());

                if (IsActive == "Active")
                    eIsActive = true;

                if (IsActive == "Passive")
                    eIsActive = false;



                //var rslt=  genData.AddFAQS(categoyFkid, Question, Answer, eIsActive, Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                id = "FAQ Master Added Successfully.";
                return Json("FAQ Master Added Successfully.");
            }
            catch (Exception m)
            {
                return Json(m.Message);
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
                categoyFkid = Convert.ToInt32(GenName.TrimStart());

                if (IsActive == "Active")
                    eIsActive = true;

                if (IsActive == "Passive")
                    eIsActive = false;

                ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                genData.EditFAQS(ePKID, categoyFkid, Question, Answer, eIsActive, ModifiedBy);

                id = "FAQ Master modified Successfully.";
            }

            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                genData.DeleteFAQS(quote);
                id = "FAQ Master deleted successfully";
            }

            return Json(id);
        }


        public ActionResult FAQCategory()
        {
            var query = (from e in genData.GetCategoryByID() select new { e.CategoryID, e.CategoryName }).ToDictionary(f => Convert.ToInt32(f.CategoryID), f => f.CategoryName.ToString());
            return PartialView("LoadYear", query);
        }




        //export FAQ excel,pdf,csv
        public ActionResult ExportFAQToExcel()
        {
            var context = (from a in genData.FAQS1View select a).OrderBy(o => o.GenName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
             
                item.GenName == null ? "" : item.GenName.ToString(),
                item.Question == null ? "" :item.Question,
                item.Answer == null ? "" :item.Answer,
                item.IsActive==null?"":item.IsActive
               
            }));
            return new ExcelResult(HeadersFAQQuestions, ColunmTypesFAQQuestions, data, "FAQ.xlsx", "FAQ Sheet");
        }
        private static readonly string[] HeadersFAQQuestions = {
                "Category", "Question","Answer","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesFAQQuestions = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportFAQToPDF()
        {
            List<FAQCls> userList = new List<FAQCls>();
            var context = (from a in genData.FAQS1View select a).OrderBy(o => o.GenName).ToList();

            foreach (var item in context)
            {
                FAQCls user = new FAQCls();

                user.Category = item.GenName == null ? "" : item.GenName.ToString();
                user.Question = item.Question == null ? "" : item.Question;
                user.Answer = item.Answer == null ? "" : item.Answer;
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
                         e.Category,e.Question,e.Answer,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "Category", "Question", "Answer", "Status" }, path);
            return File(path, "application/pdf", "FAQ.pdf");
        }
        public ActionResult ExportFAQToCsv()
        {
            var model = (from a in genData.FAQS1View select a).OrderBy(o => o.GenName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Category,Question Type,Answer,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.GenName == null ? "" : record.GenName, record.Question == null ? "" : record.Question, record.Answer == null ? "" : record.Answer, record.IsActive == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "FAQ.txt");
        }
        public class FAQCls
        {

            public string Category { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public string Status { get; set; }
        }




    }
}
