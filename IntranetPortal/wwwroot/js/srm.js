$(document).ready(function () {

    //============ Search Employee Names =======//
    $("#sn").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#sn").val();
                $.ajax({
                    type: "GET",
                    url: "/ERM/Home/GetEmployeeNames?text=" + text,
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

//======= Script to delete Service Incident from database =========//
function deleteServiceIncident(id) {
    if (confirm('Are you sure you want to delete this request permanently?')) {
        $.ajax({
            type: 'POST',
            url: '/SRM/Service/DeleteServiceIncident',
            dataType: "text",
            data: { id: id },
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

//===== Function to save or update Service System to the database ========//
function addServiceSystem() {
    console.log('saving service system started ...')

    //== validation labels==//
    const error_div = document.getElementById("div-error");
    let system_id = document.getElementById("sys_id").value;
    let system_name = document.getElementById("sys_name").value;

    console.log('System ID:' + system_id);
    console.log('System Name: ' + system_name);

    if (system_name === null || system_name === undefined || system_name.trim().length === 0) {
        error_div.innerHTML = "Please enter a name!";
        document.getElementById("sys_name").focus();
        return;
    }
    error_div.innerHTML = "";

    if (system_id !== undefined && system_id != null && system_id > 0) {
        console.log('calling UpdateServiceSystem api .....')
        $.ajax({
            type: 'POST',
            url: '/SRM/Home/UpdateServiceSystem',
            dataType: "text",
            data: { id: system_id, nm: system_name},
            success: function (result) {
                if (result == "saved") {
                    console.log(result);
                    location.reload();
                }
                else if (result == "failed") {
                    error_div.innerHTML = "Sorry, note was not saved. Please try again.";
                }
                else if (result == "parameter") {
                    error_div.innerHTML = "Sorry, some values may be invalid. Please try again.";
                }
                else {
                    error_div.innerHTML = "Sorry, an error was encountered. Please try again.";
                    console.log(result);
                }
            },
            error: function (err) {
                error_div.innerHTML = err;
                console.log(err);
            }
        })
    }
    else {
        console.log('calling AddServiceSystem api .....')
        $.ajax({
            type: 'POST',
            url: '/SRM/Home/AddServiceSystem',
            dataType: "text",
            data: {nm: system_name },
            success: function (result) {
                if (result == "saved") {
                    console.log(result);
                    location.reload();
                }
                else if (result == "failed") {
                    error_div.innerHTML = "Sorry, note was not saved. Please try again.";
                }
                else if (result == "parameter") {
                    error_div.innerHTML = "Sorry, some values may be invalid. Please try again.";
                }
                else {
                    error_div.innerHTML = "Sorry, an error was encountered. Please try again.";
                    console.log(result);
                }
            },
            error: function (err) {
                error_div.innerHTML = err;
                console.log(err);
            }
        })
    }
}

//======= Script to delete Service System from database =========//
function deleteServiceSystem(id) {
    if (confirm('Are you sure you want to delete this system permanently?')) {
        $.ajax({
            type: 'POST',
            url: '/SRM/Home/DeleteServiceSystem',
            dataType: "text",
            data: { id: id },
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

//===== Function to populate the Edit Service System form ========//
function populateSystemEditForm(id, name) {
    const sys_name_div = document.getElementById("sys_name");
    document.getElementById("sys_name").value = name;
    document.getElementById("sys_id").value = id;
    sys_name_div.focus();
}

//===== Function to save changes to Service Center to the database ========//
function addServiceCenter() {
    console.log('saving service center started ...')

    //== validation labels==//
    const error_div = document.getElementById("div-error");
    let center_id = document.getElementById("cntr_id").value;
    let center_name = document.getElementById("cntr_name").value;

    console.log('Center ID:' + center_id);
    console.log('Center Name: ' + center_name);

    if (center_name === null || center_name === undefined || center_name.trim().length === 0) {
        error_div.innerHTML = "Please enter a name!";
        document.getElementById("cntr_name").focus();
        return;
    }
    error_div.innerHTML = "";

    if (center_id !== undefined && center_id != null && center_id > 0) {
        console.log('calling UpdateServiceCenter api .....')
        $.ajax({
            type: 'POST',
            url: '/SRM/Home/UpdateServiceCenter',
            dataType: "text",
            data: { id: center_id, nm: center_name },
            success: function (result) {
                if (result == "saved") {
                    console.log(result);
                    location.reload();
                }
                else if (result == "failed") {
                    error_div.innerHTML = "Sorry, note was not saved. Please try again.";
                }
                else if (result == "parameter") {
                    error_div.innerHTML = "Sorry, some values may be invalid. Please try again.";
                }
                else {
                    error_div.innerHTML = "Sorry, an error was encountered. Please try again.";
                    console.log(result);
                }
            },
            error: function (err) {
                error_div.innerHTML = err;
                console.log(err);
            }
        })
    }
    else {
        console.log('calling AddServiceCenter api .....')
        $.ajax({
            type: 'POST',
            url: '/SRM/Home/AddServiceCenter',
            dataType: "text",
            data: { nm: center_name },
            success: function (result) {
                if (result == "saved") {
                    console.log(result);
                    location.reload();
                }
                else if (result == "failed") {
                    error_div.innerHTML = "Sorry, service center was not saved. Please try again.";
                }
                else if (result == "parameter") {
                    error_div.innerHTML = "Sorry, some values may be invalid. Please try again.";
                }
                else {
                    error_div.innerHTML = "Sorry, an error was encountered. Please try again.";
                    console.log(result);
                }
            },
            error: function (err) {
                error_div.innerHTML = err;
                console.log(err);
            }
        })
    }
}

//======= Script to Delete a Service Center =========//
function deleteServiceCenter(id) {
    if (confirm('Are you sure you want to delete this center permanently?')) {
        $.ajax({
            type: 'POST',
            url: '/SRM/Home/DeleteServiceCenter',
            dataType: "text",
            data: { id: id },
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

//===== Function to populate the Edit Crew Member form ========//
function populateCenterEditForm(id, name) {
    const cntr_name_div = document.getElementById("cntr_name");
    document.getElementById("cntr_name").value = name;
    document.getElementById("cntr_id").value = id;
    cntr_name_div.focus();
}

//===== Function to save New Note to the database ========//
function addRequestNote() {
    console.log('saving note started ...')
    //== validation labels==//
    const error_div = document.getElementById("div-error");
    const note_content_input = document.getElementById("note_content");
    let request_id = document.getElementById("request_id").value;

    let from_name = document.getElementById("from_name").value;
    let note_content = document.getElementById("note_content").value;
    let source_page = document.getElementById("source_page").value;

    console.log('ServiceIncidentId = ' + request_id);
    console.log('From=' + from_name);
    console.log('note=' + note_content);
    console.log('source=' + source_page);

    if (note_content === null || note_content === undefined || note_content.trim().length === 0) {
        error_div.innerHTML = "Please enter a note!";
        note_content_input.focus();
        return;
    }
    error_div.innerHTML = "";

    console.log('calling api .....')
    $.ajax({
        type: 'POST',
        url: '/SRM/Home/SaveRequestNote',
        dataType: "text",
        data: { nm: from_name, msg: note_content, id: request_id },
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
                error_div.innerHTML = "Sorry, an error encountered. Please try again.";
                alert(result);
            }
        },
        error: function (err) {
            error_div.innerHTML = "Sorry, an error encountered. Please try again.";
            console.log(err);
        }
    })
}

//======= Script to Update a Service Incident Status =========//
function updateIncidentStatus(request_id, old_status, new_status) {
    $.ajax({
        type: 'POST',
        url: '/SRM/Home/UpdateIncidentStatus',
        dataType: "text",
        data: { id: request_id, ns: new_status, os: old_status },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}

//======= Script to Delete a Service Incident =========//
function deleteServiceIncident(id) {
    if (confirm('Are you sure you want to delete this request permanently?')) {
        $.ajax({
            type: 'POST',
            url: '/SRM/Home/DeleteServiceIncident',
            dataType: "text",
            data: { id: id },
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
