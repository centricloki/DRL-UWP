// Azure Maps implementation for DRL Mobile Route Map Page
// EdgeHTML-compatible syntax (no arrow functions, const, let, or template literals)
var map;
var datasource;
var popup;
var pins = [];
var routeLine;

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

        routeLine = new atlas.layer.LineLayer(datasource, 'route-line', {
            strokeColor: '#0078d7',
            strokeWidth: 5,
            lineDash: []
        });
        map.layers.add(routeLine);

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
                    showPinPopup(properties.lat, properties.lng, properties.title, properties.address);
                }
            }
        });

        notify('RouteMap initialized successfully');
    });

    map.events.add('error', function (err) {
        notify('RouteMap error: ' + err.message);
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
            notify('RouteMap datasource not ready');
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

        var symbolLayer = map.layers.getLayersByID('symbol-layer');
        if (symbolLayer) {
            map.events.add('click', symbolLayer, function (e) {
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
                        showPinPopup(properties.lng, properties.lat, properties.title, '');
                    }
                }
            });
        }

        map.setCamera({
            bounds: bounds,
            padding: 50
        });

        notify('Added ' + pinsArray.length + ' route pins');
    } catch (err) {
        notify('Error adding route pins: ' + err.message);
    }
}

function addRouteFromCSharp(routeJson) {
    try {
        if (!datasource) {
            notify('RouteMap datasource not ready');
            return;
        }

        var routeData = typeof routeJson === 'string' ? JSON.parse(routeJson) : routeJson;
        
        if (!routeData || !routeData.coordinates || routeData.coordinates.length === 0) {
            notify('No route coordinates');
            return;
        }

        var lineCoordinates = [];
        for (var i = 0; i < routeData.coordinates.length; i++) {
            var coord = routeData.coordinates[i];
            lineCoordinates.push([coord.lng, coord.lat]);
        }

        var line = new atlas.data.LineString(lineCoordinates);
        datasource.add(line);

        var bounds = new atlas.data.BoundingBox;
        for (var j = 0; j < lineCoordinates.length; j++) {
            bounds = atlas.data.BoundingBox.merge(bounds, lineCoordinates[j]);
        }

        map.setCamera({
            bounds: bounds,
            padding: 50
        });

        notify('Route added with ' + lineCoordinates.length + ' points');
    } catch (err) {
        notify('Error adding route: ' + err.message);
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
