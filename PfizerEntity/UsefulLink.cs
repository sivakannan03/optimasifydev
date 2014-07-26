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


        #region Properties

        #region PCL TO MCL REQUEST

        public string Element { get; set; }
        public string Type { get; set; }
        public string MASPKID { get; set; }
        public string CustomerName { get; set; }
        public string Specialization { get; set; }
        public string AreaName { get; set; }
        public string CreatedDate { get; set; }
        public string ReasonforRequest { get; set; }

        public string LoginName { get; set; }
        public string Date { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; } 
        public string RejectedReason { get; set; }
        public int ID { get; set; }


        #endregion

        #region Call Volume Report

        public string PKID { get; set; }
        public string TeamName { get; set; }
        #endregion


        #endregion

        #region rajKumar
        public class DailyRptPSO
        {
            public string UserFKID { get; set; }
            public string EmpName { get; set; }
        }
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
                var list = (from s in datacontext.GetRoles() select new { s.RoleName, s.PKID }).ToList();
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

        public static List<System.Web.Mvc.SelectListItem> GetRoles(string PKID, string type)
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
                        Selected = true,
                        Text = x.AreaName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();

                }
            }
            return new[] { new SelectListItem { Text = "All", Value = "All" } }.Concat(returnValue);

        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetUsers(int PSO)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetPSONameforDrRepByXml(PSO) select new { s.UserFKID, s.PSOName }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.PSOName.ToString(),
                        Value = x.UserFKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetNodeName(int fkid)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetNodeByPSOFKID(fkid) select new { s.NodeFKID, s.NodeName }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.NodeName.ToString(),
                        Value = x.NodeFKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetUserName(int fkid)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetPSONamebyRepID(fkid) select new { s.UserFKID, s.EmpName }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.EmpName.ToString(),
                        Value = x.UserFKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetSDivision()
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetDivisonMasterByPkid() select new { s.PKID, s.DivName }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.DivName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetPSO(decimal userId)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetNodeByPSOFKIDForRegularVisit(userId) select new { s.NodeFKID, s.NodeName }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.NodeName.ToString(),
                        Value = x.NodeFKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

        #endregion

        #region Mathan
        public static IEnumerable<System.Web.Mvc.SelectListItem> GetUserNameList()
        {

            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetUserNameforUsageRepByXml() select new { s.PSOName, s.UserFKID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.PSOName.ToString(),
                        Value = x.UserFKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }


        public static IEnumerable<System.Web.Mvc.SelectListItem> GetTerritoryCodeForDoctor()
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetAllTerritoryCodeByXml() where s.TerName != "" select new { s.TerName, s.TerID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.TerName.ToString(),
                        Value = x.TerID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetPSONameList()
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {

                var list = (from s in datacontext.USP_GetPSONameForCustomer() select new { s.PSOName, s.pkid }).ToList();

                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.PSOName.ToString(),
                        Value = x.pkid.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetUserNameListForAudit()
        {

            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetUserNameByXML() select new { s.UserName, s.PKID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.UserName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }


        #endregion


        #region Baskar
        public static IEnumerable<System.Web.Mvc.SelectListItem> GetPSOName(int PSO)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetPsoNameByReport(PSO) select new { s.PKID, s.EmpName }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.EmpName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }
        #endregion
        #region MuniPrathap
        public static IEnumerable<System.Web.Mvc.SelectListItem> GetBrickDetails(int territory)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {

                var list = (from s in datacontext.GetPrescriptionBrickDetails(territory) select new { s.AreaName, s.PKID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.AreaName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

        //Loading Specilization For Prescriptiontracker
        public static IEnumerable<System.Web.Mvc.SelectListItem> GetSpecilizationDetails()
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetCyclePlanSpcialization() select new { s.Specialization, s.PKID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.Specialization.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }
        //Loading Product For Prescriptiontracker

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetProductDetails(int psoFkid)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetProductByTeamByXML(psoFkid) select new { s.ProductName, s.PKID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.ProductName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }
        //Loading Hospital For Prescriptiontracker

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetHospitalDetails(int psoFkid)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.PrescGetHospital(psoFkid) select new { s.Hospital, s.PKID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.Hospital.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }
        //Loading Stokiest For Prescriptiontracker
        public static IEnumerable<System.Web.Mvc.SelectListItem> GetStokiestDetails(int psoFkid)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.PrescGetStockist(psoFkid) select new { s.Stockist, s.PKID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.Stockist.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }
        
        #endregion

        #region Nagarajan
        public static IEnumerable<System.Web.Mvc.SelectListItem> DropDownBrikContents(int psoid)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.USP_GetAreaDetailsPCL(psoid) select new { s.AreaName, s.PKID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.AreaName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }


        public static IEnumerable<System.Web.Mvc.SelectListItem> GetUsersName(int psoid)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetTerNameByReport(psoid) select new { s.PKID, s.TerName }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.TerName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }


        public static IEnumerable<System.Web.Mvc.SelectListItem> GetUsersNamePSODM(int psoid)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetPsoNameByUserId(psoid) select new { s.PKID, s.EmpName }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.EmpName.ToString(),
                        Value = x.PKID.ToString()
                    }).ToList();
                }
            }
            return returnValue;

        }

       
        public static IEnumerable<System.Web.Mvc.SelectListItem> getMenuId(string keys)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetMenuForRole(keys) select new { s.MenuID, s.MenuHeading }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.MenuHeading.ToString(),
                        Value = x.MenuID.ToString()
                    }).ToList();

                }
            }
            return returnValue;

        }

        public static IEnumerable<System.Web.Mvc.SelectListItem> GetChileId(Int32 keys)
        {
            List<System.Web.Mvc.SelectListItem> returnValue = new List<System.Web.Mvc.SelectListItem>();
            using (pfizerEntities datacontext = new pfizerEntities())
            {
                var list = (from s in datacontext.GetMenuForChile(keys) select new { s.MasterID, s.MenuHeading, s.MenuSequence, s.ParentMenuID }).ToList();
                if (list != null)
                {
                    returnValue = list.Select(x => new System.Web.Mvc.SelectListItem()
                    {
                        Text = x.MenuHeading.ToString(),
                        Value = x.MasterID.ToString()
                    }).ToList();

                }
            }
            return returnValue;

        }
        #endregion
    }
}
