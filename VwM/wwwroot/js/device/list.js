$(function () {
    "use strict";

    var language = $("#language").data().value;
    var tableid = "table#devices";
    var resetProcessing = false;

    $.fn.dataTable.ext.errMode = function (settings, techNote, message) {
        switch (techNote) {
            case 7:
                var api = new $.fn.dataTable.Api(settings);
                setTimeout(function () {
                    $(".dataTables_processing").text(Lcz.DataTable.AjaxRetry).show();
                    resetProcessing = true;
                    api.ajax.reload();
                }, 500);
                break;

            default:
                alert(message);
        }
    };

    var dt = $(tableid).DataTable({
        "rowId": "id",
        "language": {
            "url": '/json/datatables/' + language + '.json'
        },
        "stateSave": false,
        "pagingType": "full_numbers",
        "processing": true,
        "serverSide": true,
        "select": {
            "info": false,
        },
        "ajax": {
            "url": "/api/device",
            "type": "POST",
            "contentType": "application/json; charset=utf-8",
            "dataType": "json",
            "data": function (d) { return JSON.stringify(d); }
        },
        "order": [[1, "asc"]],
        "columns": [
            { "data": "id", visible: false, searchable: false },
            { "data": "name" },
            { "data": "description" },
            { "data": "hostnames" }
        ],
        "buttons": [
            {
                "text": Lcz.Button.Create,
                "action": function (e, dt, node, config) {
                    window.location = "/Device/New";
                }
            },
            {
                "text": Lcz.Button.Edit,
                "action": function (e, dt, node, config) {
                    var selected = dt.row({ selected: true }).data();
                    window.location = "/Device/Edit/" + selected.id;
                },
                "enabled": false
            },
            {
                "text": Lcz.Button.Delete,
                "action": function (e, dt, node, config) {
                    dt.buttons([1,2]).enable(false);
                    var selected = dt.rows({ selected: true }).data();
                    var ids = $.map(selected, function (o, i) { return o.id; });
                    $.ajax({
                        "url": "/api/device/" + ids.join(","),
                        "type": "DELETE",
                        "success": function (data, textStatus, jqXHR) {
                            dt.row(id).remove().draw();
                        }
                    });
                },
                "enabled": false
            }
        ],
        "dom":
            "<'row'<'col-sm-12 col-md-6'B><'col-sm-12 col-md-6'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-12 col-md-5 align-self-sm-end'l><'col-sm-12 col-md-7'p>>" +
            "<'row float-right'<'col-sm col-md'i>>"
    });

    dt.on("draw.dt", function (e, settings) {
        if (resetProcessing) {
            $(".dataTables_processing").text(settings.oLanguage.processing);
            resetProcessing = false;
        }
    }).on("select deselect", function () {
        var selectedRows = dt.rows({ selected: true }).count();
        dt.buttons([1]).enable(selectedRows == 1);
        dt.buttons([2]).enable(selectedRows >= 1);
    });

    $(document).click(function (e) {
        var $table = $(tableid);
        if ($table.find(e.target).length == 0)
            $table.DataTable().rows().deselect();
    })
});
