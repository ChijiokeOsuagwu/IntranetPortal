

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
function archiveTaskFolder(folder_id) {
    $.ajax({
        type: 'POST',
        url: '/WSP/Workspace/UpdateTaskFolderArchive',
        dataType: "text",
        data: { id: folder_id, st: true },
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



//==== Task Items Action Scripts ===========//
