using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduPayBlockchainNetCore.Extensions
{
    public static class Timestamp
    {
        public static DateTime? ParseStringTimestamp(this string timestamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            double output;
            var result = double.TryParse(timestamp, out output);

            if (result != false)
            {
                return dateTime.AddSeconds(output);

            }
            return null;
        }
        public static double UnixTimestamp(this int days)
        {
            //return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            //return DateTimeOffset.Now.AddDays(days).ToUnixTimeSeconds();

            // get the current day's timestamp.
            var todaysTimestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            // get the past datetime
            var pastdatetime = DateTime.UtcNow.AddDays(days);

            // get the past timestamp
            var secondsToPast = DateTime.UtcNow.Subtract(pastdatetime).TotalSeconds;

            // subtract the seconds to past from the current timestamp seconds. 
            var pastTimestamp = todaysTimestamp - secondsToPast;

            return pastTimestamp;
        }
        public static double UnixTimestamp(this string days)
        {

            int outDays;
            var result = Int32.TryParse(days, out outDays);

            if (!result)
                return 0;

            // get the current day's timestamp.
            var todaysTimestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            // get the past datetime
            var pastdatetime = DateTime.UtcNow.AddDays(outDays);

            // get the past timestamp
            var secondsToPast = DateTime.UtcNow.Subtract(pastdatetime).TotalSeconds;

            // subtract the seconds to past from the current timestamp seconds. 
            var pastTimestamp = todaysTimestamp - secondsToPast;

            return pastTimestamp;
        }
        public static double UnixTimestampToday()
        {

            // get the current day's timestamp.
            var todaysTimestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;


            return todaysTimestamp;
        }
        public static double UnixTimestamp(this double days)
        {
            //return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            //return DateTimeOffset.Now.AddDays(days).ToUnixTimeSeconds();

            // get the current day's timestamp.
            var todaysTimestamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            // get the past datetime
            var pastdatetime = DateTime.UtcNow.AddDays(days);

            // get the past timestamp
            var secondsToPast = DateTime.UtcNow.Subtract(pastdatetime).TotalSeconds;

            // subtract the seconds to past from the current timestamp seconds. 
            var pastTimestamp = todaysTimestamp - secondsToPast;

            return pastTimestamp;
        }
    }
}
