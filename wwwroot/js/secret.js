// hi whoevers looking that this!
// i really didn't think anyone would look at this but good for you
// no cheat codes here but kinda cool idk

document.getElementById("secret").addEventListener("submit", function (event) {
    event.preventDefault();

    let inputValue = document.getElementById("fpass").value;

    if (inputValue === "i want more") {
        window.location.href = "/secret";
    }
});
