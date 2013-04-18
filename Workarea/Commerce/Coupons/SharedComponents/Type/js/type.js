Ektron.ready(function() {
	Ektron.Commerce.Coupons.Type.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
	    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.Type.initAfterAjax);
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

//define Ektron.Commerce.Coupons.Type.Shipping object only if it's not already defined
if (Ektron.Commerce.Coupons.Type === undefined) {
	//Ektron.Commerce.Coupons.Type.Shipping Object
	Ektron.Commerce.Coupons.Type = {
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
		    //code field
		    var codeField = $ektron("div.type table tbody tr.code td input.code");
		    
		    //code field - keydown: set control validation state
            codeField.bind("keydown", function(e){
                var validKey = Ektron.Commerce.Coupons.Type.Validation.Code.verifyKey(e);
                if (validKey) {
                    Ektron.Commerce.Coupons.Type.Validation.Code.disableCodeStatus();
                    Ektron.Commerce.Coupons.Type.Validation.setControlStatus("invalid");
			        $ektron("div.type table tbody tr.code td input.codeValidate").removeAttr("disabled");
			    } else {
			        if (event.preventDefault) event.preventDefault( );
                    else event.returnValue = false;
			    }
		    });
		    
		    if (codeField.length > 0) {
		        //code field - prevent paste
		        Ektron.Commerce.Coupons.Type.bindCustom(codeField[0], 'paste', function(event) {
		            Ektron.Commerce.Coupons.Type.Validation.setControlStatus("invalid");
			        $ektron("div.type table tbody tr.code td input.codeValidate").removeAttr("disabled");
                });
		    }
		    
		    //validation button - validate code and set control validation state
            $ektron("div.type table tbody tr.code td input.codeValidate").bind("click", function(e){
                $ektron("div.type table tbody tr.code td input.codeValidate").attr("disabled", "disabled");
                //code is not set - set validation status to invalid, and disable validate button
			    Ektron.Commerce.Coupons.Type.Validation.setControlStatus("invalid");
                Ektron.Commerce.Coupons.Type.Validation.Code.validate(e);
		    });
		    
		    //description field
		    var descriptionField = $ektron("div.type tr.description input.description");
		    
		    //description field - keydown: prevent angle-brackets
		    descriptionField.bind("keydown", function(e){
		        var result = Ektron.Commerce.Coupons.Type.Validation.Description.validate(e);
		        if (result == false) {
                    if (e.preventDefault) e.preventDefault();
                    else e.returnValue = false;
                }
		    });
			
			if (descriptionField.length > 0) {
			    setInterval(function(){
			        //ensure description does not contain invalid chars
                    var description = descriptionField.attr("value");
                    var validatedDescription = "";
                    if (description != undefined) {
                        var valid = true;
                        validatedDescription = description.replace(/[<>]/g, function(e){
                            valid = false;
                            return "";
                        });
                        if (validatedDescription != description) {
                            descriptionField.attr("value", validatedDescription);
                            if (valid == false & $ektron("span#EktronCommerceCouponsInvalidCharacterMessage").length == 0){
                                descriptionField.parent().append("<span id=\"EktronCommerceCouponsInvalidCharacterMessage\" style=\"margin-left:.25em;font-weight:bold;color:red;\">Invalid characters have been removed</span>");
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
			
			//bind to custom event get validation status, and return validation status
			$ektron(document).bind("GetCouponValidationStatus", function(e){
				$ektron(document).trigger("ValidationStatusChange", ["Type", Ektron.Commerce.Coupons.Type.Validation.status]);
			});
			
			//ensure currency select is disabled if percentage type is selected
			$ektron("div.type span.typeChoice input:radio").bind("click", function(){
			    var publishedCouponType = $ektron("div.type input.couponTypePublished").attr("value");
			    if ("undefined" != typeof(publishedCouponType)) {
			        alert(Ektron.Commerce.Coupons.Type.LocalizedStrings.typeChangeAlert);
			    }
			    Ektron.Commerce.Coupons.Type.Validation.Currency.select();
			});
		},
		init: function(){
		    //eliminate form autocomplete
		    $ektron("div.type input.code").attr("autocomplete", "off");
		    
			//de-serialize localized strings
			var localizedStringsInput = $ektron("div.type input.typeLocalizedStrings");
			if (localizedStringsInput != null && localizedStringsInput.length > 0) {
				var localizedStrings = localizedStringsInput.attr("value");
				Ektron.Commerce.Coupons.Type.LocalizedStrings = Ektron.JSON.parse(localizedStrings);
			}
			
			//finish init tasks
			Ektron.Commerce.Coupons.Type.initAfterAjax();
		},
		initAfterAjax: function(){
		
			//bind events
			Ektron.Commerce.Coupons.Type.bindEvents();
			
			//if in edit mode, set validation status
			if ("undefined" != typeof($ektron("div.type input.code").attr("value"))) {
				var code = $ektron("div.type input.code").attr("value");
				var originalCode = $ektron("div.type input.originalCouponCode").attr("value");
				if (code == originalCode) {
				    //code is original code - do not display status, and disable validate button
                    Ektron.Commerce.Coupons.Type.Validation.Code.disableCodeStatus();
                    $ektron("div.type table tbody tr.code td input.codeValidate").attr("disabled", "disabled");
                    //update validation status
                    Ektron.Commerce.Coupons.Type.Validation.setControlStatus("valid");
				} else {
				    Ektron.Commerce.Coupons.Type.Validation.Code.authorize(code);
				}
			} else {
			    //code is not set - set validation status to invalid, and disable validate button
			    Ektron.Commerce.Coupons.Type.Validation.setControlStatus("invalid");
			    $ektron("div.type table tbody tr.code td input.codeValidate").attr("disabled", "disabled");
			}
			
			//initialize currency select - disabled or enabled
			Ektron.Commerce.Coupons.Type.Validation.Currency.select();
		},
		Validation: {
		    Code: {
		        authorize: function(code){
		            var originalCode = $ektron("div.type input.originalCouponCode").attr("value");
				    if (code == originalCode) {
				        //code is original code - display status, and disable validate button
                        Ektron.Commerce.Coupons.Type.Validation.Code.setCodeStatus(true);
                        $ektron("div.type table tbody tr.code td input.codeValidate").attr("disabled", "disabled");
                        
                        //update validation status
                        Ektron.Commerce.Coupons.Type.Validation.setControlStatus("valid");
				    } else {
		                //test coupon code to make sure it's not already assigned
		                var controlId = $ektron("div.type input.controlId").attr("value");
		                var ektronPostParams = "";
		                ektronPostParams += "&__CALLBACKID=" + controlId; // Must use ASP.NET UniqueID as that's what's expected on server side when control is used with master pages.
		                ektronPostParams += "&__CALLBACKPARAM=" + escape(code);
		                ektronPostParams += "&__VIEWSTATE=";// + encodeURIComponent($ektron("#__VIEWSTATE").attr("value"));
		                //post an iCallback to check if code is ok
		                $ektron.ajax({
			                async: false,
			                type: "POST",
			                url: String(window.location),
			                data: ektronPostParams,
			                dataType: "html",
			                success: function(ektronCallbackResult){
				                var removeZeroPipe = String(ektronCallbackResult).replace("0|", "");
				                //alert(removeZeroPipe);
                                if (removeZeroPipe == "true") {
                                    //code is ok
		                            Ektron.Commerce.Coupons.Type.Validation.Code.setCodeStatus(true);
        							
		                            //update validation status
		                            Ektron.Commerce.Coupons.Type.Validation.setControlStatus("valid");
			                    }
			                    else {
				                    //code is already being used
				                    Ektron.Commerce.Coupons.Type.Validation.Code.setCodeStatus(false);
        							
				                    //update validation status
    				                
			                    }
			                }
		                });
		            }
			    },
			    disableCodeStatus: function(){
		            //code is available
			        $ektron("div.type tr.code span.codeValid").hide(); //hide valid message
			        $ektron("div.type tr.code span.codeInvalid").hide(); //hide invalid message
		        },
			    setCodeStatus: function(valid){
			        if (valid) {
			            //code is available
				        $ektron("div.type tr.code span.codeValid").css("display", "inline"); //show valid message
				        $ektron("div.type tr.code span.codeInvalid").hide(); //hide invalid message
			        }
			        else {
			            //code is already assigned
				        $ektron("div.type tr.code span.codeInvalid").css("display", "inline"); //show invalid message
				        $ektron("div.type tr.code span.codeValid").hide(); //hide valid message
			        }
		        },
		        validate: function(e) {
		            //esnure code does not contain invalid chars
			        var valid = true;
			        var invalidChars = " <>&+/\\'\"";
                    var code = $ektron("div.type table tbody tr.code td input.code").attr("value");
                    if ("undefined" == typeof(code)){
                        valid = false;
                    } else {
                        for(i = 0; i < invalidChars.length; i++) {
                            if (code.indexOf(invalidChars.charAt(i)) > -1) {
                                valid = false;
                            }
                        }
                    }
                    if (valid) {
                        //authorize valid code
                        Ektron.Commerce.Coupons.Type.Validation.Code.authorize(code);
                    } else {
                        //alert user code is invalid and set control status to invalid
                        alert(Ektron.Commerce.Coupons.Type.LocalizedStrings.invalidCharacterMessage);
                        Ektron.Commerce.Coupons.Type.Validation.setControlStatus("invalid");
                    }
			    },
			    verifyKey: function(e) {
			        var charCode;
                    var returnValue = true;
                    var e = (e) ? e : window.event;
                    var charCode = (e.which != null) ? e.which : e.keyCode;

                    //prevent manual cut (ctrl+x) and manual paste (ctrl+v)
                    if ((e.ctrlKey && charCode == 118) || (e.ctrlKey && charCode == 120)){
                        return false;
                    }
                    
                    //backspace - always return true
                    if (charCode == 8)
                        return true;
                    
                    //all else return false
                    return true;
			    }
		    },
		    Currency: {
		        select: function(){
			        if ("rbAmount" == $ektron(".type .typeChoice input:checked").attr("value")) {
			            var allCurrencies = $ektron(".type .currency span.allCurrencies");
			            var currencyList = $ektron(".type .currency select");
				        allCurrencies.hide();
				        currencyList.show();
				        var publishedCouponType = $ektron("div.type input.couponTypePublished").attr("value");
				        if ("Amount" == publishedCouponType || "undefined" == typeof(publishedCouponType)) {
				            $ektron("div.type input.couponTypeUserChanged").attr("value", "false");
				        } else {
				            $ektron("div.type input.couponTypeUserChanged").attr("value", "true");
				        }
			        } else  {
			            var allCurrencies = $ektron(".type .currency span.allCurrencies");
			            var currencyList = $ektron(".type .currency select");
				        allCurrencies.show();
				        currencyList.hide();
				        var publishedCouponType = $ektron("div.type input.couponTypePublished").attr("value");
				        if ("Percentage" == publishedCouponType || "undefined" == typeof(publishedCouponType)) {
				            $ektron("div.type input.couponTypeUserChanged").attr("value", "false");
				        } else {
				            $ektron("div.type input.couponTypeUserChanged").attr("value", "true");
				        }
				    }
		        }
		    },
		    Description: {
		        validate: function(e){
			        var charCode;
			        var returnValue = true;
			        var e = (e) ? e : window.event;
                    var charCode = (e.which != null) ? e.which : e.keyCode;
                    
                    var valid = true;
                    if(charCode == 188 || charCode == 190) {
                        valid = false;
                    }
                    return valid;
			    }
		    },
		    setControlStatus: function(status){
		        Ektron.Commerce.Coupons.Type.Validation.status = status
			    $ektron(document).trigger("ValidationStatusChange", ["Type", Ektron.Commerce.Coupons.Type.Validation.status]);
		    }
		}
	}
}