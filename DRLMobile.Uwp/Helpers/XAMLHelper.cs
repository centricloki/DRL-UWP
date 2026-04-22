using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace DRLMobile.Uwp.Helpers
{
    public static class XAMLHelper
    {
        public static string TrimToLength(string val, int max) => (val != null && val.Length > max) ? val.Substring(0, max).Trim(): val;
        public static bool IsGivenLengthExceed(string args, int max = 30) => !string.IsNullOrEmpty(args) && args.Trim().Length > max;

        public static IEnumerable<T> OrderByColumnName<T>(this IEnumerable<T> source, string columnName, bool ascending = true)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Column name cannot be null or empty.", nameof(columnName));

            //string orderDirection = ascending ? "ascending" : "descending";
            //return source.AsQueryable().OrderBy($"{columnName} {orderDirection}");


            // Get the property info for the given column
            PropertyInfo propertyInfo = typeof(T).GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"No property '{columnName}' found on type '{typeof(T).Name}'.", nameof(columnName));
            }

            // Define a key selector dynamically
            Func<T, object> keySelector = x => propertyInfo.GetValue(x, null);

            // Sort based on the order parameter
            if (ascending)
            {
                return source
                .OrderBy(x => isNoValue((propertyInfo).PropertyType.Name, keySelector(x))) // Nulls appear last
                .ThenBy(x => keySelector(x));         // Remaining items in ascending order
            }
            else
            {
                return source
                .OrderBy(x => isNoValue((propertyInfo).PropertyType.Name, keySelector(x)))     // Nulls appear last
                .ThenByDescending(x => keySelector(x));  // Remaining items in descending order
            }

        }

        private static bool isNoValue(string typeName, object paramValue)
        {
            if (typeName == "String")
            {
                return string.IsNullOrWhiteSpace(Convert.ToString(paramValue));
            }
            else return paramValue == null;
        }

        public static BitmapImage GetImageBasedOnExtension(string path)
        {
            //".jpg", ".jpeg", ".png", ".bmp"
            var returnPath = "";
            if (path.ToUpper().StartsWith("HTTP"))
            {
                if (path.ToLower().Contains(".pdf"))
                {
                    returnPath = (string)Application.Current.Resources["DocumentIconImage"];
                }
                else
                {
                    returnPath = (string)Application.Current.Resources["DocumentIcon"];
                }
            }
            else
            {
                if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg") || path.ToLower().Contains(".png") || path.ToLower().Contains(".bmp"))
                {
                    returnPath = path;
                }
                else if (path.ToLower().Contains(".pdf"))
                {
                    returnPath = (string)Application.Current.Resources["DocumentIconImage"];
                }
                else
                {
                    returnPath = (string)Application.Current.Resources["DocumentIcon"];
                }
            }
            return (new BitmapImage(new Uri(returnPath)));
        }
    }
}
