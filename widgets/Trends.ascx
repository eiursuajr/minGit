<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Trends.ascx.cs" Inherits="widgets_Trends" %>
<%@ Register TagPrefix="BA" TagName="AnalyticsList" Src="~/workarea/analytics/reporting/AnalyticsList.ascx" %>
<%@ Register Src="~/widgets/ListSummary/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>
<%@ Register Src="~/widgets/ContentBlock/taxonomytree.ascx" TagPrefix="UC" TagName="TaxonomyTree" %>

    <style type="text/css">
        .editPanel { font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif; font-size: 1em; }
        .editPanel input, .editPanel textarea { font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif; font-size: 1em; height: 1.1em;  }
        .editPanel select, .editPanel button { font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif; font-size: 1em; height: 1.5em;  vertical-align: bottom; }
        input.InputButton { font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif; font-size: 1em; height: 2em;  vertical-align: bottom; }
		input.numberBox { width: 3em; text-align: right; }
        div.LSfoldercontainer span.selected, div.treecontainer .selected { border: solid 2px #9cf;  display: inline !important;}
        span.folder { cursor: pointer; }
        div#PageViewFolder, div#PageViewTaxonomy { border: solid 1px grey; }
        .editPanel .buttonIcon { width: 16px; height: 16px !important; }
        .editPanel .readonly { color: Gray; font: italic; }
    </style>
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
           <BA:AnalyticsList ID="AnalyticList" runat="server" />
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div id="<%=ClientID%>_edit" class="editPanel">
                <table>
                <tr>
                    <td><asp:Label id="lblReportType" AssociatedControlID="ReportTypeList" runat="server">Report type:</asp:Label></td>
                    <td colspan="2"><asp:DropDownList ID="ReportTypeList" runat="server" ></asp:DropDownList></td>
                </tr>
                <tr>
                    <td><asp:Label id="lblDuration" AssociatedControlID="DurationTextBox" runat="server">Number of days to report:</asp:Label></td>
                    <td><asp:TextBox ID="DurationTextBox" runat="server" CssClass="numberBox" Text="7" MaxLength="4"></asp:TextBox></td>
                    <td><asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="DurationTextBox"
                            ErrorMessage="Please enter a valid numeric value." ValidationExpression="[0-9]+"></asp:RegularExpressionValidator></td>
                </tr>
                <tr>
                    <td><asp:Label id="lblOrderDir" AssociatedControlID="OrderDirList" runat="server">Order direction:</asp:Label></td>
                    <td colspan="2"><asp:DropDownList ID="OrderDirList" runat="server" ></asp:DropDownList></td>
                </tr>
                <tr>
                    <td><asp:Label id="lblPageSize" AssociatedControlID="PageSizeTextBox" runat="server">Maximum number of items:</asp:Label></td>
                    <td><asp:TextBox ID="PageSizeTextBox" runat="server" CssClass="numberBox" MaxLength="2"></asp:TextBox></td>
                    <td><asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="PageSizeTextBox"
                            ErrorMessage="Please enter a valid numeric value." ValidationExpression="[1-9]{1}[0-9]*"></asp:RegularExpressionValidator></td>
                </tr>
                <tr>
                    <td><asp:Label id="lblFilterBy" AssociatedControlID="FilterObjectsList" runat="server">Filter by:</asp:Label></td>
                    <td colspan="2" class="readonly"><asp:Literal ID="filterDisplay" runat="server" /></td>
                </tr>
                <tr>
                    <td><input type="hidden" id="chkFilterObjectRecur" runat="server" /></td>
                    <td><asp:DropDownList ID="FilterObjectsList" runat="server" ></asp:DropDownList><input type="hidden" ID="FilterIdTextBox" runat="server" /></td>
                    <td>
                        <asp:ImageButton ID="linkPopup" CssClass="buttonIcon" OnClick="linkPopup_Click" runat="server" ToolTip="XXX" />&#160;&#160;
                        <asp:ImageButton ID="linkDelete" OnClick="linkDelete_Click" runat="server" CssClass="buttonIcon" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CssClass="InputButton" /> &nbsp;&nbsp;
                        <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" CssClass="InputButton" />
                    </td>
                </tr>
                </table>

                
            </div>
        </asp:View>
        <asp:View ID="FilterPopup" runat="server">
            <div class="filtercontainer">
                <input type="hidden" ID="resTitle" class="restitle" runat="server" />
                <div id="PageViewFolder" class="LSWidget">
                    <input type="hidden" ID="folderId" class="folderid" runat="server" />
                    <asp:TextBox ID="tbData" CssClass="HiddenTBData" runat="server" Style="display: none;"></asp:TextBox>
                    <asp:TextBox ID="tbFolderPath" CssClass="HiddenTBFolderPath" runat="server" Style="display: none;"></asp:TextBox>
                    <UC:FolderTree ID="trends_foldertree" runat="server" />
                </div>
                <div id="PageViewTaxonomy" class="UCTaxTree">
                    <asp:TextBox ID="tbTaxonomyPath" CssClass="HiddenTBTaxonomyPath" runat="server" style="display:none;"></asp:TextBox>
                    <input type="hidden" ID="TaxId" class="taxid" runat="server" />
                    <UC:TaxonomyTree ID="trends_taxtree" runat="server" />
                </div>
                <table>
                <tr>
                    <td colspan="3"><asp:CheckBox ID="chkRecursion" CssClass="chkRecursion" runat="server" />
                    <asp:Label id="lblRecursion" AssociatedControlID="chkRecursion" runat="server">Include sub-items</asp:Label></td>
                </tr>
                <tr>
                    <td colspan="3" align="center">
                        <asp:Button ID="Button1" runat="server" Text="Cancel" OnClick="CancelPopupButton_Click" CssClass="InputButton" /> &nbsp;&nbsp;
                        <asp:Button ID="Button2" runat="server" Text="Save" OnClick="SavePopupButton_Click" CssClass="InputButton" />
                    </td>
                </tr>
                </table>
            </div>
        </asp:View>
    </asp:MultiView>
    <asp:HiddenField ID="uxFilterType" runat="server" />
