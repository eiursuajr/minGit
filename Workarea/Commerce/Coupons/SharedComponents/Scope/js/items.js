Ektron.ready(function(){
	//page load init
    Ektron.Commerce.Coupons.Scope.Items.init();
	var msAjax = Sys.WebForms.PageRequestManager.getInstance();
	if (msAjax != null) {
		Sys.WebForms.PageRequestManager.getInstance().add_endRequest(Ektron.Commerce.Coupons.Scope.Items.initAfterAjax);
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

//define Ektron.Commerce.Coupons.Scope object only if it's not already defined
if (Ektron.Commerce.Coupons.Scope === undefined) {
    Ektron.Commerce.Coupons.Scope = {};
}

//define Ektron.Commerce.Coupons.Scope.Items object only if it's not already defined
if (Ektron.Commerce.Coupons.Scope.Items === undefined) {
    //Ektron.Commerce.Coupons.Scope.Items Object
    Ektron.Commerce.Coupons.Scope.Items = {
        bindEvents: function(){
            $ektron("div.items ul.includeExcludeNavigation li.tooltip img").hover(
                function(){ 
                    var message = $ektron("div.items ul.includeExcludeNavigation li.tooltip div.message");
                    message.show();
                },
                function(){
                    var message = $ektron("div.items ul.includeExcludeNavigation li.tooltip div.message");
                    message.hide();
                }
            );
            $ektron("div.couponAdd table.ektronGrid div.ui-tabs ul.ui-tabs-nav li").hover(
                function(){ 
                    $(this).addClass("ui-state-hover");
                },
                function(){
                    $(this).removeClass("ui-state-hover");
                }
            );
        },
        Data: {
            init: function(){            
                //initialize Json data arrays
                Ektron.Commerce.Coupons.Scope.Items.data = new Array();
                
                //populate Json data objects
                $ektron("div.items div.itemsData table.items tr.item td.data").each(function(i){
                    //retrieve json values
                    var id = $ektron(this).find("input.id").attr("value");
                    var name = $ektron(this).find("input.name").attr("value");
                    var path = $ektron(this).find("input.path").attr("value");
                    var type = $ektron(this).find("input.type").attr("value");
                    var typeCode = $ektron(this).find("input.typeCode").attr("value");
                    var markedForDelete = $ektron(this).find("input.markedForDelete").attr("value");
                    
                    //create json object
                    var jsonData = {
                        Id: id,
                        Name: name,
                        Path: path,
                        Type: type,
                        TypeCode: typeCode,
                        MarkedForDelete: markedForDelete
                    }
                    
                    //populate json data array with item values
                    Ektron.Commerce.Coupons.Scope.Items.data.push(jsonData);
                });
                
                //populate items input field with stringified json data
                $ektron("div.items div.itemsData input.data").attr("value", Ektron.JSON.stringify(Ektron.Commerce.Coupons.Scope.Items.data));
                
                //uncomment to view data
                //alert($ektron("div.items div.itemsData input.data").attr("value"));
            },
            update: function(){
                //re-initialize json data array
                Ektron.Commerce.Coupons.Scope.ItemsData.init();
            }
        },
        init: function(){
			//finish init
			Ektron.Commerce.Coupons.Scope.Items.initAfterAjax();
        },
		initAfterAjax: function(){
		    //bind events
		    Ektron.Commerce.Coupons.Scope.Items.bindEvents();
		    
		    //re-initialize ektron grids and tabs
		    Ektron.Workarea.Grids.show();
		    Ektron.Workarea.Tabs.setWidth();
		}
    }
}
