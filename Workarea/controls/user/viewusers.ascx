<%@ Control Language="C#" AutoEventWireup="true" Inherits="viewusers" CodeFile="viewusers.ascx.cs" %>
<%@ Register TagPrefix="uxEktron" TagName="Paging" Src="../paging/paging.ascx" %>
<%@ Register TagPrefix="uxEktronGrid" TagName="Grid" Src="../generic/grid/grid.ascx" %>

<script type="text/javascript">
    function NavWorkspaceTree(elem, path)
    {
        if(typeof(top != 'undefined')){
			if(typeof(top.MakeNavTreeVisible!= 'undefined')){
				top.TreeNavigation('WorkSpaceTree', path)
			}
		}
    }
    function checkAll(ControlName){
	    if(ControlName!=''){
		    var iChecked=0;
		    var iNotChecked=0;
		    for (var i=0;i<document.forms[0].elements.length;i++){
			    var e = document.forms[0].elements[i];
			    if (e.name=='req_deleted_users'){
				    if(e.checked){iChecked+=1;}
				    else{iNotChecked+=1;}
			    }
		    }
		    if(iNotChecked>0){document.forms[0].checkall.checked=false;}
		    else{document.forms[0].checkall.checked=true;}
	    }
	    else{
		    for (var i=0;i<document.forms[0].elements.length;i++){
			    var e = document.forms[0].elements[i];
			    if (e.name=='req_deleted_users'){
				    e.checked=document.forms[0].checkall.checked
			    }
		    }
	    }
    }
    function ActivateUsers(){
        var userChecked=false;
        for (var i=0;i<document.forms[0].elements.length;i++){
		    var e = document.forms[0].elements[i];
		    if (e.name=='req_deleted_users' && e.checked){
			    userChecked=true;break;
		    }
	    }
	    if(!userChecked){
	    alert('<%=m_refMsg.GetMessage("js:select user for membership activation")%>');
	    return false;
	    }
	    document.forms[0].user_isSearchPostData.value = "";
	    document.forms[0].submit();
	    return true;
    }
    function DeleteSelectedUsers(){
        var userChecked=false;
        for (var i=0;i<document.forms[0].elements.length;i++){
	        var e = document.forms[0].elements[i];
	        if (e.name=='req_deleted_users' && e.checked){
		        userChecked=true;break;
	        }
        }
        if(!userChecked){
        alert('<%=m_refMsg.GetMessage("alt select one or more user(s) then click delete button.")%>');
        return false;
        }
        if(confirm("<%= m_refMsg.GetMessage("js: confirm delete users") %>")){
	        document.forms[0].user_isDeleted.value = "1";
	        document.forms[0].user_isSearchPostData.value = "";
            document.forms[0].submit();
            return true;
	    }else{
	        return false;
	    }
    }
	function resetPostback()
	{
	    document.forms[0].user_isPostData.value = "";
	}
	function searchuser(){
	    if(document.forms[0].txtSearch.value.indexOf('\"')!=-1){
	        alert('remove all quote(s) then click search');
	        return false;
	    }
	    document.forms[0].user_isSearchPostData.value = "1";
	    document.forms[0].user_isPostData.value="true";
	    $ektron("#txtSearch").clearInputLabel();
	    document.forms[0].submit();
	    return true;
	}

	function CheckForReturn(e)
	{
	    var keynum;
        var keychar;

        if(window.event) // IE
        {
            keynum = e.keyCode
        }
        else if(e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which
        }

        if( keynum == 13 ) {
            document.getElementById('btnSearch').focus();
        }
	}

	function IsGroupPartOfSubscriptionProduct()
	{

	    return <asp:literal runat="server" id="ltr_groupsubscription" />;

	}
	function ShowGrid(mode)
    {

         var obj = document.getElementById('dvCollGrid');
         var obj1 = document.getElementById('dvGroupGrid');
         var obj2 = document.getElementById('dvPrivacyGrid');

         if(mode=='Coll')
         {
            if (obj)
            {
              obj.style.display = 'block';
              obj1.style.display ='none';
              obj2.style.display = 'none';

              document.getElementById('dvColleaguetab').className='SelectedTab';
              document.getElementById('dvGrouptab').className='UnSelectedTab';
              document.getElementById('dvPrivacytab').className='UnSelectedTab';
            }
          }
         else if(mode=='Privacy')
         {
          obj.style.display = 'none';
          obj1.style.display = 'none';
          obj2.style.display = 'block';
          document.getElementById('dvColleaguetab').className='UnSelectedTab';
          document.getElementById('dvGrouptab').className='UnSelectedTab';
          document.getElementById('dvPrivacytab').className='SelectedTab';

         }
         else
         {
          obj.style.display = 'none';
          obj1.style.display = 'block';
          obj2.style.display = 'none';
          document.getElementById('dvColleaguetab').className='UnSelectedTab';
          document.getElementById('dvGrouptab').className='SelectedTab';
          document.getElementById('dvPrivacytab').className='UnSelectedTab';

         }
     }
    Ektron.ready(function(){
        $ektron("div#dvCustom input[type='text']").css({ width: "100px" });
    })
</script>
<style type="text/css">
    .Menu 
    {
        width: inherit;
    }
</style>
<asp:Literal ID="PostBackPage" runat="server" />
<div style="display: none; border: 1px; background-color: white; position: absolute;
    top: 48px; width: 100%; height: 1px" id="dvHoldMessage" class="center">
    <table border="1px" bgcolor="white" width="100%">
        <tr>
            <td valign="top" nowrap class="center">
                <h3 style="color: red">
                    <strong>
                        <%=m_refMsg.GetMessage("one moment msg")%>
                    </strong>
                </h3>
            </td>
        </tr>
    </table>
</div>
<div id="dhtmltooltip">
</div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server">
    </div>
    <div class="ektronToolbar" id="htmToolBar" runat="server">
    </div>
</div>
<div class="ektronPageContainer ektronPageInfo">
    <asp:DataGrid ID="MapCMSUserToADGrid" runat="server" AutoGenerateColumns="False"
        GridLines="none" Width="100%" AllowCustomPaging="True" PageSize="10" PagerStyle-Visible="False"
        CssClass="ektronGrid" EnableViewState="False">
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
    <asp:Literal ID="ltr_message" runat="server" />
    <uxEktron:Paging ID="uxPaging" runat="server" />
    <div class="tabContainerWrapper">
        <div class="tabContainer" id="viewUser" runat="server" visible="false">
            <ul>
                <li><a title="General" href="#dvGeneral"><%=m_refMsg.GetMessage("general label")%></a></li>
                <li><a title="User Groups" href="#dvUserGroups"><%=m_refMsg.GetMessage("generic user groups")%></a></li>
                <asp:PlaceHolder ID="workareaTab" runat="server">
                <li><a title="WorkPage" href="#dvWorkpage"><%=m_refMsg.GetMessage("workarea options label")%></a></li>
                </asp:PlaceHolder>
                <li><a title="Custom" href="#dvCustom"><%=m_refMsg.GetMessage("lbl custom")%></a></li>
                <li id="activitiesTab" runat="server"><a title="Activities" href="#dvActivities"><%=m_refMsg.GetMessage("lbl activities")%></a></li>     
                <li id="aliasTab" runat="server"><a title="Profile Links" href="#dvAlias"><%=m_refMsg.GetMessage("lbl profile links")%></a></li>
              </ul>
            <div id="dvGeneral">
                <asp:DataGrid ID="FormGrid" runat="server" AutoGenerateColumns="False" CssClass="ektronGrid" ShowHeader="false" />
            </div>
            <div id="dvUserGroups">
                <p><%=m_refMsg.GetMessage("user belongs to msg")%></p>
                <uxEktronGrid:Grid ID="uxUserGroupsGrid" runat="server" />
                <asp:Repeater ID="rptUserGroups" runat="server">
                    <HeaderTemplate>
                        <ul class="userGroups">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li><% if(security_data.IsAdmin){ %>
                        <a href="users.aspx?action=viewallusers&groupid=<%# DataBinder.Eval(Container.DataItem, "GroupId")%>&grouptype=<%= m_intGroupType %>&LangType=<%= ContentLanguage %>&id=<%# DataBinder.Eval(Container.DataItem, "GroupId")%>" title="<%# DataBinder.Eval(Container.DataItem, "DisplayGroupName")%>"><%# DataBinder.Eval(Container.DataItem, "DisplayGroupName")%></a>
                        <% } else {%>
                        <%# DataBinder.Eval(Container.DataItem, "DisplayGroupName")%>
                        <% }%></li>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <li class="stripe"><% if(security_data.IsAdmin){%>
                        <a href="users.aspx?action=viewallusers&groupid=<%# DataBinder.Eval(Container.DataItem, "GroupId")%>&grouptype=<%= m_intGroupType %>&LangType=<%= ContentLanguage %>&id=<%# DataBinder.Eval(Container.DataItem, "GroupId")%>" title="<%# DataBinder.Eval(Container.DataItem, "DisplayGroupName")%>"><%# DataBinder.Eval(Container.DataItem, "DisplayGroupName")%></a>
                        <% } else {%>
                        <%# DataBinder.Eval(Container.DataItem, "DisplayGroupName")%>
                        <%} %></li>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <asp:PlaceHolder ID="workareaDiv" runat="server">
            <div id="dvWorkpage">
                <asp:DataGrid ID="WorkPage" runat="server" AutoGenerateColumns="False" ShowHeader="false" CssClass="ektronGrid" />
            </div>
            </asp:PlaceHolder>
            <div id="dvCustom">
                <%--<asp:DataGrid ID="CustomProperties"
                    runat="server"
                    CssClass="ektronForm"
                    AutoGenerateColumns="False"
                    GridLines="none"/>--%>
                <table class="ektronGrid">
                    <asp:Literal runat="server" ID="ltr_CustomProperty" />
                </table>
            </div>
            <div id="dvActivities" class="EkMembershipActivityTab">
                <table id="EkMembershipActivityTable" cellspacing="0" runat="server">
                    <tr>
                        <td class="subTabsWrapper">
                            <ul class="activities_tab_subTabs">
                                <li id="dvColleaguetab" class="SelectedTab"><a title="Friends" href="#" onclick="ShowGrid('Coll');return false;">
                                    <%=m_refMsg.GetMessage("lbl friends")%></a></li>
                                <li id="dvGrouptab" class="UnSelectedTab"><a title="Groups" href="#" onclick="ShowGrid('Group');return false;">
                                     <%=m_refMsg.GetMessage("lbl groups")%></a></li>
                                <li id="dvPrivacytab" class="UnSelectedTab"><a title="Privacy" href="#" onclick="ShowGrid('Privacy');return false;">
                                     <%=m_refMsg.GetMessage("lbl privacy")%></a></li>
                            </ul>
                        </td>
                        <td>
                            <div class="dvCollGrid" id="dvCollGrid">
                            <span class="EkActivityNotifyText" title="Notify me about these Colleague activities"> <%=m_refMsg.GetMessage("lbl notify colleagues activities")%></span>
                                <asp:GridView ID="CollGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                                    CssClass="ektronGrid" GridLines="None">
                                    <HeaderStyle CssClass="title-header" />
                                </asp:GridView>
                            </div>
                        </td>
						<td>
                            <div class="dvGroupGrid" id="dvGroupGrid" style="display: none;">
                             <span class="EkActivityNotifyText" title="Notify me about these Community Group activities"><%=m_refMsg.GetMessage("lbl notify groups activities")%></span>
                                <asp:GridView ID="GroupGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                                    CssClass="ektronGrid" GridLines="None">
                                    <HeaderStyle CssClass="title-header" />
                                </asp:GridView>
                            </div>
                        </td>
                        <td>
                            <div class="dvPrivacyGrid" id="dvPrivacyGrid" style="display:none;">
                             <span class="EkActivityNotifyText" title="Notify others about my activities"> <%=m_refMsg.GetMessage("lbl notify my activities")%></span>
                                  <asp:GridView ID="PrivacyGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                                       CssClass="ektronGrid" GridLines="None">
                                       <HeaderStyle CssClass="title-header" />
                                 </asp:GridView>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="dvAlias">
               <table class="ektronGrid" id="tblAliasList" runat="server">
                   <tr>
                       <td >  
                          <p style="width: auto; height: auto; overflow: auto;" class="groupAliasList" ><%=groupAliasList%></p>
                       </td>
                   </tr>
               </table>
            </div>
        </div>
    </div>
</div>
<input type="hidden" runat="server" id="isPostData" value="true" name="isPostData" />
<input type="hidden" runat="server" id="isDeleted" value="" name="isDeleted" />
<input type="hidden" runat="server" id="isSearchPostData" value="" name="isSearchPostData" />
<input type="hidden" name="groupID" id="groupID" value="<%=uId%>" />
