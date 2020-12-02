using System;

namespace TinkoffApi.Data
{
    public class ServerTime
    {
        public static DateTime GetDate()
        {
            TimeZoneInfo moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            DateTime utcTime = DateTime.Now.ToUniversalTime();
            DateTime moscowTime = TimeZoneInfo.ConvertTime(utcTime, moscowZone);
            return moscowTime;
        }
    }
}
