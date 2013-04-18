// JScript File

function ShowAddPersonalTagArea(){
		var newTagDiv = document.getElementById("newTagNameDiv");
		if (newTagDiv){
			newTagDiv.style.display = "";
			var objTagName = document.getElementById("newTagName");
			if (objTagName){
				objTagName.value = "";
				objTagName.focus();
			}
		}
	}
	
	this.customPTagCnt = 0;
	function SaveNewPersonalTag(){
		// add new tag:
		var objTagName = document.getElementById("newTagName");
		var objTagLanguage = document.getElementById("TagLanguage");
		var objLanguageFlag = document.getElementById("flag_" + objTagLanguage.value);
		var divObj = document.getElementById("newTagNameScrollingDiv");
		
		if (null == objTagName || "undefined" == typeof objTagName.value) {
		    return;
		}
		
		if(!CheckForillegalChar(objTagName.value)){
		    return;
		}
		
		if(IsTagTooLong(objTagName.value)){
		    return;
		}
		
		if (objTagName && (objTagName.value.length > 0) && divObj){
			++this.customPTagCnt;
			divObj.innerHTML += "<input type='checkbox' checked='checked' onclick='ToggleCustomPTagsCbx(this, \"" + objTagName.value + "\");' id='userCustomPTagsCbx_" + this.customPTagCnt + "' name='userCustomPTagsCbx_" + this.customPTagCnt + "' />&#160;" 
			
			if(objLanguageFlag != null){
			    divObj.innerHTML += "<img src='" + objLanguageFlag.value + "' border=\"0\" />"
			}
			
			divObj.innerHTML +="&#160;" + objTagName.value + "<br />"
			
			AddHdnTagNames(objTagName.value + '~' + objTagLanguage.value);
			AddHdnTagNames(objTagName.value);
		}
		
		// now close window:
		CancelSaveNewPersonalTag();
	}
	
	function CancelSaveNewPersonalTag(){
		var newTagDiv = document.getElementById("newTagNameDiv");
		if (newTagDiv){
			newTagDiv.style.display = "none";
		}
	}

	function AddHdnTagNames(newTagName){
	
		objHdn = document.getElementById("newTagNameHdn");
		if (objHdn){
			var vals = objHdn.value.split(";");
			var matchFound = false;
			for (var idx = 0; idx < vals.length; idx++){
				if (vals[idx] == newTagName){
					matchFound = true;
					break;
				}
			}
			if (!matchFound){
				if (objHdn.value.length > 0){
					objHdn.value += ";";
				}
				objHdn.value += newTagName;
			}
		}
	}

	function RemoveHdnTagNames(oldTagName){
		objHdn = document.getElementById("newTagNameHdn");
		if (objHdn && (objHdn.value.length > 0)){
			var vals = objHdn.value.split(";");
			objHdn.value = "";
			for (var idx = 0; idx < vals.length; idx++){
				if (vals[idx] != oldTagName){
					if (objHdn.value.length > 0){
						objHdn.value += ";";
					}
					objHdn.value += vals[idx];
				}
			}
		}
	}
	
	function CheckForillegalChar(tag) {
       if (Trim(tag) == '')
       {
           alert('Please enter a name for the Tag.'); 
           return false; 
       } else { 

            //alphanumeric plus _ -
            var tagRegEx = /[!"#$%&'()*+,./:;<=>?@[\\\]^`{|}~ ]+/;
            if(tagRegEx.test(tag)==true) {
                alert('Tag Text can only include alphanumeric characters.');
                return false;
            }
           
       }
       return true;
    }
    
    function IsTagTooLong(tag) {
        if (tag.length > 25) {
            alert("The tag is too long! (Limit: 25 characters)");
            return true;
        }
        return false;
    }
	
	function ToggleCustomPTagsCbx(btnObj, tagName){
		if (btnObj.checked){
			AddHdnTagNames(tagName);
			btnObj.checked = true;
		}
		else{
			RemoveHdnTagNames(tagName);
			btnObj.checked = false; // otherwise re-checks when adding new custom tag.
		}
	}