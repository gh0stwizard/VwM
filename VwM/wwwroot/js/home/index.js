$(function () {
    "use strict";

    var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/home").build(),
        connectionId = null;

    connection.on("Registered", function (cid) {
        //console.debug("Registered", cid);
        connectionId = cid;
        getDbStatus();
    });

    connection.on("DbStatus", function (status) {
        //console.debug("DbStatus", status);

        var $badge = $("span.badge#db");
        $badge.removeClass("badge-secondary badge-success badge-danger");

        if (~~status) {
            $badge.addClass("badge-success");
            $badge.text(Lcz.Database.Online);
        }
        else {
            $badge.addClass("badge-danger");
            $badge.text(Lcz.Database.Offline);
        }

        setTimeout(getDbStatus, 5000);
    });

    connection.start().catch(function (err) {
        return console.error("Connection Error", err.toString());
    });


    function getDbStatus() {
        connection.invoke("GetDbStatus", connectionId);
    }
});
