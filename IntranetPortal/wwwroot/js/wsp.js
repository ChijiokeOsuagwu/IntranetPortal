
$(document).ready(function () {

    //============ Search Employee Names =======//
    $("#ToEmployeeName").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#ToEmployeeName").val();
                var emp = $("#FromEmployeeID").val();
                $.ajax({
                    type: "GET",
                    url: "/ERM/Home/GetOtherEmployeeNames?text=" + text+ "&emp=" +emp,
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


//===== Function to save New Note to the database ========//
function addNote() {
    console.log('saving note started ...')
    //== validation labels==//
    const error_div = document.getElementById("div-error");
    const note_input = document.getElementById("leave_note");
    let folder_id = document.getElementById("folder_id").value;
    let task_id = document.getElementById("task_id").value;
    let project_id = document.getElementById("project_id").value;
    let note_type = document.getElementById("note_type").value;
    let from_name = document.getElementById("from_name").value;
    let note_content = document.getElementById("note_content").value;
    let source_page = document.getElementById("source_page").value;
    
    console.log('FolderID=' + folder_id);
    console.log('TaskID=' + task_id);
    console.log('ProjectID=' + project_id);
    console.log('From=' + from_name);
    console.log('note=' + note_content);
    console.log('source=' + source_page);
    console.log('type=' + note_type);

    if (note_content === null || note_content === undefined || note_content.trim().length === 0) {
        error_div.innerHTML = "Please enter a note!";
        note_input.focus();
        return;
    }
    error_div.innerHTML = "";

    console.log('calling api .....')
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/SaveNote',
        dataType: "text",
        data: {nm: from_name, msg: note_content, td: task_id, fd:folder_id, pd:project_id },
        success: function (result) {
            if (result == "saved") {
                console.log(result);
                location.reload();
            }
            else if (result == "failed") {
                error_div.innerHTML = "Sorry, note was not saved. Please try again.";
            }
            else if (result == "parameter") {
                error_div.innerHTML = "Sorry, some values are invalid. Please try again.";
            }
            else {
                error_div.innerHTML = "Sorry, an error encountered. Please try again.";
                alert(result);
            }
        },
        error: function (err) {
            error_div.innerHTML = "Sorry, an error encountered. Please try again.";
            console.log(err);
        }
    })
}

//======= Script to Archive  a Task Folder =========//
function updateTaskFolderArchive(folder_id, archive_folder) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/UpdateTaskFolderArchive',
        dataType: "text",
        data: { id: folder_id, st: archive_folder },
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
function reactivateTaskFolder(folder_id) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/UpdateTaskFolderArchive',
        dataType: "text",
        data: { id: folder_id, st: false },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                console.log(result);
            }
        },
        error: function (err) {
            console.log(err);
        }
    })
}

//======= Script to Return Approved Task List =========//
function returnTaskFolder(folder_id, submission_id, submission_type) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/ReturnTaskFolder',
        dataType: "text",
        data: { id: folder_id, sd: submission_id, ps: submission_type},
        success: function (result) {
            if (result == "success") {
                window.location.replace("/WSP/Workspace/SubmittedToMe")
            }
            else {
                console.log(result);
            }
        },
        error: function (err) {
            console.log(err);
            alert(err);
        }
    })
}

//======= Script to Delete a Task Item =========//
function deleteTaskItem(task_item_id) {
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

//======= Script to Close a Task Item =========//
function closeTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/UpdateTaskStatus',
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
        url: '/WSP/Workspace/UpdateTaskStatus',
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

//======= Script to Update a Task Item Progress Status =========//
function updateTaskProgress(task_item_id, new_status, old_status) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/UpdateTaskItemProgressStatus',
        dataType: "text",
        data: { id: task_item_id, ns: new_status, os: old_status},
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




//======= Script to Approve a Task Item =========//
function approveTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/ApproveTaskItem',
        dataType: "text",
        data: { id: task_item_id },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                alert(result);
                console.log(result);
            }
        },
        error: function (msg) {
            alert(msg);
            console.log(msg);
        }
    })
}

//======= Script to Decline a Task Item =========//
function declineTaskItem(task_item_id) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/DeclineTaskItem',
        dataType: "text",
        data: { id: task_item_id },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                alert(result);
                console.log(result);
            }
        },
        error: function (error_message) {
            alert(error_message);
            console.log(error_message);
        }
    })
}

//======= Script to Delete a Task List Submission =========//
function deleteFolderSubmission(submission_id) {
    if (confirm('Are you sure you want to remove this record?')) {
        $.ajax({
            type: 'POST',
            url: '/WSP/Workspace/DeleteTaskFolderSubmission',
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
            error: function (err) {
                console.log(err);
            }
        })
    }
}

//======= Script to Approve a Task Item =========//
function evaluateTaskItem(task_item_id, task_folder_id, task_evaluator_id, quality_score, evaluation_header_id, evaluation_detail_id) {
    console.log("Calling ajax function....")
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/EvaluateTaskItem',
        dataType: "text",
        data: { td: task_item_id, fd:task_folder_id, ed:task_evaluator_id, qs:quality_score, hd: evaluation_header_id, dd:evaluation_detail_id },
        success: function (result) {
            if (result == "success") {
                console.log("Function completed successfully! ");
                location.reload();
            }
            else {
                alert(result);
                console.log(result);
            }
        },
        error: function (msg) {
            alert(msg);
            console.log(msg);
        }
    })
}

//======= Script to Approve a Task Item =========//
function moveTaskToFolder(task_item_id, folder_id) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/MoveTaskToFolder',
        dataType: "text",
        data: { id: task_item_id, fd: folder_id },
        success: function (result) {
            if (result == "success") {
                location.reload();
            }
            else {
                alert(result);
                console.log(result);
            }
        },
        error: function (msg) {
            alert(msg);
            console.log(msg);
        }
    })
}
