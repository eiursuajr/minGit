//Intialiaze Attributes
Ektron.ready(function(){
	Ektron.Commerce.ProductTypes.Attributes.init();
});

//Define Ektron object only if it's not already defined
if (Ektron === undefined) {
	Ektron = {};
}

//Define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce === undefined) {
	Ektron.Commerce = {};
}

//Define Ektron.Commerce.ProductTypes object only if it's not already defined
if (Ektron.Commerce.ProductTypes === undefined) {
	Ektron.Commerce.ProductTypes = {};
}

//Ektron ProductTypes Object
if (Ektron.Commerce.ProductTypes.Attributes === undefined) {
    Ektron.Commerce.ProductTypes.Attributes = {
        Add: {
            add: function(el) {

                var button = $ektron(el);

                //get name
                var name = $ektron("div#AttributesModal table tbody td.name input.name").attr("value");
                if (name === "" || name == undefined || name == "undefined") {
                    alert(Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.blankNameField);
                    return false;
                }
                if(name.indexOf('>') >-1 ||name.indexOf('<') >-1)
                {
                    alert(Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.cannotContainSpecialCharacters);
                    return false;
                }

                //get type
                var selectedOption = button.parents("div#AttributesModal").find("table tbody td.type select option:selected");
                var type = selectedOption.attr("value");

                //begin add - get and clone clonerow
                var tbody = $ektron("div.EktronAttributes table tbody");
                var cloneRow = tbody.children("tr.attributeCloneRow").clone(true);

                //set value fields
                cloneRow.find("input.id").attr("value", "0");
                cloneRow.find("input.type").attr("value", type);
                cloneRow.find("input.name").attr("value", name);
                cloneRow.find("input.value").attr("value", "");
                cloneRow.find("input.markedForDelete").attr("value", "false");

                //set name
                cloneRow.find("th.name span").text(name);

                //set type
                var typeLabel = "Unknown";
                switch (type) {
                    case "text":
                        //set label
                        typeLabel = Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.text;

                        //remove fields
                        cloneRow.find("input.numeric").remove();
                        cloneRow.find("input.date").remove();
                        cloneRow.find("select.boolean").remove();

                        //set view to default text
                        var textValue = Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.noDefaultText;
                        cloneRow.find("p.data span.view").text(textValue);
                        cloneRow.find("p.data input.text").attr("value", textValue);

                        break;
                    case "numeric":
                        //set label
                        typeLabel = Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.numeric;
                        //remove fields
                        cloneRow.find("input.text").remove();
                        cloneRow.find("input.date").remove();
                        cloneRow.find("select.boolean").remove();
                        //set view to default text
                        cloneRow.find("p.data span.view").text("0");
                        cloneRow.find("p.data input.numeric").attr("value", "0");
                        cloneRow.find("td.value input.value").attr("value", "0");
                        break;
                    case "boolean":
                        //set label
                        typeLabel = Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.booleanText;
                        //remove fields
                        cloneRow.find("input.text").remove();
                        cloneRow.find("input.date").remove();
                        cloneRow.find("input.numeric").remove();
                        //set view to default text
                        cloneRow.find("p.data span.view").text(cloneRow.find("p.data select option[value=true]").text());
                        cloneRow.find("p.data select option[value=true]").attr("selected", "selected");
                        cloneRow.find("td.value input.value").attr("value", "true");
                        break;
                    case "date":
                        //set label
                        typeLabel = Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.date;
                        //remove fields
                        cloneRow.find("input.numeric").remove();
                        cloneRow.find("input.text").remove();
                        cloneRow.find("select.boolean").remove();
                        //set view to default text
                        var today = $ektron("div.EktronAttributes input.todaysDate").attr("value");
                        cloneRow.find("p.data span.view").text(today);
                        cloneRow.find("p.data input.date").attr("value", today);
                        cloneRow.find("td.value input.value").attr("value", today);
                        break;
                }

                //set id to empty string
                cloneRow.find("td.id width-percent-10 span").text("");

                //set published status
                cloneRow.find("td.publishedStatus span").text(Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.statusNew);

                //set data type label
                cloneRow.find("td.type span").text(typeLabel);

                //hide empty row
                tbody.find("tr.attributeEmptyRow").hide();

                //append cloned row
                cloneRow.removeClass("attributeCloneRow").addClass("attribute");
                tbody.append(cloneRow);

                //hide modal
                Ektron.Commerce.ProductTypes.Attributes.Modal.hide();

                //re-initialize json data array
                Ektron.Commerce.ProductTypes.Attributes.Data.update();
            }
        },
        bindEvents: function() {
            //ensure numerics only in numeric edit field
            $ektron("div.EktronAttributes input.numeric")
	            .bind("keypress", function(evt) {
	                evt = (evt) ? evt : window.event;
	                var charCode = (evt.which !== null) ? evt.which : evt.keyCode;
	                return (charCode < 32 || (charCode >= 48 && charCode <= 57));
	            });

            //bind hover to modal close button
            $ektron(".ektronModalClose").hover(
	            function() {
	                $ektron(this).addClass("ui-state-hover");
	            },
	            function() {
	                $ektron(this).removeClass("ui-state-hover");
	            }
	        );
        },
        Buttons: {
            cancel: function(el) {
                var button = $ektron(el).parent();
                //toggle ok & cancel, hide mark for delete & restore
                button.parent().find("a.edit").show();
                button.parent().find("a.markForDelete").show();
                button.parent().find("a.restore").hide();
                button.parent().find("a.ok").hide();
                button.parent().find("a.cancel").hide();

                //toggle input and span
                var wrapper = button.parents("td.value").children("p.data");
                wrapper.removeClass("edit");
                wrapper.children("span").show();
                wrapper.children("input").hide();
                wrapper.children("select").hide();
            },
            edit: function(el) {
                //cancel all other rows in edit mode
                $ektron("div.EktronAttributes p.actions a.cancel").trigger("click");
                var type = $ektron(el).parents("tr.attribute").find("input.type").attr("value");
                if (type.toLowerCase() == "date") {
                    Ektron.Commerce.ProductTypes.Attributes.DatePicker.element = $ektron(el);

                    var currentDate = $ektron(el).parents("td.value").find("input.value").attr("value");
                    $ektron("#EktronAttributesDatePickerData").attr("value", currentDate);

                    Ektron.Commerce.ProductTypes.Attributes.Modal.showDate();
                } else {
                    var button = $ektron(el).parent();
                    //toggle ok & cancel, hide mark for delete & restore
                    button.parent().find("a.edit").hide();
                    button.parent().find("a.markForDelete").hide();
                    button.parent().find("a.restore").hide();
                    button.parent().find("a.ok").show();
                    button.parent().find("a.cancel").show();

                    //toggle input and span
                    var wrapper = button.parents("td.value").children("p.data");
                    wrapper.addClass("edit");
                    wrapper.children("span").hide();
                    wrapper.children("input").show();
                    wrapper.children("select").show();
                }
            },
            markedForDelete: function(el) {

                alert(Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.deleteAttibuteMessage);
                //get clicked button
                var button = $ektron(el).parent();

                //update data value
                button.parents("td.value").find("input.markedForDelete").val("true");

                //toggle ok & cancel, hide mark for delete & restore
                button.parent().find("a.edit").hide();
                button.parent().find("a.markForDelete").hide();
                button.parent().find("a.restore").show();
                button.parent().find("a.ok").hide();
                button.parent().find("a.cancel").hide();

                //toggle input and span
                var wrapper = button.parents("td.value").children("p.data");
                wrapper.parents("tr.attribute").find("span").addClass("markedForDelete").show();
                wrapper.children("input").hide();
                wrapper.children("select").hide();

                //re-initialize json data array
                Ektron.Commerce.ProductTypes.Attributes.Data.update();
            },
            ok: function(el) {
                //get clicked button
                var button = $ektron(el).parent();

                //update data value
                var valueWrapper = button.next();
                var value = (valueWrapper.find("input").length > 0) ? valueWrapper.find("input").val() : valueWrapper.find("select option:selected").val();
                
				if (value.indexOf('>') > -1 || value.indexOf('<') > -1 )
                {
                    alert(Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings.cannotContainSpecialCharacters);
                    return false;
                }
				
				if (value !== "") {
                    button.parents("td.value").find("input.value")
	                    .attr("value", Ektron.Commerce.ProductTypes.Attributes.escapeAndEncode(value))
	                    .attr("title", Ektron.Commerce.ProductTypes.Attributes.escapeAndEncode(value));
                    value = value == "false" ? "No" : value;
                    value = value == "true" ? "Yes" : value;
                    button.parents("td.value").find("p.data span.view").text(value);
                }

                //toggle ok & cancel, hide mark for delete & restore
                button.parent().find("a.edit").show();
                button.parent().find("a.markForDelete").show();
                button.parent().find("a.restore").hide();
                button.parent().find("a.ok").hide();
                button.parent().find("a.cancel").hide();

                //toggle input and span
                var wrapper = button.parents("td.value").children("p.data");
                wrapper.removeClass("edit");
                wrapper.children("span").toggle();
                wrapper.children("input").toggle();
                wrapper.children("select").toggle();

                //re-initialize json data array
                Ektron.Commerce.ProductTypes.Attributes.Data.update();
            },
            restore: function(el) {
                //get clicked button
                var button = $ektron(el).parent();

                //update data value
                button.parents("td.value").find("input.markedForDelete").val("false");

                //toggle ok & cancel, hide mark for delete & restore
                button.parent().find("a.edit").show();
                button.parent().find("a.markForDelete").show();
                button.parent().find("a.restore").hide();
                button.parent().find("a.ok").hide();
                button.parent().find("a.cancel").hide();

                //toggle input and span
                var wrapper = button.parents("td.value").children("p.data");
                wrapper.parents("tr.attribute").find("span").removeClass("markedForDelete").show();
                wrapper.children("input").hide();
                wrapper.children("select").hide();

                //re-initialize json data array
                Ektron.Commerce.ProductTypes.Attributes.Data.update();
            },
            setDate: function(el) {
                var localizedDateFormat = $ektron("div.EktronAttributes input.dateFormat").attr("value");
                var date = formatDate($ektron("div#EktronAttributesDatePicker").datepicker("getDate"), localizedDateFormat);
                var wrapper = Ektron.Commerce.ProductTypes.Attributes.DatePicker.element.parents("td.value");
                wrapper.find("p.data span.view").text(String(date));
                wrapper.find("input.value").attr("value", String(date));
                Ektron.Commerce.ProductTypes.Attributes.Modal.hide();

                //re-initialize json data array
                Ektron.Commerce.ProductTypes.Attributes.Data.update();
            }
        },
        DatePicker: {
            element: {} //set by Ektron.Commerce.ProductTypes.Attributes.Buttons.edit "edit" button clicked to edit date field
        },
        Data: {
            init: function() {
                //initialize Json data as array
                var data = new Array();

                //populate Json data
                $ektron("div.EktronAttributes table tbody tr.attribute").each(function(i) {

                    //retrieve json values
                    var id = $ektron(this).find("input.id").val();
                    var name = $ektron(this).find("input.name").val();
                    var type = $ektron(this).find("input.type").val();
                    var value = $ektron(this).find("input.value").val();
                    var markedForDelete = $ektron(this).find("input.markedForDelete").val();

                    //create json object
                    var AttributeData = {
                        Order: i,
                        Id: id,
                        Name: name,
                        Type: type,
                        Value: value,
                        MarkedForDelete: markedForDelete
                    };

                    //populate json data array with item values
                    data.push(AttributeData);
                });

                //populate items input field with stringified json data
                $ektron("div.EktronAttributes input.attributeData").val(Ektron.JSON.stringify(data));

                //alert($ektron("div.EktronAttributes input.attributeData").val());
            },
            update: function() {
                //re-initialize json data array
                Ektron.Commerce.ProductTypes.Attributes.Data.init();
            }
        },
        escapeAndEncode: function(string) {
            return string
		        .replace(/&/g, "&amp;")
		        .replace(/</g, "&lt;")
		        .replace(/>/g, "&gt;")
		        .replace(/'/g, "\'")
		        .replace(/\"/g, "\"");
        },
        init: function() {
            //get localized strings
            var stringsField = $ektron("div.EktronAttributes input.localizedStrings");
            if (stringsField && "undefined" != typeof stringsField && stringsField.length) {
                Ektron.Commerce.ProductTypes.Attributes.LocalizedStrings = Ektron.JSON.parse(stringsField.attr("value"));
            }

            //bind events
            Ektron.Commerce.ProductTypes.Attributes.bindEvents();

            //stripe rows
            //Ektron.Commerce.ProductTypes.Items.DefaultView.stripe();

            //initialize modal
            Ektron.Commerce.ProductTypes.Attributes.Modal.init();

            //initialize json data array
            Ektron.Commerce.ProductTypes.Attributes.Data.init();
        },
        LocalizedStrings: {},
        Modal: {
            clearFields: function() {
                $ektron('#AttributesModal').find("input").val("");
            },
            init: function() {
                $ektron('#AttributesModal').drag('.itemsModalHeader');
                $ektron('#AttributesModal').modal({
                    modal: true,
                    overlay: 0,
                    toTop: true,
                    onShow: function(hash) {
                        //cancel all other rows in edit mode
                        $ektron("div.EktronAttributes p.actions a.cancel").each(function(i) {
                            var row = $ektron(this).parents("tr.attribute");
                            var okButtonDisplay = row.find("td.value p.actions a.ok").css("display");
                            var cancelButtonDisplay = row.find("td.value p.actions a.cancel").css("display");
                            if (okButtonDisplay != "none" && cancelButtonDisplay != "none") {
                                $ektron(this).trigger("click");
                            }
                        });
                        hash.o.fadeTo("fast", 0.5, function() {
                            var originalWidth = hash.w.width();
                            hash.w.find("h4").css("width", originalWidth + "px");
                            var width = "-" + String(originalWidth / 2) + "px";
                            hash.w.css("margin-left", width);
                            hash.w.fadeIn("fast");
                        });
                    },
                    onHide: function(hash) {
                        //ensure date selector row is hidden by default, and
                        //ensure field selector row is shown by default
                        var displayShowRow = $ektron.browser.msie === true ? "block" : "table-row";
                        $ektron("#AttributesModal tr.fields").css("display", displayShowRow);
                        $ektron("#AttributesModal tr.dateSelector").css("display", "none");
                        $ektron("#AttributesModal a.ok").css("display", "block");
                        $ektron("#AttributesModal a.setDate").hide();
                        $ektron(".hasDatepicker").hide();
                        $ektron("#AttributesModal thead").show();
                        $ektron("#AttributesModal table.ektronGrid").removeClass("noBorder");
                        hash.w.fadeOut("fast");
                        hash.o.fadeOut("fast", function() {
                            if (hash.o) {
                                hash.o.remove();
                            }
                            //clear input fields
                            Ektron.Commerce.ProductTypes.Attributes.Modal.clearFields();
                        });
                    }
                });
                $ektron('#AttributesModal').find("h3 img").bind("click", function() {
                    Ektron.Commerce.ProductTypes.Items.Modal.hide();
                });
            },
            hide: function() {
                $ektron('#AttributesModal').modalHide();
            },
            show: function() {
                $ektron('#AttributesModal').modalShow();
            },
            showDate: function() {
                //ensure date selector row is shown, and
                //ensure field selector row is hidden
                var displayShowRow = $ektron.browser.msie === true ? "block" : "table-row";
                $ektron("#AttributesModal tr.fields").css("display", "none");
                $ektron("#AttributesModal tr.dateSelector").css("display", displayShowRow);
                $ektron("#AttributesModal a.ok").hide();
                $ektron("#AttributesModal a.setDate").css("display", "block");
                $ektron("#AttributesModal thead").hide();
                $ektron(".hasDatepicker").show();
                $ektron("#AttributesModal table.ektronGrid").addClass("noBorder");

                var localizedDateFormat = $ektron("div.EktronAttributes input.dateFormat").attr("value");
                localizedDateFormat = localizedDateFormat.replace(new RegExp(/\//g), "-");
                //set minimum date to be today's date minus one year.
                var minYear = dateFormat(new Date(), "yyyy");
                $ektron("div#EktronAttributesDatePicker").datepicker({
                    dateFormat: localizedDateFormat.toLowerCase(),
                    minDate: new Date((minYear - 1), 1 - 1, 1),
                    changeMonth: true,
                    changeYear: true
                });

                //set datepicker to end date - convert date from whatever format to US format MM/dd/yyyy
                localizedDateFormat = localizedDateFormat.replace(new RegExp(/-/g), "/");
                var startDate = formatDate(new Date(getDateFromFormat($ektron("div.EktronAttributes input.todaysDate").attr("value"), localizedDateFormat)), "MM/dd/yyyy");
                $ektron("div#EktronAttributesDatePicker").datepicker("setDate", new Date(startDate));  //requires US format


                Ektron.Commerce.ProductTypes.Attributes.Modal.show();
            }
        }
    };
}