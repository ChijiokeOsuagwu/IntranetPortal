$(document).ready(function () {
    $("#State").autocomplete(
        {
            minLength: 2,
            source: function (request, response) {
                var text = $("#State").val();
                $.ajax({
                    type: "GET",
                    url: "/GlobalSettings/Locations/GetStateNames?stateName=" + text,
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })

//================= Customer Names =============================//
    $("#CustomerName").autocomplete(
        {
            minLength: 2,
            source: function (request, response) {
                var text = $("#CustomerName").val();
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

//================= TeamLead Names =============================//
    $("#TeamLeadName").autocomplete(
        {
            minLength: 2,
            source: function (request, response) {
                var text = $("#TeamLeadName").val();
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

//================= TeamMember Names ===========================//
    $("#TeamMemberName").autocomplete(
        {
            minLength: 2,
            source: function (request, response) {
                var text = $("#TeamMemberName").val();
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

//================= Equipment Names ===========================//
    $("#AssetName").autocomplete(
        {
            minLength: 2,
            source: function (request, response) {
                var text = $("#AssetName").val();
                $.ajax({
                    type: "GET",
                    url: "/AssetManager/Settings/GetAssetNames?text=" + text,
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })

 //================= Recipient Names ===========================//
    $("#RecipientName").autocomplete(
        {
            minLength: 2,
            source: function (request, response) {
                var text = $("#RecipientName").val();
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

//======= Script to add new team member to the deployment batch =======//
function addTeamMember(assignment_id, deployment_id) {
    var member_name = $("#TeamMemberName").val();
    var member_role = $("#TeamMemberRole").val();
        $.ajax({
            type: 'POST',
            url: '/BAMS/Deployment/AddTeamMember',
            dataType: "text",
            data: { ad:assignment_id, dd:deployment_id, mn:member_name, mr:member_role },
            success: function (result) {
                switch (result) {
                    case "missing":
                        alert("Sorry, key values are missing. New team member could not be added.");
                        break;
                    case "exist":
                        alert("Sorry, this staff has already been deployed for this Assignment. Please check again.");
                        break;
                    case "done":
                        location.reload();
                        break;
                    default:
                        aler("Sorry, an error was encountered. New team member could not be added.");
                }
            },
            error: function () {
                alert('Sorry an error was encountered while attempting to add this team member. Please try again.');
            }
        })
}

//======= Script to add new equipment to the deployment batch =======//
function addEquipment(assignment_id, deployment_id) {
    var asset_name = $("#AssetName").val();
   $.ajax({
        type: 'POST',
        url: '/BAMS/Deployment/AddEquipment',
        dataType: "text",
        data: { ad: assignment_id, dd: deployment_id, an: asset_name },
        success: function (result) {
            switch (result) {
                case "missing":
                    alert("Sorry, key values are missing. New team member could not be added.");
                    break;
                case "exist":
                    alert("Sorry, this staff has already been deployed for this Assignment. Please check again.");
                    break;
                case "done":
                    location.reload();
                    break;
                default:
                    aler("Sorry, an error was encountered. New team member could not be added.");
            }
        },
        error: function () {
            alert('Sorry an error was encountered while attempting to add this team member. Please try again.');
        }
    })
}

//======= Script to remove team member from a deployment batch =======//
function removeTeamMember(team_member_id) {
    if (confirm("Are you sure you want to remove this team member?")) {
    $.ajax({
        type: 'POST',
        url: '/BAMS/Deployment/DeleteTeamMember',
        dataType: "text",
        data: { td: team_member_id },
        success: function (result) {
            if (result == "done") {
                location.reload();
            }
            else if (result == "none") {
                alert('Sorry, an error was encountered. Required parameter [td] is missing!');
            }
            else {
                alert('Sorry an error was encounted. Team member could not be removed.');
            }
        },
        error: function () {
            alert('Sorry an error was encountered while attempting to remove Team Member. Please try again.');
        }
    })
    }
}

//======= Script to remove equipment from a deployment batch =======//
function removeBatchEquipment(deployment_equipment_id) {
    if (confirm("Are you sure you want to remove this team member?")) {
        $.ajax({
            type: 'POST',
            url: '/BAMS/Deployment/DeleteEquipment',
            dataType: "text",
            data: { qd: deployment_equipment_id },
            success: function (result) {
                if (result == "done") {
                    alert('Equipment removed successfully!');
                    console.log(result);
                    location.reload();
                }
                else if (result == "none") {
                    alert('Sorry, an error was encountered. Required parameter [qd] is missing!');
                }
                else {
                    alert('Sorry an error was encounted. Equipment could not be removed.');
                }
            },
            error: function () {
                alert('Sorry an error was encountered while attempting to remove the equipment. Please try again.');
            }
        })
    }
}

//======= Script to delete Assignment Update ======================//
function deleteAssignmentUpdate(update_id) {
    if (confirm("Are you sure you want to delete this update?")) {
        $.ajax({
            type: 'POST',
            url: '/BAMS/Updates/Delete',
            dataType: "text",
            data: { id: update_id },
            success: function (result) {
                if (result == "done") {
                    alert('Update deleted successfully!');
                    location.reload();
                }
                else if (result == "none") {
                    alert('Sorry, an error was encountered. Required parameter [id] is missing!');
                }
                else {
                    alert('Sorry an error was encounted. Update could not be deleted.');
                }
            },
            error: function () {
                alert('Sorry an error was encountered while attempting to delete Update. Please try again.');
            }
        })
    }
}

//======= Script to remove equipment from Equipment Group =======//
function removeEquipmentFromGroup(asset_equipment_group_id) {
    if (confirm("Are you sure you want to remove this equipment from this group?")) {
        $.ajax({
            type: 'POST',
            url: '/BAMS/EquipmentGroups/Remove',
            dataType: "text",
            data: { id: asset_equipment_group_id },
            success: function (result) {
                if (result == "done") {
                    alert('Equipment removed successfully!');
                    location.reload();
                }
                else if (result == "none") {
                    alert('Sorry, an error was encountered. Required parameter is missing!');
                }
                else {
                    alert('Sorry an error was encounted. Equipment could not be removed.');
                }
            },
            error: function () {
                alert('Sorry an error was encountered while attempting to remove the equipment. Please try again.');
            }
        })
    }
}
