Ektron.ready(function() {
	Ektron.Commerce.Coupons.Type.Amount.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
	    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.Type.Amount.initAfterAjax);
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

//define Ektron.Commerce.Coupons.Type.Amount object only if it's not already defined
if (Ektron.Commerce.Coupons.Type.Amount === undefined) {
	//Ektron.Commerce.Coupons.Type.Amount Object
	Ektron.Commerce.Coupons.Type.Amount = {
	    bindEvents: function(){
	        var decimalFields = $ektron("div.amount input.leftOfDecimal, div.amount input.rightOfDecimal");
			decimalFields.unbind();
			
			//enforce numeric-only entries from pasted values
			Ektron.Commerce.Coupons.Type.Amount.Validation.enforceValidation(decimalFields[0]);
			Ektron.Commerce.Coupons.Type.Amount.Validation.enforceValidation(decimalFields[1]);		
	    
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
    				    //charCode 48-58 == [0-9],
    				    //charCode 91-106 == num lock + [0-9],
    				    //charCode 37 == Arrow Left, 
    				    //charCode 39 == Arrow Right, 
    				    //charCode 9 == TAB
                        if ((charCode == 8) || (charCode == 9) || (charCode >=48 && charCode <= 57)) {
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
			
			        //allow ctrl+x and ctrl+v
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
	       
	        //bind to custom event get validation status, and return validation status
            $ektron(document).bind("GetCouponValidationStatus", function(e) {
                $ektron(document).trigger("ValidationStatusChange", ["Amount", Ektron.Commerce.Coupons.Type.Amount.Validation.status]);
            });
	    },
		init: function(){
			//complete init
			Ektron.Commerce.Coupons.Type.Amount.initAfterAjax();
		},
		initAfterAjax: function(){
		    //bind events
		    Ektron.Commerce.Coupons.Type.Amount.bindEvents();
		    
		    //set initialized flag
            var initialized = $ektron("div.amount input.initialized");
            if (initialized.length > 0) {
                initialized.attr("value", "true");
            }
		    
		    //set validation status
		    var status = "valid";
		    Ektron.Commerce.Coupons.Type.Amount.Validation.setValidationStatus(status);
		    
		    //re-initialize Ektron Grid
		    Ektron.Workarea.Grids.show();
		},
		Validation: {
		    setValidationStatus: function(status) {		        
		        Ektron.Commerce.Coupons.Type.Amount.Validation.status = status;
		        $ektron(document).trigger("ValidationStatusChange", ["Amount", Ektron.Commerce.Coupons.Type.Amount.Validation.status]);
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
                        if (validatedNumber != number) {
                            decimalField.attr("value", validatedNumber);
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