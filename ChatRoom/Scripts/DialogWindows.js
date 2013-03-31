
//Sūta privātu ziņu
function SendPrivateMessage(tbVal, connId) {
    SendPrivateMessageToServer(tbVal.val(), connId);
    tbVal.val("");
    tbVal.focus();
}