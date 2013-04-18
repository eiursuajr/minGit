// Class CommunitySearchClass: 
function CommunitySearchClass (id) {
    ///////////////////////////////////////////////////////
	// Instance members - variables:
    this._id = id,
	this._ajaxCall = null,
	this._ajaxCallbackId = "",
	this._readyFlag = false,
    this._userCategoryXml = "",
    this._groupCategoryXml = "",
    this._userCategories = "",
    this._groupCategories = "",
    this._resultFilterCategories = "",
    this._resultFilterCategoriesOverride = "",
    this._userCategoryXmlTreeInitialized = false,
    this._groupcategoryXmlTreeInitialized = false,
    this._resultFilterCategoryXmlTreeInitialized = false,
    this._returnFriendsOnly = false,
    this._categoryRootPath = "",
    this._enableMap = true,
    this._browserInitDelay = 100, // Time in MS to allow browser to stabilize, before attempting to modify DOM/UI.
    this._retryDelay = 50, // Time in MS to wait before retrying action.
	this._filterBlockCnt = 0,
	this._basicSearchMode = true,
	this._enableUserTaxonomy = true,
	this._enableGroupTaxonomy = true,
    //this._user_cat_tree = null,
    //this._resultFilter_cat_tree = null,
	this._ajaxImage = "",
	this._imagePath = "",
	this._startingTab = "basic",
	this._currentTab = "",
    // The following messages are set to defaults here, but are set to proper local-value from server during initialization:
	this._msgEmptySearch = "Nothing to search for!",
	this._msgTags = "Tags",
	this._msgGroupName = "Group Name",
	this._msgDescription = "Description",
	this._msgGroupTags = "Group Tags",
	this._msgDisplayName = "Display Name",
	this._msgFirstName = "First Name",
	this._msgLastName = "Last Name",
	this._msgEmail = "Email",
	this._msgRemove = "Remove",
	this._msgTaxonomy = "Category",
    this._msgSelectTaxonomy = "Select Category",
	this._msgUserCustomProperty = "User Properties",
	this._resultFilterSearchMode = false,
	this._resultFilterHasData = false,
	this._lastInfoObjId = null,
	this._savedArgs = "",

    ///////////////////////////////////////////////////////
    // Instance members - methods:


    ///////////////////////////////////
    // Initialize: Called when this object is first created.
    this.Constructor = function () {
        // Initialize state here:
        // ...
    },

    ///////////////////////////////////
    // Initialize: Called externally, after startup settings have been applied (via server generated client script).
    this.Initialize = function () {
        // Callback delayed-init, to allow browser time to generate the DOM/UI:
        setTimeout('CommunitySearchClass.GetObject("' + this.GetId() + '").DelayedInitialize()', this._browserInitDelay);
    },

    ///////////////////////////////////
    // DelayedInitialize: Called by Initialize(), after a delay 
    // (allows time for the browser to stabilize).
    this.DelayedInitialize = function () {
        this.InitializeUI();
    },

    ///////////////////////////////////
    // GetId: Returns the client-id associated with the server control.
    this.GetId = function () {
        return (this._id);
    },

    ///////////////////////////////////
    // 
    this.EnableUserTaxonomy = function (enableFlag) {
        this._enableUserTaxonomy = enableFlag;
    },

    ///////////////////////////////////
    // 
    this.EnableGroupTaxonomy = function (enableFlag) {
        this._enableGroupTaxonomy = enableFlag;
    },

    ///////////////////////////////////
    // 
    this.EnableGroupResults = function (enableFlag) {
        this._enableGroupResults = enableFlag;
    },

    ///////////////////////////////////
    // 
    this.EnableUserResults = function (enableFlag) {
        this._enableUserResults = enableFlag;
    },

    ///////////////////////////////////
    // 
    this.SetReady = function (flag) {
        this._readyFlag = flag;
    },

    ///////////////////////////////////
    // 
    this.GetReady = function () {
        return (this._readyFlag);
    },

    ///////////////////////////////////
    // 
    this.SetAjaxCall = function (ptr) {
        this._ajaxCall = ptr;
    },

    ///////////////////////////////////
    //     
    this.GetAjaxCall = function () {
        return (this._ajaxCall);
    },

    ///////////////////////////////////
    // 
    this.SetAjaxCallbackId = function (id) {
        this._ajaxCallbackId = id;
    },

    ///////////////////////////////////
    //     
    this.GetAjaxCallbackId = function () {
        return (this._ajaxCallbackId);
    },

    ///////////////////////////////////
    // 
    this.SetAjaxImage = function (src) {
        this._ajaxImage = src;
    },

    ///////////////////////////////////
    // 
    this.SetImagePath = function (path) {
        this._imagePath = path;
    },

    ///////////////////////////////////
    // 
    this.SetStartingTab = function (startTab) {
        this._startingTab = startTab;
    },

    ///////////////////////////////////
    // 
    this.SetEmptySearchMsg = function (msg) {
        this._msgEmptySearch = msg;
    },

    ///////////////////////////////////
    // 
    this.SetTagsMsg = function (msg) {
        this._msgTags = msg;
    },

    ///////////////////////////////////
    // 
    this.SetGroupNameMsg = function (msg) {
        this._msgGroupName = msg;
    },

    ///////////////////////////////////
    // 
    this.SetDescriptionMsg = function (msg) {
        this._msgDescription = msg;
    },

    ///////////////////////////////////
    // 
    this.SetGroupTagsMsg = function (msg) {
        this._msgGroupTags = msg;
    },

    ///////////////////////////////////
    // 
    this.SetDisplayNameMsg = function (msg) {
        this._msgDisplayName = msg;
    },

    ///////////////////////////////////
    // 
    this.SetFirstNameMsg = function (msg) {
        this._msgFirstName = msg;
    },

    ///////////////////////////////////
    // 
    this.SetLastNameMsg = function (msg) {
        this._msgLastName = msg;
    },

    ///////////////////////////////////
    // 
    this.SetEmailMsg = function (msg) {
        this._msgEmail = msg;
    },

    ///////////////////////////////////
    // 
    this.UserCustomPropertyMsg = function (msg) {
        this._msgUserCustomProperty = msg;
    },

    ///////////////////////////////////
    // 
    this.SetRemoveMsg = function (msg) {
        this._msgRemove = msg;
    },

    ///////////////////////////////////
    // 
    this.SetTaxonomyMsg = function (msg) {
        this._msgTaxonomy = msg;
    },

    ///////////////////////////////////
    // 
    this.SetSelectTaxonomyMsg = function (msg) {
        this._msgSelectTaxonomy = msg;
    },

    ///////////////////////////////////
    // 
    this.SetUserCategoryXML = function (xml) {
        this._userCategoryXml = xml;
    },

    ///////////////////////////////////
    // 
    this.SetUserCategoryPath = function (path) {
        this._userCategoryRootPath = path;
    },

    ///////////////////////////////////
    // 
    this.SetGroupCategoryXML = function (xml) {
        this._groupCategoryXml = xml;
    },

    ///////////////////////////////////
    // 
    this.SetGroupCategoryPath = function (path) {
        this._groupCategoryRootPath = path;
    },

    ///////////////////////////////////
    // 
    this.EnableMap = function (path) {
        this._enableMap = path;
    },

    ///////////////////////////////////
    // 
    this.SetFilterCategories = function (filterCategories) {
        this._resultFilterCategoriesOverride = filterCategories;
    },

    this.IsNumeric = function (strValue) {
        var objRegExp = /(^-?\d\d*\.\d*$)|(^-?\d\d*$)|(^-?\.\d\d*$)/;
        //check for numeric characters
        return objRegExp.test(strValue);
    },
    ///////////////////////////////////
    // 
	this.DoAjaxSearch = function (args, context, page) {
	    if ((args.length > 0) || (this._userCategories.length > 0) || (this._groupCategories.length > 0)) {
	        args += "&__targpage=" + page;
	        if (this.AdvancedTabSelected()) {
	            args += "&__usercat=" + this._userCategories;
	            args += "&__groupcat=" + this._groupCategories;
	        }
	    }
	    else {
	        args = "&__searchmode=advanced&__usersearchtext=0&__usersearchtype=all_users";
	        //TODO: BCB; this isn't working: args += "&__groupsearchtext=0&__groupsearchtype=all_groups";
	        args += "&__targpage=" + page;
	    }

	    var friendsOnlyButton = document.getElementById('CommunitySearch_Advanced_FriendsOnlyButton_' + this.GetId())
	    if (friendsOnlyButton && friendsOnlyButton.checked) {
	        args += "&__friends=true";
	    }
	    else {
	        args += "&__friends=false";
	    }

	    if (this._resultFilterCategoriesOverride.length > 0) {
	        args += "&__resfiltcats=" + this._resultFilterCategoriesOverride;
	    } else {
	        args += "&__resfiltcats=" + this._resultFilterCategories;
	    }

	    var targObj = null;

	    // Show Ajax-loading image if specified:
	    if (this._ajaxImage.length > 0) {
	        // determine if this is a search or a status update-request:
	        var statusUpdateParm = '__addfid';
	        var idx = args.indexOf(statusUpdateParm);
	        if (idx < 0) {
	            statusUpdateParm = '__joingid';
	            idx = args.indexOf(statusUpdateParm);
	        }
	        if (idx >= 0) {
	            var statusObjId = 'EkUSR_Status_' + this.GetId() + '_' + context;
	            targObj = document.getElementById(statusObjId);
	            if (targObj) {
	                targObj.innerHTML = '<span class="CommunitySearch_ResultLoading" >  <img src="' + this._ajaxImage + '" />  </span> ';
	            }
	        }
	        else {
	            this.ShowSearching();
	        }
	    }

	    // Do Ajax-Callback to server:
	    var fPtr = this.GetAjaxCall();
	    if (("undefined" != typeof fPtr) && (null != fPtr)) {
	        // Use MS Ajax client side code:
	        (fPtr)(this.GetId(), args, this.GetId().toString());
	    }
	    else {
	        // Do not use MS Ajax client side code:
	        var ektronPostParams = "";
	        ektronPostParams += "&__CALLBACKID=" + this.GetAjaxCallbackId(); // Must use ASP.NET UniqueID as that's what's expected on server side when control is used with master pages.
	        ektronPostParams += "&__CALLBACKPARAM=" + escape(args);
	        ektronPostParams += "&__VIEWSTATE="; // + encodeURIComponent($ektron("#__VIEWSTATE").attr("value"));
	        var context = this.GetId().toString();
	        var ajaxUrl = String(window.location).replace("#", ""); // IE/Windows will block callbacks with a pound/hash symbol in the URL.
	        $ektron.ajax({
	            type: "POST",
	            url: ajaxUrl,
	            data: ektronPostParams,
	            dataType: "html",
	            success: function (ektronCallbackResult) {
	                var removeZeroPipe = String(ektronCallbackResult).replace("0|", "");
	                CommunitySearchClass.GetObject(context).AjaxCallback_DisplayResult(removeZeroPipe, context);
	            }
	        });
	    }

	    return (false); // cancel events.
	},

    ///////////////////////////////////
    // 
	this.ShowCommunitySearchMap = function (showFlag) {
	    var divObj = document.getElementById("CommunitySearchDivInputs" + this.GetId());
	    if (divObj) {
	        divObj.style.display = (showFlag) ? 'none' : '';
	    }
	    divObj = document.getElementById("CommunitySearchDivResults" + this.GetId());
	    if (divObj) {
	        divObj.style.display = (showFlag) ? 'none' : '';
	    }
	    divObj = document.getElementById("CommunitySearchDivMap" + this.GetId());
	    if (divObj) {
	        divObj.style.display = (showFlag && this._enableMap) ? '' : 'none';
	    }
	},

    ///////////////////////////////////
    // 
	this.ShowInfo = function (infoId, eventObj) {
	    var infoObj = document.getElementById(infoId);
	    if (this._lastInfoObjId && (this._lastInfoObjId != infoId)) {
	        this.DelayedHideInfo(this._lastInfoObjId);
	    }
	    if (infoObj) {
	        infoObj.style.display = '';
	        this._lastInfoObjId = infoId;
	    }
	},

    ///////////////////////////////////
    // 
	this.DelayedHideInfo = function (infoId, eventObj) {
	    var infoObj = document.getElementById(infoId);
	    if (infoObj) {
	        this.HideInfo(infoObj);
	    }
	},

    ///////////////////////////////////
    // 
	this.HideInfo = function (infoObj) {
	    infoObj.style.display = 'none';
	},

    ///////////////////////////////////
    // 
	this.CheckKey = function (eventObj) {
	    if (eventObj && (13 == eventObj.keyCode)) {
	        if (this._basicSearchMode) {
	            this.DoBasicSearch();
	        }
	        else {
	            this.DoAdvancedSearch();
	        }
	        return (false);
	    }
	    return (true);
	},

    ///////////////////////////////////
    // 
	this.SearchTag = function (tag, isGroup) {
	    this.SelectTab("advanced");
	    // Remove all filters, and replace with empty tag-only filters:
	    this.ReBuildFilters(false, "searchtag");
	    this.ReBuildFilters(true, "searchgrptag");

	    var container = this.GetFilterContainer(isGroup);
	    if (container) {
	        var valueNodes = container.getElementsByTagName("input");
	        if (valueNodes) {
	            valueNodes[0].value = tag;
	            this.TriggerSearch('', '', '1');
	        }
	    }
	    return (false);
	},


    ///////////////////////////////////
    ///////////////////////////////////
    // 
	this.SearchDisplayName = function (displayName, isGroup) {
	    this.SelectTab("advanced");

	    // Remove all filters:	    
	    this.ReBuildFilters(false, "searchdisplayname");
	    this.ReBuildFilters(true, "searchgroupname");
	    var container = this.GetFilterContainer(isGroup);
	    if (container) {
	        var valueNodes = container.getElementsByTagName("input");
	        if (valueNodes) {
	            valueNodes[0].value = displayName;
	            this.TriggerSearch('', '', '1');
	        }
	    }
	    return (false);
	},

    ///////////////////////////////////
    // 
	this.SearchGroupDescription = function (description, isGroup) {
	    this.SelectTab("advanced");

	    // Remove all filters:	    
	    this.ReBuildFilters(false, "");
	    this.ReBuildFilters(true, "searchgrpdescription");
	    var container = this.GetFilterContainer(isGroup);
	    if (container) {
	        var valueNodes = container.getElementsByTagName("input");
	        if (valueNodes) {
	            valueNodes[0].value = description;
	            this.TriggerSearch('', '', '1');
	        }
	    }
	    return (false);
	},
    ///////////////////////////////////
    // 
	this.SearchCategory = function (category, isGroup)	
	{
	    this.SelectTab("advanced");

	    // Remove all filters:	    
	    this.ReBuildFilters(false, "searchusercategory");
	    this.ReBuildFilters(true, "searchgroupcategory");
	    var container = this.GetFilterContainer(isGroup);
        if (container)
        {
	        if (isGroup === false) filterType = "user";
	        else filterType = "communitygroup";
	        CommunitySearchClass.GetObject(id).ShowCategories(true);
	        CommunitySearchClass.GetObject(id).CheckUserGroupTaxonomy(filterType, category);
	    }
	    return (false);
	},

    this.SearchFirstName = function (firstName, isGroup) {
        this.SelectTab("advanced");

        // Remove all filters:
        this.ReBuildFilters(false, "searchfirstname");
        this.ReBuildFilters(true, "");

        var container = this.GetFilterContainer(isGroup);
        if (container) {
            var valueNodes = container.getElementsByTagName("input");
            if (valueNodes) {
                valueNodes[0].value = firstName;
                this.TriggerSearch('', '', '1');
            }
        }
        return (false);
    },

    this.SearchLastName = function (lastName, isGroup) {
        this.SelectTab("advanced");

        // Remove all filters:	    
        this.ReBuildFilters(false, "searchlastname");
        this.ReBuildFilters(true, "");

        var container = this.GetFilterContainer(isGroup);
        if (container) {
            var valueNodes = container.getElementsByTagName("input");
            if (valueNodes) {
                valueNodes[0].value = lastName;
                this.TriggerSearch('', '', '1');
            }
        }
        return (false);
    },

    this.SearchEmail = function (email, isGroup) {
        this.SelectTab("advanced");

        // Remove all filters:

        this.ReBuildFilters(false, "searchemail");
        this.ReBuildFilters(true, "");

        var container = this.GetFilterContainer(isGroup);
        if (container) {
            var valueNodes = container.getElementsByTagName("input");
            if (valueNodes) {
                valueNodes[0].value = email;
                this.TriggerSearch('', '', '1');
            }
        }
        return (false);
    },
    this.SearchUserProperty = function (property, isGroup) {
        this.SelectTab("advanced");

        // Remove all filters:

        this.ReBuildFilters(false, "searchuserproperties");
        this.ReBuildFilters(true, "");

        var container = this.GetFilterContainer(isGroup);
        if (container) {
            var valueNodes = container.getElementsByTagName("input");
            if (valueNodes) {
                valueNodes[0].value = property;
                this.TriggerSearch('', '', '1');
            }
        }
        return (false);
    },

    ///////////////////////////////////
    // 
    this.AjaxCallback_DisplayResult = function (args, context) {
        var bgnStr = '[targ_info_begin]';
        var endStr = '[targ_info_end]';
        var idxBegin = args.indexOf(bgnStr);
        var idxEnd = args.indexOf(endStr);
        var comObj = null;
        // have to get the community-search object, as in this function 'this' is in the context of the XHTML request object!
        if (context && (0 < context.length)) {
            comObj = CommunitySearchClass.GetObject(context);
        }
        if (comObj) {
            if ((idxBegin >= 0) && (idxEnd > idxBegin)) {
                // this is a subsection (status) update callback, update just that part:
                var targId = args.substring(idxBegin + bgnStr.length, idxEnd);
                var targObj = document.getElementById(targId);
                if (targObj) {
                    var targData = args.substring(idxEnd + endStr.length);
                    targObj.innerHTML = targData;
                }
            }
            else {
                // this is a complete search result callback:
                // attempt to extract result-filtering xml, if it exists:
                bgnStr = '<wrapper_resultFilteringCategoryXml>';
                endStr = '</wrapper_resultFilteringCategoryXml>';
                idxBegin = args.indexOf(bgnStr);
                idxEnd = args.indexOf(endStr);
                if ((idxBegin >= 0) && (idxEnd > idxBegin)) {
                    var xmlData = args.substring(idxBegin + bgnStr.length, idxEnd);
                    if (!comObj._resultFilterSearchMode) {
                        if (comObj._resultFilterCategoryXmlTreeInitialized) {
                            //comObj._resultFilter_cat_tree.destructor();
                            //delete comObj._resultFilter_cat_tree;
                            //comObj._resultFilter_cat_tree = null;
                            comObj._resultFilterCategoryXmlTreeInitialized = false;
                            var targEl = document.getElementById('CommunitySearch_ResultFilterCategoryTreePane' + comObj.GetId().toString());
                            if (targEl) {
                                targEl.innerHTML = "";
                            }
                        }

                        xmlData = xmlData.replace("<resultFilteringCategoryXml>", "");
                        xmlData = xmlData.replace("</resultFilteringCategoryXml>", "");

                        if (xmlData.length > 0) {
                            comObj._resultFilterHasData = true;
                            comObj._resultFilterCategoryXmlTreeInitialized = true;
                            xmlData = xmlData.replace(/&amp;/gi, "&");

                            var divName = "CommunitySearch_ResultFilterCategoryTreePane" + comObj.GetId().toString();
                            $ektron("div#" + divName).html(xmlData);
                            $ektron("div#" + divName + " ul.taxonomyFilter").treeview({
                                collapsed: true
                            });

                            /*
                            comObj._resultFilter_cat_tree = new dhtmlXTreeObject('CommunitySearch_ResultFilterCategoryTreePane' + comObj.GetId().toString(), '100%', '100 % ', 0);
                            comObj._resultFilter_cat_tree.objectId = comObj.GetId().toString();
                            comObj._resultFilter_cat_tree.action = "filter_category";
                            comObj._resultFilter_cat_tree.isGroup = false;
                            comObj._resultFilter_cat_tree.setImagePath(comObj._imagePath + 'maps/tree/');
                            comObj._resultFilter_cat_tree.enableCheckBoxes(true);
                            comObj._resultFilter_cat_tree.enableTreeLines(false);
                            comObj._resultFilter_cat_tree.closeAllItems(0);
                            comObj._resultFilter_cat_tree.setOnCheckHandler(CommunitySearchClass.DoAction);
                            comObj._resultFilter_cat_tree.loadXMLString(xmlData);
                            comObj._resultFilter_cat_tree._unselectItems();
                            */
                            comObj.ShowFilterCategoriesButton(true);
                        }
                        else {
                            comObj.ShowFilterCategories(false);
                            comObj.ShowFilterCategoriesButton(false);
                            comObj._resultFilterHasData = false;
                        }
                    }

                    // remove xml payload:
                    args = args.substring(0, idxBegin) + args.substring(idxEnd + endStr.length);
                }
                else {
                    if ((-1 < args.indexOf("CommunitySearch_ResultNoResults")) && !comObj._resultFilterSearchMode) {
                        comObj.ShowFilterCategories(false);
                        comObj.ShowFilterCategoriesButton(false);
                    }
                }

                // finally, show the search results:
                comObj.ShowResults(args);
            }
        }
    }

	///////////////////////////////////
    // 
    this.AjaxCallback_DisplayError = function (){
        CommunitySearchClass.LogIt('Error Occurred in CommunitySearchClass: Ajax-Error!');
    }

    ///////////////////////////////////
    //
    this.ShowCategories = function (isGroup) {
        this.HideCategories(!isGroup);
        if (isGroup) {
            if (this._groupCategoryXml != '') {
                var catDivObj = document.getElementById('CommunitySearch_GroupCategoryContainer' + this.GetId());
                if (catDivObj) {
                    catDivObj.style.display = 'block';
                }
                if (!this._groupCategoryXmlTreeInitialized) {
                    this._groupCategoryXmlTreeInitialized = true;
                    $ektron("div.CommunitySearch_GroupCategoryTreePane").html(this._groupCategoryXml);
                    $ektron("div.CommunitySearch_GroupCategoryTreePane ul.taxonomyFilter").treeview({
                        collapsed: true
                    });
                    /*
                    this._group_cat_tree = new dhtmlXTreeObject('CommunitySearch_GroupCategoryTreePane', '100%', '100 % ', 0);
                    this._group_cat_tree.objectId = this.GetId().toString();
                    this._group_cat_tree.action = "show_category";
                    this._group_cat_tree.isGroup = isGroup;
                    this._group_cat_tree.setImagePath(this._imagePath + 'maps/tree/');
                    this._group_cat_tree.enableCheckBoxes(true);
                    this._group_cat_tree.enableTreeLines(false);
                    this._group_cat_tree.loadXMLString(this._groupCategoryXml);
                    this._group_cat_tree.closeAllItems(0);
                    this._group_cat_tree.setOnCheckHandler(CommunitySearchClass.DoAction);
                    this._group_cat_tree._unselectItems();
                    */
                }
            }
        }
        else {
            if (this._userCategoryXml != '') {
                var catDivObj = document.getElementById('CommunitySearch_UserCategoryContainer' + this.GetId());
                if (catDivObj) {
                    catDivObj.style.display = 'block';
                }
                if (!this._userCategoryXmlTreeInitialized) {
                    this._userCategoryXmlTreeInitialized = true;
                    $ektron("div.CommunitySearch_UserCategoryTreePane").html(this._userCategoryXml);
                    $ektron("div.CommunitySearch_UserCategoryTreePane ul.taxonomyFilter").treeview({
                        collapsed: true
                    });

                    /*
                    this._user_cat_tree = new dhtmlXTreeObject('CommunitySearch_UserCategoryTreePane', '100%', '100 % ', 0);
                    this._user_cat_tree.objectId = this.GetId().toString();
                    this._user_cat_tree.action = "show_category";
                    this._user_cat_tree.isGroup = isGroup;
                    this._user_cat_tree.setImagePath(this._imagePath + 'maps/tree/');
                    this._user_cat_tree.enableCheckBoxes(true);
                    this._user_cat_tree.enableTreeLines(false);
                    this._user_cat_tree.loadXMLString(this._userCategoryXml);
                    this._user_cat_tree.closeAllItems(0);
                    this._user_cat_tree.setOnCheckHandler(CommunitySearchClass.DoAction);
                    this._user_cat_tree._unselectItems();
                    */
                }
            }
        }
    },

    ///////////////////////////////////
    // 
	this.HideCategories = function (isGroup) {
	    var catContainerObj = null;
	    if (isGroup) {
	        catContainerObj = document.getElementById('CommunitySearch_GroupCategoryContainer' + this.GetId());
	    }
	    else {
	        catContainerObj = document.getElementById('CommunitySearch_UserCategoryContainer' + this.GetId());
	    }
	    if (catContainerObj) {
	        catContainerObj.style.display = 'none';
	    }
	},

    ///////////////////////////////////
    // 
	this.TriggerSearch = function (args, context, page) {
	    if (this._basicSearchMode) {
	        this.DoBasicSearch_Parametric(args, context, page);
	    }
	    else if (this._currentTab == "directory") {
	        if (this._savedArgs.length > 0) {
	            args = this._savedArgs;
	        }
	        this.DoAjaxSearch(args, "", page);
	    }
	    else {
	        this.DoAdvancedSearch_Parametric(args, context, page);
	    }
	    return (false);
	},

    ///////////////////////////////////
    // 
	this.OnNodeSelect = function (catId, checkedFlag, isGroup) {
	    if (isGroup) {
	        this._groupCategories = this.GetPaths(isGroup);
	    }
	    else {
	        this._userCategories = this.GetPaths(isGroup);
	    }
	    this.TriggerSearch('', '', '1');
	},


    ///////////////////////////////////
    // 
	this.ToggleFriends = function (divObj) {
	    this._returnFriendsOnly = !this._returnFriendsOnly;
	    if (this._returnFriendsOnly) {
	        divObj.innerHTML = '<img align="absmiddle" src="' + this._imagePath + 'maps/tree/iconCheckAll.gif" style = "width: 16px; height: 16px;" />  &#160; Friends Only ';
	    }
	    else {
	        divObj.innerHTML = '<img align="absmiddle" src="' + this._imagePath + 'maps/tree/iconUnCheckAll.gif" style = "width: 16px; height: 16px;" />  &#160; Friends Only ';
	    }
	    this.TriggerSearch('', '', '1');
	},

    ///////////////////////////////////
    // 
	this.GetPaths = function (isGroup) {
	    var categoryText = '';
	    var selectedids = '';
	    var selectedidlist;
	    if (isGroup) {
	        if ($ektron("div#CommunitySearch_GroupCategoryTreePane #taxonomyFilter").length > 0) {
	            var taxnomyvalues = $ektron("div#CommunitySearch_GroupCategoryTreePane input.searchTaxonomyPath");
                for(var count=0; count<taxnomyvalues.length; count++) 
                {
                    if (taxnomyvalues[count].checked === true)
                    {
	                    if (categoryText.length <= 0)
	                        categoryText = taxnomyvalues[count].value;
	                    else
	                        categoryText += "," + taxnomyvalues[count].value;
	                }
	            }
	        }
	    }
	    else {
	        if ($ektron("div#CommunitySearch_UserCategoryTreePane #taxonomyFilter").length > 0) {
	            var taxnomyvalues = $ektron("div#CommunitySearch_UserCategoryTreePane input.searchTaxonomyPath");
                for(var count=0; count<taxnomyvalues.length; count++) 
                {
                    if (taxnomyvalues[count].checked === true)
                    {
	                    if (categoryText.length <= 0)
	                        categoryText = taxnomyvalues[count].value;
	                    else
	                        categoryText += "," + taxnomyvalues[count].value;
	                }
	            }
	        }
	    }
	    categoryText = categoryText.replace(/&/gi, "#eksepamp#");
	    return (categoryText);
	},

    ///////////////////////////////////
    // 
	this.GetObjTreePath = function (id, catObj) {
	    var path = '';
	    var parentid = id;
	    if (('undefined' != typeof catObj) && (catObj != null)) {
	        path = '\\' + catObj.getItemText(id);
	        while (parentid != 0) {
	            parentid = catObj.getParentId(parentid);
	            path = '\\' + catObj.getItemText(parentid) + path;
	        }
	    }
	    path = this.Replace(path, '\\\\', '\\');
	    return (path);
	},

    ///////////////////////////////////
    // 
	this.Replace = function (str, findstr, replacestr) {
	    var ret = str;
	    if (ret != '') {
	        var index = ret.indexOf(findstr);
	        while (index >= 0) {
	            ret = ret.replace(findstr, replacestr);
	            index = ret.indexOf(findstr);
	        }
	    }
	    return (ret);
	},

    ///////////////////////////////////
    // 
    this.isTabSelected = function (tabToBeSelected,tab)
    {
        //This method matches tabToBeSelected and tab//returns bool//

        if (tabToBeSelected === tab) return (true);
        return (false);
    },

    ///////////
	this.AddFilter = function (group, suppressRemoveBtn, tabToBeSelected) {

	    var result = false;
	    var container = this.GetFilterContainer(group);
	    if (container) {
	        ++this._filterBlockCnt;

	        var filterBlock = document.createElement("div");

	        if (group)
	            filterBlock.id = "CommunitySearch_groupFilter_" + this._filterBlockCnt.toString();
	        else
	            filterBlock.id = "CommunitySearch_userFilter_" + this._filterBlockCnt.toString();

	        filterBlock.setAttribute("class", "CommunitySearch_FilterBlock");
	        filterBlock.setAttribute("className", "CommunitySearch_FilterBlock");
	        filterBlock.isGroup = group;

	        var spanNode = document.createElement("span");
	        spanNode.setAttribute("class", "CommunitySearch_FilterModeContainer");
	        spanNode.setAttribute("className", "CommunitySearch_FilterModeContainer");

	        var selNode = document.createElement("select");
	        var optionNode = document.createElement("option");

	        if (group) {
	            optionNode.value = "group_name";
	            optionNode.appendChild(document.createTextNode(this._msgGroupName));

                if(this.isTabSelected(tabToBeSelected,'searchgroupname'))
                {
	                optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);

	            optionNode = document.createElement("option");
	            optionNode.value = "description";
	            optionNode.appendChild(document.createTextNode(this._msgDescription));
                 if(this.isTabSelected(tabToBeSelected,'searchgrpdescription'))
                {
	                optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);

	            optionNode = document.createElement("option");
	            optionNode.value = "group_tags";
	            optionNode.appendChild(document.createTextNode(this._msgGroupTags));

               if(this.isTabSelected(tabToBeSelected,'searchgrptag'))
                {
	                optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);



	            if (this._groupCategoryXml != '') {
	                optionNode = document.createElement("option");
	                optionNode.value = "taxonomy";
	                optionNode.appendChild(document.createTextNode(this._msgTaxonomy));
                    if(this.isTabSelected(tabToBeSelected,'searchgroupcategory'))
                    {
	                    optionNode.setAttribute("selected", "selected");
	                }
	                selNode.appendChild(optionNode);
	            }
	        }
	        else {
	            optionNode.value = "display_name";
	            optionNode.appendChild(document.createTextNode(this._msgDisplayName));


               if(this.isTabSelected(tabToBeSelected,'searchdisplayname'))
                {
	                optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);


	            optionNode = document.createElement("option");
	            optionNode.value = "first_name";
	            optionNode.appendChild(document.createTextNode(this._msgFirstName));


                 if(this.isTabSelected(tabToBeSelected,'searchfirstname'))
                {
	                optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);


	            optionNode = document.createElement("option");
	            optionNode.value = "last_name";
	            optionNode.appendChild(document.createTextNode(this._msgLastName));

                 if(this.isTabSelected(tabToBeSelected,'searchlastname'))
                {
	                optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);


	            optionNode = document.createElement("option");
	            optionNode.value = "personal_tags";
	            optionNode.appendChild(document.createTextNode(this._msgTags));

               if(this.isTabSelected(tabToBeSelected,'searchtag'))
                {
	                optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);

	            optionNode = document.createElement("option");
	            optionNode.value = "email";
	            optionNode.appendChild(document.createTextNode(this._msgEmail));


                if(this.isTabSelected(tabToBeSelected,'searchemail'))
                {
	                optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);

	            /*
	            optionNode = document.createElement("option");
	            optionNode.value = "custom_attributes";
	            optionNode.appendChild(document.createTextNode(this._msgUserCustomProperty));

	            if(this.isTabSelected(tabToBeSelected,'searchuserproperties'))
	            {
	            optionNode.setAttribute("selected", "selected");
	            }
	            selNode.appendChild(optionNode);
	            */

	            if (this._userCategoryXml != '') {
	                optionNode = document.createElement("option");
	                optionNode.value = "taxonomy";
	                optionNode.appendChild(document.createTextNode(this._msgTaxonomy));
                    if(this.isTabSelected(tabToBeSelected,'searchusercategory'))
                    {
	                    optionNode.setAttribute("selected", "selected");
	                }
	                selNode.appendChild(optionNode);
	            }
	        }
	        selNode.objectId = this.GetId().toString();
	        selNode.filterId = filterBlock.id.toString();
	        selNode.action = "check_key";
	        selNode.auxAction = "filter_select_changed";
	        selNode.onkeypress = CommunitySearchClass.DoAction;
	        selNode.onchange = CommunitySearchClass.DoAuxAction;
	        selNode.isGroup = group;
	        spanNode.appendChild(selNode);
	        filterBlock.appendChild(spanNode);

	        spanNode = document.createElement("span");
	        spanNode.setAttribute("class", "CommunitySearch_FilterTextboxContainer");
	        spanNode.setAttribute("className", "CommunitySearch_FilterTextboxContainer");

	        var textBoxNode = document.createElement("input");
	        textBoxNode.type = "text";
	        textBoxNode.value = "";
	        textBoxNode.objectId = this.GetId().toString();
	        textBoxNode.action = "check_key";
	        textBoxNode.onkeypress = CommunitySearchClass.DoAction;

	        spanNode.appendChild(textBoxNode);
	        filterBlock.appendChild(spanNode);


	        spanNode = document.createElement("span");
	        spanNode.setAttribute("class", "CommunitySearch_FilterTaxonomyLinkContainer");
	        spanNode.setAttribute("className", "CommunitySearch_FilterTaxonomyLinkContainer");
	        spanNode.style.display = "none";
	        spanNode.isGroup = group;

	        var selLinkNode = document.createElement("a");
	        selLinkNode.appendChild(document.createTextNode(this._msgSelectTaxonomy));
	        selLinkNode.setAttribute("href", "#");
	        selLinkNode.objectId = this.GetId().toString();
	        selLinkNode.action = "select_taxonomy";
	        selLinkNode.filterId = filterBlock.id.toString();
	        selLinkNode.isGroup = group;
	        selLinkNode.onclick = CommunitySearchClass.DoAction;

	        spanNode.appendChild(selLinkNode);
	        filterBlock.appendChild(spanNode);

	        if (!suppressRemoveBtn) {
	            spanNode = document.createElement("span");
	            spanNode.setAttribute("class", "CommunitySearch_FilterRemoveBtnContainer");
	            spanNode.setAttribute("className", "CommunitySearch_FilterRemoveBtnContainer");

	            var linkNode = document.createElement("a");
	            linkNode.appendChild(document.createTextNode(this._msgRemove));
	            linkNode.setAttribute("href", "#");
	            linkNode.objectId = this.GetId().toString();
	            linkNode.action = "remove_filter";
	            linkNode.filterId = filterBlock.id.toString();
	            linkNode.onclick = CommunitySearchClass.DoAction;

	            spanNode.appendChild(linkNode);
	            filterBlock.appendChild(spanNode);
	        }

	        if (filterBlock) {
	            container.appendChild(filterBlock);
	            result = true;
	        }
	    }
	    return (result);
	},

    ///////////////////////////////////
    // 
	this.RemoveFilter = function (id) {
	    var targNode = document.getElementById(id);
	    if (targNode) {
	        var checkTaxonomy = false;
	        var isGroup = false;
	        if (this.IsValid(targNode.isGroup)) {
	            checkTaxonomy = true;
	            isGroup = targNode.isGroup;
	        }
	        var targNodeParent = targNode.parentNode;
	        if (targNodeParent) {
	            targNodeParent.removeChild(targNode);
	        }
	        if (this.IsValid(checkTaxonomy)) {
	            this.CheckAndRemoveTaxonomy(isGroup);
	        }
	    }
	    return (false);
	},

    ///////////////////////////////////
    // 
	this.InitializeUI = function () {
	    // Ensure that no filters exist (in case of partial retry):
	    if (this._enableUserResults) {
	        var container = this.GetFilterContainer(false);
	        if (container) {
	            container.innerHTML = "";
	        }
	        else {
	            setTimeout('CommunitySearchClass.GetObject("' + this.GetId() + '").InitializeUI()', this._retryDelay);
	            return;
	        }
	    }

	    if (this._enableGroupResults) {
	        container = this.GetFilterContainer(true);
	        if (container) {
	            container.innerHTML = "";
	        }
	        else {
	            setTimeout('CommunitySearchClass.GetObject("' + this.GetId() + '").InitializeUI()', this._retryDelay);
	            return;
	        }
	    }

	    // Initialize the Advanced UI, by adding the first filters:
	    if (this._enableUserResults && !this.AddFilter(false, true, [])) { // (user filter)
	        setTimeout('CommunitySearchClass.GetObject("' + this.GetId() + '").InitializeUI()', this._retryDelay);
	        return;
	    }
	    if (this._enableGroupResults && !this.AddFilter(true, true, [])) { // (group filter)
	        setTimeout('CommunitySearchClass.GetObject("' + this.GetId() + '").InitializeUI()', this._retryDelay);
	        return;
	    }
	    // Check if need to auto-search on a tag, or open a particular tab:   

	    if (!this.AutoTabSearch()) {
	        switch (this._startingTab) {
	            case "directory":
	                this.SelectTab("directory");
	                break;

	            case "basic":
	                this.SelectTab("basic");
	                break;

	            case "advanced":
	                this.SelectTab("advanced");
	                break;

	            case "location":
	                if (this._enableMap)
	                    this.SelectTab("location");
	                break;
	            default:
	                this.SelectTab("basic");
	        }
	    }

	    this.SetReady(true);
	},
    ///////////////////////////////////  

     this.AutoTabSearch = function () {
         /// This method looks for query string parameters and runs an auto search /// returns a boolean

         var result = false;
         var query = location.search;
    if (query && (query.length > 0))
    {
             query = query.replace("?", "");
             var namValPairs = query.split("&");
             var tabKey = "";
             var tabValue = "";
             var isGroup = false;

        for (var idx = 0; idx < namValPairs.length; ++idx)
        {
                 var nvpair = namValPairs[idx].split("=");
                 tabKey = decodeURIComponent(nvpair[0].toLowerCase());
                 tabValue = decodeURIComponent(nvpair[1]);

            switch(tabKey)
            {
                     case "searchtag":
                         result = true;
                         this.AutoTagSearch(tabValue, false);
                         break;

                     case "searchgrptag":
                         result = true;
                         this.AutoTagSearch(tabValue, true);
                         break;

                     case "searchdisplayname":
                         result = true;
                         this.AutoDisplayNameSearch(tabValue, false);
                         break;

                     case "searchgroupname":
                         result = true;
                         this.AutoDisplayNameSearch(tabValue, true);
                         break;

                     case "searchgrpdescription":
                         result = true;
                         this.AutoGroupDescriptionSearch(tabValue, true);
                         break;

                     case "searchfirstname":
                         result = true;
                         this.AutoFirstNameSearch(tabValue, false);
                         break;

                     case "searchlastname":
                         result = true;
                         this.AutoLastNameSearch(tabValue, false);
                         break;

                     case "searchemail":
                         result = true;
                         this.AutoEmailSearch(tabValue, false);
                         break;

                     case "searchuserproperties":
                         result = true;
                         this.AutoUserPropertySearch(tabValue, false);
                         break;

                     case "searchusercategory":
                         result = true;
                         this.AutoCategorySearch(tabValue, false);
                         break;

                     case "searchgroupcategory":
                         result = true;
                         this.AutoCategorySearch(tabValue, true);
                         break;
                     default:
                         break;
                 }
             }

         }


         return (result);

     },



    ///////////////////////////////////
    // 
    this.AutoTagSearch = function (tag, isGroup) {
        var result = false;
        this.SearchTag(tag, isGroup);
        result = true;
        return (result);
    },


    ///////////////////////////////////
    // 
    this.AutoDisplayNameSearch = function (name, isGroup) {

        var result = false;
        this.SearchDisplayName(name, isGroup);
        result = true;
        return (result);
    },

    ///////////////////////////////////
    // 
    this.AutoGroupDescriptionSearch = function (description, isGroup) {

        var result = false;
        this.SearchGroupDescription(description, isGroup);
        result = true;
        return (result);
    },
    ///////////////////////////////////
    // 
    this.AutoFirstNameSearch = function (firstname, isGroup) {

        var result = false;
        this.SearchFirstName(firstname, isGroup);
        result = true;
        return (result);
    },

    ///////////////////////////////////
    // 
    this.AutoLastNameSearch = function (lastname, isGroup) {

        var result = false;
        this.SearchLastName(lastname, isGroup);
        result = true;
        return (result);
    },

    ///////////////////////////////////
    // 
    this.AutoEmailSearch = function (email, isGroup) {

        var result = false;
        this.SearchEmail(email, isGroup);
        result = true;
        return (result);
    },
    ///////////////////////////////////
    // 
    this.AutoUserPropertySearch = function (property, isGroup) {

        var result = false;
        this.SearchUserProperty(property, isGroup);
        result = true;
        return (result);
    },

    ///////////////////////////////////
    // 
    this.AutoCategorySearch = function (category, isGroup) {

        var result = false;
        if (category.length > 0) {
            this.SearchCategory(category, isGroup);
            result = true;
        }
        return (result);
    },


    ///////////////////////////////////////

	this.ReBuildFilters = function (group, tabToBeSelected) {
	    var result = false;
	    var container = this.GetFilterContainer(group);
	    if (container) {
	        container.innerHTML = "";
	        result = this.AddFilter(group, true, tabToBeSelected);
	    }
	    return (result);
	},

    ///////////////////////////////////
    // 
	this.GetFilterContainer = function (group) {
	    var id = "";
	    if (group) {
	        id = "groupFilterContainer_" + this.GetId();
	    }
	    else {
	        id = "userFilterContainer_" + this.GetId();
	    }
	    return (document.getElementById(id));
	},

    ///////////////////////////////////
    // 
	this.DoAdvancedSearch = function () {

	    this._resultFilterCategories = "";
	    this._resultFilterSearchMode = false;
	    this._resultFilterHasData = false;
	    this.DoAdvancedSearch_Parametric('', '', '1');

	}
	
    ///////////////////////////////////
    //
	this.DoAdvancedSearch_Parametric = function (args, context, page) {
	    var idx;
	    // get all user-search fields:

	    var userSearchModes = "";
	    var userSearchValues = "";
	    var userSearchMode;
	    var userSearchValue;
	    var UserSearchItems = new Array();
	    var GroupSearchItems = new Array();
	    var container = this.GetFilterContainer(false);

	    if (this._enableUserResults) {
	        var modeNodes = container.getElementsByTagName("select");
	        var valueNodes = container.getElementsByTagName("input");

	        //populate json data array with item values
	        var searchItem;
	        if ((modeNodes.length > 0) && (modeNodes.length == valueNodes.length)) {
	            for (idx = 0; idx < modeNodes.length; idx++) {
	                searchItem = null;
	                userSearchMode = modeNodes[idx].value;
	                userSearchValue = valueNodes[idx].value;
	                if (userSearchValue && userSearchValue.length > 0)
	                    searchItem = new CommunitySearchData(userSearchMode, escape(userSearchValue));
	                if (searchItem != null) UserSearchItems.push(searchItem);
	            }
	        }
	    }
	    // get all group-search fields:
	    var groupSearchModes = "";
	    var groupSearchValues = "";
	    var groupSearchMode;
	    var groupSearchValue;
	    var container = this.GetFilterContainer(true);

	    if (this._enableGroupResults) {
	        var modeNodes = container.getElementsByTagName("select");
	        var valueNodes = container.getElementsByTagName("input");
	        if ((modeNodes.length > 0) && (modeNodes.length == valueNodes.length)) {
	            for (idx = 0; idx < modeNodes.length; idx++) {
	                groupSearchMode = modeNodes[idx].value;
	                groupSearchValue = valueNodes[idx].value;
	                searchItem = null;
	                if (groupSearchValue && groupSearchValue.length > 0)
	                    searchItem = new CommunitySearchData(groupSearchMode, escape(groupSearchValue));
	                if (searchItem != null) GroupSearchItems.push(searchItem);
	            }
	        }
	    }

	    var searchText = "";
	    if (UserSearchItems != null && UserSearchItems.length > 0) {
	        searchText += "&__usersearchItemsJSON=" + Ektron.JSON.stringify(UserSearchItems);
	    }

	    if (GroupSearchItems != null && GroupSearchItems.length > 0) {
	        searchText += "&__groupsearchItemsJSON=" + Ektron.JSON.stringify(GroupSearchItems);
	    }

	    if (searchText.length > 0) {
	        searchText += "&__searchmode=advanced";
	    }
	    this.DoAjaxSearch(searchText, "", page);

	},

	///////////////////////////////////
	// User Directory Search
	///////////////////////////////////
    this.DoDirectoryFilterName = function (args, context, page) {
        var selObj = document.getElementById('CommunitySearch_SortByDirectoryDropdownList_' + this.GetId().toString());
        var selText = selObj.value;
        var searchText = "&__userDirectoryMode=true";
        if (args) {
            if (('@id > 0' == args) || ('all_users' == args)) {
                // get all users, but pass sort-by info:
                searchText += '&__usersearchtext=' + selText + '&__usersearchtype=all_users&__searchmode=advanced';
            }
            else {
                searchText += '&__usersearchtext=' + args + '&__usersearchtype=' + selText + '&__searchmode=advanced';
            }
            this._resultFilterCategories = "";
            this._resultFilterSearchMode = false;
            this._resultFilterHasData = false;
            this._savedArgs = searchText;
        }
        return this.DoAjaxSearch(searchText, '', page);
    },

	///////////////////////////////////
	// 
	this.DoBasicSearch = function () {
	    this._resultFilterCategories = "";
	    this._resultFilterSearchMode = false;
	    this._resultFilterHasData = false;
	    this.DoBasicSearch_Parametric('', '', '1');
	},

	///////////////////////////////////
	// 
	this.DoBasicSearch_Parametric = function (args, context, page) {
	    var textObj = document.getElementById("CommunitySearch_BasicTextbox_" + this.GetId());
	    if (textObj) {
	        var searchText = "";
	        if (textObj.value.length > 0) {
	            searchText = "&__searchtext=" + escape(textObj.value);
	            searchText += "&__searchmode=basic";
	        }
	        this.DoAjaxSearch(searchText, "", page);
	    }
	},

	///////////////////////////////////
	// 
	this.SelectTabReset = function (tab) {
	    if (("location" == tab) && ("location" != this._currentTab)) {
	        if (('undefined' != typeof EMap) && (null != EMap) && (EMap.ResetUser)) {
	            EMap.ResetUser();
	        }
	    }
	    this.SelectTab(tab);
	}
	    
    ///////////////////////////////////
    //
	this.SelectTab = function (tab) {
	    try
	    {	    
	        var directoryTab = document.getElementById('CommunitySearch_DirectorySearchTab_' + this.GetId());
	        var basicTab = document.getElementById('CommunitySearch_BasicSearchTab_' + this.GetId());
	        var advancedTab = document.getElementById('CommunitySearch_AdvancedSearchTab_' + this.GetId());
	        var locationTab = document.getElementById('CommunitySearch_LocationSearchTab_' + this.GetId());

	        var directoryContainer = document.getElementById('CommunitySearch_DirectoryContainer_' + this.GetId());
	        var basicContainer = document.getElementById('CommunitySearch_BasicContainer_' + this.GetId());
	        var advancedContainer = document.getElementById('CommunitySearch_AdvancedContainer_' + this.GetId());
	        var locationContainer = document.getElementById('CommunitySearchCtl_MapContainer_' + this.GetId());

	        switch (tab) {
	            case 'directory':
	                if (this._currentTab != "directory") {
	                    this._savedArgs = "";
	                    this.SetClassName(directoryTab, "CommunitySearch_DirectorySearchTab CommunitySearch_TabSelected");
	                    this.SetClassName(basicTab, "CommunitySearch_BasicSearchTab");
	                    this.SetClassName(advancedTab, "CommunitySearch_AdvancedSearchTab");

	                    this.SetClassName(directoryContainer, "CommunitySearch_DirectoryContainer_Selected");
	                    this.SetClassName(basicContainer, "CommunitySearch_BasicContainer");
	                    this.SetClassName(advancedContainer, "CommunitySearch_AdvancedContainer");

	                    if (this._enableMap) {
	                        this.SetClassName(locationTab, "CommunitySearch_LocationSearchTab");
	                        this.SetClassName(locationContainer, "CommunitySearchCtl_MapContainer");
	                    }
	                    else {
	                        if (locationTab)
	                            locationTab.style.display = "none";
	                        if (locationContainer)
	                            locationContainer.style.display = "none";
	                    }

	                    this._basicSearchMode = false;
	                    this.RestoreResults();
	                    this._currentTab = "directory";

	                }
	                break;

	            case 'basic':
	                if (this._currentTab != "basic") {
	                    this.SetClassName(directoryTab, "CommunitySearch_DirectorySearchTab");
	                    this.SetClassName(basicTab, "CommunitySearch_BasicSearchTab CommunitySearch_TabSelected");
	                    this.SetClassName(advancedTab, "CommunitySearch_AdvancedSearchTab");

	                    this.SetClassName(directoryContainer, "CommunitySearch_DirectoryContainer");
	                    this.SetClassName(basicContainer, "CommunitySearch_BasicContainer_Selected");
	                    this.SetClassName(advancedContainer, "CommunitySearch_AdvancedContainer");

	                    if (this._enableMap) {
	                        this.SetClassName(locationTab, "CommunitySearch_LocationSearchTab");
	                        this.SetClassName(locationContainer, "CommunitySearchCtl_MapContainer");
	                    }
	                    else {
	                        if (locationTab)
	                            locationTab.style.display = "none";
	                        if (locationContainer)
	                            locationContainer.style.display = "none";
	                    }

	                    this._basicSearchMode = true;
	                    this.RestoreResults();
	                    this._currentTab = "basic";

	                }
	                break;

	            case 'advanced':
	                if (this._currentTab != "advanced") {
	                    this.SetClassName(directoryTab, "CommunitySearch_DirectorySearchTab");
	                    this.SetClassName(basicTab, "CommunitySearch_BasicSearchTab");
	                    this.SetClassName(advancedTab, "CommunitySearch_AdvancedSearchTab CommunitySearch_TabSelected");

	                    this.SetClassName(directoryContainer, "CommunitySearch_DirectoryContainer");
	                    this.SetClassName(basicContainer, "CommunitySearch_BasicContainer");
	                    this.SetClassName(advancedContainer, "CommunitySearch_AdvancedContainer_Selected");
	                    this.SetClassName(locationContainer, "CommunitySearchCtl_MapContainer");

	                    if (this._enableMap) {
	                        this.SetClassName(locationTab, "CommunitySearch_LocationSearchTab");
	                        this.SetClassName(locationContainer, "CommunitySearchCtl_MapContainer");
	                    }
	                    else {
	                        if (locationTab)
	                            locationTab.style.display = "none";
	                        if (locationContainer)
	                            locationContainer.style.display = "none";
	                    }

	                    this._basicSearchMode = false;
	                    this.RestoreResults();
	                    this._currentTab = "advanced";
	                }
	                break;

	            case 'location':
	                if (this._currentTab != "location") {
	                    this.HideResults();

	                    if (this._enableMap) {
	                        this.SetClassName(directoryTab, "CommunitySearch_DirectorySearchTab");
	                        this.SetClassName(basicTab, "CommunitySearch_BasicSearchTab");
	                        this.SetClassName(advancedTab, "CommunitySearch_AdvancedSearchTab");
	                        this.SetClassName(locationTab, "CommunitySearch_LocationSearchTab CommunitySearch_TabSelected");

	                        this.SetClassName(directoryContainer, "CommunitySearch_DirectoryContainer");
	                        this.SetClassName(basicContainer, "CommunitySearch_BasicContainer");
	                        this.SetClassName(advancedContainer, "CommunitySearch_AdvancedContainer");
	                        this.SetClassName(locationContainer, "CommunitySearchCtl_MapContainer_Selected");
	                        this._currentTab = "location";
	                    }
	                    else {
	                        if (locationTab)
	                            locationTab.style.display = "none";
	                        if (locationContainer)
	                            locationContainer.style.display = "none";
	                    }
	                }
	                break;
	        }
	    }
        catch (e)
        {
	        setTimeout('CommunitySearchClass.GetObject("' + this.GetId() + '").SelectTab(\"' + tab + '\")', this._retryDelay);
	    }
	},

	///////////////////////////////////
	// 
    this.SetClassName = function (elm, className) {
        if (elm && "undefined" != typeof elm.className)
            elm.className = className;
    },

	///////////////////////////////////
	// 
    this.ShowSearching = function () {
        var containerObj = document.getElementById("CommunitySearch_ResultsContainer_" + this.GetId());
        if (containerObj) {
            containerObj.innerHTML = '<span class="CommunitySearch_ResultLoading" >  <img src="' + this._ajaxImage + '" />  </span> ';
            containerObj.style.display = "block";
        }
    },

	///////////////////////////////////
	// 
    this.ShowResults = function (results) {
        var containerObj = document.getElementById("CommunitySearch_ResultsContainer_" + this.GetId());
        if (containerObj) {
            containerObj.innerHTML = results;
            if (!(this._currentTab == 'directory' || this._currentTab == 'basic' || this._currentTab == 'advanced')) {
                this.HideResults();
            }
            else 
                containerObj.style.display = ((results.length > 0) ? "block" : "none");
            

            // notify user code if hooked:
            if ("undefined" != typeof this.SearchResultsChanged)
                this.SearchResultsChanged(containerObj);
        }
    },

	///////////////////////////////////
	// 
    this.HideResults = function () {
        var containerObj = document.getElementById("CommunitySearch_ResultsContainer_" + this.GetId());
        if (containerObj) {
            containerObj.style.display = "none";
        }
        this.ShowFilterCategories(false);
        this.ShowFilterCategoriesButton(false);
    },

	///////////////////////////////////
	// 
    this.RestoreResults = function () {
        var containerObj = document.getElementById("CommunitySearch_ResultsContainer_" + this.GetId());
        if (containerObj) {
            containerObj.style.display = ((containerObj.innerHTML.length > 0) ? "block" : "none");
        }
        this.ShowFilterCategoriesButton(this._resultFilterHasData);
        this.ShowFilterCategories(this._resultFilterSearchMode && this._resultFilterHasData);
    },

	///////////////////////////////////
	// 
    this.ProcessFilterSelectChange = function (selObj, filterId) {
        var container = document.getElementById(filterId);
        if (container) {
            var selCtl = container.getElementsByTagName("select")[0];
            var txtCtl = container.getElementsByTagName("input")[0];
            var taxLinkCtl = container.getElementsByTagName("a")[0];

            if ("taxonomy" == selObj.value) {
                // hide text-box, and show taxonomy-link:
                txtCtl.parentNode.style.display = "none";
                taxLinkCtl.parentNode.style.display = "";
                // taxonomy filter must-not include search text:
                txtCtl.value = "";
            }
            else {
                // show text-box, and hide taxonomy-link:
                txtCtl.parentNode.style.display = "";
                taxLinkCtl.parentNode.style.display = "none";
                if (this.IsValid(container.isGroup)) {
                    this.CheckAndRemoveTaxonomy(container.isGroup);
                }
            }
        }
    },

	///////////////////////////////////
	// 
    this.SelectTaxonomy = function (objectId, filterId, isGroup) {
        this.ShowCategories(isGroup);
    },

	///////////////////////////////////
	// 
    this.ShowLocationForUser = function (userData) {
        if (this.IsValid(EMap) && this.IsValid(EMap.FindUser)) {
            if (this._enableMap) {
                this.SelectTab("location");
                EMap.FindUser(userData);
            }
        }
        return (false); // cancel link-click.
    },

	///////////////////////////////////
	// CheckAndRemoveTaxonomy: Check if any taxonomy-filters are visible for 
	// this section, if not then don't use in search results...
    this.CheckAndRemoveTaxonomy = function (isGroup) {
        var container = this.GetFilterContainer(isGroup);
        if (container) {
            var removeTaxonomy = true;
            var nodes = this.GetObjectElementsByClassName(container, "CommunitySearch_FilterTaxonomyLinkContainer");
            if (nodes && nodes.length) {
                for (var idx = 0; idx < nodes.length; ++idx) {
                    if ("none" != nodes[idx].style.display) {
                        removeTaxonomy = false;
                        break;
                    }
                }
            }
            if (removeTaxonomy) {
                if (isGroup) {
                    this._groupCategories = "";
                }
                else {
                    this._userCategories = "";
                }
            }
        }
    }
        
    ///////////////////////////////////
    // 
    this.DirectoryTabSelected = function (){
        return ("directory" == this._currentTab);
    },
    
    ///////////////////////////////////
    // 
    this.BasicTabSelected = function (){
        return ("basic" == this._currentTab);
    },
    
    ///////////////////////////////////
    // 
    this.AdvancedTabSelected = function (){
        return ("advanced" == this._currentTab);
    },
    
    ///////////////////////////////////
    // 
    this.LocationTabSelected = function (){
        return ("location" == this._currentTab);
    },
    
    ///////////////////////////////////
    // 
    this.GetObjectElementsByClassName = function (obj, className) {
	    var result = new Array;
	    if (obj){
	        var divArray = obj.getElementsByTagName("*");
	        for (var idx=0; idx < divArray.length; idx++) {
		        if (("undefined" != divArray[idx].className)
			        && (this.HasClassName(divArray[idx], className))) {
			        result[result.length] = divArray[idx];
		        }
	        }
	    }
	    return (result);
    },

    ///////////////////////////////////
    // 
    this.GetObjectElementsByClassNameAndTagName = function (obj, className, tagName) {
	    var result = new Array;
	    if (obj){
	        var divArray = obj.getElementsByTagName(tagName);
	        for (var idx=0; idx < divArray.length; idx++) {
		        if (("undefined" != divArray[idx].className)
			        && (this.HasClassName(divArray[idx], className))) {
			        result[result.length] = divArray[idx];
		        }
	        }
	    }
	    return (result);
    },

    ///////////////////////////////////
    // 
    this.GetElementsByClassName = function (className) {
	    var result = new Array;
	    var divArray = this.GetElementsByTagName("*");
	    for (var idx=0; idx < divArray.length; idx++) {
		    if (("undefined" != divArray[idx].className)
			    && (this.HasClassName(divArray[idx], className))) {
			    result[result.length] = divArray[idx];
		    }
	    }
	    return (result);
    },

    ///////////////////////////////////
    // 
    this.GetElementsByClassNameAndTagName = function (className, tagName) {
	    var result = new Array;
	    var divArray = this.GetElementsByTagName(tagName);
	    for (var idx=0; idx < divArray.length; idx++) {
		    if (("undefined" != divArray[idx].className)
			    && (this.HasClassName(divArray[idx], className))) {
			    result[result.length] = divArray[idx];
		    }
	    }
	    return (result);
    },

    ///////////////////////////////////
    // 
    this.HasClassName = function (obj, className) {
	    var idx, names;
	    if (obj && ("undefined" != typeof obj.className)
		    && ("undefined" != typeof obj.className.split)) {
		    names = obj.className.split(" ");
		    for (idx = 0; idx < names.length; idx++) {
			    if (names[idx] == className)
				    return true;
		    }
	    } 
	    return false;
    },

    ///////////////////////////////////
    // 
    this.AddClassName = function (obj, className) {
	    if (this.HasClassName(obj, className))
		    return;
    	
	    if (obj && ("undefined" != typeof obj.className)
		    && ("undefined" != typeof obj.className.length)) {
		    if (0 == obj.className.length) {
			    obj.className = className;
		    }
		    else {
			    obj.className += " " + className;
		    }
	    } 
    },

    ///////////////////////////////////
    // 
    this.RemoveClassName = function (obj, className) {
	    var idx, matchId, names, result;
	    if (obj && ("undefined" != typeof obj.className)
		    && ("undefined" != typeof obj.className.split)) {
		    names = obj.className.split(" ");
		    obj.className = "";
		    for (idx = 0; idx < names.length; idx++) {
			    if (names[idx] != className) {
				    if (idx > 0)
					    obj.className += " " + names[idx];
				    else
					    obj.className += names[idx];
			    }
		    }
	    } 
    },

    ///////////////////////////////////
    // 
    this.GetElementsByTagName = function (tagName) {
	    var result = new Array;
	    var container = this.GetCommunitySearchContainerElement();
	    if (container && ("undefined" != typeof container.getElementsByTagName)) {
		    var divArray = container.getElementsByTagName(tagName);
		    if (("undefined" != typeof divArray) && (null != divArray)) {
			    result = divArray;
		    }
	    }
	    return (result);
    },
    
    ///////////////////////////////////
    // 
    this.GetCommunitySearchContainerElement = function () {
	    var containerId = "CommunitySearchCtl_" +  this.GetId();
	    return (document.getElementById(containerId));
    },
    
    ///////////////////////////////////
    // 
    this.IsValid = function (obj){
        return (("undefined" != typeof obj) && (null != obj));
    },

    ///////////////////////////////////
    // 
	this.FilterResults = function (clientid){	
        var categoryText = '';
        var selectedids = '';
        var selectedidlist;
        var divName = "CommunitySearch_ResultFilterCategoryTreePane" + clientid;
        if ($ektron("div#" + divName + " ul.taxonomyFilter").length > 0 ) {
            var taxnomyvalues = $ektron("div#" + divName + " input.searchTaxonomyPath");
            for(var count=0; count<taxnomyvalues.length; count++) 
            {
                if (taxnomyvalues[count].checked === true)
                {
                    if (categoryText.length <= 0)
                        categoryText = taxnomyvalues[count].value;
                    else
                        categoryText += "," + taxnomyvalues[count].value;
                }
            }
        }
        categoryText = categoryText.replace(/&/gi, "#eksepamp#");
        this._resultFilterCategories = categoryText;
        this._resultFilterSearchMode = true;

        if (this._basicSearchMode){
            this.DoBasicSearch_Parametric('', '', '1');
        }
        else if (this._currentTab == "directory"){ 
            if (this._savedArgs.length > 0){
                args = this._savedArgs;
            }
            else {            
            args = "";
            }
            this.DoAjaxSearch(args, "", "1");
        }
        else{
            this.DoAdvancedSearch_Parametric('', '', '1');
        }
        return (false);

    },
    this.CheckUserGroupTaxonomy = function (filterType, taxonomyPath){
    ///This method loops through the category nodes and checks the nnode based on the taxonomy path from the query string and triggers a search ///
        var categoryText = '';
        var selectedids = '';
        var selectedidlist;
        var divClass;
        var isGroup;
        if (filterType == "user")
        {
            divClass = "CommunitySearch_UserCategoryTreePane";
            isGroup = false;
        }
        else
        {
            divClass = "CommunitySearch_GroupCategoryTreePane";
            isGroup = true;
        }
//        $ektron("div." + divClass +" ul.taxonomyFilter").treeview({
//                        collapsed: true
//                    });
        if ($ektron("div." + divClass + " ul.taxonomyFilter").length > 0 ) {
            var taxnomyvalues = $ektron("div." + divClass + " input.searchTaxonomyPath");
           
            
            for(var count=0; count<taxnomyvalues.length; count++) 
            {
            
            if(taxnomyvalues[count].value == taxonomyPath)
            {
            taxnomyvalues[count].checked = true; break;          
            }  
            
            }
        }
        categoryText = taxonomyPath;
        categoryText = categoryText.replace(/&/gi, "#eksepamp#");

        if (filterType == "user")
        {
            this._userCategories = categoryText;             
            
        }
        else
        {
            this._groupCategories = categoryText;
        }
        
        
        var container = this.GetFilterContainer(isGroup);
           if (container){
            var removeTaxonomy = true;
            var nodes = this.GetObjectElementsByClassName(container, "CommunitySearch_FilterTaxonomyLinkContainer");
            var textBoxes =  this.GetObjectElementsByClassName(container, "CommunitySearch_FilterTextboxContainer");
            if (nodes && nodes.length){
                for (var idx = 0; idx < nodes.length; ++idx){

                 nodes[idx].style.display = "block";
                }
            }
            if (textBoxes && textBoxes.length){
                for (var j = 0; j < textBoxes.length; ++j){

                 textBoxes[j].style.display = "none";
                }
            }
            }
            
        
        this.TriggerSearch('', '', '1'); 
    },
    
    this.FilterUserGroup = function (clientid, filterType){
        var categoryText = '';
        var selectedids = '';
        var selectedidlist;
        var divClass;
        if (filterType == "user")
        {
            divClass = "CommunitySearch_UserCategoryTreePane";
            
        }
        else
        {
            divClass = "CommunitySearch_GroupCategoryTreePane";
        }
        
        if ($ektron("div." + divClass + " ul.taxonomyFilter").length > 0 ) {
            var taxnomyvalues = $ektron("div." + divClass + " input.searchTaxonomyPath");
            for(var count=0; count<taxnomyvalues.length; count++) 
            {
                if (taxnomyvalues[count].checked === true)
                {
                    if (categoryText.length <= 0)
                        categoryText = taxnomyvalues[count].value;
                    else
                        categoryText += "," + taxnomyvalues[count].value;
                }
            }
        }
        categoryText = categoryText.replace(/&/gi, "#eksepamp#");

        if (filterType == "user")
        {
            this._userCategories = categoryText;
        }
        else
        {
            this._groupCategories = categoryText;
        }
        
        this.TriggerSearch('', '', '1'); 
    },

    ///////////////////////////////////
    // 
	this.ToggleFilterCategories = function (btnObj){
        var targEl = document.getElementById('CommunitySearch_ResultFilterCategoryContainer' + this.GetId().toString());
        if (targEl){
            this.ShowFilterCategories('none' == targEl.style.display);
        }
    },
    
    ///////////////////////////////////
    // 
	this.ShowFilterCategories = function (showFlag){
        var targEl = document.getElementById('CommunitySearch_ResultFilterCategoryContainer' + this.GetId().toString());
        if (targEl){
            if (showFlag){
                targEl.style.display = 'block';
                targEl = document.getElementById('CommunitySearch_FilterCategoriesButton' + this.GetId().toString());
                if (targEl){
                    targEl.className = "CommunitySearch_FilterCategoriesButton_selected";
                }
            }
            else{
                targEl.style.display = 'none';
                targEl = document.getElementById('CommunitySearch_FilterCategoriesButton' + this.GetId().toString());
                if (targEl){
                    targEl.className = "CommunitySearch_FilterCategoriesButton";
                }
            }
        }
    },
    
    ///////////////////////////////////
    // 
	this.ShowFilterCategoriesButton = function (showFlag){
        var targEl = document.getElementById('CommunitySearch_FilterCategoriesButton' + this.GetId().toString());
        if (targEl){
            if (showFlag){
                targEl.style.display = 'block';
            }
            else{
                targEl.style.display = 'none';
            }
        }
    }
}//  end of instance-level CommunitySearchClass class definition.
///////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////
// CommunitySearchClass Static members (methods):

///////////////////////////////////
// 
CommunitySearchClass.GetObject = function (id, creatable){
    var fullId = "CommunitySearchObj_" + id;
    var allowCreation = true;
    if ("undefined" != typeof creatable)
        allowCreation = (true == creatable);
    if (("undefined" != typeof window[fullId])
	    && (null != window[fullId])){
	    return (window[fullId]);
    }
    else if (allowCreation){
        var obj = new CommunitySearchClass(id);
        window[fullId] = obj;
        obj.Constructor();
        return (obj);
    }
}

///////////////////////////////////
// 
CommunitySearchClass.IsReady = function (id){
    var obj = CommunitySearchClass.GetObject(id, false);
    if (obj)
	    return (obj.GetReady());
    else
        return (false);
}

///////////////////////////////////
// 
CommunitySearchClass.DoAction = function (event, parm2){
    var result = false; // cancel click for links.
    if (this.objectId && this.action){
        var obj = CommunitySearchClass.GetObject(this.objectId);
        if (obj){
            var evt = event;
            if (!evt){
                evt = window.event;
            }
            
            switch (this.action){
                case "remove_filter":
                    if (this.filterId){
                        obj.RemoveFilter(this.filterId);
                    }
                    break;
                    
                case "check_key":
                    return obj.CheckKey(evt);
                    break;
                    
                case "select_taxonomy":
                    obj.SelectTaxonomy(this.objectId, this.filterId, this.isGroup);
                    break;
                    
                case "show_category":
                    obj.OnNodeSelect(event, parm2, this.isGroup);
                    break;
                    
                case "filter_category":
                    obj.FilterResults(event, parm2);
                    break;
            }
        }
    }
    return (result);
}

///////////////////////////////////
// 
CommunitySearchClass.DoAuxAction= function (event){
    var result = false; // cancel click for links.
    if (this.objectId && this.auxAction){
        var obj = CommunitySearchClass.GetObject(this.objectId);
        if (obj){
            var evt = event;
            if (!evt){
                evt = window.event;
            }
            
            switch (this.auxAction){
                case "filter_select_changed":
                    if (this.filterId){
                        obj.ProcessFilterSelectChange(this, this.filterId);
                    }
                    break;
            }
        }
    }
    return (result);
}

///////////////////////////////////
// 
CommunitySearchClass.LogIt = function (msg){
    if (window.console && window.console.log){
        window.console.log(msg);
    }
    else if (window.Debug && window.Debug.writeln){
        window.Debug.writeln(msg);
    }
}

///////////////////////////////////
// 
CommunitySearchClass.addLoadEvent = function(func) {
    var oldonload = window.onload;
    if (typeof window.onload != 'function') {
        window.onload = func;
    } else {
        window.onload = function() {
            if (oldonload) {
                oldonload();
            }
            func();
        }
    }
}

///////////////////////////////////
// 
CommunitySearchClass.Hook_NotifySearchResultsChanged = function(id, func) {
    if (CommunitySearchClass.IsReady(id)){
        var obj = CommunitySearchClass.GetObject(id);
        if (obj){
            // TODO: Update to handle chaining multiple methods.
            obj.SearchResultsChanged = func;
        }
    }
    else{
        setTimeout('CommunitySearchClass.Hook_NotifySearchResultsChanged("' + id + '",' + func + ')', 50);
    }
}

///////////////////////////////////
// 
CommunitySearchClass.Unhook_NotifySearchResultsChanged = function(id, func) {
    if (CommunitySearchClass.IsReady(id)){
        var obj = CommunitySearchClass.GetObject(id);
        if (obj && (func == obj.SearchResultsChanged)){
            // TODO: Update to handle removal of a method from a chain.
            obj.SearchResultsChanged = null;
        }
    }
    else{
        setTimeout('CommunitySearchClass.Unhook_NotifySearchResultsChanged("' + id + '",' + func + ')', 50);
    }
}

//
///////////////////////////////////
// 
CommunitySearchClass.CallbackWhenReady = function(id, func) {
    if (CommunitySearchClass.IsReady(id)){
        var obj = CommunitySearchClass.GetObject(id);
        if (obj && func){
            func();
        }
    }
    else{
        setTimeout('CommunitySearchClass.CallbackWhenReady("' + id + '",' + func + ')', 50);
    }
}

///////////////////////////////////
// 
CommunitySearchClass.DoSearch = function(id, searchArgs) {
    if (CommunitySearchClass.IsReady(id)){
        var obj = CommunitySearchClass.GetObject(id);
        if (obj){
            obj.DoAjaxSearch(searchArgs, '', '1');
        }
    }
    else{
        setTimeout('CommunitySearchClass.DoSearch("' + id + '","' + searchArgs + '")', 50);
    }
}

function CommunitySearchData(searchtype, searchtext) {
    this.searchType = searchtype;
    this.searchText = searchtext;
}
