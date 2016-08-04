$(document).on('turbolinks:load', function() {
    $('#command').focus();
});
$(window).load(function () {
    setTimeout(function(){
        App.messages.sendMessage({
            message: "",
            action: "stream_command",
            authid: $('#authid').val(),
            type: "command"
        });
        console.log('hihi');
    }, 1000);
});
function onEnter(event) {
    if (event.keyCode == 13) {
        var command = $('#command').val();
        if (command === "cls") {
            $("#result").val(" ");
        }
        else {
            App.messages.sendMessage({
                message: command,
                action: "stream_command",
                authid: $('#authid').val(),
                type: "command"
            });
        }
        $('#command').val("");
    }
}