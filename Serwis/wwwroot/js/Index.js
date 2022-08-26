const element = document.getElementById("products");
$(document).ready(function () {
    $('#filterForm').submit(function () {

        $.ajax({
            type: "POST",
            url: "@Url.Action("Products", "Shop")",
            data: $(this).serialize(), // przekazanie z formualrza zserializowane dane
            success: function (data) {

                $('section.products').html(data);

            },
            error: function (data) {
                alert(data.message)
            },
            dataType: "html"
        });
        return false; // ze wzgeldu na to ze submit ma ustawione akcje
    })
});


function AddPositionToCart(productId, userId) {
    swal({
        title: "Dododawanie produktu do koszyka",
        text: "Jestes pewien ze chcesz dodac te pozycję do koszyka?",
        icon: "info",
        buttons: ["Anuluj", "Ok"],
    }).then((willAdd) => {
        if (willAdd) {
            $.ajax({
                type: 'POST',
                url: "@Url.Action("AddPositionToCart", "Shop")",
                data:
                {
                    productId: productId,
                    userId: userId
                },
                success: function (data) {
                    if (data.success) {

                        let quantity = document.getElementsByClassName('cart-quantity')[0].textContent
                        quantity++;

                        if (confirmMsg !== null) {

                            var expires = "";

                            var date = new Date();
                            date.setTime(date.getTime() + (1 * 24 * 60 * 60 * 1000));

                            var data = date.toUTCString();

                            document.cookie = `cart = ${quantity}; expires = ${data}; path=/`;
                            let cookie_value = document.cookie.split(';');
                            let cookie_value_split = cookie_value[0].split('=')
                            let cart_quantity_cookie = cookie_value_split[1];

                            document.getElementsByClassName('cart-quantity')[0].textContent = cart_quantity_cookie
                            sessionStorage.removeItem('key');

                        } else {
                            sessionStorage.setItem('key', quantity.toString());
                            let cart_quantity = sessionStorage.getItem('key');
                            document.getElementsByClassName('cart-quantity')[0].textContent = cart_quantity;
                            document.cookie = 'cart =; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
                        }
                        toastr.success("Dodano do koszyka");
                    } else {
                        toastr.error("Niepowodzenie operacji \n" + data.message);
                    }

                },

                error: function (data) {
                    toastr.error("Niepowodzenie operacji \n" + data.message);
                },
                dataType: 'json'

            });
        }
    });
}