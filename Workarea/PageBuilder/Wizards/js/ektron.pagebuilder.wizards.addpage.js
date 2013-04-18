Ektron.ready(function() {
    // ensure our object heirarchy exists
    if ("undefined" === typeof Ektron.PageBuilder) {
        Ektron.PageBuilder = {};
    }
    if ("undefined" === typeof Ektron.PageBuilder.Wizards) {
        Ektron.PageBuilder.Wizards = {};
    }
    if ("undefined" === typeof Ektron.PageBuilder.Wizards.AddPage) {
        Ektron.PageBuilder.Wizards.AddPage =
        {
            init: function() {
                // initialize Templates
                Ektron.PageBuilder.Wizards.AddPage.Templates.init();

                // initialize Tabs
                Ektron.PageBuilder.Wizards.AddPage.Tabs.init();

                // initialize aliasing
                Ektron.PageBuilder.Wizards.AddPage.Aliasing.init();
            },

            Tabs:
            {
                // PROPERTIES
                initialized: false,

                init: function() {
                    if (Ektron.PageBuilder.Wizards.AddPage.Tabs.initialized == true) {
                        return;
                    }

                    $ektron(".ektronPageBuilderTab a").bind("click", function(e) {
                        var parentLi = $ektron(this).parent();
                        var targetPanelSelector = $ektron(this).attr("href");
                        var targetPanel = $ektron(targetPanelSelector + "");
                        parentLi.addClass("selected").siblings().removeClass("selected");
                        $ektron(".ektronPageBuilderTabPanel").hide();
                        targetPanel.fadeIn();
                        return false;
                    }
                    );

                    Ektron.PageBuilder.Wizards.AddPage.Tabs.initialized = true;
                }

            },

            Aliasing:
            {
                // PROPERTIES
                initialized: false,
                titlePlaceHolder: 'Title',

                init: function() {
                    if (Ektron.PageBuilder.Wizards.AddPage.Aliasing.initialized == true) {
                        return;
                    }

                    if ($ektron("#pageBuilderWizardAlias").length > 0) {
                        Ektron.PageBuilder.Wizards.AddPage.Aliasing.initManual();
                    }

                    Ektron.PageBuilder.Wizards.AddPage.Aliasing.initialized = true;
                },
                UpdateAliasTitles: function() {
                    var placeHolder = Ektron.PageBuilder.Wizards.AddPage.Aliasing.titlePlaceHolder;
                    var curtitle = $ektron("span#aliasValue").text();
                    if (curtitle == "") curtitle = "Title";
                    var aliases = $ektron("li[data-ektron-rawalias]");
                    for (var i = 0; i < aliases.length; i++) {
                        var el = $ektron(aliases[i]);
                        el.text(el.attr("data-ektron-rawalias").replace(placeHolder, curtitle));
                    }
                },
                TaxonomyChangedCallBack: function(taxids) {
                    getParents = function(id) {
                        var idlist = id;
                        var els = $ektron("span.folder[data-ektron-taxid='" + id + "']").parents("ul[data-ektron-taxid]");
                        for (var i = 0; i < els.length; i++) {
                            idlist += "," + $ektron(els[i]).attr("data-ektron-taxid");
                        }
                        return idlist.split(",");
                    }

                    makeAliasDisplay = function(aliasUL, taxID, parentlist) {
                        //retrieve settings for this ul
                        aliasUL = $ektron(aliasUL);
                        var format = aliasUL.attr("data-ektron-aliastype"); // format is ContentTitle, ContentId, ContentIdAndLanguage (Id+L = contentid/langid.aspx)
                        var extension = aliasUL.attr("data-ektron-extension");
                        var replaceChar = aliasUL.attr("data-ektron-replacechar");
                        var excludePath = aliasUL.attr("data-ektron-excludepath");
                        var example = aliasUL.attr("data-ektron-format");

                        var curPath = "/";
                        for (var i = parentlist.length - 1; i >= 0; i--) {
                            curPath += $ektron("span.folder[data-ektron-taxid=" + parentlist[i] + "]").text() + "/";
                        }
                        if (excludePath != "") curPath = curPath.replace(excludePath, "");

                        var retval = "<li data-ektron-rawalias=\"" + curPath;
                        if (format == "ContentTitle") retval += Ektron.PageBuilder.Wizards.AddPage.Aliasing.titlePlaceHolder;
                        if (format == "ContentId") retval += "354";
                        if (format == "ContentIdAndLanguage") retval += "354/1033";
                        retval += extension + "\"></li>";
                        return retval;
                    }
                    taxids = taxids.split(",");
                    var aliasgroups = $ektron("ul.taxAlias");
                    for (var k = 0; k < aliasgroups.length; k++) {
                        var newcontents = "";
                        for (var i = 0; i < taxids.length; i++) {
                            var parentlist = getParents(taxids[i]);
                            for (var j = 0; j < parentlist.length; j++) {
                                if ($ektron(aliasgroups[k]).attr("data-ektron-taxroot") == parentlist[j]) {
                                    //this is a match. add the alias to the display
                                    newcontents += makeAliasDisplay(aliasgroups[k], taxids[i], parentlist);
                                }
                            }
                        }
                        $ektron(aliasgroups[k]).html(newcontents);
                    }
                    Ektron.PageBuilder.Wizards.AddPage.Aliasing.UpdateAliasTitles();
                },
                initManual: function() {
                    var aliasel = $ektron("#aliasValue");
                    var extel = $ektron("#extValue");

                    $ektron("#pageBuilderCreateManualAlias").click(function(e) {
                        if (this.checked) {
                            $ektron(".manualContainer").show();
                        } else {
                            $ektron(".manualContainer").hide();
                        }
                    });

                    $ektron("#ExtensionDropdown").click(function(e) {
                        var alias = $ektron("#pageBuilderWizardAlias")[0].value;
                        var opts = $ektron("#ExtensionDropdown > option");
                        var selectedopt = null;
                        for (var i = 0; i < opts.length; i++) {
                            var opt = opts[i].value;
                            if (alias.substring(alias.length - opt.length, alias.length) == opt) {
                                selectedopt = opts[i];
                                break;
                            }
                        }
                        if (selectedopt != null) {
                            opts.attr("disabled", "disabled")
                            $ektron(selectedopt).removeAttr("disabled");
                        }
                    }).bind("blur", function() {
                        $ektron("#ExtensionDropdown > option").removeAttr("disabled");
                        Ektron.PageBuilder.Wizards.AddPage.Support.checkVals();
                        Ektron.PageBuilder.Wizards.AddPage.Aliasing.UpdateAliasTitles();
                    }).bind("change", function() {
                        if ($ektron("#ExtensionDropdown").val() != "none") {
                            extel.html($ektron("#ExtensionDropdown").val());
                        }
                        Ektron.PageBuilder.Wizards.AddPage.Support.checkVals();
                        Ektron.PageBuilder.Wizards.AddPage.Aliasing.UpdateAliasTitles();
                    });

                    $ektron("#ExtensionDropdown").change();

                    $ektron("#pageBuilderWizardAlias").bind("keyup", function(e) {
                        var k = e.keyCode ? e.keyCode : e.charCode ? e.charCode : e.which;
                        var aliasname = "";
                        if (k == 32)
                            this.value = this.value.replace(" ", "_");
                        if (k > 31 || k == 8) { //ignore control chars but capture backspace
                            if (this.value == "") {
                                $ektron(this).removeAttr("Ektron-Wizard-ManualAlias");
                            } else {
                                //this.value = Ektron.PageBuilder.Wizards.AddPage.Support.cleanAlias(this.value);
                                $ektron(this).attr("Ektron-Wizard-ManualAlias", this.value);

                                aliasname = this.value;
                                var selectedext = $ektron("#ExtensionDropdown > option[selected]");
                                var opts = $ektron("#ExtensionDropdown > option");
                                //opts.removeAttr("selected")
                                for (var i = 0; i < opts.length; i++) {
                                    var opt = opts[i].value;
                                    if (opt.length < this.value.length) {
                                        if (this.value.substring(this.value.length - opt.length, this.value.length) == opt) {
                                            if (opt != "none") {
                                                $ektron(opts[i]).attr("selected", "selected");
                                                aliasname = this.value.substring(0, this.value.length - opt.length);
                                                extel.html(opt);
                                                break;
                                            }
                                        }
                                    }
                                }
                                var newselected = $ektron("#ExtensionDropdown > option[selected]");
                                if (newselected.length == 0 || newselected.val() == "none") {
                                    selectedext.attr("selected", "selected");
                                }

                            }
                            aliasel.html(aliasname);
                            Ektron.PageBuilder.Wizards.AddPage.Support.checkVals();
                        }
                        Ektron.PageBuilder.Wizards.AddPage.Aliasing.UpdateAliasTitles();
                    });

                    $ektron("#pageBuilderWizardPageTitle").bind("keypress", function(e) {
                        var charCheck;
                        var k = e.keyCode ? e.keyCode : e.charCode ? e.charCode : e.which;
                        if (String.fromCharCode(k) == "*"
                                    || String.fromCharCode(k) == "/"
                                    || String.fromCharCode(k) == "|"
                                    || String.fromCharCode(k) == "\""
                                    || String.fromCharCode(k) == ">"
                                    || String.fromCharCode(k) == "<"
                                    || String.fromCharCode(k) == "("
                                    || String.fromCharCode(k) == ")"
                                    || String.fromCharCode(k) == "\\") {
                            return false;
                        }
                    }).bind("keyup", function(e) {
                        var title = this;
                        var alias = $ektron("#pageBuilderWizardAlias");
                        if (alias.length > 0) {
                            alias = alias[0]
                            var manuallychanged = false;
                            var manualattr = $ektron(alias).attr("Ektron-Wizard-ManualAlias");

                            if (manualattr == null || manualattr == "") {
                                if (e == 32)
                                    alias.value += "_";
                                else
                                    alias.value = escape(unescape(title.value)).replace(/%20/g, '_');

                                aliasel.html(alias.value);
                            }
                            Ektron.PageBuilder.Wizards.AddPage.Support.checkVals();
                        }
                        Ektron.PageBuilder.Wizards.AddPage.Aliasing.UpdateAliasTitles();
                    });
                }
            },

            Templates:
            {
                // PROPERTIES
                initialized: false,

                init: function() {
                    if (Ektron.PageBuilder.Wizards.AddPage.Templates.initialized == true) {
                        return;
                    }

                    // bind mouseover effects
                    var templateWrapper = $ektron(".ektronPageBuilderPageLayouts");
                    var templates = $ektron(".ektronWizardStepWrapper .ektronTemplateList li");
                    if (templates.length > 0) {
                        templates.bind("mouseover",
                            function() {
                                $ektron(this).addClass("hover");
                            }
                        );
                        templates.bind("mouseout",
                            function() {
                                $ektron(this).removeClass("hover");
                            }
                        );
                        templates.bind("click",
                            function() {
                                var id = $ektron(this).attr("data-ektron-id");
                                var hiddenField = $ektron("#ektronSelectedTemplate");
                                $ektron(this).toggleClass("selected");
                                $ektron(this).siblings().removeClass("selected");
                                if (($ektron(this).attr("class")).indexOf("selected") > 0) {
                                    hiddenField.val(id);
                                }
                                else {
                                    hiddenField.val("");
                                }
                            }
                        );
                        //if there is a default template, select it, otherwise select the first template
                        var templateToSelect;
                        if (templates.filter(".defaultTemplate").length > 0) {
                            templateToSelect = templates.filter(".defaultTemplate");
                            templateToSelect.addClass("selected");
                            $ektron("#ektronSelectedTemplate").val(templateToSelect.attr("data-ektron-id"));
                        }
                        else {
                            templateToSelect = templates.filter(":first")
                            templateToSelect.addClass("selected");
                            $ektron("#ektronSelectedTemplate").val(templateToSelect.attr("data-ektron-id"));
                        }
                        // show or hide the templates based on mode
                        if (parent.Ektron.PageBuilder.Wizards.mode == "add") {
                            templateWrapper.show();
                        }
                        else {
                            templateWrapper.hide();
                        }
                    }

                    Ektron.PageBuilder.Wizards.AddPage.Templates.initialized = true;
                },

                getFolderResults: function(folderid) {
                    parent.Ektron.PageBuilder.Wizards.Status.loading();
                    return $ektron.ajax({
                        async: false,
                        url: Ektron.ResourceText.PageBuilder.Wizards.appPath + 'PageBuilder/Wizards/folderbrowser/folderbrowserCB.ashx',
                        type: 'POST',
                        data: ({ 'folderid': folderid })
                    }).responseText;
                },

                showChangeFolder: function() {
                    if ($ektron("div.ui-finder").length > 0) return false;
                    //get first level of contents
                    //$ektron("ul#finder").html(Ektron.PageBuilder.Wizards.AddPage.Templates.getFolderResults(0));

                    checkFolder = function(anchor, id) {
                        var selectedfolder = "Selected Folder: ";
                        var selections = $ektron(".ui-finder .ui-finder-list-item-active");
                        var titlebar = $ektron(".ektronPageBuilderPageLayoutsFolderSelector .ui-finder-header .ui-finder-title");
                        var startindex = 0;
                        if (selections.length > 2) {
                            startindex = selections.length - 2;
                            selectedfolder += "...";
                        }
                        for (var i = startindex; i < selections.length; i++) {
                            selectedfolder += "/" + $ektron(selections[i]).children("a").text();
                        }
                        selectedfolder += "/";
                        if (anchor.parent().hasClass("hasWireframe")) {
                            var header = selectedfolder + ' - this folder allows layouts';
                            titlebar.html(header);
                            $ektron(".ui-finder-header").addClass("Allowed").removeClass("NotAllowed");
                            $ektron(document)
                            //allowed folder
                        } else {
                            var header = selectedfolder + ' - this folder does not allow layouts';
                            titlebar.html(header);
                            $ektron(".ui-finder-header").removeClass("Allowed").addClass("NotAllowed");
                            //not allowed folder
                        }
                    };

                    $ektron("ul#finder").finder({
                        title: '',
                        onRootReady: function(rootList, finderObj) { },
                        onInit: function(finderObj) {
                            closethis = function() {
                                $ektron(".ektronPageBuilderPageLayoutsFolderSelector").fadeOut(400, function() {
                                    $ektron(".ektronTemplateListWrapper").fadeIn(400);
                                    $ektron("#ektronPageBuilderPleaseSelectLayout").show();
                                    $ektron("ul#finder").finder('destroy');
                                });
                            }
                            $ektron('.ui-finder-action-save').click(function() {
                                //get existing settings
                                var url = $ektron("form").attr("action");
                                url = url.split("?")[1];
                                url = url.split("&");
                                var langid, foldid, mod, pagid, taxid;
                                for (var i = 0; i < url.length; i++) {
                                    var pair = url[i].split("=");
                                    pair[0] = pair[0].toLowerCase();
                                    if (pair[0] == "folderid") {
                                        foldid = pair[1];
                                    } else if (pair[0] == "language") {
                                        langid = pair[1];
                                    } else if (pair[0] == "mode") {
                                        mod = pair[1];
                                    } else if (pair[0] == "pageid") {
                                        pagid = pair[1];
                                    } else if (pair[0] == "taxonomyid") {
                                        taxid = pair[1];
                                    }
                                }
                                //get new folderid
                                var selected = $ektron(".ektronPageBuilderPageLayoutsFolderSelector .ui-finder-list-item-activeNow a");
                                if (selected.length == 0 || !selected.parent().hasClass("hasWireframe")) {
                                    return;
                                }
                                foldid = selected.attr("href").split("=")[1];

                                //fade window
                                $ektron(".ektronPageBuilderPageLayouts").fadeOut(2000);

                                if ($ektron("div.ektronMasterPageTitle").length == 0) {
                                    //update iframe to new folder location
                                    parent.Ektron.PageBuilder.Wizards.showAddPage({
                                        mode: mod,
                                        language: langid,
                                        folderId: foldid,
                                        pageid: pagid,
                                        defaulttaxid: taxid,
                                        animateRedirect: true
                                    });
                                } else {
                                   parent.Ektron.PageBuilder.Wizards.showAddMasterPage({
                                        mode: mod,
                                        language: langid,
                                        folderId: foldid,
                                        pageid: pagid,
                                        defaulttaxid: taxid,
                                        animateRedirect: true
                                    });
                                }
                                //closethis();
                            });
                            $ektron('.ui-finder-action-destroy').click(function() {
                                closethis();
                            });
                        },
                        onItemSelect: function(listItem, eventTarget, finderObject) {
                            //alert('onItemSelect');
                            var anchor = $ektron('a', listItem), href = anchor.attr('rel'), id = href.split("=")[1];
                            checkFolder(anchor, id);
                            return false;
                        },
                        onFolderSelect: function(listItem, eventTarget, finderObject) {
                            //alert('onFolderSelect');
                            //var anchor = $ektron('a', listItem), href = anchor.attr('rel');
                        },
                        onItemOpen: function(listItem, newColumn, finderObject) {
                            //alert('onItemOpen');
                            return false;
                        },
                        onFolderOpen: function(listItem, newColumn, finderObject) {
                            //alert('onFolderOpen');
                            var anchor = $ektron('a', listItem), href = anchor.attr('href'), id = href.split("=")[1];
                            checkFolder(anchor, id);
                        },
                        toolbarActions: function() {
                            var toolbar = '<div class="ui-finder-button ui-state-default ui-finder-action-save ui-corner-right" title="Use Selected Folder">';
                            toolbar += '<span class="ui-icon ui-icon-circle-check"/></div>';
                            toolbar += '<div class="ui-finder-button ui-state-default ui-finder-action-destroy ui-corner-left" title="Cancel">';
                            toolbar += '<span class="ui-icon ui-icon-closethick"/></div>';
                            toolbar += '<span class="ui-finder-title" style="float:left;"></span>'
                            return toolbar;
                        }
                    });

                    $ektron(".ektronTemplateListWrapper").fadeOut(400, function() {
                        $ektron("#ektronPageBuilderPleaseSelectLayout").hide();
                        $ektron(".ektronPageBuilderPageLayoutsFolderSelector").fadeIn(400);
                    });
                    parent.Ektron.PageBuilder.Wizards.Status.doneLoading()
                }
            },

            Support:
            {
                //                cleanAlias: function(alias)
                //                {
                //                    var newalias = alias.replace(/%[0-9a-fA-F](?![0-9a-fA-F])/g,''); //remove any half made entities
                //                    newalias = escape(unescape(newalias)).replace(/%20/g, '_');
                //                    return newalias
                //                },
                checkVals: function() {
                    var alias = $ektron("#aliasValue").html();
                    var ext = $ektron("#extValue").html();
                    var selectedext = $ektron("#ExtensionDropdown").val();
                    var opts = $ektron("#ExtensionDropdown > option");

                    var allOK = false;
                    var matchCount = 0;
                    for (var i = 0; i < opts.length; i++) {
                        var opt = opts[i].value;
                        if (opt != "none" && opt.length < alias.length) {
                            if (alias.substring(alias.length - opt.length, alias.length) == opt) {
                                if (selectedext == opt) {
                                    allOK = true;
                                }
                                matchCount++;
                            }
                        }
                    }

                    if (allOK || matchCount == 0) {
                        $ektron(".InvalidAlias").fadeOut();
                    } else {
                        $ektron(".InvalidAlias").fadeIn();
                    }
                }
            }
        }
    };

    // initialize everything
    Ektron.PageBuilder.Wizards.AddPage.init();
});
