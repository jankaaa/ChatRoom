function ShowWaitScreen() {
    var gab = document.createElement('div');
    gab.setAttribute('id', 'OVER');
    gab.innerHTML = '<div class="overlay"><img src="Content/Image/ajax-loader.gif" /></div>';
    document.body.appendChild(gab);
}

function ShowErrorWindows(message) {
    alert(message);
}



function RemoveWaitSCreen() {
    $('#OVER').remove();
}


