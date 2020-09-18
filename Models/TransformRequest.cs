using System;
using Microsoft.AspNetCore.Http;

namespace radio_plan_transformer.Models
{
    public class TransformRequest
    {
        public IFormFile Template { get; set; }
    }
}
