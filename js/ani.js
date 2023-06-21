var count = 0;
var flyIn = false;
var done = false;

window.addEventListener('scroll', function() {
    var scrollPosition = window.scrollY;
    var windowHeight = window.innerHeight;

    var scaledScrollPosition = scrollPosition / windowHeight;
 
    // scale pos
    // console.log('Scaled Scroll Position:', scaledScrollPosition);

    if (scaledScrollPosition > 0.85 && count == 0) {
        $('#memories').css('opacity', 1);
        count = 1;
    }
  
    if (scaledScrollPosition > 1.9 && count == 1) {
        $('.fat').css('opacity', 1);
        count = 2;
    };

    if (scaledScrollPosition > 4.8 && count == 2) {
        $('.gym').css('opacity', 1);
        count = 3;
    };

    if (scaledScrollPosition > 8 && count == 3) {
        $('.lay').css('opacity', 1);
        count = 4;
    };

    if (scaledScrollPosition > 11.25 && count == 4) {
        $('.sit').css('opacity', 1);
        count = 5;
    };

    if (scaledScrollPosition > 14.4 && count == 5) {
        $('.back').css('opacity', 1);
        count = 6;
    };

    if (scaledScrollPosition > 17.5 && count == 6) {
        $('.backside').css('opacity', 1);
        count = 7;
    };

    if (scaledScrollPosition > 20.6 && count == 7) {
        $('.weights').css('opacity', 1);
        count = 8;
    };

    if (scaledScrollPosition > 24 && count == 8) {
        $('#celebrate').css('opacity', 1);
        count = 9;

        flyIn = true;
    };

    if (flyIn && !done) { 
        document.querySelector('#fatox-card').classList.add('fa', 'unhoverable');
        document.querySelector('#gymdox-card').classList.add('ga', 'unhoverable');
        document.querySelector('#laydox-card').classList.add('la', 'unhoverable');
        document.querySelector('#sitox-card').classList.add('sa', 'unhoverable');
        document.querySelector('#backox-card').classList.add('ba', 'unhoverable');
        document.querySelector('#buttox-card').classList.add('bu', 'unhoverable');
        document.querySelector('#preddox-card').classList.add('pa', 'unhoverable');

        this.setTimeout(function() {
            document.querySelector('#fatox-card').classList.remove('fa', 'unhoverable');
            document.querySelector('#gymdox-card').classList.remove('ga', 'unhoverable');
            document.querySelector('#laydox-card').classList.remove('la', 'unhoverable');
            document.querySelector('#sitox-card').classList.remove('sa', 'unhoverable');
            document.querySelector('#backox-card').classList.remove('ba', 'unhoverable');
            document.querySelector('#buttox-card').classList.remove('bu', 'unhoverable');
            document.querySelector('#preddox-card').classList.remove('pa', 'unhoverable');
            
            document.getElementById('fatox-card').style.opacity = 1;
            document.getElementById('gymdox-card').style.opacity = 1;
            document.getElementById('laydox-card').style.opacity = 1;
            document.getElementById('sitox-card').style.opacity = 1;
            document.getElementById('backox-card').style.opacity = 1;
            document.getElementById('buttox-card').style.opacity = 1;
            document.getElementById('preddox-card').style.opacity = 1;

            console.log("welcome to the end")
        }, 8000);
        done = true;
    }
});

function delay(time) {
    return new Promise(resolve => setTimeout(resolve, time));
}
