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
function getData(url, apikey, success) {
    $.ajax({
        url: url,
        method: 'GET',
        headers: {
            'X-API-KEY': apikey
        },
        dataType: 'json',
        success: function(response) {
            success(response);
        },
        error: function(xhr, status, error) {
            console.error('Lỗi:', error);
        }
    });
}