// Google Maps implementation for 100 US addresses
// EdgeHTML-compatible syntax (no arrow functions, const, let, or template literals)
var map;
var markers = [];

function initMap() {
    var usaCenter = { lat: 37.0902, lng: -95.7129 };
    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 4,
        center: usaCenter,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    });

    addMarkers();
}

function addMarkers() {
    var locations = [
        { lat: 34.0522, lng: -118.2437, address: "Los Angeles, CA" },
        { lat: 40.7128, lng: -74.0060, address: "New York, NY" },
        { lat: 41.8781, lng: -87.6298, address: "Chicago, IL" },
        { lat: 29.7604, lng: -95.3698, address: "Houston, TX" },
        { lat: 33.4484, lng: -112.0740, address: "Phoenix, AZ" },
        { lat: 39.9526, lng: -75.1652, address: "Philadelphia, PA" },
        { lat: 29.4241, lng: -98.4936, address: "San Antonio, TX" },
        { lat: 37.7749, lng: -122.4194, address: "San Francisco, CA" },
        { lat: 32.7157, lng: -117.1611, address: "San Diego, CA" },
        { lat: 30.2672, lng: -97.7431, address: "Austin, TX" },
        { lat: 39.7392, lng: -104.9903, address: "Denver, CO" },
        { lat: 47.6062, lng: -122.3321, address: "Seattle, WA" },
        { lat: 35.2271, lng: -80.8431, address: "Charlotte, NC" },
        { lat: 40.4406, lng: -79.9959, address: "Pittsburgh, PA" },
        { lat: 33.7490, lng: -84.3880, address: "Atlanta, GA" },
        { lat: 36.1627, lng: -86.7816, address: "Nashville, TN" },
        { lat: 42.3601, lng: -71.0589, address: "Boston, MA" },
        { lat: 32.3616, lng: -86.2799, address: "Montgomery, AL" },
        { lat: 58.3019, lng: -134.4197, address: "Juneau, AK" },
        { lat: 35.1983, lng: -111.6513, address: "Flagstaff, AZ" },
        { lat: 34.4208, lng: -119.6982, address: "Santa Barbara, CA" },
        { lat: 38.5816, lng: -121.4944, address: "Sacramento, CA" },
        { lat: 39.5296, lng: -119.8138, address: "Reno, NV" },
        { lat: 38.8339, lng: -104.8214, address: "Colorado Springs, CO" },
        { lat: 41.3167, lng: -72.9167, address: "New Haven, CT" },
        { lat: 39.1582, lng: -75.5244, address: "Wilmington, DE" },
        { lat: 25.7617, lng: -80.1918, address: "Miami, FL" },
        { lat: 28.5383, lng: -81.3792, address: "Orlando, FL" },
        { lat: 30.3322, lng: -81.6557, address: "Jacksonville, FL" },
        { lat: 32.0809, lng: -81.0912, address: "Savannah, GA" },
        { lat: 21.3069, lng: -157.8583, address: "Honolulu, HI" },
        { lat: 43.6150, lng: -116.2023, address: "Boise, ID" },
        { lat: 39.7817, lng: -89.6501, address: "Springfield, IL" },
        { lat: 39.7684, lng: -86.1581, address: "Indianapolis, IN" },
        { lat: 41.5868, lng: -93.6250, address: "Des Moines, IA" },
        { lat: 37.6979, lng: -97.3148, address: "Wichita, KS" },
        { lat: 39.0997, lng: -94.5786, address: "Kansas City, MO" },
        { lat: 38.2527, lng: -85.7585, address: "Louisville, KY" },
        { lat: 29.9511, lng: -90.0715, address: "New Orleans, LA" },
        { lat: 43.6615, lng: -70.2553, address: "Portland, ME" },
        { lat: 38.9717, lng: -76.4923, address: "Annapolis, MD" },
        { lat: 42.1014, lng: -71.2798, address: "Worcester, MA" },
        { lat: 42.3314, lng: -83.0458, address: "Detroit, MI" },
        { lat: 44.9778, lng: -93.2650, address: "Minneapolis, MN" },
        { lat: 32.2988, lng: -90.1848, address: "Jackson, MS" },
        { lat: 38.6270, lng: -90.1994, address: "St. Louis, MO" },
        { lat: 45.7833, lng: -111.4167, address: "Helena, MT" },
        { lat: 40.8136, lng: -96.7026, address: "Lincoln, NE" },
        { lat: 36.1699, lng: -115.1398, address: "Las Vegas, NV" },
        { lat: 43.0731, lng: -89.4012, address: "Madison, WI" },
        { lat: 39.9612, lng: -82.9988, address: "Columbus, OH" },
        { lat: 35.4676, lng: -97.5164, address: "Oklahoma City, OK" },
        { lat: 44.0521, lng: -123.0868, address: "Eugene, OR" },
        { lat: 45.5051, lng: -122.6750, address: "Portland, OR" },
        { lat: 40.2732, lng: -76.8867, address: "Harrisburg, PA" },
        { lat: 41.8240, lng: -71.4128, address: "Providence, RI" },
        { lat: 34.0007, lng: -81.0348, address: "Columbia, SC" },
        { lat: 43.5446, lng: -96.7344, address: "Sioux Falls, SD" },
        { lat: 36.1622, lng: -86.7742, address: "Memphis, TN" },
        { lat: 30.2672, lng: -97.7431, address: "Austin, TX" },
        { lat: 40.7608, lng: -111.8910, address: "Salt Lake City, UT" },
        { lat: 44.4759, lng: -73.2121, address: "Burlington, VT" },
        { lat: 37.5407, lng: -77.4360, address: "Richmond, VA" },
        { lat: 47.2529, lng: -122.4443, address: "Tacoma, WA" },
        { lat: 38.3498, lng: -81.6326, address: "Charleston, WV" },
        { lat: 43.0389, lng: -87.9065, address: "Milwaukee, WI" },
        { lat: 41.1611, lng: -104.8053, address: "Cheyenne, WY" },
        { lat: 35.0844, lng: -106.6504, address: "Albuquerque, NM" },
        { lat: 39.4837, lng: -119.7583, address: "Carson City, NV" },
        { lat: 42.9371, lng: -71.4526, address: "Manchester, NH" },
        { lat: 40.2237, lng: -74.7467, address: "Trenton, NJ" },
        { lat: 35.6870, lng: -105.9378, address: "Santa Fe, NM" },
        { lat: 42.6526, lng: -73.7562, address: "Albany, NY" },
        { lat: 41.5205, lng: -74.1245, address: "Middletown, NY" },
        { lat: 40.8018, lng: -74.1578, address: "Paterson, NJ" },
        { lat: 40.7357, lng: -74.1724, address: "Newark, NJ" },
        { lat: 40.6008, lng: -74.0834, address: "Jersey City, NJ" },
        { lat: 40.8859, lng: -74.0073, address: "Hackensack, NJ" },
        { lat: 40.9168, lng: -74.0737, address: "Paramus, NJ" },
        { lat: 40.8357, lng: -74.1425, address: "Passaic, NJ" },
        { lat: 40.7831, lng: -73.9712, address: "Manhattan, NY" },
        { lat: 40.6782, lng: -73.9442, address: "Brooklyn, NY" },
        { lat: 40.7282, lng: -73.7949, address: "Queens, NY" },
        { lat: 40.8448, lng: -73.8648, address: "The Bronx, NY" },
        { lat: 40.5795, lng: -74.1502, address: "Staten Island, NY" },
        { lat: 41.3524, lng: -72.0995, address: "New London, CT" },
        { lat: 41.7658, lng: -72.6734, address: "Hartford, CT" },
        { lat: 41.8029, lng: -72.7498, address: "Waterbury, CT" },
        { lat: 41.1865, lng: -73.1943, address: "Danbury, CT" },
        { lat: 41.3557, lng: -73.0877, address: "Bridgeport, CT" },
        { lat: 41.1029, lng: -73.4003, address: "Norwalk, CT" },
        { lat: 41.0265, lng: -73.6282, address: "Stamford, CT" },
        { lat: 42.1031, lng: -72.5898, address: "Springfield, MA" },
        { lat: 42.3736, lng: -72.5197, address: "Northampton, MA" },
        { lat: 42.5238, lng: -70.8957, address: "Lynn, MA" },
        { lat: 42.3925, lng: -71.0995, address: "Cambridge, MA" },
        { lat: 42.0333, lng: -71.5333, address: "Framingham, MA" },
        { lat: 42.2352, lng: -71.0275, address: "Quincy, MA" },
        { lat: 42.1620, lng: -71.2019, address: "Newton, MA" },
        { lat: 42.2776, lng: -71.8023, address: "Worcester, MA" },
        { lat: 42.0752, lng: -72.6287, address: "Holyoke, MA" },
        { lat: 42.5884, lng: -70.8594, address: "Salem, MA" },
        { lat: 42.3751, lng: -71.1056, address: "Somerville, MA" }
    ];

    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
    markers = [];

    for (var i = 0; i < locations.length; i++) {
        (function(loc) {
            var marker = new google.maps.Marker({
                position: { lat: loc.lat, lng: loc.lng },
                map: map,
                title: loc.address,
                icon: {
                    url: "https://maps.gstatic.com/mapfiles/ms2/micons/blue-dot.png",
                    scaledSize: new google.maps.Size(24, 24)
                }
            });

            var infoWindow = new google.maps.InfoWindow({
                content: "<div style=\"font-weight:bold;padding:6px;font-size:12px;\">" + loc.address + "</div>"
            });

            marker.addListener("click", function() {
                for (var j = 0; j < markers.length; j++) {
                    if (markers[j].infoWindow) {
                        markers[j].infoWindow.close();
                    }
                }
                infoWindow.open(map, marker);
                marker.infoWindow = infoWindow;
            });

            markers.push(marker);
        })(locations[i]);
    }
}