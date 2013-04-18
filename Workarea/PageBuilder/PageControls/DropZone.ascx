<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DropZone.ascx.cs" Inherits="Ektron.Cms.PageBuilder.Controls.UCDropZone" %>
<%@ Register Src="WidgetHost.ascx" TagPrefix="EktronUC" TagName="WidgetHost" %>

<asp:Literal ID="blockuiCall" runat="server" Visible="false" EnableViewState="false">
<script type="text/javascript">
    <!--//--><![CDATA[//><!--
        Ektron.ready(function() {
        	Sys.WebForms.PageRequestManager.getInstance().add_beginRequest(Ektron.PageBuilder.WidgetHost.BlockUI);
        	Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.PageBuilder.WidgetHost.unBlockUI);
    	});
    //--><!]]>
</script>
</asp:Literal>

<asp:UpdatePanel ID="updatepanel" runat="server" OnLoad="DropZonePanelLoad" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="dropzone PBClear" id="dzcontainer" runat="server">
            <div class="PBDZHeader" id="dzheader" runat="server" visible="false" enableviewstate="false">
                <a href="#" title="Lock - Set as Layout Zone" onclick="Ektron.PageBuilder.WidgetHost.setMasterDropZone(this);return false;" id="masterzoneselect" runat="server" class="masterzoneselect" visible="false">
                    <img style="border:none;" alt="" id="imgsetmasterzone" runat="server" class="PBMasterbutton PB-UI-icon" src="#" />
                </a>
                <asp:ImageButton ID="AddColumn" runat="server" ToolTip="Add Column" CssClass="PBAddColumn" OnClick="AddColumn_click" />
            </div>
            <asp:Repeater ID="columnDisplay" runat="server">
                <ItemTemplate>
                    <div class="PBColumn" id="zone" runat="server">
                        <ul id="column" class="columnwidgetlist" runat="server">
                            <li class="header" id="headerItem" runat="server" enableviewstate="false">
                                <a href="#" title="Resize Column" class="resizeColumn" onclick="Ektron.PageBuilder.WidgetHost.resizeColumn(this);return false;" runat="server" id="lbResizeColumn">
                                    <img alt="" id="imgresizecolumn" runat="server" class="PBeditbutton PB-UI-icon" src="#" />
                                </a>
                                <a href="#" title="Remove Column" class="remColumn" onclick="Ektron.PageBuilder.WidgetHost.RemoveColumn(this);return false;" id="lbDeleteColumn" runat="server">
                                    <img alt="" id="imgremcolumn" runat="server" class="PBclosebutton PB-UI-icon" src="#" />
                                </a>
                            </li>
                            <asp:Repeater ID="controlcolumn" runat="server">
                                <ItemTemplate>
                                    <li class="PBItem">
                                        <EktronUC:WidgetHost ID="WidgetHost" runat="server" />
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ul>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </ContentTemplate>
</asp:UpdatePanel>

<div class="PBDropzoneError" id="PBDropZoneError" runat="server" visible="false" enableviewstate="false">

</div>