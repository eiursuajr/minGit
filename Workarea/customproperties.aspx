<%@ Page Language="C#" AutoEventWireup="true" CodeFile="customproperties.aspx.cs"
    Inherits="Workarea_customproperties" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Custom Properties</title>
    
    <asp:literal runat="server" id="jsStyleSheet" />

    <script type="text/javascript">
        
        var cmsAppPath = '<asp:literal runat="server" id="ltrAppPath" />';        
        var jsAlertNumericVal = '<asp:Literal runat="server" id="jsAlertNumericVal" />';
        var jsConfirmDelete = '<asp:Literal runat="server" id="jsConfirmDelete" />';
        var jsSelectCustomProperty = '<asp:Literal runat="server" id="jsSelectCustomProperty" />';

        // Process the form and submit it if all of the data
        // is valid.
        function SubmitForm()
        {
            if(location.href.indexOf("action=editcustomproperty") != -1 || location.href.indexOf("action=addcustomproperty") != -1)
            {
                var propertyType = $ektron("#ddDataTypes").attr("value");
                var displayType = $ektron("#ddDisplayTypes").attr("value");

                var inputValue = "";
                var isValid = true;                

                if (displayType == "MultiSelect") {
                    var options = $ektron("#multiSelectInput option");
                    if (options.length > 1) {
                        options.each(function() {
                            var optionValue = $ektron(this).attr("value");

                            if (inputValue.length > 0) {
                                inputValue += ";";
                            }

                            inputValue += encodeURIComponent($ektron(this).attr("value"));
                        });
                    }
                    else {
                        alert('Properties with a list display type must include at least two values.');
                        isValid = false;
                    }
                }
                else {
                    switch (propertyType) {
                        case "String":
                            inputValue = $ektron("#textValueInput").attr("value");
                            if (typeof (inputValue) == "undefined") {
                                inputValue = "";
                            }
                            
                            isValid = ValidateText(inputValue, true);
                            break;

                        case "DateTime":
                            inputValue = $ektron("#dateTimeSelector_0").attr("value");
                            if (typeof (inputValue) == "undefined") {
                                inputValue = "";
                            }
                            
                            isValid = ValidateDate(inputValue, true);
                            break;

                        case "Numeric":
                            inputValue = $ektron("#numericalValueInput").attr("value");
                            if (typeof (inputValue) == "undefined") {
                                inputValue = "";
                            }
                            
                            isValid = ValidateNumeric(inputValue, true);
                            break;

                        case "Boolean":
                            inputValue = $ektron("#rdoTrue").attr("checked");
                            if (inputValue == "checked") {
                                inputValue = "true";
                            }
                            else {
                                inputValue = "false";
                            }
                            break;
                    }

                    inputValue = encodeURIComponent(inputValue);
                }

                if ($ektron("#txtPropertyName").attr("value") != null && $ektron("#txtPropertyName").attr("value").length > 0) {
                    if ($ektron("#txtPropertyName").attr("value").length > 50) {
                        alert("Property names cannot be longer than 50 characters.");
                        isValid = false;
                    }
                }
                else {
                    alert("Please enter a property name.");
                    isValid = false;
                }

                if(isValid){
                    $ektron("#propertyValues").attr("value", inputValue);
                    document.forms[0].submit();
                }
            }
        }

        // Validate the input (of the specified property type). Displays
        // a message if the input is not valid.
        function Validate(input, propertyType, allowEmpty) {
            var isValid = false;
            switch (propertyType) {
                case "String":
                    isValid = ValidateText(input, allowEmpty);
                    break;
                case "DateTime":
                    isValid = ValidateDate(input, allowEmpty);
                    break;
                case "Numeric":
                    isValid = ValidateNumeric(input, allowEmpty);
                    break;
            }

            return isValid;
        }

        // Validate text input and display the appropriate message
        // as necessary.
        function ValidateText(input, allowEmpty) {
            var isValid = true;
            if (input != null) {
                isValid = input.length <= 2000;
                if (!isValid) {
                    alert("String values cannot exceed 2000 characters.");
                }
            }
            else {
                if (!allowEmpty) {
                    alert("Please enter a value for the property.");
                }
            }
            
            return isValid;
        }

        // Validate numerical input and display the appropriate message
        // as necessary.
        function ValidateNumeric(input, allowEmpty) {
            var isValid = true;
            if (input != null && input != "" && typeof (input) != "undefined") {
                isValid = !isNaN(input) && input.length <= 40;
            }
            else {
                if(!allowEmpty){
                    isValid = false;
                }
            }

            if (!isValid) {
                alert("Please enter a valid numeric value (max 40 characters).");   
            }

            return isValid;
        }

        // Validate DataTime input and display the appropriate message
        // as necessary.
        function ValidateDate(input, allowEmpty) {
            var isValid = true;
            if (!allowEmpty) {
                isValid = input != null && input != "[None]";
            }

            if (!isValid) {
                alert("Please select a valid DateTime value.");
            }

            return isValid;
        }

        // Loads a semi-colon delimited list of properties
        // selected for delete.
        function GetDeleteIds()
        {
            var deleteCheckBox = $ektron('.delete');
            var i = 0;
            var checkedBoxes = 0;
            for(i = 0; i < deleteCheckBox.length; i++)
            {
                if(deleteCheckBox[i].checked)
                {
                    var hdnSelectedId = "#hdnSelectCustProp" + i + "";
                    $ektron("input#deleteSelectedIds")[0].value = $ektron("input#deleteSelectedIds")[0].value + ";" +  $ektron(hdnSelectedId)[0].value;
                    checkedBoxes = checkedBoxes + 1;
                }
            }
            if(checkedBoxes > 0)
            {
                if(confirm(jsConfirmDelete))
                {
                    document.forms[0].submit();
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                alert(jsSelectCustomProperty);
                return false;
            }
        }
        
        // Confirms the deletion of a property ID.
        function confirmDelete()
        {
            if(confirm(jsConfirmDelete))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Sets the object type based on the specified selection.
        function SetObjectType(obj) {
            $ektron("input#isPostData").attr("value", "");
            location.href = "users.aspx?action=viewcustomprop";
            return false;
        }

        // Launches the user into "translate" mode for the sepcified
        // custom property. Property is duplicated for the new language
        // and edit screen is displayed for user to translate property.
        // (Used from the "Add" language toolbar drop down.)
        function Translate(customPropertyId, originalLangId, newLangId)
        {
            if (newLangId > 0)
            {
                if (location.href.indexOf("action=editcustomproperty") != -1)
                {
                    location.href = "customproperties.aspx?action=addcustomproperty&Translate=true&id=" +
                    customPropertyId + "&LangType=" + newLangId + "&OrigLangType=" + originalLangId;
                }
            }
        }

        // Sets the user into edit mode for the specified custom property
        // and language. (Used from the "View" language toolbar drop down.)
        function EditLanguage(customPropertyId, languageId)
        {
            if (location.href.indexOf("action=editcustomproperty") != -1)
            {
                location.href = "customproperties.aspx?action=editcustomproperty&id=" + customPropertyId +
                    "&LangType=" + languageId;
            }
        }

        // Adds the value of the specified input element to the
        // specified list.
        function AddItemToList(inputElementId, listElementId) {
                    
            var inputElement = $ektron("#" + inputElementId);

            if (inputElement != null) {

                if (AddNameValuePairToList(inputElement.attr("value"), inputElement.attr("value"), listElementId)) {
                    inputElement.attr("value", "");
                }
            }
        }

        // Adds a DateTime value to the specified list
        function AddDateTimeToList(dateTimeSelector, listElementId) {
            var inputText = $ektron("#" + dateTimeSelector + "_span").text();
            var inputValue = $ektron("#" + dateTimeSelector).attr("value");

            AddNameValuePairToList(inputText, inputValue, listElementId);
        }

        // Adds a name value pair to the specified list.
        function AddNameValuePairToList(name, value, listElementId) {
            
            var propertyType = $ektron("#ddDataTypes").attr("value");
            var isValid = Validate(value, propertyType, false);
            
            if (isValid) {
                if (value != null && value.length > 0) {
                    var option = $ektron("<option>");
                    option.attr("value", encodeURIComponent(value));
                    option.text(name);

                    var listElement = $ektron("#" + listElementId);
                    if (listElement != null) {
                        listElement.append(option);
                    }
                }
            }
            
            return isValid;
        }

        // Moves the selected item up in the specified list.
        function MoveItemUp(listElementId) {
            var selectedOption = $ektron("#" + listElementId).find(":selected");
            var prevOption = $ektron(selectedOption).prev("option");

            if ($ektron(prevOption).text() != "") {
                $ektron(selectedOption).remove();
                $ektron(prevOption).before($(selectedOption));
             }
        }

        // Moves the selected item down in the specified list.
        function MoveItemDown(listElementId) {
            var selectedOption = $ektron("#" + listElementId).find(":selected");
            var nextOption = $ektron(selectedOption).next("option");

            if ($ektron(nextOption).text() != "") {
                $ektron(selectedOption).remove();
                $ektron(nextOption).after($(selectedOption));
            }
        }   

        // Removes the selected item from the specified list.
        function RemoveSelectedItem(listElementId) {
            var selectedItem = $ektron("#" + listElementId + " :selected");
            if (selectedItem != null) {
                selectedItem.remove();
            }
        }

        // Edits the selected item in the specified list (removes it from
        // the list and adds it to the appropriate input field.
        function EditSelectedItem(listElementId) {
            var selectedItem = $ektron("#" + listElementId + " :selected");
            if (selectedItem.length > 0) {
                var propertyType = $ektron("#ddDataTypes").attr("value");
                switch (propertyType) {
                    case "String":
                        $ektron("#textValueInput").attr(
                            "value", 
                            decodeURIComponent(selectedItem.attr("value")));
                        break;
                    case "Numeric":
                        $ektron("#numericalValueInput").attr(
                            "value",
                            decodeURIComponent(selectedItem.attr("value")));
                        break;
                    case "DateTime":
                        $ektron("#dateTimeSelector_0").attr(
                            "value",
                            decodeURIComponent(selectedItem.attr("value")));
                            
                        $ektron("#dateTimeSelector_0_span").text(decodeURIComponent(selectedItem.text()));
                        break;
                }
                
                selectedItem.remove();
            }
        }

        // Displays the appropriate input elements according to the
        // selected property and display types.
        function DisplayValueInputElements() {
            var propertyType = $ektron("#ddDataTypes").attr("value");
            var displayType = $ektron("#ddDisplayTypes").attr("value");

            switch (propertyType) {

                case "String":
                    $ektron("#booleanValueInputWrapper").hide();
                    $ektron("#numericalValueInputWrapper").hide();
                    $ektron("#dateValueInputWrapper").hide();
                    $ektron("#textValueInputWrapper").show();
                    $ektron("#displayTypeRow").show();

                    if (displayType == "MultiSelect") {
                        $ektron("#multiSelectInputWrapper").show();
                        $ektron("#btnAddText").show();
                    }
                    else {
                        $ektron("#multiSelectInputWrapper").hide();
                        $ektron("#btnAddText").hide();
                    }

                    break;

                case "DateTime":
                    $ektron("#booleanValueInputWrapper").hide();
                    $ektron("#numericalValueInputWrapper").hide();
                    $ektron("#textValueInputWrapper").hide();
                    $ektron("#dateValueInputWrapper").show();
                    $ektron("#displayTypeRow").show();

                    if (displayType == "MultiSelect") {
                        $ektron("#multiSelectInputWrapper").show();
                        $ektron("#btnAddDate").show();
                    }
                    else {
                        $ektron("#multiSelectInputWrapper").hide();
                        $ektron("#btnAddDate").hide();
                    }
                    break;

                case "Numeric":
                    $ektron("#booleanValueInputWrapper").hide();
                    $ektron("#textValueInputWrapper").hide();
                    $ektron("#dateValueInputWrapper").hide();
                    $ektron("#numericalValueInputWrapper").show();
                    $ektron("#displayTypeRow").show();

                    if (displayType == "MultiSelect") {
                        $ektron("#multiSelectInputWrapper").show();
                        $ektron("#btnAddNumeric").show();
                    }
                    else {
                        $ektron("#multiSelectInputWrapper").hide();
                        $ektron("#btnAddNumeric").hide();
                    }
                    break;

                case "Boolean":
                    $ektron("#textValueInputWrapper").hide();
                    $ektron("#dateValueInputWrapper").hide();
                    $ektron("#numericalValueInputWrapper").hide();
                    $ektron("#booleanValueInputWrapper").show();
                    $ektron("#displayTypeRow").hide();
                    $ektron("#multiSelectInputWrapper").hide();

                    break;
            }

            StripeRows($ektron("#tblAddEdit"));
        }

        // Loads values, from the hidden value input field, into the
        // appropriate input elements. (Loads default values if 'defaults'
        // is true.
        function LoadValues(defaults) {
            var propertyType = $ektron("#ddDataTypes").attr("value");
            var displayType = $ektron("#ddDisplayTypes").attr("value");

            switch (propertyType) {

                case "String":
                    LoadStringValue(defaults, displayType);
                    break;
                    
                case "DateTime":
                    LoadDateValue(defaults, displayType);
                    break;

                case "Numeric":
                    LoadNumericalValue(defaults, displayType);
                    break;

                case "Boolean":
                    LoadBooleanValue(defaults);
                    break;
            }
        }

        // Loads a string value into the string value input fields.
        function LoadStringValue(defaults, displayType) {
            var value = $ektron("#propertyValues").attr("value");
            
            if (defaults) {
                $ektron("#textValueInput").attr("value", "");
                LoadMultiSelectValue(defaults);
            }
            else {
                switch (displayType) {
                    case "SingleSelect":
                        $ektron("#textValueInput").attr("value", decodeURIComponent(value));
                        break;
                    case "MultiSelect":
                        LoadMultiSelectValue(defaults);
                        break;
                }
            }
        }

        // Loads a DateTime value into the DateTime value input fields.
        function LoadDateValue(defaults, displayType) {
            var value = $ektron("#propertyValues").attr("value");
            if (defaults) {
                $ektron("#dateTimeSelector_0_span").text("[None]");
                LoadMultiSelectValue(defaults);
            }
            else {
                switch (displayType) {
                    case "SingleSelect":
                        var nameValuePair = value.split("|");

                        if (nameValuePair.length == 2) {
                            $ektron("#dateTimeSelector_0").attr("value", decodeURIComponent(nameValuePair[1]));
                            $ektron("#dateTimeSelector_0_span").text(decodeURIComponent(nameValuePair[0]));
                        }
                        else {
                            $ektron("#dateTimeSelector_0_span").attr("value", "[None]");
                        }
                        break;
                    case "MultiSelect":
                        LoadMultiSelectValue(defaults);
                        break;
                }
            }
        }

        // Loads a numerical value into the numerical value input fields.
        function LoadNumericalValue(defaults, displayType) {
            var value = $ektron("#propertyValues").attr("value");
            
            if (defaults) {
                $ektron("#numericalValueInput").attr("value", "");
                LoadMultiSelectValue(defaults);
            }
            else {
                switch (displayType) {
                    case "SingleSelect":                    
                        $ektron("#numericalValueInput").attr("value", decodeURIComponent(value));
                        break;
                    case "MultiSelect":
                        LoadMultiSelectValue(defaults);
                        break;
                }
            }
        }

        // Loads a boolean value into the boolean value input fields.
        function LoadBooleanValue(defaults) {
            var value = $ektron("#propertyValues").attr("value");
            
            if (defaults || value == null) {
                $ektron("#rdoFalse").removeAttr("checked");
                $ektron("#rdoTrue").attr("checked", "checked");
            }
            else {
                if (value.toLowerCase() == "true") {
                    $ektron("#rdoFalse").removeAttr("checked");
                    $ektron("#rdoTrue").attr("checked", "checked");
                }
                else {
                    $ektron("#rdoTrue").removeAttr("checked");
                    $ektron("#rdoFalse").attr("checked", "checked");
                }
            }
        }

        // Loads values into the multi-select value input fields.
        function LoadMultiSelectValue(defaults) {
            var value = $ektron("#propertyValues").attr("value");

            if (defaults) {
                ClearMultiSelectInput();
            }
            else {
                var splitValues = value.split(";");
                for (var i = 0; i < splitValues.length; i++) {
                    var option = $ektron("<option>");

                    var nameValuePair = splitValues[i].split("|");
                    if (nameValuePair.length == 1) {
                        option.attr("value", decodeURIComponent(nameValuePair[0]));
                        option.text(decodeURIComponent(nameValuePair[0]));
                    }
                    else if (nameValuePair.length == 2) {
                        option.text(decodeURIComponent(nameValuePair[0]));
                        option.attr("value", decodeURIComponent(nameValuePair[1]));
                    }

                    $ektron("#multiSelectInput").append(option);
                }
            }
        }

        // Clears the multi-select list box
        function ClearMultiSelectInput() {
            $ektron("#multiSelectInput option").each(function() {
                $ektron(this).remove();
            });
        }

        // Creates a date/time selector.
        function CreateDateTimeInput() {
            // Hidden input elements
            var inputHidden = $ektron("<input type=\"hidden\">");

            var mainInput = inputHidden.clone();
            mainInput.attr("id", "dateTimeSelector_0");
            mainInput.attr("name", "dateTimeSelector_0");

            var isoInput = inputHidden.clone();
            isoInput.attr("id", "dateTimeSelector_0_iso");
            isoInput.attr("name", "dateTimeSelector_0_iso");

            var dowInput = inputHidden.clone();
            dowInput.attr("id", "dateTimeSelector_0_dow");
            dowInput.attr("name", "dateTimeSelector_0_dow");

            var domInput = inputHidden.clone();
            domInput.attr("id", "dateTimeSelector_0_dom");
            domInput.attr("name", "dateTimeSelector_0_dom");

            var monumInput = inputHidden.clone();
            monumInput.attr("id", "dateTimeSelector_0_monum");
            monumInput.attr("name", "dateTimeSelector_0_monum");

            var yrnumInput = inputHidden.clone();
            yrnumInput.attr("id", "dateTimeSelector_0_yrnum");
            yrnumInput.attr("name", "dateTimeSelector_0_yrnum");

            var hrInput = inputHidden.clone();
            hrInput.attr("id", "dateTimeSelector_0_hr");
            hrInput.attr("name", "dateTimeSelector_0_hr");

            var miInput = inputHidden.clone();
            miInput.attr("id", "dateTimeSelector_0_mi");
            miInput.attr("name", "dateTimeSelector_0_mi");

            // Date display span
            var span = $ektron("<span>");
            span.attr("id", "dateTimeSelector_0_span");
            span.attr("style", "background-color: #F5F5F5; border: 1px solid #CCCCCC; display: inline-block; padding: 0.25em; text-decoration: none;");
            span.text("[None]");

            // Image buttons
            var selectButton = $ektron("<img>");
            selectButton.attr("align", "absmiddle");
            selectButton.attr("style", "vertical-align: middle;cursor:pointer;padding: 0.25em;vertical-align:middle;");
            selectButton.attr("alt", "Select a date and time");
            selectButton.attr("title", "Select a date and time");
            selectButton.attr("src", "images/ui/icons/calendarAdd.png");

            selectButton.click(
                function() {
                    // Call the data/time selector's "open" function -- it will
                    // populate the appropriate input elements upon closing.
                    openDTselector(
                        "datetime",
                        $ektron("#" + "dateTimeSelector_0").attr("value"),
                        "dateTimeSelector_0_span",
                        obtainFormID(getFormElement(this)),
                        "dateTimeSelector_0",
                        cmsAppPath);
                });

            var deleteButton = $ektron("<img>");
            deleteButton.attr("align", "absmiddle");
            deleteButton.attr("style", "vertical-align: middle;cursor:pointer;padding: 0.25em;");
            deleteButton.attr("alt", "Delete the date and time");
            deleteButton.attr("title", "Delete the date and time");
            deleteButton.attr("src", "images/ui/icons/calendarDelete.png");

            deleteButton.click(
                function() {
                    // Call the date/time selector's "delete" function -- it will
                    // clear the appropriate input elements.
                    clearDTvalue(
                        document.getElementById("dateTimeSelector_0"),
                        "dateTimeSelector_0_span",
                        "[None]");

                    $ektron("#dateTimeSelector_0_dow").attr("value", "");
                    $ektron("#dateTimeSelector_0_dom").attr("value", "");
                    $ektron("#dateTimeSelector_0_monum").attr("value", "");
                    $ektron("#dateTimeSelector_0_yrnum").attr("value", "");
                    $ektron("#dateTimeSelector_0_hr").attr("value", "");
                    $ektron("#dateTimeSelector_0_mi").attr("value", "");
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

            dateTimeSelector.append(selectButton);
            dateTimeSelector.append(deleteButton);

            return dateTimeSelector;
        }

        // Applies striping to the specified table (ignores
        // hidden rows).
        function StripeRows(table) {
            var previousRowStriped = false;
            $ektron(table).find("tr").each(function() {
                var row = $ektron(this);
                
                // Prevent sub-tables from inadvertantly being striped.
                if (row.parents("table").attr("id") == $ektron(table).attr("id")) {
                
                    // Only stripe visible rows...
                    if (row.is(':visible')) {
                        if (!previousRowStriped) {
                            row.addClass("stripe");
                            previousRowStriped = true;
                        }
                        else {
                            row.removeClass("stripe");
                            previousRowStriped = false;
                        }
                    }
                }
            });
        }

        // Updates the character counter associated with
        // the specified element.
        function UpdateCharacterCounter(element) {
            var inputElement = $ektron(element);
            var countLabel = $ektron("#" + inputElement.attr("id") + "_len");

            if (inputElement != null && countLabel != null) {
                var inputLength = 0;
                if (inputElement.attr("value") != null) {
                    inputLength = inputElement.attr("value").length;
                }

                countLabel.text(inputLength);

                if (inputLength > 2000) {
                    countLabel.addClass("maximum");
                }
                else {
                    countLabel.removeClass("maximum");
                }
            }
        }
    </script>

    <style type="text/css">
        a.arrowHeadUp { display:inline-block; height: 1.5em; width: 1.5em; margin: .25em; padding: 0; background-image: url(images/ui/icons/arrowHeadUp.png);}
        a.arrowHeadDown { display:inline-block; height: 1.5em; width: 1.5em; margin: .25em; padding: 0; background-image: url(images/ui/icons/arrowHeadDown.png);}    
        .displayInline { display: inline !important;}
        .displayNone { display: none !important;}
        .displayInlineBlock { display: inline-block !important;}
        
        #tblAddEdit {background-color:White; border:1px solid #BBDDF6; border-collapse:collapse; border-spacing:0; width:100%;}
        #tblAddEdit tr.stripe { background-color:#E7F0F7; }
        #tblAddEdit td {padding:.25em; border-right:0px solid #D5E7F5; border-bottom:0px solid #D5E7F5; padding: .25em .5em; vertical-align:top;}
        #tblAddEdit td.label {color:#1d5987; width:10%; white-space:nowrap; text-align:right; font-weight:bold;}
        #multiSelectInputWrapper {margin-top: 5px;}        
        #multiSelectInputWrapper table td {vertical-align:middle;}
        #numericalValueInput {width:200px;}
        #dateTimeSelector_0_span {width: 250px;}
        #dateValueInputWrapper table tr td {margin:0px; padding:0px;}
        #customPropertyPaging {margin: 5px;}
        .buttonWrapper {margin:10px;}
        ul {list-style: none; margin: 0px; padding: 0px;}
        ul li.displayVertical { background-color: transparent; background-image: none; border: 0 none; margin: 0; padding: 0; display: block !important; }
        .width-300 {width: 300px !important; }    
        .height-150 {height: 150px !important; }
        .maximum {font-weight: bold; color: Red;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="ScriptManager1" />
        <div id="dhtmltooltip">
        </div>
        <div class="ektronPageHeader">
            <div class="ektronTitlebar" id="txtTitleBar" runat="server">
            </div>
            <div class="ektronToolbar" id="htmToolBar" runat="server">
            </div>
        </div>
        <div class="ektronPageContainer ektronPageGrid">
        
            <asp:Panel runat="server" ID="pnlViewAll" Visible="false">
                    <input type="hidden" runat="server" id="selVal" value="" name="selVal" />
                    <asp:DataGrid ID="ViewAllGrid" runat="server" AutoGenerateColumns="false" Width="100%"
                        CssClass="ektronGrid" AllowPaging="false" AllowCustomPaging="true" Visible="false"
                        GridLines="None">
                        <HeaderStyle CssClass="title-header" />
                    </asp:DataGrid>
                    
                    <asp:Panel runat="server" ID="pnlPaging">
                        <div id="customPropertyPaging" runat="server">
                            <div class="pageLinks">
                                 <asp:Label ID="lblPageText" runat="server">&nbsp;</asp:Label>&nbsp;<asp:Label id="lblPageValue" runat="server" class="pageLinks"></asp:Label>&nbsp;<asp:Label ID="lblOfText" runat="server"></asp:Label>&nbsp;<asp:Label ID="lblTotalPagesValue" runat="server"></asp:Label>
                            </div>
                            [<a title="First Page" href="#" class="pageLinks" rel="Start" id="firstPageLink" runat="server"></a>]
                            [<a title="Previous Page" href="#" class="pageLinks" rel="Prev" id="previousPageLink" runat="server"></a>]
                            [<a title="Next Page" href="#" class="pageLinks" rel="Next" id="nextPageLink" runat="server"></a>]
                            [<a title="Last Page" href="#" class="pageLinks" rel="End" id="lastPageLink" runat="server"></a>]
                        </div>
                    </asp:Panel>
                    
                    <input type="hidden" runat="server" id="deleteSelectedIds" class="deleteSelectedIds" name="deleteSelectedIds" value="" />
            </asp:Panel>
        
            <asp:Panel ID="pnlAddEdit" runat="server" Visible="false">
                <table id="tblAddEdit" runat="server">
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblNameLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:TextBox ToolTip="Enter Name here" ID="txtPropertyName" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="idRow" runat="server">
                        <td class="label">
                            <asp:Label ID="lblIDLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblID" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblLanguageLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblLanguage" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblObjectTypeLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ToolTip="Select Object Type from the Drop Down Menu" ID="ddObjectTypes" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblEditableLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ToolTip="Enable/Disable Editable" ID="chkEditable" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblEnabledLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:CheckBox ToolTip="Enabled/Disabled" ID="chkEnabled" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblDataTypesLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ToolTip="Select Data Type From the Drop Down Menu" ID="ddDataTypes" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr id="displayTypeRow">
                        <td class="label">
                            <asp:Label ToolTip="Display Types" ID="lblDisplayTypesLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ToolTip="Select Display Types From the Drop Down Menu" ID="ddDisplayTypes" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="lblValueLabel" runat="server"></asp:Label>
                        </td>
                        <td>
                            <div id="booleanValueInputWrapper">
                                <input type="radio" title="Yes" name="booleanValueInput" id="rdoTrue" /> Yes
                                <input type="radio" title="No" name="booleanValueInput" id="rdoFalse" /> No
                            </div>
                            
                            <div id="numericalValueInputWrapper">
                                <input type="text" title="Enter Value to Add here" name="numericalValueInput" id="numericalValueInput" />  <a title="Add" id="btnAddNumeric" class="button buttonInline greenHover buttonAdd" href="#" onclick="if(ValidateNumeric($ektron('#numericalValueInput').attr('value'))) { AddItemToList('numericalValueInput', 'multiSelectInput');} return false;"><span><asp:Literal ID="ltrAddNumericCaption" runat="server"></asp:Literal></span></a>
                            </div>
                            <div id="textValueInputWrapper">
                            <table>
                                <tr>
                                    <td>
                                        <textarea title="Enter Text here" id="textValueInput" class="width-300" name="textValueInput" onchange="UpdateCharacterCounter(this);" onkeyup="UpdateCharacterCounter(this);" onclick="UpdateCharacterCounter(this);"></textarea>
                                        <div class="ektronCaption" title="current character count">
                                            current character count: <span id="textValueInput_len">0</span> (2000 max.)
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align: right">                            
                                        <a title="Add Item" id="btnAddText" class="button buttonInline greenHover buttonAdd" href="#" onclick="AddItemToList('textValueInput', 'multiSelectInput'); return false;">
                                            <span><asp:Literal ID="ltrAddTextCaption" runat="server"></asp:Literal></span>
                                        </a>
                                    </td>
                                </tr>
                                </table>
                            </div>
                            <div id="dateValueInputWrapper">
                                <table>
                                    <tr>
                                        <td><div id="dateTimeSelector"></div></td>
                                        <td style="vertical-align: middle;">
                                            <a title="Add Date" id="btnAddDate" class="button buttonLeft greenHover buttonAdd" href="#" onclick="AddDateTimeToList('dateTimeSelector_0', 'multiSelectInput'); return false;">
                                                <span><asp:Literal ID="ltrAddDateCaption" runat="server"></asp:Literal></span>
                                            </a>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div id="multiSelectInputWrapper">
                                <table>
                                    <tr>
                                        <td>
                                            <select id="multiSelectInput" class="width-300 height-150" multiple="multiple"></select>
                                        </td>
                                        <td>
                                            <span style="display: inline-block !important; vertical-align:super;">
                                                <ul style="float: none;">
                                                    <li class="floatRight displayVertical">
                                                        <a title="Up" class="arrowHeadUp" href="javascript:void MoveItemUp('multiSelectInput');"></a>
                                                    </li>
                                                    <li class="floatRight displayVertical">
                                                        <a title="Down" class="arrowHeadDown" href="javascript:void MoveItemDown('multiSelectInput');"></a>
                                                    </li>
                                                </ul>
                                            </span>
                                        </td>
                                    </tr>
                                </table>
                                <div class="buttonWrapper">
                                    <ul class="buttonWrapper ui-helper-clearfix floatLeft" style="float: none;">
                                        <li class="floatLeft">
                                            <a title="Edit" id="btnChanger" class="button buttonLeft greenHover buttonEdit" name="btnChange" type="button" href="#" onclick="EditSelectedItem('multiSelectInput'); return false;">
                                                <span><asp:Literal ID="ltrEditCaption" runat="server"></asp:Literal></span>
                                            </a>
                                        </li>
                                        <li class="floatLeft">
                                            <a title="Remove" id="btnRemove" class="button buttonLeft redHover buttonRemove" name="btnRemove" type="button" href="#" onclick="RemoveSelectedItem('multiSelectInput'); return false;">
                                                <span><asp:Literal ID="ltrRemoveCaption" runat="server"></asp:Literal></span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
                
                <input type="hidden" id="propertyValues" value="" runat="server" />
                
                <script type="text/javascript">
                    // Initialize the page for add/edit.

                    // Display form input element for the 
                    // selected property type.
                    DisplayValueInputElements();

                    // Render the DateTime selector for later use.
                    $ektron("#dateTimeSelector").append(CreateDateTimeInput());

                    // Load the values indicated in the property values hidden
                    // field. If no values are provided, populate form with
                    // defaults.
                    LoadValues($ektron("#propertyValues").attr("value") == null);

                    // Register 'onchange' handlers for property and display
                    // type drop downs, triggering them to display the approriate
                    // input elements according to the selection.
                    $ektron("#ddDataTypes").change(function() { DisplayValueInputElements(); LoadValues(true) });
                    $ektron("#ddDisplayTypes").change(function() { DisplayValueInputElements(); LoadValues(true) });
                    
                </script>
    
            </asp:Panel>
        </div>
    </form>
</body>
</html>
