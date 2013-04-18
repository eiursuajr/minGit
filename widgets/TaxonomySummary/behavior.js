if (Ektron.PFWidgets === undefined) {
    Ektron.PFWidgets = {};
}

Ektron.PFWidgets.TaxonomySummary = {
    parentID: "",
    webserviceURL: "/pagebuilder/widgets/taxonomysummary/TSHandler.ashx",
    themesPath: "widgets/taxonomysummary/themes/",
    themesPreviewPage: "Preview.aspx",
    setupAll: function() {
        Ektron.PFWidgets.TaxonomySummary.refreshTabs();
        Ektron.PFWidgets.TaxonomySummary.Folder.ConfigFolderTree();
    },
    refreshTabs: function() {
        Ektron.PFWidgets.TaxonomySummary.Tabs.init();
        $ektron("#TSTabInterface").fadeIn(250);
    },

    createRequestObj: function(action, pagenum, searchtext, objecttype, objectid) {
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
        init: function() {
            $ektron(".TSTabInterface ul li.TSTab a").bind("click", function(e) {
                var parentLi = $ektron(this).parent();
                var tabsContainer = parentLi.parents(".TSTabInterface");
                var poundChar = $ektron(this).attr("href").indexOf("#") + 1;
                var targetPanelSelector = $ektron(this).attr("href").substring(poundChar);
                var targetPanel = tabsContainer.find("." + targetPanelSelector);
                parentLi.addClass("selected").siblings().removeClass("selected");
                tabsContainer.find(".TSTabPanel").hide();
                targetPanel.show();
                return false;
            }
            );
        }
    },
    themesPreview: function(listID, prevwID, SitePath) {
        var list = document.getElementById(listID);
        var themesName = list.options[list.selectedIndex].value;

        var prevwUrl = SitePath + "Widgets/taxonomysummary/" + Ektron.PFWidgets.TaxonomySummary.themesPreviewPage + "?theme=" + themesName;
        document.getElementById(prevwID).src = prevwUrl;
    },
    SetTabs:
    {
        init: function() {
            //var tabsContainer = $ektron(".TSTabInterface");
            //var panelProperty = tabsContainer.find(".ByProperty");          
            //panelProperty.css({ "display": "block" });
            //tabsContainer.find(".ByProperty").show();
            //tabsContainer.find(".ByTaxonomy").hide();
            //tabsContainer.find(".ByThemes").hide();
            var linkP = $ektron(".TSTabInterface ul li.TSTab a.LinkProperty");
            if (linkP) {
                linkP.click();
            }
            return false;
        }
    }

};


Ektron.PFWidgets.TaxonomySummary.Folder = {
    DirectoryToHtml: function(directory) {
        var html = "";
        for (var i in directory.subdirectories) {
            html += "<li class=\"closed";
            if (i == directory.subdirectories.length - 1) {
                html += " last";
            }
            //html += "\"><span class=\"folder\" data-ektron-folid=\"" + directory.subdirectories[i].id + "\">" + directory.subdirectories[i].name + "</span>";
            html += "\"><span class=\"folder\" path=\"" + directory.subdirectories[i].path + "\" data-ektron-folid=\"" + directory.subdirectories[i].id + "\">" + directory.subdirectories[i].name + "</span>";
            if (directory.subdirectories[i].haschildren) {
                html += "<ul data-ektron-folid=\"" + directory.subdirectories[i].id + "\"></ul>";
            }
            html += "</li>";
        }

        return html;
    },
    ConfigFolderTree: function() {
        $ektron.ajax({
            type: "POST",
            cache: false,
            async: false,
            url: Ektron.PFWidgets.TaxonomySummary.webserviceURL,
            data: { "request": Ektron.PFWidgets.TaxonomySummary.createRequestObj("getchildtaxonomies", 0, "", "taxonomy", 0) },
            success: function(msg) {
                var directory = eval("(" + msg + ")");
                $ektron("ul.EktronFolderTree").html(Ektron.PFWidgets.TaxonomySummary.Folder.DirectoryToHtml(directory));
                $ektron("ul.EktronFolderTree").treeview(
                {
                    toggle: function(index, element) {
                        var $element = $ektron(element);

                        if ($element.html() === "") {
                            $ektron.ajax(
                            {
                                type: "POST",
                                cache: false,
                                async: false,
                                url: Ektron.PFWidgets.TaxonomySummary.webserviceURL,
                                data: { "request": Ektron.PFWidgets.TaxonomySummary.createRequestObj("getchildtaxonomies", 0, "", "taxonomy", $element.attr("data-ektron-folid")) },
                                success: function(msg) {
                                    var directory = eval("(" + msg + ")");
                                    var el = $(Ektron.PFWidgets.TaxonomySummary.Folder.DirectoryToHtml(directory));
                                    $element.append(el);
                                    $("ul.EktronFolderTree").treeview({ add: el });
                                    Ektron.PFWidgets.TaxonomySummary.Folder.configClickAction();
                                }
                            });
                        }
                    }
                });
                Ektron.PFWidgets.TaxonomySummary.Folder.configClickAction();
                Ektron.PFWidgets.TaxonomySummary.Folder.openToSelectedContent();
            }
        });
    },
    openToSelectedContent: function() {
        var cid = $ektron(".TSWidget .HiddenTBData");
        if (cid.length === 0) {
            return true;
        }
        cid = cid.val();

        var fid = $ektron(".TSWidget .HiddenTBFolderPath");
        if (fid.length === 0) {
            return true;
        }
        fid = fid.val().split(',');
        if (fid.length <= 1) {
            return true;
        }

        //now use fid to open all the folders
        for (var i = fid.length; i >= 0; i--) {
            var clicktarget = null;
            if (i != 0) {
                clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='" + fid[i] + "']").parent().children("div.expandable-hitarea");
            }
            else {
                clicktarget = $ektron("ul.EktronFolderTree span.folder[data-ektron-folid='" + fid[i] + "']");
            }

            if (clicktarget.length > 0) {
                clicktarget.click();
            }
        }
        //now scroll to the folder
        $ektron(".TSEdit #ByFolder").scrollTo("ul.EktronFolderTree span.folder[data-ektron-folid='" + fid[0] + "']");


    },

    configClickAction: function() {
        $ektron("div.TSfoldercontainer span[data-ektron-folid]").unbind("click").click(function() {
            $ektron("div.TSfoldercontainer .selected").removeClass("selected");
            $ektron(this).addClass("selected");
            var objectID = $ektron(this).attr("data-ektron-folid");
            var objectPath = $ektron(this).attr("path");
            Ektron.PFWidgets.TaxonomySummary.parentID = $ektron(this).parents("div.TSWidget").attr("id");
            var textbox = $ektron("#" + Ektron.PFWidgets.TaxonomySummary.parentID).find(".folderid");
            $ektron(textbox).val(objectID);

            var taxonomypath = $ektron("#" + Ektron.PFWidgets.TaxonomySummary.parentID).find(".taxonomypath");
            $ektron(taxonomypath).html(objectPath);
            var taxopath = $ektron("#" + Ektron.PFWidgets.TaxonomySummary.parentID).find(".curPath");
            $ektron(taxopath).html(objectPath);

        });
    }
};

