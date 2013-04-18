//define Ektron object only if it's not already defined
if (typeof(Ektron) == "undefined") {
	Ektron = {};
}

//define Ektron.eWebEditPro object only if it's not already defined
if (typeof(Ektron.eWebEditPro) == "undefined") {
	Ektron.eWebEditPro = {};
}

//define Ektron.eWebEditPro.Tab object only if it's not already defined
if (typeof(Ektron.eWebEditPro.Tab) == "undefined") {
	Ektron.eWebEditPro.Tab =
	{
	    moveEWebEditPro: function(tabsNav, tabContainer, anchorClicked)
	    {
	        // first hide all eWebEditPro isntances
	        var alleWebEditPros = $ektron("ewebeditproWrapper");
	        alleWebEditPros.css("left", "-10000px").css("top", "0px");
	        var tabPanelShown = $ektron(".ui-tabs-panel:not(.ui-tabs-hide)");
	        var tabPanelId = tabPanelShown.attr("id");
	        var placeHolder = $ektron(".ewebeditproPlaceholder_" + tabPanelId);
	        var eWebEditProToMove = $ektron(".ewebeditpro_" + tabPanelId);

	        if(typeof anchorClicked == "undefined")
	        {
	            anchorClicked = tabPanelShown;
	        }
	        Ektron.eWebEditPro.Tab.setPanelHeight(tabPanelShown, tabsNav, tabContainer);

	        if (placeHolder.length > 0)
	        {
	            var placeHolderOffset = placeHolder.position();
	            placeHolderOffset.left = parseInt(placeHolderOffset.left , 10);
	            placeHolderOffset.top = parseInt(placeHolderOffset.top, 10);

	            eWebEditProToMove.css("left", placeHolderOffset.left + "px").css("top", placeHolderOffset.top + "px").css("width", placeHolder.outerWidth() + "px");
	        }
	    },

	    hideEWebEditPro: function()
	    {
	        var ewebEditProContainers = $ektron(".ewebeditproWrapper");
	        ewebEditProContainers.css("left", "-10000px");
	    },

	    init: function()
	    {
	        var ewebEditProContainers = $ektron(".ewebeditproWrapper");
	        var tabContainer = $ektron("#editContentTabContainer");
	        var tabsNav = tabContainer.find("ul:first");
            var tabAnchors = tabContainer.find("ul:first li:has(a[href]) a");
            var buttons = $ektron("img[src$='contentPublish.png'],img[src$='checkIn.png'],img[src$='save.png'");

            // add placeholders for each eWebEditPro instance
            ewebEditProContainers.each(function(i)
	        {
	            var currentWrapper = $ektron(this);
	            var wrapperParent = currentWrapper.parent().attr("id");
	            var newDiv = "<div class='ewebeditproPlaceholder ewebeditproPlaceholder_" + wrapperParent + "' />";
	            currentWrapper.after(newDiv);
	        });

            // bind to the tab links
            tabAnchors.bind("click", function()
            {
                var anchorClicked = $ektron(this);
                setTimeout(function()
                {
                    Ektron.eWebEditPro.Tab.moveEWebEditPro(tabsNav, tabContainer, anchorClicked);
                }, 1);
            });

            // bind to this event which is raised  when the form wizards show various panels
            $ektron(document).bind("wizardPanelShown", function()
	        {
	            Ektron.eWebEditPro.Tab.moveEWebEditPro(tabsNav, tabContainer);
	        });

	        // bind to the wizard next and back buttons
	        $ektron("#btnBackStep, #btnNextStep").bind("click", function()
	        {
	            $ektron(this).trigger("wizardPanelShown");
	        });

	        // bind to the publish, check-in and save buttons
	        buttons.bind("click", function()
	        {
	            Ektron.eWebEditPro.Tab.hideEWebEditPro();
	        });
	    },

	    setPanelHeight: function(tabPanelShown, tabsNav, tabContainer)
	    {
            var divHeight = tabPanelShown.outerHeight();
            if (tabsNav.is(":visible"))
            {
                divHeight += tabsNav.outerHeight();
            }
            tabContainer.css("height", divHeight + "px");
	    }
	};
}



Ektron.ready(function()
{
    Ektron.eWebEditPro.Tab.init();
});