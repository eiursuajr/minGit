var timeoutPeriod = 10;
var intervalCountdown;
var timeoutWarning;
var lastActivity = new Date();
function showWarning()
{
    $ektron('#divTimeOut').modalShow();
    intervalCountdown = setInterval(countDown,1000);
}
function countDown() {
  var divCountDown = document.getElementById('sessionCountDown');
  var intCount = parseInt(divCountDown.innerHTML) - 2;

  if (intCount == 0) {
      $ektron('#sessionCountDown').html(0);  
      clearInterval(intervalCountdown);
      clearTimeout(timeoutWarning);
      SubmitForm(1, true);
  }
  else if (intCount == 10) {
    $ektron('#sessionCountDown').html(intCount.toString());  
    $ektron('#sessionCountDown').addClass('sessionCountDownWarning');
  }
  else if (intCount >= 0) {
      $ektron('#sessionCountDown').html(intCount.toString());
  }
}
function LogActivity() {
    if ($ektron('#divTimeOut').css('display') != 'block') {
        clearInterval(intervalCountdown);
        clearTimeout(timeoutWarning);
        timeoutWarning = setTimeout(showWarning, timeoutPeriod * 60000);
        var differential = Math.ceil(((new Date()).getTime() - lastActivity.getTime()) / (1000 * 60));
        if (differential > 3) {
            var decRound = Math.round(Math.random() * 1000);
            var imgRefresh = new Image(1, 1);
            imgRefresh.src = '../keepalive.aspx?id=' + decRound.toString();
        }
    }
}
function refreshPage(){
    clearInterval(intervalCountdown);
    clearTimeout(timeoutWarning);
    timeoutWarning = setTimeout(showWarning,timeoutPeriod * 60000);
    $ektron('#sessionCountDown').removeClass('sessionCountDownWarning');
    $ektron('#sessionCountDown').html('120');
    var decRound = Math.round(Math.random()*1000);
    var imgRefresh = new Image(1,1);
    imgRefresh.src = '../keepalive.aspx?id=' + decRound.toString();
    $ektron('#divTimeOut').modalHide();
}