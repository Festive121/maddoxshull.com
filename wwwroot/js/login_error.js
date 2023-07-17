window.addEventListener('DOMContentLoaded', (event) => {
    var urlParams = new URLSearchParams(window.location.search);
    var error = urlParams.get('error');

    if (error !== null && error !== '') {
        var errorMessage = document.getElementById('error-message');

        setTimeout(() => {
            errorMessage.classList.add('animate');
        }, 1000); // 1000ms = 1 second delay
    }
});

// Get the error message element
const message = document.getElementById('error-message');

// Add the 'animate' class to trigger the animation after a delay
setTimeout(() => {
    console.log("added class")
    message.classList.add('animate');
}, 1000); // 1000ms = 1 second delay





Im trying to get the error message to work good luck