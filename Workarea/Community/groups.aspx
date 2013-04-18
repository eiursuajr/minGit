<%@ Page Language="C#" AutoEventWireup="true" CodeFile="groups.aspx.cs" Inherits="Community_groups" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Groups</title>
        <script type="text/javascript" src="../java/jfunct.js">
</script>
        <script type="text/javascript" src="../java/toolbar_roll.js">
</script>
        <script type="text/javascript">
            function ClearErr(){
	            document.getElementById ('errmsg').innerHTML='';
	        }
        </script>
        <script type="text/javascript">
            Ektron.ready( function()
                {
                    var tabsContainers = $ektron(".tabContainer");
                    tabsContainers.tabs();
                }
            );
        </script>
    </head>
    <body id="body" runat="server">
        <form id="frmContent" runat="server">
            <div class="ektronPageContainer">
            <asp:Panel ID="panel1" CssClass="ektronPageGrid" runat="server" Visible="false">
                <asp:Literal ID="ltr_allgroups" runat="Server" />
                <asp:GridView ID="CommunityGroupList"
                    runat="server"
                    AutoGenerateColumns="False"
                    Width="100%"
                    EnableViewState="False"
                    CssClass="ektronGrid"
                    GridLines="None">
                    <HeaderStyle CssClass="title-header" />
                </asp:GridView>
                <p class="pageLinks">
                    <asp:Label ToolTip="Page" runat="server" ID="TPageLabel">Page</asp:Label>
                    <asp:Label ID="TCurrentPage" CssClass="pageLinks" runat="server" />
                    <asp:Label ToolTip="of" runat="server" ID="TOfLabel">of</asp:Label>
                    <asp:Label ID="TTotalPages" CssClass="pageLinks" runat="server" />
                </p>
                <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="TFirstPage" Text="[First Page]"
                    OnCommand="TNavigationLink_Click" CommandName="First" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Previous Page" runat="server" CssClass="pageLinks" ID="TPreviousPage" Text="[Previous Page]"
                    OnCommand="TNavigationLink_Click" CommandName="Prev" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Next Page" runat="server" CssClass="pageLinks" ID="TNextPage" Text="[Next Page]"
                    OnCommand="TNavigationLink_Click" CommandName="Next" OnClientClick="resetPostback()" />
                <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="TLastPage" Text="[Last Page]"
                    OnCommand="TNavigationLink_Click" CommandName="Last" OnClientClick="resetPostback()" />
                <input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
            </asp:Panel>
            <asp:Panel CssClass="ektronPageTabbed" ID="panel3" runat="server" Visible="false">
                <div class="tabContainerWrapper">
                    <div class="tabContainer">
                        <ul>
                            <li>
                                <a title="Properties Tab" href="#dvProperties">
                                     <asp:Label ID="lblProperties" runat="server" />
                                </a>
                            </li>
                            <li>
                                <a title="Tags Tab" href="#dvTags">
                                   <asp:Label ID="lblTags" runat="server" />
                                </a>
                            </li>
                            <asp:PlaceHolder ID="phCategoryTab" runat="server">
                                <li>
                                    <a title="Category Tab" href="#dvCategory">
                                     <asp:Label ID="lblCategory" runat="server" />
                                    </a>
                                </li>
                            </asp:PlaceHolder>
                             <asp:PlaceHolder ID="phAliasTab" runat="server">
                                <li>
                                 <a title="Links Tab" href="#dvAlias">
                                    Links
                                 </a>
                                </li>
                             </asp:PlaceHolder>
                        </ul>

                        <div id="dvProperties">
                            <span id="errmsg" runat="server" />
                            <table class="ektronForm">
                                <tr>
                                    <td class="label" title="Group Name"><asp:Literal ID="ltr_groupname" runat="server" />:</td>
                                    <td class="value"><asp:TextBox ToolTip="Enter Group Name here" ID="GroupName_TB" runat="server" /></td>
                                </tr>
                                <tr runat="server" id="tr_ID">
                                    <td class="label" title="ID"><asp:Literal ID="ltr_groupid" runat="server" />:</td>
                                    <td class="value"><asp:Label ID="lbl_id" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Administrator"><asp:Literal ID="ltr_admin" runat="server" />:</td>
                                    <td class="value" title="<%=ltr_admin_name.Text %>"><asp:Literal ID="ltr_admin_name" runat="Server" />&nbsp;&nbsp;<asp:Button ToolTip="Browse" ID="cmd_browse" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Membership"><asp:Literal ID="ltr_groupjoin" runat="server" />:</td>
                                    <td class="value">
                                        <asp:RadioButton ToolTip="Open Membership" ID="PublicJoinYes_RB" runat="server" GroupName="PublicJoin" Text="Yes" />&nbsp;&nbsp;
                                        <asp:RadioButton ToolTip="Restricted Membership" ID="PublicJoinNo_RB" runat="server" GroupName="PublicJoin" Text="No" />&nbsp;&nbsp;
                                        <asp:RadioButton ToolTip="Restricted Membership" ID="PublicJoinHidden_RB" runat="server" GroupName="PublicJoin" />
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="white-space: nowrap;" title="Featrures">
                                        <asp:Literal ID="ltr_groupfeatures" runat="server"></asp:Literal>
                                    </td>
                                    <td>
                                        <asp:CheckBox ToolTip="Option to Create Group Calendar" ID="FeaturesCalendar_CB" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:CheckBox ToolTip="Option to Create Group Forum" ID="FeaturesForum_CB" runat="server" />
                                    </td>
                                </tr>
                                 <tr>
                                    <td></td>
                                    <td>
                                        <asp:CheckBox ToolTip="Option to Create Group Todo List" ID="FeaturesTodo_CB" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" title="Image"><asp:Literal ID="ltr_groupavatar" runat="server" />:</td>
                                    <td class="value" title="Enter Image path here"><asp:Literal ID="ltr_avatarpath" runat="server" /><asp:TextBox ID="GroupAvatar_TB" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Location"><asp:Literal ID="ltr_grouplocation" runat="server" />:</td>
                                    <td class="value" title="Enter Location here"><asp:TextBox ID="Location_TB" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Short Description"><asp:Literal ID="ltr_groupsdesc" runat="server" />:</td>
                                    <td class="value" title="Enter Short Description here"><asp:TextBox ID="ShortDescription_TB" runat="server" /></td>
                                </tr>
                                <tr>
                                    <td class="label" title="Description"><asp:Literal ID="ltr_groupdesc" runat="server" />:</td>
                                    <td class="value" title="Enter Description here"><asp:TextBox ID="Description_TB" runat="server" Rows="6" TextMode="MultiLine" /></td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td>
                                        <div runat="server" id="tr_EnableDistribute">
                                            <asp:CheckBox ToolTip="Enable/Disable Distribution" ID="EnableDistributeToSite_CB" runat="server" /><asp:Literal ID="ltr_enabledistribute" runat="server" />
                                        </div>
                                        <div runat="server" id="tr_AllowMembersToManageFolders">
                                            <asp:CheckBox ToolTip="Option to Allow Members to Manage Folders" ID="AllowMembersToManageFolders_CB" runat="server" /><asp:Literal ID="ltr_AllowMembersToManageFolders" runat="server" />
                                        </div>
                                        <div runat="server" id="tr_MessageBoardModeration">
                                            <asp:CheckBox ToolTip="Option for Message Board Moderation" ID="chkMsgBoardModeration" runat="server" Enabled="false"/><asp:Literal ID="ltr_MsgBoardModeration" runat="server" />
                                        </div>
                                          <div runat="server" id="tr_EnableDocumentNotifications">
                                            <asp:CheckBox ToolTip="Attach Documents in Email Notifications" ID="chkEnableDocumentNotifications" runat="server"  Enabled="false"/><asp:Literal ID="ltrlEnableDocumentNotifications" Text="Attach Documents in Email Notifications" runat="server" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td><asp:Literal ID="ltr_Emaildesc" runat="server" Text="Group Email" />:</td>
                                    <td>
                                        <asp:CheckBox ToolTip="Enable Group Emails" ID="chkEnableEmail" Enabled="false" runat="server" /><asp:Literal ID="Literal1" Text="Enable Group Emails" runat="server" />
                                        <table id="ucEktronGroupEmailSetting" runat="server" class="EktronGroupEmailSetting" Visible="false">
                                            <tbody>
                                                <tr class="EktronGroupEmailSetting">
                                                    <th>
                                                        <asp:Label ID="lblEmailAddress" runat="server" Text="Email Address" />
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="lblEmailAddressValue" runat="server" Text="" />
                                                    </td>
                                                </tr>
                                                <tr class="EktronGroupEmailSetting">
                                                    <th>
                                                        <asp:Label ID="lblEmailAccount" runat="server" Text="Email Account Name" />
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="lblEmailAccountValue" runat="server" Text="" />
                                                    </td>
                                                </tr>
                                                <tr class="EktronGroupEmailSetting">
                                                    <th>
                                                        <asp:Label ID="lblEmailReplyPassword" runat="server" Text="Email Account Password" />
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="lblEmailReplyPasswordValue" runat="server" Text="*******" />
                                                    </td>
                                                </tr>
						           <tr class="EktronGroupEmailSetting">
                                                    <th>
                                                        <asp:Label ID="lblEmailServer" runat="server" Text="Email Server" />
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="lblEmailServerValue" runat="server" Text="" />
                                                    </td>
                                                </tr>
                                                <tr class="EktronGroupEmailSetting">
                                                    <th>
                                                        <asp:Label ID="lblEmailServerPort" runat="server" Text="Email Server Port" />
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="lblEmailServerPortValue" runat="server" Text="" />
                                                    </td>
                                                </tr>
                                                <tr class="EktronGroupEmailSetting">
                                                    <th>
                                                        <asp:Label ID="lblUseSsl" runat="server" Text="Use SSL" />
                                                    </th>
                                                    <td>
                                                        <asp:CheckBox ID="chkUseSsl" class="chkUseSsl" runat="server" Enabled="false"/>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                  
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="dvTags">
                            <div id="TD_personalTags" runat="server"></div>
                        </div>
                        <div id="dvCategory">
                            <div id="TD1" runat="server"><asp:Literal ID="ltr_cat" runat="server" /></div>
                        </div>
                        <asp:PlaceHolder ID="phAliasFrame" runat="server">
                           <div id="dvAlias">
                                <p style="width: auto; height: auto; overflow: auto;" class="groupAliasList" title="<%=groupAliasList%>" ><%=groupAliasList%></p>
                            </div>
                       </asp:PlaceHolder> 
                    </div>
                </div>
            </asp:Panel>
            <div>
                <input type="hidden" id="hdn_search" name="hdn_search" value="" />
                <asp:Literal ID="ltr_js" runat="server" />
            </div>
            </div>
        </form>
    </body>
</html>

