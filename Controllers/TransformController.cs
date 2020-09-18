using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using radio_plan_transformer.Models;

namespace radio_plan_transformer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransformController : ControllerBase
    {
        public async Task<bool> Post([FromForm] TransformRequest request)
        {
            await Task.Yield();
            return true;
        }
    }
}