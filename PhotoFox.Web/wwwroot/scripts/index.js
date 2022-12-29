function initialize(locations) {

    var clat = 50;
    var clng = -0.5;

    if (locations.length > 0) {
        clat = locations[0].lat;
        clng = locations[0].lng;
    }

    var latlng = new google.maps.LatLng(clat, clng);
    var options = {
        zoom: 14,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var map = new google.maps.Map(document.getElementById("map"), options);

    var infowindow = new google.maps.InfoWindow({});
    var marker, count;
    for (count = 0; count < locations.length; count++) {
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(locations[count].lat, locations[count].lng),
            map: map,
            title: locations[count].name
        });
        google.maps.event.addListener(marker, 'click', (function (marker, count) {
            return function () {
                var content = '<a href="' + locations[count].link + '" target="_blank"><img src="' + locations[count].thumb + '" width="250" /></a>'
                infowindow.setContent(content);
                infowindow.open(map, marker);
            }
        })(marker, count));
    }
}