<%@ Control Language="C#" AutoEventWireup="true" CodeFile="addsuggestedresult.ascx.cs" Inherits="Workarea_controls_search_addsuggestedresult" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../../controls/Editor/ContentDesignerWithValidator.ascx" %>
<script language="javascript" type="text/javascript"">
<!--
    var suggestedResultRecommendedMaxSize = "<%= _suggestedResultRecommendedMaxSize %>";
//-->
</script>
<asp:Literal ID="PostBackPage" runat="server" />

<div style="display: none; border: 1px; background-color: white; position: absolute;
    top: 48px; width: 100%; height: 1px; margin: 0px auto;" id="dvHoldMessage">
    <table border="1px" width="100%" style="background-color: #fff;">
        <tr>
            <td valign="top" align="center" style="white-space: nowrap">
                <h3 style="color: red">
                    <strong>
                        <%=_messageHelper.GetMessage("one moment msg")%>
                    </strong>
                </h3>
            </td>
        </tr>
    </table>
</div>
<!-- Modal Dialog: Browse CMS Content -->
<div class="ektronWindow ektronCMSContent ektronModalStandard" id="SyncStatusModal" >
    <div class="ektronModalHeader">
        <h3>
            <span class="headerText"><asp:Literal ID="lblCMSContent" runat="server" /></span>
            <asp:HyperLink ID="closeDialogLink3" CssClass="ektronModalClose" runat="server" />
        </h3>
    </div>
    <div class="ektronModalBody">
        <iframe id="ChildPage" name="ChildPage" frameborder="1" marginheight="0" marginwidth="0" width="100%" scrolling="auto"></iframe>
    </div>
</div>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    
    <table class="ektronGrid">
        <tr>
            <td class="label" id="termTypeLabel"><label for="termType" title="Type"><%=_messageHelper.GetMessage("lbl suggested results phrase")%></label></td>
            <td class="value">
                <asp:TextBox ID="txtPhrase" runat="server"></asp:TextBox>                
            </td>
        </tr>
        <tr>
            <td class="label"><label title="Term" for="term" id="termLable"><%=_messageHelper.GetMessage("lbl suggested result synonyms")%></label></td>
            <td class="value">                
                <asp:TextBox ID="txtSynonyms" runat="server"></asp:TextBox>
                <div class="ektronCaption"><%=_messageHelper.GetMessage("msg please enter suggested result phrases")%></div>
            </td>
        </tr>
    </table>
    
    <div id="resultSizeWarning" style="display:none; visibility:hidden;"><p><%=string.Format((string) (_messageHelper.GetMessage("msg suggested results size limit warning")), _suggestedResultRecommendedMaxSize)%></p></div>
    <div class="ektronHeader"><div id="optionsText" title="Click below to see your options"><%=_messageHelper.GetMessage("lbl suggested results options")%></div><%=_messageHelper.GetMessage("lbl suggested results")%></div>
    <div class="suggestedResultsItems" id="suggestedResultsItems" title="Suggested Results">
        <ul class="selectedSuggestedResults" id="selectedSuggestedResults">
        </ul>
    </div>
    <asp:HiddenField ID="submitMode" runat="server" value="1" />
</div>

 <div id="add_edit_SuggestedResult" class="hideElement">
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    </div>
    <div class="ektronPageContainer ektronPageInfo">
        <table class="ektronForm">
            <tr>
                <td class="label"><label title="Link" for="sr_link"><%=_messageHelper.GetMessage("generic link")%>:</label><span class="caption">&nbsp</span></td>
                <td class="value">
                    <input type="text" id="sr_link" name="sr_link" class="suggestedResultLink" />
                    <input type="button" title="Browse Content" name="buttonBrowseContent" id="buttonBrowseContent" value="<%=_messageHelper.GetMessage("btn browse to cms content")%>" onclick="LoadChildPage('<%= _contentLanguage %>')" />
                    <br />
                    <span class="caption" title="To select an external URL, enter its address, e.g., http://www.ektron.com"><%=_messageHelper.GetMessage("lbl to select external url")%></span>
                </td>
            </tr>
            <tr>
                <td class="label"><label for="sr_title" title="Title"><%=_messageHelper.GetMessage("generic title")%>:</label></td>
                <td class="value"><input type="text" title="Title Text" id="sr_title" name="sr_title" class="suggestedResultTitle" /></td>
            </tr>
            <tr>
                <td class="label"><label for="sr_summary" title="Summary"><%=_messageHelper.GetMessage("summary text")%>:</label></td>
                <td>
                    <div id="htmlEditorWrapper">
                        <ektron:ContentDesigner ID="HtmlEditor1" runat="server" AllowScripts="false" Height="350" Width="99%"
                            Toolbars="Minimal" ShowHtmlMode="false" />
                    </div>
                </td>
            </tr>
        </table>
        <input type="hidden" id="sr_contentID" name="sr_contentID" value="" />
        <input type="hidden" id="sr_contentLanguage" name="sr_contentLanguage" value="" />
        <input type="hidden" id="sr_ID" name="sr_ID" value="" />
        <asp:HiddenField ID="hdnOriginalPhrase" runat="server" />
     </div>
 </div>
 <div id="hiddenFormFields"></div>
<!-- Default InContext Menu -->
<div id="defaultSuggestedResultMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a title="Add Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic add title")%></a></li>
        <li class="MenuItem edit"><a title="Edit Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic edit title")%></a></li>
        <li class="MenuItem moveUp"><a title="Move Down" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("lbl move up")%></a></li>
        <li class="MenuItem moveDown"><a title="Move Up" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("lbl move down")%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a title="Remove Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic delete title")%></a></li>
    </ul>
</div>
<!-- InContext Menu for when there are Zero Suggested Results -->
<div id="zeroSuggestedResultsMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a title="Add Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic add title")%></a></li>
        <li class="MenuItem edit"><a title="Edit Title" class="menuLink disabled"><%=_messageHelper.GetMessage("generic edit title")%></a></li>
        <li class="MenuItem moveUp"><a title="Move up" class="menuLink disabled"><%=_messageHelper.GetMessage("lbl move up")%></a></li>
        <li class="MenuItem moveDown"><a title="Move Down" class="menuLink disabled"><%=_messageHelper.GetMessage("lbl move down")%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a title="Delete Title" class="menuLink disabled"><%=_messageHelper.GetMessage("generic delete title")%></a></li>
    </ul>
</div>
<!-- InContext Menu for when there is a single Suggested Result -->
<div id="singleSuggestedResultMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a title="Add Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic add title")%></a></li>
        <li class="MenuItem edit"><a title="Edit Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic edit title")%></a></li>
        <li class="MenuItem moveUp"><a title="Move up" class="menuLink disabled"><%=_messageHelper.GetMessage("lbl move up")%></a></li>
        <li class="MenuItem moveDown"><a title="Move Down" class="menuLink disabled"><%=_messageHelper.GetMessage("lbl move down")%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a title="Delete Title" class="menuLink"  onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic delete title")%></a></li>
    </ul>
</div>
<!-- InContext Menu for when there is a Single Suggested Result -->
<div id="Div2" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a title="Add Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic add title")%></a></li>
        <li class="MenuItem edit"><a title="Edit Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic edit title")%></a></li>
        <li class="MenuItem moveUp"><a title="Move up" class="menuLink disabled"><%=_messageHelper.GetMessage("lbl move up")%></a></li>
        <li class="MenuItem moveDown"><a title="Move Down" class="menuLink disabled"><%=_messageHelper.GetMessage("lbl move down")%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a title="Delete Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic delete title")%></a></li>
    </ul>
</div>
<!-- InContext Menu for the First Suggested Result -->
<div id="firstSuggestedResultMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a title="Add Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic add title")%></a></li>
        <li class="MenuItem edit"><a title="Edit Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic edit title")%></a></li>
        <li class="MenuItem moveUp"><a title="Move up" class="menuLink disabled"><%=_messageHelper.GetMessage("lbl move up")%></a></li>
        <li class="MenuItem moveDown"><a title="Move Down" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("lbl move down")%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a title="Delete Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic delete title")%></a></li>
    </ul>
</div>
<!-- InContext Menu for the Last Suggested Result -->
<div id="lastSuggestedResultMenu" class="inContextMenu Menu">
    <ul>
        <li class="MenuItem add"><a title="Add Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic add title")%></a></li>
        <li class="MenuItem edit"><a title="Edit Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic edit title")%></a></li>
        <li class="MenuItem moveUp"><a title="Move up" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("lbl move up")%></a></li>
        <li class="MenuItem moveDown"><a title="Move Down" class="menuLink disabled"><%=_messageHelper.GetMessage("lbl move down")%></a></li>
        <li class="MenuItem break"></li>
        <li class="MenuItem delete"><a title="Delete Title" class="menuLink" onclick="MenuClickEventHandler(event, this)"><%=_messageHelper.GetMessage("generic delete title")%></a></li>
    </ul>
</div>
<script language="javascript" type="text/javascript">
<!--
    // if editing an existing Suggested Result, this script tag will build the array
    <asp:Literal ID="javaScriptSRObjects" runat="server"></asp:Literal>
//-->
</script>
