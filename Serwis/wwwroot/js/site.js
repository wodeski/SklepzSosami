const confirmMsg = document.getElementById("confirm")

function getCookie(name) {
    var dc = document.cookie;
    var prefix = name + "=";
    var begin = dc.indexOf("; " + prefix);
    if (begin == -1) {
        begin = dc.indexOf(prefix);
        if (begin != 0) return null;
    }
    else {
        begin += 2;
        var end = document.cookie.indexOf(";", begin);
        if (end == -1) {
            end = dc.length;
        }
    }
    return decodeURI(dc.substring(begin + prefix.length, end));

}
$(document).ready(function () {


    if (confirmMsg !== null) {
        let cookie_value = document.cookie.split(';');
        let cookie_value_split = cookie_value[0].split('=')
        let cart_quantity_cookie = cookie_value_split[1];
        console.log(document.cookie)
        if (getCookie("cart") === null) {
            document.getElementsByClassName('cart-quantity')[0].textContent = 0;
            return;
        }
        if (cart_quantity_cookie === 0) {
            document.getElementsByClassName('cart-quantity')[0].textContent = 0;
            document.cookie = 'cart =; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
            return;
        }
        document.getElementsByClassName('cart-quantity')[0].textContent = cart_quantity_cookie
        sessionStorage.removeItem('key');
    } else {

        let cart_quantity = sessionStorage.getItem('key');
        if (cart_quantity === null) {
            document.getElementsByClassName('cart-quantity')[0].textContent = 0;

            return;
        }
        document.getElementsByClassName('cart-quantity')[0].textContent = cart_quantity;

    }

});