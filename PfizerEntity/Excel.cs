using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Linq;
using PfizerEntity;
using System.Data;
using System.Text;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
namespace Pfizer.Controllers
{
    public class ExcelResult : ActionResult
    {
        private readonly DataForExcel _data;
        private readonly string _fileName;

        public ExcelResult(string[] headers, List<string[]> data, string fileName, string sheetName)
        {
            _data = new DataForExcel(headers, data, sheetName);
            _fileName = fileName;
        }

        public ExcelResult(string[] headers, DataForExcel.DataType[] colunmTypes, List<string[]> data, string fileName, string sheetName)
        {
            _data = new DataForExcel(headers, colunmTypes, data, sheetName);
            _fileName = fileName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.ClearContent();
            response.ClearHeaders();
            response.Cache.SetMaxAge(new TimeSpan(0));

            using (var stream = new MemoryStream())
            {
                _data.CreateXlsxAndFillData(stream);

                //Return it to the client - strFile has been updated, so return it. 
                response.AddHeader("content-disposition", "attachment; filename=" + _fileName);

                // see http://filext.com/faq/office_mime_types.php
                if (_fileName == "xls" || _fileName == "xlsx")
                    response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                else
                    response.ContentType = "application/excel";
                response.ContentEncoding = Encoding.UTF8;
                stream.WriteTo(response.OutputStream);
            }
            response.Flush();
            response.Close();
        }

    }
}