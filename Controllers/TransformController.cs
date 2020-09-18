using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using radio_plan_transformer.Models;

namespace radio_plan_transformer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransformController : ControllerBase
    {
        public const string XLSX_MIME_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public FileContentResult Post([FromForm] TransformRequest request)
        {
            using (var ms = new MemoryStream())
            {
                request.Template.CopyTo(ms);
                using (var doc = SpreadsheetDocument.Open(ms, true))
                {
                    FillDocument(doc, request);
                }
                return File(ms.ToArray(), XLSX_MIME_TYPE, GetFileName(request));
            }
        }

        private string GetFileName(TransformRequest transformRequest)
        {
            return $"{transformRequest.CourseNumber}_" +
                $"{transformRequest.CourseType.Replace(" ", "_").ToLowerInvariant()}.xlsx";
        }

        private SpreadsheetDocument FillDocument(SpreadsheetDocument doc, TransformRequest request)
        {
            var sharedStringPart = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
            var courseTypeCell =
                GetCell(doc, "B4");

            UpdateCell(doc, "B4", InsertSharedStringItem(request.CourseType, sharedStringPart).ToString());

            courseTypeCell.CellValue = new CellValue(
                InsertSharedStringItem(request.CourseType, sharedStringPart).ToString());

            return doc;
        }

        // https://stackoverflow.com/questions/527028/open-xml-sdk-2-0-how-to-update-a-cell-in-a-spreadsheet
        // Given text and a SharedStringTablePart, creates a SharedStringItem with the specified text 
        // and inserts it into the SharedStringTablePart. If the item already exists, returns its index.
        private static int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            // If the part does not contain a SharedStringTable, create one.
            if (shareStringPart.SharedStringTable == null)
            {
                shareStringPart.SharedStringTable = new SharedStringTable();
            }

            int i = 0;

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (var item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    return i;
                }

                i++;
            }

            // The text does not exist in the part. Create the SharedStringItem and return its index.
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            shareStringPart.SharedStringTable.Save();

            return i;
        }

        /// <see cref="IExcelDocument.ReadCell" />
        private CellValue ReadCell(SpreadsheetDocument excelDoc, string cellCoordinates)
        {
            Cell cell = GetCell(excelDoc, cellCoordinates);
            return cell.CellValue;
        }

        /// <see cref="IExcelDocument.UpdateCell" />
        private void UpdateCell(SpreadsheetDocument excelDoc, string cellCoordinates, string cellValue)
        {
            WorksheetPart worksheetPart = GetFirstWorksheetPart(excelDoc);
            Cell cell = GetCell(worksheetPart, cellCoordinates);
            cell.CellValue = new CellValue(cellValue);
            cell.DataType = new DocumentFormat.OpenXml.EnumValue<CellValues>(CellValues.SharedString);
            worksheetPart.Worksheet.Save();
        }

        private WorksheetPart GetFirstWorksheetPart(SpreadsheetDocument excelDoc)
        {
            Sheet sheet = excelDoc.WorkbookPart.Workbook.Descendants<Sheet>().First();
            return (WorksheetPart)excelDoc.WorkbookPart.GetPartById(sheet.Id);
        }

        private Cell GetCell(SpreadsheetDocument excelDoc, string cellCoordinates)
        {
            WorksheetPart worksheetPart = GetFirstWorksheetPart(excelDoc);
            return GetCell(worksheetPart, cellCoordinates);
        }

        private Cell GetCell(WorksheetPart worksheetPart, string cellCoordinates)
        {
            int rowIndex = int.Parse(cellCoordinates.Substring(1));
            Row row = GetRow(worksheetPart, rowIndex);

            Cell cell = row.Elements<Cell>().FirstOrDefault(c => cellCoordinates.Equals(c.CellReference.Value));
            if (cell == null)
            {
                throw new ArgumentException(String.Format("Cell {0} not found in spreadsheet", cellCoordinates));
            }
            return cell;
        }

        private Row GetRow(WorksheetPart worksheetPart, int rowIndex)
        {
            Row row = worksheetPart.Worksheet.GetFirstChild<SheetData>().
                                    Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                throw new ArgumentException(String.Format("No row with index {0} found in spreadsheet", rowIndex));
            }
            return row;
        }
    }
}