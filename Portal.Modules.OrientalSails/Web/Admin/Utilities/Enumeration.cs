using Portal.Modules.OrientalSails.Web.Admin.Enums;
using Portal.Modules.OrientalSails.Web.Admin.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal.Modules.OrientalSails.Web.Admin.Utilities
{
    public static class Enumeration
    {
        public static IDictionary<int, string> GetAll<TEnum>() where TEnum : struct
        {
            var enumerationType = typeof(TEnum);

            if (!enumerationType.IsEnum)
                throw new ArgumentException("Enumeration type is expected.");

            var dictionary = new Dictionary<int, string>();

            foreach (int value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                if(name.IndexOf(" ") == -1)
                {
                    Enum enumObj = (Enum.ToObject(enumerationType, value) as Enum);
                    name = enumObj.GetDisplayName();           
                }
                dictionary.Add(value, name);
            }

            return dictionary;
        }
    }
}