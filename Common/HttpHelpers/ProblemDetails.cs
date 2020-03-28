using System;
using System.Collections.Generic;
using System.Text;

namespace Common.HttpHelpers
{
    public class ProblemDetails
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public long Status { get; set; }
        public string Detail { get; set; }
        public string Instance { get; set; }
    }
}
