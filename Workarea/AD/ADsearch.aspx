<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ADsearch.aspx.cs" Inherits="Ektron.Workarea.ActiveDirectory.AddUsers" %>
<%@ Register Src="../controls/paging/paging.ascx" TagName="Paging" TagPrefix="Ektron" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
        <title>Search Active Directory for Users</title>
        <style type="text/css">
            .ektronGrid .checkBox {width:20px;}
            div.usersAdded {margin:1em auto;padding:1em;border:1px solid #BBDDF6;text-align:center;}
            div.usersAdded h3 {display:inline;margin:0 auto;padding-left:20px;background-image:url("../images/ui/icons/check.png");background-position:left 50%;background-repeat:no-repeat;}
            div.activeDirectoryUsers td input[type=text], div.activeDirectoryUsers td select {width: 92%;}
        </style>
    </head>
    <body>
        <form id="form1" runat="server">
            <asp:Literal ID="ltr_js" runat="server" />
            <div class="ektronPageContainer ektronPageGrid activeDirectoryUsers">
                <div class="ektronPageGrid">
                    <table class="ektronGrid">
                        <thead>
                            <tr class="title-header">
			                    <th>Username</th>
			                    <th>Firstname</th>
			                    <th>Lastname</th>
			                    <th>Domain</th>
		                    </tr>
		                </thead>
		                <tfoot>
		                    <tr>
		                        <td colspan="4">
		                            <asp:Button ToolTip="Search" runat="server" ID="btnSearch" Text="Search" OnClick="btnSearch_Click"  />
		                        </td>
		                    </tr>
		                </tfoot>
		                <tbody>
		                    <tr>
		                        <td style="white-space: nowrap;">
                                    <input type="Text" name="username" maxlength="255" onkeypress="return CheckKeyValue(event,'34');" id="username" runat="server" />
                                </td>
                                <td style="white-space: nowrap;">
                                    <input type="Text" name="firstname" maxlength="50" onkeypress="return CheckKeyValue(event,'34');" id="firstname" runat="server" />
                                </td>
                                <td style="white-space: nowrap;">
                                    <input type="Text" name="lastname" maxlength="50" onkeypress="return CheckKeyValue(event,'34');" id="lastname" runat="server" />
                                    <input type="hidden" id="uid" name="uid" value="" />
                                    <input type="hidden" id="rp" name="rp" value="" /><input type="hidden" id="ep" name="e1" value="" />
                                    <input type="hidden" id="e2" name="e2" value="" /><input type="hidden" id="f" name="f" value="" />
                                </td>
                                <td style="white-space: nowrap;">
			                        <asp:DropDownList ID="ddlDomainName" runat="server" />
			                    </td>
		                    </tr>
		                </tbody>
		            </table>
		            <div class="addADUser" id="divAddUser" runat="server">
		                <asp:LinkButton ToolTip="Save" ID="lbSave" runat="server" OnClick="lbSave_Click" />
                        <asp:DataGrid ID="dgAddADUser" 
                            runat="server" 
                            AllowPaging="true" 
                            AutoGenerateColumns="false"
                            PagerStyle-Visible="False"
                            GridLines="None"
                            OnItemDataBound="dgAddADUser_ItemDataBound"
                            CssClass="ektronGrid adUser">
                            <HeaderStyle CssClass="title-header" />
                            <Columns>
                                <asp:TemplateColumn>
                                    <HeaderTemplate>
                                        <input type="checkbox" name="AddActiveDirectoryUser" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <input type="checkbox" title="Add User" name="AddActiveDirectoryUser" />
                                        <input type="hidden" id="hdnUsername" runat="server" class="username" name="AddActiveDirectoryUser" />
                                        <input type="hidden" id="hdnDomain" runat="server" class="domain" name="AddActiveDirectoryUser" />
                                    </ItemTemplate>
                                    <HeaderStyle CssClass="checkBox" />
                                    <ItemStyle CssClass="checkBox" />
                                </asp:TemplateColumn>
                                <asp:TemplateColumn>
                                    <HeaderTemplate>
                                        <asp:Literal ID="litUsernameHeader" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Literal ID="litUsername" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn>
                                    <HeaderTemplate>
                                        <asp:Literal ID="litLastNameHeader" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Literal ID="litLastName" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:TemplateColumn>
                                    <HeaderTemplate>
                                        <asp:Literal ID="litFirstNameHeader" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Literal ID="litFirstName" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                 <asp:TemplateColumn>
                                    <HeaderTemplate>
                                        <asp:Literal ID="litDomainHeader" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Literal ID="litDomain" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                            </Columns>
                        </asp:DataGrid>
                        <script type="text/javascript">
                            Ektron.ready(function() {
	                            Ektron.Workarea.ActiveDirectory.AddUser.init();
                            });

                            //define Ektron objects only if they are not already defined
                            if (Ektron === undefined) {Ektron = {};}
                            if (Ektron.Workarea === undefined) {Ektron.Workarea = {};}
                            if (Ektron.Workarea.ActiveDirectory === undefined) {Ektron.Workarea.ActiveDirectory = {};}
                            if (Ektron.Workarea.ActiveDirectory.AddUser === undefined) {
                                Ektron.Workarea.ActiveDirectory.AddUser = {
                                    //properties
                                    confirmationMessage: 'Add selected users?',
                                    noItemSelected: 'No users have been selected.',

                                    //methods
                                    bindEvents: function () {
                                        //header checkbox - toggle all checkboxes
                                        $ektron("table.adUser tr[class='title-header'] :checkbox").click(function () {
                                            //toggle checkboxes
                                            var isChecked = $ektron(this).is(":checked") ? true : false;
                                            if (isChecked) {
                                                $ektron("table.adUser tr[class!='title-header'] :checkbox").attr("checked", true);
                                            } else {
                                                $ektron("table.adUser tr[class!='title-header'] :checkbox").removeAttr("checked");
                                            }

                                            //update json data
                                            $ektron("table.adUser tr[class!='title-header'] :checkbox").each(function (i) {
                                                
                                                Ektron.Workarea.ActiveDirectory.AddUser.Data.update($ektron(this));
                                            });
                                        });

                                        //item-level checkbox - update json data
                                        $ektron("table.adUser tr[class!='title-header'] :checkbox").click(function () {
                                            Ektron.Workarea.ActiveDirectory.AddUser.Data.update($ektron(this));
                                        });
                                    },
                                    Checkboxes: {
                                        init: function () {
                                            $ektron("table.adUser tr[class!='title-header'] :checkbox").each(function (i) {
                                                var ui = $ektron(this);
                                                var username = ui.nextAll("input.username").attr("value");
                                                var index = Ektron.Workarea.ActiveDirectory.AddUser.Data.findKey(username);
                                                var isInArray = (index == -1) ? false : true;
                                                if (isInArray) {
                                                    ui.attr("checked", "checked");
                                                }
                                            });
                                        }
                                    },
                                    init: function () {
                                        //bind events
                                        Ektron.Workarea.ActiveDirectory.AddUser.bindEvents();

                                        //init data array
                                        Ektron.Workarea.ActiveDirectory.AddUser.Data.init();

                                        //ensure checkall is checked if all items on page are checked
                                        var allChecked = true;
                                        $ektron("table.adUser tr[class!='title-header'] :checkbox").each(function () {
                                            if ($ektron(this).is(":checked") == false) {
                                                allChecked = false;
                                            }
                                        });
                                        if (allChecked && $ektron("div.addADUser tr[class!='title-header'] :checkbox").length > 0) {
                                            $ektron("table.adUser :checkbox").attr("checked", true);
                                        }
                                    },
                                    submit: function () {
                                        var selectedUsers = $ektron("div.selectedItems input").val() == undefined ? "[]" : $ektron("div.selectedItems input").val();
                                        selectedUsers = Ektron.JSON.parse(selectedUsers);
                                        if (selectedUsers.length > 0) {
                                            if (confirm(Ektron.Workarea.ActiveDirectory.AddUser.confirmationMessage)) {
                                                __doPostBack('lbSave', '');
                                            }
                                        } else {
                                            alert(Ektron.Workarea.ActiveDirectory.AddUser.noItemSelected);
                                        }
                                    }
                                }
                            }

                            if (Ektron.Workarea.ActiveDirectory.AddUser.Data === undefined) {
                                Ektron.Workarea.ActiveDirectory.AddUser.Data = {
                                    init: function () {
                                        //initailize json data array
                                        var data = $ektron("div.selectedItems").children("input").attr("value");
                                        if (data.length > 0) {
                                            Ektron.Workarea.ActiveDirectory.AddUser.Data.data = Ektron.JSON.parse(data);
                                            Ektron.Workarea.ActiveDirectory.AddUser.Checkboxes.init();
                                        } else {
                                            Ektron.Workarea.ActiveDirectory.AddUser.Data.data = [];
                                        }
                                    },
                                    findKey: function (key) {
                                        var index = -1;
                                        if (Ektron.Workarea.ActiveDirectory.AddUser.Data.data != undefined) {
                                            for (i = 0; i < Ektron.Workarea.ActiveDirectory.AddUser.Data.data.length; i++) {
                                                if (key == Ektron.Workarea.ActiveDirectory.AddUser.Data.data[i].Username) {
                                                    index = i;
                                                    isInArray = true;
                                                    break;
                                                }
                                            }
                                        }
                                        return index;
                                    },
                                    update: function (ui) {
                                        //add checked
                                        var isChecked = ui.is(":checked");
                                        var username = ui.nextAll("input.username").attr("value");
                                        var domain = ui.nextAll("input.domain").attr("value");
                                        var index = Ektron.Workarea.ActiveDirectory.AddUser.Data.findKey(username);
                                        var isInArray = (index == -1) ? false : true;
                                       if (isChecked) {
                                            //checked
                                            if (!isInArray) {
                                                //add username and domain
                                                var data = {
                                                    Username: username,
                                                    Domain: domain
                                                }
//                                                if (data != undefined) {
//                                                    Ektron.Workarea.ActiveDirectory.AddUser.Data.data = new Array();
//                                                }
                                                Ektron.Workarea.ActiveDirectory.AddUser.Data.data.push(data);
                                            }
                                        } else {
                                            //unchecked
                                            if (isInArray) {
                                                //remove username
                                                Ektron.Workarea.ActiveDirectory.AddUser.Data.data.splice(index, 1);
                                            }
                                        }
                                        if (Ektron.Workarea.ActiveDirectory.AddUser.Data.data != undefined) {
                                            $ektron("div.selectedItems").children("input").attr("value", Ektron.JSON.stringify(Ektron.Workarea.ActiveDirectory.AddUser.Data.data));
                                        }
                                    }
                                }
                            }
                        </script>
                        <Ektron:Paging ID="uxPaging" runat="server" visible="false" />
                        <div class="selectedItems">
                            <asp:HiddenField ID="hdnSelectedItems" runat="server" />
                        </div>
                    </div>
                    <div class="usersAdded" id="divUsersAdded" runat="server" visible="false">
                        <h3><asp:Literal ID="litSuccess" runat="server" /></h3>
                    </div>
                </div>
            </div>
        </form>
    </body>
</html>

