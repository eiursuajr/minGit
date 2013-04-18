if (Ektron.PFWidgets === undefined) {
	Ektron.PFWidgets = {};
}

Ektron.PFWidgets.ListSummary = {
    parentID:"",
    webserviceURL: "/pagebuilder/widgets/listsummary/LSHandler.ashx",
    setupAll: function(){
        Ektron.PFWidgets.ListSummary.refreshTabs();
        Ektron.PFWidgets.ListSummary.Folder.ConfigFolderTree();
      },
    refreshTabs: function(){
        Ektron.PFWidgets.ListSummary.Tabs.init();
        $ektron("#LSTabInterface").fadeIn(250);
    },

    createRequestObj: function(action, pagenum, searchtext, objecttype, objectid){
        request = {
            "action": action,
            "filter": "content",
            "page": pagenum,
            "searchText": searchtext,
            "objectType": objecttype,
            "objectID": objectid
        };
	    return Ektron.JSON.stringify(request);
	},
   Tabs:
    {
        init: function()
        {
            $ektron(".LSTabInterface ul li.LSTab a").bind("click", function(e)
                {
                    var parentLi = $ektron(this).parent();
                    var tabsContainer = parentLi.parents(".LSTabInterface");
                    var poundChar = $ektron(this).attr("href").indexOf("#") + 1;
                    var targetPanelSelector = $ektron(this).attr("href").substring(poundChar);
                    var targetPanel = tabsContainer.find("." + targetPanelSelector);
                    parentLi.addClass("selected").siblings().removeClass("selected");
                    tabsContainer.find(".LSTabPanel").hide();
                    targetPanel.show();
                    return false;
                }
            );
        }
    }
};


Ektron.PFWidgets.ListSummary.Folder = {
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
    ConfigFolderTree: function(){
        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.ListSummary.webserviceURL,
            data: {"request" : Ektron.PFWidgets.ListSummary.createRequestObj("getchildfolders", 0, "", "folder", 0) },
            success: function(msg)
            {
                var directory = eval("(" + msg + ")");
                $ektron("ul.EktronFolderTree").html(Ektron.PFWidgets.ListSummary.Folder.DirectoryToHtml(directory));
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
                                url: Ektron.PFWidgets.ListSummary.webserviceURL,
                                data: {"request" : Ektron.PFWidgets.ListSummary.createRequestObj("getchildfolders", 0, "", "folder", $element.attr("data-ektron-folid")) },
                                success: function(msg)
                                {
                                    var directory = eval("(" + msg + ")");
                                    var el = $ektron(Ektron.PFWidgets.ListSummary.Folder.DirectoryToHtml(directory));
                                    $element.append(el);
                                    $ektron("ul.EktronFolderTree").treeview({add: el});
                                    Ektron.PFWidgets.ListSummary.Folder.configClickAction();
                                }
                            });
                        }
                    }
                });
                Ektron.PFWidgets.ListSummary.Folder.configClickAction();
                Ektron.PFWidgets.ListSummary.Folder.openToSelectedContent();
            }
        });
    },
    openToSelectedContent: function(){
        var cid = $ektron(".LSWidget .HiddenTBData");
        if(cid.length === 0) 
        {
            return true;
        }  
        cid = cid.val();

        var fid = $ektron(".LSWidget .HiddenTBFolderPath");
        if(fid.length === 0) 
        {
            return true;
        }
        fid = fid.val().split(',');
        if(fid.length < 1)
        {
             return true;
        }

        //now use fid to open all the folders
        for(var i=fid.length; i>=0; i--){
            var clicktarget = null;
            if(i!=0)
            {
                 clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='"+fid[i]+"']").parent().children("div.expandable-hitarea");
            }
            else
            {
               clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='"+fid[i]+"']");
            }

            if(clicktarget.length > 0)
            {
                clicktarget.click();
            }
        }
        //now scroll to the folder
        $ektron(".LSEdit #ByFolder").scrollTo("ul.EktronFolderTree span.folder[data-ektron-folid='"+fid[0]+"']");

        
    },
    configClickAction: function(){
        $ektron("div.LSfoldercontainer span[data-ektron-folid]").unbind("click").click(function(){
            $ektron("div.LSfoldercontainer .selected").removeClass("selected");
            $ektron(this).addClass("selected");
            var objectID = $ektron(this).attr("data-ektron-folid");
            Ektron.PFWidgets.ListSummary.parentID= $ektron(this).parents("div.LSWidget").attr("id");
            var textbox = $ektron("#" + Ektron.PFWidgets.ListSummary.parentID).find(".folderid");
            $ektron(textbox).val(objectID);
            var resTitle = $ektron(this).text();
            var titlebox = $ektron("#" + Ektron.PFWidgets.ListSummary.parentID).parent(".filtercontainer").find(".restitle");
            if (titlebox.length > 0)
            {
                $ektron(titlebox).val(resTitle);
            }
        });
    }
};

