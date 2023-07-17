// Retrieve the username from the cookie
const username = document.cookie.split(';').find(cookie => cookie.trim().startsWith('username=')).split('=')[1];
const usernamePlaceholder = document.getElementById('username-placeholder');

if (!username) {
    usernamePlaceholder.innerHTML = 'Err:001';
} else {
    usernamePlaceholder.textContent = username;
}