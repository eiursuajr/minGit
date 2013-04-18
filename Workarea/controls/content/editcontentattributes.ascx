<%@ Control Language="C#" AutoEventWireup="true" Inherits="editcontentattributes" CodeFile="editcontentattributes.ascx.cs" %>
    
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    
    <asp:Panel ID="xmlConfigPanel" runat="server">
        <div id="td_ecp_xmlconfiglbl" class="ektronHeader" runat="server"></div>
        <div id="td_ecp_btn" class="ektronHeader" runat="server"></div>

        <table>
            <tr>
                <td id="td_ecp_xmlconfig" runat="server"></td>
                <td id="td_ecp_xmlconfig_lnk" runat="server"></td>
            </tr>
        </table>
    </asp:Panel>

    <div id="searchable" class="ektronHeader" runat="server"></div>
    <div class="ektronTopSpace"></div>
    <div id="flagging" class="ektronHeader" runat="server"></div>
    <%--<asp:DropDownList ID="flaggingDefinitionsDDL" runat="server" ></asp:DropDownList>--%>
    <asp:Label ID="lblflag" runat="server" />
</div>

<input type="hidden" id="content_id" name="content_id" value="0" runat="server" />
<asp:Literal ID="flaggingClientScriptLit" runat="server" />

