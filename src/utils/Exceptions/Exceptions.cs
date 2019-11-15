

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using stock_dotnet.utils.Extensions;

namespace stock_dotnet.utils.Exceptions
{
    public class BusinessException : Exception
    {
        public Dictionary<string, string> ModelError { get; set; }
        public BusinessException(string message) : base(message) { }
        public BusinessException(List<string> message) : base(message.Join("/b")) { }

        public BusinessException(Dictionary<string, string> modelError)
        {
            this.ModelError = modelError;
        }


    }

    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message) : base(message) { }       
    }

      public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }       
    }
}