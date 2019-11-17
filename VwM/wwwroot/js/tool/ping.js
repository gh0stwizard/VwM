$(function () {
    "use strict";

    $('input[name="Mode"]').change(function () {
        $("button#start").removeAttr("disabled");
        $("button#start").removeClass("disabled");

        if ($(this).val() == "Page") {
            setTimeout(function () {
                $("input#Mode").blur();
                $("#Hostnames").attr("disabled", false);
                $("#Hostnames").focus();
            }, 100);
        }
        else {
            $("#Hostnames").attr("disabled", true);
            setTimeout(function () {
                $("input#Mode").blur();
                $("button#start").focus();
            }, 100);
        }
    });
});
