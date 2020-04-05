using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Common.HttpHelpers
{
    public class ProblemObjectResult : ObjectResult
    {
        public ProblemObjectResult(ProblemDetails value) : base(value)
        {
            StatusCode = (int)value.Status;
        }
    }
}
