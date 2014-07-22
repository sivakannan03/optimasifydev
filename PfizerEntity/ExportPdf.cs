using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using PfizerEntity;
using System.Data;
using System.Text;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Configuration;

namespace Pfizer.Controllers
{
    public class ExportPdf
    {
        public void ExportPDF<TSource>(IList<TSource> customerList, string[] columns, string filePath)
        {
            Font headerFont = FontFactory.GetFont("Verdana", 10, BaseColor.WHITE);
            Font rowfont = FontFactory.GetFont("Verdana", 10, BaseColor.BLUE);
            Document document = new Document(PageSize.A4);

            // If this directory does not exist, a DirectoryNotFoundException is thrown 
            // when attempting to set the current directory.

            // PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("Sample.pdf", FileMode.OpenOrCreate));
            //var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Sample1.pdf");
            var path = ConfigurationSettings.AppSettings["PDFfilePath"].ToString();
            var fileStream = new FileStream(path, FileMode.Create);
            var pdfWriter2 = PdfWriter.GetInstance(document, fileStream);

            document.Open();
            PdfPTable table = new PdfPTable(columns.Length);
            foreach (var column in columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column, headerFont));
                cell.BackgroundColor = BaseColor.BLACK;
                table.AddCell(cell);

            }

            string value = "";
            try
            {
                foreach (var item in customerList)
                {
                    foreach (var column in columns)
                    {
                        // item.GetType().GetProperties(column).GetValue(item).ToString();
                        value = item.GetType().GetProperty(column).GetValue(item).ToString();

                        //  value = item.GetType().GetProperty().GetValue(item).ToString();
                        //  var value = item.GetType().GetField(column).GetValue(item).ToString();
                        PdfPCell cell5 = new PdfPCell(new Phrase(value, rowfont));
                        table.AddCell(cell5);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            document.Add(table);
            document.Close();
        }
       
    }
}