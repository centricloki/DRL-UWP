// Google Maps implementation for ViewGoogleRoutePage
var map;
var markersById = {}; // for addMarkers
var routeMarkersById = {}; // for drawRoute: START, END, STOP_1...
var currentPolyline = null;
var initialCenter;
var initialZoom;
var pinTemplates = {};

function initMap() {
    var usaCenter = { lat: 37.0902, lng: -95.7129 };
    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 4,
        center: usaCenter,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        mapId: "f56be19b899d4ae8cde1ae9f",
        mapTypeControl: false,
        streetViewControl: false,
        fullscreenControl: false,
        draggableCursor: 'default',
        draggingCursor: 'grabbing'
    });
    initialCenter = map.getCenter();
    initialZoom = map.getZoom();

    // Pre-create the three pin templates once
    pinTemplates.START = buildPinTemplate('#078407');
    pinTemplates.END = buildPinTemplate('#EBA40C');
    pinTemplates.STOP = buildPinTemplate('#EE2F24');

    if (typeof notify !== 'undefined') {
        notify(JSON.stringify({ type: 'mapReady' }));
    }
}

// ─── Pin Template ──────────────────────────────────────────────────────────────
function buildPinTemplate(color) {
    var container = document.createElement('div');
    container.className = 'custom-gmp-marker';
    container.style.cssText = 'position:relative;width:18px;height:24px;cursor:pointer;pointer-events:auto;';

    var svgNS = 'http://www.w3.org/2000/svg';
    var svg = document.createElementNS(svgNS, 'svg');
    svg.setAttribute('viewBox', '0 0 32 42');
    svg.setAttribute('width', '100%');
    svg.setAttribute('height', '100%');
    svg.style.display = 'block';
    var path = document.createElementNS(svgNS, 'path');
    path.setAttribute('d', 'M16 0C7.16344 0 0 7.16344 0 16C0 28 16 42 16 42C16 42 32 28 32 16C32 7.16344 24.8366 0 16 0Z');
    path.setAttribute('fill', color);
    svg.appendChild(path);
    container.appendChild(svg);

    var textDiv = document.createElement('div');
    textDiv.className = 'pin-text';
    textDiv.style.cssText = 'position:absolute;top:9px;left:50%;transform:translate(-50%,-50%);color:white;font-size:9px;font-weight:800;font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;text-shadow:0 1px 2px rgba(0,0,0,0.6);width:100%;text-align:center;';
    container.appendChild(textDiv);

    var labelDiv = document.createElement('div');
    labelDiv.className = 'pin-label';
    labelDiv.style.cssText = 'position:absolute;left:20px;top:50%;transform:translateY(-50%);font-family:Segoe UI,Roboto,Helvetica,Arial,sans-serif;font-size:11px;font-weight:700;color:#1a1a1a;background-color:white;padding:4px 10px;border-radius:6px;box-shadow:0 2px 6px rgba(0,0,0,0.3);white-space:nowrap;border:1px solid rgba(0,0,0,0.1);z-index:1000;display:none;';
    container.appendChild(labelDiv);

    return container;
}

function getPin(type, pinText, labelText, overrideColor) {
    var base = pinTemplates[type] || pinTemplates.STOP;
    var node = base.cloneNode(true); // fast clone, no innerHTML
    if (overrideColor) {
        node.querySelector('path').setAttribute('fill', overrideColor);
    }
    var textEl = node.querySelector('.pin-text');
    if (pinText) {
        textEl.textContent = pinText;
        textEl.style.fontSize = pinText.length > 2 ? '7px' : '9px';
    } else {
        textEl.textContent = '';
    }
    var labelEl = node.querySelector('.pin-label');
    if (labelText) {
        labelEl.textContent = labelText;
        labelEl.style.display = 'block';
        node.title = labelText;
    } else {
        labelEl.style.display = 'none';
        node.title = '';
    }
    return node;
}

// ─── Map Functions ────────────────────────────────────────────────────────────
function clearMarkers() {
    for (var id in markersById) {
        if (markersById[id]) markersById[id].map = null;
    }
    markersById = {};
}

function clearRouteMarkers() {
    for (var id in routeMarkersById) {
        if (routeMarkersById[id]) routeMarkersById[id].map = null;
    }
    routeMarkersById = {};
}

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

function removeMarker(dict, id) {
    if (dict[id]) {
        dict[id].map = null;
        delete dict[id];
    }
}

// ─── addMarkers ───────────────────────────────────────────────────────────────
function addMarkers(markersJson) {
    // clear route artifacts only, keep marker instances for diffing
    clearRouteMarkers();
    if (currentPolyline) { currentPolyline.setMap(null); currentPolyline = null; }

    var data = [];
    try {
        data = typeof markersJson === 'string' ? JSON.parse(markersJson) : markersJson;
    } catch (e) {
        console.error("Failed to parse markers JSON", e);
        return;
    }

    var seen = {};
    data.forEach(function (item) {
        var id = item.customerId || item.deviceCustomerId || (item.lat + '_' + item.lng);
        seen[id] = true;
        var pos = { lat: parseFloat(item.lat), lng: parseFloat(item.lng) };
        var labelText = item.title || "";

        var m = markersById[id];
        if (m) {
            m.position = pos;
            m.title = labelText || "Marker";
        } else {
            var content = getPin('STOP', null, labelText);
            m = new google.maps.marker.AdvancedMarkerElement({
                position: pos,
                map: map,
                content: content,
                title: labelText || "Marker",
                gmpClickable: true
            });
            m.addListener('gmp-click', function () {
                if (typeof notify !== 'undefined') {
                    notify(JSON.stringify({
                        type: 'markerClick',
                        customerId: item.customerId,
                        deviceCustomerId: item.deviceCustomerId
                    }));
                }
            });
            markersById[id] = m;
        }
    });

    // remove markers not in new data
    for (var id in markersById) {
        if (!seen[id]) removeMarker(markersById, id);
    }
}

// ─── drawRoute ────────────────────────────────────────────────────────────────
function upsertRouteMarker(id, pos, pinText, labelText, color, title, data) {
    var m = routeMarkersById[id];
    var position = { lat: parseFloat(pos.lat), lng: parseFloat(pos.lng) };
    if (m) {
        m.position = position;
        if (title) m.title = title;
    } else {
        var type = id === 'START' ? 'START' : id === 'END' ? 'END' : 'STOP';
        var content = getPin(type, pinText, labelText, color);
        m = new google.maps.marker.AdvancedMarkerElement({
            position: position,
            map: map,
            content: content,
            title: title || '',
            gmpClickable: true
        });
        if (data && data.customerId) {
            m.addListener('gmp-click', function () {
                if (typeof notify !== 'undefined') {
                    notify(JSON.stringify({
                        type: 'markerClick',
                        customerId: data.customerId,
                        deviceCustomerId: data.deviceCustomerId
                    }));
                }
            });
        }
        routeMarkersById[id] = m;
    }
}

function drawRoute(routeJson) {
    // clear simple markers only, keep route markers for diffing
    clearMarkers();

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
    var path = [new google.maps.LatLng(parseFloat(originCoords[0]), parseFloat(originCoords[1]))];

    if (routeData.waypoints && routeData.waypoints.length > 0) {
        routeData.waypoints.forEach(function (wp) {
            path.push(new google.maps.LatLng(wp.lat, wp.lng));
        });
    }
    path.push(new google.maps.LatLng(parseFloat(destinationCoords[0]), parseFloat(destinationCoords[1])));

    // reuse polyline
    if (!currentPolyline) {
        currentPolyline = new google.maps.Polyline({
            path: path,
            geodesic: true,
            strokeColor: "#0000FF",
            strokeOpacity: 0.7,
            strokeWeight: 6,
            map: map
        });
    } else {
        currentPolyline.setPath(path);
        currentPolyline.setMap(map);
    }

    // START Marker
    var startData = routeData.originData || { lat: parseFloat(originCoords[0]), lng: parseFloat(originCoords[1]) };
    upsertRouteMarker('START', startData, 'START', '', '#078407', 'START');

    // STOP Markers
    var seenStops = {};
    if (routeData.waypoints && routeData.waypoints.length > 0) {
        routeData.waypoints.forEach(function (wp, index) {
            var id = 'STOP_' + (index + 1);
            seenStops[id] = true;
            var stopNumber = (index + 1).toString();
            upsertRouteMarker(id, wp, stopNumber, wp.customerNumber || '', '#EE2F24', wp.customerNumber || 'Stop ' + stopNumber, wp);
        });
    }
    // remove old stops
    for (var id in routeMarkersById) {
        if (id.indexOf('STOP_') === 0 && !seenStops[id]) {
            removeMarker(routeMarkersById, id);
        }
    }

    // END Marker
    var endData = routeData.destinationData || { lat: parseFloat(destinationCoords[0]), lng: parseFloat(destinationCoords[1]) };
    upsertRouteMarker('END', endData, 'END', '', '#EBA40C', 'END');

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
