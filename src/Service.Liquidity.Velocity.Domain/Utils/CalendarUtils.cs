using System;

namespace Service.Liquidity.Velocity.Domain.Utils
{
    public static class CalendarUtils
    {
        public static DateTime StartOfMonth(DateTime current)
        {
            DateTime first = new DateTime(current.Year, current.Month, 1);
            return first;
        }

        public static DateTime EndOfMonth(DateTime current)
        {
            DateTime last = new DateTime(current.Year, current.Month,
                                    DateTime.DaysInMonth(current.Year, current.Month));
            return last;
        }
        
        public static DateTime TwoWeeksBefore(DateTime current)
        {
            DateTime first = new DateTime(current.Year, current.Month, current.Day).AddDays(-14);
            return first;
        }
        public static DateTime OneDayBefore(DateTime current)
        {
            DateTime first = new DateTime(current.Year, current.Month, current.Day).AddDays(-1);
            return first;
        }
        
    }
}
