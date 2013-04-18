function SwapImage(Name, Num) {
		document [Name].src = option [Num].src;
}

function ExitWorkArea(sExitPage, bPrompt) {
	if (bPrompt) {
		if (confirm("Any changes made will not be saved.\n\nAre you sure you want to exit?")){
			top.location.href=sExitPage;
		}
	}
	else {
		top.location.href=sExitPage;
	}
}

function ReloadTop() {
	var bParentExists = top.opener;
	if (bParentExists != undefined) {
		top.opener.location.reload(true);
	}
}

function ReloadWindow() {
	var bParentExists = window.opener;
	if (bParentExists != undefined) {
		window.opener.location.reload(true);
	}
}

function CancelWindow() {
	self.close();
}

function PopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
	var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight;
	var popupwin = window.open(url, hWind, cToolBar);
	return popupwin;
}

function PreviewWindow(url) {
	location.href=url;
	//var cToolBar = "toolbar=1,location=1,directories=1,status=1,menubar=1,scrollbars=1,resizable=1";
	//var popupwin = window.open(url, "Preview", cToolBar);
}

function VerifyPassword(cPassword, cRetype) {
	var cTrimPwd = cPassword;
	var cRetypePwd = cRetype;
	cTrimPwd = cTrimPwd.replace(" ","");
	if (cTrimPwd.length == 0) {alert ("Please enter a password."); return false;}
	else if (cPassword != cRetypePwd) {alert ("Your password could not be confirmed."); return false;}
	return true;
}

function DisplayWarning(obj, sMsg) {
	if (obj.checked){
		bContinue = confirm (sMsg);
		if (!bContinue) { obj.checked = false;}
	}
}

function MenuPopUpWindow (url, hWind, nWidth, nHeight, nScroll, nResize) {
			var cToolBar = "toolbar=0,location=0,directories=0,status=" + nResize + ",menubar=0,scrollbars=" + nScroll + ",resizable=" + nResize + ",width=" + nWidth + ",height=" + nHeight
			var popupwin = window.open(url, hWind, cToolBar);
		}