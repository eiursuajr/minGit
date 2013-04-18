<%@ Control Language="C#" AutoEventWireup="true" CodeFile="addcollection.ascx.cs"
    Inherits="Workarea_controls_collection_AddCollection" %>
<script language="javascript" type="text/javascript">
      Ektron.ready(function(){ 
         var folderid = "<asp:literal id="jsFolderId" runat="server"/>";
         $ektron("#frm_folder_id").attr("value", folderid);
      });
</script>
<div id="divError" visible="false" runat="server" class="ektronPageHeader">
    <div class="ektronTitlebar" title="Add Collection">
        <asp:Literal ID="litAddCollectionTitle" runat="server"></asp:Literal>
    </div>
    <div class="titlebar-error" id="divErrors" runat="server">
    </div>
</div>
<div id="divMain" runat="server">
    <form action="collections.aspx?Action=doAdd" method="Post" id="form2" name="nav">
    <input type="hidden" id="frm_folder_id" name="frm_folder_id" />
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" title="Add Collection">
            <asp:Literal ID="AddCollection" runat="server"></asp:Literal>
        </div>
        <div class="ektronToolbar">
            <table>
                <tr>
                    <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                    <asp:Literal ID="litHelp" runat="server"></asp:Literal>
                </tr>
            </table>
        </div>
    </div>
    <div class="ektronPageContainer ektronPageInfo">
        <table class="ektronForm">
            <tr>
                <td class="label" title="Title">
                    <asp:Literal ID="litGeneric" runat="server"></asp:Literal>
                </td>
                <td>
                    <input type="Text" title="Enter Title here" name="frm_nav_title" maxlength="75" onkeypress="return CheckKeyValue(event,'34');" />
                </td>
            </tr>
            <tr>
                <td class="label" title="Template">
                    <asp:Literal ID="litTemplate" runat="server"></asp:Literal>
                </td>
                <td>
                    <asp:Literal ID="litSitePath" runat="server"></asp:Literal>
                    <input type="Text" title="Enter Template here" name="frm_nav_template" class="ektronTextMedium"
                        maxlength="255" value="" onkeypress="return CheckKeyValue(event,'34');" />
                    <div class="ektronCaption" title="Leave the above template empty if you wish to use the Quicklinks">
                        <asp:Literal ID="litLeave" runat="server"></asp:Literal>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="label" title="Description">
                    <asp:Literal ID="litDesc" runat="server"></asp:Literal>
                </td>
                <td>
                    <textarea title="Enter Description here" name="frm_nav_description" maxlength="255"
                        onkeypress="return CheckKeyValue(event,'34');"></textarea>
                </td>
            </tr>
            <tr>
                <td class="label" title="Include Subfolders">
                    <asp:Literal ID="litInclude" runat="server"></asp:Literal>
                </td>
                <td class="value">
                    <input title="Include Subfolders Option" type="Checkbox" name="frm_recursive" />
                </td>
            </tr>
            <tr id="trPer" runat="server">
                <td class="label" title="Approval is required">
                    <span style="color: Red">
                        <asp:Literal ID="litApprove" runat="server"></asp:Literal>
                    </span>:
                </td>
                <td class="value">
                    <input title="Approval is required Option" type="Checkbox" name="frm_approval_methhod"
                        id="frm_approval_methhod" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</div>
