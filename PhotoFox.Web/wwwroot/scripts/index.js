let map;
let markers = [];


function initialize() {
    var clat = 50;
    var clng = -0.5;

    var latlng = new google.maps.LatLng(clat, clng);
    var options = {
        zoom: 14,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.TERRAIN
    };

    map = new google.maps.Map(document.getElementById("map"), options);
}

// Adds a marker to the map and push to the array.
function addMarker(position, title, link, thumb) {
    const marker = new google.maps.Marker({
        position: position,
        map: map,
        title: title
    });

    google.maps.event.addListener(marker, 'click', (function (marker) {
        return function () {
            let infowindow = new google.maps.InfoWindow({});
            var content = '<a href="' + link + '" target="_blank"><img src="' + thumb + '" width="250" /></a>'
            infowindow.setContent(content);
            infowindow.open(map, marker);
        }
    })(marker));

    markers.push(marker);
}

// Sets the map on all markers in the array.
function setMapOnAll(map) {
    for (let i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

// Removes the markers from the map, but keeps them in the array.
function hideMarkers() {
    setMapOnAll(null);
}

// Shows any markers currently in the array.
function showMarkers() {
    setMapOnAll(map);
}

// Deletes all markers in the array by removing references to them.
function deleteMarkers() {
    hideMarkers();
    markers = [];
}

function setMarkers(locations) {
    deleteMarkers();

    for (i = 0; i < locations.length; i++) {
        addMarker(
            new google.maps.LatLng(locations[i].lat, locations[i].lng),
            locations[i].name,
            locations[i].link,
            locations[i].thumb
        );
    }

    var bounds = new google.maps.LatLngBounds();
    for (var i = 0; i < markers.length; i++) {
        bounds.extend(markers[i].position);
    }

    map.fitBounds(bounds);
}