using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public static class TimeUtils
    {
        public static string SecToTimeStr(double sec)
        {
            if(sec<0)
            {
                return "00:00:00";
            }
            TimeSpan t = TimeSpan.FromSeconds(sec);
            return t.TotalDays < 1 ? t.ToString(@"hh\:mm\:ss") : t.ToString(@"dd\:hh\:mm\:ss");
        }
    }
}
