<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewApprovalContent" CodeFile="ViewApprovalContent.ascx.cs" %>
<script type="text/javascript" language="javascript">
		
		function PopEditWindow(URLInfo, Name, a, b, c, d) {
			EditHandle = PopUpWindow(URLInfo, Name, a, b, c, d);
			top.SetEditor(EditHandle);
		}
			
		function SetMetaComplete(Complete, ID) {
			top.SetMetaComplete(Complete, ID);
		}
		//function openComment(str)
		//{
			//	window.open("taskcomment.aspx?ref_type=T&ref_id=-9999&cid="+str,"cmt_win","width=650,height=350,resizable,scrollbars,status,titlebar");
		//}

		function IsBrowserIE() {
			// document.all is an IE only property
			return (document.all ? true : false);
		}

		function PopUp_ApprovalWindow(url, hWind, nWidth, nHeight, nScroll, nResize) {
			var cToolBar = 'toolbar=0,location=0,directories=0,status=' + nResize + ',menubar=0,scrollbars=' + nScroll + ',resizable=' + nResize + ',width=' + nWidth + ',height=' + nHeight;
			var popupwin = window.open(url, hWind, cToolBar);
			return popupwin;
		}

		function LoadChildPage(str) {						
				if (IsBrowserIE()) {
					var frameObj = document.getElementById("ChildPage");
					var height = document.parentWindow.screen.height/100 *60;
					var width = document.parentWindow.screen.height/100 *50 ;
					frameObj.src = "AddTaskComment.aspx?" + str + "&height=" + height + "&width=" + width + "&declinecontent=true";
					var pageObj = document.getElementById("FrameContainer");
					pageObj.style.display = "";
					pageObj.style.width = width;
					pageObj.style.height = height;
				}
				else {
				PopUp_ApprovalWindow("blankredirect.aspx?AddTaskComment.aspx?" + str + "&height=490&width=500&declinecontent=true","Approval",490,500,1,1);
				}
		}
		
		function ReturnChildValue(str) {
			// take value, store it, write to display			
			var pageObj = document.getElementById("FrameContainer");
			pageObj.style.display = "none";
			pageObj.style.width = "1px";
			pageObj.style.height = "1px";
			
			document.location.href = "approval.aspx?"+str
		}
		
		function DeclineContent(id, fldid, page, LangType)
		{
		    var redUrl = 'approval.aspx?action=declineContentAction&id=' + id + '&fldid=' + fldid + '&page=' + page + '&LangType=' + LangType;
		    document.getElementById('dataBox').style.display = "block";
		    document.getElementById('mainview').style.display = "none";
		    document.getElementById('dcid').value = id;
		    document.getElementById('dcfldid').value = fldid;
		    document.getElementById('dcpage').value = page;
		    document.getElementById('dcLangType').value = LangType;
		    ShowPane('none');
		    //return false;
		}
		function DeclineContent2(declineComment)
		{
		    var id = document.getElementById('dcid').value;
		    var fldid = document.getElementById('dcfldid').value;
		    var page = document.getElementById('dcpage').value;
		    var LangType = document.getElementById('dcLangType').value;
		    var redUrl = 'approval.aspx?action=declineContentAction&id=' + id + '&fldid=' + fldid + '&page=' + page + '&LangType=' + LangType;
		    if( declineComment == null ) {
		        declineComment = '';
		    }
		    redUrl += '&comment=' + escape(declineComment);
		    window.location = redUrl;
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
</script>
<script type="text/javascript" language="javascript">
	function ShowPane(tabID) {
	var arTab = new Array("dvContent", "dvSummary", "dvMetadata", "dvProperties", "dvComment", "dvAlias","dvTaxonomy");
	var dvShow; //tab
	var _dvShow; //pane
	var dvHide;
	var _dvHide;
	for (var i=0; i < arTab.length; i++) {
		if (tabID == arTab[i]) {				
			dvShow = eval('document.getElementById("' + arTab[i] + '");');						
			_dvShow = eval('document.getElementById("_' + arTab[i] + '");');						
		} else {	
			dvHide = eval('document.getElementById("' + arTab[i] + '");');
			if (dvHide != null) {
				dvHide.className = "tab_disabled";
			}
			_dvHide = eval('document.getElementById("_' + arTab[i] + '");');
			if (_dvHide != null && _dvHide.style != null) {
				_dvHide.style.display = "none"; 
			}
		}
	}
	if (_dvShow != null && _dvShow.style != null)
	{
	    _dvShow.style.display = "block"; 
	}
	if (dvShow != null)
	{
	    dvShow.className = "tab_actived";
	}
}
</script>
<div id="FrameContainer" style="DISPLAY: none; Z-INDEX: 2; LEFT: 55px; WIDTH: 1px; POSITION: absolute; TOP: 110px; HEIGHT: 1px; BACKGROUND-COLOR: white">
	<iframe id="ChildPage" name="ChildPage" frameborder="1" marginheight="0" marginwidth="0"
		width="100%" height="100%" scrolling="auto"></iframe>
</div>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"/>
    <div class="ektronToolbar" id="divToolBar" runat="server"/>
</div>
<div id="mainview" class="ektronPageContainer ektronPageInfo ektronPageTabbed" style="z-index:-1;">
	    <asp:Literal id="litViewContent" runat="server"></asp:Literal>
</div>
<div id="dataBox" onclick="event.cancelBubble=true;" style="border-width: 1px; display:none; z-index:200; width: 45em;">
			    <br /><asp:Literal ID="ltr_decline" runat="server"/>
                <input name="dataBoxText" type="text" id="dataBoxText" size="45" />
                <a style="margin-top:-21px !important" class="button greenHover buttonRight buttonOk" onclick="DeclineContent2(document.getElementById('dataBoxText').value);" >Ok</a>
                <!--<input type="button" value="cancel" onclick="RuleWizardManager.hideInputForm();" />-->
                <input type="hidden" id="dcid" />
                <input type="hidden" id="dcfldid" />
                <input type="hidden" id="dcpage" />
                <input type="hidden" id="dcLangType" />
        </div>
<asp:Literal ID="litAssetControl" runat="server"></asp:Literal>
