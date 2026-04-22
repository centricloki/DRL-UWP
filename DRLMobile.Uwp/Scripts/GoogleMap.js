// Google Maps implementation for US addresses and routing
// EdgeHTML-compatible syntax (no arrow functions, const, let, or template literals)
var map;
var markers = [];
var routeMarkers = [];
var directionsService;
var directionsRenderer;

function initMap() {
    var usaCenter = { lat: 37.0902, lng: -95.7129 };
    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 4,
        center: usaCenter,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    });

    // Directions services initialization with suppressed default markers
    directionsService = new google.maps.DirectionsService();
    directionsRenderer = new google.maps.DirectionsRenderer({
        suppressMarkers: true,
        map: map
    });
    
    // Notify UWP that map is ready
    if (typeof notify !== 'undefined') {
        notify(JSON.stringify({ type: 'mapReady' }));
    }
}

function clearMap() {
    clearMarkers();
    if (directionsRenderer) {
        directionsRenderer.setDirections({ routes: [] });
    }
}

function clearMarkers() {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
    markers = [];
    clearRouteMarkers();
}

function clearRouteMarkers() {
    for (var i = 0; i < routeMarkers.length; i++) {
        routeMarkers[i].setMap(null);
    }
    routeMarkers = [];
}

function createRoute(origin, destination) {
    if (!directionsService || !directionsRenderer) {
        if (typeof notify !== 'undefined') {
            notify(JSON.stringify({ type: 'error', message: 'Directions service not initialized' }));
        }
        return;
    }

    // Clear previous route markers
    clearRouteMarkers();

    // Show all existing markers first (resetting any hidden ones)
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }

    var request = {
        origin: origin,
        destination: destination,
        travelMode: google.maps.TravelMode.DRIVING
    };

    directionsService.route(request, function(response, status) {
        if (status === google.maps.DirectionsStatus.OK) {
            directionsRenderer.setDirections(response);
            
            // Add custom A and B markers
            var leg = response.routes[0].legs[0];
            makeRouteMarker(leg.start_location, 'A', origin);
            makeRouteMarker(leg.end_location, 'B', destination);

            // Hide the original blue markers if they match address
            for (var i = 0; i < markers.length; i++) {
                var m = markers[i];
                if (m.title === origin || m.title === destination) {
                    m.setMap(null);
                }
            }

            if (typeof notify !== 'undefined') {
                notify(JSON.stringify({ 
                    type: 'routeCreated', 
                    status: 'OK', 
                    origin: origin, 
                    destination: destination 
                }));
            }
        } else {
            if (typeof notify !== 'undefined') {
                notify(JSON.stringify({ 
                    type: 'error', 
                    message: 'Directions request failed due to ' + status 
                }));
            }
        }
    });
}

function makeRouteMarker(position, label, address) {
    // Using local Red Pin from Assets/Maps
    var iconUrl = "MapPin-Red.png";

    var marker = new google.maps.Marker({
        position: position,
        map: map,
        title: address,
        icon: {
            url: iconUrl,
            scaledSize: new google.maps.Size(24, 34),
            anchor: new google.maps.Point(12, 34)
        }
    });

    var infoWindow = new google.maps.InfoWindow({
        content: "<div style=\"font-weight:bold;padding:6px;font-size:12px;\">" + address + "</div>"
    });

    marker.addListener("click", function() {
        // Close all other info windows
        for (var i = 0; i < markers.length; i++) {
            if (markers[i].infoWindow) markers[i].infoWindow.close();
        }
        for (var i = 0; i < routeMarkers.length; i++) {
            if (routeMarkers[i].infoWindow) routeMarkers[i].infoWindow.close();
        }
        
        infoWindow.open(map, marker);
        marker.infoWindow = infoWindow;

        // Send message to UWP
        if (typeof notify !== 'undefined') {
            notify(JSON.stringify({
                type: 'markerClick',
                address: address,
                lat: position.lat(),
                lng: position.lng()
            }));
        }
    });

    routeMarkers.push(marker);
}

function addMarkers(locationsJson) {
    var locations = [];
    try {
        if (typeof locationsJson === 'string') {
            locations = JSON.parse(locationsJson);
        } else {
            locations = locationsJson;
        }
    } catch (e) {
        notify(JSON.stringify({ type: 'error', message: 'Failed to parse locations: ' + e.message }));
        return;
    }

    // Clear existing markers
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(null);
    }
    markers = [];
    clearRouteMarkers();

    for (var i = 0; i < locations.length; i++) {
        (function(loc) {
            var marker = new google.maps.Marker({
                position: { lat: loc.lat, lng: loc.lng },
                map: map,
                title: loc.address,
                icon: {
                    url: "MapPin-Blue.png",
                    scaledSize: new google.maps.Size(24, 34),
                    anchor: new google.maps.Point(12, 34)
                }
            });

            var infoWindow = new google.maps.InfoWindow({
                content: "<div style=\"font-weight:bold;padding:6px;font-size:12px;\">" + loc.address + "</div>"
            });

            marker.addListener("click", function() {
                // Close other info windows
                for (var j = 0; j < markers.length; j++) {
                    if (markers[j].infoWindow) {
                        markers[j].infoWindow.close();
                    }
                }
                for (var j = 0; j < routeMarkers.length; j++) {
                    if (routeMarkers[j].infoWindow) {
                        routeMarkers[j].infoWindow.close();
                    }
                }
                infoWindow.open(map, marker);
                marker.infoWindow = infoWindow;

                // Send message to UWP
                if (typeof notify !== 'undefined') {
                    var clickData = {
                        type: 'markerClick',
                        address: loc.address,
                        lat: loc.lat,
                        lng: loc.lng
                    };
                    notify(JSON.stringify(clickData));
                }
            });

            markers.push(marker);
        })(locations[i]);
    }
}


