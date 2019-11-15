using System;
using System.Collections.Generic;

namespace stock_dotnet.utils.Extensions
{

    public static class StringExtension
    {
        public static string Format(this string s, object[] args) => string.Format(s, args);
        
    }

}