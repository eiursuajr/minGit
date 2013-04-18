<%@ Control Language="C#" AutoEventWireup="true" Inherits="movecontent" CodeFile="movecontent.ascx.cs" %>
<div id="FrameContainer" style="POSITION: absolute; TOP: 68px; LEFT: 55px; WIDTH: 1px; HEIGHT: 1px; DISPLAY: none; z-index:1000; ">
<iframe id="ChildPage" name="ChildPage" frameborder="yes" marginheight="2" marginwidth="2" width="100%" height="100%" scrolling="auto">
</iframe>
</div>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <table class="ektronForm">
	    <tr>
		    <td>
			    <input type="radio" title="Move" id="RadBtnMove" checked="checked" name="RadGrp_MoveCopy" onclick="javascript:MoveClicked();" /><label class="label" for="RadBtnMove"><%=m_refMsg.GetMessage("lbl move")%></label>
		    </td>
	    </tr>
	    <tr>
		    <td>
			    <input type="radio" title="Copy" id="RadBtnCopy" name="RadGrp_MoveCopy" onclick="javascript:CopyClicked();" /><label class="label" for="RadBtnCopy"><%=m_refMsg.GetMessage("lbl copy")%></label>
			    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input type="checkbox" title="Publish Copied Content" id="btn_PublishCopiedContent" name="btn_PublishCopiedContent" checked="true" disabled="true" title="Publish the content after copying" /><label for="btn_PublishCopiedContent">&nbsp;<%=m_refMsg.GetMessage("lbl publish copied content")%></label>
		    </td>
	    </tr>
	    <tr>
		    <td id="tdMoveToFolderList" runat="server"></td>
	    </tr>
	    <tr>
		    <td><span id="folder_xml_difference_warning" style="visibility: hidden; color: red;"></span></td>
	    </tr>
	    <tr>
		    <td>&nbsp;</td>
	    </tr>
	    <tr>
		    <td>
			    <div class="ektronPageGrid">
			        <asp:DataGrid id="MoveContentGrid"
			            runat="server"
			            AutoGenerateColumns="False"
				        Width="100%"
				        EnableViewState="False"
				        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
			        </asp:DataGrid>
			    </div>
		    </td>
	    </tr>

    </table>
    <input type="hidden" id="folder_id" name="folder_id" runat="server"/>
    <input type="hidden" id="contentids" name="contentids" runat="server"/>
    <input type="hidden" id="contentlanguages" name="contentlanguages" runat="server"/>
    <input type="hidden" id="content_id" name="content_id" value="0" runat="server"/>
    <input id="RadBtnMoveCopyValue" type="hidden" value="" name="RadBtnMoveCopyValue" runat="server" />
    <input type="hidden" id="source_folder_is_xml" name="source_folder_is_xml" runat="server"/>
    <input type="hidden" id="target_folder_is_xml" name="target_folder_is_xml" value="0"/>
    <input type="hidden" id="target_folder_id" name="target_folder_id" value="0"/>
    <input type="hidden" name="hdnCopyAll" id="hdnCopyAll" runat="server" value="false" />
        
    <script type="text/javascript" language="JavaScript">
	    <!--
		    var rootFolderIsXml = <%=m_rootFolderIsXml%>;
		    var confirmCopyAll = '<asp:Literal runat="server" id="jsConfirmCopyAll" />';
		    CheckWarnXml(rootFolderIsXml);

		    function checkAll(bChecked){
			    for (var i = 0; i < document.forms.frmContent.elements.length; i++){
				    if (document.forms[0].elements[i].type == "checkbox"){
					    document.forms[0].elements[i].checked = bChecked;
				    }
			    }
		    }

		    function checkMoveForm_Content(currentFolder){
			    var bRet;
			    var SelectedFolder = document.forms[0].move_folder_id.value;
			    var bMove=true;
			    var strMoveCopy = 'move';
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
			    if (bMove && (SelectedFolder == currentFolder)) {
				    alert('Please select a different folder to ' + strMoveCopy + ' this content block to.');
				    return false;
			    } else {
				    if (!ConfirmWarnXml()){
					    return false;
				    }

				    bRet = confirm('Are you sure you want to ' + strMoveCopy + ' this content block?');
			    }
			    if (bRet){
				    document.forms[0].submit();
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

		    function MoveClicked(){
			    var pubBtn = document.getElementById("btn_PublishCopiedContent");
			    if (pubBtn){
				    pubBtn.disabled = true;
			    }
		    }

		    function CopyClicked(){
		    if("<%=m_strContentStatus%>"=="A"){
			    var pubBtn = document.getElementById("btn_PublishCopiedContent");
			    if (pubBtn){
				    pubBtn.disabled = false;
			    }
			    }else{try{document.getElementById('RadBtnMove').checked=true;document.getElementById('RadBtnCopy').checked=false;}catch(e){}alert('CheckIn status content cannot be copied');return false;}
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
				        warnText = "<%=m_refMsg.GetMessage("warn move txt xml to non xml")%>";
				    <%if (folder_data.FolderType != 9)%>
				        warnText = "<%=m_refMsg.GetMessage("warn txt xml to non xml")%>";
				    
			    } else if (!nTargetFolderIsXml && nSourceFolderIsXml){
				    SetObjVisible('folder_xml_difference_warning', true);
				    <%if (folder_data.FolderType == 9) %>
				        warnText = "<%=m_refMsg.GetMessage("warn move txt xml to non xml")%>";
				    <%if (folder_data.FolderType != 9)%>
				        warnText = "<%=m_refMsg.GetMessage("warn txt xml to non xml")%>";
			    } else if (nTargetFolderIsXml && nSourceFolderIsXml){
				    SetObjVisible('folder_xml_difference_warning', true);
				    <%if (folder_data.FolderType == 9) %>
				        warnText = "<%=m_refMsg.GetMessage("warn move txt xml to non xml")%>";
				    <%if (folder_data.FolderType != 9)%>
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
</div>

