$(document).ready(function () {
    $('#back').css("display", "none");

});
let testVal = /(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/;

let email;

function ConfirmInvoiceOrder(email) {
    email = document.getElementById("email").value;

    if ((testVal.test(email)) === true) {


    } else {
        toastr.error("Email jest niepoprawny");
        return;
    }

    $.ajax({
        type: "post",
        url: "@Url.Action("OrderSummary","Shop")",
        data: {
            email: email
        },
        success: function (data) {
            if (data.success) {
                toastr.success("Gratuluje zakupu!")
                $('#back').css("display", "");
                $('#accept_back').css("display", "none");
                sessionStorage.removeItem('key');

                document.cookie = 'cart =; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';


                document.getElementsByClassName('cart-quantity')[0].textContent = 0;

            } else {
                toastr.error("Niestety coś poszło nie tak :(")
            }
        },
        dataType: "json"
    });
}