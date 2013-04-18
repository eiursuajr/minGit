<script type="text/javascript">

    function DefaultTitleCheck(chkDefault, frmName, FieldName)
	 {
		var objField;
		objField = eval("document.forms." + frmName + "." + FieldName);
		if (chkDefault.checked == true) {
			objField.disabled = true;
		}
		else {
			objField.disabled = false;
		}
		return true;
	}

	function Trim(string) 
	{
	    if (string.length > 0) 
		{
			string = RemoveLeadingSpaces (string);
		}
		if (string.length > 0) 
		{
			string = RemoveTrailingSpaces(string);
		}
		return string;
	}

	function RemoveLeadingSpaces(string) 
	{
	    while (string.substring(0, 1) == " ")
		{
			string = string.substring(1, string.length);
		}
		return string;
	}

	function RemoveTrailingSpaces(string) 
	{
	    while (string.substring((string.length - 1), string.length) == " ")
		{
			string = string.substring(0, (string.length - 1));
		}
		return string;
	}
	
	function resetPostback()
	{
		document.forms[0].isPostData.value = "";
	}

	function LoadLanguage(inVal) 
	{
	    if (inVal == '0')
	    {
	        return false; 
		}
		top.notifyLanguageSwitch(inVal, -1);
		document.location = '<%=Request.ServerVariables["PATH_INFO"] + "?" + Microsoft.Security.Application.AntiXss.UrlEncode(Request.ServerVariables["QUERY_STRING"].ToString()).Replace("LangType", "L").Replace("\\x", "\\\\x").Replace("%3d","=").Replace("%26","&")%>&LangType=' + inVal ;
    }

    function VerifyCollectionForm()
     {
		regexp1 = /"/gi;		// quote ... "
		document.forms.nav.frm_nav_title.value = Trim(document.forms.nav.frm_nav_title.value);
		document.forms.nav.frm_nav_title.value = document.forms.nav.frm_nav_title.value.replace(regexp1, "'");

		document.forms.nav.frm_nav_description.value = Trim(document.forms.nav.frm_nav_description.value);
		document.forms.nav.frm_nav_description.value = document.forms.nav.frm_nav_description.value.replace(regexp1, "'");
		if (document.forms.nav.frm_nav_title.value == "")
		{
			alert ('<%=(MsgHelper.GetMessage("js: collection title required msg"))%>');
			document.forms.nav.frm_nav_title.focus();
			return false;
		}
		return true;
     }

     function addBaseMenu(menuID, parentID, ancestID, foldID, langID) 
	 {
		document.location = 'collections.aspx?action=AddTransMenu&nId=' + menuID + '&backlang=<%=ContentLanguage%>&LangType=' + langID + '&folderid=' + foldID + '&ancestorid=' + ancestID + '&parentid=' + parentID   ;
     }
	 
     function CloseChildPage()
     {
	    var pageObj = document.getElementById("FrameContainer");
	    pageObj.style.display = "none";
	    pageObj.style.width = "1px";
	    pageObj.style.height = "1px";
     }
    
     function ReturnChildValue(contentid,contenttitle,QLink, FolderID,LanguageID)
     {
        CancelIframe();
        if(QLink.substring(0,7) =="http://")
        {
        document.getElementById("frm_menu_link").value = QLink;
        }else{
	    document.getElementById("frm_menu_link").value = QLink.replace('<%=SitePath%>', '');}
	 }

	 function LoadSelectContentPage(languageID)
     {
        PopUpWindow("SelectCreateContent.aspx?FolderID=0&rmadd=false&LangType=" + languageID + "&browser=1&ty=menu", "SelectContent", 490, 500, 1, 1);
     }
    
     function CancelIframe()
     {
	    var pageObj = document.getElementById("FrameContainer");
	    pageObj.style.display = "none";
	    pageObj.style.width = "1px";
	    pageObj.style.height = "1px";
     }

     function OnFileClicked(path)
     {
         if (path != null && path.length > 0)
         {
            var listObj = document.getElementById("template_list");
            if (listObj) 
               {
                var currentOption;
                var index = listObj.length;
                for (index = 0; index < listObj.length; index++) 
                    {
                    currentOption = listObj.options[index];
                    if (path == currentOption.value) {
                        alert("duplicate Text/Value supplied"); return false;
                    }
                }
                index = listObj.length;
                listObj.length = index + 1;
                listObj.options[index].value = path;
                listObj.options[index].text = path;
                document.getElementById("template_text").value = "";
                ta_updateSelectedValues();
            }
        }

        $ektron("#dlgBrowse").modal().modalHide();
      }

	 Ektron.ready(function() {
	        // add ektronPageHeader since baseClass doesn't add it for us
	        $ektron("table.baseClassToolbar").wrap("<div class='ektronPageHeader'></div>");

	        // Initialize browse dialog
	        $ektron("#dlgBrowse").modal({
	            modal: true,
	            trigger: ".ektronModal",
	            onShow: function(h) {
	                $ektron("#dlgBrowse").css("margin-top", -1 * Math.round($ektron("#dlgBrowse").outerHeight() / 2)); h.w.show();
	            }
	        });

	        var checkbox = $ektron(".pageBuilderCheckbox input");
	        checkbox.click(function() {
	        });

	        var widgets = $ektron(".widget");
	        widgets.each(function(i) 
	        {
	            var widget = $(widgets[i]);
	            if (widget.find("input").is(":checked")) 
	            {
	                widget.addClass("selected");
	            }

	            widget.click(function() {
	                var widgetCheckbox = widget.find("input");

	                ToggleCheckbox(widgetCheckbox);
	                if (widgetCheckbox.is(":checked"))
	                 {
	                    widget.addClass("selected");
	                }
	                else
	                 {
	                    widget.removeClass("selected");
	                }
	            });
	        });
	    });

	 var m_prevObj;
	 var jsAppImgPath = "<%=(AppImgPath)%>";
	 var relativeClassPath = "<%=(AppPath)%>csslib/";
	 relativeClassPath = relativeClassPath.toLowerCase();

	function IsBrowserSafari()
	 {
		var posn;
		posn = parseInt(navigator.appVersion.indexOf('Safari'));
	 	return (0 <= posn);
	 }

	// Update all the stylesheet classes now (must delay for Safari if shtylesheets inaccessible):
	InitClassPaths();
	//
	function InitClassPaths() 
	{
	    if (document.styleSheets.length > 0)
		 {
			MakeClassPathRelative("*", "button", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "button-over", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "button-selected", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "button-selectedOver", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "ektronToolbar", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "ektronTitlebar", "backgroundImage", jsAppImgPath, relativeClassPath)

         }
         else
		  {
			setTimeout('InitClassPaths()', 250);
		  }
	}

	function ShowTransString(Text)
	 {
		var ObjId = "WorkareaTitlebar";
		var ObjShow = document.getElementById('_' + ObjId);
		var ObjHide = document.getElementById(ObjId);
		if ((typeof ObjShow != "undefined") && (ObjShow != null))
		 {
			ObjShow.innerHTML = Text;
			ObjShow.style.display = "inline";
			if ((typeof ObjHide != "undefined") && (ObjHide != null))
			 {
				ObjHide.style.display = "none";
			 }
		}
}
	
	function HideTransString()
	 {
		var ObjId = "WorkareaTitlebar";
		var ObjShow = document.getElementById(ObjId);
		var ObjHide = document.getElementById('_' + ObjId);

		if ((typeof ObjShow != "undefined") && (ObjShow != null)) 
		{
			ObjShow.style.display = "inline";
			if ((typeof ObjHide != "undefined") && (ObjHide != null)) 
			{
				ObjHide.style.display = "none";
			}
		}
	}

	function UnSelectButtons() 
	{
		var iLoop = 100;
		while (document.getElementById("image_cell_" + iLoop.toString()) != null) 
		{
			document.getElementById("image_cell_" + iLoop.toString()).className = "button" ;
			iLoop++;
		}
	}

	function Trim(string) 
	{
	    if (string.length > 0) 
		{
			string = RemoveLeadingSpaces (string);
		}
		if (string.length > 0) 
		{
			string = RemoveTrailingSpaces(string);
		}
		return string;
	}

	function RemoveLeadingSpaces(string)
	{
	    while (string.substring(0, 1) == " ") 
		{
			string = string.substring(1, string.length);
		}
		return string;
	}

	function RemoveTrailingSpaces(string) 
	{
	    while (string.substring((string.length - 1), string.length) == " ") 
		{
			string = string.substring(0, (string.length - 1));
		}
		return string;
	}
	
	var m_DebugWindow=null;
	function DebugMsg(Msg) 
	{
		Msg = '>>>' + Msg + ' <br> ';
		if ((m_DebugWindow == null) || (m_DebugWindow.closed)) 
		{
			m_DebugWindow = window.open('', 'myWin', 'toolbar=no, directories=no, location=no, status=yes, menubar=no, resizable=yes, scrollbars=yes, width=300, height=300');
		}
		m_DebugWindow.document.writeln(Msg);
	}
</script>