$(document).ready(function () {
    $("#AssetName").autocomplete(
        {
            //minLength: 3,
            source: function (request, response) {
                var text = $("#AssetName").val();
                $.ajax({
                    type: "GET",
                    url: "/AssetManager/Home/GetAssetNames",
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })


    $("#SupervisedBy").autocomplete(
        {
            //minLength: 3,
            source: function (request, response) {
                var text = $("#SupervisedBy").val();
                $.ajax({
                    type: "GET",
                    url: "/EmployeeRecords/Home/GetEmployeeNames",
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })

    $("#ApprovedBy").autocomplete(
        {
            //minLength: 3,
            source: function (request, response) {
                var text = $("#ApprovedBy").val();
                $.ajax({
                    type: "GET",
                    url: "/EmployeeRecords/Home/GetEmployeeNames",
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


//=============== Script to populate fields with the AssetName retrieved by the autocomplete function ================================//
var getAssetParameters = function () {
    var asset_name = $("#AssetName").val();
    if (asset_name == "" || asset_name == undefined) {
        alert("The Equipment Name you entered appears to be incorrect. \n Please enter a correct Equipment Name and try again.");
        return false;
    }

    $.get("/AssetManager/Home/GetAssetParameters?asn=" + asset_name, function (data) {
        const obj = JSON.parse(data)
        if (obj.asset_id == "" || obj.asset_id == undefined) {
            alert("The Equipment Name you entered appears to be incorrect.\n Please enter a correct Equipment Name and try again.");
            $("#AssetName").focus();
        }
        else {
            $("#AssetID").val(obj.asset_id);
            $("#AssetTypeID").val(obj.asset_type_id);
            $("#AssetCategoryID").val(obj.asset_category_id);
        }
    });
}



