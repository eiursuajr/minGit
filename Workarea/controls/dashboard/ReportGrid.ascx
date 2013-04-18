<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReportGrid.ascx.cs" Inherits="Workarea_Widgets_Controls_ReportGrid" %>
<%@ Import Namespace="Ektron.Cms.BusinessObjects.Localization" %>
<%@ Register TagPrefix="Ek" TagName="DatePicker" Src="../generic/date/DatePicker.ascx" %>
<%@ Register TagPrefix="Ek" TagName="DateRangePicker" Src="../generic/date/DateRangePicker.ascx" %>
<asp:Panel runat="server" ID="ReportGrid">
<style>
    ul.modalButtonFix li { float: right; }
    ul.modalButtonFix { list-style-type: none; }
    .ektronWindow {max-height:65%; overflow:scroll; }
    .ektronExportPseudoIframe { width: 100%;}
    span#startdate_span { width:250px; }
</style>
<input type="hidden" id="<%= this.ClientID %>_page" value="" />
<input type="hidden" id="<%= this.ClientID %>_sort" value="" />
<input type="hidden" id="<%= this.ClientID %>_filters" value="" />
<input type="hidden" id="<%= this.ClientID %>_newfilters" value="" />
<input type="hidden" id="<%= this.ClientID %>_selected" value="" />
<script language="javascript" type="text/javascript">
function <%= this.ClientID %>_UpdateParams(page, sort, filters, clearSelections) {
    if (page != null)
        document.getElementById('<%= this.ClientID %>_page').value = page;
    if (sort != null)
        document.getElementById('<%= this.ClientID %>_sort').value = sort;
    if (filters != null) {
        document.getElementById('<%= this.ClientID %>_filters').value = filters;
        document.getElementById('<%= this.ClientID %>_newfilters').value = '';
        var filterArray = ReportGrid_ParseQuery(filters);
        for (var i = 0; i < filterArray._keys.length; i++)
            <%= this.ClientID %>_SetFilter(filterArray._keys[i], filterArray[filterArray._keys[i]], true);
    }
    if (clearSelections) {
        document.getElementById('<%= this.ClientID %>_selected').value = '';
        if (<%= this.ClientID %>_SelectionChanged != null)
            eval(<%= this.ClientID %>_SelectionChanged + '(null, false);');
    }
}
function <%= this.ClientID %>_GetParams() {
    return 'cpage=' + escape(document.getElementById('<%= this.ClientID %>_page').value) +
        '&csort=' + escape(document.getElementById('<%= this.ClientID %>_sort').value) +
        '&cfilters=' + escape(document.getElementById('<%= this.ClientID %>_filters').value) + 
        '&selected=' + document.getElementById('<%= this.ClientID %>_selected').value;
}
function <%= this.ClientID %>_HasSelections(showAlert) {
    //var has = document.getElementById('<%= this.ClientID %>_selected').value != '';
    var eCheckedContent = $ektron("td.col1 > input:checked");
    var has = (eCheckedContent.length > 0);
    if (!has && showAlert)
    {
        document.getElementById('<%= this.ClientID %>_selected').value = '';
        alert('Please make a selection first');
    }
    return has;
}
var <%= this.ClientID %>_SelectionChanged = <%= string.IsNullOrEmpty(ClientSelectionChanged) ? "null" : ("'" + (UseClientIDPrefix ? (this.ClientID + "_") : string.Empty) + ClientSelectionChanged + "'") %>;
function <%= this.ClientID %>_SetSelection(cid, selected) {
    document.getElementById('<%= this.ClientID %>_selected').value = 
        selected ?
            ReportGrid_Select(document.getElementById('<%= this.ClientID %>_selected').value, cid) :
            ReportGrid_Deselect(document.getElementById('<%= this.ClientID %>_selected').value, cid);
    if (<%= this.ClientID %>_SelectionChanged != null)
        eval(<%= this.ClientID %>_SelectionChanged + '(' + cid + ', ' + selected + ');');
}
function <%= this.ClientID %>_Page(page) {
    var params = <%= this.ClientID %>_GetParams();
    params += '&page=' + page;
    window.<%= AddCallback("page", null) %>(params,'');
    return false;
}
function <%= this.ClientID %>_Sort(sort) {
    var params = <%= this.ClientID %>_GetParams();
    params += '&sort=' + sort;
    window.<%= AddCallback("sort", null) %>(params,'');
    return false;
}
function <%= this.ClientID %>_Action(action, addparams) {
    if (!<%= this.ClientID %>_HasSelections(true))
        return false;
    var params = <%= this.ClientID %>_GetParams();
    params += (addparams == null ? '' : ('&' + addparams)) + '&action=' + action;
    window.<%= AddCallback("action", null) %>(params,'');
}
function <%= this.ClientID %>_Filter(filter) {
    var params = <%= this.ClientID %>_GetParams();
    params += '&filters=' + escape(filter);
    window.<%= AddCallback("filter", null) %>(params,'');
    return false;
}
function <%= this.ClientID %>_TransStatusSet() {
    var params = <%= this.ClientID %>_GetParams();
    var form = $ektron('.TransStatusSet');
    var ddl = form.find('select')[0];
    params += '&action=settranstatus&status=' + $ektron(ddl).val();
    window.<%= AddCallback("action", null) %>(params,'');
}
function <%= this.ClientID %>_LocaleAdd() {
    <%= this.ClientID %>_LocaleSet('add');
}
function <%= this.ClientID %>_LocaleDel() {
    <%= this.ClientID %>_LocaleSet('del');
}
function <%= this.ClientID %>_LocaleSet(mode) {
    var params = <%= this.ClientID %>_GetParams();
    window.<%= this.ClientID %>__locales = new Array();
    $ektron('.LangDialects input:checkbox').each(function(index, ele) {
        if (ele != null && ele.checked)
            window.<%= this.ClientID %>__locales.push(ele.value);
    });
    params += '&action=localeset&mode=' + mode + '&locales=' + window.<%= this.ClientID %>__locales.join(',');
    window.<%= AddCallback("action", null) %>(params,'');
}
function <%= this.ClientID %>_NotesSet() {
    var params = <%= this.ClientID %>_GetParams();
    var txt1 = $ektron('.NotesSet textarea');
    params += '&action=setnotes&notes=' + escape(txt1.val());
    window.<%= AddCallback("action", null) %>(params,'');
}
function <%= this.ClientID %>_PseudoSet() {
    var params = <%= this.ClientID %>_GetParams();
    var localeCount = $ektron(':checked[id*=chkPS]').length;
    if (localeCount > 0)
    {
        var pseudoLocales = ''; //$ektron('.NotesSet textarea');    
        for (var i = 0; i < localeCount; i++)
        {
            var pseudoLocale = $ektron(':checked[id*=chkPS]').get(i).id;
            var startFrom = pseudoLocale.indexOf('chkPS') + 5;
            pseudoLocale = pseudoLocale.substr(startFrom, pseudoLocale.length - startFrom);
            if (pseudoLocales == '')
                pseudoLocales += pseudoLocale;
            else
                pseudoLocales += ',' + pseudoLocale;
        }
        params += '&action=setpseudo&locales=' + pseudoLocales;
        $ektron('.cancelButton').hide();
        $ektron('.okButton').text('Ok');
        window.<%= AddCallback("action", null) %>(params,'');
    }
    else
    {
        alert('No pseudo locales are selected.');
    }
}
function <%= this.ClientID %>_PseudoReset() {
    var params = <%= this.ClientID %>_GetParams();
	params += '&action=pseudocomplete';
	<%= this.ClientID %>_CloseModalDialog();
	window.<%= AddCallback("action", null) %>(params,'');
}
var <%= this.ClientID %>_FilterSetCallback = <%= string.IsNullOrEmpty(ClientSetFilterCallback) ? "null" : ("'" + (UseClientIDPrefix ? (this.ClientID + "_") : string.Empty) + ClientSetFilterCallback + "'") %>;
function <%= this.ClientID %>_SetFilter(filter, value, setControl) {
    if (filter == null || filter == '')
        return false;
    var filters = document.getElementById('<%= this.ClientID %>_newfilters').value;
    var filterArray = ReportGrid_ParseQuery(filters);
    ReportGrid_SetQueryValue(filterArray, filter.toLowerCase(), value);
    document.getElementById('<%= this.ClientID %>_newfilters').value = ReportGrid_BuildQuery(filterArray);
    if (setControl && <%= this.ClientID %>_FilterSetCallback != null)
        eval(<%= this.ClientID %>_FilterSetCallback + '(\'' + filter.toLowerCase() + '\', \'' + value.replace("'", "\\'") + '\');');
    return false;
}
function <%= this.ClientID %>_GetFilter(filter, getChangedValue) {
    if (filter == null || filter == '')
        return null;
    var filters = document.getElementById('<%= this.ClientID %>_' + (getChangedValue ? 'new' : '') + 'filters').value;
    var filterArray = ReportGrid_ParseQuery(filters);
    return filterArray[filter.toLowerCase()];
}
var <%= this.ClientID %>_ShowBusyDuringCallback = <%= string.IsNullOrEmpty(ClientShowBusyDuringCallback) ? "null" : ("'" + (UseClientIDPrefix ? (this.ClientID + "_") : string.Empty) + ClientShowBusyDuringCallback + "'") %>;
function <%= this.ClientID %>_UpdateGrid(html, altControl) { document.getElementById(altControl ? altControl : ('<%= this.ClientID %>_container')).innerHTML = html; };
function <%= this.ClientID %>_Callback(result, context) {
    var data = ReportGrid_ParseQuery(result);
    switch (data['action']) {
        case 'updategrid':
            <%= this.ClientID %>_UpdateGrid(data['html']);

            if (data["newpage"] != null)
                <%= this.ClientID %>_UpdateParams(data["newpage"], null, null);
            if (data["newsort"] != null)
                <%= this.ClientID %>_UpdateParams(null, data["newsort"], null);
            if (data["newfilters"] != null)
                <%= this.ClientID %>_UpdateParams(null, null, data["newfilters"], true);
            
            break;
        case 'showmodal':
            <%= this.ClientID %>_OpenModalDialog(data['title'], data['html'], data['okclick']);
            break;
        case 'hidemodal':
            <%= this.ClientID %>_CloseModalDialog();
            if (data['html'] != null && data['html'] != '')
                <%= this.ClientID %>_UpdateGrid(data['html']);
            break;
        case null:
            break;
        default:
            alert('Invalid action specified by server');
        break;
    }
    
    if (<%= this.ClientID %>_ShowBusyDuringCallback != null)
        eval(<%= this.ClientID %>_ShowBusyDuringCallback + '(false);');
};
var <%= this.ClientID %>_OnModalOkClick = null;
function <%= this.ClientID %>_OpenModalDialog(title, html, okFunction) {
    var modalDialog = $ektron('#<%= this.ClientID %>_modal');
    var modalTitle = modalDialog.find('#<%= this.ClientID %>_modal_headertext');
    var modalText = modalDialog.find('#<%= this.ClientID %>_modal_text');
    
    modalTitle.html(title);
    modalText.html(html);
    <%= this.ClientID %>_OnModalOkClick = okFunction;
    
    modalDialog.modalShow();
}
function <%= this.ClientID %>_CloseModalDialog() {
    var modalDialog = $ektron('#<%= this.ClientID %>_modal');
    modalDialog.modalHide();
}

Ektron.ready(function()
{
    // initialize modal dialog
    var modalDialog = $ektron('#<%= this.ClientID %>_modal');
    /* 
        We have to move the modal to be as close to the <body> tag as possible to prevent z-index problems in IE.  
        Unfortunately, some of the other code requires that the datepicker within the dialog be INSIDE the <form> tag,
        so we cannot use the plugin's native "toTop" option.
        Solution:  prepend the modal DIV to the <form> tag, which is a direct child of the <body> tag already.
    */
    modalDialog.prependTo("form:first");  
    modalDialog.modal(
    {
        modal: true,
        overlay: 0,
        trigger: "",
        onShow: function(hash) {
            hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
            hash.o.fadeTo("fast", 0.5, function() {
	            hash.w.fadeIn("fast");
            });
        },
        onHide: function(hash) {
            hash.w.fadeOut("fast");
            hash.o.fadeOut("fast", function() {
	            if (hash.o)
	            {
		            hash.o.remove();
                }
            });
        }
    });

    // provide hover effects for dialog close buttons
    $ektron(".ui-widget-header .ektronModalClose").hover(
        function(){
            $ektron(this).addClass("ui-state-hover");
        },
        function(){
            $ektron(this).removeClass("ui-state-hover");
        }
    );

    // bind the deleteComment action to the "ok" button of the dialog
    modalDialog.find(".okButton").click(function()
    {
        var okButton = $ektron(this);
        okButton.attr('diabled', 'disabled');
        try {
            if (<%= this.ClientID %>_OnModalOkClick != null)
            {
                eval(<%= this.ClientID %>_OnModalOkClick + '();');
                document.getElementById('<%= this.ClientID %>_selected').value = '';
                eval(<%= this.ClientID %>_SelectionChanged + '(null, false);');
            }    
        } catch(ex) {
            throw(ex);
        } finally {
            okButton.removeAttr('disabled');
        }
        return false;
    });
});
</script>

<asp:Panel runat="server" Visible="false">
<asp:Panel runat="server" ID="pnlLangSelector" CssClass="LangDialects">
<asp:PlaceHolder runat="server" ID="plcLangsSelected">
<div class="currentLangDialects floatLeft" style="width: 65%;">
    <p>
        <strong><span id="UcLocalizationTab_lblSelectedLang"><%=this.contAPI.EkMsgRef.GetMessage("lbl Currently Selected Languages")%></span></strong></p>
    <div>
        <table class="ektronGrid" cellspacing="0" border="0" id="UcLocalizationTab_Current_selected_Dielects" style="display:block;width:100%;border-collapse:collapse;">
        <tr class="title-header">
	        <th class="title-header" align="left" scope="col" style="width:5%;">&nbsp;</th>
	        <th class="title-header" align="left" scope="col" style="width:30%;"><%=this.contAPI.EkMsgRef.GetMessage("lbl Locale")%></th>
	        <th class="title-header" align="left" scope="col" style="width:30%;"><%=this.contAPI.EkMsgRef.GetMessage("generic name")%></th>
	        <th class="title-header" align="right" scope="col" style="width:30%;"><%=this.contAPI.EkMsgRef.GetMessage("generic id")%></th>
        </tr>
        <asp:Repeater runat="server" ID="rptSelectedLangs">
            <ItemTemplate><tr class="center">
	            <td style="width:5%;white-space:nowrap;"><input type="checkbox" title="<%=contAPI.EkMsgRef.GetMessage("lbl include locales title for include checkbox of localization") %>" class="CurrentlySelectedLocales" <%# (bool)Eval("Enabled") ? "checked" : string.Empty %> <%# (bool)Eval("Default") ? "disabled" : string.Empty %> id="<%= this.ClientID %>_<%# Eval("Id") %>" value="<%# Eval("Id") %>"></td>
	            <td style="width:30%;white-space:nowrap;"><%# Eval("Loc") %></td>
	            <td style="width:30%;white-space:nowrap;"><img title="<%# Eval("EnglishName") %>" alt="<%# Eval("EnglishName") %>" src="<%# Eval("FlagUrl") %>" />&nbsp;&nbsp;&nbsp;<%# Eval("CombinedName") %></td>
	            <td style="width:30%;white-space:nowrap;"><%# Eval("Id") %></td>
            </tr></ItemTemplate>
        </asp:Repeater>
        </table>
        <asp:Literal runat="server" ID="lMultiNotice">Note: The selected content items do not have matching locale lists, so only the default locale is displayed.  Any changes made here will replace existing settings for all selected content items.</asp:Literal>
    </div>
</div>
</asp:PlaceHolder>
<div class="otherLangDialects floatLeft" style="width: 100%;">
    <p>
        <strong><span id="UcLocalizationTab_lblOtherLang"><%=this.contAPI.EkMsgRef.GetMessage("lbl Other Available Languages")%></span></strong></p>
    <div>
        <table class="ektronGrid" cellspacing="0" border="0" id="UcLocalizationTab_AvailableLanguages_grid" style="display:block;width:100%;border-collapse:collapse;">
        <tr class="title-header">
	        <th class="title-header" align="left" scope="col" style="width:5%;">&nbsp;</th>
	        <th class="title-header" align="left" scope="col" style="width:30%;"><%=this.contAPI.EkMsgRef.GetMessage("lbl Locale")%></th>
	        <th class="title-header" align="left" scope="col" style="width:30%;"><%=this.contAPI.EkMsgRef.GetMessage("generic name")%></th>
	        <th class="title-header" align="right" scope="col" style="width:30%;"><%=this.contAPI.EkMsgRef.GetMessage("generic id")%></th>
        </tr>
        <asp:Repeater runat="server" ID="rptAvailableLangs">
            <ItemTemplate><tr class="center">
	            <td style="width:5%;white-space:nowrap;"><input type="checkbox" title="<%= contAPI.EkMsgRef.GetMessage("lbl include locales title for include checkbox of localization")%>" class="CurrentlySelectedLocales" <%# (bool)Eval("Enabled") ? "checked" : string.Empty %> id="<%= this.ClientID %>_locale<%# Eval("Id") %>" value="<%# Eval("Id") %>"></td>
	            <td style="width:30%;white-space:nowrap;"><%# Eval("Loc") %></td>
	            <td style="width:30%;white-space:nowrap;"><img title="<%# Eval("EnglishName") %>" alt="<%# Eval("EnglishName") %>" src="<%# Eval("FlagUrl") %>" />&nbsp;&nbsp;&nbsp;<%# Eval("CombinedName") %></td>
	            <td style="width:30%;white-space:nowrap;"><%# Eval("Id") %></td>
            </tr></ItemTemplate>
        </asp:Repeater>
        </table>
    </div>
    <div id="UcLocalizationTab_uxAvailableLanguageDiv" class="selectDeselect">
        <asp:PlaceHolder runat="server" ID="plcSelectLinks"></asp:PlaceHolder>
    </div>
</div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlNotes" CssClass="NotesSet">
    <asp:Label runat="server" AssociatedControlID="txtNotesN">Notes:</asp:Label>
    <asp:TextBox runat="server" ID="txtNotesN" TextMode="MultiLine"></asp:TextBox>
    <asp:Literal runat="server" ID="lNoteN"><p><b>Note: </b>Because you have selected multiple content items, the notes you enter here will overwrite any notes that may exist in the selected content.  This action cannot be undone once you click "Save."</p></asp:Literal>
</asp:Panel>
<asp:Panel runat="server" ID="pnlSetTransStatus" CssClass="TransStatusSet">
    <asp:Label runat="server" AssociatedControlID="ddlTranReady"><%= this.contAPI.EkMsgRef.GetMessage("lbl Translation Readiness")%></asp:Label>
    <asp:DropDownList runat="server" ID="ddlTranReady">
        <asp:ListItem Text="Not ready" Value="NotReady"></asp:ListItem>
        <asp:ListItem Text="Ready" Value="Ready"></asp:ListItem>
        <asp:ListItem Text="Do not translate" Value="DoNotTranslate"></asp:ListItem>
    </asp:DropDownList>
</asp:Panel>
<asp:Panel runat="server" ID="pnlPseudo" CssClass="HandoffSelector">
    <asp:Label ID="lblPseudoInstructions" runat="server" ><%= this.contAPI.EkMsgRef.GetMessage("lbl Pseudo localization languages")%></asp:Label><br />
</asp:Panel>
<asp:Panel runat="server" ID="pnlSetPseudo" CssClass="HandoffSelector">
    <iframe src="#" noresize="noresize" frameborder="0" border="0"  marginwidth="0" marginheight="0" id="ektronExportPseudoIframe" class="ektronExportPseudoIframe" scrolling="auto" runat="server"></iframe>
</asp:Panel>
</asp:Panel>
<div id="<%= this.ClientID %>_modal" class="ektronWindow ektronSyncModal ektronModalWidth-70 ui-dialog ui-widget ui-widget-content ui-corner-all" style="display: none;">
    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix ektronModalHeader">
        <h3 id="<%= this.ClientID %>_modal_header" class="ui-dialog-title header">
            <span class="headerText" id="<%= this.ClientID %>_modal_headertext"></span>
	        <a href="#Cancel" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose"><img src="<%= Page.ResolveClientUrl("~/workarea/images/UI/modal/closeButton.png") %>" alt="<%= contAPI.EkMsgRef.GetMessage("lbl cancel and close window")%>" /></a>
        </h3>
    </div>
    <div class="ektronModalBody">
        <div class="ui-dialog-content ui-widget-content ektronPageInfo" style="overflow-y:hidden;">
	        <p id="<%= this.ClientID %>_modal_text" class="messages"></p>
	    </div>
        <ul class="ektronModalButtonWrapper ektronSyncButtons ui-dialog-buttonpane ui-widget-content ui-helper-clearfix modalButtonFix">
            <li><a href="#Cancel" title="Cancel" class="redHover button cancelButton buttonRight ektronModalClose"><%= this.contAPI.EkMsgRef.GetMessage("generic cancel")%></a></li>
            <li><a href="#Ok" title="Save" class="greenHover button okButton buttonRight" id="modalSave" runat="server"><%= this.contAPI.EkMsgRef.GetMessage("btn save")%></a></li>
	    </ul>
    </div>
</div>
<div id="<%= this.ClientID %>_container"><asp:PlaceHolder runat="server" ID="plcRenderMe">
<asp:PlaceHolder runat="server" ID="plcResultCountTop">
<p>
    <asp:Literal runat="server" ID="lResultCount"></asp:Literal>
</p>
</asp:PlaceHolder>
<%--pagination begins--%>
<asp:PlaceHolder runat="server" ID="plcPaginationTop">
<div class="navbar">
    <ul class="ektronPaging">
        <asp:Literal runat="server" ID="lPaging1"></asp:Literal>
    </ul>
</div>
</asp:PlaceHolder>
<%--pagination ends--%>
<%--listing starts here--%>
<table class="listing localeReport">
    <thead class="listingHdr">
        <tr class="hdrBack">
            <asp:PlaceHolder runat="server" ID="plcColumnHeaders">
            <td class="gridCol col1">
                <input type="checkbox" class="selectAll" onclick="var rootchecked = $ektron('.localeReport input.selectAll:checkbox').get(0).checked;$ektron('.localeReport input[class!=selectAll]:checkbox').each(function(index,ele){ele.checked=!rootchecked;$ektron(ele).get(0).click();;});return true;" title="Select/Deselect all items on this page" />
                <input type="button" value="Action" onclick="$ektron('#<%= this.ClientID %>_actiondd').css('display','block');return false;" class="goBtn" style="float:right;">
                <div style="position: relative;">
                <div style="width: 190px; position: absolute; top: 0px; left: 20px; border: 1px solid #808080; background-color: #f0f0f0; display: none;" id="<%= this.ClientID %>_actiondd">
                    &#187; <a href="#" onclick="$ektron('#<%= this.ClientID %>_actiondd').css('display','none');<%= this.ClientID %>_Action('locales');return false;">Set Locales</a><br />
                    &#187; <a href="#" onclick="$ektron('#<%= this.ClientID %>_actiondd').css('display','none');<%= this.ClientID %>_Action('locales', 'mode=add');return false;">Add Locale(s)</a><br />
                    &#187; <a href="#" onclick="$ektron('#<%= this.ClientID %>_actiondd').css('display','none');<%= this.ClientID %>_Action('locales', 'mode=del');return false;">Remove Locale(s)</a><br />
                    &#187; <a href="#" onclick="$ektron('#<%= this.ClientID %>_actiondd').css('display','none');<%= this.ClientID %>_Action('transstatus');return false;">Set Translation Status</a><br />
                    <%--&#187; <a href="#" onclick="$ektron('#<%= this.ClientID %>_actiondd').css('display','none');<%= this.ClientID %>_Action('pseudo');return false;">Pseudo Localize</a><br />--%>
                </div>
                </div>
                </td>
            <td class="<%= GetSortClass(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.Title, "gridCol col2") %>">
                <a href="#" onclick="<%= GenerateHeaderUrl(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.Title) %>"><%= this.contAPI.EkMsgRef.GetMessage("generic title")%></a></td>
            <td class="<%= GetSortClass(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.ContentStatus, "gridCol col3") %>">
                <a href="#" onclick="<%= GenerateHeaderUrl(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.ContentStatus) %>" title="Content Status"><%= this.contAPI.EkMsgRef.GetMessage("generic status")%></a></td>
            <td class="<%= GetSortClass(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.TranslationStatus, "gridCol col5") %>">
                <a href="#" onclick="<%= GenerateHeaderUrl(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.TranslationStatus) %>"><%= this.contAPI.EkMsgRef.GetMessage("lbl Trans Status")%></a></td>
            <td class="<%= GetSortClass(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.Locale, "gridCol col6") %>">
                <a href="#" onclick="<%= GenerateHeaderUrl(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.Locale) %>"><%= this.contAPI.EkMsgRef.GetMessage("lbl wa locale")%></a></td>
            <td class="<%= GetSortClass(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.LastModified, "gridCol col8") %>">
                <a href="#" onclick="<%= GenerateHeaderUrl(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.LastModified) %>"><%= this.contAPI.EkMsgRef.GetMessage("lbl Last Modified")%></a></td>
            <td class="<%= GetSortClass(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.DateCreated, "gridCol col9") %>">
                <a href="#" onclick="<%= GenerateHeaderUrl(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.DateCreated) %>"><%= this.contAPI.EkMsgRef.GetMessage("generic datecreated")%></a></td>
            <td class="<%= GetSortClass(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.ContentId, "gridCol col12") %>">
                <a href="#" onclick="<%= GenerateHeaderUrl(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.ContentId) %>"><%= this.contAPI.EkMsgRef.GetMessage("generic content id")%></a></td>
            <td class="<%= GetSortClass(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.AuthorId, "gridCol col13") %>">
                <a href="#" onclick="<%= GenerateHeaderUrl(Ektron.Cms.BusinessObjects.Localization.ReportingProperty.AuthorId) %>"><%= this.contAPI.EkMsgRef.GetMessage("generic author")%></a></td>
            </asp:PlaceHolder>
        </tr>
    </thead>
    <tbody class="listRows">
        <asp:Repeater runat="server" ID="rptRows">
            <ItemTemplate>
                <tr class="row">
                    <td class="gridCol col1">
                        <input id="chk_<%# Eval("ContentId") %>" type="checkbox" cid="<%# Eval("ContentId") %>" onclick="<%= this.ClientID %>_SetSelection(<%# Eval("ContentId") %>, this.checked);"  />
                        <a href="<%# GetPreviewUrl((Ektron.Cms.BusinessObjects.Localization.ReportingData)Container.DataItem) %>" target="_blank">
                            <%# GetContentTypeIcon((Ektron.Cms.BusinessObjects.Localization.ReportingData)Container.DataItem)%></a>
                        </td>
                    <td class="gridCol col2">
                        <a href="<%# Page.ResolveUrl(String.Format("~/workarea/content.aspx?action=" + GetViewAction((string)Eval("ContentStatus")) + "&folder_id={0}&id={1}&LangType={2}", Eval("FolderId"), Eval("ContentId"), Eval("LanguageId"))) %>" target="_blank"><%# Eval("Title")%></a>
                        <%# this.IncludeFolderPath ? ("<br/><span class=\"FolderPath\">" + SafeString(Eval("FolderPath")) + "</span>") : string.Empty%>
                    </td>
                    <td class="gridCol col3">
                        <%# GetContentStatusIcon((string)Eval("ContentStatus"))%></td>
                    <td class="gridCol col5">
                        <%# GetLocStatus((Ektron.Cms.Localization.LocalizationState)(byte)Eval("TranslationStatus"))%></td>
                    <td class="gridCol col6">
                        <%# GetLocales((System.Collections.Generic.List<int>)Eval("Locale"), 2)%></td>
                    <td class="gridCol col8">
                        <%# FormatDate(Eval("LastModified"))%></td>
                    <td class="gridCol col9">
                        <%# FormatDate(Eval("DateCreated"))%></td>
                    <td class="gridCol col12">
                        <%# Eval("ContentId")%></td>
                    <td class="gridCol col13">
                        <%# Eval("AuthorDisplayName") /* Eval("EditorFirstName") % > < % # Eval("EditorLastName")*/ %></td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr class="altRow">
                    <td class="gridCol col1">
                        <input id="chk_<%# Eval("ContentId") %>" type="checkbox" onclick="<%= this.ClientID %>_SetSelection(<%# Eval("ContentId") %>, this.checked);"  />
                        <a href="<%# GetPreviewUrl((Ektron.Cms.BusinessObjects.Localization.ReportingData)Container.DataItem) %>" target="_blank">
                            <%# GetContentTypeIcon((Ektron.Cms.BusinessObjects.Localization.ReportingData)Container.DataItem)%></a>
                        </td>
                    <td class="gridCol col2">
                        <a href="<%# Page.ResolveUrl(String.Format("~/workarea/content.aspx?action=" + GetViewAction((string)Eval("ContentStatus")) + "&folder_id={0}&id={1}&LangType={2}", Eval("FolderId"), Eval("ContentId"), Eval("LanguageId"))) %>" target="_blank"><%# Eval("Title")%></a>
                        <%# this.IncludeFolderPath ? ("<br/><span class=\"FolderPath\">" + SafeString(Eval("FolderPath")) + "</span>") : string.Empty%>
                    </td>
                    <td class="gridCol col3">
                        <%# GetContentStatusIcon((string)Eval("ContentStatus"))%></td>
                    <td class="gridCol col5">
                        <%# GetLocStatus((Ektron.Cms.Localization.LocalizationState)(byte)Eval("TranslationStatus"))%></td>
                    <td class="gridCol col6">
                        <%# GetLocales((System.Collections.Generic.List<int>)Eval("Locale"), 2)%></td>
                    <td class="gridCol col8">
                        <%# FormatDate(Eval("LastModified"))%></td>
                    <td class="gridCol col9">
                        <%# FormatDate(Eval("DateCreated"))%></td>
                    <td class="gridCol col12">
                        <%# Eval("ContentId")%></td>
                    <td class="gridCol col13">
                        <%# Eval("AuthorDisplayName") /* Eval("EditorFirstName") % > < % # Eval("EditorLastName")*/ %></td>
                </tr>
            </AlternatingItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
<%--listing ends--%>
<%--bottom pagination begins--%>
<asp:PlaceHolder runat="server" ID="plcPaginationBottom">
<div style="clear:both;"></div>
<div class="navbar">
    <ul class="ektronPaging">
        <asp:Literal runat="server" ID="lPaging2"></asp:Literal>
    </ul>
</div>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="plcResultCountBottom">
<div style="clear:both;"></div>
<p>
    <asp:Literal runat="server" ID="lResultCount2"></asp:Literal>
</p>
</asp:PlaceHolder>
<%--bottom pagination ends--%>
</asp:PlaceHolder>
</div>
</asp:Panel>