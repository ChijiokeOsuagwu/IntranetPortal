$(document).ready(function () {
    $("#an").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#an").val();
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

    $("#ParentAssetName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#ParentAssetName").val();
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

    $("#BinLocationName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#BinLocationName").val();
                $.ajax({
                    type: "GET",
                    url: "/AssetManager/Home/GetBinLocationNames",
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
    var parent_asset_name = $("#ParentAssetName").val();
    if (parent_asset_name != "" || parent_asset_name != undefined) {

        $.get("/AssetManager/Home/GetAssetParameters?asn=" + parent_asset_name, function (data) {
            const obj = JSON.parse(data)
            if (obj.asset_id == "" || obj.asset_id == undefined) {
                alert("The Parent Asset Name you entered appears to be incorrect.\n Please enter a correct Parent Asset Name and try again.");
                $("#ParentAssetName").focus();
            }
            else {
                $("#ParentAssetID").val(obj.asset_id);
                //$("#AssetTypeID").val(obj.asset_type_id);
                //$("#AssetCategoryID").val(obj.asset_category_id);
            }
        });
    }
}
