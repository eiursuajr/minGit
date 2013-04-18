Ektron.ready(function() {
	Ektron.Commerce.Coupons.Add.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
	    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.Add.init);
	}
	
    //disable Next button when intially on page (adding a valid code will enable nav)
    Ektron.Commerce.Coupons.Add.Navigation.disable();
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

//define Ektron.Commerce.Coupons.Add object only if it's not already defined
if (Ektron.Commerce.Coupons.Add === undefined) {
    //Ektron.Commerce.Coupons.Add object
    Ektron.Commerce.Coupons.Add = {
        bindEvents: function(){
            $ektron(document).bind("ValidationStatusChange", function(e, component, status) {
                if (component == $ektron("input.currentControl").attr("value")) {
                switch(status) {
                    case "valid":
                        Ektron.Commerce.Coupons.Add.Navigation.enable();
                        break;
                    case "invalid":
                        Ektron.Commerce.Coupons.Add.Navigation.disable();
                        break;
                    }
                }
            });
        },
        init: function() {
            //turn autocompelte off
            $ektron("form").attr("autocomplete", "off");
            
            //remove disabled attribute from finish sidebar link (ie display problem)
            if ($ektron.browser.msie)
                $ektron("td.sidebar a[disabled]").removeAttr("disabled");
            
            //init bind events
            Ektron.Commerce.Coupons.Add.bindEvents();
            
            //get validation status
            $ektron(document).trigger("GetCouponValidationStatus");

        },
        Navigation: {
            disable: function(){
                //prevent moving on to next step via 'next' button
                $ektron("input[type='submit'][value='Next']").attr("disabled", "disabled");
            },
            enable: function(){
                //allow moving on to next step via 'next' button
                $ektron("input[type='submit'][value='Next']").removeAttr("disabled"); 
            }
	    }
	}
}