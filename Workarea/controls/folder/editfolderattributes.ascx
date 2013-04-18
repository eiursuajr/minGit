<%@ Control Language="C#" AutoEventWireup="true" Inherits="editfolderattributes" CodeFile="editfolderattributes.ascx.cs" %>

<script type="text/javascript">
    <!--//--><![CDATA[//><!--
        var taxonomytreearr="<%= _SelectedTaxonomyList %>".split(",");
        var taxonomyparenttreearr="<%= _SelectedTaxonomyParentList %>".split(",");
        var __jscatrequired="<%= _CurrentCategoryChecked %>";
        var __jsparentcatrequired="<%= _ParentCategoryChecked %>";
        var isProductCatalog = <%=_IsCatalog.ToString().ToLower()%>;
	    function ReturnChildValue(id,value) {
		    CloseChildPage();
		    document.getElementById("contentidspan").innerHTML = "<div id=\"div3\" style=\"display: none;position: block;\"></div><div id=\"contentidspan\" style=\"display: block;position: block;\">(" + contentid + ")&nbsp;" + contenttitle + "&nbsp;&nbsp;</div>";
		    document.getElementById("a_change").style.visibility ="visible";
		    document.getElementById("a_none").style.visibility ="visible";
		    document.getElementById("content_id").value = contentid;
		    document.getElementById("state").selectedIndex = 0;
		    document.getElementById("state").disabled = true;
		    document.getElementById("current_language").value =  contentlanguage;

		    var objLanguage=document.getElementById("language");

		    if (("object"==typeof(objLanguage)) && (objLanguage!= null))
		    {
			    if (objLanguage.disabled==false) {objLanguage.disabled=true;}
		    }
	    }

	    function CloseChildPage()
	    {
	        var frameObj = document.getElementById("ChildPage");
	        frameObj.src = "";
		    var pageObj = document.getElementById("FrameContainer");
	        pageObj.className = "ChildPageHide";
	    }
		 function CheckPdfServiceProvider() {
             if ($ektron("#publishAsPdf").attr("checked")) {
              $ektron("#pdfGenerationMessage").show();
              }
               else {
              $ektron("#pdfGenerationMessage").hide();
              }             
            }      
	    function LoadChildPage() {
		        var languageID;
			    var frameObj = document.getElementById("ChildPage");

			    languageID = document.getElementById("language").value
			    frameObj.src = "blankredirect.aspx?template_config.aspx?view=add&folder_edit=1";

			    var pageObj = document.getElementById("FrameContainer");
			    pageObj.className = "ChildPageShow";
	    }
	    function PreviewProductTypeByID(xml_id) {
            if (xml_id != 0) {
                PopUpWindow('commerce/producttypes.aspx?LangType='+jsContentLanguage+'&action=viewproducttype&id=' + xml_id + '&caller=content', 'Preview', 700, 540, 1, 0);
            }
        }
	//--><!]]>
</script>
<style type="text/css">
    div.ChildPageHide
    {
        width: 1px;
        height: 1px;
        display: none;
        overflow: hidden;
    }
    div.ChildPageShow
    {
        margin-top: .25em;
        width: 99%;
        height: 100px;
        display: block;
        overflow: hidden;
        border: solid 1px #999;
    }
</style>
<style type="text/css">
    div.ChildPageHide
    {
        width: 1px;
        height: 1px;
        display: none;
        overflow: hidden;
    }
    div.ChildPageShow
    {
        margin-top: .25em;
        width: 99%;
        height: 100px;
        display: block;
        overflow: hidden;
        border: solid 1px #999;
    }
    .selectContent { background-image: url('Images/ui/icons/check.png');background-repeat: no-repeat;background-position:.5em center; }
    .useCurrent{ background-image: url('Images/ui/icons/shape_square.png'); background-repeat: no-repeat; background-position:.5em center; }
</style>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <ul>
                <li>
                    <a title="<%=_MessageHelper.GetMessage("properties text")%>" href="#dvProperties">
                        <%=_MessageHelper.GetMessage("properties text")%>
                    </a>
                </li>
                <li>
                    <a title="<%=_MessageHelper.GetMessage("lbl taxonomy")%>" href="#dvTaxonomy">
                        <%=_MessageHelper.GetMessage("generic taxonomy lbl")%>
                    </a>
                </li>
                <li>
                    <a title="<%=_MessageHelper.GetMessage("lbl templates")%>" href="#dvTemplates">
                        <%=_MessageHelper.GetMessage("lbl templates")%>
                    </a>
                </li>
                <li>
                    <a title="<%=_MessageHelper.GetMessage("lbl flagging")%>" href="#dvFlagging">
                        <%=_MessageHelper.GetMessage("lbl flagging")%>
                    </a>
                </li>
                <li>
                    <a title="<%=_MessageHelper.GetMessage("metadata text")%>" href="#dvMetadata">
                        <%=_MessageHelper.GetMessage("metadata text")%>
                    </a>
                </li>
                <asp:PlaceHolder ID="phSubjects" Visible="false" runat="server">
                    <li>
                        <a title="<%=_MessageHelper.GetMessage("subjects text")%>" href="#dvSubjects">
                            <%=_MessageHelper.GetMessage("subjects text")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phWebAlerts" Visible="false" runat="server">
                    <li>
                        <a title="<%=_MessageHelper.GetMessage("lbl web alert tab")%>" href="#dvWebAlerts">
                            <%=_MessageHelper.GetMessage("lbl web alert tab")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phContentType" Visible="true" runat="server">
                <li>
                    <a title="Types" href="#dvTypes"> <!-- Smart Forms or Product Types -->
                        <asp:Literal ID="ltrTypes" runat="server" Text="" />
                    </a>
                </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phBlogRoll" Visible="false" runat="server">
                    <li>
                        <a title="<%=_MessageHelper.GetMessage("lbl blog roll")%>" href="#dvBlogRoll">
                            <%=_MessageHelper.GetMessage("lbl blog roll")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <li>
                    <a title="<%=_MessageHelper.GetMessage("lbl sitemap path")%>" href="#dvBreadcrumb">
                        <%=_MessageHelper.GetMessage("lbl sitemap path")%>
                    </a>
                </li>
                <asp:PlaceHolder ID="phSiteAlias" Visible="false" runat="server">
                    <li>
                        <a title="<%=_MessageHelper.GetMessage("lbl site alias")%>" href="#dvSiteAlias">
                            <%=_MessageHelper.GetMessage("lbl site alias")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phFolderAliases" runat="server" Visible="false">
                    <li>
                        <a title="<%=_MessageHelper.GetMessage("lbl forcemanualaliasing")%>" href="#dvAlias">
                            <%=_MessageHelper.GetMessage("lbl forcemanualaliasing")%>
                        </a>
                    </li>
                </asp:PlaceHolder>
            </ul>

            <div id="dvProperties">
                <table class="ektronGrid">
                    <asp:PlaceHolder ID="phBlogProperties1" Visible="false" runat="server">
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl name")%>"><%=_MessageHelper.GetMessage("lbl name")%>:</td>
                            <td class="value" id="td_vf_nametxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl title")%>"><%=_MessageHelper.GetMessage("lbl title")%>:</td>
                            <td class="value" id="td_vf_titletxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl visibility")%>"><%=_MessageHelper.GetMessage("lbl visibility")%>:</td>
                            <td class="value" id="td_vf_visibilitytxt" runat="server"></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phFolderProperties1" Visible="false" runat="server">
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("foldername label")%>"><%=_MessageHelper.GetMessage("foldername label")%></td>
                            <td class="readOnlyValue"><asp:Literal ID="lit_ef_folder" runat="server" /></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phBlogProperties2" Visible="false" runat="server">
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl tag line")%>"><%=_MessageHelper.GetMessage("lbl tag line")%>:</td>
                            <td class="value"><input title="Tag Line" type="text" maxlength="255" name="tagline" id="tagline" runat="server" /></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl posts visible")%>"><%=_MessageHelper.GetMessage("lbl posts visible")%>:</td>
                            <td class="readOnlyValue" id="td_vf_postsvisibletxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("comments label")%>"><%=_MessageHelper.GetMessage("comments label")%>:</td>
                            <td class="readOnlyValue" id="td_vf_commentstxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl update services")%>"><%=_MessageHelper.GetMessage("lbl update services")%>:</td>
                            <td class="readOnlyValue" id="td_vf_updateservicestxt" runat="server"></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phDescription" Visible="true" runat="server">
                    <tr>
                        <td class="label" title="<%=_MessageHelper.GetMessage("lbl description")%>"><%=_MessageHelper.GetMessage("lbl description")%>:</td>
                        <td class="value"><input title="Description" type="text" maxlength="255" name="folderdescription" id="folderdescription" runat="server" /></td>
                    </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td title="<%=_MessageHelper.GetMessage("lbl style sheet")%>" class="label">
                            <%=_MessageHelper.GetMessage("lbl style sheet")%>
                            :<br />
                            <div title="<%=_MessageHelper.GetMessage("leave blank to inherit msg")%>" class="ektronCaption">
                                <%=_MessageHelper.GetMessage("leave blank to inherit msg")%>
                            </div>
                        </td>
                        <td class="value">
                            <asp:Literal ID="lit_ef_ss" runat="server" />
                        </td>
                    </tr>
                    <asp:Literal ID="DomainFolder" runat="server" />
                    <asp:PlaceHolder ID="phPDF" Visible="false" runat="server">
                                    <tr>
                                        <td class="label" title="<%=_MessageHelper.GetMessage("lbl office documents")%>"><%=_MessageHelper.GetMessage("lbl office documents")%>:</td>
                                        <td class="readOnlyValue">
                                            <input type="checkbox" title="<%=_MessageHelper.GetMessage("publish as pdf")%>" id="publishAsPdf" name="publishAsPdf" onclick="CheckPdfServiceProvider()" <% =IsPublishedAsPdf() %> />
                                            <label for="publishAsPdf" id="lblPublishAsPdf" runat="server"></label>*
                                            <div class="ektronCaption"><%=_MessageHelper.GetMessage("pdf generation warning")%></div>
                                            <div class="ektronCaption">* <%=_MessageHelper.GetMessage("publish help")%></div>
                                            <div class="ektronCaption">
                                                <div id="pdfGenerationMessage" style="display:none">
                                                    <strong style="color:#C00000"><asp:Literal ID="ltrCheckPdfServiceProvider" runat="server" Text=""></asp:Literal></strong>
                                                 </div>
                                             </div>
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                    <asp:Literal ID="ReplicationMethod" runat="server" />
                    <asp:PlaceHolder ID="phContSearch2" Visible="true" runat="server">
                        <div id="dvContSearch">
                            <asp:Literal ID="ltrContSearch2" runat="server"></asp:Literal>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phDisplaySettings2" Visible="false" runat="server">
                        <div id="dvDisplaySettings2">
                            <asp:Literal ID="ltrDisplaySettings2" runat="server"></asp:Literal>
                        </div>
                    </asp:PlaceHolder>
                </table>               
                <input type="hidden" name="oldfolderdescription" id="oldfolderdescription" runat="server" />
            </div>
            <div id="dvTaxonomy">
                <asp:Literal ID="taxonomy_list" runat="server" />
            </div>
            <div id="dvTemplates">
                <asp:Literal ID="template_list" runat="server" />
                <asp:Literal ID="lit_ef_templatedata" runat="server" />
            </div>
            <div id="dvFlagging">
                <asp:Literal ID="inheritFlag" runat="server" />
                <div class="ektronTopSpace"></div>
                <asp:DropDownList ID="ddflags" runat="server" />
            </div>
            <div id="dvMetadata">
                <asp:Literal ID="lit_vf_customfieldassingments" runat="server" />
                <input type="hidden" name="folder_cfld_assignments" id="folder_cfld_assignments" value="" />
            </div>
            <div id="dvSubjects">
                <asp:Literal ID="ltr_vf_categories_lbl" runat="server" />
                <asp:Literal ID="ltr_vf_categories" runat="server" />
            </div>
            <div id="dvWebAlerts" class="ui-tabs-hide">
                <asp:Literal ID="lit_vf_subscription_properties" runat="server" />
                <asp:Literal ID="lit_vf_subscription_assignments" runat="server" />
                <input type="hidden" name="folder_sub_assignments" value="" />
            </div>
            <div id="dvTypes">
                <asp:Literal ID="ltr_vf_types" runat="server" Text="Text" />
                <input type="hidden" id="language" value="1033" />
            </div>
            <div id="dvBreadcrumb">
                <div id="dvInheritSitemap">
                    <input type="checkbox" title="Inherit Parent Configuration" onclick="InheritSitemapPath(this.checked);" id="chkInheritSitemapPath"
                        name="chkInheritSitemapPath" checked="checked" />
                    <asp:Literal ID="ltInheritSitemapPath" runat="server" />
                    <div class="ektronTopSpace"></div>
                </div>

                <table class="ektronGrid">
                    <tr>
                        <td class="label"><%=_MessageHelper.GetMessage("lbl path")%>:</td>
                        <td class="readOnlyValue"><span id="sitepath_preview"></span></td>
                    </tr>
                </table>
                <div class="ektronTopSpace"></div>
                <div id="dvUpDownArrow">
                    <img alt="Move selected up" onclick="moveSitemapPathNode('up')" src="images/UI/Icons/up.png" />
                    <img alt="Move selected down" onclick="moveSitemapPathNode('down')" src="images/UI/Icons/down.png" />
                </div>
                <div id="sitemap_nodes"></div>
                <div class="ektronTopSpace"></div>
                <div id="AddSitemapNode">
                    <table class="ektronGrid">
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("generic title")%>:</td>
                            <td class="value"><input type="text" id="sitemaptitle_input" /></td>
                        </tr>
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("generic url link")%>:</td>
                            <td class="value">
                                <input type="text" id="sitemapurl_input" />
                                <img alt="<%=_MessageHelper.GetMessage("tooltip:select quicklink") %>" onclick="PopBrowseWin('quicklinks', '', 'document.forms[0].sitemapurl_input');return false;" src="images/UI/Icons/linkAdd.png" class="ektronClickableImage" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label"><%=_MessageHelper.GetMessage("lbl description")%>:</td>
                            <td class="value"><input title="<%=_MessageHelper.GetMessage("lbl description")%>" type="text" id="sitemapdesc_input" /></td>
                        </tr>
                    </table>
                    <div class="ektronTopSpaceSmall"></div>
                    <input type="button" title="Click here to add a new sitemap path." onclick="addSiteMapNode(this); if($ektron('div#sitemap_nodes div').length >= 2) { $ektron('#dvUpDownArrow')[0].style.display='block';};" id="btnAddSitepath" value="<%=_MessageHelper.GetMessage("lbl sitealias add")%>"/>
                    <input type="button" title="Reset title / url" onclick="clearSitemapForm()" value="<%=_MessageHelper.GetMessage("lbl sitealias reset")%>"  />
                </div>
                <input type="hidden" id="hdnInheritSitemap" name="hdnInheritSitemap" value="" />
                <input type="hidden" id="saved_sitemap_path" name="saved_sitemap_path" value="" />
            </div>
            <asp:PlaceHolder ID="phSiteAlias2" Visible="false" runat="server">
                <div id="dvSiteAlias">
                    <table class="ektronGrid">
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl name")%>"><%=_MessageHelper.GetMessage("lbl name")%>:</td>
                            <td class="value">http:// <input type="text" title="<%=_MessageHelper.GetMessage("lbl name")%>" id="txtAliasName" /></td>
                        </tr>
                    </table>
                    <input type="button" title="<%=_MessageHelper.GetMessage("lbl add new site alias")%>" onclick="addSiteAliasName(this);" id="btnAddSiteAlias" value="<%=_MessageHelper.GetMessage("lbl sitealias add")%>" />
                    <input type="button" title="<%=_MessageHelper.GetMessage("lbl reset alias name")%>" onclick="clearAliasName();" value="<%=_MessageHelper.GetMessage("lbl sitealias reset")%>" />
                    <div class="ektronTopSpace"></div>
                    <div id="divSiteAliasList"></div>
                </div>
            </asp:PlaceHolder>
            <div id="dvBlogRoll">
                <asp:Label ID="lbl_vf_roll" runat="server" />
            </div>
            <asp:PlaceHolder ID="phFolderAliases2" runat="server" Visible="false">
                <div id="dvAlias">
                    <asp:Literal ID="ltrFolderAliases2" runat="server"></asp:Literal>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</div>

<input type="hidden" id="savedSiteAlias" name="savedSiteAlias" value="" />
<asp:Label runat="server" ID="lbl_vf_showpane" />

<script type="text/javascript">
	<!--//--><![CDATA[//><!--
	    document.forms.frmContent.foldername.onkeypress = document.forms.frmContent.netscape.onkeypress;
	    document.forms.frmContent.stylesheet.onkeypress = document.forms.frmContent.netscape.onkeypress;
	    document.forms.frmContent.templatefilename.onkeypress = document.forms.frmContent.netscape.onkeypress;
		<asp:literal id="js_ef_focus" runat="server"/>
		<asp:literal id="ltr_blog_js" runat="server"/>
		Ektron.ready(function(){
            if($ektron('div#sitemap_nodes > div').length >= 2){ 
                $ektron('#dvUpDownArrow')[0].style.display='block';
            } else {
                $ektron('#dvUpDownArrow')[0].style.display='none';
            }
		});
	//--><!]]>
</script>

<input type="hidden" name="netscape" onkeypress="javascript:return CheckKeyValue(event,'34');" />
<input type="hidden" id="folder_id" name="folder_id" runat="server" />
<input type="hidden" id="contentids" name="contentids" runat="server" />
<input type="hidden" id="contentlanguages" name="contentlanguages" runat="server" />
<input type="hidden" id="content_id" name="content_id" value="0" runat="server" />
<input id="inherit_taxonomy_from" type="hidden" name="inherit_taxonomy_from" runat="server" value="0" />
<input id="current_category_required" type="hidden" name="current_category_required" runat="server" value="0" />
<input id="parent_category_required" type="hidden" name="current_category_required" runat="server" value="0" />
<input id="inherit_alias_from" type="hidden" name="inherit_alias_from" runat="server" value="0" />
<input id="current_alias_required" runat="server" type="hidden" name="current_alias_required" value="0" />
<input type="hidden" value="" name="defaultTemplateID" id="defaultTemplateID" />
<input id="inherit_IscontentSearchable_from" type="hidden" name="inherit_IscontentSearchable_from" runat="server" value="0" />
<input id="current_IscontentSearchable" runat="server" type="hidden" name="current_IscontentSearchable_required" value="0" />
<input id="inherit_IsDisplaySettings_from" type="hidden" name="inherit_IsDisplaySettings_from" runat="server" value="0" />
<input id="current_IsDisplaySettings" runat="server" type="hidden" name="current_IsDisplaySettings_required" value="0" />