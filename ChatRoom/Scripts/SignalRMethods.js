
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
        var generalChatBox = $('#GlobalMessages');
        var message = '<div class="GlobMsg"> <b>' + username + '</b>: ' + msg + '</div>';
        generalChatBox.append(message);
        generalChatBox.scrollTop(9999999999999999);
    }

    //Privāta ziņa 
    Hubs.client.sendPrivateMessage = function (toUserId, userName, msg) {

    }

    //Error logs
    Hubs.client.sendErrorMessage = function (user, msg) {
        var message = '<div class="GlobMsg" style="Color:Red">' + user + ': ' + msg + '</div>';
        $('#GlobalMessages').append(message);
    }

    //Visiem ziņa, ka ir pienācis jauns ļietotājs
    Hubs.client.onConnection = function (allUsers) {

        var userList = $('#ActiveUserCount');
        userList.empty();
        for (i = 0; i < allUsers.length; i++) {
            var users = '<div onmousedown="javascript:CallPrivateMessageDialog(this)" class="ActiveUser" id="' + allUsers[i].Connection + '">' + allUsers[i].Name + '</div>';
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



function CallPrivateMessageDialog(item) {
    //alert(item.id);
}




