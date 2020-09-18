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
        public bool Post([FromForm] TransformRequest request)
        {
            using (var ms = new MemoryStream())
            {
                request.Template.CopyTo(ms);
                using (var doc = SpreadsheetDocument.Open(ms, true))
                {

                }
            }
            return true;
        }
    }
}