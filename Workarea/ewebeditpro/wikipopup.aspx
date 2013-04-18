<%@ Page Language="C#" AutoEventWireup="true" Inherits="wikipopup" ValidateRequest="false" CodeFile="wikipopup.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Wiki Link</title>
    <asp:PlaceHolder runat="server" id="phHead">
        <script type="text/javascript">

	    //<!--
	    var selectedText='';
	    var selectedHTML='';
	    var deffolderid;
	    var folderid;
	    var languageID;
        var searchText = '';
        var contentLink = '';
        var wiki_cont_title = '';
        var wiki_link_target = '';
        var contentId = 0
        var selectedContentId = -1;

    	var pageNum = 1;
        var TOTAL_PAGES = 0;

        if (top.opener && top.opener.document.getElementById("content_id") != null)
        {
            contentId = top.opener.document.getElementById("content_id").value;
        }
	    function isWikiSelectContainer()
	    {
	        return true;
	    }
	    function loadselectedtext() {
	    <%if(Request.QueryString["EditorName"] == "JSEditor") { %>
	        var sel = window.opener.FTB_API['EkInnerEditor'].GetSelection();
	        if (window.opener.FTB_Browser.isIE) {
		        selectedText = sel.createRange().text;
		        selectedHTML = sel.createRange().htmlText;
	        } else {
		        selectedText = new String(sel);
	        }
	        searchText = trim(selectedText);
		    deffolderid = <%= Request.QueryString["FolderID"]%>;
		    languageID = <%= m_commonApi.ContentLanguage %>; 
		    document.forms[0].text2.value = selectedText;
        <% } else if("ContentDesigner" == SelectedEditControl) { %>
            var args = GetDialogArguments();
	        if (args)
	        {
	            selectedText = args.selectedText;
	            selectedHTML = args.selectedHTML;
	        }
	        searchText = trim(selectedText);
            deffolderid = <%= Request.QueryString["FolderID"] %>;
		    languageID = <%= m_commonApi.ContentLanguage %>;
		    document.forms[0].text2.value = selectedText;
		            if ("<img " == selectedHTML.substr(0, 5).toLowerCase())
		            {
		                document.forms[0].text2.value = selectedHTML;
		            }
        <% } else if(IsMac) { %>
            <% if(!IsBrowserIE) { %>
                selectedText = top.opener.document.getElementById('selectedtext').value;
	            searchText = trim(selectedText);
                selectedHTML = top.opener.document.getElementById('selectedhtml').value;
			    deffolderid = top.opener.document.forms[0].content_folder.value;
			    languageID = top.opener.document.forms[0].content_language.value;
			    document.forms[0].text2.value = selectedHTML;
            <% }
             } %>
        <% else { %>
		    var objInstance = eWebEditProUtil.getOpenerInstance();
		    if (objInstance && objInstance.isEditor())
		    {
			    selectedText = objInstance.editor.getSelectedText();
			    searchText = trim(selectedText);
			    selectedHTML = objInstance.editor.getSelectedHTML();
			    deffolderid = top.opener.document.forms[0].content_folder.value;
			    languageID = top.opener.document.forms[0].content_language.value;
			    document.forms[0].text2.value = selectedText;
		    }
        <% } %>
            var pos1 = 0;
            var pos2 = 0;
            var arStr = '';
                    pos1 = selectedHTML.indexOf("javascript:void window.open('");
                    if (pos1 > -1)
                    {
                        pos1 = pos1 + 29;
                        pos2 = selectedHTML.indexOf("'", pos1);
                    }
                    if (pos1 == -1)
                    {
                        pos1 = selectedHTML.indexOf('href="');
                        if (pos1 > -1) {
                            pos1 = pos1 + 6;
                            pos2 = selectedHTML.indexOf('"', pos1);
                        }
                    }
                    if (pos1 > -1)
                    {

                        arStr = selectedHTML.substring(pos1, pos2);
                        arStr = arStr.split('?');
                        if (arStr.length > 1)
                        {
                            var parms = arStr[1].split('&');
                            for (var i=0;i < parms.length; i++)
                            {
                                var parm = parms[i].split('=');
                                if (parm[0].toLowerCase() == "id" || parm[0].toLowerCase() == "itemid")
                                {
                                    if (isNumeric(parm[1]))
                                    {
                                        selectedContentId = parm[1];
                                    }
                                }
                            }
                        }
                    }

	    }
	    function isNumeric(x) {

            var RegExp = /^(-)?(\d*)(\.?)(\d*)$/;
            var result = x.match(RegExp);
            return result;
        }

	    function inserthyperlink() {
	        var bpaste = false;
	        <% if(Request.QueryString["EditorName"] == "JSEditor") { %>
	        var oOpen;
	        try
	        {
	            oOpen = window.opener.FTB_API['EkInnerEditor']
	        } catch(ex)
	        {
	            oOpen = null;
	        }
		    if (oOpen == null){
		    <%} else if("ContentDesigner" == SelectedEditControl) { %>
		        if ("undefined" == typeof window.radWindow)    {
		    <%} else if(IsMac && !IsBrowserIE) { %>
		        if (!window.opener.GetEphoxEditor()) {
		    <% } else {%>
		        if (!eWebEditProUtil.isOpenerAvailable()){
		    <% } %>
			            alert("Your link could not be inserted because the editor page has been closed.");
		        }
		    else if (document.forms[0].text2.value == ''){
			    alert("Your link text is blank and would create an empty link.");
		    }
		    else{
		        var pastevalue = '';
		        var qlink = '';
		        var bEndSpace = false;
	            var bStartSpace = false;
	            qlink = document.forms[0].text2.value;
	            if (qlink.length > 0 && (qlink.charAt(qlink.length - 1) == " " || qlink.charAt(qlink.length - 1) == "\n"))
	            {
	                bEndSpace = true;
	            }

	            if (qlink.length > 0 && (qlink.charAt(0) == " " || qlink.charAt(0) == "\n"))
	            {
	                bStartSpace = true;
	            }
	            qlink = trim(qlink);
		        pastevalue = GetContentLink();
		        if (pastevalue == '') {
			        var targetvalue;
			        var targetvaluepaste;
			        targetvalue = document.forms[0].Target.options[document.forms[0].Target.selectedIndex].value;
			        if (targetvalue == "") {
				        targetvaluepaste = "";
			        }
			        else {
				        targetvaluepaste = "target=" + targetvalue;
			        }
    			    wiki_cont_title = document.getElementById( 'wiki_cont_title' ).value;
    			    var wiki_cont_paste = '';
    			    if (wiki_cont_title == '')
    			    {
    			        alert("Title can not be blank.");
    			        document.getElementById( 'wiki_cont_title' ).focus();
    			        return false;
    			    } else {
    			        wiki_cont_title = wiki_cont_title.replace(/\"/g, '&quot;');
    			        wiki_cont_paste = 'wikititle="' + wiki_cont_title + '"';
    			    }
			        var catvaluepaste;
			        catvaluepaste = document.forms[0].txtCategory.value;

			        var foldervaluepaste = "";
			        folderid = document.getElementById('txtfolderid').value;
			        if (folderid != '' && (folderid != deffolderid))
			        {
			            foldervaluepaste = 'folderid="' + folderid + '"';
			        }

			        pastevalue = '<span  class="MakeLink" ' + foldervaluepaste + ' category="' + catvaluepaste + '" ' + targetvaluepaste + ' ' + wiki_cont_paste + '>' + document.forms[0].text2.value + '</span>';

			    }
			    else
			    {
    	            <% if(Request.QueryString["EditorName"] != "JSEditor"){  %>
			            pastevalue = '<a href="' + pastevalue + '">' + qlink + '</a>';
			        <% } %>
			        bpaste = true;
			    }
			    if (bStartSpace)
    	        {
    	            pastevalue = ' ' + pastevalue;
    	        }
    	        if (bEndSpace == true)
    	        {
    	            pastevalue = pastevalue + ' ';
    	        }

    	        <% if(Request.QueryString["EditorName"] == "JSEditor") { %>
    	        // alert(document.forms[0].text2.value + '-' + wiki_cont_title + '-' + folderid + '-' + targetvalue + '-' + catvaluepaste + '-' + pastevalue);
	            if (bpaste == true)
                {
	                window.opener.JSEURLInsert(pastevalue,document.forms[0].text2.value);
                }
                else
                {
                    window.opener.FTB_API['EkInnerEditor'].WikiLink(document.forms[0].text2.value, wiki_cont_title, folderid, targetvalue, catvaluepaste, pastevalue);
                }
                <% } else if("ContentDesigner" == SelectedEditControl) { %>
                    if (typeof window.radWindow != "undefined")
                    {
                        var args = { sHtml: pastevalue };
                        CloseDlg(args);
                    }
                <% } else if(IsMac && !IsBrowserIE) { %>
                    top.opener.insertHTML(pastevalue);
	            <% } else 
	                  {%>
	            eWebEditProUtil.getOpenerInstance().editor.ExecCommand('cmddelete','',0);
			    eWebEditProUtil.getOpenerInstance().editor.pasteHTML(pastevalue);
			        <%} %>
			    self.close();
		    }
	    }

		    function IsBrowserIE_Email() {
		    // document.all is an IE only property
		    return (document.all ? true : false);
	        }

		    function LoadChildPage() {
		     var frameObj = document.getElementById("ChildPage");
		     var pageObj = document.getElementById("FrameContainer");
			    if (IsBrowserIE_Email())
			    {
				    frameObj.src = "../blankredirect.aspx?SelectFolder.aspx?FolderID=" + document.getElementById("txtfolderid").value + "&LangType=" + languageID + "&browser=0&for_wiki=1";
				    
				    pageObj.style.display = "";
				    if (IsBrowserIE6() == true)
				    {
				        pageObj.style.width = "100%";
				        pageObj.style.height = 250; // IE6 fails to show window when  string value is assigned: "80%"; (this fails).
				        pageObj.style.top = 15;
				    }
				    else
				    {
				        pageObj.style.width = "98%";
				        pageObj.style.height = "80%";
				    }
			    }
			    else
			    {
				    frameObj.setAttribute('src', '../blankredirect.aspx?SelectFolder.aspx?FolderID=' + document.getElementById('txtfolderid').value + '&LangType=' + languageID + '&browser=0&for_wiki=1');
				    pageObj.style.display = "";
				    pageObj.style.width = "80%";
				    pageObj.style.height = "80%";
			    }

		    }

		    function ReturnChildValue(folderid,foldertitle,thirdpram) {
			    // take value, store it, write to display
			    CloseChildPage();
			    document.getElementById("contentidspan").innerHTML = "<div id=\"div3\" style=\"display: none;position: block;\"></div><div id=\"contentidspan\" style=\"display: block;position: block;\">(" + folderid + ")&nbsp;" + foldertitle + "&nbsp;&nbsp;</div>";
			    if (IsBrowserIE6() == true)
			    {
			        document.getElementById("a_change").style.visibility ="";
			        document.getElementById("a_none").style.visibility ="";
			    }
			    else
			    {
			        document.getElementById("a_change").style.visibility ="visible";
			        document.getElementById("a_none").style.visibility ="visible";
			    }
			    document.getElementById("txtfolderid").value = folderid;
		    }

		    function UnSelectContent()
		    {
			    document.getElementById("contentidspan").innerHTML = "<div id=\"div3\" style=\"display: none;position: block;\"></div><div id=\"contentidspan\" style=\"display: block;position: block;\">" + "<a href=\"#\" onclick=\"LoadChildPage();return false;\"><%= m_refMsg.GetMessage("lbl select")%></a></div>";
			    document.getElementById("a_change").style.visibility ="hidden";
			    document.getElementById("a_none").style.visibility="hidden";
		        document.getElementById("contentidspan").value = "";

			    var objLanguage=document.getElementById("language");

			    if (("object"==typeof(objLanguage)) && (objLanguage!= null))
			    {
				    if (objLanguage.disabled==true) {objLanguage.disabled=false;}
			    }
		    }

		    function CloseChildPage()
		    {
			    var pageObj = document.getElementById("FrameContainer");
			    pageObj.style.display = "none";
			    pageObj.style.width = "1px";
			    pageObj.style.height = "1px";
		    }

		    function IsChildWaiting() {
			    var pageObj = document.getElementById("FrameContainer");
			    if (pageObj == null) {
				    return (false);
			    }
			    if (pageObj.style.display == "") {
				    return (true);
			    }
			    else {
				    return (false);
			    }
		    }
		    function IsBrowserIE6()
		    {
			    if (IsBrowserIE() == true)
			    {
			        if (navigator.userAgent.indexOf("MSIE 6.0") > -1)
			        {
			            return true;
			        }
			    }
			    else
			    {
			        return false;
			    }
		    }
		    function IsBrowserIE()
		    {
			    // document.all is an IE only property
			    return (document.all ? true : false);
		    }
		    function trim(str)
            {
               if ("string" == typeof str)
               {
                    return str.replace(/^\s*|\s*$/g,"");
               }
            }
		    function ShowPane(tabID)
		    {
			    var arTab = new Array("dvNewContent", "dvRelatedContent");
			    var dvShow; //tab
			    var _dvShow; //pane
			    var dvHide;
			    var _dvHide;

			    for (var i=0; i < arTab.length; i++) {
			        if (tabID == arTab[1])
			        {
			            CloseChildPage();
			        }
				    if (tabID == arTab[i]) {
					    dvShow = eval('document.getElementById("' + arTab[i] + '");');
					    _dvShow = eval('document.getElementById("_' + arTab[i] + '");');
				    } else {

					    dvHide = eval('document.getElementById("' + arTab[i] + '");');
					    if (dvHide != null) {
						    dvHide.className = "tab_disabled";
					    }
					    _dvHide = eval('document.getElementById("_' + arTab[i] + '");');
					    if (_dvHide != null) {
						    _dvHide.style.display = "none";
					    }
				    }
			    }
			    _dvShow.style.display = "block";
			    dvShow.className = "tab_actived";
		    }

            function create_Object(){
                var xmlhttp;
                // This if condition for Firefox and Opera Browsers
                if (!xmlhttp && typeof XMLHttpRequest != 'undefined')
                {
                    try
                    {
                        xmlhttp = new XMLHttpRequest();
                    }
                    catch (e)
                    {
                        alert('<%=m_refMsg.GetMessage("js xmlhttprequest not supported")%>');
                        xmlhttp = false;
                    }
                }
                else
                {
                    xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
                }
                return xmlhttp;
            }

            var request = create_Object();

            function getContentByID() {
                var selectedId = '';
                if (selectedContentId > 0)
                {
                    selectedId = '&selectedid=' + selectedContentId;
                }
                request.open("GET", "wikilinksearch.aspx?cid=" + contentId + "&text=" + searchText + "&pnum=" + pageNum + selectedId, true);
                request.onreadystatechange = updatePage;
                request.send(null);
            }

            function wiki_search()
            {
                searchText = document.getElementById('SearchBox').value;
                getContentByID();
            }


            function updatePage()
            {

                if (request.readyState == 4) {
                    if (request.status == 200) {
                        var html = request.responseText;
                        try
                        {
                            var pos1 = html.indexOf("<totalPages>");
                            var pos2 = html.indexOf("</totalPages>");
                            if (pos1 > -1)
                            {
                                TOTAL_PAGES = html.substring(pos1 + "<totalPages>".length, pos2);
                            }
                            pos1 = html.indexOf("<content>");
                            pos2 = html.indexOf("</content>");
                            document.getElementById('dvResults').innerHTML = html.substring(pos1 + "<content>".length, pos2);
                            InitPagesNavLinks();
                        }
                        catch (e)
                        {
                            alert(e.message);
                        }
                        Ektron.Workarea.Grids.show();
                    }
                }
            }

            function FirstPage() {
                pageNum = 1;
                getContentByID();
            }
            function PreviousPage() {
                if (pageNum > 1)
                {
                    pageNum--;
                }
                getContentByID();
            }
            function NextPage() {
                if (pageNum < TOTAL_PAGES)
                {
                    pageNum++;
                }
                getContentByID();
            }
            function LastPage() {
                pageNum = TOTAL_PAGES;
                getContentByID();
            }
            var oFirstPage = null;
            var oPreviousPage = null;
            var oNextPage = null;
            var oLastPage = null;
            function InitPagesNavLinks()
            {
                GetPageNavFields();
                oFirstPage.className = 'but_first_disable';
                oPreviousPage.className = 'but_previous_disable';
                oNextPage.className = 'but_next_disable';
                oLastPage.className = 'but_last_disable';

                if (pageNum != TOTAL_PAGES)
                {
                    oLastPage.className = 'but_last';
                    oNextPage.className = 'but_next';
                }
                if (pageNum != 1)
                {
                    oFirstPage.className = 'but_first';
                    oPreviousPage.className = 'but_previous';
                }
            }
            function GetPageNavFields()
            {
                if (oFirstPage == null)
                    oFirstPage = document.getElementById( 'first' );
                if (oPreviousPage == null)
                    oPreviousPage = document.getElementById( 'previous' );
                if (oNextPage == null)
                    oNextPage = document.getElementById( 'next' );
                if (oLastPage == null)
                    oLastPage = document.getElementById( 'last' );

            }
            function SelectContent(elemId, link)
            {
                var elem = null;
                elem = document.getElementById( elemId );
                if (elem != null)
                {
                    elem.checked = true;
                    contentLink = link;
                }
                return false;
            }
            function GetContentLink()
            {
                var link = '';
                var tab = null;
			    tab = document.getElementById( "_dvRelatedContent" );
			    if (tab != null) {
			        if (tab.className.indexOf('hide') == -1)
			        {
			            return contentLink;
			        }
			    }
			    return "";
            }
	    //-->
        </script>



    <style type="text/css">
        .highlight {
            background-color:yellow;
        }

        .but_first a:link,
        .but_first a:visited,
        .but_first a:hover {
            padding:0px 2px 0px 2px;
            text-indent:-5000px;
            display:block;
            height:20px;
            width:30px;
        }

        .but_first {
            float:left;
            display:block;
            height:20px;
            width:30px;
            background-image:url(../images/application/but_first.gif);
            background-repeat:no-repeat;
            background-position:center center;
        }

        .but_first_disable {
            float:left;
            text-indent:-5000px;
            display:block;
            height:20px;
            width:30px;
            background-image:url(../images/application/but_first_d.gif);
            background-repeat:no-repeat;
            background-position:center center;
        }

        .but_previous a:link,
        .but_previous a:visited,
        .but_previous a:hover {
            padding:0px 2px 0px 2px;
            text-indent:-5000px;
            display:block;
            height:20px;
            width:24px;
        }

        .but_previous {
            float:left;
            display:block;
            height:20px;
            width:24px;
            background-image:url(../images/application/but_prev.gif);
            background-repeat:no-repeat;
            background-position:center center;
        }

        .but_previous_disable {
            float:left;
            text-indent:-5000px;
            display:block;
            height:20px;
            width:24px;
            background-image:url(../images/application/but_prev_d.gif);
            background-repeat:no-repeat;
            background-position:center center;
        }

        .but_next a:link,
        .but_next a:visited,
        .but_next a:hover {
            display:block;
            height:20px;
            width:24px;
            text-indent:-5000px;
            padding:0px 2px 0px 2px;
            text-indent:-5000px;
        }

        .but_next {
            float:left;
            display:block;
            height:20px;
            width:24px;
            margin-left:5px;
            background-image:url(../images/application/but_next.gif);
            background-repeat:no-repeat;
            background-position:center center;
        }

        .but_next_disable {
            float:left;
            text-indent:-5000px;
            display:block;
            height:20px;
            width:24px;
            margin-left:5px;
            background-image:url(../images/application/but_next_d.gif);
            background-repeat:no-repeat;
            background-position:center center;
        }


        .but_last a:link,
        .but_last a:visited,
        .but_last a:hover {
            padding:0px 2px 0px 2px;
            display:block;
            height:20px;
            width:30px;
            text-indent:-5000px;
        }

        .but_last {
            float:left;
            display:block;
            height:20px;
            width:30px;
            background-image:url(../images/application/but_last.gif);
            background-repeat:no-repeat;
            background-position:center center;
        }

        .but_last_disable {
            float:left;
            text-indent:-5000px;
            display:block;
            height:20px;
            width:30px;
            background-image:url(../images/application/but_last_d.gif);
            background-repeat:no-repeat;
            background-position:center center;
        }
        p, li, div
        {
            margin:0in;
            margin-bottom:.0001pt;
            font-size:10.0pt;
            font-family:Verdana, Geneva, Arial, Helvetica, sans-serif;
        }
        .title
        {
            font-weight:bold;
        }
    </style>
    </asp:PlaceHolder>
</head>
<body onload="window.focus();">
    <form id="frmHyperlinks" method="post" runat="server" name="frmHyperlinks">
        <asp:Literal ID="StyleSheetJS" runat="server"></asp:Literal>
        <div id="FrameContainer" style="position: absolute; top: 15px; left: 10px; width: 1px;
            height: 1px; display: none; z-index:100;">
            <iframe id="ChildPage" name="ChildPage" frameborder="yes" marginheight="2" marginwidth="2"
                width="100%" height="100%" scrolling="auto"></iframe>
        </div>
        <div id="dhtmltooltip"></div>
            <div class="ektronPageHeader">
                <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
                <div class="ektronToolbar" id="divToolBar" runat="server"></div>
            </div>
        <div class="ektronPageContainer ektronPageTabbed">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li>
                            <a href="#_dvNewContent" title="New Content Tab">
                                <asp:Literal ID="divNewContentText" runat="server">New Content</asp:Literal>
                            </a>
                        </li>
                        <li>
                            <a href="#_dvRelatedContent" title="Related Content Tab">
                                <asp:Literal ID="divdvRelatedContentText" runat="server">Related Content</asp:Literal>
                            </a>
                        </li>
                    </ul>
                    <div style="width: 95%; height: 95%; padding:10px;" id="_dvNewContent">
                        <table class="ektronForm" width="100%" height="100%">
                            <tr>
                                <td valign="top" height="100%" id="divContentHtml" runat="server">
                                    <table>
                                        <tr>
                                           <td class="label" title="Folder">
                                                <%=m_refMsg.GetMessage("lbl folder")%>:
                                            </td>
                                            <td>
                                                <div id="Div3" style="display: none;">
                                                </div>
                                                <div id="contentidspan" style="display: block;">
                                                    <a href="#" onclick="LoadChildPage();return false;" title="Select"><%=m_refMsg.GetMessage("lbl select")%></a></div>
                                                <a href="#" title="Change" id="a_change" name="a_change" style="visibility: hidden;" onclick="LoadChildPage();return false;">
                                                    <%=m_refMsg.GetMessage("btn change")%></a>&nbsp;&nbsp;<a href="#" title="Default" id="a_none" name="a_none" style="visibility: hidden;" onclick="UnSelectContent();return false;"><%=m_refMsg.GetMessage("lbl default")%></a>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label" title="Article Title"><asp:Literal ID="ltContentTitle" runat="server">Article Title:</asp:Literal></td>
                                            <td><input title="Enter Article Title Text" type="text" name="wiki_cont_title" maxlength="255" size="22" id="wiki_cont_title" /></td>
                                        </tr>
                                        <tr>
                                            <td class="label" title="Target Frame">
                                                <%=m_refMsg.GetMessage("lbl target frame")%>:</td>
                                            <td>
                                                <select id="Target" name="Target" title="Target Frame Drop Down Menu">
                                                    <option value="" selected></option>
                                                    <option value="_blank" title="New Window (_blank)"><%=m_refMsg.GetMessage("dropdown new win")%></option>
                                                    <option value="_self" title="Same Window (_self)"><%=m_refMsg.GetMessage("dropdown same win")%></option>
                                                    <option value="_parent" title="Parent Window (_parent)"><%=m_refMsg.GetMessage("dropdown parent win")%></option>
                                                    <option value="_top" title="Browser Window (_top)"><%=m_refMsg.GetMessage("dropdown browser win")%></option>
                                                </select>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <input type="hidden" id="txtCategory" name="txtCategory" size="60" maxlength="1000" value="" />
                        <input type="hidden" id="text2" name="text2" value="" />
                        <input type="hidden" id="txtfolderid" name="txtfolderid" value="" />
                    </div>
                    <div style="width: 95%; height: 95%; padding:10px;" id="_dvRelatedContent">
                        <strong title="Search"><%=m_refMsg.GetMessage("res_isrch_btn")%>:&nbsp;</strong><input title="Enter Search Text" type="text" runat="server" name="SearchBox" id="SearchBox" />
                        <asp:Button ToolTip="Search" ID="searchButton" runat="server" Text="go!" UseSubmitBehavior="true"  OnClientClick="wiki_search(); return false;" />

                        <div id="dvResults"><img src="../csslib/loading.gif" alt="loading..." /></div>
                        <div style="width:100%">
                            <span class="but_first_disable" id="first"><a href="#" id="btn_first" onclick="FirstPage();return false;" title="<%= m_refMsg.GetMessage("lbl first page")%>"></a></span> <span class="but_previous_disable" id="previous"><a href="#" id="btn_previous" onclick="PreviousPage();return false;" title="<%= m_refMsg.GetMessage("lbl previous page")%>"></a></span>
                            <span class="but_next" id="next"><a href="#" id="btn_next" onclick="NextPage();return false;" title="<%= m_refMsg.GetMessage("lbl next page")%>"></a></span>
                            <span class="but_last" id="last"><a href="#" id="btn_last" onclick="LastPage();return false;" title="<%= m_refMsg.GetMessage("lbl last page")%>"></a></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <asp:Literal ID="jsSearchRelatedContent" runat="server"></asp:Literal>
        <script type="text/javascript">

            if (wiki_cont_title == '')
            {
                wiki_cont_title = searchText;
            }
            document.getElementById( 'wiki_cont_title' ).value = wiki_cont_title;
            document.getElementById( 'SearchBox' ).value = wiki_cont_title;

            function SelectTarget(value)
            {
                var elSel = document.getElementById('Target');
                var i;
                for (i = elSel.length - 1; i>=0; i--) {
                    if (elSel.options[i].value == value) {
                      elSel.options[i].selected = true;
                    }
                }
            }
            if (wiki_link_target == '')
            {
                wiki_link_target = "_self";
            }
            SelectTarget(wiki_link_target);
        </script>
    </form>
</body>
</html>

