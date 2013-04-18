<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewAllDeviceConfigurations.ascx.cs" Inherits="ViewAllDeviceConfigurations" %>

    <div id="dhtmltooltip"></div>
    <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    </div>

    <div class="ektronPageContainer">
         <div  visible="false" runat="server" id="dvMessage">
            <span class="required">
                <asp:Label  ID="lblEnableDeviceDetection" runat="server" ></asp:Label>
            </span>
         </div>
         <div title="Device Type List" id="dvDeviceList" class="ektronPageInfo">
              <asp:DataGrid id="DeviceListGrid"
                            runat="server"
                            CssClass="ektronGrid"
                            AutoGenerateColumns="False"
                            EnableViewState="False">
                            <HeaderStyle CssClass="title-header" />
              </asp:DataGrid>
         </div>
         
   </div> 
   