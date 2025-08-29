var formData = new FormData();
function formDataFileLength() {
    var fileCount = 0;
    for (var [key, value] of formData.entries()) {
        if (key === "files") {
            fileCount++;
        }
    }
    return fileCount;
}
function initFileInput() {
    $('#lblattach').on('click', function () {
        formData = new FormData();
        $('#fileinput').trigger('click');
    });
    $('#fileinput').on('change', function () {
        if (this.files.length == 0) {
            return;
        }
        for (let i = 0; i < this.files.length; i++) {
            formData.append("files", this.files[i]); // "files" matches API parameter
            // $('#divattachmentdetail').append(`<div>${this.files[i].name}<img src='img/delete.png' onclick='deletefile(this)' /></div>`);
        }
        $('#divattachment').html(formDataFileLength());
        $('#imgattachment').show();
    });
}

function uploadFileInputs(url, apikey) {
    var uploadCompleted = false;
    var files;
    var message

    if (formDataFileLength() > 0) {
        $.ajax({
            url: url, // replace with your actual API endpoint
            type: 'POST',
            headers: {
                'X-API-KEY': apikey
            },
            data: formData,
            contentType: false,
            processData: false,
            async: false,
            beforeSend: function () {
                $('#busyindicator').show();
            },
            success: function (response) {
                if (response.errorCode != 0) {
                    uploadCompleted = false;
                    message = response.message;
                }
                else {
                    uploadCompleted = true;
                    files = response.data;
                }
            },
            error: function (err) {
                console.error("Upload failed", err);
            },
            complete: function () {
                $('#busyindicator').hide();
            }
        });
    } else {
        uploadCompleted = true;
    }

    return {
        uploadCompleted: uploadCompleted,
        files: files,
        message: message
    };
}

function deletefile(element) {
    var indexToDelete = $(element.parentElement).index();
    var newFormData = new FormData();
    let fileCount = 0;
    $('#divattachmentdetail').html('');
    for (const [key, value] of formData.entries()) {
        if (value instanceof File && key === 'files') { // Change 'files' to your file input's name
            if (fileCount !== indexToDelete) {
                newFormData.append(key, value);
                $('#divattachmentdetail').append(`<div>${value.name}<img src='img/delete.png' onclick='deletefile(this)' /></div>`);
            }
        } else {
            newFormData.append(key, value);
        }
        fileCount++;
    }
    formData = new FormData();
    formData = newFormData;
    $('#divattachment').html(formDataFileLength());
}