window.onload = function() {
    var url = window.location.href;
    if(url.indexOf('?subscribed=true') > -1) {
      document.getElementById('subscribed').classList.add('subIn');
    }
  };

function imgClick(goto) {
    window.location.href = "/" + goto;
    console.log("Going to " + goto);
}
