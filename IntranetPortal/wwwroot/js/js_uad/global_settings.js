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