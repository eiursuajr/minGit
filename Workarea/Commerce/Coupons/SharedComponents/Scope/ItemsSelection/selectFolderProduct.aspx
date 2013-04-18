<%@ Page Language="C#" AutoEventWireup="true" CodeFile="selectFolderProduct.aspx.cs" Inherits="Ektron.Cms.Commerce.Workarea.Coupons.SelectFolderProduct" %>
<%@ Register TagPrefix="ucEktron" TagName="Paging" Src="../../../../../controls/paging/paging.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <title>Select Folder</title>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <style type="text/css">
            .baseClassToolbar {display:none !important;}
        </style>
    </head>
    <body class="UiMain" onclick="MenuUtil.hide();">
        <form id="frmSelectFolderProduct" runat="server">
            <input type="hidden" id="hdnData" runat="server" class="data" name="SelectFolderData" value="" />
            <input type="hidden" id="hdnFolderList" runat="server" class="folderIds" name="SelectFolderFolderList" value="" />
            <input type="hidden" id="hdnProductList" runat="server" class="productIds" name="SelectFolderProductList" value="" />
            <input type="hidden" id="hdnFolderId" runat="server" class="folderId" name="SelectFolderFolderId" value="" />
            <asp:PlaceHolder ID="phLocalizedStrings" runat="server">
                <input type="hidden" class="localizedStrings" name="SelectFolder" value='<%=GetLocalizedJavascriptStrings()%>' />
            </asp:PlaceHolder>
            <p class="currentFolder"><asp:Literal id="litCurrentFolderName" runat="server"></asp:Literal></p>
            <div class="ektronPageGrid">
                <asp:GridView ID="gvFolders"
                    runat="server"
                    CssClass="folders selectFolderProduct"
                    OnRowDataBound="gvFolders_OnRowDataBound"
                    AutoGenerateColumns="false"
                    ShowHeader="false"
                    BorderStyle="None"
                    BorderWidth="0">
                    <Columns>
                        <asp:TemplateField ItemStyle-CssClass="data">
                            <ItemTemplate>
                                <input id="cbxFolder" runat="server" type="checkbox" class="selected" onclick="Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.Click.checkbox(this);" />
                                <input id="hdnFolderId" runat="server" type="hidden" class="id" name="CatalogEntry" />
                                <input id="hdnFolderName" runat="server" type="hidden" class="name" name="CatalogEntry" />
                                <input id="hdnFolderPath" runat="server" type="hidden" class="path" name="CatalogEntry" />
                                <input type="hidden" class="type" name="CatalogEntry" value="catalog" />
                                <input type="hidden" class="typeCode" name="CatalogEntry" value="1" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <img id="imgFolderIcon" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a id="aFolder" runat="server" href="#ClickToDrillDown"></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:GridView ID="gvProducts"
                    runat="server"
                    CssClass="products selectFolderProduct"
                    OnRowDataBound="gvProducts_OnRowDataBound"
                    AutoGenerateColumns="false"
                    ShowHeader="false"
                    BorderWidth="0">
                    <Columns>
                        <asp:TemplateField ItemStyle-CssClass="data">
                            <ItemTemplate>
                                <input id="cbxProduct" runat="server" type="checkbox" class="selected" onclick="Ektron.Commerce.Coupons.Scope.Items.SelectFolderProduct.Actions.Click.checkbox(this);" />
                                <input id="hdnProductId" runat="server" type="hidden" class="id" name="CatalogEntry" />
                                <input id="hdnProductName" runat="server" type="hidden" class="name" name="CatalogEntry" />
                                <input id="hdnProductPath" runat="server" type="hidden" class="path" name="CatalogEntry" />
                                <input type="hidden" class="type" name="CatalogEntry" value="product" />
                                <input id="hdnProductSubType" runat="server" type="hidden" class="subtype" name="CatalogEntry" />
                                <input type="hidden" class="typeCode" name="CatalogEntry" value="0" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <img id="imgProduct" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <span id="spanProduct" runat="server"></span>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <ucEktron:Paging ID="uxPaging" runat="server" />
            </div>
        </form>
    </body>
</html>
