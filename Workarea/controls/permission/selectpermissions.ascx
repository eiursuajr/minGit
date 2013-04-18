<%@ Control Language="C#" AutoEventWireup="true" Inherits="selectpermissions" CodeFile="selectpermissions.ascx.cs" %>
<%@ Register TagPrefix="ucEktron" TagName="Paging" Src="../paging/paging.ascx" %>
<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <div class="ektronTitlebar" id="txtTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="htmToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageInfo permissions">
    <script type="text/javascript">
        //define Ektron objects only if they are not already defined
        if (Ektron === undefined) {Ektron = {};}
        if (Ektron.Workarea === undefined) {Ektron.Workarea = {};}
        if (Ektron.Workarea.Permissions === undefined) {Ektron.Workarea.Permissions = {};}
        
        //create add object
        Ektron.Workarea.Permissions.Add = {
            //properties
            approvedUsersAndGroups: new Array(),
            noItemSelected: '<asp:literal id="litNoItemSelected" runat="server" />',
            
            //methods
            bindEvents: function(){
                $ektron("div.permissions table.ektronGrid tr[class!='title-header'] :checkbox").click(function(){
                    Ektron.Workarea.Permissions.Add.Data.update(this);
                });
            },
            checkApprovedItems: function(){
                $ektron("div.permissions table.ektronGrid tr[class!='title-header'] :checkbox").each(function(){
                    for(i=0; i<Ektron.Workarea.Permissions.Add.approvedUsersAndGroups.length; i++){
                        //get id values
                        var id = $ektron(this).next().val();
                        var isGroup = $ektron(this).next().next().val();
                        
                        //if item is approved, check checkbox
                        if ((Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].id == id) && (Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].isGroup == isGroup)) {
                            $ektron(this).attr("checked", true);
                        }
                    }
                });
            },
            checkAll: function(){
                //if all checkboxes are checked, or all checkboxes are unchecked, toggle checkall checkbox
                var checkboxes = $ektron("div.permissions table.ektronGrid tr[class!='title-header'] :checkbox");
                var checkedCheckboxes = $ektron("div.permissions table.ektronGrid tr[class!='title-header'] :checked");
                if (checkboxes.length == checkedCheckboxes.length) {
                    //all are checked, check checkall checkbox
                    $ektron("div.permissions table.ektronGrid tr[class='title-header'] :checkbox").attr("checked", true);
                } else {
                    //at least one is unchecked, uncheck checkall checkbox
                    $ektron("div.permissions table.ektronGrid tr[class='title-header'] :checkbox").attr("checked", false);
                }
                if (checkedCheckboxes.length == 0) {
                    //all are not checked, check checkall checkbox
                    $ektron("div.permissions table.ektronGrid tr[class='title-header'] :checkbox").attr("checked", false);
                }
            },
            Data: {
                init: function(){
                    //get array of approved items from hidden field
                    var approvedItems = $ektron("div.permissions input.approvedUsersAndGroups").attr("value");
                    if ("undefined" != typeof (approvedItems) && approvedItems != "") {
                        Ektron.Workarea.Permissions.Add.approvedUsersAndGroups = Ektron.JSON.parse(approvedItems);
                        Ektron.Workarea.Permissions.Add.checkApprovedItems();
                        Ektron.Workarea.Permissions.Add.checkAll();
                    } else {
                        Ektron.Workarea.Permissions.Add.approvedUsersAndGroups = [];
                    }
                },
                update: function(ui){
                    //get selected values
                    var id = $ektron(ui).next().val();
                    var isGroup = $ektron(ui).next().next().val();
                    var selectedUserOrGroup = {
                        "id": id,
                        "isGroup": isGroup
                    };
                    
                    //update array
                    if ($ektron(ui).is(":checked") == true) {
                        //checked - add to array if necessary
                        var addKey = true;
                        for(i=0; i<Ektron.Workarea.Permissions.Add.approvedUsersAndGroups.length; i++){
                            if ((Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].id == id) && (Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].isGroup == isGroup)) {
                                addKey = false;
                            }
                        }
                        if (addKey) {
                            Ektron.Workarea.Permissions.Add.approvedUsersAndGroups.push(selectedUserOrGroup);
                        }
                    } else {
                        //unchecked - remove from array if necessary
                        for(i=0; i<Ektron.Workarea.Permissions.Add.approvedUsersAndGroups.length; i++){
                            if ((Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].id == id) && (Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].isGroup == isGroup)) {
                                Ektron.Workarea.Permissions.Add.approvedUsersAndGroups.splice(i, 1);
                            }
                        }
                    }
                    
                    //check all checkboxes (if necessary)
                    Ektron.Workarea.Permissions.Add.checkAll();
                    
                    //set updated array of approved items to hidden field
                    $ektron("div.permissions input.approvedUsersAndGroups").attr("value", Ektron.JSON.stringify(Ektron.Workarea.Permissions.Add.approvedUsersAndGroups));
                }
            },
            init: function(){
                //bind events
                Ektron.Workarea.Permissions.Add.bindEvents();
            
                //init data
                Ektron.Workarea.Permissions.Add.Data.init();
            
                //ensure checkall is checked if all items on page are checked
                var allChecked = true;
                $ektron("div.permissions table.ektronGrid tr[class!='title-header'] :checkbox").each(function(){
                    if ($ektron(this).is(":checked") == false) {
                       allChecked = false;
                    }
                });
                if (allChecked && $ektron("div.permissions table.ektronGrid tr[class!='title-header'] :checkbox").length > 0) {
                    $ektron("div.permissions table.ektronGrid :checkbox").attr("checked", true);
                }
            },
            submit: function(){
                //get array of approved items from hidden field
                var approvedItems = $ektron("div.permissions input.approvedUsersAndGroups").attr("value");
                Ektron.Workarea.Permissions.Add.approvedUsersAndGroups = new Array();
                if ("undefined" != typeof (approvedItems) && approvedItems != "") {
                    Ektron.Workarea.Permissions.Add.approvedUsersAndGroups = Ektron.JSON.parse(approvedItems);
                }
            
                //submit selected users and/or groups
                if (Ektron.Workarea.Permissions.Add.approvedUsersAndGroups.length == 0) {
                    //alert if none selected
                    alert(Ektron.Workarea.Permissions.Add.noItemSelected);
                } else {
                    //set required submit querystring values
                    var contLang = "<asp:Literal runat='server' id='litLanguageId' />";
                    var contId = "<asp:Literal runat='server' id='litId' />";
                    var itemType = "<asp:Literal runat='server' id='litItemType' />";
                    var isMembership = "<asp:Literal runat='server' id='litIsMembership' />";
                    
                    //init groups and users to empty string
                    var selectedGroup = "";
                    var selectedUsers = "";
                
                    //populate selected groups and users vars
                    for (i = 0; i < Ektron.Workarea.Permissions.Add.approvedUsersAndGroups.length; i++ )
                    { 
                        if (Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].isGroup == "true"){
                            if (selectedGroup.length != 0) { selectedGroup += ",";}
                            selectedGroup += Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].id;
                        } else {
                            if (selectedUsers.length != 0) { selectedUsers += ",";}
                            selectedUsers += Ektron.Workarea.Permissions.Add.approvedUsersAndGroups[i].id;
                        }
                    }
                    
                    //alert("content.aspx?LangType=" + contLang + "&action=AddPermissions&id=" + contId + "&type=" + itemType  + "&groupIDs=" + selectedGroup + "&userIDs=" + selectedUsers + "&membership=" + isMembership);
                    
                    //redirect page to add users and/or groups
                    window.location = "content.aspx?LangType=" + contLang + "&action=AddPermissions&id=" + contId + "&type=" + itemType  + "&groupIDs=" + selectedGroup + "&userIDs=" + selectedUsers + "&membership=" + isMembership;
                }
            },
            Search: {
                checkForReturn: function(e){
                    var keynum;
                    var keychar;
                    if(window.event){
                        keynum = e.keyCode //ie
                    } else if(e.which) {
                        keynum = e.which //else
                    }
                    if( keynum == 13 ) {
                        Ektron.Workarea.Permissions.Add.Search.submit();
                        return false;
                    }
                },
                submit: function(){
                    if($ektron("#txtSearch").attr("value") !== undefined)
                    {
                        if($ektron("#txtSearch").attr("value").indexOf('\"')!= -1){
	                        alert('remove all quote(s) then click search');
	                        $ektron("#permission_hdnStopExecution")[0].value = "false";
	                        return false;
	                    }
	                    else if ($ektron("#selecttype option:selected").attr("value") == -1) {	                    
	                        alert('You must select either a user or a group from the dropdownlist before entering search text');
	                        $ektron("#permission_hdnStopExecution")[0].value = "false";
	                        return false;
	                    }
	                    else {
                            document.forms[0].submit();
                        }
                    }
                }
            },
            toggleCheckboxes: function(ui){
                $ektron("div.permissions table.ektronGrid :checkbox").attr("checked", $ektron(ui).is(":checked"));
                $ektron("div.permissions table.ektronGrid tr[class!='title-header'] :checkbox").each(function(){
                    Ektron.Workarea.Permissions.Add.Data.update(this);
                });
            }
        };
        
        //initialize object
        Ektron.ready(function() {
	        Ektron.Workarea.Permissions.Add.init();
        });
        
        $ektron.addLoadEvent(function(){
            $ektron("#permission_hdnAssignedUserGroupIds")[0].value = top.Ektron.Workarea.AddPermissionItems;
        });
        
        // Set the User or Group View
        setUserGroupView = function(obj) {
            var contLang = "<asp:Literal runat='server' id='ltrLanguageId' />";
            var contId = "<asp:Literal runat='server' id='ltrId' />";
            var itemType = "<asp:Literal runat='server' id='ltrItemType' />";
            var isMembership = "<asp:Literal runat='server' id='ltrIsMembership' />";
            $ektron("#permission_hdnSelectType")[0].value = obj.value;
            window.location = "content.aspx?LangType=" + contLang + "&action=SelectPermissions&id=" + contId + "&type=" + itemType  + "&membership=" + isMembership + "&selectType=" + obj.value; 
        };
        
    </script>
    <input id="hdnRetrievalMode" runat="server" type="hidden" />
    <input id="hdnUserOrGroups" runat="server" type="hidden" />
    <input id="hdnSearchTerms" runat="server" type="hidden" />
    <input id="hdnApprovedUsersAndGroups" runat="server" class="approvedUsersAndGroups" type="hidden" />
    <asp:DataGrid ID="uxPermissionsGrid" runat="server"
        OnItemDataBound="uxPermissionsGrid_OnItemDataBound"
        CssClass="ektronGrid"
        PagerStyle-Visible="false" 
        AutoGenerateColumns="false">
        <HeaderStyle CssClass="title-header" />
        <Columns>
            <asp:TemplateColumn 
                HeaderStyle-Width="20px" 
                HeaderStyle-HorizontalAlign="Center"
                ItemStyle-Width="20px"
                ItemStyle-HorizontalAlign="Center">
                <HeaderTemplate>
                    <input id="uxSelectAll" runat="server" type="checkbox" title="Select All" class="selectAll" onclick="Ektron.Workarea.Permissions.Add.toggleCheckboxes(this);" />
                </HeaderTemplate>
                <ItemTemplate>
                    <input id="uxSelect" runat="server" type="checkbox" title="Select User or Group" class="select" />
                    <input id="uxId" runat="server" type="hidden" name="permissions" class="id" />
                    <input id="uxIsGroup" runat="server" type="hidden" name="permissions" class="isGroup" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="uxNameHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Image ID="uxIcon" runat="server" />
                    <asp:HyperLink ID="uxName" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="uxTypeHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="uxType" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
    </asp:DataGrid>
    <ucEktron:Paging ID="uxPaging" runat="server" />
    <input type="hidden" runat="server" id="hdnSelectType" name="hdnSelectType" />
    <input type="hidden" runat="server" id="hdnStopExecution" name="hdnStopExecution" />
    <input type="hidden" runat="server" id="hdnAssignedUserGroupIds" name="hdnAssignedUserGroupIds" />
</div>
