$(document).ready(function () {
    $("#CheckedOutTo").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#CheckedOutTo").val();
                $.ajax({
                    type: "GET",
                    url: "/ERM/Home/GetEmployeeNames",
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