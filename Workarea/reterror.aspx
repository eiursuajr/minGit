<%@ Page Language="C#" AutoEventWireup="true" Inherits="reterror" validateRequest="false" CodeFile="reterror.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">        
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>Error</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1" />
    <meta name="vs_defaultClientScript" content="JavaScript" />
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <asp:literal id="StyleSheetJS" runat="server"/>
    <script type="text/javascript">
	<!--//--><![CDATA[//><!-
	// Adjusts the navigation-tree frame (if function exists; ie workarea).
	// (True Shows the nav-tree, False hides it)
	function ResizeFrame(val) {
		if ((typeof(top.ResizeFrame) == "function") && top != self) {
			top.ResizeFrame(val);
		}
	}

		function ShowReferrer() {
			var objRefEl = document.getElementById('referrer_text');
			if ((typeof(objRefEl)).toLowerCase() == 'object') {
				objRefEl.innerText = '(Referrer: ' + document.referrer + ')';
			}
		}

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
		function SubmitForm(Validate) {
			if (Validate.length > 0) {
				if (eval(Validate)) {
					document.forms[0].submit();
					return false;
				}
				else {
					return false;
				}
			}
			else {
				document.forms[0].submit();
				return false;
			}
		}
	//--><!]]>
	</script>
	<style type="text/css">	  
	    .exception
	    {
	        background-color:#FBE3E4;
	        border: 1px solid #FBC2C4;
	        color: #D12F19;
	        display: block;
	        margin: 0.25em;
	        padding: 0;
	        background-image: url('images/ui/icons/error.png');
	        background-repeat: no-repeat;
	        background-position: .25em .25em
	    }

	    .exception td {padding: .25em 0 .25em 1.75em;}	    
	</style>
  </head>
  <body onload="ShowReferrer();">
    <form id="Form1" method="post" runat="server">
	<div id="dhtmltooltip"></div>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" title="The following error has occurred" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    </div>
    <div class="ektronPageContainer ektronPageInfo">
        <table width="100%" class="exception">
		    <tr>
			    <td id="td_error" runat="server" />
		    </tr>
		    <tr>
			    <td>&nbsp;</td>
		    </tr>
		    <tr>
			    <td title="Referrer" id="referrer_text">&nbsp;</td>
		    </tr>
	    </table>
	</div>
	<input type="hidden" name="back" value="back" id="Hidden1" />

    <script type="text/javascript">
	<!--//--><![CDATA[//><!--
		ResizeFrame(1);
	//--><!]]>
    </script>

    </form>
  </body>
</html>
