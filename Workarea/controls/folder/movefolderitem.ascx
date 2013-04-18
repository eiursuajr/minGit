<%@ Control Language="C#" AutoEventWireup="true" Inherits="movefolderitem" CodeFile="movefolderitem.ascx.cs" %>

<div id="FrameContainer" style="display: none; left: 55px; width: 1px; position: absolute;
    top: 68px; height: 1px; z-index:3000;">
    <iframe id="ChildPage" name="ChildPage" frameborder="yes" marginheight="2" marginwidth="2"
        width="100%" height="100%" scrolling="auto"></iframe>
</div>

<script type="text/javascript">
function resetPostback()
{
    document.forms[0].movefolderitem_isPostData.value = "";
}
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronPageGrid ektronGrid">
        <tr>
            <td>
                <input type="radio" title="<%= m_refMsg.GetMessage("lbl move")%>" id="RadBtnMove" name="RadGrp_MoveCopy" onclick="MoveClicked();"
                    <%=m_strRadBtnMove%> />
                <label title="<%= m_refMsg.GetMessage("lbl move")%>" for="RadBtnMove">
                    <%= m_refMsg.GetMessage("lbl move")%>
                </label>
            </td>
        </tr>
        <tr>
            <td>
                <div id="td_copy" runat="Server">
                   <input type="radio" title=<%= m_refMsg.GetMessage("lbl copy")%> id="RadBtnCopy" name="RadGrp_MoveCopy" onclick="CopyClicked();"
                        <%=m_strRadBtnCopy%> />
                    <label title="<%= m_refMsg.GetMessage("lbl copy")%>" for="RadBtnCopy">
                        <%= m_refMsg.GetMessage("lbl copy")%>
                    </label>
                    <input style="margin:0 0 0 1em;" type="checkbox" id="btn_PublishCopiedContent" name="btn_PublishCopiedContent"
                        checked <%=m_strDisabled%> title="<%= m_refMsg.GetMessage("lbl publish copied content")%>" />
                    <label for="btn_PublishCopiedContent" title="<%= m_refMsg.GetMessage("lbl publish copied content")%>">
                        &nbsp;<asp:literal ID="ltr_publishcopied" runat="server"/></label>
                </div>
            </td>
        </tr>
    </table>
    <div class="ektronTopSpace"></div>
    <table class="ektronGrid">
        <tr>
            <td class="label" title="<%=m_refMsg.GetMessage("lbl destination folder")%>"><%=m_refMsg.GetMessage("lbl destination folder")%></td>
            <td class="value"><asp:Label ToolTip="Destination Folder" ID="lblDestinationFolder" runat="server" /></td>
        </tr>
    </table>

    <!-- TODO: Ross - Move style to CSS -->
    <div id="folder_xml_difference_warning" style="visibility: hidden; color: red;"></div>
    
    <div class="ektronTopSpace"></div>
    <div>
        <div class="ektronPageGrid">
            <asp:DataGrid ID="MoveContentByGategoryGrid" 
                runat="server" 
                AutoGenerateColumns="False"
                GridLines="None" 
                CssClass="ektronGrid ektronBorder"
                Width="100%">
                <HeaderStyle CssClass="title-header" />   
            </asp:DataGrid>
        </div>
    </div>    

<p class="pageLinks">
    <asp:Label runat="server" ID="PageLabel" Visible="false">Page</asp:Label>
    <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
    <asp:Label runat="server" ID="OfLabel" Visible="false">of</asp:Label>
    <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
</p>
<asp:LinkButton runat="server" CssClass="pageLinks" ID="ctrlFirstPage" Text="[First Page]"
    OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()"
    Visible="false" />
<asp:LinkButton runat="server" CssClass="pageLinks" ID="ctrlPreviousPage" Text="[Previous Page]"
    OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()"
    Visible="false" />
<asp:LinkButton runat="server" CssClass="pageLinks" ID="ctrlNextPage" Text="[Next Page]"
    OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()"
    Visible="false" />
<asp:LinkButton runat="server" CssClass="pageLinks" ID="ctrlLastPage" Text="[Last Page]"
    OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()"
    Visible="false" />   

</div> 
                
<input type="hidden" id="isPostData" value="true" name="isPostData" runat="server" />
<input type="hidden" id="folder_id" name="folder_id" runat="server" />
<input type="hidden" id="contentids" name="contentids" runat="server" />
<input type="hidden" id="contentlanguages" name="contentlanguages" runat="server" />
<input type="hidden" id="content_id" value="0" name="content_id" runat="server" />
<input type="hidden" id="RadBtnMoveCopyValue" name="RadBtnMoveCopyValue" runat="server" />
<input type="hidden" id="source_folder_is_xml" name="source_folder_is_xml" runat="server" />
<input type="hidden" id="target_folder_is_xml" name="target_folder_is_xml" value="0" />
<input type="hidden" id="target_folder_id" name="target_folder_id" value="0" />
<input type="hidden" name="hdnCopyAll" id="hdnCopyAll" runat="server" value="false" />

<script type="text/javascript" >
	<!--
		var rootFolderIsXml = <%=m_rootFolderIsXml%>;
		var _pbMsg="<%=pbcAction%>";
        var confirmCopyAll = '<asp:Literal runat="server" id="jsConfirmCopyAll" />';
		CheckWarnXml(rootFolderIsXml);
		
		function checkAll(bChecked){
			for (var i = 0; i < document.forms.frmContent.elements.length; i++){
				if (document.forms.frmContent.elements[i].type == "checkbox" && document.forms.frmContent.elements[i].name!="btn_PublishCopiedContent"){		
					document.forms.frmContent.elements[i].checked = bChecked;
				}										
			}
		}
		
		function checkMoveForm_Folder(currentFolder){
			var bFound = false;
			var bRet;
			var bMove=true;
			var strMoveCopy = 'move'; // we'll default to old behaviour: moving content item.
			var copyBtn = document.getElementById('RadBtnCopy');
			var hiddenInput = document.getElementById(UniqueID + 'RadBtnMoveCopyValue');
			var elObj;
			if (copyBtn != null) {
				if (copyBtn.checked) {
					strMoveCopy = 'copy';
					bMove = false;
					if (hiddenInput != null){
						hiddenInput.value = 'copy';
					}
					if(confirm(confirmCopyAll)) {
				        document.getElementById(UniqueID + 'hdnCopyAll').value = true;
					}
				}
			}
			for (var i = 0; i < document.forms[0].elements.length; i++){
				elObj = document.forms[0].elements[i];
				if ((elObj.type == "checkbox") && (elObj.id != 'btn_PublishCopiedContent')){		
					if (elObj.checked){
						bFound = true;
					}
				}										
			}
			
			if (!ConfirmWarnXml()){
				return false;
			}
			
			if (bFound == false){
			    <%if (folder_data.FolderType == 9) %>
					    alert('<%=m_refMsg.GetMessage("msg sel entry")%>');
				<%if (folder_data.FolderType != 9) %>
					    alert('<%=m_refMsg.GetMessage("msg sel content block")%>');
		        
				return false;
			}
			else{
				var SelectedFolder = document.forms[0].move_folder_id.value;
				<%if (folder_data.FolderType == 9) %>
				if (!bMove && (SelectedFolder == 0 || SelectedFolder == "\\"))
				{
				    alert('Please select a different folder to ' + strMoveCopy + ' the item(s) to.');
					return false;
				}
				else 
				    if (bMove && (SelectedFolder == currentFolder
				    <%if (folder_data.FolderType == 9) %> || SelectedFolder == 0 || SelectedFolder == "\\")) {
					alert('Please select a different folder to ' + strMoveCopy + ' the item(s) to.');
					return false;									
				}
				else {
				     var pbmsg='';
				    if(_pbMsg=="1") pbmsg="\nPlease verify all selected page builder content templates and alias links.";		
				    <%if (folder_data.FolderType == 9) %>
					    bRet = confirm('Are you sure you want to ' + strMoveCopy + ' this entry?');					        
				    <%else %>
					    bRet = confirm('Are you sure you want to ' + strMoveCopy + ' this content block?'+pbmsg);			    
					if (bRet){
						document.forms[0].submit();
					}
				}
			}
		}
		
		function checkAllFalse(){
			document.forms.frmContent.all.checked = false;
		}
		function LoadSelectFolderChildPage()
		{
			if (IsBrowserIE_Email()) 
			{
			
				var frameObj = document.getElementById("ChildPage");
				frameObj.src = "blankredirect.aspx?SelectFolder.aspx?FolderID=0&browser=0&WantXmlInfo=1&noblogfolders=1";
				
				var pageObj = document.getElementById("FrameContainer");
				pageObj.style.display = "";
				pageObj.style.width = "450px";
				pageObj.style.height = "500px";
			}
			else
			{
				// Using Netscape; cant use transparencies & eWebEditPro preperly 
				// - so launch in a seperate pop-up window:
				PopUpWindow("SelectFolder.aspx?FolderID=0&browser=1&WantXmlInfo=1&noblogfolders=1","SelectFolder", 490,500,1,1);
				
			}
		}
		
		function LoadSelectCatalogFolderChildPage()
		{
			if (IsBrowserIE_Email()) 
			{
			
				var frameObj = document.getElementById("ChildPage");
				frameObj.src = "blankredirect.aspx?SelectFolder.aspx?FolderID=0&FolderType=9&browser=0&WantXmlInfo=1&noblogfolders=1";
				
				var pageObj = document.getElementById("FrameContainer");
				pageObj.style.display = "";
				pageObj.style.width = "450px";
				pageObj.style.height = "500px";
			}
			else
			{
				// Using Netscape; cant use transparencies & eWebEditPro preperly 
				// - so launch in a seperate pop-up window:
				PopUpWindow("SelectFolder.aspx?FolderID=0&FolderType=9&browser=1&WantXmlInfo=1&noblogfolders=1","SelectFolder", 490,500,1,1);
				
			}
		}
		
		function ReturnChildValue(folderid, folderpath, targetFolderIsXml)
		{
			// take value, store it, write to display
			CloseChildPage();
			document.getElementById("move_folder_id").value = folderpath;
			document.getElementById("target_folder_id").value = folderid;
			document.getElementById("target_folder_is_xml").value = targetFolderIsXml;
		
			CheckWarnXml(targetFolderIsXml);
		}
		
		
		function CloseChildPage()
		{
			if (IsBrowserIE_Email()) 
			{
				var pageObj = document.getElementById("FrameContainer");
				pageObj.style.display = "none";
				pageObj.style.width = "1px";
				pageObj.style.height = "1px";
			}
					
		}
	
		function IsChildWaiting()
		{
			var pageObj = document.getElementById("FrameContainer");
			if (pageObj == null) 
			{
				return (false);
			}
			if (pageObj.style.display == "")
			{
				return (true);
			}
			else 
			{
				return (false);
			}
		}
		var __href="<%=m_refCopyMoveHref %>";
		function MoveClicked(){
//			var pubBtn = document.getElementById("btn_PublishCopiedContent");
//			if (pubBtn){
//				pubBtn.disabled = true;
//			}
document.location.href=__href+"&op=move";
		}
		
		function CopyClicked(){
//			var pubBtn = document.getElementById("btn_PublishCopiedContent");
//			if (pubBtn){
//				pubBtn.disabled = false;
//			}
document.location.href=__href+"&op=copy";
		}
		
		function CheckWarnXml(targetFolderIsXml){
			var srcFolderIsXml = document.getElementById(UniqueID  + "source_folder_is_xml");
			var objWarnMsgBlock = document.getElementById("folder_xml_difference_warning");
			var nSourceFolderIsXml = parseInt(srcFolderIsXml.value);
			var nTargetFolderIsXml = parseInt(targetFolderIsXml);
			var warnText = "";
			
			if (nTargetFolderIsXml && !nSourceFolderIsXml){
				SetObjVisible('folder_xml_difference_warning', true);
				<%if (folder_data.FolderType == 9) %>
					    warnText = "";					    
				<%else %>
					    warnText = "<%=m_refMsg.GetMessage("warn txt xml to non xml")%>";
			} 
			else if (!nTargetFolderIsXml && nSourceFolderIsXml)
			{
				SetObjVisible('folder_xml_difference_warning', true);
				<%if (folder_data.FolderType == 9) %>
					    warnText = "";
				<%else %>
					    warnText = "<%=m_refMsg.GetMessage("warn txt xml to non xml")%>";
			} else if (nTargetFolderIsXml && nSourceFolderIsXml){
				SetObjVisible('folder_xml_difference_warning', true);
				<%if (folder_data.FolderType == 9) %>
					    warnText = "";					    
				<%else %>
					    warnText = "<%=m_refMsg.GetMessage("warn txt xml to non xml")%>";
			} else {
				SetObjVisible('folder_xml_difference_warning', false);
			}
			objWarnMsgBlock.innerText = warnText;
		}
		
		function ConfirmWarnXml(){
			var objWarnMsgBlock = document.getElementById("folder_xml_difference_warning");
			var strWarnMsg = objWarnMsgBlock.innerText;
			if (strWarnMsg.length){
				return confirm(strWarnMsg);
			}
			return true;
		}

		function SetObjVisible(itemId, flag) {
			var obj;
			if (('string' == typeof(itemId)) && (0 < itemId.length)) {
				obj = document.getElementById(itemId);
				if ((null != obj) && ('undefined' != typeof(obj.style)) && ('undefined' != typeof(obj.style.visibility))) {
					if (flag) {
						obj.style.visibility = "visible";
					} else {
						obj.style.visibility = "hidden";
					}
				}
			}
		}
		
	//-->		
</script>


