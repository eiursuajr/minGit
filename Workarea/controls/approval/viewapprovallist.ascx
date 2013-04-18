<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewApprovalList" CodeFile="ViewApprovalList.ascx.cs" %>
<%@ Register TagPrefix="ucEktron" TagName="Paging" Src="../paging/paging.ascx" %>

<script type="text/javascript">
    Ektron.ready(function() {
	    Ektron.Workarea.Reports.Approval.init();
    });

    //define Ektron objects only if they are not already defined
    if (Ektron === undefined) {Ektron = {};}
    if (Ektron.Workarea === undefined) {Ektron.Workarea = {};}
    if (Ektron.Workarea.Reports === undefined) {Ektron.Workarea.Reports = {};}
    if (Ektron.Workarea.Reports.Approval === undefined) {
        Ektron.Workarea.Reports.Approval = {
            //properties
            approveAllMessage: '<asp:literal id="litApproveAllWarning" runat="server" />',
            declineAllMessage: '<asp:literal id="litDeclineAllWarning" runat="server" />',
            noItemSelected: '<asp:literal id="litNoItemSelected" runat="server" />',

            //methods
            bindEvents: function() {
                $ektron("div.approvals tr[class!='title-header'] :checkbox").click(function() {
                    Ektron.Workarea.Reports.Approval.checkboxClick($ektron(this));
                });
            },
            checkboxClick: function(checkbox) {
                //get key
                var id = checkbox.next().val();
                var languageId = checkbox.next().next().val();
                var key = id + "_" + languageId;

                //populate array of approved items
                var approvedItemsArray = new Array()
                var approvedItemsString = $ektron("div.approvedItems input").val();
                approvedItemsArray = approvedItemsString.split(",");

                //update array
                if (checkbox.is(":checked") == true) {
                    //checked - add key to array if necessary
                    var addKey = true;
                    for (i = 0; i < approvedItemsArray.length; i++) {
                        if (approvedItemsArray[i] == key) {
                            addKey = false;
                        }
                    }
                    if (addKey) {
                        approvedItemsArray.push(key);
                    }
                } else {
                    //unchecked - remove key from array if necessary
                    for (i = 0; i < approvedItemsArray.length; i++) {
                        if (approvedItemsArray[i] == key) {
                            approvedItemsArray.splice(i, 1);
                        }
                    }
                }

                //serialize array to hidden field
                var items = approvedItemsArray.join(",");
                $ektron("div.approvedItems input").val(items);

                //if all checkboxes are checked, or all checkboxes are unchecked, toggle checkall checkbox
                var checkboxes = $ektron("div.approvals tr[class!='title-header'] :checkbox");
                var checkedCheckboxes = $ektron("div.approvals tr[class!='title-header'] :checked");
                if (checkboxes.length == checkedCheckboxes.length) {
                    //all are checked, check checkall checkbox
                    $ektron("div.approvals tr[class='title-header'] :checkbox").attr("checked", true);
                }
                if (checkedCheckboxes.length == 0) {
                    //all are not checked, check checkall checkbox
                    $ektron("div.approvals tr[class='title-header'] :checkbox").attr("checked", false);
                }
            },
            init: function() {
                //bind events
                Ektron.Workarea.Reports.Approval.bindEvents();

                //ensure checkall is checked if all items on page are checked
                var allChecked = true;
                $ektron("div.approvals tr[class!='title-header'] :checkbox").each(function() {
                    if ($ektron(this).is(":checked") == false) {
                        allChecked = false;
                    }
                });
                if (allChecked && $ektron("div.approvals tr[class!='title-header'] :checkbox").length > 0) {
                    $ektron("div.approvals :checkbox").attr("checked", true);
                }
            },
            loadLanguage: function(FormName) {
                var languageId = $ektron("#selLang option:selected").attr("value");
                var url = "approval.aspx?action=viewApprovalList&LangType=" + languageId;
                window.location.replace(url);
            },
            submit: function(bApprove) {

                if ($ektron("div.approvedItems input").val() != "") {
                    var confirmApproveOrDecline;
                    if (bApprove) {
                        confirmApproveOrDecline = Ektron.Workarea.Reports.Approval.approveAllMessage;
                    }
                    else {
                        confirmApproveOrDecline = Ektron.Workarea.Reports.Approval.declineAllMessage;
                    }
                    if (confirm(confirmApproveOrDecline)) {
                        var lbSubmitId = $ektron("input.lbSumbitId").attr("value");
                        __doPostBack(lbSubmitId, bApprove);
                    }
                } else {
                    alert(Ektron.Workarea.Reports.Approval.noItemSelected);
                }
            },
            toggleCheckboxes: function(ui) {
                $ektron("div.approvals :checkbox").attr("checked", $ektron(ui).is(":checked"));
                $ektron("div.approvals tr[class!='title-header'] :checkbox").each(function() {
                    Ektron.Workarea.Reports.Approval.checkboxClick($ektron(this));
                });
            },
            updateView: function() {
                var oForm = document.forms[0];
                var lang = "";
                var ContType = "";
                var folderId = 0;
                var strAction = ""
                var strTempOrg = "";
                strAction = '<asp:Literal id="litAction" runat="server" />';

                var objSelLang = document.getElementById('selLang');
                if (objSelLang != null) {
                    lang = objSelLang.value;

                    if (strAction.indexOf("LangType") > -1) {
                        strTemp = strAction.substring(strAction.indexOf("LangType"), strAction.length);
                        if ((strTemp.indexOf("&") > -1) && (strTemp.indexOf("&") < strTemp.length)) {
                            strTemp = strTemp.substring(0, strTemp.indexOf("&"));
                        }
                        strTempOrg = strTemp;
                        strTemp = strTemp.replace(strTemp.substring(strTemp.indexOf("=") + 1), lang)
                        strAction = strAction.replace(strTempOrg, strTemp);
                    }
                    else {
                        strAction = strAction + "&LangType=" + lang;
                    }
                }
                var objSelSupertype = document.getElementById('selAssetSupertype');
                if (objSelSupertype != null) {
                    var ContType = objSelSupertype.value;
                    if (strAction.indexOf("ContType") > -1) {
                        strTemp = strAction.substring(strAction.indexOf("ContType"), strAction.length);
                        if ((strTemp.indexOf("&") > -1) && (strTemp.indexOf("&") < strTemp.length)) {
                            strTemp = strTemp.substring(0, strTemp.indexOf("&"));
                        }
                        strTempOrg = strTemp;
                        strTemp = strTemp.replace(strTemp.substring(strTemp.indexOf("=") + 1), ContType)
                        strAction = strAction.replace(strTempOrg, strTemp);
                    }
                    else {
                        strAction = strAction + "&ContType=" + ContType;
                    }
                }
                strAction = "Approval.aspx?action=" + strAction;
                oForm.action = strAction;
                window.location.replace(strAction);
                return false;
            }
        };
    }
</script>

<div id="dhtmltooltip"></div>
<div class="ektronPageHeader">
    <asp:LinkButton ID="lbSubmit" runat="server" OnClick="lbSubmit_Click"></asp:LinkButton>
    <input type="hidden" id="lbSubmitId" runat="server" class="lbSumbitId" />
    <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
    <div class="ektronToolbar" id="divToolBar" runat="server"></div>
</div>
<div class="ektronPageContainer ektronPageGrid approvals" style="top:87px !important">
    <asp:HiddenField ID="hdnNeedingApproval" runat="server" />
    <asp:DataGrid 
            ID="dgItemsNeedingApproval" 
            runat="server" 
            OnItemDataBound="dgItemsNeedingApproval_ItemDataBound"
            AllowPaging="true"
            GridLines="None"
            PagerStyle-Visible="False"
            CssClass="ektronGrid" 
            AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <input type="checkbox" name="ApprovalList" onclick="Ektron.Workarea.Reports.Approval.toggleCheckboxes(this);" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:CheckBox ToolTip="Approve" ID="cbApproval" runat="server" />
                    <asp:HiddenField ID="hdnId" runat="server" />
                    <asp:HiddenField ID="hdnLanguageID" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litIconHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Image ToolTip="Content" ID="imgContentIcon" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litTitleHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Hyperlink ID="aTitle" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litRequestTypeHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <span id="spanRequestType" runat="server"></span>
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litStartDateHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litStartDateValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litModifiedDateHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litModifiedDateValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litSubmittedByHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litSubmittedByValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litIdHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litIdValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litLanguageHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litLanguageValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litPathHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Hyperlink ID="aPathValue" runat="server" />
                </ItemTemplate>
            </asp:TemplateColumn>
        </Columns>
        <HeaderStyle CssClass="title-header" />
    </asp:DataGrid>
    <ucEktron:Paging ID="ucPaging" runat="server" />
    <div class="approvedItems">
        <asp:HiddenField ID="hdnApprovedItems" runat="server" />
    </div>
</div>

