$(document).ready(function () {
    $("#State").autocomplete(
        {
            minLength: 2,
            source: function (request, response) {
                var text = $("#State").val();
                $.ajax({
                    type: "GET",
                    url: "/GlobalSettings/Locations/GetStateNames?stateName="+text,
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