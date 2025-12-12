$(document).ready(function () {
    //===== Search Locations ====//
    $("#ln").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#ln").val();
                $.ajax({
                    type: "GET",
                    url: "/GlobalSettings/Locations/GetLocationNames?text=" + text,
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })
});

//===== Function to save New Note to the database ========//
function addLocationToLocationGroup() {
    console.log('saving note started ...')
    //== validation labels==//
    const error_div = document.getElementById("div-error");
    const location_input = document.getElementById("ln");

    let location_group_id = document.getElementById("id").value;
    let location_name = document.getElementById("ln").value;

    console.log('LocationGroupId = ' + location_group_id);
    console.log('LocationName = ' + ln);

    if (location_name === null || location_name === undefined || location_name.trim().length === 0) {
        error_div.innerHTML = "Please enter a Location!";
        location_input.focus();
        return;
    }
    error_div.innerHTML = "";

    console.log('calling api .....')
    $.ajax({
        type: 'POST',
        url: '/GlobalSettings/Locations/AddLocationToLocationGroup',
        dataType: "text",
        data: { ln: location_name, id: location_group_id },
        success: function (result) {
            if (result == "saved") {
                console.log(result);
                location.reload();
            }
            else if (result == "failed") {
                error_div.innerHTML = "Sorry, note was not saved. Please try again.";
            }
            else if (result == "parameter") {
                error_div.innerHTML = "Sorry, some values are invalid. Please try again.";
            }
            else {
                error_div.innerHTML = "Sorry, an error was encountered. Please try again.";
                console.log(result);
            }
        },
        error: function (err) {
            error_div.innerHTML = "Sorry, an error encountered. Please try again.";
            console.log(err);
        }
    })
}

//======= Script to Remove Location from a Location Group =========//
function removeLocationFromLocationGroup(location_group_member_id) {
    if (confirm('Are you sure you want to remove this Location from this Location Group?')) {
        $.ajax({
            type: 'POST',
            url: '/GlobalSettings/Locations/RemoveLocationFromLocationGroup',
            dataType: "text",
            data: { id: location_group_member_id },
            success: function (result) {
                if (result == "success") {
                    location.reload();
                }
                else {
                    console.log(result);
                }
            },
            error: function (error) {
                console.log(error);
            }
        })
    }
}