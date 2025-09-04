// Hàm Loaded
$(function () {
    $('#reportmenu').width(250);
    $('#reportmenu div.reportmenubutton').click(function () {
        if ($('#reportmenu table').is(':visible')) {
            $('#reportmenu table').toggle(false);
            $('#reportmenu').width(30);
            $('#reportmenu div.reportmenubutton i').attr('class', 'bi bi-arrow-bar-right');
        } else {
            $('#reportmenu table').toggle(true);
            $('#reportmenu').width(250);
            $('#reportmenu div.reportmenubutton i').attr('class', 'bi bi-arrow-bar-left');
        }
    })
})
// Hàm Loaded