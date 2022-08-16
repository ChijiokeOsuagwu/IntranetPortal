
//======= Script to Grant the Permission (with RoleID) to User (with UserID) =======//
function grantPermission(user_id, role_id) {
    const btnGrant = document.getElementById("btn_grant_" + role_id);
    $.ajax({
        type: 'POST',
        url: '/UserAdministration/Home/GrantUserPermission',
        dataType: "text",
        data: {usd:user_id, rld:role_id},
        success: function (result) {
            if (result == "granted") {
                alert('Permission Granted Successfully!');
                console.log(result);
                btnGrant.disabled = true;
            }
            else{
                alert('Granting Permission failed!');
                console.log(result);
            }
        },
        error: function () {
            alert('Sorry Permission was not Granted.');
            console.log('Failed ');
        }
    })
}


//===== Function to Revoke the Permission (with PermissionID) from User ========//
function revokePermission(user_id, role_id) {
    const btnRevoke = document.getElementById("btn_revoke_" + role_id);
    $.ajax({
        type: 'POST',
        url: '/UserAdministration/Home/RevokeUserPermission',
        dataType: "text",
        data: { usd: user_id, rld: role_id },
        success: function (result) {
            if (result == "revoked") {
                alert('Permission Revoked Successfully!');
                console.log(result);
                btnRevoke.disabled = true;
            }
            else {
                alert('Revoking Permission failed!');
                console.log(result);
            }
        },
        error: function () {
            alert('Sorry Permission was not Revoked.');
            console.log('Failed ');
        }
    })
}