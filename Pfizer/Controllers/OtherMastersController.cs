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
using System.Web.Script.Serialization;

namespace Pfizer.Controllers
{
    public class OtherMastersController : Controller
    {
        private pfizerEntities OtherMasters = new pfizerEntities();
        ExportPdf pdf = new ExportPdf();
        string path = ConfigurationManager.AppSettings["PDFfilePath"].ToString();

      /* Question Master Starts */

        public ActionResult QuestionMaster()
        {
            

            return View();
        }

        public JsonResult QuestionGridData(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var result = OtherMasters.QuestionMasters.Select(x => new
                {
                    
                    PKID = x.PKID,
                    Question = x.Question,
                    IsActive = x.IsActive == true ? "Active" : "In Active",
                    
                }).OrderBy(x=>x.Question).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "Question")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where sq.Question != null && sq.Question.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where sq.Question != null && sq.Question.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Question).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where sq.Question != null && sq.Question.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
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


        public JsonResult EditQuestionMaster(string id, string oper, string PKID, string Question, string IsActive)
        {

            if (oper == "edit")
            {
                int ePKID = Convert.ToInt32(PKID);
                var rslt = (from a in OtherMasters.QuestionMasters where a.PKID == ePKID select a).Single();

                rslt.Question = Question.TrimStart();
                rslt.ModifiedDate = DateTime.Now;
                rslt.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                if (IsActive == "on")
                    rslt.IsActive = true;
                if (IsActive == "off")
                    rslt.IsActive = false;
                OtherMasters.SaveChanges();
                id = "Question modified successfully";
            }
            if (oper == "add")
            {

                var rslt = (from a in OtherMasters.QuestionMasters where a.Question==Question select a).ToList();
                if (rslt.Count() > 0)
                    id = "QuestionMaster already Exists";
                else
                {
                    QuestionMaster qm = new PfizerEntity.QuestionMaster();
                    qm.Question = Question.TrimStart();
                    if (IsActive == "on")
                        qm.IsActive = true;
                    qm.CreatedDate = DateTime.Now;
                    qm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    OtherMasters.QuestionMasters.Add(qm);
                    OtherMasters.SaveChanges();
                    id = "QuestionMaster added successfully";
                }

            }
            if (oper == "del")
            {
                var quote = Convert.ToInt32(id);
                var delete_quote = (from a in OtherMasters.QuestionMasters where a.PKID == quote select a).Single();
                delete_quote.IsActive = false;
                delete_quote.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                OtherMasters.SaveChanges();
                id = "Question deleted successfully";
            }

            return Json(id);
        } 

        //  Export --> Excel ,CSV,PDF 

        public ActionResult ExportQuestionMasterToExcel()
        {
            var context = OtherMasters.QuestionMasters.Select(x => new
            {
 
                Question = x.Question,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.Question).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                  item.Question == null ? "" :item.Question,
                  item.IsActive == null ? "" :item.IsActive,
                
               
            }));
            return new ExcelResult(HeadersHolidayMaster, ColunmTypesHolidayMaster, data, "QuestionMaster.xlsx", "Question Master");
        }
        private static readonly string[] HeadersHolidayMaster = {
               "Question","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesHolidayMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportQuestionMasterToPDF()
        {
            try
            {
                List<QuestionMasterPDF> userList = new List<QuestionMasterPDF>();
                var context = OtherMasters.QuestionMasters.Select(x => new
                {

                    Question = x.Question,
                    IsActive = x.IsActive == true ? "Active" : "In Active",
                }).OrderBy(x => x.Question).ToList();

                foreach (var item in context)
                {
                    QuestionMasterPDF user = new QuestionMasterPDF();

                    user.Question = item.Question == null ? "" : item.Question;
                    user.Status = item.IsActive == null ? "" : item.IsActive;
                    userList.Add(user);
                }
                var customerList = userList;

               
                   pdf.ExportPDF(customerList, new string[] { "Question", "Status" }, path);
                return File(path, "application/pdf", "QuestionMaster.pdf");
            }
            catch {
                return View();
            }
        }
        public ActionResult ExportQuestionMasterToCsv()
        {
            var model = OtherMasters.QuestionMasters.Select(x => new
            {

                Question = x.Question,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.Question).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Question,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record == null ? "" : record.Question, record == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "Question.txt");
        }
        public class QuestionMasterPDF
        {

            public string Question { get; set; }
            public string Status { get; set; }
        }

        //    end



        /*QuestionMaster Ends */


        /* SpecializationMaster Starts */

        public ActionResult SpecializationMaster()
        {

            return View();
        }

        public JsonResult SpecializeGridData(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;


                var result = (from a in OtherMasters.SpecializationTeamlinkMasters
                              join b in OtherMasters.SpecializationMasters on a.SpecializationFKID equals b.PKID
                              join c in OtherMasters.TeamMasters on a.TeamFKID equals c.PKID
                              orderby b.Specialization
                             
                              select new
                              {
                                  a.PKID,
                                  IsActive = a.IsActive == true ? "Active" : "In Active",
                                  b.Specialization,
                                  c.TeamName
                              }).ToList();
                var count = result.Count();
                var pageData = result.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
               
                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "Specialization") || (searchField == "TeamName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where (sq.Specialization != null && sq.Specialization.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "Specialization") || (sq.TeamName != null && sq.TeamName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "TeamName") select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where (sq.Specialization != null && sq.Specialization.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "Specialization") || (sq.TeamName != null && sq.TeamName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "TeamName") select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where (sq.Specialization != null && sq.Specialization.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "Specialization") || (sq.TeamName != null && sq.TeamName.ToUpper().EndsWith(searchString) && searchField == "TeamName") select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in result where (sq.Specialization != null && sq.Specialization.ToUpper().ToString().Contains(searchString) && searchField == "Specialization") || (sq.TeamName != null && sq.TeamName.ToUpper().Contains(searchString) && searchField == "TeamName") select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                    
                }
                else
                {
                    count = result.Count();

                   if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.Specialization ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.Specialization descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    else if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.TeamName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.TeamName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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




        public string GetSpecialMasterTeam(string TeamName)
        {
            string[] TeamId = TeamName.Split(',');
            StringBuilder sb = new StringBuilder();
            sb.Append("<row>");
            for (int i = 0; i < TeamId.Count(); i++)
            {
                sb.Append("<dp TeamFKID='" + TeamId[i]);
                sb.Append("'/>");

            }

            sb.Append("</row>");
            return sb.ToString();
        }

        public JsonResult EditSpecializationMaster(string id, string oper, string PKID, string Specialization, string TeamName, string IsActive)
        {
            try
            {
                decimal USERFKID = Convert.ToDecimal(Session["USER_FKID"].ToString());

                string xml = "";

                if (oper == "edit")
                {
                    if (IsActive == "Active")  {
                        IsActive = "true";
                    }
                    else { 
                        IsActive = "false";
                    }
                    xml = GetSpecialMasterTeam(TeamName);
                    OtherMasters.EditSpecializationMaster2(Convert.ToDecimal(id), xml, Specialization, Convert.ToBoolean(IsActive), USERFKID);
                    id = "Specialization Master Edited Successfully";
                }
                if (oper == "add")
                {

                    var rslt = (from a in OtherMasters.SpecializationMasters where a.Specialization == Specialization select a).ToList();
                    if (rslt.Count() > 0)
                        id = Specialization + " Name already Exists";
                    else
                    {
                        if (IsActive == "Active")
                        {
                            IsActive = "true";
                        }
                        else
                        {
                            IsActive = "false";
                        }
                        xml = GetSpecialMasterTeam(TeamName);
                        OtherMasters.AddSpecializationTeamlinkMaster(Specialization, xml, Convert.ToBoolean(IsActive), USERFKID);
                        id = "Specialization Master Added Successfully";
                    }

                }
                if (oper == "del")
                {
                    OtherMasters.DeleteSpecializationTeamlinkMaster(Convert.ToDecimal(id), USERFKID);
                    id = "Specialization Master Deleted Successfully";

                }
            }
            catch (Exception ex)
            {

            }

            return Json(id);
        }

        public ActionResult LoadSpecialization(string type, string PKID)
        {
               ViewBag.type = type;
               if ((PKID == null) || (PKID == "undefined"))
            { 
                var query = (from a in OtherMasters.GetSplTeamByXml() select new { a.Teamid, a.TName }).ToDictionary(f => Convert.ToInt32(f.Teamid), f => f.TName.ToString());
                return PartialView("PVFirstLoadspecial", query);
            }
            else
            {
                decimal TeamID;
                var query = (from a in OtherMasters.GetSpecializationMasterByPKID(Convert.ToDecimal(PKID)) select new { a.Teamid }).FirstOrDefault();
                TeamID = query.Teamid;
                var query1 = (from a in OtherMasters.GetSplSelectedTeamByXml(TeamID) where a.TName!="" select new { a.Teamid, a.TName }).ToDictionary(f => Convert.ToInt32(f.Teamid), f => f.TName.ToString());
                return PartialView("PVLoadspecial", query1);
                
            }
        }



        //  Export --> Excel ,CSV,PDF 

        public ActionResult ExportSpecializationMasterToExcel()
        {
            var context = (from a in OtherMasters.SpecializationTeamlinkMasters
                           join b in OtherMasters.SpecializationMasters on a.SpecializationFKID equals b.PKID
                           join c in OtherMasters.TeamMasters on a.TeamFKID equals c.PKID
                           orderby b.Specialization

                           select new
                           { 
                               IsActive = a.IsActive == true ? "Active" : "In Active",
                               b.Specialization,
                               c.TeamName
                           }).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                 item.Specialization == null ? "" :item.Specialization,
                 item.TeamName == null ? "" :item.TeamName,
                  item.IsActive == null ? "" :item.IsActive,
                
               
            }));
            return new ExcelResult(HeadersSpecializationMaster, ColunmTypesSpecializationMaster, data, "SpecializationMaster.xlsx", "Specialization Master");
        }
        private static readonly string[] HeadersSpecializationMaster = {
               "Specialization","TeamName","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesSpecializationMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportSpecializationMasterToPDF()
        {
            List<SpecializationMasterPDF> userList = new List<SpecializationMasterPDF>();
            var context = (from a in OtherMasters.SpecializationTeamlinkMasters
                           join b in OtherMasters.SpecializationMasters on a.SpecializationFKID equals b.PKID
                           join c in OtherMasters.TeamMasters on a.TeamFKID equals c.PKID
                           orderby b.Specialization

                           select new
                           {
                               IsActive = a.IsActive == true ? "Active" : "In Active",
                               b.Specialization,
                               c.TeamName
                           }).ToList();

            foreach (var item in context)
            {
                SpecializationMasterPDF user = new SpecializationMasterPDF();

                user.Specialization = item.Specialization == null ? "" : item.Specialization;
                user.Team = item.TeamName == null ? "" : item.TeamName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
                userList.Add(user);
            }
            var customerList = userList;

           
            pdf.ExportPDF(customerList, new string[] { "Specialization", "Team", "Status" }, path);
            return File(path, "application/pdf", "SpecializationMaster.pdf");
        }
        public ActionResult ExportSpecializationMasterToCsv()
        {
            var model = (from a in OtherMasters.SpecializationTeamlinkMasters
                         join b in OtherMasters.SpecializationMasters on a.SpecializationFKID equals b.PKID
                         join c in OtherMasters.TeamMasters on a.TeamFKID equals c.PKID
                         orderby b.Specialization
                         select new
                         {
                             IsActive = a.IsActive == true ? "Active" : "In Active",
                             b.Specialization,
                             c.TeamName
                         }).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Specialization,Team,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record == null ? "" : record.Specialization, record == null ? "" : record.TeamName,record == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "SpecializationMaster.txt");
        }
        public class SpecializationMasterPDF
        {
            public string Specialization { get; set; }
            public string Team { get; set; }
            public string Status { get; set; }
        }

        //    end



        /*SpecializationMaster Ends */



        /* Rating  Master starts */

        public ActionResult RatingMaster()
        {

            return View();
        }

        public JsonResult RatingMasterGrid(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var result = OtherMasters.RatingMasters.Select(x => new
                { 
                    PKID = x.PKID,
                    x.RatingCode,
                    x.RatingName,
                    IsActive = x.IsActive == true ? "Active" : "In Active",
                }).OrderBy(x => x.RatingName).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.RatingName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);




                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "RatingName")
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where sq.RatingName != null && sq.RatingName.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RatingName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where sq.RatingName != null && sq.RatingName.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RatingName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where sq.RatingName != null && sq.RatingName.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RatingName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in result where sq.RatingName != null && sq.RatingName.ToUpper().ToString().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.RatingName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = result.Count();
                    
                    if (gridQueryModel.sidx == "RatingCode" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.RatingCode ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "RatingCode" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.RatingCode descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "RatingName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.RatingName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "RatingName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.RatingName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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

        public JsonResult EditRatingMaster(string id, string oper, string PKID, string RatingName,string RatingCode, string IsActive)
        {
            try
            {

                if (oper == "edit")
                {
                    int ePKID = Convert.ToInt32(PKID);

                    var rslt = (from a in OtherMasters.RatingMasters where a.PKID == ePKID select a).Single();

                    rslt.RatingName = RatingName;
                    rslt.RatingCode = RatingCode;
                    rslt.ModifiedDate = DateTime.Now;
                    rslt.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    if (IsActive == "on")
                        rslt.IsActive = true;
                    else if (IsActive == "off")
                        rslt.IsActive = false;


                    OtherMasters.SaveChanges();
                    id = "Rating Master modified Successfully.";

                }
                if (oper == "add")
                {

                    var rslt = (from a in OtherMasters.RatingMasters where a.RatingName==RatingName select a).ToList();
                    if (rslt.Count() > 0)
                        id = "Rating Master already Exists";
                    else
                    {
                        RatingMaster ratingMast = new PfizerEntity.RatingMaster();
                        ratingMast.RatingName = RatingName;
                        ratingMast.RatingCode = RatingCode;
                        ratingMast.IsActive = true; 
                        ratingMast.CreatedDate = DateTime.Now;
                        ratingMast.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                        OtherMasters.RatingMasters.Add(ratingMast);
                        OtherMasters.SaveChanges();
                        id = "Rating Master added successfully";

                    }

                }
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in OtherMasters.RatingMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    delete_quote.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    OtherMasters.SaveChanges();
                    id = "Rating Master deleted successfully";
                }
                return Json(id);
            }
            catch
            {
                return Json(id);
            }


        }




        //  Export --> Excel ,CSV,PDF 

        public ActionResult ExportRatingMasterToExcel()
        {
            var context = OtherMasters.RatingMasters.Select(x => new
            {
                PKID = x.PKID,
                x.RatingCode,
                x.RatingName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.RatingName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.RatingCode == null ? "" :item.RatingCode,
                 item.RatingName == null ? "" :item.RatingName,
                  item.IsActive == null ? "" :item.IsActive,

                
               
            }));
            return new ExcelResult(HeadersRatingMaster, ColunmTypesRatingMaster, data, "RatingMaster.xlsx", "Rating Master");
        }
        private static readonly string[] HeadersRatingMaster = {
               "Rating Code","Rating Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesRatingMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportRatingMasterToPDF()
        {
            List<RatingMasterPDF> userList = new List<RatingMasterPDF>();
            var context = OtherMasters.RatingMasters.Select(x => new
            {
                PKID = x.PKID,
                x.RatingCode,
                x.RatingName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.RatingName).ToList();
          
            foreach (var item in context)
            {
                RatingMasterPDF user = new RatingMasterPDF();
                user.RatingCode = item.RatingCode == null ? "" : item.RatingCode;
                user.RatingName = item.RatingName == null ? "" : item.RatingName;
                user.Status = item.IsActive == null ? "" : item.IsActive;
                userList.Add(user);
            }
            var customerList = userList;

            
            pdf.ExportPDF(customerList, new string[] { "RatingCode", "RatingName", "Status" }, path);
            return File(path, "application/pdf", "RatingMaster.pdf");
        }
        public ActionResult ExportRatingMasterToCsv()
        {
            var model = OtherMasters.RatingMasters.Select(x => new
            {
                PKID = x.PKID,
                x.RatingCode,
                x.RatingName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.RatingName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("RatingCode,RatingName,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record == null ? "" : record.RatingCode, record == null ? "" : record.RatingName,record == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "RatingMaster.txt");
        }
        public class RatingMasterPDF
        {
            public string RatingCode { get; set; }
            public string RatingName { get; set; }
            public string Status { get; set; }
        }

        //    end


        /* Rating  Master Ends */


        /*RBM FTM LinkMaster starts*/

        public ActionResult RBMFTMLinkMaster()
        {

            return View();
        }

        public JsonResult RBMGridData(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var result = OtherMasters.usp_GetRBMMaster().Select(x => new
                {
                    PKID = x.PKID,
                    FTM_Name= x.FTMFirstName+" "+x.FTMMiddleName+" "+x.FTMLastName,
                    RBM_Name=x.RBMFirstName+" "+x.RBMMiddleName+" "+x.RBMLastName,
                    IsActive = x.IsActive == true ? "Active" : "In Active",
                }).OrderBy(x => x.FTM_Name).ToList();

                var count = result.Count();
                var pageData = result.OrderBy(x => x.FTM_Name).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "FTM_Name") || (searchField == "RBM_Name"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where (sq.FTM_Name != null && sq.FTM_Name.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "FTM_Name") || (sq.RBM_Name != null && sq.RBM_Name.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "RBM_Name") select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FTM_Name).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where (sq.FTM_Name != null && sq.FTM_Name.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "FTM_Name") || (sq.RBM_Name != null && sq.RBM_Name.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "RBM_Name") select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FTM_Name).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where (sq.FTM_Name != null && sq.FTM_Name.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "FTM_Name") || (sq.RBM_Name != null && sq.RBM_Name.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) && searchField == "RBM_Name") select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FTM_Name).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in result where (sq.FTM_Name != null && sq.FTM_Name.ToUpper().ToString().Contains(searchString) && searchField == "FTM_Name") || (sq.RBM_Name != null && sq.RBM_Name.ToUpper().Contains(searchString) && searchField == "RBM_Name") select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FTM_Name).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }

                else
                {
                    count = result.Count();

                    if (gridQueryModel.sidx == "FTM_Name" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.FTM_Name ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "FTM_Name" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.FTM_Name descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "RBM_Name" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.RBM_Name ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "RBM_Name" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.RBM_Name descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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


        public ActionResult LoadFTM()
        {
           
           var query = (from a in OtherMasters.GetFTMName() select new { a.FTMFKID, a.FTMName }).ToDictionary(f => Convert.ToInt32(f.FTMFKID), f => f.FTMName.ToString());  
            return PartialView("LoadYear", query);
        }


        public ActionResult LoadRBM()
        {
            dynamic  query = (from a in OtherMasters.GetRBMName() select new { a.RBMFKID, a.RBMName }).ToDictionary(f => Convert.ToInt32(f.RBMFKID), f => f.RBMName.ToString());
            return PartialView("LoadYear", query);
        }

        public JsonResult EditRBMFTMlink(string id,string PKID, string FTM_Name, string RBM_Name, string IsActive)
        {
            try
            {
                int a = 0;

                if (IsActive == "on")
                {
                    a = 1;
                }
                else
                {
                    a = 0;

                }
                   var m = OtherMasters.EditRBMFTMLinkMaster(Convert.ToDecimal(PKID), Convert.ToInt32(RBM_Name), Convert.ToInt32(FTM_Name), Convert.ToBoolean(a));
                   id = "RBMFTM LinkMaster Edited Successfully.";
                  return Json(id);
            }
                
            catch
            {
                id = "RBMFTM LinkMaster already Exists.";  
                return Json(id);
            }
        }

        public JsonResult AddRBMFTMlink(string id, string FTM_Name, string RBM_Name)
        {
            try
            {

                var rslt = OtherMasters.AddRBMFTMLinkMaster(Convert.ToInt32(RBM_Name), Convert.ToInt32(FTM_Name));
               id = "RBMFTM LinkMaster added Successfully.";
                return Json(id);
            }

            catch
            {
                id = "RBMFTM LinkMaster already Exists.";  
                return Json(id);
            }
        }

        public JsonResult DeleteRBMFTMlink(string id)
        {
            try
            {

                OtherMasters.DeleteRBMFTMLinkMaster(Convert.ToDecimal(id));
                id = "RBMFTM LinkMaster deleted successfully";
                return Json(id);
            }

            catch
            {
                id = "RBMFTM LinkMaster already Exists.";
                return Json(id);
            }
        }

        public JsonResult EditRBMGrid(string id, string oper, string PKID, string FTM_Name, string RBM_Name, string IsActive)
        {
            try
            {
                int a = 0;
                
                if (IsActive == "on")
                {
                    a = 1;
                }
                else
                {
                    a = 0;
                
                }
            
                if (oper == "edit")
                {
                    var m = OtherMasters.EditRBMFTMLinkMaster(Convert.ToDecimal(PKID), Convert.ToInt32(RBM_Name), Convert.ToInt32(FTM_Name),Convert.ToBoolean(a));
                    id = "RBMFTM LinkMaster Edited Successfully.";

                }
                else  if (oper == "add")
                {
                   var rslt= OtherMasters.AddRBMFTMLinkMaster(Convert.ToInt32(RBM_Name), Convert.ToInt32(FTM_Name));
                   
                    id = "RBMFTM LinkMaster added Successfully.";                    

                }
                else if (oper == "del")
                {

                    OtherMasters.DeleteRBMFTMLinkMaster(Convert.ToDecimal(id));
                    id = "RBMFTM LinkMaster deleted successfully";
                }
                return Json(id);
            }
            catch
            {
                id = "RBMFTM LinkMaster already Exists.";    
                return Json(id);
            }

            
        } 

        //  Export --> Excel ,CSV,PDF 

        public ActionResult ExportRBMFTMLinkMasterToExcel()
        {
            var context = OtherMasters.usp_GetRBMMaster().Select(x => new
            {
                PKID = x.PKID,
                FTM_Name = x.FTMFirstName + " " + x.FTMMiddleName + " " + x.FTMLastName,
                RBM_Name = x.RBMFirstName + " " + x.RBMMiddleName + " " + x.RBMLastName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.FTM_Name).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                 item.FTM_Name == null ? "" :item.FTM_Name,
                 item.RBM_Name == null ? "" :item.RBM_Name,
                  item.IsActive == null ? "" :item.IsActive,

            }));
            return new ExcelResult(HeadersRBMFTMLinkMaster, ColunmTypesRBMFTMLinkMaster, data, "RBMFTMLinkMaster.xlsx", "RBM FTM LinkMaster");
        }
        private static readonly string[] HeadersRBMFTMLinkMaster = {
               "FTM Name","RBM Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesRBMFTMLinkMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportRBMFTMLinkMasterToPDF()
        {
            List<RBMFTMLinkMasterPDF> userList = new List<RBMFTMLinkMasterPDF>();
            var context = OtherMasters.usp_GetRBMMaster().Select(x => new
            {
                PKID = x.PKID,
                FTM_Name = x.FTMFirstName + " " + x.FTMMiddleName + " " + x.FTMLastName,
                RBM_Name = x.RBMFirstName + " " + x.RBMMiddleName + " " + x.RBMLastName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.FTM_Name).ToList();

            foreach (var item in context)
            {
                RBMFTMLinkMasterPDF user = new RBMFTMLinkMasterPDF();

                user.FTMName = item.FTM_Name == null ? "" : item.FTM_Name;
                user.RBMName = item.RBM_Name == null ? "" : item.RBM_Name;
                user.Status = item.IsActive == null ? "" : item.IsActive;
                userList.Add(user);
            }
            var customerList = userList;

            
            pdf.ExportPDF(customerList, new string[] { "FTMName", "RBMName", "Status" }, path);
            return File(path, "application/pdf", "RBMFTMLinkMaster.pdf");
        }
        public ActionResult ExportRBMFTMLinkMasterToCsv()
        {
            var model = OtherMasters.usp_GetRBMMaster().Select(x => new
            {
                PKID = x.PKID,
                FTM_Name = x.FTMFirstName + " " + x.FTMMiddleName + " " + x.FTMLastName,
                RBM_Name = x.RBMFirstName + " " + x.RBMMiddleName + " " + x.RBMLastName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.FTM_Name).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("FTM Name,RBM Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record == null ? "" : record.FTM_Name, record == null ? "" : record.RBM_Name,record == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "RBMFTMLinkMaster.txt");
        }
        public class RBMFTMLinkMasterPDF
        {
            public string FTMName { get; set; }
            public string RBMName { get; set; }
            public string Status { get; set; }
        }

        //    end

        /*RBM FTM LinkMaster ends*/

        /* Input Master starts*/
        public ActionResult InputMaster()
        {

            return View();
        }

        public JsonResult InputMasterGridData(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var result = OtherMasters.uspGetInputMaster().Select(x => new
                {
                    PKID = x.PKID,
                    x.InputName,
                    x.GenName, 
                    IsActive = x.IsActive == true ? "Active" : "In Active",
                }).OrderBy(x => x.InputName).ToList();



                var count = result.Count();
                var pageData = result.OrderBy(x => x.InputName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);



                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "InputName") )
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in result where sq.InputName != null && sq.InputName.ToUpper().ToString().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.InputName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in result where sq.InputName != null && sq.InputName.ToUpper().ToString().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.InputName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in result where sq.InputName != null && sq.InputName.ToUpper().ToString().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.InputName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in result where sq.InputName != null && sq.InputName.ToUpper().ToString().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.InputName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = result.Count();

                    if (gridQueryModel.sidx == "InputName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.InputName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "InputName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.InputName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in result orderby cust.GenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in result orderby cust.GenName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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


        public ActionResult LoadInputType()
        {
            var query = (from a in OtherMasters.uspGetInputType() select new { a.PKID, a.GenName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.GenName.ToString());
            return PartialView("LoadYear", query);
        }

        public ActionResult LoadIBIS(string id)
        {
           if(id!="null")
           { 
            var query = OtherMasters.GetInputMasterbyID(Convert.ToDecimal(id)).Select(x => new { x.IBISCode }).ToDictionary(f => f.IBISCode.ToString(), f => f.IBISCode.ToString());
            return PartialView("PVLoadIBIS", query);
           }
            else
           {
               var query = (from a in OtherMasters.uspGetInputType() select new { a.PKID, a.GenName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.GenName.ToString());
               return PartialView("LoadYear", query);
           }
          
        }


        public JsonResult JsonLoadIBIS(string id)
        {
            string IBISCode = "";
            var query = OtherMasters.GetInputMasterbyID(Convert.ToDecimal(id)).Select(x => new { x.IBISCode }).FirstOrDefault();
            return Json(new
            {
                IBISCode = query.IBISCode == null ? string.Empty : query.IBISCode, 

            }, JsonRequestBehavior.AllowGet);
            
        }



        public JsonResult EditInputMaster(string id, string oper, string PKID, string InputName, string GenName, string IsActive, string IBISCode, string IBISCode1)
        {
            try
            {
              

                if (oper == "edit")                
                {
                    if ((IsActive == "Active") || (IsActive == "on"))
                    {
                        IsActive = "1";
                    }
                    else
                    { 
                        IsActive = "0";
                    }
                   
                    OtherMasters.EditINPUTMaster(Convert.ToDecimal(PKID), Convert.ToDecimal(GenName), InputName, IBISCode, Convert.ToInt32(IsActive), Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                    id = "Input Master Edited Successfully.";

                }
                else if (oper == "add")
                {
                    var rslt = OtherMasters.AddiNPUTMaster(Convert.ToDecimal(GenName), InputName, IBISCode, Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));

                    id = "Input Master Added Successfully.";

                }
                else if (oper == "del")
                {

                    OtherMasters.DeleteINPUTMaster(Convert.ToDecimal(id), Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                    id = "Input Master Deleted Successfully.";
                }
                return Json(id);
            }
            catch
            {
                id = "Input Master Already Exists.";
                return Json(id);
            }


        }

        //  Export --> Excel ,CSV,PDF 

        public ActionResult ExportInputMasterToExcel()
        {
            var context = OtherMasters.uspGetInputMaster().Select(x => new
            {
                PKID = x.PKID,
                x.InputName,
                x.GenName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.InputName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {

                item.InputName == null ? "" :item.InputName,
                 item.GenName == null ? "" :item.GenName,
                  item.IsActive == null ? "" :item.IsActive,
               
               
            }));
            return new ExcelResult(HeadersInputMaster, ColunmTypesInputMaster, data, "InputMaster.xlsx", "Input Master");
        }
        private static readonly string[] HeadersInputMaster = {
               "Input Name","Input Type","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesInputMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };


        public ActionResult ExportInputMasterToPDF()
        {
            List<InputMasterPDF> userList = new List<InputMasterPDF>();
            var context = OtherMasters.uspGetInputMaster().Select(x => new
            {
                PKID = x.PKID,
                x.InputName,
                x.GenName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.InputName).ToList();

            foreach (var item in context)
            {
                InputMasterPDF user = new InputMasterPDF();

                user.InputName = item.InputName == null ? "" : item.InputName;
                user.InputType = item.GenName == null ? "" : item.GenName;
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
                            e.InputName,
                            e.InputType,
                            e.Status
                        }
                        })
                ).ToArray()
            };
            
            pdf.ExportPDF(customerList, new string[] { "InputName", "InputType", "Status" }, path);
            return File(path, "application/pdf", "InputMaster.pdf");
        }
        public ActionResult ExportInputMasterPDFToCsv()
        {
            var model = OtherMasters.uspGetInputMaster().Select(x => new
            {
                PKID = x.PKID,
                x.InputName,
                x.GenName,
                IsActive = x.IsActive == true ? "Active" : "In Active",
            }).OrderBy(x => x.InputName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Input Name,Input Type,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record == null ? "" : record.InputName, record == null ? "" : record.GenName,record == null ? "" : record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "InputMaster.txt");
        }
        public class InputMasterPDF 
        {
            public string InputName { get; set; }
            public string InputType { get; set; }
            public string Status { get; set; }
        }

        //    end
        /*Input Master ends*/


        //Baskar Start InputAcknowledgement

        public ActionResult InputAcknowledgement()
        {
            return View();
        }

        public JsonResult GridDataInputAcknowledgement(GridQueryModel gridQueryModel)
        {

            string Territory_FKID = Session["Territory_FKID"].ToString();
            decimal UserFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            var query = (from q in OtherMasters.GetPSOInputSample(UserFKID) select q).ToList();
            var pageData = query.OrderBy(x => x.AllocatedDate).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

            return Json(new
            {
                page = gridQueryModel.page,
                records = query.Count(),
                rows = pageData,
                total = Math.Ceiling((decimal)query.Count() / gridQueryModel.rows)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateGridDataInputAcknowledgement(string gridData)
        {
            string Territory_FKID = Session["Territory_FKID"].ToString();
            decimal UserFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            var serialize = new JavaScriptSerializer();
            var deserialize = serialize.Deserialize<List<InputAcknowledgement>>(gridData);

            StringBuilder sb = new StringBuilder();
            sb.Append("<row>");

            foreach (var item in deserialize)
            {
                if (Convert.ToUInt32(item.BalanceQty) <= Convert.ToUInt32(item.AckQty) + Convert.ToUInt32(item.DamageQty))
                {

                }
                else
                {
                    if (Convert.ToUInt32(item.AckQty) + Convert.ToUInt32(item.DamageQty) > 0)
                    {
                        var TotalAckqty = Convert.ToUInt32(item.AckQty);
                        var IBalanceQty = Convert.ToUInt32(item.DamageQty);

                        sb.Append("<dp IAMFKID='" + item.PKID);
                        sb.Append("'");
                        sb.Append(" AckQty='" + TotalAckqty);
                        sb.Append("'");
                        sb.Append(" DQty='" + IBalanceQty);
                        sb.Append("'/>");
                    }
                }
            }
            sb.Append("</row>");
            OtherMasters.InputMasterAck(sb.ToString(), UserFKID, Convert.ToDecimal(Territory_FKID));
            return Json("");
        }

        #region TeamReminderLinkMaster

        public ActionResult TeamReminderLinkMaster()
        {
            return View();
        }
        public JsonResult GetTeamReminderLinkMaster(GridQueryModel gridQueryModel)
        {
            try
            {


                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in OtherMasters.RR_GetAllTeam() select q).ToList();
                //var query = (from a in OtherMasters.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "TeamName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query
                                     where sq.TeamName != null && sq.TeamName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Reminder.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)
                                     select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.TeamName != null && sq.TeamName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Reminder.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.TeamName != null && sq.TeamName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.Reminder.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.TeamName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.TeamName != null && sq.TeamName.ToUpper().Contains(searchString) || sq.Reminder.Contains(searchString) select sq).ToList();
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
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public ActionResult UpdateTeamReminderLinkMaster(string gridData)
        {
            try
            {
                JavaScriptSerializer objJavaScriptSerializer = new JavaScriptSerializer();

                TeamReminderLink[] objStatus = objJavaScriptSerializer.Deserialize<TeamReminderLink[]>(gridData);
                string msg = string.Empty;

                //List<string> objStatus = objJavaScriptSerializer.Deserialize<string<new{x=>x.PKID,y=>y.Question }>>(gridData);
                string s = string.Empty;
                StringBuilder sb = new StringBuilder();
                string Flag = string.Empty;
                sb.Append("<row>");
                foreach (TeamReminderLink val in objStatus)
                {
                    sb.Append("<remin ");
                    sb.Append("TeamFKID= '" + val.PKID + "' ");
                    if (val.Reminder == "Active")
                        Flag = "1";
                    else
                        Flag = "0";
                    sb.Append(" Reminder='" + Flag + "'");
                    sb.Append("/> ");

                }
                sb.Append("</row>");

                string strxml = sb.ToString();
                int userfkid = Convert.ToInt32(Session["USER_FKID"]);
                int save = OtherMasters.RR_InsertUserReminderLink(strxml, userfkid);
                if (save == -1)
                {
                    msg = "Team Reminder Link Submitted Successfully.";
                }

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportTeamReminderLinkMasterToExcel()
        {
            try
            {
                var context = (from q in OtherMasters.RR_GetAllTeam() select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.PKID.ToString(),
                item.TeamName,
                item.Reminder
               
            }));
                return new ExcelResult(HeadersTeamReminderLink, ColunmTypesTeamReminderLink, data, "TeamReminderLink.xlsx", "TeamReminderLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersTeamReminderLink = {
                "PKID", "TeamName", "Reminder"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesTeamReminderLink = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportTeamReminderLinkMasterToPDF()
        {
            try
            {
                List<TeamReminderLink> userList = new List<TeamReminderLink>();
                var context = (from q in OtherMasters.RR_GetAllTeam() select q).ToList();

                foreach (var item in context)
                {
                    TeamReminderLink user = new TeamReminderLink();
                    user.PKID = item.PKID.ToString();
                    user.TeamName = item.TeamName;
                    user.Reminder = item.Reminder;
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
                            e.PKID.ToString(),e.TeamName,e.Reminder
                        }
                            })
                    ).ToArray()
                };
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Sample1.pdf");
                pdf.ExportPDF(customerList, new string[] { "PKID", "TeamName", "Reminder" }, path);
                return File(path, "application/pdf", "TeamReminderLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportTeamReminderLinkMasterToCsv()
        {
            try
            {
                var model = (from q in OtherMasters.RR_GetAllTeam() select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("PKID,TeamName,Reminder");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2}", record.PKID, record.TeamName, record.Reminder

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "TeamReminderLink.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class TeamReminderLink
        {
            public string PKID { get; set; }
            public string TeamName { get; set; }
            public string Reminder { get; set; }
        }
        #endregion

        #region Input Team Link
        public ActionResult InputTeamLink()
        {

            return View();
        }
        public JsonResult GetInputTeamLink(GridQueryModel gridQueryModel)
        {
            try
            {
                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in OtherMasters.GetInputteamlink() select new { q.PKID,q.InputFKID,q.InputName,q.GenName,q.TeamName,q.TeamFKID}).ToList();
                //var query = (from a in OtherMasters.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.InputName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


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
                    else if (gridQueryModel.sidx == "TeamName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.TeamName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "InputName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.InputName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "InputName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.InputName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.TeamName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "desc")
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
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }

        public string AppendXml(string SelTeamName) {

            StringBuilder SbInputFKID = new StringBuilder();

            var TFKID = SelTeamName.Split(',');

            SbInputFKID.Append("<root>");
            foreach (var record in TFKID)
            {
                SbInputFKID.Append("<TeamUserLink");
                SbInputFKID.Append(" TeamFKID='" + record + "'");
                SbInputFKID.Append("/>");
            }
            SbInputFKID.Append("</root>");



            return  SbInputFKID.ToString();

        }

        //public JsonResult EditInputTeamLink(string id, string oper, string PKID, string InputFKID, string TeamFKID, string TeamName, string InputName, string GenName, string SelTeamName, string id_g,FormCollection form)
        //{
        //    try
        //    {


        //        string msg = string.Empty; 
        //        string xmlstr = "";
               
               
        //        if (oper == "del")
        //        {
        //            OtherMasters.DeleteInputTeamLinkMaser(Convert.ToDecimal(id), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
        //            msg = "Input Team Link deleted Successfully.";
                    
        //        }

        //        return Json(msg);
        //    }
        //    catch (Exception ex)
        //    { return Json(new { error = ex.Message }); }
        //}

        public JsonResult AddInfolink(string oper, string SelTeamName, string InputName)
        {
            var msg = "";
            string xmlstr = AppendXml(SelTeamName);
            try
            {
                OtherMasters.AddInputTeamLinkMaser(Convert.ToDecimal(InputName), xmlstr, Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                msg = "Input Team Link Added Successfully.";
            }
            catch(Exception ex) {
                msg = ex.Message;
            }
            return Json(msg);
        }

        public JsonResult EditInfolink(string oper, string SelTeamName, string InputName)
        {
            var msg = "";
            string xmlstr = AppendXml(SelTeamName);
            try
            {
                xmlstr = AppendXml(SelTeamName);
                OtherMasters.AddInputTeamLinkMaser(Convert.ToDecimal(InputName), xmlstr, Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                msg = "Input Team Link Edited Successfully."; 
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(msg);
        }

        public JsonResult DeleteInfolink(string oper, string PKID)
        {
            var msg = "";
           
            try
            { 
                OtherMasters.DeleteInputTeamLinkMaser(Convert.ToDecimal(PKID), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                msg = "Input Team Link Deleted Successfully.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(msg);
        }
        
        public ActionResult GetInputName()
        {
            try
            {
                ViewBag.Type = "InputName";
                var query = (from e in OtherMasters.GetInputNamebyXML() select new { e.PKID, e.InputName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.InputName);

                return PartialView("PVInputTeamLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }


        public ActionResult GetTeamName(string pkid,string type)
        {
            try
            {
                dynamic query = "";
                ViewBag.Type = "TeamName";
                 query = (from e in OtherMasters.GetTeamNamefromInputMaster(Convert.ToDecimal(pkid), type) where e.Element == "SHDay" select new { e.PKID, e.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName);
                return PartialView("PVInputTeamLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }


        public ActionResult GetSelectedTeamName(string pkid, string type)
        {
            try
            {
                dynamic query = "";
                ViewBag.Type = "SelTeamName";
                query = (from e in OtherMasters.GetTeamNamefromInputMaster(Convert.ToDecimal(pkid), type) where e.Element == "NSHDay" select new { e.PKID, e.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName);
                return PartialView("PVInputTeamLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }


        public ActionResult DefaultTeamLoad(string name)
        {
            try
            {
                
                dynamic query = "";
                ViewBag.Type = name;
                query = (from e in OtherMasters.GetTeamNamefromInputMaster(Convert.ToDecimal(0), "") where 1==2 select new { e.PKID, e.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName);
                return PartialView("PVInputTeamLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }

        public JsonResult JsonGetGenName(string pkid,string type)
        {

            string genId="";

          var rslt = (from a in OtherMasters.getInputTypebyInputMaster(Convert.ToDecimal(pkid),type) select new { a.InputTypeFKID}).First();
         
            
            return Json(new
            {
                genId = rslt.InputTypeFKID
                
            }, JsonRequestBehavior.AllowGet);
        }
       
        




        public ActionResult ExportInputTeamLinkToExcel()
        {
            try
            {

                var context = (from q in OtherMasters.GetInputteamlink() select new { q.PKID, q.InputFKID, q.InputName, q.GenName, q.TeamName, q.TeamFKID }).OrderBy(x => x.InputName).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.InputName == null ? "" :item.InputName,
                 item.GenName == null ? "" :item.GenName,
                  item.TeamName == null ? "" :item.TeamName,
              
                
               
            }));
                return new ExcelResult(HeadersInputTeamLink, ColunmTypesInputTeamLink, data, "InputTeamLink.xlsx", "InputTeamLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersInputTeamLink = {
                "Input Name","Input Type","Team Name"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesInputTeamLink = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportInputTeamLinkToPDF()
        {
            try
            {
                List<clsInputTeamLink> userList = new List<clsInputTeamLink>();

                var context = (from q in OtherMasters.GetInputteamlink() where  q.InputName != null && q.GenName != null && q.TeamName != null select q).OrderBy(x => x.InputName).ToList();
               
                foreach (var item in context)
                {
                    clsInputTeamLink user = new clsInputTeamLink();
                   // user.InputFkid = item.InputFKID.ToString();
                    user.InputName = item.InputName == null ? "" : item.InputName;
                    user.InputType = item.GenName == null ? "" : item.GenName;
                    user.TeamName = item.TeamName == null ? "" : item.TeamName;

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
                                id = e.InputFkid,
                                cell = new string[]{
                            e.InputName,e.InputType,e.TeamName
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] { "InputName", "InputType", "TeamName" }, path); 
                return File(path, "application/pdf", "InputTeamLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportUserInputTeamLinkCsv()
        {
            try
            {
                var model = (from q in OtherMasters.GetInputteamlink() select q).OrderBy(x => x.InputName).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Input Name,Input Type,Team Name");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2}", record == null ? "" : record.InputName, record == null ? "" : record.GenName, record == null ? "" : record.TeamName

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "InputTeamLink.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class clsInputTeamLink
        {
            public string InputFkid { get; set; }
            public string InputName { get; set; }
            public string InputType { get; set; }
            public string TeamName { get; set; }

        }

        #endregion
        #region Input Product Link Master
        public ActionResult InputProductLinkMaster()
        {

            return View();
        }
        public JsonResult GetInputProductLinkMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in OtherMasters.GetInputProductLinkMaster() select q).ToList();
                //var query = (from a in OtherMasters.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "ProductName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.ProductName!=null && sq.ProductName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)  select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.ProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ProductName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "InputName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.InputName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "InputName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.InputName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.GenName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "desc")
                        pageData = (from cust in query orderby cust.GenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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
       
        //public JsonResult EditInputProductLinkMaster(string id, string oper, string PKID, string InputFKID, string TeamFKID, string TeamName, string InputName, string GenName, string IsActive)

        //{
        //    try
        //    {


        //        string msg = string.Empty;

        //        int createdby = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

        //        StringBuilder SbInputFKID = new StringBuilder();

        //        var TFKID = TeamFKID.Split(',');

        //        SbInputFKID.Append("<root>");
        //        foreach (var record in TFKID)
        //        {
        //            SbInputFKID.Append("<TeamUserLink");
        //            SbInputFKID.Append(" TeamFKID='" + record + "'");
        //            SbInputFKID.Append("/>");
        //        }
        //        SbInputFKID.Append("</root>");



        //        string xmlstr = SbInputFKID.ToString();

        //        if (oper == "edit")
        //        {

        //            int result = OtherMasters.AddInputProductLinkMaser(Convert.ToDecimal(InputFKID), xmlstr, createdby);

        //            if (result == 1)
        //            {
        //                msg = "Input Product Team Link Edited Successfully.";
        //            }

        //        }
        //        if (oper == "add")
        //        {

        //            int query = OtherMasters.AddInputProductLinkMaser(Convert.ToDecimal(InputFKID), xmlstr, createdby);

        //            if (query == 1)
        //            {
        //                msg = "Input Product Team Link Added Successfully.";
        //            }

        //        }
        //        if (oper == "del")
        //        {
        //            int result = OtherMasters.DeleteInputProductLinkMaser(Convert.ToDecimal(id), createdby);

        //            if (result == 1)
        //            {
        //                msg = "Input Product Team Link deleted Successfully.";
        //            }
        //        }

        //        return Json(msg);
        //    }
        //    catch (Exception ex)
        //    { return Json(new { error = ex.Message }); }
        //}

        // newly done


        public string AppendProductXml(string SelTeamName)
        {

            StringBuilder SbInputFKID = new StringBuilder();

            var TFKID = SelTeamName.Split(',');

            SbInputFKID.Append("<root>");
            foreach (var record in TFKID)
            {
                SbInputFKID.Append("<TeamUserLink");
                SbInputFKID.Append(" TeamFKID='" + record + "'");
                SbInputFKID.Append("/>");
            }
            SbInputFKID.Append("</root>");



            return SbInputFKID.ToString();

        }
        public JsonResult AddInfoProductLlink(string oper, string SelTeamName, string InputName)
        {
            var msg = "";
            string xmlstr = AppendProductXml(SelTeamName);
            try
            {
                OtherMasters.AddInputProductLinkMaser(Convert.ToDecimal(InputName), xmlstr, Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                msg = "Input Product Team Link Added Successfully.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(msg);
        }

        public JsonResult EditInfoProductLlink(string oper, string SelTeamName, string InputName)
        {
            var msg = "";
            string xmlstr = AppendProductXml(SelTeamName);
            try
            {
                xmlstr = AppendXml(SelTeamName);
                OtherMasters.AddInputProductLinkMaser(Convert.ToDecimal(InputName), xmlstr, Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                msg = "Input Product Team Link Edited Successfully.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(msg);
        }

        public JsonResult DeleteInfoProductLlink(string oper, string PKID)
        {
            var msg = "";

            try
            {
              var rslt=  OtherMasters.DeleteInputProductLinkMaser(Convert.ToDecimal(PKID), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString()));
                msg = "Input Product Team Link Deleted Successfully.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(msg);
        }

        public ActionResult GetProductInputName()
        {
            try
            {
                ViewBag.Type = "InputName";
                var query = (from e in OtherMasters.GetInputNamebyXML() select new { e.PKID, e.InputName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.InputName);

                return PartialView("PVProductLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }


        public ActionResult GetProductTeamName(string pkid, string type)
        {
            try
            {
                dynamic query = "";
                ViewBag.Type = "ProductName";
                query = (from e in OtherMasters.GetProductNamefromInputMaster(Convert.ToDecimal(pkid), type) where e.Element == "SHDay" select new { e.PKID, e.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName);
                return PartialView("PVProductLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }


        public ActionResult GetProductSelectedTeamName(string pkid, string type)
        {
            try
            {
                dynamic query = "";
                ViewBag.Type = "SelProductName";
                query = (from e in OtherMasters.GetProductNamefromInputMaster(Convert.ToDecimal(pkid), type) where e.Element == "NSHDay" select new { e.PKID, e.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName);
                return PartialView("PVProductLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }


        public ActionResult DefaultProductTeamLoad(string name)
        {
            try
            {

                dynamic query = "";
                ViewBag.Type = name;
                query = (from e in OtherMasters.GetTeamNamefromInputMaster(Convert.ToDecimal(0), "") where 1 == 2 select new { e.PKID, e.TeamName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.TeamName);
                return PartialView("PVProductLink", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }

        public JsonResult JsonGetProductGenName(string pkid, string type)
        {

            string genId = "";

            var rslt = (from a in OtherMasters.getInputTypebyInputProdLinkMaster(Convert.ToDecimal(pkid), type) select new { a.InputTypeFKID }).First();


            return Json(new
            {
                genId = rslt.InputTypeFKID

            }, JsonRequestBehavior.AllowGet);
        }

        
        // end


        public ActionResult ExportInputProductinkMasterToExcel()
        {
            try
            {
                var context = (from q in OtherMasters.GetInputProductLinkMaster() select q).OrderBy(x => x.ProductName).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                 item.InputName == null ? "" :item.InputName,
                 item.GenName == null ? "" :item.GenName,
                  item.ProductName == null ? "" :item.ProductName,
             
                
               
            }));
                return new ExcelResult(HeadersInputProductLinkMaster, ColunmTypesInputProductLinkMaster, data, "InputProductLinkMaster.xlsx", "InputProductLinkMaster");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersInputProductLinkMaster = {
                "Input Name","Input Type","Product Name"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesInputProductLinkMaster = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
               
            };
        public ActionResult ExportInputProductLinkMasterToPDF()
        {
            try
            {
                List<clsInputProductLinkMaster> userList = new List<clsInputProductLinkMaster>();
                var context = (from q in OtherMasters.GetInputProductLinkMaster() 
                               select q).OrderBy(x => x.ProductName).ToList();

                foreach (var item in context)
                {
                    clsInputProductLinkMaster user = new clsInputProductLinkMaster();
                   // user.ProductFkid = item.ProductFKID.ToString();
                    user.InputName = item.InputName == null ? "" : item.InputName;
                    user.InputType = item.GenName == null ? "" : item.GenName;
                    user.ProductName = item.ProductName == null ? "" : item.ProductName;
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
                                id = e.ProductFkid,
                                cell = new string[]{
                           e.InputName,e.InputType,e.ProductName
                        }
                            })
                    ).ToArray()
                }; 
               
                pdf.ExportPDF(customerList, new string[] { "InputName", "InputType", "ProductName" }, path);
                return File(path, "application/pdf", "InputProductLinkMaster.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportUserInputProductLinkMasterCsv()
        {
            try
            {
                var model = (from q in OtherMasters.GetInputProductLinkMaster() select q).OrderBy(x => x.ProductName).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Input Name,Input Type,Product Name");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2}", record == null ? "" : record.InputName, record == null ? "" : record.GenName, record == null ? "" : record.ProductName

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "InputProductLinkMaster.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class clsInputProductLinkMaster
        {
            public string ProductFkid { get; set; }
            public string InputName { get; set; }
            public string InputType { get; set; }
            public string ProductName { get; set; }

        }

        #endregion


        #region MuniPrathap
        public ActionResult PrescriptionTracker()
        {

            return View();
        }
        //For loading Product Drop Down 
        public ActionResult ProductLoad(string type, int Count)
        {
            ViewBag.Type = type;
            dynamic query = "";
            decimal psoFkid = 0;
            //ViewBag.Row = Count;
            ViewBag.Count = Convert.ToString("drpProduc_" + Count);
            ViewBag.Id = Count;
            psoFkid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            query = (from a in OtherMasters.GetProductByTeamByXML(psoFkid) select new { a.PKID, a.ProductName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.ProductName.ToString());

            return PartialView("PVFirstLoadspecial", query);


        }

        public ActionResult ProductPack(string type, int Count)
        {
            ViewBag.Type = type;
            dynamic query = "";
            decimal psoFkid = 0;
            ViewBag.Count = Convert.ToString("drpPack_" + Count);
            return PartialView("PVFirstLoadspecial");

        }
        //ProductPackByProduct
        public ActionResult ProductPackByProduct(string type, string Product)
        {
            ViewBag.Type = type;
            dynamic query = "";
            decimal psoFkid = 0;
            psoFkid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


            query = (from a in OtherMasters.getProdPackSizeByXML(Convert.ToInt32(Product)) select new { a.PKID, a.ProductPack }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.ProductPack.ToString());

            return PartialView("PVFirstLoadspecial", query);


        }
        public ActionResult ProductHospital(string type, int Count)
        {
            ViewBag.Type = type;
            dynamic query = "";
            decimal psoFkid = 0;
            ViewBag.Count = Convert.ToString("drpHosp_" + Count);
            psoFkid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            query = (from s in OtherMasters.PrescGetHospital(psoFkid) select new { s.Hospital, s.PKID }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.Hospital.ToString());

            return PartialView("PVFirstLoadspecial", query);


        }

        public ActionResult ProductStokiest(string type, int Count)
        {
            ViewBag.Type = type;
            dynamic query = "";
            decimal psoFkid = 0;
            ViewBag.Count = Convert.ToString("drpStok_" + Count);
            psoFkid = Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            query = (from s in OtherMasters.PrescGetStockist(psoFkid) select new { s.Stockist, s.PKID }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.Stockist.ToString());

            return PartialView("PVFirstLoadspecial", query);


        }

        public ActionResult PrescriptionTracker1(decimal DoctorFkid)
        {

            var item = (from a in OtherMasters.getDoctorPrescriptionTrackerDtls(DoctorFkid, 5) select new { a.Date, a.ProductName, a.Productpack, a.Prescription, a.Hospital, a.Stockist }).ToList();
            return Json(item);

        }


        //PrescriptionAdd
        public JsonResult PrescriptionAdd(string TblValue, string DoctorFkid, string SpecialityFKID)
        {
            dynamic msg = "";
            string xml = "";
            decimal territory = Convert.ToDecimal(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString());
            decimal psoFkid = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            xml = ProductPackXml(TblValue);
            OtherMasters.AddPrescriptionTrackermaster(Convert.ToDecimal(DoctorFkid), Convert.ToDecimal(SpecialityFKID), psoFkid, territory, xml);
            msg = "Successfully";
            return Json(msg);



        }
        public string ProductPackXml(string TblValue)
        {
            string Xml = "";
            string[] ProductpackDetails = TblValue.Split('|');
            Xml = "<Root>";
            for (int i = 0; i < ProductpackDetails.Length - 1; i++)
            {
                string[] temp = ProductpackDetails[i].Split(',');
                Xml += "<Table ";
                Xml += "  Dateid='" + Convert.ToDateTime(temp[0]) + "'";
                Xml += "  Productid='" + temp[1] + "'";
                Xml += "  Packsizeid='" + temp[2] + "'";
                Xml += "  Prescriptionid='" + temp[3] + "'";
                Xml += "  HoId='" + temp[4] + "'";
                Xml += "  StId='" + temp[5] + "'";
                Xml += "/>";


            }
            Xml += "</Root>";
            return Xml;
        }




        public JsonResult GridPrecscriptionMaster(GridQueryModel gridQueryModel, string brick, string specialization)
        {
            try
            {

                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                //var  query="";

                var query = (from a in OtherMasters.GetDoctorPSOLinkMaster(Convert.ToDecimal(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString())) where 1 == 2 orderby a.DoctorName select new { a.DoctorName, a.DoctorFKID, a.Specialization, a.SpecialityFKID, a.AreaName }).ToList();


                if (brick == "" && specialization == "")
                    query = (from a in OtherMasters.GetDoctorPSOLinkMaster(Convert.ToDecimal(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString())) orderby a.DoctorName select new { a.DoctorName, a.DoctorFKID, a.Specialization, a.SpecialityFKID, a.AreaName }).ToList();
                else
                {
                    query = (from a in OtherMasters.AreaSearch(Convert.ToInt32(brick), Convert.ToInt32(specialization), Convert.ToInt32(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString())) orderby a.DoctorName select new { a.DoctorName, a.DoctorFKID, a.Specialization, a.SpecialityFKID, a.AreaName }).ToList();

                }

                var count = query.Count();
                var pageData = query.OrderBy(x => x.DoctorName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if (searchField == "AreaName")
                    {
                        if (searchOper == "bw")//begins with
                        {

                            var q = (from sq in query where sq.AreaName != null && sq.AreaName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.AreaName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.AreaName != null && sq.AreaName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.AreaName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.AreaName != null && sq.AreaName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.AreaName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.AreaName != null && sq.AreaName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.AreaName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                    else
                    {
                        if (searchOper == "bw")//begins with
                        {

                            var q = (from sq in query where sq.Specialization != null && sq.Specialization.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.Specialization != null && sq.Specialization.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.Specialization != null && sq.Specialization.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.Specialization != null && sq.Specialization.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.Specialization).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                    }
                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "DoctorName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.DoctorName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Doctor" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.DoctorName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    if (gridQueryModel.sidx == "Brick" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.AreaName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "AreaName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.AreaName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Specialization ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Specialization descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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
        public ActionResult ExportPrescriptionTrackerToExcel()
        {
            var context = (from a in OtherMasters.GetDoctorPSOLinkMaster(Convert.ToDecimal(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString())) orderby a.DoctorName select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.DoctorName== null ? "" : item.DoctorName.ToString(),                
                item.Specialization== null ? "":item.Specialization.ToString(),
                item.AreaName== null ? "":item.AreaName.ToString()
               
            }));
            return new ExcelResult(HeadersActivityMaster, ColunmTypesActivityMaster, data, "PrescriptionTracker.xlsx", "PrescriptionTracker");
        }
        private static readonly string[] HeadersActivityMaster = {
               "DoctorName","Specialization","AreaName"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesActivityMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String 
                
            };


        public ActionResult ExportPrescriptionTrackerToPDF()
        {
            List<PrescriptionTrackerPDF> userList = new List<PrescriptionTrackerPDF>();
            var context = (from a in OtherMasters.GetDoctorPSOLinkMaster(Convert.ToDecimal(Session["Territory_FKID"] == null ? "0" : Session["Territory_FKID"].ToString()), Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString())) orderby a.DoctorName select a).ToList();

            foreach (var item in context)
            {
                PrescriptionTrackerPDF user = new PrescriptionTrackerPDF();

                user.DoctorName = item.DoctorName == null ? "" : item.DoctorName.ToString();
                user.Specialization = item.Specialization == null ? "" : item.Specialization.ToString();
                user.AreaName = item.AreaName == null ? "" : item.AreaName.ToString();
                userList.Add(user);
            }
            var customerList = userList;
            pdf.ExportPDF(customerList, new string[] { "DoctorName", "Specialization", "AreaName" }, path);
            return File(path, "application/pdf", "PrescriptionTracker.pdf");
        }
        public ActionResult ExportPrescriptionTrackerToCsv()
        {
            var model = (from a in OtherMasters.GetDoctorPSOLinkMaster(0, Convert.ToInt32(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString())) orderby a.DoctorName select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("DoctorName,Specialization,AreaName");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.DoctorName == null ? "" : record.DoctorName.ToString(), record.Specialization == null ? "" : record.Specialization.ToString(),
                    record.AreaName == null ? "" : record.AreaName.ToString()

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "PrescriptionTracker.txt");
        }
        public class PrescriptionTrackerPDF
        {
            public string DoctorName { get; set; }
            public string Specialization { get; set; }
            public string AreaName { get; set; }
        }
        
        
        #endregion

    }
    
}
