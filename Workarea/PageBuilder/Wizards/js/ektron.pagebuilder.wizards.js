Ektron.ready(function() {
    if ("undefined" === typeof Ektron.PageBuilder) {
        Ektron.PageBuilder = {};
    }

    if ("undefined" === typeof Ektron.PageBuilder.Wizards) {
        Ektron.PageBuilder.Wizards =
        {
            // PROPERTIES
            mode: "add",
            language: 1033,
            currentStep: 1,
            finishClicked: false,

            // METHODS
            centerModal: function(modalId) {
                $ektron(modalId).css("margin-top", -1 * Math.round($ektron(modalId).outerHeight() / 2));
            },

            checkAliasName: function(options) {
                var returnValue = false;
                params = {
                    aliasName: "",
                    extension: ".aspx",
                    language: 1033,
                    folderid: 0
                };
                $ektron.extend(params, options);
                $ektron.ajax({
                    url: Ektron.ResourceText.PageBuilder.Wizards.appPath + "urlaliasdialoghandler.ashx?action=checkaliasname&aliasname=" + params.aliasName + "&fileextension=" + params.extension + "&langtype=" + params.language + "&folderid=" + params.folderid,
                    cache: false,
                    async: false,
                    success: function(html) {
                        if (html.indexOf("<aliasname>") != -1) {
                            returnValue = true;
                            return true;
                        }
                        else {
                            returnValue = html;
                            return false;
                        }
                    }
                });
                return returnValue;
            },

            init: function() {
                // add the Markup to the page
                Ektron.PageBuilder.Wizards.Markup.init();

                // initialize Buttons
                Ektron.PageBuilder.Wizards.Buttons.init();

                // initialize Modals
                Ektron.PageBuilder.Wizards.Modals.init();
            },

            parseAliasExtension: function(alias) {
                var result = "/"
                // get everything after the last "/"
                alias = alias.substring(alias.lastIndexOf("\/") + 1);
                // now lop off anything that might be a query string
                if (alias.indexOf("?") !== -1) {
                    alias = alias.substring(0, alias.indexOf("?"));
                }
                // check for a period in what's left
                if (alias.lastIndexOf(".") > -1) {
                    // get everythign after the last period
                    result = alias.substring(alias.lastIndexOf(".") + 1);
                }
                if (result.length === 0) {
                    result = "/";
                }
                return result
            },

            redirectIframe: function(iframeSelector, newUrl) {
                var iframe = $ektron(iframeSelector);
                $ektron(iframeSelector).attr("src", newUrl);
            },

            redirectPage: function() {
                var iframe = $ektron(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe");
                var fullUrlAlias = iframe.contents().find(".redirectMessage input[type='hidden']").val();
                if (location.href.toLowerCase().indexOf(Ektron.ResourceText.PageBuilder.Wizards.appPath.toLowerCase()) != -1) {
                    $ektron(this).parents().filter(".ektronPageBuilderWizard").modalHide();
                    window.open(fullUrlAlias, "_blank");
                    location.href = location.href;
                }
                else {
                    window.location = fullUrlAlias;
                }
                return false;
            },

            showAddMasterPage: function(options) {
                Ektron.PageBuilder.Wizards.currentStep = 1;
                Ektron.PageBuilder.Wizards.finishClicked = false;
                var pageTitle = $ektron(".ektronPageBuilderWizard .ektronModalHeader h3 span.addPageTitle");
                var iframe = $ektron(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe");
                var buttons = $ektron(".ektronPageBuilderWizard .ektronPageBuilderWizardButtons");

                $ektron(".ektronPageBuilderWizard").addClass("MasterLayoutWizard");

                //reset buttons shown
                buttons.find(".button").hide();
                buttons.find(".nextButton").show();
                buttons.find(".cancelButton").show();

                params =
                    {
                        mode: "add",
                        language: "",
                        folderId: "",
                        pageid: "",
                        defaulttaxid: "-1",
                        animateRedirect: false,
                        isChangeFolder: false
                    };
                // if an options object is provided,
                // extend the params with the options
                $ektron.extend(params, options);

                // alter UI based on mode paramter
                Ektron.PageBuilder.Wizards.Status.loading();
                if (params.animateRedirect) {
                    var iframe = $ektron(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe");
                    iframe.fadeTo(500, .01, function() {
                        Ektron.PageBuilder.Wizards.redirectIframe(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe", Ektron.ResourceText.PageBuilder.Wizards.path + "addmasterpage.aspx?folderid=" + params.folderId + "&language=" + params.language + "&LangType=" + params.language + "&mode=" + params.mode + "&pageid=" + params.pageid);
                    });
                } else {
                    Ektron.PageBuilder.Wizards.redirectIframe(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe", Ektron.ResourceText.PageBuilder.Wizards.path + "addmasterpage.aspx?folderid=" + params.folderId + "&language=" + params.language + "&LangType=" + params.language + "&mode=" + params.mode + "&pageid=" + params.pageid);
                }

                Ektron.PageBuilder.Wizards.language = params.language;
                if (params.mode == "add") {
                    Ektron.PageBuilder.Wizards.mode = "add";
                    pageTitle.html(Ektron.ResourceText.PageBuilder.Wizards.addMasterLayout);
                    iframe.css("height", "30.5em");
                }
                else {
                    Ektron.PageBuilder.Wizards.mode = "saveAs";
                    pageTitle.html(Ektron.ResourceText.PageBuilder.Wizards.savePageAs);
                    Ektron.PageBuilder.Wizards.currentStep++;
                }
                // show the modal
                if (!params.isChangeFolder) {
                    Ektron.PageBuilder.Wizards.centerModal(".ektronPageBuilderAddPage");
                    $ektron(".ektronPageBuilderWizard").modalShow();
                }
                return false;
            },

            showAddPage: function(options) {
                Ektron.PageBuilder.Wizards.currentStep = 1;
                Ektron.PageBuilder.Wizards.finishClicked = false;
                var pageTitle = $ektron(".ektronPageBuilderWizard .ektronModalHeader h3 span.addPageTitle");
                var iframe = $ektron(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe");
                var buttons = $ektron(".ektronPageBuilderWizard .ektronPageBuilderWizardButtons");

                $ektron(".ektronPageBuilderWizard").removeClass("MasterLayoutWizard");

                //reset buttons shown
                buttons.find(".button").hide();
                buttons.find(".nextButton").show();
                buttons.find(".cancelButton").show();

                params =
                {
                    mode: "add",
                    language: "",
                    folderId: "",
                    pageid: "",
                    defaulttaxid: "-1",
                    animateRedirect: false,
                    isChangeFolder: false
                };
                // if an options object is provided,
                // extend the params with the options
                $ektron.extend(params, options);

                // alter UI based on mode paramter
                Ektron.PageBuilder.Wizards.Status.loading();
                if (params.animateRedirect) {
                    var iframe = $ektron(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe");
                    iframe.fadeTo(500, .01, function() {
                        Ektron.PageBuilder.Wizards.redirectIframe(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe", Ektron.ResourceText.PageBuilder.Wizards.path + "addpage.aspx?folderid=" + params.folderId + "&language=" + params.language + "&LangType=" + params.language + "&mode=" + params.mode + "&pageid=" + params.pageid + "&taxonomyid=" + params.defaulttaxid);
                    });
                } else {
                    Ektron.PageBuilder.Wizards.redirectIframe(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe", Ektron.ResourceText.PageBuilder.Wizards.path + "addpage.aspx?folderid=" + params.folderId + "&language=" + params.language + "&LangType=" + params.language + "&mode=" + params.mode + "&pageid=" + params.pageid + "&taxonomyid=" + params.defaulttaxid);
                }

                Ektron.PageBuilder.Wizards.language = params.language;
                if (params.mode == "add") {
                    Ektron.PageBuilder.Wizards.mode = "add";
                    pageTitle.html(Ektron.ResourceText.PageBuilder.Wizards.addPage);
                    iframe.css("height", "30.5em");
                }
                else {
                    Ektron.PageBuilder.Wizards.mode = "saveAs";
                    pageTitle.html(Ektron.ResourceText.PageBuilder.Wizards.savePageAs);
                    Ektron.PageBuilder.Wizards.currentStep++;
                }
                if (!params.isChangeFolder) {
                    // show the modal
                    Ektron.PageBuilder.Wizards.centerModal(".ektronPageBuilderAddPage");
                    $ektron(".ektronPageBuilderWizard").modalShow();
                }
                return false;
            },

            stepNext: function(clickedButton) {
                var wizardFrame = $ektron(".ektronPageBuilderWizard");
                if (Ektron.PageBuilder.Wizards.currentStep == 1) {
                    //check if folder selector is out and something new has been selected without being confirmed. if so, ask user if they want to switch
                    var iframe = $ektron(".ektronPageBuilderIframe");
                    var findercontainer = iframe.contents().find("div.ektronPageBuilderPageLayoutsFolderSelector > div");
                    if (findercontainer.length > 0) {
                        //it's open
                        var selections = findercontainer.find(".ui-finder-list-item-active");
                        if (selections.length > 0) {
                            //they've selected something
                            //now get the path to the selected folder, and confirm
                            var selectedfolder = "";
                            var startindex = 0;
                            var selectedel;
                            if (selections.length > 2) {
                                startindex = selections.length - 2;
                                selectedfolder += "...";
                            }
                            for (var i = startindex; i < selections.length; i++) {
                                selectedel = $ektron(selections[i]).children("a");
                                selectedfolder += "/" + selectedel.text();

                            }
                            selectedfolder += "/";
                            //selectedfolder now contains the path. make sure it's a valid choice
                            if ($ektron(selections[selections.length - 1]).hasClass("hasWireframe")) {
                                var response = confirm('You have not confirmed changing folders to ' + selectedfolder + '. Would you like to do so now?');
                                if (response) {
                                    var ismasterwizard = false;
                                    var params =
                                    {
                                        mode: "add",
                                        language: "",
                                        folderId: "",
                                        pageid: "",
                                        defaulttaxid: "-1",
                                        animateRedirect: false,
                                        isChangeFolder: true
                                    };
                                    var url = iframe.attr("src");
                                    if (url.toLowerCase().indexOf("addmasterpage.aspx") > -1) {
                                        ismasterwizard = true;
                                    }
                                    url = url.split("?")[1];
                                    url = url.split("&");
                                    for (var j = 0; j < url.length; j++) {
                                        var pair = url[j].split("=");
                                        pair[0] = pair[0].toLowerCase();
                                        if (pair[0] == "folderid") {
                                            params.folderId = pair[1];
                                        } else if (pair[0] == "language") {
                                            params.language = pair[1];
                                        } else if (pair[0] == "mode") {
                                            params.mode = pair[1];
                                        } else if (pair[0] == "pageid") {
                                            params.pageid = pair[1];
                                        } else if (pair[0] == "taxonomyid") {
                                            params.defaulttaxid = pair[1];
                                        }
                                    }

                                    //get new folderid
                                    params.folderId = selectedel.attr("href").split("=")[1];

                                    if (ismasterwizard) {
                                        //update iframe to new folder location
                                        Ektron.PageBuilder.Wizards.showAddMasterPage(params);
                                    } else {
                                        Ektron.PageBuilder.Wizards.showAddPage(params);
                                    }
                                }
                                return false;
                            } else {
                                alert('The folder you have selected (' + selectedfolder + ') does not have any wireframes associated with it, and cannot be used for pagebuilder content.');
                                return false;
                            }
                        }
                    }
                }
                if (!wizardFrame.hasClass("MasterLayoutWizard")) {
                    var retval = true;
                    Ektron.PageBuilder.Wizards.currentStep++;
                    if (Ektron.PageBuilder.Wizards.currentStep == 1) {
                        retval = Ektron.PageBuilder.Wizards.stepOne(clickedButton);
                    } else if (Ektron.PageBuilder.Wizards.currentStep == 2) {
                        retval = Ektron.PageBuilder.Wizards.stepTwo(clickedButton);
                    } else if (Ektron.PageBuilder.Wizards.currentStep == 3) {
                        retval = Ektron.PageBuilder.Wizards.stepThree(clickedButton);
                    }
                    if (!retval) Ektron.PageBuilder.Wizards.currentStep--;
                    return false;
                } else {
                    var retval = true;
                    Ektron.PageBuilder.Wizards.currentStep++;
                    if (Ektron.PageBuilder.Wizards.currentStep == 1) {
                        retval = Ektron.PageBuilder.Wizards.stepOne(clickedButton);
                    } else if (Ektron.PageBuilder.Wizards.currentStep == 2) {
                        retval = Ektron.PageBuilder.Wizards.stepThree(clickedButton);
                    }
                    if (!retval) Ektron.PageBuilder.Wizards.currentStep--;
                    return false;
                }
                return false;
            },
            stepBack: function(clickedButton) {
                var wizardFrame = $ektron(".ektronPageBuilderWizard");

                if (!wizardFrame.hasClass("MasterLayoutWizard")) {
                    var retval = true;
                    Ektron.PageBuilder.Wizards.currentStep--;
                    if (Ektron.PageBuilder.Wizards.currentStep == 1) {
                        retval = Ektron.PageBuilder.Wizards.stepOne(clickedButton);
                    } else if (Ektron.PageBuilder.Wizards.currentStep == 2) {
                        retval = Ektron.PageBuilder.Wizards.stepTwo(clickedButton);
                    } else if (Ektron.PageBuilder.Wizards.currentStep == 3) {
                        retval = Ektron.PageBuilder.Wizards.stepThree(clickedButton);
                    }
                    if (!retval) Ektron.PageBuilder.Wizards.currentStep++;
                    return false;
                } else {
                    var retval = true;
                    Ektron.PageBuilder.Wizards.currentStep--;
                    if (Ektron.PageBuilder.Wizards.currentStep == 1) {
                        retval = Ektron.PageBuilder.Wizards.stepOne(clickedButton);
                    } else if (Ektron.PageBuilder.Wizards.currentStep == 2) {
                        retval = Ektron.PageBuilder.Wizards.stepThree(clickedButton);
                    }
                    if (!retval) Ektron.PageBuilder.Wizards.currentStep++;
                    return false;
                }
                return false;
            },
            stepOne: function(clickedButton) {
                clickedButton = $ektron(clickedButton);
                var modal = clickedButton.parent().parent().parent().parent();
                var iframe = $ektron(".ektronPageBuilderIframe");
                var messages = modal.find(".messages");
                var finishButton = clickedButton.parent().parent().find(".finishButton");
                var nextButton = clickedButton.parent().parent().find(".nextButton");
                var cancelButton = clickedButton.parent().parent().find(".cancelButton");
                var backButton = clickedButton.parent().parent().find(".backButton");
                var okButton = clickedButton.parent().parent().find(".okButton");
                var step1 = $ektron(iframe).contents().find("#step1");
                var step2 = $ektron(iframe).contents().find("#step2");
                var step3 = $ektron(iframe).contents().find("#step3");
                modal.fadeOut(200);
                window.setTimeout(function() {
                    finishButton.hide();
                    nextButton.show();
                    cancelButton.show();
                    backButton.hide();
                    okButton.hide();
                    messages.empty();
                    step2.hide();
                    step3.hide();
                    if (Ektron.PageBuilder.Wizards.mode == "saveAs") {
                        iframe.css("height", "6em");
                    }
                    step1.find(".messages").empty();
                    step1.css("display", "block");
                    Ektron.PageBuilder.Wizards.centerModal(".ektronPageBuilderAddPage");
                    modal.fadeIn("slow");
                }, 200);
                return true;
            },


            stepTwo: function(clickedButton) {
                if (Ektron.PageBuilder.Wizards.verifyTemplateInfo() === false) {
                    return false;
                }

                clickedButton = $ektron(clickedButton);
                var modal = clickedButton.parent().parent().parent().parent();
                var iframe = $ektron(".ektronPageBuilderIframe");
                var finishButton = clickedButton.parent().parent().find(".finishButton");
                var nextButton = clickedButton.parent().parent().find(".nextButton");
                var cancelButton = clickedButton.parent().parent().find(".cancelButton");
                var backButton = clickedButton.parent().parent().find(".backButton");
                var okButton = clickedButton.parent().parent().find(".okButton");
                var messages = $ektron(".ektronPageBuilderAddPage .messages");
                var step1 = $ektron(iframe).contents().find("#step1");
                var step2 = $ektron(iframe).contents().find("#step2");
                var step3 = $ektron(iframe).contents().find("#step3");
                modal.fadeOut(200);
                window.setTimeout(function() {
                    finishButton.hide();
                    nextButton.show();
                    cancelButton.show();
                    if (Ektron.PageBuilder.Wizards.mode !== "saveAs") {
                        backButton.show();
                    }
                    else {
                        backButton.hide();
                    }
                    okButton.hide();
                    step1.hide();
                    step3.hide();
                    iframe.css("height", "30.5em");
                    messages.empty();
                    step2.css("display", "block");
                    Ektron.PageBuilder.Wizards.centerModal(".ektronPageBuilderAddPage");
                    modal.fadeIn("slow");
                }, 200);
                return true;
            },

            stepThree: function(clickedButton) {
                var wizardFrame = $ektron(".ektronPageBuilderWizard");
                if (!wizardFrame.hasClass("MasterLayoutWizard")) {
                    if (Ektron.PageBuilder.Wizards.verifyTaxonomy() === false) {
                        return false;
                    }
                } else {
                    if (Ektron.PageBuilder.Wizards.verifyTemplateInfo() === false) {
                        return false;
                    }
                }

                //PageLayout Title Validation
                var iframe = $ektron(".ektronPageBuilderIframe");
                var pageTitle = iframe.contents().find(".pageBuilderWizardPageTitle").val();
                var messageContainer = $ektron(".ektronPageBuilderAddPage .messages");
                if (($ektron.trim(pageTitle)).length == 0) {
                    Ektron.PageBuilder.Wizards.updateMessage(
                        messageContainer,
                        Ektron.ResourceText.PageBuilder.Wizards.errorPageTitle
                    );
                    return false;
                }
                if (($ektron.trim(pageTitle)).length > 200) {
                    Ektron.PageBuilder.Wizards.updateMessage(
                        messageContainer,
                        "Title is too long. Please enter a title less than 200 characters."
                    );
                    return false;
                }
                /////
                if (!wizardFrame.hasClass("MasterLayoutWizard")) {
                    var manualaliaschecked = iframe.contents().find(".createManualAlias input:checked");
                    if (manualaliaschecked.length > 0) {
                        var urlAlias = $ektron.trim(iframe.contents().find(".pageBuilderWizardAlias").val());
                        var extension = $ektron.trim(iframe.contents().find(".manualContainer select").val());
                        var folid = $ektron.trim(iframe.contents().find(".manualContainer .Folid input").val());
                        var re = new RegExp(extension + "$", "g");
                        urlAlias = urlAlias.replace(re, ""); //strip extension if necessary

                        if (urlAlias.length == 0) {
                            Ektron.PageBuilder.Wizards.updateMessage(
                            messageContainer, Ektron.ResourceText.PageBuilder.Wizards.errorUrlAlias);
                            return false;
                        } else {
                            // verify the url/alias is valid.
                            // parse out the file extension to use
                            var aliasInUse = Ektron.PageBuilder.Wizards.checkAliasName({
                                aliasName: urlAlias,
                                extension: extension,
                                language: Ektron.PageBuilder.Wizards.language,
                                folderid: folid
                            });
                            if (aliasInUse !== true) {
                                // we've got an error
                                Ektron.PageBuilder.Wizards.updateMessage(messageContainer, aliasInUse);
                                return false;
                            }
                        }
                    }
                }
                ////////////
                clickedButton = $ektron(clickedButton);
                var modal = clickedButton.parent().parent().parent().parent();
                var iframe = $ektron(".ektronPageBuilderIframe");
                var buttonsToShow = clickedButton.parent().parent().find(".backButton, .finishButton");
                var finishButton = clickedButton.parent().parent().find(".finishButton");
                var nextButton = clickedButton.parent().parent().find(".nextButton");
                var cancelButton = clickedButton.parent().parent().find(".cancelButton");
                var backButton = clickedButton.parent().parent().find(".backButton");
                var okButton = clickedButton.parent().parent().find(".okButton");

                var messages = $ektron(".ektronPageBuilderAddPage .messages");
                var step1 = $ektron(iframe).contents().find("#step1");
                var step2 = $ektron(iframe).contents().find("#step2");
                var step3 = $ektron(iframe).contents().find("#step3");
                modal.fadeOut(200);
                window.setTimeout(function() {
                    finishButton.show();
                    backButton.show();
                    nextButton.hide();
                    cancelButton.show();
                    okButton.hide();
                    step1.hide();
                    step2.hide();
                    iframe.css("height", "30.5em");
                    messages.empty();
                    step3.css("display", "block");
                    Ektron.PageBuilder.Wizards.centerModal(".ektronPageBuilderAddPage");
                    modal.fadeIn("slow");
                }, 200);
                return true;
            },

            updateMessage: function(messageContainer, message, messageType) {
                // search the message for any URLs and wrap them.
                message = message.replace(Ektron.RegExp.PageBuilderURL, ' <span class="EkForceWrap">$2</span>');
                message = message.replace(Ektron.RegExp.PageBuilderObjectName, function($0_match) { return $0_match.replace(".", '<span class="EkForceWrap">.</span>'); });
                if (typeof (messageType) == "undefined") {
                    messageType = "error";
                }
                // NOTE: the messageContainer expected is an Ektron Library object
                if (messageContainer.length != 1) {
                    // message DIV to update was not found, or there is more than one
                    // DIV that matches the request
                    return false;
                }
                else {
                    messageContainer.empty();
                    messageContainer.html('<span class="' + messageType + '">' + message + "</span>").fadeIn("slow");
                }
                Ektron.PageBuilder.Wizards.centerModal(".ektronPageBuilderAddPage");
                return true;
            },

            verifyTemplateInfo: function() {
                var result = true;
                var iframe = $ektron(".ektronPageBuilderAddPage .ektronPageBuilderIframe");
                var iframeBodyHeight = iframe.contents().find("body").outerHeight();
                var pageTitle = iframe.contents().find(".pageBuilderWizardPageTitle").val();
                var urlAlias = iframe.contents().find(".pageBuilderWizardAlias");
                var AliasExt = iframe.contents().find("#ExtensionDropdown").val();
                var AliasOpts = iframe.contents().find("#ExtensionDropdown > option");
                var template = iframe.contents().find("#ektronSelectedTemplate").val();
                var messageContainer = $ektron(".ektronPageBuilderAddPage .messages");

                if (Ektron.PageBuilder.Wizards.mode === "add") {
                    // need to verify the template data
                    if (($ektron.trim(template)).length == 0) {
                        // template has not been selected
                        Ektron.PageBuilder.Wizards.updateMessage(messageContainer, Ektron.ResourceText.PageBuilder.Wizards.errorSelectLayout);
                        result = false;
                        return result;
                    }
                }
                return result;
            },

            verifyMetadataInfo: function() {
                var result = true;
                var iframe = $ektron(".ektronPageBuilderAddPage .ektronPageBuilderIframe");
                var messageContainer = $ektron(".ektronPageBuilderAddPage .messages");
                var reqfield = iframe.contents().find("[name='req_fields']");

                if (reqfield.length > 0) {
                    reqfield = reqfield.val();

                    var reqfield = reqfield.split(',');
                    for (var i = 0; i < reqfield.length; i++) {
                        if (reqfield[i] != "") {
                            var value = iframe.contents().find("#" + reqfield[i] + ", input[name='" + reqfield[i] + "']").val();
                            if (value == null || value == "") {
                                result = false;

                                Ektron.PageBuilder.Wizards.updateMessage(
                                    messageContainer,
                                    Ektron.ResourceText.PageBuilder.Wizards.errorMetadata
                                );

                                break;
                            }
                        }
                    }
                }
                return result;
            },

            verifyTaxonomy: function() {
                var result = true;
                var iframe = $ektron(".ektronPageBuilderAddPage .ektronPageBuilderIframe");
                var messageContainer = $ektron(".ektronPageBuilderAddPage .messages");
                var req = iframe.contents().find(".TaxRequiredBool");
                if (req.length > 0) {
                    if (req.html() == "true") {
                        //verify that at least one is checked
                        var checked = iframe.contents().find("div.treecontainer input.categoryCheck:checked");
                        if (checked.length == 0) checked = iframe.contents().find("div.treecontainer input.categoryCheck:checked"); //this is weird but ie doesn't necessarily return the correct set the first time.
                        if (checked.length == 0) {
                            Ektron.PageBuilder.Wizards.updateMessage(
                                messageContainer,
                                Ektron.ResourceText.PageBuilder.Wizards.errorTaxonomy
                            );
                            return false;
                        }
                    }
                }

                return result;
            },

            // CLASSES
            Buttons:
            {
                // PROPERTIES
                initialized: false,

                init: function() {
                    if (Ektron.PageBuilder.Wizards.Buttons.initialized == true) {
                        return;
                    }

                    var wizardButtons = $ektron(".ektronPageBuilderAddPage .ektronPageBuilderWizardButtons");
                    // bind button handlers
                    wizardButtons.find(".nextButton").bind("click", function(e) {
                        return Ektron.PageBuilder.Wizards.stepNext(this);
                    }
                    );

                    wizardButtons.find(".backButton").bind("click", function(e) {
                        return Ektron.PageBuilder.Wizards.stepBack(this);
                    }
                    );

                    wizardButtons.find(".cancelButton").bind("click", function(e) {
                        $ektron(this).parents().filter(".ektronPageBuilderWizard").modalHide();
                        if (location.href.indexOf(Ektron.ResourceText.PageBuilder.Wizards.appPath) != -1) {
                            location.href = location.href;
                        }
                        return false;
                    }
                    );

                    wizardButtons.find(".finishButton").bind("click", function(e) {
                        if (!Ektron.PageBuilder.Wizards.finishClicked) {
                            var iframe = $ektron(".ektronPageBuilderAddPage .ektronPageBuilderIframe");
                            var messageContainer = $ektron(".ektronPageBuilderAddPage .messages");
                            var wizardFrame = $ektron(".ektronPageBuilderWizard");
                            messageContainer.empty();

                            if (wizardFrame.hasClass("MasterLayoutWizard")) {
                                //verify that template is selected and that title is valid
                                var messageContainer = $ektron(".ektronPageBuilderAddPage .messages");

                                var template = iframe.contents().find("#ektronSelectedTemplate").val();
                                if (template == null || template == "") {
                                    Ektron.PageBuilder.Wizards.updateMessage(
                                    messageContainer,
                                    "You must select a wireframe to base this master layout on."
                                );
                                    return false;
                                }
                                var pageTitle = iframe.contents().find(".pageBuilderWizardPageTitle").val();
                                if (($ektron.trim(pageTitle)).length == 0) {
                                    Ektron.PageBuilder.Wizards.updateMessage(
                                    messageContainer,
                                    Ektron.ResourceText.PageBuilder.Wizards.errorPageTitle
                                );
                                    return false;
                                }
                                //check metadata + taxonomy
                                if (Ektron.PageBuilder.Wizards.verifyMetadataInfo() === false) {
                                    return false;
                                }
                                if (Ektron.PageBuilder.Wizards.verifyTaxonomy() === false) {
                                    return false;
                                }
                            } else {
                                //check metadata + taxonomy
                                if (Ektron.PageBuilder.Wizards.verifyMetadataInfo() === false) {
                                    return false;
                                }
                            }
                            var submitButton = iframe.contents().find("#btnFinish");
                            submitButton.click();
                            Ektron.PageBuilder.Wizards.finishClicked = true;
                        }
                        return false;
                    }
                    );

                    wizardButtons.find(".okButton").bind("click", function(e) {
                        return Ektron.PageBuilder.Wizards.redirectPage();
                    }
                    );

                    Ektron.PageBuilder.Wizards.Buttons.initialized = true;
                },

                showPromptButtons: function() {
                    var wizardButtons = $ektron(".ektronPageBuilderWizard .ektronPageBuilderWizardButtons");
                    wizardButtons.find(".button").hide();
                    wizardButtons.find(".cancelButton").show();
                    wizardButtons.find(".okButton").show();
                }
            },

            Markup:
            {
                // PROPERTIES
                initialized: false,

                init: function() {
                    if (Ektron.PageBuilder.Wizards.Markup.initialized == true) {
                        return;
                    }
                    if ($ektron("#PageBuilderAddPageModal").length > 0) {
                        // prevent the markup from being added twice
                        Ektron.PageBuilder.Wizards.Markup.initialized = true;
                        return;
                    }

                    var modal = new String();
                    modal = '<div class="ektronWindow ektronModalStandard ektronPageBuilderWizard ektronPageBuilderAddPage" id="PageBuilderAddPageModal">\n';
                    modal += '  <div class="ektronModalHeader">\n';
                    modal += '    <h3>\n';
                    modal += '      <span class="addPageTitle"></span>\n';
                    modal += '      <a class="ektronModalClose"></a>\n';
                    modal += '    </h3>\n';
                    modal += '  </div>\n';
                    modal += '  <div class="ektronModalBody">\n';
                    modal += '    <div class="messages"></div>\n';
                    modal += '    <iframe noresize="noresize" frameborder="0" border="0"  marginwidth="0" marginheight="0" id="ektronPageBuilderAddPageIframe" class="ektronPageBuilderIframe ektronPageBuilderAddPageIframe" scrolling="auto"></iframe>\n';
                    modal += '    <ul class="ektronModalButtonWrapper ektronPageBuilderWizardButtons clearfix">\n';
                    modal += '      <li><div class="wizardStatus"><p><span></span><b>' + Ektron.ResourceText.PageBuilder.Wizards.loading + '</b></p></div></li>\n';
                    modal += '      <li><a id="ektronPageBuilderFinish" title="' + Ektron.ResourceText.PageBuilder.Wizards.finish + '" class="greenHover button finishButton buttonRight" href="#' + Ektron.ResourceText.PageBuilder.Wizards.finish + '">' + Ektron.ResourceText.PageBuilder.Wizards.finish + '</a></li>\n';
                    modal += '      <li><a id="ektronPageBuilderFinish" title="' + Ektron.ResourceText.PageBuilder.Wizards.ok + '" class="greenHover button okButton buttonRight" href="#' + Ektron.ResourceText.PageBuilder.Wizards.ok + '">' + Ektron.ResourceText.PageBuilder.Wizards.ok + '</a></li>\n';
                    modal += '      <li><a id="ektronPageBuilderNext" title="' + Ektron.ResourceText.PageBuilder.Wizards.next + '" class="blueHover button nextButton buttonRight" href="#' + Ektron.ResourceText.PageBuilder.Wizards.next + '">' + Ektron.ResourceText.PageBuilder.Wizards.next + '</a></li>\n';
                    modal += '      <li><a id="ektronPageBuilderCancel" title="' + Ektron.ResourceText.PageBuilder.Wizards.cancel + '" class="redHover button cancelButton buttonRight" href="#' + Ektron.ResourceText.PageBuilder.Wizards.cancel + '">' + Ektron.ResourceText.PageBuilder.Wizards.cancel + '</a></li>\n';
                    modal += '      <li><a id="ektronPageBuilderBack" title="' + Ektron.ResourceText.PageBuilder.Wizards.back + '" class="blueHover button backButton buttonRight" href="#' + Ektron.ResourceText.PageBuilder.Wizards.back + '">' + Ektron.ResourceText.PageBuilder.Wizards.back + '</a></li>\n';
                    modal += '    </ul>\n';
                    modal += '  </div>\n';
                    modal += '</div>\n';

                    var pageBody = $ektron("body");
                    pageBody.append(modal);

                    Ektron.PageBuilder.Wizards.Markup.initialized;
                }
            },

            Modals:
            {
                // PROPERTIES
                initialized: false,

                init: function() {
                    if (Ektron.PageBuilder.Wizards.Modals.initialized == true) {
                        return;
                    }

                    var addPage = $ektron(".ektronPageBuilderAddPage");
                    var messages = addPage.find(".messages");
                    addPage.modal({
                        modal: true,
                        toTop: true,
                        overlay: 0,
                        onShow: function(hash) {
                            Ektron.PageBuilder.Wizards.centerModal(".ektronPageBuilderAddPage");
                            hash.o.fadeTo("fast", 0.5, function() {
                                hash.w.fadeIn("fast");
                            });
                        },
                        onHide: function(hash) {
                            messages.empty();
                            hash.w.fadeOut("fast");
                            hash.o.fadeOut("fast", function() {
                                if (hash.o) {
                                    hash.o.remove();
                                }
                            });
                        }
                    });

                    Ektron.PageBuilder.Wizards.Modals.initialized;
                }
            },

            Status:
            {
                loading: function() {
                    $ektron("div.wizardStatus").fadeIn(1000);
                },
                doneLoading: function() {
                    $ektron(".ektronPageBuilderWizard iframe.ektronPageBuilderAddPageIframe").fadeTo(500, 1)
                    $ektron("div.wizardStatus").fadeOut(1000);
                }
            }
        };
    }

    // initialize the Wizards
    Ektron.PageBuilder.Wizards.init();
    // Define Reular Expressions for later use
    Ektron.RegExp.PageBuilderURL = new RegExp("(^|[ \t\r\n])((ftp|http|https|file)://(([A-Za-z0-9$_.+!*(),;/?:@&~=-])|%[A-Fa-f0-9]{2}){2,}(#([a-zA-Z0-9][a-zA-Z0-9$_.+!*(),;/?:@&~=%-]*))?(([A-Za-z0-9$_+!*();/?:~-])|%[A-Fa-f0-9]{2}))", "gi");
    Ektron.RegExp.PageBuilderObjectName = new RegExp("\\w+(\\.\\w+)+", "g");
});
