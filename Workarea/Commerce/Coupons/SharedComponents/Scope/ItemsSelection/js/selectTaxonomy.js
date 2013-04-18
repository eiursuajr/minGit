Ektron.ready(function() {
	Ektron.Commerce.Coupons.SelectTaxonomy.init();
	if ("undefined" != typeof(Sys)) {
	    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.SelectTaxonomy.initAfterAjax);
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

//define Ektron.Commerce.Coupons.SelectTaxonomy object only if it's not already defined
if (Ektron.Commerce.Coupons.SelectTaxonomy === undefined) {
	//Ektron Commerce Coupons SelectTaxonomy Object
	Ektron.Commerce.Coupons.SelectTaxonomy = {
	    Actions: {
	        checkboxClick: function (e){
                var e = (e) ? e : window.event;      
		        if (undefined == typeof e.target)
		            return;
    		        
                if (e.target.checked){
                    // ensure item is in the hidden field
		            // attempt to pull data from cache
		            var info = Ektron.Commerce.Coupons.SelectTaxonomy.Data.getCacheData(e.target.value);
		            if (info){
		                Ektron.Commerce.Coupons.SelectTaxonomy.Data.addSelectedItem(info);
		                return;
		            }
    		        
		            // not in cache; get from server, response will update cache and hidden field
		            Ektron.Commerce.Coupons.SelectTaxonomy.Utils.getServerData(e.target.value);
                }
                else{
                    // remove item from hidden field
                    Ektron.Commerce.Coupons.SelectTaxonomy.Data.removeItem(e.target.value);
                }
		    }
	    },
        bindEvents: function(){
            $ektron(document).bind("CMSAPIAjaxComplete", function(){
		        // must unbind first, otherwise outer elements fire multiple times
	            $ektron(".ektree input:checkbox").unbind("click","");
	            $ektron(".ektree input:checkbox").bind("click","", function (event){ 
                    Ektron.Commerce.Coupons.SelectTaxonomy.Actions.checkboxClick(event); 
                });
            });
        },
        Data: {
            addCacheData: function(item){
		        var itemData = Ektron.JSON.parse(item);
		        if (null == Ektron.Commerce.Coupons.SelectTaxonomy.Data.getCacheData(itemData.Id))
		            Ektron.Commerce.Coupons.SelectTaxonomy.Data.cache[itemData.Id] = item;
    		    
		        // add item to selected list
		        Ektron.Commerce.Coupons.SelectTaxonomy.Data.addSelectedItem(item);
		    },
            addSelectedItem: function(item){
		        // update the hidden element; insert if not present
                if ("undefined" != typeof(item)) {
                    Ektron.Commerce.Coupons.SelectTaxonomy.Data.data.push(Ektron.JSON.parse(item));
                    $ektron("div.taxonomySelect input.data").attr("value", Ektron.JSON.stringify(Ektron.Commerce.Coupons.SelectTaxonomy.Data.data));
                }
		    },
            getCacheData: function(id){
		        // todo: recover data...
		        return Ektron.Commerce.Coupons.SelectTaxonomy.Data.cache[id];
		    },
		    init: function(){
                //initailize json data array
                Ektron.Commerce.Coupons.SelectTaxonomy.Data.data = new Array();
                //populate with existing data (if any)
	            var data = $ektron("div.taxonomySelect input.data").attr("value");
	            if ("undefined" != typeof(data) && data.length > 0) {
                    Ektron.Commerce.Coupons.SelectTaxonomy.Data.data = Ektron.JSON.parse(data);
                } else {
                    Ektron.Commerce.Coupons.SelectTaxonomy.Data.addSelectedItem();
                }
            },
		    removeItem: function(id){
		        // update the hidden element; remove if present
                for (var i = 0; i < Ektron.Commerce.Coupons.SelectTaxonomy.Data.data.length; i++){
                    if (Ektron.Commerce.Coupons.SelectTaxonomy.Data.data[i].Id == id){
                        if (Ektron.Commerce.Coupons.SelectTaxonomy.Data.data[i].NewlyAdded == "true")
                            Ektron.Commerce.Coupons.SelectTaxonomy.Data.data.splice(i, 1);
                        else
                            Ektron.Commerce.Coupons.SelectTaxonomy.Data.data[i].MarkedForDelete = "true";
                        break;
                    }
                }
                $ektron("div.taxonomySelect input.data").attr("value", Ektron.JSON.stringify(Ektron.Commerce.Coupons.SelectTaxonomy.Data.data));
		    }
        },
		init: function(){
		    // init variables:
			Ektron.Commerce.Coupons.SelectTaxonomy.Data.tabsEnabled = true;
			Ektron.Commerce.Coupons.SelectTaxonomy.Data.cache = new Array();
            
            //init data
            Ektron.Commerce.Coupons.SelectTaxonomy.Data.init();

	        //finish initialization
			Ektron.Commerce.Coupons.SelectTaxonomy.initAfterAjax();
		},
		initAfterAjax: function(){
		    //init bind events
            Ektron.Commerce.Coupons.SelectTaxonomy.bindEvents();
		},
		Utils: {
		    getServerData: function (id){
                var uniqueId = $ektron("div.taxonomySelect input.uniqueId").attr("value");
                var ektronPostParams = "&__CALLBACKID=" + uniqueId; // Must use ASP.NET UniqueID
                ektronPostParams += "&__CALLBACKPARAM=" + escape("taxid=" + id);
                ektronPostParams += "&__VIEWSTATE=";
                
                //post an iCallback to check if code is ok
                $ektron.ajax({
                    type: "POST",
                    url: String(window.location),
                    data: ektronPostParams,
                    dataType: "html",
                    success: function(ektronCallbackResult){
					 try{var removeZeroPipe = String(ektronCallbackResult).replace("0|","");
                        if (removeZeroPipe && removeZeroPipe.length)
                            Ektron.Commerce.Coupons.SelectTaxonomy.Data.addCacheData(removeZeroPipe);
                        }
                        catch(e){}
                    }
                });
		    }
		}
	}
}
