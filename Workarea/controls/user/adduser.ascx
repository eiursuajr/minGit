<%@ Control Language="C#" AutoEventWireup="true" CodeFile="adduser.ascx.cs" Inherits="adduser" %>
<script type="text/javascript">
    function AvatorDialogClose()
    {
         var jsAvatorUploadDlgId = "<asp:literal id="jsUxDialogSelectorTxt" runat="server"/>";
         $ektron(jsAvatorUploadDlgId).dialog('close'); 
         return false;
    }

    function AvatarDialogInit()
    {
        setTimeout( function() 
        {
            $ektron(document).find(".uxAvatarUploadIframe").attr("src", "upload.aspx?action=edituser&addedit=true&returntarget=user_avatar&modal=true");
        }, 0);
    }

</script>
<asp:Literal ID="PostBackPage" runat="server" />
<div id="FrameContainer" style="display: none; left: 55px; width: 1px; position: absolute;
    top: 48px; height: 1px">
    <iframe id="ChildPage" name="ChildPage" frameborder="yes" marginheight="0" marginwidth="0"
        width="100%" height="100%" scrolling="auto"></iframe>
</div>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer">
    <div id="TR_AddUserDetail" runat="server">
        <div class="ektronPageTabbed">
            <div class="tabContainerWrapper">
                <div class="tabContainer">
                    <ul>
                        <li><a href="#General" title="General"><%=m_refMsg.GetMessage("general label")%></a></li>
                        <li><a href="#CustomProperties" title="Custom Properties"><%=m_refMsg.GetMessage("custom properties")%></a></li>
                    </ul>
                    <div id="General">
                        <table class="ektronGrid">
                            <tbody>
                                <asp:Literal ID="err_msg" EnableViewState="false" runat="server" />
                                <tr>
                                    <td class="label" title="Username"><span style="color:red;">*</span><%=m_refMsg.GetMessage("username label")%></td>
                                    <td class="value"><input title="Username Text" type="text" id="username" name="username" maxlength="255" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="First Name"><span style="color:red;">*</span><%=m_refMsg.GetMessage("first name label")%></td>
                                    <td class="value"><input title="First Name Text" type="text" id="firstname" name="firstname" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Last Name"><span style="color:red;">*</span><%=m_refMsg.GetMessage("last name label")%></td>
                                    <td class="value"><input title="Last Name Text" type="text" id="lastname" name="lastname" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Display Name"><span style="color:red;">*</span><%=m_refMsg.GetMessage("display name label")%>:</td>
                                    <td class="value"><input title="Display Name Text" type="text" id="displayname" name="displayname" maxlength="55" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Password"><span style="color:red;">*</span><%=m_refMsg.GetMessage("password label")%></td>
                                    <td class="value"><input title="Password  Text" type="password" id="pwd" name="pwd" maxlength="255" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Confirm Password"><span style="color:red;">*</span><%=m_refMsg.GetMessage("confirm pwd label")%></td>
                                    <td class="value"><input title="Confirm Password Text" type="password" id="confirmpwd" name="confirmpwd" maxlength="255" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                                </tr>
                                <% if(m_intGroupType == 0){ %>
                                <tr>
                                    <td class="label" title="<%=m_refMsg.GetMessage("user language label")%>"><%=m_refMsg.GetMessage("user language label")%></td>
                                    <td class="value"><asp:DropDownList ToolTip="Select User Language from Drop Down List" ID="language" runat="server" /></td>
                                </tr>
                                <% } %>
                                <tr>
                                    <td class="label" title="<%=m_refMsg.GetMessage("email address label")%>"><%=m_refMsg.GetMessage("email address label")%></td>
                                    <td class="value"><input title="E-Mail Address Text" type="text" maxlength="255" size="25" id="email_addr1" name="email_addr1" onkeypress="javascript:return CheckKeyValue(event,'34,32');" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="<%=m_refMsg.GetMessage("lbl editor")%>"><%=m_refMsg.GetMessage("lbl editor")%>:</td>
                                    <td class="value"><asp:DropDownList ToolTip="Select Content and Forum Editor from Drop Down List" ID="drp_editor" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="<%=m_refMsg.GetMessage("lbl avatar")%>"><%=m_refMsg.GetMessage("lbl avatar")%>:</td>
                                    <td class="value"><input title="Enter address of Avatar" type="text" id="avatar" name="avatar" maxlength="255" size="19" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" />
									<ektronUI:Dialog ID="uxDialog" CssClass="EktronAvatarUploadUI" Width="400" Height="300" Modal="true" Title="Avatar Upload" runat="server">
                                        <ContentTemplate>
                                            <iframe class="uxAvatarUploadIframe xUploadUIControls" frameborder="0" border="0"  ID="uxAvatarUploadIframe" scrolling="no" runat="server" height="100%" width="100%"></iframe>	
                                        </ContentTemplate>
                                    </ektronUI:Dialog>
                                    <asp:Literal ID="ltr_upload" runat="server"/></td>
                                </tr>
                                <tr>
                                    <td class="label" title="<%=m_refMsg.GetMessage("lbl user add address")%>"><%=m_refMsg.GetMessage("lbl user add address")%>:</td>
                                    <td class="value"><input title="Address Text" type="text" id="mapaddress" name="mapadderss" maxlength="100" size="19" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="<%=m_refMsg.GetMessage("lbl user add latitude")%>"><%=m_refMsg.GetMessage("lbl user add latitude")%>:</td>
                                    <td class="value"><input title="Latitude Text" type="text" disabled="disabled" id="maplatitude" name="maplatitude" maxlength="100" size="19" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="<%=m_refMsg.GetMessage("lbl user add longitude")%>"><%=m_refMsg.GetMessage("lbl user add longitude")%>:</td>
                                    <td class="value"><input title="Longitude Text" type="text" disabled="disabled" id="maplongitude" name="maplongitude" maxlength="100" size="19" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server" /></td>
                                </tr>               
                            </tbody>
                        </table>
                    </div>
                    <div id="CustomProperties">
                        <table class="ektronGrid">
                            <tbody>
                                <% if(m_intGroupType == 0){ %>
                                <tr>
                                    <td class="label">
                                        <asp:Literal ID="litDisableMessage" runat="server" />
                                    </td>
                                    <td>
                                        <asp:CheckBox ToolTip="Disable Message" ID="disable_msg" runat="server" />
                                        <span style="color:red;"><asp:Literal ID="msg" runat="server" /></span>
                                    </td>
                                </tr>
                                <% } %>
                                <asp:Literal ID="litUCPUI" runat="server" />
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="TR_AddLDAPDetail" runat="server">
        <div class="ektronPageInfo">
            <table class="ektronGrid">
                <tr>
                    <td class="label" title="Username"><%=m_refMsg.GetMessage("username label")%></td>
                    <td class="value"><input title="Username Text" type="text" id="LDAP_username" name="username" maxlength="255" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label" title="Path"><%=m_refMsg.GetMessage("generic path")%>:</td>
                    <td class="value"><input title="Path Text" type="text" id="LDAP_ldapdomain" name="LDAP_ldapdomain" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label" title="First Name"><%=m_refMsg.GetMessage("first name label")%></td>
                    <td class="value"><input title="First Name Text" type="text" id="LDAP_firstname" name="firstname" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label" title="Last  Name"><%=m_refMsg.GetMessage("last name label")%></td>
                    <td class="value"><input title="Last  Name Text" type="text" id="LDAP_lastname" name="lastname" maxlength="50" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label" title="Display Name"><%=m_refMsg.GetMessage("display name label")%>:</td>
                    <td class="value"><input title="Display Name Text" type="text" id="LDAP_displayname" name="displayname" maxlength="55" size="25" onkeypress="javascript:return CheckKeyValue(event,'34');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label" title="User Language"><%=m_refMsg.GetMessage("user language label")%></td>
                    <td class="value"><asp:DropDownList ToolTip="Select User Language from Drop Down List" ID="LDAP_language" runat="server" /></td>
                </tr>
                <tr>
                    <td class="label" title="E-Mail address"><%=m_refMsg.GetMessage("email address label")%></td>
                    <td class="value"><input title="E-Mail address Text" type="text" maxlength="255" size="25" id="LDAP_email_addr1" name="email_addr1" onkeypress="javascript:return CheckKeyValue(event,'34,32');" runat="server"/></td>
                </tr>
                <tr>
                    <td class="label" title="Content and Forum Editor"><%=m_refMsg.GetMessage("lbl editor")%>:</td>
                    <td class="value"><asp:DropDownList ToolTip="Select Content and Forum Editor From Drop Down List" ID="drp_LDAPeditor" runat="server" /></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:CheckBox ToolTip="Disable E-Mail Notifications" ID="LDAP_disable_msg" runat="server" /><br/>
                        <asp:Literal ID="LDAP_msg" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Literal ID="LDAP_litUCPUI" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="TR_AddUserList" runat="server">
        <div class="ektronPageGrid">
            <asp:DataGrid ID="AddUserGrid"
                runat="server"
                AutoGenerateColumns="False"
                Width="100%"
                EnableViewState="False"
                GridLines="None">
                <HeaderStyle CssClass="title-header" />
            </asp:DataGrid>
        </div>
    </div>

    <input type="hidden" name="netscape" onkeypress="javascript:return CheckKeyValue(event,'34');" />
    <input type="hidden" id="addusercount" name="addusercount" value="0" runat="server" />
</div>
