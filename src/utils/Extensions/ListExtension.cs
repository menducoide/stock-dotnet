using System;
using System.Collections.Generic;

namespace stock_dotnet.utils.Extensions
{

    public static class ListExtension
    {
        public static string Join(this IEnumerable<string> list, string separator)
        {
            string result = "";
            foreach (var item in list)
                result += result == "" ? item : separator + item;
            return result;
        }
        public static string Join(this IEnumerable<int> list, string separator)
        {
            string result = "";
            foreach (var item in list)
                result += result == "" ? item.ToString() : separator + item.ToString();
            return result;
        }
    }

}