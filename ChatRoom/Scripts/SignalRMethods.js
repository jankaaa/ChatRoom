
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

        } else {
            $.connection.hub.start().done(function () {
                SetClientMethods();
            });
        }
    });
}

//Pasaka serverim, lai pieliek sarakstam ja user.length > 0
function OnAuth(user) {
    $.connection.hub.start().done(function () {
        var chatHub = $.connection.chatHub;
        SetClientMethods();
        chatHub.server.addPersonToGroup(user);
    });

}

//Pasaka serverim lai izņem no sarakstam
function OnSignOut() {
    //location = '';
        var chatHub = $.connection.chatHub;
        SetClientMethods();
        chatHub.server.removePersonFromGroup();
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

    //Saņemot privātu ziņu
    Hubs.client.sendPrivateMessage = function (partnerId, sender, msg) {
        //Atveram dialog logu ja vēl nav atvērts
        CallPrivateMessageDialog(partnerId);
        var chatMessages = $('#' + partnerId + '_msgContent');
        var message = '<div class=""> <b>' + sender + '</b>: ' + msg + '</div>';
        chatMessages.append(message);
        chatMessages.scrollTop(9999999999999999);
    }

    //Error logs
    Hubs.client.sendErrorMessage = function (user, msg) {
        var message = '<div class="GlobMsg" style="Color:Red">' + user + ': ' + msg + '</div>';
        $('#GlobalMessages').append(message);
        $('#GlobalMessages').scrollTop(9999999999999999);
    }

    //Visiem ziņa, ka ir pienācis jauns ļietotājs
    Hubs.client.onConnection = function (allUsers) {

        var userList = $('#ActiveUserCount');
        userList.empty();
        for (i = 0; i < allUsers.length; i++) {
            var users = '<a href="#"><div onmousedown="javascript:CallPrivateMessageDialog(this.id)" class="ActiveUser" id="'
                + allUsers[i].Connection + '">' + allUsers[i].Name + '</div></a>';
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

//Globālā ziņa visiem
function SendGlobalMessage(message) {
    if (message.val().length > 0) {
        var Hubs = $.connection.chatHub;
        Hubs.server.globalMessages(message.val());
        message.val("");
        message.focus();
    }
}

//Privāta ziņa
function SendPrivateMessageToServer(msgVal, toConnId) {
    var Hubs = $.connection.chatHub;
    Hubs.server.sendPrivateMessage(msgVal, toConnId);
}

//Atver dialog logu 
//Item - Elements kura Id ir connId;
function CallPrivateMessageDialog(item) {
    var msgDialog = $('#' + item + '_dialog');
    //Atveram jaunu modal logu
    if (msgDialog.length == 0) {
        ShowWaitScreen();

        $.ajax({
            url: 'Home/GetPrivateMessageDialog',
            data: "Id=" + item,
            type: "POST",
            async: false,
            success: function (data) {
                $('#DialogContent').append(data).hide().show('slow');
                $('.DialogTable').draggable();
                RemoveWaitSCreen();
            }
        });
    }
}






