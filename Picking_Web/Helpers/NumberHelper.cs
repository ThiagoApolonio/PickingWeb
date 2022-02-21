using System;

namespace Picking_Web.Helpers
{
    public static class NumberHelper
    {
        public static Int32 GetFromDBToInt(object number)
        {
            return Convert.ToInt32(number);
        }

        public static Int32 StringToInt(string number)
        {
            return Convert.ToInt32(number);
        }

        public static Double StringToDouble(string number)
        {
            return Convert.ToDouble(number.Replace(",", "").Replace(".", ","));
        }

        public static Double GetFromDBToDouble(object number)
        {
            return Convert.ToDouble(number);
        }

        public static Double GetFromStringToDouble(object number)
        {
            return Convert.ToDouble(number);
        }
    }
}