<%@ Page Language="C#" AutoEventWireup="true" Inherits="approval" validateRequest="false" CodeFile="approval.aspx.cs" %>
<%@reference Control ="controls/approval/viewapprovalcontent.ascx" %>
<%@reference Control ="controls/approval/viewapprovallist.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
	    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
		<title title="Approvals">Approvals</title>
		<asp:Literal id="litStyleSheetJS" runat="server" />
		<style type="text/css" >
        .ektronPageContainer
        {
	        position: absolute;
	        left:0 !important;
	        right:0 !important;
	        top: 87px !important;
	        bottom:0 !important;
	        overflow: auto;
        }

        </style>
		<script type="text/javascript">
		    <!--//--><![CDATA[//><!--
			//hide the drag and drop uploader ////
			if (typeof top.HideDragDropWindow != "undefined")
			{				
				var dragDropFrame = top.GetEkDragDropObject();
				if (dragDropFrame != null) {
					dragDropFrame.location.href = "blank.htm";
				}
				top.HideDragDropWindow();
			}
			//////////////////////////////////////
			function CloseChildPage() {
	
				if (document.getElementById("EmailFrameContainer") != null) {
					var pageObj = document.getElementById("EmailFrameContainer");
				}
				else {
					if (parent.document.getElementById("EmailFrameContainer") != null) {
						var pageObj = parent.document.getElementById("EmailFrameContainer");
					}
				}
				// Configure the email window to be invisible:
				pageObj.style.display = "none";
				pageObj.style.width = "1px";
				pageObj.style.height = "1px";

				// Ensure that the transparent layer does not cover any of the parent window:
				pageObj = document.getElementById("EmailActiveOverlay");
				
				if (document.getElementById("EmailActiveOverlay") != null) 
				{
					var pageObj = document.getElementById("EmailActiveOverlay");
				}
				else 
				{
					if (parent.document.getElementById("EmailActiveOverlay") != null) 
					{
						var pageObj = parent.document.getElementById("EmailActiveOverlay");
					}
				}
				pageObj.style.display = "none";
				pageObj.style.width = "1px";
				pageObj.style.height = "1px";
			}
			//--><!]]>
			noDataPrintMsg = "<asp:Literal runat='server' ID='noDataPrintMsg' /> ";
		</script>
	</head>
	<body>
		<form id="frmMain" method="post" runat="server">
			<input type="hidden" name="rptHtml" id="rptHtml"/>
			<input type="hidden" name="rptTitle" id="rptTitle"/>
			<asp:literal id="EmailArea" runat="server" />
			<asp:PlaceHolder ID="DataHolder" Runat="server"/>
		</form>
	</body>
</html>

