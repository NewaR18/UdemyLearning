$(document).ready(function () {
    var pidValue = $("#txtPid").text();
    var countValue = $("#txtCount").text();
    if (pidValue != "") {
        $.ajax({
            url: '/Admin/Account/GetPaymentDetails',
            method: "get",
            dataType: "json",
            data: { pid: pidValue},
            success: function (data) {
                if (data.params != null) {
                    var form = document.createElement("form");
                    form.setAttribute("method", "POST");
                    form.setAttribute("action", data.path);
                    for (var key in data.params) {
                        var hiddenField = document.createElement("input");
                        hiddenField.setAttribute("type", "hidden");
                        hiddenField.setAttribute("name", key);
                        hiddenField.setAttribute("value", data.params[key]);
                        form.appendChild(hiddenField);
                    }
                    document.body.appendChild(form);
                    form.submit();
                } else {
                    window.location = "PaymentFailure"
                }
            },
            error: function (req, status, error) {
                window.location = "PaymentFailure"
            }
        });
    } else {
        window.location = "PaymentFailure"
    }
    
})