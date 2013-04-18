<%@ Page Language="C#" AutoEventWireup="true" CodeFile="taxonomy.aspx.cs" Inherits="taxonomy" %>

<%@ Reference Control="controls/taxonomy/addtaxonomy.ascx" %>
<%@ Reference Control="controls/taxonomy/viewtaxonomy.ascx" %>
<%@ Reference Control="controls/taxonomy/edittaxonomy.ascx" %>
<%@ Reference Control="controls/taxonomy/taxonomytree.ascx" %>
<%@ Reference Control="controls/taxonomy/assigntaxonomy.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Taxonomy</title>
    <asp:Literal id="displaystylesheet" runat="server" />
    <script type="text/javascript" src="java/ektron.workarea.js">
</script>
    <script type="text/javascript" src="java/internCalendarDisplayFuncs.js">
</script>
    <script type="text/javascript">

        var cmsAppPath = '<asp:Literal id="litAppPath" runat="server" />';
        
        function Delete(){
       
            if(!IsSelected('selected_taxonomy')){
                alert('<asp:Literal id="ltr_selTaxMsg" runat="server" />');
                return false;
            }
            var delCnfrmMsg = '<asp:Literal id="ltr_delCnfrmMsg" runat="server"/>';
            if(confirm(delCnfrmMsg))
            {
                document.getElementById("submittedaction").value="deleteSelected";
                $ektron("#txtSearch").clearInputLabel();
                document.forms[0].submit();
                return true;
	        }
	        else
	        {
	            return false;
	        }
        }

        function checkAll(ControlName,flag){
	        if(flag==true){
		        var iChecked=0;
		        var iNotChecked=0;
		        for (var i=0;i<document.forms[0].elements.length;i++){
			        var e = document.forms[0].elements[i];
			        if (e.name==ControlName){
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
			        if (e.name==ControlName){
				        e.checked=document.forms[0].checkall.checked
			        }
		        }
	        }
        }

        function IsSelected(ControlName){
            var userChecked=false;
            for (var i=0;i<document.forms[0].elements.length;i++){
	            var e = document.forms[0].elements[i];
	            if (e.name==ControlName && e.checked){
		            userChecked=true;
		            break;
	            }
            }
            return userChecked;
        }

	    function defaultLoadLanguage(num){
	        top.notifyLanguageSwitch(num, -1);
		    document.forms[0].action="taxonomy.aspx?LangType="+num;
		    document.forms[0].isSearchPostData.value = "1";
            document.forms[0].isPostData.value="true";
            $ektron("#txtSearch").clearInputLabel();
		    document.forms[0].submit();
		    return false;
		}
	    
	    function pageLoaded()
        {
	        setTimeout("showSelectedFolderTree();", 100);
	    }
        
        function resetPostback(){
            document.forms[0].isPostData.value = "";
        }
        
        function searchtaxonomy(){
            if(document.forms[0].txtSearch.value.indexOf('\"')!=-1){
                alert('<asp:Literal id="ltr_msgSearch" runat="server" />');
                return false;
            }
            document.forms[0].isSearchPostData.value = "1";
            document.forms[0].isPostData.value="true";
            $ektron("#txtSearch").clearInputLabel();
            document.forms[0].submit();
            return true;
        }
        
        function CheckForReturn(e)
	    {
	        var keynum;var keychar;
            if(window.event) // IE
                keynum = e.keyCode
            else if(e.which) // Netscape/Firefox/Opera
                keynum = e.which
            if( keynum == 13 )
                document.getElementById('btnSearch').focus();
        }
	    
	    function IsLanguage() {
	        var bContinue = true;
	        var lang = '<asp:Literal id="ltr_lang" runat="server" />';
	        if (lang < 1) {
	            var contLangCnfrmMsg = '<asp:Literal id="ltr_contLangCnfrmMsg" runat="server" />';
	            bContinue = confirm(contLangCnfrmMsg);
	            return bContinue;
	        }
	        
	        return bContinue;
	    }

	    // ----------------------------
        // Custom Property Editting
	    // ----------------------------

	    var disabledPropertyOptions = null;
        
	    // Retrieves data for the selected custom property and
        // adds a corresponding entry to the custom property table.
	    function AddCustomProperty()
	    {
	        var customPropId = $ektron("#taxonomy_availableCustomProp")[0].value;
	        var alterMsg = '<asp:Literal id="ltr_AddPropMsg" runat="server" />';
	        if (customPropId != null && customPropId != -1) {
	            $ektron.getJSON(
	            "controls/content/CustomPropertyHandler.ashx?action=getcustompropdata&propid=" + customPropId,
	            function(data) {
	                AddCustomPropertyToTable(data);
	            });
	        }
	        else {
	            alert(alterMsg);
	        }
	    }

	    // Adds an entry to the table of custom properties associated
	    // with the current taxonomy category.
	    function AddCustomPropertyToTable(data) {
	        var propertyRow = $ektron("<tr>");      // Table row for new property
	        var titleCell = $ektron("<td>");        // Property title cell
	        var dataTypeCell = $ektron("<td>");     // Property data type cell
	        var valueCell = $ektron("<td>");        // Property data value cell
	        var removeCell = $ektron("<td>");       // Remove row cell
	     
	        // Add remove link to the appropriate cell.
	        var removeAnchor = $ektron("<a>");
	        removeCell.append(removeAnchor);
	        removeAnchor.attr("href", "#RemoveCustomProperty");
	        removeAnchor.text("Remove");
	        removeAnchor.addClass("removeCustomProperty");
	        removeAnchor.bind(
	            "click",
	            function() {
	                var propertyId = this.parentNode.parentNode.id.replace("customPropertyRow", "");
	                RemoveCustomPropertyFromTable(propertyId);
	                return false;
	            });

            // Populate the title cell with property title.
	        titleCell.text(data.Title);
	        
	        // Populate data type cell with property data type.
	        dataTypeCell.text(data.DataType);
	        
	        // Load input elements into the value cell.
	        LoadCustomPropertyInput(data, valueCell);

            // Add table cells to the property table row.
	        propertyRow.append(titleCell);
	        propertyRow.append(dataTypeCell);
	        propertyRow.append(valueCell);
	        propertyRow.append(removeCell);

            // Set row ID and class attributes.
	        propertyRow.attr("id", "customPropertyRow" + data.Id);
	        propertyRow.addClass("customPropertyRow");

            // Add new property row to the property table.
	        $ektron("#taxonomy_customPropertyTable").append(propertyRow);

            // Re-stripe the rows to account for any new additions.
	        StripeCustomPropertyRows();

	        // Disable the option in the list of available properties.
	        //$ektron("#taxonomy_availableCustomProp option[value=" + data.Id + "]").attr("disabled", "disabled");

	        if (disabledPropertyOptions == null) {
	            disabledPropertyOptions = $ektron("<select>");
	        }
	        
	        disabledPropertyOptions.append($ektron($ektron("#taxonomy_availableCustomProp option[value=" + data.Id + "]").remove()));

	        // Set the selected custom property to the default option.
            if($("#taxonomy_availableCustomProp").length>0){   
                $("#taxonomy_availableCustomProp")[0].selectedIndex = 0;
            }
	    }

	    // Removes a custom property from the table and returns it
	    // to the list of available options.
	    function RemoveCustomPropertyFromTable(propertyId) {
	        // Remove the specified row from the table.
	        $ektron("#customPropertyRow" + propertyId).remove();
	        
	        // Enable the corresponding property option in the drop down.
	        $ektron("#taxonomy_availableCustomProp").append(disabledPropertyOptions.find("option[value=" + propertyId + "]").remove());

	        // Set the selected custom property to the default option.
	        $("#taxonomy_availableCustomProp")[0].selectedIndex = 0;

            // Re-stripe the rows of the table.
	        StripeCustomPropertyRows();
	    }

	    // Re-stripes custom property rows, removing any
        // previous striping and re-applying as necessary.
	    function StripeCustomPropertyRows() {
	        var allRows = $ektron(".customPropertyRow");
	        for (var i = 0; i < allRows.length; i++) {
	            $ektron(allRows[i]).removeClass("stripe");
	            if (i % 2 != 0) {
	                $ektron(allRows[i]).addClass("stripe");
	            }
	        }
	    }

	    // Populates the specified container with input elements for
	    // custom property represented by the provided data.
	    function LoadCustomPropertyInput(data, container) {
	        if (data != null) {
	            // If the data type for the property is a boolean
	            // return a pair of radio buttons.
	            if (data.DataType == "Boolean") {
	                var trueInput = null;
	                var falseInput = null;
	                
	                if(data.Items.length > 0 && data.Items[0].Value.toLowerCase() == "true"){
	                    trueInput = $ektron("<input type=\"radio\" name=\"" + "boolInput_" + data.Id + "\" checked>");
	                    falseInput = $ektron("<input type=\"radio\" name=\"" + "boolInput_" + data.Id + "\">");
	                }
	                else {
	                    trueInput = $ektron("<input type=\"radio\" name=\"" + "boolInput_" + data.Id + "\">");
	                    falseInput = $ektron("<input type=\"radio\" name=\"" + "boolInput_" + data.Id + "\" checked>");
	                }
	                
	                container.append(trueInput);
	                container.append(falseInput);

	                trueInput.attr("value", "true").after("Yes");
	                falseInput.attr("value", "false").after("No");

	                if (!data.IsEditable) {
	                    trueInput.attr("disabled", "disabled");
	                    falseInput.attr("disabled", "disabled");
	                }
	                
	            }
	            else {
	                // Otherwise we're dealing with a text-based data
	                // type. If it's a single item display textbox/textarea,
	                // otherwise display a drop down.
	                if (data.Items.length <= 1) {
	                    var textInput = null;
	                    
	                    if (data.DataType == "String") {
	                        // If its a single string, return a textarea element.
	                        textInput = $ektron("<textarea>");
	                        textInput.attr("id", "textarea" + data.Id);

	                        if (data.Items.length == 1) {
	                            textInput.text(data.Items[0].Value);
	                        }
	                        
                            // Bind input validation.
	                        textInput.keyup(
	                            function(e) {
	                                ValidateTextInput(this.id, 2000);
	                            });

	                        textInput.change(
	                            function(e) {
	                                ValidateTextInput(this.id, 2000);
	                            });
	                        
	                        // Character count warning label.
	                        var countLabel = $ektron("<div>").addClass("ektronCaption");
	                        countLabel.attr("id", "textarea" + data.Id + "_len");

	                        container.append(textInput);
	                        container.append(countLabel);
	                    }
	                    else {
	                        if (data.DataType == "DateTime") {
	                            textInput = CreateDateTimeInput(data);
	                        }
	                        else {
	                            // If its some other single entry (numeric, etc), return 
	                            // a standard textbox.
	                            textInput = $ektron("<input type=\"text\">");
	                            if (data.Items.length == 1) {
	                                textInput.attr("value", data.Items[0].Value);
	                            }
	                        }
	                        
	                        container.append(textInput);
	                    }

	                    if (!data.IsEditable) {
	                        textInput.attr("disabled", "disabled");
	                    }
	                }
	                else if (data.Items.length > 1) {
	                    // If its a multi-select entry, return a
	                    // combobox populated with the options.
	                    var comboInput = $ektron("<select>");
	                    $ektron.each(data.Items, function(index, item) {
	                        var option = $ektron("<option>");
	                        option.text(item.FormattedValue);
	                        option.attr("value", item.Value);
	                        
	                        if (item.IsSelected) {
	                            option.attr("selected", "selected");
	                        }

	                        comboInput.append(option);
	                    });

	                    if (!data.IsEditable) {
	                        comboInput.attr("disabled", "disabled");
	                    }
	                    
	                    container.append(comboInput);
	                }
	            }
	        }
	    }

	    // Creates a date/time selector for the specified
        // custom property data.
	    function CreateDateTimeInput(data) {
	        // Hidden input elements
	        var inputHidden = $ektron("<input type=\"hidden\">");
	        
	        var mainInput = inputHidden.clone();
	        mainInput.attr("id", "dateTimeSelector_" + data.Id);
	        mainInput.attr("name", "dateTimeSelector_" + data.Id);

	        var isoInput = inputHidden.clone();
	        isoInput.attr("id", "dateTimeSelector_" + data.Id + "_iso");
	        isoInput.attr("name", "dateTimeSelector_" + data.Id + "_iso");

	        var dowInput = inputHidden.clone();
	        dowInput.attr("id", "dateTimeSelector_" + data.Id + "_dow");
	        dowInput.attr("name", "dateTimeSelector_" + data.Id + "_dow");

	        var domInput = inputHidden.clone();
	        domInput.attr("id", "dateTimeSelector_" + data.Id + "_dom");
	        domInput.attr("name", "dateTimeSelector_" + data.Id + "_dom");

	        var monumInput = inputHidden.clone();
	        monumInput.attr("id", "dateTimeSelector_" + data.Id + "_monum");
	        monumInput.attr("name", "dateTimeSelector_" + data.Id + "_monum");

	        var yrnumInput = inputHidden.clone();
	        yrnumInput.attr("id", "dateTimeSelector_" + data.Id + "_yrnum");
	        yrnumInput.attr("name", "dateTimeSelector_" + data.Id + "_yrnum");

	        var hrInput = inputHidden.clone();
	        hrInput.attr("id", "dateTimeSelector_" + data.Id + "_hr");
	        hrInput.attr("name", "dateTimeSelector_" + data.Id + "_hr");

	        var miInput = inputHidden.clone();
	        miInput.attr("id", "dateTimeSelector_" + data.Id + "_mi");
	        miInput.attr("name", "dateTimeSelector_" + data.Id + "_mi");

	        // Date display span
	        var span = $ektron("<span>");
	        span.attr("id", "dateTimeSelector_" + data.Id + "_span");
	        span.attr("style", "background-color: #F5F5F5; border: 1px solid #CCCCCC; display: inline-block; padding: 0.25em; text-decoration: none;");

	        var value = "";
	        var formattedValue = "[None]"
	        mainInput.attr("value", value);
	        span.text(formattedValue);

            // Load any initial values into the selector.
	        if (data.Items != null && data.Items.length > 0) {
	            if (data.Items[0].Value != null && data.Items[0].Value != "") {
	                value = data.Items[0].Value;
	                formattedValue = data.Items[0].FormattedValue;
	                mainInput.attr("value", value);
	                span.text(formattedValue);
	            }
	        }

	        // Image buttons
            var selectButton = $ektron("<img>");
            selectButton.attr("align", "absmiddle");
            selectButton.attr("style", "cursor:pointer;padding: 0.25em;vertical-align:middle;");
            selectButton.attr("alt", "Select a date and time");
            selectButton.attr("title", "Select a date and time");
            selectButton.attr("src", "images/ui/icons/calendarAdd.png");

            selectButton.click(
                function() {
                    // Call the data/time selector's "open" function -- it will
                    // populate the appropriate input elements upon closing.
                    openDTselector(
                        "datetime",
                        $ektron("#" + "dateTimeSelector_" + data.Id).attr("value"),
                        "dateTimeSelector_" + data.Id + "_span",
                        obtainFormID(getFormElement(this)),
                        "dateTimeSelector_" + data.Id, 
                        cmsAppPath);
                });

            var deleteButton = $ektron("<img>");
            deleteButton.attr("align", "absmiddle");
            deleteButton.attr("style", "cursor:pointer;padding: 0.25em;");
            deleteButton.attr("alt", "Delete the date and time");
            deleteButton.attr("title", "Delete the date and time");
            deleteButton.attr("src", "images/ui/icons/calendarDelete.png");

            deleteButton.click(
                function() {
                    // Call the date/time selector's "delete" function -- it will
                    // clear the appropriate input elements.
                    clearDTvalue(
                        document.getElementById("dateTimeSelector_" + data.Id),
                        "dateTimeSelector_" + data.Id + "_span",
                        "[None]");

                    $ektron("#dateTimeSelector_" + data.Id + "_dow").attr("value", "");
                    $ektron("#dateTimeSelector_" + data.Id + "_dom").attr("value", "");
                    $ektron("#dateTimeSelector_" + data.Id + "_monum").attr("value", "");
                    $ektron("#dateTimeSelector_" + data.Id + "_yrnum").attr("value", "");
                    $ektron("#dateTimeSelector_" + data.Id + "_hr").attr("value", "");
                    $ektron("#dateTimeSelector_" + data.Id + "_mi").attr("value", "");
                });

            var dateTimeSelector = $ektron("<div>");
	        dateTimeSelector.append(mainInput);
	        dateTimeSelector.append(isoInput);
	        dateTimeSelector.append(dowInput);
	        dateTimeSelector.append(domInput);
	        dateTimeSelector.append(monumInput);
	        dateTimeSelector.append(yrnumInput);
	        dateTimeSelector.append(hrInput);
	        dateTimeSelector.append(miInput);
	        dateTimeSelector.append(span);

	        // Add the edit buttons only if the property
            // allows editting of its values.
	        if (data.IsEditable) {
	            dateTimeSelector.append(selectButton);
	            //dateTimeSelector.append(deleteButton);
	        }
	        
	        return dateTimeSelector;
	    }

	    // Validates text area input length and updates the
        // associated label if one exists.
	    function ValidateTextInput(id, limit) {
	        var inputElement = $ektron("#" + id);
	        var countElement = $ektron("#" + id + "_len");

	        if (inputElement) {
	            var inputText = inputElement.attr("value");
	            var inputLength = 0;
	            if (inputText != null) {
	                inputLength = inputText.length;
	            }

	            if (countElement) {
	                countElement.text("current character count: " + inputLength + " (2000 max)");
	            }

	            if (inputLength > limit) {
	                countElement.attr("style", "font-weight: bold; color: red;");
	            }
	            else {
	                countElement.attr("style", "");
	            }
	        }
        }
	    
	    function SetPropertyIds() {
	        var propertyRows = $ektron(".customPropertyRow");
	        var selectedIDs = "";
	        var selectedValues = "";

	        var i = 0;
	        
	        var isValid = true;
	        for (var i = 0; i < propertyRows.length && isValid; i++) {

	            var cells = $ektron(propertyRows[i]).find("td");
	            var propertyName = cells[0].childNodes[0].data;
	            var dataType = cells[1].childNodes[0].data;
	            var inputElements = $ektron(cells[2]).find(":input");

	            if (dataType == "Boolean") {
	                // Retrieve boolean input values
	                selectedValues += escape(inputElements[0].checked);
	            }
	            else if (dataType == "DateTime") {
	                if (inputElements.length > 1) {
	                    // Retrieve DateTime value from date/time 
	                    // selector and store them if valid.
	                    
	                    var propertyId = propertyRows[i].id.replace("customPropertyRow", "");
	                    var dateTimeSelectorValue = $ektron("#dateTimeSelector_" + propertyId);

	                    isValid = typeof dateTimeSelectorValue.attr("value") != "undefined";

                        if (isValid) {
                            selectedValues += escape(dateTimeSelectorValue.attr("value"));
                        }
                        else {
                            alert("Please select a valid date value for the property \"" + propertyName + "\".");
                        }
	                }
	                else {
	                    // Retrieve DateTime value from drop down
	                    // list. No validation necessary.

	                    selectedValues += escape(inputElements[0].value);
	                }
	            }
	            else if (dataType == "Numeric") {
	                // Retrieve numeric value and validate that it
	                // is actually a number.

	                isValid = !isNaN(inputElements[0].value) && inputElements[0].value != "" && inputElements[0].value.length <= 40;
	                if (isValid) {
	                    selectedValues += escape(inputElements[0].value);
	                }
	                else {
	                    alert("Please enter a valid numeric value for the property \"" + propertyName + "\" (max 40 characters).");
	                }
	            }
	            else {
	                // Retrieve string value and validate that it
	                // does not exceed the length restriction.

	                isValid = inputElements[0].value.length > 0 && inputElements[0].value.length <= 2000;
	                if (isValid) {
	                    selectedValues += escape(inputElements[0].value);
	                }
	                else {
	                    alert("Please enter a valid value for the property \"" + propertyName + "\". (It may not exceed 2000 characters.)");
	                }
	            }

	            selectedValues += ";"
	            
	            selectedIDs += propertyRows[i].id.replace("customPropertyRow", "") + ";";   
	        }

	        if (selectedIDs != undefined) {
	            $ektron("#taxonomy_hdnSelectedIDS")[0].value = selectedIDs;
	        }
	        if (selectedIDs != undefined) {
	            $ektron("#taxonomy_hdnSelectValue")[0].value = selectedValues;
	        }

	        return isValid;
	    }
    </script>
</head>
<body id="body" runat="server">
    <form id="form1" runat="server">
        <div id="div_taxonomylist" visible="false" runat="server">
            <div id="dhtmltooltip"></div>
            <div class="ektronPageHeader">
                <div class="ektronTitlebar" id="divTitleBar" runat="server"></div>
                <div class="ektronToolbar" id="divToolBar" runat="server"></div>
            </div>
            <div id="tr_taxonomylist" class="ektronPageContainer" runat="server">
                <div class="ektronPageGrid">
                    <asp:GridView ID="TaxonomyList"
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
                </div>
            </div>
        </div>
        <div id="dvHolder" style="z-index: auto">
            <asp:PlaceHolder ID="DataHolder" runat="server" />
        </div>
        <input type="hidden" id="submittedaction" name="submittedaction" runat="server" />
        <input type="hidden" runat="server" id="isSearchPostData" value="" />
        <asp:Literal ID="litRefreshAccordion" runat="server" />
    </form>
</body>
</html>
