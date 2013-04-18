<%@ Control Language="C#" AutoEventWireup="true" CodeFile="editcollection.ascx.cs" Inherits="Workarea_controls_collection_editcollection" %>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="Edit Collection">
        <asp:Literal ID="litEditCollectionTitle" runat="server"></asp:Literal>
    </div>
    <div class="titlebar-error" id="divErrorString" runat="server">
    </div>
</div>
<form method="Post" name="nav" action="collections.aspx?folderId=<%=folderId %>&nID=<%=nId %>&Action=doEdit&<%=checkout %>>
<input type="hidden" value="<%=nId %>" name="frm_nav_id" />
<input type="hidden" value="<%=folderId %>" name="frm_folder_id" />
<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <asp:Literal ID="litEditColl" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <asp:Literal ID="litSearch" runat="server"></asp:Literal>
            </tr>
        </table>
    </div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <div class="heightFix">
        <table class="ektronForm">
            <tr>
                <td class="label" title="Title">
                    <asp:Literal ID="litTitle" runat="server"></asp:Literal>
                </td>
                <td class="value">
                    <input title="Enter Title here" type="Text" value="<%=(gtNavs["CollectionTitle"]) %>" name="frm_nav_title"
                        maxlength="75" onkeypress="return CheckKeyValue(event,'34');" />
                </td>
            </tr>
            <tr>
                <td class="label" title="ID">
                    <asp:Literal ID="litIDLabel" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="trID" runat="server">
                </td>
            </tr>
            <tr id="trFolderPath" class="trFolderPath" runat="server" visible="false">
                <td class="label" title="Path">
                    <asp:Literal ID="litPath" runat="server"></asp:Literal>
                </td>
                <td class="value">
                    <asp:Label ID="lblFolderPath" CssClass="frm_nav_path" runat="server" />
                    <span style="margin-left: 1em;" >
                        <asp:Literal ID="litFolderSelect" runat="server" />
                    </span>
                </td>
            </tr>
            <tr>
                <td class="label" title="Template">
                    <asp:Literal ID="litTemplate" runat="server"></asp:Literal>
                </td>
                <td class="value">
                    <asp:Literal ID="litSitePath" runat="server"></asp:Literal>
                    <input id="frm_nav_template" runat="server" title="Enter Template here" type="text"
                        name="frm_nav_template" maxlength="255" onkeypress="return CheckKeyValue(event,'34');" />
                    <div class="ektronCaption" title="Leave the above template empty if you wish to use the Quicklinks">
                        <asp:Literal ID="litLeaveTemplate" runat="server"></asp:Literal></div>
                </td>
            </tr>
            <tr>
                <td class="label" title="Description">
                    <asp:Literal ID="litDesc" runat="server"></asp:Literal>
                </td>
                <td class="value">
                    <textarea title="Enter Description here" name="frm_nav_description"  maxlength="255" onkeypress="return CheckKeyValue(event,'34');"><%=(gtNavs["CollectionDescription"])%></textarea>
                </td>
            </tr>
            <tr>
                <td class="checkbox" colspan="2">
                    <input title="Include Subfolders" <%=(gtNavs["Recursive"].ToString() == "1" ? "checked" : "") %> type="checkbox" name="frm_recursive"/>
                    <asp:Literal ID="litInclueSub" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr id="trApproval" runat="server" visible="false">
                <td class="checkbox" colspan="2">
                    <input title="Approval Required Option" <%=(gtNavs["ApprovalRequired"].ToString().ToLower() == "true" ? "checked" : "") %> type="checkbox" name="frm_approval_methhod"/>
                    <asp:Literal ID="litApproval" runat="server"></asp:Literal>
                </td>
            </tr>
            <input type="hidden" <%=gtNavs["ApprovalRequired"].ToString() %> name="frm_approval_methhod" />
        </table>
    </div>
</div>
<input type="hidden" id="frm_nav_folderid" class="ektron_hdn_frm_nav_folderid" name="frm_nav_folderid" value="<%=(folderId) %>" />
<script type="text/javascript" language="javascript" >
    function ReturnChildValue(folderid, folderpath, targetFolderIsXml) {
        if ("undefined" != typeof $ektron) {
            $ektron('#frm_nav_folderid').val(folderid);
            $ektron('.ektronPageContainer .trFolderPath .frm_nav_path').text(folderpath);
        }
    }
</script>
</form>
