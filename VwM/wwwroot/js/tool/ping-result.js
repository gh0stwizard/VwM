$(function () {
    "use string";

    var language = $("#language").data().value,
        queueId = $("input#Id").val();
    var startDate;

    var table = $("table").DataTable({
        "language": {
            "url": '/json/datatables/' + language + '.json'
        },
        "paging": false,
        "columnDefs": [
            { "targets": 1, "type": "version" },
            {
                "targets": 2,
                "render": function (data, type, row, meta) {
                    if (type === "display") {
                        var int = parseInt(data);
                        if (!isNaN(int) && isFinite(int))
                            return data + " " + Lcz.Measure.ms;
                        else
                            return '<span class="text-danger">' + data + '</span>';
                    }

                    return data;
                }
            }
        ],
        "dom":
            "<'row'<'col-sm-12 col-md-3 pt-1'<'progress'<'progress-bar'>>><'col-sm-12 col-md-9'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7'p>>",
    });

    table.on("init.dt", function (e, settings, json) {
        initProgressbar();
        connect(queueId, language);
    });


    function connect(queueId, language) {
        var connection = new signalR.HubConnectionBuilder().withUrl("/hubs/ping").build(),
            connectionId = null;

        connection.on("Registered", function (cid) {
            //console.debug("Registered", cid);
            connectionId = cid;
            connection.invoke("ClientReady", connectionId, queueId, language);
            startDate = Date.now();
        });

        connection.on("Exception", function (message) {
            console.error("Exception", message);
        });

        connection.on("Result", function (data) {
            //console.debug("Result", data);
            addResult(data);
        });

        connection.start().catch(function (err) {
            return console.error("Connection Error", err.toString());
        });
    }


    function initProgressbar() {
        $('.progress').css("height", "2em");
        $('.progress-bar').attr({
            "role": "progressbar",
            "aria-valuemin": 0,
            "aria-valuemax": 100,
            "aria-valuenow": 0
        });
    }


    var count = 0,
        total = parseInt($("input#Total").val());

    function addResult(data) {
        table.row.add([data.name, data.hostname, data.result]).draw();
        var value = ++count == total
            ? 100
            : Math.round((count * 100) / total);
        var pcnt = value + "%";
        $('.progress-bar').css("width", pcnt).attr("aria-valuenow", value).text(pcnt);

        if (value == 100) {
            endDate = Date.now();

            var delta = endDate - startDate;
            var measure = Lcz.Measure.ms;
            if (delta >= 1000) {
                delta = Number(delta / 1000).toFixed(2);
                measure = Lcz.Measure.sec;
            }
            $('.progress-bar').text(Lcz.Progressbar.CompletedIn + delta + " " + measure);
        }
    }
});
