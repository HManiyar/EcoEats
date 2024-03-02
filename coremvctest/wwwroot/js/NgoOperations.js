$(document).ready(function () {
    var initialFoodSelection = document.getElementById('foodSelection');
    getFoodOptions(initialFoodSelection);
});
function getFoodOptions(foodSelection) {
    showLoader();
    // Make an AJAX call to the controller action to get food options
    fetch('/NGO/GetFoodOptions')
        .then(response => response.json())
        .then(data => {
            // Populate the food selection dropdown with the retrieved options
            populateFoodOptions(foodSelection, data);
        })
        .catch(error => console.error('Error:', error));
}

function populateFoodOptions(selectElement, options) {
    // Clear existing options
    selectElement.innerHTML = '';

    // Populate with new options
    options.forEach(option => {
        var optionElement = document.createElement('option');
        optionElement.value = option.foodEntity.foodId;
        var formattedDistance = option.distance.toFixed(1);
        optionElement.text = `${option.foodEntity.foodName} - (${option.locationName})`;

        selectElement.appendChild(optionElement);
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