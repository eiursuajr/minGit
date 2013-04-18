
if (Ektron === undefined) {
	Ektron = {};
}
if (Ektron.Commerce === undefined) {
	Ektron.Commerce = {};
}
Ektron.Commerce.Pricing = {
    bindEvents: function() {
        //ensure numerics only in numeric edit field
        $ektron('table.ektron_RecurringPricing_Table tr.interval input.interval')
            .bind("keypress", function(evt){
                var evt = (evt) ? evt : window.event;
                var charCode = (evt.which != null) ? evt.which : evt.keyCode;
                return (charCode < 32 || (charCode >=48 && charCode <= 57))
        });
        
        //ensure numerics and decimal in numeric edit field
         $ektron('table.ektron_UnitPricing_Table input')
            .bind("keypress", function(evt){
                var evt = (evt) ? evt : window.event;
                var charCode = (evt.which != null) ? evt.which : evt.keyCode;
                if (charCode == 46 ){
                    var value = $ektron(this).val();
                    var hasDecimal = value.indexOf(".") == -1 ? false : true;
                    if (hasDecimal) {
                        return false;
                    }
                }
                return (charCode == 46 || charCode < 32 || (charCode >=48 && charCode <= 57))
        });
    },
    floatToggle: function(obj){
        if(obj.checked){
            alert("Note: Pricing tier information applied to the default currency will now apply to this currency.");
        }
        else{
            alert("Please Note: This will allow you to add or edit the tier price information.");	            
        }
        
        $ektron(obj).parents("table.ektron_UnitPricing_Table").find("tbody td :text").each(function(i){
		    $ektron(this).attr("disabled", (obj.checked == true) ? "disabled" : "");
	    });
	    
        $ektron(obj).parents("table.ektron_UnitPricing_Table").next().find("input:text").each(function(i){
            $ektron(this).attr("disabled", (obj.checked == true) ? "disabled" : "");
        });
        
        $ektron("p.ektron_TierPricing_Commands").toggle();
    	
    },
    floatRecurring: function(obj){
        
        var useRecurrentBilling = $ektron(obj);
        if (useRecurrentBilling.find("option:selected").attr("value") == "true") {
            useRecurrentBilling.parents("tbody").find("tr.billingCycle select").removeAttr("disabled");
            useRecurrentBilling.parents("tbody").find("tr.interval input").removeAttr("disabled");
            useRecurrentBilling.parents("tbody").find("tr.interval span").css("display", "inline");
        } else {
            useRecurrentBilling.parents("tbody").find("tr.billingCycle select").attr("disabled", "disabled");
            useRecurrentBilling.parents("tbody").find("tr.interval input").attr("disabled", "disabled");
            useRecurrentBilling.parents("tbody").find("tr.interval span").css("display", "none");
        }
    },
    init: function(){
        //bind events
        Ektron.Commerce.Pricing.bindEvents();
    },
    selectCurrency: function(currencyData, defaultCurrency) {
	    var splitString = currencyData.split(";");
	    var idSplit = splitString[0].split(":");
	    var id = idSplit[1];
	    var labelSplit = splitString[1].split(":");
	    var label = unescape(labelSplit[1]);
	    var symbolSplit = splitString[2].split(":");
	    var symbol = symbolSplit[1];
	    var idDefault = "ektron_Pricing_" + defaultCurrency;
	    var ObjAddRemovePricing = $ektron("p.ektron_TierPricing_Commands");
	    var chkDefault = document.getElementById("ektron_UnitPricing_Float_" + id.split("_")[2]);
    	
	    if (typeof ObjAddRemovePricing == "object")
	    {
	        if(id != idDefault && chkDefault.checked)
	        {
	            ObjAddRemovePricing.hide();
	        }
	        else
	        {
	            ObjAddRemovePricing.show();
	        }
	    }
    	
	    $ektron(".ektron_PricingWrapper h3 span.currencyLabel").html(symbol + "&#160;" + label);
	    $ektron(".ektron_Pricing_CurrencyWrapper_Active").fadeOut("fast", function(){
		    //remove selected flag from old currency
		    $ektron(this).removeClass("ektron_Pricing_CurrencyWrapper_Active");

		    //add selected flag to selected currency
		    var selectedCurrency = $ektron("#" + id);
		    selectedCurrency.addClass("ektron_Pricing_CurrencyWrapper_Active");
    		
		    //set "remove pricing tier" button to proper state for selected currency
		    Ektron.Commerce.Pricing.Tier.toggleRemove();
		    var removeTierButton = $ektron(".ektron_RemovePricingTier_Button");
    		
		    if (selectedCurrency.find("div.ektron_TierPricing_Wrapper").css("display") === "none") {
			    if (removeTierButton.css("display") !== "none") {
				    removeTierButton.fadeOut("slow");
			    }
		    } else {
			    if (removeTierButton.css("display") === "none") {
				    removeTierButton.fadeIn("slow");
			    }
		    }
		    selectedCurrency.fadeIn("slow");
	    });
    },
    Tier: {
	    addRule: function(obj) {
		    var currentTier = $ektron(obj).parent();
		    var clonedTier = currentTier.clone();
    		
		    currentTier.parent().append(clonedTier);
	    },
	    addTier: function(obj) {
		    var activeCurrencyWrapper = $ektron(obj).parent().prevAll(".ektron_Pricing_CurrencyWrapper_Active");
		    var tierTableWrapper = activeCurrencyWrapper.find(".ektron_TierPricing_Wrapper");
		    if (tierTableWrapper.css("display") === "none") {
			    tierTableWrapper.slideDown("slow", function(){
				    $ektron(obj).next().fadeIn("slow");
				    tierTableWrapper.find("img").each(function(b){
					    $ektron(this).fadeIn("slow");
				    });
			    });
		    } else {
			    var tierPricingTableBody = tierTableWrapper.find('.ektron_TierPricing_Table tbody');
			    var tiers = tierTableWrapper.find("tr.tier");
			    var clonedTier = tierPricingTableBody.children("tr:last").clone();
			    clonedTier.find("input").each(function(i){
			        var input = $ektron(this);
			        if (input.attr("id") !== undefined) {
			            var IdandName = Ektron.Commerce.Pricing.Tier.setId(input.attr("id"));
				        input.attr("id", IdandName);
				        input.attr("name", IdandName);
				        input.removeAttr("checked");
				        input.removeAttr("value");
				    }
			    });
			    if (Ektron.Commerce.Pricing.Tier.isOdd(tiers.length + 1) === true) {
				    clonedTier.addClass("stripe");
			    } else {
				    clonedTier.removeClass("stripe");
			    }
			    tierPricingTableBody.append(clonedTier);
		    }
	    },
	    itemAdded: function(){
	        //this function is called from Ektron.Commerce.CatalogEntry.Items.DefaultView.Add.addItem()
	        //when a product becomes a complex product (an item is added), pricing tiers must be hidden
	        $ektron("div.ektron_TierPricing_Wrapper").remove();
	        $ektron("p.ektron_TierPricing_Commands").remove();
	    },
	    isEven: function(number) {
		    return ! (number % 2);
	    },
	    isOdd: function(number) {
		    return ! Ektron.Commerce.Pricing.Tier.isEven(number);
	    },
	    removeTier: function() {
            var removeTierButton = $ektron(".ektron_RemovePricingTier_Button");

            if (removeTierButton.attr("class") && removeTierButton.attr("class").indexOf("disabled") > 0
                || removeTierButton.attr("classname") && removeTierButton.attr("classname").indexOf("disabled") > 0)
                return false;

		    var confirmation = confirm ("Remove?");
		    if (confirmation === true) {
    		
    			
			    var activeCurrency = $ektron("div.ektron_Pricing_CurrencyWrapper_Active");
    			
			    //get all tier pricing rows that are marked for removal (checked)
			    activeCurrency.find("input.ektron_RemoveTier_Checkbox:checked").each(function(a){
				    var tier = $ektron(this).parents("tr.tier");
				    if (tier.siblings().length > 0) {
					    tier.remove(); //if two or more tiers remain just remove the tier marked for removal
				    }
				    else {
					    //all tiers are removed - zero-out first-row and hide pricing tier table
					    var pricingTierWrapper = tier.parents(".ektron_TierPricing_Wrapper");
					    tier.parent().parent().find("img").each(function(b){
						    $ektron(this).fadeOut("slow");
					    });
					    removeTierButton.fadeOut("slow", function(){
						    pricingTierWrapper.slideUp("slow");
					    });
					    tier.find("input").each(function(c){
						    var input = $ektron(this);
						    input.removeAttr("value");
						    input.removeAttr("checked");
					    });
				    }
			    });
			    //renumber tier ids so they reflect the order they are in now
			    var tierCounter = 0;
			    activeCurrency.find("tr.tier").each(function(d){
    			
				    $ektron(this).find("input").each(function(c){
					    var oldId = $ektron(this).attr("id");
					    var oldName = $ektron(this).attr("name");
					    if (oldId !== undefined) {
						    var newId = oldId.substring(0, oldId.length - 1) + String(tierCounter);
						    $ektron(this).attr("id", newId);
					    }
					    if (oldName !== undefined) {
						    var newName = oldName.substring(0, oldName.length - 1) + String(tierCounter);
						    $ektron(this).attr("name", newName);
					    }
				    });
				    tierCounter++;
    				
				    if (Ektron.Commerce.Pricing.Tier.isOdd(d - 1) === true) {
					    $ektron(this).addClass("stripe");
				    }
				    else {
					    $ektron(this).removeClass("stripe");
				    }
			    });
			    removeTierButton.addClass("disabled");
			    removeTierButton.removeClass("redHover");
			    removeTierButton.unbind("click");
			    removeTierButton.bind("click", function(){
				    return false;
			    });
		    }
	    },
	    setId: function(oldId) {
		    var newId;
		    if (oldId !== undefined) {
			    var newTierNumber = parseInt(oldId.substr(oldId.length -1, 1));
			    var newIdNumber = newTierNumber + 1;
			    newId = oldId.substring(0, oldId.length - 1) + String(newIdNumber);
		    }
		    return newId;
	    },
	    toggleRemove: function(){
		    var tierTable = $ektron(".ektron_Pricing_CurrencyWrapper_Active").find("table.ektron_TierPricing_Table");
		    var removeTierButton = $ektron(".ektron_RemovePricingTier_Button");
    	
		    var removeTierCheckboxes = tierTable.find('.ektron_RemoveTier_Checkbox:checked');
		    if (removeTierCheckboxes.length === 0) {
			    removeTierButton.addClass("disabled");
			    removeTierButton.removeClass("redHover");
			    removeTierButton.unbind("click");
			    removeTierButton.bind("click", function() {
				    return false;
			    });
		    } else {
			    removeTierButton.removeClass("disabled");
			    removeTierButton.addClass("redHover");
			    removeTierButton.bind("click", function() {
				    $ektron('#ektron_Pricing_Modal').modalShow();
				    return false;
			    });
		    }
	    }
    }
}

/*
Ektron.ready(function() {
	$ektron('#ektron_Pricing_Modal').drag('#ektron_Pricing_Modal_Header');
    $ektron('#ektron_Pricing_Modal').modal({ 
        trigger: '', 
        modal: true,
		overlay: 0,
        onShow: function(hash) {
			hash.o.fadeTo("fast", 0.5, function() {
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
});
*/

Ektron.ready(function() {
	Ektron.Commerce.Pricing.init();
});
