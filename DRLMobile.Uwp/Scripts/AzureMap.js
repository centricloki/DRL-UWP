// Azure Maps implementation for DRL Mobile Map Page (AzureMapPage)
// EdgeHTML-compatible syntax (no arrow functions, const, let, or template literals)
var map;
var datasource;
var popup;
var pins = [];

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

        var bubbleLayer = new atlas.layer.BubbleLayer(datasource, 'bubble-layer', {
            radius: 10,
            color: '#0078d7',
            strokeColor: 'white',
            strokeWidth: 2
        });
        map.layers.add(bubbleLayer);

        map.layers.add(new atlas.layer.SymbolLayer(datasource, 'symbol-layer', {
            textOptions: {
                textField: ['get', 'title'],
                offset: [0, 1.2]
            },
            iconOptions: {
                image: 'pin-round-blue',
                anchor: 'bottom',
                allowOverlap: false
            }
        }));

        if (!popup) {
            popup = new atlas.Popup({
                pixelOffset: [0, -18],
                closeButton: true
            });
        }

        map.events.add('click', function (e) {
            popup.close();
        });

        map.events.add('click', 'bubble-layer', function (e) {
            if (e.shapes && e.shapes.length > 0) {
                var shape = e.shapes[0];
                var properties = shape.getProperties();
                if (properties) {
                    var pinData = {
                        customerId: properties.customerId || 0,
                        deviceCustomerId: properties.deviceCustomerId || '',
                        title: properties.title || ''
                    };
                    notify('pinClicked:' + JSON.stringify(pinData));
                    showPinPopup(properties.lat, properties.lng, properties.title, '');
                }
            }
        });

        notify('Map initialized successfully');
    });

    map.events.add('error', function (err) {
        notify('Map error: ' + err.message);
    });
}

function clearAllPins() {
    if (datasource) {
        datasource.clear();
    }
    pins = [];
    notify('All pins cleared');
}

function addPinsFromCSharp(pinsJson) {
    try {
        if (!datasource) {
            notify('Datasource not ready');
            return;
        }

        datasource.clear();
        pins = [];

        var pinsArray = typeof pinsJson === 'string' ? JSON.parse(pinsJson) : pinsJson;

        if (!pinsArray || pinsArray.length === 0) {
            notify('No pins to display');
            return;
        }

        var bounds = new atlas.data.BoundingBox();

        for (var i = 0; i < pinsArray.length; i++) {
            var pinData = pinsArray[i];
            var lat = parseFloat(pinData.lat);
            var lng = parseFloat(pinData.lng);
            var title = pinData.title || '';
            var customerId = pinData.customerId || 0;
            var deviceCustomerId = pinData.deviceCustomerId || '';

            var point = new atlas.data.Point([lng, lat]);
            var feature = new atlas.data.Feature(point, {
                title: title,
                customerId: customerId,
                deviceCustomerId: deviceCustomerId,
                lat: lat,
                lng: lng
            });
            datasource.add(feature);
            pins.push({
                lat: lat,
                lng: lng,
                title: title,
                customerId: customerId,
                deviceCustomerId: deviceCustomerId
            });

            bounds = atlas.data.BoundingBox.merge(bounds, [lng, lat]);
        }

        map.setCamera({
            bounds: bounds,
            padding: 50
        });

        notify('Added ' + pinsArray.length + ' pins');
    } catch (err) {
        notify('Error adding pins: ' + err.message);
    }
}

function fitMapToPins(pinsJson) {
    try {
        if (!map || !datasource) {
            notify('Map not ready');
            return;
        }

        var pinsArray = typeof pinsJson === 'string' ? JSON.parse(pinsJson) : pinsJson;

        if (!pinsArray || pinsArray.length === 0) {
            notify('No pins to fit');
            return;
        }

        var bounds = new atlas.data.BoundingBox();

        for (var i = 0; i < pinsArray.length; i++) {
            var lat = parseFloat(pinsArray[i].lat);
            var lng = parseFloat(pinsArray[i].lng);
            bounds = atlas.data.BoundingBox.merge(bounds, [lng, lat]);
        }

        if (pinsArray.length === 1) {
            map.setCamera({
                center: [pinsArray[0].lng, pinsArray[0].lat],
                zoom: 14
            });
        } else {
            map.setCamera({
                bounds: bounds,
                padding: 50
            });
        }

        notify('Map fitted to ' + pinsArray.length + ' pins');
    } catch (err) {
        notify('Error fitting map: ' + err.message);
    }
}

function showPinPopup(lat, lng, title, content) {
    if (popup) {
        popup.setOptions({
            position: [lng, lat],
            content: '<div style="padding:8px;font-size:12px;max-width:200px;">' +
                '<div style="font-weight:bold;margin-bottom:4px;">' + title + '</div>' +
                '<div>' + (content || '') + '</div>' +
                '</div>'
        });
        popup.open(map);
    }
}

function centerMap(lat, lng, zoom) {
    if (map) {
        map.setCamera({
            center: [lng, lat],
            zoom: zoom || 12
        });
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
