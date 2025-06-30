let map;
let marker;
let geocoder;
let dotNetHelper;

function initializeMap(dotNetObjRef) {
    dotNetHelper = dotNetObjRef;
    geocoder = new google.maps.Geocoder();
}

function loadMap(lat, lng) {
    const userLocation = { lat: lat, lng: lng };

    map = new google.maps.Map(document.getElementById("map"), {
        center: userLocation,
        zoom: 12,
        disableDefaultUI: true
    });

    marker = new google.maps.Marker({
        position: userLocation,
        map: map,
        draggable: true
    });
    updateLocation(lat, lng);

    map.addListener("click", (event) => setMarker(event.latLng));
    marker.addListener("dragend", (event) => setMarker(event.latLng));
}

function loadDefaultMap() {
    loadMap(-22.9519, -43.2105);
}

function setMarker(location) {
    marker.setPosition(location);
    updateLocation(location.lat(), location.lng());
}

function updateLocation(lat, lng) {
    geocoder.geocode({ location: { lat, lng } }, (results, status) => {
        if (status === "OK" && results && results.length > 0) {
            let chosenResult = null;

            for (let i = 0; i < results.length; i++) {
                const types = results[i].types;
                if (types.includes("point_of_interest") || types.includes("establishment") || types.includes("tourist_attraction")) {
                    chosenResult = results[i];
                    break;
                }
            }
            if (!chosenResult) {
                chosenResult = results[0];
            }
            let placeId = chosenResult.place_id ? chosenResult.place_id : "";
            const locationName = chosenResult.formatted_address;
            document.getElementById("location").value = locationName;
            if (dotNetHelper) {
                dotNetHelper.invokeMethodAsync("UpdateLocation", locationName, placeId, lat, lng);
            } else {
                console.error("DotNet object reference is not set.");
            }
        } else {
            console.error("Geocode error:", status);
        }
    });
}