$(document).ready(function () {

    //================= Search Employee Names =============================//
    $("#AssignedToName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#AssignedToName").val();
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