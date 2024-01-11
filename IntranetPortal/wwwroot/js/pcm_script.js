//======= Script to Delete a Post =========//
function deletePost(post_id) {
    if (confirm('Are you sure you want to delete this record permanently?')) {
        $.ajax({
            type: 'POST',
            url: '/ContentManager/Posts/DeletePost',
            dataType: "text",
            data: { id: post_id },
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
