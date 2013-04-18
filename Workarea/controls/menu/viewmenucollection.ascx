<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewmenucollection.ascx.cs"
    Inherits="Workarea_controls_menu_viewmenucollection" %>
<script type="text/javascript">
    function onMenuItemEdit(ItemId, MenuId, ItemType) {
        if (MenuId == "") {
            MenuId = '<%=nId%>';
        }
        LoadChildPage("submenuedititem", ItemId, MenuId, ItemType);
        //alert(ItemId + ", " + MenuId + ', ' + ItemType);
    }
    function onMenuItemDelete(ItemId, MenuId, ItemType) {
        var lItemType;
        if (MenuId == "") {
            MenuId = '<%=nId%>';
        }
        if (confirm(MsgHelper.Getmessage("alt Are you sure you want to delete this item?"))) {
            document.forms.netscapefix.frm_content_ids.value = ItemId + '.' + 0;
            document.forms.netscapefix.CollectionID.value = MenuId;
            document.forms.netscapefix.action = "collections.aspx?iframe=true&action=doDeleteMenuItem&folderid=<%=folderId%>&nid=<%=nId%>";
            document.forms.netscapefix.submit();
        }
        return false;
    }
    function onSubMenuAddItem(MenuId, FolderId) {
        if (MenuId == "") {
            alert("Please select a menu.");
            return false;
        }
        LoadChildPage("submenuadditem", FolderId, MenuId, "");
        //document.location.href = "collections.aspx?iframe=true&action=AddMenuItem&nid=" + MenuId + "&folderid=" + FolderId + "&parentid=" + MenuId + "&ancestorid=<%=AncestorMenuId%>";
    }
    function onSubMenuEdit(MenuId) {
        if (MenuId == "") {
            MenuId = '<%=nId%>';
        }
        LoadChildPage("submenuedit", "", MenuId, "");
        //alert(MenuId);
    }
    function onSubMenuDelete(MenuId) {
        if (MenuId == "") {
            MenuId = '<%=nId%>';
        }
        if (confirm(msghelper.getmessage("alt Are you sure you want to delete this item?"))) {
            document.forms.netscapefix.frm_content_ids.value = MenuId + '.' + 4 + '.' + MenuId;
            document.forms.netscapefix.CollectionID.value = MenuId;
            document.forms.netscapefix.action = "collections.aspx?iframe=true&action=doDeleteMenuItem&folderid=<%=folderId%>&nid=<%=nId%>";
            document.forms.netscapefix.submit();
        }
    }
    function onSubMenuOrderItem(MenuId, FolderId) {
        LoadChildPage("submenuorderitem", FolderId, MenuId, "");
    }
    function LoadChildPage(tAction, ItemId, MenuId, ItemType) {
        var frameObj = document.getElementById("ChildPage");
        if (tAction == "submenuedit") {
            frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=EditMenu&nid=" + MenuId + "&folderid=" + '<%=folderId%>&Ty=' + ItemType;
        } else if (tAction == "submenuedititem") {
            frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=EditMenuItem&nid=" + MenuId + "&folderid=<%=folderId%>&id=" + ItemId + '&Ty=' + ItemType;
        } else if (tAction == "submenuadditem") {
            frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=AddMenuItem&nid=" + MenuId + "&folderid=" + ItemId + "&parentid=" + MenuId + "&ancestorid=<%=AncestorMenuId%>";
        } else if (tAction == "submenuorderitem") {
            frameObj.src = "blankredirect.aspx?collections.aspx?iframe=true&action=ReOrderMenuItems&nid=" + MenuId + "&folderid=" + ItemId;
        }


        var pageObj = document.getElementById("FrameContainer");
        pageObj.style.display = "";
        pageObj.style.width = "90%";
        pageObj.style.height = "90%";

    }
</script>
<form name="netscapefix" method="post" action="#" id="Form3">
<div id="FrameContainer" class="ektronBorder" style="position: absolute; top: 48px;
    left: 40px; width: 1px; height: 1px; display: none;">
    <iframe id="ChildPage" src="javascript:false;" name="ChildPage" frameborder="1"
        marginheight="0" marginwidth="0" width="100%" height="100%" scrolling="auto">
    </iframe>
</div>
<input type="hidden" name="CollectionID" id="CollectionID" value="" />
<input type="hidden" name="frm_content_ids" id="frm_content_ids" value="" />
<div class="ektronPageHeader">
    <div class="ektronTitlebar">
        <asp:Literal ID="litTitle" runat="server"></asp:Literal>
    </div>
    <div class="ektronToolbar">
        <table width="100%">
            <tr>
                <asp:Literal ID="litButtons" runat="server"></asp:Literal>
                <asp:Literal ID="litSubMenu" runat="server"></asp:Literal>
                <asp:Literal ID="litHelp" runat="server"></asp:Literal>
            </tr>
        </table>
    </div>
</div>
<br />
<table width="100%">
    <tr>
        <td>
            <asp:Literal ID="litMenuXML" runat="server"></asp:Literal>
        </td>
    </tr>
</table>
<div class="switchcontent" name="sc1" id="moreInfo2">
    <table class="ektronForm">
        <tr>
            <td class="label" title="Title">
                <asp:Literal ID="litGenericTitleLabel" runat="server"></asp:Literal> 
            </td>
            <td>
                <asp:Literal ID="litGenericTitle" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td class="label" title="Content ID">
                <asp:Literal ID="litIDLabel" runat="server"></asp:Literal> 
            </td>
            <td>
                <asp:Literal ID="litID" runat="server"></asp:Literal> 
            </td>
        </tr>
        <tr>
            <td class="label" title="Path">
                <asp:Literal ID="litPathLabel" runat="server"></asp:Literal>:
            </td>
            <td>
                <asp:Literal ID="litPath" runat="server"></asp:Literal> 
            </td>
        </tr>
        <tr>
            <td class="label" title="Last User To Edit">
                <asp:Literal ID="litLUELabel" runat="server"></asp:Literal> 
            </td>
            <td>
                <asp:Literal ID="litLUE" runat="server"></asp:Literal>
            </td>
        </tr>
        <tr>
            <td class="label" title="Last Edit Date">
                <asp:Literal ID="litLEDLabel" runat="server"></asp:Literal> 
            </td>
            <td>
                <asp:Literal ID="litLED" runat="server"></asp:Literal> 
            </td>
        </tr>
        <tr>
            <td class="label" title="Date Created">
                <asp:Literal ID="litDCLabel" runat="server"></asp:Literal> 
            </td>
            <td>
                <asp:Literal ID="litDC" runat="server"></asp:Literal> 
            </td>
        </tr>
        <tr>
            <td class="label" title="Description">
                <asp:Literal ID="litDescLabel" runat="server"></asp:Literal> 
            </td>
            <td>
                <asp:Literal ID="litDesc" runat="server"></asp:Literal> 
            </td>
        </tr>
    </table>
</div>
</form>