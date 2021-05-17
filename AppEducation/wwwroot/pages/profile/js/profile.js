var newinput = document.getElementsByClassName("newInput");
var oldinput = document.getElementsByClassName("oldInput");
var saveButton = document.getElementById("save");
var changeButton = document.getElementById("changes");
var cancelButton = document.getElementById("cancel");
var seePW = document.getElementById("seePW");
function changeFunction(){
    for (var i = 0; i < newinput.length; i++) {
        newinput[i].disabled = false;
    }
    changeButton.style.display = 'none';
    saveButton.style.display = 'block';
    cancelButton.style.display = "block";
}
cancelButton.onclick = () => {
    for (var i = 0; i < newinput.length; i++) {
        newinput[i].disabled = false;
    }
    changeButton.style.display = "block";
    saveButton.style.display = 'none';
    cancelButton.style.display = "none";
}
seePW.onclick = () => {
    var pw = document.getElementById("Password");
    if (pw.getAttribute("type") == "password") {
        pw.setAttribute("type", "text");
    }
    else {
        pw.setAttribute("type", "password");
    }
}

var modal = document.getElementById("myModal");
var img = document.getElementById("myImg");
var modalImg = document.getElementById("img01");
var captionText = document.getElementById("caption");
    
img.onclick = function(){
    modal.style.display = "block";
    modalImg.src = this.src;
    captionText.innerHTML = this.alt;
}
var span = document.getElementsByClassName("close")[0];
span.onclick = function() { 
    modal.style.display = "none";
}
