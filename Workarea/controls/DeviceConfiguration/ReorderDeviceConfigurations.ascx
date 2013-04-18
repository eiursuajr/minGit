<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReorderDeviceConfigurations.ascx.cs" Inherits="ReorderDeviceConfigurations" %>
  <script type="text/javascript" language="javascript" >
    
     function Move(sDir, objList, objOrder)
        {
            if (objList.selectedIndex != null && objList.selectedIndex >= 0)
            {
                nSelIndex = objList.selectedIndex;
                sSelValue = objList[nSelIndex].value.split("|");
                sSelText = objList[nSelIndex].text;
                objList[nSelIndex].selected = false;
                if (sDir == "up" && nSelIndex > 0)
                {
                    sSwitchValue = objList[nSelIndex - 1].value.split("|");
                    sSwitchText = objList[nSelIndex - 1].text;
                    objList[nSelIndex].value = sSwitchValue[0] + "|" + sSelValue[1] + "|changed|";
                    objList[nSelIndex].text = sSwitchText;
                    objList[nSelIndex - 1].value = sSelValue[0] + "|" + sSwitchValue[1] + "|changed|";
                    objList[nSelIndex - 1].text = sSelText;
                    objList[nSelIndex - 1].selected = true;
                }
                else if (sDir == "dn" && nSelIndex < (objList.length - 1))
                {
                    sSwitchValue = objList[nSelIndex + 1].value.split("|");
                    sSwitchText = objList[nSelIndex + 1].text;
                    objList[nSelIndex].value = sSwitchValue[0] + "|" + sSelValue[1] + "|changed|";
                    objList[nSelIndex].text = sSwitchText;
                    objList[nSelIndex + 1].value = sSelValue[0] + "|" + sSwitchValue[1] + "|changed|";
                    objList[nSelIndex + 1].text = sSelText;
                    objList[nSelIndex + 1].selected = true;
                }
            }
            objOrder.value = "";
            for (i = 0; i < objList.length; i++)
            {
                objOrder.value = objOrder.value + objList[i].value;
                if (i < (objList.length - 1))
                {
                    objOrder.value = objOrder.value + ",";
                }
            }
        }
  
  </script>
  <div id="dhtmltooltip"></div>
  
  <div class="ektronPageHeader">
        <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
        <div class="ektronToolbar" id="divToolBar" runat="server"></div>
  </div>
  
  <div class="ektronPageContainer ektronPageInfo">
    <div class="heightFix">
        <table>
            <tr>
                <td>
                    <select id="OrderList" runat="server" name="OrderList">
                    </select>
                </td>
                <td>
                    &nbsp;&nbsp;
                </td>
                <td>
                    <a href="javascript:Move('up', document.getElementById('<%=OrderList.ClientID %>'), document.getElementById('DeviceOrder'))">
                        <img id="Up" runat="server" />
                    </a>
                    <br />
                    <a href="javascript:Move('dn', document.getElementById('<%=OrderList.ClientID %>'), document.getElementById('DeviceOrder'))">
                        <img id="Down" runat="server" />
                    </a>
                </td>
            </tr>
        </table>
         <input type="hidden" id="DeviceOrder" name="DeviceOrder" value="" />
    </div>
    <div>
       <asp:Literal ID="ltrMessage" runat="server" ></asp:Literal>
    </div>
</div>