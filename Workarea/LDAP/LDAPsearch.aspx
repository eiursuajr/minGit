<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LDAPsearch.aspx.cs" Inherits="LDAP_LDAPsearch" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Search Active Directory for Users</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:Literal ID="ltr_js" runat="server"/>
        
        <div style="visibility: hidden; z-index: 2; width: 100%; position: absolute; top: 140px;
            height: 1px; background-color: white" id="dvHoldMessage" align="center">
            <table style="height: 1px" border="2" bgcolor="white" width="80%" id="Table5">
                <tr>
                    <td valign="top" nowrap align="center">
                        <h3 style="color: red">
                            <strong>
                                One Moment Please....</strong></h3>
                        <img alt="" align="top" style="width: 95%; height: 8px" src="../images/application/loading.gif"
                            ondragstart="return false;" /></td>
                </tr>
            </table>
            
        </div>
        
        <div class="ektronPageContainer ektronPageGrid">
            <div class="ektronPageGrid">
            
                <table width="100%" class="ektronGrid">
                <tr class="title-header">
			        <td title="Username">Username</td><td class="title-header" title="Firstname">Firstname</td><td class="title-header" title="Lastname">Lastname</td><td class="title-header" title="Path">Path</td>
		        </tr>
		        <tr>
			        <td style="white-space:nowrap;"><input title="Enter Username here" type="Text" name="username" maxlength="255" class="ektronTextXSmall" onkeypress="javascript:return CheckKeyValue(event,'34');" id="username" runat="server"/></td><td style="white-space:nowrap;"><input title="Enter First Name here" type="Text" name="firstname" maxlength="50" class="ektronTextXSmall" onkeypress="javascript:return CheckKeyValue(event,'34');" id="firstname" runat="server"/></td><td style="white-space:nowrap;"><input title="Enter Last Name here" type="Text" name="lastname" maxlength="50" class="ektronTextXSmall" onkeypress="javascript:return CheckKeyValue(event,'34');" id="lastname" runat="server"/><input type="hidden" id="uid" name="uid" value=""/> <input type="hidden" id="rp" name="rp" value=""/><input type="hidden" id="ep" name="e1" value=""/> <input type="hidden" id="e2" name="e2" value=""/><input type="hidden" id="f" name="f" value=""/></td><td style="white-space:nowrap;">
			        <asp:DropDownList ToolTip="Paths Drop Down Menu" ID="paths" runat="server"/>
                    </td>
		        </tr>
		        <tr>
			        <td style="white-space:nowrap;">
			            <asp:Button ToolTip="Search" runat="server" ID="cmsSearch" Text="Search"  OnClick="cmsSearch_Click"/>
			        </td>
			        <td style="white-space:nowrap;">&nbsp;</td><td style="white-space:nowrap;">&nbsp;</td><td style="white-space:nowrap;">&nbsp;</td>
		        </tr>
		        </table> 
        		
        	    <br />&nbsp;<br />
        		
		        <div class="ektronBorder">
                    <asp:DataGrid ID="AddUserGrid" 
                        runat="server" 
                        AllowPaging="true" 
                        AutoGenerateColumns="false"
                        Width="100%" 
                        EnableViewState="True"
                        OnPageIndexChanged="PageChange"
                        GridLines="None"
                        CssClass="ektronGrid">
                        <HeaderStyle CssClass="title-header" />
                        <PagerStyle Mode="NumericPages" />
                        <Columns> 
                            <asp:BoundColumn />
                        </Columns>
                    </asp:DataGrid>
                    <asp:Literal ID="ltr_message" runat="server"/>
                    <asp:LinkButton ToolTip="First Page" runat="server" CssClass="pageLinks" ID="FirstPage" Text="[First Page]" Visible="false"
                                OnCommand="NavigationLink_Click" CommandName="First" />
                    <asp:LinkButton ToolTip="Last Page" runat="server" CssClass="pageLinks" ID="LastPage" Text="[Last Page]" Visible="false"
                                OnCommand="NavigationLink_Click" CommandName="Last" />
                  
                    <input type="hidden" name="netscape" onkeypress="javascript:return CheckKeyValue(event,'34');"/>
                    <input type="hidden" id="addusercount" name="addusercount" value="0" runat="server"/>
                </div>            
            </div>
        </div>
        
    </form>
</body>
</html>

