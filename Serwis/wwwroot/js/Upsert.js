let price = document.getElementById('price');
const input = document.getElementById('imageFile');
const preview = document.getElementById('preview');

//input.style.opacity = 0;


input.addEventListener('change', updateImageDisplay); // oczekiwanie na zmiane i wtedy wywołanie
function updateImageDisplay() {
    while (preview.firstChild) {
        preview.removeChild(preview.firstChild); //oczyszczenie z poprzedniego widoku
    }

    const curFiles = input.files;
    if (curFiles.length === 0) { // w momencie gdy niczego nie zaznaczono
        const para = document.createElement('p');
        para.textContent = 'Nie wybrałeś żadnego elementu';
        preview.appendChild(para); //wyswietlenie błedu
    } else {
        for (const file of curFiles) {
            const para = document.createElement('p');
            if (validFileType(file)) {//jesli format jest odpowiedni
                const image = document.createElement('img'); //stworzenie znacznika img
                image.src = URL.createObjectURL(file); //pobranie zrodla zdjecia
                preview.appendChild(image);
            } else {
                para.textContent = `Plik ${file.name}: nie jest odpowiedniego typu!`;
                para.style.color = 'rgb(255, 0, 0)';
                preview.appendChild(para);
            }

            list.appendChild(listItem);
        }
    }
}
const fileTypes = [
    "image/apng",
    "image/bmp",
    "image/gif",
    "image/jpeg",
    "image/pjpeg",
    "image/png",
    "image/svg+xml",
    "image/tiff",
    "image/webp",
    "image/x-icon"
];

function validFileType(file) {
    return fileTypes.includes(file.type);
}