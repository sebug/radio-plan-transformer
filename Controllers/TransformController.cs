using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace radio_plan_transformer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransformController : ControllerBase
    {
        public bool Post()
        {
            return true;
        }
    }
}