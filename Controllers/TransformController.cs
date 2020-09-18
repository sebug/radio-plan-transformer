using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
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

                }
                return File(ms.ToArray(), XLSX_MIME_TYPE, GetFileName(request));
            }
        }

        private string GetFileName(TransformRequest transformRequest)
        {
            return $"{transformRequest.CourseNumber}_" +
                $"{transformRequest.CourseType.Replace(" ", "_").ToLowerInvariant()}.xlsx";
        }
    }
}