


var input = document.getElementById("fileUpload");

var infoArea = document.getElementById("infoArea");

input.addEventListener('change', showFileName);

function showFileName() {
    var file_input = event.srcElement;
    var fileName = file_input.files[0].name;
    infoArea.textContent = fileName;
}