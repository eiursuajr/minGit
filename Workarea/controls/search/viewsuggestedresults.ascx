<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewsuggestedresults.ascx.cs" Inherits="Workarea_controls_search_viewsuggestedresults" %>

<asp:Literal ID="PostBackPage" runat="server" />

<div style="display: none; border: 1px; background-color: white; position: absolute;
    top: 48px; width: 100%; height: 1px; margin: 0px auto;" id="dvHoldMessage">
    <table border="1px" width="100%" style="background-color: #fff;">
        <tr>
            <td valign="top" align="center" style="white-space: nowrap">
                <h3 style="color: red">
                    <strong>
                        <%=messageHelper.GetMessage("one moment msg")%>
                    </strong>
                </h3>
            </td>
        </tr>
    </table>
</div>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <asp:Literal ID="suggestedResultOutput" runat="server" />
    
    <table id="tblSuggestedResultSets" runat="server" class="ektronGrid">
        <tr class="title-header">
            <th><%= messageHelper.GetMessage("lbl suggested results header search for") %></th>            
            <th><%= messageHelper.GetMessage("msg view suggested results") %></th>
        </tr>
    
    </table>
</div>
