function postData(url, apikey, bodydata, success) {
    $.ajax({
        url: url,
        type: 'POST',
        headers: {
            'X-API-KEY': apikey
        },
        contentType: 'application/json',
        data: JSON.stringify(bodydata),
        beforeSend: function () {
            $('#busyindicator').show();
        },
        success: function (response) {
            success(response);
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
        },
        complete: function () {
            $('#busyindicator').hide();
        }
    });
}