using System;

namespace Horarium
{
    public static class Cron
    {
        public static string Secondly()
        {
            return "* * * * * *";
        }

        public static string Minutely()
        {
            return "0 * * * * *";
        }

        public static string Hourly()
        {
            return Hourly(minute: 0);
        }

        public static string Hourly(int minute)
        {
            return $"0 {minute} * * * *";
        }

        public static string Daily()
        {
            return Daily(hour: 0);
        }

        public static string Daily(int hour)
        {
            return Daily(hour, minute: 0);
        }

        public static string Daily(int hour, int minute)
        {
            return $"0 {minute} {hour} * * *";
        }

        public static string Weekly()
        {
            return Weekly(DayOfWeek.Monday);
        }

        public static string Weekly(DayOfWeek dayOfWeek)
        {
            return Weekly(dayOfWeek, hour: 0);
        }

        public static string Weekly(DayOfWeek dayOfWeek, int hour)
        {
            return Weekly(dayOfWeek, hour, minute: 0);
        }

        public static string Weekly(DayOfWeek dayOfWeek, int hour, int minute)
        {
            return $"0 {minute} {hour} * * {(int)dayOfWeek}";
        }

        public static string Monthly()
        {
            return Monthly(day: 1);
        }

        public static string Monthly(int day)
        {
            return Monthly(day, hour: 0);
        }

        public static string Monthly(int day, int hour)
        {
            return Monthly(day, hour, minute: 0);
        }

        public static string Monthly(int day, int hour, int minute)
        {
            return $"0 {minute} {hour} {day}  * *";
        }

        public static string MinuteInterval(int interval)
        {
            return $"0 */{interval} * * * *";
        }

        public static string HourInterval(int interval)
        {
            return $"0 0 */{interval} * * *";
        }

        public static string DayInterval(int interval)
        {
            return $"0 0 0 */{interval} * *";
        }

        public static string MonthInterval(int interval)
        {
            return $"0 0 0 1 */{interval} *";
        }

        public static string SecondInterval(int interval)
        {
            return $"*/{interval} * * * * *";
        }
    }
}