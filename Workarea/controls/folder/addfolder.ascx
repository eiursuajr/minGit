<%@ Control Language="C#" AutoEventWireup="true" Inherits="addfolder" CodeFile="addfolder.ascx.cs" %>
<script type="text/javascript">
    <!--//--><![CDATA[//><!--
        var taxonomytreearr="".split(",");
        var taxonomyparenttreearr="<%=_SelectedTaxonomyList%>".split(",");
        var __jscatrequired="0";
        var __jsparentcatrequired="<%=_CurrentCategoryChecked%>";
        var isProductCatalog = <%=_IsCatalog.ToString().ToLower()%>;
        
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

            ShowServerErrorMessage();
        });
        
        function ShowServerErrorMessage() {
            var errorMessage = $ektron(".hdnErrorMessage").val();
            if (errorMessage != null && errorMessage.length > 0) {
                alert(errorMessage);
            }
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
	    function checkForDefaultTemplate(){
	        var defaultTemplate = $ektron('tbody#templateTable input:radio');
	        var i = 0;
            var j = 0;
            
            for(i = 0; i < defaultTemplate.length; i++)
            {
               if(defaultTemplate[i].checked){
                   j = j + 1;
               }
            }
            if( j == 0 ){
               alert('<asp:Literal runat="server" id="ltrSelectDefTemp" />');
               return false;
            }
	    }
    //--><!]]>
</script>
<!--[if lte IE 7]>
    <style type="text/css">
        div#parah input {position:relative;top:-5px;}
    </style>
<![endif]-->
 <style type="text/css">
    .selectContent { background-image: url('Images/ui/icons/check.png');background-repeat: no-repeat;background-position:.5em center; }
    .useCurrent{ background-image: url('Images/ui/icons/shape_square.png'); background-repeat: no-repeat; background-position:.5em center; }
    #FrameContainer{ width: 80%; height: 60%; margin: -150px 0 0 -380px !important;position:absolute; margin:-120px 0 0 -10px; display:none; }
    #ChildPage { frameborder:0; border:0; marginheight:2; marginwidth:2; scrolling:auto; border:none; width:100%; height:100%; scrolling:auto; background-color: white; }
</style>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <asp:Panel ID="pnlOuterContainer" CssClass="ektronPageTabbed" runat="server">
        <div class="tabContainerWrapper">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <asp:PlaceHolder ID="phFolder" runat="server">
                            <li>
                                <a title="<%=MessageHelper.GetMessage("properties text")%>" href="#dvProperties">
                                    <%=MessageHelper.GetMessage("properties text")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("generic taxonomy lbl")%>" href="#dvTaxonomy">
                                    <%=MessageHelper.GetMessage("generic taxonomy lbl")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("lbl templates")%>" href="#dvTemplates">
                                     <%=MessageHelper.GetMessage("lbl templates")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("lbl flagging")%>" href="#dvFlagging">
                                    <%=MessageHelper.GetMessage("lbl flagging")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("metadata text")%>" href="#dvMetadata">
                                    <%=MessageHelper.GetMessage("metadata text")%>
                                </a>
                            </li>
                            <asp:PlaceHolder ID="phSubjects" Visible="false" runat="server">
                                <li>
                                    <a title="<%=MessageHelper.GetMessage("subjects text")%>" href="#dvSubjects">
                                        <%=MessageHelper.GetMessage("subjects text")%>
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phWebAlerts" Visible="false" runat="server">
                                <li>
                                    <a title="<%=MessageHelper.GetMessage("subjects text")%>" href="#dvWebAlerts">
                                        <%=MessageHelper.GetMessage("lbl web alert tab")%>
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phTypes" Visible="false" runat="server">
                                <li>
                                    <a title="<%=MessageHelper.GetMessage("Smart Forms txt") %>" href="#dvTypes">
                                        <asp:Literal ID="ltrTypes" runat="server" />
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("lbl sitemap path")%>" href="#dvBreadcrumb">
                                    <%=MessageHelper.GetMessage("lbl sitemap path")%>
                                </a>
                            </li>
                            <asp:PlaceHolder ID="phSiteAlias" Visible="false" runat="server">
                                <li>
                                    <a title="<%=MessageHelper.GetMessage("lbl site alias")%>" href="#dvSiteAlias">
                                        <asp:Literal ID="lblSiteAlias" runat="server" />
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="phFolderAliases" Visible="false" runat="server">
                                <li>
                                    <a title="<%=MessageHelper.GetMessage("lbl forcemanualaliasing")%>" href="#dvAlias">
                                        <%=MessageHelper.GetMessage("lbl forcemanualaliasing")%>
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phBlog" Visible="false" runat="server">
                            <li>
                                <a title="<%=MessageHelper.GetMessage("properties text")%>" href="#dvProperties">
                                    <%=MessageHelper.GetMessage("properties text")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("generic taxonomy lbl")%>" href="#dvTaxonomy">
                                    <%=MessageHelper.GetMessage("generic taxonomy lbl")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("lbl templates")%>" href="#dvTemplates">
                                    <%=MessageHelper.GetMessage("lbl templates")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("subjects text")%>" href="#dvCategories">
                                    <%=MessageHelper.GetMessage("subjects text")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("lbl blog roll")%>" href="#dvBlogRoll">
                                    <%=MessageHelper.GetMessage("lbl blog roll")%>
                                </a>
                            </li>
                            <asp:PlaceHolder ID="phBlogAlias" runat="server">
                                <li>
                                    <a title="aliases" href="#dvBlogAliases">
                                       <%=MessageHelper.GetMessage("lbl forcemanualaliasing")%>
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phDiscussionBoard" Visible="false" runat="server">
                            <li>
                                <a title="<%=MessageHelper.GetMessage("properties text")%>" href="#dvProperties">
                                    <%=MessageHelper.GetMessage("properties text")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("lbl templates")%>" href="#dvTemplates">
                                    <%=MessageHelper.GetMessage("lbl templates")%>
                                </a>
                            </li>
                            <li>
                                <a title="<%=MessageHelper.GetMessage("lbl blog cat")%>" href="#dvCategories">
                                    <%=MessageHelper.GetMessage("lbl blog cat")%>
                                </a>
                            </li>
                        </asp:PlaceHolder>
                    </ul>
                    <asp:Panel ID="pnlFolder" runat="server">
                        <div id="dvProperties">
                            <table class="ektronForm" width="60%">
                                <tr>
                                    <td class="label" id="tdfoldernamelabel" runat="server"></td>
                                    <td><input type="text" maxlength="100" name="foldername" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="<%=MessageHelper.GetMessage("generic description")%>"><%=MessageHelper.GetMessage("generic description")%>:</td>
                                    <td><input type="text" maxlength="256" name="folderdescription" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="<%=MessageHelper.GetMessage("lbl style sheet")%>"><%=MessageHelper.GetMessage("lbl style sheet")%>:</td>
                                    <td id="tdsitepath" runat="server" ></td>
                                </tr>
                                <asp:PlaceHolder ID="phProductionDomain" Visible="false" runat="server">
								    <tr>
										<td class="label" title="<%=MessageHelper.GetMessage("lbl Staging Domain")%>"><%=MessageHelper.GetMessage("lbl Staging Domain")%>:</td>
                                        <td class="value" id="tdstagingdomain" runat="server"></td>
									</tr>
                                    <tr>
                                        <td class="label" title="<%=MessageHelper.GetMessage("lbl Production Domain")%>"><%=MessageHelper.GetMessage("lbl Production Domain")%>:</td>
                                        <td class="value" id="tdproductiondomain" runat="server"></td>
                                    </tr>
								</asp:PlaceHolder>
                                 <asp:PlaceHolder ID="phPDF" Visible="false" runat="server">
                                    <tr>
                                        <td class="label" title="<%=MessageHelper.GetMessage("lbl office documents")%>"><%=MessageHelper.GetMessage("lbl office documents")%>:</td>
                                        <td class="value">
                                            <input type="checkbox" title="Publish as a PDF" id="publishAsPdf" name="publishAsPdf" onclick="CheckPdfServiceProvider()" <% =IsPublishedAsPdf() %> />
                                            <label for="publishAsPdf" id="lblPublishAsPdf" runat="server"></label>*
                                            <div class="ektronCaption"><%=MessageHelper.GetMessage("pdf generation warning")%></div>
                                            <div class="ektronCaption">* <%=MessageHelper.GetMessage("publish help")%></div>
                                            <div class="ektronCaption">
                                                <div id="pdfGenerationMessage" style="display:none">
                                                    <strong style="color:#C00000"><asp:Literal ID="ltrCheckPdfServiceProvider" runat="server" Text="Your PDF service provider is pdf.ektron.com. This service can be used for demonstration purposes only. To install a functioning PDF generator, contact your Ektron account manager"></asp:Literal></strong>
                                                 </div>
                                             </div>
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phContSearch" Visible="false" runat="server">
                                    <div id="dvContSearch">
                                        <asp:Literal ID="ltrContSearch" runat="server" Text ="iscont"></asp:Literal>
                                    </div>
                                </asp:PlaceHolder>
                                 <asp:PlaceHolder ID="phDisplaySettings" Visible="false" runat="server">
                                    <div id="dvDisplaySettings">
                                        <asp:Literal ID="ltrDisplaySettings" runat="server" Text ="isdisp"></asp:Literal>
                                    </div>
                                </asp:PlaceHolder>
                                <asp:Literal ID="ReplicationMethod" runat="server" />
                            </table>
                        </div>
                        <div id="dvTaxonomy">
                            <asp:Literal ID="taxonomy_list" runat="server" />
                        </div>
                        <div id="dvTemplates">
                            <asp:Literal ID="template_list" runat="server" />
                            <asp:Literal ID="ltrTemplateFilePath" runat="server" />
                        </div>
                        <div id="dvFlagging">
                            <asp:Literal ID="inheritFlag" runat="server" />
                            <div class="ektronTopSpace"></div>
                            <table class="ektronForm">
                                <tr>
                                    <td class="label" title="<%=MessageHelper.GetMessage("lbl flagging:")%>"><%=MessageHelper.GetMessage("lbl flagging:")%></td>
                                    <td class="value"><asp:DropDownList ToolTip="Flagging" ID="ddflags" runat="server" /></td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvMetadata">
                            <asp:Literal ID="lit_vf_customfieldassingments" runat="server" />
                            <input type="hidden" name="folder_cfld_assignments" id="folder_cfld_assignments" value="" />
                        </div>
                        <div id="dvWebAlerts" class="ui-tabs-hide">
                            <asp:Literal ID="lit_vf_subscription_properties" runat="server" />
                            <asp:Literal ID="lit_vf_subscription_assignments" runat="server" />
                            <input type="hidden" name="folder_sub_assignments" />
                        </div>
                        <asp:PlaceHolder ID="phTypesPanel" Visible="false" runat="server">
                            <div id="dvTypes">
                                <asp:Literal ID="ltr_vf_types" runat="server" />
                                <input type="hidden" id="language" value="1033" />
                            </div>
                        </asp:PlaceHolder>
                        <div id="dvBreadcrumb">
                            <input type="checkbox" onclick="InheritSitemapPath(this.checked);" name="chkInheritSitemapPath"
                                id="chkInheritSitemapPath" checked="checked" />
                            <asp:Literal ID="ltInheritSitemapPath" runat="server" />
                            <div class="ektronTopSpace"></div>
                            <table class="ektronForm">
                                <tr>
                                    <td class="label" title="<%=MessageHelper.GetMessage("lbl path")%>"><%=MessageHelper.GetMessage("lbl path")%>:</td>
                                    <td class="readOnlyValue"><span id="sitepath_preview"></span></td>
                                </tr>
                            </table>
                            <div class="ektronTopSpace"></div>
                            <div id="dvUpDownArrow" style="display:none;">
                                <img title="Move selected up" alt="Move selected up" onclick="moveSitemapPathNode('up')"
                                    src="images/UI/Icons/up.png" />
                                <img title="Move selected down" alt="Move selected down" onclick="moveSitemapPathNode('down')"
                                    src="images/UI/Icons/down.png" />
                            </div>
                            <div id="sitemap_nodes"></div>
                            <div class="ektronTopSpace"></div>
                            <div id="AddSitemapNode">
                                <table class="ektronForm">
                                    <tr>
                                        <td class="label" title="<%=MessageHelper.GetMessage("generic title")%>"><%=MessageHelper.GetMessage("generic title")%>:</td>
                                        <td class="value"><input title="<%=MessageHelper.GetMessage("generic title")%>" type="text" id="sitemaptitle_input" /></td>
                                    </tr>
                                    <tr>
                                        <td class="label" title="<%=MessageHelper.GetMessage("generic url link")%>"><%=MessageHelper.GetMessage("generic url link")%>:</td>
                                        <td class="value"><input title="<%=MessageHelper.GetMessage("generic url link")%>" type="text" id="sitemapurl_input" /><img alt="<%=MessageHelper.GetMessage("tooltip:select quicklink") %>" onclick="PopBrowseWin('quicklinks', '', 'document.forms[0].sitemapurl_input');return false;" src="images/UI/Icons/linkAdd.png" class="ektronClickableImage" /></td>
                                    </tr>
                                    <tr>
                                        <td class="label" title="<%=MessageHelper.GetMessage("generic description")%>"><%=MessageHelper.GetMessage("generic description")%>:</td>
                                        <td class="value"><input title="<%=MessageHelper.GetMessage("generic description")%>" type="text" id="sitemapdesc_input" /></td>
                                    </tr>
                                </table>
                                <div class="ektronTopSpaceSmall"></div>
                                <input type="button" title="<%=MessageHelper.GetMessage("tooltip:add sitemap")%>" onclick="addSiteMapNode(this); if($ektron('div#sitemap_nodes div').length > 2) { $ektron('#dvUpDownArrow')[0].style.display='block';};" id="btnAddSitepath" value="<%=MessageHelper.GetMessage("generic add title")%>" />
                                <input type="button" title="<%=MessageHelper.GetMessage("tooltip:reset sitemap")%>" onclick="clearSitemapForm()" value="<%=MessageHelper.GetMessage("res_mem_reset")%>" />
                            </div>
                            <input type="hidden" id="hdnInheritSitemap" name="hdnInheritSitemap" value="" />
                            <input type="hidden" id="saved_sitemap_path" name="saved_sitemap_path" value="" />
                        </div>
                        <asp:PlaceHolder ID="phSiteAlias2" Visible="false" runat="server">
                            <div id="dvSiteAlias">
                                <table class="ektronForm">
                                    <tr>
                                        <td class="label"><%=MessageHelper.GetMessage("lbl name")%>:</td>
                                        <td class="value">http:// <input type="text" id="txtAliasName" /></td>
                                    </tr>
                                </table>
                                <input type="button" title="<%=MessageHelper.GetMessage("lbl add new site alias")%>" onclick="addSiteAliasName(this);" id="btnAddSiteAlias" value="<%=MessageHelper.GetMessage("lbl sitealias add")%>" />
                                <input type="button" title="<%=MessageHelper.GetMessage("lbl reset alias name")%>" onclick="clearAliasName();" value="<%=MessageHelper.GetMessage("lbl sitealias reset")%>" />                                <div class="ektronTopSpace"></div>
                                <%=MessageHelper.GetMessage("lbl aliaslist") %>
                                <div id="divSiteAliasList"></div>
                                <input type="hidden" id="savedSiteAlias" name="savedSiteAlias" value="" />
                            </div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="phFolderAliases2" runat="server" Visible="false">
                            <div id="dvAlias">
                                <asp:Literal ID="ltrFolderAliases" runat="server"></asp:Literal>
                            </div>
                        </asp:PlaceHolder>
                    </asp:Panel>
                    <asp:Panel id="pnlBlog" Visible="false" runat="server">
                        <div id="dvProperties">
                            <table class="ektronForm">
                                <tr>
                                    <td class="label" title="<%=MessageHelper.GetMessage("lbl blog name")%>"><%=MessageHelper.GetMessage("lbl blog name")%>:</td>
                                    <td class="value"><asp:TextBox ID="txtBlogName" runat="server" MaxLength="70" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=MessageHelper.GetMessage("lbl blog title")%>:</td>
                                    <td class="value"><asp:TextBox ID="txtTitle" runat="server" MaxLength="75" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><asp:Label ToolTip="Visibility" ID="lblVisibility" runat="server" CssClass="label"><%=MessageHelper.GetMessage("lbl visibility")%>:</asp:Label></td>
                                    <td class="value">
                                        <asp:DropDownList ID="drpVisibility" runat="server">                                           
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" title="<%=MessageHelper.GetMessage("comments label")%>"><%=MessageHelper.GetMessage("comments label")%>:</td>
                                    <td class="value">
                                        <asp:CheckBox  ID="chkEnable" runat="server"  Checked="True" onclick="javascript:UpdateBlogCheckBoxes();" />
                                        <br />
                                        <asp:CheckBox  ID="chkModerate" runat="server"  Checked="True" />
                                        <br />
                                        <asp:CheckBox  ID="chkRequire" runat="server" Checked="True" />
                                    </td>
                                </tr>
                                <tr id="tr_enableblogreplication" visible="false" runat="server">
                                    <td class="label"><asp:Literal ID="BlogEnableReplication" runat="server" /></td>
                                </tr>
                                <asp:PlaceHolder ID="phContSearch1" Visible="false" runat="server">
                                    <div id="dvContSearch">
                                        <asp:Literal ID="ltrContSearch1" runat="server"></asp:Literal>
                                    </div>
                                </asp:PlaceHolder>
                                 <asp:PlaceHolder ID="phDisplaySettings1" Visible="false" runat="server">
                                    <div id="dvDisplaySettings1">
                                        <asp:Literal ID="ltrDisplaySettings1" runat="server"></asp:Literal>
                                    </div>
                                </asp:PlaceHolder>
                            </table>
                        </div>
                        <div id="dvTaxonomy">
                            <asp:Literal ID="litBlogTaxonomy" runat="server" />
                        </div>
                        <div id="dvTemplates">
                            <asp:Literal ID="litBlogTemplate" runat="server" />
                            <asp:Literal ID="litBlogTemplatedata" runat="server" />
                        </div>
                        <div id="dvCategories">
                            <asp:Literal ID="ltr_ab_cat" runat="server" />
                        </div>
                        <div id="dvBlogRoll">
                            <asp:Label ID="lbl_ab_roll" runat="server" />
                        </div>
                        <asp:HiddenField ID="hdnfolderid" runat="server" />
                        <asp:PlaceHolder ID="phBlogAliases2" runat="server">
                            <div id="dvBlogAliases">
                                <asp:Literal ID="ltrBlogAliases" runat="server"></asp:Literal>
                                <%--<input type="checkbox" id="chkInheritAliases" checked="checked" onclick="InheritAliasedChanged('chkblogaliasrequired')" /> Inherit parent configuration
                                <div class="ektronTopSpace"></div>
                                <table class="ektronForm">
                                    <tr>
                                        <td class="label">Force Aliasing</td>
                                        <td class="value"><input disabled="disabled" type="checkbox" id="chkblogaliasrequired" /></td>
                                    </tr>
                                </table>--%>
                            </div>
                        </asp:PlaceHolder>
                    </asp:Panel>
                    <asp:Panel ID="pnlDiscussionBoard" Visible="false" runat="server">
                        <div id="dvProperties">
                            <table class="ektronForm">
                                <tr>
                                    <td class="label"><%=MessageHelper.GetMessage("generic name")%>:</td>
                                    <td class="value"><asp:TextBox ID="txt_adb_boardname" runat="server" MaxLength="70" /><span class="required">*</span></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=MessageHelper.GetMessage("generic title")%>:</td>
                                    <td class="value"><asp:TextBox ToolTip="Title" ID="txt_adb_title" runat="server" MaxLength="75" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=MessageHelper.GetMessage("generic topics")%>:</td>
                                    <td class="value">
                                        <asp:CheckBox ToolTip="Require Authentication" ID="chk_adb_ra" runat="server"  Checked="True" />
                                        <br />
                                        <asp:CheckBox ToolTip="Moderate Comments" ID="chk_adb_mc" runat="server"  Checked="True" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label"><%=MessageHelper.GetMessage("generic css theme")%>:</td>
                                    <td class="value"><asp:DropDownList ID="drp_theme" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label"><%=MessageHelper.GetMessage("lbl style sheet")%>:</td>
                                    <td class="value">
                                        <asp:Literal ID="ltr_sitepath" runat="server" />
                                        <asp:TextBox ToolTip="Style Sheet" ID="txt_adb_stylesheet" runat="server" />
                                    </td>
                                </tr>
                                <asp:Literal ID="ltr_dyn_repl" runat="server" />
                                <asp:PlaceHolder ID="phContSearch2" Visible="false" runat="server">
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
                            <p class="required">* <%=MessageHelper.GetMessage("generic required field")%></p>
                        </div>
                        <div id="dvTemplates">
                            <asp:Literal ID="template_list_cat" runat="server" />
                            <asp:Literal ID="lit_ef_templatedata" runat="server" />
                        </div>
                        <div id="dvCategories">
                            <asp:Literal ID="ltr_adb_cat" runat="server" />
                        </div>
                        <asp:HiddenField ID="hdn_adb_folderid" runat="server" />
                    </asp:Panel>
                </div>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlDiscussionForum" CssClass="ektronPageInfo" Visible="false" runat="server">
        <table class="ektronForm">
            <tr>
                <td class="label"><%=MessageHelper.GetMessage("lbl DiscussionForumName")%>:</td>
                <td class="value"><asp:TextBox ID="txt_adf_forumname" runat="server" MaxLength="70" /></td>
            </tr>
            <tr>
                <td class="label"><%=MessageHelper.GetMessage("lbl DiscussionForumTitle")%>:</td>
                <td class="value"><asp:TextBox ID="txt_adf_forumtitle" runat="server" MaxLength="75" /></td>
            </tr>
            <tr>
                <td class="label"><%=MessageHelper.GetMessage("lbl discussionforumsortorder")%>:</td>
                <td class="value"><asp:TextBox ID="txt_adf_sortorder" runat="server" CssClass="ektronTextXXXSmall" Text="1" /></td>
            </tr>
            <tr>
                <td class="label"><%=MessageHelper.GetMessage("lbl discussionforumsubject")%>:</td>
                <td class="value"><asp:DropDownList ID="drp_adf_category" runat="server" /></td>
            </tr>
            <tr>
                <td class="label"><%=MessageHelper.GetMessage("lbl moderate comments")%>:</td>
                <td class="value"><asp:CheckBox ToolTip="Moderate Comments" ID="chk_adf_moderate" runat="server" Checked="True" /></td>
            </tr>
            <tr>
                <td class="label"><%=MessageHelper.GetMessage("lbl lock")%>:</td>
                <td class="value"><asp:CheckBox ToolTip="Lock" ID="chk_adf_lock" runat="server" Checked="False" /></td>
            </tr>
        </table>
        <asp:HiddenField ID="hdn_adf_folderid" runat="server" />
        <asp:Literal ID="ltr_adf_properties" runat="server" />
    </asp:Panel>
     <script type="text/javascript">
	    <!--//--><![CDATA[//><!--
	        var bexists = null;
            <asp:Literal runat="server" id="ltr_af_js" />
	    //--><!]]>
    </script>
    <input id="ParentID" type="hidden" name="ParentID" class="ParentID" runat="server" />
    <input id="frm_callingpage" type="hidden" name="frm_callingpage" runat="server" />
    <input id="inherit_taxonomy_from" type="hidden" name="inherit_taxonomy_from" runat="server" value="0" />
    <input id="current_category_required" type="hidden" name="current_category_required" runat="server" value="0" />
    <input type="hidden" name="parent_flag" id="parent_flag" value="0" runat="server"/>
    <input id="inherit_alias_from" type="hidden" name="inherit_alias_from" runat="server" value="0" />
    <input id="current_alias_required" runat="server" type="hidden" name="current_alias_required" value="0" />
    <input type="hidden" ID="hdnErrorMessage" name="hdnErrorMessage" runat="server" class="hdnErrorMessage" value="" />
    <input id="inherit_IscontentSearchable_from" type="hidden" name="inherit_IscontentSearchable_from" runat="server" value="0" />
    <input id="current_IscontentSearchable" runat="server" type="hidden" name="current_IscontentSearchable_required" value="0" />
    <input id="inherit_IsDisplaySettings_from" type="hidden" name="inherit_IsDisplaySettings_from" runat="server" value="0" />
    <input id="current_IsDisplaySettings" runat="server" type="hidden" name="current_IsDisplaySettings_required" value="0" />
</div>