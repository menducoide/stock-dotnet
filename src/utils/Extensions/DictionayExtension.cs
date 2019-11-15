using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace stock_dotnet.utils.Extensions
{

    public static class DictionaryExtension
    {
        public static Dictionary<string,string> ToDictionary(this ModelStateDictionary model ){
            return model.Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).Join("/b")
                );
        }
    }

}