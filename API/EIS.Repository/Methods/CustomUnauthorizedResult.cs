using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Repositories.Methods
{
    public class CustomUnauthorizedResult:JsonResult
    {
        public CustomUnauthorizedResult(string message) : base(new CustomError(message))
        {
            StatusCode = 401;
        }
    }
    public class CustomError
    {
        public string Error { get; }
        public CustomError(string message)
        {
            Error = message;
        }
    }
}
