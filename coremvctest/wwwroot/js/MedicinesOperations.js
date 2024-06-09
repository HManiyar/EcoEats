function openTab(tabName) {
    var tabContents = document.getElementsByClassName("tab-content");
    for (var i = 0; i < tabContents.length; i++) {
        tabContents[i].style.display = "none";
    }

    var tabLinks = document.getElementsByClassName("tab");
    for (var i = 0; i < tabLinks.length; i++) {
        tabLinks[i].classList.remove("active");
    }

    document.getElementById(tabName).style.display = "block";
    document.querySelector("[onclick*='" + tabName + "']").classList.add("active");

    if (tabName == "updateunusedfoodTab") {
        window.location.href = '/FoodInventory/UpdateFood';
        return;
    }
    if (tabName == "logoutTab") {
        window.location.href = '/CommonDashboard/LogOut';
        return;
    }
}
function loaddatetimefield() { 
    var expiryWarningSelect = document.getElementById("ExpiryWarningId");
    var customDateContainer = document.getElementById("customDateContainer");
    var customExpiryDateInput = document.getElementById("CustomExpiryDate");

    // Add an event listener to the ExpiryWarning dropdown
    expiryWarningSelect.addEventListener("change", function () {
        if (expiryWarningSelect.value === "3") {
            // If "Other" is selected, enable the CustomExpiryDate field and set it as required
            customDateContainer.style.display = "block";
            customExpiryDateInput.setAttribute("required", "required");
        } else {
            // If another option is selected, hide and disable the CustomExpiryDate field
            customDateContainer.style.display = "none";
            customExpiryDateInput.removeAttribute("required");
        }
    });

    // Set a minimum date for the CustomExpiryDate field (e.g., today's date)
    customExpiryDateInput.min = new Date().toISOString().split("T")[0];
}
// Optional: Disable past dates in the CustomExpiryDate field


function handleImageUpload() {
    var input = document.getElementById("ImageFileName");
    var imageBase64Input = document.getElementById("ImageBase64");

    var file = input.files[0];
    var reader = new FileReader();

    reader.onload = function () {
        var base64Data = reader.result.split(',')[1];
        imageBase64Input.value = base64Data;

        // Once the base64 data is set, you can submit the form
        // You can do this here, or trigger the form submission elsewhere in your code
        // Example: document.getElementById("yourFormId").submit();
    };

    reader.readAsDataURL(file);
}

function submitFoodData() {
    var form = document.getElementById('foodRequestForm');

    // Get NGO Id (replace 'yourNgoId' with the actual way you obtain NGO Id)
   
    // Get Expected Delivery Time
    var deliveryTime = form.querySelector('#deliveryTime').value;

    // Get Medicine Requests
    var foodRequests = [];
    var medicineContainers = form.querySelectorAll('.medicine-request');
    medicineContainers.forEach(function (container, index) {
        var category = container.querySelector('#foodSelection').value;
        var quantity = container.querySelector('#quantity').value;

        // Create an object for each medicine request
        var medicineRequest = {
            category: category,
            quantity: quantity
        };

        foodRequests.push(medicineRequest);
    });

    // Prepare JSON object
    var requestData = {
        deliveryTime: deliveryTime,
        foods: foodRequests
    };

    $.ajax({
        type: 'POST',
        url: '/NGO/UpdateRequestedFood',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function (response) {
            // Handle success response from the server
            alert("Foods is send successfully");
        },
        error: function (error) {
            // Handle error response from the server
            alert("Something went wrong");
        }
    });
    // Log the JSON object (you can send it to the server or perform any other action)
}

function goBack() {
    window.history.back();
}

