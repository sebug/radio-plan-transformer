using System;
using Microsoft.AspNetCore.Http;

namespace radio_plan_transformer.Models
{
    public class TransformRequest
    {
        public string CourseType { get; set; }
        public string CourseNumber { get; set; }
        public string CourseDates { get; set; }
        public string CreatedBy { get; set; }
        public string Phone { get; set; }
        public IFormFile Template { get; set; }
    }
}
