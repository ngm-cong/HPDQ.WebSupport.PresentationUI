// Hàm Loaded
$(function () {
    $('.edit-btn').click(function () {
        var tag = parseInt($(this).attr('tag'));
        var filteredArray = $.grep(myModel, function(element) {
            return element.code === tag;
        });
        //filteredArray[0]
        $('#txtcode').val(filteredArray[0].code);
        $('#txtdescription').val(filteredArray[0].description);
        var dialogModalEdit = new bootstrap.Modal(document.getElementById('dialogModalEdit'), { backdrop: true, keyboard: true });
        dialogModalEdit.show();
    })
    $('#lbaddnew').click(function () {
        $('#txtcode').val('0');
        $('#txtdescription').val('');
        var dialogModalEdit = new bootstrap.Modal(document.getElementById('dialogModalEdit'), { backdrop: true, keyboard: true });
        dialogModalEdit.show();
    })
    $('#dialogModalEdit .btn.btn-primary').click(function () {
        var bodydata = {
            code: $('#txtcode').val(),
            description: $('#txtdescription').val()
        };
        postData(`${domainAPIUrl}/CodeDetails/AddOrUpdate/${$('#cbomaster').val()}`, apiKey, bodydata, function(response) {
            if (response.data) {
                $('#dialogModalEdit').modal('hide');
                location.reload();
            }
        });
    });
})
// Hàm Loaded