<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewDeviceConfiguration.ascx.cs" Inherits="ViewDeviceConfiguration" %>
    
    <script type="text/javascript" language="javascript">
        function ConfirmConfigurationDelete()
         {
		    return confirm('<%=_MessageHelper.GetMessage("js:alert you sure you wish to delete this configuration Continue?")%>');
	     }
    </script>
    
    
    <div id="dhtmltooltip"></div>
    <div class="ektronPageHeader">
         <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
         <div class="ektronToolbar" id="divToolBar" runat="server"></div>
    </div>
    <div class="ektronPageContainer">
         <div id="dvDeviceAdd" class="ektronPageInfo">
              <table class="ektronForm">
                 <tr>
                    <td class="label" ><%=_MessageHelper.GetMessage("lbl Device")%>:</td>
                    <td class="value">
                          <asp:Label ID="lblDeviceName" runat="server" />
                    </td>
                 </tr>
                 <tr>
                    <div id="dvModels" runat="server" >
                          <asp:Literal ID="ltrModels" runat="server" ></asp:Literal>
                    </div>
                 </tr>
                     <tr id="trDeviceType" runat="server" visible="false">
                        <td id="lblDeviceType" class="label" title="">:</td>
                        <td class="value">
                            <asp:Literal ID="ltrDeviceType" runat="server" ></asp:Literal>
                        </td>
                     </tr>
                 <tr>
                     <td class="label" ><%=_MessageHelper.GetMessage("lbl device preview width")%>:</td>
                     <td class="value">
                         <asp:Label  id="lblPreviewWidth" runat="server"/> px
                    </td>
                 </tr>
                 <tr>
                     <td class="label" ><%=_MessageHelper.GetMessage("lbl device preview height")%>:</td>
                     <td class="value">
                         <asp:Label  id="lblPreviewHeight" runat="server"/> px
                    </td>
                 </tr> 
                 </table>
            </div>
            </div>
            
     </div>