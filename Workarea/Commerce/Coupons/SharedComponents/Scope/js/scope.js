Ektron.ready(function() {
	Ektron.Commerce.Coupons.Scope.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
	   Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.Scope.initAfterAjax);
    }
});

//define Ektron object only if it"s not already defined
if (Ektron === undefined) {
	Ektron = {};
}

//define Ektron.Commerce object only if it"s not already defined
if (Ektron.Commerce === undefined) {
	Ektron.Commerce = {};
}

//define Ektron.Commerce.Coupons object only if it"s not already defined
if (Ektron.Commerce.Coupons === undefined) {
	Ektron.Commerce.Coupons = {};
}

//define Ektron.Commerce.Coupons.Scope.IncludedProducts object only if it"s not already defined
if (Ektron.Commerce.Coupons.Scope === undefined) {
	//Ektron.Commerce.Coupons.Scope.IncludedProducts Object
	Ektron.Commerce.Coupons.Scope = {
	    bindCustom: function(obj, type, fn) {
	        if (obj.attachEvent) {
                obj['e'+type+fn] = fn;
                obj[type+fn] = function(){obj['e'+type+fn]( window.event );}
                obj.attachEvent( 'on'+type, obj[type+fn] );
            } else {
                obj.addEventListener( type, fn, false );
            }
        },
	    bindEvents: function(){
	        $ektron(".ektronModalClose").hover(
                function(){
                    $ektron(this).addClass("ui-state-hover");
                },
                function(){
                    $ektron(this).removeClass("ui-state-hover");
                }
            );
	    
	        //auto-validate max redemptions and minimum required basket value
			var numericFields = $ektron("div.scope input.dollars, div.scope input.cents, div.scope input.maxRedemptions");
            
            if (numericFields.length > 0) {
			    //enforce numeric-only entries from pasted values
			    Ektron.Commerce.Coupons.Scope.Validation.enforceValidation(numericFields[0]);
			    Ektron.Commerce.Coupons.Scope.Validation.enforceValidation(numericFields[1]);
			    Ektron.Commerce.Coupons.Scope.Validation.enforceValidation(numericFields[2]);
            }
            
            //validate code on keypress/keydown depending on browser
			if (!$ektron.browser.msie) {
			    numericFields.bind("keypress", function(e){

                    var charCode;
				    var returnValue = true;
				    var e = (e) ? e : window.event;
                    var charCode = (e.which != null) ? e.which : e.keyCode;

                    //allow ctrl+x and ctrl+v
                    if ((e.ctrlKey && charCode == 118) || (e.ctrlKey && charCode == 120)) {
                        return true;
                    } else {
    				    //charCode 8 == BACKSPACE, charCode 48-58 == [0-9],charCode 37 == Arrow Left, charCode 39 == Arrow Right, charCode 9 == TAB
                        if (charCode == 0 && "undefined" !== typeof(evt.keyCode)) //for some reason FF is not getting evt.which for several chars, try keyCode if possible
                            charCode = evt.keyCode;
                        if ((charCode == 8) || (charCode >=48 && charCode <= 57)) {
                            return charCode;
                        } else {
                            return false;
                        }
    				}
			    });
			} else {
			    numericFields.bind("keydown", function(e){

			        var charCode;
				    var returnValue = true;
				    var e = (e) ? e : window.event;
                    var charCode = (e.which != null) ? e.which : e.keyCode;

			        //allow ctrl+x and ctrl+v
			        if ((e.ctrlKey && charCode == 88) || (e.ctrlKey && charCode == 86)) {
			            return true;
                    } else {
                        if (!e.ctrlKey)
    				    //charCode 8 == BACKSPACE, charCode 48-58 == [0-9],charCode 37 == Arrow Left, charCode 39 == Arrow Right, charCode 9 == TAB
    				    //charCode 96-105 = [0-9] on keypad
                        if ((charCode == 8) || ((!e.shiftKey) && (charCode >=48 && charCode <= 57))
                             || (charCode == 37) || (charCode == 39) || (charCode == 9)
                             || (charCode >=96 && charCode <= 105)) {
                            return true;
                        } else {
                            return false;
                        }
    			    }
			    });
			}
	    },
	    Dates: {
	        clear: function(elem, type){
	            var button = $ektron(elem);
	            //hide clear button
	            button.addClass("hide");
                if ("start" == type){
	                //clear display
	                Ektron.Commerce.Coupons.Scope.setDisplayedStartTime("12:00 AM");
	                var todaysDate = $ektron("div.scope table tbody tr.startDate td.content input.todaysDate").attr("value");
		            var localizedDateFormat = $ektron("div.scope input.dateFormat").attr("value").toLowerCase();
	                var date = dateFormat(todaysDate, localizedDateFormat);
	                Ektron.Commerce.Coupons.Scope.setDisplayedStartDate(date);

		            //clear hidden fields
	                Ektron.Commerce.Coupons.Scope.setStartTime("12:00 AM");
	                Ektron.Commerce.Coupons.Scope.setStartDate(todaysDate);

	                //validate
	                Ektron.Commerce.Coupons.Scope.Dates.validate("start", todaysDate, "12:00 AM");
	            }
	            else{	            
	                //clear display
	                Ektron.Commerce.Coupons.Scope.setDisplayedEndTime("12:00 AM");
	                var todaysDate = new Date();
		            todaysDate.setFullYear(todaysDate.getFullYear() + 10);
		            var localizedDateFormat = $ektron("div.scope input.dateFormat").attr("value").toLowerCase();
	                var date = dateFormat(todaysDate, localizedDateFormat);
	                Ektron.Commerce.Coupons.Scope.setDisplayedEndDate(date);

		            //clear hidden fields
	                Ektron.Commerce.Coupons.Scope.setEndTime("12:00 AM");
	                Ektron.Commerce.Coupons.Scope.setEndDate(date);

	                //validate
	                Ektron.Commerce.Coupons.Scope.Dates.validate("end", date, "12:00 AM");
	            }
	        },
	        validate: function(startOrEnd, date, time){
	            var startDate;
	            var startTime;
	            var endDate;
	            var endTime;

	            //get dates
	            switch(startOrEnd){
		            case "start":
		                startDate = date;
		                startTime = time;
		                endDate = Ektron.Commerce.Coupons.Scope.getEndDate();
		                endTime = Ektron.Commerce.Coupons.Scope.getEndTime();
		                break;
		            case "end":
		                startDate = Ektron.Commerce.Coupons.Scope.getStartDate();
		                startTime = Ektron.Commerce.Coupons.Scope.getStartTime();
		                endDate = date;
		                endTime = time;
		                break;
		        }
                
                var dateMask = $ektron("input.dateFormat").attr("value");
                var timeMask = "h:mm a";
                
                var dateStart = getDateFromFormat(startDate, dateMask);
                var dateEnd = getDateFromFormat(endDate, dateMask);
            
                var result = true;
                if (dateStart > dateEnd) {
                    result = false;
                } else if (dateStart == dateEnd) {
                    var timeComparison = compareDates(startTime, timeMask, endTime, timeMask);
                    if (timeComparison === 1) {
                        result = false;
                    }
                }

		        if (result == false) {
		            //end date is less than start date
		            $ektron(document).trigger("ValidationStatusChange", ["Scope", "invalid"]);
		            $ektron("div.scope tr.startDate td.content span, div.scope tr.endDate td.content span").addClass("error");
		            alert(Ektron.Commerce.Coupons.Scope.LocalizedStrings.invalidStartEndDateMessage);
		        } else {
		            //end date is greater than or equal to start date
		            $ektron("div.scope tr.startDate td.content span, div.scope tr.endDate td.content span").removeClass("error");
		            $ektron(document).trigger("ValidationStatusChange", ["Scope", "valid"]);
		        }
	        }
	    },
		init: function(){
		    //bind to custom event get validation status, and return validation status
            $ektron(document).bind("GetCouponValidationStatus", function(e) {
                $ektron(document).trigger("ValidationStatusChange", ["Scope", "valid"]);
            });

		    //complete init
		    Ektron.Commerce.Coupons.Scope.initAfterAjax();
		},
		initAfterAjax: function(){
		    //de-serialize localized strings
		    var localizedStringsInput = $ektron("div.scope input.scopeLocalizedStrings");
		    if (localizedStringsInput != null && localizedStringsInput.length > 0) {
		        var localizedStrings = localizedStringsInput.attr("value");
                Ektron.Commerce.Coupons.Scope.LocalizedStrings = Ektron.JSON.parse(localizedStrings);
		    }

		    //bind events
		    Ektron.Commerce.Coupons.Scope.bindEvents();

		    //init modal
		    Ektron.Commerce.Coupons.Scope.Modal.init();

            // show start and expiration dates & times
            Ektron.Commerce.Coupons.Scope.showCouponDateTime();

            //set initialized flag
            var initialized = $ektron("div.scope input.initialized");
            if (initialized.length > 0) {
                initialized.attr("value", "true");
            }
            
            //re-initialize ektron grid
            Ektron.Workarea.Grids.show();
		},
		Modal: {
		    clearFields: function(){
	            $ektron("#EktronCouponsScopeModalDatePicker").find("input").val("");
	        },
	        init: function(){
	            $ektron(".ui-widget-header .ektronModalClose").hover(
		            function(){
		                $ektron(this).addClass("ui-state-hover");
		            },
		            function(){
		                $ektron(this).removeClass("ui-state-hover");
		            }
		        );
	        
			    $ektron("#EktronCouponsScopeModalDatePicker").drag(".scopeModalHeader");
			    $ektron("#EktronCouponsScopeModalDatePicker").modal({
			        modal: true,
				    overlay: 0,
				    toTop: true,
			        onShow: function(hash) {
					    hash.o.fadeTo("fast", 0.5, function() {
						    var originalWidth = hash.w.width();
						    hash.w.find("h4").css("width",  originalWidth + "px");
						    var width = "-" + String(originalWidth / 2) + "px";
						    hash.w.css("margin-left", width);
						    hash.w.fadeIn("fast");
					    });
			        },
			        onHide: function(hash) {
		                hash.w.fadeOut("fast");
					    hash.o.fadeOut("fast", function(){
						    if (hash.o)
							    hash.o.remove();
						    //clear input fields
                            Ektron.Commerce.Coupons.Scope.Modal.clearFields();
					    });
		            }
			    });
		    },
		    hide: function(){
			    $ektron("#EktronCouponsScopeModalDatePicker").modalHide();
		    },
		    show: function(elem){
		        var time = "";
		        if ($ektron(elem).hasClass("start")){
		            // configure datepicker
		            Ektron.Commerce.Coupons.Scope.Modal.dateField = "start";

		            //set minimum date to be today's date minus one year.
		            var startDate = dateFormat(Ektron.Commerce.Coupons.Scope.getStartDate(), localizedDateFormat);
		            var minYear = dateFormat(startDate, "yyyy");
		            var localizedDateFormat = $ektron("div.scope input.dateFormat").attr("value");
	                $ektron("div#EktronScopeDatePicker").datepicker({
	                    dateFormat: localizedDateFormat,
	                    minDate: new Date((minYear - 1), 1 - 1, 1),
	                    changeMonth: true,
			            changeYear: true
	                });
	                
	                //set datepicker to start date - convert date from whatever format to US format MM/dd/yyyy
	                localizedDateFormat = localizedDateFormat.replace(new RegExp(/-/g),"/");
	                var setDate = formatDate(new Date(getDateFromFormat(Ektron.Commerce.Coupons.Scope.getStartDate(), localizedDateFormat)),"MM/dd/yyyy");
	                $ektron("div#EktronScopeDatePicker").datepicker("setDate", new Date(setDate));  //requires US format

	                time = Ektron.Commerce.Coupons.Scope.getStartTime();
		        }
		        else{
		            // configure datepicker
		            Ektron.Commerce.Coupons.Scope.Modal.dateField = "end";

		            //set minimum date to be today's date minus one year.
		            var startDate = dateFormat(Ektron.Commerce.Coupons.Scope.getStartDate(), localizedDateFormat);
		            var minYear = dateFormat(startDate, "yyyy");
		            var localizedDateFormat = $ektron("div.scope input.dateFormat").attr("value");
		            localizedDateFormat = localizedDateFormat.replace(new RegExp(/\//g),"-");
	                $ektron("div#EktronScopeDatePicker").datepicker({
	                    dateFormat: localizedDateFormat.toLowerCase(),
	                    minDate: new Date((minYear - 1), 1 - 1, 1),
	                    changeMonth: true,
			            changeYear: true
	                });
	                
	                //set datepicker to end date - convert date from whatever format to US format MM/dd/yyyy
	                localizedDateFormat = localizedDateFormat.replace(new RegExp(/-/g),"/");
	                var setDate = formatDate(new Date(getDateFromFormat(Ektron.Commerce.Coupons.Scope.getEndDate(), localizedDateFormat)),"MM/dd/yyyy");
	                $ektron("div#EktronScopeDatePicker").datepicker("setDate", new Date(setDate));  //requires US format

	                time = Ektron.Commerce.Coupons.Scope.getEndTime();
		        }

		        // set clock time
		        time = time == "" ? "12:00 am" : time;
                Ektron.Commerce.Coupons.Scope.setClockTimeHour(time.split(":")[0]);
                Ektron.Commerce.Coupons.Scope.setClockTimeMinute(time.split(":")[1].split(" ")[0]);
                Ektron.Commerce.Coupons.Scope.setClockTimeAmPm(time.split(":")[1].split(" ")[1]);

			    $ektron("#EktronCouponsScopeModalDatePicker").modalShow();
		    },
		    save: function(){
		        //get time
		        var hour = Ektron.Commerce.Coupons.Scope.getClockTimeHour();
		        var minute = Ektron.Commerce.Coupons.Scope.getClockTimeMinute();
		        var ampm = Ektron.Commerce.Coupons.Scope.getClockTimeAmPm();
		        var time = hour + ":" + minute + " " + ampm;

		        //get date
		        var localizedDateFormat = $ektron("div.scope input.dateFormat").attr("value").toLowerCase();
	            var date = dateFormat($ektron("div#EktronScopeDatePicker").datepicker("getDate"), localizedDateFormat);

		        //set date and time
		        switch(Ektron.Commerce.Coupons.Scope.Modal.dateField){
		            case "start":
	                    Ektron.Commerce.Coupons.Scope.setDisplayedStartDate(date);
	                    Ektron.Commerce.Coupons.Scope.setDisplayedStartTime(time);
	                    Ektron.Commerce.Coupons.Scope.setStartDate(date);
	                    Ektron.Commerce.Coupons.Scope.setStartTime(time);
	                    Ektron.Commerce.Coupons.Scope.Dates.validate("start", date, time);
		                break;
		            case "end":
	                    Ektron.Commerce.Coupons.Scope.setDisplayedEndDate(date);
	                    Ektron.Commerce.Coupons.Scope.setDisplayedEndTime(time);
	                    Ektron.Commerce.Coupons.Scope.setEndDate(date);
	                    Ektron.Commerce.Coupons.Scope.setEndTime(time);
	                    Ektron.Commerce.Coupons.Scope.Dates.validate("end", date, time);
		                break;
		        }

		        //toggle clear button
		        Ektron.Commerce.Coupons.Scope.Modal.toggleClearButton();

		        //hide modal
		        Ektron.Commerce.Coupons.Scope.Modal.hide();
		    },
		    toggleClearButton: function(){
		        switch(Ektron.Commerce.Coupons.Scope.Modal.dateField){
		            case "start":
		                var clearButton = $ektron("div.scope table tbody tr.startDate td.content a.clear");
		                clearButton.removeClass("hide");
		                break;
		            case "end":
		                var clearButton = $ektron("div.scope table tbody tr.endDate td.content a.clear");
		                clearButton.removeClass("hide");
		                break;
		        }
		    }
		},
		Validation: {
		    enforceValidation: function(numericField){
		        numericField = $ektron(numericField);
		        setInterval(function(){
		            //ensure description does not contain invalid chars
                    var number = numericField.attr("value");
                    var validatedNumber = ""
                    if (number != undefined) {
                        var valid = true;
                        validatedNumber = number.replace(/[^0-9]/g, function(e){
                            valid = false;
                            return "";
                        });
                        if (validatedNumber != number){
                            numericField.attr("value", validatedNumber);
                            if (valid == false & $ektron("span#EktronCommerceCouponsInvalidCharacterMessage").length == 0){
                                numericField.parent().append("<span id=\"EktronCommerceCouponsInvalidCharacterMessage\" style=\"margin-left:.25em;font-weight:bold;color:red;\">Invalid characters have been removed</span>");
                                setTimeout(function(){
                                    $ektron("span#EktronCommerceCouponsInvalidCharacterMessage").fadeOut("slow", function(){
                                        $ektron(this).remove();
                                    });
                                }, 1000);
                            }
                        }
                    }
		        }, 100);
		    }
		},
		getStartTime: function(){
	        return $ektron("div.scope table tbody tr.startDate td.content input.startTime").attr("value");
		},
		setStartTime: function(time){
	        $ektron("div.scope table tbody tr.startDate td.content input.startTime").attr("value", time);
		},
		getStartDate: function(){
	        return $ektron("div.scope table tbody tr.startDate td.content input.startDate").attr("value");
		},
		setStartDate: function(date){
	        $ektron("div.scope table tbody tr.startDate td.content input.startDate").attr("value", date);
		},
		getEndTime: function(){
	        return $ektron("div.scope table tbody tr.endDate td.content input.endTime").attr("value");
		},
		setEndTime: function(time){
	        $ektron("div.scope table tbody tr.endDate td.content input.endTime").attr("value", time);
		},
		getEndDate: function(){
	        return $ektron("div.scope table tbody tr.endDate td.content input.endDate").attr("value");
		},
		setEndDate: function(date){
	        $ektron("div.scope table tbody tr.endDate td.content input.endDate").attr("value", date);
		},
		getHourFromTime: function(time){
	        return time.split(":")[0];
		},
		getMinuteFromTime: function(time){
	        return time.split(":")[1].split(" ")[0];
		},
		getAmPmFromTime: function(time){
	        return time.split(":")[1].split(" ")[1];
		},
		setDisplayedStartTime: function(time){
		    $ektron("div.scope table tbody tr.startDate td.content span.time").text(time);
		},
		setDisplayedStartDate: function(date){
		    $ektron("div.scope table tbody tr.startDate td.content span.date").text(date);
		},
		setDisplayedEndTime: function(time){
		    $ektron("div.scope table tbody tr.endDate td.content span.time").text(time);
		},
		setDisplayedEndDate: function(date){
		    $ektron("div.scope table tbody tr.endDate td.content span.date").text(date);
		},
		showCouponDateTime: function(){
            if (0 == $ektron("div.scope").length)
                return;

		    Ektron.Commerce.Coupons.Scope.setDisplayedStartDate(Ektron.Commerce.Coupons.Scope.getStartDate()
		        + " " + Ektron.Commerce.Coupons.Scope.getStartTime());
		    Ektron.Commerce.Coupons.Scope.setDisplayedEndDate(Ektron.Commerce.Coupons.Scope.getEndDate()
		        + " " + Ektron.Commerce.Coupons.Scope.getEndTime());
		},
		getClockTimeHour: function(){
		    return $ektron("div#EktronCouponsScopeModalDatePicker div.time select.hour option:selected").attr("value")
		},
		setClockTimeHour: function(hour){
		    $ektron("div#EktronCouponsScopeModalDatePicker div.time select.hour").attr("value", hour)
		},
		getClockTimeMinute: function(){
		    return $ektron("div#EktronCouponsScopeModalDatePicker div.time select.minute option:selected").attr("value");
		},
		setClockTimeMinute: function(minute){
		    $ektron("div#EktronCouponsScopeModalDatePicker div.time select.minute").attr("value", minute);
		},
		getClockTimeAmPm: function(){
		    return $ektron("div#EktronCouponsScopeModalDatePicker div.time select.ampm option:selected").attr("value");
		},
		setClockTimeAmPm: function(ampm){
		    if ("am" == ampm.toLowerCase())
		        $ektron("div#EktronCouponsScopeModalDatePicker div.time select.ampm").attr("value", "AM");
		    else
		        $ektron("div#EktronCouponsScopeModalDatePicker div.time select.ampm").attr("value", "PM");
		}
	}
}