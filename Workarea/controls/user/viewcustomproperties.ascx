<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewcustomproperties.ascx.cs" Inherits="viewcustomproperties" %>
<script type="text/javascript" language="javascript">
    function Move(sDir, objList, objOrder) {
		if (objList.selectedIndex != null) {
			nSelIndex = objList.selectedIndex;
			sSelValue = objList[nSelIndex].value;
			sSelText = objList[nSelIndex].text;
			objList[nSelIndex].selected = false;
			if (sDir == "up" && nSelIndex > 0) {
				sSwitchValue = objList[nSelIndex -1].value;
				sSwitchText = objList[nSelIndex - 1].text;
				objList[nSelIndex].value = sSwitchValue;
				objList[nSelIndex].text = sSwitchText;
				objList[nSelIndex - 1].value = sSelValue;
				objList[nSelIndex - 1].text = sSelText;
				objList[nSelIndex - 1].selected = true;
			}
			else if (sDir == "dn" && nSelIndex < (objList.length - 1)) {
				sSwitchValue = objList[nSelIndex + 1].value;
				sSwitchText = objList[nSelIndex +  1].text;
				objList[nSelIndex].value = sSwitchValue;
				objList[nSelIndex].text = sSwitchText;
				objList[nSelIndex + 1].value = sSelValue;
				objList[nSelIndex + 1].text = sSelText;
				objList[nSelIndex + 1].selected = true;
			}
		}
		objOrder.value = "";
		for (i = 0; i < objList.length; i++) {
			objOrder.value = objOrder.value + objList[i].value;
			if (i < (objList.length - 1)) {
				objOrder.value = objOrder.value + ",";
			}
		}
	}
	function SetObjectType(obj)
    {
        $ektron("input#isPostData").attr("value", "");
        location.href = "customproperties.aspx?action=viewall&type=" + obj.selectedIndex + "";
        return false;
    }
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid">
    <asp:datagrid id="ViewAllGrid" 
        runat="server"
        AutoGenerateColumns="False"
        Width="100%"
        CssClass="ektronGrid"
        GridLines="None">
        <HeaderStyle CssClass="title-header" />				
    </asp:datagrid>

    <asp:Literal id="litReOrder" runat="server" />
</div>