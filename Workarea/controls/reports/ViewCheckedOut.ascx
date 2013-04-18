<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewCheckedOut.ascx.cs" Inherits="Ektron.Workarea.Reports.ViewCheckedOut" %>
<%@ Register TagPrefix="ucEktron" TagName="Paging" Src="../paging/paging.ascx" %>
<script type="text/javascript">
    Ektron.ready(function() {
	    Ektron.Workarea.Reports.CheckedOut.init();
    });
    Ektron.ready(function() {
        Ektron.Workarea.Reports.CheckedOut.init();
    });

    //define Ektron objects only if they are not already defined
    if (Ektron === undefined) {Ektron = {};}
    if (Ektron.Workarea === undefined) {Ektron.Workarea = {};}
    if (Ektron.Workarea.Reports === undefined) {Ektron.Workarea.Reports = {};}
    if (Ektron.Workarea.Reports.CheckedOut === undefined) {
        Ektron.Workarea.Reports.CheckedOut = {
            //properties
            confirmationMessage: 'Checkin selected items?',
            noItemSelected: 'No items have been selected.',
	        
            //methods
            bindEvents: function(){
                //header checkbox - toggle all checkboxes
                $ektron("div.CheckedOut table.ektronGrid tr[class='title-header'] :checkbox").click(function(){
                    //toggle checkboxes
                    var isChecked = $ektron(this).is(":checked") ? true : false;
                    if (isChecked) {
                        $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checkbox").attr("checked", true);
                    } else {
                        $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checkbox").removeAttr("checked");
                    }
                    
                    //update json data
                    $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checkbox").each(function(i){
                        Ektron.Workarea.Reports.CheckedOut.Data.update($ektron(this));
                    });
                });
                
                //item-level checkbox - update json data
                $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checkbox").click(function(){
                    var itemCheckBoxes = $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checkbox");
                    var itemCheckBoxesChecked = $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checked");
                    var headerCheckboxChecked = (itemCheckBoxes.length == itemCheckBoxesChecked.length) ? true : false;
                    $ektron("div.CheckedOut table.ektronGrid tr[class='title-header'] :checkbox").attr("checked", headerCheckboxChecked);
                    Ektron.Workarea.Reports.CheckedOut.Data.update($ektron(this));
                });
            },
            Checkboxes: {
                init: function(){
                    $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checkbox").each(function(i){
                        var ui = $ektron(this);
                        var id = ui.nextAll("span.id").attr("data-ektron-id");
                        var languageId = ui.nextAll("span.languageId").attr("data-ektron-languageId");
                        var index = Ektron.Workarea.Reports.CheckedOut.Data.findKey(id);
                        var isInArray = (index == -1) ? false : true;
                        if (isInArray) {
                            ui.attr("checked", "checked");
                        }
                    }); 
                }
            },
            Data: {
                init: function(){
                    //initailize json data array
                    var data = $ektron("div.CheckedOut div.checkedItems").children("input").attr("value");
                    if (data != undefined) {
                       Ektron.Workarea.Reports.CheckedOut.Data.data = Ektron.JSON.parse(data);
                       Ektron.Workarea.Reports.CheckedOut.Checkboxes.init();
                    } else {
                       Ektron.Workarea.Reports.CheckedOut.Data.data = new Array();
                    }
                },
                findKey: function(key){
                    var index = -1;
                    for(i=0;i < Ektron.Workarea.Reports.CheckedOut.Data.data.length; i++) {
                        if (key == Ektron.Workarea.Reports.CheckedOut.Data.data[i].Id) {
                            index = i;
                            isInArray = true;
                            break;
                        }
                    }
                    return index;
                },
                update: function(ui){                
                    //add checked
                    var isChecked = ui.is(":checked");
                    var id = ui.nextAll("span.id").attr("data-ektron-id");
                    var languageId = ui.nextAll("span.languageId").attr("data-ektron-languageId");
                    var index = Ektron.Workarea.Reports.CheckedOut.Data.findKey(id);
                    var isInArray = (index == -1) ? false : true;
                    
                    if (isChecked) {
                        //checked
                        if (!isInArray) {
                            //add username and domain
                            var data = {
                                Id: id,
                                LanguageId: languageId
                            }
                            Ektron.Workarea.Reports.CheckedOut.Data.data.push(data);
                        }
                    } else {
                        //unchecked
                        if (isInArray) {
                            //remove username
                            Ektron.Workarea.Reports.CheckedOut.Data.data.splice(index, 1);
                        }
                    }
                    
                    if (Ektron.Workarea.Reports.CheckedOut.Data.data != undefined) {
                        $ektron("div.CheckedOut div.checkedItems").children("input").attr("value", Ektron.JSON.stringify(Ektron.Workarea.Reports.CheckedOut.Data.data));
                    }
                }
            },
            init: function(){
                //bind events
                Ektron.Workarea.Reports.CheckedOut.bindEvents();
	        
	            //init data array
	            Ektron.Workarea.Reports.CheckedOut.Data.init();
	        
                //ensure checkall is checked if all items on page are checked
                var allChecked = true;
                $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checkbox").each(function(){
                    if ($ektron(this).is(":checked") == false) {
                       allChecked = false;
                    }
                });
                if (allChecked && $ektron("div.CheckedOut table.ektronGrid tr[class!='title-header'] :checkbox").length > 0) {
                    $ektron("div.CheckedOut table.ektronGrid :checkbox").attr("checked", true);
                }
            },
            submit: function(){
                var selectedItems = $ektron("div.CheckedOut div.checkedItems input").val() == undefined ? "[]" : $ektron("div.CheckedOut div.checkedItems input").val();
                if (selectedItems.length > 0 && selectedItems != "") {
                    selectedItems = Ektron.JSON.parse(selectedItems);
                    if (confirm(Ektron.Workarea.Reports.CheckedOut.confirmationMessage)){
                        __doPostBack('uxViewCheckedOut$lbCheckIn','')
                    }
                } else {
                    alert(Ektron.Workarea.Reports.CheckedOut.noItemSelected);
                }
            }
        }
    }
    function LoadLanguage(FormName){
        var languageId = $ektron("#selLang option:selected").attr("value");
        var url = "reports.aspx?action=ViewCheckedOut&LangType=" + languageId;
        window.location.replace(url);
    }
</script>
<div title="Checked Out" id="ReportDataGrid" class="CheckedOut">
    <asp:LinkButton ToolTip="Check In" ID="lbCheckIn" runat="server" OnClick="lbCheckIn_Click" />
    <asp:DataGrid 
            ID="dgCheckedOut" 
            runat="server" 
            OnItemDataBound="dgCheckedOut_ItemDataBound"
            AllowPaging="true"
            GridLines="None"
            PagerStyle-Visible="False"
            CssClass="ektronGrid" 
            AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <input title="Approval List" type="checkbox" name="ApprovalList" class="checkAll" />
                </HeaderTemplate>
                <ItemTemplate>
                    <input title="Select Item" type="checkbox" class="itemSelect" name="itemSelect" />
                    <span class="id" id="spanId" runat="server" />
                    <span class="languageId" id="spanLanguageId" runat="server" />
                </ItemTemplate>
                <HeaderStyle CssClass="smallCell" />
                <ItemStyle CssClass="smallCell" />
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litIconHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Image ID="imgContentIcon" runat="server" />
                </ItemTemplate>
                <HeaderStyle CssClass="smallCell" />
                <ItemStyle CssClass="smallCell" />
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
                    <asp:Literal ID="litIdHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Literal ID="litIdValue" runat="server" />
                </ItemTemplate>
                <HeaderStyle CssClass="mediumCell" />
                <ItemStyle CssClass="mediumCell" />
            </asp:TemplateColumn>
            <asp:TemplateColumn>
                <HeaderTemplate>
                    <asp:Literal ID="litSubmittedByHeader" runat="server" />
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Hyperlink ID="aSubmittedBy" runat="server" />
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
    <div class="checkedItems">
        <asp:HiddenField ID="hdnCheckedItems" runat="server" />
    </div>
</div>