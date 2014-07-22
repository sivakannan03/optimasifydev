using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PfizerEntity
{
    public class UsefulLink
    {

     

     public static IEnumerable<System.Web.Mvc.SelectListItem> DropDownListContents()
        {
         List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
         using (pfizerEntities datacontext = new pfizerEntities())
         {
             var list = (from s in datacontext.GetUsefulLinks() select new { s.GenName, s.PKID }).ToList();
                 if (list != null)
                 {
                     returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                     {
                         Text = x.GenName.ToString(),
                         Value = x.GenName.ToString()
                     }).ToList();
                 }             
             }           
            return returnValue;

        }


     public static IEnumerable<System.Web.Mvc.SelectListItem> DropDownListRoleContents()
     {
         List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
         using (pfizerEntities datacontext = new pfizerEntities())
         {
             var list = (from s in datacontext.GetRoles() select new { s.RoleName,s.PKID}).ToList();
             if (list != null)
             {
                 returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                 {
                     Text = x.RoleName.ToString(),
                     Value = x.PKID.ToString()
                 }).ToList();
             }
         }
         return returnValue;

     }


     public static IEnumerable<System.Web.Mvc.SelectListItem> GetEmployee()
     {
         List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
         using (pfizerEntities datacontext = new pfizerEntities())
         {
             var list = (from s in datacontext.GetEmployeeCreateUserMasterByXml() select new { s.EmployeeName, s.EmployeeFKID }).ToList();
             if (list != null)
             {
                 returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                 {
                     Text = x.EmployeeName.ToString(),
                     Value = x.EmployeeFKID.ToString()
                 }).ToList();
             }
         }
         return returnValue;

     }


     public static List<System.Web.Mvc.SelectListItem> GetRoles(string PKID,string type)
     {
        
         List<System.Web.Mvc.SelectListItem> returnRole = new List<System.Web.Mvc.SelectListItem>();
         
         using (pfizerEntities dataContext = new pfizerEntities())
         {
             if (type == "1")
             {


                 var item = (from a in dataContext.GetUserSelectedRoles(Convert.ToDecimal(PKID)) where a.Element == "Role" select new { a.PKID, a.RoleName, a.Element }).ToList().OrderBy(O => O.PKID).Distinct();


                 if (item.Count() > 0)
                 {
                     returnRole = item.Select(x => new System.Web.Mvc.SelectListItem()
                     {
                         Text = x.RoleName,
                         Value = x.PKID.ToString()
                     }).ToList();

                 }
             }
             else
             {
                 var item = (from a in dataContext.GetUserSelectedRoles(Convert.ToDecimal(PKID)) where a.Element != "Role" select new { a.PKID, a.RoleName, a.Element }).ToList().OrderBy(O => O.PKID).Distinct();


                 if (item.Count() > 0)
                 {
                     returnRole = item.Select(x => new System.Web.Mvc.SelectListItem()
                     {
                         Text = x.RoleName,
                         Value = x.PKID.ToString()
                     }).ToList();

                 }
             }

         }

         return returnRole;
     }

     public static IEnumerable<System.Web.Mvc.SelectListItem> GetBricks(int Territory_FKID)
     {
         List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
         using (pfizerEntities datacontext = new pfizerEntities())
         {
             var list = (from s in datacontext.GetAreaDetailsPCL(Territory_FKID) select new { s.AreaName, s.PKID }).ToList();
             if (list != null)
             {
                 returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                 {
                     Text = x.AreaName.ToString(),
                     Value = x.PKID.ToString()
                 }).ToList();
                 
             }
         }
         return new[]{new SelectListItem{Text="All", Value="All"}}.Concat(returnValue);

     }



     //public static IEnumerable<System.Web.Mvc.SelectListItem> GetYear()
     //{
     //    List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
     //    using (pfizerEntities datacontext = new pfizerEntities())
     //    {
     //        var list = (from s in datacontext.GetCycleYearMonth() select new { s., s.EmployeeFKID }).First();
     //        if (list != null)
     //        {
     //            returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
     //            {
     //                Text = x.EmployeeName.ToString(),
     //                Value = x.EmployeeFKID.ToString()
     //            }).ToList();
     //        }
     //    }
     //    return returnValue;

     //}


        

    }
}
