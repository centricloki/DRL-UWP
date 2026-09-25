using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DRLMobile.Core.Helpers
{
    // ============================================
    // RESPONSE MODELS
    // ============================================

    public class GraphHopperErrorResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("hints")]
        public List<Hint> Hints { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class Hint
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("details")]
        public string Details { get; set; }
    }

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
            var problematicVehicleLocations = new List<string>(); // "start" and/or "end"

            if (response?.Hints != null)
            {
                foreach (var hint in response.Hints.Where(h => h.Details == "ConnectionNotFound"))
                {
                    connectionNotFoundMessages.Add(hint.Message);

                    // NEW: Check if this hint refers to a vehicle start/end location error.
                    // e.g. hint message: "Connection between locations not found:
                    //   [ start location (lon: ...) of vehicle 'custom_vehicle'],
                    //   [ end location (lon: ...) of vehicle 'custom_vehicle']"
                    // This runs independently — does NOT replace the service-ID check below.
                    var vehicleLocs = ExtractVehicleLocationsFromMessage(hint.Message);
                    if (vehicleLocs.Count > 0)
                        problematicVehicleLocations.AddRange(vehicleLocs);

                    // EXISTING (unchanged): Extract customer service stop IDs.
                    // e.g. hint message: "...service '12345'..."
                    // Always runs so existing RouteError handling is never blocked.
                    var serviceIds = ExtractServiceIdsFromMessage(hint.Message);
                    if (serviceIds.Count > 0)
                        connectionNotFoundServices.AddRange(serviceIds);
                }
            }

            return new ParsedErrorInfo
            {
                ConnectionNotFoundServices = connectionNotFoundServices.Distinct().ToList(),
                ConnectionNotFoundMessages = connectionNotFoundMessages,
                ProblematicVehicleLocations = problematicVehicleLocations.Distinct().ToList(),
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

        /// <summary>
        /// Extracts vehicle start/end location labels from a ConnectionNotFound hint message.
        /// Returns a list containing "start" and/or "end" when the hint refers to a vehicle
        /// location (not a customer service stop).
        /// Example message: "Connection between locations not found:
        ///   [ start location (lon: -84.09, lat: 40.76) of vehicle 'custom_vehicle'],
        ///   [ end location (lon: 77.34, lat: 28.63) of vehicle 'custom_vehicle']"
        /// </summary>
        private static List<string> ExtractVehicleLocationsFromMessage(string message)
        {
            var locations = new List<string>();

            // Match "start location ... of vehicle" or "end location ... of vehicle"
            var regex = new Regex(@"\b(start|end)\s+location\b.*?of\s+vehicle", RegexOptions.IgnoreCase);
            var matches = regex.Matches(message);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    string loc = match.Groups[1].Value.ToLower(); // "start" or "end"
                    if (!locations.Contains(loc))
                        locations.Add(loc);
                }
            }
            return locations;
        }
    }

    // Result class to hold parsed error information
    public class ParsedErrorInfo
    {
        public List<string> ConnectionNotFoundServices { get; set; } = new List<string>();
        public List<string> ConnectionNotFoundMessages { get; set; } = new List<string>();

        /// <summary>
        /// Contains "start" and/or "end" if the vehicle's start/end location
        /// cannot be connected to the road network (e.g. location is in a different country).
        /// </summary>
        public List<string> ProblematicVehicleLocations { get; set; } = new List<string>();

        public int TotalErrors { get; set; }
        public string OriginalMessage { get; set; }

        /// <summary>True when any customer service stop has a connection error.</summary>
        public bool HasConnectionNotFoundErrors => ConnectionNotFoundServices.Any();

        /// <summary>True when the vehicle's start or end location cannot be routed.</summary>
        public bool HasVehicleLocationErrors => ProblematicVehicleLocations.Any();

        /// <summary>True when any kind of ConnectionNotFound error exists (service or vehicle).</summary>
        public bool HasAnyConnectionError => HasConnectionNotFoundErrors || HasVehicleLocationErrors;

        public string GetCustomErrorMessage()
        {
            if (!HasConnectionNotFoundErrors)
                return "No connection errors found.";

            var serviceList = string.Join(", ", ConnectionNotFoundServices);
            return $"The following services have connection issues and cannot be included in the optimal route: {serviceList}. Please verify the addresses are accessible via road network.";
        }
    }
}