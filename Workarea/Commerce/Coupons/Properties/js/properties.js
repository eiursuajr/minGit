Ektron.ready(function() {
	Ektron.Commerce.Coupons.Properties.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
	    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.Properties.initAfterAjax);
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

//define Ektron.Commerce.Coupons.Properties object only if it's not already defined
if (Ektron.Commerce.Coupons.Properties === undefined) {
	//Ektron Commerce Coupons Properties Object
	Ektron.Commerce.Coupons.Properties = {
        bindEvents: function(){
            $ektron(".ui-tabs .ui-tabs-nav li").hover(
                function(){
                    $ektron(this).addClass("ui-state-hover");
                },
                function(){
                    $ektron(this).removeClass("ui-state-hover");
                }
            );
        
            $ektron(document).bind("ValidationStatusChange", function(e, component, status) {
                //alert(component + ": " + status);
                if (component == $ektron("input.currentControl").attr("value")) {
                    switch(status) {
                        case "valid":
                            Ektron.Commerce.Coupons.Properties.Navigation.enable();
                            break;
                        case "invalid":
                            Ektron.Commerce.Coupons.Properties.Navigation.disable();
                            break;
                    }
                }
            });
        },
		init: function(){
		    //turn autocompelte off
            $ektron("form").attr("autocomplete", "off");
            
		    // init variables:
			Ektron.Commerce.Coupons.Properties.tabsEnabled = true;

		    //init bind events
            Ektron.Commerce.Coupons.Properties.bindEvents();
		
		    //get validation status
            $ektron(document).trigger("GetCouponValidationStatus");
		
	        //initialize modal
	        Ektron.Commerce.Coupons.Properties.Modal.init();

	        //finish initialization
            Ektron.Commerce.Coupons.Properties.enableToolbar(true);
			Ektron.Commerce.Coupons.Properties.initAfterAjax();
		},
		initAfterAjax: function(){
		},
		Redirect: function (path){
		    window.location = path;
		},
		Modal: {
		    init: function(){
		        $ektron(".ektronModalClose").hover(
		            function(){
		                $ektron(this).addClass("ui-state-hover");
		            },
		            function(){
		                $ektron(this).removeClass("ui-state-hover");
		            }
		        );
		    
	            var modal = $ektron("#EktronCouponsPropertiesModal");
	            modal.drag('div.propertiesModalHeader');
	            modal.modal({
	                toTop: true,
	                modal: true,
			        overlay: 0,
	                onShow: function(hash) {
		                hash.o.fadeTo("fast", 0.5, function(){						
			                hash.w.fadeIn("fast");
		                });
	                }, 
	                onHide: function(hash) {
	                    hash.w.fadeOut("fast");
				        hash.o.fadeOut("fast", function(){
					        if (hash.o) 
						        hash.o.remove();
				        });
	                }  
	            });
	        },
		    hide: function() {
		        $ektron("#EktronCouponsPropertiesModal").modalHide();
		    },
		    show: function(type) {
		        switch(type) {
		            case "save":
		                $ektron("#EktronCouponsPropertiesModal input.modalType").attr("value", "save");
		                $ektron("#EktronCouponsPropertiesModal #confirmDeleteHeader").css("display", "none");
		                $ektron("#EktronCouponsPropertiesModal #confirmSaveHeader").css("display", "inline");
		                $ektron("#EktronCouponsPropertiesModal #confirmDeleteMessage").css("display", "none");
		                $ektron("#EktronCouponsPropertiesModal #confirmSaveMessage").css("display", "block");
		                break;
		            case "delete":
		                $ektron("#EktronCouponsPropertiesModal input.modalType").attr("value", "delete");
		                $ektron("#EktronCouponsPropertiesModal #confirmDeleteHeader").css("display", "inline");
		                $ektron("#EktronCouponsPropertiesModal #confirmSaveHeader").css("display", "none");
		                $ektron("#EktronCouponsPropertiesModal #confirmDeleteMessage").css("display", "block");
		                $ektron("#EktronCouponsPropertiesModal #confirmSaveMessage").css("display", "none");
		                break;
		        }
		        
		        $ektron("#EktronCouponsPropertiesModal").modalShow();
		    },
		    DeleteDialog: {
		        no: function(){
		            Ektron.Commerce.Coupons.Properties.Modal.hide('EktronCouponConfirmDelete');
		        },
		        yes: function() {
		            //postback delete click, remove coupon server side
		            __doPostBack('lbDelete','');
		        }
		    },
		    ok: function() {
		        var type = $ektron("input.modalType").attr("value");
		        switch(type) {
		            case "save":
		                Ektron.Commerce.Coupons.Properties.Modal.SaveDialog.yes();
		                break;
		            case "delete":
		                Ektron.Commerce.Coupons.Properties.Modal.DeleteDialog.yes();
		                break;
		        }
		    },
		    SaveDialog: {
		        no: function(){
		            Ektron.Commerce.Coupons.Properties.Modal.hide('EktronCouponConfirmSave');
		        },
		        yes: function() {
		            //postback delete click, remove coupon server side
		            __doPostBack('lbSave','');
		        }
		    }
		},
        Navigation: {
            disable: function(){
                //prevent changing tabs
                Ektron.Commerce.Coupons.Properties.tabsEnabled = false;
                $ektron(".ektron .coupon .TabControls a").css("cursor", "default");
                $ektron(".ektron .coupon .TabControls a.InactiveTab").addClass("DisabledTab").removeClass("InactiveTab").attr("disabled", "disabled");
                $ektron(".ui-tabs .ui-tabs-nav li").unbind('mouseenter mouseleave');
                $ektron(".ektron .coupon .TabControls a.ActiveTab[disabled!=disabled]").attr("disabled", "disabled");
                Ektron.Commerce.Coupons.Properties.enableToolbar(false);
            },
            enable: function(){
                //allow changing tabs
                Ektron.Commerce.Coupons.Properties.tabsEnabled = true;
                $ektron(".ektron .coupon .TabControls a").css("cursor", "pointer");
                $ektron(".ektron .coupon .TabControls a.DisabledTab").addClass("InactiveTab").removeClass("DisabledTab").removeAttr("disabled");
                //do not rebind all events, just this one
                $ektron(".ui-tabs .ui-tabs-nav li").hover(
                    function(){
                        $ektron(this).addClass("ui-state-hover");
                    },
                    function(){
                        $ektron(this).removeClass("ui-state-hover");
                    }
                );
                $ektron(".ektron .coupon .TabControls a.ActiveTab[disabled=disabled]").removeAttr("disabled");
                Ektron.Commerce.Coupons.Properties.enableToolbar(true);
            },
            tabClick: function(){
                if (Ektron.Commerce.Coupons.Properties.tabsEnabled){
                    // temporarily update mouse pointer to show waiting for server
                    $ektron(".ektron .coupon .TabControls a").css("cursor", "wait");
                    return true;
                }
                return false;
            }
	    },
	    Actions: {
	        cancel: function(){
	            // if not editable, skip the "Verify Cancel" modal
	            if (!Ektron.Commerce.Coupons.Properties.isEditable())
	                return true;
	            
	            $ektron('#EktronCouponConfirmCancel').modalShow();
	            return false;
	        }
	    },
	    enableToolbar: function(enableFlag){
	        if (enableFlag && Ektron.Commerce.Coupons.Properties.isEditable())
	            $ektron("#divActions").removeClass("limitedActions");
	        else
	            $ektron("#divActions").addClass("limitedActions");
	    },
	    isEditable: function(){
	        var editable = $ektron("#hfEditable").attr("value").toLowerCase();
	        var returnValue = "undefined" != typeof(editable) ? editable : false;
	        return returnValue;
	    },
	    getHomePageUrl: function (){
	        return $ektron("#hfHomePage").attr("value");
	    }
	}
}