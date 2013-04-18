<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MostPopular.ascx.cs" Inherits="widgets_MostPopular" %>
<%@ Register TagPrefix="BA" TagName="AnalyticsList" Src="~/workarea/analytics/reporting/AnalyticsList.ascx" %>
<%@ Register Src="~/widgets/ListSummary/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>
<%@ Register Src="~/widgets/ContentBlock/taxonomytree.ascx" TagPrefix="UC" TagName="TaxonomyTree" %>

    <style type="text/css">
        .editPanel { font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif; font-size: 1em; }
        .editPanel input, .editPanel textarea { font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif; font-size: 1em; height: 1.1em; }
        .editPanel select, .editPanel button, input.InputButton { font-family: Trebuchet MS, Tahoma, Verdana, Arial, sans-serif; font-size: 1em; height: 1.5em !important; vertical-align: bottom; }
        input.numberBox { width: 3em; text-align: right; }
        input.InputButton { margin : 0.2em; }
        div.LSfoldercontainer span.selected, div.treecontainer .selected { border: solid 2px #9cf;  display: inline !important;}
        span.folder { cursor: pointer; }
        div#PageViewFolder, div#PageViewTaxonomy { border: solid 1px grey; }
        .editPanel .buttonIcon { width: 16px; height: 16px !important; }
        .editPanel .readonly { color: Gray; font: italic; }
    </style>
    <div id="Err_MostPopularWidget" runat="server">Only 1 Most Popular widget is supported on a page.</div>
    <asp:MultiView ID="ViewSet" runat="server" ActiveViewIndex="0">
        <asp:View ID="View" runat="server">
            <div id="tabsMostPopular">
	            <ul>
                    <li id="tab1" runat="server"></li>
                    <li id="tab2" runat="server"></li>
                    <li id="tab3" runat="server"></li>
                    <li id="tab4" runat="server"></li>
                </ul>
                <div id="fragment-1">
                    <BA:AnalyticsList ID="List1" OrderDirection="Descending" runat="server" />
                </div>
	            <div id="fragment-2">
                    <BA:AnalyticsList ID="List2" OrderDirection="Descending" runat="server" />
                </div>
	            <div id="fragment-3">
		            <BA:AnalyticsList ID="List3" OrderDirection="Descending" runat="server" />
	            </div>
	            <div id="fragment-4">
                    <BA:AnalyticsList ID="List4" OrderDirection="Descending" runat="server" />
                </div> 
            </div>
        </asp:View>
        <asp:View ID="Edit" runat="server">
            <div id="<%=ClientID%>_edit" class="editPanel">
                 <div id="tabsMostPopular_edit">
	                <ul>
                        <li id="tab1_edit" runat="server"></li>
                        <li id="tab2_edit" runat="server"></li>
                        <li id="tab3_edit" runat="server"></li>
                        <li id="tab4_edit" runat="server"></li>
                    </ul>
                    <div id="editfragment-1">
                        <table>
                        <tr>
                            <td><asp:Label id="lblTab1" AssociatedControlID="tab1TextBox" runat="server">Tab text:</asp:Label></td>
                            <td><asp:TextBox ID="tab1TextBox" runat="server" Style="width: 20em;"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td><asp:Label id="lblDuration1" AssociatedControlID="durationTextBox1" runat="server">Number of days to report:</asp:Label></td>
                            <td><asp:TextBox ID="durationTextBox1" runat="server" CssClass="numberBox" Text="7" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="durationTextBox1"
                            ErrorMessage="Please enter a valid numeric value." ValidationExpression="[0-9]+"></asp:RegularExpressionValidator></td>
                        </tr>
                        <tr>
                            <td><asp:Label id="lblEvent1" AssociatedControlID="ReportTypeList1" runat="server">Tab 1 Event:</asp:Label></td>
                            <td><asp:DropDownList ID="ReportTypeList1" runat="server" ></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td><asp:Label id="lblFilterBy1" AssociatedControlID="FilterObjectsList1" runat="server">Filter by:</asp:Label></td>
                            <td class="readonly"><asp:Literal ID="filterDisplay1" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><input type="hidden" id="chkFilterObjectRecur1" runat="server" /></td>
                            <td><asp:DropDownList ID="FilterObjectsList1" runat="server" ></asp:DropDownList><input type="hidden" ID="FilterIdTextBox1" runat="server" />
                            <asp:ImageButton ID="linkPopup1" CssClass="buttonIcon" OnClick="linkPopup_Click" runat="server" />&#160;&#160;<asp:ImageButton ID="linkDelete1" OnClick="linkDelete_Click" runat="server" CssClass="buttonIcon" /></td>
                        </tr>
                        <!-- need to have at least 1 tab in the widget. therefore, this tab cannot be invisible -->
                        </table>
                    </div>
                    <div id="editfragment-2">
                        <table>
                        <tr>
                            <td><asp:Label id="lblTab2" AssociatedControlID="tab2TextBox" runat="server">Tab text:</asp:Label></td>
                            <td><asp:TextBox ID="tab2TextBox" runat="server" Style="width: 20em;"></asp:TextBox>
                            <asp:CheckBox ID="chkTab2" runat="server" Checked="true" />
                            <asp:Label id="lblCheckVisible2" AssociatedControlID="chkTab2" runat="server">Visible</asp:Label></td>
                        </tr>
                        <tr>
                            <td><asp:Label id="lblDuration2" AssociatedControlID="durationTextBox2" runat="server">Number of days to report:</asp:Label></td>
                            <td><asp:TextBox ID="durationTextBox2" runat="server" CssClass="numberBox" Text="7" MaxLength="4"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="durationTextBox2"
                            ErrorMessage="Please enter a valid numeric value." ValidationExpression="[0-9]+"></asp:RegularExpressionValidator></td>
                        </tr>
                        <tr>
                            <td><asp:Label id="lblEvent2" AssociatedControlID="ReportTypeList2" runat="server">Tab 1 Event:</asp:Label></td>
                            <td><asp:DropDownList ID="ReportTypeList2" runat="server" ></asp:DropDownList></td>
                        </tr>
                        <tr>
                            <td><asp:Label id="lblFilterBy2" AssociatedControlID="FilterObjectsList2" runat="server">Filter by:</asp:Label></td>
                            <td class="readonly"><asp:Literal ID="filterDisplay2" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><input type="hidden" id="chkFilterObjectRecur2" runat="server" /></td>
                            <td><asp:DropDownList ID="FilterObjectsList2" runat="server" ></asp:DropDownList><input type="hidden" ID="FilterIdTextBox2" runat="server" />
                            <asp:ImageButton ID="linkPopup2" CssClass="buttonIcon" OnClick="linkPopup_Click" runat="server" />&#160;&#160;<asp:ImageButton ID="linkDelete2" OnClick="linkDelete_Click" runat="server" CssClass="buttonIcon" /></td>
                        </tr>
                        </table>
                    </div>
	                <div id="editfragment-3">
	                    <table>
	                    <tr>
	                        <td><asp:Label id="lblTab3" AssociatedControlID="tab3TextBox" runat="server">Tab text:</asp:Label></td>
	                        <td><asp:TextBox ID="tab3TextBox" runat="server" Style="width: 20em;"></asp:TextBox>
	                        <asp:CheckBox ID="chkTab3" runat="server" Checked="true" />
	                        <asp:Label id="lblCheckVisible3" AssociatedControlID="chkTab3" runat="server">Visible</asp:Label></td>
	                    </tr>
	                    <tr>
	                        <td><asp:Label id="lblDuration3" AssociatedControlID="durationTextBox3" runat="server">Number of days to report:</asp:Label></td>
	                        <td><asp:TextBox ID="durationTextBox3" runat="server" CssClass="numberBox" Text="7" MaxLength="4"></asp:TextBox>
	                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="durationTextBox3"
                            ErrorMessage="Please enter a valid numeric value." ValidationExpression="[0-9]+"></asp:RegularExpressionValidator></td>
	                    </tr>
	                    <tr>
	                        <td><asp:Label id="lblEvent3" AssociatedControlID="ReportTypeList3" runat="server">Tab 1 Event:</asp:Label></td>
	                        <td><asp:DropDownList ID="ReportTypeList3" runat="server" ></asp:DropDownList></td>
	                    </tr>
	                    <tr>
                            <td><asp:Label id="lblFilterBy3" AssociatedControlID="FilterObjectsList3" runat="server">Filter by:</asp:Label></td>
                            <td class="readonly"><asp:Literal ID="filterDisplay3" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><input type="hidden" id="chkFilterObjectRecur3" runat="server" /></td>
                            <td><asp:DropDownList ID="FilterObjectsList3" runat="server" ></asp:DropDownList><input type="hidden" ID="FilterIdTextBox3" runat="server" />
                            <asp:ImageButton ID="linkPopup3" CssClass="buttonIcon" OnClick="linkPopup_Click" runat="server" />&#160;&#160;<asp:ImageButton ID="linkDelete3" OnClick="linkDelete_Click" runat="server" CssClass="buttonIcon" /></td>
                        </tr>
	                    </table>
	                </div>
	                <div id="editfragment-4">
	                    <table>
	                    <tr>
	                        <td><asp:Label id="lblTab4" AssociatedControlID="tab4TextBox" runat="server">Tab text:</asp:Label></td>
	                        <td><asp:TextBox ID="tab4TextBox" runat="server" Style="width: 20em;"></asp:TextBox>
	                        <asp:CheckBox ID="chkTab4" runat="server" Checked="true" />
	                        <asp:Label id="lblCheckVisible4" AssociatedControlID="chkTab4" runat="server">Visible</asp:Label></td>
	                    </tr>
	                    <tr>
	                        <td><asp:Label id="lblDuration4" AssociatedControlID="durationTextBox4" runat="server">Number of days to report:</asp:Label></td>
	                        <td><asp:TextBox ID="durationTextBox4" runat="server" CssClass="numberBox" Text="7" MaxLength="4"></asp:TextBox>
	                        <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" ControlToValidate="durationTextBox4"
                            ErrorMessage="Please enter a valid numeric value." ValidationExpression="[0-9]+"></asp:RegularExpressionValidator></td>
	                    </tr>
	                    <tr>
	                        <td><asp:Label id="lblEvent4" AssociatedControlID="ReportTypeList4" runat="server">Tab 1 Event:</asp:Label></td>
	                        <td><asp:DropDownList ID="ReportTypeList4" runat="server" ></asp:DropDownList></td>
	                    </tr>
	                    <tr>
                            <td><asp:Label id="lblFilterBy4" AssociatedControlID="FilterObjectsList4" runat="server">Filter by:</asp:Label></td>
                            <td class="readonly"><asp:Literal ID="filterDisplay4" runat="server" /></td>
                        </tr>
                        <tr>
                            <td><input type="hidden" id="chkFilterObjectRecur4" runat="server" /></td>
                            <td><asp:DropDownList ID="FilterObjectsList4" runat="server" ></asp:DropDownList><input type="hidden" ID="FilterIdTextBox4" runat="server" />
                            <asp:ImageButton ID="linkPopup4" CssClass="buttonIcon" OnClick="linkPopup_Click" runat="server" />&#160;&#160;<asp:ImageButton ID="linkDelete4" OnClick="linkDelete_Click" runat="server" CssClass="buttonIcon" /></td>
                        </tr>
	                    </table>
	                </div> 
                 </div> 
                 <div align="center">
                 <asp:Button CssClass="InputButton" ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" /> &nbsp;&nbsp;
                 <asp:Button CssClass="InputButton" ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                 </div>
            </div>
        </asp:View>
        <asp:View ID="FilterPopup" runat="server">
            <div class="filtercontainer">
                <input type="hidden" id="tabNum" runat="server" />
                <input type="hidden" ID="resTitle" class="restitle" runat="server" />
                <div id="PageViewFolder" class="LSWidget">
                    <input type="hidden" ID="folderId" class="folderid" runat="server" />
                    <asp:TextBox ID="tbData" CssClass="HiddenTBData" runat="server" Style="display: none;"></asp:TextBox>
                    <asp:TextBox ID="tbFolderPath" CssClass="HiddenTBFolderPath" runat="server" Style="display: none;"></asp:TextBox>
                    <UC:FolderTree ID="mostpopular_foldertree" runat="server" />
                </div>
                <div id="PageViewTaxonomy" class="UCTaxTree">
                    <input type="hidden" ID="TaxId" class="taxid" runat="server" />
                    <asp:TextBox ID="tbTaxonomyPath" CssClass="HiddenTBTaxonomyPath" runat="server" style="display:none;"></asp:TextBox>
                    <UC:TaxonomyTree ID="mostpopular_taxtree" runat="server" />
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

