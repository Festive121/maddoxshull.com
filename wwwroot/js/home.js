function imgClick(goto) {
    window.location.href = "/" + goto;
    console.log("Going to " + goto);
}

window.addEventListener('DOMContentLoaded', (event) => {
    var urlParams = new URLSearchParams(window.location.search);
    var error = urlParams.get('subscribed');
  
    if (error) {
        const errorMessage = document.getElementById('subscribed');
        errorMessage.style.display = 'block';
        errorMessage.classList.add('subIn');
        setTimeout(() => {
            errorMessage.style.zIndex = '1';
        }, 500);
    }
});
