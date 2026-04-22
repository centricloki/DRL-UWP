// Azure Maps implementation for POC Azure Map Page
// EdgeHTML-compatible syntax (no arrow functions, const, let, or template literals)
var map;
var datasource;
var popup;

function initMap() {
    var usaCenter = [-95.7129, 37.0902];
    
    map = new atlas.Map('map', {
        center: usaCenter,
        zoom: 4,
        view: 'Auto',
        authOptions: {
            authType: 'subscriptionKey',
            subscriptionKey: '5pITTtdBpweUbpEcst8r0IttHOycmPyyq4p87Vam5lhw3A9pwmJNJQQJ99CDACYeBjFoLCFrAAAgAZMP3R9g'
        }
    });

    map.events.add('ready', function () {
        datasource = new atlas.source.DataSource();
        map.sources.add(datasource);

        map.layers.add(new atlas.layer.BubbleLayer(datasource));

        addMarkers();
    });
}

function addMarkers() {
    var locations = [
        { lat: 34.0522, lng: -118.2437, address: "Los Angeles, CA" },
        { lat: 40.7128, lng: -74.0060, address: "New York, NY" },
        { lat: 41.8781, lng: -87.6298, address: "Chicago, IL" }
    ];

    datasource.clear();

    if (!popup) {
        popup = new atlas.Popup();
    }

    for (var i = 0; i < locations.length; i++) {
        (function(loc) {
            var point = new atlas.data.Point([loc.lng, loc.lat]);
            var feature = new atlas.data.Feature(point, {
                title: loc.address,
                address: loc.address
            });
            datasource.add(feature);

            var marker = new atlas.HtmlMarker({
                position: [loc.lng, loc.lat],
                text: '',
                color: 'White',
                markerType: 'pin-round'
            });

            map.events.add('click', marker, function () {
                popup.setOptions({
                    content: '<div style="font-weight:bold;padding:6px;font-size:12px;">' + loc.address + '</div>',
                    position: [loc.lng, loc.lat]
                });
                popup.open(map);
            });

            map.markers.add(marker);
        })(locations[i]);
    }
}

function notify(msg) {
    if (window.chrome && window.chrome.webview && window.chrome.webview.postMessage) {
        window.chrome.webview.postMessage(msg);
    } else if (window.external && window.external.notify) {
        window.external.notify(msg);
    }
    var statusDiv = document.getElementById('status');
    if (statusDiv) {
        statusDiv.innerHTML = msg;
    }
}
