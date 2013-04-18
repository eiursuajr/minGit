<%@ Control Language="C#" AutoEventWireup="true" CodeFile="uxLogin.ascx.cs" Inherits="SiteLoginPanel" %>
<%@ Register Assembly="Ektron.Cms.Controls" Namespace="Ektron.Cms.Controls" TagPrefix="CMS" %>
    <div id="loginContainer" class="userLogin">
        <asp:Panel ID="LoginRequestPanel"  Visible="false" runat="server" >
            <asp:Login runat="server" ID="loginControl" MembershipProvider="EktronMembershipProvider" DisplayRememberMe="true" CssClass="loginControl">
                <LayoutTemplate>
                    <div id="loginPanel">
                        <p class="intro"><asp:Literal ID="introText" runat="server" /></p>
                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False" />
                        <ul>
                            <li class="ui-helper-clearfix">
                                <asp:Label ToolTip="Username" ID="UserNameLabel" runat="server" AssociatedControlID="UserName" CssClass="label" />
                                <div>
                                    <asp:TextBox ToolTip="Username" ID="UserName" runat="server" autocomplete="off" CssClass="inputBox inputUsername" />
                                </div>
                            </li>
                            <li class="ui-helper-clearfix">
                                <asp:Label ToolTip="Password" ID="PasswordLabel" runat="server" AssociatedControlID="Password" CssClass="label" />
                                <div>
                                    <asp:TextBox ToolTip="Password" ID="Password" runat="server" autocomplete="off" TextMode="Password" CssClass="inputBox inputPassword"></asp:TextBox>
                                </div>
                            </li>
                        </ul>
                        <p class="ui-helper-clearfix">
                            <asp:Button ToolTip="Login" ID="LoginButton" runat="server" CommandName="Login" Text="Login" CssClass="ui-state-default ui-corner-all inputButton inputLoginButton" ValidationGroup="userLogin" OnClientClick="return Ektron.UX.Login.Validate();" />
                        </p>                        
                    </div>
                </LayoutTemplate>
            </asp:Login>
        </asp:Panel>
        <input type="hidden" id="test" name="test" class="loginAttempt" runat="server" />
        
        <asp:Panel ID="LoginSuceededPanel" runat="server" Visible="False" >
            <p style="margin-top: 50px; margin-bottom: 50px; font-weight: bold; font-size: 11px; color: #333333; font-family: tahoma; text-align: center; vertical-align: middle;">
                You are now logged in.
            </p>
            <p style="text-align: center; vertical-align: middle;">
                <input type="button" title="Continue" CssClass="ui-state-default ui-corner-all inputButton" id="btnContinue" runat="server" value="Continue" onclick="cl();" />
            </p>
        </asp:Panel>
    </div>
