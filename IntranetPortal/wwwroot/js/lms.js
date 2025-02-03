

//===== Function to save New Leave Note to the database ========//
function addLeaveNote() {
    console.log('function started ...')
    //== validation labels==//
    const error_div = document.getElementById("div-error");
    const note_input = document.getElementById("leave_note");
    let leave_id = document.getElementById("leave_id").value;
    let from_name = document.getElementById("from_name").value;
    let leave_note = document.getElementById("leave_note").value;
    let source_page = document.getElementById("source_page").value;
    let leave_year = document.getElementById("leave_year").value;
    console.log('LeaveID=' + leave_id);
    console.log('From=' + from_name);
    console.log('note=' + leave_note);
    console.log('source=' + source_page);
    console.log('year=' + leave_year);

    if (leave_note === null || leave_note === undefined || leave_note.trim().length === 0) {
        error_div.innerHTML = "Please enter a note!";
        note_input.focus();
        return;
    }
    error_div.innerHTML = "";
    console.log('calling api .....')
    $.ajax({
        type: 'POST',
        url: '/LMS/Home/SaveLeaveNote',
        dataType: "text",
        data: { id: leave_id, nm: from_name, msg: leave_note },
        success: function (result) {
            if (result == "saved") {
                console.log(result);
                location.reload();
                //window.location.href = "/LMS/Home/LeaveNotes?id="+leave_id+"&sp="+source_page+"&yr="+leave_year;
            }
            else if (result == "failed") {
                error_div.innerHTML = "Sorry, note was not saved. Please try again.";
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
