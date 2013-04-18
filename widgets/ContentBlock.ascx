<%@ Control Language="C#" AutoEventWireup="true" Debug="true" CodeFile="ContentBlock.ascx.cs" Inherits="Widgets_ContentBlock" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
<%@ Register Src="~/widgets/ContentBlock/foldertree.ascx" TagPrefix="UC" TagName="FolderTree" %>
<%@ Register Src="~/widgets/ContentBlock/taxonomytree.ascx" TagPrefix="UC" TagName="TaxonomyTree" %>

<asp:MultiView ID="ViewSet" runat="server">
    <asp:View ID="View" runat="server">
        <asp:Literal ID="litGoogleHeader" Text="" runat="server"></asp:Literal>
        <CMS:ContentBlock ID="CB" runat="server" Visible="false" />
        <CMS:FormBlock ID="FB" runat="server" Visible="false" />
        <asp:Literal ID="litGoogleFooter" Text="" runat="server"></asp:Literal>
        <asp:Label ID="errorLb" runat="server" />
    </asp:View>
    <asp:View ID="Edit" runat="server">
        <div id="<%=ClientID%>" class="CBWidget">
            <div class="CBEdit">
                <asp:Label ID="editError" runat="server" />
                <div id="CBFilterOptions">
                    <span class="FilterOption">Filter By:</span>
                    <select id="CBTypeFilter">
                        <option title="content" value="content">Content</option>
                        <option title="forms" value="forms">Forms</option>
                        <option title="assets" value="assets">Multimedia</option>
                    </select>
                </div>
                <div class="CBTabInterface">
                    <ul class="CBTabWrapper clearfix">
                        <li class="CBTab selected"><a href="#ByFolder"><span>Folder</span></a></li>
                        <li class="CBTab"><a href="#ByTaxonomy"><span>Taxonomy</span></a></li>
                        <li class="CBTab"><a href="#BySearch"><span>Search</span></a></li>
                        <li class="CBTab" runat="server" id="tabTesting"><a href="#ByABTesting"><span>AB-Testing</span></a></li>
                    </ul>
                    <div class="ByFolder CBTabPanel">
                        <UC:FolderTree ID="CBFolderTree" runat="server" />
                    </div>
                    <div class="ByTaxonomy CBTabPanel">
                        <UC:TaxonomyTree ID="CBTaxTree" runat="server" />
                    </div>
                    <div class="BySearch CBTabPanel">
                        Enter Search Terms: <input type="text" class="searchtext" /><a href="#" class="searchSubmit" onclick="return Ektron.PFWidgets.ContentBlock.Search.DoSearch(this);" title="Search">Search</a>
                    </div>
                    <div class="ByABTesting CBTabPanel" style="display: none;">
                        <div class="google-variate-checkbox">
                            <asp:CheckBox ID="cbMultiVariate" runat="server" Text="Google Multivariate" />
                        </div>
                        <label for="testing">Section Name: </label>
                        <asp:TextBox ID="tbSectionName" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div id="ResultsToggle"><a href="#" onclick="return Ektron.PFWidgets.ContentBlock.toggleResultsPane();">View Results</a></div>
                <div id="CBResults" style="display:none;"></div>
                <div id="CBPaging"></div>
                <asp:TextBox ID="tbData" CssClass="HiddenTBData" runat="server" style="display:none;"></asp:TextBox>
                <asp:TextBox ID="tbFolderPath" CssClass="HiddenTBFolderPath" runat="server" style="display:none;"></asp:TextBox>
                <div class="CBEditControls">
                    <asp:Button ID="CancelButton" CssClass="CBCancel" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                    <asp:Button ID="SaveButton" CssClass="CBSave" runat="server" Text="Save" OnClick="SaveButton_Click" OnClientClick="return Ektron.PFWidgets.ContentBlock.Save();" />
                    <asp:Button ID="NewButton" CssClass="CBAdd" runat="server" Text="New"  OnClientClick="EkTbWebMenuPopUpWindow(document.getElementById('hdnAppPath').value + 'editarea.aspx?LangType='+ document.getElementById('hdnLangType').value +'&amp;id='+document.getElementById('hdnFolderId').value+'&amp;control=cbwidget&amp;buttonid='+ this.id + '&amp;type=add&amp;pullapproval=', 'Add', 900,  580 , 1, 1);return false;"/>
                    <asp:Button ID="NewFormButton" CssClass="CBAddForm" runat="server" Text="New" OnClientClick="EkTbWebMenuPopUpWindow(document.getElementById('hdnAppPath').value + 'cmsform.aspx?action=Addform&ContType=2&LangType='+ document.getElementById('hdnLangType').value +'&type=add&createtask=1&id=0&folder_id='+document.getElementById('hdnFolderId').value+'&back_file=content.aspx&back_action=ViewContentByCategory&back_id=0&back_LangType='+ document.getElementById('hdnLangType').value +'&buttonid='+ this.id, 'Add', 900,  580 , 1, 1);return false;"/>
                </div>
            </div>
            <input type="hidden" id="hdnAppPath" name = "hdnAppPath" value="<%=appPath%>" />
            <input type="hidden" id="hdnLangType" name ="hdnLangType" value="<%=langType%>" />
            <input type="hidden" id="hdnFolderId" name="hdnFolderId" value="0"/>
        </div>
    </asp:View>
</asp:MultiView>