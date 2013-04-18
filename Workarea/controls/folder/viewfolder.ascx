<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewfolder" CodeFile="viewfolder.ascx.cs" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<style type="text/css">
    div#EkTB_ajaxWindowTitle { padding: 0;}
</style>
<script type="text/javascript">
    <!--//--><![CDATA[//><!--

	    var url_id="<asp:Literal ID="url_id" Runat="server"/>";
	    var url_action="<asp:Literal ID="url_action" Runat="server"/>";
	    var is_archived="<asp:Literal ID="is_archived" Runat="server"/>"
	    
	    // Setting these JS variables in the user control, 
	    // and ultimately setting them in Ektron.Workarea.FolderContext object, 
	    // for accessing through out the content.aspx page.
        var pasteFolderType = "<asp:Literal runat="server" id="pasteFolderType"/>";
        var pasteFolderId = "<asp:Literal runat="server" id="pasteFolderId"/>";
        var pasteParentId = "<asp:Literal runat="server" id="pasteParentId"/>";
	    <asp:literal id="ltr_js" runat="server" />
        
	    function LoadLanguage(num){
	        top.notifyLanguageSwitch(num, url_id);
		    document.forms[0].action="content.aspx?id="+url_id+"&action="+url_action+"&LangType="+num+"&IsArchivedEvent="+is_archived;
		    document.forms[0].submit();
		    return false;
	    }
	    function AddNewEvent()
	    {
	        var contentLang = parseInt(folderjslanguage, 10);
	        if(typeof(contentLang) != 'number'){
	            contentLang = parseInt(jsContentLanguage, 10);
	        }
            var multiSupport = jsEnableMultilingual;
            if ((contentLang < 1) && multiSupport)
            {
                bContinue = confirm("Do you wish to add an event in the default language?");
                if (bContinue){
                    contentLang = jsDefaultContentLanguage;
        	        top.notifyLanguageSwitch(contentLang);
                }else{
                    return;
                }
            }
            if(contentLang > 1){
		        self.location.href = "content.aspx?id="+url_id+"&action="+url_action+"&LangType="+contentLang+"&showAddEventForm=true";
		    }else{
		        self.location.href = "content.aspx?id="+url_id+"&action="+url_action+"&LangType="+jsDefaultContentLanguage+"&showAddEventForm=true";
		    }
	    }
	    function AddNewTopic()
	    {
	        var contentLang = parseInt(folderjslanguage, 10);
            var multiSupport = jsEnableMultilingual;
            if ((contentLang < 1) && multiSupport)
            {
                bContinue = confirm("Do you wish to add topic in the default language?");
                if (bContinue){
                    contentLang = jsDefaultContentLanguage;
        	        top.notifyLanguageSwitch(contentLang);
                }
            }
            if(contentLang > 1){
		        self.location.href = "threadeddisc/addedittopic.aspx?action=add&id="+url_id;
		    }
	    }
	    function AddNewPage()
	    {
	        var contentLang = parseInt(jsContentLanguage, 10);
            var multiSupport = jsEnableMultilingual;
            if ((contentLang < 1) && multiSupport)
            {
                bContinue = confirm("Do you wish to add page in the default language?");
                if (bContinue){
                    return jsDefaultContentLanguage;
                }
            }
            return contentLang;
	    }
	    function AddNewContent(payload, ContType) {
		    var bContinue = true;
		    if (typeof ContType != "undefined")
		    {
			    // add multiple
			    payload = "<%= _ContentTypeUrlParam %>=" + ContType + "&" + payload;
		    }
		    else
		    {
			    // add single
			    if (null == objSelSupertype)
			    {
				    payload = "<%= _ContentTypeUrlParam %>=" + <%=_CMSContentType_AllTypes%> + "&" + payload;
			    }
			    else
			    {
				    ContType = objSelSupertype.value;
				    payload = "<%= _ContentTypeUrlParam %>=" + ContType + "&" + payload;
				    if (<%=_CMSContentType_AllTypes%> == ContType)
				    {
					    bContinue = confirm("Do you wish to add HTML content?");
				    }
			    }
		    }
		    if (bContinue)
		    {
		        // when the workarea is first opened, jsContentLanguage is invalid, so we have to use this control's version
			    var contentLang = parseInt(folderjslanguage, 10);
			    var multiSupport = jsEnableMultilingual;
			    if ((contentLang < 1) && multiSupport)
			    {
				    bContinue = confirm('<asp:literal id="addContentLanguageMessage" runat="server" />');
				    if (bContinue){
					    // force language to default:
					    payload = replaceAll(payload, 'LangType=-1&', 'LangType=' + jsDefaultContentLanguage + '&');
					    payload = replaceAll(payload, 'LangType=0&', 'LangType=' + jsDefaultContentLanguage + '&');
				    }
			    }
		    }

		    if (bContinue)
		    {
		        if(ContType==2){
				    // FireFox fix (the browser thinks it is already at the
				    // target location. IE will obediently jump to the same
				    // location but FF/NS won't, so we need to make them see
				    // a difference. Only an issue with forms type content):

				    var objTop=top.document.getElementById("ek_main");

				    if (("object"==typeof(objTop)) && (objTop!= null))
				    {
					    top.document.getElementById('ek_main').src = '';
					    top.document.getElementById('ek_main').src = 'cmsform.aspx?action=Addform&' + payload;
				     }
				     else
				     {
					    self.location.href = 'cmsform.aspx?action=Addform&' + payload;
				     }
		        }
		        else if(ContType == 9876)
		        {
		            var objTop=top.document.getElementById("ek_main");

				    if (("object"==typeof(objTop)) && (objTop!= null))
				    {
					    top.document.getElementById('ek_main').src = 'edit.aspx?close=false&type=multiple&' + payload;
				     }
				     else
				     {
					    self.location.href =  'edit.aspx?close=false&type=multiple&' + payload;
				     }
		        }
                else if(ContType == 9875)
		        {
		            var objTop=top.document.getElementById("ek_main");

				    if (("object"==typeof(objTop)) && (objTop!= null))
				    {
					    top.document.getElementById('ek_main').src = 'stsverselect.aspx?close=false&type=multiple&' + payload;
				     }
				     else
				     {
					    self.location.href =  'edit.aspx?close=false&type=multiple&' + payload;
				     }
		        }
		        else if(ContType == 3333){

				    var objTop=top.document.getElementById("ek_main");

				    if (("object"==typeof(objTop)) && (objTop!= null))
				    {
					    top.document.getElementById('ek_main').src = 'commerce/CatalogEntry.aspx?close=false&' + payload;
				     }
				     else
				     {
					    self.location.href =  'commerce/CatalogEntry.aspx?close=false&' + payload;
				     }
		        }
		        else{

				    var objTop=top.document.getElementById("ek_main");

				    if (("object"==typeof(objTop)) && (objTop!= null))
				    {
					    top.document.getElementById('ek_main').src = 'edit.aspx?close=false&' + payload;
				     }
				     else
				     {
					    self.location.href =  'edit.aspx?close=false&' + payload;
				     }
		        }
		    }
		    return false;
	    }

	    function replaceAll(inStr, searchStr, replaceStr){
		    var retStr = inStr;
		    var index = retStr.indexOf(searchStr);
		    while(index>=0){
			    retStr = retStr.replace(searchStr, replaceStr);
			    index = retStr.indexOf(searchStr);
		    }
		    return (retStr);
	    }

	    // Adjusts the navigation-tree frame (if function exists; ie workarea).
	    // (True Shows the nav-tree, False hides it)
	    function ResizeFrame(val) {
		    if ((typeof(top.ResizeFrame) == "function") && top != self) {
			    top.ResizeFrame(val);
		    }
	    }

	    window.EditorCleanup = function(){
            try {
                if (window != null) {
                    if (typeof window.RadEditorGlobalArray != 'undefined') {
                        var length = window.RadEditorGlobalArray.length;
                        if (length > 0) {
                            for (var i = 0; i < length; i++) {
                                if (document.getElementById(window.RadEditorGlobalArray[i].Id) == null) {
                                    window.RadEditorGlobalArray.splice(i, 1);
                                    length--;
                                    i--;
                                }
                            }
                        }
                    }
                }
            } catch (ex) { }
	    }
	//--><!]]>
	 $ektron(document).ready(function()
        {
            $ektron('#DmsMenu').hover(function(){
                $("#file").css("visibility", "hidden");
                $("#view").css("visibility", "hidden");
                $("#delete").css("visibility", "hidden");
                $("#action").css("visibility", "hidden");
            });
        });	
</script>

<style type="text/css">
    </style>
<script type="text/javascript">
    Ektron.ready(function() 
    {
        $("#ReplyDesc" + " a").click(function() {
            this.blur();
            alert('<asp:Literal id="errorLinksDisabled" runat="server" />');
            return false;
        });
        
        //Start: Event Binding for Drag and Select various rows of the content screen.
        
        var container = $ektron(".ektronContextMenuSelect").filter(":not(:has(#calendardisplay))");
      
        var urlPathName = window.location.href;
        
        if(urlPathName.indexOf("action=ViewContentByCategory") != -1 || urlPathName.indexOf("action=viewcontentbycategory") != -1)
        {
            container.bind("mousedown", function(e)
            {
                var ctrl = false;
                var meta = false;
                var clickType = 1;
                if (e.which)
                {
                    clickType = e.which;
                }
                else
                {
                    clickType = event.button;
                }
                ctrl = e.ctrlKey;
                meta = e.metaKey;
//TODO: UX group to check highlighting is OK when selecting multiple row using mouse button down                
//                document.onselectstart = function()
//                {
//                    return false;
//                };

//                document.onmousedown = function()
//                {
//                    return false;
//                };
                // only bind to the left mouse button (clickType == 1)
                if (clickType == 1)
                {
                    //Drag and Select content items.
                    if(!ctrl && !meta)
                    {
                        Ektron.ContentContextMenu.SetClassSelected(e);
                    }
                    //Press + Hold Control key and select individual items.
                    else
                    {
                        Ektron.ContentContextMenu.ToggleClass(e);
                    }
                    var parentTR = $ektron(e.target).closest("tr:not(.title-header)");
                    parentTR.addClass("selected");
                }

            });

            container.bind("mouseup", function(e)
            {
                var clickType = 2;
                if (e.which)
                {
                    clickType = e.which;
                }
                else
                {
                    clickType = event.button;
                }
                var selectedRows = $ektron(Ektron.ContentContextMenu.Content.selectedRows);
                if (selectedRows.length > 0 && clickType == 1)
                {
                    //Context Menu Initialization.
                    Ektron.ContentContextMenu.Init();
                }
                $ektron("#viewfolder_FolderDataGrid").find("tr").unbind("mouseenter");
                Ektron.ContentContextMenu.lastSelected = null;
            });
        }
        //End: Event Binding for Drag and Select various rows of the content screen.
    });
    $ektron.addLoadEvent(function () {
        if ("undefined" !== typeof (top.Ektron.Workarea.FolderContext)) {
            top.Ektron.Workarea.FolderContext.folderType = pasteFolderType;
            top.Ektron.Workarea.FolderContext.folderId = pasteFolderId;
            top.Ektron.Workarea.FolderContext.folderParentId = pasteParentId;
        }
    });
</script>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<%--The class "ektronContextMenuSelect" has been added to only target the view folder grid for the drag and select various contents for cut/copy functionality.--%>
<div class="ektronPageContainer ektronPageGrid ektronContextMenuSelect">
    <asp:DataGrid ID="FolderDataGrid"
            CssClass="ektronGrid"
            runat="server"
            AllowPaging="False"
            AllowCustomPaging="True"
            AutoGenerateColumns="False"
            EnableViewState="False">
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
    <div class="paging" id="divPaging" runat="server" visible="false">
        <p class="adHoc">
            <span class="page" title="Page"><asp:Literal ID="litPage" runat="server" /></span>
            <span class="pageNumber"><asp:TextBox CssClass="currentPage" ID="currentPage" runat="server"></asp:TextBox></span>
            <span class="pageOf" title="of"><asp:Literal ID="litOf" runat="server" /></span>
            <input type="hidden" runat="server" name="hdnCurrentPage" value="hidden" id="hdnCurrentPage" />
            <span class="pageTotal"><asp:Literal ID="totalPages" runat="server" /></span>&#160;|&#160;<asp:LinkButton ID="lbPageGo" CssClass="adHocPage" runat="server" OnCommand="AdHocPaging_Click" CommandName="AdHocPage" />
        </p>
        <ul class="direct">
            <li><asp:ImageButton ID="ibFirstPage" runat="server" OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" /></li>
            <li><asp:ImageButton ID="ibPreviousPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" /></li>
            <li><asp:ImageButton ID="ibNextPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" /></li>
            <li>
                <asp:ImageButton ID="ibLastPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
                <asp:HiddenField ID="hdnTotalPages" runat="server" />
            </li>
        </ul>
    </div>
    <CMS:WebCalendar runat="server" ID="calendardisplay" Visible="false" UseUpdatePanel="false" EnableViewState="false"></CMS:WebCalendar>

    <asp:Panel ID="pnlThreadedDiscussions" runat="server" Visible="False">
        <table class="ektronGrid">
            <tbody>
                <tr class="title-header">
                    <td colspan="2" class="title-header left"><%=_MessageHelper.GetMessage("lbl Forum")%></td>
                    <td class="title-header"><%=_MessageHelper.GetMessage("lbl discussionforumtopics")%></td>
                    <td class="title-header"><%=_MessageHelper.GetMessage("lbl Posts")%></td>
                    <td class="title-header"><%=_MessageHelper.GetMessage("lbl Last Post")%></td>
                </tr>
                <asp:repeater id="CategoryList" runat="server" OnItemDataBound="CategoryList_ItemDataBound">
                    <ItemTemplate>
                        <tr class="ektronSubjectHeader">
                            <td colspan="5" class="left">
                                <%#DataBinder.Eval(Container.DataItem, "name")%>
                                <input type="hidden" runat="server" id="hdn_categoryid" value='<%#DataBinder.Eval(Container.DataItem, "id")%>' />
                            </td>
                        </tr>
                        <asp:Repeater id="ForumList" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td style="width:16px;"><img alt="" src="<%=_ContentApi.AppPath + "images/ui/icons/folderBoard.png" %>" /></td>
                                    <td class="left"><%#DataBinder.Eval(Container.DataItem, "Name")%><br /><%# DataBinder.Eval(Container.DataItem, "Description") %></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "Topics")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "Posts")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "LastPosted")%></td>
                                </tr>
                            </ItemTemplate>
                            <AlternatingItemTemplate>
                                <tr class="stripe">
                                    <td style="width:16px;"><img alt="" src="<%=_ContentApi.AppPath + "images/ui/icons/folderBoard.png" %>" /></td>
                                    <td class="left"><%#DataBinder.Eval(Container.DataItem, "Name")%><br /><%# DataBinder.Eval(Container.DataItem, "Description") %></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "Topics")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "Posts")%></td>
                                    <td align="center"><%#DataBinder.Eval(Container.DataItem, "LastPosted")%></td>
                                </tr>
                            </AlternatingItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:repeater>
            </tbody>
        </table>
    </asp:Panel>
    
    <input type="hidden" runat="server" id="hdnIsPostData" value="true" class="isPostData" name="isPostData" />
    
    <asp:Literal ID="dropuploader" runat="server" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
          function UpdateViewwithSubtype(AssetType, SubType,IsArchived){
	            var oForm = document.forms[0];
	            var lang = jsContentLanguage;
	            var objSelLang = document.getElementById('selLang');
	            if (objSelLang != null)
	            {
		            lang = objSelLang.value;
	            }
	            var strAction = "content.aspx?id=<%=Request.QueryString["id"]%>" + "&action=<%=Request.QueryString["action"]%>" + "&LangType=" + lang;
	            strAction += "&ContType=" + AssetType;
	            strAction += "&SubType=" + SubType;
	           if(AssetType == 1101 || AssetType == 1102 || AssetType == 1104 || AssetType == 1106 || IsArchived == true)
	            strAction += "&IsArchivedEvent=true";
                oForm.action = strAction;
	            oForm.submit();
	            return false;
            }
            
            function UpdateView(AssetType){
                return UpdateViewwithSubtype(AssetType, -1,false)
            }
             function UpdateArchiveView(AssetType,IsArchived){
                return UpdateViewwithSubtype(AssetType, -1,IsArchived)
            }


            function resetPostback()
            {
                $ektron("input.isPostData").attr("value", "");
            }

            function CheckWorkOfflineStatus(viewurl,editurl,assetid){
	            var objTop=top.document.getElementById("ek_main");

	            if (("object"==typeof(objTop)) && (objTop!= null))
	            {

  		            var dragDropFrame = top.GetEkDragDropObject();
		              if (dragDropFrame != null && dragDropFrame.IsInWorkOffline != undefined) {
			            if(dragDropFrame.IsInWorkOffline(assetid)){
				            if (!confirm("Click Ok to check-in your local file.\n Click Cancel to lose your changes.")) {
					            dragDropFrame.CancelWorkOffline(assetid)
					            document.forms[0].action=viewurl+"&cancelaction=undocheckout";
				            }else{
					            document.forms[0].action=editurl;
				            }
			            }else{
			                if(false == confirm("This content is already checked out to this user.\nIf you edit it at the same time in two locations, some edits may be lost or an error may occur.\nIf the document is open in another location, please close it before continuing.\nClick 'Ok' to Continue."))
	                            return;
	                        document.forms[0].action=viewurl;
			            }
		            }else{
		                 if(false == confirm("This content is already checked out to this user.\nIf you edit it at the same time in two locations, some edits may be lost or an error may occur.\nIf the document is open in another location, please close it before continuing.\nClick 'Ok' to Continue."))
	                            return;
	                     document.forms[0].action=viewurl;
		            }
		            document.forms[0].__VIEWSTATE.name = 'NOVIEWSTATE';
		            document.forms[0].submit();

	            }
            }
            ResizeFrame(1);
        //--><!]]>
    </script>
</div>
