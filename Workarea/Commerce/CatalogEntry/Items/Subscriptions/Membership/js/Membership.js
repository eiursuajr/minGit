//Intialiaze Membership
Ektron.ready(function(){
	Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.init();
});

//Define Ektron object only if it's not already defined
if (Ektron === undefined) {
	Ektron = {};
}

//Define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce === undefined) {
	Ektron.Commerce = {};
}

//Define Ektron.Commerce object only if it's not already defined
if (Ektron.Commerce.CatalogEntry === undefined) {
	Ektron.Commerce.CatalogEntry = {};
}

//Define Ektron.Commerce.Items object only if it's not already defined
if (Ektron.Commerce.CatalogEntry.Items === undefined) {
	Ektron.Commerce.CatalogEntry.Items = {};
}

//Define Ektron.Commerce.Items.Subscriptions object only if it's not already defined
if (Ektron.Commerce.CatalogEntry.Items.Subscriptions === undefined) {
	Ektron.Commerce.CatalogEntry.Items.Subscriptions = {};
}

//Ektron Personalization Object
Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership = {
    add: function(type, groupId, groupName) {
        switch(type)
        { 
            case "membership":
                $ektron("div.subscriptionView tr.membershipGroup span.groupName").text(groupName);
                $ektron("div.subscriptionView tr.membershipGroup input.groupId").attr("value", groupId);
                $ektron("div.subscriptionView tr.membershipGroup a.markForDelete").removeClass("groupNotSet");
                Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Modal.hide();
                break;
            case "cms":
                $ektron("div.subscriptionView tr.cmsGroup span").text(groupName);
                $ektron("div.subscriptionView tr.cmsGroup input.groupId").attr("value", groupId);
                $ektron("div.subscriptionView tr.cmsGroup a.markForDelete").removeClass("groupNotSet");
                Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Modal.hide();
                break;
        }
        Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.update();
    },
    Data: {
		init: function(){
			//initialize Json data as array
			Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.data = new Array();
			
			//populate Json data - cms group
			var cmsGroupId = $ektron("div.subscriptionView tr.cmsGroup input.groupId").attr("value");
			var cmsGroupMarkedForDelete = $ektron("div.subscriptionView tr.cmsGroup input.markedForDelete").attr("value");
			var cmsGroupData = {
			    "GroupType": "cms",
			    "GroupId": cmsGroupId,
			    "MarkedForDelete": cmsGroupMarkedForDelete
			}
			Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.data.push(cmsGroupData);
			
			//populate Json data - membership group
			var membershipGroupId = $ektron("div.subscriptionView tr.membershipGroup input.groupId").attr("value");
			var membershipGroupMarkedForDelete = $ektron("div.subscriptionView tr.membershipGroup input.markedForDelete").attr("value");
			var membershipGroupData = {
			    "GroupType": "membership",
			    "GroupId": membershipGroupId,
			    "MarkedForDelete": membershipGroupMarkedForDelete
			}
			Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.data.push(membershipGroupData);
			
			//populate items input field with stringified json data
			$ektron("div.subscriptionView input.subscriptionMembershipData").attr("value", Ektron.JSON.stringify(Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.data));
			
			//alert($ektron("div.subscriptionView input.subscriptionMembershipData").attr("value"));
		},
		update: function(){
			//re-initialize json data array
			Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.init();
		}
	},
    edit: function(elem) {
        var editButton = $ektron(elem);
        var sitePath = $ektron("div.subscriptionView input.subscriptionMembershipSitePath").attr("value");
        var iFrame = $ektron("div.subscriptionView iframe.ItemsSubscriptionMembershipGroupSelector");
        var iFrameSrc;
        if (editButton.hasClass("editCmsAuthorGroup")) {
            //edit cms author group clicked
            iFrameSrc = sitePath + "/users.aspx?action=viewallgroups&grouptype=0&RequestedBy=EktronCommerceItemsSusbscriptionsMembership";
        } else {
            //edit membership group clicked
            iFrameSrc = sitePath + "/users.aspx?action=viewallgroups&grouptype=1&RequestedBy=EktronCommerceItemsSusbscriptionsMembership";
        }
        iFrame.attr("src", iFrameSrc);
        $ektron('#ItemsSubscriptionMembershipModal').modalShow();
    },
    init: function() {
        //initialize data
        Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.init();
        
        //initialize modal
        Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Modal.init();
    },
    markForDelete: function(elem) {
        var deleteButton = $ektron(elem);
        var restoreButton = deleteButton.next();
        var editButton = deleteButton.prev();
        var span = deleteButton.prevAll("span");
        deleteButton.toggle();
        restoreButton.toggle();
        editButton.toggle();
        span.addClass("markedForDelete");
        var group = $ektron(elem).parents("tr.group");
        group.find("input.markedForDelete").attr("value", "true");
        Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.update();
    },
    Modal: {
        Buttons: {
            close: function(){ 
                Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Modal.hide();
            }
	    },
        init: function(){
			$ektron('#ItemsSubscriptionMembershipModal').drag('.itemsSubscriptionMembershipModalHeader');
			$ektron('#ItemsSubscriptionMembershipModal').modal({
			    modal: true,
				overlay: 0,
				toTop: true,
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
			$ektron('#ItemsSubscriptionMembershipModal').find("h3 img").bind("click", function(){
			    $ektron("ul.kitGroups").removeClass("addItemMarker");
				Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Modal.hide();
			});
		},
		hide: function(){
			$ektron('#ItemsSubscriptionMembershipModal').modalHide();
		}
    },
    restore: function(elem) {
        var restoreButton = $ektron(elem);
        var deleteButton = restoreButton.prev();
        var editButton = deleteButton.prevAll("a.edit");
        var span = restoreButton.prevAll("span");
        deleteButton.toggle();
        restoreButton.toggle();
        editButton.toggle();
        span.removeClass("markedForDelete");
        var group = $ektron(elem).parents("tr.group");
        group.find("input.markedForDelete").attr("value", "false");
        Ektron.Commerce.CatalogEntry.Items.Subscriptions.Membership.Data.update();
    }
}

function validate_SubscriptionDelegate()
{

    var authorGroupId = $ektron("div.subscriptionView tr.cmsGroup input.groupId").attr("value");
    var memberGroupId = $ektron("div.subscriptionView tr.membershipGroup input.groupId").attr("value");
    
    if ( !(memberGroupId > 0 && authorGroupId >= 0) )
        AddError('Please select a membership group. The author group is optional.');
    
    return (memberGroupId > 0 && authorGroupId >= 0);

}