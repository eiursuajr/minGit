<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewxml_config" CodeFile="viewxml_config.ascx.cs" %>

<script type="text/javascript" language="javascript">
	function ConfirmDelete() {
		return (confirm('<asp:Literal id="ltr_delXMLConfig" runat="server" />'));
	}
	Ektron.ready(function(){
	    $ektron("table#ctl06_PreviewGrid > *").attr("style","white-space:normal");	    
	    if (typeof($ektron("label.addLeft").parent()[0]) !== 'undefined' && $ektron("label.addLeft").parent()[0] !== null)
	        $ektron("label.addLeft").parent()[0].className = "label left";
	});
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <div id="TR_ViewAll" runat="server">
	    <div class="ektronPageGrid">
	        <asp:DataGrid ID="XMLList"
	            runat="server"
	            AutoGenerateColumns="False"
		        EnableViewState="False"
		        CssClass="ektronGrid"
		        GridLines="None">
                <HeaderStyle CssClass="title-header" />
	        </asp:DataGrid>
	    </div>
    </div>
    <div id="TR_View" runat="server">
        <div class="ektronPageTabbed">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li>
                            <a title="Properties" href="#dvProperties">
                                <%=m_refMsg.GetMessage("properties text")%>
                            </a>
                        </li>
                        <li>
                            <a title="Display Information" href="#dvDisplayInfo">
                                <%=m_refMsg.GetMessage("display info label")%>
                            </a>
                        </li>
                        <li>
                            <a title="Preview" href="#dvPreview">
                                <%=m_refMsg.GetMessage("generic preview title")%>
                            </a>
                        </li>
                    </ul>

	                <div id="dvProperties">
	                    <asp:DataGrid ID="PropertiesGrid"
	                        runat="server"
	                        AutoGenerateColumns="False"
		                    OnItemDataBound="DisplayGrid_ItemDataBound"
		                    EnableViewState="False"
		                    GridLines="None"
		                    CssClass="ektronGrid"
		                    ShowHeader="false"
		                    >
                        </asp:DataGrid>
	                </div>
                    <div id="dvDisplayInfo">
                        <asp:DataGrid ID="DisplayGrid"
                            runat="server"
                            AutoGenerateColumns="False"
	                        OnItemDataBound="DisplayGrid_ItemDataBound"
	                        EnableViewState="False"
	                        GridLines="None"
	                        CssClass="ektronGrid"
	                        ShowHeader="false"
	                        >
                        </asp:DataGrid>
	                </div>
	                <div id="dvPreview">
	                    <asp:DataGrid ID="PreviewGrid"
	                        runat="server"
	                        AutoGenerateColumns="False"
		                    EnableViewState="False"
		                    GridLines="None"
		                    ShowHeader="false">
                        </asp:DataGrid>
	                </div>
	            </div>
	        </div>
	    </div>
    </div>
</div>
