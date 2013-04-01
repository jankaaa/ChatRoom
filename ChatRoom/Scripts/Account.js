

function CallServerLogin(baseUrl) {
    var user = $('#Id').val();
    ShowWaitScreen();
    $.post(baseUrl + 'Account/Login?Id=' + user, function (data) {
        RemoveWaitSCreen();
        $('#login').empty();
        $('#login').append(data);
        location = '';
        //signalr izmanto to pašu authentifikāciju, līdz ar to- connection Id sabojājas
        //OnAuth(user);
      
    });
}

function CallServerSignOut(baseUrl) {
    ShowWaitScreen();
    OnSignOut();
    $.post(baseUrl + 'Account/SignOut', function (data) {
        RemoveWaitSCreen();
        $('#login').empty();
        $('#login').append(data);
        location = '';
    });
}