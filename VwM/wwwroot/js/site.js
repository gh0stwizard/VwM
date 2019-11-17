$(function () {
    "use strict";

    $(document).on("click", ".vwm-close", function (e) {
        $("." + $(this).data().dismiss).remove();
    });
});
