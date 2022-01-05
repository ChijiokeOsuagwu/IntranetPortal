$(document).ready(function () {
    $('#employeesDatatable').dataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/EmployeeRecords/Home/Register",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "employeeNo1", "name": "No", "autoWidth": true },
            { "data": "fullName", "name": "Name", "autoWidth": true },
            { "data": "sex", "name": "Sex", "autoWidth": true },
            { "data": "unitName", "name": "Unit", "autoWidth": true },
            { "data": "locationName", "name": "Location", "autoWidth": true },
            { "data": "currentDesignation", "name": "Position", "autoWidth": true },
            {
                "render": function (data, row) { return "<a href='#' class='btn btn-danger' onclick=DeleteCustomer('" + row.id + "'); >Delete</a>"; }
            },
        ]
    });
});
