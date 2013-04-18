if (Ektron.PFWidgets === undefined) {
	Ektron.PFWidgets = {};
}

Ektron.PFWidgets.MetadataList = {
    parentID:"",
    webserviceURL: "/pagebuilder/widgets/metadatalist/CBHandler.ashx",
    setupAll: function(widgetId){
        Ektron.PFWidgets.MetadataList.refreshTabs();
        Ektron.PFWidgets.MetadataList.Folder.ConfigFolderTree(widgetId);
    },
    refreshTabs: function(){
        Ektron.PFWidgets.MetadataList.Tabs.init();
        $ektron("#CBtabInterface").fadeIn(250);
    },
    createRequestObj: function(action, objectid){
        request = {
            "action": action,
            "objectID": objectid
        };
	    return Ektron.JSON.stringify(request);
	},

    Tabs:
    {
        init: function()
        {
            $ektron(".MDTabInterface ul li.MDTab a").bind("click", function(e)
                {
                    var parentLi = $ektron(this).parent();
                    var tabsContainer = parentLi.parents(".MDTabInterface");
                    var poundChar = $ektron(this).attr("href").indexOf("#") + 1;
                    var targetPanelSelector = $ektron(this).attr("href").substring(poundChar);
                    var targetPanel = tabsContainer.find("." + targetPanelSelector);
                    parentLi.addClass("selected").siblings().removeClass("selected");
                    tabsContainer.find(".MDTabPanel").hide();
                    targetPanel.show();
                    return false;
                }
            );
            $ektron(".MDTabInterface .selected a").click();
        }
    }
};

Ektron.PFWidgets.MetadataList.Folder = {
	DirectoryToHtml: function(directory){
        var html = "";
        for(var i in directory.subdirectories)
        {
            html += "<li class=\"closed";
            if(i == directory.subdirectories.length - 1)
            {
                html += " last";
            }
            html += "\"><span class=\"folder\" data-ektron-folid=\"" + directory.subdirectories[i].id + "\">" +
                directory.subdirectories[i].name + "</span>";
                if(directory.subdirectories[i].haschildren)
                {
                    html += "<ul data-ektron-folid=\"" + directory.subdirectories[i].id + "\"></ul>";
                }
                html += "</li>";
        }

        return html;
    },
    ConfigFolderTree: function(widgetId){
        
        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.MetadataList.webserviceURL,
            data: {"request" : Ektron.PFWidgets.MetadataList.createRequestObj("getchildfolders", 0) },
            success: function(msg)
            {
                var directory = eval("(" + msg + ")");
                $ektron("ul.EktronFolderTree").html(Ektron.PFWidgets.MetadataList.Folder.DirectoryToHtml(directory));
                $ektron("ul.EktronFolderTree").treeview(
                {
                    toggle : function(index, element)
                    {
                        var $element = $ektron(element);

                        if($element.html() === "")
                        {
                            $ektron.ajax(
                            {
                                type: "POST",
                                cache: false,
                                async: false,
                                url: Ektron.PFWidgets.MetadataList.webserviceURL,
                                data: {"request" : Ektron.PFWidgets.MetadataList.createRequestObj("getchildfolders", $element.attr("data-ektron-folid")) },
                                success: function(msg)
                                {
                                    var directory = eval("(" + msg + ")");
                                    var el = $(Ektron.PFWidgets.MetadataList.Folder.DirectoryToHtml(directory));
                                    $element.append(el);
                                    $("ul.EktronFolderTree").treeview({add: el});
                                    Ektron.PFWidgets.MetadataList.Folder.configClickAction(widgetId);
                                }
                            });
                        }
                    }
                });
                Ektron.PFWidgets.MetadataList.Folder.configClickAction(widgetId);
                Ektron.PFWidgets.MetadataList.Folder.openToSelectedContent(widgetId);
            }
        });
    },
    openToSelectedContent: function(widgetId){
        var fid = $ektron("#" + widgetId + " .hdnFolderPath");
        if(fid.length === 0)
        {
            return true;
        }
        fid = fid.val().split(',');
        if(fid.length <= 1)
        {
            return true;
        }
        //now use fid to open all the folders
        for(var i=fid.length; i>=0; i--){
            var clicktarget = null;
            if(i!==0)
            {
                clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='"+fid[i]+"']").parent().children("div.expandable-hitarea");
                if(clicktarget.length > 0)
                {
                    clicktarget.click();
                }
            }
            else
            {
                clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='"+fid[i]+"']");
                $ektron("#" + widgetId + " .EktronFolderTree span").removeClass("selected");
                clicktarget.addClass("selected");
            }
        }
    },
    configClickAction: function(widgetId){
        $ektron("#" + widgetId + " .CBfoldercontainer span.folder").unbind("click").click(function(){
            var selectedfol = $ektron(this);
            var folval = selectedfol.attr("data-ektron-folid");
            if(folval != null){
                var fid = $ektron("#" + widgetId + " .folderid").val(folval);
                $ektron("#" + widgetId + " .EktronFolderTree span").removeClass("selected");
                selectedfol.addClass("selected");
                $ektron("#" + widgetId + " .MDTabInterface .MDTabWrapper a[href*='#SourceOptions']").click();
            }
            return false;
        }).unbind("hover").hover(function(){ $ektron(this).addClass("hover"); }, function(){ $ektron(this).removeClass("hover"); });
    }
};