using System;
using System.Globalization;

using DRLMobile.ExceptionHandler;

namespace DRLMobile.Core.Helpers
{
    public static class DateTimeHelper
    {
        public const string USDateFormat = "MM/dd/yyyy";
        public const string DbFormat = "MM/dd/yyyy HH:mm:ss";
        public const string InsertDbFormat = "MM/dd/yyyy HH:mm:ss.fff";
        private const string SyncDateTimeFormat = "MMM dd, yyyy hh:mm tt";

        /// <summary>
        /// convert the string date to US date format
        /// </summary>
        /// <param name="date"></param>
        /// <returns>formated string</returns>
        public static string ConvertStringDateToMM_DD_YYYY(string date)
        {
            DateTime outDate;
            
            var isValidDate = DateTime.TryParse(s: date, new CultureInfo("en-US"), DateTimeStyles.None, out outDate);

            return isValidDate ? outDate.ToString(USDateFormat) : string.Empty;
        }

        /// <summary>
        /// convert the string date to US date format
        /// </summary>
        /// <param name="date"></param>
        /// <returns>formated string</returns>
        public static string ConvertToDBDateTimeFormat(string date)
        {
            DateTime outDate;

            var isValidDate = DateTime.TryParse(s: date, new CultureInfo("en-US"), DateTimeStyles.None, out outDate);

            return isValidDate ? outDate.ToString(DbFormat) : string.Empty;
        }
        /// <summary>
        /// convert the string date to US date format
        /// </summary>
        /// <param name="date"></param>
        /// <returns>formated string</returns>
        public static DateTime ConvertToDBDateTime(string date)
        {
            if(DateTime.TryParse(s: date, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime outDate))
            {
                return outDate;
            }
            return default;
        }

        public static string ConvertStringDateToMM_DD_YYYYForProductDistribution(DateTime date)
        {
            var tempDate = date.ToString(USDateFormat);

            return tempDate;
        }

        public static string ConvertToDbInsertDateTimeFormat(DateTime dateTime)
        {
            return dateTime.ToString(DbFormat, CultureInfo.InvariantCulture);
        }

        public static string ConvertToDbInsertDateTimeMilliSecondFormat(DateTime dateTime)
        {
            return dateTime.ToString(InsertDbFormat, CultureInfo.InvariantCulture);
        }

        public static string ConvertStringToSyncDateTimeFormat(string dateTimeString)
        {
            string syncDateTime="";

            try
            {
                //CultureInfo culture = new CultureInfo("en-US");
                DateTimeStyles styles = DateTimeStyles.None;
                DateTime dateResult;
                if(DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, styles, out dateResult))
                {
                    var temp = dateResult.ToLocalTime();
                    syncDateTime = temp.ToString(SyncDateTimeFormat);
                }               
                else
                {
                    syncDateTime = dateTimeString;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                ErrorLogger.WriteToErrorLog(nameof(DateTimeHelper), nameof(ConvertStringToSyncDateTimeFormat), ex.StackTrace);
            }

            return syncDateTime;
        }

        public static string ConvertEmptyStringDateToMM_DD_YYYY(string date)
        {
            DateTime outDate;
            if (!String.IsNullOrEmpty(date))
            {
                DateTime.TryParse(s: date, out outDate);
                return String.IsNullOrEmpty(date) ? date : outDate.ToString(USDateFormat);

            }
            return date;
        }
    }
}
