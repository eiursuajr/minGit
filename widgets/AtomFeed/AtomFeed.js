// JScript File
if ("undefined" === typeof Ektron.Widgets)
{
    Ektron.Widgets = {}
}
if ("undefined" === typeof Ektron.Widgets.AtomFeed)
{
    Ektron.Widgets.AtomFeed = 
    {
        init: function()
        {          
            $ektron(".atomFeedSaveButton").live("click", function()
            {
                var clicked = $ektron(this);                
                clicked.parents(".atomFeedEdit").hide("fast");
                clicked.parents(".atom").find(".atomFeedSaving").show("fast");
            });
            
        }
    }
}
Ektron.Widgets.AtomFeed.init();