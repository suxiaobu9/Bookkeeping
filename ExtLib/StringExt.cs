using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtLib
{
    public static class  StringExt
    {
        public static bool Ext_IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

    }
}
