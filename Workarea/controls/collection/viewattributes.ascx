<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewattributes.ascx.cs" Inherits="Workarea_controls_collection_ViewAttributes" %>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="View Collection">
        <asp:Literal ID="litViewCollection" runat="server"></asp:Literal>
    </div>
    <div class="titlebar-error" id="titlebarerror" runat="server"></div>
</div>
<form name="netscapefix" method="post" action="#">
<div class="ektronPageHeader">
    <div class="ektronTitlebar" title="View Collection">
         <asp:Literal ID="litViewToolBarCollection" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table>
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <asp:Literal ID="litLang" runat="server"></asp:Literal>
                <asp:Literal ID="litHelp" runat="server"></asp:Literal>
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
                <td class="readOnlyValue" id="tdTitle" runat="server"></td>
            </tr>
            <tr>
                <td class="label" title="Label">
                    <asp:Literal ID="litLabel" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="tdID" runat="server">
                </td>
            </tr>
            <tr>
                <td class="label" title="Path">
                    <asp:Literal ID="litPath" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="tdPath" runat="server">
                </td>
            </tr>
            <tr>
                <td class="label" title="Template">
                    <asp:Literal ID="litTemplate" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="tdTemplate" runat="server">
                </td>
            </tr>
            <tr>
                <td class="label" title="Last User To Edit">
                    <asp:Literal ID="litContentLUE" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="tdLUE" runat="server">
                </td>
            </tr>
            <tr>
                <td class="label" title="Last Edit Date">
                    <asp:Literal ID="litLED" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="tdLastEditDate" runat="server">
                </td>
            </tr>
            <tr>
                <td class="label" title="Date Created">
                    <asp:Literal ID="litDC" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="tdDateCreated" runat="server">
                </td>
            </tr>
            <tr>
                <td class="label" title="Description">
                    <asp:Literal ID="litDesc" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="tdDesc" runat="server">
                </td>
            </tr>
            <tr>
                <td class="label" title="Status">
                    <asp:Literal ID="litStatus" runat="server"></asp:Literal>
                </td>
                <td class="readOnlyValue" id="tdStatus" runat="server">
                </td>
            </tr>
            <tr>
                <td class="label" title="Include Subfolders">
                    <asp:Literal ID="litSubFolders" runat="server"></asp:Literal>:
                </td>
                <td class="value">
                    <input type="Checkbox" title="Include Subfolders Option" name="frm_recursive" id="frm_recursive" runat="server" disabled="disabled" onclick="return false;" />
                </td>
            </tr>
            <tr>
                <td class="label" title="Approval is required">
                    <asp:Literal ID="litApproval" runat="server"></asp:Literal>:
                </td>
                <td class="value">
                    <input title="Approval is required Option" type="checkbox" id="approval" runat="server" name="frm_approval_required"
                         disabled="disabled" onclick="return false;" />
                </td>
            </tr>
        </table>
    </div>
</div>

<script type="text/javascript">
		        <!--    //--><![CDATA[//><!--
    do_onload();
    //--><!]]>
</script>

</form>
