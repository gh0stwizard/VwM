$(function () {
    var hostnamesid = 'select#SelectedHostnames';
    var language = $('#language').data().value;

    $(hostnamesid).select2({
        "language": language,
        "tags": true,
        "tokenSeparators": [',', ' '],
    }).on('select2:unselect', function (e) {
        $(e.params.data.element).remove();
    });
});
