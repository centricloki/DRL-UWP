// Google Maps implementation for ViewGoogleRoutePage

var map;
var markers = [];
var routeMarkers = [];
var currentPolyline = null;
var initialCenter;
var initialZoom;

function initMap() {
    var usaCenter = { lat: 37.0902, lng: -95.7129 };
    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 4,
        center: usaCenter,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapId: "f56be19b899d4ae8cde1ae9f",		
		mapTypeId: google.maps.MapTypeId.ROADMAP,
		mapTypeControl: false,
        streetViewControl: false,
		fullscreenControl: false,
        draggableCursor: 'default',
        draggingCursor: 'grabbing'
    });
    initialCenter = map.getCenter();
    initialZoom = map.getZoom();

    if (typeof notify !== 'undefined') {
        notify(JSON.stringify({ type: 'mapReady' }));
    }
}
// ─── Map Functions ────────────────────────────────────────────────────────────

function clearMap() {
    clearMarkers();
    clearRouteMarkers();
    if (currentPolyline) {
        currentPolyline.setMap(null);
        currentPolyline = null;
    }
    if (initialCenter && initialZoom) {
        map.setCenter(initialCenter);
        map.setZoom(initialZoom);
    }
}

function clearMarkers() {
    for (var i = 0; i < markers.length; i++) {
        markers[i].map = null;
    }
    markers = [];
}

function clearRouteMarkers() {
    for (var i = 0; i < routeMarkers.length; i++) {
        routeMarkers[i].map = null;
    }
    routeMarkers = [];
}

// ─── Pin Element ──────────────────────────────────────────────────────────────
function createPinElement(pinText, labelText, color) {
    var container = document.createElement('div');
    container.classList.add('custom-gmp-marker');
    container.style.cssText = `
        position: relative;
        width: 18px;
        height: 24px;
        cursor: pointer;
        pointer-events: auto;
    `;
    container.title = labelText || "";

    const pinSvg = `
        <svg width="100%" height="100%" viewBox="0 0 32 42" fill="none"
             xmlns="http://www.w3.org/2000/svg"
             style="display:block; cursor:pointer; pointer-events:auto;">
            <path d="M16 0C7.16344 0 0 7.16344 0 16C0 28 16 42 16 42C16 42 32 28 32 16C32 7.16344 24.8366 0 16 0Z"
                  fill="${color}" style="cursor:pointer;"/>
        </svg>
    `;
    container.innerHTML = pinSvg;

    if (pinText) {
        var textDiv = document.createElement('div');
        textDiv.textContent = pinText;
        textDiv.style.cssText = `
            position: absolute;
            top: 9px;
            left: 50%;
            transform: translate(-50%, -50%);
            color: white;
            font-size: ${pinText.length > 2 ? '7px' : '9px'};
            font-weight: 800;
            font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            text-shadow: 0 1px 2px rgba(0,0,0,0.6);
            width: 100%;
            text-align: center;            
        `;
        container.appendChild(textDiv);
    }

    if (labelText) {
        var labelDiv = document.createElement('div');
        labelDiv.textContent = labelText;
        labelDiv.style.cssText = `
            position: absolute;
            left: 20px;
            top: 50%;
            transform: translateY(-50%);
            font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            font-size: 11px;
            font-weight: 700;
            color: #1a1a1a;
            background-color: white;
            padding: 4px 10px;
            border-radius: 6px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.3);
            white-space: nowrap;
            border: 1px solid rgba(0,0,0,0.1);
            z-index: 1000;
        `;
        container.appendChild(labelDiv);
    }

    return container;
}
// ─── addMarkers ───────────────────────────────────────────────────────────────
function addMarkers(markersJson) {
    clearMap();

    var data = [];
    try {
        data = typeof markersJson === 'string' ? JSON.parse(markersJson) : markersJson;
    } catch (e) {
        console.error("Failed to parse markers JSON", e);
        return;
    }

    data.forEach(function (item) {
        var labelText = item.title || "";
        var content = createPinElement(null, labelText, "#EE2F24");

        var marker = new google.maps.marker.AdvancedMarkerElement({
            position: { lat: parseFloat(item.lat), lng: parseFloat(item.lng) },
            map: map,
            content: content,
            title: labelText || "Marker",
            gmpClickable: true
        });

        marker.addListener('gmp-click', function () {
            if (typeof notify !== 'undefined') {
                notify(JSON.stringify({
                    type: 'markerClick',
                    customerId: item.customerId,
                    deviceCustomerId: item.deviceCustomerId
                }));
            }
        });

        markers.push(marker);
    });
}

// ─── drawRoute ────────────────────────────────────────────────────────────────

function drawRoute(routeJson) {
    clearMap();

    var routeData = [];
    try {
        routeData = typeof routeJson === 'string' ? JSON.parse(routeJson) : routeJson;
    } catch (e) {
        console.error("Failed to parse route JSON", e);
        return;
    }

    if (!routeData.origin || !routeData.destination) {
        console.error("Missing origin or destination");
        return;
    }

    var originCoords = routeData.origin.split(',');
    var destinationCoords = routeData.destination.split(',');

    var path = [
        new google.maps.LatLng(parseFloat(originCoords[0]), parseFloat(originCoords[1]))
    ];

    if (routeData.waypoints && routeData.waypoints.length > 0) {
        routeData.waypoints.forEach(function (wp) {
            path.push(new google.maps.LatLng(wp.lat, wp.lng));
        });
    }

    path.push(new google.maps.LatLng(parseFloat(destinationCoords[0]), parseFloat(destinationCoords[1])));

    currentPolyline = new google.maps.Polyline({
        path: path,
        geodesic: true,
        strokeColor: "#0000FF",
        strokeOpacity: 0.7,
        strokeWeight: 6
    });
    currentPolyline.setMap(map);

    // START Marker
    var startData = routeData.originData || {
        lat: parseFloat(originCoords[0]),
        lng: parseFloat(originCoords[1])
    };
    var startContent = createPinElement("START", '', "#078407");
    var startMarker = new google.maps.marker.AdvancedMarkerElement({
        position: { lat: startData.lat, lng: startData.lng },
        map: map,
        content: startContent,
        title: "START",
        gmpClickable: true
    });
    routeMarkers.push(startMarker);

    // STOP Markers
    if (routeData.waypoints && routeData.waypoints.length > 0) {
        routeData.waypoints.forEach(function (wp, index) {
            var stopNumber = (index + 1).toString();
            var stopContent = createPinElement(stopNumber, wp.customerNumber || "", "#EE2F24");
            var stopMarker = new google.maps.marker.AdvancedMarkerElement({
                position: { lat: wp.lat, lng: wp.lng },
                map: map,
                content: stopContent,
                title: wp.customerNumber || "Stop " + stopNumber
            });
            stopMarker.addListener('gmp-click', function () {
                if (typeof notify !== 'undefined') {
                    notify(JSON.stringify({
                        type: 'markerClick',
                        customerId: wp.customerId,
                        deviceCustomerId: wp.deviceCustomerId
                    }));
                }
            });

            routeMarkers.push(stopMarker);
        });
    }

    // END Marker
    var endData = routeData.destinationData || {
        lat: parseFloat(destinationCoords[0]),
        lng: parseFloat(destinationCoords[1])
    };
    var endContent = createPinElement("END", '', "#EBA40C");
    var endMarker = new google.maps.marker.AdvancedMarkerElement({
        position: { lat: endData.lat, lng: endData.lng },
        map: map,
        content: endContent,
        title: "END"
    });
    // wireMarkerCursor(endContent, endMarker);
    routeMarkers.push(endMarker);

    // Fit map
    var bounds = new google.maps.LatLngBounds();
    path.forEach(function (point) { bounds.extend(point); });
    map.fitBounds(bounds);

    if (typeof notify !== 'undefined') {
        notify(JSON.stringify({ type: 'routeCreated', status: 'OK' }));
    }
}

// ─── Geocode ──────────────────────────────────────────────────────────────────

function geocodeAddress(address, requestId) {
    if (!address) return;
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === 'OK') {
            var lat = results[0].geometry.location.lat();
            var lng = results[0].geometry.location.lng();
            var city = "", state = "", country = "";
            var components = results[0].address_components;
            for (var i = 0; i < components.length; i++) {
                if (components[i].types.includes("locality")) city = components[i].long_name;
                if (components[i].types.includes("administrative_area_level_1")) state = components[i].long_name;
                if (components[i].types.includes("country")) country = components[i].long_name;
            }
            notify(JSON.stringify({
                type: 'geocoded', requestId: requestId, status: 'OK',
                lat: lat, lng: lng, city: city, state: state, country: country
            }));
        } else {
            notify(JSON.stringify({ type: 'geocoded', requestId: requestId, status: status }));
        }
    });
}

function notify(msg) {
    if (window.chrome && window.chrome.webview && window.chrome.webview.postMessage) {
        window.chrome.webview.postMessage(msg);
    }
}