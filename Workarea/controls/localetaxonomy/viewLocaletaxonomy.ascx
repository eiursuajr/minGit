<%@ Control Language="C#" AutoEventWireup="true" CodeFile="viewLocaletaxonomy.ascx.cs"
 Inherits="viewLocaletaxonomy" %>
 
 <script type="text/javascript">
    <!--//--><![CDATA[//><!--
    function LoadLanguage(inVal) {
		if(inVal=='0') { return false ; }
		//top.notifyLanguageSwitch(inVal, <%=TaxonomyId %>);
        document.location = 'LocaleTaxonomy.aspx?action=view&view=<%=_ViewItem%>&taxonomyid='+<%=TaxonomyId%>+'&LangType=' + inVal ;
	}

	function TranslateTaxonomy(TaxonomyId, ParentId, LanguageId) {
		document.location = 'LocaleTaxonomy.aspx?action=add&taxonomyid=' + TaxonomyId + '&LangType='+LanguageId+'&parentid=' + ParentId;
	}

	function DeleteItem(op){
	//debugger;
        if(op=='items'){
            if(!IsSelected('selected_items')){
                alert('<%=m_strSelDelWarning%>');
                return false;
            }
            if(confirm("<%=m_strDelItemsConfirm %>")){
                document.getElementById("submittedaction").value="deleteitem";
                document.forms[0].submit();
            }
        }
        else{
            if(confirm("<%=m_strDelConfirm%>")){
                document.getElementById("submittedaction").value="delete";
                document.forms[0].submit();
            }
        }
        return false;
    }
    
    function LoadViewType(type){
	    document.location='LocaleTaxonomy.aspx?action=view&view='+type+'&folderid=0&taxonomyid=<%=TaxonomyId %>&parentid=<%=TaxonomyParentId %>' ;
    }
	function resetPostback(){
        document.forms[0].taxonomy_isPostData.value = "";
	}
	
//	/*
//	    Custom Property Functionality	
//	*/
//	
//	var customPropertyTotalPages = 1;
//	var customPropertyCurrentPage = 1;
//	var customPropertyRecordsPerPage = 50;
//	var customPropertyObjectId = 0;
//	
//	function LoadCustomProperties(page)
//	{	    
//	    var handlerUrl = "controls/content/CustomPropertyHandler.ashx?action=getcustompropobjectdata";
//        handlerUrl += "&objectid=" + customPropertyObjectId;
//        handlerUrl += "&page=" + page;
//        handlerUrl += "&count=" + customPropertyRecordsPerPage;
//        
//        $ektron.getJSON(
//            handlerUrl,
//            function(data) {
//                // Update current page and total pages values
//                customPropertyTotalPages = data.TotalPages;
//                customPropertyCurrentPage = data.Page;
//               
//                // Toggle the paging controls as necessary.
//                UpdatePaging();
//                
//                // Clear the property table prior to adding new properties
//                ClearPropertyTable();
//                
//                // Load new properties into the table.
//                $ektron.each(data.PropertyInfo, function(index, item) { AddPropertyToTable(item); });
//                
//                // Stripe the custom property rows.
//                StripeCustomPropertyRows();
//            });
//	}
//	
//	function UpdatePaging()
//	{
//        if(customPropertyTotalPages > 1){
//            $("#customPropertyPaging").show();            
//        }
//        else {
//            $("#customPropertyPaging").hide();
//        }
//        
//        $("#customPropertyPage").text(customPropertyCurrentPage);
//        $("#customPropertyTotalPages").text(customPropertyTotalPages);
//	}
//	
//	function BindPagingEvents(){
//	    $ektron("#customPropertyPaging a.pageLinks").click(HandlePagingClick);
//	}
//	
//	function HandlePagingClick(){
//	    var anchor = $ektron(this);
//	    switch(anchor.attr("rel").toLowerCase()){
//	        case "start":
//	            if(customPropertyCurrentPage != 1){
//	                
//	                LoadCustomProperties(1);
//	            }
//	            break;
//	            
//	        case "prev":
//	            if(customPropertyCurrentPage > 1){
//	                LoadCustomProperties(customPropertyCurrentPage - 1);
//	            }
//	            break;
//	            
//	        case "next":
//	            if(customPropertyCurrentPage < customPropertyTotalPages){
//	                LoadCustomProperties(customPropertyCurrentPage + 1);
//	            }
//	            break;
//	        default:
//	            if(customPropertyCurrentPage != customPropertyTotalPages){
//	                LoadCustomProperties(customPropertyTotalPages);
//	            }
//	            break;
//	    }
//	}
//	
//	function AddPropertyToTable(item){
//	    var row = $ektron("<tr>").addClass("customPropertyRow");
//	    var titleCell = $ektron("<td>");
//	    var dataTypeCell = $ektron("<td>");
//	    var valueCell = $ektron("<td>");
//	    
//	    titleCell.text(item.Title);
//	    dataTypeCell.text(item.DataType);
//	    
//	    
//	    var valueFound = false;
//        for(var i = 0; i < item.Items.length && !valueFound; i++){
//            if(item.Items[i].IsSelected){
//                valueFound = true;
//                if(item.DataType == "Boolean"){
//                    if(item.Items[i].Value.toLowerCase() == "true")
//                    {
//                        valueCell.text("Yes");
//                    }
//                    else {
//                        valueCell.text("No");
//                    }
//                }
//                else {
//                    valueCell.text(item.Items[i].FormattedValue);
//                }
//            }
//	    }	    
//	    
//	    row.append(titleCell);
//	    row.append(dataTypeCell);
//	    row.append(valueCell);
//	    
//	    $ektron("#customProperties").append(row);
//	}
//	
//	function ClearPropertyTable()
//	{
//	    $ektron("#customProperties tr").each(function(index, item){ 
//	        var row = $ektron(item);
//	        if (!row.hasClass("title-header")){
//	            row.remove();
//	        }
//	    });
//	}

    Ektron.ready(function() {                
        $ektron('#TaxonomySelect').modal({
            toTop: true,
            trigger: '.viewTaxonomyList',
            modal: true,
            onShow: function(hash)
            {
                hash.w.css("margin-top", -1 * Math.round(hash.w.outerHeight()/2)).css("top", "50%");
                hash.o.fadeTo("fast", 0.5, function()
                    {
	                    hash.w.fadeIn("fast");
                    }
                );
            },
            onHide: function(hash)
            {
                hash.w.fadeOut("fast");
                hash.o.fadeOut("fast", function()
                    {
                        if (hash.o)
                        {
	                        hash.o.remove();
                        }
                    }
                );
            }
        });
        
        if (typeof Ektron.DMSMenu != "undefined")
        {
            Ektron.DMSMenu.menuAppPath = "<asp:literal id="MenuAppPath" runat="server"/>";
        }
//        // Load the first page of custom property data.
//        customPropertyObjectId = parseInt($ektron("#taxonomy_customPropertyObjectId").attr("value"));
//        if(customPropertyObjectId != 0){
//            customPropertyRecordsPerPage = parseInt($ektron("#taxonomy_customPropertyRecordsPerPage").attr("value"));
//            LoadCustomProperties(1);
//            BindPagingEvents();
//        }              
    });
    //--><!]]>
</script>
<style type="text/css">
<!--/*--><![CDATA[/*><!--*/
    .viewTaxonomyList{ display: inline-block !important;}
    div.modalIframeWrapper {border: solid 1px #ccc;}
    div.modalIframeWrapper iframe.modalIframe {height: 30em; width: 100%;}
    /*]]>*/-->
</style>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronWindow ektronModalWidth-40 ui-dialog ui-widget ui-widget-content ui-corner-all" id="TaxonomySelect">
    <div class="ui-dialog-titlebar ui-widget-header ui-corner-all ui-helper-clearfix propertiesModalHeader">
        <span class="ui-dialog-title header"><%=_MessageHelper.GetMessage("lbl select locale taxonomy")%></span>
        <a href="#" class="ui-dialog-titlebar-close ui-corner-all ektronModalClose"><span class="ui-icon ui-icon-closethick">&nbsp;</span></a>
    </div>
    <div class="ui-dialog-content ui-widget-content">
        <div class="modalIframeWrapper">
            <iframe frameborder="0" scrolling="auto" src="<%=_Common.AppPath%>urlAutoAliasSourceSelector.aspx?FolderID=0&browser=1&WantXmlInfo=1&noblogfolders=1&mode=Taxonomy&action=movecopy&CopyType=Locale" class="modalIframe">
            </iframe>
        </div>
    </div>
    <input type="hidden" runat="Server" name="hdnSourceId" id="hdnSourceId" />
    <input type="hidden" runat="server" id="hdn_parentTaxonomyPath" name="hdn_parentTaxonomyPath" />
</div>
<div id="searchpanel" class="ektronPageContainer ektronPageInfo">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <ul>
                <li>
                    <a title="Iems" href="#dvItems">
                        <%=_MessageHelper.GetMessage("generic items")%>
                    </a>
                </li>
                <li>
                    <a title="Properties" href="#dvProperties">
                        <%=_MessageHelper.GetMessage("properties text")%>
                    </a>
                </li>
                <%--<li>
                    <a title="Custom Properties" href="#dvMetadata" id="customPropertiesTabButton">
                        <%=_MessageHelper.GetMessage("custom properties")%>
                    </a>
                </li>--%>
            </ul>
             <div id="breadCrumbControl" style="margin-left:5px;">
               <%=m_strCurrentBreadcrumb%>	 </div>
           
            <div id="dvItems">
                <div class="ektronPageGrid">
                <div class="ui-helper-clearfix" id="removeItemsWrapper" runat="server" visible="false" style="margin-bottom: .5em;">
                    <asp:HyperLink CssClass="button buttonRight redHover buttonRemove" id="removeItemsLink" runat="server"   onclick="return DeleteItem('items');"/>
                </div>
                    <asp:GridView ID="TaxonomyItemList"
                        runat="server"
                        AutoGenerateColumns="False"
                        Width="100%"
                        EnableViewState="False"
                        GridLines="None"
                        CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                    </asp:GridView>
                    <p class="pageLinks">
                        <asp:Label runat="server" ID="PageLabel">Page</asp:Label>
                        <asp:Label ID="CurrentPage" CssClass="pageLinks" runat="server" />
                        <asp:Label runat="server" ID="OfLabel">of</asp:Label>
                        <asp:Label ID="TotalPages" CssClass="pageLinks" runat="server" />
                    </p>
                    <asp:LinkButton runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]"
                        OnCommand="NavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
                    <asp:LinkButton runat="server" CssClass="pageLinks" ID="PreviousPage" Text="[Previous Page]"
                        OnCommand="NavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
                    <asp:LinkButton runat="server" CssClass="pageLinks" ID="NextPage" Text="[Next Page]"
                        OnCommand="NavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
                    <asp:LinkButton runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]"
                        OnCommand="NavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
                </div>
            </div>
            <div id="dvProperties">
                <table class="ektronGrid">
                    <%--<tr>
                        <td class="label" title="Breadcrumb"><%=_MessageHelper.GetMessage("lbl sitemap path")%>:</td>
                        <td class="readOnlyValue"><%=m_strCurrentBreadcrumb%></td>
                    </tr>--%>
                    <tr>
                        <td class="label" title="Taxnomy Id"><%=_MessageHelper.GetMessage("lbl locale taxonomy id")%>:</td>
                        <td class="readOnlyValue"><asp:Label ID="lbltaxonomyid" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label" title="Taxonomy Title"><%=_MessageHelper.GetMessage("localetaxonomytitle")%>:</td>
                        <td class="readOnlyValue"><asp:Label ID="taxonomytitle" runat="server" /></td>
                    </tr>
                    <tr>
                        <td class="label" title="Taxonomy Description"><%=_MessageHelper.GetMessage("localetaxonomydescription")%>:</td>
                        <td class="readOnlyValue"><asp:Label ID="taxonomydescription" runat="server" /></td>
                    </tr>
<%--                    <tr>
                        <td class="label" title="Taxonomy Image"><%=_MessageHelper.GetMessage("taxonomyimage")%>:</td>
                        <td class="readOnlyValue">
                            <span id="sitepath"><asp:Literal ID="ltr_sitepath" runat="Server" /></span>
                            <asp:literal id="taxonomy_image" runat="server" />
                            <div class="ektronTopSpaceSmall"></div>
                            <asp:Image ID="taxonomy_image_thumb" runat="server" />
                        </td>
                    </tr>
                    <tr id="tr_tmpl" runat="server">
                        <td class="label" title="Template"><%=_MessageHelper.GetMessage("template label")%>:</td>
                        <td class="readOnlyValue"><asp:Label ID="lblTemplate" runat="server" /></td>
                    </tr>
                    <tr id="tr_tmplinhrt" runat="server">
                        <td class="label" title="Inherit Template"><%=_MessageHelper.GetMessage("lbl inherit template")%>:</td>
                        <td class="readOnlyValue"><asp:Label ID="lblTemplateInherit" runat="server" /></td>
                    </tr>
                    <tr id="tr_config" runat="server">
                        <td class="label" title="Category Link"><%=_MessageHelper.GetMessage("config page html title")%>:</td>
                        <td class="readOnlyValue"><asp:Label ID="configlist" runat="server" /></td>
                    </tr>
                    <tr>
                        <td id="tr_catLink" class="label" title="Category Link"><%=_MessageHelper.GetMessage("lbl category link")%>:</td>
                        <td class="readOnlyValue"><asp:Label ID="catLink" runat="server" /></td>
                    </tr>
                    <tr>
                       <td id="tr_enDis" class="label" title="Display to Users"><%=_MessageHelper.GetMessage("lbl enable/disable")%></td>
                       <td class="readOnlyValue"><asp:Literal ID="ltrStatus" runat="server" /></td>
                    </tr>--%>
                    <tr id="tr_catcount" runat="server">
                       <td class="label" title="Subcategories in Category"><%=_MessageHelper.GetMessage("lbl locale taxonomy subcat count")%></td>
                       <td class="readOnlyValue"><asp:Literal ID="ltrCatCount" runat="server" /></td>
                    </tr>
                </table>
            </div>
            
           <%-- <div id="dvMetadata">                    
                <table id="customProperties" class="ektronGrid">
                    <tbody>
                        <tr class="title-header">
                            <td style="width: 40%;" title="Title">Title<asp:Label ID="lblTitleHeader" runat="server"></asp:Label></td>
                            <td style="width: 20%;" title="Data Type">Data Type<asp:Label ID="lblDataTypeHeader" runat="server"></asp:Label></td>
                            <td style="width: 30%;" title="Value">Value<asp:Label ID="lblValueHeader" runat="server"></asp:Label></td>
                        </tr>
                    </tbody>
                </table>
                <div id="customPropertyPaging">
                    <p class="pageLinks" title="gfhdhdsfg">
                        Page <span id="customPropertyPage" class="pageLinks"></span> of <span class="pageLinks" id="customPropertyTotalPages"></span>
                    </p>
                    <a href="#" class="pageLinks" rel="Start" title="First Page">[First Page]</a>
                    <a href="#" class="pageLinks" rel="Prev" title="Previous Page">[Previous Page]</a>
                    <a href="#" class="pageLinks" rel="Next" title="Next Page">[Next Page]</a>
                    <a href="#" class="pageLinks" rel="End" title="Last Page">[Last Page]</a>
                </div>
            </div>--%>
        </div>
    </div>
</div>

<input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />

<input type="hidden" runat="server" id="customPropertyObjectId" value="0" name="taxonomyId" />
<input type="hidden" runat="server" id="customPropertyRecordsPerPage" value="0" name="taxonomyId" />

