// Hàm Loaded
$(function () {
    const toggleItems = document.querySelectorAll('.can-toggle');
    toggleItems.forEach(item => {
        item.addEventListener('click', function (event) {
            event.stopPropagation();
            this.classList.toggle('open');
        });
    });
})
// Hàm Loaded