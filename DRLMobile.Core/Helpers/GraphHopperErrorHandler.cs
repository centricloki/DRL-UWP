using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DRLMobile.Core.Helpers
{
    // Response classes to deserialize the JSON
    public class GraphHopperErrorResponse
    {
        public string Message { get; set; }
        public List<Hint> Hints { get; set; }
        public string Status { get; set; }
    }

    public class Hint
    {
        public string Message { get; set; }
        public string Details { get; set; }
    }

    // Service to handle the response parsing
    public class GraphHopperErrorHandler
    {
        public static ParsedErrorInfo ParseErrorResponse(string jsonResponse)
        {
            var response = JsonSerializer.Deserialize<GraphHopperErrorResponse>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var connectionNotFoundServices = new List<string>();
            var connectionNotFoundMessages = new List<string>();

            if (response?.Hints != null)
            {
                foreach (var hint in response.Hints.Where(h => h.Details == "ConnectionNotFound"))
                {
                    // Extract service IDs using regex pattern matching
                    var serviceIds = ExtractServiceIdsFromMessage(hint.Message);
                    connectionNotFoundServices.AddRange(serviceIds);
                    connectionNotFoundMessages.Add(hint.Message);
                }
            }

            return new ParsedErrorInfo
            {
                ConnectionNotFoundServices = connectionNotFoundServices.Distinct().ToList(),
                ConnectionNotFoundMessages = connectionNotFoundMessages,
                TotalErrors = connectionNotFoundMessages.Count,
                OriginalMessage = response?.Message
            };
        }

        private static List<string> ExtractServiceIdsFromMessage(string message)
        {
            var serviceIds = new List<string>();

            // Regex pattern to match "service 'number'" format
            var regex = new Regex(@"service '(\d+)'");
            var matches = regex.Matches(message);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    string id = match.Groups[1].Value;
                    if (!serviceIds.Any(x => x == id))
                        serviceIds.Add(match.Groups[1].Value);
                }
            }
            return serviceIds;
        }
    }

    // Result class to hold parsed error information
    public class ParsedErrorInfo
    {
        public List<string> ConnectionNotFoundServices { get; set; } = new List<string>();
        public List<string> ConnectionNotFoundMessages { get; set; } = new List<string>();
        public int TotalErrors { get; set; }
        public string OriginalMessage { get; set; }

        public bool HasConnectionNotFoundErrors => ConnectionNotFoundServices.Any();

        public string GetCustomErrorMessage()
        {
            if (!HasConnectionNotFoundErrors)
                return "No connection errors found.";

            var serviceList = string.Join(", ", ConnectionNotFoundServices);
            return $"The following services have connection issues and cannot be included in the optimal route: {serviceList}. Please verify the addresses are accessible via road network.";
        }

        // Usage example in your UWP C# code:

    }
}