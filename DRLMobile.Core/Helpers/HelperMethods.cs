using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using Windows.ApplicationModel;

namespace DRLMobile.Core.Helpers
{
    public static class HelperMethods
    {
        public static string GetValueFromIdNameDictionary(Dictionary<int, string> dict, int id)
        {
            string value = string.Empty;
            dict?.TryGetValue(id, out value);
            return value;
        }
        public static int GetKeyFromIdNameDictionary(Dictionary<int, string> categoryDict, string value)
        {
            var firstEntry = categoryDict?.FirstOrDefault(x => x.Value.Equals(value));
            return firstEntry.HasValue ? firstEntry.Value.Key : 0;
        }
        public static string GetNameFromURL(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                var splittedVal = url.Split('/', '\\');
                var returnVal = splittedVal.LastOrDefault();
                return returnVal;
            }
            return string.Empty;
        }
        public static string GetAppVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
        public static string GetSalesTypeString(string value)
        {
            switch (value)
            {
                case "1":
                    return "Cash Sale";
                case "2":
                    return "Prebook";
                case "3":
                    return "Bill Through";
                case "4":
                    return "SuggestedOrder";
                case "5":
                    return "Rack POS";
                case "6":
                    return "Distributor";
                case "7":
                    return "Distributor Invoice";
                case "8":
                    return "Credit Request";
                case "9":
                    return "Cash Sales Initiative";
                case "10":
                    return "POP";
                case "11":
                    return "Sample Order";
                case "12":
                    return "Chain Distribution";
                case "13":
                    return "Credit Card Sales";
                case "14":
                    return "Car Stock Order";
                default:
                    return string.Empty;
            }
        }
        public static void RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source", "source is null.");

            if (predicate == null)
                throw new ArgumentNullException("predicate", "predicate is null.");

            source.Where(predicate).ToList().ForEach(e => source.Remove(e));
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        public static int GenerateRandomNumberForGivenRange(int minRangeNumber, int maxRangeNumber)
        {
            Random random = new Random();

            return random.Next(minRangeNumber, maxRangeNumber);
        }
        public static bool IsProdEnviornment
        {
            get
            {
                string url = ApplicationConstants.UATBaseUrl;
                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    return uri.Host.Equals(
                        "honeyapi.drl-ent.com",
                        StringComparison.OrdinalIgnoreCase
                    );
                }
                else return false;
            }
        }
    }
}