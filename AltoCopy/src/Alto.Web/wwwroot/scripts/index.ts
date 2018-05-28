var componentForm = {
    locality: 'long_name',
    administrative_area_level_1: 'short_name',
    country: 'long_name',
    postal_code: 'short_name'
};

var componentFormValues = {
    locality: null,
    administrative_area_level_1: null,
    country: null,
    postal_code: null,
    longitude: null,
    latitude: null
};
var askLocation = () => {
    var getLocation = (position: Position) => {
        console.log(position.coords);
        var geocoder = new google.maps.Geocoder;
        var location = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
        componentFormValues.longitude = position.coords.longitude;
        componentFormValues.latitude = position.coords.latitude;

        geocoder.geocode({ 'location': location }, (results, status) => {
            console.log(status);
            console.log(results);
            if (status.toString() === 'OK') {
                for (var i = 0; i < results[0].address_components.length; i++) {
                    const addressType = results[0].address_components[i].types[0];
                    if (componentForm[addressType]) {
                        const val = results[0].address_components[i][componentForm[addressType]];
                        componentFormValues[addressType] = val;
                    }
                }
            }
        });
    };

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(getLocation);
    } else {
        Materialize.toast('Geolocation is not supported', 5000);
    }
};