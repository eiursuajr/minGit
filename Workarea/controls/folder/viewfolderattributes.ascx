<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewfolderattributes" CodeFile="viewfolderattributes.ascx.cs" %>

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
                    <a title="<%=_MessageHelper.GetMessage("generic taxonomy lbl")%>" href="#dvTaxonomy">
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
                        <asp:Literal ID="ltrTypes" runat="server" />
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
                            <asp:Literal ID="ltrAliases" Text="Aliasing" runat="server" />
                        </a>
                    </li>
                </asp:PlaceHolder>
            </ul>

            <div id="dvProperties">
                <table class="ektronGrid">
                    <tr>
                        <td class="label" title="<%=_MessageHelper.GetMessage("id label")%>"><%=_MessageHelper.GetMessage("id label")%></td>
                        <td class="value" id="td_vf_idtxt" runat="server"></td>
                    </tr>
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
                            <td class="value" id="td_vf_foldertxt" runat="server"></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phBlogProperties2" Visible="false" runat="server">
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl tag line")%>"><%=_MessageHelper.GetMessage("lbl tag line")%>:</td>
                            <td class="value" id="td_vf_taglinetxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl posts visible")%>"><%=_MessageHelper.GetMessage("lbl posts visible")%>:</td>
                            <td class="value" id="td_vf_postsvisibletxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("comments label")%>"><%=_MessageHelper.GetMessage("comments label")%>:</td>
                            <td class="value" id="td_vf_commentstxt" runat="server"></td>
                        </tr>
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl update services")%>"><%=_MessageHelper.GetMessage("lbl update services")%>:</td>
                            <td class="value" id="td_vf_updateservicestxt" runat="server"></td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phDescription" Visible="true" runat="server">
                    <tr>
                        <td class="label" title="<%=_MessageHelper.GetMessage("lbl description")%>"><%=_MessageHelper.GetMessage("lbl description")%>:</td>
                        <td class="value" id="td_vf_folderdesctxt" runat="server"></td>
                    </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td class="label" title="<%=_MessageHelper.GetMessage("lbl style sheet")%>"><%=_MessageHelper.GetMessage("lbl style sheet")%>:</td>
                        <td class="value" id="td_vf_stylesheettxt" runat="server"></td>
                    </tr>
                    <asp:PlaceHolder ID="phProductionDomain" Visible="false" runat="server">
                        <asp:Literal ID="DomainFolder" runat="server" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phPublishAsPdf" Visible="true" runat="server">
                    <asp:Panel ID="officedocumentspanel" Visible = "true" runat ="server">
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl office documents")%>"><%=_MessageHelper.GetMessage("lbl office documents")%>:</td>
                            <td class="value" id="td_vf_pdfactivetxt" runat="server"></td>
                        </tr>
                    </asp:Panel>
                    
                    <tr runat="server" visible="true" id="LocaleTaxonomy">
                    <td><%=_MessageHelper.GetMessage("lbl translation packages")%></td>
                    <td>
                       <asp:Literal ID="LocaleTaxonomyList" runat="server" />
                    </td>
                    </tr>
					 <tr>
                         <td></td>
                           <td><asp:Literal ID="ltrCheckPdfServiceProvider" runat="server" Text=""></asp:Literal>
                        </td>
                    </tr>
                    </asp:PlaceHolder>  
                    <asp:PlaceHolder ID="phContSearch2" Visible="true" runat="server">
                        <div id="dvContSearch">
                            <asp:Literal ID="ltrContSearch2" runat="server"></asp:Literal>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phDisplaySettings2" Visible="false" runat="server">
                        <div id="dvDisplaySettings">
                            <asp:Literal ID="ltrDisplaySettings2" runat="server"></asp:Literal>
                        </div>
                    </asp:PlaceHolder>            
                    <asp:PlaceHolder ID="phPreapprovalGroup" Visible="false" runat="server">
                        <tr>
                            <td class="label" title="<%=_MessageHelper.GetMessage("lbl preapproval group")%>">
                                <%=_MessageHelper.GetMessage("lbl preapproval group")%>:
                            </td>
                            <td class="value" id="td_vf_preapprovaltxt" runat="server">
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <%--<asp:PlaceHolder ID="LocaleTaxonomy" Visible="false" runat="server">
                    <tr>
                    <td><%=_MessageHelper.GetMessage("lbl translation packages")%></td>
                    <td>
                       <asp:Literal ID="LocaleTaxonomyList" runat="server" />
                    </td>
                    </tr></asp:PlaceHolder>--%>                    
                </table>                   
            </div>
            <div id="dvTaxonomy">
                <asp:Literal ID="taxonomy_list" runat="server" />
            </div>
            <div id="dvTemplates">
                <asp:Literal ID="template_list" runat="server" />
            </div>
            <div id="dvFlagging">
                <asp:Literal ID="flagging_options" runat="server" />
            </div>
            <div id="dvMetadata">
                <asp:Literal ID="litMetadata" runat="server" />
            </div>
            <div id="dvWebAlerts" class="ui-tabs-hide">
                <asp:Literal ID="lit_vf_subscription_properties" runat="server" />
                <asp:Literal ID="lit_vf_subscription_assignments" runat="server" />
            </div>
            <div id="dvTypes">
                <asp:Literal ID="ltr_vf_smartforms" runat="server" />
            </div>
            <div id="dvBreadcrumb">
                <asp:Panel ID="pnlInheritSitemapPath" runat="server">
                    <input type="checkbox" title="Inherit Parent Configuration" runat="server" name="chkInheritSitemapPath" id="chkInheritSitemapPath" checked="checked" />
                    <asp:Literal ID="ltInheritSitemapPath" runat="server" />
                    <div class="ektronTopSpace"></div>
                </asp:Panel>
                <table class="ektronGrid">
                    <tr>
                        <td class="label"><%=_MessageHelper.GetMessage("lbl path")%>:</td>
                        <td class="readOnlyValue"><span id="sitepath_preview"></span></td>
                    </tr>
                </table>
            </div>
            <div id="dvSiteAlias">
                <div id="viewSiteAliasList" runat="server"></div>
                <asp:Literal ID="ReplicationMethod" runat="server" />
            </div>
            <div id="dvSubjects">
                <asp:Literal ID="ltr_vf_categories_lbl" runat="server" />
                <asp:Literal ID="ltr_vf_categories" runat="server" />
            </div>
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
<asp:Label runat="server" ID="lbl_vf_showpane" />
