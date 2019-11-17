$(function () {
    "use strict";

    // enable tooltips
    $('[data-toggle="tooltip"]').tooltip();

    var language = $("#language").data().value,
        queueId = $("input#Id").val();

    if (queueId)
        connect(queueId, language);

    function connect(queueId, language) {
        var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/whois").build(),
            connectionId = null;

        connection.on("Registered", function (cid) {
            //console.debug("Registered", cid);
            connectionId = cid;
            block();
            connection.invoke("ClientReady", connectionId, queueId, language);
        });

        connection.on("Exception", function (message) {
            console.error("Exception", message);
        });

        connection.on("Result", function (host, result) {
            //console.debug("Result", host, result);
            $("<pre>").appendTo("#result-value").text(result);
            $("#result").show();
            unblock();
        });

        connection.start().catch(function (err) {
            return console.error("Connection Error", err.toString());
        });
    }


    function block() {
        $("html").focus();
        $("#overlay").show();
    }


    function unblock() {
        $("#overlay").hide();
    }
});
