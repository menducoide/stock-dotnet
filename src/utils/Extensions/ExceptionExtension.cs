using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using stock_dotnet.utils.Exceptions;

namespace stock_dotnet.utils.Extensions
{
    public static class ExceptionExtension
    {
        public static ErrorDetails HandleException(this Exception ex)
        {
            var type = ex.GetType();
            switch (ex)
            {
                case BusinessException businessEx:
                    if (businessEx.Data.Count > 0)
                        return new ErrorDetails
                        {
                            StatusCode = 400,
                            Message = businessEx.ModelError
                        };
                    else return new ErrorDetails
                    {
                        StatusCode = 400,
                        Message = businessEx.Message
                    };
                case FormatException formatException:
                return new ErrorDetails
                    {
                        StatusCode = 400,
                        Message = formatException.Message
                    };
                    case AuthorizationException authorizationException:
                     return new ErrorDetails
                    {
                        StatusCode = 401,
                        Message = authorizationException.Message
                    };
                default:
                    return new ErrorDetails
                    {
                        StatusCode = 500,
                        Message = ex.Message
                    };
            }
        }


    }
}