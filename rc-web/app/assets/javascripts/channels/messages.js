$(document).on('turbolinks:load', function() {
    initAppCable();
});
function initAppCable() {
    App.messages = App.cable.subscriptions.create({
        channel: 'MessagesChannel',
        authid: $('#authid').val()
    }, {
        received: function(data) {
            if(data.type === "result") {
                $('#result').append(data.message);
                $("#result").animate({ scrollTop: 9007199254740991 }, "slow");
            }
        },

        sendMessage: function(data) {
            this.send(data);
        }
    });
}