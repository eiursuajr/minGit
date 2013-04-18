<%@ Control Language="C#" AutoEventWireup="true" CodeFile="personalization.ascx.cs" Inherits="WidgetControls_WidgetSpace" %>
<%@ Reference Control="Views/personalization_widget_list.ascx" %>
<%@ Reference Control="Views/personalization_widget_list_container.ascx" %>
<asp:PlaceHolder ID="EktronClientManager" runat="server"></asp:PlaceHolder>
<div class="EktronPersonalizationWrapper">
    <asp:UpdatePanel ID="apPersonalization" runat="server" UpdateMode="Conditional" OnLoad="apPersonalization_Load" ChildrenAsTriggers="true" RenderMode="Block">
        <ContentTemplate>
            <div class="EktronPersonalization clearfix">
                <div class="tabWrapper clearfix">
                    <div class="innerWrapper">
                        <ul id="ulTabs" class="tabs clearfix" runat="server">
                            <li  class="tab tabOptions" id="liOptions" runat="server">
                                <a onclick="return false;" class="tabOptions" href="#Options">&#160;</a>
                            </li>
                            <asp:Repeater ID="repTabs"  runat="server" OnItemDataBound="repTabs_ItemDataBound" OnItemCommand="repTabs_ItemCommand" EnableViewState="False">
                                <ItemTemplate>
                                    <li id="liTab" runat="server">
                                        <asp:LinkButton ToolTip="Remove" ID="lbRemoveTab" runat="server" CssClass="remove" OnClientClick="Ektron.Personalization.Tabs.remove(this);return false;"></asp:LinkButton>
                                        <asp:LinkButton ToolTip="Select" ID="lbSelectTab" runat="server" CssClass="label"></asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
		                </ul>
		            </div>
                    <ul id="ulTabOptions" class="tabOptions clearfix" runat="server">
                        <li title="<%=m_refMsg.GetMessage("add tab")%>" class="tab addTab" id="ctl00_ContentPlaceHolder1_widgetSpace_liAddTab">
                            <a onclick="Ektron.Personalization.Tabs.add();return false;" class="ektronPersonalizationAddTab" href="#AddTab" title="<%=m_refMsg.GetMessage("add tab")%>">
                                <span class="addTab"><%=m_refMsg.GetMessage("add tab")%></span>
                            </a>
                        </li>
                        <li title="<%=m_refMsg.GetMessage("add column")%>" class="tab addColumn">
                            <asp:LinkButton  ID="lbAddColumn" CssClass="ektronPersonalizationAddColumn" runat="server" OnClick="lbAddColumn_Click">
                                <span class="addColumn"><%=m_refMsg.GetMessage("add column")%></span>
                            </asp:LinkButton>
                        </li>
                        <li id="liResetWidgets" runat="server" title="set in code hehind" class="tab resetWidgets">
                            <a onclick="Ektron.Personalization.Widgets.reset(this);return false;" class="ektronPersonalizationResetWidgets" href="#ResetWidgets">
                                <span class="resetWidgets"><%=m_refMsg.GetMessage("reset widgets")%></span>
                            </a>
                        </li>
                        <li id="liEditDefaultWidgets" runat="server" title="set in code hehind" class="tab editDefaultWidgets">
                            <a onclick="Ektron.Personalization.Widgets.editDefaults(this);return false;" class="ektronPersonalizationEditDefaultWidgets" href="#ResetWidgets">
                                <span class="editDefaultWidgets"><%=m_refMsg.GetMessage("edit default widgets")%></span>
                            </a>
                        </li>
                        <li id="liDone" runat="server" title="set in code hehind" class="tab done">
                            <asp:LinkButton ID="lbDone" CssClass="ektronPersonalizationDone" runat="server" OnClick="lbDone_Click">
                                <span class="done"><%=m_refMsg.GetMessage("save default")%></span>
                            </asp:LinkButton>
                        </li>
                    </ul>
	            </div>
                <div class="widgetTray">
                    <div class="innerWrapper">
                        <p class="scrollWrapper">
                            <a onclick="Ektron.Personalization.WidgetTray.previous();return false;" class="scrollLeft hide" href="#ScrollLeft"><img id="imgWidgetTrayScrollLeft" src="" class="widgetTrayLeft" runat="server" /></a>
				            <a onclick="Ektron.Personalization.WidgetTray.next();return false;" class="scrollRight" href="#ScrollRight"><img id="imgWidgetTrayScrollRight" src="" class="widgetTrayRight" runat="server" />&#160;</a>
                        </p>
                        <div class="overflowWrapper">
                            <asp:Repeater ID="repWidgetTypes" runat="server">
                                <HeaderTemplate>
                                    <ul class="ektronPersonalizationWidgetList widgetList">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li title="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title %>" class="widgetToken">
                                        <img src="<%# WidgetsPath + (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ControlURL.Replace("\\", "/") %>.jpg" title="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title %>" alt="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).Title %>" />
                                        <span><%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ButtonText %></span>
                                        <input type="hidden" value="<%# (Container.DataItem as Ektron.Cms.Widget.WidgetTypeData).ID %>" />
                                    </li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
	            <p class="widgetTrayHandle">&#160;
                    <asp:HyperLink ID="aWidgetTrayHandle" runat="server" Visible="False" CssClass="widgetTrayToggle" NavigateUrl="#ToggleWidgetTray">
                        <img id="imgWidgetHandle" runat="server" src="" class="widgetHandle"  />
                        <span class="direction">&#160;</span>
                    </asp:HyperLink>
                </p>
                <table id="ektronPersonalizationTabModal" class="ektronWindow tabWindow" summary="Tab Modal Window">
                    <thead>
                        <tr>
                            <th class="tabHeader">
                                 <img id="imgEktronModalClose" src="" class="modalClose ektronModalClose" runat="server"  />
                                <span class="label">&#160;</span>
                            </th>
                        </tr>
                    </thead>
                    <tfoot>
                        <tr>
                            <td>
                                <div class="addTab buttons">
                                    <a href="#Cancel" class="button buttonRight redHover ektronModalClose" title="Cancel" onclick="return false;">
				                        <img id="imgAddTabEktronModalCancel" runat="server" src="" class="modalCancel" alt="Cancel" title="Cancel" />
				                        <%=m_refMsg.GetMessage("btn cancel")%>
			                        </a>
			                        <asp:LinkButton ToolTip="OK" CssClass="button buttonRight greenHover gutterLeft" ID="lbAddTab" runat="server" OnClick="lbAddTab_Click" OnClientClick="Ektron.Personalization.Tabs.vaidateLabel(this);return false;">
			                            <img id="imgAddTabEktronModalOk" runat="server" src="" class="modalOK" alt="OK" />
				                       <%=m_refMsg.GetMessage("btn ok")%>
			                        </asp:LinkButton>
			                    </div>
                                <div class="removeTab buttons">
                                    <a href="#Cancel" class="button buttonRight redHover ektronModalClose" title="Cancel" onclick="return false;">
			                            <img id="imgRemoveTabEktronModalCancel" runat="server" src="" class="modalCancel" alt="Cancel" title="Cancel" />
			                             <%=m_refMsg.GetMessage("btn cancel")%>
		                            </a>
                                    <asp:LinkButton ToolTip="OK" CssClass="button buttonRight greenHover gutterLeft confirmRemove" ID="lbRemoveTabConfirmation" runat="server" OnClick="lbRemoveTabConfirmation_Click" OnClientClick="Ektron.Personalization.Tabs.removeConfirmation(this);return false;">
			                            <img id="imgRemoveTabEktronModalOk" runat="server" src="" class="modalOK" alt="OK" title="OK" />
				                         <%=m_refMsg.GetMessage("btn ok")%>
			                        </asp:LinkButton>
		                        </div>
		                        <div class="removeColumn buttons">
		                            <a href="#Cancel" class="button buttonRight redHover ektronModalClose" title="Cancel" onclick="return false;">
			                            <img id="imgRemoveColumnEktronModalCancel" runat="server" src="" class="modalCancel" alt="Cancel" title="Cancel" />
			                            <%=m_refMsg.GetMessage("btn cancel")%>
		                            </a>
                                    <a href="#RemoveColumn" title="OK" class="button buttonRight greenHover gutterLeft confirmRemove" onclick="Ektron.Personalization.Columns.removeConfirmation(this);return false;">
			                            <img id="imgRemoveColumnEktronModalOk" runat="server" src="" class="modalOK" alt="OK" title="OK" />
				                         <%=m_refMsg.GetMessage("btn ok")%>
			                        </a>
		                        </div>
		                        <div class="resetWidgets buttons">
		                            <a href="#Cancel" class="button buttonRight redHover ektronModalClose" title="Cancel" onclick="return false;">
			                            <img id="imgResetWidgetsEktronModalCancel" runat="server" src="" class="modalCancel" alt="Cancel" title="Cancel" />
			                            <%=m_refMsg.GetMessage("btn cancel")%>
		                            </a>
		                            <asp:LinkButton ToolTip="Reset Widgets" CssClass="button buttonRight greenHover gutterLeft confirmRemove" ID="lbResetWidgets" runat="server" OnClick="lbResetWidgets_Click">
			                            <img id="imgResetWidgetsEktronModalOk" runat="server" src="" class="modalOK" alt="OK" title="OK" />
				                         <%=m_refMsg.GetMessage("btn ok")%>
			                        </asp:LinkButton>
		                        </div>
		                        <div class="editDefaultWidgets buttons">
		                            <a href="#Cancel" class="button buttonRight redHover ektronModalClose" title="Cancel" onclick="return false;">
			                            <img id="imgEditDefaultWidgetsEktronModalCancel" runat="server" src="" class="modalCancel" alt="Cancel" title="Cancel" />
			                            <%=m_refMsg.GetMessage("btn cancel")%>
		                            </a>
		                            <asp:LinkButton ToolTip="Reset Widgets" CssClass="button buttonRight greenHover gutterLeft confirmEditDefaultWidgets" ID="lbEditDefaultWidgets" runat="server" OnClick="lbEditDefaultWidgets_Click">
			                            <img id="imgEditDefaultWidgetsEktronModalOk" runat="server" src="" class="modalOK" alt="OK" title="OK" />
				                         <%=m_refMsg.GetMessage("btn ok")%>
			                        </asp:LinkButton>
		                        </div>
                            </td>
                        </tr>
                    </tfoot>
                    <tbody>
                        <tr>
                            <td>
                                <div class="wrapper">
				                    <div class="addTab clearfix">
				                        <div class="form">
				                            <p class="dialogLabel greenLabel"><%=m_refMsg.GetMessage("lbl personalization add tab")%></p>
				                            <p class="first">
				                                <asp:Label CssClass="label" runat="server" AssociatedControlID="tbTitle"  ID="lblAddTabTitle"/>
                                                <asp:TextBox ID="tbTitle" runat="server" MaxLength="25" Width="240px"/>
                                                <span class="required">*</span>
                                                <span class="required requiredReminder"> <%=m_refMsg.GetMessage("generic required field")%></span>
                                            </p>
                                            <p class="last">
                                                <asp:Label runat="server" AssociatedControlID="ddlScope" Text="Tab Scope:" ID="lblAddTabScope"/>
                                                <asp:DropDownList runat="server" ID="ddlScope">
                                                    <asp:ListItem Selected="True" Enabled="true" Text="Public" Value="true"/>
                                                    <asp:ListItem Selected="False" Enabled="true" Text="Private" Value="false"/>
                                                </asp:DropDownList>
                                            </p>
				                        </div>
				                    </div>
			                        <div class="removeTab clearfix">
			                            <div class="form">
			                                <p class="dialogLabel redLabel"><%=m_refMsg.GetMessage("lbl personalization remove tab txt")%></p>
			                                <p><%=m_refMsg.GetMessage("lbl personalization remove body text")%> <span class="label emphasis"></span>?</p>
			                            </div>
				                    </div>
				                    <div class="removeColumn clearfix">
			                            <div class="form">
			                                <p class="dialogLabel redLabel"><%=m_refMsg.GetMessage("lbl personalization remove cloumn text")%></p>
			                                <p><%=m_refMsg.GetMessage("lbl personalization remove cloumn confirm")%></p>
			                                <p class="smallText italic gutterTop"><%=m_refMsg.GetMessage("lbl personalization remove cloumn note")%></p>
			                            </div>
				                    </div>
				                    <div class="resetWidgets clearfix">
			                            <div class="form">
			                                <p class="dialogLabel redLabel"><%=m_refMsg.GetMessage("lbl personalization reset widget txt")%></p>
			                                <p><%=m_refMsg.GetMessage("lbl personalization reset widget confirm")%></p>
			                                <p class="smallText italic gutterTop"><%=m_refMsg.GetMessage("lbl personalization reset widget notes")%></p>
			                            </div>
				                    </div>
				                    <div class="editDefaultWidgets clearfix">
			                            <div class="form">
			                                <p class="dialogLabel redLabel"><%=m_refMsg.GetMessage("lbl personalization edit default text")%></p>
			                                <p class="smallText italic gutterTop">
			                                    <%=m_refMsg.GetMessage("lbl personalization edit default notes")%>
			                                </p> 
			                            </div>
				                    </div>
				                </div>
				            </td>
			            </tr>
		            </tbody>
	            </table>
	            <div class="widgetWrapper">
                    <asp:PlaceHolder ID="phWidgetPages" runat="server"></asp:PlaceHolder>
					<asp:PlaceHolder ID="errorMessages" runat="server"></asp:PlaceHolder>
                </div>
            </div>
            <ektronUI:JavaScriptBlock ID="PersonalizationInitJS" runat="server" ExecutionMode="OnEktronReady">
                <ScriptTemplate>
                    Ektron.Personalization.init();
                </ScriptTemplate>
            </ektronUI:JavaScriptBlock>
        </ContentTemplate>
    </asp:UpdatePanel>
</div>