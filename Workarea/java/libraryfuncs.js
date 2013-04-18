	function InsertFunction(insertvalue, title, type){	
		if (!top.opener.closed){
			if (!document.all && document.getElementById) {
				var typename = "function";
			}
			else {
				var typename = "object";
			}
			if (typeof(top.opener.eWebEditPro["content_html"]) == typename) {
				var textsection = "content_html";
			}
			else {
				var textsection = "content_teaser";
			}
			var selectedText = top.opener.eWebEditPro[textsection].getSelectedHTML();
			if (type == "images"){
				top.opener.eWebEditPro.instances[textsection].insertMediaFile(insertvalue,false,title,"IMAGE",0,0);
				top.close();
			}
			else if (type == "hyperlinks"){
				if ((insertvalue.substring(0, 7) != "http://") && (insertvalue.substring(0, 8) != "https://")) {
					insertvalue = "http://" + insertvalue; 
				}
				if (selectedText == "") {
					var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + title + '</a>';
				}
				else{
					var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + selectedText + '</a>';
				}
				top.opener.eWebEditPro[textsection].pasteHTML(stuff);
				top.close();
			}
			else{ 
				if (selectedText == ""){
					var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + title + '</a>';
				}
				else{
					var stuff = '<a href="' + insertvalue + '" title="' + title + '">' + selectedText + '</a>';
				}
				top.opener.eWebEditPro[textsection].pasteHTML(stuff);
				top.close();
			}
		}
		else{
			alert("Your file can not be inserted because the editor page has been closed.");
			return false;
		}		
	}

	function PreviewFunct(oldURL){
		var regexp1 = / /gi;
		if (window.navigator.userAgent.indexOf("Gecko") > -1) {
			var msg = "Ektron apologizes. Mozilla, Nestcape 6.0, 6.1, and 6.2 do not support";
			msg += "\n";
			msg += "local file preview from a URL launched Web page.";
			msg += "\n";
			msg += "This feature will be enabled when Netscape6 or Mozilla include support for it.";
			msg += "\n";
			msg += "However this feature is operational work in Netscape 4.7x and MS IE.";
			alert(msg);
			return false;
		}
		if (document.LibraryItem.frm_filename.value.length == 0) {
			alert("Please make a selection.");
			return false;
		}
		else {
			regexp2 = /\\/gi;
			var tempHREF = 'file:///' + document.LibraryItem.frm_filename.value.replace( regexp1, '%20');
			var tempHREF = tempHREF.replace(regexp2, "/");
		}
		for (var i = 0; i < document.links.length; i++) {
			if (document.links[i].href == oldURL) {
				break;
			}
		}
		document.links[i].href = tempHREF;
		return true;
	}

  