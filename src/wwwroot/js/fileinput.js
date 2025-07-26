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
        }
        $('#divattachment').html(formDataFileLength());
        $('#imgattachment').show();
    });
}