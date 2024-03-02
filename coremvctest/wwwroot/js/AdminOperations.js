
function openSelectedTab(tabName) {
    if (tabName == "searchFood") {
        $.ajax({
            url: '/Admin/SearchedFoods', // Replace with the actual URL of your action
            method: 'GET',
            success: function (data) {
                // Render the returned view in the specified div
                $('#searchedFoodView').html('');
                $('#searchedShortLivedFoodView').html('');
                $('#searchedFoodView').html(data);
                // Populate the DataTable with search results
                /* dataTable.rows.add(data).draw();*/
            },
            error: function (error) {
                // Handle any errors here
                console.error(error);
            }
        });
    }
    else if (tabName == "searchShortFood") {
        $.ajax({
            url: '/Admin/ShortLivedFoods', // Replace with the actual URL of your action
            method: 'GET',
            success: function (data) {
                // Render the returned view in the specified div
                $('#searchedShortLivedFoodView').html('');
                $('#searchedFoodView').html('');
                var response = $('#searchedShortLivedFoodView').html(data);
                if (response) { 
                    fillShortlyExpiredFoodData();
                }
            },
            error: function (error) {
                // Handle any errors here
                console.error(error);
            }
        });
    }
}
function searchFood(searchedMedicine) {
    if (searchedMedicine) {
        var recordsTable = document.getElementById("recordsTable");

        // Remove the hidden attribute
        recordsTable.removeAttribute("hidden");
        var dataTable = $('#recordsTable').DataTable();
        var searchValue = searchedMedicine;
        $.ajax({
            url: '/Admin/SearchFood', // Replace with the actual route
            method: 'GET',
            data: { search: searchValue },
            success: function (data) {
                // Clear the DataTable
                dataTable.clear().draw();

                data.forEach(function (item) {
                    var imageUrl = '/FoodInventory/GetImage?fileName=' + item.imageFileName;
                    var imageLink = '<a href="' + imageUrl + '" download="' + item.imageFileName + '">' + item.imageFileName + '</a>';

                    var row = [
                        item.FoodName,
                        item.quantity,
                        item.lotNumber,
                        item.storageConditions,
                        item.packaging,
                        item.manufacturerInformation,
                        item.additionalNotes,
                        item.category,
                        imageLink
                    ];

                    dataTable.row.add(row).draw();
                });

            }
        });
    }

}
function fillShortlyExpiredFoodData() {
    var shortLivedFoodRecordsTable = document.getElementById("shortLivedFoodRecordsTable");

    // Remove the hidden attribute
    shortLivedFoodRecordsTable.removeAttribute("hidden");
    var dataTable = $('#shortLivedFoodRecordsTable').DataTable();
    $.ajax({
        url: '/Admin/SearchShortDurationFoods', // Replace with the actual URL of your action
        method: 'GET',
        success: function (data) {
            dataTable.clear().draw();

            data.forEach(function (item) {
                var imageUrl = '/FoodInventory/GetImage?fileName=' + item.imageFileName;
                var imageLink = '<a href="' + imageUrl + '" download="' + item.imageFileName + '">' + item.imageFileName + '</a>';

                var remainingDays = item.remainingDays !== null && item.remainingDays!='undefined' ? '<span style="color: red;">' + item.remainingDays + '</span>' : '';

                var row = [
                    remainingDays,
                    item.FoodName,
                    item.quantity,
                    item.lotNumber,
                    item.storageConditions,
                    item.packaging,
                    item.manufacturerInformation,
                    item.additionalNotes,
                    item.category,
                    imageLink
                ];

                dataTable.row.add(row).draw();
            });
        },
        error: function (error) {
            // Handle any errors here
            console.error(error);
        }
    });
}

function inquiryViewFunction() {
    var inquiryMedTable = document.getElementById("inquiryMedTable");

    // Remove the hidden attribute
    inquiryMedTable.removeAttribute("hidden");
    var dataTable = $('#inquiryMedTable').DataTable();
    $.ajax({
        url: '/Admin/InquiredFoodsData', // Replace with the actual URL of your action
        method: 'GET',
        success: function (data) {
            dataTable.clear().draw();

            data.forEach(function (item) {
                var reportURL = '/Admin/GetRequestedFoodReports?ngoId=' + item.ngoId + '&deliveryTime=' + encodeURIComponent(item.deliveryTime);

                var deliveryDate = new Date(item.deliveryTime).toLocaleDateString();
                var downloadLink = '<a href="' + reportURL + '" download>Download Report</a>';
                var deliveryDate = new Date(item.deliveryTime).toLocaleDateString();
                var row = [
                    item.ngoId,
                    deliveryDate,
                    downloadLink
                ];

                dataTable.row.add(row).draw();
            });
        },
        error: function (error) {
            // Handle any errors here
            console.error(error);
        }
    });
}