$(document).ready(function () {
    $("#ReportsToEmployeeName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#ReportsToEmployeeName").val();
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
});