<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ViewCheckedIn.ascx.cs" Inherits="Ektron.Workarea.Reports.ViewCheckedIn" %>
<%@ Register TagPrefix="ucEktron" TagName="Paging" Src="../paging/paging.ascx" %>
<script type="text/javascript">
    Ektron.ready(function() {
	    Ektron.Workarea.Reports.CheckedIn.init();
    });

    //define Ektron objects only if they are not already defined
    if (Ektron === undefined) {Ektron = {};}
    if (Ektron.Workarea === undefined) {Ektron.Workarea = {};}
    if (Ektron.Workarea.Reports === undefined) {Ektron.Workarea.Reports = {};}
    if (Ektron.Workarea.Reports.CheckedIn === undefined) {
	    Ektron.Workarea.Reports.CheckedIn = {
	        //properties
	        approveAllMessage: 'Checkin all selected content?',
	        noItemSelected: 'No content has been selected to checkin.',
	        
	        //methods
	        bindEvents: function(){
	            $ektron("div.checkedIn tr.title-header input.checkAll").click(function(){
	                $ektron("div.checkedIn :checkbox").attr("checked", $ektron(this).is(":checked"));
	            });
	        
	            $ektron("div.checkedIn tr[class!='title-header'] :checkbox").click(function(){
                    //get key
                    var id = $ektron(this).next().attr("data-ektron-id");
                    var folderId = $ektron(this).next().next().attr("data-ektron-folderid");
                    var languageId = $ektron(this).next().next().next().attr("data-ektron-languageId");
                    var key = id + "_" + folderId + "_" + languageId;
                    
                    //populate array of checked items
                    var checkedItemsArray = new Array()
                    var checkedItemsString = $ektron("div.checkedItems input").val();
	                checkedItemsArray = checkedItemsString.split(",");
	                           
	                //update array
	                if ($ektron(this).is(":checked") == true) {
	                    //checked - add key to array if necessary
	                    var addKey = true;
	                    for(i=0; i<checkedItemsArray.length; i++){
	                        if (checkedItemsArray[i] == key) {
	                            addKey = false;
	                        }
	                    }
	                    if (addKey) {
	                        checkedItemsArray.push(key);
	                    }
	                } else {
	                    //unchecked - remove key from array if necessary
	                    for(i=0; i<checkedItemsArray.length; i++){
	                        if (checkedItemsArray[i] == key) {
	                            checkedItemsArray.splice(i, 1);
	                        }
	                    }
	                }
	                
	                //serialize array to hidden field
	                var items = checkedItemsArray.join(",");
	                $ektron("div.checkedIn div.checkedItems input").val(items);
	                
	                //if all checkboxes are checked, or all checkboxes are unchecked, toggle checkall checkbox
	                var checkboxes = $ektron("div.checkedIn tr[class!='title-header'] :checkbox");
	                var checkedCheckboxes = $ektron("div.checkedIn tr[class!='title-header'] :checked");
	                if (checkboxes.length == checkedCheckboxes.length) {
	                    //all are checked, check checkall checkbox
	                    $ektron("div.checkedIn tr[class='title-header'] :checkbox").attr("checked", true);
	                } else {
	                    //all are not checked, check checkall checkbox
	                    $ektron("div.checkedIn tr[class='title-header'] :checkbox").attr("checked", false);
	                }
	            });
	        },
	        Checkboxes: {
	            init: function(){
	                //populate array of checked items
                    var checkedItemsArray = new Array()
                    var checkedItemsString = $ektron("div.checkedItems input").val();
	                checkedItemsArray = checkedItemsString.split(",");
	                
	                //check any previously selected checkboxes
	                $ektron("div.checkedIn tr[class!='title-header'] :checkbox").each(function(i){
	                    //get key
	                    var id = $ektron(this).next().attr("data-ektron-id");
                        var folderId = $ektron(this).next().next().attr("data-ektron-folderid");
                        var languageId = $ektron(this).next().next().next().attr("data-ektron-languageId");
                        var key = id + "_" + folderId + "_" + languageId;
	                    
	                    for(i=0; i<checkedItemsArray.length; i++){
                            if (checkedItemsArray[i] == key) {
                                $ektron(this).attr("checked", "checked");
                            }
                        }
                    });
	            }
	        },
	        init: function(){	        
	            //bind events
	            Ektron.Workarea.Reports.CheckedIn.bindEvents();
	            
	            //ensure selected items are checked
	            Ektron.Workarea.Reports.CheckedIn.Checkboxes.init();
	        
	            //ensure checkall is checked if all items on page are checked
	            var allChecked = true;
	            $ektron("div.checkedIn tr[class!='title-header'] :checkbox").each(function(){
	                if ($ektron(this).is(":checked") == false) {
	                   allChecked = false;
	                }
	            });
	            if (allChecked && $ektron("div.checkedIn tr[class!='title-header'] :checkbox").length > 0) {
	                $ektron("div.checkedIn :checkbox").attr("checked", true);
	            }
	        },
	        submit: function(){
	            if ($ektron("div.checkedItems input").val() != "") {
	                if (confirm(Ektron.Workarea.Reports.CheckedIn.approveAllMessage)){
    	                __doPostBack('uxViewCheckedIn$lbSubmitForPublication','')
	                }
	            } else {
	                alert(Ektron.Workarea.Reports.CheckedIn.noItemSelected);
	            }
	        }
	    };
    }
    function LoadLanguage(FormName){
        var languageId = $ektron("#selLang option:selected").attr("value");
        var url = "reports.aspx?action=ViewCheckedIn&LangType=" + languageId;
        window.location.replace(url);
    }
</script>
<div id="ReportDataGrid" class="checkedIn">
    <asp:LinkButton ToolTip="Submit For Publication" ID="lbSubmitForPublication" runat="server" OnClick="lbSubmitForPublication_Click" />
    <asp:DataGrid 
            ID="dgCheckedIn" 
            runat="server" 
            OnItemDataBound="dgCheckedIn_ItemDataBound"
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
                    <span class="id" id="hdnId" runat="server" />
                    <span class="folderId" id="hdnFolderId" runat="server" />
                    <span class="languageId" id="hdnLanguageId" runat="server" />
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