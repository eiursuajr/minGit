<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserGroupPreferences.ascx.cs"
    Inherits="Workarea_Notifications_UserGroupPreferences" %>
     <style type="text/css">
	    .exception
	    {
	        background-color:#FBE3E4;
	        border: 1px solid #FBC2C4;
	        color: #D12F19;
	        display: block;
	        margin: 0.25em;
	        padding: 0;
	        background-image: url('../images/ui/icons/error.png');
	        background-repeat: no-repeat;
	        background-position: .25em .25em
	    }

	    .exception {padding: .25em 0 .25em 1.75em;}
	</style>
 <div class="ektronPageContainer ektronPageInfo">
            <div id="agentDisabled" class="exception" runat="server">
                Turn on the agents before setting up the default preferences
            </div>
            <div style="color:#235478;">
            <center>Notify me about these activities for this Community Group</center>
            </div>
            <asp:GridView ID="UserGroupPrefGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                CssClass="ektronGrid"  GridLines="None" AlternatingRowStyle-BackColor="#D5E7F5"  BorderWidth="1px" Font-Size="Small"   BorderColor="#D5E7F5">
                <HeaderStyle  BackColor="#d5e7f4" ForeColor="#235478"  CssClass="title-header" />
            </asp:GridView>
        </div>



