const description = document.getElementsByClassName("description");

console.log(description);
$(document).ready(function () {

    for (let i = 0; i < description.length; i++) {
        if (description[i].textContent.length > 17) {
            let short_description = description[i].textContent.substring(0, 16);
            console.log(short_description);
            short_description += "(...)";
            description[i].innerHTML = short_description;
        }
    }
});


function removeRow(id, btn) {
    swal({
        title: "Usuwanie?",
        text: "Jestes pewien ze chcesz usunac te pozycje?",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'POST',
                url: "@Url.Action("Delete", "Admin")",
                data:
                {
                    id: id,
                },
                success: function (data) {
                    if (data.success) { // musza byc male litery i tu i tu
                        //usuwanie w html
                        const table = document.getElementById("orders");

                        const row = btn.parentNode.parentNode;

                        //pierwszy parentNode to <td> drugi parentNode dotyczy <tr> - cały wiersz
                        row.parentNode.removeChild(row);
                        //kolejny parentNode tyczy sie <tbody> po wywolaniu nastepuje usuwanie wiersza

                        let tbodyRowCount = table.tBodies[0].rows.length;

                        if (tbodyRowCount === 0) {

                            let row = table.insertRow(1);

                            let cell1 = row.insertCell(0);


                            cell1.innerHTML = "Brak zaplanowanych zadan";
                        }

                        toastr.success("Operacja się powiodła");
                    } else {
                        toastr.error("Niepowodzenie operacji \n" + data.message);
                    }
                },
                error: function (data) {
                    alert(data.message);
                },
                dataType: 'json'

            });
        }

    });
}