using System;

namespace Service.Liquidity.Velocity.Domain.Utils
{
    public static class CalendarUtils
    {
        public static DateTime StartOfMonth(DateTime current)
        {
            var first = new DateTime(current.Year, current.Month, 1);
            return first;
        }

        public static DateTime EndOfMonth(DateTime current)
        {
            var last = new DateTime(current.Year, current.Month,
                                    DateTime.DaysInMonth(current.Year, current.Month));
            return last;
        }
        
        public static DateTime TwoWeeksBefore(DateTime current)
        {
            var date = new DateTime(current.Year, current.Month, current.Day).AddDays(-14);
            return date;
        }
        public static DateTime OneDayBefore(DateTime current)
        {
            var date = new DateTime(current.Year, current.Month, current.Day).AddDays(-1);
            return date;
        }

        public static DateTime OneMinuteBefore(DateTime current)
        {
            var date = new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute, 0)
                .AddMinutes(-1);
            return date;
        }

        public static DateTime CountOfMinutesBefore(DateTime current, uint minutes)
        {
            var date = new DateTime(current.Year, current.Month, current.Day, current.Hour, current.Minute, 0)
                .AddMinutes(-minutes);
            return date;
        }
    }
}
