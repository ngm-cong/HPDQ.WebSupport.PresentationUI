// Hàm Loaded
$(function () {
    const toggleItems = document.querySelectorAll('.can-toggle');
    toggleItems.forEach(item => {
        item.addEventListener('click', function (event) {
            if (event.target === this) {
                var opened = $('.can-toggle.open');
                console.log(opened.removeClass('open'));

                event.stopPropagation();
                this.classList.toggle('open');
                $('#txtgroup').val(this.textContent.trim());
            }
        });
    });
})
// Hàm Loaded