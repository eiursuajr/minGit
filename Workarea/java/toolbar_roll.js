//Begin Image Swap Code
				
	function SwapImageUp(Myobject) {
		var MyArray = document.images[Myobject].src 
		var MyArrays = document.images[Myobject].src.lastIndexOf("-");
		var Path = MyArray.substring(0,MyArrays) + "-up.gif";
		document.images[Myobject].src = Path
	}
	function SwapImageOver(Myobject) {
		var MyArray = document.images[Myobject].src 
		var MyArrays = document.images[Myobject].src.lastIndexOf("-");
		var Path = MyArray.substring(0,MyArrays) + "-up.gif";
		document.images[Myobject].src = Path
		
	}
	function SwapImageOut(Myobject) {
		var MyArray = document.images[Myobject].src 
		var MyArrays = document.images[Myobject].src.lastIndexOf("-");
		var Path = MyArray.substring(0,MyArrays) + "-nm.gif";
		document.images[Myobject].src = Path
	}
	function SwapImageDown(Myobject) {
		var MyArray = document.images[Myobject].src 
		var MyArrays = document.images[Myobject].src.lastIndexOf("-");
		var Path = MyArray.substring(0,MyArrays) + "-dn.gif";
		document.images[Myobject].src = Path
	}


function whichpage() {
  	document.mylanguage.action = "http://www.worldlingo.com/S606.1/api";
  	document.mylanguage.wl_password.value = "ektroncms43";
	loadxhtmlandworldlingo();
 	if (validate() == true) {
	   document.mylanguage.submit();
    }
}


//End Image Swap Code
function DisplayHoldMsg(bShow) {	
		var obj = document.getElementById("dvHoldMessage");
		if (obj != null) { 
			if (bShow == false) {obj.style.display = "none";}
			else {obj.style.display = "block";}
		} 			
		// don't return false or true;
	}

