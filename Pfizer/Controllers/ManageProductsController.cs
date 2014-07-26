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
    public class ManageProductsController : Controller
    {

        private pfizerEntities genData = new pfizerEntities();
        ExportPdf pdf = new ExportPdf();
        string path = ConfigurationSettings.AppSettings["PDFfilePath"].ToString();
        //
        // GET: /genData/

        public ActionResult Index()
        {
            return View();
        }

        //Product Master .
        #region Nagarajan
        //Product Master .

        public ActionResult ProductMaster()
        {
            return View();
        }

        public JsonResult GridDataProductMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GET_PRODUCTMASTER() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "ProductName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
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
                        pageData = (from cust in query orderby cust.ProductName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.GenName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "GenName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.GenName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                    if (gridQueryModel.sidx == "METISProductName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.METISProductName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "METISProductName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.METISProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "IsKDP" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.IsKDP ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "IsKDP" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.IsKDP descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        public string ReturnProdMgeXml(string ProductManager)
        {

            string[] arrayUser = ProductManager.Split(',');
            string xml = "<root>";
            for (int i = 0; i < arrayUser.Count(); i++)
            {
                xml += "<CompBrand";
                xml += " ManagerFKID='" + arrayUser[i] + "'";
                xml += "/>";
            }
            xml += "</root>";
            return xml;
        }

        public string ReturnsXmlTeam(string Team)
        {

            string[] arrayUser = Team.Split(',');
            string xml = "<root>";
            for (int i = 0; i < arrayUser.Count(); i++)
            {
                xml += "<CompBrand";
                xml += " TeamFKID='" + arrayUser[i] + "'";
                xml += "/>";
            }
            xml += "</root>";
            return xml;
        }
        public string ProductPackXml(string ProductPackDetails)
        {
            string Xml = "";
            string[] ProductpackDetails1 = ProductPackDetails.Split('|');
            Xml = "<Root>";
            for (int i = 0; i < ProductpackDetails1.Length - 1; i++)
            {
                string[] temp = ProductpackDetails1[i].Split(',');

                Xml += "<ProductPack ";
                Xml += " ProdSamFKID='" + temp[0] + "'";
                Xml += " Productpack='" + temp[1] + "'";
                Xml += " FormFKID='" + temp[2] + "'";
                Xml += " IBISCode='" + temp[3] + "'";
                Xml += " SampleFlag='" + temp[4] + "'";
                Xml += " NonSampleFlag='" + temp[5] + "'";
                Xml += "/>";


            }
            Xml += "</Root>";
            return Xml;
        }
        public JsonResult ProductMasRetriverRecords(string Pkid)
        {
            dynamic query = (from a in genData.GetProductPackDetails(Convert.ToDecimal(Pkid)) select a).ToList();
            return Json(query);

        }
        public JsonResult AddProductMaster(string ProductGroup, string ProductCode, string ProductName, string NodeType, string IMSCode, string ProductManager, string Team, string IsKDP, string METISProductName, string ProductPackDetails)
        {
            string msg = "";
            string xmlProductManager = "";
            string xmlTeam = "";
            string ProdXml = "";
            decimal CreatedBy;
            string Metisprod;
            try
            {
                xmlProductManager = ReturnProdMgeXml(ProductManager);
                xmlTeam = ReturnsXmlTeam(Team);
                ProdXml = ProductPackXml(ProductPackDetails);
                CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                Metisprod = (METISProductName == null || METISProductName == "") ? "0" : METISProductName;
                genData.AddProductmaster(ProductCode, ProductName, Convert.ToDecimal(ProductGroup), Convert.ToDecimal(NodeType), Convert.ToDecimal(Metisprod), IMSCode, Convert.ToInt32(IsKDP), xmlProductManager, xmlTeam, ProdXml, Convert.ToInt32(CreatedBy));
              
                    msg = "Product Master added Successfully.";
              
              

                return Json(msg);
            }

            catch (Exception ex)
            {
                 return Json("Product Master already Exists");
              //  return Json(new { error = ex.Message });

            }
        }

     
        //Edit Control
        public JsonResult EditProductMaster1(string id, string oper, string ProductGroup, string ProductCode, string ProductName, string NodeType, string IMSCode, string ProductManager, string Team, string IsKDP, string METISProductName, string ProductPackDetails, string Status)
        {
            string msg ="";
            string xmlProductManager = "";
            string xmlTeam = "";
            string ProdXml = "";
            decimal CreatedBy;
            string Metisprod;
            int Statusfid;
            try
            {
                if (oper == "edit")
                {
                    if (Status == "Active")
                    {
                        Statusfid = 1;
                    }
                    else
                    {
                        Statusfid = 0;
                    }
                    xmlProductManager = ReturnProdMgeXml(ProductManager);
                    xmlTeam = ReturnsXmlTeam(Team);
                    ProdXml = ProductPackXml(ProductPackDetails);
                    CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    Metisprod = Convert.ToDecimal((METISProductName == null || METISProductName == "") ? "0" : METISProductName).ToString();

           
                   
                     genData.EditProductmaster(ProductCode, ProductName, Convert.ToDecimal(ProductGroup), Convert.ToDecimal(NodeType), Convert.ToDecimal(Metisprod), IMSCode, Convert.ToInt32(IsKDP), xmlProductManager, xmlTeam, ProdXml, Convert.ToInt32(CreatedBy), Statusfid, Convert.ToDecimal(id));
                     msg = "Product Master modified Successfully.";
                }
                 return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public JsonResult EditProductMaster(string id, string oper, string ProductGroup, string ProductCode, string ProductName, string NodeType, string IMSCode, string ProductManager, string Team, string IsKDP, string METISProductName, string ProductPackDetails, string Status)
        {
            string msg = "";
               try
                {
                    if (oper == "del")
                    {
                        var Result = genData.DeleteProductMaster(Convert.ToDecimal(id));
                        msg = "Product Master deleted Successfully.";
                    }
                   return Json(msg);
            }
          catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }


        //Product master Partial View on NodeTypeMaster & TeamMaster .

        public ActionResult GetNodeType(string type, string data)
        {
            dynamic query = "";
            dynamic edit = "";
            ViewBag.Type = type;
            ViewBag.Selected = 0;

            query = (from a in genData.GetNodeTypeByXML() select new { a.NodeTypeFKID, a.NodeName }).ToDictionary(f => f.NodeTypeFKID, f => f.NodeName.ToString());
            if (data != "null" && data != "")
            {
                var rslt = (from a in genData.GetProductByPKIDXml(Convert.ToDecimal(data)) select new { a.NodeTypeFKID }).FirstOrDefault();
                ViewBag.Selected = rslt.NodeTypeFKID;

            }
            return PartialView("PVProductMaster", query);
        }
        public ActionResult GetTeamType(string type, string data)
        {
            dynamic query = "";
            ViewBag.Type = type;

            query = (from a in genData.GetTeamByXML() select new { a.TeamFKID, a.TeamName }).ToDictionary(f => f.TeamFKID, f => f.TeamName.ToString());
            if (data != "null" && data != "")
            {
                query = (from a in genData.GetTeamforProductByXml(Convert.ToDecimal(data)) select new { a.TeamFKID, a.TeamName }).ToDictionary(f => Convert.ToDecimal(f.TeamFKID), f => f.TeamName.ToString());
            }
            //if (data == null || data == "null")
            //{
            //    query = (from a in genData.GetProductMgrByXml(0) where 1 == 2 select new { a.PKID, a.ProductManger }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.ProductManger.ToString());
            //    query = (from a in genData.GetTeamByXML() where 1==2 select new { a.TeamFKID, a.TeamName }).ToDictionary(f => f.TeamFKID, f => f.TeamName.ToString());
            //    return PartialView("PVProductMaster", query);
            //}
            //else
            //{
            //    query = (from a in genData.GetTeamforProductByXml(Convert.ToDecimal(data)) select new { a.TeamFKID, a.TeamName }).ToDictionary(f => Convert.ToDecimal(f.TeamFKID), f => f.TeamName.ToString());

            //}

            return PartialView("PVProductMaster", query);

        }

        public ActionResult ProductManagerLoad(string type, string NodeTypeFKID)
        {
            ViewBag.Type = type;
            dynamic query = "";
            if (NodeTypeFKID == "undefined")
            {
                query = (from a in genData.GetProductMgrByXml(0) where 1 == 2 select new { a.PKID, a.ProductManger }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.ProductManger.ToString());
                return PartialView("PVProductMaster", query);
            }
            else
            {
                ViewBag.Type = type;
                query = (from a in genData.GetProductMgrByXml(Convert.ToDecimal(NodeTypeFKID)) select new { a.PKID, a.ProductManger }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.ProductManger.ToString());
                return PartialView("PVProductMaster", query);
            }

        }
        public ActionResult ProductManagerLoads(string type, string data)
        {
            dynamic query;
            ViewBag.Type = type;

            if (data == null || data == "null")
            {
                query = (from a in genData.GetProductMgrByXml(0) where 1 == 2 select new { a.PKID, a.ProductManger }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.ProductManger.ToString());
                return PartialView("PVProductMaster", query);
            }
            else
            {
                query = (from a in genData.GetProductManagerByXml(Convert.ToDecimal(data)) select new { a.PKID, a.ProductManger }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.ProductManger.ToString());

            }

            return PartialView("PVProductMaster", query);


        }

        public ActionResult MetisProductLoads(string type, string data)
        {
            dynamic query = "";
            ViewBag.Type = type;

            query = (from e in genData.GetAllMetisProductName() select new { e.PKID, e.MPName }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.MPName.ToString());

            if (data != "null" && data != "")
            {
                query = (from a in genData.GetMappedMetisProductName(Convert.ToDecimal(data)) select new { a.PKID, a.ProductName }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.ProductName.ToString());
            }
            return PartialView("PVProductMaster", query);

        }

        public ActionResult PackFrmLoad(string type, int Dynamics)
        {
            ViewBag.Type = type;
            ViewBag.id = Convert.ToString("drpForm_" + Dynamics);
            var query = (from e in genData.GetFormforProductByXML() select new { e.formFKID, e.FormName }).ToDictionary(f => Convert.ToDecimal(f.formFKID), f => f.FormName.ToString());
            return PartialView("PVProductMaster", query);

        }

        public ActionResult NodeProductLinkMaster(string data)
        {

            dynamic query = "";
            if (data == "undefined" || data == null)
            {
                query = (from a in genData.USP_GET_PROD_NODETYPEMASTER(0) where 1 == 2 select new { a.NodeTypeFKID, a.NodeName }).ToDictionary(f => Convert.ToInt32(f.NodeTypeFKID), f => f.NodeName.ToString());
                return PartialView("NodeTypeMaster", query);
            }
            else
            {
                query = (from a in genData.USP_GET_PROD_NODETYPEMASTER(Convert.ToInt32(data)) select new { a.NodeTypeFKID, a.NodeName }).ToDictionary(f => Convert.ToInt32(f.NodeTypeFKID), f => f.NodeName.ToString());
                return PartialView("NodeTypeMaster", query);
            }

        }

        public ActionResult TeamProductlinkMaster(string data)
        {
            var query = (from e in genData.USP_GET_PROD_TEAMMASTER(Convert.ToInt32(data)) select new { e.TEAMFKID, e.TEAMNAME }).ToDictionary(f => Convert.ToInt32(f.TEAMFKID), f => f.TEAMNAME.ToString());
            return PartialView("NodeTypeMaster", query);
        }


        public ActionResult GetProductGroup(string data)
        {
            var query = (from e in genData.GetProductNameByXml() select new { e.PKID, e.ProductName }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.ProductName.ToString());
            return PartialView("PVProductGroup", query);
        }

        public ActionResult MetisProductName(string data)
        {
            var query = (from e in genData.GetAllMetisProductName() select new { e.PKID, e.MPName }).ToDictionary(f => Convert.ToDecimal(f.PKID), f => f.MPName.ToString());
            return PartialView("NodeTypeMaster", query);
        }
        //Product Master Excel,PDF and CSV File download
        public ActionResult ExportProductMasterToExcel()
        {
            try
            {
                var context = (from q in genData.USP_GET_PRODUCTMASTER() where q.ProductName != null select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                    item.GenName,
                    item.ProductName,
                    item.IsKDP,
                    item.METISProductName,
                    item.IsActive
                     }));
                return new ExcelResult(HeadersProductMaster, ColunmTypesProductMaster, data, "ProductMasters.xlsx", "ProductMasters");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

        }
        private static readonly string[] HeadersProductMaster =
        {
          "Product Group","Product Name","KDP","Metis Product","Status"
        };

        private static readonly DataForExcel.DataType[] ColunmTypesProductMaster =
       {
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,
        DataForExcel.DataType.String,                                        
        DataForExcel.DataType.String,                                        
        DataForExcel.DataType.String,                                        
                                
        };
        public ActionResult ExportProductMasterToPDF()
        {
            try
            {
                List<ProductMasters> userList = new List<ProductMasters>();
                var context = (from q in genData.USP_GET_PRODUCTMASTER() where q.ProductName != null select q).ToList();
                foreach (var item in context)
                {
                    ProductMasters user = new ProductMasters();
                    user.ProductGroup = item.GenName == null ? "" : item.GenName.ToString();
                    user.ProductName = item.ProductName == null ? "" : item.ProductName.ToString();
                    user.KDP = item.IsKDP == null ? "" : item.IsKDP.ToString();
                    user.MetisProduct = item.METISProductName == null ? "" : item.METISProductName.ToString();
                    user.Status = item.IsActive == null ? "" : item.IsActive.ToString();
                    userList.Add(user);

                }
                var customerList = userList;

                pdf.ExportPDF(customerList, new string[] { "ProductGroup", "ProductName", "KDP", "MetisProduct", "Status" }, path);
                return File(path, "application/pdf", "ProductMasters.pdf");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public class ProductMasters
        {
            public string ProductGroup { get; set; }
            public string ProductName { get; set; }
            public string KDP { get; set; }
            public string MetisProduct { get; set; }
            public string Status { get; set; }
        }
        public ActionResult ExportProductMasterToCsv()
        {
            try
            {
                var model = (from q in genData.USP_GET_PRODUCTMASTER() where q.ProductName != null select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("ProductGroup,ProductName,KDP,MetisProduct,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4}", record.GenName, record.ProductName, record.IsKDP, record.METISProductName, record.IsActive);
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);
                return File(csvBytes, "text/csv", "ProductMasters.txt");


            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        #endregion

        /* Anand Starts */

        #region Form Master
        public ActionResult FormMaster()
        {
            return View();
        }
        // Get Form Master Details
        public JsonResult GetFormMaster(GridQueryModel gridQueryModel)
        {
            try
            {


                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in genData.GetFormMaster() select q).ToList();
                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "FormName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.FormName != "" && sq.FormName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.FormName != "" && sq.FormName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.FormName != "" && sq.FormName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.FormName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.FormName != "" && sq.FormName.ToUpper().Contains(searchString) select sq).ToList();
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


        public JsonResult AddFormMaster(string FormName, string Status)
        {
            try
            {
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }
                string msg = string.Empty;
                var rslt = (from a in genData.FormMasters where a.FormName == FormName select a).ToList();
                if (rslt.Count() > 0)
                {
                    msg = "Form Master already Exists";
                }
                else
                {
                    FormMaster formMas = new FormMaster();
                    formMas.FormName = FormName.TrimStart();
                    formMas.IsActive = Convert.ToBoolean(Status);
                    formMas.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    formMas.CreatedDate = DateTime.Now;
                    genData.FormMasters.Add(formMas);
                    var Result = genData.SaveChanges();
                    if (Result == 1)
                    {
                        msg = "Form Master added Successfully.";
                    }
                }
                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public JsonResult EditFormMaster(string id, string oper, string PKID, string FormName, string Status)
        {
            try
            {
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }
                string msg = string.Empty;
                if (oper == "edit")
                {

                    Decimal ePKID = Convert.ToDecimal(id);

                    // var qry = (from q in ManageUser.LocationMasters select q).ToList();
                    var qry = (from a in genData.FormMasters where a.PKID == ePKID select a).Single();

                    qry.FormName = FormName;
                    //  qry.IsActive = Convert.ToBoolean(Status);
                    qry.IsActive = Convert.ToBoolean(Status);
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    qry.ModifiedDate = DateTime.Now;
                    if (qry.IsActive == false)
                        qry.IsActive = Convert.ToBoolean(Status);
                    var result = genData.SaveChanges();
                    if (result == 1)
                    {
                        msg = "Form Master Updated Successfully";
                    }

                }

                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in genData.FormMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    var Result = genData.SaveChanges();
                    if (Result == 1)
                    {
                        msg = "Form Master deleted Successfully.";
                    }

                }

                return Json(msg);
                // return Json(new { success = false, showMessage = true, message = msg, title = "Error" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public ActionResult ExportFormMasterToExcel()
        {
            try
            {
                var context = (from q in genData.GetFormMaster() orderby q.FormName ascending select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
               // item.PKID.ToString(),
                item.FormName,
                item.Status
               
            }));
                return new ExcelResult(HeadersQuestions, ColunmTypesQuestions, data, "FormMaster.xlsx", "FormMaster");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        private static readonly string[] HeadersQuestions = {
                "Form Name", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesQuestions = {
              //  DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportFormMasterToPDF()
        {
            try
            {
                List<FrmMaster> userList = new List<FrmMaster>();
                var context = (from q in genData.GetFormMaster() orderby q.FormName ascending select q).ToList();

                foreach (var item in context)
                {
                    FrmMaster user = new FrmMaster();
                    user.PKID = item.PKID.ToString();
                    user.FormName = item.FormName;
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
                                id = e.PKID,
                                cell = new string[]{
                            e.PKID.ToString(),e.FormName,e.Status
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] { "FormName", "Status" }, path);
                return File(path, "application/pdf", "FormMaster.pdf");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public ActionResult ExportFormMasterToCsv()
        {
            var model = (from q in genData.GetFormMaster() orderby q.FormName ascending select q).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Form Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1}", record.FormName, record.Status

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "FormMaster.txt");
        }
        public class FrmMaster
        {
            public string PKID { get; set; }
            public string FormName { get; set; }
            public string Status { get; set; }
        }
        #endregion
        #region Molecule
        public ActionResult Molecule()
        {
            return View();
        }
        // Get Form Master Details
        public JsonResult GetMolecule(GridQueryModel gridQueryModel)
        {
            try
            {


                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in genData.GetMoleculeMaster() select q).ToList();
                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "MoleculeName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query
                                     where sq.MoleculeName != null && sq.MoleculeName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)
                                     select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.MoleculeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.MoleculeName != null && sq.MoleculeName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.MoleculeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.MoleculeName != null && sq.MoleculeName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.MoleculeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.MoleculeName != null && sq.MoleculeName.ToUpper().Contains(searchString) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.MoleculeName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }

                    }

                }
                else
                {
                    count = query.Count();
                    if (gridQueryModel.sidx == "MoleculeName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.MoleculeName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "MoleculeName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.MoleculeName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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


        public JsonResult AddMoleculeMaster(string MoleculeName1, string Status1)
        {
            try
            {
                string msg = string.Empty;
                if (Status1 == "Active")
                {
                    Status1 = "True";
                }
                else
                {
                    Status1 = "False";
                }
                var resultCheck = (from a in genData.MoleculeMasters where a.MoleculeName == MoleculeName1 select a).ToList();
                if (resultCheck.Count > 0)
                {
                    msg = "Molecule Master already Exists";
                }
                else
                {
                    MoleculeMaster MoleculeMas = new MoleculeMaster();
                    MoleculeMas.MoleculeName = MoleculeName1.TrimStart();
                    MoleculeMas.IsActive = Convert.ToBoolean(Status1);
                    MoleculeMas.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    MoleculeMas.CreatedDate = DateTime.Now;
                    genData.MoleculeMasters.Add(MoleculeMas);
                    var Result = genData.SaveChanges();
                    if (Result == 1)
                    {
                        msg = "Molecule Master added Successfully.";
                    }
                }
                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }


        public JsonResult EditMolecule(string id, string oper, string PKID, string MoleculeName, string Status)
        {
            try
            {
                if (Status == "Active")
                {
                    Status = "True";
                }
                else
                {
                    Status = "False";
                }
                string msg = string.Empty;
                if (oper == "edit")
                {

                    Decimal ePKID = Convert.ToDecimal(id);

                    // var qry = (from q in ManageUser.LocationMasters select q).ToList();
                    var qry = (from a in genData.MoleculeMasters where a.PKID == ePKID select a).Single();

                    qry.MoleculeName = MoleculeName.TrimStart();
                    //  qry.IsActive = Convert.ToBoolean(Status);
                    qry.IsActive = Convert.ToBoolean(Status);
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    qry.ModifiedDate = DateTime.Now;
                    if (qry.IsActive == false)
                        qry.IsActive = Convert.ToBoolean(Status);
                    var result = genData.SaveChanges();
                    if (result == 1)
                    {
                        msg = "Molecule Master Updated Successfully";
                    }

                }
                if (oper == "del")
                {
                    var quote = Convert.ToInt32(id);
                    var delete_quote = (from a in genData.MoleculeMasters where a.PKID == quote select a).Single();
                    delete_quote.IsActive = false;
                    var Result = genData.SaveChanges();
                    if (Result == 1)
                    {
                        msg = "Molecule Master deleted Successfully.";
                    }

                }

                return Json(msg);
                // return Json(new { success = false, showMessage = true, message = msg, title = "Error" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public ActionResult ExportMoleculeToExcel()
        {
            try
            {
                var context = (from q in genData.GetMoleculeMaster() orderby q.MoleculeName ascending select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                //item.PKID.ToString(),
                item.MoleculeName,
                item.Status
               
            }));
                return new ExcelResult(HeadersMolecule, ColunmTypesMolecule, data, "Molecule.xlsx", "Molecule");
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        private static readonly string[] HeadersMolecule = {
                "MoleculeName", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesMolecule = {
              //  DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportMoleculeToPDF()
        {
            try
            {
                List<MoleculeMas> userList = new List<MoleculeMas>();
                var context = (from q in genData.GetMoleculeMaster() orderby q.MoleculeName ascending select q).ToList();

                foreach (var item in context)
                {
                    MoleculeMas user = new MoleculeMas();
                    user.PKID = item.PKID.ToString();
                    user.MoleculeName = item.MoleculeName;
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
                                id = e.PKID,
                                cell = new string[]{
                            e.PKID.ToString(),e.MoleculeName,e.Status
                        }
                            })
                    ).ToArray()
                };
                //  var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Sample2.pdf");
                // pdf.ExportPDF(customerList, new string[] {"MoleculeName", "Status" }, path);
                // return File(path, "application/pdf", "FormMaster.pdf");
                pdf.ExportPDF(customerList, new string[] { "MoleculeName", "Status" }, path);
                return File(path, "application/pdf", "MoleculeMaster.pdf");

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public ActionResult ExportMoleculeToCsv()
        {
            try
            {
                var model = (from q in genData.GetMoleculeMaster() orderby q.MoleculeName ascending select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Molecule Name,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1}", record.MoleculeName, record.Status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "Molecule.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class MoleculeMas
        {
            public string PKID { get; set; }
            public string MoleculeName { get; set; }
            public string Status { get; set; }
        }

        #endregion
        #region Product Type and Product Group Master

        public ActionResult ProductType(string GeneralFKID)
        {
            //Session["GeneralFKID"] = GeneralFKID;
            //ViewData["TableName"] = "ProductTypeMaster";
            //return View();

            ViewBag.FkId = GeneralFKID;
            Session["GeneralFKID"] = GeneralFKID;
            Session["TableType"] = "ProductTypeMaster";
            ViewData["TableName"] = "ProductTypeMaster";
            return View();
        }
        public ActionResult ProductGroup(string GeneralFKID)
        {
            //Session["GeneralFKID"] = GeneralFKID;
            //ViewData["TableName"] = "ProductGroupMaster";
            //return View();
            ViewBag.FkId = GeneralFKID;
            Session["GeneralFKID"] = GeneralFKID;
            Session["TableType"] = "ProductGroupMaster";
            ViewData["TableName"] = "ProductGroupMaster";
            return View();
        }

        // Get Form Master Details

        public JsonResult GetProductType(GridQueryModel gridQueryModel)
        {
            try
            {

                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                string GeneralFKID = Session["GeneralFKID"].ToString();
                var query = (from q in genData.GetProductType(GeneralFKID) select q).ToList();
                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "GenName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query
                                     where sq.GenName != null && sq.GenName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)
                                     select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.GenName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.GenName != null && sq.GenName.ToUpper().Contains(searchString) select sq).ToList();
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
        public JsonResult DeleteProductType(string id, string oper, string PKID, string GenCode, string TableName, string GenName, string Status)
        {
            try
            {
                int Result = genData.DeleteGeneralMaster(Convert.ToDecimal(id));

                if (Result == 1)
                {

                    id = "ProductType deleted Successfully.";


                }
                return Json(id);
            }
            catch (Exception msg)
            {
                return Json(msg);
            }
        }
        public JsonResult AddProductGroupMaster(string TableName1, string GenCode1, string GenName1, string Status1)
        {
            try
            {
                if (Status1 == "Active")
                {
                    Status1 = "True";
                }
                else
                {
                    Status1 = "False";
                }
                string msg = string.Empty;
                var rslt = (from a in genData.GeneralMasters where a.GenName == GenName1 select a).ToList();
                if (rslt.Count() > 0)
                {                       // msg = "ProductGroup Master already Exists";
                    if (TableName1 == "ProductTypeMaster")
                        msg = "ProductType Master already Exists";
                    else
                        msg = "ProductGroup Master already Exists";
                }
                else
                {
                    GeneralMaster gm = new GeneralMaster();
                    gm.GenFKID = Convert.ToDecimal(Session["GeneralFKID"] == null ? "0" : Session["GeneralFKID"].ToString());//24;
                    gm.GenCode = GenCode1.TrimStart();
                    gm.GenName = GenName1.TrimStart();
                    gm.IsActive = true;
                    gm.CreatedDate = DateTime.Now;
                    gm.CreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                    genData.GeneralMasters.Add(gm);
                    genData.SaveChanges();

                    if (TableName1 == "ProductTypeMaster")
                    {
                        msg = "ProductType added Successfully.";
                    }
                    else
                    {
                        msg = "ProductGroup added Successfully.";
                    }
                }

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }

        public JsonResult EditProductType(string id, string oper, string PKID, string GenCode, string TableName, string GenName, string Status)
        {
            try
            {

                //   Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                //  Decimal GeneralFKID = Convert.ToDecimal(Session["GeneralFKID"] == null ? "0" : Session["GeneralFKID"].ToString());
                string msg = string.Empty;
                if (oper == "edit")
                {


                    int ePKID = Convert.ToInt32(id);
                    var qry = (from a in genData.GeneralMasters where a.PKID == ePKID select a).Single();

                    qry.GenCode = GenCode.TrimStart();
                    qry.GenName = GenName.TrimStart();
                    qry.ModifiedDate = DateTime.Now;
                    qry.ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                    if (Status == "on" || Status == "Active")
                        qry.IsActive = true;
                    else if (Status == "off" || Status == "Inactive")
                        qry.IsActive = false;

                    genData.SaveChanges();
                    if (TableName == "ProductTypeMaster")
                    {
                        msg = "ProductType Updated Successfully.";
                    }
                    else
                    {
                        msg = "ProductGroup Updated Successfully.";
                    }
                }
                if (oper == "del")
                {

                    int Result = genData.DeleteGeneralMaster(Convert.ToDecimal(id));
                    if (Result == 1)
                    {
                        msg = "ProductGroup deleted Successfully.";

                    }

                }

                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });

            }
        }
        public ActionResult ExportProductTypeToExcel()
        {
            try
            {
                string GeneralFKID = Session["GeneralFKID"].ToString();
                var context = (from q in genData.GetProductType(GeneralFKID) orderby q.GenName ascending select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
              //  item.PKID.ToString(),
                item.TableName,
                item.GenName,
                item.Status
               
            }));
                return new ExcelResult(HeadersProductType, ColunmTypesProductType, data, "ProductType.xlsx", "ProductType");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersProductType = {
               "TableType","Name", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesProductType = {
                //DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
            };
        public ActionResult ExportProductTypeToPDF()
        {
            try
            {
                List<ProductTypeMas> userList = new List<ProductTypeMas>();
                string GeneralFKID = Session["GeneralFKID"].ToString();
                var context = (from q in genData.GetProductType(GeneralFKID) where q.GenName != null orderby q.GenName ascending select q).ToList();

                foreach (var item in context)
                {
                    ProductTypeMas user = new ProductTypeMas();
                    user.PKID = item.PKID.ToString();
                    user.TableType = item.TableName;
                    user.Name = item.GenName;
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
                                id = e.PKID,
                                cell = new string[]{
                            e.PKID.ToString(),e.TableType,e.Name,e.Status
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] { "TableType", "Name", "Status" }, path);
                return File(path, "application/pdf", "ProductType.pdf");

            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportProductTypeToCsv()
        {
            try
            {
                string GeneralFKID = Session["GeneralFKID"].ToString();
                var model = (from q in genData.GetProductType(GeneralFKID) orderby q.GenName ascending select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("Table Type,Name,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2}", record.TableName, record.GenName, record.Status

                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "ProductType.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class ProductTypeMas
        {
            public string PKID { get; set; }
            public string TableType { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
        }

        #endregion
        #region Product Molecule Link
        public ActionResult ProductMoleculeLink()
        {
            return View();
        }
        public JsonResult GetProductMoleculeLink(GridQueryModel gridQueryModel)
        {
            try
            {


                //   decimal QrygeneralFKID = Convert.ToDecimal(fkId);
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;
                var query = (from q in genData.GetProductMoleculeLink1() select q).ToList();
                //var query = (from a in ManageUser.LocationMasters select a).ToList();
                var count = query.Count();
                var pageData = query.Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);


                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "ProductName") || (searchField == "MoleculeName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.MoleculeName.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "eq") //equal
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().Equals(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.MoleculeName.Equals(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "ew") // ends with
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) || sq.MoleculeName.EndsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
                            count = q.Count();
                            pageData = q.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                        }
                        else if (searchOper == "cn")//contains
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().Contains(searchString) || sq.MoleculeName.Contains(searchString) select sq).ToList();
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
                    else
                        if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ProductName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
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
        public JsonResult EditProductMoleculeLink(string id, string oper, string MoleculeFKID, string ProductFKID, string ProductName, string MoleculeName, string Status)
        {
            try
            {
                string msg = string.Empty;
                StringBuilder SbMoleculeFKID = new StringBuilder();

                var MFKID = MoleculeName.Split(',');

                if (Status == "Active")
                {
                    Status = "1";
                }
                else
                {
                    Status = "0";
                }
                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());


                SbMoleculeFKID.Append("<root>");
                foreach (var record in MFKID)
                {
                    SbMoleculeFKID.Append("<CompBrand");
                    SbMoleculeFKID.Append(" MoleculeFKID='" + record + "'");
                    SbMoleculeFKID.Append("/>");
                }
                SbMoleculeFKID.Append("</root>");



                string xmlMoleculeFKID = SbMoleculeFKID.ToString();
                if (oper == "edit")
                {
                    int result = genData.EditMoleculetest1(Convert.ToInt32(MoleculeFKID), Convert.ToDecimal(ProductFKID), xmlMoleculeFKID, Convert.ToInt32(ModifiedBy), Convert.ToInt32(Status));

                    if (result == 1)
                    {
                        msg = "ProductMoleculeLink Updated Successfully.";
                    }

                }
                if (oper == "add")
                {


                    //  var Result = string.Empty;
                    int result = genData.AddProductMoleculeLinkMaster(Convert.ToDecimal(ProductFKID), xmlMoleculeFKID, Convert.ToInt32(ModifiedBy));
                    if (result == 1)
                    {
                        msg = "ProductMoleculeLink Added Successfully.";
                    }

                }
                if (oper == "del")
                {
                    int result = genData.DeleteMoleculeProductMaster(Convert.ToDecimal(ProductFKID), Convert.ToDecimal(xmlMoleculeFKID));
                    if (result == 1)
                    {
                        msg = "ProductMoleculeLink deleted Successfully.";
                    }

                }

                return Json(msg);
                // return Json(new { success = false, showMessage = true, message = msg, title = "Error" });
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult ExportProductMoleculeLinkToExcel()
        {
            try
            {
                var context = (from q in genData.GetProductMoleculeLink1() select q).ToList();
                if (context.Count == 0)
                    return new EmptyResult();
                var data = new List<string[]>(context.Count);
                data.AddRange(context.Select(item => new[] {
                item.PKID.ToString(),
                item.ProductFKID.ToString(),
                item.MoleculeFKID.ToString(),
                item.ProductName,
                item.MoleculeName,
                item.Status
                
               
            }));
                return new ExcelResult(HeadersProductMoleculeLink, ColunmTypesProductMoleculeLink, data, "ProductMoleculeLink.xlsx", "ProductMoleculeLink");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        private static readonly string[] HeadersProductMoleculeLink = {
                "PKID", "ProductFKID", "MoleculeFKID", "ProductName", "MoleculeName", "Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesProductMoleculeLink = {
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String

            };
        public ActionResult ExportProductMoleculeLinkToPDF()
        {
            try
            {
                List<ProductMolecule> userList = new List<ProductMolecule>();
                var context = (from q in genData.GetProductMoleculeLink1() select q).ToList();

                foreach (var item in context)
                {
                    ProductMolecule user = new ProductMolecule();
                    user.PKID = item.PKID.ToString();
                    user.ProductFKID = item.ProductFKID.ToString();
                    user.MoleculeFKID = item.MoleculeFKID.ToString();
                    user.ProductName = item.ProductName;
                    user.MoleculeName = item.MoleculeName;
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
                                id = e.PKID,
                                cell = new string[]{
                            e.PKID.ToString(),e.ProductFKID,e.MoleculeFKID,e.ProductName,e.MoleculeName,e.Status
                        }
                            })
                    ).ToArray()
                };

                pdf.ExportPDF(customerList, new string[] { "PKID", "ProductFKID", "MoleculeFKID", "ProductName", "MoleculeName", "Status" }, path);
                return File(path, "application/pdf", "ProductMoleculeLink.pdf");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public class ProductMolecule
        {
            public string PKID { get; set; }
            public string ProductFKID { get; set; }
            public string MoleculeFKID { get; set; }
            public string ProductName { get; set; }
            public string MoleculeName { get; set; }
            public string Status { get; set; }
        }



        public ActionResult ExportProductMoleculeLinkToCsv()
        {
            try
            {
                var model = (from q in genData.GetProductMoleculeLink1() select q).ToList();
                var sb = new StringBuilder();
                sb.AppendLine("PKID,ProductFKID,MoleculeFKID,ProductName,MoleculeName,Status");
                foreach (var record in model)
                {
                    sb.AppendFormat("{0},{1},{2},{3},{4},{5}", record.PKID, record.ProductFKID, record.MoleculeFKID, record.ProductName, record.MoleculeName, record.Status
                     );
                    sb.AppendLine();
                }
                string data = sb.ToString();
                var csvBytes = Encoding.ASCII.GetBytes(data);

                return File(csvBytes, "text/csv", "ProductMoleculeLink.txt");
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult GetProduct()
        {
            try
            {
                var query = (from e in genData.GetProductforProductMatrixByXml() select new { e.PKID, e.ProductName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.ProductName.ToString());

                return PartialView("ProductMolecule", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }
        public ActionResult MoleculeName()
        {
            try
            {
                var query = (from e in genData.GetMoleculeforProductByXML() select new { e.MoleculeID, e.MoleculeName }).ToDictionary(f => Convert.ToInt32(f.MoleculeID), f => f.MoleculeName.ToString());

                return PartialView("ProductMolecule", query);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }

        }


        #endregion




        //Sample Acknowledgement

        public ActionResult SampleAcknowledgement()
        {
            return View();
        }

        public JsonResult GridDataSampleAcknowledgement(GridQueryModel gridQueryModel)
        {
            string Territory_FKID = Session["Territory_FKID"].ToString();
            decimal UserFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
            var query = (from q in genData.GetPSOSample(UserFKID) select q).ToList();
            var pageData = query.OrderBy(x => x.EffectiveDate).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

            return Json(new
            {
                page = gridQueryModel.page,
                records = query.Count(),
                rows = pageData,
                total = Math.Ceiling((decimal)query.Count() / gridQueryModel.rows)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateGridData(string gridData)
        {
            string Territory_FKID = Session["Territory_FKID"].ToString();
            decimal UserFKID = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

            var serialize = new JavaScriptSerializer();
            var deserialize = serialize.Deserialize<List<Acknowledgement>>(gridData);

            StringBuilder sb = new StringBuilder();
            sb.Append("<row>");

            foreach (var item in deserialize)
            {
                if (Convert.ToUInt32(item.Pendingack) != 0)
                {
                    var TotalAckqty = Convert.ToUInt32(item.AckQty) + Convert.ToUInt32(item.AcknowledgeQty);
                    var IBalanceQty = Convert.ToUInt32(item.Pendingack) - Convert.ToUInt32(item.AckQty);
                    sb.Append("<dp PKID='" + item.PKID);
                    sb.Append("'");
                    sb.Append(" AcknowledgeQty='" + TotalAckqty);
                    sb.Append("'");
                    sb.Append(" BalanceQty='" + IBalanceQty);
                    sb.Append("'");

                    sb.Append(" Clsbal='" + Convert.ToUInt32(item.AckQty));
                    sb.Append("'");
                    sb.Append(" RShortfall='" + item.ReasonForShortfall);
                    sb.Append("'");

                    sb.Append(" Territory='" + Territory_FKID);
                    sb.Append("'");
                    sb.Append(" ModifiedBy='" + UserFKID);
                    sb.Append("'/>");
                }
            }
            sb.Append("</row>");
            genData.SampleAllocationAck(sb.ToString());
            return Json("");
        }



        //ProductDetailingMatrix 

        public ActionResult ProductDetailingMatrix()
        {
            return View();
        }

        public JsonResult GridDataProductDetailingMatrix(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_GET_ProductDetailingMatrix() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "ProductName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
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
                        pageData = (from cust in query orderby cust.ProductName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.Specialization ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "Specialization" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.Specialization descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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


        public JsonResult AddProductDetailingMatrix(string ProductName, string SelSpecialization)
        {
            try
            {
                string msg = string.Empty;
                string str = "";
                decimal StrcreatedBy;
                string StrProductName = "";

                StrcreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                StrProductName = ProductName;

                str = "<root>";
                string StrSelSpecialization = SelSpecialization;
                string[] SelSpecializationvalues = StrSelSpecialization.Split(',');
                for (int i = 0; i < SelSpecializationvalues.Length; i++)
                {
                    str += "<CompBrand";
                    str += " SpecialityFKID ='" + SelSpecializationvalues[i] + "'";
                    str += "/>";
                }
                str += "</root>";

                genData.AddProductSpecialityLinkMaster(Convert.ToDecimal(StrProductName), str, Convert.ToInt32(StrcreatedBy));
                msg = "Product Detailing Matrix Master Added Successfully.";
                return Json(msg);
            }
            catch (Exception ex)
            {
                return Json("Product Detailing Matrix Master already Exists");
            }
        }
        public JsonResult EditProductDetailingMatrix(string id, string oper, string ProductFKID, string ProductName, string SelSpecialization, string SpecialityFKID, string IsActive)
        {

            string str = string.Empty;
            decimal StrcreatedBy;
            decimal StrModifiedBy;
            string StrProductFKID = string.Empty;
            bool eIsactive = true;
            string StrSpecialityFKID = string.Empty;

            if (oper == "edit")
            {
                StrcreatedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                StrModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());
                StrProductFKID = ProductFKID;
                StrSpecialityFKID = SpecialityFKID;


                str = "<root>";
                string StrSelSpecialization = SelSpecialization;
                string[] SelSpecializationvalues = StrSelSpecialization.Split(',');
                for (int i = 0; i < SelSpecializationvalues.Length; i++)
                {
                    str += "<CompBrand";
                    str += " SpecialityFKID ='" + SelSpecializationvalues[i] + "'";
                    str += "/>";
                }
                str += "</root>";

                genData.EditSpecMaster(Convert.ToInt32(StrSpecialityFKID), Convert.ToDecimal(StrProductFKID), str, Convert.ToInt32(StrcreatedBy), Convert.ToInt32(eIsactive));

                id = "Product Detailing Matrix Master modified Successfully.";

            }

            if (oper == "del")
            {
                if (Session["ProductFKID"] != null)
                    StrProductFKID = Session["ProductFKID"].ToString();
                if (Session["SpecialityFKID"] != null)
                    StrSpecialityFKID = Session["SpecialityFKID"].ToString();
                if (StrProductFKID != "" && StrSpecialityFKID != "")
                    genData.DeleteProductSpecialityLinkMaster(Convert.ToDecimal(StrProductFKID), Convert.ToDecimal(StrSpecialityFKID));
                id = "Product Detailing Matrix Master deleted Successfully.";
            }

            return Json(id);
        }



        //ProductDetailingMatrix Delete Position Get TeamFKID,RegionFKID.

        public JsonResult GetProductFKID(string ProductFKID, string SpecialityFKID)
        {

            Session["ProductFKID"] = ProductFKID;
            Session["SpecialityFKID"] = SpecialityFKID;
            return Json(new
            {
                areaItems = "",
                areaCount = "",
            }, JsonRequestBehavior.AllowGet);
        }

        //Dropdown SelectedIndex Changed ...
        public ActionResult GetSelectedIndexChangedProductName(string Type, string ProductFKID)
        {
            dynamic query = "";
            ViewBag.Type = "";
            query = (from a in genData.GetSpecforProductPKIDByXMLN(Convert.ToInt32(ProductFKID)) select new { a.SpecilizationId, a.Specialization }).ToDictionary(f => Convert.ToInt32(f.SpecilizationId), f => f.Specialization.ToString());
            return PartialView("PVMPSelectedDDL", query);
        }


        public ActionResult GetSelectedIndexChangedSelSpecialization(string Type, string ProductFKID)
        {
            // var query = "";
            ViewBag.Type = "SelSpecialization";
            var query = (from a in genData.GetSpecforProductPKIDByXMLNNotInN(Convert.ToInt32(ProductFKID)) select new { a.SpecilizationId, a.Specialization }).ToDictionary(f => Convert.ToInt32(f.SpecilizationId), f => f.Specialization.ToString());
            return PartialView("PVMPSelectedDDL", query);
        }

        //ProductDetailingMatrix Partial View

        public ActionResult NotSelectedSpecialization(string Type, string ProductFKID)
        {
            ViewBag.Type = Type;
            dynamic query = "";
            if (ProductFKID == "undefined")
            {

                query = (from a in genData.GetSpecializationforProductMatrixByXML() select new { a.SpecializationFKID, a.Specialization }).ToDictionary(f => Convert.ToInt32(f.SpecializationFKID), f => f.Specialization.ToString());
                return PartialView("PVMPSelectedDDL", query);
            }
            else
            {

                ViewBag.Type = Type;
                query = (from a in genData.GetSpecforProductPKIDByXMLN(Convert.ToInt32(ProductFKID)) select new { a.SpecilizationId, a.Specialization }).ToDictionary(f => Convert.ToInt32(f.SpecilizationId), f => f.Specialization.ToString());
                return PartialView("PVMPSelectedDDL", query);
            }


        }

        public ActionResult SelectedSpecialization(string Type, string ProductFKID)
        {
            ViewBag.Type = Type;
            dynamic query = "";
            if (ProductFKID == "undefined")
            {
                query = (from a in genData.GetAllTeamRegionLinkByXml("0") where 1 == 2 select new { a.RegionFKID, a.RegionName }).ToDictionary(f => Convert.ToInt32(f.RegionFKID), f => f.RegionName.ToString());
                return PartialView("PVMPSelectedDDL", query);
            }
            else
            {

                query = (from a in genData.GetSpecforProductPKIDByXMLNNotInN(Convert.ToInt32(ProductFKID)) select new { a.SpecilizationId, a.Specialization }).ToDictionary(f => Convert.ToInt32(f.SpecilizationId), f => f.Specialization.ToString());
                return PartialView("PVMPSelectedDDL", query);
            }
        }

        public ActionResult LoadProductName(string Type, string PKID)
        {
            if (PKID == "undefined")
            {
                ViewBag.Type = Type;
                var query = (from a in genData.GetProductforProductMatrixByXml() select new { a.PKID, a.ProductName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.ProductName.ToString());
                return PartialView("PVMPSelectedDDL", query);
            }
            else
            {
                ViewBag.Type = Type;
                var query = (from a in genData.GetProductforSpecialityByXML(Convert.ToDecimal(PKID)) select new { a.PKID, a.ProductName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.ProductName.ToString());
                return PartialView("PVMPSelectedDDL", query);
            }

        }





        //ProductDetailingMatrix export excel,pdf,csv

        public ActionResult ExportProductDetailingMatrixToExcel()
        {
            var context = (from a in genData.USP_GET_ProductDetailingMatrix() orderby a.ProductName ascending select a).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
               // item.PKID.ToString(),
                item.ProductName==null?"":item.ProductName,                
                item.Specialization==null?"":item.Specialization,                
                item.IsActive
               
            }));
            return new ExcelResult(HeadersProductDetailingMatrix, ColunmTypesProductDetailingMatrix, data, "ProductDetailingMatrix.xlsx", "ProductDetailingMatrix");
        }
        private static readonly string[] HeadersProductDetailingMatrix = {
                 "Product Name","Specialization","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesProductDetailingMatrix = {
                DataForExcel.DataType.String,                                            
                DataForExcel.DataType.String, 
                DataForExcel.DataType.String
            };
        public ActionResult ExportProductDetailingMatrixToPDF()
        {
            List<ProductDetailingMatrixTypes> userList = new List<ProductDetailingMatrixTypes>();
            var context = (from a in genData.USP_GET_ProductDetailingMatrix() orderby a.ProductName ascending select a).ToList();

            foreach (var item in context)
            {
                ProductDetailingMatrixTypes user = new ProductDetailingMatrixTypes();
                user.ProductName = item.ProductName == null ? "" : item.ProductName;
                user.Specialization = item.Specialization == null ? "" : item.Specialization;
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
                            e.ProductName,e.Specialization,e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "ProductName", "Specialization", "Status" }, path);
            return File(path, "application/pdf", "ProductDetailingMatrix.pdf");
        }
        public ActionResult ExportProductDetailingMatrixToCsv()
        {
            var model = (from a in genData.USP_GET_ProductDetailingMatrix() orderby a.ProductName ascending select a).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("HQ Name,Territory Code,Territory Name,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.ProductName == null ? "" : record.ProductName, record.Specialization == null ? "" : record.Specialization, record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "ProductDetailingMatrix.txt");
        }
        public class ProductDetailingMatrixTypes
        {
            public string ProductName { get; set; }
            public string Specialization { get; set; }
            public string Status { get; set; }
        }

        #region ProductMoleculeLinkMaster

        #region ProductMoleculeLinkMaster ActionController

        public ActionResult ProductMoleculeLinkMaster()
        {
            return View();
        }

        #endregion

        #region Bind Datas To Grid

        public JsonResult GetProductMoleculeLinkMaster(GridQueryModel gridQueryModel)
        {
            try
            {
                var searchString = gridQueryModel.searchString == null ? "" : gridQueryModel.searchString.ToUpper();
                var searchOper = gridQueryModel.searchOper;
                var searchField = gridQueryModel.searchField;

                var query = (from a in genData.USP_MoleculeProductMasterGrid() select a).ToList();
                var count = query.Count();
                var pageData = query.OrderBy(x => x.ProductName).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                if (gridQueryModel._search == true && searchString != "")
                {
                    if ((searchField == "ProductName"))
                    {
                        if (searchOper == "bw")//begins with
                        {
                            var q = (from sq in query where sq.ProductName != null && sq.ProductName.ToUpper().StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase) select sq).ToList();
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
                        pageData = (from cust in query orderby cust.ProductName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "ProductName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.ProductName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

                    if (gridQueryModel.sidx == "MoleculeName" && gridQueryModel.sord == "asc")
                        pageData = (from cust in query orderby cust.MoleculeName ascending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);
                    else
                        if (gridQueryModel.sidx == "MoleculeName" && gridQueryModel.sord == "desc")
                            pageData = (from cust in query orderby cust.MoleculeName descending select cust).Skip((gridQueryModel.page - 1) * gridQueryModel.rows).Take(gridQueryModel.rows);

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

        #endregion

        #region ProductMoleculeLinkMaster PartialView

        //AddDetails In DropDown ...
        public ActionResult GetProductNameForMoleculeLink(string Type)
        {
            dynamic query = "";
            ViewBag.Type = Type;
            query = (from a in genData.GetProductforProductMatrixByXml() select new { a.PKID, a.ProductName }).ToDictionary(f => Convert.ToInt32(f.PKID), f => f.ProductName.ToString());
            return PartialView("PVProductMoleculeLinkMaster", query);
        }

        public ActionResult GetMoleculeNameLink(string Type)
        {
            dynamic query = "";
            ViewBag.Type = Type;
            query = (from a in genData.GetMoleculeforProductByXML() select new { a.MoleculeID, a.MoleculeName }).ToDictionary(f => Convert.ToInt32(f.MoleculeID), f => f.MoleculeName.ToString());
            return PartialView("PVProductMoleculeLinkMaster", query);
        }
        public ActionResult SelectedDummyName(string Type, string data)
        {

            ViewBag.Type = Type;
            dynamic query = "";
            if (data == "undefined")
            {
                query = (from a in genData.GetMoleculeforProductByXML() where 1 == 2 select new { a.MoleculeID, a.MoleculeName }).ToDictionary(f => Convert.ToInt32(f.MoleculeID), f => f.MoleculeName.ToString());
                return PartialView("PVProductMoleculeLinkMaster", query);
            }
            else
            {
                query = (from a in genData.GetMoleculeforProductPKIDByXMLNotInN(Convert.ToInt32(data)) select new { a.MoleculeId, a.MoleculeName }).ToDictionary(f => Convert.ToInt32(f.MoleculeId), f => f.MoleculeName.ToString());
                return PartialView("PVProductMoleculeLinkMaster", query);
            }

        }

        #endregion

        #region Delete Product MoleculeLink

        public JsonResult DeleteProductMoleculeLink(string MoleculeFKID, string ProductFKID)
        {
            var msg = "";
            try
            {
                genData.DeleteMoleculeProductMaster(Convert.ToDecimal(ProductFKID), Convert.ToDecimal(MoleculeFKID));
                msg = "Product Molecule Link Master Deleted Successfully.";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return Json(msg);
        }

        #endregion

        #region Add MoleculeLink

        public JsonResult AddProductMoleculelink(string ProductName, string SelMoleculeName)
        {
            try
            {
                string msg = string.Empty;
                string str = "";

                Decimal ModifiedBy = Convert.ToDecimal(Session["USER_FKID"] == null ? "0" : Session["USER_FKID"].ToString());

                str = "<root>";
                string StrSelectedSelMolecule = SelMoleculeName;
                string[] SelectedSelMoleculevalues = StrSelectedSelMolecule.Split(',');

                for (int i = 0; i < SelectedSelMoleculevalues.Length; i++)
                {
                    str += "<CompBrand";
                    str += " MoleculeFKID ='" + SelectedSelMoleculevalues[i] + "'";
                    str += "/>";
                }

                str += "</root>";

                genData.AddProductMoleculeLinkMaster(Convert.ToDecimal(ProductName), str, Convert.ToInt32(ModifiedBy));
                msg = "Product Molecule Link Master Added Successfully.";
                return Json(msg);

            }
            catch (Exception ex)
            {
                return Json("Product Molecule Link Master already Exists");
            }

        }
        #endregion

        #region Edit MoleculeLink

        public JsonResult EditProductMoleculeLinkMaster(string id, string oper, string ProductName, string MoleculeFKID, string SelMoleculeName, string total, string IsActive)
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
                    string StrSelectedSelMolecule = SelMoleculeName;
                    string[] SelectedSelMoleculevalues = StrSelectedSelMolecule.Split(',');
                    for (int i = 0; i < SelectedSelMoleculevalues.Length; i++)
                    {
                        str += "<CompBrand";
                        str += " MoleculeFKID ='" + SelectedSelMoleculevalues[i] + "'";
                        str += "/>";
                    }
                    str += "</root>";


                    int result = genData.EditMoleculetest1(Convert.ToInt32(MoleculeFKID), Convert.ToDecimal(ProductName), str, Convert.ToInt32(ModifiedBy), Convert.ToInt32(IsActive));

                    if (result >= 1)
                    {
                        msg = "Product Molecule Link Master Updated Successfully.";
                    }


                }

                //if (oper == "del")
                //{

                //    int result = genData.DeleteMoleculeProductMaster(Convert.ToDecimal(id), Convert.ToDecimal(MoleculeFKID));
                //    if (result == 1)
                //    {
                //       msg = "Product Molecule Link Master Deleted Successfully.";
                //    }

                //}

                return Json(msg);
            }
            catch (Exception ex)
            { return Json(new { error = ex.Message }); }
        }

        #endregion

        #region Convert Excel,PDF,CSV File Format

        #region Convert To Excel

        public ActionResult ExportProductMoleculeLinkMasterToExcel()
        {
            var context = genData.USP_MoleculeProductMasterGrid().Select(x => new
            {
                PKID = x.PKID,
                x.ProductName,
                x.MoleculeName,
                IsActive = x.IsActive == "1" ? "Active" : "In Active",
            }).OrderBy(o => o.ProductName).ToList();
            if (context.Count == 0)
                return new EmptyResult();
            var data = new List<string[]>(context.Count);
            data.AddRange(context.Select(item => new[] {
                item.ProductName.ToString()==null ?"":item.ProductName.ToString(),
                item.MoleculeName==null?"":item.MoleculeName,
                item.IsActive 
               
            }));
            return new ExcelResult(HeadersProductMoleculeLinkMaster, ColunmTypesProductMoleculeLinkMaster, data, "ProductMoleculeLinkMaster.xlsx", "Product MoleculeLink Master");
        }
        private static readonly string[] HeadersProductMoleculeLinkMaster = {
               "Product Name","Molecule Name","Status"
            };
        private static readonly DataForExcel.DataType[] ColunmTypesProductMoleculeLinkMaster = { 
               
                DataForExcel.DataType.String,
                DataForExcel.DataType.String,
                DataForExcel.DataType.String
                
            };

        #endregion

        #region Convert to PDF

        public ActionResult ExportProductMoleculeLinkMasterToPDF()
        {
            List<ProductMoleculeLinkMasterPDF> userList = new List<ProductMoleculeLinkMasterPDF>();
            var context = genData.USP_MoleculeProductMasterGrid().Select(x => new
            {
                PKID = x.PKID,
                x.ProductName,
                x.MoleculeName,
                IsActive = x.IsActive == "1" ? "Active" : "In Active",
            }).OrderBy(o => o.ProductName).ToList();


            foreach (var item in context)
            {
                ProductMoleculeLinkMasterPDF user = new ProductMoleculeLinkMasterPDF();


                user.ProductName = item.ProductName == null ? "" : item.ProductName;
                user.MoleculeName = item.MoleculeName == null ? "" : item.MoleculeName;
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
                            e.ProductName,
                            e.MoleculeName,
                            e.Status
                        }
                        })
                ).ToArray()
            };

            pdf.ExportPDF(customerList, new string[] { "ProductName", "MoleculeName", "Status" }, path);
            return File(path, "application/pdf", "ProductMoleculeLinkMaster.pdf");
        }

        #endregion

        #region Convert To CSV

        public ActionResult ExportProductMoleculeLinkMasterToCsv()
        {
            var model = genData.USP_MoleculeProductMasterGrid().Select(x => new
            {
                PKID = x.PKID,
                x.ProductName,
                x.MoleculeName,
                IsActive = (x.IsActive == "1") ? "Active" : "In Active",
            }).OrderBy(o => o.ProductName).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("ProductName,MoleculeName,Status");
            foreach (var record in model)
            {
                sb.AppendFormat("{0},{1},{2}", record.ProductName, record.MoleculeName, record.IsActive

                 );
                sb.AppendLine();
            }
            string data = sb.ToString();
            var csvBytes = Encoding.ASCII.GetBytes(data);

            return File(csvBytes, "text/csv", "ProductMoleculeLinkMaster.txt");
        }

        #endregion

        #region PDF Class

        public class ProductMoleculeLinkMasterPDF
        {
            public string ProductName { get; set; }
            public string MoleculeName { get; set; }
            public string Status { get; set; }
        }

        #endregion

        #endregion

        #endregion


    }
}
