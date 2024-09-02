$(document).ready(function () {
    $("#ss").autocomplete(
        {
            minLength: 3,
            source: function (request, response) {
                var text = $("#ss").val();
                $.ajax({
                    type: "GET",
                    url: "/UserAdministration/Home/GetNamesOfEmployeeUsers?text=" + text,
                    data: { text: request.term },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item, value: item }
                        }))
                    }
                })
            }
        })
})

//======= Script to Grant the Permission (with RoleID) to User (with UserID) =======//
function grantPermission(user_id, role_id) {
    const btnGrant = document.getElementById("btn_grant_" + role_id);
    $.ajax({
        type: 'POST',
        url: '/UserAdministration/Home/GrantUserPermission',
        dataType: "text",
        data: { usd: user_id, rld: role_id },
        success: function (result) {
            if (result == "granted") {
                btnGrant.disabled = true;
                location.reload();
            }
            else {
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

//===== Function to Revoke User Permission (with PermissionID) from User ========//
function revokePermission(user_id, role_id) {
    const btnRevoke = document.getElementById("btn_revoke_" + role_id);
    $.ajax({
        type: 'POST',
        url: '/UserAdministration/Home/RevokeUserPermission',
        dataType: "text",
        data: { usd: user_id, rld: role_id },
        success: function (result) {
            if (result == "revoked") {
                btnRevoke.disabled = true;
                location.reload();
            }
            else {
                alert('Revoking Permission failed!');
            }
        },
        error: function () {
            alert('Sorry Permission was not Revoked.');
            console.log('Failed ');
        }
    })
}

//===== Function to Revoke User Asset Permission (with AssetPermissionID) from User ========//
function revokeAssetPermission(asset_permission_id) {
    $.ajax({
        type: 'POST',
        url: '/UserAdministration/Home/RevokeUserAssetPermission',
        dataType: "text",
        data: { assetPermissionId: asset_permission_id },
        success: function (result) {
            if (result == "revoked") {
                location.reload();
            }
            else {
                alert('Revoking Permission failed!');
            }
        },
        error: function () {
            alert('Sorry Permission was not Revoked.');
        }
    })
}

//====== Function to Grant a Location's Permission to a User ====================//
function grantLocationPermission(user_id, location_id) {
    const btnGrant = document.getElementById("btn_grant_" + location_id);
    $.ajax({
        type: 'POST',
        url: '/UserAdministration/Home/GrantLocationPermission',
        dataType: "text",
        data: { userId: user_id, locationId: location_id },
        success: function (result) {
            if (result == "granted") {
                btnGrant.disabled = true;
                location.reload();
            }
            else {
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

//===== Function to Revoke User Location Permission (with LocationPermissionID) ========//
function revokeLocationPermission(location_permission_id) {
    $.ajax({
        type: 'POST',
        url: '/UserAdministration/Home/RevokeUserLocationPermission',
        dataType: "text",
        data: { locationPermissionId: location_permission_id },
        success: function (result) {
            if (result == "revoked") {
                location.reload();
            }
            else {
                alert('Revoking Permission failed!');
            }
        },
        error: function () {
            alert('Sorry Permission was not Revoked.');
        }
    })
}
