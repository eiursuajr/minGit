<%@ Control Language="C#" AutoEventWireup="true" CodeFile="editLocaleTaxonomy.ascx.cs" 
 Inherits="editLocaleTaxonomy" %>

<script type="text/javascript">
    var controlid = "taxonomy_";

//    Ektron.ready(function () { InitCustomPropertyEditting(); });

    function Validate(force) {
        if (force) {
            var taxonomyName = document.getElementById(controlid + "taxonomytitle").value;
            if (document.getElementById(controlid + "taxonomytitle").value == '') {
                alert('<%=m_refMsg.GetMessage("js:alert taxonomy required field")%>');
                return false;
            }
            if (document.getElementById(controlid + 'chkConfigContent') != null && document.getElementById(controlid + 'chkConfigUser') != null && document.getElementById(controlid + 'chkConfigGroup') != null) {
                if (document.getElementById(controlid + 'chkConfigContent').checked == false && document.getElementById(controlid + 'chkConfigGroup').checked == false && document.getElementById(controlid + 'chkConfigUser').checked == false) {
                    alert('<%=m_refMsg.GetMessage("js:alert configuration selection required")%>');
                    return false;
                }
            }
            if ((taxonomyName.indexOf('>') > -1) || (taxonomyName.indexOf('<') > -1) || (taxonomyName.indexOf('"') > -1)) {
                alert("The taxonomy name can not contain <, > or \"");
                return false;
            }
            else {
                document.forms[0].submit();
            }
        }
        document.forms[0].submit();
    }
    function Move(sDir, objList, objOrder) {
        if (objList.selectedIndex != null && objList.selectedIndex >= 0) {
            nSelIndex = objList.selectedIndex;
            sSelValue = objList[nSelIndex].value;
            sSelText = objList[nSelIndex].text;
            objList[nSelIndex].selected = false;
            if (sDir == "up" && nSelIndex > 0) {
                sSwitchValue = objList[nSelIndex - 1].value;
                sSwitchText = objList[nSelIndex - 1].text;
                objList[nSelIndex].value = sSwitchValue;
                objList[nSelIndex].text = sSwitchText;
                objList[nSelIndex - 1].value = sSelValue;
                objList[nSelIndex - 1].text = sSelText;
                objList[nSelIndex - 1].selected = true;
            }
            else if (sDir == "dn" && nSelIndex < (objList.length - 1)) {
                sSwitchValue = objList[nSelIndex + 1].value;
                sSwitchText = objList[nSelIndex + 1].text;
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
    function LoadReorderType(type) {
        document.location = 'LocaleTaxonomy.aspx?action=reorder&reorder=' + type + '&folderid=0&taxonomyid=<%=TaxonomyId %>&parentid=<%=TaxonomyParentId %>';
    }



    function updateText(obj) {
        $ektron("#taxonomy_txtValue")[0].value = obj.value;
    }

//    function InitCustomPropertyEditting() {
//        var handlerUrl =
//            "controls/content/CustomPropertyHandler.ashx?action=getcustompropobjectdata&objectid=" +
//            $ektron("#taxonomy_hdnTaxonomyId")[0].value;

//        $ektron.getJSON(
//            handlerUrl,
//            function (data) {
//                $ektron.each(data, function (index, item) { AddCustomPropertyToTable(item); });
//            });
//    }
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <% if (m_strPageAction == "edit")
       {%>
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <ul>
                <li>
                    <a title="Properties" href="#dvProperties">
                        <%=m_refMsg.GetMessage("properties text")%>
                    </a>
                </li>
               <%-- <li>
                    <a href="#dvMetadata">
                        <%=m_refMsg.GetMessage("custom properties")%>
                    </a>
                </li>--%>
            </ul>
            <div title="Properties" id="dvProperties">
                <table class="ektronForm">
                    <tr>
                        <td class="label" title="Title"><%=m_refMsg.GetMessage("locale lbl title")%>:</td>
                        <td class="value">
                            <asp:TextBox ID="taxonomytitle" runat="server" />
                            <asp:Label ID="lblLanguage" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" title="Description"><%=m_refMsg.GetMessage("locale lbl description")%>:</td>
                        <td class="value"><asp:TextBox ToolTip="Description" ID="taxonomydescription" TextMode="MultiLine" runat="server" /></td>
                    </tr>

                </table>
            </div>    
         <%--   <div id="dvMetadata">
                <div style="display:inline">
                    <asp:DropDownList ToolTip="Select a Custom Property" runat="server" ID="availableCustomProp" AppendDataBoundItems="true">
                        <asp:ListItem Value="-1">
                        Select a Custom Property
                        </asp:ListItem>
                    </asp:DropDownList>
                    <a class="greenHover buttonAddTag" style="display:inline; vertical-align:middle;" onclick="AddCustomProperty();"></a>
                </div>
                <div class="ektronTopSpace"></div>
                <table id="customPropertyTable" class="ektronGrid" runat="server">
                    <tr class="title-header">
                        <td width="40%" title="Title">Title</td>
                        <td width="20%" title="Data Type">Data Type</td>
                        <td width="30" title="Value">Value</td>
                        <td width="10%" title="Action">Action</td>
                    </tr>
                </table>
            </div>--%>
        </div>
    </div>
    <%}%>
    <%else
        {%>
        <table width="100%">
            <tr>
                <td valign="top"><asp:ListBox ID="OrderList" runat="server" /></td>
                <td id="td_moveicon" runat="server" align="left" width="80%">
                    <a href="javascript:Move('up', document.forms[0].taxonomy_OrderList, document.forms[0].taxonomy_LinkOrder)">
                        <img src="<%=(AppImgPath)%>moveup.gif" border="0" alt="<%=m_refMsg.GetMessage("move selection up msg")%>"
                            title="<%=(m_refMsg.GetMessage("move selection up msg"))%>" /></a><br />
                    <a href="javascript:Move('dn', document.forms[0].taxonomy_OrderList, document.forms[0].taxonomy_LinkOrder)">
                        <img src="<%=(AppImgPath)%>movedown.gif" border="0" alt="<%=m_refMsg.GetMessage("move selection down msg")%>"
                            title="<%=(m_refMsg.GetMessage("move selection down msg"))%>" /></a>
                </td>
            </tr>
        </table>
        <br />
        <table class="ektronForm" id="AllLangForm" runat="server">
           <tr id="tr_ordering" runat="server">
              <td class="label" title="Apply ordering for all languages"><%=m_refMsg.GetMessage("lbl apply ordering languages")%>:</td>
              <td class="value"><asp:CheckBox ToolTip="Apply ordering for all languages" ID="chkOrderAllLang" runat="server" /></td>
           </tr>
        </table>
        <input type="hidden" runat="server" id="LinkOrder" name="LinkOrder" value="" />
        <script type="text/javascript"><asp:Literal ID="loadscript" runat="server" /></script>
    <%}%>
    <input type="hidden" id="alllanguages" runat="server"  />
    <input type="hidden" id="visibility" runat="server" />
    <input type="hidden" runat="server" id="txtValue" name="txtValue" />
    <input type="hidden" id='hdnSelectedIDS' name='hdnSelectedIDS' runat="server" />
    <input type="hidden" id='hdnSelectValue' name='hdnSelect' runat="server" />
    <input type="hidden" id="hdnTaxonomyId" name="hdnTaxonomyId" runat="server" />
    <asp:Literal Visible="false" ID="td_msg" runat="server" />

</div>