(function() {
    /*
     * Taken from https://stackoverflow.com/questions/7717109
     * Author: Gras Double
     */

    function padParts(version) {
        return version
            .split('.')
            .map(function (part) {
                var int = parseInt(part);

                if (!isNaN(int) && isFinite(int))
                    return '00000000'.substr(0, 8 - part.length) + part;
                else
                    return part;
            })
            .join('.');
    }

    function cmpVersions(a, b) {
        a = padParts(a);
        b = padParts(b);

        return a.localeCompare(b);
    }

    jQuery.extend(jQuery.fn.dataTableExt.oSort, {
        "version-asc": function (a, b) {
            return cmpVersions(a, b);
        },

        "version-desc": function (a, b) {
            return cmpVersions(a, b) * -1;
        },

        "version-ci-asc": function (a, b) {
            a = a.toString().toLowerCase();
            b = b.toString().toLowerCase();

            return cmpVersions(a, b);
        },

        "version-ci-desc": function (a, b) {
            a = a.toString().toLowerCase();
            b = b.toString().toLowerCase();

            return cmpVersions(a, b) * -1;
        }
    });
}());
