<%@ Page Language="C#" AutoEventWireup="true" Inherits="email" validateRequest="false" CodeFile="email.aspx.cs" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <title>email</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1"/>
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1"/>
    <meta name="vs_defaultClientScript" content="JavaScript"/>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5"/>
	<script type="text/javascript">
	<!--//--><![CDATA[//><!--
		function IsBrowserIE() {
		    if (window.location.href.indexOf('override_ie=true') > 0)
		        return false;

	        if (null == document.getElementById("EmailFrameContainer"))
	            return false;

			// document.all is an IE only property
			return (document.all ? true : false);
        }
        
		// Hides (or closes) the email window:
		function closeEmailChildPage()
		{
			if (!IsBrowserIE())
			{
				//window.close(); // For Netscape, this is running in a popup-window.
				window.setTimeout(function() { window.close(); }, 100);
			}
			if ((typeof(parent) != "undefined")
				&& (typeof(parent.CloseEmailChildPage) != "undefined"))
			{
				parent.CloseEmailChildPage();
			}
			else if (!IsBrowserIE())
			{
				window.close(); // For Netscape, this is running in a popup-window.
			}
		}

		// Called when the user clicks the "Send" button...
		// Saves users data from eWebEditPro to the Form:
		function submit_form()
		{
		    __doPostBack("send", "");		   
		}

		Ektron.ready( function() {
		    $ektron("table#RadEWrappermessage").addClass("ektronBorder");
		});

	//--><!]]>
	</script>
	<style type="text/css">
	    <!--/*--><![CDATA[/*><!--*/
	    .edit { margin-left: .25em; }
	    .indentFrame { margin:0 .25em 0 .5em; }
	    table.maxWidth { width: 460; }
	    .contentDesignerWrapper {padding: .25em 0;}
        .ui-state-error span.warning {background-image: url('images/ui/icons/error.png'); background-position: 0 0; float: left; margin: .1em .5em .1em .25em;}
	    /*]]>*/-->
	</style>
</head>
  <body>
    <form id="Form1" runat="server" class="indentFrame">
		<asp:Literal ID="TD_msg" runat="server" />
		<asp:Literal ID="EmailData" Runat="server" />
		<asp:RegularExpressionValidator ID="RegExpValidator" runat="server" ControlToValidate="message"></asp:RegularExpressionValidator>
		<div class="contentDesignerWrapper ui-helper-clearfix">
		    <ektron:ContentDesigner ID="message" runat="server" AllowScripts="false" Height="300" Width="100%" Toolbars="Minimal" ShowHtmlMode="false" Visible="false" />
		</div>
		<asp:Literal ID="EmailData2" Runat="server" />
        <div id="dvButtons" visible="false"  runat="server">
            <ul class="buttonWrapper buttonWrapperLeft ektronTopSpace ui-helper-clearfix">
                <li>
                    <asp:LinkButton  ID="btnSendMail" CssClass="button buttonInlineBlock greenHover buttonSendMail" PostBackUrl="email.aspx"  runat="server" />
               </li>
               <asp:Literal ID="ltrCancel" Runat="server" />
           </ul>
        </div>
       
	</form>
  </body>
</html>

