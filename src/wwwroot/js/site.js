$(function () {
    $('#toggleSidebar').on('click', function () {
        $('#sidebar').toggleClass('collapsed');
    });

    $('#toggleSidebar').on('click', function () {
        $('#sidebar').toggleClass('sidebar-visible');

        var icon = $(this).find('i');

        if ($('#sidebar').hasClass('sidebar-visible')) {
            icon.removeClass('bi-arrow-right').addClass('bi-list');
        } else {
            icon.removeClass('bi-list').addClass('bi bi-arrow-bar-right');
        }
    });

    $('#logout_btn').on('click', function () {
        $('#logoutModal').modal('show');
    });
});