using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace rabbitmqcore.common
{
    public static class Extension
    {
        
        public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
        {
            var model = new TModel();
            configuration.GetSection(section).Bind(model);

            return model;
        }
        
        public static string Underscore(this string value)
            => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()));


        public static T GetOptionFromJsonSettings<T>(this IConfiguration config,string jsonsection ) where T : new()
        {
            var model = new T();
           
            config.GetSection(jsonsection).Bind(model);


            return model;

        }
    }
    
}
