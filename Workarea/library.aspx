<%@ Page Language="C#" AutoEventWireup="true" Inherits="library" ValidateRequest="false" CodeFile="library.aspx.cs" EnableEventValidation="false" %>
<%@ Register TagPrefix="ektron" TagName="ContentDesigner" Src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<%@ Register TagPrefix="uxEktron" TagName="Paging" Src="controls/paging/paging.ascx" %>
<%@ Reference Control="controls/library/librarytoolbar.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" <%=_Direction%>>
  <head runat="server">
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <title>Library</title>
	<meta http-equiv="Pragma" content="no-cache"/>
	<meta http-equiv="cache-control" content="no-store"/>
    <asp:literal id="StyleSheetJS" runat="server" />
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
        Ektron.ready( function()
        {
            var tabsContainers = $ektron(".tabContainer");
            tabsContainers.tabs();

            //Tag Modal
		    $ektron("#newTagNameDiv").modal(
            {
                trigger: '',
                modal: true,
                toTop: true,
                onShow: function(hash)
                {
                    hash.o.fadeIn();
                    hash.w.fadeIn();
                },
                onHide: function(hash)
                {
                    hash.w.fadeOut("fast");
                    hash.o.fadeOut("fast", function()
                    {
                        if (hash.o)
                        {
                            hash.o.remove();
                        }
                    });
                }
            });
        });
        //--><!]]>
    </script>
	<script type="text/javascript" id="EktronResourceTextJS">
	<!--//--><![CDATA[//><!-
		var ResourceText =
		{
			sDemoEktronComDetected : "<asp:literal id="sDemoEktronComDetected" runat="server"/>",
			sTitleRequired : "<asp:literal id="sTitleRequired" runat="server"/>",
			sTaxCatReq : "<asp:literal id="sTaxCatReq" runat="server"/>",
			sUrlLinkReq : "<asp:literal id="sUrlLinkReq" runat="server"/>",
			sFilenameReq : "<asp:literal id="sFilenameReq" runat="server"/>",
			sIdReq : "<asp:literal id="sIdReq" runat="server"/>",
			sDeleteLibItem : "<asp:literal id="sDeleteLibItem" runat="server"/>",
			sRemoveLibItem : "<asp:literal id="sRemoveLibItem" runat="server"/>",
			sValidPathReq : "<asp:literal id="sValidPathReq" runat="server"/>",
			sNoItemsSelected : "<asp:literal id="sNoItemsSelected" runat="server"/>",
			sLibPathDeletion : "<asp:literal id="sLibPathDeletion" runat="server"/>",
			sSupplyValidImagePath : "<asp:literal id="sSupplyValidImagePath" runat="server"/>",
			sSupplyValidFilePath : "<asp:literal id="sSupplyValidFilePath" runat="server"/>",
			sMissingLibPathStartSlash : "<asp:literal id="sMissingLibPathStartSlash" runat="server"/>"
		};
		var jsSelectedDivStyleClass="<asp:literal id="jsSelectedDivStyleClass" runat="server"/>";
		var jsUnSelectedDivStyleClass="<asp:literal id="jsUnSelectedDivStyleClass" runat="server"/>";
		var jsCategoryRequired ="<asp:literal id="jsCategoryrequired" runat="server" />";
        var CurrentFolderId="<asp:literal id="jsCurrentFolderId" runat="server"/>";
        var jsIsAjaxTree = <asp:literal id="jsIsAjaxTree" runat="server"/>;
        var MyUrl = "<asp:literal id="jsMyUrl" runat="server"/>";
        var jsDisableNavigation = "<asp:literal id="jsDisableNav" runat="server" text="false"/>";
	//--><!]]>
	</script>
	<script type="text/javascript">
		<!--//--><![CDATA[//><!--
		var m_jsIsMacNoIE = false;
		var m_fullScreenView = false;
		if (jsIsAjaxTree)
		{
		if(typeof(top["ek_nav_bottom"])!= 'undefined'){
            if(typeof(top["ek_nav_bottom"]["NavIframeContainer"])!= 'undefined'){
                if(typeof(top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"])!= 'undefined'){
		            if(typeof(top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["LibraryTree"])!= 'undefined'){
			            var treeobj=top["ek_nav_bottom"]["NavIframeContainer"]["nav_folder_area"]["LibraryTree"];
			            if(treeobj.document.getElementById("selected_folder_id")!=null){
				            var SelectedTreeId=treeobj.document.getElementById("selected_folder_id").value;
				            if(CurrentFolderId==0 && SelectedTreeId!=0) {
					            var stylenode = treeobj.document.getElementById( SelectedTreeId );
					            if(stylenode!=null){
						            stylenode.style["background"] = "#ffffff";
						            stylenode.style["color"] = "#000000";
						            var stylenode = treeobj.document.getElementById( CurrentFolderId );
						            if (stylenode != null)
						            {
							            stylenode.style["background"] = "#3366CC";
							            stylenode.style["color"] = "#ffffff";
						            }
					            }
				            }
				        }
			        }
		        }
		    }
		}
		} // jsIsAjaxTree


		function IsDemo() {
			if (MyUrl.indexOf("demo.ektron.com") != -1) {
				alert(ResourceText.sDemoEktronComDetected);
				return false;
			}
			return true;
		}

		function CheckLibraryForm(LibType) {
			regexp1 = /"/gi;
			document.forms.LibraryItem.frm_title.value = Trim(document.forms.LibraryItem.frm_title.value.replace(regexp1, "'"));
			document.forms.LibraryItem.frm_content_id.value = Trim(document.forms.LibraryItem.frm_content_id.value.replace(regexp1, "'"));
			if (document.forms.LibraryItem.frm_title.value == "") {
				alert(ResourceText.sTitleRequired);
				document.forms.LibraryItem.frm_title.focus();
				return false;
			}
			 if(LibType != "quicklinks" && LibType != "forms" && jsCategoryRequired=="true")
			{
                if(Trim(document.getElementById('taxonomyselectedtree').value) == '')
                {
                   alert(ResourceText.sTaxCatReq);
                   return false;
                }
			}
			if ((LibType == "quicklinks") || (LibType == "hyperlinks")) {
				document.forms.LibraryItem.frm_filename.value = Trim(document.forms.LibraryItem.frm_filename.value);
				if (document.forms.LibraryItem.frm_filename.value == "") {
					alert(ResourceText.sUrlLinkReq);
					document.forms.LibraryItem.frm_filename.focus();
					return false;
				}
			}
			else
			{
				var szTempFilename = Trim(document.forms.LibraryItem.frm_filename.value);
				if (szTempFilename == "") {
					alert(ResourceText.sFilenameReq);
					document.forms.LibraryItem.frm_filename.focus();
					return false;
				}
				else{
                		if(((szTempFilename.replace(/^.*\\/, '')).indexOf('#') > -1) || ((szTempFilename.replace(/^.*\\/, '')).indexOf('&') > -1) ||  ((szTempFilename.replace(/^.*\\/, '')).indexOf(';') > -1))
						{
		        		alert("A filename cannot contain '#','&',';'");
		        		document.forms.LibraryItem.frm_filename.focus();
					    return false;
						}
				}
			}
			if (LibType == "quicklinks") {
				document.forms.LibraryItem.frm_content_id.value = Trim(document.forms.LibraryItem.frm_content_id.value);
				if (document.forms.LibraryItem.frm_content_id.value == "") {
					alert(ResourceText.sIdReq);
					document.forms.LibraryItem.frm_content_id.focus();
					return false;
				}
			}
			return true;
		}

		function ConfirmLibraryDelete(formname, fieldname) {
			var msg = ResourceText.sDeleteLibItem;
			if (typeof(document.forms[formname]) != "undefined") {
				if (typeof(document.forms[formname][fieldname]) != "undefined") {
					if (document.forms[formname][fieldname].checked) {
						msg = msg + "\n\n";
						msg = msg + ResourceText.sRemoveLibItem;
						msg = msg + "\n\n";
					}
				}
			}
			return confirm(msg);
		}

		function CheckKeyValue(item, keys) {
			var keyArray = keys.split(",");
			for (var i = 0; i < keyArray.length; i++) {
				if ((document.layers) || ((!document.all) && (document.getElementById))) {
					if (item.which == keyArray[i]) {
						return false;
					}
				}
				else {
					if (event.keyCode == keyArray[i]) {
						return false;
					}
				}
			}
		}

		function VerifyLoadBalancePath() {
			var lbPath = document.forms[0].loadBalancePath;
			lbPath.value = Trim(lbPath.value);
			if (lbPath.value == "") {
				alert(ResourceText.sValidPathReq);
				lbPath.focus();
				return false;
			}
			if (!ConfirmPath(lbPath.value.substring(0,1), document.forms[0].MakeRelative)) {
				lbPath.focus();
				return false;
			}
			return IsDemo();
		}

		function VerifyLbPathDeletion() {
			var formObj = document.forms[0];
			var selected = false;
			var iLoop = 0;
			for(iLoop = 0; iLoop < formObj.length; iLoop++) {
				if (formObj(iLoop).type.toLowerCase() == "checkbox") {
					if (formObj(iLoop).name.substring(0, "makerelative_".length).toLowerCase() == "makerelative_") {
						if (formObj(iLoop).checked) {
							selected = true;
							break;
						}
					}
				}
			}
			if (selected == false) {
				var msg = ResourceText.sNoItemsSelected;
				alert(msg);
				return false;
			}
			var msg = ResourceText.sLibPathDeletion;
			if (!confirm(msg)) {
				return false;
			}
			return IsDemo();
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


		function trim(text){
			while (' ' == text.charAt(0)) {
				text = text.subString(1);
			}
			while (' ' == text.charAt(text.length-1)) {
				text = text.subString(0, text.length-1);
			}
			return (text);
		}

		function FinallySubmitForm()
		{
			try {
				document.forms[0].submit();
			}
			catch(e) {
				alert(e.message + '\nCheck the file name and upload again.');
			}
		}
		function SubmitForm(FormName, Validate, ReqVal)
		{
			if(ReqVal == true)
			{
				if (!ValidateMeta(FormName))
				{
					return false;
				}
			}
			if (Validate.length > 0)
			{
				if (eval(Validate))
				{
					FinallySubmitForm();
					return false;
				}
				else
				{
					return false;
				}
			}
			else
			{
				FinallySubmitForm();
				return false;
			}
		}

		function TryReload()
		{
			if (top.HasReloaded())
			{
				top.ResetReload();
			}
			else
			{
				top.ReloadMe();
			}
		}

		function CheckLibSettings()
		{
			var msg = "";
			var imgDir = document.forms[0].imagedirectory;
			var imgExt = document.forms[0].imageextensions;
			var fileDir = document.forms[0].filedirectory;
			var fileExt = document.forms[0].fileextensions;
			imgDir.value = Trim(imgDir.value);
			imgExt.value = Trim(imgExt.value);
			fileDir.value = Trim(fileDir.value);
			fileExt.value = Trim(fileExt.value);
			if (imgExt.value.length != 0)
			{
				if ((imgDir.value.length == 0) && (document.forms[0].relativeimages.checked == false))
				{
					msg = ResourceText.sSupplyValidImagePath;
					alert(msg);
					imgDir.focus();
					return false;
				}
			}
			if (fileExt.value.length != 0)
			{
				if ((fileDir.value.length == 0) && (document.forms[0].relativefiles.checked == false))
				{
					msg = ResourceText.sSupplyValidFilePath;
					alert(msg);
					fileDir.focus();
					return false;
				}
			}
			if (!ConfirmPath(imgDir.value.substring(0,1), document.forms[0].relativeimages))
			{
				imgDir.focus();
				return false;
			}

			if (!ConfirmPath(fileDir.value.substring(0,1), document.forms[0].relativefiles))
			{
				fileDir.focus();
				return false;
			}
			return (IsDemo());
		}

		function ConfirmPath(firstChar, relativeObj)
		{
			msg = ResourceText.sMissingLibPathStartSlash;
			if (((firstChar != "/") && (firstChar != "\\")) && (relativeObj.checked == false))
			{
				return(confirm(msg));
			}
			return true;
		}
		function CheckFileType1()
		{
			CheckFileType('<asp:literal id="jsOperation" runat="server"/>');
		}
		function VerifyOverwriteSubmission1()
		{
			VerifyOverwriteSubmission('<asp:literal id="jsType" runat="server"/>');
		}

		function ShowPane(tabID,action)
            {
                if(action == "add")
	                var arTab = new Array("adddvSummary","adddvMetadata","adddvCategory");
    			else
				    var arTab = new Array("editdvSummary","editdvMetadata","editdvCategory");

		                var dvShow; //tab
		                var _dvShow; //pane
		                var dvHide;
		                var _dvHide;

		                for (var i=0; i < arTab.length; i++)
		                 {
			                if (tabID == arTab[i])
			                  {
				                dvShow = eval('document.getElementById("' + arTab[i] + '");');
				                 if(arTab[i] == "adddvMetadata" || arTab[i] == "editdvMetadata")
				                    _dvShow = eval('document.getElementById("_dvMetadata");');
				                 else if(arTab[i] == "adddvCategory" || arTab[i] == "editdvCategory")
				                    _dvShow = eval('document.getElementById("_dvCategory");');
				                 else if(arTab[i] == "adddvSummary" || arTab[i] == "editdvSummary")
				                    _dvShow = eval('document.getElementById("_dvSummary");');
				                 else
				                    _dvShow = eval('document.getElementById("_' + arTab[i] + '");');

			                  }
			                 else
			                  {
				                    dvHide = eval('document.getElementById("' + arTab[i] + '");');
				                    if (dvHide != null)
				                     {
					                    dvHide.className = "tab_disabled";
				                     }

				                     if(arTab[i] == "adddvMetadata" ||arTab[i] == "editdvMetadata")
				                        _dvHide = eval('document.getElementById("_dvMetadata");');
				                     else if(arTab[i] == "adddvCategory" || arTab[i] == "editdvCategory")
				                        _dvHide = eval('document.getElementById("_dvCategory");');
				                     else if(arTab[i] == "adddvSummary" || arTab[i] == "editdvSummary")
				                        _dvHide = eval('document.getElementById("_dvSummary");');
				                     else
				                        _dvHide = eval('document.getElementById("_' + arTab[i] + '");');

				                    if (_dvHide != null)
				                     {
					                    _dvHide.className = jsUnSelectedDivStyleClass;
				                     }
			                  }
		                }
		                _dvShow.className =  jsSelectedDivStyleClass;
		                dvShow.className = "tab_actived";
            }

            function IsBrowserIE()
            {
               return (document.all ? true : false); //document.all is an IE only property. So if present the its IE
            }
            function pageLoaded()
            {
                setTimeout("showSelectedFolderTree();", 100);
            }
	        function CanNavigate() {
		        // Block navigation when editing library item
		        return (jsDisableNavigation == "false");
	        }
	        function CanShowNavTree() {
		        // Block navigation when editing library item
		        return (jsDisableNavigation == "false");
	        }
	//--><!]]>
	</script>
	<style type="text/css">
	    .addFile{ color:#1D5987; font-weight:bold; text-align:right; white-space:nowrap; width:10%; }
	    div#newTagNameDiv { height: 95px; width:350px; margin: 10em 0 0 -15em; border: solid 1px #aaaaaa; z-index: 10; background-color: white; }
	</style>
  </head>
  <body onload="javascript:pageLoaded()"  >
    <form id="LibraryItem" runat="server">
	    <input type="hidden" name="LastClickedOn" id="LastClickedOn" value="" />
        <input type="hidden" name="LastClickedOnChecked" id="LastClickedOnChecked" value="false" />
	    <input type="hidden" name="taxonomyselectedtree" id="taxonomyselectedtree" value="" runat="server" />
		<input type="hidden" name="netscape" id="netscape" onkeypress="javascript:return CheckKeyValue(event,'34,13');"/>
        <input type="hidden" name="hdnIsPostBack" id="hdnIsPostBack" runat="server"/>
		<asp:Literal ID="PostBackPage" Runat="server"/>
		<asp:PlaceHolder ID="ToolBarHolder" Runat="server" />
		<div class="ektronPageContainer">
		    <asp:Panel ID="ViewLibraryCategoryPanel" Visible="False" Runat="server" CssClass="ektronPageGrid" ><!--ViewLibraryCategory OR ViewLibraryByCategory-->
	            <asp:DataGrid id="ViewLibraryCategoryGrid"
	                runat="server"
	                AutoGenerateColumns="False"
	                Width="100%"
	                OnItemDataBound="ViewLibraryCategoryGrid_ItemDataBound"
	                EnableViewState="False"
	                AllowPaging="False"
	                AllowCustomPaging="True"
	                PageSize="10"
	                PagerStyle-Visible="False"
	                CssClass="ektronGrid"
	                GridLines="None">
                    <HeaderStyle CssClass="title-header" />
	            </asp:DataGrid>
	            <uxEktron:Paging ID="uxPaging" runat="server" />
		    </asp:Panel>
		    <asp:Panel ID="UpdateQlinkTemplateByCategoryPanel" CssClass="ektronPageGrid" Visible="False" Runat="server">
		        <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
			        function checkAll(bChecked){
				        for (var i = 0; i < document.forms[0].elements.length; i++){
					        if (document.forms[0].elements[i].type == "checkbox"){
						        document.forms[0].elements[i].checked = bChecked;
					        }
				        }
			        }

			        function checkUpdateForm(){
				        var bFound = false;
				        var bRet;
				        $ektron("#hdnIsPostBack")[0].value = "true";

				        for (var i = 0; i < document.forms[0].elements.length; i++){
					        if (document.forms[0].elements[i].type == "checkbox"){
						        if (document.forms[0].elements[i].checked){
							        bFound = true;
						        }
					        }
				        }
				        if (bFound == false){
					        alert('<%=_MessageHelper.GetMessage("msg sel content block")%>');
					        return false;
				        }
				        else{
					        if (document.forms[0].template_to.value == document.forms[0].template_from.value){
						        alert('<%=_MessageHelper.GetMessage("msg diff input")%>');
						        return false;
					        }
					        else{
						        bRet = confirm('<%=_MessageHelper.GetMessage("msg update sel content block")%>');
						        if (bRet){
							        document.forms[0].submit();
							        return false;

						        }
					        }
				        }
			        }
		        //--><!]]>
		        </script>
		        <div class="ektronPageInfo">
		            <table class="ektronGrid">
			            <tr>
				            <td class="label" title="From"><%=_MessageHelper.GetMessage("generic from label")%></td>
				            <td class="value" id="qlinkfrom" runat="server"></td>
			            </tr>
			            <tr>
				            <td class="label" title="To"><%=_MessageHelper.GetMessage("generic to label")%></td>
				            <td class="value" id="qlinkto" runat="server"></td>
			            </tr>
		            </table>
		            <div class="ektronTopSpace"></div>
		            <div class="ektronBorder">
	                    <asp:DataGrid id="QlinkTemplateByCategoryGrid"
	                        runat="server"
	                        AutoGenerateColumns="False"
	                        Width="100%"
	                        EnableViewState="False"
	                        CssClass="ektronGrid"
	                        GridLines="None">
                            <HeaderStyle CssClass="title-header" />
	                    </asp:DataGrid>
		            </div>
		        </div>
		        <input type="hidden" id="libids" name="libids" value="" runat="server"/>
		        <input type="hidden" id="folder_id" name="folder_id" value="" runat="server"/>
                <uxEktron:Paging runat="server" ID="uxPagingUpdateLink" />
		    </asp:Panel>
		    <asp:Panel ID="AddLibraryItemPanel" Visible="False"  Runat="server">
		        <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
			        var jsImageExtension="<asp:literal id="jsImageExtension" runat="server"/>"
			        var jsFileExtension="<asp:literal id="jsFileExtension" runat="server"/>"
			        var jsAddToImageLib="<asp:literal id="jsAddToImageLib" runat="server"/>"
			        var jsAddToFileLib="<asp:literal id="jsAddToFileLib" runat="server"/>"
			        function CheckAddSubmission(LibraryType) {
				        if (!CheckLibraryForm(LibraryType)) {
					        return false;
				        }
				        if ((LibraryType == "quicklinks") || (LibraryType == "hyperlinks")) {
					        return true;
				        }
				        // TT 29811 - It is a knowledgebase issue
				        /*if (document.forms[0].frm_filename.value.indexOf('&') != -1) {
				            alert("File name can not contain '&'.");
				            return false;
				        }*/
				        if (CheckFileType(LibraryType)) {
					        var VerifiedType = CheckExtensions();
					        if ((VerifiedType == "images") || (VerifiedType == "files")) {
						        if (VerifiedType != LibraryType) {
							        if (VerifiedType == "images") {
								        var msg = "<%= _MessageHelper.GetMessage("js: confirm upload image to files") %>";
							        }
							        else {
								        var msg = "<%= _MessageHelper.GetMessage("js: confirm upload files to images") %>";
							        }
							        msg += "\n" + "<%= _MessageHelper.GetMessage("js: confirm upload correct folder") %>";
							        if (confirm(msg)) {
								        if (VerifiedType == "images") {
									        document.forms[0].frm_libtype.value = "images";
								        }
								        else {
									        document.forms[0].frm_libtype.value = "files";
								        }
								        return true;
							        }
							        return false;
						        }
						        return true;
					        }
				        }
				        else {
					        return false;
				        }
			        }

			        function VerifyOverwriteSubmission(LibraryType) {
				        var VerifiedType;
				        var localExtension;
				        var serverExtension;
				        // TT 29811 - It is a knowledgebase issue
				        /*if (document.forms[0].frm_filename.value.indexOf('&') != -1) {
				            alert("File name can not contain '&'.");
				            return false;
				        }*/
				        if(LibraryType != "quicklinks" && LibraryType != "forms" && jsCategoryRequired=="true")
		                    {
                                if(Trim(document.getElementById('taxonomyselectedtree').value) == '')
                                {
                                   alert ("<%= _MessageHelper.GetMessage("js tax cat req") %>");
                                   return false;
                                }
		                    }
				        if ((LibraryType == "images") || (LibraryType == "files")) {
					        if (CheckFileType('overwrite')) {
						        localExtension = document.forms[0].frm_filename.value.split(".");
						        serverExtension = document.forms[0].frm_oldfilename.value.split(".");
						        if (localExtension[localExtension.length - 1].toLowerCase() != serverExtension[serverExtension.length - 1].toLowerCase()) {
							        alert("<%=(_MessageHelper.GetMessage("js: same extension for overwrite error"))%>" + "\n\n" + "<%=(_MessageHelper.GetMessage("js: local file ext prompt"))%>" + " " + localExtension[localExtension.length - 1] + "\n\n" + "<%=(_MessageHelper.GetMessage("js: library file ext prompt"))%>" + " " + serverExtension[serverExtension.length - 1]);
							        return false;
						        }
						        VerifiedType = CheckExtensions();
						        if (VerifiedType != LibraryType) {
							        return false;
						        }
						        return true;
					        }
				        }
				        return false;
			        }

			        function CheckOverwriteSubmission(LibraryType) {
				        var localExtension;
				        var serverExtension;
				        var regexp1 = /\//gi;

				        if (!VerifyOverwriteSubmission(LibraryType)) {
					        return false;
				        }
				        localExtension = document.forms[0].frm_filename.value.replace(regexp1, "/");
				        localExtension = localExtension.split("/");
				        serverExtension = document.forms[0].frm_oldfilename.value.split("/");
				        if (localExtension[localExtension.length - 1] != serverExtension[serverExtension.length - 1]) {
					        var msg = "<%=(_MessageHelper.GetMessage("js: warn of overwrite filename change"))%>";
					        return confirm(msg);
				        }
				        else
				            return true;
			        }

			        function CheckFileType(operation) {
				        var VerifiedType = CheckExtensions(operation);
				        if ((VerifiedType == "images") || (VerifiedType == "files")) {
					        return true;
				        }
				        if (VerifiedType != "empty") {
					        if (VerifiedType == "nouploadimage") {
						        alert("<%= _MessageHelper.GetMessage("js: alert upload image denied") %>");
						        return false;
					        }
					        if (VerifiedType == "nouploadfile") {
						        alert("<%= _MessageHelper.GetMessage("js: alert upload file denied") %>");
						        return false;
					        }
					        if (VerifiedType == "noupload") {
						        alert("<%= _MessageHelper.GetMessage("js: alert upload image/file denied") %>");
						        return false;
					        }
					        if (VerifiedType == "noselection") {
						        alert("<%= _MessageHelper.GetMessage("js: alert select folder") %>");
						        return false;
					        }
					        var msg = "<%= _MessageHelper.GetMessage("js: alert invalid extension") %>";
					        msg += "\n\n";
					        var msg1 = "";
					        if (jsAddToImageLib) {
						        msg1 += "<%= _MessageHelper.GetMessage("js: alert for images") %>";
						        msg1 += "\n";
						        msg1 += jsImageExtension;
						        msg1 += "\n\n";
					        }
					        if (jsAddToFileLib) {
						        msg1 += "<%= _MessageHelper.GetMessage("js: alert for files") %>";
						        msg1 += "\n";
						        msg1 += jsFileExtension;
						        msg1 += "\n";
					        }
					        alert (msg + msg1);
				        }
				        document.forms.LibraryItem.frm_filename.focus();
				        return false;
			        }

			        function IsExtensionValid(libType, filename) {
				        if (libType == "images") {
					        var ExtensionList = jsImageExtension;
				        }
				        else if (libType == "files") {
					        var ExtensionList = jsFileExtension;
				        }
				        if (ExtensionList.length > 0) {
					        var ExtensionArray = ExtensionList.split(",");
					        var FileExtension = filename.split(".");
					        for (var i = 0; i < ExtensionArray.length; i++) {
						        if (FileExtension[FileExtension.length - 1].toLowerCase() == Trim(ExtensionArray[i].toLowerCase())) {
							        return true;
						        }
					        }
					        return false;
				        }
			        }

			        function CheckExtensions(operation) {
				        var szTempFilename;
				        szTempFilename = Trim(document.forms.LibraryItem.frm_filename.value);
				        if (szTempFilename == "") {
					        return 'empty';
				        }
				        if ((!jsAddToImageLib) && (!jsAddToFileLib)) {
					        return 'noupload';
				        }
				        if (IsExtensionValid("images", szTempFilename)) {
					        if ((operation == "overwrite") || (jsAddToImageLib)) {
						        return 'images';
					        }
					        else {
						        return 'nouploadimage';
					        }
				        }
				        if (IsExtensionValid("files", szTempFilename)) {
					        if ((operation == "overwrite") || (jsAddToFileLib)) {
						        return 'files';
					        }
					        else {
						        return 'nouploadfile';
					        }
				        }
				        return 'false';
			        }

			        function CheckPreview (UploadType, URLref, operation) {
				        var szFilename;
				        szFilename = Trim(document.forms[0].frm_filename.value);
				        if ((UploadType == "files") || (UploadType == "images")) {
					        if (window.navigator.userAgent.indexOf("Gecko") > -1) {
						        alert("<%= _MessageHelper.GetMessage("js: alert no local preview") %>");
						        return false;
					        }
					        if (szFilename == "") {
						        alert("<%= _MessageHelper.GetMessage("js: alert select file") %>");
						        return false;
					        }
					        var VerifiedType = CheckExtensions(operation);
					        if ((VerifiedType == "images") || (VerifiedType == "files")) {
						        return PreviewSelection(URLref, UploadType);
					        }
					        var msg = "<%= _MessageHelper.GetMessage("js: alert invalid extension") %>" + "\n";
					        msg += jsImageExtension+','+jsFileExtension;
					        alert (msg);
					        return false;
				        }
				        else if ((UploadType == "hyperlinks") ||(UploadType == "quicklinks")) {
					        if (szFilename == "") {
						        alert("<%= _MessageHelper.GetMessage("js: alert enter valid url link") %>");
						        return false;
					        }
					        return PreviewSelection(URLref, UploadType);
				        }
			        }

			        function PreviewSelection(oldURL, LibraryType) {
				        var filename;
				        var regexp2 = / /gi;
				        filename = document.forms[0].frm_filename.value;
				        if ((LibraryType == "images") || (LibraryType == "files")) {
					        var regexp1 = /\\/gi;
					        filename = filename.replace(regexp2, '%20');
					        var tempHREF = 'file:///' + filename.replace(regexp1, '/');
				        }
				        else if (LibraryType == "hyperlinks") {
					        var tempHREF = filename.toLowerCase();
					        if ((tempHREF.substring(0,7) == "http://") ||
								        (tempHREF.substring(0,8) == "https://")) {
						        tempHREF = filename.replace(regexp2, '%20');
					        }
					        else {
						        tempHREF = "http://" + filename.replace(regexp2, '%20');
					        }
				        }
				        else {
					        var tempHREF = "<%= _SitePath %>";
					        if (filename.indexOf("?") != -1) {
						        tempHREF += filename.replace(regexp2, '%20'); + "&Preview=True";
					        }
					        else {
						        tempHREF += filename.replace(regexp2, '%20'); + "?Preview=True";
					        }
				        }
				        for (var i = 0; i < document.links.length; i++) {
					        if (document.links[i].href == oldURL) {
						        break;
					        }
				        }
				        document.links[i].href = tempHREF;
				        return true;
			        }
		        //--><!]]>
		        </script>
		        <input type="hidden" id="frm_folder_id" name="frm_folder_id" value="" runat="server"/>
		        <input type="hidden" id="frm_libtype" name="frm_libtype" value="" runat="server"/>
		        <input type="hidden" id="frm_operation" name="frm_operation" value="" runat="server"/>
		        <input type="hidden" id="frm_library_id" name="frm_library_id" value="" runat="server"/>
		        <input type="hidden" id="upload_directory" name="upload_directory" value="" runat="server"/>
		        <table class="ektronGrid" width="100%">
			        <tr class="title-header">
				        <td id="tr1_td1_ali" runat="server"/>
				        <td id="tr1_td2_ali" runat="server"/>
				        <td id="tr1_td3_ali" runat="server"/>
			        </tr>
			        <tr>
				        <td id="tr2_td1_ali" runat="server"/>
				        <td>
					        <asp:Literal ID="tr2_td2_ali_controls" Runat="server"/>
					        <asp:Panel id="OverwriteSubPanel0" Visible="False" Runat="server">
						        <input type="file" size="40" id="frm_filename" name="frm_filename" onkeypress="javascript:return CheckKeyValue(event,'34');" onclick="javascript:CheckFileType1();" runat="server" />
					        </asp:Panel>
					        <input type="hidden" id="frm_oldfilename" name="frm_oldfilename" value="" runat="server"/>
				        </td>
				        <td id="tr2_td3_ali" runat="server"/>
			        </tr>
			        <asp:Panel id="OverwriteSubPanel1" Visible="False" Runat="server">
			            <tr>
				            <td class="label" title="Please select a replacement file"><%=_MessageHelper.GetMessage("overwrite selection request msg")%></td>
				            <td id="TD_filename" runat="server"></td>
			            </tr>
			        </asp:Panel>
		        </table>
		        <asp:Panel id="OverwriteSubPanel2" Visible="False" Runat="server">
		            <table class="ektronGrid">
			            <tr>
				            <td><label class="label" title="Current library item"><%=_MessageHelper.GetMessage("current library item msg")%></label></td>
			            </tr>
		            </table>
		        </asp:Panel>
		        <asp:Image ID="Overwrite_Image" ImageUrl="" Visible="False" Runat="server"/>
		        <asp:HyperLink ID="Overwrite_link" NavigateUrl="" Visible="False" Runat="server"/>

	            <asp:Panel ID="addTabs" Visible="false" runat="server" CssClass="ektronPageTabbed">
                    <div class="tabContainerWrapper">
                        <div class="tabContainer">
	                        <ul>
                                <li>
                                    <a title="Summary" href="#dvSummary">
                                        <asp:literal ID="AdddvSummaryTxt" runat="server" />
                                    </a>
                                </li>
                                <li>
                                    <a title="Metadata" href="#dvMetadata">
                                        <asp:literal ID="AdddvMetadataTxt" runat="server" />
                                    </a>
                                </li>
                                <asp:PlaceHolder ID="phAddCategoryTab" runat="server">
                                    <li>
                                        <a title="Category" href="#dvCategory">
                                            <asp:literal ID="AdddvCategoryTxt" runat="server" />
                                        </a>
                                    </li>
                                </asp:PlaceHolder>
                            </ul>

                        </div>
                        </div>
                </asp:Panel>

                <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
		        Ektron.ready( function() {
			        document.forms[0].frm_title.onkeypress = document.forms[0].netscape.onkeypress;
			        document.forms[0].frm_filename.onkeypress = document.forms[0].netscape.onkeypress;
			        <asp:literal id="AddItemFocus" runat="server"/>
			    });
		        //--><!]]>
		        </script>
		    </asp:Panel>
		    <asp:Panel CssClass="ektronPageInfo" ID="ViewLibraryItemPanel" Runat="server" Visible="False">
	            <asp:DataGrid id="ViewLibraryItemGrid"
                    runat="server"
                    AutoGenerateColumns="False"
                    EnableViewState="False"
                    GridLines="None"
                    ShowHeader="false"
                    CssClass="ektronGrid" />
                <asp:Literal ID="ViewLibraryMeta" Runat="server"/>
	            <asp:Literal ID="ViewTaxonomy" runat="server" />
	            <asp:Literal ID="ViewLibraryTags" Runat="server"/>
                <asp:DataGrid id="ViewLibraryItemLinkGrid"
                    runat="server"
                    AutoGenerateColumns="False"
                    Width="100%"
                    EnableViewState="False"
                    GridLines="None" />
		    </asp:Panel>
		    <asp:Panel ID="DeleteLibraryItemPanel" Runat="server" Visible="False">
	            <asp:Literal ID="DeleteItemHiddenFields" Runat="server"/>
                <asp:DataGrid id="DeleteLibraryItemGrid"
                    runat="server"
                    AutoGenerateColumns="False"
                    OnItemDataBound="DeleteLibraryItemGrid_ItemDataBound"
                    EnableViewState="False"
                    GridLines="None"
                    ShowHeader="false"
                    CssClass="ektronGrid" />
                <asp:DataGrid id="DeleteLibraryItemLinkGrid"
                    runat="server"
                    AutoGenerateColumns="False"
                    Width="100%"
                    EnableViewState="False"
                    GridLines="None" />
		    </asp:Panel>
		    <asp:Panel ID="EditLibraryItemPanel" Runat="server" Visible="False">
                <div class="ektronPageGrid">
                    <asp:DataGrid id="EditLibraryItemGrid"
                        runat="server"
                        AutoGenerateColumns="False"
                        EnableViewState="False"
                        Width="100%"
                        CssClass="ektronGrid"
                        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
                    </asp:DataGrid>
                </div>
                <asp:Panel CssClass="ektronPageTabbed" ID="editTabs" Visible="false" runat="server">
                    <div class="tabContainerWrapper">
                        <div class="tabContainer">
                            <ul runat="server" id="editTabsUl">
                                <li>
                                    <a title="Summary" href="#dvSummary">
                                        <asp:literal ID="EditdvSummaryTxt" runat="server" />
                                    </a>
                                </li>
                                <li>
                                    <a title="Metadata" href="#dvMetadata">
                                        <asp:literal ID="EditdvMetadataTxt" runat="server" />
                                    </a>
                                </li>
                                <asp:PlaceHolder ID="phCategory" runat="server">
                                    <li>
                                        <a title="Category" href="#dvCategory">
                                            <asp:literal ID="EditdvCategoryTxt" runat="server" />
                                        </a>
                                    </li>
                                </asp:PlaceHolder>
                            </ul>

	                        <div title="Summary" id="dvSummary" runat="server">
	                            <asp:Literal ID="EditSummaryHtml" Runat="server" />
	                            <ektron:ContentDesigner ID="cdContent_teaser" runat="server" AllowScripts="true" Height="200" Width="100%" Visible="false"
										Toolbars="Minimal" ShowHtmlMode="false" />
                            </div>
	                        <div title="Metadata" id="dvMetadata" runat="server" >
	                            <asp:Literal ID="ShowMeta" runat="server"/>
                                <asp:Literal ID="ShowTags" runat="server"/>
	                        </div>
	                        <asp:PlaceHolder ID="phCategory2" runat="server">
                                <div title="Category" id="dvCategory" runat="server">
                                    <div id="TreeOutput"></div>
                                </div>
                            </asp:PlaceHolder>
                        </div>
                    </div>
                </asp:Panel>
		        <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
		        Ektron.ready( function()
                {
			        document.forms[0].frm_title.onkeypress = document.forms[0].netscape.onkeypress;
			        document.forms[0].frm_filename.onkeypress = document.forms[0].netscape.onkeypress;
			     }
			     );
		        //--><!]]>
		        </script>
		    </asp:Panel>
		    <asp:Panel ID="ViewLibrarySettingsPanel" Runat="server">
	            <div class="ektronPageGrid">
	                <asp:DataGrid id="ViewLibrarySettingsGrid"
	                    runat="server"
	                    AutoGenerateColumns="False"
	                    EnableViewState="False"
	                    GridLines="None"
	                    ShowHeader="false"
	                    CssClass="ektronGrid" />
	            </div>
		    </asp:Panel>
		    <asp:Panel ID="ViewLoadBalancePanel" CssClass="ektronPageGrid" Runat="server" Visible="False" >
	            <asp:DataGrid id="ViewLoadBalanceGrid"
	                runat="server"
	                AutoGenerateColumns="False"
	                Width="100%"
	                EnableViewState="False"
	                GridLines="None">
                    <HeaderStyle CssClass="title-header" />
	            </asp:DataGrid>
		    </asp:Panel>
		    <asp:Panel ID="AddLoadBalancePanel" Runat="server" Visible="False">
		        <div class="ektronPageInfo">
		            <asp:RadioButtonList ID="AssetType" Runat="server" RepeatLayout="Table" />
		            <div class="ektronTopSpace"></div>
                    <table class="ektronGrid">
		                <tr>
			                <td class="label" title="Load Balance Path"><%=_MessageHelper.GetMessage("load balance path label")%></td>
		                    <td class="value"><input type="text" title="Enter Load Balance Path here" size="75" maxlength="255" name="loadBalancePath"/></td>
		                </tr>
		            </table>
		            <div class="ektronTopSpaceSmall"></div>
		            <input title="Make Relative to Site" type="checkbox" name="MakeRelative" checked="checked" /><%=_MessageHelper.GetMessage("make dir rel to site") + " " + _SitePath%>
		        </div>
		        <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
		        Ektron.ready( function()
                {
			        document.forms[0].loadBalancePath.onkeypress = document.forms[0].netscape.onkeypress;
			        document.forms[0].loadBalancePath.focus();
                }
		        //--><!]]>
		        </script>
		    </asp:Panel>
		    <asp:Panel ID="EditLoadBalanceSettingsPanel" Runat="server" Visible="False">
		        <div class="ektronPageInfo">
		            <asp:Literal ID="DisplayEditLBSettingsData" Runat="server" />
		        </div>
		        <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
		        Ektron.ready( function()
                {
			        document.forms[0].loadBalancePath.onkeypress = document.forms[0].netscape.onkeypress;
			        document.forms[0].loadBalancePath.focus();
		        }
		        //--><!]]>
		        </script>
		    </asp:Panel>
		    <asp:Panel ID="RemoveLoadBalancePanel" CssClass="ektronPageGrid" Runat="server" Visible="False">
	            <asp:DataGrid id="RemoveLoadBalanceGrid"
	                runat="server"
	                AutoGenerateColumns="False"
	                Width="100%"
	                EnableViewState="False"
	                GridLines="None">
                    <HeaderStyle CssClass="title-header" />
	            </asp:DataGrid>
		        <asp:Literal ID="RLB_Hidden" Runat="server"/>
		        <input type="hidden" id="lbPathCount" name="lbPathCount" value="" runat="server" />
		    </asp:Panel>
		    <asp:Panel ID="EditLibrarySettingsPanel" Runat="server" Visible="False">
		        <div class="ektronPageInfo">
		            <table class="ektronGrid">
			            <tr>
				            <td class="label" title="Image Extensions"><%=_MessageHelper.GetMessage("image extensions label")%></td>
				            <td class="value"><input type="text" title="Enter Image Extensions here" size="75" maxlength="255" id="imageextensions" name="imageextensions" value="" runat="server"/></td>
			            </tr>
			            <tr>
			                <td class="label" title="Image Upload Directory"><%=_MessageHelper.GetMessage("image upload directory label")%></td>
				            <td class="value" id="td_els_imgdirectory" runat="server"/>
			            </tr>
			            <tr>
				            <td colspan="2"><asp:CheckBox ToolTip="Relative Images Option" ID="relativeimages" Runat="server"/></td>
			            </tr>
                        <tr>
				            <td colspan="2"><div class="ektronTopSpace"></div></td>
			            </tr>
			            <tr>
				            <td class="label" title="File Extensions"><%=_MessageHelper.GetMessage("file extensions label")%></td>
				            <td class="value"><input type="text" title="Enter File Extensions here" id="fileextensions" name="fileextensions" size="75" maxlength="255" value="" runat="server"/></td>
			            </tr>
			            <tr>
			                <td class="label" title="File Upload Directory"><%=_MessageHelper.GetMessage("file upload directory label")%></td>
				            <td class="value" id="td_els_directory" runat="server"/>
			            </tr>
			            <tr>
				            <td colspan="2"><input title="Relative Files" type="checkbox" size="75" id="relativefiles" name="relativefiles" runat="server" /><%=_MessageHelper.GetMessage("make dir rel to site") + " " + _SitePath%></td>
			            </tr>
		            </table>
		        </div>
		        <script type="text/javascript">
		        <!--//--><![CDATA[//><!--
		            Ektron.ready(function() {
		                    document.forms[0].imageextensions.onkeypress = document.forms[0].netscape.onkeypress;
		                    document.forms[0].imagedirectory.onkeypress = document.forms[0].netscape.onkeypress;
		                    document.forms[0].fileextensions.onkeypress = document.forms[0].netscape.onkeypress;
		                    document.forms[0].filedirectory.onkeypress = document.forms[0].netscape.onkeypress;
			                document.forms[0].imageextensions.focus();
			            }
                    );
		        //--><!]]>
		        </script>
		    </asp:Panel>

	        <%if( TaxonomyRoleExists){%>
                <div id="FrameContainer" style="position: absolute; top: 0px; left: 0px; width: 1px;
                    height: 1px; display: none; z-index: 1000;">
                    <iframe id="ChildPage" src="javascript:false;" frameborder="1" marginheight="0" marginwidth="0" width="100%"
                        height="100%" scrolling="auto" style="background-color: white;">
                    </iframe>
                </div>
            <%}%>

	        <script type="text/javascript">
	            <!--//--><![CDATA[//><!--
	            function ShowAddPersonalTagArea(){
		            $ektron("#newTagNameDiv").modalShow();
	            }

	            this.customPTagCnt = 0;
	            function SaveNewPersonalTag(){
		            // add new tag:
		            //<input " + IIf(htTagsAssignedToUser.ContainsKey(td.Id), "checked=""checked"" ", "") + " type=""checkbox"" id=""userPTagsCbx_" + td.Id.ToString + """ name=""userPTagsCbx_" + td.Id.ToString + """ />&#160;" + td.Text + "<br />
		            var objTagName = document.getElementById("newTagName");
		            var objTagLanguage = document.getElementById("TagLanguage");
		            var objLanguageFlag = document.getElementById("flag_" + objTagLanguage.value);

		            var divObj = document.getElementById("newAddedTagNamesDiv");

		            if(!CheckForillegalChar(objTagName.value)){
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
		            }

		            // now close window:
		            CancelSaveNewPersonalTag();
	            }

	            function CancelSaveNewPersonalTag(){
		            $ektron("#newTagNameDiv").modalHide();
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

                function CheckForillegalChar(tag) {
                   if (Trim(tag) == '')
                   {
                       alert('<asp:Literal ID="error_TagsCantBeBlank" Text="Please enter a name for the Tag." runat="server"/>');
                       return false;
                   } else {

                        //alphanumeric plus _ -
                        var tagRegEx = /[!"#$%&'()*+,.\/:;<=>?@[\\\]^`{|}~ ]+/;
                        if(tagRegEx.test(tag)==true) {
                            alert('<asp:Literal ID="error_InvalidChars" Text="Tag Text can only include alphanumeric characters." runat="server"/>');
                            return false;
                        }

                   }
                   return true;
                }

	            //--><!]]>
	        </script>
	        <script type="text/javascript">
	            <!--//--><![CDATA[//><!--
                var taxonomytreearr="<%=TaxonomyTreeIdList%>".split(",");
                var taxonomytreedisablearr="<%=TaxonomyTreeParentIdList%>".split(",");
                var __EkFolderId="<%=_FolderId%>";
                var __TaxonomyOverrideId="<%=TaxonomyOverrideId%>";
                function fetchtaxonomyid(pid){
                    for(var i=0;i<taxonomytreearr.length;i++){
                        if(taxonomytreearr[i]==pid){
                            return true;
                            break;
                        }
                    }
                    return false;
                }
                 function fetchdisabletaxonomyid(pid){
                    for(var i=0;i<taxonomytreedisablearr.length;i++){
                        if(taxonomytreedisablearr[i]==pid){
                            return true;
                            break;
                        }
                    }
                    return false;
                }
                function updatetreearr(pid,op){
                    if(op=="remove"){
                        for(var i=0;i<taxonomytreearr.length;i++){
                            if(taxonomytreearr[i]==pid){
                                taxonomytreearr.splice(i,1);break;
                            }
                        }
                    }
                    else{
                        taxonomytreearr.splice(0,0,pid);
                    }
                    document.getElementById("taxonomyselectedtree").value="";
                    for(var i=0;i<taxonomytreearr.length;i++){
                        if(document.getElementById("taxonomyselectedtree").value==""){
                            document.getElementById("taxonomyselectedtree").value=taxonomytreearr[i];
                        }else{
                            document.getElementById("taxonomyselectedtree").value=document.getElementById("taxonomyselectedtree").value+","+taxonomytreearr[i];
                        }
                    }
                }
                function selecttaxonomy(control){
                    var pid=control.value;
                    if(control.checked)
                    {
                        updatetreearr(pid,"add");
                    }
                    else
                    {
                        updatetreearr(pid,"remove");
                    }
                    var currval=eval(document.getElementById("chkTree_T"+pid).value);
                    var node = document.getElementById( "T" + pid );
                    var newvalue=!currval;
                    document.getElementById("chkTree_T"+pid).value=eval(newvalue);
                    if(control.checked)
                      {
                        Traverse(node,true);
                      }
                    else
                      {
                        Traverse(node,false);
                        var hasSibling = false;
                        if (taxonomytreearr != "")
                          { for(var i = 0 ;i<taxonomytreearr.length;i++)
                                {
                                  if(taxonomytreearr[i] != "")
                                    {
                                      var newnode = document.getElementById( "T" + taxonomytreearr[i]);
                                        if(newnode != null && newnode.parentNode == node.parentNode)
                                           {Traverse(node,true);hasSibling=true;break;}
                                    }
                                }
                          }
                        if(hasSibling == false)
                        {
                         checkParent(node);
                        }
                      }
                }

                function checkParent(node)
                { if(node!= null)
                    {
                          var subnode = node.parentNode;
                          if(subnode!=null && subnode.id!="T0" &&  subnode.id!="")
                          {
                                    for(var j=0;j<subnode.childNodes.length;j++)
                                      {var pid=subnode.childNodes[j].id;
                                       if(document.getElementById("chkTree_"+pid).value == true || document.getElementById("chkTree_"+pid).value == "true")
                                          {Traverse(subnode.childNodes[j],true);return;}
                                      }
                           checkParent(subnode.parentNode);
                          }
                    }
                }
                function Traverse(node,newvalue){
                    if(node!=null){
                        subnode=node.parentNode;
                         if(subnode!=null && subnode.id!="T0" &&  subnode.id!=""){
                            for(var j=0;j<subnode.childNodes.length;j++){
                                var n=subnode.childNodes[j]
                                if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                                    var pid=subnode.id;
                                    updatetreearr(pid.replace("T",""),"remove");
                                    document.getElementById("chkTree_"+pid).value=eval(newvalue);
                                    n.setAttribute("checked",eval(newvalue));
                                    n.setAttribute("disabled",eval(newvalue));

                                }
                            }
                            if(HasChildren(subnode) && subnode.getAttribute("checked")){
                                   subnode.setAttribute("checked",true);
                                    subnode.setAttribute("disabled",true);
                            }
                            Traverse(subnode,newvalue);
                        }
                    }
                }
                function HasChildren(subnode)
                {
                    if(subnode!=null){
                        for(var j=0;j<subnode.childNodes.length;j++)
                        {
                            for(var j=0;j<subnode.childNodes.length;j++){
                                var n=subnode.childNodes[j]
                                if(n.nodeName=="INPUT" && n.attributes["type"].value=="checkbox"){
                                    var pid=subnode.id;
                                    var v=document.getElementById("chkTree_"+pid).value;
                                    if(v==true || v=="true"){
                                    return true;break;
                                    }
                                }
                            }
                        }
                    }
                    return false;
                }
                //--><!]]>
            </script>
       <%--   /-- common/taxonomy_editor_menu.inc code starts here--%>
       
       
       <%if( TaxonomyRoleExists) {%>
    <script language="javascript" type="text/javascript">
        var taxonomytreemenu = true;
        var g_delayedHideTimer = null;
        var g_delayedHideTime = 1000;
        var g_wamm_float_menu_treeid = -1;
        var g_isIeInit = false;
        var g_isIeFlag = false;

        function IsBrowserIE() {
            if (!g_isIeInit) {
                var ua = window.navigator.userAgent.toLowerCase();
                g_isIeFlag = (ua.indexOf('msie') > -1) && (!(ua.indexOf('opera') > -1));
                g_isIeInit = true;
            }
            return (g_isIeFlag);
        }

        function markMenuObject(markFlag, id) {
            if (id && (id > 0)) {
                var obj = document.getElementById(id);
                if (obj && obj.className) {
                    if (markFlag) {
                        if (obj.className.indexOf("linkStyle_selected") < 0) {
                            obj.className += " linkStyle_selected";
                        }
                    }
                    else {
                        if (obj.className.indexOf("linkStyle_selected") >= 0) {
                            obj.className = "linkStyle";
                        }
                    }
                }
            }
        }

        function showWammFloatMenuForMenuNode(show, delay, event, treeId) {
            var el = document.getElementById("wamm_float_menu_block_menunode");
            var visible = "";
            if (el) {
                if (g_delayedHideTimer) {
                    clearTimeout(g_delayedHideTimer);
                    g_delayedHideTimer = null;
                }
                var tree = null;
                if (treeId > 0) {
                    tree = TreeUtil.getTreeById(treeId);
                }
                if (tree && tree.node && tree.node.data) {
                    visible = tree.node.data.visible;
                }
                if (show) {
                    el.style.display = "none";
                    if (visible != "false")
                        markMenuObject(false, g_wamm_float_menu_treeid);
                    if (null != event) {
                        var hoverElement = $ektron("#" + treeId);
                        var offset = hoverElement.offset();
                        var hoverElementHeight = parseInt(hoverElement.height(), 10);
                        var hoverElementWidth = parseInt(hoverElement.width(), 10)

                        var fixedPositionToolbarFix = 0;
                        if ($ektron("form#LibraryItem").length > 0) {
                            fixedPositionToolbarFix = 44;
                        }

                        el.style.top = (parseInt(offset.top, 10) + hoverElementHeight - 5 - fixedPositionToolbarFix) + "px";
                        el.style.left = (parseInt(offset.left, 10) + hoverElementWidth - 5) + "px";

                        el.style.display = "";
                        if (treeId && (treeId > 0)) {
                            g_wamm_float_menu_treeid = treeId;
                            if (visible != "false")
                                markMenuObject(true, treeId);
                        }
                        else {
                            g_wamm_float_menu_treeid = -1;
                        }
                    }
                }
                else {
                    if (delay) {
                        g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
                    }
                    else {
                        el.style.display = "none";
                        if (visible != "false")
                            markMenuObject(false, g_wamm_float_menu_treeid);
                    }
                }
            }
        }

        function getEventX(event) {
            var xVal;
            if (IsBrowserIE()) {
                xVal = event.x;
            }
            else {
                xVal = event.pageX;
            }
            return (xVal)
        }

        function getShiftedEventX(event) {
            var srcLeft;
            var xVal;
            if (IsBrowserIE()) {
                xVal = event.x;
            }
            else {
                xVal = event.pageX;
            }

            // attempt to shift div-tag to the right of the menu items:
            srcLeft = xVal;
            if (event.srcElement && event.srcElement.offsetLeft) {
                srcLeft = event.srcElement.offsetLeft;
            }
            else if (event.target && event.target.offsetLeft) {
                srcLeft = event.target.offsetLeft;
            }

            if (event.srcElement) {
                if (event.srcElement.offsetWidth) {
                    xVal = srcLeft + event.srcElement.offsetWidth;
                }
                else if (event.srcElement.scrollWidth) {
                    xVal = srcLeft + event.srcElement.scrollWidth;
                }
            }
            else if (event.target && event.target.offsetLeft) {
                if (event.target.offsetWidth) {
                    xVal = srcLeft + event.target.offsetWidth;
                }
                else if (event.target.scrollWidth) {
                    xVal = srcLeft + event.target.scrollWidth;
                }
            }

            return (xVal)
        }


        function getEventY(event) {
            var yVal;
            if (IsBrowserIE()) {
                yVal = event.y;
            }
            else {
                yVal = event.pageY;
            }
            return (yVal)
        }

        function wamm_float_menu_block_mouseover(obj) {
            if (g_delayedHideTimer) {
                clearTimeout(g_delayedHideTimer);
                g_delayedHideTimer = null;
            }
        }

        function wamm_float_menu_block_mouseout(obj) {
            if (null != obj) {
                g_delayedHideTimer = setTimeout("showWammFloatMenuForMenuNode(false, false, null, -1)", g_delayedHideTime);
            }
        }

        function routeAction(containerFlag, op) {
            var tree = null;
            if (g_wamm_float_menu_treeid > 0) {
                tree = TreeUtil.getTreeById(g_wamm_float_menu_treeid);
            }

            if (tree && tree.node && tree.node.data) {
                var TaxonomyId = tree.node.data.id;
                var ParentId = tree.node.pid;
                if (ParentId == null || ParentId == 'undefined') {
                    ParentId = 0;
                }

                showWammFloatMenuForMenuNode(false, false, null, -1);
                LoadChildPage(op, TaxonomyId, ParentId);
            }
        }
        function LoadChildPage(Action, TaxonomyId, ParentId) {
            var frameObj = document.getElementById("ChildPage");
            var lastClickedOn = document.getElementById("LastClickedOn");
            lastClickedOn.value = TaxonomyId;
            document.getElementById("LastClickedParent").value = ParentId;
            if (parseInt(ParentId) == 0) { document.getElementById("ClickRootCategory").value = "true"; }
            else { document.getElementById("ClickRootCategory").value = "false"; }
            switch (Action) {
                case "add":
                    if (TaxonomyId == "") {
                        alert("Please select a taxonomy.");
                        return false;
                    }
                    frameObj.src = "blankredirect.aspx?taxonomy.aspx?iframe=true&action=add&parentid=" + TaxonomyId;
                    break;
                default:
                    break;
            }
            if (Action != "delete") {
                DisplayIframe();
            }
        }
        function DisplayIframe() {
            var pageObj = document.getElementById("FrameContainer");
            pageObj.style.display = "";
            if (navigator.userAgent.indexOf("MSIE 6.0") > -1) {
                pageObj.style.width = "100%";
                pageObj.style.height = "500px";
            }
            else {
                pageObj.style.width = "95%";
                pageObj.style.height = "95%";
            }
        }
        function CancelIframe() {
            var pageObj = document.getElementById("FrameContainer");
            pageObj.style.display = "none";
            pageObj.style.width = "1px";
            pageObj.style.height = "1px";
        }
        function CloseChildPage() {
            CancelIframe();
            var ClickRootCategory = document.getElementById("ClickRootCategory");
            var lastClickedOn = document.getElementById("LastClickedOn");
            var clickType = document.getElementById("ClickType");
            if (ClickRootCategory.value == "true")
                __EkFolderId = "<%=m_intTaxFolderId%>";
            else {
                __EkFolderId = -1;
                TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
            }
            var node = document.getElementById("T" + lastClickedOn.value);
            if (node != null) {
                for (var i = 0; i < node.childNodes.length; i++) {
                    if ($ektron.browser.msie && parseInt($ektron.browser.version, 10) < 9) {
                        if (node.childNodes(i).nodeName == 'LI' || node.childNodes(i).nodeName == 'UL') {
                            var parent = node.childNodes(i).parentElement;
                            parent.removeChild(node.childNodes(i));
                        }
                    }
                    else {
                        if (node.childNodes[i].nodeName == 'LI' || node.childNodes[i].nodeName == 'UL') {
                            var parent = node.childNodes[i].parentNode;
                            parent.removeChild(node.childNodes[i]);
                        }
                    }
                }
                TREES["T" + lastClickedOn.value].children = [];
                TreeDisplayUtil.reloadParentTree(lastClickedOn.value);
                onToggleClick(lastClickedOn.value, TreeUtil.addChildren, lastClickedOn.value);
            }
        }
    </script>
	<% if ((Page.Request.Url.AbsoluteUri.IndexOf("membership_add_content.aspx") == -1 && Page.Request.Url.ToString().IndexOf("forum=1") == -1)) {%>
    <div id="wamm_float_menu_block_menunode" class="Menu" style="position:absolute; left:10px; top:10px;
        display:none; z-index:3200;" onmouseover="wamm_float_menu_block_mouseover(this)"
        onmouseout="wamm_float_menu_block_mouseout(this)">
        <input type="hidden" name="LastClickedParent" id="LastClickedParent" value="" />
        <input type="hidden" name="ClickRootCategory" id="ClickRootCategory" value="false" />
        <ul>
            <li class="MenuItem add">
                <a href="#" onclick="routeAction(true, 'add');"><%=(m_refMsg.GetMessage("generic add title"))%></a>
            </li>
        </ul>
    </div>
    <% } %>
    <%} else {%>
    <script type="text/javascript" >
        var taxonomytreemenu = false;
    </script>
    <%}%>
       
       <%--// Taxo inc code ends here--------%>
       
          <%-- //--- common/treejs.inc starts here -->--%>
 <link   type='text/css' rel='stylesheet' href='Tree/css/com.ektron.ui.tree.css' />
<script type="text/javascript">
    var taxonomytreemode = "editor"; var ____ek_appPath2 = "";
</script>
<script type="text/javascript" src="Tree/js/com.ektron.utils.url.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.explorer.init.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.explorer.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.explorer.config.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.explorer.windows.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.cms.types.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.cms.parser.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.cms.toolkit.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.cms.api.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.contextmenu.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.iconlist.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.tabs.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.explore.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.ui.taxonomytree.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.net.http.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.lang.exception.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.form.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.log.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.dom.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.debug.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.string.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.cookie.js"></script>

<script type="text/javascript" src="Tree/js/com.ektron.utils.querystring.js"></script>
<script type="text/javascript">
    var clickedElementPrevious = null;
    var clickedIdPrevious = null;

    function onDragEnterHandler(id, element) {
        folderID = id;
        if (clickedElementPrevious != null) {
            clickedElementPrevious.style["background"] = "#ffffff";
            clickedElementPrevious.style["color"] = "#000000";
        }
        element.style["background"] = "#3366CC";
        element.style["color"] = "#ffffff";
    }

    function onMouseOverHandler(id, element) {
        element.style["background"] = "#ffffff";
        element.style["color"] = "#000000";
    }

    function onDragLeaveHandler(id, element) {
        element.style["background"] = "#ffffff";
        element.style["color"] = "#000000";
    }

    function onFolderClick(id, clickedElement) {
        var tree = null;
        var visible = "";
        if (id > 0) {
            tree = TreeUtil.getTreeById(id);
        }
        if (tree) {
            if (tree.node) {
                if (tree.node.data) {
                    visible = tree.node.data.visible;
                }
            }
        }
        if (clickedElementPrevious != null) {
            var previousTree = null;
            var previousVisible = "";
            if (clickedElementPrevious.id > 0)
                previousTree = TreeUtil.getTreeById(clickedElementPrevious.id);
            if (previousTree) {
                if (previousTree.node) {
                    if (previousTree.node.data) {
                        previousVisible = previousTree.node.data.visible;
                    }
                }
            }
            if (previousVisible != "false") {
                clickedElementPrevious.style["background"] = "#ffffff";
                clickedElementPrevious.style["color"] = "#000000";
            }
            else {
                clickedElementPrevious.style["background"] = "#808080";
                clickedElementPrevious.style["color"] = "#000000";
            }
        }
        if (visible != "false") {
            clickedElement.style["background"] = "#3366CC";
            clickedElement.style["color"] = "#ffffff";
        }
        else {
            clickedElement.style["background"] = "#808080";
            clickedElement.style["color"] = "#ffffff";
        }
        clickedElementPrevious = clickedElement;
        clickedIdPrevious = id;

        var name = clickedElement.innerText;
        var folder = new Asset();
        folder.set("name", name);
        folder.set("id", id);
        folder.set("folderid", __EkFolderId);
        __EkFolderId = -1;
    }

    function onToggleClick(id, callback, args) {
        toolkit.getAllSubCategory(id, -99, callback, args);
    }

    function makeElementEditable(element) {
        element.contentEditable = true;
        element.focus();
        element.style.background = "#fff";
        element.style.color = "#000";
    }

    var baseUrl = URLUtil.getAppRoot(document.location) + "images/ui/icons/tree/";
    TreeDisplayUtil.plusclosefolder = baseUrl + "taxonomyCollapsed.png";
    TreeDisplayUtil.plusopenfolder = baseUrl + "taxonomyCollapsed.png";
    TreeDisplayUtil.minusclosefolder = baseUrl + "taxonomyExpanded.png";
    TreeDisplayUtil.minusopenfolder = baseUrl + "taxonomyExpanded.png";
    TreeDisplayUtil.folder = baseUrl + "taxonomy.png";

    var g_menu_id = "";
    function displayCategory(categoryRoot) {
        document.body.style.cursor = "default";
        var taxonomyTitle = null;
        try {
            taxonomyTitle = categoryRoot.title;
            g_menu_id = categoryRoot.id;
        } catch (e) {
            ;
        }

        if (taxonomyTitle != null) {
            treeRoot = new Tree(taxonomyTitle, __TaxonomyOverrideId, null, categoryRoot, 0);
            TreeDisplayUtil.showSelf(treeRoot, document.getElementById("TreeOutput"));
            TreeDisplayUtil.toggleTree(treeRoot.node.id);
        } else {
            var element = document.getElementById("TreeOutput");
            var debugInfo = "<b>Cannot connect to the service</b>";
            element.innerHTML = debugInfo;
        }
    }

    var toolkit = new EktronToolkit();
    toolkit.getTaxonomy(__TaxonomyOverrideId, -99, displayCategory, __TaxonomyOverrideId);

    function reloadTreeRoot(id) {
        TREES = {};
        toolkit.getTaxonomy(id, -99, displayCategory, __TaxonomyOverrideId);
    }

    var g_selectedFolderList = "0";
    var g_timerForFolderTreeDisplay;
    function showSelectedFolderTree() {
        if (g_timerForFolderTreeDisplay) {
            window.clearTimeout(g_timerForFolderTreeDisplay);
        }
        g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
    }

    function showSelectedFolderTree_delayed() {
        var bSuccessFlag = false;
        if (g_timerForFolderTreeDisplay) {
            window.clearTimeout(g_timerForFolderTreeDisplay);
        }

        if (g_selectedFolderList.length > 0) {
            var tree = TreeUtil.getTreeById(g_menu_id);
            if (tree) {
                var lastId = 0;
                var folderList = g_selectedFolderList.split(",");
                bSuccessFlag = TreeDisplayUtil.expandTreeSet(folderList);
            }

            if (!bSuccessFlag) {
                g_timerForFolderTreeDisplay = setTimeout("showSelectedFolderTree_delayed();", 100);
            }
        }
    }
</script>
          
          
          <%--//---------------- treejs.inc ends here--%>
            <script type="text/javascript">
                <!--//--><![CDATA[//><!--
                 g_selectedFolderList = '<%=_SelectedTaxonomyList%>';
                var taxonomytreemode="editor";var ____ek_appPath2="";
                //--><!]]>
            </script>
        </div>
    </form>
  </body>
</html>
