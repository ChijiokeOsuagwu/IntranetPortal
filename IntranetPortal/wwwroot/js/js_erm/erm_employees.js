$(document).ready(function () {

    //================= Search Employee Names =============================//
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

    $("#pn").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#pn").val();
                $.ajax({
                    type: "GET",
                    url: "/ERM/Home/GetPersonNames?text=" + text,
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })

    //===== Script to get the expected last worked date =====//
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

    //===== Script to get the last leave date from the start date and duration =====//
    getLastLeaveDate = function () {
        let leave_start_date = $("#LeaveStartDate").val();
        let leave_duration = $("#Duration").val();
        let duration_type_id = $("#DurationTypeId").val();

        if ((leave_start_date != "" || leave_start_date != undefined) && (leave_duration != 0 || leave_duration != undefined) && (duration_type_id != "" || duration_type_id != undefined)) {
            console.log(leave_start_date);
            console.log(leave_duration);
            console.log(duration_type_id);

            $.get("/LMS/Home/GetLeaveEndDate?sd=" + leave_start_date + "&dr=" + leave_duration + "&dt=" + duration_type_id, function (data) {
                const obj = JSON.parse(data)
                console.log(obj);
                if (obj.errormsg !== "" || obj.errormsg !== null || obj.errormsg !== undefined) {
                    $("#LeaveEndDate").val(obj.result);
                    //$("#LeaveEndDate").focus();
                }
                else {
                    $("#LeaveEndDate").focus();
                    document.getElementById("errorSpan").innerText = obj.errormsg;
                }
            });
        }
    }


    //===== Script to populate fields with the EmployeeName retrieved by the autocomplete function =====//
    //var getEmployeeParameters = function () {
    //    var parent_asset_name = $("#ParentAssetName").val();
    //    if (parent_asset_name != "" || parent_asset_name != undefined) {

    //        $.get("/AssetManager/Home/GetAssetParameters?asn=" + parent_asset_name, function (data) {
    //            const obj = JSON.parse(data)
    //            if (obj.asset_id == "" || obj.asset_id == undefined) {
    //                alert("The Parent Asset Name you entered appears to be incorrect.\n Please enter a correct Parent Asset Name and try again.");
    //                $("#ParentAssetName").focus();
    //            }
    //            else {
    //                $("#ParentAssetID").val(obj.asset_id);
    //                //$("#AssetTypeID").val(obj.asset_type_id);
    //                //$("#AssetCategoryID").val(obj.asset_category_id);
    //            }
    //        });
    //    }
    //}

});