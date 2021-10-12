// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let map;

function initMap() {
    const myLatLng = { lat: 54.6872, lng: 25.2797 }
    map = new google.maps.Map(document.getElementById("map"), {
        center: myLatLng,
        zoom: 12,
    });

    new google.maps.Marker({
        position: myLatLng,
        map,
        title: "Restaurant",
    });
}
