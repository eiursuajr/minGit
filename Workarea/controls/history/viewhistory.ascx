<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewHistory" CodeFile="ViewHistory.ascx.cs" %>

<script type="text/javascript">
    <!--//--><![CDATA[//><!--
    $ektron.addLoadEvent(function(){
        $ektron("a[href='#dvContent']").click();
        //var div = $ektron("#dvContent");
        //div.css("height", $ektron(document).height() - div.offset().top - 30 + "px");
    });
    //--><!]]>
</script>
<asp:literal id="CloseOnRestore" runat="server" />
<asp:Literal ID="EnhancedMetadataScript" runat="server"/>
<asp:Literal ID="EnhancedMetadataArea" runat="server" />
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <ul>
                <asp:PlaceHolder ID="phPropertiesTab" runat="server">
                    <li>
                        <a title="Properties" href="#dvProperties">
                            <%=m_refMsg.GetMessage("properties text")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phPricingTab" runat="server">
                    <li>
                        <a title="Pricing" href="#dvPricing">
                            <%=m_refMsg.GetMessage("lbl pricing")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phAttributesTab" runat="server">
                    <li>
                        <a title="Attributes" href="#dvAttributes">
                            <%=m_refMsg.GetMessage("lbl attributes")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phItemsTab" runat="server">
                    <li>
                        <a title="Items" href="#dvItems">
                            <%=m_refMsg.GetMessage("generic items")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phContentTab" runat="server">
                    <li>
                        <a title="Content" href="#dvContent">
                            <%=m_refMsg.GetMessage("content text")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phMetadataTab" runat="server">
                    <li>
                        <a title="Metadata" href="#dvMetadata">
                            <%=m_refMsg.GetMessage("metadata text")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phSummaryTab" runat="server">
                    <li>
                        <a title="Summary" href="#dvSummary">
                            <%=m_refMsg.GetMessage("Summary text")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phCommentTab" runat="server">
                    <li>
                        <a title="Comment" href="#dvComment">
                            <%=m_refMsg.GetMessage("comment text")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
            </ul>
            <asp:PlaceHolder ID="phProperties" runat="server">
                <div id="dvProperties">
		            <div class="ektronPageGrid">
	                    <asp:DataGrid id="PropertiesGrid"
	                        runat="server"
	                        CssClass="ektronForm"
	                        AutoGenerateColumns="False"
		                    OnItemDataBound="PropertiesGrid_ItemDataBound"
		                    EnableViewState="False"
		                    GridLines="None">
		                </asp:DataGrid>
		            </div>
	            </div>
	        </asp:PlaceHolder>
	        <asp:PlaceHolder ID="phPricing" runat="server">
                <div id="dvPricing">
                    <asp:Literal ID="ltr_pricing" runat="server"/>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phAttributes" runat="server">
                <div id="dvAttributes">
                    <asp:Literal ID="ltr_attrib" runat="server"/>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="phItems" runat="server">
                <div id="dvItems">
                    <asp:Literal ID="ltr_items" runat="server"/>
                    <asp:Literal ID="ltr_optiongroups" runat="server"/>
                </div>
            </asp:PlaceHolder>
	        <asp:PlaceHolder ID="phContent" runat="server">
	            <div id="dvContent">
		            <table border="0" cellpadding="0" cellspacing="0" height="100%" width="100%">
			            <tr>
				            <td class="title-header" id="tdcontlbl" runat="server"></td>
			            </tr>
			            <tr>
				            <td valign="top" height="100%" id="divContentHtml" runat="server"></td>
			            </tr>
		            </table>
	            </div>
	        </asp:PlaceHolder>
	        <asp:PlaceHolder ID="phMetadata" runat="server">
	            <div id="dvMetadata">
		            <asp:Literal ID="MetaDataValue" Runat="server" />
	            </div>
	        </asp:PlaceHolder>
	        <asp:PlaceHolder ID="phSummary" runat="server">
	            <div id="dvSummary">
		            <table border="0" cellpadding="0" cellspacing="0">
			            <tr>
				            <td class="title-header" id="tdsummaryhead" runat="server"></td>
			            </tr>
			            <tr>
				            <td valign="top" id="tdsummarytext" runat="server"></td>
			            </tr>
		            </table>
	            </div>
	        </asp:PlaceHolder>
	        <asp:PlaceHolder ID="phComment" runat="server">
	            <div id="dvComment">
		            <table border="0" cellpadding="0" cellspacing="0">
			            <tr>
				            <td nowrap class="title-header" id="tdcommenthead" runat="server"></td>
			            </tr>
			            <tr>
				            <td valign="top" id="tdcommenttext" runat="server"></td>
			            </tr>
		            </table>
	            </div>
	        </asp:PlaceHolder>
	    </div>
	</div>
</div>
<input type="hidden" id="restore_id" name="restore_id" runat="server"/>

