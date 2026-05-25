
$(document).ready(function () {

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

    //===== Get the expected Last Worked Date =====//
    getExpectedLastWorkedDate = function () {
        let notice_date = $("#NoticeServeDate").val();
        let notice_period = $("#NoticePeriodInMonths").val();
        if ((notice_date != "" || notice_date != undefined) && (notice_period != "" || notice_period != undefined)) {
            console.log(notice_date);
            console.log(notice_period);

            $.get("/ERM/Home/GetExpectedLastWorkDate?nd=" + notice_date + "&np=" + notice_period, function (data) {
                const obj = JSON.parse(data)
                console.log(obj);
                if (obj.errormsg !== "" || obj.errormsg !== null || obj.errormsg !== undefined) {
                    $("#ExpectedLastWorkedDate").val(obj.result);
                    $("#ExpectedLastWorkedDate").focus();
                }
                else {
                    $("#ExpectedLastWorkedDate").focus();
                    document.getElementById("errorSpan").innerText = obj.errormsg;
                }
            });
        }
    }

    getOutstandingWorkDays = function () {
        let expected_last_date = $("#ExpectedLastWorkedDate").val();
        let actual_last_date = $("#ActualLastWorkedDate").val();

        if ((expected_last_date != "" || expected_last_date != undefined) && (actual_last_date != "" || actual_last_date != undefined)) {
            console.log(expected_last_date);
            console.log(actual_last_date);

            $.get("/ERM/Home/GetOutstandingWorkDays?xd=" + expected_last_date + "&ad=" + actual_last_date, function (data) {
                const obj = JSON.parse(data)
                console.log(obj);
                if (obj.errormsg !== "" || obj.errormsg !== null || obj.errormsg !== undefined) {
                    $("#OutstandingWorkDays").val(obj.result);
                    $("#OutstandingWorkDays").focus();
                }
                else {
                    $("#ExpectedLastWorkedDate").focus();
                    document.getElementById("noErrorSpan").innerText = obj.errormsg;
                }
            });
        }
    }

    //======= Leave Plan Helper Functions =======//
    //===== Get the Last Leave Date from the Start Date and Duration =====//
    getLastLeaveDate = function () {
        let leave_start_date = $("#LeavePlanStartDate").val();
        let leave_duration = $("#LeavePlanDuration").val();
        let duration_type_id = $("#LeavePlanDurationTypeId").val();

        if ((leave_start_date != "" || leave_start_date != undefined) && (leave_duration != 0 || leave_duration != undefined) && (duration_type_id != "" || duration_type_id != undefined)) {
            console.log(leave_start_date);
            console.log(leave_duration);
            console.log(duration_type_id);

            $.get("/LVM/Leave/GetLeaveEndDate?sd=" + leave_start_date + "&dr=" + leave_duration + "&dt=" + duration_type_id, function (data) {
                const obj = JSON.parse(data)
                console.log(obj);
                if (obj.errormsg !== "" || obj.errormsg !== null || obj.errormsg !== undefined) {
                    $("#LeavePlanEndDate").val(obj.result);
                    //$("#LeaveEndDate").focus();
                }
                else {
                    $("#LeavePlanEndDate").focus();
                    document.getElementById("leavePlanEndDateErrorSpan").innerText = obj.errormsg;
                }
            });
        }
    }

    //===== Get Expected Resumption Date from the Leave End Date =====//
    getExpectedResumptionDate = function () {
        let leave_end_date = $("#LeavePlanEndDate").val();

        if (leave_end_date != "" || leave_end_date != undefined) {
            console.log(leave_end_date);

            $.get("/LVM/Leave/GetResumptionDate?ed=" + leave_end_date, function (data) {
                const obj = JSON.parse(data)
                console.log(obj);
                if (obj.errormsg !== "" || obj.errormsg !== null || obj.errormsg !== undefined) {
                    $("#LeavePlanResumptionDate").val(obj.result);
                    //$("#LeaveEndDate").focus();
                }
                else {
                    $("#LeavePlanResumptionDate").focus();
                    document.getElementById("LeavePlanResumptionDateErrorSpan").innerText = obj.errormsg;
                }
            });
        }
    }

    //===== Leave Request Helper Functions =======//
    //===== Get the Last Leave Date from the Start Date and Duration =====//
    getRequestLastLeaveDate = function () {
        let requested_start_date = $("#RequestedStartDate").val();
        let requested_duration = $("#RequestedDuration").val();
        let requested_duration_type_id = $("#RequestedDurationTypeId").val();

        if ((requested_start_date != "" || requested_start_date != undefined) && (requested_duration != 0 || requested_duration != undefined) && (requested_duration_type_id != "" || requested_duration_type_id != undefined)) {
            console.log(requested_start_date);
            console.log(requested_duration);
            console.log(requested_duration_type_id);

            $.get("/LVM/Leave/GetLeaveEndDate?sd=" + requested_start_date + "&dr=" + requested_duration + "&dt=" + requested_duration_type_id, function (data) {
                const obj = JSON.parse(data)
                console.log(obj);
                if (obj.errormsg !== "" || obj.errormsg !== null || obj.errormsg !== undefined) {
                    $("#RequestedEndDate").val(obj.result);
                }
                else {
                    $("#RequestedEndDate").focus();
                    document.getElementById("requestedEndDateErrorSpan").innerText = obj.errormsg;
                }
            });
        }
    }

    //===== Get Expected Resumption Date from the Leave End Date =====//
    getRequestedExpectedResumptionDate = function () {
        let requested_end_date = $("#RequestedEndDate").val();

        if (requested_end_date != "" || requested_end_date != undefined) {
            console.log(requested_end_date);

            $.get("/LVM/Leave/GetResumptionDate?ed=" + requested_end_date, function (data) {
                const obj = JSON.parse(data)
                console.log(obj);
                if (obj.errormsg !== "" || obj.errormsg !== null || obj.errormsg !== undefined) {
                    $("#RequestedResumptionDate").val(obj.result);
                    //$("#LeaveEndDate").focus();
                }
                else {
                    $("#RequestedResumptionDate").focus();
                    document.getElementById("requestedResumptionDateErrorSpan").innerText = obj.errormsg;
                }
            });
        }
    }


})

//===== Function to save New Leave Note to the database ========//
addLeaveNote = function () {
    console.log('function started ...')
    //== validation labels==//
    const error_div = document.getElementById("div-error");
    const note_input = document.getElementById("leave_note");
    let leave_plan_id = document.getElementById("leave_plan_id").value;
    let leave_request_id = document.getElementById("leave_request_id").value;

    let from_name = document.getElementById("from_name").value;
    let leave_note = document.getElementById("leave_note").value;
    let source_page = document.getElementById("source_page").value;
    let leave_year = document.getElementById("leave_year").value;
    console.log('LeavePlanId: ' + leave_plan_id);
    console.log('LeaveRequestId: ' + leave_request_id);

    console.log('From: ' + from_name);
    console.log('Note: ' + leave_note);
    console.log('source: ' + source_page);
    console.log('LeaveYear: ' + leave_year);
    console.log('LeavePlanId: ' + leave_plan_id);
    console.log('LeaveRequestId: ' + leave_request_id);

    if (leave_note === null || leave_note === undefined || leave_note.trim().length === 0) {
        error_div.innerHTML = "Please enter a note!";
        note_input.focus();
        return;
    }
    error_div.innerHTML = "";
    console.log('calling api .....')
    $.ajax({
        type: 'POST',
        url: '/LVM/Leave/SaveLeaveNote',
        dataType: "text",
        data: { nm: from_name, msg: leave_note, pd: leave_plan_id, rd: leave_request_id },
        success: function (result) {
            if (result == "saved") {
                console.log(result);
                location.reload();
            }
            else if (result == "failed") {
                error_div.innerHTML = "Sorry, note was not saved. Please try again.";
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


//===== Function to delete a submission message ========//
function deleteLeaveSubmission(submission_id) {
    if (confirm("Are you sure you want to delete this item?")) {
        $.ajax({
            type: 'POST',
            url: '/LVM/Leave/DeleteLeaveSubmission',
            dataType: "text",
            data: { sd: submission_id },
            success: function (result) {
                if (result == "deleted") {
                    location.reload();
                }
                else {
                    alert('Deleting record failed!');
                    console.log(result);
                }
            },
            error: function () {
                alert('Sorry deleting operation could not be completed.');
                console.log('Failed ');
            }
        })
    }

}

