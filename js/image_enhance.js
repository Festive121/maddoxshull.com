function imageClick() {
    var img = document.getElementById("img");
    img.src = "images/2.jpg";
    img.style.width = "100%";
    img.style.height = "100%";
    img.style.transition = "all 1s";
    img.style.webkitTransition = "all 1s";
    img.style.mozTransition = "all 1s";
    img.style.oTransition = "all 1s";
    img.style.msTransition = "all 1s";
}