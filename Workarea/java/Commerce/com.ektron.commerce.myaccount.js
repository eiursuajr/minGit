// Class Ektron_Ecommerce_MyAccountClass:
function Ektron_Ecommerce_MyAccountClass(id)
{
    ///////////////////////////////////////////////////////
    // Instance members - variables:
    this._id = id;
    this._ajaxCall = null;
    this._ajaxCallbackId = "";
    this._readyFlag = false;
    this._browserInitDelay = 100;
    this._retryDelay = 50;
    this._ajaxImage = "";
    this._imagePath = "";
    this._blockedItems = [];

    ///////////////////////////////////////////////////////
    // Instance members - methods:


    ///////////////////////////////////
    // Initialize: Called when this object is first created.
    this.Constructor = function()
    {
        // Initialize state here:
        // ...
        var myaccountObjId = this.GetId();
        //$ektron(document).load(function(){Ektron_Ecommerce_MyAccountClass.GetObject(myaccountObjId).Initialize()});
        setTimeout(function() { Ektron_Ecommerce_MyAccountClass.GetObject(myaccountObjId).Initialize(); }, this._browserInitDelay);
    };

    ///////////////////////////////////
    // Initialize: 
    //
    this.Initialize = function()
    {
        this.InitializeUI();
    };

    ///////////////////////////////////
    // GetId: Returns the client-id associated with the server control.
    this.GetId = function()
    {
        return (this._id);
    };

    ///////////////////////////////////
    //
    this.IsReady = function()
    {
        return (this._readyFlag);
    };

    ///////////////////////////////////
    //
    this.SetAjaxCall = function(ptr)
    {
        this._ajaxCall = ptr;
    };

    ///////////////////////////////////
    //
    this.GetAjaxCall = function()
    {
        return (this._ajaxCall);
    };

    ///////////////////////////////////
    //
    this.SetAjaxCallbackId = function(id)
    {
        this._ajaxCallbackId = id;
    };

    ///////////////////////////////////
    //
    this.GetAjaxCallbackId = function()
    {
        return (this._ajaxCallbackId);
    };

    ///////////////////////////////////
    //
    this.SetAjaxImage = function(src)
    {
        this._ajaxImage = src;
    };

    ///////////////////////////////////
    //
    this.SetImagePath = function(path)
    {
        this._imagePath = path;
    };


    ///////////////////////////////////
    //
    this.DoAjaxCallback = function(args, context)
    {
        var targObj = null;

        // Show Ajax-loading image if specified:
        if (this._ajaxImage.length > 0)
        {
            this.ShowAjaxWorking();
        }

        // Do Ajax-Callback to server:
        var fPtr = this.GetAjaxCall();
        if (("undefined" != typeof fPtr) && (null !== fPtr))
        {
            // Use MS Ajax client side code:
            (fPtr)(this.GetId(), args, this.GetId().toString());
        }
        else
        {
            // Do not use MS Ajax client side code:
            var ektronPostParams = "";
            ektronPostParams += "&__CALLBACKID=" + this.GetAjaxCallbackId(); // Must use ASP.NET UniqueID as that's what's expected on server side when control is used with master pages.
            ektronPostParams += "&__CALLBACKPARAM=" + escape(args);
            ektronPostParams += "&__VIEWSTATE="; // + encodeURIComponent($ektron("#__VIEWSTATE").attr("value"));
            context = this.GetId().toString();
            var ajaxUrl = window.location.href.replace(window.location.hash, ""); // IE/Windows will block callbacks with a pound/hash symbol in the URL.
            $ektron.ajax({
                type: "POST",
                url: ajaxUrl,
                data: ektronPostParams,
                dataType: "html",
                error: function(ektronCallbackResult) { Ektron_Ecommerce_MyAccountClass.GetObject(context).AjaxCallback_DisplayError(ektronCallbackResult, context); },
                success: function(ektronCallbackResult)
                {
                    var PACKET_START = "EKTRON_PACKET_START|";
                    var payload = String(ektronCallbackResult);
                    var idx = payload.indexOf(PACKET_START);
                    if (idx >= 0) {
                        var payload = payload.substring(idx + PACKET_START.length);
                    }
                    Ektron_Ecommerce_MyAccountClass.GetObject(context).AjaxCallback_DisplayResult(payload, context);
                }
            });
        }

        return (false); // cancel events.
    };

    ///////////////////////////////////
    //
    this.AjaxCallback_DisplayResult = function(args, context)
    {
        if (args && args.length)
        {
            var checkOutObj = Ektron_Ecommerce_MyAccountClass.GetObject(context, false);
            var responseBlocks = args.split("|");
            if (responseBlocks && responseBlocks.length)
            {
                var resStatus = responseBlocks[0];
                if ("1" == resStatus)
                {
                    var jsonLength = parseInt(responseBlocks[1], 10);
                    var markupLength = parseInt(responseBlocks[2], 10);
                    var markupTargetId = responseBlocks[3];
                    var unblock = responseBlocks[4];
                    var json = "";
                    var markup = "";
                    var jsonPayloadOffset = (responseBlocks[0].length + 1 + responseBlocks[1].length + 1 + responseBlocks[2].length + 1 + responseBlocks[3].length + 1 + responseBlocks[4].length + 1);
                    if ("1" == unblock)
                    {
                        if (checkOutObj)
                        {
                            checkOutObj.UnBlockCurrentItems();
                        }
                    }
                    if (jsonLength)
                    {
                        json = args.substr(jsonPayloadOffset, jsonLength);
                        eval(json);
                    }
                    var marupPayloadOffset = jsonPayloadOffset + jsonLength;
                    if (markupLength)
                    {
                        markup = args.substr(marupPayloadOffset, markupLength);
                        if (markupTargetId.length > 0)
                        {
                            $ektron(markupTargetId).html(markup);
                        }
                    }
                }
            }
        }
    };

    ///////////////////////////////////
    //
    this.AjaxCallback_DisplayError = function(args, context)
    {
        if (context && context.length > 0)
        {
            var checkOutObj = Ektron_Ecommerce_MyAccountClass.GetObject(context, false);
            if (checkOutObj)
            {
                checkOutObj.UnBlockCurrentItems();
            }
        }
        var statusCode = "";
        var statusText = "";
        if (args && args.status && args.status.toString().length > 0)
        {
            statusCode = "Status: " + args.status.toString();
        }
        if (args && args.statusText && args.statusText.length > 0)
        {
            statusText = " " + args.statusText;
        }
        var errorDetails = "";
        if ((statusCode.length > 0) || (statusText.length > 0))
        {
            errorDetails = " Details: " + statusCode + ((statusCode.length > 0) ? ", " : "") + statusText;
        }
        Ektron_Ecommerce_MyAccountClass.LogIt('Error Occurred in Ektron_Ecommerce_MyAccountClass: Ajax-Error!' + errorDetails);
    };

    ///////////////////////////////////
    //
    this.Replace = function(str, findstr, replacestr)
    {
        var ret = str;
        if (ret != '')
        {
            var index = ret.indexOf(findstr);
            while (index >= 0)
            {
                ret = ret.replace(findstr, replacestr);
                index = ret.indexOf(findstr);
            }
        }
        return (ret);
    };

    ///////////////////////////////////
    // This prevents the browser from caching/auto-completing fields (which would overwrite ajax callback values on page refresh).
    this.DisableInputAutocomplete = function()
    {
        $ektron("#EktronMyAccountCtl_myaccountContainer_" + this.GetId() + " input[type='text']").attr("autocomplete", "off");
    };

    ///////////////////////////////////
    // 
	this.InitializeUI = function()
	{
	    this._readyFlag = true;
	    this.DisableInputAutocomplete();
	};

    ///////////////////////////////////
    //
	this.ShowAjaxWorking = function()
	{
	    // handled elsewhere:
	    //    var containerObj = document.getElementById("_ResultsContainer_" + this.GetId());
	    //    if (containerObj){
	    //        containerObj.innerHTML = '<span class="_ResultLoading" >  <img src="' + this._ajaxImage + '" />  </span> ';
	    //        containerObj.style.display = "block";
	    //    }
	};

    this.UnBlockCurrentItems = function()
    {
    };

    this.Login = function()
    {
        var emText = $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_LoginBlock .EktronMyAccount_LoginEmail input:text")[0].value;
        var codeText = $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_LoginBlock .EktronMyAccount_LoginPassword input:password")[0].value;
        var hdnElm = $ektron("#EktronMyAccount_" + this.GetId() + " #EktronMyAccount_LoginBlock_Hidden_" + this.GetId());
        if (hdnElm && hdnElm[0] && (emText.length > 0) && (codeText.length > 0))
        {
            hdnElm[0].value = "__opcode=logcustomerin&param__email=" + emText + "&param__code=" + codeText;
            // do postback:
            return (true);
        }
        else
        {
            return (false);
        }
    };

    this.HideMutableShowBusy = function()
    {
        // hide mutable controls and show busy image:
        $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_MutableControlContainer").children().hide();
        $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_AjaxBusyImageContainer").show();
    };

    this.ShowMutableClearBusy = function()
    {
        // hide mutable controls and show busy image:
        $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_MutableControlContainer").children().show();
        $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_AjaxBusyImageContainer").hide();
    };

    this.NextPage = function()
    {
        this.HideMutableShowBusy();
        this.DoAjaxCallback("__opcode=nextpage", "");
    };

    this.PreviousPage = function(containerClassName)
    {
        this.HideMutableShowBusy();
        this.DoAjaxCallback("__opcode=prevpage", "");
    };

    this.Edit = function(editContainerClassName)
    {
        $ektron("#EktronMyAccount_" + this.GetId() + " .viewBlock").hide();
        $ektron("#EktronMyAccount_" + this.GetId() + " ." + editContainerClassName).show();
    };

    this.SaveInfo = function(containerClassName, opcode)
    {
        var containerId = "#EktronMyAccount_" + this.GetId() + " ." + containerClassName;
        if (this.SaveInfoToServer(containerId, opcode))
        {
            this.HideMutableShowBusy();
        }
    };

    this.ShippingIsBillingAddress = function(evtObj, containerClassName)
    {
        var containerId = "#EktronMyAccount_" + this.GetId() + " ." + containerClassName;
        var containerObject = $ektron(containerId);
        if (evtObj && containerObject)
        {
            if (evtObj.checked)
            {
                // same as billing, these fields not needed:
                containerObject.find(".EktronMyAccount_Required").removeClass("EktronMyAccount_Required").addClass("EktronMyAccount_Required_Disabled");
                containerObject.find(".EktronMyAccount_MakeRow_Container").addClass("EktronMyAccount_Row_Disabled");
                $ektron(containerId + " input:text").attr("disabled", "disabled");
                $ektron(containerId + " select").attr("disabled", "disabled");
            }
            else
            {
                // not the same as billing, mark fields as needed:
                containerObject.find(".EktronMyAccount_Required_Disabled").removeClass("EktronMyAccount_Required_Disabled").addClass("EktronMyAccount_Required");
                containerObject.find(".EktronMyAccount_Row_Disabled").removeClass("EktronMyAccount_Row_Disabled");
                $ektron(containerId + " input:text").removeAttr("disabled");
                $ektron(containerId + " select").removeAttr("disabled");
            }
        }
    };

    this.SaveInfoToServer = function(containerId, opcode)
    {
        // validate user input; if failure then show error, and return:
        if (!this.ValidateUserFields(containerId))
        {
            return (false);
        }
        // serialize user input fields:
        var parms = "";
        var name = "";
        var nameParts;
        var inputObjects = $ektron(containerId + " .EktronMyAccount_SerializableContainer input");
        var idx;
        for (idx = 0; idx < inputObjects.length; idx++)
        {
            if (("undefined" != typeof inputObjects[idx].name) && ("undefined" != typeof inputObjects[idx].value))
            {
                nameParts = inputObjects[idx].name.split("_");
                name = "param__" + nameParts[2].toLowerCase();
                switch (inputObjects[idx].type)
                {
                    case "radio":
                        if (inputObjects[idx].checked)
                        {
                            parms += "&" + name + "=" + inputObjects[idx].value;
                        }
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

        inputObjects = $ektron(containerId + " .EktronMyAccount_SerializableContainer select");
        for (idx = 0; idx < inputObjects.length; idx++)
        {
            if (("undefined" != typeof inputObjects[idx].name) && ("undefined" != typeof inputObjects[idx].value))
            {
                nameParts = inputObjects[idx].name.split("_");
                name = "param__" + nameParts[2].toLowerCase();
                parms += "&" + name + "=" + inputObjects[idx].value;
            }
        }

        this.DoAjaxCallback("__opcode=" + opcode + parms, "");
        return (true);
    };

    // clears any existing validation error flags:
    this.ClearValidationErrors = function(containerId)
    {
        if (null === containerId || "" == containerId)
        {
            containerId = "#EktronMyAccount_" + this.GetId();
        }
        $ektron(containerId + " .EktronMyAccount_Required").removeClass("EktronMyAccount_Required_FailedVerification");
        $ektron(containerId + " .EktronMyAccount_RequiredNotice").removeClass("EktronMyAccount_RequiredNotice_FailedVerification");
    };

    this.GetLocalizedString = function(key)
    {
        var result = $ektron("#EktronMyAccount_" + this.GetId() + "_" + key).val();
        return (result !== null ? result : "");
    };

    this.ValidateUserFields = function(containerId)
    {
        var result = true;
        var parms = "";
        var name = "";
        var nameParts;
        var inputObjects = null;
        var subObj = null;
        var failedText = "";
        var failedTextMessage = this.GetLocalizedString("jsEnterValueForRequiredField");

        // clear previous validation error flags:
        this.ClearValidationErrors(containerId);

        // test for empty strings:
        inputObjects = $ektron(containerId + " .EktronMyAccount_Required");
        var idx;
        var innerIdx;
        for (idx = 0; idx < inputObjects.length; idx++)
        {
            subObj = inputObjects.eq(idx).find("input");
            for (innerIdx = 0; innerIdx < subObj.length; innerIdx++)
            {
                if (("text" == subObj[innerIdx].type) && ("" == subObj[innerIdx].value))
                {
                    inputObjects.eq(idx).addClass("EktronMyAccount_Required_FailedVerification");
                    result = false;
                    if (failedText.length === 0)
                    {
                        try
                        {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                        }
                        catch (e1) { }
                    }
                }
            }
        }

        // test for select still at default option:
        inputObjects = $ektron(containerId + " .EktronMyAccount_Required");
        for (idx = 0; idx < inputObjects.length; idx++)
        {
            subObj = inputObjects.eq(idx).find("select");
            for (innerIdx = 0; innerIdx < subObj.length; innerIdx++)
            {
                if ((subObj[innerIdx].type.indexOf("select") >= 0) && ("0" == subObj[innerIdx].value))
                {
                    inputObjects.eq(idx).addClass("EktronMyAccount_Required_FailedVerification");
                    result = false;
                    if (failedText.length === 0)
                    {
                        try
                        {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                        }
                        catch (e2) { }
                    }
                }
            }
        }

        // test for numeric values:
        inputObjects = $ektron(containerId + " .EktronMyAccount_Required");
        for (idx = 0; idx < inputObjects.length; idx++)
        {
            subObj = inputObjects.eq(idx).find("input.EktronMyAccount_NumericField");
            for (innerIdx = 0; innerIdx < subObj.length; innerIdx++)
            {
                if (("text" == subObj[innerIdx].type) && !this.ValidatePostalCode(subObj[innerIdx].value, containerId))
                {
                    inputObjects.eq(idx).addClass("EktronMyAccount_Required_FailedVerification");
                    result = false;
                    if (failedText.length === 0)
                    {
                        try
                        {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsEnterNumericValueForRequiredField");
                        }
                        catch (e3) { }
                    }
                }
            }
        }

        // test telephone fields:
        inputObjects = $ektron(containerId + " .EktronMyAccount_Required");
        for (idx = 0; idx < inputObjects.length; idx++)
        {
            subObj = inputObjects.eq(idx).find("input.EktronMyAccount_TelephoneField");
            for (innerIdx = 0; innerIdx < subObj.length; innerIdx++)
            {
                if (("text" == subObj[innerIdx].type) && !this.ValidatePhoneNumber(subObj[innerIdx].value))
                {
                    inputObjects.eq(idx).addClass("EktronMyAccount_Required_FailedVerification");
                    result = false;
                    if (failedText.length === 0)
                    {
                        try
                        {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsEnterValidTelephoneNumberForRequiredField");
                        }
                        catch (e4) { }
                    }
                }
            }
        }

        // validate email fields:
        inputObjects = $ektron(containerId + " .EktronMyAccount_Required");
        for (idx = 0; idx < inputObjects.length; idx++)
        {
            subObj = inputObjects.eq(idx).find("input.EktronMyAccount_EmailAddressField");
            for (innerIdx = 0; innerIdx < subObj.length; innerIdx++)
            {
                if (("text" == subObj[innerIdx].type) && !this.ValidateEmailAddress(subObj[innerIdx].value))
                {
                    inputObjects.eq(idx).addClass("EktronMyAccount_Required_FailedVerification");
                    result = false;
                    if (failedText.length === 0)
                    {
                        try
                        {
                            failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            failedTextMessage = this.GetLocalizedString("jsEnterValidEmailAddressForRequiredField");
                        }
                        catch (e5) { }
                    }
                }
            }
        }

        if (containerId.indexOf(" .EktronMyAccount_PersonalInfo_Edit") > 0)
        {
            // test for empty password fields:
            inputObjects = $ektron(containerId + " .EktronMyAccount_Required");
            for (idx = 0; idx < inputObjects.length; idx++)
            {
                subObj = inputObjects.eq(idx).find("input");
                for (innerIdx = 0; innerIdx < subObj.length; innerIdx++)
                {
                    if (("password" == subObj[innerIdx].type) && ("" == subObj[innerIdx].value))
                    {
                        inputObjects.eq(idx).addClass("EktronMyAccount_Required_FailedVerification");
                        result = false;
                        if (failedText.length === 0)
                        {
                            try
                            {
                                failedText = inputObjects.eq(idx).find("label").text().split(":")[0].replace("*", "");
                            }
                            catch (e6) { }
                        }
                    }
                }
            }
            // verify matching passwords:
            inputObjects = $ektron(containerId + " .EktronMyAccount_Required input:password");
            if (inputObjects && (2 == inputObjects.length))
            {
                if (inputObjects[0].value != inputObjects[1].value)
                {
                    result = false;
                    if (failedText.length === 0)
                    {
                        failedTextMessage = "";
                        failedText = this.GetLocalizedString("jsPasswordsDontMatch");
                    }
                }
            }
            // check for password validation from server:
            var passwordValidationString = $ektron("#EktronMyAccount_" + this.GetId() + " .passwordValidationString").val();
            if (failedText.length === 0 && passwordValidationString && passwordValidationString.length > 0)
            {
                inputObjects = $ektron(containerId + " .EktronMyAccount_Required");
                for (idx = 0; idx < inputObjects.length; idx++)
                {
                    subObj = inputObjects.eq(idx).find("input");
                    for (innerIdx = 0; innerIdx < subObj.length; innerIdx++)
                    {
                        if ("password" == subObj[innerIdx].type && "unchanged" != subObj[innerIdx].value)
                        {
                            var errorMsgs = this.ValidateRegExMsgArray(subObj[innerIdx].value, passwordValidationString);
                            if (errorMsgs.length > 0)
                            {
                                result = false;
                                failedTextMessage = "";
                                failedText = errorMsgs[0];
                                inputObjects.eq(idx).addClass("EktronMyAccount_Required_FailedVerification");
                            }
                            break;
                        }
                    }
                }
            }
        }

        if (!result)
        {
            $ektron(containerId + " .EktronMyAccount_RequiredNotice").addClass("EktronMyAccount_RequiredNotice_FailedVerification");
            if (failedText)
            {
                alert(failedTextMessage + "'" + failedText + "'");
            }
        }
        return (result);
    };

    this.ValidateRegExMsgArray = function(password, regexAndErrorMessages)
    {
        var errors = [];
        var regex, errorMessage;
        var parts = [];
        var raw = this.trimStart(regexAndErrorMessages, "[");
        raw = this.trimEnd(raw, "]");
        raw = raw.split("],[");

        for (var idx = 0; idx < raw.length; idx++)
        {
            parts = raw[idx].split("/,");
            regex = this.trimStart(parts[0], "/");

            errorMessage = this.trimStart(this.trimStart(parts[1], ' '), '\\"');
            errorMessage = this.trimEnd(errorMessage, '\\"');

            var re = new RegExp(regex);
            if (!re.test(password))
            {
                errors[errors.length] = errorMessage;
            }
        }
        return errors;
    };

    this.trimStart = function(text, toRemove)
    {
        if (text.indexOf(toRemove) === 0 && text.length >= toRemove.length)
        {
            return text.substr(toRemove.length);
        }
        return text;
    };

    this.trimEnd = function(text, toRemove)
    {
        if (text.length > toRemove.length && toRemove == text.substr(text.length - toRemove.length))
        {
            return text.substr(0, text.length - toRemove.length);
        }
        return text;
    };

    this.ValidatePhoneNumber = function(phoneText)
    {
        var regEx = /^((\+\d{1,3}(-|.| )?\(?\d\)?(-|.| )?\d{1,5})|(\(?\d{2,6}\)?))(-|.| )?(\d{3,4})(-|.| )?(\d{4})(( x| ext)\d{1,5}){0,1}$/;
        return (regEx.test(phoneText));
    };

    this.CountryIsUsa = function(containerId)
    {
        var selectObject = $ektron(containerId + " .EktronMyAccount_CountrySelect");
        return ("840" == selectObject.val());
    };

    this.ValidatePostalCode = function(postalCodeText, containerId)
    {
        if (this.CountryIsUsa(containerId))
        {
            var regEx = /^\d{5}(-\d{4})?$/;
            return (regEx.test(postalCodeText));
        }
        else
        {
            return (postalCodeText.length > 0);
        }
    };

    this.ValidateEmailAddress = function(phoneText)
    {
        var regEx = /^\w+([\.\-]?\w+)*@\w+([\.\-]?\w+)*\.(\w{2}|(com|net|org|edu|int|mil|gov|arpa|biz|aero|name|coop|info|pro|museum))$/;
        return (regEx.test(phoneText));
    };

    this.ValidateRegexField = function(value, validationRegEx)
    {
        if (validationRegEx && validationRegEx.length > 0)
        {
            // because it is passed as a string, need to remove any leading and trailing forward slashes:
            var valRegEx = validationRegEx;
            if (valRegEx.charAt(0) == "/")
            { valRegEx = valRegEx.substring(1); }
            if (valRegEx.length > 0 && valRegEx.charAt(valRegEx.length - 1) == "/")
            { valRegEx = valRegEx.substring(0, valRegEx.length - 1); }

            var re = new RegExp(valRegEx);
            return (re.test(value));
        }
        return true;
    };

    this.Cancel = function(containerClassName)
    {
        // clear any validation errors:
        this.ClearValidationErrors(containerClassName);

        $ektron("#EktronMyAccount_" + this.GetId() + " .editBlock").hide();
        $ektron("#EktronMyAccount_" + this.GetId() + " .viewBlock").show();
    };

    this.CountryChanged = function(countryCtlId, regionCtlId)
    {
        var countrySel = $ektron("#" + countryCtlId);

        if (countrySel && countrySel[0] && countrySel[0].selectedIndex > 0)
        {
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
    };

    this.SelectShippingAddress = function()
    {
        var addSel = $ektron("#EktronMyAccount_" + this.GetId() + " #EktronMyAccount_ShippingInfo_MultipleAddressSelect_" + this.GetId());
        if (addSel)
        {
            var addressId = addSel.children("option:selected").val();
            $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_ShippingAddressDetails").hide();
            this.HideMutableShowBusy();
            this.DoAjaxCallback("__opcode=select_shippingaddress&param__addressid=" + addressId.toString(), "");
            return (true);
        }
        return (true);
    };

    //
    this.AddNewAddress = function()
    {
        var addObj = $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_ShippingInfo_Add");

        // clear all input fields:
        addObj.find(" input[type='text']").val("");

        // clear all select controls, except for the country control:
        addObj.find(" select:not(.EktronMyAccount_CountrySelect)").val("0");

        // ensure all fields are enabled:
        addObj.find(".EktronMyAccount_Required_Disabled").removeClass("EktronMyAccount_Required_Disabled").addClass("EktronMyAccount_Required");
        addObj.find(".EktronMyAccount_Row_Disabled").removeClass("EktronMyAccount_Row_Disabled");
        addObj.find(" input:text").removeAttr("disabled");
        addObj.find(" select").removeAttr("disabled");

        // initialize default address checkbox:
        addObj.find("#EktronMyAccount_ShippingInfo_DefaultAddress_" + this.GetId()).removeAttr("disabled").removeAttr("checked");


        // hide the view-blocks, and show the add screen:
        $ektron("#EktronMyAccount_" + this.GetId() + " .viewBlock").hide();
        $ektron("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_ShippingInfo_Add").show();
    };

    this.SaveNewAddress = function()
    {
        //SaveShippingInfo
        if (this.SaveInfoToServer("#EktronMyAccount_" + this.GetId() + " .EktronMyAccount_ShippingInfo_Add", "add_shippingaddress"))
        {
            // now hide the buttons and show ajax image:
            this.HideMutableShowBusy();
        }
    };

    this.CancelAddNewAddress = function()
    {
        // clear any validation errors:
        this.ClearValidationErrors(" .EktronMyAccount_ShippingInfo_Add");

        // restore view screen:
        $ektron("#EktronMyAccount_" + this.GetId() + " .editBlock").hide();
        $ektron("#EktronMyAccount_" + this.GetId() + " .viewBlock").show();
    };

    this.DeleteAddress = function(addressId)
    {
        //Deletes the default shipping address
        if (addressId > 0 && confirm("Are you sure you want to delete this address?"))
        {
            this.HideMutableShowBusy();
            this.DoAjaxCallback("__opcode=delete_shippingaddress&param__addressid=" + addressId.toString(), "");
            return (true);
        }
        return (true);
    };

    ///////////////////////////////////
    // Call Constructor at creation time:
    this.Constructor();
} //  end of instance-level Ektron_Ecommerce_MyAccountClass class definition.
///////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////
// Ektron_Ecommerce_MyAccountClass Static members (variables and constants):
Ektron_Ecommerce_MyAccountClass.enableShowLog = false;

///////////////////////////////////////////////////////////
// Ektron_Ecommerce_MyAccountClass Static members (methods):

///////////////////////////////////
//
Ektron_Ecommerce_MyAccountClass.GetObject = function(id, creatable)
{
    var fullId = "MyAccountObj_" + id;
    var allowCreation = true;
    if ("undefined" != typeof creatable)
    {
        allowCreation = (true === creatable);
    }
    if (("undefined" != typeof window[fullId]) && (null !== window[fullId]))
    {
        return (window[fullId]);
    }
    else if (allowCreation)
    {
        var obj = new Ektron_Ecommerce_MyAccountClass(id);
        window[fullId] = obj;
        return (obj);
    }
};

///////////////////////////////////
//
Ektron_Ecommerce_MyAccountClass.IsReady = function(id)
{
    var obj = Ektron_Ecommerce_MyAccountClass.GetObject(id, false);
    if (obj)
    {
        return (obj.IsReady());
    }
    else
    {
        return (false);
    }
};

///////////////////////////////////
//
Ektron_Ecommerce_MyAccountClass.DoAction = function(event, parm2)
{
    var result = false; // cancel click for links.
    if (this.objectId && this.action)
    {
        var obj = Ektron_Ecommerce_MyAccountClass.GetObject(this.objectId);
        if (obj)
        {
            var evt = event;
            if (!evt)
            {
                evt = window.event;
            }

            switch (this.action)
            {
                default:
                    obj.TestIt(event, parm2);
                    break;
            }
        }
    }
    return (result);
};

///////////////////////////////////
//
Ektron_Ecommerce_MyAccountClass.LogIt = function(msg)
{
    var textMsg = "Ektron_Ecommerce_MyAccountClass: " + msg;
    if (Ektron_Ecommerce_MyAccountClass.enableShowLog)
    {
        var logObj = $ektron("#MyAccount_log");
        if (!logObj || !logObj.length)
        {
            logObj = $ektron("<div class='MyAccount_log' id='MyAccount_log'><h3>MyAccount Debug Message Log</h3></div>");
            $ektron(".EktronMyAccountCtl").append(logObj);
        }
        logObj.append("<div class='MyAccount_log_msg'>" + textMsg + "</div>");
    }
    else
    {
        if (window.console && window.console.log)
        {
            window.console.log(textMsg);
        }
        else if (window.Debug && window.Debug.writeln)
        {
            window.Debug.writeln(textMsg);
        }
    }
};

///////////////////////////////////
//
Ektron_Ecommerce_MyAccountClass.addLoadEvent = function(func)
{
    var oldonload = window.onload;
    if (typeof window.onload != 'function')
    {
        window.onload = func;
    } else
    {
        window.onload = function()
        {
            if (oldonload)
            {
                oldonload();
            }
            func();
        };
    }
};