<%@ Page Language="C#" AutoEventWireup="true" CodeFile="addeditboard.aspx.cs" Inherits="threadeddisc_addeditboard" ValidateRequest="false" %>
<%@ Register tagprefix="ektron" tagname="ContentDesigner" src="../controls/Editor/ContentDesignerWithValidator.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <meta content="text/html; charset=UTF-8" http-equiv="Content-Type" />
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <title>Add / Edit Board</title>
    <script type="text/javascript">
        <!--//--><![CDATA[//><!--
            Ektron.ready( function() {
                var tabsContainers = $ektron(".tabContainer");
                tabsContainers.tabs();
            });
        //--><!]]>
	            function ShowPane(tabID)
	            {
		            $ektron(".tabContainer").tabs('select', tabID);
	            }
         
    </script>
    <!--[if IE]>
        <style type="text/css">
            #content_html_wrapper {white-space:normal;}
            .tabContainer {width:800px;}
        </style>
    <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <asp:Panel ID="pnlTabContainer" CssClass="ektronPageContainer ektronPageTabbed" runat="server">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li>
                            <a href="#dvProp" title="Properties">
                                <%=m_refMsg.GetMessage("properties text")%>
                            </a>
                        </li>
                        <li>
                            <a href="#dvTaxonomy" title="Taxonomy">
                                <%=m_refMsg.GetMessage("generic taxonomy lbl")%>
                            </a>
                        </li>
                        <li>
                            <a href="#dvTemplate" title="Template">
                                <%=m_refMsg.GetMessage("template label")%>
                            </a>
                        </li>
                        <li id="liSubjects" visible="false" runat="server" title="Subjects">
                            <a href="#dvSubjects">
                                <%=m_refMsg.GetMessage("subjects text")%>
                            </a>
                        </li>
                        <li id="liTerms" visible="false" runat="server" title="Terms">
                            <a href="#dvTerms">
                                <%=m_refMsg.GetMessage("terms text")%>
                            </a>
                        </li>
                        <li id="liBreadCrumb" visible="false" runat="server" title="Sitemap">
                            <a href="#dvBreadCrumb">
                                <%=m_refMsg.GetMessage("lbl sitemap path")%>
                            </a>
                        </li>
                    </ul>
                    <div id="dvProp">
	                    <table class="ektronGrid" id="tblmain" runat="server">
                            <tbody>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_boardid" runat="server"/>:</td>
                                    <td><asp:Literal ID="ltr_boardid_data" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Name"><%=m_refMsg.GetMessage("lbl boardname")%>:</td>
                                    <td>
                                        <asp:TextBox ToolTip="Enter Name here" ID="txt_adb_boardname" runat="server" MaxLength="70"/>
                                        <asp:HiddenField ID="hdn_adb_boardname" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" title="Title"><%=m_refMsg.GetMessage("lbl boardtitle")%>:</td>
                                    <td><asp:TextBox ToolTip="Enter Title here" ID="txt_adb_title" runat="server" MaxLength="75"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Accepted HTML"><asp:Literal ID="ltr_acceptedhtml" runat="server"/>:</td>
                                    <td>
                                        <asp:TextBox ToolTip="Enter Accepted HTML here" ID="txt_acceptedhtml" runat="server" MaxLength="400"/>
                                        <span class="ektronCaption"><asp:Literal ID="ltr_comma_html" runat="server"/></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" title="Accepted Extensions"><asp:Literal ID="ltr_acceptedextensions" runat="server"/>:</td>
                                    <td>
                                        <asp:TextBox ToolTip="Enter Accepted Extensions here" ID="txt_acceptedextensions" runat="server" MaxLength="400"/>
                                        <span class="ektronCaption"><asp:Literal ID="ltr_comma_ext" runat="server"/></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" title="Max File Size"><asp:Literal ID="ltr_maxfilesize" runat="server"/>:</td>
                                    <td>
                                        <asp:TextBox ToolTip="Enter Max File Size here" ID="txt_maxfilesize" runat="server" MaxLength="400"/>
                                        <span class="ektronCaption"><asp:Literal ID="ltr_bytes" runat="server"/></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" title="CSS Theme"><%=m_refMsg.GetMessage("lbl css theme")%>:</td>
                                    <td><asp:DropDownList ToolTip="Select CSS Theme from the drop down menu" ID="drp_theme" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Style Sheet"><%=m_refMsg.GetMessage("lbl style sheet")%>:</td>
                                    <td>
                                        <asp:Literal ID="ltr_sitepath" runat="server"/><asp:TextBox ToolTip="Enter Style Sheet here" ID="txt_adb_stylesheet" runat="server"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Literal ID="ltr_adb_ra" runat="server" />:</td>
                                    <td class="value"><asp:CheckBox ToolTip="Require Authentication" ID="chk_adb_ra" runat="server" Checked="True" /></td>
                                </tr>
                                <tr id="tr_moderate" runat="server">
                                    <td class="label"><asp:Literal ID="ltr_adb_mc" runat="server" />:</td>
                                    <td class="value"><asp:CheckBox ToolTip="Label Moderate Contents" ID="chk_adb_mc" runat="server" Checked="True" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=m_refMsg.GetMessage("lbl lock")%>:</td>
                                    <td class="value"><asp:CheckBox ToolTip="Lock Board" ID="chk_lock_board" runat="server" Checked="True" /></td>
                                </tr>
                                <tr id="tr_repl" runat="server">
                                    <td class="label"><asp:Literal ID="ltr_dyn_repl" runat="server"/></td>
                                    <td><asp:CheckBox ToolTip="Replicate Folder Contents"  ID="chk_repl" runat="server" /></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div id="dvTaxonomy">
                        <asp:Literal ID="taxonomy_list" runat="server" />
                    </div>
                    <div id="dvTemplate">
                        <asp:Literal ID="template_list" runat="server" />
                        <asp:Literal ID="hidden_dyn_repl" runat="server" />
                        <asp:Literal ID="lit_ef_templatedata" runat="server" />
                    </div>
                    <div id="dvSubjects" visible="false" runat="server">
                        <table class="ektronGrid">
                            <asp:Literal id="ltr_adb_cat" Runat="server"/>
                        </table>
                    </div>
                    <div id="dvTerms" visible="false" runat="server">
                        <div>
                            <table class="ektronGrid">
                                <tbody>
                                    <tr>
                                        <td class="noStripe" style="white-space:nowrap;">
                                            <asp:Panel ID="pnlTerms" runat="server"/>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div id="dvBreadCrumb" visible="false" runat="server">
                        <asp:Panel ID="pnlInheritSitemapPath" runat="server" Visible="false">
                            <input title="Inherit Sitemap Path" type="checkbox" runat="server" name="chk_InheritSitemapPath" id="chk_InheritSitemapPath"
                                checked="checked" />
                            <asp:Literal ID="ltrInheritSitemapPath" runat="server" />
                            <div class="ektronTopSpace">
                            </div>
                            <table class="ektronGrid">
                                <tr>
                                    <td class="label">
                                        <%=m_refMsg.GetMessage("lbl path")%>
                                        :</td>
                                    <td class="readOnlyValue">
                                        <span id="sitepath_preview"></span>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="pnlEditBreakCrumb">
                            <div id="dvInheritSitemap">
                                <input title="Inherit Sitmap Path" type="checkbox" runat="server" onclick="InheritSitemapPath(this.checked);"
                                    id="chkInheritSitemapPath" name="chkInheritSitemapPath" checked="checked" />
                                <asp:Literal ID="ltInheritSitemapPath" runat="server" />
                                <div class="ektronTopSpace">
                                </div>
                            </div>
                            <table class="ektronGrid">
                                <tr>
                                    <td class="label" title="Path">
                                        <%=m_refMsg.GetMessage("lbl path")%>
                                        :</td>
                                    <td class="readOnlyValue">
                                        <span id="sitepath_preview"></span>
                                    </td>
                                </tr>
                            </table>
                            <asp:Panel runat="server" ID="pnlBreadCrumbEdit">
                                <div class="ektronTopSpace">
                                </div>
                                <img title="Move selected up" alt="Move selected up" onclick="moveSitemapPathNode('up')" src="../images/UI/Icons/up.png" />
                                <img title="Move selected down" alt="Move selected down" onclick="moveSitemapPathNode('down')" src="../images/UI/Icons/down.png" />
                                <div id="sitemap_nodes">
                                </div>
                                <div class="ektronTopSpace">
                                </div>
                                <div id="AddSitemapNode" runat="Server">
                                    <table class="ektronGrid">
                                        <tr>
                                            <td class="label" title="Title">
                                                <%=m_refMsg.GetMessage("generic title")%>
                                                :</td>
                                            <td class="value">
                                                <input title="Enter Title here" type="text" id="sitemaptitle_input" /></td>
                                        </tr>
                                        <tr>
                                            <td class="label" title="URL Link">
                                                <%=m_refMsg.GetMessage("generic url link")%>
                                                :</td>
                                            <td class="value">
                                                <input title="Enter URL Link here" type="text" id="sitemapurl_input" />
                                                <img title="Select quicklink" alt="Select quicklink" onclick="PopBrowseWin('quicklinks', '', 'document.forms[0].sitemapurl_input');return false;"
                                                    src="../images/UI/Icons/linkAdd.png" class="ektronClickableImage" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label" title="Description">
                                                <%=m_refMsg.GetMessage("lbl description")%>
                                                :</td>
                                            <td class="value">
                                                <input title="Enter Description here" type="text" id="sitemapdesc_input" /></td>
                                        </tr>
                                    </table>
                                    <div class="ektronTopSpaceSmall">
                                    </div>
                                    <input type="button" title="Click here to add a new sitemap path." onclick="addSiteMapNode(this)"
                                        id="btnAddSitepath" value="<%=m_refMsg.GetMessage("lbl sitealias add")%>"  />
                                    <input type="button" title="Reset title / url" onclick="clearSitemapForm()" value="<%=m_refMsg.GetMessage("lbl sitealias reset")%>" />
                                </div>
                                <input type="hidden" id="hdnInheritSitemap" name="hdnInheritSitemap" value="" />
                                <input type="hidden" id="saved_sitemap_path" name="saved_sitemap_path" value="" />
                            </asp:Panel>
                        </asp:Panel>
                    </div>
                </div>
                </div>
        </asp:Panel>
        <asp:Panel ID="pnlSubjectEdit" runat="server" Visible="false" CssClass="ektronPageInfo ektronPageContainer">
            <table class="ektronGrid" runat="server">
                <tbody>
                    <tr>
                        <td class="label" title="Subject"><asp:Literal ID="ltr_catname" runat="server"/></td>
                        <td><asp:TextBox ToolTip="Enter Subject here" ID="txt_catname" runat="server" MaxLength="50"/></td>
                    </tr>
                    <tr runat="server" id="tr_catid">
                        <td class="label" title="Id"><asp:Literal ID="ltr_catid" runat="server"/></td>
                        <td><asp:Label ID="lbl_catid" runat="server"/></td>
                    </tr>
                    <tr>
                        <td class="label" title="Sort Order"><asp:Literal ID="ltr_catsort" runat="server"/></td>
                        <td><asp:TextBox ToolTip="Enter Sort Order here" ID="txt_catsort" runat="server" MaxLength="10"/></td>
                    </tr>
                </tbody>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlSubjectDelete" runat="server" Visible="False" CssClass="ektronPageInfo ektronPageContainer">
            <table runat="server" class="ektronGrid">
                <tbody>
                    <tr>
                        <td class="label" title="Subject"><asp:Literal ID="ltr_delcatname" runat="server"/></td>
                        <td><asp:Literal ID="ltr_delcatnamedata" runat="server"/></td>
                    </tr>
                    <tr>
                        <td class="label" title="Move Forums to Subject"><asp:Literal ID="ltr_movecat" runat="server"/></td>
                        <td><asp:DropDownList ToolTip="Select Move Forums to Subject from drop down menu" ID="drp_movecat" runat="server"/></td>
                    </tr>
                </tbody>
            </table>
        </asp:Panel>
        <script type="text/javascript">
            <!--//--><![CDATA[//><!--
                Ektron.ready( function()
                {
                    // ADD TEMPLATE MODAL DIALOG
                    $ektron("#FrameContainer").modal(
                    {
                        trigger: '',
                        modal: true,
                        toTop: true,
                        onShow: function(hash)
                        {
                            hash.o.fadeIn();
                            hash.w.fadeIn();
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
                            });
                        }
                    });
                });
                <asp:Literal ID="tax_js" runat="server"/>
                <asp:Literal ID="ltr_af_js" runat="server"/>
                function IsBrowserIE()
                {
                    // document.all is an IE only property"
                    return (document.all ? true : false);
                }

    	        function CloseChildPage()
                {
	                $ektron('#FrameContainer').modalHide();
                }
    	        function LoadChildPage() {                    
                    var frameObj = document.getElementById("ChildPage");
                    frameObj.src = "../template_config.aspx?view=add&folder_edit=1";
                    $ektron('#FrameContainer').modalShow();
                }
                function ToggleTaxonomyInherit(control){
                    var element = document.forms[0].elements;
                    var len = element.length;
                    var catarr=taxonomytreearr;
                    if(control.checked){
                        for (var i=0; i<len; i++) {
                            if(element[i].name=="taxlist" && element[i].type=='radio')
                                element[i].enabled=false;
                        }
                        catarr=taxonomyparenttreearr;
                        for (var i=0; i<len; i++) {
                            var item = element[i];
                            if(item.name=="taxlist" && item.type=='radio'){
                                if(fetchtaxonomyid(item.value,catarr)) {item.enabled=true;}item.disabled=true;
                            }
                        }
                        CheckCatRequired(1);
                    }
                    else{
                        if( confirm('Are you sure you want to break inheritance?') ) {
                            for (var i=0; i<len; i++) {
                                var item = element[i];
                                if(item.name=="taxlist" && item.type=='radio'){
                                item.enabled=false;item.disabled=false;
                                }
                            }
                            for (var i=0; i<len; i++) {
                            if(element[i].name=="taxlist" && element[i].type=='radio')
                            element[i].enabled=false;
                            }
                            for (var i=0; i<len; i++) {
                                var item = element[i];
                                if(item.name=="taxlist" && item.type=='radio'){
                                    if(fetchtaxonomyid(item.value,catarr)) {item.enabled=true;}
                                }
                            }
                            CheckCatRequired(0)
                        }
                        else{
                            control.checked=true;CheckCatRequired(1);return false;
                        }
                    }
                }

                function fetchtaxonomyid(pid,arr){
                    for(var i=0;i<arr.length;i++){
                        if(arr[i]==pid){
                            return true;
                            break;
                        }
                    }
                    return false;
                }

	            function CheckCatRequired(v){
                    var cr=document.getElementById("CategoryRequired");
                    if(cr){
                        if(v==1){ cr.disabled=true;
                        if(__jsparentcatrequired==1) cr.checked=true;
                        else cr.checked=false;
                        }
                        else {
                        cr.disabled=false;
                        if(__jscatrequired==1) cr.checked=true;
                        else cr.checked=false;
                        }
                    }
                }
            //--><!]]>
        </script>
        <style type="text/css">
            <!--/*--><![CDATA[/*><!--*/
            #FrameContainer{ width: 80%; height: 60%; margin: -25% 0 0 -40% !important;position:absolute; display:none; }
            #ChildPage { frameborder:0; border:0; marginheight:2; marginwidth:2; scrolling:auto; border:none; width:100%; height:100%; scrolling:auto; background-color: white; }
            <asp:literal id="modalCss" runat="server" />
            /*]]>*/-->
        </style>
        <asp:HiddenField ID="hdn_adf_folderid" runat="server" />
        <asp:HiddenField ID="hdn_adb_action" runat="server" />
        <input id="inherit_taxonomy_from" type="hidden" name="inherit_taxonomy_from" runat="server" value="0" />
        <input id="current_category_required" type="hidden" name="current_category_required" runat="server" value="0" />
        <input id="parent_category_required" type="hidden" name="current_category_required" runat="server" value="0" />
    </form>
    <asp:literal runat="server" ID="ltr_reload_js" />
</body>
</html>

