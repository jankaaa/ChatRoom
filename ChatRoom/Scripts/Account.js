

function CallServerLogin(baseUrl) {
    var user = $('#Id').val();
    ShowWaitScreen();
    $.post(baseUrl + 'Account/Login?Id=' + user, function (data) {
        $('#login').empty();
        $('#login').append(data);
        OnAuth(user);
        RemoveWaitSCreen();
    });
}

function CallServerSignOut(baseUrl) {
    ShowWaitScreen();
    $.post(baseUrl + 'Account/SignOut', function (data) {
        $('#login').empty();
        $('#login').append(data);
        OnSignOut();
        RemoveWaitSCreen();
    });
}