<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewconfiguration" CodeFile="viewconfiguration.ascx.cs" %>

<script type="text/javascript">
    Ektron.ready( function()
        {
            var tabsContainers = $ektron(".tabContainer");
            tabsContainers.tabs();
        }
    );
</script>

<div class="ektronPageContainer ektronPageTabbed">
    <div class="tabContainerWrapper">
        <div class="tabContainer">
            <asp:PlaceHolder ID="phTabs" runat="server">
                <ul>
                    <li>
                        <a title="<%=m_refMsg.GetMessage("general label")%>" href="#dvGeneral">
                            <%=m_refMsg.GetMessage("general label")%>
                        </a>
                    </li>
                    <li>
                        <a title="<%=m_refMsg.GetMessage("editor options label")%>" href="#dvEditor">
                            <%=m_refMsg.GetMessage("editor options label")%>
                        </a>
                    </li>
                    <li>
                        <a title="<%=m_refMsg.GetMessage("workarea options label")%>" href="#dvWorkarea">
                            <%=m_refMsg.GetMessage("workarea options label")%>
                        </a>
                    </li>
                    <li>
                        <a title="<%=m_refMsg.GetMessage("generic system")%>" href="#dvSystem">
                            <%=m_refMsg.GetMessage("generic system")%>
                        </a>
                    </li>
                </ul>
            </asp:PlaceHolder>

            <div id="dvGeneral">
                <span class="label" id="td_version" runat="server"></span>
                <span class="build" id="td_buildnumber" runat="server"></span>
                <span class="servicepack" id="td_ServicePack" runat="server"></span>

                <table class="ektronGrid">
		            <tr>
		                <td title="<%=m_refMsg.GetMessage("lbl Default Site Language")%>" class="label"><%=m_refMsg.GetMessage("lbl Default Site Language")%>:</td>
		                <td id="td_Language" runat="server"></td>
	                </tr>
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("license key input message")%>" class="label"><%=m_refMsg.GetMessage("license key input message")%></td>
		                <td id="td_licensekey" runat="server"></td>
	                </tr>
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("lbl Module Licenses")%>" class="label"><%=m_refMsg.GetMessage("lbl Module Licenses")%>:</td>
		                <td id="td_modulelicense" runat="server"></td>
	                </tr>
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("setup default language prompt")%>" class="label"><%=m_refMsg.GetMessage("setup default language prompt")%></td>
		                <td id="td_languagelist" runat="server"></td>
	                </tr>
	                <!--<tr>
		                <td class="label"><%=m_refMsg.GetMessage("settings max content label")%></td>
		                <td id="td_maxcontent" runat="server"></td>
	                </tr>-->
	               <%-- <tr>
		                <td title="<%=m_refMsg.GetMessage("settings max summary label")%>" class="label"><%=m_refMsg.GetMessage("settings max summary label")%>:</td>
		                <td id="td_maxsummary" runat="server"></td>
	                </tr>--%>
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("system email address label")%>" class="label"><%=m_refMsg.GetMessage("system email address label")%></td>
		                <td id="td_email" runat="server"></td>
	                </tr>
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("email notifications label")%>" class="label"><%=m_refMsg.GetMessage("email notifications label")%></td>
		                <td id="td_email_msg" runat="server"></td>
	                </tr>
					<tr>
		                <td title="<%=m_refMsg.GetMessage("lbl server type")%>" class="label"><%=m_refMsg.GetMessage("lbl server type")%></td>
		                <td id="td_server_type" runat="server"></td>
	                </tr>
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("lbl Asynchronous Processor Location")%>" class="label"><%=m_refMsg.GetMessage("lbl Asynchronous Processor Location")%>:</td>
		                <td id="td_asynch_location" runat="server"></td>
	                </tr>
	                <tr id="trPublishPDF" runat="server">
		                <td title="Publish In Other Format" class="label"><%=m_refMsg.GetMessage("alt publish in other format")%>:</td>
		                <td id="td_publish_pdf" runat="server"></td>
	                </tr>
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("library filesytem folders label")%>" class="label"><%=m_refMsg.GetMessage("library filesytem folders label")%>:</td>
		                <td id="td_libfolder" runat="server"></td>
	                </tr>
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("built in user label")%>" class="label"><%=m_refMsg.GetMessage("built in user label")%></td>
		                <td id="td_user" runat="server"></td>
	                </tr>
                </table>
            </div>
            <div id="dvEditor">
	            <table class="ektronGrid">
	                <tr>
		                <td title="<%=m_refMsg.GetMessage("styles label")%>" class="label"><%=m_refMsg.GetMessage("styles label")%></td>
		                <td id="td_wordstyle" runat="server"></td>
	                </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("fonts label")%>" class="label">
                            <%=m_refMsg.GetMessage("fonts label")%>
                        </td>
                        <td id="td_enablefont" runat="server">
                        </td>
                    </tr>
                    <tr id="tr_wordclass" visible="false" runat="server">
                        <td id="td_wordclass" runat="server">
                        </td>
                    </tr>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("accessibility label")%>" class="label">
                            <%=m_refMsg.GetMessage("accessibility label")%>
                        </td>
                        <td id="td_access" runat="server">
                        </td>
                    </tr>
	            </table>
            </div>
            <div id="dvWorkarea">
                <div title="<%=m_refMsg.GetMessage("lbl landing page after login")%>" class="ektronHeader"><%=m_refMsg.GetMessage("lbl landing page after login")%></div>
	            <table class="ektronGrid">
	                <tr>
	                    <td class="label"><span id="td_template" runat="server"></span></td>
	                    <td></td>
	                </tr>
		            <tr>
                        <td  title="<%=m_refMsg.GetMessage("alt set smart desktop as the start location in the workarea")%>" class="label"><%=m_refMsg.GetMessage("alt set smart desktop as the start location in the workarea")%>:</td>
	                    <td id="td_folder" runat="server"></td>
                    </tr>
	                <%--<tr>
	                    <td title="<%=m_refMsg.GetMessage("lbl display button text in the title bar")%>" class="label"><%=m_refMsg.GetMessage("lbl display button text in the title bar")%>:</td>
		                <td class="value" id="td_titletext" runat="server"></td>
	                </tr>--%>
                    <tr>
                        <td title="<%=m_refMsg.GetMessage("force preferences msg")%>" class="label"><%=m_refMsg.GetMessage("force preferences msg")%>:</td>
                        <td class="value"><span id="td_force" runat="server"></span></td>
                    </tr>
	                <tr>
	                    <td title="<%=m_refMsg.GetMessage("verify user on add lbl")%>" class="label"><%=m_refMsg.GetMessage("verify user on add lbl")%>:</td>
	                    <td class="value">
	                        <span id="td_verify_user_on_add" runat="server"></span>
	                        <br />
	                        <span title="<%=m_refMsg.GetMessage("verify user on add desc")%>" class="ektronCaption"><%=m_refMsg.GetMessage("verify user on add desc")%></span>
	                    </td>
	                </tr>
	                <tr>
	                    <td title="<%=m_refMsg.GetMessage("lbl Enable preapproval group")%>" class="label"><%=m_refMsg.GetMessage("lbl Enable preapproval group")%>:</td>
	                    <td class="value"><span id="td_enable_preapproval" runat="server"></span></td>
	                </tr>
	            </table>
            </div>
            <div id="dvSystem">
                <table class="ektronGrid">
		            <tr>
		                <td title="<%=m_refMsg.GetMessage("generic application")%>" class="label"><%=m_refMsg.GetMessage("generic application")%>:</td>
		                <td id="td_shutdown">
                            <asp:Button ToolTip="Restart" ID="ShutDownClick" runat="server" Text="Restart"  OnClick="ShutDownClick_Click"/>
                            <br title="<%=m_refMsg.GetMessage("msg application reset")%>" /><%=m_refMsg.GetMessage("msg application reset")%>
                        </td>
		            </tr>
		        </table>
            </div>
        </div>
    </div>
</div>

