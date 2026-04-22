var map = null;
var markers = [];

function createPinElement(color) {
    var container = document.createElement('div');
    container.style.position = 'relative';
    container.style.width = '18px';
    container.style.height = '24px';

    const pinSvg = `
        <svg width="100%" height="100%" viewBox="0 0 32 42" fill="none" xmlns="http://www.w3.org/2000/svg" style="display: block;">
            <path d="M16 0C7.16344 0 0 7.16344 0 16C0 28 16 42 16 42C16 42 32 28 32 16C32 7.16344 24.8366 0 16 0Z" fill="${color}"/>
        </svg>
    `;
    container.innerHTML = pinSvg;

    return container;
}

function initMap() {
    var usaCenter = { lat: 37.0902, lng: -95.7129 };
    map = new google.maps.Map(document.getElementById("map"), {
        zoom: 4,
        center: usaCenter,
        mapTypeId: google.maps.MapTypeId.ROADMAP,
        streetViewControl: false,
        fullscreenControl: false,
        mapId: "DEMO_MAP_ID",
		mapTypeId: google.maps.MapTypeId.ROADMAP,
		mapTypeControl: false,
        streetViewControl: false,
		fullscreenControl: false,
        draggableCursor: 'default',
        draggingCursor: 'grabbing'
    });

    if (typeof notify !== 'undefined') {
        notify(JSON.stringify({ type: 'mapReady' }));
    }
}

function clearMap() {
    markers.forEach(function (marker) {
        marker.map = null;
    });
    markers = [];
}

function addMarkers(markersJson) {
    clearMap();
    var data = [];
    try {
        data = typeof markersJson === 'string' ? JSON.parse(markersJson) : markersJson;
    } catch (e) {
        console.error("Failed to parse markers JSON", e);
        return;
    }

    if (data.length === 0) {
        return;
    }

    var bounds = new google.maps.LatLngBounds();

    data.forEach(function (item) {
        // Create SVG pin element
        var pinElement = createPinElement(item.color);

        // Create AdvancedMarkerElement
        var marker = new google.maps.marker.AdvancedMarkerElement({
            position: { lat: item.lat, lng: item.lng },
            map: map,
            title: item.title,
            content: pinElement
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

        // Extend bounds
        bounds.extend({ lat: item.lat, lng: item.lng });
    });

    // Adjust map to fit all markers
    map.fitBounds(bounds);
}

function setMapView(boundsJson) {
    var boundsData = [];
    try {
        boundsData = typeof boundsJson === 'string' ? JSON.parse(boundsJson) : boundsJson;
    } catch (e) {
        console.error("Failed to parse bounds JSON", e);
        return;
    }

    if (boundsData.length === 0) return;

    if (boundsData.length === 1) {
        map.setCenter({ lat: boundsData[0].lat, lng: boundsData[0].lng });
        map.setZoom(14);
    } else {
        var bounds = new google.maps.LatLngBounds();
        boundsData.forEach(function (p) {
            bounds.extend({ lat: p.lat, lng: p.lng });
        });
        map.fitBounds(bounds);
    }
}

function notify(msg) {
    if (window.chrome && window.chrome.webview && window.chrome.webview.postMessage) {
        window.chrome.webview.postMessage(msg);
    }
}