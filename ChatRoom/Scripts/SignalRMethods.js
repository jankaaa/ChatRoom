
//Konstruktors.
$(function () {
    // Declare a proxy to reference the hub. 
    var chatHub = $.connection.chatHub;
    // Start Hub
    $.connection.hub.start().done(function () {
        SetClientMethods();
    });

});

//Pievienot aktīvo lietotāju sarakstam
function RegisterNewClient() {
    var chatHub = $.connection.chatHub;
    $.connection.hub.stateChanged(function (change) {
        if ($.signalR.connectionState["connected"] === change.newState) {

        }else{
            $.connection.hub.start().done(function () {
                SetClientMethods();
            });
        }
    });
}

//Pasaka serverim, lai pieliek sarakstam ja user.length > 0
function OnAuth(user) {
    var chatHub = $.connection.chatHub;
    chatHub.server.addPersonToGroup(user);
}

//Pasaka serverim lai izņem no sarakstam
function OnSignOut() {
    var chatHub = $.connection.chatHub;
    chatHub.server.removePersonToGroup();
}

//Reģistrē callback funkcijas no Hub
function SetClientMethods() {
    var Hubs = $.connection.chatHub;

    //Ziņa Visiem
    Hubs.client.sendToGlobal = function (username, msg) {
        var message = '<p>' + username + ': ' + msg + '</p>';
        $('#GlobalMessages').append(message);
    }

    //Privāta ziņa 
    Hubs.client.sendPrivateMessage = function (toUserId, userName, msg) {

    }

    //Error logs
    Hubs.client.sendErrorMessage = function (user, msg) {
        var message = '<p style="Color:Red">' + user + ': ' + msg + '</p>';
        $('#GlobalMessages').append(message);
    }

    //Visiem ziņa, ka ir pienācis jauns ļietotājs
    Hubs.client.onConnection = function (allUsers) {
        var userList = $('#ActiveUserCount');
        userList.empty();
        for (i = 0; i < allUsers.length; i++) {
            var users = '<p id="' + allUsers[i].Connection + '">' + allUsers[i].Name + '</p>';
            userList.append(users);
        }
    }

    //Izgājis lietotājs
    Hubs.client.onDisconnection = function (connId, userName) {
        $('#' + connId).remove();
    }

    //Kļūdu paziņojums
    Hubs.client.sendErrorAlert = function (message) {
        ShowErrorWindows(message);
    }

}

//Privātā ziņa
function SendPrivateMessage() {

}

//Globālā ziņa visiem
function SendGlobalMessage(message) {
    if (message.val().length > 0) {
        var Hubs = $.connection.chatHub;
        Hubs.server.globalMessages(message.val());
        message.val("");
        message.focus();
    }
}




