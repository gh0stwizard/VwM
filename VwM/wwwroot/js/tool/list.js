(function ($) {
    var Shuffle = window.Shuffle;
    var element = document.querySelector('.my-shuffle-container');
    var sizer = element.querySelector('.my-sizer-element');

    var shuffleInstance = new Shuffle(element, {
        itemSelector: '.item',
        sizer: sizer // could also be a selector: '.my-sizer-element'
    });

    $('button').click(function () {
        var group = $(this).data().group;
        if (group == 'all')
            shuffleInstance.filter();
        else
            shuffleInstance.filter(group);
        $(this).blur();
    });
}(jQuery));
