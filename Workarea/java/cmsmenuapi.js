
function showBranch(branch){
	var objBranch = document.getElementById(branch).style;
	if(objBranch.display=="block")
		objBranch.display="none";
	else
		objBranch.display="block";
	swapFolder('I' + branch);
}

function swapFolder(img){
	var MyArray;
	var MyArrays;
	var Path;
	objImg = document.getElementById(img);
	if(objImg.src.indexOf('-closed.gif')>-1) {
		MyArray = objImg.src 
		MyArrays = objImg.src.lastIndexOf("-");
		Path = MyArray.substring(0,MyArrays) + "-open.gif";
		objImg.src = Path
	} else {
		MyArray = objImg.src 
		MyArrays = objImg.src.lastIndexOf("-");
		Path = MyArray.substring(0,MyArrays) + "-closed.gif";
		objImg.src = Path
	}
}
