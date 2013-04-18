///////////////////////////////////////////////////////////
// Ektron_Ecommerce_CheckoutClass Static members (methods):
// (this was moved up here because addLoadEvent is used while browsers are still parsing the class below
///////////////////////////////////
// 
Ektron_Ecommerce_CheckoutClass.GetObject = function(id, creatable) {
    var fullId = "CheckoutObj_" + id;
    var allowCreation = true;
    if ("undefined" != typeof creatable)
        allowCreation = (true == creatable);
    if (("undefined" != typeof window[fullId])
	    && (null != window[fullId])) {
        return (window[fullId]);
    }
    else if (allowCreation) {
        var obj = new Ektron_Ecommerce_CheckoutClass(id);
        window[fullId] = obj;
        return (obj);
    }
}

///////////////////////////////////
// 
Ektron_Ecommerce_CheckoutClass.IsReady = function(id) {
    var obj = Ektron_Ecommerce_CheckoutClass.GetObject(id, false);
    if (obj)
        return (obj.IsReady());
    else
        return (false);
}

///////////////////////////////////
// 
Ektron_Ecommerce_CheckoutClass.DoAction = function(event, parm2) {
    var result = false; // cancel click for links.
    if (this.objectId && this.action) {
        var obj = Ektron_Ecommerce_CheckoutClass.GetObject(this.objectId);
        if (obj) {
            var evt = event;
            if (!evt) {
                evt = window.event;
            }

            switch (this.action) {
                default:
                    obj.TestIt(event, parm2);
                    break;
            }
        }
    }
    return (result);
}

///////////////////////////////////
// 
Ektron_Ecommerce_CheckoutClass.LogIt = function(msg) {
    if (!Ektron_Ecommerce_CheckoutClass.enableLogging)
    { return; }

    var textMsg = ((Ektron_Ecommerce_CheckoutClass.enableMsgPrefix) ? "Ektron_Ecommerce_CheckoutClass: " : "") + msg;
    if (Ektron_Ecommerce_CheckoutClass.enableShowLog) {
        var logObj = $ektron("#EktronCheckout_log");
        if (!logObj || !logObj.length) {
            logObj = $ektron("<div class='EktronCheckout_log' id='EktronCheckout_log'><h3>Checkout Debug Message Log</h3></div>");
            $ektron("body").append(logObj);
        }
        logObj.append("<div class='EktronCheckout_log_msg'>" + textMsg + "</div>");
    }
    else {
        if (window.console && window.console.log) {
            window.console.log(textMsg);
        }
        else if (window.Debug && window.Debug.writeln) {
            window.Debug.writeln(textMsg);
        }
    }
}

///////////////////////////////////
// 
Ektron_Ecommerce_CheckoutClass.addLoadEvent = function(func) {
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

// Class Ektron_Ecommerce_CheckoutClass:
function Ektron_Ecommerce_CheckoutClass(id) {
    ///////////////////////////////////////////////////////
    // Instance members - variables:
    this._id = id,
	this._ajaxCall = null,
	this._ajaxCallbackId = "",
	this._readyFlag = false,
    this._browserInitDelay = 100,
    this._retryDelay = 50,
	this._ajaxImage = "",
	this._imagePath = "",
	this._appPath = "",
    this._blockedItems = new Array(),

    ///////////////////////////////////////////////////////
    // Instance members - methods:


    ///////////////////////////////////
    // Initialize: Called when this object is first created.
    this.Constructor = function () {
        var checkoutObjId = this.GetId();
        setTimeout(function () { Ektron_Ecommerce_CheckoutClass.GetObject(checkoutObjId).PreInitialize(); }, this._browserInitDelay);
    },

    ///////////////////////////////////
    // PreInitialize: Called to wire-up deffered call to initialize.
    this.PreInitialize = function () {
        var checkoutObjId = this.GetId();
        var retry = false;

        try {
            if (("undefined" != typeof $ektron)
                && (null != $ektron)
                && ("undefined" != typeof $ektron(document))
                && (null != $ektron(document))
                && ("undefined" != typeof Ektron.ready))
            { this.Initialize(); }
            else { retry = true; }
        }
        catch (e) { retry = true; }

        if (retry) { setTimeout(function () { Ektron_Ecommerce_CheckoutClass.GetObject(checkoutObjId).PreInitialize(); }, this._browserInitDelay); }
    },

    ///////////////////////////////////
    // Initialize: 
    // 
    this.Initialize = function () {
        if (this.IsReady()) return;

        // Initialize state here:

        var checkoutObjId = this.GetId();

        // init this controls UI:
        this.InitializeUI();
        this.InitializeModals();
        this.HandleServerValidationFailures();

        // complete:
        this._readyFlag = true;
    },

    ///////////////////////////////////
    // InitializeModals:
    // 
    this.InitializeModals = function () {

        // initialize the modal dialogs, if they exist:
        if ($ektron("#EktronCheckout_" + this.GetId() + " .EktronCheckout_modalContainer").length > 0) {
            $ektron("#EktronCheckout_" + this.GetId()
                + " .EktronCheckout_modalContainer").modal(
                    { modal: true, toTop: true,
                        onShow: function (obj) { obj.w.fadeIn(333); }
                    });
        }
    },

    ///////////////////////////////////
    // 
    this.HandleServerValidationFailures = function () {
        if ($ektron("#EktronCheckout_" + this.GetId() + " .EktronCheckout_validationFailure").length > 0) {
            this.ShowModal("EktronCheckout_validationFailure");
        }
    },


    ///////////////////////////////////
    // GetId: Returns the client-id associated with the server control.
    this.GetId = function () {
        return (this._id);
    },

    ///////////////////////////////////
    // 
    this.IsReady = function () {
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
    this.SetAppPath = function (path) {
        this._appPath = path;
    },

    ///////////////////////////////////
    // 
    this.EnableScriptLogging = function (flag) {
        Ektron_Ecommerce_CheckoutClass.enableLogging = flag;
    },

    ///////////////////////////////////
    // 
    this.ForceUILogging = function (flag) {
        Ektron_Ecommerce_CheckoutClass.enableShowLog = flag;
    },

    ///////////////////////////////////
    // 
    this.ModalCentering = function (flag) {
        Ektron_Ecommerce_CheckoutClass.enableModalCentering = flag;
    },

    ///////////////////////////////////
    // 
	this.DoAjaxCallback = function (args, context) {
	    var targObj = null;

	    // Show Ajax-loading image if specified:
	    if (this._ajaxImage.length > 0) {
	        this.ShowAjaxWorking();
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
	        var ajaxUrl = window.location.href.replace(window.location.hash, "") // IE/Windows will block callbacks with a pound/hash symbol in the URL.
	        $ektron.ajax({
	            type: "POST",
	            url: ajaxUrl,
	            data: ektronPostParams,
	            dataType: "html",
	            error: function (ektronCallbackResult) { Ektron_Ecommerce_CheckoutClass.GetObject(context).AjaxCallback_DisplayError(ektronCallbackResult, context); },
	            success: function (ektronCallbackResult) {
	                var PACKET_START = "EKTRON_PACKET_START|";
	                var payload = String(ektronCallbackResult);
	                var idx = payload.indexOf(PACKET_START);
	                if (idx >= 0) {
	                    var payload = payload.substring(idx + PACKET_START.length);
	                }
	                Ektron_Ecommerce_CheckoutClass.GetObject(context).AjaxCallback_DisplayResult(payload, context);
	            }
	        });
	    }

	    return (false); // cancel events.
	},

    ///////////////////////////////////
    // 
    this.AjaxCallback_DisplayResult = function (args, context) {
        if (args && args.length) {
            var checkOutObj = Ektron_Ecommerce_CheckoutClass.GetObject(context, false);
            var responseBlocks = args.split("|");
            if (responseBlocks && responseBlocks.length) {
                var resStatus = responseBlocks[0];
                if ("1" == resStatus) {
                    var jsonLength = parseInt(responseBlocks[1]);
                    var markupLength = parseInt(responseBlocks[2]);
                    var markupTargetId = responseBlocks[3];
                    var unblock = responseBlocks[4];
                    var json = "";
                    var markup = "";
                    var jsonPayloadOffset = (responseBlocks[0].length + 1
                        + responseBlocks[1].length + 1
                        + responseBlocks[2].length + 1
                        + responseBlocks[3].length + 1
                        + responseBlocks[4].length + 1);
                    if ("1" == unblock) {
                        if (checkOutObj)
                            checkOutObj.UnBlockCurrentItems();
                    }
                    if (jsonLength) {
                        json = args.substr(jsonPayloadOffset, jsonLength);
                        eval(json);
                    }
                    var marupPayloadOffset = jsonPayloadOffset + jsonLength;
                    if (markupLength) {
                        markup = args.substr(marupPayloadOffset, markupLength);
                        if (markupTargetId.length > 0) {
                            //$ektron("#" + markupTargetId).html(markup);
                            $ektron(markupTargetId).html(markup);
                        }
                    }

                    // call back for post-success cleanup:
                    var checkoutObjId = this.GetId();
                    setTimeout(function () { Ektron_Ecommerce_CheckoutClass.GetObject(checkoutObjId).PostAjaxCleanup(); }, this._browserInitDelay);
                }
            }
        }
    },

    ///////////////////////////////////
    // 
    this.PostAjaxCleanup = function () {
        // cleanup (e.g. may need to wireup the Ektron 
        // Javascript library to newly rendered elements):
        this.InitializeModals();

        // do any other related work...
        this.DisableInputAutocomplete();
        this.HandleServerValidationFailures();
        this.HookUIEvents();
    },

    ///////////////////////////////////
    // 
    this.AjaxCallback_DisplayError = function (args, context) {
        if (context && context.length > 0) {
            var checkOutObj = Ektron_Ecommerce_CheckoutClass.GetObject(context, false);
            if (checkOutObj)
                checkOutObj.UnBlockCurrentItems();
        }
        var statusCode = "";
        var statusText = "";
        if (args && args.status && args.status.toString().length > 0) {
            statusCode = "Status: " + args.status.toString();
        }
        if (args && args.statusText && args.statusText.length > 0) {
            statusText = " " + args.statusText;
        }
        var errorDetails = "";
        if ((statusCode.length > 0) || (statusText.length > 0)) {
            errorDetails = " Details: " + statusCode + ((statusCode.length > 0) ? ", " : "") + statusText;
        }
        Ektron_Ecommerce_CheckoutClass.LogIt('Error Occurred in Ektron_Ecommerce_CheckoutClass: Ajax-Error!' + errorDetails);
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
    // This prevents the browser from caching/auto-completing fields (which would overwrite ajax callback values on page refresh).
	this.DisableInputAutocomplete = function () {
	    $ektron("#EktronCheckoutCtl_checkoutContainer_" + this.GetId() + " input[type='text']").attr("autocomplete", "off");
	},

    ///////////////////////////////////
    // 
	this.InitializeUI = function () {
	    this.DisableInputAutocomplete();
	    this.HookUIEvents();
	},

    ///////////////////////////////////
    // 
	this.HookUIEvents = function () {
	    var thisId = this.GetId();
	    $ektron("#EktronCheckout_" + thisId + " .EktronCheckout_RoutingAccountNumberHelp").hover(
            function () { $ektron("#EktronCheckout_" + thisId + " .EktronCheckout_RoutingAccountNumberHelp .innerContainer").show(); },
            function () { $ektron("#EktronCheckout_" + thisId + " .EktronCheckout_RoutingAccountNumberHelp .innerContainer").hide(); }
        );
	},

    ///////////////////////////////////
    // 
    this.ShowAjaxWorking = function () {
        // handled elsewhere:
        //    var containerObj = document.getElementById("_ResultsContainer_" + this.GetId());
        //    if (containerObj){
        //        containerObj.innerHTML = '<span class="_ResultLoading" >  <img src="' + this._ajaxImage + '" />  </span> ';
        //        containerObj.style.display = "block";
        //    }
    },

    this.UnBlockCurrentItems = function () {
    },

    this.Login = function () {
        var emText = $ektron("#EktronCheckout_" + this.GetId() + " .EktronCheckout_LoginBlock .EktronCheckout_LoginEmail input:text")[0].value;
        var codeText = $ektron("#EktronCheckout_" + this.GetId() + " .EktronCheckout_LoginBlock .EktronCheckout_LoginPassword input:password")[0].value;
        var hdnElm = $ektron("#EktronCheckout_" + this.GetId() + " #EktronCheckout_LoginBlock_Hidden_" + this.GetId());
        if (hdnElm && hdnElm[0] && (emText.length > 0) && (codeText.length > 0)) {
            hdnElm[0].value = "__opcode=logcustomerin&param__email=" + emText + "&param__code=" + codeText;
            // do postback:
            return (true);
        }
        else {
            return (false);
        }
    },

    this.HideMutableShowBusyContainer = function (container) {
        // hide mutable controls and show busy image:
        $ektron(".EktronCheckout_MutableControlContainer").children().hide();
        if (container != "")
            $ektron(container + " .EktronCheckout_AjaxBusyImageContainer").show();
        else
            $ektron(".EktronCheckout_AjaxBusyImageContainer").show();
    },

    this.HideMutableShowBusy = function () {
        // hide mutable controls and show busy image:
        $ektron(".EktronCheckout_MutableControlContainer").children().hide();
        $ektron(".EktronCheckout_AjaxBusyImageContainer").show();
    },

    this.ShowMutableClearBusy = function () {
        // hide mutable controls and show busy image:
        $ektron(".EktronCheckout_MutableControlContainer").children().show();
        $ektron(".EktronCheckout_AjaxBusyImageContainer").hide();
    },

    this.NextPage = function () {
        this.HideMutableShowBusy();
        this.DoAjaxCallback("__opcode=nextpage", "");
    },

    this.PreviousPage = function () {
        this.HideMutableShowBusy();
        this.DoAjaxCallback("__opcode=prevpage", "");
    },

    this.ShowModal = function (suffix) {
        $ektron("#EktronCheckout_" + this.GetId()
            + this.MakeClassSelector(suffix)).modalShow();

        this.centerModal(suffix);
    },

    this.centerModal = function (suffix) {
        if (!Ektron_Ecommerce_CheckoutClass.enableModalCentering)
            return;

        try {
            // attempt to centrally position the modal dialog over the checkout control:

            var modalObj = $ektron(this.MakeClassSelector(suffix));
            if (modalObj.length
                && modalObj.parent()[0].tagName.toLowerCase() == "body") {

                var y = $ektron(".EktronCheckout").position()["top"] + 25;
                if (y > 0)
                    modalObj.css({ top: y.toString() + "px" })

                //
                var pw = $ektron("body").width();
                var cw = modalObj.width()
                if (pw > 0 && cw > 0 && pw > cw) {
                    var x = pw / 2 - cw / 2;
                    if (x > 0)
                        modalObj.css({ left: x.toString() + "px" })
                }
            }
        }
        catch (e) { }
    },

    this.HideAllModals = function () {
        // jqModal moves the block to be a direct child of the body, 
        // so cannot target only items inside this controls markup:
        $ektron(".EktronCheckout_modalContainer").modalHide();
    },

    this.Edit = function (containerClassName) {
        //this.ShowModal(containerClassName + ((containerClassName.indexOf("EktronCheckout_modalContainer") > 0) ? "" : " .EktronCheckout_modalContainer"));
        this.ShowModal(containerClassName);
    },

    this.ShippingIsBillingAddress = function (evtObj, containerClassName) {
        var containerId = this.MakeClassSelector(containerClassName);
        var containerObject = $ektron(containerId);
        if (evtObj && containerObject) {
            if (evtObj.checked) {
                // same as billing, these fields not needed:
                containerObject.find(".EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Disabled");
                containerObject.find(".EktronCheckout_Rows_Container li").addClass("EktronCheckout_Row_Disabled");
                $ektron(containerId + " input:text").attr("disabled", "disabled");
                $ektron(containerId + " select").attr("disabled", "disabled");
            }
            else {
                // not the same as billing, mark fields as needed:
                containerObject.find(".EktronCheckout_Required_Disabled").removeClass("EktronCheckout_Required_Disabled").addClass("EktronCheckout_Required");
                containerObject.find(".EktronCheckout_Row_Disabled").removeClass("EktronCheckout_Row_Disabled");
                $ektron(containerId + " input:text").attr("disabled", "");
                $ektron(containerId + " select").attr("disabled", "");
            }

            // save the checkboxes state, for undoing:
            if ("undefined" == typeof this.OnCancel_CheckboxObject
                || null == this.OnCancel_CheckboxObject) {
                this.OnCancel_CheckboxObject = evtObj;
                this.OnCancel_CheckboxInitialState = !evtObj.checked; // toggled...
                this.OnCancel_CheckboxContainerClassName = containerClassName;
            }
        }
    },

    this.SaveBillingGetNextPage = function (containerClassName) {
        var containerId = this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "save_billinginfo_getnextpage")) {
            this.HideMutableShowBusy();
            // show the 'wait while creatign account' message
            $ektron(".EktronCheckout_waitCreatingAccountMessageContainer").show();
        }
    },

    this.SaveBillingInfo = function (containerClassName) {
        var containerId = this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "save_billinginfo")) {
            // now hide the editable fields:
            this.HideAllModals();
            this.HideMutableShowBusy();
        }
    },

    this.SaveShippingGetNextPage = function (containerClassName) {
        var containerId = this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "save_shippinginfo_getnextpage")) {
            this.HideMutableShowBusy();
        }
    },

    this.SaveShippingInfo = function (containerClassName) {
        var containerId = this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "save_shippinginfo")) {
            // now hide the editable fields:
            this.HideAllModals();
            this.HideMutableShowBusy();
        }
    },

    this.EditAddress = function (addressId) {
        if (addressId > 0) {
            this.HideMutableShowBusy();
            this.DoAjaxCallback("__opcode=editaddress" + addressId.toString(), "");
        }
    },

    this.SaveAddressInfoAndSelect = function (containerClassName) {
        var containerId = this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "update_shippingaddress")) {
            // now hide the editable fields:
            this.HideAllModals();
            this.HideMutableShowBusy();
        }
    },

    this.SaveAddressInfoAndReturn = function (containerClassName) {
        var containerId = this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "return_shippingaddress")) {
            // now hide the editable fields:
            this.HideAllModals();
            this.HideMutableShowBusy();
        }
    },

    this.CancelAddressInfoAndReturn = function () {
        this.HideMutableShowBusy();
        this.DoAjaxCallback("__opcode=cancelupdate_shippingaddress", "");
    },

    this.NewCustomerCheckout = function () {
        this.HideMutableShowBusyContainer('.EktronCheckout_NewCustomer');
        this.DoAjaxCallback("__opcode=nextpage", "");
    },

    this.GuestCheckout = function () {
        this.HideMutableShowBusyContainer('.EktronCheckout_GuestCustomer');
        this.DoAjaxCallback("__opcode=guestcheckout", "");
    },

    this.SaveInfoToServer = function (containerId, opcode) {
        // validate user input; if failure then show error, and return:
        if (!this.ValidateUserFields(containerId))
            return (false);

        // serialize user input fields:
        var parms = "";
        var name = "";
        var nameParts;
        var inputObjects = $ektron(containerId + " .EktronCheckout_SerializableContainer input");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            if (("undefined" != typeof inputObjects[idx].name)
                && ("undefined" != typeof inputObjects[idx].value)) {
                nameParts = inputObjects[idx].name.split("_");
                name = "param__" + nameParts[2].toLowerCase();
                switch (inputObjects[idx].type) {
                    case "radio":
                        if (inputObjects[idx].checked)
                            parms += "&" + name + "=" + inputObjects[idx].value;
                        break;

                    case "checkbox":
                        parms += "&" + name + "=" + inputObjects[idx].checked.toString();
                        break;

                    default:
                        parms += "&" + name + "=" + encodeURI(inputObjects[idx].value);
                        break;
                }
            }
        }

        inputObjects = $ektron(containerId + " .EktronCheckout_SerializableContainer select");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            if (("undefined" != typeof inputObjects[idx].name)
                && ("undefined" != typeof inputObjects[idx].value)) {
                nameParts = inputObjects[idx].name.split("_");
                name = "param__" + nameParts[2].toLowerCase();
                parms += "&" + name + "=" + inputObjects[idx].value;
            }
        }

        this.DoAjaxCallback("__opcode=" + opcode + parms, "");
        return (true);
    },

    this.GetLocalizedString = function (key) {
        var result = $ektron("#EktronCheckout_" + this.GetId()
            + "_" + key).val();
        return (result != null ? result : "");
    },

    this.SkipValidation = function (obj) {
        return ($ektron(obj).hasClass("skip_validation"));
    },

    this.ValidateUserFields = function (containerId) {
        
        var result = true;
        var parms = "";
        var name = "";
        var nameParts;
        var inputObjects = null;
        var subObj = null;
        var failedText = "";
        var failedTextMessage = this.GetLocalizedString("jsEnterValueForRequiredField");

        // clear previous validation error flags:
        $ektron(containerId + " .EktronCheckout_Required").removeClass("EktronCheckout_Required_FailedVerification");
        $ektron(containerId + " .EktronCheckout_RequiredNotice").removeClass("EktronCheckout_RequiredNotice_FailedVerification");

        

        // test for select still at default option:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("select");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if ((subObj[innerIdx].type.indexOf("select") >= 0) && !this.SkipValidation(subObj[innerIdx]) && ("0" == subObj[innerIdx].value)) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // test for empty strings:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("text" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && ("" == subObj[innerIdx].value)) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // test for numeric values:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input.EktronCheckout_NumericField");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("text" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && !(-1 < subObj[innerIdx].value)) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsEnterNumericValueForRequiredField");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // test telephone fields:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input.EktronCheckout_TelephoneField");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("text" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && !this.ValidatePhoneNumber(subObj[innerIdx].value)) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsEnterValidTelephoneNumberForRequiredField");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // test postal fields:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input.EktronCheckout_PostalField");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("text" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && !this.ValidatePostalCode(subObj[innerIdx].value)) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsEnterValidValueForRequiredField");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // validate email fields:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input.EktronCheckout_EmailAddressField");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("text" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && !this.ValidateEmailAddress(subObj[innerIdx].value)) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsEnterValidEmailAddressForRequiredField");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // validate minimum-length fields:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input[" + Ektron_Ecommerce_CheckoutClass.validateMinimumLength + "]");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("text" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && !this.ValidateMinimumLength(subObj[innerIdx].value, subObj[innerIdx].getAttribute(Ektron_Ecommerce_CheckoutClass.validateMinimumLength))) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsInsufficientCharactersForRequiredField");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // validate maximum-length fields:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input[" + Ektron_Ecommerce_CheckoutClass.validateMaximumLength + "]");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("text" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && !this.ValidateMaximumLength(subObj[innerIdx].value, subObj[innerIdx].getAttribute(Ektron_Ecommerce_CheckoutClass.validateMaximumLength))) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsTooManyCharactersForRequiredField");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // validate regex fields:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input[data-ektron-validation-regex]");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("text" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && !this.ValidateRegexField(subObj[innerIdx].value, subObj[innerIdx].getAttribute(Ektron_Ecommerce_CheckoutClass.validationRegex))) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsEnterValidValueForRequiredField");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // test for empty password fields:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required");
        for (var idx = 0; idx < inputObjects.length; idx++) {
            subObj = inputObjects.eq(idx).find("input");
            for (var innerIdx = 0; innerIdx < subObj.length; innerIdx++) {
                if (("password" == subObj[innerIdx].type) && !this.SkipValidation(subObj[innerIdx]) && ("" == subObj[innerIdx].value)) {
                    inputObjects.eq(idx).addClass("EktronCheckout_Required_FailedVerification");
                    result = false;
                    if (failedText.length == 0) {
                        try {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                        }
                        catch (e) { }
                    }
                }
            }
        }

        // verify matching passwords:
        inputObjects = $ektron(containerId + " .EktronCheckout_Required input:password");
        if (inputObjects && (2 == inputObjects.length)) {
            if (inputObjects[0].value != inputObjects[1].value) {
                result = false;
                if (failedText.length == 0) {
                    failedTextMessage = "";
                    failedText = this.GetLocalizedString("jsPasswordsDontMatch");
                }
            }
        }
        
        if (!result) {
            $ektron(containerId + " .EktronCheckout_RequiredNotice").addClass("EktronCheckout_RequiredNotice_FailedVerification");
            if (failedText) {
                alert(failedTextMessage + "'" + failedText + "'");
            }
        }


        return (result);
    },

    this.CountryIsUsa = function () {
        return ("840" == $ektron(".EktronCheckout_CountrySelect").val());
    },

    this.ValidatePhoneNumber = function (phoneText) {
        if (this.CountryIsUsa()) {
            var regEx = /^((\+\d{1,3}(-|.| )?\(?\d\)?(-|.| )?\d{1,5})|(\(?\d{2,6}\)?))(-|.| )?(\d{3,4})(-|.| )?(\d{4})(( x| ext)\d{1,5}){0,1}$/;
            return (regEx.test(phoneText));
        }
        else {
            return (phoneText.length > 0);
        }
    },

    this.ValidatePostalCode = function (postalCodeText) {
        if (this.CountryIsUsa()) {
            var regEx = /^\d{5}(-\d{4})?$/;
            return (regEx.test(postalCodeText));
        }
        else {
            return (postalCodeText.length > 0);
        }
    },

    this.ValidateEmailAddress = function (emailText) {
        var regEx = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*\.(\w{2}|(com|net|org|edu|int|mil|gov|arpa|biz|aero|name|coop|info|pro|museum))$/;
        return (regEx.test(emailText));
    },

    this.ValidateMinimumLength = function (value, minLength) {
        if (minLength && minLength.length > 0) {
            var minLen = parseInt(minLength, 10);
            return (value.length >= minLen);
        }
        return true;
    },

    this.ValidateMaximumLength = function (value, maxLength) {
        if (maxLength && maxLength.length > 0) {
            var maxLen = parseInt(maxLength, 10);
            return (value.length <= maxLen);
        }
        return true;
    },

    this.ValidateRegexField = function (value, validationRegEx) {
        if (validationRegEx && validationRegEx.length > 0) {
            // because it is passed as a string, need to remove any leading and trailing forward slashes:
            var valRegEx = validationRegEx;
            if (valRegEx.charAt(0) == "/")
            { valRegEx = valRegEx.substring(1); }
            if (valRegEx.length > 0 && valRegEx.charAt(valRegEx.length - 1) == "/")
            { valRegEx = valRegEx.substring(0, valRegEx.length - 1); }

            var re = new RegExp(valRegEx)
            return (re.test(value));
        }
        return true;
    },

    this.SaveShippingMethod_GetNextPage = function (containerClassName) {
        var containerId = "#EktronCheckout_" + this.GetId()
            + this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "save_shipmethod_nextpage")) {
            this.HideMutableShowBusy();
        }
    },

    this.SaveShippingMethod_GetPreviousPage = function (containerClassName) {
        var containerId = "#EktronCheckout_" + this.GetId() + this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "save_shipmethod_prevpage")) {
            this.HideMutableShowBusy();
        }
    },

    this.SubmitCheckoutOrder = function (containerClassName) {

        var doSubmit = true;
        var paymentType = $ektron("#EktronCheckout_SubmitOrder_PaymentTypeSelect_" + this.GetId().toString()).val();

        if (paymentType == "2") {

            var payPalPayerId = $ektron("#EktronCheckout_SubmitOrder_Token_" + this.GetId().toString()).val();
            var payPalToken = $ektron("#EktronCheckout_SubmitOrder_PayerId_" + this.GetId().toString()).val();

            if (payPalPayerId == "" || payPalToken == "") { doSubmit = false; window.location = this._appPath + "commerce/Checkout/PayPal/ExpressCheckout.aspx?checkout=true&phase=commit&returnURL=" + escape(document.URL); }

        }

        if (doSubmit) {
            var containerId = "#EktronCheckout_" + this.GetId()
                + this.MakeClassSelector(containerClassName);

            if (this.SaveInfoToServer(containerId, "submitcustomerorder")) {
                this.HideMutableShowBusy();
            }
        }
    },

    this.Cancel = function () {
        // restore previous values if pertinent:
        if ("undefined" != typeof this.OnCancel_CheckboxObject
            && null != this.OnCancel_CheckboxObject) {
            if (this.OnCancel_CheckboxInitialState != this.OnCancel_CheckboxObject.checked) {
                this.OnCancel_CheckboxObject.checked = this.OnCancel_CheckboxInitialState;
                this.ShippingIsBillingAddress(this.OnCancel_CheckboxObject, this.OnCancel_CheckboxContainerClassName);
                //this.OnCancel_CheckboxObject.click(); 
            }

            this.OnCancel_CheckboxObject = null;
            this.OnCancel_CheckboxInitialState = false;
            this.OnCancel_CheckboxContainerClassName = ""
        }

        // now hode the modals:
        this.HideAllModals();
    },

    this.CountryChanged = function (countryCtlId, regionCtlId) {
        var countrySel = $ektron("#" + countryCtlId);

        if (countrySel && countrySel[0] && countrySel[0].selectedIndex > 0) {
            var countryId = countrySel.children("option:selected").val();
            $ektron("#" + regionCtlId).attr("disabled", "disabled");

            // TODO: Store regions for each country on client side, so don't have to hit the server for countries that have already processed.
            //...
            // does region object for the previous country id exist?
            // save if not:
            //...
            // does region object exist for the newly selected country id?
            // load it if so:
            //...
            // otherwise:
            // replace select control with ajax image:
            // begin ajax callback for regions:
            //...

            this.DoAjaxCallback("__opcode=getsubfragment_regions&param__containerid=" + regionCtlId + "&param__countryid=" + countryId.toString(), "");
            return (true);
        }
        return (true);
    },

    this.SelectShippingAddress = function (addressId) {
        var addSel = $ektron("#EktronCheckout_" + this.GetId() + " #EktronCheckout_ShippingInfo_MultipleAddressSelect_" + this.GetId());
        if (addSel || addressId > 0) {
            if (addressId == 0)
                addressId = addSel.children("option:selected").val();
            this.HideMutableShowBusy();
            this.DoAjaxCallback("__opcode=select_shippingaddress&param__addressid=" + addressId.toString(), "");
            return (true);
        }
        return (true);
    },

    this.SaveNewAddress = function (containerClassName) {
        var containerId = this.MakeClassSelector(containerClassName);
        if (this.SaveInfoToServer(containerId, "add_shippingaddress")) {
            // now hide the editable fields:
            this.HideAllModals();
            this.HideMutableShowBusy();
        }
    },

    this.DeleteAddress = function (addressId) {
        if (addressId > 0 && confirm("Are you sure you want to delete this address?")) {
            this.HideMutableShowBusy();
            this.DoAjaxCallback("__opcode=delete_shippingaddress&param__addressid=" + addressId.toString(), "");
            return (true);
        }
        return (true);
    },

    ///////////////////////////////////
    // PaymentTypeChanged: Update the corresponding field group.
    this.PaymentTypeChanged = function () {
        var paymentType = $ektron("#EktronCheckout_SubmitOrder_PaymentTypeSelect_" + this.GetId().toString()).val();

        // determine selected type:
        //   0 == credit card
        //   1 == check
        //   2 == paypal
        //   3 == google
        if (paymentType == "1") {
            // show check related fields
            $ektron("div .EktronCheckout_Check").removeClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_Check .EktronCheckout_Required_Hidden").removeClass("EktronCheckout_Required_Hidden").addClass("EktronCheckout_Required");

            // hide credit card related fields
            $ektron("div .EktronCheckout_CreditCard").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_CreditCard .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

            // hide paypal related fields
            $ektron("div .EktronCheckout_PayPal").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_PayPal .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

            // hide google related fields
            $ektron("div .EktronCheckout_Google").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_Google .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");
        } else if (paymentType == "2") {
            // hide check related fields
            $ektron("div .EktronCheckout_Check").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_Check .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

            // hide credit card related fields
            $ektron("div .EktronCheckout_CreditCard").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_CreditCard .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

            // show paypal related fields
            $ektron("div .EktronCheckout_PayPal").removeClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_PayPal .EktronCheckout_Required").removeClass("EktronCheckout_Required_Hidden").addClass("EktronCheckout_Required");

            // hide google related fields
            $ektron("div .EktronCheckout_Google").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_Google .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");
        } else if (paymentType == "3") {
            // hide check related fields
            $ektron("div .EktronCheckout_Check").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_Check .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

            // hide credit card related fields
            $ektron("div .EktronCheckout_CreditCard").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_CreditCard .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

            // hide paypal related fields
            $ektron("div .EktronCheckout_PayPal").removeClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_PayPal .EktronCheckout_Required").removeClass("EktronCheckout_Required_Hidden").addClass("EktronCheckout_Required");

            // hide google related fields
            $ektron("div .EktronCheckout_Google").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_Google .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");
        } else {
            // hide check related fields
            $ektron("div .EktronCheckout_Check").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_Check .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

            // show credit card related fields
            $ektron("div .EktronCheckout_CreditCard").removeClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_CreditCard .EktronCheckout_Required_Hidden").removeClass("EktronCheckout_Required_Hidden").addClass("EktronCheckout_Required");

            // hide paypal related fields
            $ektron("div .EktronCheckout_PayPal").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_PayPal .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

            // hide google related fields
            $ektron("div .EktronCheckout_Google").addClass("EktronCheckout_Hidden");
            $ektron("div .EktronCheckout_Google .EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");
        }

        //$ektron("div .EktronCheckout_Check")
    },

    ///////////////////////////////////
    // CardTypeChanged: Update the corresponding input fields regex value.
    this.CardTypeChanged = function (cardTypeSelectId, cardNumberInputId) {
        var cardNumberInput = $ektron("#" + cardNumberInputId)[0];
        if (cardNumberInput) {
            cardNumberInput.removeAttribute(Ektron_Ecommerce_CheckoutClass.validationRegex);
            var cardTypeSelect = $ektron("#" + cardTypeSelectId)[0];
            if (cardTypeSelect && cardTypeSelect.selectedIndex > 0) {
                var validationRegEx = cardTypeSelect[cardTypeSelect.selectedIndex].getAttribute(Ektron_Ecommerce_CheckoutClass.cardRegex)
                if (validationRegEx && validationRegEx.length > 0) {
                    cardNumberInput.setAttribute(Ektron_Ecommerce_CheckoutClass.validationRegex, validationRegEx);
                }
            }
        }

        // hide or show various card fields:
        var cardType = cardTypeSelect[cardTypeSelect.selectedIndex].getAttribute("data-ektron-cardType");
        if (null == cardType)
            return;

        var containerId = "#EktronCheckout_" + this.GetId();
        switch (cardType.toLowerCase()) {
            case "solo":
            case "maestro":
                $ektron(containerId + " .EktronCheckout_SoloCard.EktronCheckout_Required_Hidden").removeClass("EktronCheckout_Required_Hidden").addClass("EktronCheckout_Required");
                $ektron(containerId + " .EktronCheckout_MaestroCard.EktronCheckout_Required_Hidden").removeClass("EktronCheckout_Required_Hidden").addClass("EktronCheckout_Required");

                $ektron(containerId + " .EktronCheckout_SoloCard").removeClass("EktronCheckout_Hidden");
                $ektron(containerId + " .EktronCheckout_MaestroCard").removeClass("EktronCheckout_Hidden");
                break;

            default:
                $ektron(containerId + " .EktronCheckout_SoloCard.EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");
                $ektron(containerId + " .EktronCheckout_MaestroCard.EktronCheckout_Required").removeClass("EktronCheckout_Required").addClass("EktronCheckout_Required_Hidden");

                $ektron(containerId + " .EktronCheckout_SoloCard").addClass("EktronCheckout_Hidden");
                $ektron(containerId + " .EktronCheckout_MaestroCard").addClass("EktronCheckout_Hidden");
        }
    },

    ///////////////////////////////////
    // When called like onkeypress='FilterNonNumerics(event)' will 
    // only allow numbers to be entered into input field.
    // Note: For editing purposes, does allow control codes (e.g. backspace, 
    // left-arrow, delete, etc.) to be passed through for editing:
	this.FilterSpecialCharacters = function (eventObj) {
	    if (eventObj) {
	        var charKey = 0;
	        var ctrl = 0;
	        var ctrlKey = false;

	        // IE doesn't pass control-codes, only standard keys (seems to handle internally):
	        if ("undefined" == typeof eventObj.charCode
                && "undefined" != typeof eventObj.keyCode)
	        { charKey = eventObj.keyCode; }

	        // FireFox passes normal chars in charCode, control chars in keyCode:
	        if ("undefined" != typeof eventObj.charCode
                && "undefined" != typeof eventObj.keyCode) {
	            ctrl = eventObj.keyCode;
	            charKey = eventObj.charCode;
	        }

	        if ("undefined" != typeof eventObj.ctrlKey)
	        { ctrlKey = eventObj.ctrlKey }

	        //if ((charKey > 47 && charKey < 58) // numbers
	        //    || (ctrlKey && 118 == charKey) // <ctrl>-v (paste)
	        //    || (8 == ctrl) // backspace
	        //    || (46 == ctrl) // delete
	        //    || (9 == ctrl) // tab
	        //    || (ctrl > 36 && ctrl < 41) ) // arrow-keys
	        //    { return (true); } // accept keypress (pass on to control).
	    }
	    return (true); // eat keypress.
	},

    ///////////////////////////////////
    // When called like onkeypress='FilterNonNumerics(event)' will 
    // only allow numbers to be entered into input field.
    // Note: For editing purposes, does allow control codes (e.g. backspace, 
    // left-arrow, delete, etc.) to be passed through for editing:
	this.FilterNonNumerics = function (eventObj) {
	    if (eventObj) {
	        var charKey = 0;
	        var ctrl = 0;
	        var ctrlKey = false;

	        // IE doesn't pass control-codes, only standard keys (seems to handle internally):
	        if ("undefined" == typeof eventObj.charCode
                && "undefined" != typeof eventObj.keyCode)
	        { charKey = eventObj.keyCode; }

	        // FireFox passes normal chars in charCode, control chars in keyCode:
	        if ("undefined" != typeof eventObj.charCode
                && "undefined" != typeof eventObj.keyCode) {
	            ctrl = eventObj.keyCode;
	            charKey = eventObj.charCode;
	        }

	        if ("undefined" != typeof eventObj.ctrlKey)
	        { ctrlKey = eventObj.ctrlKey }

	        if ((charKey > 47 && charKey < 58) // numbers
                || (ctrlKey && 118 == charKey) // <ctrl>-v (paste)
                || (8 == ctrl) // backspace
                || (46 == ctrl) // delete
                || (9 == ctrl) // tab
                || (ctrl > 36 && ctrl < 41)) // arrow-keys
	        { return (true); } // accept keypress (pass on to control).
	    }
	    return (false); // eat keypress.
	},

	this.SetValidationTestSkip = function (obj, enableValidation, disableValidation) {
	    var containerId = "#EktronCheckout_" + this.GetId();
	    $ektron(containerId + " " + enableValidation).removeClass("skip_validation");
	    $ektron(containerId + " " + disableValidation + " input:text").addClass("skip_validation");
	    $ektron(containerId + " " + disableValidation + " select").addClass("skip_validation");
	},

    this.MakeClassSelector = function (rawSelector) {
        var result = " "
            + (($ektron.trim(rawSelector).indexOf(".") != 0) ? "." : "")
            + $ektron.trim(rawSelector);
        return (result);
    }

    ///////////////////////////////////
    // Call Constructor at creation time:
    this.Constructor();
} //  end of instance-level Ektron_Ecommerce_CheckoutClass class definition.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
// Ektron_Ecommerce_CheckoutClass Static members (variables and constants):
// 
// enableLogging, true allows messages to be logged:
Ektron_Ecommerce_CheckoutClass.enableLogging = false;
// enableShowLog, true exposes the messages in a simple div-block in the UI:
Ektron_Ecommerce_CheckoutClass.enableShowLog = false;
// enableMsgPrefix, true prefixes each message with this class name:
Ektron_Ecommerce_CheckoutClass.enableMsgPrefix = false;
// enableModalCentering, true to place the modal centered above the checkout control.
Ektron_Ecommerce_CheckoutClass.enableModalCentering = true;
//
Ektron_Ecommerce_CheckoutClass.cardRegex = "data-ektron-cardRegex";
Ektron_Ecommerce_CheckoutClass.validationRegex = "data-ektron-validation-regex";
Ektron_Ecommerce_CheckoutClass.validateMinimumLength = "data-ektron-validation-minimumLength";
Ektron_Ecommerce_CheckoutClass.validateMaximumLength = "data-ektron-validation-maximumLength";
