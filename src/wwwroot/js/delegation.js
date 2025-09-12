// Hàm Loaded
$(function () {
    const toggleItems = document.querySelectorAll('.can-toggle');
    toggleItems.forEach(item => {
        item.addEventListener('click', function (event) {
            if (event.target === this) {
                var opened = $('.can-toggle.open');

                event.stopPropagation();
                this.classList.toggle('open');
                

                var parentElement = this; // 'this' refers to the element you clicked on
                var directText = $(parentElement).contents().filter(function() {
                    // Check for nodeType 3, which represents a text node
                    return this.nodeType === 3;
                }).text().trim();
                directText = directText.trim();

                $('#txtgroup').val(directText);

                if (directText) {
                    var bodydata = { Job_Group: directText };
                    postData(`${domainAPIUrl}/EmployeeFunctions/Load`, apiKey, bodydata, function(response) {
                        if (response.data) {
                            $('#divtickettype input[type=checkbox]').each(function() {
                                var exists = response.data.some(function(item) {
                                    return item.ticketType === parseInt($(this).val());
                                }.bind(this));
                                if (exists) {
                                    $(this).prop('checked', true);
                                } else {
                                    $(this).prop('checked', false);
                                }
                            });
                        }
                    });
                }
            }
        });
    });
    $('button.btn.btn-primary').on('click', function () {
        var job_group = $('#txtgroup').val().trim();
        if (!job_group) {
            alert('Vui lòng chọn nhóm nhân sự đang xử lý');
            return;
        }
        var ticket_types = [];
        $('#divtickettype input[type=checkbox]:checked').each(function() {
            ticket_types.push({ TicketType: parseInt($(this).val()) });
        });
        var bodydata = { Job_Group: job_group, EmployeeFunctions: ticket_types };
        postData(`${domainAPIUrl}/EmployeeFunctions/DropAndCreate`, apiKey, bodydata, function(response) {
            if (response.errorCode === 0) {
                alert('Lưu phân quyền xử lý thành công');
            } else {
                alert(`Lưu phân quyền xử lý thất bại: ${response.message}`);
            }
        });
    });
})
// Hàm Loaded