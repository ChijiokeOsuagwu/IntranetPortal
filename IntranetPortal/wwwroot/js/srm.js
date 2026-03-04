

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