using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Common.HttpHelpers
{
    public class ProblemObjectResult : ObjectResult
    {
        public ProblemObjectResult(object value, HttpStatusCode statusCode) : base(value)
        {
            StatusCode = (int)statusCode;
        }

    }
}
