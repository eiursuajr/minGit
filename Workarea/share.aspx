<%@ Page Language="C#" AutoEventWireup="true" CodeFile="share.aspx.cs" Inherits="Workarea_share" %>

<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register tagprefix="ux" tagname="login" src="controls/login/uxLogin.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Share this Page</title>
    <style type="text/css">
        #loginControlPanel table 
        {
        	width: 100%;
        }
        p.intro
        {
        	background: #e47e0f url( 'images/UI/Grid/headerBackground.png' ) repeat-x;
        	font-weight: bold;
        	color: #fff;
        	font-size: 1.2em;
        	padding: .5em;
        	margin-bottom: .5em;
        }
        
        #loginControlPanel ul
        {
        	margin-bottom: 0em;
        	list-style-type: none;
        }
        
        #loginControlPanel ul li
        {
        	margin-bottom: .5em;
        }
        
        #loginControlPanel ul li label
        {
        	text-align: right;
        	display: block;
        	float: left;
        	width: 15em;
        	padding-right: 1em;
        }
        
        #loginControlPanel ul li div
        {
        	float: left;
        	width: 50%;
        }
        
        #loginControlPanel ul li div .inputBox
        {
        	width: 15em;
        	font-weight: normal;
        	border: solid 1px #ccc;
        	padding: 0 .25em;
        	height: 1.25em;
        }
        
        input.inputLoginButton
        {
            margin-left: 15.75em;	
        }
         
        input.inputButton
        {
        	padding: .4em 1em;
        	cursor: pointer;
        }
        
        .errorMessage, .shareSuccessMessage
        {
            margin: 0 1em 1em;
            padding: 0;
        }
        
        .errorMessage h2
        {
        	margin-left: 1.5em;
        	color: #fff;
        	font-weight: bold;
        	margin-bottom: 0em;
        }
        
        .errorMessage div div
        {
        	margin-left: 2em;
        }
        
        .errorMessage .errorIcon
        {
        	float: left; 
        	margin: 0.2em 0.3em; 
        }
        
        .invalidField
        {
        	border: solid 1px #D12F19;
        	background-color: #FBE3E4;
        }
        
        .forceWrap 
        {
        	word-break: break-all; 
        	-moz-binding: url('csslib/xml/moz_wordwrap.xml#wordwrap');
        }
        
        .commentsWrapper
        {
        	margin: 1em 2em;
        }
        
        div.siteInfo span.url, div.siteInfo a.pageDescription 
        {
        	font-size: .9em;
        	color: #333;
        }
        
        div.siteInfo span.url
        {
        	display: block;
        	margin-bottom: .5em;
        	color: #777;
        }
        
        div.siteInfo a.pageTitle
        {
        	color: #333;
        	font-weight: bold;
        }        
        
        div.siteInfo .edit 
        {
        	cursor: pointer;
        }
        
        div.siteInfo .edit:hover
        {
        	text-decoration: underline;
        	background-color: #f7eda1;
        }
        
        div.commentsWrapper .editField
        {
        	width: 100%;
        }
        
        div.commentsWrapper textarea
        {
        	font-family: Arial,Tahoma,sans-serif;
        	font-size: 1em;
        	overflow-x: hidden;
        	overflow-y: scroll;
        	height: 5em;
        	width: 100%;
        }
        
        ul.shareControlButtons
        {
            border-top: solid 1px #e47e0f;
            padding: .5em 0;
            margin: .5em 0;
            float: none;
            list-style-type: none;
        }
        
        ul.shareControlButtons li
        {
        	float: right;
        	padding: 0 .5em 0 0;
        }
        
        .shareSuccessMessage h2
        {
        	padding: 0 .5em;
        	font-size: 1em;
        }
        
        .windowClosingWrapper
        {
        	padding: .25em .5em;
        }
        
        div.shareSuccessMessage .closeWindowLink
        {
        	color: #235478; 
        	font-size: .9em;
        }
    </style>
    <script type="text/javascript">
        $ektron(document).ready(function()
        {       
            // add hover effects for the inputButtons
            $ektron(".inputButton").hover(
                function()
                {
                    $ektron(this).addClass("ui-state-hover");
                },
                function()
                {
                    $ektron(this).removeClass("ui-state-hover");
                }
            );
            // add inputLabel functionality to the comments field
            $ektron(".commentsText").inputLabel();
            // bind editable fields functionality
            $ektron(".edit").click(function()
            {
                var textToEdit = $ektron(this);
                var editText = textToEdit.text();
                var hiddenField = textToEdit.next();
                var textarea = textToEdit.is(".editModeTextarea");  
                if (textarea)
                {
                    textToEdit.after("<textarea class='editField'></textarea>");
                }
                else
                {           
                    textToEdit.after("<input class='editField' type='text' />");
                }
                textToEdit.hide();
                var input = textToEdit.next();
                // set the input field to have the same MaxLength as the hiddenField
                
                input.val(editText);
                input[0].focus();
                input[0].select();
                // bind blur event to modify the text and return everythign to normal again
                input.bind("blur", function(event)
                {
                    var inputField = $ektron(this);
                    var fieldValue = inputField.val();
                    var textToEdit = inputField.prev();
                    // update the span and hidden textbox
                    textToEdit.html(fieldValue);
                    // update hiddenField with new fieldValue;
                    hiddenField[0].value = fieldValue;
                    // remove the inputField
                    inputField.remove();                    
                    // reveal the changes
                    textToEdit.show();
                });
                // bind the return key to cause blur effect.
                input.keyup(function(e)
                {
                    if(e.keyCode == 13)
                    {
                        $ektron(this)[0].blur();
                    }   
                });
                return false;
             });
             // limit textareas to specified character limit
             $ektron("textarea.editField").live("keypress", function(event)
             {
                var key = event.which;
        	 
    	        // check all key codes above 32 (space) + return key
                if(key >= 32 || key == 13) 
                {
    	            var maxLength = <asp:Literal ID="maxLength" Text="200" runat="server" />;
    	            var length = this.value.length;
    	            if(length >= maxLength) 
    	            {
    	                event.preventDefault();
    	            }
    	        }
	        });
        });
        
        function validateShare()
        {
            var result = false;
            var comments = $ektron(".commentsText");
            var titleField = $ektron(".pageTitleField").val();
            var url = $ektron(".pageUrlField").val();
            var descriptionField = $ektron(".pageDescriptionField").val();
            var invalidFields = $ektron(".invalidField")
            var errorMessage = $ektron(".shareFormPanel .errorMessage");
            var totalLength = (comments.val() + "\n\n" + titleField + "\n" + url + "\n" + descriptionField).length;
            
            // reset any previous errors reported
            errorMessage.html("");
            invalidFields.removeClass("invalidField");
            if (totalLength > 2000)
            {
                var characterLimitHeader = '<asp:Literal ID="characterLimitExceededHeader" runat="server" />';
                var characterLimitBody = '<asp:Literal ID="characterLimitExceededBody" runat="server" />';
                errorMessage.html('<div class="ui-state-error ui-corner-all ui-helper-clearfix"><span class="ui-icon ui-icon-alert errorIcon"></span><h2>' + characterLimitHeader + '</h2><div>' + characterLimitBody + '</div></div>');
            }
            else
            {
                result = true;
            }            
            return result;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel ID="loginControlPanel" CssClass="loginControlPanel" runat="server">
        <ux:login ID="login" runat="server" />
    </asp:Panel>    
    <asp:Panel ID="shareFormPanel" CssClass="shareFormPanel" runat="server" Visible="false">
        <p class="intro" title="Post to Profile"><asp:Literal ID="shareHeader" runat="server" /></p>
        <div class="ui-widget errorMessage"></div>
        <asp:Panel ID="commentsWrapper" CssClass="commentsWrapper" runat="server">
            <asp:Label AssociatedControlID="comments" runat="server" ID="commentsLabel" />
            <asp:TextBox ToolTip="Enter your Post here" TextMode="MultiLine" CssClass="commentsText" ID="comments" runat="server" />
            <div class="siteInfo">
                <a href="JavascriptRequired.aspx" class="pageTitle edit"><asp:Literal ID="pageTitle" runat="server" /></a>
                <asp:TextBox ID="pageTitleField" MaxLength="255" runat="server" CssClass="pageTitleField ui-helper-hidden" />
                <span class="url forceWrap"><asp:Literal ID="pageUrl" runat="server" /></span>
                <asp:TextBox ID="pageUrlField" CssClass="pageUrlField ui-helper-hidden" runat="server" />
                <div>
                    <a class="pageDescription edit editModeTextarea"><asp:Literal ID="pageDescription" runat="server" /></a>
                    <asp:TextBox ID="pageDescriptionField" TextMode="MultiLine" CssClass="pageUrlField ui-helper-hidden" runat="server" />
                </div>                
            </div>
            <ul class="ui-helper-clearfix shareControlButtons">
                <li>
                    <asp:Button ToolTip="Cancel Post" ID="cancelPost" CssClass="ui-state-default ui-corner-all inputButton" OnClientClick="self.close(); return false;" runat="server" />
                </li>
                <li>
                    <asp:Button ToolTip="Share Post" ID="sharePost" 
                        CssClass="ui-state-default ui-corner-all inputButton" 
                        OnClientClick="return validateShare();" runat="server" 
                        onclick="sharePost_Click" />
                </li>
            </ul>
        </asp:Panel>
    </asp:Panel>
    <asp:Panel ID="shareSuccess" CssClass="shareSuccess" Visible=false runat="server">
        <div class="ui-state-highlight ui-widget shareSuccessMessage">
            <h2 class="ui-widget-header ui-corner-all"><asp:Literal ID="shareSuccessHeader" runat="server" /></h2>
            <div class="windowClosingWrapper">
                <asp:Literal ID="shareSuccessWindowClosing" runat="server" />
            </div>
        </div>
    </asp:Panel>
    </form>
</body>
</html>
