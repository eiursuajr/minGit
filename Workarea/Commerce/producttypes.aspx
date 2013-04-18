<%@ Page Language="C#" AutoEventWireup="true" Inherits="product_type" CodeFile="producttypes.aspx.cs" %>
<%@ Register TagPrefix="ucEktron" TagName="Attributes" Src="ProductTypes/Attributes/Attributes.ascx" %>
<%@ Register TagPrefix="ucEktron" TagName="MediaDefaults" Src="ProductTypes/MediaDefaults/MediaDefaults.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <meta content="text/html; charset=UTF-8" http-equiv="content-type" />
        <title>Product Types</title>
        <asp:literal id="ltr_js" runat="server" />
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
                function resetPostback() { document.forms[0].isPostData.value = ""; }
                function toggleSubscriptionRow(elem, show) {
                    var subscriptionProvider = $ektron(elem).parent().find("#tr_provider");
                    if (show == true) {
                        subscriptionProvider.css("display", "inline");
                    } else {
                       subscriptionProvider.hide();
                    }
                }
            //--><!]]>
        </script>
        <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
                #tr_provider {display:none;}
                td#htmToolBar {height:22px !important;}  /* note: workaround */
                span.subscription {padding-left:20px;background-image:url('../images/ui/icons/bookGreen.png');background-position:0 50%;background-repeat:no-repeat;}
                span.complexproduct {padding-left:20px;background-image:url('../images/ui/icons/bricks.png');background-position:0 50%;background-repeat:no-repeat;}
                span.bundle {padding-left:20px;background-image:url('../images/ui/icons/package.png');background-position:0 50%;background-repeat:no-repeat;}
                span.kit {padding-left:20px;background-image:url('../images/ui/icons/box.png');background-position:0 50%;background-repeat:no-repeat;}
                span.product {padding-left:20px;background-image:url('../images/ui/icons/brick.png');background-position:0 50%;background-repeat:no-repeat;}
                div.innerTable {}
                div.innerTable table {border-collapse:collapse;width:98%}
                div.innerTable table th {text-align:right;white-space:normal;width:20%;}
                div.innerTable table th, div.innerTable table td {padding:.5em;border:none;border:1px solid silver;}
                div.innerTable table td {width:80%;}
                div.xPaths h3 {padding:1em 1em .25em 0em;margin:0;font-weight:bold;color:#1d5987;}
                div.xPaths ul {list-style:none;margin:0;padding:0;border:1px solid #d5e7f5;}
                div.xPaths ul li {display:block;padding:.25em;border-bottom:none;}
                div.xPaths ul li.stripe {background-color:#e7f0f7;}
            /*]]>*/-->
        </style>
    </head>
    <body onclick="MenuUtil.hide()">
        <form id="xmlconfiguration" method="post" runat="server">
            <div id="dhtmltooltip"></div>
            <asp:MultiView ID="mvViews" runat="server">
                <asp:View ID="vwViewAll" runat="server">
                    <div class="ektronPageContainer">
                        <asp:DataGrid ID="dgList"
                            runat="server"
                            CssClass="ektronGrid"
                            AutoGenerateColumns="false"
                            GridLines="None">
                            <HeaderStyle CssClass="title-header" />
                            <Columns>
                                <asp:TemplateColumn HeaderText="Title" HeaderStyle-CssClass="left" ItemStyle-CssClass="left">
                                    <ItemTemplate>
                                        <input type="hidden" name="radio_gateway" id="radio_gateway" value='<%#DataBinder.Eval(Container.DataItem, "id")%>' />
                                        <span class="<%#Util_GetType(Convert.ToInt64(DataBinder.Eval(Container.DataItem, "entryclass")))%>">
                                            <a title="<%#DataBinder.Eval(Container.DataItem, "title")%>" href="producttypes.aspx?action=viewproducttype&id=<%#DataBinder.Eval(Container.DataItem, "id")%>"><%#DataBinder.Eval(Container.DataItem, "title")%></a>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:HyperLinkColumn DataTextField="Id" HeaderText="Id" HeaderStyle-CssClass="center" ItemStyle-CssClass="center" DataNavigateUrlFormatString="producttypes.aspx?action=viewproducttype&id={0}"></asp:HyperLinkColumn>
                                <asp:TemplateColumn HeaderText="Class" HeaderStyle-CssClass="center" ItemStyle-CssClass="left">
                                    <ItemTemplate><%#Util_ShowType(Convert.ToInt64(DataBinder.Eval(Container.DataItem, "entryclass")))%></ItemTemplate>
                                </asp:TemplateColumn >
                                <asp:BoundColumn HeaderStyle-CssClass="center" ItemStyle-CssClass="left" DataField="DisplayLastEditDate" HeaderText="Date Modified"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                        <div class="paging" id="divPaging" runat="server" visible="false">
                            <ul class="direct">
                                <li><asp:ImageButton ToolTip="First Page" ID="ibFirstPage" runat="server" OnCommand="NavigationLink_Click" CommandName="First" /></li>
                                <li><asp:ImageButton ToolTip="Previous Page" ID="ibPreviousPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Prev" /></li>
                                <li><asp:ImageButton ToolTip="Next Page" ID="ibNextPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Next" /></li>
                                <li>
                                    <asp:ImageButton ToolTip="Last Page" ID="ibLastPage" runat="server" OnCommand="NavigationLink_Click" CommandName="Last" />
                                    <asp:HiddenField ID="hdnTotalPages" runat="server" />
                                </li>
                            </ul>
                            <script type="text/javascript">
                                <!--//--><![CDATA[//><!--
                                    function GoToPage(elem){
                                        var valid = true;
                                        var adHocButton = $ektron(elem);
                                        var selectedPage = adHocButton.prevAll("span.pageNumber").find("input.currentPage").attr("value");
                                        var isValueNumeric = selectedPage.match(/^\d+$/) == null ? false : true;
                                        if (isValueNumeric){
                                            var currentPage = parseInt(adHocButton.prevAll("input.currentPage").attr("value"), 10);
                                            var totalPages = parseInt(adHocButton.prevAll("span.pageTotal").text(), 10);
                                            if (selectedPage == 0 || selectedPage > totalPages){
                                                valid = false;
                                                alert("Page number must be between 1 and " + totalPages + "!");
                                                adHocButton.prevAll("span.pageNumber").find("input.currentPage").attr("value", currentPage);
                                            }
                                        } else {
                                            valid = false;
                                            alert("Page number must be numeric!");
                                        }
                                        return valid;
                                    }
                                //--><!]]>
                            </script>
                            <p class="adHoc">
                                <span class="page"><asp:Literal ID="litPage" runat="server" /></span>
                                <span class="pageNumber"><asp:TextBox CssClass="currentPage" ID="txtCurrentPage" runat="server"></asp:TextBox></span>
                                <span class="pageOf"><asp:Literal ID="litOf" runat="server" /></span>
                                <input type="hidden" runat="server" name="hdnCurrentPage" class="currentPage" id="hdnCurrentPage" />
                                <span class="pageTotal"><asp:Literal ID="litTotalPages" runat="server" /></span>
                                <asp:ImageButton ToolTip="Go" ID="ibPageGo" CssClass="adHocPage" runat="server" OnCommand="AdHocPaging_Click" CommandName="AdHocPage" />
                            </p>
                        </div>
                    </div>
                </asp:View>
                <asp:View ID="vwViewAddEdit" runat="server">
                    <div class="ektronPageContainer ektronPageTabbed">
                        <div class="tabContainerWrapper">
                            <div class="tabContainer">
                                <ul>
                                    <asp:PlaceHolder ID="phTabProperties" runat="server" Visible="true">
                                        <li>
                                            <a title="Properties" href="#dvProperties">
                                                <%=_MessageHelper.GetMessage("properties text")%>
                                            </a>
                                        </li>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phTabAttributes" runat="server" Visible="false">
                                        <li>
                                            <a title="Attributes" href="#dvAttributes">
                                                <%=_MessageHelper.GetMessage("lbl entry attrib tab")%>
                                            </a>
                                        </li>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phTabMediaDefaults" runat="server" Visible="false">
                                        <li>
                                            <a title="Media Defaults" href="#dvMediaDefaults">
                                                <%=_MessageHelper.GetMessage("lbl media def")%>
                                            </a>
                                        </li>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phTabDisplayInfo" runat="server" Visible="false">
                                        <li>
                                            <a title="Display Info" href="#dvDisplayInfo">
                                                <%=_MessageHelper.GetMessage("display info label")%>
                                            </a>
                                        </li>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="phTabPreview" runat="server" Visible="false">
                                        <li>
                                            <a title="Preview" href="#dvPreview">
                                                <%=_MessageHelper.GetMessage("generic preview title")%>
                                            </a>
                                        </li>
                                    </asp:PlaceHolder>
                                </ul>
                                <asp:PlaceHolder ID="phAddEdit" runat="server" Visible="false">
                                    <div id="dvProperties">
                                        <table class="ektronGrid">
                                            <tbody>
                                                <tr id="trError" runat="server" visible="false">
                                                    <td colspan="2">
                                                        <asp:Literal ID="litErrorMessage" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="label"><asp:Literal ID="litTitleLabel" runat="server" /></td>
                                                    <td><asp:textbox ID="txtTitle" runat="server" Columns="50" MaxLength="75" /></td>
                                                </tr>
                                                <tr id="tr_id" runat="server" visible="false">
                                                    <td class="label"><asp:Literal ID="litIdLabel" runat="server" /></td>
                                                    <td><asp:Literal ID="txt_id" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td class="label"><asp:Literal ID="litDescriptionLabel" runat="server" /></td>
                                                    <td><asp:textbox ID="txtDescription" runat="server" Columns="50" MaxLength="255" /></td>
                                                </tr>
                                                <tr>
                                                    <td class="label"><asp:Literal ID="litTypeLabel" runat="server" /></td>
                                                    <td>
                                                        <asp:DropDownList ID="drp_type" runat="Server" Visible="false" />
                                                        <asp:Literal ID="litType" runat="server" />
                                                        <div id="tr_provider" runat="server">
                                                            <asp:Literal ID="ltr_subprovider" runat="server" />
                                                            <asp:DropDownList ID="drp_SubscriptionProvider" runat="Server" />
                                                        </div>
                                                    </td>
                                                </tr>
			                                    <tr id="trXslt" runat="server">
			                                        <td class="label" style="white-space:normal;">
			                                            <asp:Literal ID="litDisplayLabel" runat="server" />
			                                        </td>
				                                    <td colspan="2">
				                                        <div class="innerTable">
					                                        <table>
					                                            <tbody>
						                                            <tr>
							                                            <th>
							                                                <asp:RadioButton ToolTip="XSLT 0" ID="frm_xsltdefault0" GroupName="frm_xsltdefault" runat="server" />
							                                                <asp:Literal ID="litXsltDefaultLabel" runat="server" />
							                                            </th>
							                                            <td><asp:Literal ID="ltr_deflabel" runat="server" /></td>
						                                            </tr>
						                                            <tr>
							                                            <th>
							                                                <asp:RadioButton ToolTip="XSLT 1" ID="frm_xsltdefault1" GroupName="frm_xsltdefault" runat="server" />
							                                                <asp:Literal ID="litXslt1Label" runat="server" />
							                                            </th>
							                                            <td style="white-space:nowrap">
							                                                <asp:TextBox ID="txt_xslt1" runat="server" Columns="35" MaxLength="255" />
								                                            <asp:Literal ID="ltr_verify1" runat="server" />
							                                            </td>
						                                            </tr>
						                                            <tr>
							                                            <th>
							                                                <asp:RadioButton ToolTip="XSLT 2" ID="frm_xsltdefault2" GroupName="frm_xsltdefault" runat="server"/>
							                                                <asp:Literal ID="litXslt2Label" runat="server" />
							                                            </th>
							                                            <td style="white-space:nowrap">
							                                                <asp:TextBox ID="txt_xslt2" runat="server" Columns="35" MaxLength="255" />
								                                            <asp:Literal ID="ltr_verify2" runat="server" />
							                                            </td>
						                                            </tr>
						                                            <tr>
							                                            <th>
							                                                <asp:RadioButton ToolTip="XSLT 3" ID="frm_xsltdefault3" GroupName="frm_xsltdefault" runat="server"/>
							                                                <asp:Literal ID="litXslt3Label" runat="server" />
							                                            </th>
							                                            <td style="white-space:nowrap">
							                                                <asp:TextBox ID="txt_xslt3" runat="server" Columns="35" MaxLength="255" />
								                                            <asp:Literal ID="ltr_verify3" runat="server" />
							                                            </td>
						                                            </tr>
						                                        </tbody>
					                                        </table>
					                                        <p style="margin:0;padding:0;"><asp:Literal ID="litDisplayXsltPathMessage" runat="server" /></p>
					                                    </div>
				                                    </td>
			                                    </tr>
			                                </tbody>
                                        </table>
                                    </div>
                                    <div id="dvAttributes">
                                        <ucEktron:Attributes ID="ucAttributesEdit" runat="server" />
                                    </div>
                                    <div id="dvMediaDefaults">
                                        <ucEktron:MediaDefaults ID="ucMediaDefaultsEdit" runat="server" />
                                    </div>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phView" Visible="false" runat="server">
			                        <div id="dvProperties">
			                            <asp:DataGrid ID="PropertiesGrid"
			                                runat="server"
			                                AutoGenerateColumns="False"
			                                OnItemDataBound="DisplayGrid_ItemDataBound"
			                                EnableViewState="False"
			                                CssClass="ektronGrid"
			                                ShowHeader="false" />
			                        </div>
                                    <div id="dvAttributes">
                                        <ucEktron:Attributes ID="ucAttributes" runat="server" />
                                    </div>
                                    <div id="dvMediaDefaults">
                                        <ucEktron:MediaDefaults ID="ucMediaDefaults" runat="server" />
                                    </div>
			                        <div id="dvDisplayInfo">
			                            <asp:DataGrid ID="DisplayGrid"
			                                runat="server"
			                                AutoGenerateColumns="False"
			                                OnItemDataBound="DisplayGrid_ItemDataBound"
			                                EnableViewState="False"
			                                GridLines="None"
			                                HeaderStyle-CssClass="title-header"
			                                CssClass="ektronGrid"
			                                ShowHeader="true" />

			                            <div class="ektronPageInfo xPaths">
			                                <h3 id="h3Xpaths" runat="server" class="label"></h3>
			                                <ul class="xpaths">
			                                    <asp:Literal ID="litXpaths" runat="server" />
			                                </ul>
			                            </div>
			                        </div>
			                        <asp:PlaceHolder ID="phPreview" runat="server" Visible="True">
			                            <div id="dvPreview">
			                                <asp:DataGrid ID="PreviewGrid"
			                                    runat="server"
			                                    AutoGenerateColumns="False"
			                                    EnableViewState="False"
			                                    GridLines="None"
			                                    CssClass="ektronGrid"
			                                    HeaderStyle-CssClass="title-header" />
			                            </div>
			                        </asp:PlaceHolder>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phAddEditAttributes" runat="server" Visible="false">
                                    <table class="ektronGrid">
			                            <tr>
				                            <td class="label">
					                            <asp:Literal ID="ltr_attrname" runat="server" />
				                            </td>
				                            <td>
					                            <asp:TextBox ID="txt_attrname" runat="server" MaxLength="255" Columns="50" />
				                            </td>
			                            </tr>
		                                <tr>
			                                <td class="label">
				                                <asp:Literal ID="ltr_attrtype" runat="server" />
			                                </td>
			                                <td>
				                                <asp:DropDownList ID="drp_attrtype" runat="server">
				                                </asp:DropDownList>
			                                </td>
		                                </tr>
		                                <tr id="tr_text" runat="server">
		                                    <td class="label">
		                                        <asp:Literal ID="ltr_def" runat="server" />
		                                    </td>
			                                <td>
			                                    <div id="divText" runat="server">
			                                        <asp:TextBox ID="txt_textdefault" runat="server" TextMode="MultiLine" Columns="75" Rows="3" style="width:100%" />
			                                    </div>
			                                    <div id="divNum" runat="server">
			                                        <asp:TextBox ID="txt_number" runat="server" MaxLength="7" Text="0" />
			                                    </div>
			                                    <div id="divChk" runat="server">
			                                        <asp:CheckBox ID="chk_bool" runat="server" />
			                                    </div>
			                                </td>
		                                </tr>
			                        </table>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phAddThumbnail" runat="server" Visible="false">
                                    <table id="tblMedia" border="0" style="width: auto;">
                                        <tr>
                                            <td class="label">
                                                <asp:Literal ID="ltr_addthumbnail" runat="server" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Literal ID="ltr_name" runat="server" />&nbsp;
                                                <input type="text" id="txtName" name="txtName" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Literal ID="ltr_width" runat="server" />&nbsp;
                                                <input type="text" name="txtWidth" id="txtWidth" maxlength="9" runat="server"/>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:Literal ID="ltr_height" runat="server" />
                                                <input type="text" id="txtHeight" name="txtHeight" maxlength="9" runat="server"/>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:PlaceHolder>
                            </div>
                        </div>
                    </div>
                    <iframe height="1" frameborder="0" width="1" id="iframe1" src="../blank.htm"/>
                </asp:View>
            </asp:MultiView>
        </form>
    </body>
</html>
