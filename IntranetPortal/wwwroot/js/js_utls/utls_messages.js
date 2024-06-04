
$(document).ready(function () {

});

//======= Script to delete message by message detail Id ===========//
function deleteMessageByMessageDetailId(msg_detail_id) {
    if (confirm("Are you sure you want to delete this message?")) {
        $.ajax({
            type: 'POST',
            url: '/Home/DeleteMessage',
            dataType: "text",
            data: { id: msg_detail_id },
            success: function (result) {
                if (result == "done") {
                    //alert('Update deleted successfully!');
                    location.reload();
                }
                else if (result == "none") {
                    alert('Sorry, a required parameter has an invalid value.');
                }
                else {
                    alert('Sorry an error was encounted. Message could not be deleted.');
                }
            },
            error: function () {
                alert('Sorry an error was encountered while attempting to delete Message. Please try again.');
            }
        })
    }
}

//======= Script to delete all read messages by recipient id ======//
function deleteReadMessages(recipient_id) {
    if (confirm("All read messages will be deleted. Proceed?")) {
        $.ajax({
            type: 'POST',
            url: '/Home/DeleteRead',
            dataType: "text",
            data: { rd: msg_recipient_id },
            success: function (result) {
                if (result == "done") {
                    location.reload();
                }
                else if (result == "none") {
                    alert('Sorry, a required parameter has an invalid value.');
                }
                else {
                    alert('Sorry an error was encounted. Messages could not be deleted.');
                }
            },
            error: function () {
                alert('Sorry an error was encountered while attempting to delete Messages. Please try again.');
            }
        })
    }
}

//==== Script to update message read status by message detail Id ==//
function updateReadStatus(msg_detail_id) {
    $.ajax({
        type: 'POST',
        url: '/Home/ReadMessage',
        dataType: "text",
        data: { id: msg_detail_id },
        success: function (result) {
            if (result == "done") {
                console.log("Read Status Updated successfully!")
            }
            else {
                console.log("Updating Read Status failed.")
            }
    },
        error: function () {
            alert('Sorry an error was encountered while attempting to delete Message. Please try again.');
        }
    })
}
