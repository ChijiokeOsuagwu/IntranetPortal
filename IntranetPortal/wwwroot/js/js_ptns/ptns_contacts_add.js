$(document).ready(function () {
    $("#BusinessName").autocomplete(
        {
            minLength: 2,
            source: function (request, response) {
                var text = $("#BusinessName").val();
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
});