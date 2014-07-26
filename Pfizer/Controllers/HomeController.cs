using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.Mvc;

using System.Data;
using System.Text;
using Sify.Encryption;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using PfizerEntity;
using System.Configuration;
using System.Web.Mail;
using Sify;
using SifyGrid.GridHelpers;


namespace Pfizer.Controllers
{
    
    public class HomeController : Controller
    {
        private pfizerEntities dataContext = new PfizerEntity.pfizerEntities();
        Sify.Encryption.Base64 bs = new Base64();
        enum NodeType { RBM = 12, DM = 14, PSO = 15 };
       
       [HttpGet]
        public ActionResult Login(UserMaster user)
        {

            try
            {

                Session.Abandon();
                GetQuestions();
                GetTimer();
                ViewData["Questions"] = Session["Questions"].ToString();
                ViewData["QuestId"] = Session["QuestId"].ToString();
                ViewData["Timer"] = Session["Timer"].ToString();
                ViewData["Error"] = (Session["Error"] != null) ? Session["Error"].ToString() : "";
                ViewBag.HelpDesk = ConfigurationManager.AppSettings["HelpDesk"].ToString();
                return View();
            }
            catch {
                return RedirectToAction("Error");
            }

        }

        [HttpPost]
        public ActionResult Login(FormCollection form, UserMaster user)
        {

             
            try
            {

                var retVal = dataContext.LoginLock(form["Loginname"].ToString());


                if (retVal.Count() > 0)
                {

                    Session["Error"] = "Your Account is locked . Contact Administrator .";

                }

                else
                {
                    var rslt = (from a in dataContext.usp_AuthenticateUser(form["Loginname"].ToString(), bs.Encrypt(form["Password"].ToString()), Convert.ToDecimal(form["hdnQuesId"].ToString()), form["txtAnswer"].ToString()) select new { a.LoginName, a.Result, a.ROLE_FKID, a.Territory_FKID, a.USER_FKID, a.LastAddresUpdateDate, a.LastLogin, a.Division_FKID, a.EMP_FKID, a.EmpName, a.NodeType_FKID }).FirstOrDefault();

                    Session["pageFlag"] = "";
                    Session["USER_FKID"] = "";

                    switch (rslt.Result)
                    {

                        case "0":
                            Session["USER_FKID"] = rslt.USER_FKID.ToString();//.Rows[0]["USER_FKID"].ToString();
                            Session["USER_PKID"] = rslt.USER_FKID.ToString();//dtRetval.Rows[0]["USER_FKID"].ToString();
                            Session["Emp_Name"] = rslt.EmpName.ToString();//dtRetval.Rows[0]["EmpName"].ToString();
                            Session["Emp_FKID"] = rslt.EMP_FKID.ToString();//dtRetval.Rows[0]["Emp_FKID"].ToString();

                            Session["Division_FKID"] = rslt.Division_FKID.ToString();//dtRetval.Rows[0]["Division_FKID"].ToString();
                            Session["Territory_FKID"] = rslt.Territory_FKID.ToString();//dtRetval.Rows[0]["Territory_FKID"].ToString();
                            Session["NodeType_FKID"] = rslt.NodeType_FKID.ToString();// dtRetval.Rows[0]["NodeType_FKID"].ToString();
                            Session["Role_FKID"] = rslt.ROLE_FKID.ToString();//dtRetval.Rows[0]["ROLE_FKID"].ToString();
                            Session["LastLogin"] = rslt.LastLogin.ToString();// dtRetval.Rows[0]["LastLogin"].ToString();
                            Session["hdrLogin_USERId"] = rslt.LoginName.ToString();// dtRetval.Rows[0]["LoginName"].ToString();
                            Session["LastAddresUpdateDate"] = rslt.LastAddresUpdateDate.ToString();// dtRetval.Rows[0]["LastAddresUpdateDate"];

                            if (form["Password"] != "optimaii")
                            {

                                if (Int16.Parse(Session["NodeType_FKID"].ToString()) == (int)NodeType.PSO)
                                {
                                    return RedirectToAction("Welcome");
                                    // HttpContext.Current.Response.Redirect("~/scripts/transactions/DailyReportInterMediate.aspx?menuId=47",false);	
                                }
                                else
                                {
                                    return RedirectToAction("Welcome");

                                    //HttpContext.Current.Response.Redirect("~/scripts/masters/Welcome1.aspx",false);
                                }

                            }
                            if (form["Password"].ToString() == "optimaii")
                            {
                                return View("ChangePassword");
                                //HttpContext.Current.Response.Redirect("~/scripts/masters/FChangePassword.aspx",false);
                            }
                            break;
                        case "1":
                            Session["Error"] = "Invalid LoginId or Password";
                            break;
                        case "-1":
                            Session["Error"] = "Invalid Answer ";
                            break;
                        case "2":
                            Session["Error"] = "Your Account is locked . Contact Administrator .";
                            break;

                    }
                }
                GetQuestions();
                GetTimer();
                ViewData["Questions"] = Session["Questions"].ToString();
                ViewData["QuestId"] = Session["QuestId"].ToString();
                ViewData["Timer"] = Session["Timer"].ToString();
                ViewData["Error"] = Session["Error"].ToString();
            }
            catch (Exception ex)
            {
                ViewData["Questions"] = Session["Questions"].ToString();
                ViewData["QuestId"] = Session["QuestId"].ToString();
                ViewData["Timer"] = Session["Timer"].ToString();
                ViewData["Error"] = ex.Message;

            }
            return View();
        }

        public ActionResult Welcome()
        {
            Session["selectedMenu"] = "";
            if (Session["Role_FKID"] == null || Session["Role_FKID"].ToString() == string.Empty)
                return RedirectToAction("Login", "Home");

            // Alert Div :
            int pkId = 0;
            if (Session["USER_FKID"] == null)
                return View("Login");
            else
                pkId = Convert.ToInt32(Session["USER_FKID"].ToString());

            var rslt = (from a in dataContext.GetTopAlertsByXML(pkId) select new { a.AlertStatus, a.PKID, a.Title }).ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var bindRslt in rslt)
            {
                //sb.Append(" <li><a href='AlertPopUp?PkId="+bindRslt.PKID+"' target='_blank'>" + bindRslt.Title + "</a></li>");
                sb.Append(" <li><a href='#' onclick=\"CalPopUp('" + bindRslt.PKID + "');\">" + bindRslt.Title + "</a></li>");
            }
            ViewData["Alert"] = sb;

            // Opinion Poll

            var opinionRslt = (from a in dataContext.GetTopOneOpinionByXML(pkId) select new { a.Option1, a.Option2, a.Option3, a.Option4, a.Option5, a.Question, a.PKID }).ToList();

            StringBuilder sbOpinion = new StringBuilder();


            foreach (var bindOpinion in opinionRslt)
            {
                sbOpinion.Append("<p>" + bindOpinion.Question + "</p><div class='options'>");
                if (bindOpinion.Option1 != null && bindOpinion.Option1 != string.Empty)
                    sbOpinion.Append("<input name='Yes'  type='radio' value='1' class='r_btn'/><span class='gn-op'>" + bindOpinion.Option1 + "</span><div class='clear'></div>");
                if (bindOpinion.Option2 != null && bindOpinion.Option2 != string.Empty)
                    sbOpinion.Append("<input name='Yes'  type='radio' value='2' class='r_btn'/><span class='gn-op'>" + bindOpinion.Option2 + "</span><div class='clear'></div>");
                if (bindOpinion.Option3 != null && bindOpinion.Option3 != string.Empty)
                    sbOpinion.Append("<input name='Yes' type='radio' value='3' class='r_btn'/><span class='gn-op'>" + bindOpinion.Option3 + "</span><div class='clear'></div>");
                if (bindOpinion.Option4 != null && bindOpinion.Option4 != string.Empty)
                    sbOpinion.Append("<input name='Yes'  type='radio' value='4' class='r_btn'/><span class='gn-op'>" + bindOpinion.Option4 + "</span><div class='clear'></div>");
                if (bindOpinion.Option5 != null && bindOpinion.Option5 != string.Empty)
                    sbOpinion.Append("<input name='Yes' type='radio' value='5' class='r_btn'/><span class='gn-op'>" + bindOpinion.Option5 + "</span><div class='clear'></div>");
                sbOpinion.Append("</div><div class='clear'></div><div class='btn_biv'><input class='blue_button' type='button' onclick='VoteOpinion(" + bindOpinion.PKID + ");' value='Vote' align='middle'></div>");

            }
            ViewBag.Opinion = sbOpinion;

            //Remainder Div

            var remainderRslt = (from a in dataContext.vw_MeetingReminderReadstatus select new { a.Element, a.UnReadstatusCurrentDate }).ToList();


            foreach (var remainder in remainderRslt)
            {

                if (remainder.Element == "Today")
                {
                    @ViewBag.Today = remainder.UnReadstatusCurrentDate;
                    continue;
                }
                if (remainder.Element == "TodayR")
                {
                    @ViewBag.UnRead = remainder.UnReadstatusCurrentDate;
                    continue;
                }
                if (remainder.Element == "Month")
                {
                    @ViewBag.Month = remainder.UnReadstatusCurrentDate;
                    continue;
                }
                if (remainder.Element == "MonthR")
                {
                    @ViewBag.UnReadMonth = remainder.UnReadstatusCurrentDate;
                    continue;
                }

            }



            // News Div

            string fkId = Session["Division_FKID"].ToString();

            var newsRslt = (from a in dataContext.GetContentByXML(4, fkId) select new { a.NewStausFlag, a.PKID, a.Title, a.PriorityFKID }).ToList();

            StringBuilder sbNews = new StringBuilder();


            foreach (var bindNews in newsRslt)
            {

                if (bindNews.NewStausFlag == Convert.ToBoolean(1))
                    sbNews.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindNews.PKID + "','News');\">" + bindNews.Title + "</a><img src='Content/images/newBlink.gif' /></li>");
                else
                    sbNews.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindNews.PKID + "','News');\">" + bindNews.Title + "</a></li>");


            }
            if (newsRslt.Count() <= 0)
            {
                ViewBag.News = "<span style='color:red'>No Content Available</span>";
            }
            else
                ViewBag.News = sbNews;


            // Success Div :


            var successRslt = (from a in dataContext.GetContentByXML(5, fkId) select new { a.NewStausFlag, a.PKID, a.Title, a.PriorityFKID }).ToList();

            StringBuilder sbSuccess = new StringBuilder();

            foreach (var bindSuccess in successRslt)
            {

                if (bindSuccess.NewStausFlag == Convert.ToBoolean(1))
                    sbSuccess.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindSuccess.PKID + "','Success');\">" + bindSuccess.Title + "</a><img src='Content/images/newBlink.gif' /></li>");
                else
                    sbSuccess.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindSuccess.PKID + "','Success');\">" + bindSuccess.Title + "</a></li>");
            }

            if (successRslt.Count() <= 0)
            {
                ViewBag.Success = "<span style='color:red'>No Content Available</span>";
            }
            else
                ViewBag.Success = sbSuccess;



            // Industry Div

            var indusRslt = (from a in dataContext.GetContentByXML(2, fkId) select new { a.NewStausFlag, a.PKID, a.Title, a.PriorityFKID }).ToList();

            StringBuilder sbIndus = new StringBuilder();

            foreach (var bindIndus in indusRslt)
            {

                if (bindIndus.NewStausFlag == Convert.ToBoolean(1))
                    sbIndus.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindIndus.PKID + "','Industry');\">" + bindIndus.Title + "</a><img src='Content/images/newBlink.gif' /></li>");
                else
                    sbIndus.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindIndus.PKID + "','Industry');\">" + bindIndus.Title + "</a></li>");

            }


            if (indusRslt.Count() <= 0)
            {
                ViewBag.Indus = "<span style='color:red'>No Content Available</span>";
            }
            else
                ViewBag.Indus = sbIndus;





            // Tips Div :

            var TipsRslt = (from a in dataContext.GetContentByXML(1, fkId) select new { a.NewStausFlag, a.PKID, a.Title, a.PriorityFKID }).ToList();

            StringBuilder sbTips = new StringBuilder();

            foreach (var bindTips in TipsRslt)
            {

                if (bindTips.NewStausFlag == Convert.ToBoolean(1))
                    sbTips.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindTips.PKID + "','Tips');\">" + bindTips.Title + "</a><img src='Content/images/newBlink.gif' /></li>");
                else
                    sbTips.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindTips.PKID + "','Tips');\">" + bindTips.Title + "</a></li>");


            }

            if (TipsRslt.Count() <= 0)
            {
                ViewBag.Tips = "<span style='color:red'>No Content Available</span>";
            }
            else
                ViewBag.Tips = sbTips;


            // Information Div

            var infoRslt = (from a in dataContext.buildInfoBank() select new { a.PKID, a.ParentFKID, a.Category }).ToList();
            //var abc = (from a in infoRslt select a.PKID).ToArray();
            StringBuilder sbInforslt = new StringBuilder();
            var tempRslt = (from a in dataContext.buildInfoBank() where 1 == 2 select new { a.PKID, a.ParentFKID, a.Category }).ToList();



            foreach (var infoDiv in infoRslt)
            {
                sbInforslt.Append("<ul><li><a href='#'>" + infoDiv.Category + "</a></li>\n");
                break;

                //tempRslt = (from a in dataContext.buildInfoBank() where a.ParentFKID == infoDiv.PKID select new { a.PKID, a.ParentFKID, a.Category }).ToList();

                //if (infoRslt.Count() == 0)
                //{
                //    continue;
                //}
                //else
                //    foreach (var infoSecondRslt in infoRslt)
                //    {
                //        sbInforslt.Append("<a href='#'>" + infoSecondRslt.Category + "</a>\n");
                //        tempRslt = (from a in dataContext.buildInfoBank() where a.ParentFKID == infoSecondRslt.PKID select new { a.PKID, a.ParentFKID, a.Category }).ToList();
                //        if (infoRslt.Count() == 0)
                //            continue;
                //    }



            }


            ViewData["InfoDesk"] = sbInforslt;

            // Notification Div :

            var notifiRslt = (from a in dataContext.GetNotificationByXML() select new { a.PKID, a.Title }).ToList();

            StringBuilder sbNotifi = new StringBuilder();

            foreach (var bindNotifi in notifiRslt)
            {
                sbNotifi.Append(" <li><a href='#' onclick=\"CalModalWindow('" + bindNotifi.PKID + "','Notify');\">" + bindNotifi.Title + "</a></li>");
            }

            if (notifiRslt.Count() <= 0)
            {
                ViewBag.Notifi = "<span style='color:red'>No Content Available</span>";
            }
            else
                ViewBag.Notifi = sbNotifi;


            return View();
        }

        public ActionResult Error()
        {
            if (Session["USER_FKID"] == null)
            {
                return RedirectToAction("UnderConstruction");
            }
            else { 
             
                return View();
            }
        }

        public ActionResult UnderConstruction()
        {
            return View();

        }

        // Partial Views :
        public ActionResult MenuBuilder()
        {

            //if(Session["Menu"]==null)
            //{
                int roleId = Convert.ToInt32(Session["Role_FKID"].ToString());

                var m = (from a in dataContext.BuildMenu(roleId) select a).ToList();

                int id = 0;
                int count = 0;
                int selectedMenu = 0;


                StringBuilder mainDiv = new StringBuilder();
                Session["MainMenu"]=(Session["MainMenu"]==null)?"":Session["MainMenu"].ToString();
                Session["SubMenu"] = (Session["SubMenu"] == null) ? "" : Session["SubMenu"].ToString();
                foreach (var c in m)
                {
                    if (count == 0)
                    {
                        selectedMenu++;
                        count++;
                        id = c.id;
                        mainDiv.Append("<a class=\"menuitem submenuheader \" href=\"#\" onclick=\"GetSelectedMenu('" + c.DisplayHeading + "');\">" + c.DisplayHeading + "</a>");
                       
                        if (Session["SubMenu"].ToString() == c.cDisplayHeading)
                            mainDiv.Append("<div class=\"submenu\"><ul><li><a href=\"" + c.cHref + "\" class=\"active\" onclick=\"GetSelectedSubMenu('" + c.cDisplayHeading + "');\">" + c.cDisplayHeading + "</a></li>");
                        else
                            mainDiv.Append("<div class=\"submenu\"><ul><li><a href=\"" + c.cHref + "\" onclick=\"GetSelectedSubMenu('" + c.cDisplayHeading + "');\">" + c.cDisplayHeading + "</a></li>");


                        if (Session["MainMenu"].ToString() == c.DisplayHeading)
                        {
                            Session["selectedMenu"] = selectedMenu - 1;
                        }


                    }
                    else
                    {
                        if (id != c.id)
                        {
                            selectedMenu++;
                            id = c.id;
                            mainDiv.Append("</ul></div>");

                            mainDiv.Append("<a class=\"menuitem submenuheader \" href=\"#\"  onclick=\"GetSelectedMenu('" + c.DisplayHeading + "');\">" + c.DisplayHeading + "</a>");

                            if (Session["SubMenu"].ToString() != c.cDisplayHeading)
                            mainDiv.Append("<div class=\"submenu\"><ul><li><a href=\"" + c.cHref + "\">" + c.cDisplayHeading + "</a></li>");
                            else
                                mainDiv.Append("<div class=\"submenu\"><ul><li><a href=\"" + c.cHref + "\"  class=\"active\" >" + c.cDisplayHeading + "</a></li>");
                            
                            if (Session["MainMenu"].ToString() == c.DisplayHeading)
                            {                               
                                Session["selectedMenu"] = selectedMenu-1;
                            }
                            
                            

                        }
                        else
                        {
                            
                            if (Session["SubMenu"].ToString() == c.cDisplayHeading)
                                mainDiv.Append("<li><a href=\"" + c.cHref + "\" class=\"active\"  onclick=\"GetSelectedSubMenu('" + c.cDisplayHeading + "');\">" + c.cDisplayHeading + "</a></li>");
                            else
                                mainDiv.Append("<li><a href=\"" + c.cHref + "\"  onclick=\"GetSelectedSubMenu('" + c.cDisplayHeading + "');\">" + c.cDisplayHeading + "</a></li>");


                        }
                    }


                }

               
            // ViewBag.Menu = ("<div class=\"ui-layout-west\"><div class=\"glossymenu\">" + mainDiv.ToString() + "</ul></div></div></div>").ToString();
              
                ViewBag.Menu = ("<div class=\"glossymenu\">" + mainDiv.ToString() + "</ul></div></div>").ToString();

             //Session["Menu"] = ViewBag.Menu;
    
            //}
           
               return PartialView("MenuBuilder", ViewBag.Menu);
            // return PartialView("MenuBuilder", Session["Menu"].ToString()); 
        

        }




        // Json Methodds :

        public JsonResult JsonCal(string pkid, string val)
        {

            string genName = "";
            string title = "";
            string body = "";

            try
            {

                var rslt = (from a in dataContext.GetContentBodyTextByXML(pkid) select new { a.GenName, a.Title, a.Body }).FirstOrDefault();

                if (val == "Notify")
                {
                    var rsltNotify = (from a in dataContext.GetNotificationTextByXML(pkid) select new { a.PKID, a.GenName, a.Body }).FirstOrDefault();

                    return Json(new
                    {
                        genName = ((rsltNotify.GenName != null) || (rsltNotify.GenName != string.Empty)) ? string.Empty : rsltNotify.GenName,
                        title = string.Empty,
                        body = ((rsltNotify.Body != null) || (rsltNotify.Body != string.Empty)) ? string.Empty : rsltNotify.GenName,

                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new
                {
                    genName = ((rslt.GenName != null) || (rslt.GenName != string.Empty)) ? string.Empty : rslt.GenName,
                    title = ((rslt.Title != null) || (rslt.Title != string.Empty)) ? string.Empty : rslt.GenName,
                    body = ((rslt.Body != null) || (rslt.Body != string.Empty)) ? string.Empty : rslt.GenName,

                }, JsonRequestBehavior.AllowGet);
            }


            catch
            {
                return Json(new
                {
                    genName = string.Empty,
                    title = string.Empty,
                    body = "No Content Available",

                }, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult JsonAlertCal(string pkid)
        {

            var rslt = (from a in dataContext.GetAlertsTextByXML(pkid) select new { a.Body3, a.Body1, a.Body }).First();

            return Json(new
            {
                Body3 = (rslt.Body3 == null) ? "" : rslt.Body3,
                Body1 = (rslt.Body1 == null) ? "" : rslt.Body1,
                Body = (rslt.Body == null) ? "" : rslt.Body
            }, JsonRequestBehavior.AllowGet);

        }



        public JsonResult JsonChageStatus(decimal pkid)
        {

            dataContext.UpdatReadAlert(pkid);
            return Json(pkid);


        }


        public JsonResult JsonUpdateVote(int pkid, int answer)
        {
            decimal userId = Convert.ToDecimal(Session["USER_FKID"].ToString());

            dataContext.UpdateOpinionPollAnswer(pkid, answer, userId);
            return Json(pkid);


        }



        public JsonResult JsonGetPassword(string userId, string questId, string ans)
        {
            string error = string.Empty;
            string status = string.Empty;
            try
            {
                var retVal = dataContext.LoginLock(userId);

                if (retVal.Count() > 0)
                {

                    error = "Your Account is locked . Contact Administrator .";
                    return Json(error);

                }

                var rslt = (from a in dataContext.GetForgetPassword(userId, Convert.ToDecimal(questId), ans) select new { a.Pass, a.status }).Single();

                status = rslt.status.ToString();
                if (rslt.status == Convert.ToBoolean("True"))
                {
                    GetPassword_Result(userId, rslt.Pass);
                }
                else
                    error = rslt.Pass.ToString();


            }
            catch (Exception ex)
            {
               
                return Json(new
                {
                     status = "0",
                    error =  ex.Message  
                }, JsonRequestBehavior.AllowGet);
                
            }

            return Json(new
            {
                status = status,
                error =error,
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult JsonGetSelectedMenu(string menu,string submenu)
        {
           
            if (menu != "")
            {
                Session["MainMenu"] = menu;

            }
            if (submenu != "")
                Session["SubMenu"] = submenu;
            return Json(menu);


        }



        //Methods:
        public void GetQuestions()
        {
            var questions = dataContext.GetQuestion().ToList();
            var timer = (from a in dataContext.TimeSetting() select new { a.FromValue }).FirstOrDefault();

            StringBuilder stQuestion = new StringBuilder("");
            StringBuilder stQuestionId = new StringBuilder("");
            foreach (var quest in questions)
            {
                stQuestion.Append(quest.Question.ToString() + "|");
                stQuestionId.Append(quest.PKID.ToString() + "|");
            }
            Session["Questions"] = stQuestion.Remove(stQuestion.Length - 1, 1);
            Session["QuestId"] = stQuestionId.Remove(stQuestionId.Length - 1, 1);
            var timeCount = (timer.FromValue != null) ? timer.FromValue : "60";
            ViewData["Timer"] = timeCount;

        }

        public void GetTimer()
        {
            var timer = (from a in dataContext.TimeSetting() select new { a.FromValue }).FirstOrDefault();
            var timeCount = (timer.FromValue != null) ? timer.FromValue : "60";
            Session["Timer"] = timeCount;
        }

        public void GetPassword_Result(string userId, string pass)
        {

            var rslt = (from a in dataContext.GetPassword(userId) select new { a.FirstName, a.Email }).Single();
            if (rslt != null && rslt.FirstName != "")
            {

                SendMail(pass, rslt.FirstName, rslt.Email, userId);
            }

        }
        public void SendMail(string pass, string firstName, string email, string userId)
        {
            var fromAddress = ConfigurationManager.AppSettings["AdminEMail"].ToString();
            var toAddress = email;
            string subject = ConfigurationManager.AppSettings["FPwdMailSubject"].ToString();
            string body = "<font size=2px face=arial>Dear " + firstName + ", <br><br>";
            body += "Please find below your Optima login credentials:";
            body += "<br> <br> ";
            body += "User ID: <b>" + userId + "</b><br> <br> ";
            body += "Password: <b>" + bs.Encrypt(pass) + "</b><br> <br> <br> ";
            body += "Regards, <br><br> Optima Team </font>";

            // smtp settings
            System.Net.Mail.SmtpClient obj = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SMTPServer"].ToString());
            
           // obj.Send(fromAddress, toAddress, subject, body);
            obj.Send("rajkumarthulasi@gmail.com", "rajkumarthulasi@gmail.com", subject, body);
        }

    }
}
