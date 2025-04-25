

$(document).ready(function () {

    //============ Search Employee Names =======//
    $("#ToEmployeeName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#ToEmployeeName").val();
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

    $("#FromEmployeeName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#FromEmployeeName").val();
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

});



//======= Script to Delete a Task Item =========//
function deleteTask(task_item_id) {
    if (confirm('Are you sure you want to delete this task permanently?')) {
        $.ajax({
            type: 'POST',
            url: '/WSP/Workspace/DeleteTaskItem',
            dataType: "text",
            data: { id: task_item_id },
            success: function (result) {
                if (result == "success") {
                    location.reload();
                }
                else {
                    console.log(result);
                }
            },
            error: function (error) {
                console.log(error);
            }
        })
    }
}








//======= Script to Archive  a TaskList =========//
function archiveTaskList(task_list_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/UpdateTaskListArchive',
        dataType: "text",
        data: { id: task_list_id, st: true },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}

//======= Script to reactivate a TaskList =======//
function reactivateTaskList(task_list_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/UpdateTaskListArchive',
        dataType: "text",
        data: { id: task_list_id, st: false },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}

//======= Script to Return Approved Task List =========//
function returnTaskList(task_list_id, submission_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/ReturnTaskList',
        dataType: "text",
        data: { id: task_list_id, sd: submission_id },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}

//======= Script to Delete a Task List Submission =========//
function deleteTaskListSubmission(submission_id) {
    if (confirm('Are you sure you want to delete this record permanently?')) {
        $.ajax({
            type: 'POST',
            url: '/WKS/Tasks/DeleteTaskListSubmission',
            dataType: "text",
            data: { id: submission_id },
            success: function (result) {
                if (result == "success") {
                    location.reload();
                }
                else {
                    console.log(result);
                }
            },
            error: function () {
                console.log('Error Code: 500. Failure due to server error.');
            }
        })
    }
}

//======= Script to Close a Task Item =========//
function closeTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/UpdateTaskStatus',
        dataType: "text",
        data: { id: task_item_id, cls: true },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}

//======= Script to Re-Open a Task Item =========//
function ReopenTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/UpdateTaskStatus',
        dataType: "text",
        data: { id: task_item_id, cls: false },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}

//======= Script to Cancel a Task Item =========//
function CancelTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/CancelTaskItem',
        dataType: "text",
        data: { id: task_item_id, ccl: true },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}

//======= Script to Cancel a Task Item =========//
function ReverseCancelTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/CancelTaskItem',
        dataType: "text",
        data: { id: task_item_id, ccl: false },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}


//======= Script to Delete a Task Item =========//
function DeleteTaskItem(task_item_id) {
    if (confirm('This task will be deleted permanently?')) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/DeleteTask',
        dataType: "text",
        data: { id: task_item_id },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
    }
}


//======= Script to Approve a Task Item =========//
function approveTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/ApproveTaskItem',
        dataType: "text",
        data: { id: task_item_id},
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}

//======= Script to Decline a Task Item =========//
function declineTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WKS/Tasks/DeclineTaskItem',
        dataType: "text",
        data: { id: task_item_id },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function () {
            console.log('Error Code: 500. Failure due to server error.');
        }
    })
}


