<%@ Control Language="C#" AutoEventWireup="true" Inherits="editpermissions" CodeFile="editpermissions.ascx.cs" %>
<script type="text/javascript">
    <!--//--><![CDATA[//><!--
        function CheckReadOnlyForMembershipUser(permission){
            if (permission == "frm_readonly") {
                if (document.getElementById(UniqueID+"frm_readonly").checked = true) {
                    alert("<%=_MessageHelper.GetMessage("js: alert cannot disable membership readonly")%>");
                    document.getElementById(UniqueID+"frm_readonly").value = 1;
                    document.getElementById(UniqueID+"frm_readonly").checked = true;
                    return false;
                }
            }
        }
    //--><!]]>
</script>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronTabUpperContainer">
    <asp:DropDownList ID="ddlUserType" AutoPostBack="true" EnableViewState="true" runat="server" />
</div>
<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <asp:PlaceHolder ID="phTabs" runat="server">
                <ul>
                    <li>
                        <a title="<%=_MessageHelper.GetMessage("tooltip:view standard permissions")%>" href="#dvStandard">
                            <%=_MessageHelper.GetMessage("standard permissions")%>
                        </a>
                    </li>
                    <li <%=GetDisplay()%>>
                        <a title="<%=_MessageHelper.GetMessage("tooltip:view advanced permissions")%>" href="#dvAdvanced">
                            <%=_MessageHelper.GetMessage("advanced permissions")%>
                        </a>
                    </li>
                </ul>
            </asp:PlaceHolder>
            <div id="dvStandard" class="ektronPageInfo">
                <asp:DataGrid ID="PermissionsGenericGrid"
                    AutoGenerateColumns="false"
                    runat="server"
                    CssClass="ektronGrid"
                    EnableViewState="False">
                    <HeaderStyle CssClass="title-header" />
                </asp:DataGrid>
            </div>
            <div id="dvAdvanced" class="ektronPageInfo">
                <asp:DataGrid ID="PermissionsAdvancedGrid"
                    AutoGenerateColumns="false"
                    runat="server"
                    CssClass="ektronGrid"
                    EnableViewState="False">
                    <HeaderStyle CssClass="title-header" />
                </asp:DataGrid>
            </div>
            <div id="td_ep_membership" runat="server" style="margin:0em 0em 1em 1.25em;"></div>
        </div>
    </div>
</div>
<asp:HiddenField ID="hdnUserType" runat="server" />
<input type="hidden" name="frm_itemid" id="frm_itemid" runat="server" />
<input type="hidden" name="frm_type" id="frm_type" runat="server" />
<input type="hidden" name="frm_base" id="frm_base" runat="server" />
<input type="hidden" name="frm_permid" id="frm_permid" runat="server" />
<input type="hidden" name="frm_membership" id="frm_membership" runat="server" />
<input type="hidden" name="hmembershiptype" id="hmembershiptype" runat="server" />
<input type="hidden" name="frm_readonly" id="frm_readonly" value="0" runat="server" />
<input type="hidden" name="frm_edit" id="frm_edit" value="0" runat="server" />
<input type="hidden" name="frm_add" id="frm_add" value="0" runat="server" />
<input type="hidden" name="frm_delete" id="frm_delete" value="0" runat="server" />
<input type="hidden" name="frm_restore" id="frm_restore" value="0" runat="server" />
<input type="hidden" name="frm_libreadonly" id="frm_libreadonly" value="0" runat="server" />
<input type="hidden" name="frm_addimages" id="frm_addimages" value="0" runat="server" />
<input type="hidden" name="frm_addfiles" id="frm_addfiles" value="0" runat="server" />
<input type="hidden" name="frm_addhyperlinks" id="frm_addhyperlinks" value="0" runat="server" />
<input type="hidden" name="frm_overwritelib" id="frm_overwritelib" value="0" runat="server" />
<input type="hidden" name="frm_navigation" id="frm_navigation" value="0" runat="server" />
<input type="hidden" name="frm_add_folders" id="frm_add_folders" value="0" runat="server" />
<input type="hidden" name="frm_edit_folders" id="frm_edit_folders" value="0" runat="server" />
<input type="hidden" name="frm_delete_folders" id="frm_delete_folders" value="0" runat="server" />
<input type="hidden" name="frm_transverse_folder" id="frm_transverse_folder" value="1" runat="server" />
<input type="hidden" name="frm_origreadonly" id="frm_origreadonly" value="0" runat="server" />

<%if (_EnablePreaproval == true)%>
    <input type="hidden" name="frm_edit_preapproval" id="frm_edit_preapproval" value="0" runat="server" />


