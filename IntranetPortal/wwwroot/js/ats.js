
$(document).ready(function () {

    //===== Search Employee ====//
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
    //===== Search Customers ===//
    $("#cn").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#cn").val();
                console.log(text);
                $.ajax({
                    type: "GET",
                    url: "/PartnerServices/Customers/GetCustomerNames?customerName=" + text,
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })
    //===== Search Customers ===//
    $("#qn").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#qn").val();
                console.log(text);
                $.ajax({
                    type: "GET",
                    url: "/AssetManager/Home/GetAssetNames?text=" + text,
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })

    const feedbackCheckBox = document.getElementById('chx_feedback');
    const feedbackDiv = document.getElementById('div_feedback_details');
    feedbackDiv.hidden = !feedbackCheckBox.checked; //Toggles the hidden state

    const incidentsCheckBox = document.getElementById('chx_incidents');
    const incidentDiv = document.getElementById('div_incident_details');
    incidentDiv.hidden = !incidentsCheckBox.checked; //Toggles the hidden state
});

//===== Function to save New Note to the database ========//
function addAssignmentNote() {
    console.log('saving note started ...')
    //== validation labels==//
    const error_div = document.getElementById("div-error");
    const note_input = document.getElementById("leave_note");
    let assignment_id = document.getElementById("assignment_id").value;
    let from_name = document.getElementById("from_name").value;
    let note_content = document.getElementById("note_content").value;
    let source_page = document.getElementById("source_page").value;

    console.log('AssignmentID=' + assignment_id);
    console.log('From=' + from_name);
    console.log('note=' + note_content);
    console.log('source=' + source_page);

    if (note_content === null || note_content === undefined || note_content.trim().length === 0) {
        error_div.innerHTML = "Please enter a note!";
        note_content.focus();
        return;
    }
    error_div.innerHTML = "";

    console.log('calling api .....')
    $.ajax({
        type: 'POST',
        url: '/ATS/Assignments/SaveNote',
        dataType: "text",
        data: { nm: from_name, msg: note_content, id: assignment_id },
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

//======= Script to Delete a Task Item =========//
function deleteAssignment(assignment_id) {
    if (confirm('Are you sure you want to delete this Assignment permanently?')) {
        $.ajax({
            type: 'POST',
            url: '/ATS/Assignments/DeleteAssignment',
            dataType: "text",
            data: { id: assignment_id },
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

//===== Function to save New Crew Member to the database ========//
function addCrewMember() {
    console.log('saving crew member started ...')

    //== validation labels==//
    const error_div = document.getElementById("div-error");
    let assignment_crew_id = document.getElementById("crew_member_id").value;
    let staff_name = document.getElementById("sn").value;
    let assignment_id = document.getElementById("assignment_id").value;
    let staff_role1 = document.getElementById("role1").value;
    let staff_role2 = document.getElementById("role2").value;
    let staff_role3 = document.getElementById("role3").value;
    let source_page = document.getElementById("source_page").value;

    console.log('AssignmentID:' + assignment_id);
    console.log('StaffName: ' + staff_name);
    console.log('Staff Role1: ' + staff_role1);
    console.log('Staff Role2: ' + staff_role2);
    console.log('Staff Role3: ' + staff_role3);
    console.log('Source: ' + source_page);

    if (staff_name === null || staff_name === undefined || staff_name.trim().length === 0) {
        error_div.innerHTML = "Please enter a name!";
        document.getElementById("sn").focus();
        return;
    }

    if (staff_role1 === null || staff_role1 === undefined || staff_role1.trim().length === 0) {
        error_div.innerHTML = "Please select a role!";
        document.getElementById("role1").focus();
        return;
    }

    error_div.innerHTML = "";

    if (assignment_crew_id !== undefined && assignment_crew_id != null && assignment_crew_id > 0) {
        console.log('calling UpdateCrewMember api .....')
        $.ajax({
            type: 'POST',
            url: '/ATS/Assignments/UpdateCrewMember',
            dataType: "text",
            data: { id: assignment_crew_id, nm: staff_name, r1: staff_role1, r2: staff_role2, r3: staff_role3 },
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
        console.log('calling AddCrewMember api .....')
        $.ajax({
            type: 'POST',
            url: '/ATS/Assignments/AddCrewMember',
            dataType: "text",
            data: { id: assignment_id, nm: staff_name, r1: staff_role1, r2: staff_role2, r3: staff_role3 },
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

//======= Script to Remove an Assignment Crew Member =========//
function removeCrewMember(assignment_crew_id) {
    if (confirm('Drop this member from this crew?')) {
        $.ajax({
            type: 'POST',
            url: '/ATS/Assignments/RemoveAssignmentCrewMember',
            dataType: "text",
            data: { id: assignment_crew_id },
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

//======= Script to Delete a Task Item =========//
function updateCrewLead(assignment_crew_id, isLead) {
        $.ajax({
            type: 'POST',
            url: '/ATS/Assignments/UpdateAssignmentCrewLead',
            dataType: "text",
            data: { id: assignment_crew_id, isl:isLead },
            success: function (result) {
                console.log(result);
                    location.reload();
            },
            error: function (error) {
                console.log(error);
            }
        })
}

//===== Function to populate the Edit Crew Member form ========//
function populateEditForm(assignment_crew_id, crew_member_name, member_role1, member_role2, member_role3) {
    const role1_div = document.getElementById("role1");
    document.getElementById("sn").value = crew_member_name;
    document.getElementById("crew_member_id").value = assignment_crew_id;
    document.getElementById("role1").value = member_role1;
    document.getElementById("role2").value = member_role2;
    document.getElementById("role3").value = member_role3;
    role1_div.focus();
}

//===== Function to add New Assignment Equipment to the database ========//
function addEquipmentAssignment() {
    console.log('saving equipment started ...')

    //== validation labels==//
    const error_div = document.getElementById("div-error");
    let assignment_equipment_id = document.getElementById("assignment_equipment_id").value;
    let staff_id = document.getElementById("sd").value;
    let assignment_id = document.getElementById("assignment_id").value;
    let equipment_name = document.getElementById("qn").value;
    let source_page = document.getElementById("source_page").value;

    console.log('AssignmentID:' + assignment_id);
    console.log('StaffID: ' + staff_id);
    console.log('EquipmentName: ' + equipment_name);
    console.log('AssignmentEquipmentID: ' + assignment_equipment_id);
    console.log('Source: ' + source_page);

    if (staff_id === null || staff_id === undefined || staff_id.trim().length === 0) {
        error_div.innerHTML = "Please enter a staff to assign this equipment to!";
        document.getElementById("sd").focus();
        return;
    }

    if (equipment_name === null || equipment_name === undefined || equipment_name.trim().length === 0) {
        error_div.innerHTML = "Please enter an equipment name!";
        document.getElementById("qn").focus();
        return;
    }

    error_div.innerHTML = "";

    if (assignment_equipment_id !== undefined && assignment_equipment_id != null && assignment_equipment_id > 0) {
        console.log('calling UpdateEquipment api .....')
        $.ajax({
            type: 'POST',
            url: '/ATS/Assignments/UpdateEquipment',
            dataType: "text",
            data: { id: assignment_equipment_id, sd: staff_id, qn: equipment_name },
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
        console.log('calling AddEquipment api .....')
        $.ajax({
            type: 'POST',
            url: '/ATS/Assignments/AddEquipment',
            dataType: "text",
            data: { id: assignment_id, sd: staff_id, qn: equipment_name },
            success: function (result) {
                if (result == "saved") {
                    console.log(result);
                    location.reload();
                }
                else if (result == "failed") {
                    error_div.innerHTML = "Sorry, equipment was not saved. Please try again.";
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

//======= Script to Remove an Assignment Crew Member =========//
function removeEquipment(assignment_equipment_id) {
    if (confirm('Drop this equipment from this assignment?')) {
        $.ajax({
            type: 'POST',
            url: '/ATS/Assignments/RemoveAssignmentEquipment',
            dataType: "text",
            data: { id: assignment_equipment_id },
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
function populateAssignmentEquipmentEditForm(assignment_equipment_id, asset_name, employee_id) {
    const select_staff_div = document.getElementById("sd");
    document.getElementById("sd").value = employee_id;
    document.getElementById("qn").value = asset_name;
    document.getElementById("assignment_equipment_id").value = assignment_equipment_id;
    select_staff_div.focus();
}

//===== Function to Toggle the visibility of the Feedback Details Div Element ========//
function toggleFeedbackInputVisibility() {
    const feedbackDiv = document.getElementById('div_feedback_details');
    feedbackDiv.hidden = !feedbackDiv.hidden; // Toggles the hidden state
}

//===== Function to Toggle the visibility of the Incidents Details Div Element ========//
function toggleIncidentInputVisibility() {
    const incidentDiv = document.getElementById('div_incident_details');
    incidentDiv.hidden = !incidentDiv.hidden; // Toggles the hidden state
}

