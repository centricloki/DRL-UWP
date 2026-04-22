// Google Maps implementation for POC
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
        { lat: 41.8781, lng: -87.6298, address: "Chicago, IL" }
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
