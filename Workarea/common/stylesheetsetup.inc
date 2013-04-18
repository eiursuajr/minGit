<script language=vb runat=server>
'Begin Image Swap Code
Dim myTemp as Object
Dim MyButtonName as integer=100
Dim ApplicationUI As New ApplicationAPI
Dim DisplayTransText As Boolean=ApplicationUI.DisplayTransText

Function HyperlinkWCaption(ByVal hrefPath as Object, ByVal DisplayText as Object, ByVal HeaderText as Object, ByVal specialEvents as Object) as Object
	Dim Output as Object

	Output = "<a href=""" & hrefPath & """ onMouseOver="""
	if (DisplayTransText) then
		if (HeaderText <> "") then
			Output = Output & "ShowTransString('" & Replace(HeaderText, "'", "\'") & "');"
		end if
	end if
	' Output = Output & "RollOver(this);"" onMouseOut="""
	Output = Output & """ onMouseOut="""
	if (DisplayTransText) then
		if (HeaderText <> "") then
			Output = Output & "HideTransString();"
		end if
	end if
	' Output = Output & "RollOut(this);"">" & DisplayText & "</a>"
	Output = Output & """>" & DisplayText & "</a>"

	MyButtonName = MyButtonName + 1
	HyperlinkWCaption = Output

End Function

Function GetButtonEvents(ByVal imageFile as Object, ByVal hrefPath as Object, ByVal altText as Object, ByVal specialEvents as Object) as Object
	Dim Output as Object
	Dim OnClick as Object

	OnClick = "SelectButton(this);"
	Output = "<td " _
			& "id=""image_cell_" & MyButtonName & """ " _
			& "class=""button"" title=""" & altText & """>" _
			& "<a id=""image_link_" & MyButtonName & """ href=""" & hrefPath & """ " & specialEvents & " " _
			& "onMouseOver=""RollOver(this);"" onMouseOut=""RollOut(this);"" style=""cursor: default;"">" _
			& "<img onClick=""" & OnClick & """ src=""" & imageFile & """ id=""image_" & MyButtonName & """ class=""button"">" _
			& "</a></td>"

	MyButtonName = MyButtonName + 1
	GetButtonEvents = Output

	End Function
	Function GetButtonEventsWCaption(ByVal imageFile as Object, ByVal hrefPath as Object, ByVal altText as Object, ByVal HeaderText as Object, ByVal specialEvents as Object) as Object
	Dim Output as Object
	Dim OnClick as Object

	OnClick = "SelectButton(this);"
	Output = "<td " _
			& "id=""image_cell_" & MyButtonName & """ " _
			& "class=""button"" title=""" & altText & """>" _
			& "<a id=""image_link_" & MyButtonName & """ href=""" & hrefPath & """ " & specialEvents & " "
	Output = Output	& "onMouseOver="""
	if (DisplayTransText) then
		if (HeaderText <> "") then
			Output = Output & "ShowTransString('" & Replace(HeaderText, "'", "\'") & "');"
		end if
	end if
	Output = Output & "RollOver(this);"" onMouseOut="""
	if (DisplayTransText) then
		if (HeaderText <> "") then
			Output = Output & "HideTransString();"
		end if
	end if
	Output = Output & "RollOut(this);"""
	Output = Output & " style=""cursor: default;"">" _
			& "<img onClick=""" & OnClick & """ src=""" & imageFile & """ id=""image_" & MyButtonName & """ class=""button"">" _
			& "</a></td>"

	MyButtonName = MyButtonName + 1
	GetButtonEventsWCaption = Output

	End Function

	Function GetTitleBar(ByVal Title as Object) as Object
	Dim Output as Object
	' The nowrap is placed in here because some versions of
	' Netscape do not support nowrap in the class definition.
	Output = "<td nowrap class=""ektronTitlebar"" colspan=""2""><span id=""WorkareaTitlebar"">" & Title & "</span>"
	Output = Output & "<span style=""display:none"" id=""_WorkareaTitlebar""></span></td>"
	GetTitleBar = Output
	End Function
	Function GetShowAllActiveLanguage(ByVal showAllOpt as Object, ByVal bgColor as Object, ByVal OnChangeEvt as Object, ByVal SelLang as Object) as Object
	GetShowAllActiveLanguage = "<td>" & ShowAllActiveLanguage(showAllOpt, bgColor, OnChangeEvt, SelLang) & "</td>"
	End Function

Function ShowAllActiveLanguage(ByVal showAllOpt as Object, ByVal bgColor as Object, ByVal OnChangeEvt as Object, ByVal SelLang as Object) as String
	Dim outDD as String = ""
	Dim fnMsgObj, fnColLang, fnLang as Object
	Dim colLang as Object
	If CStr(showAllOpt) = "" Then
	showAllOpt = False
	End If
	colLang = ContentLanguage
	if CStr(SelLang) <> "" then
		colLang = SelLang
	end if
	if (OnChangeEvt = "") then
		OnChangeEvt = "JavaScript:SelLanguage(this.value)"
	end if
	fnMsgObj = AppUI.EkSiteRef()
	fnColLang = fnMsgObj.GetAllActiveLanguages()
	If (EnableMultilingual = "1") Then
		outDD = "<select id=""frm_langID"" name=""frm_langID"" OnChange=""" & OnChangeEvt & """>" & vbcrlf

		If CBool(showAllOpt) Then
			If (colLang = ALL_CONTENT_LANGUAGES) Then
				outDD = outDD & "<option value=" & ALL_CONTENT_LANGUAGES & " selected>All</option>"
			else
				outDD = outDD & "<option value=" & ALL_CONTENT_LANGUAGES & ">All</option>"
			End If
		End If
		For Each fnLang In fnColLang
			If(fnLang.value("ID")<>"0") Then
				If Cstr(colLang)=Cstr(fnLang.value("ID")) Then
					outDD = outDD & "<option value=" & fnLang.value("ID") & " selected>" &  fnLang.value("Name") & "</option>"
				Else
					outDD = outDD & "<option value=" & fnLang.value("ID") & ">" &  fnLang.value("Name") & "</option>"
				End If
			End If
		Next
		outDD = outDD & "</select>"
		End If

		fnMsgObj = Nothing
		fnColLang = Nothing
		fnLang = Nothing
		Return outDD
		End Function

		Function ShowActiveLangsInList(ByVal showAllOpt as Object, ByVal bgColor as Object, ByVal OnChangeEvt as Object, ByVal SelLang as Object, ByVal includeList as Object) as Object
		' This function works like "ShowAllActiveLanguage" except it accepts a comma-seperated
		' string value of Language IDs. Use this for showing a drop-down of languages that
		' a piece of content, or other item, has been translated to.
		'
		' For example, if your active languages are French, German dn English and you have a
		' piece of content that is translated in English and French already but not German, you would
		' call ShowActiveLangsInList(showAllOpt, bgColor, OnChangeEvt, SelLang, "1033,1036"). This function
		' will go to the DB, determine what 1033 and 1036 is and display them in a drop down, but would
		' not show German.
		'
		return(showClusivityLangs(showAllOpt, bgColor, OnChangeEvt, SelLang, includeList, true))
		End Function
		Function ShowActiveLangsNotInList(ByVal showAllOpt as Object, ByVal bgColor as Object, ByVal OnChangeEvt as Object, ByVal SelLang as Object, ByVal excludelist as Object) as Object
		' This function works like ShowActiveLansInList, however, it does the opposite. It
		' shows all languages in the drop-down that are not in the excludelist. This way you
		' can show a drop-down of all the languages for which a piece of content, or other item,
		' has not yet been translated.
		'
		' For example, if your active languages are French, German and English and you have a
		' piece of content that is translated in English and French already but not German, you would
		' call ShowActiveLangsNotInList(showAllOpt, bgColor, OnChangeEvt, SelLang, "1033,1036"). This function
		' will go to the DB, get all the active languages and build a drop-down without 1033 and 1036 in it, thereby
		' leaving only German (and "All" if available).
		'
		return(showClusivityLangs(showAllOpt, bgColor, OnChangeEvt, SelLang, excludelist, false))
		End Function

		Function showClusivityLangs(ByVal showAllOpt as Object, ByVal bgColor as Object, ByVal OnChangeEvt as Object, ByVal SelLang as Object, ByVal excludelist as Object, ByVal include as Object) as Object
		' This function used as a private member. See "ShowActiveLangsInList" and "ShowActiveLangsNotInList"
		'
		Dim outDD, fnMsgObj, fnColLang, fnLang, idList as Object
		Dim colLang as Object

		idList = split(excludelist,",")

		' return(idList(0) & idList(1) )

	If CStr(showAllOpt) = "" Then
		showAllOpt = False
	End If
	colLang = ContentLanguage
	if CStr(SelLang) <> "" then
		colLang = SelLang
	end if
	if (OnChangeEvt = "") then
		OnChangeEvt = "JavaScript:SelLanguage(this.value)"
	end if
	fnMsgObj = AppUI.EkSiteRef()
	fnColLang = fnMsgObj.GetAllActiveLanguages()
	If (EnableMultilingual = "1") Then
		outDD = "<select id=""frm_langID"" name=""frm_langID"" OnChange=""" & OnChangeEvt & """>" & vbcrlf

		If CBool(showAllOpt) Then
			If (colLang = ALL_CONTENT_LANGUAGES) Then
				outDD = outDD & "<option value=" & ALL_CONTENT_LANGUAGES & " selected>All</option>"
			else
				outDD = outDD & "<option value=" & ALL_CONTENT_LANGUAGES & ">All</option>"
			End If
		End If

		If (not include) then
			outDD = outDD & "<option value=0 >Select a Language</option>"
		End If

		For Each fnLang In fnColLang
			If (fnLang.value("ID")<>"0") Then
				If(include) Then
					If (isInList(fnLang.value("ID"),idList)) Then
						If Cstr(colLang)=Cstr(fnLang.value("ID")) Then
							outDD = outDD & "<option value=" & fnLang.value("ID") & " selected>" &  fnLang.value("Name") & "</option>"
						Else
							outDD = outDD & "<option value=" & fnLang.value("ID") & ">" &  fnLang.value("Name") & "</option>"
						End If
					End If
				Else
					If (not isInList(fnLang.value("ID"),idList)) Then
						'If Cstr(colLang)=Cstr(fnLang.value("ID")) Then
						'	outDD = outDD & "<option value=" & fnLang.value("ID") & " selected>" &  fnLang.value("Name") & "</option>"
						'Else
							outDD = outDD & "<option value=" & fnLang.value("ID") & ">" &  fnLang.value("Name") & "</option>"
						'End If
					End If
				End If
			End If
		Next
		outDD = outDD & "</select>"
		return(outDD)
		End If

		fnMsgObj = Nothing
		fnColLang = Nothing
		fnLang = Nothing
		return Nothing
		End Function
		Function isInList(ByVal LanguageID as Object, ByVal includeArr as Object) as Object
		Dim x as Object

		For x = 0 to includeArr.Length - 1
		If (Trim(includeArr(x)) = Trim(LanguageID)) Then
		isInList = True
		Exit Function
		End If
		Next

		isInList = False
		Exit Function

		End Function
	</script>

<script type="text/javascript" src="java/stylehelper.js"></script>
<script type="text/javascript" language="javascript">
<!---
	var m_prevObj;
	var jsAppImgPath = "<%=(AppImgPath)%>";
	var relativeClassPath = "<%=(AppPath)%>csslib/";
	relativeClassPath = relativeClassPath.toLowerCase();

	function IsBrowserSafari() {
		var posn;
		posn = parseInt(navigator.appVersion.indexOf('Safari'));
		return (0 <= posn);
	}

	// Update all the stylesheet classes now (must delay for Safari if shtylesheets inaccessible):
	InitClassPaths();
	//
	function InitClassPaths() {
		if (document.styleSheets.length > 0) {
			MakeClassPathRelative("*", "button", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "button-over", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "button-selected", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "button-selectedOver", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "ektronToolbar", "backgroundImage", jsAppImgPath, relativeClassPath)
			MakeClassPathRelative("*", "ektronTitlebar", "backgroundImage", jsAppImgPath, relativeClassPath)
		} else {
			setTimeout('InitClassPaths()', 250);
		}
	}

	function ShowTransString(Text) {
		var ObjId = "WorkareaTitlebar";
		var ObjShow = document.getElementById('_' + ObjId);
		var ObjHide = document.getElementById(ObjId);
		if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
			ObjShow.innerHTML = Text;
			ObjShow.style.display = "inline";
			if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
				ObjHide.style.display = "none";
			}
		}

	}
	function HideTransString() {
		var ObjId = "WorkareaTitlebar";
		var ObjShow = document.getElementById(ObjId);
		var ObjHide = document.getElementById('_' + ObjId);

		if ((typeof ObjShow != "undefined") && (ObjShow != null)) {
			ObjShow.style.display = "inline";
			if ((typeof ObjHide != "undefined") && (ObjHide != null)) {
				ObjHide.style.display = "none";
			}
		}
	}
	function GetCellObject(MyObj) {
		var tmpName = "";

		tmpName = MyObj.id;
		if (tmpName.indexOf("link_") >= 0) {
			tmpName = tmpName.replace("link_", "cell_");
		}
		else if (tmpName.indexOf("cell_") >= 0) {
			tmpName = tmpName;
		}
		else {
			tmpName = tmpName.replace("image_", "image_cell_");
		}
		MyObj = document.getElementById(tmpName);
		return (MyObj);
	}

	function RollOver(MyObj) {
		MyObj = GetCellObject(MyObj);
		if (IsBrowserSafari()){
			if (m_prevObj && (m_prevObj != MyObj)) {
				RollOut(m_prevObj);
			}
			MyObj.className = "button-over";
			m_prevObj = MyObj;
		} else {
		    MyObj.className = "button-over";
		}
	}

	function RollOut(MyObj) {		
		MyObj = GetCellObject(MyObj);
		MyObj.className = "button";
	}

	function SelectButton(MyObj) {
		// Do not execute the following lines (leaves button selected,
		// which can be annoying if window is not refreshed/closed...):
		//MyObj = GetCellObject(MyObj);
		//UnSelectButtons();
		//MyObj.className = "button-selectedOver";
	}

	function UnSelectButtons() {
		var iLoop = 100;

		while (document.getElementById("image_cell_" + iLoop.toString()) != null) {
			document.getElementById("image_cell_" + iLoop.toString()).className = "button" ;
			iLoop++;
		}
	}

	function Trim (string) {
		if (string.length > 0) {
			string = RemoveLeadingSpaces (string);
		}
		if (string.length > 0) {
			string = RemoveTrailingSpaces(string);
		}
		return string;
	}

	function RemoveLeadingSpaces(string) {
		while(string.substring(0, 1) == " ") {
			string = string.substring(1, string.length);
		}
		return string;
	}

	function RemoveTrailingSpaces(string) {
		while(string.substring((string.length - 1), string.length) == " ") {
			string = string.substring(0, (string.length - 1));
		}
		return string;
	}

	function SelLanguage(inVal) {
		// TODO: Use RegEx to replace the querystring
		//
		// Reconsider previous statement:
		// Note, to fix defect #10951 ("can't add subfolders to folders
		// whose names start with 'x' or 'u'") we could use RegEx "\\?\\[xu]"
		// but this could leave out many other possibilities...
		// Probably best to use URLEncode(?) as done below. BCB, November 29th 2004.
		//
		<%

		myTemp =  Request.ServerVariables("PATH_INFO").Substring(Request.ServerVariables("PATH_INFO").lastIndexOf("/")+1 )
		myTemp=myTemp & "?" & _
		Ektron.Cms.Common.EkFunctions.HtmlEncode(Replace(Request.ServerVariables("QUERY_STRING"),"LangType","L")) 'TODO:UDAI/12/02/04 THIS SEEMS TO ME WRONG THIS HAS TO BE REMOVED
		myTemp = Replace(mytemp, "'", "")
		myTemp = Replace(mytemp, "\x", "\\x")
		myTemp = Replace(mytemp, "\u", "\\u")
		myTemp = Replace(mytemp, "&amp;", "&")
		'myTemp = Server.URLEncode(myTemp)
		%>
		top.notifyLanguageSwitch(inVal, -1);
		document.location = '<%=(myTemp)%>&LangType=' + inVal ;
	}

	var m_DebugWindow=null;
	function DebugMsg(Msg) {
		Msg = '>>>' + Msg + ' <br> ';
		if ((m_DebugWindow == null) || (m_DebugWindow.closed)) {
			m_DebugWindow = window.open('', 'myWin', 'toolbar=no, directories=no, location=no, status=yes, menubar=no, resizable=yes, scrollbars=yes, width=300, height=300');
		}
		m_DebugWindow.document.writeln(Msg);
	}

//-->
</script>