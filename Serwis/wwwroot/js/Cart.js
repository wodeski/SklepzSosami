let testVal = /[0-9]/;

let price_value = document.getElementById('price_value');
let sum_value = document.getElementById('sum_value');

let set_value;

const parent_node = document.getElementsByClassName('parent_node');
const sum_of_all = document.getElementsByClassName('sum_of_all');

let sum = 0

let quantityCl = document.getElementsByClassName('quantityCl');
const productId = document.getElementsByClassName('productId');

const orderId = document.getElementById('orderId').textContent;

console.log(orderId)

$(document).ready(function () {

    for (let i = 0; i < sum_of_all.length; i++) {
        sum += parseInt(sum_of_all[i].textContent);

        sum_value.textContent = sum.toFixed(2)
    }


});

let new_quantity;
let new_price_value;

function BtnMinus(index, btn) {
    let i = parseInt(btn.parentNode.getAttribute("id")); // i to jest indeks on nie ulega zmianie



    if ((testVal.test(quantityCl[i].innerHTML)) === false) {

        quantityCl[i].innerHTML = 1;
        toastr.error("Nie zmieniaj danych!");

        return;
    }

    if (i !== parseInt(index)) {
        toastr.error("Wystąpił problem przy walidacji danych");
        return;
    }

    new_quantity = parseInt(quantityCl[i].innerHTML)
    new_price_value = parseFloat(btn.parentNode.nextElementSibling.innerHTML) / parseInt(quantityCl[i].innerHTML); //wartosc produktu

    if (new_quantity <= 1) {
        return;
    }
    new_quantity--; //wartosc ilosci
    set_value = new_quantity * new_price_value; // wartosc ogólna
    sum_value.innerHTML = set_value.toFixed(2).replace(".", ",");

    quantityCl[i].innerHTML = new_quantity

    btn.parentNode.nextElementSibling.innerHTML = set_value.toFixed(2).replace(".", ",");

    sum = 0; // wyzerowanie sumy
    for (let i = 0; i < sum_of_all.length; i++) {
        sum += parseInt(sum_of_all[i].textContent);

        sum_value.textContent = sum.toFixed(2)
    }
}

//zwiekszenie ilosci
function BtnPlus(index, btn) {



    let i = parseInt(btn.parentNode.getAttribute("id")); // i jest przypisany do kazdego wiersza



    if ((testVal.test(quantityCl[i].innerHTML)) === false) {

        quantityCl[i].innerHTML = 1;
        toastr.error("Nie zmieniaj danych!");

        return;
    }

    if (i !== parseInt(index)) {
        toastr.error("Wystąpił problem przy walidacji danych");
        return;
    }

    new_price_value = parseFloat(btn.parentNode.nextElementSibling.innerHTML) / parseInt(quantityCl[i].innerHTML);
    //wartosc produktu podzielenie przez ilosc aby operowac na wartosci bazowej

    new_quantity = parseInt(quantityCl[i].innerHTML)

    new_quantity++;
    set_value = new_quantity * new_price_value; // wartosc ogólna
    sum_value.innerHTML = set_value.toFixed(2).replace(".", ",");

    quantityCl[i].innerHTML = new_quantity
    btn.parentNode.nextElementSibling.innerHTML = set_value.toFixed(2).replace(".", ",");

    sum = 0; // wyzerowanie wartosci ogólnej przed ponownym obliczaniem
    for (let i = 0; i < sum_of_all.length; i++) {
        sum += parseInt(sum_of_all[i].textContent);

        sum_value.textContent = sum.toFixed(2)
    }
}

function PayForOrder(orderId) {


    var quantityOfPositions = []
    for (let i = 0; i < productId.length; i++) {


        if ((testVal.test(quantityCl[i].innerHTML)) === false) {

            quantityCl[i].innerHTML = 1;
            toastr.error("Nie zmieniaj danych!");

            return;
        }
        quantityOfPositions.push(productId[i].innerHTML, quantityCl[i].innerHTML)
    }

    $.ajax({
        type: "post",
        url: "@Url.Action("Cart", "Shop")",
        data: {
            "orderId": orderId,
            "quantityOfPositions": quantityOfPositions
        },
        success: function (data) {
            if (data.success) {
                toastr.success("Już blisko ");
                window.location = data.redirectToUrl;

            } else {

                toastr.error("Niepowodzenie operacji \n" + data.message);
            }
        },
        dataType: "json"
    });

}

function DeletePosition(productId, orderId, btn) {
    swal({
        title: "Usuwanie?",
        text: "Jestes pewien ze chcesz usunac te pozycje?",
        icon: "warning",
        buttons: ["Anuluj", "Ok"],
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "post",
                url: "@Url.Action("DeleteUserOrderPositionFromCart", "Shop")",
                data: {
                    productId: productId,
                    orderId: orderId
                },
                success: function (data) {
                    if (data.success) {
                        const row = btn.parentNode.parentNode;
                        row.parentNode.removeChild(row);
                        const tbody = parent_node[0].parentNode;

                        if (sum_of_all.length < 1) {
                            tbody.removeChild(parent_node[0]); //usuwa pierwszy dlatego trzeba znowu podawc 0 element
                            tbody.removeChild(parent_node[0]);
                        } else {
                            let sum = 0;

                            for (let i = 0; i < sum_of_all.length; i++) {
                                sum += parseInt(sum_of_all[i].textContent);
                            }
                            sum_value.textContent = sum.toFixed(2);
                        }
                        console.log("test3")


                        let quantity = document.getElementsByClassName('cart-quantity')[0].textContent;
                        quantity--;

                        if (null !== confirmMsg) { // sprawdzenie czy uzytkownik jest zalogowany czy jest anonimowo
                            document.cookie = `cart = ${quantity}; expires = ${data}; path=/`;
                            let cookie_value = document.cookie.split(';');
                            let cookie_value_split = cookie_value[0].split('=')
                            let cart_quantity_cookie = cookie_value_split[1];

                            document.getElementsByClassName('cart-quantity')[0].textContent = cart_quantity_cookie
                        } else {

                            sessionStorage.setItem('key', quantity.toString());
                            let cart_quantity = sessionStorage.getItem('key');

                            document.getElementsByClassName('cart-quantity')[0].textContent = cart_quantity;
                        }

                    } else {
                        toastr.error("Niepowodzenie operacji \n" + data.message);
                    }
                },
                dataType: "json"
            });
        }
    });

}