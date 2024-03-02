$(document).ready(function () {
    // Call this function on page load
    loadLocations();
});

function loadLocations() {
    showLoader();
    // Make an AJAX call to get locations from the server
    $.ajax({
        url: '/Location/GetLocations',  // Adjust the URL based on your controller and action
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            // Assuming data is an array of locations, update the dropdown
            updateLocationDropdown(data);
        },
        error: function (error) {
            console.error('Error fetching locations:', error);
        }
    });
}

function updateLocationDropdown(locations) {
    // Assuming locations is an array of objects with Name and Id properties
    var locationDropdown = $('#Location');

    // Clear existing options
    locationDropdown.empty();

    // Add a default option
    locationDropdown.append($('<option>', {
        value: '',
        text: 'Select Location'
    }));

    // Add each location to the dropdown
    $.each(locations, function (index, location) {
        locationDropdown.append($('<option>', {
            value: location.id,  // Assuming Id is a property of your location object
            text: location.name  // Assuming Name is a property of your location object
        }));
    });
    hideLoader();
}


function showLoader() {
    $('#loader').show();
    $('#loader .spinner').addClass('rotating');
}
function hideLoader() {
    $('#loader .spinner').removeClass('rotating');
    $('#loader').hide();
}