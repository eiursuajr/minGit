if (Ektron.PFWidgets === undefined) {
	Ektron.PFWidgets = {};
}

Ektron.PFWidgets.ContentBlock = {
    parentID: "",
    webserviceURL: "/pagebuilder/widgets/contentblock/CBHandler.ashx",
    setupAll: function(widgetId) {
        
        Ektron.PFWidgets.ContentBlock.refreshTabs();
        Ektron.PFWidgets.ContentBlock.Taxonomy.configTaxonomyTreeView();
        Ektron.PFWidgets.ContentBlock.Folder.ConfigFolderTree(widgetId);
        Ektron.PFWidgets.ContentBlock.Search.ConfigSearch();
        var localparent = $ektron("#" + widgetId);
        localparent.find(".CBAddForm").hide();
        Ektron.PFWidgets.ContentBlock.configDropdown(widgetId);
    },
    refreshTabs: function() {
        Ektron.PFWidgets.ContentBlock.Tabs.init();
        $ektron("#CBtabInterface").fadeIn(250);
    },
    toggleResultsPane: function() {
        if ($ektron("#CBResults").children().length == 0) {
            $ektron("div.CBfoldercontainer > span.folder[data-ektron-folid='0']").click();
        } else {
            $ektron("#CBResults").slideToggle(750);
        }
        return false;
    },

    getResults: function(action, objectID, pageNum, objecttype, search, el) {
        var str = Ektron.PFWidgets.ContentBlock.createRequestObj(action, pageNum, search, objecttype, objectID);

        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.ContentBlock.webserviceURL,
            data: { "request": str },
            success: function(msg) {
                var contentitems = eval("(" + msg + ")");
                if (el != "") {
                    Ektron.PFWidgets.ContentBlock.parentID = $ektron(el).parents("div.CBWidget");
                    Ektron.PFWidgets.ContentBlock.parentID = $ektron(Ektron.PFWidgets.ContentBlock.parentID).attr("id");
                }
                var CBResults = $ektron("#" + Ektron.PFWidgets.ContentBlock.parentID);
                CBResults = $ektron(CBResults).find("#CBResults");
                CBResults.html(Ektron.PFWidgets.ContentBlock.ContentToHtml(contentitems));

                $ektron(CBResults).find("div.CBresult").click(function() {
                    Ektron.PFWidgets.ContentBlock.parentID = $ektron(this).parents("div.CBWidget").attr("id");
                    $ektron(CBResults).find("div.CBresult").removeClass("selected");
                    $ektron(this).addClass("selected");
                }).bind("dblclick", function() {
                    $ektron("div.CBEditControls input.CBSave").click();
                });
                //CBResults.cluetip({cursor:'pointer', arrows:true, leftOffset:'25px'});
                $ektron(CBResults).find("div.CBresult").cluetip({ positionBy: "bottomTop", cursor: 'pointer', arrows: true, leftOffset: "25px", topOffset: "20px", cluezIndex: 9999 });
                $ektron("#" + Ektron.PFWidgets.ContentBlock.parentID).find("#CBPaging").html(contentitems.paginglinks);
                //$ektron("#CBPaging").html(contentitems.paginglinks);

                CBResults.slideDown(750);
            }
        });
        return false;
    },

    createRequestObj: function(action, pagenum, searchtext, objecttype, objectid) {
        request = {
            "action": action,
            "filter": ($ektron("select#CBTypeFilter")[0] ? $ektron("select#CBTypeFilter")[0].value : objecttype),
            "page": pagenum,
            "searchText": searchtext,
            "objectType": objecttype,
            "objectID": objectid
        };
        return Ektron.JSON.stringify(request);
    },
    ContentToHtml: function(contentlist) {
        var html = "";
        if (contentlist.contents === null || contentlist.contents.length === 0) {
            html = "<div class=\"CBNoresults\">No Results</div>";
        } else {
			for(var i = 0; i < contentlist.contents.length; i++) {
                html += "<div ";
                html += "class=\"CBresult " + ((i % 2 === 0) ? "even" : "odd") + "\" ";
                html += "rel=\"" + Ektron.PFWidgets.ContentBlock.webserviceURL + "?detail=" + contentlist.contents[i].id + "\" ";
                html += "title=\"" + contentlist.contents[i].title + "\">";
                html += "<span class=\"title\">" + contentlist.contents[i].title + "</span>";
                html += "<span class=\"contentid\">" + contentlist.contents[i].id + "</span>";
                html += "<br class=\"clearall\" />";
                html += "</div>";
            }
        }
        return html;
    },
    Save: function() {
        var el = ((Ektron.PFWidgets.ContentBlock.parentID.length > 0)
                ? $ektron("#" + Ektron.PFWidgets.ContentBlock.parentID).find("#CBResults").find("div.selected")
                : null);
        if (el === null || el.length === 0) {
            alert("Please select a piece of content to display.");
        } else if (el.length > 1) {
            alert("You have selected multiple pieces of content. You shouldn't do that.");
        } else {
            $ektron("#" + Ektron.PFWidgets.ContentBlock.parentID).find(".HiddenTBData")[0].value = parseInt(el.children("span.contentid").html(), 10);
            //make sure any cluetips get lost
            $ektron("#cluetip, #cluetip-waitimage").remove();
            return true;
        }
        return false;
    },
    configDropdown: function(widgetid) {
        var dropdown = $ektron("#" + widgetid + " select#CBTypeFilter");
        dropdown.change(function() {
            var selectedtab = $ektron("#" + widgetid + " .CBTabWrapper .selected a");
            selectedtab = selectedtab[0].href.substr(selectedtab[0].href.indexOf("#") + 1);
            var tabcontainer = $ektron("#" + widgetid + " ." + selectedtab);
            if (selectedtab != "BySearch") {
                //if folder is visible tab, reselect current folder
                //if taxonomy is visible tab, reselect current taxnode
                tabcontainer.find("span.selected").click();
            } else {
                //if search is visible tab, rerun search
                tabcontainer.find("a.searchSubmit").click();
            }
        });
    },
    Tabs:
    {
        init: function() {
            $ektron(".CBTabInterface ul li.CBTab a").bind("click", function(e) {
                var parentLi = $ektron(this).parent();
                var tabsContainer = parentLi.parents(".CBTabInterface");
                var poundChar = $ektron(this).attr("href").indexOf("#") + 1;
                var targetPanelSelector = $ektron(this).attr("href").substring(poundChar);
                var targetPanel = tabsContainer.find("." + targetPanelSelector);
                parentLi.addClass("selected").siblings().removeClass("selected");
                tabsContainer.find(".CBTabPanel").hide();
                targetPanel.show();
                return false;
            }
            );
        }
    }
};


Ektron.PFWidgets.ContentBlock.Taxonomy = {
	configTaxonomyTreeView: function(){
	    var els = $ektron("ul.EktronTaxonomyTree");
        for(var i=0; i<els.length; i++){
            $ektron.ajax({
                type: "POST",
                cache: false,
                async: false,
                url: Ektron.PFWidgets.ContentBlock.webserviceURL,
                data: {"request" : Ektron.PFWidgets.ContentBlock.createRequestObj("getchildtaxonomy", 0, "", "taxonomy", $ektron(els[i]).attr("data-ektron-taxid")) },
                success: function(msg)
                {
                    $ektron(els[i]).html(msg);
                    Ektron.PFWidgets.ContentBlock.Taxonomy.configTreeview(els[i]);
                    Ektron.PFWidgets.ContentBlock.Taxonomy.configClickAction();
                    var $TaxPath = $ektron(".HiddenTBTaxonomyPath");
                    if($TaxPath.length > 0)
                    {
                        Ektron.PFWidgets.ContentBlock.Taxonomy.openToSelectedTaxonomy();
                    }
                }
            });
        }
	},
    configTreeview: function(el){
        $ektron(el).treeview({
            toggle : function(index, element) {
                var $element = $ektron(element);
                if($element.html() === ""){
                    $ektron.ajax(
                    {
                        type: "POST",
                        cache: false,
                        async: false,
                        url: Ektron.PFWidgets.ContentBlock.webserviceURL,
                        data: {"request" : Ektron.PFWidgets.ContentBlock.createRequestObj("getchildtaxonomy", 0, "", "taxonomy", $element.attr("data-ektron-taxid")) },
                        success: function(msg)
                        {
                            var thisel = $ektron(msg);
                            $element.append(thisel);
                            $ektron(el).treeview({add: thisel});
                            Ektron.PFWidgets.ContentBlock.Taxonomy.configClickAction();
                        }
                    });
                }
            }
        });
    },
    configClickAction: function(){
        $ektron("div.treecontainer span[data-ektron-taxid]").unbind("click").click(function(){
            $ektron("div.treecontainer .selected").removeClass("selected");
            $ektron(this).addClass("selected");
            var objectID = $ektron(this).attr("data-ektron-taxid");
            var pageNum = 0;
            var action = "gettaxonomycontent";
            var objecttype = "taxonomy";
            var parent = $ektron(this).parents("div.UCTaxTree");
            if (0 == parent.length)
            {
                //gettaxonomycontent: drill into the content list in the selected taxonomy
                Ektron.PFWidgets.ContentBlock.getResults(action, objectID, pageNum, objecttype, "",this);
            }
            else
            {
                //remember the current selected taxonomy id in a text box.
                var textbox = $ektron("#" + parent.attr("id")).find(".taxid");
                $ektron(textbox).val(objectID);
                var resTitle = $ektron(this).text();
                var titlebox = $ektron("#" + parent.attr("id")).parent(".filtercontainer").find(".restitle");
                if (titlebox.length > 0)
                {
                    $ektron(titlebox).val(resTitle);
                }
            }
        });
    },
    openToSelectedTaxonomy: function()
    {
        var tid = $ektron(".HiddenTBTaxonomyPath"); //a list of taxonomy id (taxonomy path) to reload the previous selection
        if (0 === tid.length)
        {
            return true;
        }
        var tid = tid.val().split(",");
        if (0 === tid.length || 0 === tid[0].length)
        {
            return true;
        }
        //now use tid to open all the folders 
        for (var i = 0; i < tid.length; i++)
        {
            var clicktarget = null;
            if (i != tid.length - 1)
            {
                clicktarget = $ektron("div.treecontainer span.folder[data-ektron-taxid='" + tid[i] + "']").parent().children("div.expandable-hitarea");
            }
            else
            {
                clicktarget = $ektron("div.treecontainer span.folder[data-ektron-taxid='" + tid[i] + "']");
            }

            if(clicktarget.length > 0)
            {
                clicktarget.click();
            }
        }
        //now scroll to the folder
        $ektron("#PageViewTaxonomy").scrollTo("div.treecontainer span.folder[data-ektron-taxid='" + tid[tid.length - 1] + "']");
    }
};


Ektron.PFWidgets.ContentBlock.Folder = {
	DirectoryToHtml: function(directory){
        var html = "";
        for(var i = 0; i < directory.subdirectories.length; i++)
        {
            html += "<li class=\"closed";
            if(i == directory.subdirectories.length - 1)
            {
                html += " last";
            }
            html += "\"><span class=\"folder\" data-ektron-canadd=\"" + directory.subdirectories[i].canadd + "\"data-ektron-folid=\"" + directory.subdirectories[i].id + "\"data-ektron-foltype=\""+ directory.subdirectories[i].type + "\" >" +
                directory.subdirectories[i].name + "</span>";
                if(directory.subdirectories[i].haschildren)
                {
                    html += "<ul  data-ektron-canadd=\"" + directory.subdirectories[i].canadd + "\"data-ektron-folid=\"" + directory.subdirectories[i].id + "\"data-ektron-foltype=\""+ directory.subdirectories[i].type + "\"></ul>";
                }
                html += "</li>";
        }

        return html;
    },
    ConfigFolderTree: function(id){
        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.ContentBlock.webserviceURL,
            data: {"request" : Ektron.PFWidgets.ContentBlock.createRequestObj("getchildfolders", 0, "", "folder", 0) },
            success: function(msg)
            {
                var directory = eval("(" + msg + ")");
                $ektron("#"+id+" ul.EktronFolderTree").html(Ektron.PFWidgets.ContentBlock.Folder.DirectoryToHtml(directory));
                $ektron("#"+id+" ul.EktronFolderTree").treeview(
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
                                url: Ektron.PFWidgets.ContentBlock.webserviceURL,
                                data: {"request" : Ektron.PFWidgets.ContentBlock.createRequestObj("getchildfolders", 0, "", "folder", $element.attr("data-ektron-folid")) },
                                success: function(msg)
                                {
                                    var directory = eval("(" + msg + ")");
                                    var el = $ektron(Ektron.PFWidgets.ContentBlock.Folder.DirectoryToHtml(directory));
                                    $element.append(el);
                                    $ektron("#"+id+" ul.EktronFolderTree").treeview({add: el});
                                    Ektron.PFWidgets.ContentBlock.Folder.configClickAction();
                                }
                            });
                        }
                    }
                });
                Ektron.PFWidgets.ContentBlock.Folder.configClickAction();
                Ektron.PFWidgets.ContentBlock.Folder.openToSelectedContent();
            }
        });
    },
    openToSelectedContent: function(){
        var cid = $ektron(".CBEdit .HiddenTBData");
        if(cid.length === 0)
        {
            return true;
        }
        cid = cid.val();

        var fid = $ektron(".CBEdit .HiddenTBFolderPath");
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
            if(i!==0)
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
        $ektron(".CBEdit #ByFolder").scrollTo("ul.EktronFolderTree span.folder[data-ektron-folid='"+fid[0]+"']");

        //now select the content and scroll to it
        var citem = $ektron(".CBEdit #CBResults .CBresult span.contentid:contains('"+cid+"')").parent();
        if(citem.length > 0){
            citem.click();
            $ektron(".CBEdit #CBResults").scrollTo(citem);
        }
    },
    configClickAction: function(){
        $ektron("div.CBfoldercontainer span[data-ektron-folid]").unbind("click").click(function(){
            $ektron("div.CBfoldercontainer .selected").removeClass("selected");
            $ektron(this).addClass("selected");
            var objectID = $ektron(this).attr("data-ektron-folid");
            var FolderType = $ektron(this).attr("data-ektron-foltype");
            var CanAdd =  $ektron(this).attr("data-ektron-canadd");
            $ektron("#hdnFolderId").val(objectID);
            var pageNum = 0;
            var action = "getfoldercontent";
            var objecttype = "folder";
            var localparent = $ektron(this).parents("div.CBWidget");
            Ektron.PFWidgets.ContentBlock.parentID = localparent.attr("id");

            if(localparent.find("select#CBTypeFilter")[0].value.toLowerCase() == "forms")
            {
              localparent.find(".CBAdd").hide();
              localparent.find(".CBAddForm").show();
            }
            else
            {
              localparent.find(".CBAddForm").hide();
              localparent.find(".CBAdd").show();
            }
            if(FolderType == "1" || FolderType =="3" || FolderType == "4" || CanAdd == "false")
            {
                localparent.find(".CBAdd").hide();
            }
            Ektron.PFWidgets.ContentBlock.getResults(action, objectID, pageNum, objecttype, "", this);
        });
    }
};

Ektron.PFWidgets.ContentBlock.Search = {
	DoSearch: function(el){
        var objectID = 0;
        var pageNum = 0;
        var action = "search";
        var objecttype = "search";
        var searchtext = $ektron("div.BySearch input.searchtext")[0].value;
        Ektron.PFWidgets.ContentBlock.getResults(action, objectID, pageNum, objecttype, searchtext,el);
        return false;
    },
    ConfigSearch: function(){
        $ektron("div.BySearch input.searchtext").bind("keydown", function(evt){
            evtElement = $ektron(this)[0];
            if(evt.keyCode == 13){
                evt.preventDefault();
                evt.stopPropagation();
                evt.returnValue = false;
                evt.cancel = true;
                setTimeout(function(){
                    Ektron.PFWidgets.ContentBlock.Search.DoSearch(evtElement);
                }, 1);
                return false;
            }
        });
    }
};