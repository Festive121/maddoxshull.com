window.addEventListener('DOMContentLoaded', (event) => {
    var urlParams = new URLSearchParams(window.location.search);
    var error = urlParams.get('error');
  
    if (error) {
        const errorMessage = document.getElementById('error-message');
        errorMessage.style.display = 'block';
        errorMessage.classList.add('move-down');
        setTimeout(() => {
            errorMessage.style.zIndex = '1';
        }, 500); // Set the delay to 500 milliseconds (0.5 seconds)
    }
});
  