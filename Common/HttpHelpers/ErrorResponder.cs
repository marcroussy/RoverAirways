using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Common.HttpHelpers
{
    public class ErrorResponder
    {
        public static ObjectResult CreateResponse(
            HttpStatusCode statusCode,
            string type,
            string title = "",
            string detail = "",
            string instance = "")
        {
            var problem = new ProblemDetails()
            {
                Status = (int)statusCode,
                Type = type,
                Title = title,
                Instance = instance,
                Detail = detail
            };

            return new ProblemObjectResult(problem, statusCode);
        }
    }
}
