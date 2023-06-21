// this is a js function that will allow a user into the secret part of maddoxshull.com
// idk why you went to here instead of just looking at the console
// but ig everyone has a way of finding 

function valForm() {
    let x = document.forms["secret"]["fpass"].value;
    if (x == "welcome to the end") {
        alert("welcome to the end...");
        return true;
    }
}
