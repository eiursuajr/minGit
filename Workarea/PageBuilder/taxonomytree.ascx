<%@ Control Language="C#" AutoEventWireup="true" CodeFile="taxonomytree.ascx.cs" Inherits="Workarea_pagebuilder_taxonomytree" %>
<asp:Label ID="noTaxonomies" CssClass="NoTaxonomiesLabel" runat="server" />
<asp:Literal ID="script" runat="server" Visible="false"></asp:Literal>
<div class="treecontainer">
    <ul class="EktronTaxonomyTree">
    <asp:Repeater ID="taxonomies" runat="server">
        <ItemTemplate>
            <li class="closed">
                <span class="folder" data-ektron-taxid="<%#DataBinder.Eval(Container.DataItem, "TaxonomyId")%>">
                    <input type="checkbox" title="<%#DataBinder.Eval(Container.DataItem, "TaxonomyName")%>" class="categoryCheck"><%#DataBinder.Eval(Container.DataItem, "TaxonomyName")%>
                </span>
                <ul data-ektron-taxid="<%#DataBinder.Eval(Container.DataItem, "TaxonomyId")%>"></ul>
            </li>
        </ItemTemplate>
    </asp:Repeater>
    </ul>
</div>
<span style="display:none;" class="hdnSelectedNodes"><asp:TextBox ID="txtselectedTaxonomyNodes" runat="server"></asp:TextBox></span>
<span style="display:none;" class="hdnJSCallBack"><asp:TextBox ID="txtJSCallBack" runat="server"></asp:TextBox></span>
<span id="taxRequired" class="TaxRequiredBool" runat="server" style="display:none;"></span>
