var count = 0;

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
});
