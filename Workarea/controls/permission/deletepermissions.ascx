<%@ Control Language="C#" AutoEventWireup="true" Inherits="deletepermissions" CodeFile="deletepermissions.ascx.cs" %>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronTabUpperContainer">
    <div id="td_dp_title" runat="server"></div>
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
                    <asp:PlaceHolder ID="phAdvancedTab" runat="server" Visible="false">
                        <li>
                            <a title="<%=_MessageHelper.GetMessage("tooltip:view advanced permissions")%>" href="#dvAdvanced">
                                <%=_MessageHelper.GetMessage("advanced permissions")%>
                            </a>
                        </li>
                    </asp:PlaceHolder>
                </ul>
            </asp:PlaceHolder>
            <div id="dvStandard">
	            <asp:DataGrid id="PermissionsGenericGrid"
	                runat="server"
	                CssClass="ektronGrid"
	                AutoGenerateColumns="False"
		            EnableViewState="False">
                    <HeaderStyle CssClass="title-header" />
	            </asp:DataGrid>
            </div>
            <asp:PlaceHolder ID="phAdvancedContent" runat="server" Visible="false">
                <div id="dvAdvanced">
	                <asp:DataGrid id="PermissionsAdvancedGrid"
	                    runat="server"
	                    CssClass="ektronGrid"
	                    AutoGenerateColumns="False"
		                EnableViewState="False">
                        <HeaderStyle CssClass="title-header" />
	                </asp:DataGrid>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</div>
<script type="text/javascript">
    function Import(obj)
    {
        var users = $ektron("#permission_PermissionsGenericGrid input:checkbox");
        var i = 0;
        var contLang = "<asp:Literal runat='server' id='ltr_contLang' />";
        var contId = "<asp:Literal runat='server' id='ltr_id' />";
        var itemType = "<asp:Literal runat='server' id='ltr_itemType' />";
        var isMembership = "<asp:Literal runat='server' id='ltr_isMembership' />";
        var delAlertNoSel="<asp:Literal runat='server' id='ltr_delAlertNoSel' />"
        var id = 0;
        var selected = 0;
        var selectedGroup = "";
        var selectedUsers = "";
        
        for (i = 0; i < users.length; i++ )
        {
            if( users[i].checked )
            {
                selected = selected + 1;
                if (users[i].id.indexOf("group") != -1)
                {
                    id = users[i].id.replace("group", "");
                    selectedGroup += "," + id;
                }
                else
                {
                    id = users[i].id.replace("user", "");                
                    selectedUsers += "," + id;
                }
            }
        }
        
        selectedGroup = selectedGroup.replace(",", "");
        selectedUsers = selectedUsers.replace(",", "");
        
        if (selected == 0)
        {
            alert(delAlertNoSel);
        }
        else
        {            
            this.window.location = "content.aspx?LangType=" + contLang + "&action=DoDeletePermissions&type=" + itemType  + "&id=" + contId + "&groupIDs=" + selectedGroup + "&userIDs=" + selectedUsers + "&membership=" + isMembership;
        }
    }
</script>
