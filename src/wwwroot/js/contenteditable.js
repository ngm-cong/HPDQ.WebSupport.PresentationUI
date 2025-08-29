$(function () {
    $('#txtdescription').on('paste', function(event) {
        var items = (event.originalEvent || event).clipboardData.items;

        if (items.length > 0 && items[0].type.indexOf('text') >= 0) return;

        // Prevent the default paste behavior (which might insert a large image)
        event.preventDefault();

        // Check for an image in the clipboard data
        for (var i = 0; i < items.length; i++) {
            var file = items[i].getAsFile();

            // Create a new DataTransfer object
            var dataTransfer = new DataTransfer();

            // Add the file to the DataTransfer object
            dataTransfer.items.add(file);

            // Get your file input element (assuming it has ID 'your-file-input')
            var fileInput = document.getElementById('fileinput');

            // Set the files property of the file input
            fileInput.files = dataTransfer.files;

            formData.append("files", file);
            if ($('#divattachmentdetail').length) {
                $('#divattachmentdetail').append(`<div>${file.name}<img src='img/delete.png' onclick='deletefile(this)' /></div>`);
            }
            $('#divattachment').html(formDataFileLength());
        }
    });
    //document.getElementById('txtdescription').addEventListener('keydown', (e) => {
    //    if (e.key === 'Enter') {
    //        e.preventDefault();

    //        // Lấy đối tượng Selection hiện tại
    //        const selection = window.getSelection();
    //        if (!selection.rangeCount) return;

    //        // Tạo một Node mới để chèn vào
    //        const brNode = document.createElement('br');

    //        // Lấy Range (vùng chọn) hiện tại
    //        const range = selection.getRangeAt(0);

    //        // Xóa nội dung được chọn (nếu có)
    //        range.deleteContents();

    //        // Chèn <br> tại vị trí con trỏ
    //        range.insertNode(brNode);

    //        // Di chuyển con trỏ xuống sau thẻ <br> vừa chèn
    //        range.setStartAfter(brNode);
    //        range.setEndAfter(brNode);
    //        selection.removeAllRanges();
    //        selection.addRange(range);
    //    }
    //});
})