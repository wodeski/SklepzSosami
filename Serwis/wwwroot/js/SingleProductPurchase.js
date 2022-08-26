let testVal = /[0-9]/;

let price_value = document.getElementById('price_value');
let sum_value = document.getElementById('sum_value');

let set_value;

const parent_node = document.getElementsByClassName('parent_node');
const sum_of_all = document.getElementsByClassName('sum_of_all');

let sum = 0

let quantityCl = document.getElementsByClassName('quantityCl');
const productId = document.getElementsByClassName('productId');


$(document).ready(function () {

    for (let i = 0; i < sum_of_all.length; i++) {
        sum += parseInt(sum_of_all[i].textContent);

        sum_value.textContent = sum.toFixed(2)
    }


});

function Test() {
    const tablica = [];

    for (let i = 0; i < productId.length; i++) {

        tablica.push(productId[i].innerHTML, quantityCl[i].innerHTML)
    }
}
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

    if (new_quantity == 1)
        return;

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
    let i = parseInt(btn.parentNode.getAttribute("id")); // i to jest indeks on nie ulega zmianie jest przypisany do kazdego wiersza 



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
    let sumValue = sum_value.textContent

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
                toastr.success("Powodzenie operacji ");
                window.location = data.redirectToUrl;

            } else {
                toastr.error("Niepowodzenie operacji \n" + data.message);
            }
        },
        dataType: "json"
    });
}