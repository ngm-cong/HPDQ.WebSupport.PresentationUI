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
            $('#divattachment').html(formDataFileLength());
        }
    });
})