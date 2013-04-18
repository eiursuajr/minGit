ektj$(document).ready(function() {
    if (ektj$('#container-1')[0] != null) {
        ektj$('#container-1')[0].style.visibility = 'visible';
    }
    ektj$('#container-1').tabs({ fxFade: true, fxSpeed: 'fast' });
});
function popAvatarTemplate(imgPath)
  { 
    var left = (screen.width/2)-(400/2);
    var top = (screen.height/2)-(500/2);
    window.open (imgPath, 'UploadAvatar', 'scrollbars=no, resizable=yes,width=400, height=500, top='+top+', left='+left);
  }