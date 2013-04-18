<%@ Register TagPrefix="telerik" NameSpace="Ektron.Telerik.WebControls.EditorControls" Assembly="Ektron.RadEditor" %>
<%@ Register TagPrefix="telerik" TagName="CssClassSelector" Src="./CssClassSelector.ascx" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="LinkManagerControl.ascx.cs" Inherits="Ektron.Telerik.WebControls.EditorControls.LinkManagerControl" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<div id="TabbedHyperlink">
	<div class="Ektron_TopLabel">
	    <label title="Link Url" for="<%=this.ClientID%>_linkUrl"><script>localization.showText('LinkUrl')</script></label>
	</div>
	<input title="Enter Link Url here" type="text" id="<%=this.ClientID%>_linkUrl" value="http://" class="Ektron_StandardTextBox" />
	<!-- Ektron starts --><button title="Open" id='<%=this.ClientID%>_dialogOpenerButton' onclick=" return CallLibrary();">
&nbsp;...&nbsp;</button><!-- Ektron ends -->
	<div class="Ektron_TopLabel">
	    <label title="Link To Anchor" for="<%=this.ClientID%>_pageAnchorsHolder"><script>localization.showText('LinkToAnchor')</script></label>
	</div>
	<div class="Ektron_TopLabel">
	    <select id="<%=this.ClientID%>_pageAnchorsHolder" onchange="<%=this.ClientID%>.SetLinkToAnchor(this);">								
		    <script>
			    document.write('<option selected value="">'	+ localization.getText('AnchorNone') + '</option>');								
		    </script>
	    </select><!-- Ektron starts -->
	    <label for="<%=this.ClientID%>_typeInBookmark"> <script>localization.showText('Or')</script> # </label><input type="text" id="<%=this.ClientID%>_typeInBookmark" /><!-- Ektron ends -->
	</div>
	<div id="<%=this.ClientID%>_rowLinkText">
		<div class="Ektron_TopLabel">
			<label title="Link Text" for="<%=this.ClientID%>_linkText"><script>localization.showText('LinkText')</script></label>
		</div>
		<input title="Enter Link Text here" type="text" id="<%=this.ClientID%>_linkText" class="Ektron_StandardTextBox" />
	</div>
	<div class="Ektron_TopLabel">
		<label title="Link Type" for="<%=this.ClientID%>_linkType"><script>localization.showText('LinkType')</script></label>
	</div>
	<select title="Select a Link Type Option From the List below" class="Text" id="<%=this.ClientID%>_linkType" onchange="<%=this.ClientID%>.ChangeLinkType(this.value);">
		<option value="">
			<script>localization.showText('Other')</script>
		</option>
		<option title="Select file Option" value="file://">file:</option>
		<option title="Select ftp Option" value="ftp://">ftp:</option>
		<option title="Select gopher Option" value="gopher://">gopher:</option>
		<option title="Select https Option" value="http://" selected>http:</option>
		<option title="Select https Option" value="https://">https:</option>
		<option title="Select javascript Option" value="javascript:">javascript:</option>
		<option title="Select news Option" value="news:">news:</option>
		<option title="Select telnet Option" value="telnet:">telnet:</option>
		<option title="Select wais Option" value="wais:">wais:</option>
	</select>
	<div class="Ektron_TopLabel">
	    <label title="Link Target" for="<%=this.ClientID%>_linkTarget"><script>localization.showText('LinkTarget')</script></label>
	</div>
    <input type="text" title="Enter Link Target here" id="<%=this.ClientID%>_linkTarget" readonly="readonly" />
    <select id="<%=this.ClientID%>_linkTargetSelector" class="DropDown" onchange="<%=this.ClientID%>.ChangeLinkTarget(this);">
        <script>
        document.write(														
            '<option selected value="">' + localization.getText('Target') + '</option>' + 
            '<option value="_blank">' + localization.getText('_blank') + '</option>' + 
            '<option value="_parent">' + localization.getText('_parent') + '</option>' + 
            '<option value="_self">' + localization.getText('_self') + '</option>' + 
            '<option value="_top">' + localization.getText('_top') + '</option>' + 
            '<option value="_search">' + localization.getText('_search') + '</option>' + 
            '<option value="_media">' + localization.getText('_media') + '</option>'
        );
        </script>								
    </select>		            
	<div class="Ektron_TopLabel">
	    <label title="Link Tooltip" for="<%=this.ClientID%>_titleText"><script>localization.showText('LinkTooltip')</script></label>
	</div>
	<table cellpadding="0" cellspacing="0">
		<tr>
			<td><input title="Enter Link Tooltip Text here" type="text" id="<%=this.ClientID%>_titleText" class="Ektron_StandardTextBox" /></td>
			<td width="20" style="PADDING-LEFT:5px"><telerik:editorschemeimage relsrc="Dialogs/Accessibility.gif" id="constrainTop" runat="server" /></td>
		</tr>
	</table>
	<div class="Ektron_TopLabel">
	<label title="CssClass"><script>localization.showText('CssClass');</script></label>
	</div> 
	<telerik:cssclassselector id="HyperlinkCssClassSelector" cssfilter="A,ALL" width="150px" 
	    popupwidth="240px"	popupheight="160px" runat="server" />
</div>

<div id="TabbedAnchor">
	<div class="Ektron_TopLabel">
	    <label title="Link Name" for="<%=this.ClientID%>_linkName"><script>localization.showText('LinkName')</script></label>
		<input title="Enter Link Name here" type="text" id="<%=this.ClientID%>_linkName" class="Ektron_StandardTextBox"><br />
	<!-- Ektron starts -->
	    <label title="Link To Anchor" for="<%=this.ClientID%>_pageExistingBookmarks"><script>localization.showText('LinkToAnchor')</script></label>
	</div>
    <div id="<%=this.ClientID%>_pageExistingBookmarks" class="Ektron_ScrollDivBlock">
    </div>
	<!-- Ektron ends -->
</div>

<div id="TabbedEmail">
	<div class="Ektron_TopLabel">
		<label title="Link Address" for="<%=this.ClientID%>_address"><script>localization.showText('LinkAddress')</script></label>
	</div>
	<input title="Enter Link Address here" type="text" id="<%=this.ClientID%>_address" class="Ektron_StandardTextBox" />
	<div id="<%=this.ClientID%>_rowEmailText">
		<div class="Ektron_TopLabel">
			<label title="Link Text" for="<%=this.ClientID%>_emailText"><script>localization.showText('LinkText')</script></label>
		</div>
		<input title="Enter Link Text here" type="text" id="<%=this.ClientID%>_emailText" class="Ektron_StandardTextBox" />		
	</div>
	<div class="Ektron_TopLabel">
	    <label title="Link Subject" for="<%=this.ClientID%>_subject"><script>localization.showText('LinkSubject')</script></label>
	</div>
	<input title="Enter Link Subject Text here" type="text" id="<%=this.ClientID%>_subject" class="Ektron_StandardTextBox">
	<div class="Ektron_TopLabel">
		<label title="Css Class"><script>localization.showText('CssClass');</script></label>
	</div>
	<telerik:cssclassselector id="EmailCssClassSelector" cssfilter="A,ALL" width="150px" 
	    popupwidth="240px" popupheight="160px" runat="server" />
</div>

<div class="Ektron_Dialogs_ButtonContainer">
	<button class="Ektron_StandardButton" title="Submit" id="submitButton" onclick="<%=this.ClientID%>.OkClicked();" type="button">
		<script>localization.showText('OK')</script>
	</button>
	<span class="Ektron_LeftSpaceSmall"></span>
	<button class="Ektron_StandardButton" title="Cancel" onclick="<%=this.ClientID%>.CancelClicked();" type="button">
		<script>localization.showText('Cancel')</script>
	</button>
</div>							

<script type="text/javascript" language="javascript">
	var <%=this.ClientID%> = new LinkManager('<%=this.ClientID%>', <%=this.HyperlinkCssClassSelector.ClientID%>, <%=this.EmailCssClassSelector.ClientID%>);
    //Ektron starts
    function CallLibrary()
    {
	    var args = GetDialogArguments();
	    var paramList = EkUtil_parseQuery();
	    var folderId = 0;
	    if(typeof paramList.FolderId != "undefined")
	    {
	        folderId = parseInt(paramList.FolderId, 10);
	    }
	    if (isNaN(folderId)) folderId = 0;
	    var oEditor = null;
	    if (args)
	    {
	        oEditor = (args.editor || parent.GetRadEditor(args.editorID));
	    }
        var callBackParams = {
		    CallLibrary : this
		    , oEditor   : args.editor
	    };
        if (oEditor)
        {
            var editorPage = ""
            if (window.location.search) 
            {
                editorPage = window.location.search.substring(1);;
            }
            if (editorPage.indexOf("editorID=ekImagegalleryDescription") > 0)
            {
                var assetDialogWidth = 525;
                var assetDialogHeight = 400;
            }
            else 
            {
                var assetDialogWidth = 790;
                var assetDialogHeight = 550;
            }
	        var argument = {InternalParameters : oEditor.GetDialogInternalParameters("LinkManager")};
	        oEditor.ShowDialog(
	            oEditor.workareaPath + "mediamanager.aspx?caller=editor&actiontype=library&ldata=0&scope=all&autonav=" + folderId
		        , argument
		        , assetDialogWidth
                , assetDialogHeight
		        , SetLinkCallerValue
		        , callBackParams);
	        return false;
	    }
    }
    function SetLinkCallerValue(retValue, params)
    {
	    if (retValue && retValue.sFilename)
	    {
	        var retString = retValue.sFilename;
	        if (retString.indexOf("</a>") > -1)
	        {
	            var eRetString = $ektron(retString);
	            if (1 == eRetString.length && "A" == eRetString.get(0).tagName) 
                {
                    //need to extract the filename from the thumbnail hyperlink for the link manager dialog.
                    retString = eRetString.attr("title");
                }
            }
		    <%=this.ClientID%>.LinkUrlHolder.value = retString;
            var options = <%=this.ClientID%>.LinkTypeHolder.getElementsByTagName("OPTION");
		    <%=this.ClientID%>.LinkTypeHolder.selectedIndex = 0;
		    for (var i = 1; i < options.length; i++)
		    {
			    if ("http://" == options[i].value)
			    {
				    <%=this.ClientID%>.LinkTypeHolder.selectedIndex = i;
				    break;
			    }
		    }
		    
		    if ("" == <%=this.ClientID%>.LinkTextHolder.value && $ektron(<%=this.ClientID%>.LinkTextRow).is(":visible"))
		    {
		        <%=this.ClientID%>.LinkTextHolder.value = retValue.sCaption;
		    }
		    if ("" == <%=this.ClientID%>.TitleTextHolder.value) 
		    {
		        <%=this.ClientID%>.TitleTextHolder.value = retValue.sCaption;
		    }
	    }
	    document.body.focus();

	    if (<%=this.ClientID%>.onSrcChangeCallback != null) {
		    <%=this.ClientID%>.onSrcChangeCallback();
	    }
    	
	    return false;//SAFARI
    }
//Ektron ends
</script>
