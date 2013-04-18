Ektron.ready(function() {
	Ektron.Commerce.Coupons.Type.Percent.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
	    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.Type.Percent.initAfterAjax);
	}
});

//define Ektron object only if it's not already defined
if (Ektron === undefined) {
	Ektron = {};
}

//define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce === undefined) {
	Ektron.Commerce = {};
}

//define Ektron.Commerce.Coupons object only if it's not already defined
if (Ektron.Commerce.Coupons === undefined) {
	Ektron.Commerce.Coupons = {};
}

//define Ektron.Commerce.Coupons.Type object only if it's not already defined
if (Ektron.Commerce.Coupons.Type === undefined) {
	Ektron.Commerce.Coupons.Type = {};
}

//define Ektron.Commerce.Coupons.Type.Percent object only if it's not already defined
if (Ektron.Commerce.Coupons.Type.Percent === undefined) {
	//Ektron.Commerce.Coupons.Type.Percent Object
	Ektron.Commerce.Coupons.Type.Percent = {
	    bindEvents: function(){
	        var decimalFields = $ektron("div.percent input.hundreds, div.percent input.hundredths, div.percent input.dollars, div.percent input.cents");
			
			if (decimalFields.length > 0) {
			    //enforce numeric-only entries from pasted values
			    Ektron.Commerce.Coupons.Type.Amount.Validation.enforceValidation(decimalFields[0]);
			    Ektron.Commerce.Coupons.Type.Amount.Validation.enforceValidation(decimalFields[1]);
			    Ektron.Commerce.Coupons.Type.Amount.Validation.enforceValidation(decimalFields[2]);
			    Ektron.Commerce.Coupons.Type.Amount.Validation.enforceValidation(decimalFields[3]);
            }

	        //validate code on keypress/keydown depending on browser
	        //ensure numerics only in dollar edit field & two digits only in cents feild
			if (!$ektron.browser.msie) {
			    decimalFields.bind("keypress", function(e){
			    
                    var charCode;
				    var returnValue = true;
				    var e = (e) ? e : window.event;
                    var charCode = (e.which != null) ? e.which : e.keyCode;
                    
                    //allow ctrl+x and ctrl+v
                    if ((e.ctrlKey && charCode == 118) || (e.ctrlKey && charCode == 120)) {
                        return true;
                    } else {
    				    //charCode 8 == BACKSPACE, 
    				    //e.keyCode 8 == TAB, 
    				    //charCode 48-58 == [0-9],
    				    //charCode 91-106 == num lock + [0-9],
    				    //charCode 37 == Arrow Left, 
    				    //charCode 39 == Arrow Right, 
    				    //charCode 9 == TAB
                        if ((charCode == 8) || (e.keyCode == 9) || (charCode == 9) || (charCode >=48 && charCode <= 57)) {
                            return charCode;
                        } else {
                            return false;
                        }
    				}
			    });
			} else {
			    decimalFields.bind("keydown", function(e){
			        var charCode;
				    var returnValue = true;
				    var e = (e) ? e : window.event;
                    var charCode = (e.which != null) ? e.which : e.keyCode;
			        
			        //true ctrl+x and ctrl+v
			        if ((e.ctrlKey && charCode == 88) || (e.ctrlKey && charCode == 86)) {
                        return true;
                    } else {
                        if (e.shiftKey) {
                            //do not allow shift+numeric keys
                            return false;
                        }
    				    //charCode 8 == BACKSPACE, charCode 48-58 == [0-9],charCode 37 == Arrow Left, charCode 39 == Arrow Right, charCode 9 == TAB
                        if ((charCode == 8) || (charCode == 9) || (charCode >=48 && charCode <= 57) || (charCode == 37) ||(charCode == 39) ||(charCode >=91 && charCode <= 105)) {
                            return true;
                        } else {
                            return false;
                        }
    			    }
			    });
			}
	       
	        //validate entry
            decimalFields.bind("keyup", function(e){
                var charCode;
                var returnValue = true;
                var e = (e) ? e : window.event;
                var charCode = (e.which != null) ? e.which : e.keyCode;
                
                //if user presses enter to clear alert below, just exit.
                if (charCode == 13)
                    return;
                
                //determine max amount
                Ektron.Commerce.Coupons.Type.Percent.Validation.determineMaximumAmount(e.target);
            });
	        
	        //bind to custom event get validation status, and return validation status
            $ektron(document).bind("GetCouponValidationStatus", function(e) {
                $ektron(document).trigger("ValidationStatusChange", ["Percent", Ektron.Commerce.Coupons.Type.Percent.Validation.status]);
            });
	    },
		init: function(){
			//complete init
			Ektron.Commerce.Coupons.Type.Percent.initAfterAjax();
		},
		initAfterAjax: function(){
		    //get localized strings
		    var localizedStringsInput = $ektron("div.percent input.percentLocalizedStrings");
		    if (localizedStringsInput != null && localizedStringsInput.length > 0) {
		        localizedStrings = localizedStringsInput.attr("value");
                Ektron.Commerce.Coupons.Type.Percent.Validation.LocalizedStrings = Ektron.JSON.parse(localizedStrings);
		    }
		    
		    //bind events
		    Ektron.Commerce.Coupons.Type.Percent.bindEvents();
		    
		    //set validation status
		    var charCode;//leave this as undefined
		    var status = "valid";
		    Ektron.Commerce.Coupons.Type.Percent.Validation.setValidationStatus(status);
		    
		    //set initialized flag
            var initialized = $ektron("div.percent input.initialized");
            if (initialized.length > 0) {
                initialized.attr("value", "true");
            }
            
            //ensure ektron grid is initialized
            Ektron.Workarea.Grids.show()
		},
		Validation: {
		    LocalizedStrings: {},
		    setValidationStatus: function(status) {		        
		        Ektron.Commerce.Coupons.Type.Percent.Validation.status = status;
		        $ektron(document).trigger("ValidationStatusChange", ["Percent", Ektron.Commerce.Coupons.Type.Percent.Validation.status]);
		    },
		    determineMaximumAmount: function(elem){
		        var hundreds = $ektron("div.percent input.hundreds").attr("value");
                var hundreths = $ektron("div.percent input.hundreths").attr("value");
                var dollars = $ektron("div.percent input.dollars").attr("value");
                var cents = $ektron("div.percent input.cents").attr("value");
                
                //get input field (either dollars or cents)
                var inputField = $ektron(elem);
                var inputType;
                inputType = inputField.hasClass("hundreds") == true ? "hundreds" : false;
                if (inputType == false)
                    inputType = inputField.hasClass("hundredths") == true ? "hundredths" : false;
                if (inputType == false)
                    inputType = inputField.hasClass("dollars") == true ? "dollars" : false;
                if (inputType == false)
                    inputType = inputField.hasClass("cents") == true ? "cents" : false;
                
                switch(inputType) {
                    case "hundreds":
                        hundreds = $ektron(elem).attr("value");
                        break;
                    case "hundredths":
                        hundreths = $ektron(elem).attr("value");
                        break;
                    case "dollars":
                        dollars = $ektron(elem).attr("value");
                        break;
                    case "cents":
                        cents = $ektron(elem).attr("value");
                        break;  
                }
                
                var percentage = parseFloat(hundreds + "." + hundreths);
                if (percentage > 100 && (inputType == "hundreds" || inputType == "hundredths")) {
                    alert(Ektron.Commerce.Coupons.Type.Percent.Validation.LocalizedStrings.over100percentWarning);
                }
                
                var maxAmount = parseFloat(dollars + "." + cents);
                if (maxAmount > 0) {
                    var maximumDiscount = (percentage / 100) * maxAmount;
                    maximumDiscount = Math.round(maximumDiscount*Math.pow(10, 2))/Math.pow(10, 2);
                    maximumDiscount = String(maximumDiscount);
                    if (maximumDiscount.indexOf(".") == -1) {
                        maximumDiscount = maximumDiscount + ".00";
                    } else {
                        if (maximumDiscount.substring(maximumDiscount.indexOf(".")).length == 2)
                            maximumDiscount = maximumDiscount + "0";
                    }
                    $ektron("div.percent td.content div.maxAmountMessage span.calculation")
                        .text("¤" + maximumDiscount);
                    $ektron("div.percent td.content div.maxAmountMessage").slideDown();
                } else {
                    $ektron("div.percent td.content div.maxAmountMessage").slideUp();
                }
		    },
		    enforceValidation: function(decimalField){
		        decimalField = $ektron(decimalField);
		        setInterval(function(){
		            //ensure description does not contain invalid chars
                    var number = decimalField.attr("value");
                    var validatedNumber = ""
                    if (number != undefined) {
                        var valid = true;
                        validatedNumber = number.replace(/[^0-9]/g, function(e){
                            valid = false;
                            return "";
                        });
                        
                        if (validatedNumber != number){
                            decimalField.attr("value", validatedNumber);
                            //determine max amount
                            Ektron.Commerce.Coupons.Type.Percent.Validation.determineMaximumAmount(decimalField[0]);
                    
                            if (valid == false & $ektron("span#EktronCommerceCouponsInvalidCharacterMessage").length == 0){
                                decimalField.parent().append("<span id=\"EktronCommerceCouponsInvalidCharacterMessage\" style=\"margin-left:.25em;font-weight:bold;color:red;\">Invalid characters have been removed</span>");
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
		}
	}
}
